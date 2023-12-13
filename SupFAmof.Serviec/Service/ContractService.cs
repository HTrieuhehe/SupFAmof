using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Firebase.Auth;
using Firebase.Storage;
using FirebaseAdmin;
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
using System.Net;
using System.Net.NetworkInformation;
using System.Xml;
using System.Xml.Linq;
using static SupFAmof.Service.Helpers.Enum;
using static SupFAmof.Service.Helpers.ErrorEnum;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace SupFAmof.Service.Service
{
    public class ContractService : IContractService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISendMailService _sendMailService;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public ContractService(IUnitOfWork unitOfWork, IMapper mapper, ISendMailService mailService, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sendMailService = mailService;
            _notificationService = notificationService;
        }

        public async Task<BaseResponseViewModel<AdmissionAccountContractResponse>> CreateAdmissionContract(int accountId, CreateAdmissionContractRequest request)
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
                    throw new ErrorResponse(403, (int)ContractErrorEnum.ACCOUNT_CREATE_CONTRACT_INVALID,
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
                    //báo lỗi thời gian yêu cầu cộng thêm 10 phút
                    var timeRequired = getCurrentTime.AddDays(2).AddHours(0.1);
                    throw new ErrorResponse(400, (int)ContractErrorEnum.SIGNING_DATE_INVALID_WITH_CURRENT_DATE,
                                        ContractErrorEnum.SIGNING_DATE_INVALID_WITH_CURRENT_DATE.GetDisplayName() + $": {timeRequired}");
                }

                else if (request.StartDate < request.SigningDate)
                {
                    throw new ErrorResponse(400, (int)ContractErrorEnum.START_DATE_INVALID_WITH_SIGNING_DATE,
                                       ContractErrorEnum.START_DATE_INVALID_WITH_SIGNING_DATE.GetDisplayName());
                }

                else if (request.EndDate < request.StartDate)
                {
                    throw new ErrorResponse(400, (int)ContractErrorEnum.END_DATE_INVALID_WITH_START_DATE,
                                       ContractErrorEnum.END_DATE_INVALID_WITH_START_DATE.GetDisplayName());
                }

                else if (request.EndDate > request.StartDate.AddDays(30))
                {
                    throw new ErrorResponse(400, (int)ContractErrorEnum.END_DATE_INVALID,
                                       ContractErrorEnum.END_DATE_INVALID.GetDisplayName() + request.StartDate.AddDays(30));
                }

                var contract = _mapper.Map<CreateAdmissionContractRequest, Contract>(request);

                contract.CreatePersonId = accountId;
                contract.IsActive = true;
                //contract.EndDate = contract.StartDate.AddDays(30);
                contract.CreateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<Contract>().InsertAsync(contract);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AdmissionAccountContractResponse>
                {
                    Status = new StatusViewModel
                    {
                        Message = "Success",
                        ErrorCode = 0,
                        Success = true,
                    },
                    Data = _mapper.Map<AdmissionAccountContractResponse>(contract)

                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AdmissionAccountContractResponse>> UpdateAdmissionContract(int accountId, int contractId, UpdateAdmissionContractRequest request)
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

                return new BaseResponseViewModel<AdmissionAccountContractResponse>
                {
                    Status = new StatusViewModel
                    {
                        Message = "Success",
                        ErrorCode = 0,
                        Success = true,
                    },
                    Data = _mapper.Map<AdmissionAccountContractResponse>(contractUpdate)

                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AdmissionAccountContractResponse>> GetAdmissionContractById(int accountId, int contractId)
        {
            try
            {
                var contract = await _unitOfWork.Repository<Contract>().FindAsync(x => x.Id == contractId && x.CreatePersonId == accountId);
                if (contract == null)
                {
                    throw new ErrorResponse(404, (int)ContractErrorEnum.NOT_FOUND_CONTRACT,
                                        ContractErrorEnum.NOT_FOUND_CONTRACT.GetDisplayName());
                }
                return new BaseResponseViewModel<AdmissionAccountContractResponse>
                {
                    Status = new StatusViewModel
                    {
                        Message = "Success",
                        ErrorCode = 0,
                        Success = true,
                    },
                    Data = _mapper.Map<AdmissionAccountContractResponse>(contract)

                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<AdmissionAccountContractResponse>> GetAdmissionContracts(int accountId, AdmissionAccountContractResponse filter, PagingRequest paging)
        {
            try
            {
                var contract = _unitOfWork.Repository<Contract>().GetAll()
                                                .Where(x => x.CreatePersonId == accountId && x.IsActive == true)
                                                .ProjectTo<AdmissionAccountContractResponse>(_mapper.ConfigurationProvider)
                                                .DynamicFilter(filter)
                                                .DynamicSort(paging.Sort, paging.Order)
                                                .PagingQueryable(paging.Page, paging.PageSize);

                return new BaseResponsePagingViewModel<AdmissionAccountContractResponse>
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

        public async Task<BaseResponsePagingViewModel<AdmissionAccountContractResponse>> AdmisionSearchContract(int accountId, string search, PagingRequest paging)
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
                                    .ProjectTo<AdmissionAccountContractResponse>(_mapper.ConfigurationProvider)
                                    .Where(x => x.ContractName.Contains(search) && x.CreatePersonId == accountId)
                                    .PagingQueryable(paging.Page, paging.PageSize,
                                                        Constants.LimitPaging, Constants.DefaultPaging);

                if (!contract.Item2.Any())
                {
                    throw new ErrorResponse(404, (int)ContractErrorEnum.NOT_FOUND_CONTRACT,
                                        ContractErrorEnum.NOT_FOUND_CONTRACT.GetDisplayName());
                }

                return new BaseResponsePagingViewModel<AdmissionAccountContractResponse>()
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

        public async Task<BaseResponseViewModel<AdmissionAccountContractResponse>> DisableAdmissionContract(int accountId, int contractId)
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
                                                              .FirstOrDefaultAsync(x => x.ContractId == contract.Id && x.Status == (int)AccountContractStatusEnum.Confirm);

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

                return new BaseResponseViewModel<AdmissionAccountContractResponse>
                {
                    Status = new StatusViewModel
                    {
                        Message = "Success",
                        ErrorCode = 0,
                        Success = true,
                    },
                    Data = _mapper.Map<AdmissionAccountContractResponse>(contract)

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
                var contract = await _unitOfWork.Repository<Contract>().FindAsync(x => x.Id == contractId && x.CreatePersonId == accountId);

                if (contract == null)
                {
                    throw new ErrorResponse(404, (int)ContractErrorEnum.NOT_FOUND_CONTRACT,
                                        ContractErrorEnum.NOT_FOUND_CONTRACT.GetDisplayName());
                }

                //access to each accountId in list of accountIds
                var listCollab = collaboratorAccountId.ToList();
                foreach (int collab in listCollab)
                {
                    var checkCollab = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == collab);

                    //if account banned or any contract confirmed. status will fail
                    if (checkCollab == null || checkCollab.AccountBanneds.Any() && checkCollab.AccountBanneds.Max(x => x.DayEnd <= Ultils.GetCurrentDatetime()))
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

                        //remove accountId from the list to avoid sending notification to this account
                        collaboratorAccountId.Remove(collab);
                        continue;
                    }

                    //validate if there is any contract in range of this collaborator
                    /*
                      chúng ta so sánh ngày bắt đầu của hợp đồng (StartDate) phải nhỏ hơn hoặc bằng ngày kết thúc của khoảng (endDate), 
                        và ngược lại, ngày kết thúc của hợp đồng (EndDate) phải lớn hơn hoặc bằng ngày bắt đầu của khoảng (startDate)
                     */
                    var checkCurrentContract = _unitOfWork.Repository<AccountContract>()
                                .GetAll().Where(x => x.Contract.StartDate <= contract.EndDate
                                                                && x.Contract.EndDate >= contract.StartDate
                                                                && x.AccountId == collab);

                    if (checkCurrentContract.Any(x => x.Status == (int)AccountContractStatusEnum.Confirm))
                    {
                        //cannot send email. the collaborator has confirmed one contract already
                        //remove accountId from the list to avoid sending notification to this account

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

                        collaboratorAccountId.Remove(collab);
                        continue;
                    }

                    if (checkCurrentContract.Any(x => x.Contract.Id == contractId))
                    {
                        collaboratorAccountId.Remove(collab);
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
                        ContractName = contract.ContractName.ToString(),
                        SigningDate = contract.SigningDate.Date.ToString(),
                        StartDate = contract.StartDate.Date.ToString(),
                        EndDate = contract.EndDate.Date.ToString(),
                        TotalSalary = contract.TotalSalary.ToString(),
                    };

                    //send Email
                    await _sendMailService.SendEmailContract(mailContractRequest);

                    //saving data to context
                    await _unitOfWork.Repository<AccountContract>().InsertAsync(accountContractMapping);
                    //continue;
                }

                //sending Notification
                //create notification request 
                if (collaboratorAccountId.Any())
                {
                    PushNotificationRequest notificationRequest = new PushNotificationRequest()
                    {
                        Ids = collaboratorAccountId,
                        Title = NotificationTypeEnum.Contract_Request.GetDisplayName(),
                        Body = "New contract is sent to you! Check it now!",
                        NotificationsType = (int)NotificationTypeEnum.Contract_Request
                    };

                    await _notificationService.PushNotification(notificationRequest);

                    await _unitOfWork.CommitAsync();

                    return new BaseResponseViewModel<bool>
                    {
                        Status = new StatusViewModel
                        {
                            Message = "Sending Success",
                            ErrorCode = 0,
                            Success = true,
                        },
                        Data = true
                    };
                }

                else
                {
                    //saving data into database before return error
                    await _unitOfWork.CommitAsync();
                    throw new ErrorResponse(400, (int)AccountContractErrorEnum.OVER_COLLABORATOR,
                                       AccountContractErrorEnum.OVER_COLLABORATOR.GetDisplayName());
                }
            }
            catch (Exception ex)
            {
                throw;
            }
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
                                          .Where(x => x.AccountId == accountId && x.Status != (int)AccountContractStatusEnum.Fail)
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

        public async Task<BaseResponsePagingViewModel<AccountContractResponse>> ConfirmContract(int accountId, int accountContractId, int status)
        {
            /*
            
            - confirm contract sẽ bao gồm các bước, yêu cầu validate như sau:
                + Check Account đã có confirm 1 contract nào trong thời gian này chưa
                + Gởi noti cho Admission
                +

             */

            try
            {
                var currentTime = Ultils.GetCurrentDatetime();

                //check Account
                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);

                if (account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                                         AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                //check account banned current or not
                var accountBanned = Ultils.CheckAccountBanned(account.AccountBanneds);

                //if it true
                if (accountBanned)
                {
                    throw new ErrorResponse(403, (int)AccountErrorEnums.BANNED_IN_PROCESS,
                                                         AccountErrorEnums.BANNED_IN_PROCESS.GetDisplayName());
                }

                switch (status)
                {
                    case (int)AccountContractStatusEnum.Confirm:

                        #region Code Here

                        //validate account banking information
                        if (!account.AccountBankings.Where(x => x.AccountId == accountId).Any())
                        {
                            throw new ErrorResponse(400, (int)AccountContractErrorEnum.MISSING_BANKING_INFORMATION,
                                                                 AccountContractErrorEnum.MISSING_BANKING_INFORMATION.GetDisplayName());
                        }

                        //find account Contract information
                        var accountContract = await _unitOfWork.Repository<AccountContract>()
                                                .FindAsync(x => x.Id == accountContractId && x.AccountId == accountId);

                        if (accountContract == null)
                        {
                            throw new ErrorResponse(404, (int)AccountContractErrorEnum.NOT_FOUND_ACCOUNT_CONTRACT,
                                                                 AccountContractErrorEnum.NOT_FOUND_ACCOUNT_CONTRACT.GetDisplayName());
                        }

                        //account contract has been confirm or reject before
                        if (accountContract.Status != (int)AccountContractStatusEnum.Pending)
                        {
                            throw new ErrorResponse(400, (int)AccountContractErrorEnum.CONTRACT_ACCOUNT_ALREADY_UPDATE,
                                                                 AccountContractErrorEnum.CONTRACT_ACCOUNT_ALREADY_UPDATE.GetDisplayName());
                        }

                        //validate Signing time with currentDateTime

                        var signingTimeCheck = accountContract.Contract.SigningDate;

                        //cannot confirm if currentTime pass 5PM of SigningDate
                        if (signingTimeCheck.AddHours(17) <= currentTime)
                        {
                            //reject that contract request
                            accountContract.Status = (int)AccountContractStatusEnum.Reject;
                            await _unitOfWork.Repository<AccountContract>().UpdateDetached(accountContract);
                            await _unitOfWork.CommitAsync();
                            throw new ErrorResponse(400, (int)AccountContractErrorEnum.CONFIRM_INVALID,
                                                                 AccountContractErrorEnum.CONFIRM_INVALID.GetDisplayName());
                        }

                        // validate if there is any contract in range of this collaborator
                        var checkCurrentContract = await _unitOfWork.Repository<AccountContract>()
                                                                    .GetAll()
                                                                    .FirstOrDefaultAsync(x => x.Status == (int)AccountContractStatusEnum.Confirm &&
                                                                                              x.Contract.EndDate >= accountContract.Contract.StartDate &&
                                                                                              x.AccountId == accountId);

                        if (checkCurrentContract != null)
                        {
                            //reject all contract have overlap time

                            var overlapContract = _unitOfWork.Repository<AccountContract>()
                                                             .GetAll()
                                                             .Where(x => x.Status == (int)AccountContractStatusEnum.Pending &&
                                                                                              x.Contract.EndDate >= checkCurrentContract.Contract.StartDate &&
                                                                                              x.AccountId == accountId);
                            foreach (var item in overlapContract)
                            {
                                item.Status = (int)AccountContractStatusEnum.Reject;

                                await _unitOfWork.Repository<AccountContract>().UpdateDetached(item);
                            }

                            await _unitOfWork.CommitAsync();

                            throw new ErrorResponse(400, (int)AccountContractErrorEnum.CONTRACT_ALREADY_CONFIRM,
                                                                 AccountContractErrorEnum.CONTRACT_ALREADY_CONFIRM.GetDisplayName());
                        }

                        var uploadedFile = await FillingDocument(account, accountContract.Contract);

                        if (string.IsNullOrEmpty(uploadedFile))
                        {
                            throw new ErrorResponse(400, (int)AccountContractErrorEnum.UPLOADED_FAILED,
                                                                 AccountContractErrorEnum.UPLOADED_FAILED.GetDisplayName());
                        }

                        accountContract.SubmittedFile = uploadedFile.Trim();
                        accountContract.Status = (int)AccountContractStatusEnum.Confirm;
                        accountContract.UpdateAt = Ultils.GetCurrentDatetime();

                        await _unitOfWork.Repository<AccountContract>().UpdateDetached(accountContract);
                        await _unitOfWork.CommitAsync();

                        var overlapContracts = _unitOfWork.Repository<AccountContract>()
                                                             .GetAll()
                                                             .Where(x => x.Status == (int)AccountContractStatusEnum.Pending &&
                                                                                              x.Contract.EndDate >= accountContract.Contract.StartDate &&
                                                                                              x.AccountId == accountId);
                        foreach (var item in overlapContracts)
                        {
                            item.Status = (int)AccountContractStatusEnum.Reject;

                            await _unitOfWork.Repository<AccountContract>().UpdateDetached(item);
                        }

                        List<int> collaboratorIds = new List<int>();
                        collaboratorIds.Add(accountId);

                        //create notification request 
                        PushNotificationRequest notificationRequest = new PushNotificationRequest()
                        {
                            Ids = collaboratorIds,
                            Title = NotificationTypeEnum.Contract_Request.GetDisplayName(),
                            Body = "Congratulation! You are confirmed your contract!",
                            NotificationsType = (int)NotificationTypeEnum.Contract_Request
                        };

                        await _notificationService.PushNotification(notificationRequest);
                        await _unitOfWork.CommitAsync();

                        var accountContractResponse = await _unitOfWork.Repository<AccountContract>()
                                                                 .GetAll()
                                                                 .Where(x => x.AccountId == accountId)
                                                                 .ProjectTo<AccountContractResponse>(_mapper.ConfigurationProvider)
                                                                 .OrderBy(x => x.Status)
                                                                 .ToListAsync();

                        return new BaseResponsePagingViewModel<AccountContractResponse>
                        {
                            Data = accountContractResponse,
                        };

                    #endregion

                    case (int)AccountContractStatusEnum.Reject:

                        #region Code here

                        //find account Contract information
                        var accountContractCheck = await _unitOfWork.Repository<AccountContract>()
                                                .FindAsync(x => x.Id == accountContractId && x.AccountId == accountId);

                        if (accountContractCheck == null)
                        {
                            throw new ErrorResponse(404, (int)AccountContractErrorEnum.NOT_FOUND_ACCOUNT_CONTRACT,
                                                                 AccountContractErrorEnum.NOT_FOUND_ACCOUNT_CONTRACT.GetDisplayName());
                        }

                        //account contract has been confirm or reject before
                        if (accountContractCheck.Status != (int)AccountContractStatusEnum.Pending)
                        {
                            throw new ErrorResponse(404, (int)AccountContractErrorEnum.CONTRACT_ACCOUNT_ALREADY_UPDATE,
                                                                 AccountContractErrorEnum.CONTRACT_ACCOUNT_ALREADY_UPDATE.GetDisplayName());
                        }

                        accountContractCheck.Status = (int)AccountContractStatusEnum.Reject;
                        accountContractCheck.UpdateAt = Ultils.GetCurrentDatetime();

                        await _unitOfWork.Repository<AccountContract>().UpdateDetached(accountContractCheck);
                        await _unitOfWork.CommitAsync();

                        var accountContractResponses = await _unitOfWork.Repository<AccountContract>()
                                                                 .GetAll()
                                                                 .Where(x => x.AccountId == accountId)
                                                                 .ProjectTo<AccountContractResponse>(_mapper.ConfigurationProvider)
                                                                 .ToListAsync();

                        return new BaseResponsePagingViewModel<AccountContractResponse>
                        {
                            Data = accountContractResponses,
                        };
                    #endregion

                    default:

                        throw new ErrorResponse(400, (int)AccountContractErrorEnum.CONTRACT_REMOVED_ADMISSION,
                                                          AccountContractErrorEnum.CONTRACT_REMOVED_ADMISSION.GetDisplayName());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AdmissionAccountContractResponse>> AdmissionCompleteContract(int accountId, int accountContractId)
        {
            try
            {
                var accountContract = await _unitOfWork.Repository<AccountContract>().FindAsync(x => x.Id == accountContractId && x.Contract.CreatePersonId == accountId);

                if (accountContract == null)
                {
                    throw new ErrorResponse(404, (int)AccountContractErrorEnum.NOT_FOUND_ACCOUNT_CONTRACT,
                                        AccountContractErrorEnum.NOT_FOUND_ACCOUNT_CONTRACT.GetDisplayName());
                }

                if (accountContract.Status != (int)AccountContractStatusEnum.Confirm)
                {
                    throw new ErrorResponse(400, (int)AccountContractErrorEnum.COMPLETE_INVALID,
                                        AccountContractErrorEnum.COMPLETE_INVALID.GetDisplayName());
                }

                accountContract.Status = (int)AccountContractStatusEnum.Complete;
                accountContract.UpdateAt = Ultils.GetCurrentDatetime();

                return new BaseResponseViewModel<AdmissionAccountContractResponse>
                {
                    Status = new StatusViewModel
                    {
                        Message = "Success",
                        ErrorCode = 0,
                        Success = true,
                    },
                    Data = _mapper.Map<AdmissionAccountContractResponse>(accountContract)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<string> FillingDocument(Account account, Contract contract)
        {
            try
            {
                string fileName = $"Hợp đồng khoán gọn - {account.Name}.docx"; // Replace with the actual document ID or name

                //calculate vat tax
                double salaryAfterVAT = 0.0;

                if (contract.TotalSalary >= 2000000)
                {
                    salaryAfterVAT = contract.TotalSalary * (1 - 0.1);
                }

                var salaryAfterVATInWord = Ultils.NumberToText(salaryAfterVAT);
                var salaryInWord = Ultils.NumberToText(contract.TotalSalary);

                //download
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(contract.SampleFile, fileName);
                }

                // Mở file DOCX để chỉnh sửa
                using (WordprocessingDocument doc = WordprocessingDocument.Open(fileName, true))
                {
                    // 2. Lấy main document part
                    MainDocumentPart mainPart = doc.MainDocumentPart;

                    // 3. Tạo mới một Document rỗng
                    Document newDoc = new Document();

                    // 4. Copy toàn bộ nội dung hiện tại sang newDoc
                    newDoc.Append(mainPart.Document);

                    // Duyệt qua các paragraph
                    foreach (Paragraph para in doc.MainDocumentPart.Document.Descendants<Paragraph>())
                    {
                        // Thay thế từng biến trong các Run của Paragraph
                        foreach (Run run in para.Elements<Run>())
                        {
                            foreach (Text textElement in run.Elements<Text>())
                            {
                                // Clone node before modifying
                                var newText = (Text)textElement.CloneNode(true);

                                newText.Text = newText.Text.Replace("{CurrentDate}", contract.SigningDate.Day.ToString())
                                   .Replace("{CurrentMonth}", contract.SigningDate.Month.ToString())
                                   .Replace("{CurrentYear}", contract.SigningDate.Year.ToString())
                                   .Replace("{Name}", account.Name.ToUpper().ToString())
                                   .Replace("{Address}", account.AccountInformation.Address.ToString())
                                   .Replace("{PhoneNumber}", account.Phone.ToString())
                                   .Replace("{IdentityNumber}", account.AccountInformation.IdentityNumber.ToString())
                                   .Replace("{IdentityIssueDate}", account.AccountInformation.IdentityIssueDate.ToString())
                                   .Replace("{Email}", account.Email.ToString())
                                   .Replace("{IdentityIssuePlace}", account.AccountInformation.PlaceOfIssue.ToString())
                                   .Replace("{TaxNumber}", account.AccountInformation.TaxNumber.ToString())
                                   .Replace("{BankNumber}", account.Name.ToString())
                                   .Replace("{BankName}", account.Name.ToString())
                                   .Replace("{Branch}", account.Name.ToString())
                                   .Replace("{ContractDescription}", account.Name.ToString())
                                   .Replace("{StartDate}", contract.StartDate.ToString())
                                   .Replace("{EndDate}", contract.EndDate.ToString())
                                   .Replace("{SalaryInWord}", salaryInWord)
                                   .Replace("{SigningDate}", contract.SigningDate.Day.ToString())
                                   .Replace("{SigningMonth}", contract.SigningDate.Month.ToString())
                                   .Replace("{SigningYear}", contract.SigningDate.Year.ToString())
                                   .Replace("{SalaryAfterVAT}", salaryAfterVAT.ToString())
                                   .Replace("{SalaryAfterVATInWord}", salaryAfterVATInWord)
                                   .Replace("{TotalSalary}", salaryInWord);

                                // Replace the original text node with the cloned and modified one
                                run.ReplaceChild(newText, textElement);
                            }
                        }
                    }

                    // Lưu lại nội dung đã thay đổi
                    mainPart.Document = newDoc;
                    doc.Save();
                }

                string uploadURL = "";

                // 3. Upload lên Firebase  
                using (FileStream stream = File.Open(fileName, FileMode.Open))
                {
                    uploadURL = await UploadDocxFile(stream, fileName);
                }

                // Xóa file ban đầu
                File.Delete(fileName);

                return uploadURL;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region Upload File

        private static readonly string ApiKey = "AIzaSyAVNIh-RA2rgMZh3zGvQsO2DIepWfVIGJ8";
        private static readonly string Bucket = "supfamof-c8c84.appspot.com";
        private static readonly string AuthEmail = "asdf@gmail.com";
        private static readonly string AuthPassword = "asdf123";

        public async Task<string> UploadDocxFile(Stream fileStream, string fileName)
        {
            try
            {
                // https://github.com/step-up-labs/firebase-storage-dotnet/blob/master/samples/SimpleConsole/SimpleConsole/Program.cs

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Referrer = new Uri("https://dev.supfamof.id.vn/");

                FirebaseAuthProvider auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));

                var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

                var cancellationToken = new CancellationTokenSource();

                var uploadTask = new FirebaseStorage(
                    Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child("contract")
                    .Child(fileName)  // Append ".docx" to the file name
                    //.PutAsync(fileStream, cancellationToken.Token, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                    .PutAsync(fileStream, cancellationToken.Token);


                uploadTask.Progress.ProgressChanged += (s, e) =>
                {
                    Console.WriteLine($"Upload Progress: {e.Percentage} %");
                };

                return await uploadTask;
            }
            catch (FirebaseAuthException ex)
            {
                Console.WriteLine($"Firebase Authentication Error Message: {ex.Message}");
                // Xử lý mã lỗi theo yêu cầu
                throw;
            }

        }

        #endregion


        #region Unsed Code in Confirm Contract

        /*
            //filling data in 

            //var contract = await _unitOfWork.Repository<Contract>().FindAsync(x => x.Id == accountContract);

            var fileDocByte = await FillingDocTransfer(account, accountContract.Contract);

            accountContract.SubmittedBinaryFile = fileDocByte;
         */

        #endregion

        #endregion
    }
}
