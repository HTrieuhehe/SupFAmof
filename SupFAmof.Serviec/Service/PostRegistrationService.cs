using AutoMapper;
using SupFAmof.Data.Entity;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using Microsoft.EntityFrameworkCore;
using SupFAmof.Service.DTO.Response;
using Microsoft.Extensions.Configuration;
using static SupFAmof.Service.Helpers.Enum;
using static SupFAmof.Service.Utilities.Ultils;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.ErrorEnum;
using AutoMapper.QueryableExtensions;
using NTQ.Sdk.Core.Utilities;
using Service.Commons;
using SupFAmof.Service.DTO.Response.Admission;

namespace SupFAmof.Service.Service
{
    public class PostRegistrationService : IPostRegistrationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PostRegistrationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponsePagingViewModel<PostRegistrationResponse>> GetPostRegistrationByAccountId
            (int accountId, PagingRequest paging)
        {
            try
            {
                var postRegistration = _unitOfWork.Repository<PostRegistration>().GetAll()
                                                  .Where(x => x.AccountId == accountId)
                                                  .ProjectTo<PostRegistrationResponse>(_mapper.ConfigurationProvider)
                                                  .PagingQueryable(paging.Page, paging.PageSize,
                                                    Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<PostRegistrationResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = postRegistration.Item1
                    },
                    Data = postRegistration.Item2.ToList()
                };
            }
            catch(Exception)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<PostRegistrationResponse>> CreatePostRegistration(PostRegistrationRequest request)
        {
            try
            {
                request.RegistrationCode = GenerateRandomCode();
                var postRegistration = _mapper.Map<PostRegistration>(request);
                postRegistration.Status = (int)PostRegistrationStatusEnum.Pending;
                string specific = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                postRegistration.CreateAt = DateTime.Parse(specific);
                var checkPostPostion = _unitOfWork.Repository<PostPosition>().GetAll().Where(x => x.PostId == postRegistration.PostRegistrationDetails.First().PostId &&
                                                                                                x.Id == postRegistration.PostRegistrationDetails.First().PositionId).First();
                var countAllRegistrationForm = _unitOfWork.Repository<PostRegistrationDetail>().GetAll().Where(x => x.PositionId == postRegistration.PostRegistrationDetails.First().PositionId).Count();
                var checkDuplicateForm = await _unitOfWork.Repository<PostRegistration>().FindAsync(x => x.AccountId == postRegistration.AccountId
                                                                            && x.PostRegistrationDetails.First().PostId == postRegistration.PostRegistrationDetails.First().PostId);
                if (checkDuplicateForm == null)
                {
                    if (checkPostPostion.Amount - countAllRegistrationForm > 0)
                    {

                        await _unitOfWork.Repository<PostRegistration>().InsertAsync(postRegistration);
                        await _unitOfWork.CommitAsync();
                    }
                    else
                    {
                        throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.FULL_SLOT,
                                                PostRegistrationErrorEnum.FULL_SLOT.GetDisplayName());
                    }
                }
                else
                {
                    throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.ALREADY_REGISTERED,
                                                PostRegistrationErrorEnum.ALREADY_REGISTERED.GetDisplayName());
                }
                return new BaseResponseViewModel<PostRegistrationResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<PostRegistrationResponse>(postRegistration)
                };

            }
            catch (Exception ex)
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
                throw ;
            }
        }

        public async Task<BaseResponseViewModel<PostRegistrationResponse>> UpdatePostRegistration(int postRegistrationId, PostRegistrationUpdateRequest request)
        {
            try
            {
                var original = _unitOfWork.Repository<PostRegistration>()
                                           .GetAll()
                                           .SingleOrDefault(x => x.Id == postRegistrationId);
                PostRegistration updateEntity = _mapper.Map<PostRegistration>(request);
                var checkPostPostion = _unitOfWork.Repository<PostPosition>().GetAll().Where(x => x.PostId == original.PostRegistrationDetails.First().PostId &&
                                                                                               x.Id == updateEntity.PostRegistrationDetails.First().PositionId).First();
                var CountAllRegistrationForm = _unitOfWork.Repository<PostRegistrationDetail>().GetAll().Where(x => x.PositionId == updateEntity.PostRegistrationDetails.First().PositionId).Count();
                if (updateEntity != null && original != null)
                {
                    switch ((PostRegistrationStatusEnum)original.Status)
                    {
                        case PostRegistrationStatusEnum.Pending:
                            if (checkPostPostion.Amount - CountAllRegistrationForm > 0)
                            {
                                original.SchoolBusOption = updateEntity.SchoolBusOption;
                                original.PostRegistrationDetails.First().PositionId = updateEntity.PostRegistrationDetails.First().PositionId;
                                if (CompareDateTime(original.CreateAt, updateEntity.CreateAt, TimeSpan.FromHours(2)))
                                {
                                    original.UpdateAt = updateEntity.CreateAt;
                                    await _unitOfWork.Repository<PostRegistration>().UpdateDetached(original);
                                    await _unitOfWork.CommitAsync();
                                }
                                else
                                {
                                    throw new ErrorResponse(400,
                                        (int)PostRegistrationErrorEnum.EXCEEDING_TIME_LIMIT,
                                        PostRegistrationErrorEnum.EXCEEDING_TIME_LIMIT.GetDisplayName());
                                }
                            }
                            else
                            {
                                throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.FULL_SLOT,
                                                        PostRegistrationErrorEnum.FULL_SLOT.GetDisplayName());
                            }
                            break;

                        case PostRegistrationStatusEnum.Confirm:
                            if (checkPostPostion.Amount - CountAllRegistrationForm > 0)
                            {
                                if (CompareDateTime(original.CreateAt, updateEntity.CreateAt, TimeSpan.FromHours(2)))
                                {
                                    PostTgupdateHistory postTgupdate = new PostTgupdateHistory
                                    {
                                        PostId = original.PostRegistrationDetails.First().PostId,
                                        PostRegistrationId = original.PostRegistrationDetails.First().PostRegistrationId,
                                        PositionId = updateEntity.PostRegistrationDetails.First().PositionId,
                                        BusOption = updateEntity.SchoolBusOption,
                                        CreateAt = updateEntity.CreateAt,
                                        Status = (int)PostRegistrationStatusEnum.Update_Request,

                                    };
                                    await _unitOfWork.Repository<PostTgupdateHistory>().InsertAsync(postTgupdate);
                                    await _unitOfWork.CommitAsync();
                                }
                                else
                                {
                                    throw new ErrorResponse(400,
                       (int)PostRegistrationErrorEnum.EXCEEDING_TIME_LIMIT,
                       PostRegistrationErrorEnum.EXCEEDING_TIME_LIMIT.GetDisplayName());
                                }
                            }
                            else
                            {
                                throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.FULL_SLOT,
                                                        PostRegistrationErrorEnum.FULL_SLOT.GetDisplayName());
                            }
                            break;


                        default:
                            // Handle any default case
                            break;
                    }
                    return new BaseResponseViewModel<PostRegistrationResponse>
                    {
                        Status = new StatusViewModel()
                        {

                            Message = "Success",
                            Success = true,
                            ErrorCode = 0

                        },
                        Data = _mapper.Map<PostRegistrationResponse>(_unitOfWork.Repository<PostRegistration>().GetAll().SingleOrDefault(x => x.Id == postRegistrationId))
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
        public async Task<BaseResponseViewModel<PostRegistrationResponse>> ApproveUpdateRequest(int id, bool approve)
        {
            try
            {
                var findRequest = await _unitOfWork.Repository<PostTgupdateHistory>().GetAll().SingleOrDefaultAsync(x => x.Id == id);
                if (findRequest == null)
                {
                    throw new ErrorResponse(404,
                        (int)PostRegistrationErrorEnum.NOT_FOUND_POST,
                        PostRegistrationErrorEnum.NOT_FOUND_POST.GetDisplayName());
                }

                switch (findRequest.Status)
                {
                    case (int)PostRegistrationStatusEnum.Approved_Request:
                        throw new ErrorResponse(400,
                            (int)PostRegistrationErrorEnum.ALREADY_APPROVE,
                            PostRegistrationErrorEnum.ALREADY_APPROVE.GetDisplayName());

                    // Add more cases here if needed
                    case (int)PostRegistrationStatusEnum.Reject:
                        throw new ErrorResponse(400,
                            (int)PostRegistrationErrorEnum.ALREADY_REJECT,
                            PostRegistrationErrorEnum.ALREADY_REJECT.GetDisplayName());

                    default:
                        break;
                }

                switch (approve)
                {
                    case true:
                        var checkPostPostion = _unitOfWork.Repository<PostPosition>().GetAll().Where(x => x.PostId == findRequest.PostId &&
                                                                                             x.Id == findRequest.PositionId).First();
                        var countAllRegistrationForm = _unitOfWork.Repository<PostRegistrationDetail>().GetAll().Where(x => x.PositionId == findRequest.PositionId).Count();
                        var matchingEntity = _unitOfWork.Repository<PostRegistration>().GetAll().FirstOrDefault(x => x.Id == findRequest.PostRegistrationId
                                                                                        && x.PostRegistrationDetails.First().PostId == findRequest.PostId);
                        var checkMatching = _unitOfWork.Repository<PostRegistration>().GetAll();

                        if (checkMatching.Contains(matchingEntity))
                        {
                            if (checkPostPostion.Amount - countAllRegistrationForm > 0)
                            {
                                matchingEntity.SchoolBusOption = findRequest.BusOption;
                                matchingEntity.PostRegistrationDetails.First().PositionId = (int)findRequest.PositionId;

                                if (CompareDateTime(matchingEntity.CreateAt, findRequest.CreateAt, TimeSpan.FromHours(2)))
                                {
                                    matchingEntity.UpdateAt = DateTime.Now;
                                    findRequest.Status = (int)PostRegistrationStatusEnum.Approved_Request;
                                    await _unitOfWork.Repository<PostRegistration>().Update(matchingEntity, matchingEntity.Id);
                                    await _unitOfWork.Repository<PostTgupdateHistory>().UpdateDetached(findRequest);
                                    await _unitOfWork.CommitAsync();
                                }
                            }
                            else
                            {
                                throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.FULL_SLOT,
                                                      PostRegistrationErrorEnum.FULL_SLOT.GetDisplayName());
                            }
                        }
                        else
                        {
                            throw new ErrorResponse(404, (int)PostRegistrationErrorEnum.NOT_FOUND_POST,
                                           PostRegistrationErrorEnum.NOT_FOUND_POST.GetDisplayName());
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

                    case false:
                        findRequest.Status = (int)PostRegistrationStatusEnum.Reject;
                        await _unitOfWork.Repository<PostTgupdateHistory>().UpdateDetached(findRequest);
                        await _unitOfWork.CommitAsync();
                        return new BaseResponseViewModel<PostRegistrationResponse>()
                        {
                            Status = new StatusViewModel()
                            {
                                Message = "Denied",
                                Success = true,
                                ErrorCode = 0
                            }
                        };
                    default:
                        throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.APPROVE_OR_DISAPPROVE, PostRegistrationErrorEnum.APPROVE_OR_DISAPPROVE.GetDisplayName());
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }


}
