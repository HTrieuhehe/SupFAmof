using System;
using AutoMapper;
using System.Linq;
using System.Text;
using Service.Commons;
using SupFAmof.Data.Entity;
using System.Threading.Tasks;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.Utilities;
using System.Collections.Generic;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using AutoMapper.QueryableExtensions;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.ErrorEnum;
using LAK.Sdk.Core.Utilities;

namespace SupFAmof.Service.Service
{
    public class ApplicationService : IApplicationService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public ApplicationService(IMapper mapper, IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<BaseResponseViewModel<ApplicationResponse>> CreateAccountApplication(int accountId, CreateAccountApplicationRequest request)
        {
            try
            {
                var checkAccountCollab =
                    await _unitOfWork.Repository<Account>()
                                     .FindAsync(x => x.Id == accountId && x.RoleId == (int)SystemRoleEnum.Collaborator);

                if (checkAccountCollab == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                var report = _mapper.Map<CreateAccountApplicationRequest, Application>(request);

                report.AccountId = accountId;
                report.ReportDate = Ultils.GetCurrentDatetime();
                report.Status = (int)ReportProblemStatusEnum.Pending;

                await _unitOfWork.Repository<Application>().InsertAsync(report);

                //create notification request 

                List<int> accountIds = new List<int> { accountId };

                PushNotificationRequest notificationRequest = new PushNotificationRequest()
                {
                    Ids = accountIds,
                    Title = NotificationTypeEnum.Application.GetDisplayName(),
                    Body = "Application Request is now created! Waiting for replying",
                    NotificationsType = (int)NotificationTypeEnum.Application
                };

                await _notificationService.PushNotification(notificationRequest);

                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<ApplicationResponse>
                {
                    Status = new StatusViewModel
                    {
                        Message = "Success",
                        ErrorCode = 0,
                        Success = true,
                    },
                    Data = _mapper.Map<ApplicationResponse>(report)

                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<ApplicationResponse>> GetAccountApplicationsByToken(int accountId, ApplicationResponse filter, PagingRequest paging)
        {
            try
            {
                var reportProblem = _unitOfWork.Repository<Application>().GetAll()
                                               .Where(x => x.AccountId == accountId)
                                               .OrderByDescending(x => x.ReportDate)
                                               .ProjectTo<ApplicationResponse>(_mapper.ConfigurationProvider)
                                               .DynamicFilter(filter)
                                               .DynamicSort(paging.Sort, paging.Order)
                                                .PagingQueryable(paging.Page, paging.PageSize);

                return new BaseResponsePagingViewModel<ApplicationResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = reportProblem.Item1
                    },
                    Data = reportProblem.Item2.ToList(),
                };

            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<AdmissionApplicationResponse>> GetAdmissionAccountApplications(int accountId, AdmissionApplicationResponse filter, PagingRequest paging)
        {
            try
            {
                //check admission permission

                var checkAdmission = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);

                if (checkAdmission == null )
                {
                    throw new ErrorResponse(403, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                var reportProblem = _unitOfWork.Repository<Application>().GetAll()
                                               .ProjectTo<AdmissionApplicationResponse>(_mapper.ConfigurationProvider)
                                               .OrderByDescending(x => x.ReportDate)
                                               .DynamicFilter(filter)
                                               .DynamicSort(paging.Sort, paging.Order)
                                               .PagingQueryable(paging.Page, paging.PageSize);

                return new BaseResponsePagingViewModel<AdmissionApplicationResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = reportProblem.Item1
                    },
                    Data = reportProblem.Item2.ToList(),
                };

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AdmissionApplicationResponse>> RejectApplication(int accountId, int reportId, UpdateAdmissionAccountApplicationRequest request)
        {
            try
            {
                var checkAdmission = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);

                if (checkAdmission == null)
                {
                    throw new ErrorResponse(403, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                var report = await _unitOfWork.Repository<Application>().FindAsync(r => r.Id == reportId);

                if (report == null)
                {
                    throw new ErrorResponse(404, (int)ComplaintErrorEnum.NOT_FOUND_REPORT,
                                        ComplaintErrorEnum.NOT_FOUND_REPORT.GetDisplayName());
                }
                switch (report.Status)
                {
                    case (int)ReportProblemStatusEnum.Approve:
                        throw new ErrorResponse(400,
                            (int)ComplaintErrorEnum.ALREADY_APPROVE,
                            ComplaintErrorEnum.ALREADY_APPROVE.GetDisplayName());

                    // Add more cases here if needed
                    case (int)ReportProblemStatusEnum.Reject:
                        throw new ErrorResponse(400,
                            (int)ComplaintErrorEnum.ALREADY_REJECT,
                            ComplaintErrorEnum.ALREADY_REJECT.GetDisplayName());

                    default:
                        break;
                }

                var replyReport = _mapper.Map<UpdateAdmissionAccountApplicationRequest, Application>(request, report);

                replyReport.ReplyDate = Ultils.GetCurrentDatetime();
                replyReport.Status = (int)ReportProblemStatusEnum.Reject;

                await _unitOfWork.Repository<Application>().UpdateDetached(replyReport);

                //create notification request 

                List<int> accountIds = new List<int> { accountId };

                PushNotificationRequest notificationRequest = new PushNotificationRequest()
                {
                    Ids = accountIds,
                    Title = NotificationTypeEnum.Application.GetDisplayName(),
                    Body = "Your application is replied! Check it now",
                    NotificationsType = (int)NotificationTypeEnum.Application
                };

                await _notificationService.PushNotification(notificationRequest);

                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AdmissionApplicationResponse>
                {
                    Status = new StatusViewModel
                    {
                        Message = "Success",
                        ErrorCode = 0,
                        Success = true,
                    },
                    Data = _mapper.Map<AdmissionApplicationResponse>(report)

                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AdmissionApplicationResponse>> ApproveApplication(int accountId, int reportId, UpdateAdmissionAccountApplicationRequest request)
        {
            try
            {
                var checkAdmission = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);

                if (checkAdmission == null)
                {
                    throw new ErrorResponse(403, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                var report = await _unitOfWork.Repository<Application>().FindAsync(r => r.Id == reportId);

                if (report == null)
                {
                    throw new ErrorResponse(404, (int)ComplaintErrorEnum.NOT_FOUND_REPORT,
                                        ComplaintErrorEnum.NOT_FOUND_REPORT.GetDisplayName());
                }
                switch (report.Status)
                {
                    case (int)ReportProblemStatusEnum.Approve:
                        throw new ErrorResponse(400,
                            (int)ComplaintErrorEnum.ALREADY_APPROVE,
                            ComplaintErrorEnum.ALREADY_APPROVE.GetDisplayName());

                    // Add more cases here if needed
                    case (int)ReportProblemStatusEnum.Reject:
                        throw new ErrorResponse(400,
                            (int)ComplaintErrorEnum.ALREADY_REJECT,
                            ComplaintErrorEnum.ALREADY_REJECT.GetDisplayName());

                    default:
                        break;
                }

                var replyReport = _mapper.Map<UpdateAdmissionAccountApplicationRequest, Application>(request, report);

                replyReport.ReplyDate = Ultils.GetCurrentDatetime();
                replyReport.Status = (int)ReportProblemStatusEnum.Approve;

                await _unitOfWork.Repository<Application>().UpdateDetached(replyReport);

                //create notification request 

                List<int> accountIds = new List<int> { accountId };

                PushNotificationRequest notificationRequest = new PushNotificationRequest()
                {
                    Ids = accountIds,
                    Title = NotificationTypeEnum.Application.GetDisplayName(),
                    Body = "Your application is replied! Check it now",
                    NotificationsType = (int)NotificationTypeEnum.Application
                };

                await _notificationService.PushNotification(notificationRequest);

                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AdmissionApplicationResponse>
                {
                    Status = new StatusViewModel
                    {
                        Message = "Success",
                        ErrorCode = 0,
                        Success = true,
                    },
                    Data = _mapper.Map<AdmissionApplicationResponse>(report)

                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
