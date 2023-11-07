using QRCoder;
using AutoMapper;
using Newtonsoft.Json;
using Service.Commons;
using SupFAmof.Data.Entity;
using LAK.Sdk.Core.Utilities;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.Utilities;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using Microsoft.EntityFrameworkCore;
using SupFAmof.Service.DTO.Response;
using System.Net.NetworkInformation;
using AutoMapper.QueryableExtensions;
using SixLabors.ImageSharp.Formats.Png;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.ErrorEnum;

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
                                                          .SingleOrDefaultAsync(x => x.PostRegistration.Position.Post.Id == checkin.PostId
                                                                                 && x.PostRegistration.AccountId == accountId
                                                                                 );

                if (existingAttendance != null)
                {
                    throw new ErrorResponse(404, (int)AttendanceErrorEnum.ALREADY_CHECK_IN,
                                        AttendanceErrorEnum.ALREADY_CHECK_IN.GetDisplayName());
                }

                var postVerification = await _unitOfWork.Repository<PostRegistration>().GetAll().SingleOrDefaultAsync(x => x.Position.PostId == checkin.PostId && x.PositionId == checkin.PositionId && x.AccountId == accountId && x.Status == (int)PostRegistrationStatusEnum.Confirm);
                if (postVerification == null)
                {
                    throw new ErrorResponse(404, (int)AttendanceErrorEnum.WRONG_INFORMATION,
                                        AttendanceErrorEnum.WRONG_INFORMATION.GetDisplayName());
                }
                if (postVerification.Position.Latitude == null || postVerification.Position.Longtitude == null)
                {
                    throw new ErrorResponse(500, (int)AttendanceErrorEnum.MISSING_INFORMATION_POSITION,
                                       AttendanceErrorEnum.MISSING_INFORMATION_POSITION.GetDisplayName());
                }
                double distance = 0.04; // kilometer 

                double userCurrentPosition = ((double)Ultils.CalculateDistance(postVerification.Position.Latitude, postVerification.Position.Longtitude, checkin.Latitude, checkin.Longtitude));

                if (userCurrentPosition > distance)
                {
                    throw new ErrorResponse(404, (int)AttendanceErrorEnum.DISTANCE_TOO_FAR,
                                        AttendanceErrorEnum.DISTANCE_TOO_FAR.GetDisplayName() + $":{userCurrentPosition} km");
                }
                if (VerifyDateTimeCheckin(postVerification, checkAttendance.CheckInTime))
                {
                    var registration = _unitOfWork.Repository<PostRegistration>()
                                                .Find(x => x.AccountId == accountId && x.PositionId == checkin.PositionId);

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
                            Message = "Check In Success",
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
                                    .FirstOrDefault(x => x.PostRegistration.AccountId == accountId && x.PostRegistration.Position.PostId == request.PostId && request.PositionId == request.PositionId);

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
                                                .Find(x => x.AccountId == accountId && x.PositionId == request.PositionId);

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
                        PositionId = position.Id,
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
                                                .Find(x => x.AccountId == accountId && x.PositionId == checkOut.PostRegistration.PositionId);

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
                        PositionId = position.Id,
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

        private static bool VerifyDateTimeCheckin(PostRegistration postTime, DateTime checkInTime)
        {
            if (postTime.Position.Post.DateFrom.Date != checkInTime.Date)
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

                accountReport.CreateAt = Ultils.GetCurrentDatetime();

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


        public async Task<byte[]> QrGenerate(QrRequest request)
        {
            string data = JsonConvert.SerializeObject(request);
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            int originalWidth = qrCodeData.ModuleMatrix.Count;
            int originalHeight = qrCodeData.ModuleMatrix.Count;

            int scaledWidth = originalWidth * 7;
            int scaledHeight = originalHeight * 7;

            using (Image<Rgba32> qrCodeImage = new Image<Rgba32>(scaledWidth, scaledHeight))
            {
                // Draw scaled QR code onto ImageSharp image
                for (int x = 0; x < scaledWidth; x++)
                {
                    for (int y = 0; y < scaledHeight; y++)
                    {
                        int originalX = x / 7;
                        int originalY = y / 7;

                        if (qrCodeData.ModuleMatrix[originalX][originalY])
                        {
                            qrCodeImage[x, y] = new Rgba32(0, 0, 0, 255); // Set QR code modules to black
                        }
                        else
                        {
                            qrCodeImage[x, y] = new Rgba32(255, 255, 255, 255); // Set QR code background modules to white
                        }
                    }
                }

                // Convert ImageSharp image to byte array
                using (MemoryStream stream = new MemoryStream())
                {
                    qrCodeImage.Save(stream, new PngEncoder());
                    return stream.ToArray();
                }
            }

        }


        public async Task<BaseResponseViewModel<List<CheckAttendanceResponse>>> CheckAttendanceHistory(int accountId)
        {
            try
            {
                var attendanceRecord = _mapper.Map<List<CheckAttendanceResponse>>(await _unitOfWork.Repository<CheckAttendance>()
                                                                            .GetWhere(x => x.PostRegistration.AccountId == accountId));
                return new BaseResponseViewModel<List<CheckAttendanceResponse>>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Record of attendance",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = attendanceRecord
                };

            }
            catch(Exception ex)
            {
                throw; 
            }
        }
        public async Task<BaseResponsePagingViewModel<CheckAttendancePostResponse>> AdmissionManageCheckAttendanceRecord(int accountId,PagingRequest paging)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);
                if (!account.PostPermission)
                {
                    throw new Exceptions.ErrorResponse(401, (int)AccountReportErrorEnum.UNAUTHORIZED, AccountReportErrorEnum.UNAUTHORIZED.GetDisplayName());
                }
                var attendanceRecord =_unitOfWork.Repository<Post>().GetAll().Where(x=>x.AccountId == accountId)
                                          .ProjectTo<CheckAttendancePostResponse>(_mapper.ConfigurationProvider)
                                          .PagingQueryable(paging.Page, paging.PageSize,
                                                           Constants.LimitPaging, Constants.DefaultPaging);
                return new BaseResponsePagingViewModel<CheckAttendancePostResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = attendanceRecord.Item1
                    },
                    Data = attendanceRecord.Item2.ToList()
                };

            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
