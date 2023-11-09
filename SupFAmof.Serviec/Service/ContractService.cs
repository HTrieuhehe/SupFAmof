using AutoMapper;
using AutoMapper.QueryableExtensions;
using LAK.Sdk.Core.Utilities;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Service.Commons;
using ServiceStack.Web;
using SupFAmof.Data.Entity;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Service.ServiceInterface;
using SupFAmof.Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static SupFAmof.Service.Helpers.Enum;
using static SupFAmof.Service.Helpers.ErrorEnum;

namespace SupFAmof.Service.Service
{
    public class ContractService : IContractService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISendMailService _sendMailService;
        private readonly IMapper _mapper;

        public ContractService(IUnitOfWork unitOfWork, IMapper mapper, ISendMailService mailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sendMailService = mailService;
        }

        public async Task<BaseResponseViewModel<AdmissionContractResponse>> CreateAdmissionContract(int accountId, CreateAdmissionContractRequest request)
        {
            try
            {
                DateTime getCurrentTime = Ultils.GetCurrentDatetime();
                var checkAccount = await _unitOfWork.Repository<Account>().GetAll().FirstOrDefaultAsync(x => x.Id == accountId);

                if (checkAccount == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                else if (checkAccount.PostPermission == false)
                {
                    throw new ErrorResponse(400, (int)ContractErrorEnum.ACCOUNT_CREATE_CONTRACT_INVALID,
                                        ContractErrorEnum.ACCOUNT_CREATE_CONTRACT_INVALID.GetDisplayName());
                }

                //vaidate date
                //signing date cannot be greater than starting date and at least 2 days greater than current day
                //start date cannot be less than signing date

                if (request.SigningDate > request.StartDate)
                {
                    throw new ErrorResponse(400, (int)ContractErrorEnum.SIGNING_DATE_INVALID_WITH_START_DATE,
                                        ContractErrorEnum.SIGNING_DATE_INVALID_WITH_START_DATE.GetDisplayName());
                }

                else if (request.SigningDate < getCurrentTime.AddDays(2))
                {
                    throw new ErrorResponse(400, (int)ContractErrorEnum.SIGNING_DATE_INVALID_WITH_CURRENT_DATE,
                                        ContractErrorEnum.SIGNING_DATE_INVALID_WITH_CURRENT_DATE.GetDisplayName());
                }

                else if(request.StartDate < request.SigningDate)
                {
                    throw new ErrorResponse(400, (int)ContractErrorEnum.START_DATE_INVALID_WITH_SIGNING_DATE,
                                       ContractErrorEnum.START_DATE_INVALID_WITH_SIGNING_DATE.GetDisplayName());
                }

                else if (request.EndDate < request.StartDate)
                {
                    throw new ErrorResponse(400, (int)ContractErrorEnum.END_DATE_INVALID_WITH_START_DATE,
                                       ContractErrorEnum.END_DATE_INVALID_WITH_START_DATE.GetDisplayName());
                }

                else if (request.EndDate < request.StartDate.AddDays(30) || request.EndDate > request.StartDate.AddDays(30) )
                {
                    throw new ErrorResponse(400, (int)ContractErrorEnum.END_DATE_INVALID,
                                       ContractErrorEnum.END_DATE_INVALID.GetDisplayName() + request.StartDate.AddDays(30));
                }

                var contract = _mapper.Map<CreateAdmissionContractRequest, Contract>(request);

                contract.CreatePersonId = accountId;
                contract.IsActive = true;
                contract.CreateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<Contract>().InsertAsync(contract);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AdmissionContractResponse>
                {
                    Status = new StatusViewModel
                    {
                        Message = "Success",
                        ErrorCode = 0,
                        Success = true,
                    },
                    Data = _mapper.Map<AdmissionContractResponse>(contract)

                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AdmissionContractResponse>> UpdateAdmissionContract(int accountId, int contractId, UpdateAdmissionContractRequest request)
        {
            //allow update if there is no sending email to someone

            try
            {
                DateTime getCurrentTime = Ultils.GetCurrentDatetime();
                var checkAccount = await _unitOfWork.Repository<Account>().GetAll().FirstOrDefaultAsync(x => x.Id == accountId);

                if (checkAccount == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                else if (checkAccount.PostPermission == false)
                {
                    throw new ErrorResponse(400, (int)ContractErrorEnum.ACCOUNT_CREATE_CONTRACT_INVALID,
                                        ContractErrorEnum.ACCOUNT_CREATE_CONTRACT_INVALID.GetDisplayName());
                }

                var contract = await _unitOfWork.Repository<Contract>().FindAsync(x => x.Id == contractId);

                if (contract == null)
                {
                    throw new ErrorResponse(404, (int)ContractErrorEnum.NOT_FOUND_CONTRACT,
                                        ContractErrorEnum.NOT_FOUND_CONTRACT.GetDisplayName());
                }

                var checkSendEmailContract = await _unitOfWork.Repository<AccountContract>()
                                                              .GetAll()
                                                              .FirstOrDefaultAsync(x => x.ContractId == contract.Id || x.Status == (int)AccountContractStatusEnum.Confirm);

                if (checkSendEmailContract != null)
                {
                    throw new ErrorResponse(400, (int)ContractErrorEnum.UPDATE_CONTRACT_INVALID,
                                        ContractErrorEnum.UPDATE_CONTRACT_INVALID.GetDisplayName());
                }

                var contractUpdate = _mapper.Map<UpdateAdmissionContractRequest, Contract>(request, contract);

                //validate date

                if (contractUpdate.SigningDate > contractUpdate.StartDate)
                {
                    throw new ErrorResponse(400, (int)ContractErrorEnum.SIGNING_DATE_INVALID_WITH_START_DATE,
                                        ContractErrorEnum.SIGNING_DATE_INVALID_WITH_START_DATE.GetDisplayName());
                }

                //at least signing date must greater than 1 day
                else if (contractUpdate.SigningDate < getCurrentTime.AddDays(1))
                {
                    throw new ErrorResponse(400, (int)ContractErrorEnum.SIGNING_DATE_INVALID_WITH_CURRENT_DATE,
                                        ContractErrorEnum.SIGNING_DATE_INVALID_WITH_CURRENT_DATE.GetDisplayName());
                }

                else if (contractUpdate.StartDate < contractUpdate.SigningDate)
                {
                    throw new ErrorResponse(400, (int)ContractErrorEnum.START_DATE_INVALID_WITH_SIGNING_DATE,
                                       ContractErrorEnum.START_DATE_INVALID_WITH_SIGNING_DATE.GetDisplayName());
                }

                contract.UpdateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<Contract>().UpdateDetached(contractUpdate);
                await _unitOfWork.CommitAsync();

                //send update notification

                return new BaseResponseViewModel<AdmissionContractResponse>
                {
                    Status = new StatusViewModel
                    {
                        Message = "Success",
                        ErrorCode = 0,
                        Success = true,
                    },
                    Data = _mapper.Map<AdmissionContractResponse>(contractUpdate)

                };
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AdmissionContractResponse>> GetAdmissionContractById(int accountId, int contractId)
        {
            try
            {
                var contract = await _unitOfWork.Repository<Contract>().FindAsync(x => x.Id == contractId && x.CreatePersonId == accountId);
                if (contract == null)
                {
                    throw new ErrorResponse(404, (int)ContractErrorEnum.NOT_FOUND_CONTRACT,
                                        ContractErrorEnum.NOT_FOUND_CONTRACT.GetDisplayName());
                }
                return new BaseResponseViewModel<AdmissionContractResponse>
                {
                    Status = new StatusViewModel
                    {
                        Message = "Success",
                        ErrorCode = 0,
                        Success = true,
                    },
                    Data = _mapper.Map<AdmissionContractResponse>(contract)

                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<AdmissionContractResponse>> GetAdmissionContracts(int accountId, AdmissionContractResponse filter, PagingRequest paging)
        {
            try
            {
                var contract = _unitOfWork.Repository<Contract>().GetAll()
                                                .Where(x => x.CreatePersonId == accountId && x.IsActive == true)
                                                .ProjectTo<AdmissionContractResponse>(_mapper.ConfigurationProvider)
                                                .DynamicFilter(filter)
                                                .DynamicSort(paging.Sort, paging.Order)
                                                .PagingQueryable(paging.Page, paging.PageSize);

                return new BaseResponsePagingViewModel<AdmissionContractResponse>
                {
                    Metadata = new PagingsMetadata
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = contract.Item1
                    },
                    Data = contract.Item2.ToList(),
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
        public async Task<BaseResponsePagingViewModel<AdmissionContractResponse>> AdmisionSearchContract(int accountId, string search, PagingRequest paging)
        {
            //Search by Name
            try
            {

                if (search == null || search.Length == 0)
                {
                    throw new ErrorResponse(404, (int)ContractErrorEnum.NOT_FOUND_CONTRACT,
                                        ContractErrorEnum.NOT_FOUND_CONTRACT.GetDisplayName());
                }

                var contract = _unitOfWork.Repository<Contract>().GetAll()
                                    .ProjectTo<AdmissionContractResponse>(_mapper.ConfigurationProvider)
                                    .Where(x => x.ContractName.Contains(search) && x.CreatePersonId == accountId)
                                    .PagingQueryable(paging.Page, paging.PageSize,
                                                        Constants.LimitPaging, Constants.DefaultPaging);

                if (!contract.Item2.Any())
                {
                    throw new ErrorResponse(404, (int)ContractErrorEnum.NOT_FOUND_CONTRACT,
                                        ContractErrorEnum.NOT_FOUND_CONTRACT.GetDisplayName());
                }

                return new BaseResponsePagingViewModel<AdmissionContractResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = contract.Item1
                    },
                    Data = contract.Item2.ToList()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AdmissionContractResponse>> DisableAdmissionContract(int accountId, int contractId)
        {
            try
            {
                var checkAccount = await _unitOfWork.Repository<Account>().GetAll().FirstOrDefaultAsync(x => x.Id == accountId);

                if (checkAccount == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                else if (checkAccount.PostPermission == false)
                {
                    throw new ErrorResponse(400, (int)ContractErrorEnum.ACCOUNT_CREATE_CONTRACT_INVALID,
                                        ContractErrorEnum.ACCOUNT_CREATE_CONTRACT_INVALID.GetDisplayName());
                }

                var contract = await _unitOfWork.Repository<Contract>().FindAsync(x => x.Id == contractId);

                if (contract == null)
                {
                    throw new ErrorResponse(404, (int)ContractErrorEnum.NOT_FOUND_CONTRACT,
                                        ContractErrorEnum.NOT_FOUND_CONTRACT.GetDisplayName());
                }

                var checkSendEmailContract = await _unitOfWork.Repository<AccountContract>()
                                                              .GetAll()
                                                              .FirstOrDefaultAsync(x => x.ContractId == contract.Id || x.Status == (int)AccountContractStatusEnum.Confirm);

                if (checkSendEmailContract != null)
                {
                    throw new ErrorResponse(400, (int)ContractErrorEnum.DISABLE_CONTRACT_INVALID,
                                        ContractErrorEnum.DISABLE_CONTRACT_INVALID.GetDisplayName());
                }

                contract.IsActive = false;
                contract.UpdateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<Contract>().UpdateDetached(contract);
                await _unitOfWork.CommitAsync();

                //send disable notification

                return new BaseResponseViewModel<AdmissionContractResponse>
                {
                    Status = new StatusViewModel
                    {
                        Message = "Success",
                        ErrorCode = 0,
                        Success = true,
                    },
                    Data = _mapper.Map<AdmissionContractResponse>(contract)

                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<bool>> AdmissionSendContractEmail(int accountId, int contractId, List<int> collaboratorAccountId)
        {
            try
            {
                var checkContract = await _unitOfWork.Repository<Contract>().FindAsync(x => x.Id == contractId && x.CreatePersonId == accountId);

                if (checkContract == null)
                {
                    throw new ErrorResponse(404, (int)ContractErrorEnum.NOT_FOUND_CONTRACT,
                                        ContractErrorEnum.NOT_FOUND_CONTRACT.GetDisplayName());
                }

                foreach (int collab in collaboratorAccountId)
                {
                    var checkCollab = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == collab);

                    //if account banned or any contract confirmed. status will fail
                    if (checkCollab == null || checkCollab.AccountBanneds.Max(x => x.DayEnd <= Ultils.GetCurrentDatetime()))
                    {
                        //account is banned

                        CreateAccountContractRequest accountContractNew = new CreateAccountContractRequest()
                        {
                            ContractId = contractId,
                            AccountId = collab,
                            SubmittedFile = null,
                        };

                        var mapAccountContract = _mapper.Map<AccountContract>(accountContractNew);

                        mapAccountContract.Status = (int)AccountContractStatusEnum.Fail;
                        mapAccountContract.CreateAt = Ultils.GetCurrentDatetime();

                        await _unitOfWork.Repository<AccountContract>().InsertAsync(mapAccountContract);
                        continue;
                    }

                    var checkCurrentContract = _unitOfWork.Repository<AccountContract>()
                                .GetAll()
                                .Where(x => x.Status == (int)AccountContractStatusEnum.Fail && x.Contract.StartDate <= Ultils.GetCurrentDatetime() && x.Contract.EndDate >= Ultils.GetCurrentDatetime());

                    if (checkCurrentContract == null)
                    {
                        //cannot send email. the collaborator has confirmed one contract already
                        continue;
                    }

                    //account allow to send email

                    CreateAccountContractRequest accountContract = new CreateAccountContractRequest()
                    {
                        ContractId = contractId,
                        AccountId = collab,
                        SubmittedFile = null,
                    };

                    var accountContractMapping = _mapper.Map<AccountContract>(accountContract);

                    accountContractMapping.Status = (int)AccountContractStatusEnum.Pending;
                    accountContractMapping.CreateAt = Ultils.GetCurrentDatetime();

                    var mailContractRequest = new MailContractRequest()
                    {
                        Id = contractId.ToString(),
                        Email = checkCollab.Email.ToString(),
                        ContractName = checkContract.ContractName.ToString(),
                        SigningDate = checkContract.SigningDate.ToString(),
                        StartDate = checkContract.StartDate.ToString(),
                        TotalSalary = checkContract.TotalSalary.ToString(),
                    };

                    //send Email
                    await _sendMailService.SendEmailContract(mailContractRequest);

                    //sending Notification
                    //TODO...

                    await _unitOfWork.Repository<AccountContract>().InsertAsync(accountContractMapping);
                }

                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<bool>
                {
                    Status = new StatusViewModel
                    {
                        Message = "Saving success",
                        ErrorCode = 0,
                        Success = true,
                    },
                    Data = true
                };
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    
        private byte[] FillingDocTransfer(byte[] docByte)
        {
            byte[] something = null;

            return something;
        }

        #region Collab Contract
        
        public async Task<BaseResponsePagingViewModel<AccountContractResponse>> GetContracts(AccountContractResponse filter, PagingRequest paging)
        {
            try
            {
                var contract = _unitOfWork.Repository<AccountContract>().GetAll()
                                                .ProjectTo<AccountContractResponse>(_mapper.ConfigurationProvider)
                                                .Where(x => x.Status == (int)AccountContractStatusEnum.Pending)
                                                .DynamicFilter(filter)
                                                .DynamicSort(paging.Sort, paging.Order)
                                                .PagingQueryable(paging.Page, paging.PageSize);

                return new BaseResponsePagingViewModel<AccountContractResponse>
                {
                    Metadata = new PagingsMetadata
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = contract.Item1
                    },
                    Data = contract.Item2.ToList(),
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AccountContractResponse>> GetContractById(int contractId)
        {
            try
            {
                var contract = await _unitOfWork.Repository<AccountContract>().FindAsync(x => x.Id == contractId);
                if (contract == null)
                {
                    throw new ErrorResponse(404, (int)ContractErrorEnum.NOT_FOUND_CONTRACT,
                                        ContractErrorEnum.NOT_FOUND_CONTRACT.GetDisplayName());
                }
                return new BaseResponseViewModel<AccountContractResponse>
                {
                    Status = new StatusViewModel
                    {
                        Message = "Success",
                        ErrorCode = 0,
                        Success = true,
                    },
                    Data = _mapper.Map<AccountContractResponse>(contract)

                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<AccountContractResponse>> GetContractsByAccountId(int accountId, PagingRequest paging)
        {
            try
            {
                var contracts = _unitOfWork.Repository<AccountContract>()
                                          .GetAll()
                                          .ProjectTo<AccountContractResponse>(_mapper.ConfigurationProvider)
                                          .Where(x => x.AccountId == accountId)
                                          .DynamicSort(paging.Sort, paging.Order)
                                          .PagingQueryable(paging.Page, paging.PageSize);

                if (contracts.Item2 == null)
                {
                    throw new ErrorResponse(404, (int)ContractErrorEnum.NOT_FOUND_CONTRACT,
                                        ContractErrorEnum.NOT_FOUND_CONTRACT.GetDisplayName());
                }

                return new BaseResponsePagingViewModel<AccountContractResponse>
                {
                    Metadata = new PagingsMetadata
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = contracts.Item1
                    },
                    Data = contracts.Item2.ToList(),
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<BaseResponseViewModel<AccountContractResponse>> ConfirmContract(int accountId)
        {
            /*
            - check lại send Email contract phải validate coi collab có confirm cái nào chưa mới cho gởi
            
            - confirm contract sẽ bao gồm các bước, yêu cầu validate như sau:
                + 
             */

            throw new NotImplementedException();
        }

        #endregion
    }
}
