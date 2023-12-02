using AutoMapper;
using System.Drawing.Text;
using SupFAmof.Data.Entity;
using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Ocsp;
using LAK.Sdk.Core.Utilities;
using NetTopologySuite.Noding;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.Utilities;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using Microsoft.EntityFrameworkCore;
using SupFAmof.Service.DTO.Response;
using AutoMapper.QueryableExtensions;
using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Response.Admission;
using static SupFAmof.Service.Utilities.Ultils;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.ErrorEnum;

namespace SupFAmof.Service.Service
{
    public class TrainingCertificateService : ITrainingCertificateService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public TrainingCertificateService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponsePagingViewModel<TrainingCertificateResponse>> GetTrainingCertificates(TrainingCertificateResponse filter, PagingRequest paging)
        {
            try
            {
                var role = _unitOfWork.Repository<TrainingCertificate>().GetAll()
                                    .ProjectTo<TrainingCertificateResponse>(_mapper.ConfigurationProvider)
                                    .DynamicFilter(filter)
                                    .DynamicSort(paging.Sort, paging.Order)
                                    .Where(x => x.IsActive == true)
                                    .PagingQueryable(paging.Page, paging.PageSize);

                return new BaseResponsePagingViewModel<TrainingCertificateResponse>()
                {
                    Metadata = new PagingsMetadata()
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

        public async Task<BaseResponseViewModel<TrainingCertificateResponse>> GetTrainingCertificateById(int trainingCertificateId)
        {
            try
            {
                var trainingCertificate = await _unitOfWork.Repository<TrainingCertificate>().GetAll()
                                      .FirstOrDefaultAsync(x => x.Id == trainingCertificateId && x.IsActive == true);

                if (trainingCertificate == null)
                {
                    throw new ErrorResponse(404, (int)TrainingCertificateErrorEnum.NOT_FOUND_ID,
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

        public async Task<BaseResponseViewModel<TrainingCertificateResponse>> CreateTrainingCertificate(int accountId, CreateTrainingCertificateRequest request)
        {
            try
            {
                //check account post Permission
                var checkAccount = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);

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

                if (request.TrainingTypeId == null || request.TrainingTypeId == "")
                {
                    throw new ErrorResponse(400, (int)TrainingCertificateErrorEnum.INVALID_TRAINING_CERTIFICATE_TYPE,
                                        TrainingCertificateErrorEnum.INVALID_TRAINING_CERTIFICATE_TYPE.GetDisplayName());
                }

                var tranningCertificate = await _unitOfWork.Repository<TrainingCertificate>()
                                           .FindAsync(x => x.TrainingTypeId.Contains(request.TrainingTypeId) && x.IsActive == true);

                if (tranningCertificate != null)
                {
                    throw new ErrorResponse(400, (int)TrainingCertificateErrorEnum.TRAINING_CERTIFICATE_EXISTED,
                                        TrainingCertificateErrorEnum.TRAINING_CERTIFICATE_EXISTED.GetDisplayName());
                }

                var result = _mapper.Map<CreateTrainingCertificateRequest, TrainingCertificate>(request);

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

        public async Task<BaseResponseViewModel<TrainingCertificateResponse>> UpdateTrainingCertificate(int accountId, int trainingCertificateId, UpdateTrainingCertificateRequest request)
        {
            try
            {
                //check account post Permission
                var checkAccount = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);

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

                if (request.TrainingTypeId == null || request.TrainingTypeId == "")
                {
                    throw new ErrorResponse(400, (int)TrainingCertificateErrorEnum.INVALID_TRAINING_CERTIFICATE_TYPE,
                                        TrainingCertificateErrorEnum.INVALID_TRAINING_CERTIFICATE_TYPE.GetDisplayName());
                }

                var checkCertificate = await _unitOfWork.Repository<TrainingCertificate>()
                                                        .FindAsync(x => x.TrainingTypeId == request.TrainingTypeId.ToUpper() && x.IsActive == true);

                if (checkCertificate == null)
                {
                    throw new ErrorResponse(404, (int)TrainingCertificateErrorEnum.TRAINING_CERTIFICATE_EXISTED,
                                             TrainingCertificateErrorEnum.TRAINING_CERTIFICATE_EXISTED.GetDisplayName());
                }

                var tranningCertificate = await _unitOfWork.Repository<TrainingCertificate>()
                                                        .FindAsync(x => x.Id == trainingCertificateId && x.IsActive == true);

                if (tranningCertificate == null)
                {
                    throw new ErrorResponse(404, (int)TrainingCertificateErrorEnum.NOT_FOUND_ID,
                                             TrainingCertificateErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                var updateTrainingCertificate = _mapper.Map<UpdateTrainingCertificateRequest, TrainingCertificate>(request, tranningCertificate);

                updateTrainingCertificate.TrainingTypeId = updateTrainingCertificate.TrainingTypeId.ToUpper();
                updateTrainingCertificate.UpdateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<TrainingCertificate>().UpdateDetached(updateTrainingCertificate);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<TrainingCertificateResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<TrainingCertificateResponse>(updateTrainingCertificate)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<TrainingCertificateResponse>> SearchTrainingCertificate(string search, PagingRequest paging)
        {
            //Search by Name or Type
            try
            {

                if (search == null || search.Length == 0)
                {
                    throw new ErrorResponse(404, (int)TrainingCertificateErrorEnum.NOT_FOUND_ID,
                                        TrainingCertificateErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                var certificate = _unitOfWork.Repository<TrainingCertificate>().GetAll()
                                    .ProjectTo<TrainingCertificateResponse>(_mapper.ConfigurationProvider)
                                    .Where(x => x.CertificateName.Contains(search) || x.TrainingTypeId.Contains(search.ToUpper()))
                                    .PagingQueryable(paging.Page, paging.PageSize);

                if (!certificate.Item2.Any())
                {
                    throw new ErrorResponse(404, (int)TrainingCertificateErrorEnum.NOT_FOUND_ID,
                                        TrainingCertificateErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                return new BaseResponsePagingViewModel<TrainingCertificateResponse>()
                {
                    Metadata = new PagingsMetadata()
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

        public async Task<BaseResponseViewModel<bool>> DisableTrainingCertificate(int accountId, int trainingCertificateId)
        {
            try
            {
                //check account post Permission
                var checkAccount = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);

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

                var trainingCertificate = await _unitOfWork.Repository<TrainingCertificate>()
                                            .FindAsync(x => x.Id == trainingCertificateId && x.IsActive == true);

                if (trainingCertificate == null)
                {
                    throw new ErrorResponse(404, (int)TrainingCertificateErrorEnum.NOT_FOUND_ID,
                                             TrainingCertificateErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                trainingCertificate.IsActive = false;
                trainingCertificate.UpdateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<TrainingCertificate>().UpdateDetached(trainingCertificate);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<bool>()
                {
                    Status = new StatusViewModel()
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
        public async Task<BaseResponseViewModel<dynamic>> CreateDaysForCertificateInterview(int accountId,EventDaysCertificate request)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);
                if (!account.PostPermission)
                {
                    throw new ErrorResponse(401, (int)AccountErrorEnums.API_INVALID, AccountErrorEnums.API_INVALID.GetDisplayName());
                }
                var eventDay = _mapper.Map<TrainingEventDay>(request);
                if(!await CheckTimeAvailability((DateTime)eventDay.Date,eventDay.TimeFrom,eventDay.TimeTo))
                {
                    throw new ErrorResponse(400, (int)TrainingCertificateErrorEnum.OVERLAP_EVENTS,
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
        public async Task<BaseResponseViewModel<dynamic>> UpdateDaysForCertificateInterview(int accountId,int evenDayId, UpdateDaysCertifcate request)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);
                if (!account.PostPermission)
                {
                    throw new ErrorResponse(401, (int)AccountErrorEnums.API_INVALID, AccountErrorEnums.API_INVALID.GetDisplayName());
                }
                var eventDay = await _unitOfWork.Repository<TrainingEventDay>().FindAsync(x => x.Id == evenDayId);
                if(eventDay==null)
                {
                    throw new ErrorResponse(401, (int)TrainingCertificateErrorEnum.TRAINING_DAY_DOES_NOT_EXIST, TrainingCertificateErrorEnum.TRAINING_DAY_DOES_NOT_EXIST.GetDisplayName());

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
        private async Task<bool> CheckTimeAvailability(DateTime timeOfClass, TimeSpan? timeFrom, TimeSpan? timeTo)
        {
            var positions = await _unitOfWork.Repository<PostPosition>().GetAll().Where(x => x.Date == timeOfClass.Date)
                .OrderByDescending(x=>x.Id)
                .ToListAsync();

            foreach (var position in positions)
            {
                if (IsTimeSpanOverlapPostion(position.TimeFrom, position.TimeTo, timeFrom, timeTo))
                {
                    return false;
                }
            }

            return true;
        }
        private bool IsTimeSpanOverlapPostion(TimeSpan? start1, TimeSpan? end1, TimeSpan? start2, TimeSpan? end2)
        {
            return (start1 < end2 && end1 > start2);
        }


        public async Task<BaseResponseViewModel<dynamic>> TrainingCertificateRegistration(int accountId,TrainingCertificateRegistration request)
        {
            try
            {
                var registration = _mapper.Map<TrainingRegistration>(request);
                registration.AccountId = accountId;
                if(registration==null)
                {
                    throw new ErrorResponse(400,(int)TrainingCertificateErrorEnum.REGISTER_FAILED,TrainingCertificateErrorEnum.REGISTER_FAILED.GetDisplayName());
                }
                if(!await CheckDuplicateTrainingCertificateRegistration(registration))
                    {
                    throw new ErrorResponse(400, (int)TrainingCertificateErrorEnum.DUPLICATE_REGISTRATION, TrainingCertificateErrorEnum.DUPLICATE_REGISTRATION.GetDisplayName());

                }
                await _unitOfWork.Repository<TrainingRegistration>().InsertAsync(registration);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<dynamic>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Create success",
                        Success = true,
                        ErrorCode = 0
                    }
                };
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        private async Task<bool> CheckDuplicateTrainingCertificateRegistration(TrainingRegistration request)
        {
            var duplicates = await _unitOfWork.Repository<TrainingRegistration>().GetWhere(x => x.TrainingCertificateId == request.TrainingCertificateId && x.AccountId == request.AccountId&&x.Status != 3);
            if(duplicates.Any())
            {
                return false;
            }
            return true;
        }

        public async Task<BaseResponseViewModel<dynamic>> AssignDayToRegistration(int accountId, List<AssignEventDayToAccount> requests)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);
                if (!account.PostPermission)
                {
                    throw new ErrorResponse(401, (int)AccountErrorEnums.API_INVALID, AccountErrorEnums.API_INVALID.GetDisplayName());
                }
                var requestStatusMap = requests.ToDictionary(request => request.TrainingRegistrationId, request => request.EventDayId);
                var requestIds = requests.Select(x => x.TrainingRegistrationId);
                var filteredList = _unitOfWork.Repository<TrainingRegistration>()
                      .GetAll().Where(registration => requestIds.Contains(registration.Id) && registration.Status != 3);
                if (!filteredList.Any())
                {
                    throw new ErrorResponse(400, 4001,
                                       "Nothing");
                }
                foreach (var registration in filteredList)
                {

                    if (!await AssignDuplicateTime(registration, requestStatusMap[registration.Id].Value,registration.AccountId))
                    {
                        throw new ErrorResponse(400, (int)TrainingCertificateErrorEnum.OVERLAP_INTERVIEW,
                                                                    TrainingCertificateErrorEnum.OVERLAP_INTERVIEW.GetDisplayName());
                    }
                    registration.EventDayId = requestStatusMap[registration.Id].Value;
                 
                    registration.UpdateAt = GetCurrentDatetime();
                 
                    await _unitOfWork.Repository<TrainingRegistration>().UpdateDetached(registration);
                }
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
        private async Task<bool> AssignDuplicateTime(TrainingRegistration request,int? eventDayNeedUpdate,int accountId)
        {
            var day = await _unitOfWork.Repository<TrainingEventDay>().FindAsync(x => x.Id == eventDayNeedUpdate);
            var listOfRegistration = await _unitOfWork.Repository<TrainingRegistration>().GetWhere(x => x.AccountId == accountId&&x.Id != request.Id);
            if (!listOfRegistration.Any(x=>x.EventDayId.HasValue)) {
                return true;
            }
            foreach(var registration in listOfRegistration)
            {
                if(registration.EventDay.Date == day.Date&& IsTimeSpanOverlapPostion(registration.EventDay.TimeFrom, registration.EventDay.TimeTo,day.TimeFrom,day.TimeTo))
                {
                    return false;
                }
            }
            return true;

        }

        public async Task<BaseResponsePagingViewModel<ViewCollabInterviewClassResponse>> GetCollabInClass(int accountId,int eventDayId,PagingRequest paging)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);
                if (!account.PostPermission)
                {
                    throw new ErrorResponse(401, (int)AccountErrorEnums.API_INVALID, AccountErrorEnums.API_INVALID.GetDisplayName());
                }
                var list = _unitOfWork.Repository<TrainingEventDay>().GetAll()
                           .Where(x => x.Id == eventDayId)
                           .ProjectTo<ViewCollabInterviewClassResponse>(_mapper.ConfigurationProvider)
                           .PagingQueryable(paging.Page, paging.PageSize);
                return new BaseResponsePagingViewModel<ViewCollabInterviewClassResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = list.Item1
                    },
                    Data = list.Item2.ToList()
                };
            }catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<BaseResponsePagingViewModel<AdmissionGetCertificateRegistrationResponse>> GetCertificateRegistration(int accountId,int certificateId,PagingRequest paging)
        {
            try
            {

                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);
                if (!account.PostPermission)
                {
                    throw new ErrorResponse(401, (int)AccountErrorEnums.API_INVALID, AccountErrorEnums.API_INVALID.GetDisplayName());
                }
                var list = _unitOfWork.Repository<TrainingCertificate>().GetAll()
                           .Where(x => x.Id == certificateId)
                           .ProjectTo<AdmissionGetCertificateRegistrationResponse>(_mapper.ConfigurationProvider)
                           .PagingQueryable(paging.Page, paging.PageSize);
                return new BaseResponsePagingViewModel<AdmissionGetCertificateRegistrationResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = list.Item1
                    },
                    Data = list.Item2.ToList()
                };

            }
            catch(Exception ex)
            {
                throw;
            }
        }



        public async Task<BaseResponseViewModel<dynamic>> ReviewInterviewProcess(int accountId,int eventDayId,List<UpdateStatusRegistrationRequest> requests)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);
                if (!account.PostPermission)
                {
                    throw new ErrorResponse(401, (int)AccountErrorEnums.API_INVALID, AccountErrorEnums.API_INVALID.GetDisplayName());
                }
                if (!await UpdateMinAndMax(eventDayId))
                {
                    throw new ErrorResponse(400, (int)TrainingCertificateErrorEnum.CANT_CHECK_ATTEDANCE, TrainingCertificateErrorEnum.CANT_CHECK_ATTEDANCE.GetDisplayName());
                }
                var requestStatusMap = requests.ToDictionary(request => request.TrainingRegistrationId, request => request.Status);
                var listRegistration = await _unitOfWork.Repository<TrainingRegistration>().GetWhere(x => x.EventDayId == eventDayId);
                foreach(var registration in listRegistration )
                {
                    if (requestStatusMap.ContainsKey(registration.Id))
                        {
                        registration.Status = requestStatusMap[registration.Id];
                        registration.UpdateAt = GetCurrentDatetime();
                      
                    }
                    await _unitOfWork.Repository<TrainingRegistration>().UpdateDetached(registration);

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

            } catch (Exception ex)
            {
                throw;
            }
        }
        private async Task<bool> UpdateMinAndMax(int eventDayId)
        {
            var eventDay = await _unitOfWork.Repository<TrainingEventDay>().FindAsync(x => x.Id == eventDayId);
            var current = GetCurrentDatetime();
            if (eventDay == null)
            {
                throw new ErrorResponse(400, (int)TrainingCertificateErrorEnum.TRAINING_DAY_DOES_NOT_EXIST, TrainingCertificateErrorEnum.TRAINING_DAY_DOES_NOT_EXIST.GetDisplayName());
            }
            if(eventDay.Date == current.Date&& eventDay.TimeFrom<= current.TimeOfDay) {
                return true;
             
            }
            return false;
        }
        #endregion
    }
}
