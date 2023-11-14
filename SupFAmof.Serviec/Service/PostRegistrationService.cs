﻿using AutoMapper;
using System.Linq;
using Service.Commons;
using ServiceStack.Text;
using SupFAmof.Data.Entity;
using LAK.Sdk.Core.Utilities;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.Utilities;
using System.Collections.Generic;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using Microsoft.EntityFrameworkCore;
using SupFAmof.Service.DTO.Response;
using System.Net.NetworkInformation;
using AutoMapper.QueryableExtensions;
using static SupFAmof.Service.Helpers.Enum;
using DocumentFormat.OpenXml.Wordprocessing;
using static SupFAmof.Service.Utilities.Ultils;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.ErrorEnum;

namespace SupFAmof.Service.Service
{
    public class PostRegistrationService : IPostRegistrationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISendMailService sendMailService;

        public PostRegistrationService(IUnitOfWork unitOfWork, IMapper mapper, ISendMailService sendMailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            this.sendMailService = sendMailService;
        }

        public async Task<BaseResponsePagingViewModel<CollabRegistrationResponse>> GetPostRegistrationByAccountId(int accountId, PagingRequest paging, CollabRegistrationResponse filter)
        {
            try
            {
                //var list = _unitOfWork.Repository<PostRegistration>().GetAll()
                //                                  .Where(x => x.AccountId == accountId);
                var postRegistration = _unitOfWork.Repository<PostRegistration>().GetAll()
                                                  .Where(x => x.AccountId == accountId)
                                                  .ProjectTo<CollabRegistrationResponse>(_mapper.ConfigurationProvider)
                                                  .DynamicFilter(filter)
                                                  .DynamicSort(paging.Sort, paging.Order)
                                                  .PagingQueryable(paging.Page, paging.PageSize);

                return new BaseResponsePagingViewModel<CollabRegistrationResponse>()
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

        public async Task<BaseResponsePagingViewModel<AdmissionPostsResponse>> AdmssionPostRegistrations(int admissionAccountId, PagingRequest paging)
        {
            try
            {

                var list = _unitOfWork.Repository<Post>()
                                                      .GetAll()
                                                      .Where(pr => pr.AccountId == admissionAccountId)
                                                      .Include(x => x.PostPositions)
                                                      .ThenInclude(x => x.PostRegistrations)
                                                      .ProjectTo<AdmissionPostsResponse>(_mapper.ConfigurationProvider)
                                                      .PagingQueryable(paging.Page, paging.PageSize);


                return new BaseResponsePagingViewModel<AdmissionPostsResponse>()
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
            catch (Exception)
            {
                throw;
            }

        }
        public async Task<BaseResponseViewModel<CollabRegistrationResponse>> CreatePostRegistration(int accountId, PostRegistrationRequest request)
        {
            //TO-DO LIst:VALIDATE 1 PERSON CANNOT REGISTER 2 EVENT THE SAME DAY,CHECK IF USER HAS A TRAINING POSITION 
            try
            {
                var postRegistration = _mapper.Map<PostRegistration>(request);
                postRegistration.AccountId = accountId;

                var postPosition = await _unitOfWork.Repository<PostPosition>()
                                .FindAsync(x => x.Id == request.PositionId);

                if (postPosition == null)
                {
                    throw new ErrorResponse(404, (int)PostRegistrationErrorEnum.POSITION_NOTFOUND,
                        PostRegistrationErrorEnum.POSITION_NOTFOUND.GetDisplayName());
                }

                var post = await _unitOfWork.Repository<Post>()
                    .FindAsync(x => x.Id == postPosition.PostId);

                if (post == null)
                {
                    throw new ErrorResponse(404, (int)PostErrorEnum.NOT_FOUND_ID,
                        PostErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                // Count the number of registrations for the same position
                var countAllRegistrationForm = _unitOfWork.Repository<PostRegistration>()
                    .GetAll()
                    .Count(x => x.PositionId == request.PositionId &&
                                x.Status == (int)PostRegistrationStatusEnum.Confirm);

                // Check for duplicate forms
                var checkDuplicateForm = await _unitOfWork.Repository<PostRegistration>()
                    .GetAll()
                    .SingleOrDefaultAsync(x => x.AccountId == postRegistration.AccountId &&
                                                  x.PositionId == request.PositionId);

                // Check for existing registrations on the same day
                //var existingEventDate = await _unitOfWork.Repository<PostRegistration>()
                //    .FindAsync(x => x.AccountId == postRegistration.AccountId &&
                //                     x.Status == (int)PostRegistrationStatusEnum.Confirm &&
                //                     x.PostRegistrationDetails.Any(d => d.Post.DateFrom.Date == post.DateFrom.Date));

                // Continue with your logic

                // if (checkDuplicateForm == null && existingEventDate == null)
                if (checkDuplicateForm == null)
                {
                    if (post.AccountId == postRegistration.AccountId)
                    {
                        throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.POST_CREATOR,
                           PostRegistrationErrorEnum.POST_CREATOR.GetDisplayName());
                    }
                    if(post.Status != (int)PostStatusEnum.Opening && post.Status != (int)PostStatusEnum.Re_Open)
                    {
                        throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.POST_NOT_AVAILABLE,
                           PostRegistrationErrorEnum.POST_NOT_AVAILABLE.GetDisplayName());
                    }
                        if (!await CheckPostPositionBus(postRegistration))
                    {
                        throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.NOT_QUALIFIED_SCHOOLBUS,
                            PostRegistrationErrorEnum.NOT_QUALIFIED_SCHOOLBUS.GetDisplayName());
                    }
                    if (!await CheckDateTimePosition(postRegistration))
                    {
                        throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.POSITION_OUTDATED,
                            PostRegistrationErrorEnum.POSITION_OUTDATED.GetDisplayName());
                    }
                    if (!await CheckCertificate(postRegistration))
                    {
                        throw new ErrorResponse(404, (int)PostRegistrationErrorEnum.NOT_FOUND_CERTIFICATE,
                            PostRegistrationErrorEnum.NOT_FOUND_CERTIFICATE.GetDisplayName());
                    }
                    //if (!await CheckDatePost(postRegistration))
                    //{
                    //    throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.POST_OUTDATED,
                    //        PostRegistrationErrorEnum.POST_OUTDATED.GetDisplayName());
                    //}

                    if (!await CheckTimePosition(postRegistration))
                    {
                        throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.DUPLICATE_TIME_POSTION,
                            PostRegistrationErrorEnum.DUPLICATE_TIME_POSTION.GetDisplayName());
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
                return new BaseResponseViewModel<CollabRegistrationResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<CollabRegistrationResponse>(postRegistration)
                };

            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<BaseResponseViewModel<dynamic>> CancelPostregistration(int accountId, int postRegistrationId)
        {
            try
            {
                if (postRegistrationId == 0 || postRegistrationId == null)
                {
                    throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.POST_REGISTRATION_CANNOT_NULL_OR_EMPTY,
                                            PostRegistrationErrorEnum.POST_REGISTRATION_CANNOT_NULL_OR_EMPTY.GetDisplayName());
                }

                var postRegistration = _unitOfWork.Repository<PostRegistration>()
                                                .GetAll()
                                                .FirstOrDefault(x => x.Id == postRegistrationId && x.AccountId == accountId);

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

        public async Task<BaseResponseViewModel<dynamic>> UpdatePostRegistration(int accountId, int postRegistrationId, PostRegistrationUpdateRequest request)
        {
            try
            {

                var original = _unitOfWork.Repository<PostRegistration>()
                                           .GetAll()
                                           .SingleOrDefault(x => x.Id == postRegistrationId && x.AccountId == accountId);
                if (original == null)
                {
                    throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.NOT_FOUND_POST,
                                   PostRegistrationErrorEnum.NOT_FOUND_POST.GetDisplayName());
                }
                if (request.PositionId != null)
                {
                    original.PositionId = request.PositionId;
                }
                if (request.SchoolBusOption.HasValue)
                {
                    original.SchoolBusOption = request.SchoolBusOption;
                }
                PostRegistration updateEntity = original;
                PostPosition checkPostPostion = new PostPosition();

                checkPostPostion = _unitOfWork.Repository<PostPosition>().GetAll().Where(x => x.PostId == updateEntity.Position.PostId &&
                                                                                              x.Id == request.PositionId).First();

                var CountAllRegistrationForm = _unitOfWork.Repository<PostRegistration>().GetAll().Where(x => x.PositionId == request.PositionId
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
                            if (!await CheckCertificate(updateEntity))
                            {
                                throw new ErrorResponse(404, (int)PostRegistrationErrorEnum.NOT_FOUND_CERTIFICATE,
                                    PostRegistrationErrorEnum.NOT_FOUND_CERTIFICATE.GetDisplayName());
                            }
                            if (checkPostPostion.Amount - CountAllRegistrationForm > 0)
                            {
                                original.SchoolBusOption = updateEntity.SchoolBusOption;
                                original.PositionId = request.PositionId;
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
                            if (!await CheckCertificate(updateEntity))
                            {
                                throw new ErrorResponse(404, (int)PostRegistrationErrorEnum.NOT_FOUND_CERTIFICATE,
                                    PostRegistrationErrorEnum.NOT_FOUND_CERTIFICATE.GetDisplayName());
                            }
                            PostRgupdateHistory entityPostTgupdate = new PostRgupdateHistory();

                            if (checkPostPostion.Amount - CountAllRegistrationForm > 0)
                            {

                                PostRgupdateHistory postTgupdate = new PostRgupdateHistory
                                {
                                    PostRegistrationId = updateEntity.Id,
                                    PositionId = updateEntity.PositionId,
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
                            throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.CANT_BE_UPDATED,
                                                     PostRegistrationErrorEnum.CANT_BE_UPDATED.GetDisplayName());
                    }
                    return new BaseResponseViewModel<dynamic>
                    {
                        Status = new StatusViewModel()
                        {

                            Message = "UPDATE SUCCESS",
                            Success = true,
                            ErrorCode = 0

                        },
                        Data = _mapper.Map<CollabRegistrationResponse>(_unitOfWork.Repository<PostRegistration>().GetAll().SingleOrDefault(x => x.Id == postRegistrationId))
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
        public async Task<BaseResponseViewModel<List<PostRegistrationResponse>>> ApproveUpdateRequest(List<int> Ids, bool approve)
        {
            try
            {
                var listResponse = new List<PostRegistration>();
                // Get all entities with matching IDs
                var findRequests = await _unitOfWork.Repository<PostRgupdateHistory>()
                    .GetAll()
                    .Where(x => Ids.Contains(x.Id))
                    .ToListAsync();

                if (Ids.Distinct().Count() != Ids.Count)
                {
                    throw new ErrorResponse(400,
                        (int)PostRegistrationErrorEnum.DUPLICATE_IDS,
                        PostRegistrationErrorEnum.DUPLICATE_IDS.GetDisplayName());
                }

                if (findRequests.Count == 0)
                {
                    throw new ErrorResponse(404,
                        (int)PostRegistrationErrorEnum.NOT_FOUND_UPDATE_REGISTRATION_REQUEST,
                        PostRegistrationErrorEnum.NOT_FOUND_UPDATE_REGISTRATION_REQUEST.GetDisplayName());
                }

                if (findRequests.Count == 0)
                {
                    throw new ErrorResponse(404,
                        (int)PostRegistrationErrorEnum.NOT_FOUND_POST,
                        PostRegistrationErrorEnum.NOT_FOUND_POST.GetDisplayName());
                }

                foreach (var findRequest in findRequests)
                {
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
                            var checkPostPostion = _unitOfWork.Repository<PostPosition>()
                                .GetAll()
                                .Where(x => x.PostId == findRequest.Position.PostId && x.Id == findRequest.PositionId)
                                .First();

                            var countAllRegistrationForm = _unitOfWork.Repository<PostRegistration>()
                                .GetAll()
                                .Where(x => x.PositionId == findRequest.PositionId && x.Status == (int)PostRegistrationStatusEnum.Confirm)
                                .Count();

                            var matchingEntity = _unitOfWork.Repository<PostRegistration>()
                                .GetAll()
                                .FirstOrDefault(x => x.Id == findRequest.PostRegistrationId && x.Position.PostId == findRequest.Position.PostId);

                            var checkMatching = _unitOfWork.Repository<PostRegistration>().GetAll();

                            if (checkMatching.Contains(matchingEntity))
                            {
                                if (checkPostPostion.Amount - countAllRegistrationForm > 0)
                                {
                                    matchingEntity.SchoolBusOption = findRequest.BusOption;
                                    matchingEntity.PositionId = (int)findRequest.PositionId;
                                    matchingEntity.UpdateAt = GetCurrentDatetime();
                                    findRequest.Status = (int)PostRegistrationStatusEnum.Approved_Request;
                                    await _unitOfWork.Repository<PostRegistration>().Update(matchingEntity, matchingEntity.Id);
                                    await _unitOfWork.Repository<PostRgupdateHistory>().UpdateDetached(findRequest);
                                    await UpdatePostAttendeeUser(findRequest);
                                    await _unitOfWork.CommitAsync();
                                    listResponse.Add(matchingEntity);
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
                            break;

                        case false:
                            findRequest.Status = (int)PostRegistrationStatusEnum.Reject;
                            await _unitOfWork.Repository<PostRgupdateHistory>().UpdateDetached(findRequest);
                            await _unitOfWork.CommitAsync();
                            break;

                        default:
                            throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.APPROVE_OR_DISAPPROVE, PostRegistrationErrorEnum.APPROVE_OR_DISAPPROVE.GetDisplayName());
                    }
                }

                // You can return a response for all processed entities if needed
                return new BaseResponseViewModel<List<PostRegistrationResponse>>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = approve ? "Success" : "Denied",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<List<PostRegistrationResponse>>(listResponse)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<bool> CheckPostPositionBus(PostRegistration rq)
        {
            var entityMatching = await _unitOfWork.Repository<PostPosition>().GetAll().SingleOrDefaultAsync(x => x.Id == rq.PositionId);
            switch (entityMatching.IsBusService)
            {
                case true:
                    return true;
                case false:
                    if (rq.SchoolBusOption == false)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
            }

            return true;
        }

        public async Task<BaseResponseViewModel<dynamic>> ApprovePostRegistrationRequest(int accountId, List<int> postRegistrationIds, bool approve)
        {
            try
            {
                var updatedEntities = new List<PostRegistration>();
                var notUpdatedEntities = new List<PostRegistration>();
                var listPr = await _unitOfWork.Repository<PostRegistration>()
                                            .GetAll()
                                            .Where(x => postRegistrationIds.Contains(x.Id) && x.Position.Post.AccountId == accountId)
                                            .ToListAsync();

                if (postRegistrationIds.Distinct().Count() != postRegistrationIds.Count)
                {
                    throw new ErrorResponse(400,
                        (int)PostRegistrationErrorEnum.DUPLICATE_IDS,
                        PostRegistrationErrorEnum.DUPLICATE_IDS.GetDisplayName());
                }

                if (listPr.Count == 0)
                {
                    throw new ErrorResponse(404,
                        (int)PostRegistrationErrorEnum.NOT_FOUND_POST,
                        PostRegistrationErrorEnum.NOT_FOUND_POST.GetDisplayName());
                }

                var checkMatching = _unitOfWork.Repository<PostRegistration>().GetAll();
                var listToUpdate = new List<PostRegistration>();

                foreach (var postRegis in listPr)
                {
                    switch (postRegis.Status)
                    {
                        case (int)PostRegistrationStatusEnum.Confirm:
                            notUpdatedEntities.Add(postRegis);
                            continue;
                        // Add more cases here if needed
                        case (int)PostRegistrationStatusEnum.Reject:
                            notUpdatedEntities.Add(postRegis);
                            continue;

                        default:
                            break;
                    }

                    switch (approve)
                    {
                        case true:
                            var checkPostPosition = _unitOfWork.Repository<PostPosition>()
                              .GetAll()
                              .Where(x => x.PostId == postRegis.Position.PostId && x.Id == postRegis.PositionId)
                              .FirstOrDefault();

                            if (checkPostPosition != null)
                            {

                                var availableSlot = checkPostPosition.Amount - _unitOfWork.Repository<PostRegistration>()
                                    .GetAll()
                                    .Count(x => x.PositionId == postRegis.PositionId && x.Status == (int)PostRegistrationStatusEnum.Confirm);

                                if (availableSlot > 0 && listPr.Count <= availableSlot)
                                {
                                    if (checkMatching.Contains(postRegis))
                                    {
                                        postRegis.Status = (int)PostRegistrationStatusEnum.Confirm;
                                        updatedEntities.Add(postRegis);
                                        listToUpdate.Add(postRegis);
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
                            break;

                        case false:
                            postRegis.Status = (int)PostRegistrationStatusEnum.Reject;
                            listToUpdate.Add(postRegis);
                            break;

                        default:
                            throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.APPROVE_OR_DISAPPROVE, PostRegistrationErrorEnum.APPROVE_OR_DISAPPROVE.GetDisplayName());

                    }
                }

                // Update the range of entities in your repository
                _unitOfWork.Repository<PostRegistration>().UpdateRange(listToUpdate.AsQueryable());
                await AddUserToPostAttendee(listToUpdate);
                await _unitOfWork.CommitAsync();
                await sendMailService.SendEmailBooking(MailEntity(listToUpdate));
                var resultMap = new Dictionary<int, List<PostRegistration>>
        {
            { 1, updatedEntities },
            { 2, notUpdatedEntities }
        };
                var flattenedList = resultMap.Values.SelectMany(x => x).ToList();

                return new BaseResponseViewModel<dynamic>
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Confirm success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<List<PostRegistrationResponse>>(flattenedList)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task AddUserToPostAttendee(List<PostRegistration> list)
        {
            //if (list.Any(postRegistration => postRegistration.Status == (int)PostRegistrationStatusEnum.Reject))
            //{
            //    return;
            //}
            //// Map incoming data to PostAttendeeRequest and PostAttendee
            //var listAttendeeRequest = _mapper.Map<List<PostAttendeeRequest>>(list);
            //var listAttendeeFinal = _mapper.Map<List<PostAttendee>>(listAttendeeRequest);

            //// Retrieve existing data from the database
            //var existingAttendees = _unitOfWork.Repository<PostAttendee>().GetAll();

            //// Identify duplicates by comparing based on some unique identifier (e.g., UserId)
            //var duplicateAttendees = listAttendeeFinal
            //    .Where(newAttendee => existingAttendees.Any(existingAttendee => existingAttendee.AccountId == newAttendee.AccountId && existingAttendee.PositionId == newAttendee.PositionId))
            //    .ToList();
            //// Insert only the non-duplicate attendees
            //var nonDuplicateAttendees = listAttendeeFinal.Except(duplicateAttendees).ToList();
            //await _unitOfWork.Repository<PostAttendee>().InsertRangeAsync(nonDuplicateAttendees.AsQueryable());
        }

        private async Task UpdatePostAttendeeUser(PostRgupdateHistory matching)
        {

            //var orginalPostRegistration = _unitOfWork.Repository<PostRegistration>().GetAll().FirstOrDefault(x => x.Id == matching.PostRegistrationId);
            //var attendNeedToBeUpdated = _unitOfWork.Repository<PostAttendee>()
            //                  .GetAll()
            //                  .FirstOrDefault(x => x.AccountId == orginalPostRegistration.AccountId && x.PostId == orginalPostRegistration.Position.PostId);
            //if (attendNeedToBeUpdated != null)
            //{
            //    attendNeedToBeUpdated.PositionId = matching.PositionId;
            //    attendNeedToBeUpdated.ConfirmAt = GetCurrentDatetime();
            //}
            //await _unitOfWork.Repository<PostAttendee>().Update(attendNeedToBeUpdated, attendNeedToBeUpdated.Id);



        }

        private List<MailBookingRequest> MailEntity(List<PostRegistration> request)
        {
            List<MailBookingRequest> listMail = new List<MailBookingRequest>();
            foreach (PostRegistration postRegistration in request)
            {
                var postPosition = _unitOfWork.Repository<PostPosition>().GetAll().SingleOrDefault(x => x.Id == postRegistration.PositionId);

                MailBookingRequest mailBookingRequest = new MailBookingRequest
                {
                    Email = postRegistration.Account?.Email ?? "N/A",
                    RegistrationCode = postRegistration.RegistrationCode ?? "N/A",
                    PostName = postRegistration.Position.Post.PostCategory.PostCategoryType ?? "N/A",
                    DateFrom = postRegistration.Position.Post.DateFrom.ToString() ?? "N/A",
                    DateTo = postRegistration.Position.Post.DateTo.ToString() ?? "N/A",
                    PositionName = postPosition?.PositionName ?? "N/A",
                    TimeFrom = postPosition?.TimeFrom.ToString() ?? "N/A",
                    TimeTo = postPosition?.TimeTo?.ToString() ?? "N/A",
                    SchoolName = postPosition?.SchoolName ?? "N/A",
                    Location = postPosition?.Location ?? "N/A",
                    Note = postRegistration.Note ?? "N/A"
                };

                listMail.Add(mailBookingRequest);
            }
            return listMail;
        }

        private async Task<bool> CheckCertificate(PostRegistration request)
        {
            var userCertificate = _unitOfWork.Repository<AccountCertificate>()
                .GetAll().Where(x => x.AccountId == request.AccountId).Select(x => x.TrainingCertificateId).ToList() ?? new List<int>();
            var positionCertificate = await _unitOfWork.Repository<PostPosition>()
                                                        .FindAsync(x => x.Id == request.PositionId);
            if (userCertificate.Count() > 0 && positionCertificate.TrainingCertificateId == null)
            {
                return true;
            }
            if (userCertificate.Count() == 0 && positionCertificate.TrainingCertificateId == null)
            {
                return true;
            }
            if (userCertificate.Contains((int)positionCertificate.TrainingCertificateId))
            {
                return true;
            }
            return false;

        }

        private async Task<bool> CheckDatePost(PostRegistration request)
        {
            var postDate = await _unitOfWork.Repository<PostPosition>().FindAsync(x => x.Id == request.PositionId);
            if (postDate.Post.DateTo >= request.CreateAt && postDate.Post.DateFrom.TimeOfDay <= new TimeSpan(17, 0, 0))
            {
                return true;
            }
            return false;
        }
        private async Task<bool> CheckDateTimePosition(PostRegistration request)
        {
            var postDate = await _unitOfWork.Repository<PostPosition>().FindAsync(x => x.Id == request.PositionId);
            var timeLimitation = postDate.Date + postDate.TimeFrom;
            //if date that you register is after the timeLimitation it should return false
            if (timeLimitation >= request.CreateAt)
            {
                return true;
            }
            return false;
        }
        private async Task<bool> CheckTimePosition(PostRegistration request)
        {
            var postsAttended = _unitOfWork.Repository<PostRegistration>()
                                    .GetAll()
                                    .Where(x => x.AccountId == request.AccountId && x.Status == (int)PostRegistrationStatusEnum.Confirm)
                                    .ToList();

            if (postsAttended != null && postsAttended.Any())
            {
                var positionTimeFromPostRegistered = _unitOfWork.Repository<PostPosition>()
                    .GetAll()
                    .Where(x => x.Id == request.PositionId)
                    .FirstOrDefault();

                if (positionTimeFromPostRegistered != null)
                {
                    foreach (var attendedPost in postsAttended)
                    {
                        if (attendedPost.Position.Post.DateFrom.Date == positionTimeFromPostRegistered.Post.DateFrom.Date)
                        {
                            // Use Any() with a lambda expression to check for overlaps
                            if (IsTimeSpanOverlap(attendedPost.Position.TimeFrom, attendedPost.Position.TimeTo,
                                positionTimeFromPostRegistered.TimeFrom, positionTimeFromPostRegistered.TimeTo))
                            {
                                return false; // If there is an overlap, return false
                            }
                        }
                    }
                }
            }

            return true;
        }

        private bool IsTimeSpanOverlap(TimeSpan? start1, TimeSpan? end1, TimeSpan? start2, TimeSpan? end2)
        {
            return (start1 < end2 && end1 > start2);
        }

        public async Task<BaseResponsePagingViewModel<PostRgupdateHistoryResponse>> GetUpdateRequestByAccountId(int accountId, PagingRequest paging)
        {
            try
            {
                var list = _unitOfWork.Repository<PostRgupdateHistory>()
                                                      .GetAll()
                                                      .Where(pr => pr.PostRegistration.AccountId == accountId)
                                                      .ProjectTo<PostRgupdateHistoryResponse>(_mapper.ConfigurationProvider)
                                                      .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<PostRgupdateHistoryResponse>()
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
            catch (Exception ex)

            { throw ex; }
        }

        #region Code của Hải Triều

        public async Task<BaseResponsePagingViewModel<CollabRegistrationResponse>> GetPostRegistrationCheckIn(int accountId, PagingRequest paging)
        {
            try
            {
                //var postRegistration = _unitOfWork.Repository<PostRegistration>().GetAll()
                //                                  .ProjectTo<CollabRegistrationResponse>(_mapper.ConfigurationProvider)
                //                                  .Where(x => x.Status == (int)PostRegistrationStatusEnum.Confirm && x.PostPosition.Date == Ultils.GetCurrentDatetime().Date)
                //                                  .PagingQueryable(paging.Page, paging.PageSize);

                var currentDate = Ultils.GetCurrentDatetime();
                var postRegistration = _unitOfWork.Repository<PostRegistration>().GetAll()
                                                 .Where(x => x.Status == (int)PostRegistrationStatusEnum.Confirm
                                                                    && x.AccountId == accountId
                                                                    && x.Position.Date == currentDate.Date
                                                                    && x.Position.TimeFrom >= currentDate.TimeOfDay
                                                                    && x.Position.TimeFrom <= currentDate.TimeOfDay.Add(TimeSpan.FromHours(1)))
                                                 .ProjectTo<CollabRegistrationResponse>(_mapper.ConfigurationProvider)
                                                 .PagingQueryable(paging.Page, paging.PageSize);


                return new BaseResponsePagingViewModel<CollabRegistrationResponse>()
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
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<PostRegistrationResponse>> GetAccountByPostPositionId
            (int accountId, int positionId, string searchEmail, PostRegistrationResponse filter, PagingRequest paging)
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

                if(!string.IsNullOrEmpty(searchEmail))
                {
                    var searchAccount = _unitOfWork.Repository<PostRegistration>().GetAll()
                                         .Where(x => x.PositionId == positionId && x.Account.Email.Contains(searchEmail))
                                         .ProjectTo<PostRegistrationResponse>(_mapper.ConfigurationProvider)
                                         .DynamicFilter(filter)
                                         .DynamicSort(paging.Sort, paging.Order)
                                         .PagingQueryable(paging.Page, paging.PageSize);


                    return new BaseResponsePagingViewModel<PostRegistrationResponse>()
                    {

                        Metadata = new PagingsMetadata()
                        {
                            Page = paging.Page,
                            Size = paging.PageSize,
                            Total = searchAccount.Item1
                        },
                        Data = searchAccount.Item2.OrderByDescending(x => x.CreateAt).ToList(),
                    };
                }

                var account = _unitOfWork.Repository<PostRegistration>().GetAll()
                                         .Where(x => x.PositionId == positionId)
                                         .ProjectTo<PostRegistrationResponse>(_mapper.ConfigurationProvider)
                                         .DynamicFilter(filter)
                                         .DynamicSort(paging.Sort, paging.Order)
                                         .PagingQueryable(paging.Page, paging.PageSize);


                return new BaseResponsePagingViewModel<PostRegistrationResponse>()
                {

                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = account.Item1
                    },
                    Data = account.Item2.OrderByDescending(x => x.CreateAt).ToList(),
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
        public async Task<BaseResponsePagingViewModel<CollabRegistrationResponse>> FilterPostRegistration
            (int accountId, CollabRegistrationResponse postRegistrationfilter, FilterPostRegistrationResponse filter, PagingRequest paging)
        {
            try
            {
                var postRegistration = _unitOfWork.Repository<PostRegistration>().GetAll()
                                                   .Where(x => x.AccountId == accountId)
                                                   .ProjectTo<CollabRegistrationResponse>(_mapper.ConfigurationProvider)
                                                   .DynamicFilter(postRegistrationfilter)
                                                   .PagingQueryable(paging.Page, paging.PageSize);

                var list = FilterPostRegis(postRegistration.Item2.ToList(), filter);

                return new BaseResponsePagingViewModel<CollabRegistrationResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = list.Keys.First(),
                    },
                    Data = list.Values.First().ToList(),
                };
            }catch(Exception ex) { throw; }
        }
        
        private static Dictionary<int,IQueryable<CollabRegistrationResponse>> FilterPostRegis(List<CollabRegistrationResponse> list, FilterPostRegistrationResponse filter)
        {
            var query = list.AsQueryable();
            if (filter.Status != null && filter.Status.Any())
            {
                query = query.Where(d => filter.Status.Contains((int)d.Status));
            }
            int size = query.Count();
            return new Dictionary<int, IQueryable<CollabRegistrationResponse>>
            {
                { size, query }
            };
        }
    }

}
