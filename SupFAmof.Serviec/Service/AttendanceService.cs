using AutoMapper;
using SupFAmof.Data.Entity;
using LAK.Sdk.Core.Utilities;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.Utilities;
using Org.BouncyCastle.Asn1.Ocsp;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using AutoMapper.QueryableExtensions;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.DTO.Response.Admission;
using static SupFAmof.Service.Utilities.Ultils;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.ErrorEnum;

namespace SupFAmof.Service.Service
{
    public class AttendanceService : IAttendanceService
    {
        //chỉ có những post có status close hoặc end thì mới cho phép coi attendance

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public AttendanceService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        #region Admission

        public async Task<BaseResponsePagingViewModel<AdmissionAttendanceResponse>> GetAttendanceHistoryByPositionId
            (int accountId, int positionId, PagingRequest paging)
        
        { 
            try
            {
                if (positionId == 0)
                {
                    throw new ErrorResponse(404, (int)CheckAttendanceErrorEnum.ATTENDANCE_NOT_FOUND,
                                        CheckAttendanceErrorEnum.ATTENDANCE_NOT_FOUND.GetDisplayName());
                }

                //check account post Permission
                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);

                if (account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                else if (account.PostPermission == false)
                {
                    throw new ErrorResponse(403, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                //view attendance
                var attendanceHistory = _unitOfWork.Repository<CheckAttendance>().GetAll()
                                                   .Where(x => x.PostRegistration.PositionId == positionId)
                                                   .ProjectTo<AdmissionAttendanceResponse>(_mapper.ConfigurationProvider)
                                                   .PagingQueryable(paging.Page, paging.PageSize);


                return new BaseResponsePagingViewModel<AdmissionAttendanceResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = attendanceHistory.Item1
                    },
                    Data = attendanceHistory.Item2.ToList(),
                };
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<dynamic>> AdmissionConfirmAttendance(int accountId, int positionId, List<AdmissionConfirmAttendanceRequest> requests)
        {
            try
            {
                var position = await _unitOfWork.Repository<PostPosition>().FindAsync(x => x.Id == positionId && x.Post.AccountId == accountId);
                var requestStatusMap = requests.ToDictionary(request => request.Id, request => request.Status);
                var requestIds = requests.Select(x=>x.Id);
                var filteredList = _unitOfWork.Repository<CheckAttendance>()
                      .GetAll().Where(attendance => requestIds.Contains(attendance.Id)&&attendance.PostRegistration.PositionId == positionId);

                //check if list has the same position Id
                 if(!filteredList.Any(x=>x.PostRegistration.PositionId == positionId)) {
                    throw new ErrorResponse(400, (int)CheckAttendanceErrorEnum.CANT_UPDATE_WRONG_POSITION,
                                       CheckAttendanceErrorEnum.CANT_UPDATE_WRONG_POSITION.GetDisplayName());
                }
                if (position == null)
                {
                    throw new ErrorResponse(404, (int)PostRegistrationErrorEnum.POSITION_NOTFOUND,
                                        PostRegistrationErrorEnum.POSITION_NOTFOUND.GetDisplayName());
                }
                if (!await MinumumTimeToConfirm((DateTime)(position.Date + position.TimeTo)))
                {
                    throw new ErrorResponse(400, (int)CheckAttendanceErrorEnum.CONFIRM_TIME_NOT_BEGIN,
                                       CheckAttendanceErrorEnum.CONFIRM_TIME_NOT_BEGIN.GetDisplayName());
                }
                if (!await MaximumTimeToConfirm((DateTime)(position.Date + position.TimeTo), 1))
                {
                    throw new ErrorResponse(400, (int)CheckAttendanceErrorEnum.CONFIRM_TIME_EXPIRED,
                                       CheckAttendanceErrorEnum.CONFIRM_TIME_EXPIRED.GetDisplayName());
                }
                foreach (var item in filteredList)
                {
                    item.Status = requestStatusMap[item.Id].Value;
                    item.ConfirmTime = GetCurrentDatetime();
                    if (item.Status == (int)CheckAttendanceEnum.Approved)
                    {
                        CreateAccountReportRequest request = new CreateAccountReportRequest
                        {
                            AccountId = item.PostRegistration.AccountId,
                            PositionId = item.PostRegistration.PositionId,
                            Salary = item.PostRegistration.Salary,
                        };
                        if (await CheckDuplicateAccountReport(request))
                        {
                            await CreateAccountReport(request);
                        }
                    }
                    if(item.Status == (int)CheckAttendanceEnum.Pending)
                    {
                        CreateAccountReportRequest reject = new CreateAccountReportRequest
                        {
                            AccountId = item.PostRegistration.AccountId,
                            PositionId = item.PostRegistration.PositionId,
                            Salary = item.PostRegistration.Salary,
                        };
                        if (!await CheckDuplicateAccountReport(reject))
                        {
                            await RemoveAccountReport(reject);
                        }
                    }
                    await _unitOfWork.Repository<CheckAttendance>().UpdateDetached(item);
                  
                }
                await _unitOfWork.CommitAsync();
                List<int> accountIds = new List<int>(filteredList.Select(x=>x.PostRegistration.AccountId)).ToList();
                PushNotificationRequest notificationRequest = new PushNotificationRequest()
                {
                    Ids = accountIds,
                    Title = NotificationTypeEnum.CheckOut_Confirmed.GetDisplayName(),
                    Body = "Your attendance is reivewed.Check now!",
                    NotificationsType = (int)NotificationTypeEnum.CheckOut_Confirmed
                };
                await _notificationService.PushNotification(notificationRequest);
                return new BaseResponseViewModel<dynamic>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Update success",
                        Success = true,
                        ErrorCode = 0
                    }
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private async Task<bool> MaximumTimeToConfirm(DateTime timeOfCurrentPosition, int day)
        {
            DateTime timeMaximum = timeOfCurrentPosition.AddDays(day);
            DateTime currentTime = GetCurrentDatetime();
            if (currentTime > timeMaximum)
            {
                return false;
            }
            return true;
        }
        private async Task<bool> MinumumTimeToConfirm(DateTime timeOfCurrentPosition)
        {
            DateTime currentTime = GetCurrentDatetime();
            if (currentTime > timeOfCurrentPosition)
            {
                return true;
            }
            return false ;
        }
        #endregion
        private async Task<bool> CheckDuplicateAccountReport(CreateAccountReportRequest request)
        {
                var accountReport = await _unitOfWork.Repository<AccountReport>().FindAsync(x=>x.AccountId == request.AccountId && x.PositionId == request.PositionId);
                if (accountReport == null)
                {
                    return true;
                }
                return false;

        }
        private async Task CreateAccountReport(CreateAccountReportRequest request)
        {
            var accountReport = _mapper.Map<CreateAccountReportRequest, AccountReport>(request);

            accountReport.CreateAt = Ultils.GetCurrentDatetime();

            await _unitOfWork.Repository<AccountReport>().InsertAsync(accountReport);
        }
        private async Task RemoveAccountReport(CreateAccountReportRequest request)
        {
            var getAccountReport = await _unitOfWork.Repository<AccountReport>().FindAsync(x => x.AccountId == request.AccountId && x.PositionId == request.PositionId);
            await _unitOfWork.Repository<AccountReport>().HardDelete(getAccountReport.Id);
        }
    }
}
