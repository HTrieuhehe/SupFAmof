using AutoMapper;
using ServiceStack;
using SupFAmof.Data.Entity;
using LAK.Sdk.Core.Utilities;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.Utilities;
using SupFAmof.Service.DTO.Request;
using Microsoft.EntityFrameworkCore;
using SupFAmof.Service.DTO.Response;
using AutoMapper.QueryableExtensions;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Response.Admission;
using static SupFAmof.Service.Utilities.Ultils;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.ErrorEnum;
using ErrorResponse = SupFAmof.Service.Exceptions.ErrorResponse;

namespace SupFAmof.Service.Service
{
    public class TrainingCertificateService : ITrainingCertificateService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public TrainingCertificateService(IMapper mapper, IUnitOfWork unitOfWork,
                                          INotificationService notificationService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<BaseResponsePagingViewModel<TrainingCertificateResponse>>
        GetTrainingCertificates(TrainingCertificateResponse filter,
                                PagingRequest paging)
        {
            try
            {
                var role = _unitOfWork.Repository<TrainingCertificate>()
                               .GetAll()
                               .ProjectTo<TrainingCertificateResponse>(
                                   _mapper.ConfigurationProvider)
                               .DynamicFilter(filter)
                               .DynamicSort(paging.Sort, paging.Order)
                               .Where(x => x.IsActive == true)
                               .PagingQueryable(paging.Page, paging.PageSize);

                return new BaseResponsePagingViewModel<TrainingCertificateResponse>()
                {
                    Metadata =
                      new PagingsMetadata()
                      {
                          Page = paging.Page,
                          Size = paging.PageSize,
                          Total = role.Item1
                      },
                    Data = role.Item2.ToList()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<TrainingCertificateResponse>>
        GetTrainingCertificateById(int trainingCertificateId)
        {
            try
            {
                var trainingCertificate =
                    await _unitOfWork.Repository<TrainingCertificate>()
                        .GetAll()
                        .FirstOrDefaultAsync(x => x.Id == trainingCertificateId &&
                                                  x.IsActive == true);

                if (trainingCertificate == null)
                {
                    throw new ErrorResponse(
                        404, (int)TrainingCertificateErrorEnum.NOT_FOUND_ID,
                        TrainingCertificateErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                return new BaseResponseViewModel<TrainingCertificateResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<TrainingCertificateResponse>(trainingCertificate)
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponseViewModel<TrainingCertificateResponse>>
        CreateTrainingCertificate(int accountId,
                                  CreateTrainingCertificateRequest request)
        {
            try
            {
                // check account post Permission
                var checkAccount = await _unitOfWork.Repository<Account>().FindAsync(
                    x => x.Id == accountId);

                if (checkAccount == null)
                {
                    throw new ErrorResponse(
                        404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                else if (checkAccount.PostPermission == false)
                {
                    throw new ErrorResponse(
                        403, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                if (request.TrainingTypeId == null || request.TrainingTypeId == "")
                {
                    throw new ErrorResponse(
                        400,
                        (int)TrainingCertificateErrorEnum.INVALID_TRAINING_CERTIFICATE_TYPE,
                        TrainingCertificateErrorEnum.INVALID_TRAINING_CERTIFICATE_TYPE
                            .GetDisplayName());
                }

                var tranningCertificate =
                    await _unitOfWork.Repository<TrainingCertificate>().FindAsync(
                        x => x.TrainingTypeId.Contains(request.TrainingTypeId) &&
                             x.IsActive);

                if (tranningCertificate != null)
                {
                    throw new ErrorResponse(
                        400, (int)TrainingCertificateErrorEnum.TRAINING_CERTIFICATE_EXISTED,
                        TrainingCertificateErrorEnum.TRAINING_CERTIFICATE_EXISTED
                            .GetDisplayName());
                }

                var result =
                    _mapper.Map<CreateTrainingCertificateRequest, TrainingCertificate>(
                        request);

                result.TrainingTypeId = result.TrainingTypeId.ToUpper();
                result.IsActive = true;
                result.CreateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<TrainingCertificate>().InsertAsync(result);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<TrainingCertificateResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<TrainingCertificateResponse>(result)
                };
            }
            catch (ErrorResponse ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<TrainingCertificateResponse>>
        UpdateTrainingCertificate(int accountId, int trainingCertificateId,
                                  UpdateTrainingCertificateRequest request)
        {
            try
            {
                // check account post Permission
                var checkAccount = await _unitOfWork.Repository<Account>().FindAsync(
                    x => x.Id == accountId);

                if (checkAccount == null)
                {
                    throw new ErrorResponse(
                        404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                else if (checkAccount.PostPermission == false)
                {
                    throw new ErrorResponse(
                        403, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                if (request.TrainingTypeId == null || request.TrainingTypeId == "")
                {
                    throw new ErrorResponse(
                        400,
                        (int)TrainingCertificateErrorEnum.INVALID_TRAINING_CERTIFICATE_TYPE,
                        TrainingCertificateErrorEnum.INVALID_TRAINING_CERTIFICATE_TYPE
                            .GetDisplayName());
                }

                var checkCertificate =
                    await _unitOfWork.Repository<TrainingCertificate>().FindAsync(
                        x => x.TrainingTypeId == request.TrainingTypeId.ToUpper() &&
                             x.IsActive);

                if (checkCertificate == null)
                {
                    throw new ErrorResponse(
                        404, (int)TrainingCertificateErrorEnum.TRAINING_CERTIFICATE_EXISTED,
                        TrainingCertificateErrorEnum.TRAINING_CERTIFICATE_EXISTED
                            .GetDisplayName());
                }

                var tranningCertificate =
                    await _unitOfWork.Repository<TrainingCertificate>().FindAsync(
                        x => x.Id == trainingCertificateId && x.IsActive == true);

                if (tranningCertificate == null)
                {
                    throw new ErrorResponse(
                        404, (int)TrainingCertificateErrorEnum.NOT_FOUND_ID,
                        TrainingCertificateErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                var updateTrainingCertificate =
                    _mapper.Map<UpdateTrainingCertificateRequest, TrainingCertificate>(
                        request, tranningCertificate);

                updateTrainingCertificate.TrainingTypeId =
                    updateTrainingCertificate.TrainingTypeId.ToUpper();
                updateTrainingCertificate.UpdateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<TrainingCertificate>().UpdateDetached(
                    updateTrainingCertificate);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<TrainingCertificateResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data =
                      _mapper.Map<TrainingCertificateResponse>(updateTrainingCertificate)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<TrainingCertificateResponse>>
        SearchTrainingCertificate(string search, PagingRequest paging)
        {
            // Search by Name or Type
            try
            {

                if (search == null || search.Length == 0)
                {
                    throw new ErrorResponse(
                        404, (int)TrainingCertificateErrorEnum.NOT_FOUND_ID,
                        TrainingCertificateErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                var certificate =
                    _unitOfWork.Repository<TrainingCertificate>()
                        .GetAll()
                        .ProjectTo<TrainingCertificateResponse>(
                            _mapper.ConfigurationProvider)
                        .Where(x => x.CertificateName.Contains(search) ||
                                    x.TrainingTypeId.Contains(search.ToUpper()))
                        .PagingQueryable(paging.Page, paging.PageSize);

                if (!certificate.Item2.Any())
                {
                    throw new ErrorResponse(
                        404, (int)TrainingCertificateErrorEnum.NOT_FOUND_ID,
                        TrainingCertificateErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                return new BaseResponsePagingViewModel<TrainingCertificateResponse>()
                {
                    Metadata =
                      new PagingsMetadata()
                      {
                          Page = paging.Page,
                          Size = paging.PageSize,
                          Total = certificate.Item1
                      },
                    Data = certificate.Item2.ToList()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<bool>>
        DisableTrainingCertificate(int accountId, int trainingCertificateId)
        {
            try
            {
                // check account post Permission
                var checkAccount = await _unitOfWork.Repository<Account>().FindAsync(
                    x => x.Id == accountId);

                if (checkAccount == null)
                {
                    throw new ErrorResponse(
                        404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                else if (checkAccount.PostPermission == false)
                {
                    throw new ErrorResponse(
                        403, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                var trainingCertificate =
                    await _unitOfWork.Repository<TrainingCertificate>().FindAsync(
                        x => x.Id == trainingCertificateId && x.IsActive == true);

                if (trainingCertificate == null)
                {
                    throw new ErrorResponse(
                        404, (int)TrainingCertificateErrorEnum.NOT_FOUND_ID,
                        TrainingCertificateErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                trainingCertificate.IsActive = false;
                trainingCertificate.UpdateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<TrainingCertificate>().UpdateDetached(
                    trainingCertificate);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<bool>()
                {
                    Status =
                      new StatusViewModel()
                      {
                          Message = "Delete Training Certificate Successfully",
                          Success = true,
                          ErrorCode = 0
                      },
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region Manh
        public async Task<BaseResponseViewModel<dynamic>>
        CreateDaysForCertificateInterview(int accountId,
                                          EventDaysCertificate request)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().FindAsync(
                    x => x.Id == accountId);
                if (!account.PostPermission)
                {
                    throw new ErrorResponse(401, (int)AccountErrorEnums.API_INVALID,
                                            AccountErrorEnums.API_INVALID.GetDisplayName());
                }
                var eventDay = _mapper.Map<TrainingEventDay>(request);
                if (!await CheckTimeAvailability((DateTime)eventDay.Date,
                                                 eventDay.TimeFrom, eventDay.TimeTo))
                {
                    throw new ErrorResponse(
                        400, (int)TrainingCertificateErrorEnum.OVERLAP_EVENTS,
                        TrainingCertificateErrorEnum.OVERLAP_EVENTS.GetDisplayName());
                }
                await _unitOfWork.Repository<TrainingEventDay>().InsertAsync(eventDay);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<dynamic>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Create success ",
                        Success = true,
                        ErrorCode = 0
                    }
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<dynamic>>
        UpdateDaysForCertificateInterview(int accountId, int evenDayId,
                                          UpdateDaysCertifcate request)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().FindAsync(
                    x => x.Id == accountId);
                if (!account.PostPermission)
                {
                    throw new ErrorResponse(401, (int)AccountErrorEnums.API_INVALID,
                                            AccountErrorEnums.API_INVALID.GetDisplayName());
                }
                var eventDay = await _unitOfWork.Repository<TrainingEventDay>().FindAsync(
                    x => x.Id == evenDayId);
                if (eventDay == null)
                {
                    throw new ErrorResponse(
                        401, (int)TrainingCertificateErrorEnum.TRAINING_DAY_DOES_NOT_EXIST,
                        TrainingCertificateErrorEnum.TRAINING_DAY_DOES_NOT_EXIST
                            .GetDisplayName());
                }
                eventDay.Updateat = request.Updateat;
                if (!string.IsNullOrEmpty(request.Class))
                {
                    eventDay.Class = request.Class;
                }
                if (!string.IsNullOrEmpty(request.Date.ToString()))
                {
                    eventDay.Date = request.Date;
                }

                if (!string.IsNullOrEmpty(request.TimeFrom.ToString()))
                {
                    eventDay.TimeFrom = request.TimeFrom;
                }

                if (!string.IsNullOrEmpty(request.TimeTo.ToString()))
                {
                    eventDay.TimeTo = request.TimeTo;
                }
                await _unitOfWork.Repository<TrainingEventDay>().UpdateDetached(eventDay);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<dynamic>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Update success ",
                        Success = true,
                        ErrorCode = 0
                    }
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<bool> CheckTimeAvailability(DateTime timeOfClass,
                                                       TimeSpan? timeFrom,
                                                       TimeSpan? timeTo)
        {
            var positions = await _unitOfWork.Repository<PostPosition>()
                                .GetAll()
                                .Where(x => x.Date == timeOfClass.Date)
                                .OrderByDescending(x => x.Id)
                                .ToListAsync();

            foreach (var position in positions)
            {
                if (IsTimeSpanOverlapPostion(position.TimeFrom, position.TimeTo, timeFrom,
                                             timeTo))
                {
                    return false;
                }
            }

            return true;
        }
        private bool IsTimeSpanOverlapPostion(TimeSpan? start1, TimeSpan? end1,
                                              TimeSpan? start2, TimeSpan? end2)
        {
            return (start1 < end2 && end1 > start2);
        }

        public async Task<BaseResponseViewModel<CollabRegistrationsResponse>>
        TrainingCertificateRegistration(int accountId,
                                        TrainingCertificateRegistration request)
        {
            try
            {
                var accountBanned =
                    _unitOfWork.Repository<AccountBanned>().GetAll().Where(
                        x => x.AccountIdBanned == accountId && x.IsActive);
                if (accountBanned.Any())
                {
                    var currentDateTime = Ultils.GetCurrentDatetime();

                    var maxDayEnd = accountBanned.Max(x => x.DayEnd);

                    if (maxDayEnd > currentDateTime)
                    {
                        throw new ErrorResponse(
                            400, (int)PostRegistrationErrorEnum.ACCOUNT_BANNED,
                            PostRegistrationErrorEnum.ACCOUNT_BANNED.GetDisplayName());
                    }
                }
                if (request == null)
                {
                    throw new ErrorResponse(
                        400, (int)TrainingCertificateErrorEnum.REGISTER_FAILED,
                        TrainingCertificateErrorEnum.REGISTER_FAILED.GetDisplayName());
                }
                var registration = _mapper.Map<TrainingRegistration>(request);
                registration.AccountId = accountId;
                if (!await CheckIfAccountHasCertificate(request, accountId))
                {
                    throw new ErrorResponse(
                        400, (int)TrainingCertificateErrorEnum.ALREADY_HAVE_CERTIFICATE,
                        TrainingCertificateErrorEnum.ALREADY_HAVE_CERTIFICATE
                            .GetDisplayName());
                }
                if (!await CheckDuplicateTrainingCertificateRegistration(registration))
                {
                    throw new ErrorResponse(
                        400, (int)TrainingCertificateErrorEnum.DUPLICATE_REGISTRATION,
                        TrainingCertificateErrorEnum.DUPLICATE_REGISTRATION
                            .GetDisplayName());
                }
                await _unitOfWork.Repository<TrainingRegistration>().InsertAsync(
                    registration);
                await _unitOfWork.CommitAsync();

                var result = await _unitOfWork.Repository<TrainingRegistration>()
                                 .GetAll()
                                 .Include(x => x.TrainingCertificate)
                                 .FirstOrDefaultAsync(x => x.Id == registration.Id);
                return new BaseResponseViewModel<CollabRegistrationsResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Create success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<CollabRegistrationsResponse>(result)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private async Task<bool>
        CheckDuplicateTrainingCertificateRegistration(TrainingRegistration request)
        {
            var duplicates =
                await _unitOfWork.Repository<TrainingRegistration>().GetWhere(
                    x => x.TrainingCertificateId == request.TrainingCertificateId &&
                         x.AccountId == request.AccountId &&
                         x.Status != (int)TrainingRegistrationStatusEnum.Not_Passed &&
                         x.Status != (int)TrainingRegistrationStatusEnum.Canceled);
            if (duplicates.Any())
            {
                return false;
            }
            return true;
        }
        private async Task<bool>
        CheckIfAccountHasCertificate(TrainingCertificateRegistration request,
                                     int accountId)
        {
            var accountCertificates =
                await _unitOfWork.Repository<AccountCertificate>().FindAsync(
                    x => x.TrainingCertificateId == request.TrainingCertificateId &&
                         x.AccountId == accountId &&
                         x.Status == (int)AccountCertificateStatusEnum.Complete);
            if (accountCertificates != null)
            {
                return false;
            }
            return true;
        }

        public async Task<BaseResponseViewModel<dynamic>>
        AssignDayToRegistration(int accountId,
                                List<AssignEventDayToAccount> requests)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().FindAsync(
                    x => x.Id == accountId);
                if (!account.PostPermission)
                {
                    throw new ErrorResponse(401, (int)AccountErrorEnums.API_INVALID,
                                            AccountErrorEnums.API_INVALID.GetDisplayName());
                }
                var requestStatusMap =
                    requests.ToDictionary(request => request.TrainingRegistrationId,
                                          request => request.EventDayId);
                var requestIds = requests.Select(x => x.TrainingRegistrationId);
                var filteredList =
                    _unitOfWork.Repository<TrainingRegistration>().GetAll().Where(
                        registration => requestIds.Contains(registration.Id) &&
                                        registration.Status != 3);
                if (!filteredList.Any())
                {
                    throw new ErrorResponse(400, 4001, "Nothing");
                }
                foreach (var registration in filteredList)
                {

                    if (!await AssignDuplicateTime(registration,
                                                   requestStatusMap[registration.Id].Value,
                                                   registration.AccountId))
                    {
                        throw new ErrorResponse(
                            400, (int)TrainingCertificateErrorEnum.OVERLAP_INTERVIEW,
                            TrainingCertificateErrorEnum.OVERLAP_INTERVIEW.GetDisplayName());
                    }
                    registration.EventDayId = requestStatusMap[registration.Id].Value;
                    registration.Status = (int)TrainingRegistrationStatusEnum.Assigned;
                    registration.UpdateAt = GetCurrentDatetime();

                    await _unitOfWork.Repository<TrainingRegistration>().UpdateDetached(
                        registration);
                }
                PushNotificationRequest notificationRequest =
                    new PushNotificationRequest()
                    {
                        Ids = filteredList.Select(x => x.AccountId).ToList(),
                        Title = NotificationTypeEnum.Interview_Day.GetDisplayName(),
                        Body = "Your interview is set up ! Check it now!",
                        NotificationsType = (int)NotificationTypeEnum.Interview_Day
                    };
                await _notificationService.PushNotification(notificationRequest);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<dynamic>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Assign success",
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
        private async Task<bool> AssignDuplicateTime(TrainingRegistration request,
                                                     int? eventDayNeedUpdate,
                                                     int accountId)
        {
            var day = await _unitOfWork.Repository<TrainingEventDay>().FindAsync(
                x => x.Id == eventDayNeedUpdate);
            var listOfRegistration =
                await _unitOfWork.Repository<TrainingRegistration>().GetWhere(
                    x => x.AccountId == accountId && x.Id != request.Id &&
                         x.EventDayId.HasValue);
            foreach (var registration in listOfRegistration)
            {
                if (registration.EventDay.Date == day.Date &&
                    IsTimeSpanOverlapPostion(registration.EventDay.TimeFrom,
                                             registration.EventDay.TimeTo, day.TimeFrom,
                                             day.TimeTo))
                {
                    return false;
                }
            }
            return true;
        }

        public async
            Task<BaseResponsePagingViewModel<ViewCollabInterviewClassResponse>>
            GetCollabInClass(int accountId, ViewCollabInterviewClassResponse filter,
                             PagingRequest paging)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().FindAsync(
                    x => x.Id == accountId);
                if (!account.PostPermission)
                {
                    throw new ErrorResponse(401, (int)AccountErrorEnums.API_INVALID,
                                            AccountErrorEnums.API_INVALID.GetDisplayName());
                }
                var list =
                    _unitOfWork.Repository<TrainingEventDay>()
                        .GetAll()
                        .Where(x => !x.TrainingRegistrations.Any(
                                   y => y.Status ==
                                        (int)TrainingRegistrationStatusEnum.Canceled))
                        .ProjectTo<ViewCollabInterviewClassResponse>(
                            _mapper.ConfigurationProvider)
                        .DynamicFilter(filter)
                        .PagingQueryable(paging.Page, paging.PageSize);
                return new BaseResponsePagingViewModel<
                    ViewCollabInterviewClassResponse>()
                {
                    Metadata =
                      new PagingsMetadata()
                      {
                          Page = paging.Page,
                          Size = paging.PageSize,
                          Total = list.Item1
                      },
                    Data = list.Item2.ToList()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<
            BaseResponsePagingViewModel<AdmissionGetCertificateRegistrationResponse>>
        GetCertificateRegistration(int accountId,
                                   AdmissionGetCertificateRegistrationResponse filter,
                                   PagingRequest paging)
        {
            try
            {

                var account = await _unitOfWork.Repository<Account>().FindAsync(
                    x => x.Id == accountId);
                if (!account.PostPermission)
                {
                    throw new ErrorResponse(401, (int)AccountErrorEnums.API_INVALID,
                                            AccountErrorEnums.API_INVALID.GetDisplayName());
                }
                var list =
                    _unitOfWork.Repository<TrainingCertificate>()
                        .GetAll()
                        .Where(x => !x.TrainingRegistrations.Any(
                                   y => y.Status ==
                                        (int)TrainingRegistrationStatusEnum.Canceled))
                        .ProjectTo<AdmissionGetCertificateRegistrationResponse>(
                            _mapper.ConfigurationProvider)
                        .DynamicFilter(filter)
                        .PagingQueryable(paging.Page, paging.PageSize);
                return new BaseResponsePagingViewModel<
                    AdmissionGetCertificateRegistrationResponse>()
                {
                    Metadata =
                      new PagingsMetadata()
                      {
                          Page = paging.Page,
                          Size = paging.PageSize,
                          Total = list.Item1
                      },
                    Data = list.Item2.ToList()
                };

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<dynamic>>
        ReviewInterviewProcess(int accountId, int eventDayId,
                               List<UpdateStatusRegistrationRequest> requests)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().FindAsync(
                    x => x.Id == accountId);
                if (!account.PostPermission)
                {
                    throw new ErrorResponse(401, (int)AccountErrorEnums.API_INVALID,
                                            AccountErrorEnums.API_INVALID.GetDisplayName());
                }
                if (!await UpdateMinAndMax(eventDayId))
                {
                    throw new ErrorResponse(
                        400, (int)TrainingCertificateErrorEnum.CANT_CHECK_ATTEDANCE,
                        TrainingCertificateErrorEnum.CANT_CHECK_ATTEDANCE.GetDisplayName());
                }
                var requestStatusMap = requests.ToDictionary(
                    request => request.TrainingRegistrationId, request => request.Status);
                var listRegistration =
                    await _unitOfWork.Repository<TrainingRegistration>().GetWhere(
                        x => x.EventDayId == eventDayId);
                foreach (var registration in listRegistration)
                {
                    if (requestStatusMap.ContainsKey(registration.Id))
                    {
                        registration.Status = requestStatusMap[registration.Id];
                        registration.ConfirmedAt = GetCurrentDatetime();
                    }
                    await AddCertificateToAccount(registration);
                    await _unitOfWork.Repository<TrainingRegistration>().UpdateDetached(
                        registration);
                }
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<dynamic>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Update status success ",
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
        private async Task AddCertificateToAccount(TrainingRegistration request)
        {
            var check = await _unitOfWork.Repository<AccountCertificate>().FindAsync(
                x => x.AccountId == request.AccountId &&
                     x.Status == (int)AccountCertificateStatusEnum.Complete &&
                     x.TrainingCertificateId == request.TrainingCertificateId);
            switch (request.Status)
            {
                case (int)TrainingRegistrationStatusEnum.Passed:

                    if (check == null)
                    {
                        AccountCertificate accountCertificate = new AccountCertificate
                        {
                            AccountId = request.AccountId,
                            TrainingCertificateId = request.TrainingCertificateId,
                            Status = (int)AccountCertificateStatusEnum.Complete,
                            CreateAt = GetCurrentDatetime()
                        };
                        await _unitOfWork.Repository<AccountCertificate>().InsertAsync(
                            accountCertificate);
                    }
                    break;
                case (int)TrainingRegistrationStatusEnum.Not_Passed:
                    if (check != null)
                    {
                        check.Status = (int)AccountCertificateStatusEnum.Reject;
                        await _unitOfWork.Repository<AccountCertificate>().UpdateDetached(
                            check);
                    }
                    break;
            }
            await _unitOfWork.CommitAsync();
        }
        private async Task<bool> UpdateMinAndMax(int eventDayId)
        {
            var eventDay = await _unitOfWork.Repository<TrainingEventDay>().FindAsync(
                x => x.Id == eventDayId);
            var current = GetCurrentDatetime();
            if (eventDay == null)
            {
                throw new ErrorResponse(
                    400, (int)TrainingCertificateErrorEnum.TRAINING_DAY_DOES_NOT_EXIST,
                    TrainingCertificateErrorEnum.TRAINING_DAY_DOES_NOT_EXIST
                        .GetDisplayName());
            }
            if (eventDay.Date == current.Date &&
                eventDay.TimeFrom <= current.TimeOfDay)
            {
                return true;
            }
            return false;
        }

        public async Task<BaseResponseViewModel<dynamic>>
        CancelCertificateRegistration(int accountId, int certificateRegistrationId)
        {
            try
            {
                var accountBanned =
                    _unitOfWork.Repository<AccountBanned>().GetAll().Where(
                        x => x.AccountIdBanned == accountId && x.IsActive);
                if (accountBanned.Any())
                {
                    var currentDateTime = Ultils.GetCurrentDatetime();

                    var maxDayEnd = accountBanned.Max(x => x.DayEnd);

                    if (maxDayEnd > currentDateTime)
                    {
                        throw new ErrorResponse(
                            400, (int)PostRegistrationErrorEnum.ACCOUNT_BANNED,
                            PostRegistrationErrorEnum.ACCOUNT_BANNED.GetDisplayName());
                    }
                }
                if (certificateRegistrationId == 0)
                {
                    throw new ErrorResponse(
                        400,
                        (int)
                            TrainingCertificateErrorEnum.CERTIFICATE_REGISTRATION_NOT_FOUND,
                        TrainingCertificateErrorEnum.CERTIFICATE_REGISTRATION_NOT_FOUND
                            .GetDisplayName());
                }

                var certificateRegistration =
                    _unitOfWork.Repository<TrainingRegistration>()
                        .GetAll()
                        .FirstOrDefault(x => x.Id == certificateRegistrationId &&
                                             x.AccountId == accountId);

                if (certificateRegistration == null)
                {
                    throw new ErrorResponse(
                        400,
                        (int)
                            TrainingCertificateErrorEnum.CERTIFICATE_REGISTRATION_NOT_FOUND,
                        TrainingCertificateErrorEnum.CERTIFICATE_REGISTRATION_NOT_FOUND
                            .GetDisplayName());
                }

                switch ((TrainingRegistrationStatusEnum)certificateRegistration.Status)
                {
                    case TrainingRegistrationStatusEnum.Canceled:
                        throw new ErrorResponse(
                            400, (int)PostRegistrationErrorEnum.CANCEL_FAILED,
                            PostRegistrationErrorEnum.CANCEL_FAILED.GetDisplayName());
                    default:
                        certificateRegistration.Status =
                            (int)TrainingRegistrationStatusEnum.Canceled;
                        certificateRegistration.UpdateAt = GetCurrentDatetime();
                        await _unitOfWork.Repository<TrainingRegistration>().UpdateDetached(
                            certificateRegistration);
                        await _unitOfWork.CommitAsync();
                        break;
                }
                return new BaseResponseViewModel<dynamic>()
                {
                    Status = new StatusViewModel()
                    {
                        Success = true,
                        Message = "Cancel Successfully",
                        ErrorCode = 200
                    }
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<dynamic>>
        CancelCertificateRegistrationAdmission(int accountId,
                                               int certificateRegistrationId)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().FindAsync(
                    x => x.Id == accountId);
                if (account == null || !account.PostPermission)
                {
                    throw new ErrorResponse(
                        400, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                if (certificateRegistrationId == 0)
                {
                    throw new ErrorResponse(
                        400,
                        (int)
                            TrainingCertificateErrorEnum.CERTIFICATE_REGISTRATION_NOT_FOUND,
                        TrainingCertificateErrorEnum.CERTIFICATE_REGISTRATION_NOT_FOUND
                            .GetDisplayName());
                }

                var certificateRegistration =
                    _unitOfWork.Repository<TrainingRegistration>()
                        .GetAll()
                        .FirstOrDefault(x => x.Id == certificateRegistrationId);

                if (certificateRegistration == null)
                {
                    throw new ErrorResponse(
                        400,
                        (int)
                            TrainingCertificateErrorEnum.CERTIFICATE_REGISTRATION_NOT_FOUND,
                        TrainingCertificateErrorEnum.CERTIFICATE_REGISTRATION_NOT_FOUND
                            .GetDisplayName());
                }

                switch ((TrainingRegistrationStatusEnum)certificateRegistration.Status)
                {
                    case TrainingRegistrationStatusEnum.Canceled:
                        throw new ErrorResponse(
                            400, (int)PostRegistrationErrorEnum.CANCEL_FAILED,
                            PostRegistrationErrorEnum.CANCEL_FAILED.GetDisplayName());
                    default:
                        certificateRegistration.Status =
                            (int)TrainingRegistrationStatusEnum.Canceled;
                        await _unitOfWork.Repository<TrainingRegistration>().UpdateDetached(
                            certificateRegistration);
                        await _unitOfWork.CommitAsync();
                        break;
                }
                return new BaseResponseViewModel<dynamic>()
                {
                    Status = new StatusViewModel()
                    {
                        Success = true,
                        Message = "Cancel Successfully",
                        ErrorCode = 200
                    }
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        public async Task<BaseResponsePagingViewModel<CollabRegistrationsResponse>>
        GetRegistrationByCollabId(int collabId, PagingRequest paging,
                                  FilterStatusRegistrationResponse filter)
        {
            try
            {
                var accountBanned =
                    _unitOfWork.Repository<AccountBanned>().GetAll().Where(
                        x => x.AccountIdBanned == collabId && x.IsActive);
                if (accountBanned.Any())
                {
                    var currentDateTime = Ultils.GetCurrentDatetime();

                    var maxDayEnd = accountBanned.Max(x => x.DayEnd);

                    if (maxDayEnd > currentDateTime)
                    {
                        throw new ErrorResponse(
                            400, (int)PostRegistrationErrorEnum.ACCOUNT_BANNED,
                            PostRegistrationErrorEnum.ACCOUNT_BANNED.GetDisplayName());
                    }
                }
                var list =  _unitOfWork.Repository<TrainingRegistration>()
                               .GetAll().Where(x=>x.AccountId== collabId)
                               .ProjectTo<CollabRegistrationsResponse>(
                                   _mapper.ConfigurationProvider);

                var listAfterFilter = FilterStatusRegistration(list, filter)
                                      .DynamicSort(paging.Sort,paging.Order)
                                      .PagingQueryable(paging.Page, paging.PageSize);
                return new BaseResponsePagingViewModel<CollabRegistrationsResponse>()
                {
                    Metadata =
                      new PagingsMetadata()
                      {
                          Page = paging.Page,
                          Size = paging.PageSize,
                          Total = listAfterFilter.Item1
                      },
                    Data = listAfterFilter.Item2.ToList()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private static IQueryable<CollabRegistrationsResponse>
        FilterStatusRegistration(IQueryable<CollabRegistrationsResponse> list,
                                 FilterStatusRegistrationResponse filter)
        {
            if (filter.Status != null)
            {
                list = list.Where(d => d.Status == filter.Status);
            }
            return list;
        }
    }

}
