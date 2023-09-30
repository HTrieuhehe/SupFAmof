using System;
using AutoMapper;
using System.Linq;
using SupFAmof.Data.Entity;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using Microsoft.EntityFrameworkCore;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.ErrorEnum;
using NTQ.Sdk.Core.Utilities;
using SupFAmof.Service.Utilities;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Request.Account;

namespace SupFAmof.Service.Service
{
    public class CheckInService : ICheckInService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CheckInService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponseViewModel<dynamic>> CheckIn(int accountId, CheckInRequest checkin)
        {
            try
            {
                var checkAttendance = _mapper.Map<CheckAttendance>(checkin);
                checkAttendance.CheckInTime = Ultils.GetCurrentDatetime();

                var existingAttendance = await _unitOfWork.Repository<CheckAttendance>()
                                                          .GetAll()
                                                          .SingleOrDefaultAsync(x => x.PostId == checkin.PostId
                                                                                 && x.AccountId == accountId
                                                                                 );

                if (existingAttendance != null)
                {
                    throw new ErrorResponse(404, (int)AttendanceErrorEnum.ALREADY_CHECK_IN,
                                        AttendanceErrorEnum.ALREADY_CHECK_IN.GetDisplayName());
                }

                var postVerification = await _unitOfWork.Repository<PostAttendee>().GetAll().SingleOrDefaultAsync(x => x.PostId == checkin.PostId && x.PositionId == checkin.PositionId && x.AccountId == accountId);
                double distance = 0.04; // kilometer 
                double userCurrentPosition = ((double)Utilities.Ultils.CalculateDistance(postVerification.Position.Latitude, postVerification.Position.Longtitude, checkin.Latitude, checkin.Longtitude));
                if (userCurrentPosition > distance)
                {
                    //throw new ErrorResponse(400, 400, $"Distance {userCurrentPosition} is to far ");

                    throw new ErrorResponse(404, (int)AttendanceErrorEnum.DISTANCE_TOO_FAR,
                                        AttendanceErrorEnum.DISTANCE_TOO_FAR.GetDisplayName() + $":{userCurrentPosition} km");
                }
                if (VerifyDateTimeCheckin(postVerification, checkAttendance.CheckInTime))
                {
                    var registration = _unitOfWork.Repository<PostRegistration>()
                                                .Find(x => x.AccountId == accountId && x.PostRegistrationDetails.Any(x => x.Id == checkin.PositionId));

                    if (registration == null)
                    {
                        throw new ErrorResponse(404, (int)PostRegistrationErrorEnum.NOT_FOUND_POST,
                                        PostRegistrationErrorEnum.NOT_FOUND_POST.GetDisplayName());
                    }

                    registration.Status = (int)PostRegistrationStatusEnum.CheckOut;

                    await _unitOfWork.Repository<CheckAttendance>().InsertAsync(checkAttendance);
                    await _unitOfWork.CommitAsync();

                    return new BaseResponseViewModel<dynamic>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "Check Out Success",
                            Success = true,
                            ErrorCode = 0
                        },
                        Data = ""
                    };
                }

            }
            catch (Exception ex)
            {
                throw;
            }

            throw new ErrorResponse(400, (int)AttendanceErrorEnum.CAN_NOT_CHECK_OUT,
                                    AttendanceErrorEnum.CAN_NOT_CHECK_OUT.GetDisplayName());
        }

        public async Task<BaseResponseViewModel<dynamic>> CheckOut(int accountId, CheckOutRequest request)
        {
            try
            {
                //check Checking Existed
                var checkOutCheck = _unitOfWork.Repository<CheckAttendance>().GetAll()
                                    .FirstOrDefault(x => x.AccountId == accountId && x.PostId == request.PostId && request.PositionId == request.PositionId);

                if (checkOutCheck == null)
                {
                    throw new ErrorResponse(404, (int)AttendanceErrorEnum.CHECK_OUT_FAIL,
                                        AttendanceErrorEnum.CHECK_OUT_FAIL.GetDisplayName());
                }

                //check timeTo have or not have data and validate in 2 situation

                var position = _unitOfWork.Repository<PostPosition>().GetAll().FirstOrDefault(x => x.Id == request.PositionId);

                if (position == null)
                {
                    throw new ErrorResponse(404, (int)PostErrorEnum.POSITION_NOT_FOUND,
                                                            PostErrorEnum.POSITION_NOT_FOUND.GetDisplayName());
                }

                else if (position.TimeTo.HasValue)
                {
                    //get current Time
                    var currentTime = DateTime.Now.TimeOfDay;

                    if (position.TimeTo > currentTime)
                    {
                        throw new ErrorResponse(400, (int)AttendanceErrorEnum.CHECK_OUT_TIME_INVALID,
                                        AttendanceErrorEnum.CHECK_OUT_TIME_INVALID.GetDisplayName());
                    }

                    var checkOut = _mapper.Map<CheckOutRequest, CheckAttendance>(request, checkOutCheck);
                    checkOut.CheckOutTime = Ultils.GetCurrentDatetime();

                    var registration = _unitOfWork.Repository<PostRegistration>()
                                                .Find(x => x.AccountId == accountId && x.PostRegistrationDetails.Any(x => x.Id == request.PositionId));

                    if (registration == null)
                    {
                        throw new ErrorResponse(404, (int)PostRegistrationErrorEnum.NOT_FOUND_POST,
                                        PostRegistrationErrorEnum.NOT_FOUND_POST.GetDisplayName());
                    }

                    registration.Status = (int)PostRegistrationStatusEnum.CheckOut;

                    CreateAccountReportRequest accountReport = new CreateAccountReportRequest()
                    {
                        AccountId = accountId,
                        PostId = position.PostId,
                        Salary = position.Salary,
                    };

                    await CreateAccountReport(accountReport);

                    await _unitOfWork.Repository<CheckAttendance>().UpdateDetached(checkOut);
                    await _unitOfWork.Repository<PostRegistration>().UpdateDetached(registration);
                    await _unitOfWork.CommitAsync();
                }

                //if time to did not have value
                else if (!position.TimeTo.HasValue)
                {
                    //get current Time
                    var currentTime = DateTime.Now.TimeOfDay;
                    var time = position.TimeFrom;
                    time = time.Add(TimeSpan.FromHours(2));

                    //allow checkout after 2 hours
                    if (time > currentTime)
                    {
                        throw new ErrorResponse(400, (int)AttendanceErrorEnum.CHECK_OUT_TIME_INVALID,
                                        AttendanceErrorEnum.CHECK_OUT_TIME_INVALID.GetDisplayName());
                    }

                    var checkOut = _mapper.Map<CheckOutRequest, CheckAttendance>(request, checkOutCheck);
                    checkOut.CheckOutTime = Ultils.GetCurrentDatetime();

                    var registration = _unitOfWork.Repository<PostRegistration>()
                                                .Find(x => x.AccountId == accountId && x.PostRegistrationDetails.Any(x => x.PositionId == checkOut.PositionId));

                    if (registration == null)
                    {
                        throw new ErrorResponse(404, (int)PostRegistrationErrorEnum.NOT_FOUND_POST,
                                        PostRegistrationErrorEnum.NOT_FOUND_POST.GetDisplayName());
                    }

                    registration.Status = (int)PostRegistrationStatusEnum.CheckOut;

                    CreateAccountReportRequest accountReport = new CreateAccountReportRequest()
                    {
                        AccountId = accountId,
                        PostId = position.PostId,
                        Salary = position.Salary,
                    };

                    await CreateAccountReport(accountReport);

                    await _unitOfWork.Repository<CheckAttendance>().UpdateDetached(checkOut);
                    await _unitOfWork.Repository<PostRegistration>().UpdateDetached(registration);
                    await _unitOfWork.CommitAsync();

                    return new BaseResponseViewModel<dynamic>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "Check Out Success",
                            Success = true,
                            ErrorCode = 0
                        },
                        Data = ""
                    };
                }

                throw new ErrorResponse(400, (int)AttendanceErrorEnum.CAN_NOT_CHECK_OUT,
                                    AttendanceErrorEnum.CAN_NOT_CHECK_OUT.GetDisplayName());
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static bool VerifyDateTimeCheckin(PostAttendee postTime, DateTime checkInTime)
        {
            if (postTime.Post.DateFrom.Date != checkInTime.Date)
            {
                throw new ErrorResponse(400, 400, "Cant register if you are not on the day of event");
            }

            // Calculate the time difference in hours between checkInTime and postTime.Position.TimeFrom
            TimeSpan timeDifference = checkInTime.TimeOfDay - postTime.Position.TimeFrom;

            // Check if the time difference is within a 2-hour range
            if (timeDifference.TotalHours < -1 || timeDifference.TotalHours > 0.5)
            {
                throw new ErrorResponse(400, 400, "Must check in around 30 minutes when the position start");
            }

            return true;
        }

        private async Task<BaseResponseViewModel<AccountReportResponse>> CreateAccountReport(CreateAccountReportRequest request)
        {
            try
            {
                var accountReport = _mapper.Map<CreateAccountReportRequest, AccountReport>(request);

                accountReport.Date = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<AccountReport>().InsertAsync(accountReport);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AccountReportResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Check Out Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountReportResponse>(accountReport)
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

    }

}
