using AutoMapper;
using SupFAmof.Data.Entity;
using LAK.Sdk.Core.Utilities;
using SupFAmof.Data.UnitOfWork;
using System.Linq.Dynamic.Core;
using SupFAmof.Service.Utilities;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using Microsoft.EntityFrameworkCore;
using SupFAmof.Service.DTO.Response;
using AutoMapper.QueryableExtensions;
using static SupFAmof.Service.Helpers.Enum;
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
        private readonly INotificationService _notificationService;

        public PostRegistrationService(IUnitOfWork unitOfWork, IMapper mapper, ISendMailService sendMailService, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            this.sendMailService = sendMailService;
            _notificationService = notificationService;
        }

        public async Task<BaseResponsePagingViewModel<CollabRegistrationUpdateViewResponse>> GetPostRegistrationByAccountId
            (int accountId, PagingRequest paging, CollabRegistrationUpdateViewResponse filter, FilterPostRegistrationResponse statusFilter)
        {
            try
            {
                var accountBanned = _unitOfWork.Repository<AccountBanned>().GetAll()
                                           .Where(x => x.AccountIdBanned == accountId && x.IsActive);
                #region Check Banned

                if (accountBanned.Any())
                {
                    var currentDateTime = Ultils.GetCurrentDatetime();

                    var maxDayEnd = accountBanned.Max(x => x.DayEnd);

                    if (maxDayEnd > currentDateTime)
                    {
                        throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.ACCOUNT_BANNED,
                                                       PostRegistrationErrorEnum.ACCOUNT_BANNED.GetDisplayName());
                    }
                }

                #endregion

                int totalCount = 0;
                int? totalAmountPosition = 0;

                //find registration base on collaborator based on their accountId
                var postRegistration = _unitOfWork.Repository<PostRegistration>().GetAll()
                                                  .Where(x => x.AccountId == accountId)
                                                  .ProjectTo<CollabRegistrationUpdateViewResponse>(_mapper.ConfigurationProvider);
                //filter
                var list = FilterPostRegis(postRegistration, statusFilter)
                                    .DynamicFilter(filter)
                                    .DynamicSort(paging.Sort, paging.Order)
                                    .PagingQueryable(paging.Page, paging.PageSize);

                //convert it into a list
                var postRegistrationResponse = await list.Item2.ToListAsync();


                // Get active position IDs
                var positionIds = await postRegistration
                    .Where(x => x.Status != (int)PostRegistrationStatusEnum.Cancel)
                    .Select(x => x.PositionId)
                    .Distinct()
                    .ToListAsync();

                foreach (var registration in postRegistrationResponse)
                {
                    //tìm ra các position đã đăng ký của bạn í
                    var unregisteredPositions = registration.PostPositionsUnregistereds
                        .Where(x => positionIds.Contains(x.Id))
                        .ToList();

                    var positionIdsForCount = registration.PostPositionsUnregistereds.Select(x => x.Id).ToList();

                    // tìm post Registration có position Id trung với các bài post
                    var postRegistrations = await _unitOfWork.Repository<PostRegistration>()
                                                        .GetAll()
                                                        .Where(reg => positionIdsForCount.Contains(reg.PositionId) && reg.Status == (int)PostRegistrationStatusEnum.Confirm)
                                                        .ToListAsync();

                    // tính tổng các registration đã được confirm
                    registration.Post.RegisterAmount = postRegistrations.Count;

                    foreach (var unregisteredPosition in unregisteredPositions)
                    {
                        registration.PostPositionsUnregistereds.Remove(unregisteredPosition);
                    }

                    #region Count Registration Amount

                    foreach (var postPosition in registration.PostPositionsUnregistereds)
                    {
                        //count register amount in post attendee based on position
                        totalCount += CountRegisterAmount(postPosition.Id, postRegistrations);

                        //transafer data to field in post position
                        postPosition.PositionRegisterAmount = totalCount;

                        //add number of amount required to total amount of a specific post
                        totalAmountPosition += postPosition.Amount;

                        //reset temp count
                        totalCount = 0;
                    }

                    //transfer data from position after add to field in post
                    registration.Post.TotalAmountPosition = totalAmountPosition;

                    // Reset temp variable
                    totalAmountPosition = 0;
                   
                    #endregion
                }

                return new BaseResponsePagingViewModel<CollabRegistrationUpdateViewResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = list.Item1,
                    },
                    Data = postRegistrationResponse,
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

        public async Task<BaseResponsePagingViewModel<AdmissionUpdateRequestResponse>> AdmissionUpdateRequests(int admissionAccountId, PagingRequest paging,FilterUpdateRequestResponse IdFilter)
        {
            try
            {

                var list = _unitOfWork.Repository<PostRgupdateHistory>()
                                                      .GetAll()
                                                      .Where(pr => pr.Position.Post.AccountId == admissionAccountId)
                                                      .ProjectTo<AdmissionUpdateRequestResponse>(_mapper.ConfigurationProvider);


                var filteredList = FilterUpdateRequest(list, IdFilter)
                                 .DynamicSort(paging.Sort, paging.Order)
                                 .PagingQueryable(paging.Page, paging.PageSize);
                return new BaseResponsePagingViewModel<AdmissionUpdateRequestResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = filteredList.Item1
                    },
                    Data = filteredList.Item2.ToList()
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
                var accountBanned = _unitOfWork.Repository<AccountBanned>().GetAll()
                                              .Where(x => x.AccountIdBanned == accountId&& x.IsActive);
                if (accountBanned.Any())
                {
                    var currentDateTime = Ultils.GetCurrentDatetime();

                    var maxDayEnd = accountBanned.Max(x => x.DayEnd);

                    if (maxDayEnd > currentDateTime)
                    {
                        throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.ACCOUNT_BANNED,
                                                       PostRegistrationErrorEnum.ACCOUNT_BANNED.GetDisplayName());
                    }
                }
                var postRegistration = _mapper.Map<PostRegistration>(request);
                postRegistration.AccountId = accountId;

                var postPosition = await _unitOfWork.Repository<PostPosition>()
                                .FindAsync(x => x.Id == request.PositionId);
                postRegistration.Salary = postPosition.Salary;
                if (postPosition == null)
                {
                    throw new ErrorResponse(404, (int)PostRegistrationErrorEnum.POSITION_NOTFOUND,
                        PostRegistrationErrorEnum.POSITION_NOTFOUND.GetDisplayName());
                }
                if(!await CheckPendingDuplicateTimePosition(request,accountId))
                {
                    throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.REQUEST_FAILED,
                       PostRegistrationErrorEnum.REQUEST_FAILED.GetDisplayName());
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
                                                  x.PositionId == request.PositionId && x.Status == (int)PostRegistrationStatusEnum.Confirm);

                if (checkDuplicateForm == null)
                {
                    if (post.AccountId == postRegistration.AccountId)
                    {
                        throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.POST_CREATOR,
                           PostRegistrationErrorEnum.POST_CREATOR.GetDisplayName());
                    }
                    if (post.Status != (int)PostStatusEnum.Opening && post.Status != (int)PostStatusEnum.Re_Open)
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
                        throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.NOT_FOUND_CERTIFICATE,
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
                var accountBanned = _unitOfWork.Repository<AccountBanned>().GetAll()
                                             .Where(x => x.AccountIdBanned == accountId && x.IsActive);
                if (accountBanned.Any())
                {
                    var currentDateTime = Ultils.GetCurrentDatetime();

                    var maxDayEnd = accountBanned.Max(x => x.DayEnd);

                    if (maxDayEnd > currentDateTime)
                    {
                        throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.ACCOUNT_BANNED,
                                                       PostRegistrationErrorEnum.ACCOUNT_BANNED.GetDisplayName());
                    }
                }
                if (postRegistrationId == 0)
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

                switch ((PostRegistrationStatusEnum)postRegistration.Status)
                {
                    case PostRegistrationStatusEnum.Pending:
                        postRegistration.Status = (int)PostRegistrationStatusEnum.Cancel;
                        postRegistration.UpdateAt = Ultils.GetCurrentDatetime();
                        await _unitOfWork.Repository<PostRegistration>().UpdateDetached(postRegistration);
                        await _unitOfWork.CommitAsync();
                        break;
                    case PostRegistrationStatusEnum.Confirm:
                        DateTime combinedDateTime = postRegistration.Position.Date.Add(postRegistration.Position.TimeFrom);
                        DateTime currentTime = GetCurrentDatetime();
                        var timeDifference = combinedDateTime - currentTime;
                        if (timeDifference.TotalHours < 6)
                        {
                            throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.CANCEL_FAILED,
                      PostRegistrationErrorEnum.CANCEL_FAILED.GetDisplayName());
                        }
                        CreateAccountApplicationRequest application = new CreateAccountApplicationRequest
                        {
                            ProblemNote = $"Request for cancellation for Position {postRegistration.Position.PositionName} from Post {postRegistration.Position.Post.PostCode}",
                        };
                        var report = _mapper.Map<CreateAccountApplicationRequest, Application>(application);
                        report.AccountId = accountId;
                        report.ReportDate = Ultils.GetCurrentDatetime();
                        report.Status = (int)ReportProblemStatusEnum.Pending;
                        await _unitOfWork.Repository<Application>().InsertAsync(report);
                        await _unitOfWork.CommitAsync();
                        return new BaseResponseViewModel<dynamic>()
                        {
                            Status = new StatusViewModel()
                            {
                                Success = true,
                                Message = "Send Application to Admission",
                                ErrorCode = 200
                            }
                            ,
                            Data = report
                        };
                    default:
                        throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.CANCEL_FAILED,
                        PostRegistrationErrorEnum.CANCEL_FAILED.GetDisplayName());

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

        public async Task<BaseResponseViewModel<dynamic>> UpdatePostRegistration(int accountId, PostRegistrationUpdateRequest request)
        {
            try
            {
                var accountBanned = _unitOfWork.Repository<AccountBanned>().GetAll()
                                            .Where(x => x.AccountIdBanned == accountId && x.IsActive);
                if (accountBanned.Any())
                {
                    var currentDateTime = Ultils.GetCurrentDatetime();

                    var maxDayEnd = accountBanned.Max(x => x.DayEnd);

                    if (maxDayEnd > currentDateTime)
                    {
                        throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.ACCOUNT_BANNED,
                                                       PostRegistrationErrorEnum.ACCOUNT_BANNED.GetDisplayName());
                    }
                }
                var original = _unitOfWork.Repository<PostRegistration>()
                                           .GetAll()
                                           .SingleOrDefault(x => x.Id == request.Id && x.AccountId == accountId);
                if (!await CheckUpdatePosition(request))
                {
                    throw new ErrorResponse(404, (int)PostRegistrationErrorEnum.WRONG_POSITION,
                        PostRegistrationErrorEnum.WRONG_POSITION.GetDisplayName());
                }

                if (original == null)
                {
                    throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.NOT_FOUND_POST,
                                   PostRegistrationErrorEnum.NOT_FOUND_POST.GetDisplayName());
                }
                PostRegistration updateEntity = original;
                var checkPostPostion = _unitOfWork.Repository<PostPosition>().GetAll().Where(x => x.PostId == updateEntity.Position.PostId &&
                                                                                              x.Id == request.PositionId).First();

                var CountAllRegistrationForm = _unitOfWork.Repository<PostRegistration>().GetAll().Where(x => x.PositionId == request.PositionId
                                                                                                            && x.Status == (int)PostRegistrationStatusEnum.Confirm).Count();
                if (updateEntity != null)
                {
                    switch ((PostRegistrationStatusEnum)original.Status)
                    {
                        case PostRegistrationStatusEnum.Pending:
                            if (checkPostPostion.Amount - CountAllRegistrationForm > 0)
                            {
                                updateEntity.PositionId = request.PositionId;
                                updateEntity.SchoolBusOption = request.SchoolBusOption;
                                updateEntity.UpdateAt = GetCurrentDatetime();
                                if (!await CheckDuplicateUpdate(updateEntity))
                                {
                                    throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.UPDATE_FAILED,
                                                   PostRegistrationErrorEnum.UPDATE_FAILED.GetDisplayName());
                                }
                                if (!await CheckCertificate(updateEntity))
                                {
                                    throw new ErrorResponse(404, (int)PostRegistrationErrorEnum.NOT_FOUND_CERTIFICATE,
                                        PostRegistrationErrorEnum.NOT_FOUND_CERTIFICATE.GetDisplayName());
                                }
                                if (!await CheckPostPositionBus(updateEntity))
                                {
                                    throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.NOT_QUALIFIED_SCHOOLBUS,
                                                   PostRegistrationErrorEnum.NOT_QUALIFIED_SCHOOLBUS.GetDisplayName());
                                }
                                if (!await CheckTimePositionUpdate(updateEntity.PositionId,accountId))
                                {
                                    throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.DUPLICATE_TIME_POSTION,
                                        PostRegistrationErrorEnum.DUPLICATE_TIME_POSTION.GetDisplayName());
                                }
                                await _unitOfWork.Repository<PostRegistration>().UpdateDetached(updateEntity);
                                await _unitOfWork.CommitAsync();

                            }
                            else
                            {
                                throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.FULL_SLOT,
                                                        PostRegistrationErrorEnum.FULL_SLOT.GetDisplayName());
                            }
                            break;

                        case PostRegistrationStatusEnum.Confirm:
                            PostRgupdateHistory entityPostTgupdate = new PostRgupdateHistory();

                            if (checkPostPostion.Amount - CountAllRegistrationForm > 0)
                            {

                                PostRgupdateHistory postTgupdate = new PostRgupdateHistory
                                {
                                    OriginalPositionId = original.PositionId,
                                    PostRegistrationId = request.Id,
                                    PositionId = request.PositionId,
                                    BusOption = request.SchoolBusOption,
                                    Note = request.Note,
                                    CreateAt = GetCurrentDatetime(),
                                    Status = (int)PostRGUpdateHistoryEnum.Pending,

                                };
                                if (!CheckConfirmPostRegistration(postTgupdate, accountId))
                                {
                                    throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.UPDATE_FAILED,
                                                   PostRegistrationErrorEnum.UPDATE_FAILED.GetDisplayName());
                                }
                                if (!CheckDuplicatePostRgUpdateSendPending(postTgupdate, accountId))
                                {
                                    throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.DUPLICATE_PENDING,
                                                   PostRegistrationErrorEnum.DUPLICATE_PENDING.GetDisplayName());
                                }
                                if (!await CheckCertificateConfirmUpdate((int)postTgupdate.PositionId,accountId))
                                {
                                    throw new ErrorResponse(404, (int)PostRegistrationErrorEnum.NOT_FOUND_CERTIFICATE,
                                        PostRegistrationErrorEnum.NOT_FOUND_CERTIFICATE.GetDisplayName());
                                }
                                if (!await CheckTimePositionUpdate((int)postTgupdate.PositionId,accountId))
                                {
                                    throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.DUPLICATE_TIME_POSTION,
                                        PostRegistrationErrorEnum.DUPLICATE_TIME_POSTION.GetDisplayName());
                                }
                                if (!await CheckPostPositionBusConfirmUpdate((int)postTgupdate.PositionId, (bool)postTgupdate.BusOption))
                                {
                                    throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.NOT_QUALIFIED_SCHOOLBUS,
                                                   PostRegistrationErrorEnum.NOT_QUALIFIED_SCHOOLBUS.GetDisplayName());
                                }
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
                        Data = _mapper.Map<CollabRegistrationResponse>(_unitOfWork.Repository<PostRegistration>().GetAll().SingleOrDefault(x => x.Id == request.Id))
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
                        case (int)PostRGUpdateHistoryEnum.Approved:
                            throw new ErrorResponse(400,
                                (int)PostRegistrationErrorEnum.ALREADY_APPROVE,
                                PostRegistrationErrorEnum.ALREADY_APPROVE.GetDisplayName());

                        // Add more cases here if needed
                        case (int)PostRGUpdateHistoryEnum.Rejected:
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
                                    findRequest.Status = (int)PostRGUpdateHistoryEnum.Approved;
                                    if (!await CheckDuplicateUpdate(matchingEntity))
                                    {
                                        throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.UPDATE_FAILED,
                                                       PostRegistrationErrorEnum.UPDATE_FAILED.GetDisplayName());
                                    }
                                    await _unitOfWork.Repository<PostRegistration>().Update(matchingEntity, matchingEntity.Id);
                                    await _unitOfWork.Repository<PostRgupdateHistory>().UpdateDetached(findRequest);
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
                            findRequest.Status = (int)PostRGUpdateHistoryEnum.Rejected;
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
        private async Task<bool> CheckPostPositionBusConfirmUpdate(int positionId,bool SchoolsBusOptions)
        {
            var entityMatching = await _unitOfWork.Repository<PostPosition>().GetAll().SingleOrDefaultAsync(x => x.Id == positionId);
            switch (entityMatching.IsBusService)
            {
                case true:
                    return true;
                case false:
                    if (!SchoolsBusOptions)
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

                var accountIds = listPr.Select(x => x.AccountId).ToList();

                //create notification request 
                PushNotificationRequest notificationRequest = new PushNotificationRequest()
                {
                    Ids = accountIds,
                    Title = NotificationTypeEnum.PostRegistration_Confirm.GetDisplayName(),
                    Body = "Your Post Registration is confirmed! Check it now!",
                    NotificationsType = (int)NotificationTypeEnum.PostRegistration_Confirm
                };

                await _notificationService.PushNotification(notificationRequest);

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
                    Note = postRegistration.Note ?? "N/A",
                    Link = postPosition?.Document?.DocUrl ?? "N/A"
                };

                listMail.Add(mailBookingRequest);
            }
            return listMail;
        }

        private async Task<bool> CheckCertificate(PostRegistration request)
        {
            var userCertificate = _unitOfWork.Repository<AccountCertificate>()
                .GetAll().Where(x => x.AccountId == request.AccountId &&x.Status == (int)AccountCertificateStatusEnum.Complete).Select(x => x.TrainingCertificateId).ToList() ?? new List<int>();
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
        private async Task<bool> CheckCertificateConfirmUpdate(int positionId,int accountId)
        {
            var userCertificate = _unitOfWork.Repository<AccountCertificate>()
                .GetAll().Where(x => x.AccountId == accountId && x.Status == (int)AccountCertificateStatusEnum.Complete).Select(x => x.TrainingCertificateId).ToList() ?? new List<int>();
            var positionCertificate = await _unitOfWork.Repository<PostPosition>()
                                                        .FindAsync(x => x.Id == positionId);
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
                        if (attendedPost.Position.Date.Date == positionTimeFromPostRegistered.Date.Date)
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
        private async Task<bool> CheckTimePositionUpdate(int positionId,int accountId)
        {
            var postsAttended = _unitOfWork.Repository<PostRegistration>()
                                    .GetAll()
                                    .Where(x => x.AccountId == accountId && x.Status == (int)PostRegistrationStatusEnum.Confirm)
                                    .ToList();

            if (postsAttended != null && postsAttended.Any())
            {
                var positionTimeFromPostRegistered = _unitOfWork.Repository<PostPosition>()
                    .GetAll()
                    .Where(x => x.Id == positionId)
                    .FirstOrDefault();

                if (positionTimeFromPostRegistered != null)
                {
                    foreach (var attendedPost in postsAttended)
                    {
                        if (attendedPost.Position.Date.Date == positionTimeFromPostRegistered.Date.Date)
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

        //private async Task<bool> CheckDuplicatePostRgUpdate(PostRgupdateHistory request)
        //{
        //    var duplicate = await _unitOfWork.Repository<PostRgupdateHistory>().FindAsync(x => x.PostRegistrationId == request.PostRegistrationId && x.PositionId == request.PositionId);
        //    if (duplicate != null)
        //    {
        //        return false;
        //    }
        //    return true;
        //}
        //private async Task<bool> CheckDuplicatePostRgUpdateSend(PostRgupdateHistory request, int accountId)
        //{
        //    var duplicate = await _unitOfWork.Repository<PostRegistration>().FindAsync(x => x.AccountId == accountId
        //                                                                                 && x.PositionId == request.PositionId);
        //    if (duplicate != null)
        //    {
        //        return false;
        //    }
        //    return true;
        //}
        private bool CheckDuplicatePostRgUpdateSendPending(PostRgupdateHistory request, int accountId)
        {
            var duplicate = _unitOfWork.Repository<PostRgupdateHistory>().GetAll().Where(x => x.PostRegistration.AccountId == accountId
                                                                                         && x.PostRegistrationId == request.PostRegistrationId
                                                                                         && x.Status == (int)PostRegistrationStatusEnum.Pending);
            if (duplicate.Any())
            {
                return false;
            }
            return true;
        }
        private bool CheckConfirmPostRegistration(PostRgupdateHistory request, int accountId)
        {
            var duplicate = _unitOfWork.Repository<PostRegistration>().GetAll().Where(x => x.AccountId == accountId
                                                                                         && x.PositionId == request.PositionId
                                                                                         && x.Status == (int)PostRegistrationStatusEnum.Confirm);
            if (duplicate.Any())
            {
                return false;
            }
            return true;
        }

        private bool IsTimeSpanOverlap(TimeSpan? start1, TimeSpan? end1, TimeSpan? start2, TimeSpan? end2)
        {
            return (start1 < end2 && end1 > start2);
        }

        public async Task<BaseResponsePagingViewModel<PostRgupdateHistoryResponse>> GetUpdateRequestByAccountId(int accountId, PostRgupdateHistoryResponse filter, PagingRequest paging)
        {
            try
            {
                var accountBanned = _unitOfWork.Repository<AccountBanned>().GetAll()
                                              .Where(x => x.AccountIdBanned == accountId && x.IsActive);
                if (accountBanned.Any())
                {
                    var currentDateTime = Ultils.GetCurrentDatetime();

                    var maxDayEnd = accountBanned.Max(x => x.DayEnd);

                    if (maxDayEnd > currentDateTime)
                    {
                        throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.ACCOUNT_BANNED,
                                                       PostRegistrationErrorEnum.ACCOUNT_BANNED.GetDisplayName());
                    }
                }

                var list = _unitOfWork.Repository<PostRgupdateHistory>()
                                                      .GetAll()
                                                      .Where(pr => pr.PostRegistration.AccountId == accountId)
                                                      .ProjectTo<PostRgupdateHistoryResponse>(_mapper.ConfigurationProvider)
                                                      .DynamicFilter(filter)
                                                      .DynamicSort(paging.Sort, paging.Order)
                                                      .PagingQueryable(paging.Page, paging.PageSize);

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
                var accountBanned = _unitOfWork.Repository<AccountBanned>().GetAll()
                                            .Where(x => x.AccountIdBanned == accountId && x.IsActive);
                if (accountBanned.Any())
                {
                    var currentDateTime = Ultils.GetCurrentDatetime();

                    var maxDayEnd = accountBanned.Max(x => x.DayEnd);

                    if (maxDayEnd > currentDateTime)
                    {
                        throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.ACCOUNT_BANNED,
                                                       PostRegistrationErrorEnum.ACCOUNT_BANNED.GetDisplayName());
                    }
                }
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

                if (!string.IsNullOrEmpty(searchEmail))
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

        private static IQueryable<CollabRegistrationUpdateViewResponse> FilterPostRegis(IQueryable<CollabRegistrationUpdateViewResponse> list, FilterPostRegistrationResponse filter)
        {
            if (filter.RegistrationStatus != null && filter.RegistrationStatus.Any())
            {
                list = list.Where(d => filter.RegistrationStatus.Contains((int)d.Status));
            }
            return list;
        }
        private static IQueryable<AdmissionUpdateRequestResponse> FilterUpdateRequest(IQueryable<AdmissionUpdateRequestResponse> list, FilterUpdateRequestResponse filter)
        {
            if (filter.Id != null)
            {
                list = list.Where(d => d.Post.Id == filter.Id);
            }
            return list;
        }

        private async Task<bool> CheckUpdatePosition(PostRegistrationUpdateRequest request)
        {
            var currentPosition = await _unitOfWork.Repository<PostRegistration>().FindAsync(x => x.Id == request.Id);
            var positions = _unitOfWork.Repository<PostPosition>().GetAll().Where(x => x.PostId == currentPosition.Position.PostId);
            var positionCanbeUpdate = positions.Where(x => x.Id != currentPosition.PositionId);
            if (positionCanbeUpdate.Any(x => x.Id == request.PositionId))
            {
                return true;
            }
            return false;
        }
        public async Task<BaseResponseViewModel<dynamic>> UpdateSchoolBus(int accountId, UpdateSchoolBusRequest request)
        {
            try
            {
                var accountBanned = _unitOfWork.Repository<AccountBanned>().GetAll()
                                              .Where(x => x.AccountIdBanned == accountId && x.IsActive);
                if (accountBanned.Any())
                {
                    var currentDateTime = Ultils.GetCurrentDatetime();

                    var maxDayEnd = accountBanned.Max(x => x.DayEnd);

                    if (maxDayEnd > currentDateTime)
                    {
                        throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.ACCOUNT_BANNED,
                                                       PostRegistrationErrorEnum.ACCOUNT_BANNED.GetDisplayName());
                    }
                }
                var schoolBusOriginal = await _unitOfWork.Repository<PostRegistration>().FindAsync(x => x.Id == request.Id && x.AccountId == accountId);
                if (schoolBusOriginal == null)
                {
                    throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.NOT_FOUND_POST,
                                   PostRegistrationErrorEnum.NOT_FOUND_POST.GetDisplayName());
                }
                schoolBusOriginal.SchoolBusOption = request.SchoolBusOption;
                schoolBusOriginal.UpdateAt = GetCurrentDatetime();
                if (!await CheckPostPositionBus(schoolBusOriginal))
                {
                    throw new ErrorResponse(400, (int)PostRegistrationErrorEnum.NOT_QUALIFIED_SCHOOLBUS,
                                   PostRegistrationErrorEnum.NOT_QUALIFIED_SCHOOLBUS.GetDisplayName());
                }
                await _unitOfWork.Repository<PostRegistration>().UpdateDetached(schoolBusOriginal);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<dynamic>
                {
                    Status = new StatusViewModel()
                    {

                        Message = "UPDATE SUCCESS",
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

        public async Task<BaseResponseViewModel<List<PostRegistrationResponse>>> CancelPostRegistrationAdmission(List<int> Ids, int accountId)
        {
            try
            {
                var listResponse = new List<PostRegistration>();
                var findRequests = await _unitOfWork.Repository<PostRegistration>()
                    .GetAll()
                    .Where(x => Ids.Contains(x.Id) && x.Position.Post.AccountId == accountId)
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
                        (int)PostRegistrationErrorEnum.NOT_FOUND_POST,
                        PostRegistrationErrorEnum.NOT_FOUND_POST.GetDisplayName());
                }

                foreach (var findRequest in findRequests)
                {
                    switch (findRequest.Status)
                    {
                        case (int)PostRegistrationStatusEnum.Cancel:
                            throw new ErrorResponse(400,
                                (int)PostRegistrationErrorEnum.ALREADY_CANCELLED,
                                PostRegistrationErrorEnum.ALREADY_CANCELLED.GetDisplayName());
                        default:
                            findRequest.Status = (int)PostRegistrationStatusEnum.Cancel;
                            await _unitOfWork.Repository<PostRegistration>().UpdateDetached(findRequest);
                            await _unitOfWork.CommitAsync();
                            break;
                    }
                }
                listResponse = findRequests;
                return new BaseResponseViewModel<List<PostRegistrationResponse>>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Cancel success",
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

        private async Task<bool> CheckDuplicateUpdate(PostRegistration update)
        {
            var duplicate = await _unitOfWork.Repository<PostRegistration>().FindAsync(x => x.AccountId == update.AccountId
                                                                                           && x.PositionId == update.PositionId && x.Status == (int)PostRegistrationStatusEnum.Confirm);
            if (duplicate != null)
            {
                return false;
            }
            return true;
        }

        private static int CountRegisterAmount(int positionId, List<PostRegistration> postRegistrations)
        {
            return postRegistrations.Count(x => x.PositionId == positionId);
        }

        private async Task<bool> CheckPendingDuplicateTimePosition(PostRegistrationRequest request, int accountId)
        {
            var positionWorkTime = await _unitOfWork.Repository<PostPosition>()
                                                        .FindAsync(x => x.Id == request.PositionId);

            var postRegistrations = _unitOfWork.Repository<PostRegistration>()
                                                .GetAll()
                                                .Where(x => x.Position.Date == positionWorkTime.Date
                                                            && x.AccountId == accountId
                                                            && (x.Status == 1 || x.Status == 2)
                                                            && x.Position.TimeFrom < positionWorkTime.TimeTo
                                                            && x.Position.TimeTo > positionWorkTime.TimeFrom);
            return !postRegistrations.Any();
        }
    }

}
