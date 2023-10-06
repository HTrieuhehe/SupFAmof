using AutoMapper;
using System.Linq;
using ServiceStack;
using Service.Commons;
using SupFAmof.Data.Entity;
using NTQ.Sdk.Core.Utilities;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.DTO.Request;
using Microsoft.EntityFrameworkCore;
using SupFAmof.Service.DTO.Response;
using AutoMapper.QueryableExtensions;
using System.Runtime.CompilerServices;
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
        private readonly ISendMailService sendMailService;

        public PostRegistrationService(IUnitOfWork unitOfWork, IMapper mapper, ISendMailService sendMailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            this.sendMailService = sendMailService;
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

        public async Task<BaseResponsePagingViewModel<AdmissionPostsResponse>> AdmssionPostRegistrations(int admissionAccountId, PagingRequest paging)
        {
            try
            {
                var list = _unitOfWork.Repository<Post>()
                                                      .GetAll()
                                                      .Where(pr => pr.AccountId == admissionAccountId)
                                                      .ProjectTo<AdmissionPostsResponse>(_mapper.ConfigurationProvider)
                                                      .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);

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
        public async Task<BaseResponseViewModel<PostRegistrationResponse>> CreatePostRegistration(int accountId ,PostRegistrationRequest request)
        {
            //TO-DO LIst:VALIDATE 1 PERSON CANNOT REGISTER 2 EVENT THE SAME DAY,CHECK IF USER HAS A TRAINING POSITION 
            try
            {
                var postRegistration = _mapper.Map<PostRegistration>(request);
                postRegistration.AccountId = accountId;
                postRegistration.PostRegistrationDetails.Add(new PostRegistrationDetail
                {
                    PositionId = request.PositionId,
                    PostId = request.PostId,
                });

                // Fetch the relevant PostPosition and Post
                var postPosition = _unitOfWork.Repository<PostPosition>()
                    .GetAll()
                    .FirstOrDefault(x => x.Id == request.PositionId && x.PostId == request.PostId);

                var post = _unitOfWork.Repository<Post>()
                    .GetAll()
                    .FirstOrDefault(x => x.Id == request.PostId);

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
                    .Count(x => x.PostRegistrationDetails.Any(d => d.PositionId == request.PositionId) &&
                                x.Status == (int)PostRegistrationStatusEnum.Confirm);

                // Check for duplicate forms
                var checkDuplicateForm = await _unitOfWork.Repository<PostRegistration>()
                    .GetAll()
                    .SingleOrDefaultAsync(x => x.AccountId == postRegistration.AccountId &&
                                                 x.PostRegistrationDetails.Any(d => d.PostId == request.PostId&& d.PositionId == request.PositionId));

                // Check for existing registrations on the same day
                //var existingEventDate = await _unitOfWork.Repository<PostRegistration>()
                //    .FindAsync(x => x.AccountId == postRegistration.AccountId &&
                //                     x.Status == (int)PostRegistrationStatusEnum.Confirm &&
                //                     x.PostRegistrationDetails.Any(d => d.Post.DateFrom.Date == post.DateFrom.Date));

                // Continue with your logic

                // if (checkDuplicateForm == null && existingEventDate == null)
                if (checkDuplicateForm == null )
                {
                    if (post.AccountId == postRegistration.AccountId)
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
                    if (!await CheckCertificate(postRegistration))
                    {
                        throw new ErrorResponse(404, (int)PostRegistrationErrorEnum.NOT_FOUND_CERTIFICATE,
                            PostRegistrationErrorEnum.NOT_FOUND_CERTIFICATE.GetDisplayName());
                    }
                    if (!await CheckDatePost(postRegistration))
                    {
                        throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.POST_OUTDATED,
                            PostRegistrationErrorEnum.POST_OUTDATED.GetDisplayName());
                    }
                    
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


        public async Task<BaseResponseViewModel<dynamic>> CancelPostregistration(int accountId, int postRegistrationId)
        {
            try
            {
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
                PostRegistration updateEntity = _mapper.Map<PostRegistration>(request);
                if (updateEntity.SchoolBusOption == null)
                {
                    updateEntity.SchoolBusOption = original.SchoolBusOption;
                }
                if (updateEntity.PostRegistrationDetails.Count() == 0)
                {
                    updateEntity.PostRegistrationDetails = original.PostRegistrationDetails;
                }
                else
                {
                    updateEntity.PostRegistrationDetails.First().PostId = original.PostRegistrationDetails.First().PostId;

                }
                PostPosition checkPostPostion = new PostPosition();

                checkPostPostion = _unitOfWork.Repository<PostPosition>().GetAll().Where(x => x.PostId == updateEntity.PostRegistrationDetails.First().PostId &&
                                                                                              x.Id == request.PositionId).First();

                var CountAllRegistrationForm = _unitOfWork.Repository<PostRegistration>().GetAll().Where(x => x.PostRegistrationDetails.First().PositionId == request.PositionId
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
                                original.PostRegistrationDetails.First().PositionId = request.PositionId;
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
                                .Where(x => x.PostId == findRequest.PostId && x.Id == findRequest.PositionId)
                                .First();

                            var countAllRegistrationForm = _unitOfWork.Repository<PostRegistrationDetail>()
                                .GetAll()
                                .Where(x => x.PositionId == findRequest.PositionId && x.PostRegistration.Status == (int)PostRegistrationStatusEnum.Confirm)
                                .Count();

                            var matchingEntity = _unitOfWork.Repository<PostRegistration>()
                                .GetAll()
                                .FirstOrDefault(x => x.Id == findRequest.PostRegistrationId && x.PostRegistrationDetails.First().PostId == findRequest.PostId);

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
            var entityMatching = await _unitOfWork.Repository<PostPosition>().GetAll().SingleOrDefaultAsync(x => x.PostId == rq.PostRegistrationDetails.First().PostId
                                                                                            && x.Id == rq.PostRegistrationDetails.First().PositionId);
            switch(entityMatching.IsBusService)
            {
                case true:
                    return true;
                case false:
                    if(rq.SchoolBusOption == false)
                    {
                        return true;
                    }else
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
                                            .Where(x => postRegistrationIds.Contains(x.Id) && x.PostRegistrationDetails.First().Post.AccountId == accountId)
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
                              .Where(x => x.PostId == postRegis.PostRegistrationDetails.First().PostId && x.Id == postRegis.PostRegistrationDetails.First().PositionId)
                              .FirstOrDefault();

                            if (checkPostPosition != null)
                            {
                                var availableSlot = checkPostPosition.Amount - _unitOfWork.Repository<PostRegistrationDetail>()
                                    .GetAll()
                                    .Count(x => x.PositionId == postRegis.PostRegistrationDetails.First().PositionId && x.PostRegistration.Status == (int)PostRegistrationStatusEnum.Confirm);

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
            if (list.Any(postRegistration => postRegistration.Status == (int)PostRegistrationStatusEnum.Reject))
            {
                return;
            }
            // Map incoming data to PostAttendeeRequest and PostAttendee
            var listAttendeeRequest = _mapper.Map<List<PostAttendeeRequest>>(list);
            var listAttendeeFinal = _mapper.Map<List<PostAttendee>>(listAttendeeRequest);

            // Retrieve existing data from the database
            var existingAttendees = _unitOfWork.Repository<PostAttendee>().GetAll();

            // Identify duplicates by comparing based on some unique identifier (e.g., UserId)
            var duplicateAttendees = listAttendeeFinal
                .Where(newAttendee => existingAttendees.Any(existingAttendee => existingAttendee.AccountId == newAttendee.AccountId && existingAttendee.PositionId == newAttendee.PositionId))
                .ToList();
            // Insert only the non-duplicate attendees
            var nonDuplicateAttendees = listAttendeeFinal.Except(duplicateAttendees).ToList();
            await _unitOfWork.Repository<PostAttendee>().InsertRangeAsync(nonDuplicateAttendees.AsQueryable());
        }

        private async Task UpdatePostAttendeeUser(PostRgupdateHistory matching)
        {

            var orginalPostRegistration = _unitOfWork.Repository<PostRegistration>().GetAll().FirstOrDefault(x => x.Id == matching.PostRegistrationId);
            var attendNeedToBeUpdated = _unitOfWork.Repository<PostAttendee>()
                              .GetAll()
                              .FirstOrDefault(x => x.AccountId == orginalPostRegistration.AccountId && x.PostId == orginalPostRegistration.PostRegistrationDetails.First().PostId);
            if (attendNeedToBeUpdated != null)
            {
                attendNeedToBeUpdated.PositionId = matching.PositionId;
                attendNeedToBeUpdated.ConfirmAt = GetCurrentTime();
            }
            await _unitOfWork.Repository<PostAttendee>().Update(attendNeedToBeUpdated, attendNeedToBeUpdated.Id);



        }

        private List<MailBookingRequest> MailEntity(List<PostRegistration> request)
        {
            List<MailBookingRequest> listMail = new List<MailBookingRequest>();
            foreach (PostRegistration postRegistration in request)
            {
                var postPosition = _unitOfWork.Repository<PostPosition>().GetAll().SingleOrDefault(x => x.Id == postRegistration.PostRegistrationDetails.First().PositionId);

                MailBookingRequest mailBookingRequest = new MailBookingRequest
                {
                    Email = postRegistration.Account?.Email ?? "N/A",
                    RegistrationCode = postRegistration.RegistrationCode ?? "N/A",
                    PostName = postRegistration.PostRegistrationDetails?.FirstOrDefault(x => x.PostRegistrationId == postRegistration.Id)?.Post?.PostCategory?.PostCategoryType ?? "N/A",
                    DateFrom = postRegistration.PostRegistrationDetails?.FirstOrDefault(x => x.PostRegistrationId == postRegistration.Id)?.Post?.DateFrom.ToString() ?? "N/A",
                    DateTo = postRegistration.PostRegistrationDetails?.FirstOrDefault(x => x.PostRegistrationId == postRegistration.Id)?.Post?.DateTo?.ToString() ?? "N/A",
                    PositionName = postPosition?.PositionName ?? "N/A",
                    TimeFrom = postPosition?.TimeFrom.ToString() ?? "N/A",
                    TimeTo = postPosition?.TimeTo?.ToString() ?? "N/A",
                    SchoolName = postPosition?.SchoolName ?? "N/A",
                    Location = postPosition?.Location ?? "N/A",
                    Note = postRegistration.PostRegistrationDetails?.FirstOrDefault(x => x.PostRegistrationId == postRegistration.Id)?.Note ?? "N/A"
                };

                listMail.Add(mailBookingRequest);
            }
            return listMail;
        }

        private async Task<bool> CheckCertificate(PostRegistration request)
        {
            var userCertificate = _unitOfWork.Repository<AccountCertificate>()                    
                .GetAll().Where(x=>x.AccountId == request.AccountId).Select(x=>x.TraningCertificateId).ToList() ?? new List<int>();
            var positionCertificate = await _unitOfWork.Repository<PostPosition>()
                                                        .FindAsync(x => x.Id == request.PostRegistrationDetails.First().PositionId);
            if (userCertificate.Count() > 0 && positionCertificate.TrainingCertificateId == null)
            {
                return true;
            }
            if (userCertificate.Count() == 0 && positionCertificate.TrainingCertificateId == null)
            {
                return true;
            }
            if(userCertificate.Contains((int)positionCertificate.TrainingCertificateId))
            {
                return true;
            }
            return false;

        }

        private async Task<bool> CheckDatePost (PostRegistration request)
        {
            var postDate = await _unitOfWork.Repository<Post>().FindAsync(x => x.Id == request.PostRegistrationDetails.First().PostId);
            if(postDate.DateFrom > request.CreateAt)
            {
                return true;
            }
            return false;
        }
        private async Task<bool> CheckTimePosition(PostRegistration request)
        {
            var postsAttended = _unitOfWork.Repository<PostAttendee>()
                                    .GetAll()
                                    .Where(x => x.AccountId == request.AccountId)
                                    .ToList();

            if (postsAttended != null && postsAttended.Any())
            {
                var positionTimeFromPostRegistered = _unitOfWork.Repository<PostPosition>()
                    .GetAll()
                    .Where(x => x.Id == request.PostRegistrationDetails.First().PositionId
                             && x.PostId == request.PostRegistrationDetails.First().PostId)
                    .FirstOrDefault();

                if (positionTimeFromPostRegistered != null)
                {
                    foreach (var attendedPost in postsAttended)
                    {
                        if (attendedPost.Post.DateFrom.Date == positionTimeFromPostRegistered.Post.DateFrom.Date)
                        {
                            // Use Any() with a lambda expression to check for overlaps
                            if ( IsTimeSpanOverlap(attendedPost.Position.TimeFrom, attendedPost.Position.TimeTo,
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

    }

}
