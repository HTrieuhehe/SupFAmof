using AutoMapper;
using SupFAmof.Data.Entity;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.Exceptions;
using Microsoft.OpenApi.Extensions;
using SupFAmof.Service.DTO.Request;
using Microsoft.EntityFrameworkCore;
using SupFAmof.Service.DTO.Response;
using Microsoft.Extensions.Configuration;
using static SupFAmof.Service.Helpers.Enum;
using static SupFAmof.Service.Utilities.Ultils;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.ErrorEnum;

namespace SupFAmof.Service.Service
{
    public class PostRegistrationService : PostRegistrationIService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public PostRegistrationService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<BaseResponseViewModel<List<PostRegistrationResponse>>> GetPostRegistrationByAccountId(int accountId)
        {
            List<PostRegistrationResponse> postRegistrationResponses = new List<PostRegistrationResponse>();
            try
            {
                var PostRegistrations = _unitOfWork.Repository<PostRegistration>()
                    .FindAll(x=>x.AccountId == accountId)
                    .Include(x=>x.PostRegistrationDetails);
            if(PostRegistrations.Any()) {
                    postRegistrationResponses = _mapper.Map<List<PostRegistrationResponse>>(PostRegistrations);
                }
            else
                {
                    throw new ErrorResponse(404, 
                        (int)PostRegistrationErrorEnum.NOT_FOUND_POST, 
                        PostRegistrationErrorEnum.NOT_FOUND_POST.GetDisplayName());
                }
                return new BaseResponseViewModel<List<PostRegistrationResponse>>
                {
                    Status = new StatusViewModel
                    {
                        Message = "Success",
                        ErrorCode = 0,
                        Success = true,
                    },
                    Data = postRegistrationResponses

                };
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<PostRegistrationResponse>> CreatePostRegistration(PostRegistrationRequest request)
        {
            try
            {
                var PostRegistration = _mapper.Map<PostRegistration>(request);
                PostRegistration.Status = (int)PostRegistrationStatusEnum.Pending;
                string specific = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                PostRegistration.CreateAt = DateTime.Parse(specific);
                await _unitOfWork.Repository<PostRegistration>().InsertAsync(PostRegistration);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<PostRegistrationResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<PostRegistrationResponse>(PostRegistration)
                };

            }
            catch(Exception ex)
            {
                throw;
            }
        }
       

        public async Task<BaseResponseViewModel<dynamic>> CancelPostregistration(int postRegistrationId)
        {
            try
            {
                var postRegistration = _unitOfWork.Repository<PostRegistration>()
                                                .GetAll()
                                                .FirstOrDefault(x => x.Id == postRegistrationId);
                
                if (postRegistration == null)
                {
                    throw new ErrorResponse(404, (int)PostRegistrationErrorEnum.NOT_FOUND_POST, 
                                            PostRegistrationErrorEnum.NOT_FOUND_POST.GetDisplayName());
                }
                
                postRegistration.Status = (int)PostRegistrationStatusEnum.Cancel;

                await _unitOfWork.Repository<PostRegistration>().UpdateDetached(postRegistration);
                await _unitOfWork.CommitAsync();

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

        public async Task<BaseResponseViewModel<PostRegistrationResponse>> UpdatePostRegistration(int PostRegistrationId , PostRegistrationUpdateRequest request)
        {
            try
            {
                var original = _unitOfWork.Repository<PostRegistration>()
                                           .GetAll()
                                           .SingleOrDefault(x => x.Id == PostRegistrationId);
                PostRegistration updateEntity = _mapper.Map<PostRegistration>(request);
                if (updateEntity != null&&original!=null)
                {
                        original.SchoolBusOption = updateEntity.SchoolBusOption;
                        original.UpdateAt = updateEntity.UpdateAt;
                        original.PostRegistrationDetails.First().PositionId = updateEntity.PostRegistrationDetails.First().PositionId;
                    if (CompareDateTime(original.CreateAt, updateEntity.UpdateAt, TimeSpan.FromHours(2)))
                    {
                        await _unitOfWork.Repository<PostRegistration>().UpdateDetached(original);
                        await _unitOfWork.CommitAsync();
                      
                    }

                    return new BaseResponseViewModel<PostRegistrationResponse>
                    {
                        Status = new StatusViewModel()
                        {

                            Message = "Success",
                            Success = true,
                            ErrorCode = 0

                        },
                        Data = _mapper.Map<PostRegistrationResponse>(_unitOfWork.Repository<PostRegistration>().GetAll().SingleOrDefault(x => x.Id == PostRegistrationId))
                    };
                }
                else
                {
                    throw new ErrorResponse(400,
                        (int)PostRegistrationErrorEnum.UPDATE_FAILED_POST,
                        PostRegistrationErrorEnum.UPDATE_FAILED_POST.GetDisplayName());

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<PostRegistrationResponse>> UpdateRequest(int PotRegistrationRequestId , PostRegistrationUpdateBookingRequest request)
        {
            try
            {
                var original = _unitOfWork.Repository<PostRegistration>()
                                          .GetAll()
                                          .FirstOrDefault(x => x.Id == PotRegistrationRequestId);
                if (original == null || request == null)
                {
                    throw new ErrorResponse(404,
                        (int)PostRegistrationErrorEnum.NOT_FOUND_POST,
                        PostRegistrationErrorEnum.NOT_FOUND_POST.GetDisplayName());
                }
                else if (!CompareDateTime(original.CreateAt, request.CreateAt, TimeSpan.FromHours(2)))
                {
                    throw new ErrorResponse(400,
                       (int)PostRegistrationErrorEnum.EXCEEDING_TIME_LIMIT,
                       PostRegistrationErrorEnum.EXCEEDING_TIME_LIMIT.GetDisplayName());
                }
                else
                {
                    var updateEntity = _mapper.Map<PostRegistration>(request);
                    string specific = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    updateEntity.CreateAt = DateTime.Parse(specific);
                    updateEntity.AccountId = original.AccountId;
                    updateEntity.RegistrationCode = "UPDATE REQUEST";
                    updateEntity.Status = (int)PostRegistrationStatusEnum.Update_Request;
                    updateEntity.PostRegistrationDetails.First().PostId = original.PostRegistrationDetails.First().PostId;
                    await _unitOfWork.Repository<PostRegistration>().InsertAsync(updateEntity);
                    await _unitOfWork.CommitAsync();
                    return new BaseResponseViewModel<PostRegistrationResponse>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "Success",
                            Success = true,
                            ErrorCode = 0
                        },
                        Data = _mapper.Map<PostRegistrationResponse>(updateEntity)
                    };
                }
            }
            catch
            (Exception ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<PostRegistrationResponse>> ApproveUpdateRequest(int Id)
        {
            try
            {
                var FindRequest = await _unitOfWork.Repository<PostRegistration>().GetAll().SingleOrDefaultAsync(x => x.Id == Id);
                if(FindRequest==null)
                {
                    throw new ErrorResponse(404,
                        (int)PostRegistrationErrorEnum.NOT_FOUND_POST,
                        PostRegistrationErrorEnum.NOT_FOUND_POST.GetDisplayName());
                }
                var matchingEntity = _unitOfWork.Repository<PostRegistration>().GetAll().FirstOrDefault(x => x.AccountId == FindRequest.AccountId
                                                                                && x.PostRegistrationDetails.First().PostId == FindRequest.PostRegistrationDetails.First().PostId);
                var CheckMatching = _unitOfWork.Repository<PostRegistration>().GetAll();
                if(CheckMatching.Contains(matchingEntity))
                {
                    matchingEntity.SchoolBusOption = FindRequest.SchoolBusOption;
                    matchingEntity.PostRegistrationDetails.First().PositionId = FindRequest.PostRegistrationDetails.First().PositionId;
                    if (CompareDateTime(matchingEntity.CreateAt, FindRequest.CreateAt, TimeSpan.FromHours(2)))
                    {
                        matchingEntity.UpdateAt = DateTime.Now;
                        await _unitOfWork.Repository<PostRegistration>().Update(matchingEntity,matchingEntity.Id);
                        await _unitOfWork.CommitAsync();

                    }
                 
                }
                return new BaseResponseViewModel<PostRegistrationResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<PostRegistrationResponse>(matchingEntity)
                };
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        
    }


}
