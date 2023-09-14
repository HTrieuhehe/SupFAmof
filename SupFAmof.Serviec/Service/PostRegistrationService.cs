﻿using AutoMapper;
using ServiceStack;
using Service.Commons;
using SupFAmof.Data.Entity;
using NTQ.Sdk.Core.Utilities;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.DTO.Request;
using Microsoft.EntityFrameworkCore;
using SupFAmof.Service.DTO.Response;
using AutoMapper.QueryableExtensions;
using System.Runtime.InteropServices;
using static SupFAmof.Service.Helpers.Enum;
using static SupFAmof.Service.Utilities.Ultils;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.ErrorEnum;
using ErrorResponse = SupFAmof.Service.Exceptions.ErrorResponse;

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

        public async Task<BaseResponsePagingViewModel<PostRegistrationResponse>> GetPostRegistrationByAccountId(int accountId, PagingRequest paging)
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
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<PostRegistrationResponse>> AdmssionPostRegistrations(int admissionAccountId, PagingRequest paging)
        {
            try
            {
                var list = _unitOfWork.Repository<PostRegistration>()
                                                      .GetAll()
                                                      .Where(pr => pr.PostRegistrationDetails.First().Post.AccountId == admissionAccountId)
                                                      .ProjectTo<PostRegistrationResponse>(_mapper.ConfigurationProvider)
                                                      .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<PostRegistrationResponse>()
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
            catch(Exception) {
                throw;
            }
           
        }
        public async Task<BaseResponseViewModel<PostRegistrationResponse>> CreatePostRegistration(PostRegistrationRequest request)
        {
            //TO-DO LIst:VALIDATE 1 PERSON CANNOT REGISTER 2 EVENT THE SAME DAY,CHECK IF USER HAS A TRAINING POSITION 
            try
            {
                var postRegistration = _mapper.Map<PostRegistration>(request);
                postRegistration.Status = (int)PostRegistrationStatusEnum.Pending;
                postRegistration.RegistrationCode = GenerateRandomCode();
                postRegistration.CreateAt = GetCurrentTime();

                var postDetails = postRegistration.PostRegistrationDetails.First();
                var positionId = postDetails.PositionId;
                var postId = postDetails.PostId;

                // Fetch the relevant PostPosition and Post
                var postPosition = _unitOfWork.Repository<PostPosition>()
                    .GetAll()
                    .FirstOrDefault(x => x.PostId == postId && x.Id == positionId);

                var post = _unitOfWork.Repository<Post>()
                    .GetAll()
                    .FirstOrDefault(x => x.Id == postId);

                if (postPosition == null)
                {
                    throw new Exception("PostPosition not found."); // Replace with an appropriate exception type and message.
                }

                if (post == null)
                {
                    throw new Exception("Post not found."); // Replace with an appropriate exception type and message.
                }

                // Count the number of registrations for the same position
                var countAllRegistrationForm = _unitOfWork.Repository<PostRegistration>()
                    .GetAll()
                    .Count(x => x.PostRegistrationDetails.Any(d => d.PositionId == positionId) &&
                                x.Status == (int)PostRegistrationStatusEnum.Confirm);

                // Check for duplicate forms
                var checkDuplicateForm = await _unitOfWork.Repository<PostRegistration>()
                    .GetAll()
                    .SingleOrDefaultAsync(x => x.AccountId == postRegistration.AccountId &&
                                                 x.PostRegistrationDetails.Any(d => d.PostId == postId));

                // Check for existing registrations on the same day
                var existingEventDate = await _unitOfWork.Repository<PostRegistration>()
                    .FindAsync(x => x.AccountId == postRegistration.AccountId &&
                                     x.Status == (int)PostRegistrationStatusEnum.Confirm &&
                                     x.PostRegistrationDetails.Any(d => d.Post.DateFrom.Date == post.DateFrom.Date));

                // Continue with your logic

                if (checkDuplicateForm == null && existingEventDate == null)
                {
                    //if (CheckOneDayDifference(postCheckDate.DateFrom, postRegistration.CreateAt, 0) && await CheckPostPositionBus(request))
                    //{
                    //    if (checkPostPostion.Amount - countAllRegistrationForm > 0)
                    //    {

                    //        await _unitOfWork.Repository<PostRegistration>().InsertAsync(postRegistration);
                    //        await _unitOfWork.CommitAsync();
                    //    }
                    //    else
                    //    {
                    //        throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.FULL_SLOT,
                    //                            PostRegistrationErrorEnum.FULL_SLOT.GetDisplayName());
                    //    }

                    //}
                    //else
                    //{
                    //    throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.OUTDATED_REGISTER,
                    //                            PostRegistrationErrorEnum.OUTDATED_REGISTER.GetDisplayName());
                    //}
                    if(post.AccountId == postRegistration.AccountId)
                    {
                        throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.POST_CREATOR,
                           PostRegistrationErrorEnum.POST_CREATOR.GetDisplayName());
                    }
                    if (!CheckOneDayDifference(post.DateFrom, postRegistration.CreateAt, 0))
                    {
                        throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.OUTDATED_REGISTER,
                            PostRegistrationErrorEnum.OUTDATED_REGISTER.GetDisplayName());
                    }

                    if (!await CheckPostPositionBus(postRegistration))
                    {
                        throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.NOT_QUALIFIED_SCHOOLBUS,
                            PostRegistrationErrorEnum.NOT_QUALIFIED_SCHOOLBUS.GetDisplayName());
                    }

                    if (postPosition.Amount - countAllRegistrationForm <= 0)
                    {
                        throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.FULL_SLOT,
                            PostRegistrationErrorEnum.FULL_SLOT.GetDisplayName());
                    }

                    // If both conditions are met, proceed with registration
                    await _unitOfWork.Repository<PostRegistration>().InsertAsync(postRegistration);
                    await _unitOfWork.CommitAsync();

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

        public async Task<BaseResponseViewModel<dynamic>> UpdatePostRegistration(int postRegistrationId, PostRegistrationUpdateRequest request)
        {
            try
            {
                var original = _unitOfWork.Repository<PostRegistration>()
                                           .GetAll()
                                           .SingleOrDefault(x => x.Id == postRegistrationId);
                PostRegistration updateEntity = _mapper.Map<PostRegistration>(request);
                if(updateEntity.SchoolBusOption == null)
                {
                    updateEntity.SchoolBusOption = original.SchoolBusOption;
                }
                if(updateEntity.PostRegistrationDetails.Count() == 0 ) {
                    updateEntity.PostRegistrationDetails = original.PostRegistrationDetails;
                }
                PostPosition checkPostPostion = new PostPosition();
               
                    checkPostPostion = _unitOfWork.Repository<PostPosition>().GetAll().Where(x => x.PostId == updateEntity.PostRegistrationDetails.First().PostId &&
                                                                                                  x.Id == updateEntity.PostRegistrationDetails.First().PositionId).First();

                var CountAllRegistrationForm = _unitOfWork.Repository<PostRegistration>().GetAll().Where(x => x.PostRegistrationDetails.First().PositionId == updateEntity.PostRegistrationDetails.First().PositionId
                                                                                                            && x.Status == (int)PostRegistrationStatusEnum.Confirm).Count();
                if (updateEntity != null && original != null)
                {
                    switch ((PostRegistrationStatusEnum)original.Status)
                    {
                        case PostRegistrationStatusEnum.Pending:
                            if (!await CheckPostPositionBus(updateEntity))
                            {
                                throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.NOT_QUALIFIED_SCHOOLBUS,
                                    PostRegistrationErrorEnum.NOT_QUALIFIED_SCHOOLBUS.GetDisplayName());
                            }
                            if (checkPostPostion.Amount - CountAllRegistrationForm > 0)
                            {
                                original.SchoolBusOption = updateEntity.SchoolBusOption;
                                original.PostRegistrationDetails.First().PositionId = checkPostPostion.Id;
                                original.UpdateAt = updateEntity.CreateAt;
                                await _unitOfWork.Repository<PostRegistration>().UpdateDetached(original);
                                await _unitOfWork.CommitAsync();

                            }
                            else
                            {
                                throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.FULL_SLOT,
                                                        PostRegistrationErrorEnum.FULL_SLOT.GetDisplayName());
                            }
                            break;

                        case PostRegistrationStatusEnum.Confirm:
                            if (!await CheckPostPositionBus(updateEntity))
                            {
                                throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.NOT_QUALIFIED_SCHOOLBUS,
                                    PostRegistrationErrorEnum.NOT_QUALIFIED_SCHOOLBUS.GetDisplayName());
                            }
                            PostRgupdateHistory entityPostTgupdate = new PostRgupdateHistory();

                            if (checkPostPostion.Amount - CountAllRegistrationForm > 0)
                            {

                                PostRgupdateHistory postTgupdate = new PostRgupdateHistory
                                {
                                    PostId = original.PostRegistrationDetails.First().PostId,
                                    PostRegistrationId = original.PostRegistrationDetails.First().PostRegistrationId,
                                    PositionId = checkPostPostion.Id,
                                    BusOption = updateEntity.SchoolBusOption,
                                    CreateAt = updateEntity.CreateAt,
                                    Status = (int)PostRegistrationStatusEnum.Update_Request,

                                };

                                await _unitOfWork.Repository<PostRgupdateHistory>().InsertAsync(postTgupdate);
                                await _unitOfWork.CommitAsync();
                                entityPostTgupdate = postTgupdate;
                            }
                            else
                            {
                                throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.FULL_SLOT,
                                                        PostRegistrationErrorEnum.FULL_SLOT.GetDisplayName());
                            }
                            return new BaseResponseViewModel<dynamic>
                            {
                                Status = new StatusViewModel()
                                {

                                    Message = "Send Request for Admisson",
                                    Success = true,
                                    ErrorCode = 0

                                },
                                Data = _mapper.Map<PostRgupdateHistoryResponse>(_unitOfWork.Repository<PostRgupdateHistory>().GetAll().FirstOrDefault(x => x.Id == entityPostTgupdate.Id))
                            };


                        default:
                            // Handle any default case
                            break;
                    }
                    return new BaseResponseViewModel<dynamic>
                    {
                        Status = new StatusViewModel()
                        {

                            Message = "UPDATE SUCCESS",
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
        public async Task<BaseResponseViewModel<PostRegistrationResponse>> ApproveUpdateRequest(int Id, bool approve)
        {
            try
            {
                var findRequest = await _unitOfWork.Repository<PostRgupdateHistory>().GetAll().SingleOrDefaultAsync(x => x.Id == Id);
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
                                    matchingEntity.UpdateAt = GetCurrentTime();
                                    findRequest.Status = (int)PostRegistrationStatusEnum.Approved_Request;
                                    await _unitOfWork.Repository<PostRegistration>().Update(matchingEntity, matchingEntity.Id);
                                    await _unitOfWork.Repository<PostRgupdateHistory>().UpdateDetached(findRequest);
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
                        await _unitOfWork.Repository<PostRgupdateHistory>().UpdateDetached(findRequest);
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
        private async Task<bool> CheckPostPositionBus(PostRegistration rq)
        {
            var entityMatching = await _unitOfWork.Repository<PostPosition>().GetAll().SingleOrDefaultAsync(x => x.PostId == rq.PostRegistrationDetails.First().PostId
                                                                                            && x.Id == rq.PostRegistrationDetails.First().PositionId);
            if(entityMatching!=null && (entityMatching.IsBusService != rq.SchoolBusOption))
            {
                    return false;
            }

            return true;
        }

    }
   


}
