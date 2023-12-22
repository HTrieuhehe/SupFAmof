using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.Office2016.Excel;
using LAK.Sdk.Core.Utilities;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Tls;
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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SupFAmof.Service.Helpers.Enum;
using static SupFAmof.Service.Helpers.ErrorEnum;

namespace SupFAmof.Service.Service
{
    public class AccountCertificateService : IAccountCertificateService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public AccountCertificateService(IMapper mapper, IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<BaseResponseViewModel<AccountCertificateResponse>> CreateAccountCertificate(int certificateIssuerId, CreateAccountCertificateRequest request)
        {
            // Mỗi collab account có 1 và chỉ 1 certi/loại. Có thể có certi A, B, C
            // nhưng không được có > 1 certi cùng loại 

            try
            {
                //check account post Permission
                var checkAccount = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == certificateIssuerId);

                if (checkAccount == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                else if (checkAccount.PostPermission == false)
                {
                    throw new ErrorResponse(403, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                //check accountId is collaborator or not
                var checkCollaboratorAccount = await _unitOfWork.Repository<Account>()
                                                    .FindAsync(x => x.Id == request.AccountId && x.RoleId == (int)SystemRoleEnum.Collaborator);

                if (checkCollaboratorAccount == null)
                {
                    throw new ErrorResponse(404, (int)AccountCertificateErrorEnum.ACCOUNT_COLLABORATOR_INVALID,
                                         AccountCertificateErrorEnum.ACCOUNT_COLLABORATOR_INVALID.GetDisplayName());
                }

                //check data is null or empty
                if (!request.TrainingCertificates.Any() || request.TrainingCertificates == null)
                {
                    throw new ErrorResponse(404, (int)AccountCertificateErrorEnum.CERTIFICATE_LIST_EMPTY,
                                         AccountCertificateErrorEnum.CERTIFICATE_LIST_EMPTY.GetDisplayName());
                }

                //get All training certificate
                var trainingCertificates = _unitOfWork.Repository<TrainingCertificate>()
                                                      .GetAll()
                                                      .Where(a => a.IsActive == true);

                // get all account certificate to check
                var accountCertificates = _unitOfWork.Repository<AccountCertificate>()
                                                     .GetAll()
                                                     .Where(a => a.AccountId == request.AccountId && a.Status == (int)AccountCertificateStatusEnum.Complete);

                var requestInLists = request.TrainingCertificates.ToList();
                var currentTime = Ultils.GetCurrentDatetime();

                foreach (var certificate in requestInLists)
                {
                    //check trainingCertificateId matching with databae
                    if (trainingCertificates.Any(x => x.Id == certificate.TrainingCertificateId))
                    {
                        //check one more time if collaborator already has it
                        if (!accountCertificates.Any(x => x.TrainingCertificateId == certificate.TrainingCertificateId))
                        {
                            //insert to database
                            AccountCertificate newAccountCertificate = new AccountCertificate()
                            {
                                AccountId = request.AccountId,
                                CertificateIssuerId = certificateIssuerId,
                                TrainingCertificateId = certificate.TrainingCertificateId,
                                Status = (int)AccountCertificateStatusEnum.Complete,
                                CreateAt = currentTime
                            };
                            await _unitOfWork.Repository<AccountCertificate>().InsertAsync(newAccountCertificate);
                            continue;
                        }

                        //if Collab training certificate already has in database
                        //remove it
                        request.TrainingCertificates.Remove(certificate);
                        continue;
                    }

                    //if training certificate not in database
                    //remove it
                    request.TrainingCertificates.Remove(certificate);
                }

                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AccountCertificateResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Account Certificate Create Successfully",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = null
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AccountCertificateResponse>> GetAccountCertificateById(int accountCertiId)
        {
            try
            {
                var postTitle = _unitOfWork.Repository<AccountCertificate>().GetAll()
                                      .FirstOrDefault(x => x.Id == accountCertiId);

                if (postTitle == null)
                {
                    throw new ErrorResponse(404, (int)AccountCertificateErrorEnum.NOT_FOUND_ID,
                                         AccountCertificateErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                return new BaseResponseViewModel<AccountCertificateResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountCertificateResponse>(postTitle)
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponsePagingViewModel<AccountCertificateResponse>> GetAccountCertificates(AccountCertificateResponse filter, PagingRequest paging)
        {
            try
            {
                var accountCerti = _unitOfWork.Repository<AccountCertificate>().GetAll()
                                    .ProjectTo<AccountCertificateResponse>(_mapper.ConfigurationProvider)
                                    .OrderByDescending(x => x.CreateAt)
                                    .DynamicFilter(filter)
                                    .DynamicSort(paging.Sort, paging.Order)
                                    .PagingQueryable(paging.Page, paging.PageSize,
                                    Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<AccountCertificateResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = accountCerti.Item1
                    },
                    Data = accountCerti.Item2.ToList()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AccountCertificateResponse>> UpdateAccountCertificate(int certificateIssuerId, UpdateAccountCertificateRequest request)
        {
            // chỉ có người tạo certi cho account mới có quyền update status
            // Mỗi collab account có 1 và chỉ 1 certi/loại. Có thể có certi A, B, C
            // nhưng không được có > 1 certi cùng loại 
            try
            {
                //check account post Permission
                var checkAccount = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == certificateIssuerId);

                if (checkAccount == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                else if (checkAccount.PostPermission == false)
                {
                    throw new ErrorResponse(403, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                //check accountId is collaborator or not
                var checkCollaboratorAccount = await _unitOfWork.Repository<Account>()
                                                    .FindAsync(x => x.Id == request.AccountId && x.RoleId == (int)SystemRoleEnum.Collaborator);

                if (checkCollaboratorAccount == null)
                {
                    throw new ErrorResponse(400, (int)AccountCertificateErrorEnum.ACCOUNT_COLLABORATOR_INVALID,
                                         AccountCertificateErrorEnum.ACCOUNT_COLLABORATOR_INVALID.GetDisplayName());
                }

                var accountCertificate = await _unitOfWork.Repository<AccountCertificate>()
                                                  .FindAsync(a => a.AccountId == request.AccountId
                                                  && a.Id == request.AccountCertificateId);

                if (accountCertificate == null)
                {
                    throw new ErrorResponse(400, (int)AccountCertificateErrorEnum.NOT_FOUND_ID,
                                         AccountCertificateErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                if (accountCertificate.Status == request.Status)
                {
                    throw new ErrorResponse(400, (int)AccountCertificateErrorEnum.STATUS_ALREADY_SAME,
                                         AccountCertificateErrorEnum.STATUS_ALREADY_SAME.GetDisplayName());
                }

                switch (accountCertificate.Status)
                {
                    case (int)AccountCertificateStatusEnum.Complete:
                        var accountCertificateResult = _mapper.Map<UpdateAccountCertificateRequest, AccountCertificate>(request, accountCertificate);

                        accountCertificateResult.UpdateAt = Ultils.GetCurrentDatetime();

                        await _unitOfWork.Repository<AccountCertificate>().UpdateDetached(accountCertificateResult);
                        await _unitOfWork.CommitAsync();
                        break;

                    case (int)AccountCertificateStatusEnum.Reject:

                        //check recruitment registration to remove them and send notification
                        var currentDate = Ultils.GetCurrentDatetime();

                        //check recruit registration to remove
                        var registrationUnComplete = _unitOfWork.Repository<PostRegistration>()
                                                                .GetAll()
                                                                .Where(p => p.Position.TrainingCertificate != null && p.Position.TrainingCertificate.Id == accountCertificate.TrainingCertificateId);

                        var findingRegistrations = registrationUnComplete.Where(p => p.Status >= (int)PostRegistrationStatusEnum.Pending
                                                                                                           && p.Status <= (int)PostRegistrationStatusEnum.Confirm);
                        foreach (var registration in findingRegistrations)
                        {
                            registration.Status = (int)PostRegistrationStatusEnum.Cancel;
                            registration.CancelTime = currentDate;
                        }
                        _unitOfWork.Repository<PostRegistration>().UpdateRange(findingRegistrations);
                        await _unitOfWork.CommitAsync();

                        var account = _unitOfWork.Repository<Account>().GetAll()
                                            .Where(x => x.Id == accountCertificate.AccountId && x.RoleId == (int)SystemRoleEnum.Collaborator);

                        var accountIds = account.Select(p => p.Id).ToList();

                        //create notification request 
                        PushNotificationRequest notificationRequest = new PushNotificationRequest()
                        {
                            Ids = accountIds,
                            Title = NotificationTypeEnum.Post_Created.GetDisplayName(),
                            Body = "Your both certificate and relevant recruitment registrations have been removed!",
                            NotificationsType = (int)NotificationTypeEnum.ACCOUNT_CERTIFICATE_REMOVED
                        };

                        await _notificationService.PushNotification(notificationRequest);
                        break;

                }
                return new BaseResponseViewModel<AccountCertificateResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountCertificateResponse>(accountCertificate)
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<AccountCertificateResponse>> GetAccountCertificateByAccountId(int accountId, AccountCertificateResponse filter, PagingRequest paging)
        {
            try
            {

                var accountCerti = _unitOfWork.Repository<AccountCertificate>().GetAll()
                                    .Where(a => a.AccountId == accountId)
                                    .ProjectTo<AccountCertificateResponse>(_mapper.ConfigurationProvider)
                                    .DynamicFilter(filter)
                                    .DynamicSort(paging.Sort, paging.Order)
                                    .OrderByDescending(x => x.CreateAt)
                                    .PagingQueryable(paging.Page, paging.PageSize);

                return new BaseResponsePagingViewModel<AccountCertificateResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = accountCerti.Item1
                    },
                    Data = accountCerti.Item2.ToList()
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }
    }
}
