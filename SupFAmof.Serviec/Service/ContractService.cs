﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
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
using System.Xml;
using System.Xml.Linq;
using static SupFAmof.Service.Helpers.Enum;
using static SupFAmof.Service.Helpers.ErrorEnum;

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

                //access to each accountId in list of accountIds
                foreach (int collab in collaboratorAccountId)
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
                    var checkCurrentContract = await _unitOfWork.Repository<AccountContract>()
                                .FindAsync(x => x.Status == (int)AccountContractStatusEnum.Confirm && x.Contract.StartDate <= Ultils.GetCurrentDatetime() 
                                                                                               && x.Contract.EndDate >= Ultils.GetCurrentDatetime()
                                                                                               && x.AccountId == collab);

                    if (checkCurrentContract != null)
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
                        SigningDate = checkContract.SigningDate.Date.ToString(),
                        StartDate = checkContract.StartDate.Date.ToString(),
                        EndDate = checkContract.EndDate.Date.ToString(),
                        TotalSalary = checkContract.TotalSalary.ToString(),
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
            catch(Exception ex)
            {
                throw;
            }
        }
    
        private async Task<byte[]> FillingDocTransfer(Account account, Contract contract)
        {
            //khởi tạo các biến liên quan
            string fileName = "contract.docx";
            var salaryInWord = Ultils.NumberToText(contract.TotalSalary);

            //calculate vat tax
            var salaryAfterVAT = contract.TotalSalary * (1 - 0.1);
            var salaryAfterVATInWord = Ultils.NumberToText(salaryAfterVAT);


            using (WebClient client = new WebClient())
            {
                client.DownloadFile(contract.SampleFile, fileName);
            }

            // Mở file DOCX để chỉnh sửa
            using (WordprocessingDocument doc = WordprocessingDocument.Open(fileName, true))
            {
                // Lấy nội dung tài liệu
                MainDocumentPart mainPart = doc.MainDocumentPart;
                Document docContent = mainPart.Document;

                // Duyệt qua các paragraph
                foreach (Paragraph para in doc.MainDocumentPart.Document.Descendants<Paragraph>())
                {
                    // Thay thế từng biến
                    string text = para.InnerText;
                    text = text.Replace("{CurrentDate}", contract.SigningDate.Day.ToString())
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
                               .Replace("{TotalSalary}", salaryInWord)
                               ;

                    // Gán nội dung mới 
                    para.RemoveAllChildren();

                    DocumentFormat.OpenXml.Wordprocessing.Text newText = new DocumentFormat.OpenXml.Wordprocessing.Text(text);
                    para.AppendChild(newText);
                }

                // Lưu lại nội dung đã thay đổi
                mainPart.Document = docContent;
                doc.Save();
            }

            // Chuyển đổi sang Base64
            byte[] fileBytes = File.ReadAllBytes(fileName);

            // Xóa file ban đầu
            File.Delete(fileName);

            return fileBytes;
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

        public async Task<BaseResponseViewModel<AccountContractResponse>> ConfirmContract(int accountId, int contractId)
        {
            /*
            
            - confirm contract sẽ bao gồm các bước, yêu cầu validate như sau:
                + Check Account đã có confirm 1 contract nào trong thời gian này chưa
                + Gởi noti cho Admission
                +

             */

            try
            {
                //check Account
                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);

                if (account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                                         AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                //check account banned current or not
                if (account.AccountBanneds.Any() && account.AccountBanneds.Max(x => x.DayEnd <= Ultils.GetCurrentDatetime()))
                {
                    throw new ErrorResponse(403, (int)AccountErrorEnums.BANNED_IN_PROCESS,
                                                         AccountErrorEnums.BANNED_IN_PROCESS.GetDisplayName());
                }

                // validate if there is any contract in range of this collaborator
                var checkCurrentContract = _unitOfWork.Repository<AccountContract>()
                            .GetAll()
                            .Where(x => x.Status == (int)AccountContractStatusEnum.Confirm && x.Contract.StartDate <= Ultils.GetCurrentDatetime()
                                                                                            && x.Contract.EndDate >= Ultils.GetCurrentDatetime()
                                                                                            && x.AccountId == accountId);
                if (checkCurrentContract != null)
                {
                    throw new ErrorResponse(400, (int)AccountContractErrorEnum.CONTRACT_ALREADY_CONFIRM,
                                                         AccountContractErrorEnum.CONTRACT_ALREADY_CONFIRM.GetDisplayName());
                }

                var accountContract = await _unitOfWork.Repository<AccountContract>()
                                                .FindAsync(x => x.Id == contractId && x.Status == (int)AccountContractStatusEnum.Pending);

                if (accountContract == null)
                {
                    throw new ErrorResponse(400, (int)AccountContractErrorEnum.CONTRACT_REMOVED_ADMISSION,
                                                         AccountContractErrorEnum.CONTRACT_REMOVED_ADMISSION.GetDisplayName());
                }

                //filling data in 

                var contract = await _unitOfWork.Repository<Contract>().FindAsync(x => x.Id == contractId);

                var fileDocByte = await FillingDocTransfer(account, contract);

                accountContract.SubmittedFile = fileDocByte;
                accountContract.UpdateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<AccountContract>().UpdateDetached(accountContract);

                List<int> admissionIds = new List<int>();
                admissionIds.Add(accountContract.Contract.CreatePersonId);

                //create notification request 
                PushNotificationRequest notificationRequest = new PushNotificationRequest()
                {
                    Ids = admissionIds,
                    Title = NotificationTypeEnum.Contract_Request.GetDisplayName(),
                    Body = "New contract is sent to you! Check it now!",
                    NotificationsType = (int)NotificationTypeEnum.Contract_Request
                };

                await _notificationService.PushNotification(notificationRequest);
                await _unitOfWork.CommitAsync();

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
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
