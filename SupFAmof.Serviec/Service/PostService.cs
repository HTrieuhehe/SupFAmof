using System;
using AutoMapper;
using System.Data;
using System.Linq;
using System.Text;
using Service.Commons;
using SupFAmof.Data.Entity;
using LAK.Sdk.Core.Utilities;
using System.Threading.Tasks;
using SupFAmof.Data.UnitOfWork;
using System.Security.Principal;
using SupFAmof.Service.Utilities;
using System.Collections.Generic;
using System.Reflection.Metadata;
using NetTopologySuite.Geometries;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using Microsoft.EntityFrameworkCore;
using SupFAmof.Service.DTO.Response;
using System.Net.NetworkInformation;
using AutoMapper.QueryableExtensions;
using NetTopologySuite.Index.HPRtree;
using SupFAmof.Service.DTO.Request.Account;
using System.Reflection.PortableExecutable;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.ErrorEnum;
using static ServiceStack.Diagnostics;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Math;

namespace SupFAmof.Service.Service
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public PostService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<BaseResponseViewModel<AdmissionPostResponse>> EndPost(int accountId, int postId)
        {
            try
            {
                var currentTime = Ultils.GetCurrentDatetime();

                #region Check Account 

                var account = _unitOfWork.Repository<Account>().GetAll().FirstOrDefault(x => x.Id == accountId);

                if (account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                else if (account.PostPermission == false)
                {
                    throw new ErrorResponse(403, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                #endregion

                var post = await _unitOfWork.Repository<Post>().FindAsync(x => x.Id == postId && x.AccountId == accountId);

                //var minPositionTime = post.PostPositions.Min(p => p.TimeFrom);
                //var maxPositionTime = post.PostPositions.Max(p => p.TimeTo);

                if (post == null)
                {
                    throw new ErrorResponse(404, (int)PostErrorEnum.NOT_FOUND_ID,
                                        PostErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                //validate nếu post chưa xong thì không được end
                // update lại thời gian đóng post phải nằm trong range ngày bắt đầu và kết thúc

                if (post.DateFrom > currentTime
                    || post.DateTo > currentTime
                    || post.PostPositions.Any(x => x.TimeFrom >= currentTime.TimeOfDay && x.Date <= currentTime
                                                || x.TimeTo > currentTime.TimeOfDay && x.Date <= currentTime))
                {
                    throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_END_POST,
                                        PostErrorEnum.INVALID_END_POST.GetDisplayName());
                }

                post.Status = (int)PostStatusEnum.Ended;
                post.UpdateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<Post>().UpdateDetached(post);

                #region Update Post Registration Pending to Cancel

                var postRegistration = _unitOfWork.Repository<PostRegistration>().GetAll()
                                                  .Where(x => x.Position.PostId == post.Id && x.Status == (int)PostRegistrationStatusEnum.Pending);

                foreach (var item in postRegistration)
                {
                    item.Status = (int)PostRegistrationStatusEnum.Cancel;

                    await _unitOfWork.Repository<PostRegistration>().UpdateDetached(item);
                }

                #endregion

                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AdmissionPostResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdmissionPostResponse>(post)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AdmissionPostResponse>> ReOpenPostRegistration(int accountId, int postId)
        {
            try
            {
                #region Account Checking 

                var account = _unitOfWork.Repository<Account>().GetAll().FirstOrDefault(x => x.Id == accountId);

                if (account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                #endregion

                var post = _unitOfWork.Repository<Post>().Find(x => x.Id == postId && x.AccountId == accountId);

                if (post == null)
                {
                    throw new ErrorResponse(404, (int)PostErrorEnum.NOT_FOUND_ID,
                                        PostErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                //foreach (var item in post.PostPositions)
                //{
                //    //tìm kiếm các post Regist có cùng position Id để so sánh giữa amount và số lượng đk
                //    var totalPostRegist = _unitOfWork.Repository<PostRegistration>().GetAll()
                //                                     .Include(x => x.PostRegistrationDetails)
                //                                     .Where(x => x.PostRegistrationDetails.Any(pd => pd.PositionId == item.Id) && x.Status == (int)PostRegistrationStatusEnum.Confirm);

                //    if (totalPostRegist.Count() != item.Amount)
                //    {
                //        throw new ErrorResponse(404, (int)PostErrorEnum.INVALID_RUN_POST,
                //                            PostErrorEnum.INVALID_RUN_POST.GetDisplayName());
                //    }
                //}

                if (post.Status != (int)PostStatusEnum.Avoid_Regist)
                {
                    throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_RE_OPEN_POST,
                                            PostErrorEnum.INVALID_RE_OPEN_POST.GetDisplayName());
                }

                //get time min of position and date too
                var timeCheck = post.PostPositions.Min(p => p.TimeFrom);
                var dateCheck = post.PostPositions.Min(p => p.Date.ToString("yyyy-MM-dd"));

                //reduce timeCheck 30 minute
                var timeRequired = timeCheck.Subtract(TimeSpan.FromMinutes(30));

                //if (timeCheck >= Ultils.GetCurrentDatetime().TimeOfDay || timeCheck < Ultils.GetCurrentDatetime().AddMinutes(-30).TimeOfDay)
                if (dateCheck == Ultils.GetCurrentDatetime().Date.ToString("yyyy-MM-dd") && timeRequired < Ultils.GetCurrentDatetime().AddMinutes(-30).TimeOfDay)
                {
                    throw new ErrorResponse(400, (int)PostErrorEnum.RE_OPEN_POST_FAIL,
                                            PostErrorEnum.RE_OPEN_POST_FAIL.GetDisplayName());
                }

                //reopen event để cho phép apply đăng ký
                post.Status = (int)PostStatusEnum.Re_Open;
                post.UpdateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<Post>().UpdateDetached(post);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AdmissionPostResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdmissionPostResponse>(post)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AdmissionPostResponse>> ClosePostRegistration(int accountId, int postId)
        {
            try
            {
                #region Account Checking 

                var account = _unitOfWork.Repository<Account>().GetAll().FirstOrDefault(x => x.Id == accountId);

                if (account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                #endregion

                var post = _unitOfWork.Repository<Post>().Find(x => x.Id == postId && x.AccountId == accountId);

                if (post == null)
                {
                    throw new ErrorResponse(404, (int)PostErrorEnum.NOT_FOUND_ID,
                                        PostErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                //nếu dữ liệu trùng khớp thì tiến hành run event
                post.Status = (int)PostStatusEnum.Avoid_Regist;
                post.UpdateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<Post>().UpdateDetached(post);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AdmissionPostResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdmissionPostResponse>(post)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region Admission Post Service

        public async Task<BaseResponseViewModel<AdmissionPostResponse>> CreateAdmissionPost(int accountId, CreatePostRequest request)
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

                if (request.DateFrom <= Ultils.GetCurrentDatetime())
                {
                    throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_DATE_CREATE_POST,
                                         PostErrorEnum.INVALID_DATE_CREATE_POST.GetDisplayName());
                }

                else if (request.DateTo.HasValue && request.DateTo < request.DateFrom)
                {
                    throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_DATETIME_CREATE_POST,
                                         PostErrorEnum.INVALID_DATETIME_CREATE_POST.GetDisplayName());
                }

                //check post position need to full fill in range date from and to
                var validatePositionMissing = request.PostPositions.Find(x => x.Date.Date >= request.DateTo);
                if (validatePositionMissing == null)
                {
                    throw new ErrorResponse(400, (int)PostErrorEnum.VALID_POSITION_MISSING,
                                         PostErrorEnum.VALID_POSITION_MISSING.GetDisplayName() + $": {request.DateTo?.ToString("dd/MM/yyyy")}");
                }

                //check document
                foreach (var position in request.PostPositions)
                {
                    if (position.DocumentId != null)
                    {
                        var checkdocument = await _unitOfWork.Repository<DocumentTemplate>().GetAll().FirstOrDefaultAsync(x => x.Id == position.DocumentId);

                        if (checkdocument == null)
                        {
                            throw new ErrorResponse(400, (int)DocumentErrorEnum.NOT_FOUND_DOCUMENT,
                                             DocumentErrorEnum.NOT_FOUND_DOCUMENT.GetDisplayName() + $" in Position Name: {position.PositionName}");
                        }

                        //continue -> 
                    }

                    //validate Date
                    //request DateFrom must be greater than Current time or before 12 hours before event start

                    //validate Certificate if it is not null
                    if (position.TrainingCertificateId != null)
                    {
                        var checkCerti = _unitOfWork.Repository<TrainingCertificate>().GetAll().FirstOrDefault(x => x.Id == position.TrainingCertificateId);

                        if (position.TrainingCertificateId > 0 && checkCerti == null)
                        {
                            throw new ErrorResponse(400, (int)TrainingCertificateErrorEnum.NOT_FOUND_ID,
                                                 TrainingCertificateErrorEnum.NOT_FOUND_ID.GetDisplayName());
                        }
                    }

                    //validate date in range
                    if (request.DateFrom > position.Date || request.DateTo < position.Date)
                    {
                        throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_POSITION_DATE,
                                         PostErrorEnum.INVALID_POSITION_DATE.GetDisplayName());
                    }

                    //validate Time
                    if (position.TimeFrom < TimeSpan.FromHours(3) || position.TimeFrom > TimeSpan.FromHours(20))
                    {
                        throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_TIME_CREATE_POST,
                                             PostErrorEnum.INVALID_TIME_CREATE_POST.GetDisplayName());
                    }

                    if (position.TimeTo.HasValue)
                    {
                        if (position.TimeTo <= position.TimeFrom)
                        {
                            throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_TIME_CREATE_POST,
                                             PostErrorEnum.INVALID_TIME_CREATE_POST.GetDisplayName());
                        }
                    }
                    position.Status = (int)PostPositionStatusEnum.Active;
                    position.PositionName.Trim();
                    position.PositionDescription.Trim();
                    position.Location.Trim();
                }

                var post = _mapper.Map<Post>(request);

                post.PostDescription = post.PostDescription.Trim();
                post.PostCode = Ultils.GenerateRandomCode();
                post.AccountId = accountId;
                post.Status = (int)PostStatusEnum.Opening;
                post.CreateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<Post>().InsertAsync(post);


                //send notification
                //get account available to send

                var account = _unitOfWork.Repository<Account>().GetAll()
                                            .Where(x => x.IsActive == true && x.RoleId == (int)SystemRoleEnum.Collaborator
                                                                           || x.AccountBanneds.Any() // Đảm bảo có ít nhất một bản ghi AccountBanned
                                                                           && x.AccountBanneds.Max(b => b.DayEnd) <= Ultils.GetCurrentDatetime());

                var accountIds = account.Select(p => p.Id).ToList();

                //create notification request 
                PushNotificationRequest notificationRequest = new PushNotificationRequest()
                {
                    Ids = accountIds,
                    Title = NotificationTypeEnum.Post_Created.GetDisplayName(),
                    Body = "New event is available! Apply now!",
                    NotificationsType = (int)NotificationTypeEnum.Post_Created
                };

                await _notificationService.PushNotification(notificationRequest);

                //after completing, commmit them into database
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AdmissionPostResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdmissionPostResponse>(post)
                };
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<BaseResponsePagingViewModel<AdmissionPostResponse>> GetAdmissionPosts(int accountId, AdmissionPostResponse filter, PagingRequest paging)
        {
            try
            {
                int totalCount = 0;
                int totalPositionCount = 0;
                int? totalAmountPosition = 0;

                var post = _unitOfWork.Repository<Post>().GetAll()
                                    .ProjectTo<AdmissionPostResponse>(_mapper.ConfigurationProvider)
                                    .Where(x => x.AccountId == accountId && x.Status != (int)PostStatusEnum.Delete)
                                    .DynamicFilter(filter)
                                    .DynamicSort(paging.Sort, paging.Order)
                                    .PagingQueryable(paging.Page, paging.PageSize);

                var postResponses = post.Item2.ToList();

                foreach (var item in postResponses)
                {
                    // lấy tất cả các position Id của bài post hiện tại
                    var postPositionIds = item.PostPositions.Select(p => p.Id).ToList();

                    // tìm post Registration có position Id trung với các bài post
                    var postRegistrations = await _unitOfWork.Repository<PostRegistration>()
                            .GetAll()
                            .Where(reg => postPositionIds.Contains(reg.PositionId))
                            .ToListAsync();

                    var postRegistrationsTotal = postRegistrations.Where(reg => reg.Status == (int)PostRegistrationStatusEnum.Pending);

                    item.TotalRegisterAmount = postRegistrationsTotal.Count();

                    //lấy những position pending, confirm , check in và check out
                    var postRegistrationsFiltering = postRegistrations.Where(reg => reg.Status != (int)PostRegistrationStatusEnum.Cancel
                                                                            && reg.Status != (int)PostRegistrationStatusEnum.Quit
                                                                            && reg.Status != (int)PostRegistrationStatusEnum.Reject
                                                                            && reg.Status != (int)PostRegistrationStatusEnum.Pending);

                    // tính tổng các registration đã được confirm
                    item.RegisterAmount = postRegistrationsFiltering.Count();

                    foreach (var itemDetail in item.PostPositions)
                    {
                        //count register amount in post attendee based on position
                        totalCount += CountRegisterAmount(itemDetail.Id, postRegistrationsFiltering);
                        totalPositionCount += CountRegisterAmount(itemDetail.Id, postRegistrationsTotal);

                        //transafer data to field in post position
                        itemDetail.PositionRegisterAmount = totalCount;
                        itemDetail.TotalPositionRegisterAmount = totalPositionCount;

                        //add number of amount required to total amount of a specific post
                        totalAmountPosition += itemDetail.Amount;

                        //tìm post update history có status pending
                        var updateRegistration = _unitOfWork.Repository<PostRgupdateHistory>()
                                .GetAll().Where(x => x.PositionId == itemDetail.Id && x.Status == (int)PostRGUpdateHistoryEnum.Pending);

                        item.TotalUpdateRegisterAmount += updateRegistration.Count();

                        //reset temp count
                        totalCount = 0;
                        totalPositionCount = 0;
                    }
                    //transfer data from position after add to field in post
                    item.TotalAmountPosition = totalAmountPosition;

                    // Reset temp variable
                    totalAmountPosition = 0;
                }

                return new BaseResponsePagingViewModel<AdmissionPostResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = post.Item1
                    },
                    //Data = postResponses.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Priority).ToList()
                    Data = postResponses.ToList()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<AdmissionPostResponse>> GetPostByAccountId(int accountId, AdmissionPostResponse filter, PagingRequest paging)
        {
            try
            {
                int totalCount = 0;
                int totalPositionCount = 0;
                int? totalAmountPosition = 0;

                var post = _unitOfWork.Repository<Post>().GetAll()
                                      .ProjectTo<AdmissionPostResponse>(_mapper.ConfigurationProvider)
                                      .Where(x => x.AccountId == accountId)
                                      .DynamicFilter(filter)
                                      .DynamicSort(paging.Sort, paging.Order)
                                      .PagingQueryable(paging.Page, paging.PageSize);

                var postResponses = post.Item2.ToList();

                foreach (var item in postResponses)
                {
                    // lấy tất cả các position Id của bài post hiện tại
                    var postPositionIds = item.PostPositions.Select(p => p.Id).ToList();

                    // tìm post Registration có position Id trung với các bài post
                    var postRegistrations = await _unitOfWork.Repository<PostRegistration>()
                            .GetAll()
                            .Where(reg => postPositionIds.Contains(reg.PositionId))
                            .ToListAsync();

                    if(postRegistrations.Any(x => x.Status != (int)PostRegistrationStatusEnum.Cancel || x.Status != (int)PostRegistrationStatusEnum.Reject))
                    {
                        item.AnyRegister = true;
                    }

                    var postRegistrationsTotal = postRegistrations.Where(reg => reg.Status == (int)PostRegistrationStatusEnum.Pending);

                    item.TotalRegisterAmount = postRegistrationsTotal.Count();

                    //lấy những position pending, confirm , check in và check out
                    var postRegistrationsFiltering = postRegistrations.Where(reg => reg.Status != (int)PostRegistrationStatusEnum.Cancel
                                                                            && reg.Status != (int)PostRegistrationStatusEnum.Quit
                                                                            && reg.Status != (int)PostRegistrationStatusEnum.Reject
                                                                            && reg.Status != (int)PostRegistrationStatusEnum.Pending);

                    // tính tổng các registration đã được confirm
                    item.RegisterAmount = postRegistrationsFiltering.Count();

                    foreach (var itemDetail in item.PostPositions)
                    {
                        //count register amount in post attendee based on position
                        totalCount += CountRegisterAmount(itemDetail.Id, postRegistrationsFiltering);
                        totalPositionCount += CountRegisterAmount(itemDetail.Id, postRegistrationsTotal);

                        //transafer data to field in post position
                        itemDetail.PositionRegisterAmount = totalCount;
                        itemDetail.TotalPositionRegisterAmount = totalPositionCount;

                        //add number of amount required to total amount of a specific post
                        totalAmountPosition += itemDetail.Amount;

                        //tìm post update history có status pending
                        var updateRegistration = _unitOfWork.Repository<PostRgupdateHistory>()
                                .GetAll().Where(x => x.PositionId == itemDetail.Id && x.Status == (int)PostRGUpdateHistoryEnum.Pending);

                        item.TotalUpdateRegisterAmount += updateRegistration.Count();

                        //reset temp count
                        totalCount = 0;
                        totalPositionCount = 0;
                    }
                    //transfer data from position after add to field in post
                    item.TotalAmountPosition = totalAmountPosition;

                    // Reset temp variable
                    totalAmountPosition = 0;
                }

                return new BaseResponsePagingViewModel<AdmissionPostResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = post.Item1
                    },
                    //Data = postResponses.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Priority).ToList()
                    Data = postResponses
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponseViewModel<AdmissionPostResponse>> GetPostByPostcode(string postCode)
        {
            try
            {
                var post = _unitOfWork.Repository<Post>().GetAll().FirstOrDefault(x => x.PostCode.Contains(postCode));

                if (post == null)
                {
                    throw new ErrorResponse(404, (int)PostErrorEnum.NOT_FOUND_ID,
                                         PostErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }


                return new BaseResponseViewModel<AdmissionPostResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdmissionPostResponse>(post)
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponseViewModel<AdmissionPostResponse>> UpdateAdmissionPost(int accountId, int postId, UpdatePostRequest request)
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

                var checkPost = _unitOfWork.Repository<Post>()
                                        .Find(x => x.Id == postId && x.AccountId == accountId);

                if (checkPost == null)
                {
                    throw new ErrorResponse(404, (int)PostErrorEnum.NOT_FOUND_ID,
                                         PostErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                //post ended or canceled so can not update
                if (checkPost.Status == (int)PostStatusEnum.Ended || checkPost.Status == (int)PostStatusEnum.Cancel)
                {
                    throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_UPDATE_POST_BY_STATUS,
                                         PostErrorEnum.INVALID_UPDATE_POST_BY_STATUS.GetDisplayName());
                }

                var postPositionIds = checkPost.PostPositions.Select(p => p.Id).ToList();

                //check if anyone apply to any position
                var postRegistration = _unitOfWork.Repository<PostRegistration>().GetAll().Where(x => postPositionIds.Contains(x.PositionId)
                                                                                                          && x.Status != (int)PostRegistrationStatusEnum.Cancel
                                                                                                          && x.Status != (int)PostRegistrationStatusEnum.Reject);

                //update những field vô tri
                checkPost.Id = postId;
                checkPost.AccountId = accountId;
                checkPost.PostCategoryId = request.PostCategoryId;
                checkPost.PostDescription = request.PostDescription.Trim();
                checkPost.PostImg = request.PostImg.Trim();
                
                checkPost.UpdateAt = Ultils.GetCurrentDatetime();

                if (!postRegistration.Any())
                {
                    //allow to update premium credential
                    checkPost.IsPremium = request.IsPremium;

                    //allow to update full including time
                    //date and validate
                    if (request.DateFrom != null && request.DateTo != null)
                    {
                        checkPost.DateFrom = request.DateFrom;
                        checkPost.DateTo = request.DateTo;

                        //validate date
                        if (request.DateFrom <= Ultils.GetCurrentDatetime())
                        {
                            throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_DATE_UPDATE_POST,
                                                 PostErrorEnum.INVALID_DATE_UPDATE_POST.GetDisplayName());
                        }

                        else if (request.DateTo.HasValue && request.DateTo < request.DateFrom)
                        {
                            throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_DATETIME_UPDATE_POST,
                                                 PostErrorEnum.INVALID_DATETIME_UPDATE_POST.GetDisplayName());
                        }
                    }

                    //update current position
                    foreach (var updatePosition in request.PostPositions)
                    {
                        if (updatePosition.Id == 0)
                        {
                            //create new position for this post
                            PostPosition newPosition = new PostPosition()
                            {
                                PostId = checkPost.Id,
                                TrainingCertificateId = updatePosition.TrainingCertificateId,
                                DocumentId = updatePosition.DocumentId,
                                PositionName = updatePosition.PositionName.Trim(),
                                PositionDescription = updatePosition.PositionDescription.Trim(),
                                SchoolName = updatePosition.SchoolName.Trim(),
                                Location = updatePosition.Location.Trim(),  
                                Latitude = updatePosition.Latitude,
                                Longitude = updatePosition.Longitude,
                                Date = updatePosition.Date,
                                TimeFrom = updatePosition.TimeFrom.Value,
                                TimeTo = updatePosition.TimeTo,
                                IsBusService = updatePosition.IsBusService,
                                
                                //Active this position
                                Status = (int)PostPositionStatusEnum.Active,

                                Amount = updatePosition.Amount,
                                Salary = updatePosition.Salary,
                            };

                            //validate date and time in position

                            //validate date in range
                            if (checkPost.DateFrom > newPosition.Date || checkPost.DateTo < newPosition.Date)
                            {
                                throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_POSITION_DATE,
                                                 PostErrorEnum.INVALID_POSITION_DATE.GetDisplayName());
                            }

                            //validate Time
                            if (newPosition.TimeFrom < TimeSpan.FromHours(3) || newPosition.TimeFrom > TimeSpan.FromHours(20))
                            {
                                throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_TIME_CREATE_POST,
                                                     PostErrorEnum.INVALID_TIME_CREATE_POST.GetDisplayName());
                            }

                            if (newPosition.TimeTo.HasValue)
                            {
                                if (newPosition.TimeTo <= newPosition.TimeFrom)
                                {
                                    throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_TIME_CREATE_POST,
                                                     PostErrorEnum.INVALID_TIME_CREATE_POST.GetDisplayName());
                                }
                            }

                            await _unitOfWork.Repository<PostPosition>().InsertAsync(newPosition);

                            continue;
                        }

                        //find root position to update
                        var currentPosition = checkPost.PostPositions
                                        .FirstOrDefault(x => x.Id == updatePosition.Id);

                        if (currentPosition == null)
                        {
                            continue;
                        }

                        else if (currentPosition.Status == (int)PostPositionStatusEnum.Delete)
                        {
                            throw new ErrorResponse(400, (int)PostErrorEnum.POSITION_EDITED_FORBIDDEN,
                                        $"The position {currentPosition.PositionName} " + PostErrorEnum.POSITION_EDITED_FORBIDDEN.GetDisplayName());
                        }

                        //validate date and location
                        if (updatePosition.Date < checkPost.DateFrom || updatePosition.Date > checkPost.DateTo)
                        {
                            throw new ErrorResponse(400, (int)PostErrorEnum.POSITION_DATE_UPDATE_INVALID,
                                        PostErrorEnum.POSITION_DATE_UPDATE_INVALID.GetDisplayName() + $"'{updatePosition.PositionName}' date must in range of post");
                        }

                        currentPosition.Id = currentPosition.Id;
                        currentPosition.TrainingCertificateId = updatePosition.TrainingCertificateId;
                        currentPosition.DocumentId = updatePosition.DocumentId;
                        currentPosition.PositionName = updatePosition.PositionName.Trim();
                        currentPosition.PositionDescription = updatePosition.PositionDescription.Trim();
                        currentPosition.SchoolName = updatePosition.SchoolName.Trim();
                        currentPosition.Location = updatePosition.Location.Trim();
                        currentPosition.Latitude = updatePosition.Latitude;
                        currentPosition.Longitude = updatePosition.Longitude;
                        currentPosition.Date = updatePosition.Date;
                        currentPosition.TimeFrom = updatePosition.TimeFrom.Value;
                        currentPosition.TimeTo = updatePosition.TimeTo;
                        currentPosition.IsBusService = updatePosition.IsBusService;
                        currentPosition.Amount = updatePosition.Amount;
                        currentPosition.Salary = updatePosition.Salary;
                    }

                    await _unitOfWork.Repository<Post>().UpdateDetached(checkPost);
                    await _unitOfWork.CommitAsync();

                    return new BaseResponseViewModel<AdmissionPostResponse>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "Success",
                            Success = true,
                            ErrorCode = 0
                        },
                        Data = _mapper.Map<AdmissionPostResponse>(checkPost)
                    };
                }

                // error mess: There is register so keep
                if (request.DateFrom.Date != checkPost.DateFrom.Date)
                {
                    throw new ErrorResponse(400, (int)PostErrorEnum.FIELD_MUST_KEEP,
                                         PostErrorEnum.FIELD_MUST_KEEP.GetDisplayName() + $" Post start date at: {checkPost.DateFrom}");
                }

                if (request.DateTo?.Date != checkPost.DateTo?.Date)
                {
                    throw new ErrorResponse(400, (int)PostErrorEnum.FIELD_MUST_KEEP,
                                         PostErrorEnum.FIELD_MUST_KEEP.GetDisplayName() + $" Post end Date at: {checkPost.DateTo}");
                }

                //not allow to upgrade to premium -> Allow to downgrade
                if (checkPost.IsPremium == true && request.IsPremium == false)
                {
                    throw new ErrorResponse(400, (int)PostErrorEnum.PREMIUM_MUST_KEEP,
                                    PostErrorEnum.PREMIUM_MUST_KEEP.GetDisplayName());
                }

                //update current position except post Date, position Date, position time and position salary
                //validate amount need to greater than position
                foreach (var updatePosition in request.PostPositions)
                {
                    if (updatePosition.Id == 0)
                    {
                        //create new position for this post
                        PostPosition newPosition = new PostPosition()
                        {
                            PostId = checkPost.Id,
                            TrainingCertificateId = updatePosition.TrainingCertificateId,
                            DocumentId = updatePosition.DocumentId,
                            PositionName = updatePosition.PositionName.Trim(),
                            PositionDescription = updatePosition.PositionDescription.Trim(),
                            SchoolName = updatePosition.SchoolName.Trim(),
                            Location = updatePosition.Location.Trim(),
                            Latitude = updatePosition.Latitude,
                            Longitude = updatePosition.Longitude,
                            Date = updatePosition.Date,
                            TimeFrom = updatePosition.TimeFrom.Value,
                            TimeTo = updatePosition.TimeTo,
                            IsBusService = updatePosition.IsBusService,

                            //Active this position
                            Status = (int)PostPositionStatusEnum.Active,

                            Amount = updatePosition.Amount,
                            Salary = updatePosition.Salary,
                        };

                        //validate date and time in position
                        //validate date in range
                        if (checkPost.DateFrom > newPosition.Date || checkPost.DateTo < newPosition.Date)
                        {
                            throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_POSITION_DATE,
                                             PostErrorEnum.INVALID_POSITION_DATE.GetDisplayName());
                        }

                        //validate Time
                        if (newPosition.TimeFrom < TimeSpan.FromHours(3) || newPosition.TimeFrom > TimeSpan.FromHours(20))
                        {
                            throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_TIME_CREATE_POST,
                                                 PostErrorEnum.INVALID_TIME_CREATE_POST.GetDisplayName());
                        }

                        if (newPosition.TimeTo.HasValue)
                        {
                            if (newPosition.TimeTo <= newPosition.TimeFrom)
                            {
                                throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_TIME_CREATE_POST,
                                                 PostErrorEnum.INVALID_TIME_CREATE_POST.GetDisplayName());
                            }
                        }

                        await _unitOfWork.Repository<PostPosition>().InsertAsync(newPosition);

                    }

                    //find root position to update
                    var currentPosition = checkPost.PostPositions
                                        .FirstOrDefault(x => x.Id == updatePosition.Id);

                    if (currentPosition == null)
                    {
                        continue;
                    }

                    else if (currentPosition.Status == (int)PostPositionStatusEnum.Delete)
                    {
                        throw new ErrorResponse(400, (int)PostErrorEnum.POSITION_EDITED_FORBIDDEN,
                                    $"The position {currentPosition.PositionName}" + PostErrorEnum.POSITION_EDITED_FORBIDDEN.GetDisplayName());
                    }

                    //validate Post and Position date and time and salary

                    // error mess: There is register so keep

                    if (updatePosition.Date != currentPosition.Date)
                    {
                        throw new ErrorResponse(400, (int)PostErrorEnum.FIELD_MUST_KEEP,
                                             PostErrorEnum.FIELD_MUST_KEEP.GetDisplayName() + $" Position Date at: {currentPosition.Date}");
                    }

                    if (updatePosition.TimeFrom != currentPosition.TimeFrom)
                    {
                        throw new ErrorResponse(400, (int)PostErrorEnum.FIELD_MUST_KEEP,
                                             PostErrorEnum.FIELD_MUST_KEEP.GetDisplayName() + $" Position Start Time at: {currentPosition.TimeFrom}");
                    }

                    if (updatePosition.TimeTo != currentPosition.TimeTo)
                    {
                        throw new ErrorResponse(400, (int)PostErrorEnum.FIELD_MUST_KEEP,
                                             PostErrorEnum.FIELD_MUST_KEEP.GetDisplayName() + $" Position End Time at: {currentPosition.TimeTo}");
                    }

                    //check amount of position to makesure that the new amount can not less than the old one
                    var positionCounting = postRegistration.Where(x => x.PositionId == currentPosition.Id
                                                                            && x.Status == (int)PostRegistrationStatusEnum.Confirm
                                                                            || x.Status == (int)PostRegistrationStatusEnum.CheckOut
                                                                            || x.Status == (int)PostRegistrationStatusEnum.CheckIn).Count();

                    if (updatePosition.Amount < positionCounting)
                    {
                        throw new ErrorResponse(400, (int)PostErrorEnum.AMOUNT_INVALID,
                                        PostErrorEnum.AMOUNT_INVALID.GetDisplayName() + $"'{updatePosition.PositionName}' amount must higher than: {positionCounting}");
                    }


                    //check salary
                    if (updatePosition.Salary != currentPosition.Salary)
                    {
                        throw new ErrorResponse(400, (int)PostErrorEnum.FIELD_MUST_KEEP,
                                             PostErrorEnum.FIELD_MUST_KEEP.GetDisplayName() + $" Salary at: {currentPosition.Salary}");
                    }

                    if (updatePosition.Salary != currentPosition.Salary)
                    {
                        throw new ErrorResponse(400, (int)PostErrorEnum.FIELD_MUST_KEEP,
                                             PostErrorEnum.FIELD_MUST_KEEP.GetDisplayName() + $" Salary at: {currentPosition.Salary}");
                    }

                    if(updatePosition.TrainingCertificateId != null && updatePosition.TrainingCertificateId != currentPosition.TrainingCertificateId)
                    {
                        throw new ErrorResponse(400, (int)PostErrorEnum.CERTIFICATE_MUST_KEEP,
                                             PostErrorEnum.CERTIFICATE_MUST_KEEP.GetDisplayName());
                    }

                    currentPosition.Id = currentPosition.Id;
                    currentPosition.DocumentId = updatePosition.DocumentId;
                    currentPosition.PositionName = updatePosition.PositionName.Trim();
                    currentPosition.PositionDescription = updatePosition.PositionDescription.Trim();
                    currentPosition.SchoolName = updatePosition.SchoolName.Trim();
                    currentPosition.Location = updatePosition.Location.Trim();
                    currentPosition.Latitude = updatePosition.Latitude;
                    currentPosition.Longitude = updatePosition.Longitude;
                    currentPosition.IsBusService = updatePosition.IsBusService;
                    currentPosition.Amount = updatePosition.Amount;
                }

                await _unitOfWork.Repository<Post>().UpdateDetached(checkPost);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AdmissionPostResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdmissionPostResponse>(checkPost)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AdmissionPostResponse>> DeletePostPosition(int accountId, int positionId)
        {
            try
            {
                //define collabroator accountId to send notification
                List<int> collaboratorId = new List<int>();

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

                //make sure position is update status by someone who created them
                var postPosition = await _unitOfWork.Repository<PostPosition>()
                                    .FindAsync(x => x.Id == positionId && x.Post.AccountId == accountId && x.Post.Id == x.PostId);

                if (postPosition == null)
                {
                    throw new ErrorResponse(404, (int)PostErrorEnum.NOT_FOUND_ID,
                                         PostErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                //validate to makesure there is no applied in this position

                var checkApplied = _unitOfWork.Repository<PostRegistration>()
                                                   .GetAll()
                                                   .Where(x => x.PositionId == positionId);

                //check if there is any registration is confirmed or more
                if (checkApplied.Any(x => x.Status == (int)PostRegistrationStatusEnum.Confirm
                                         || x.Status == (int)PostRegistrationStatusEnum.CheckIn
                                         || x.Status == (int)PostRegistrationStatusEnum.CheckOut))
                {
                    throw new ErrorResponse(400, (int)PostErrorEnum.DELETE_POSITION_FAIL,
                        PostErrorEnum.DELETE_POSITION_FAIL.GetDisplayName());
                }

                //remove registration 
                if (checkApplied != null)
                {
                    foreach (var registration in checkApplied)
                    {
                        if (registration.Status == (int)PostRegistrationStatusEnum.Pending)
                        {
                            collaboratorId.Add(registration.AccountId);
                            registration.Status = (int)PostRegistrationStatusEnum.Cancel;
                            registration.CancelTime = Ultils.GetCurrentDatetime();

                            await _unitOfWork.Repository<PostRegistration>().UpdateDetached(registration);
                        }
                    }
                }

                postPosition.Status = (int)PostPositionStatusEnum.Delete;

                await _unitOfWork.Repository<PostPosition>().UpdateDetached(postPosition);


                //create notification request 
                PushNotificationRequest notificationRequest = new PushNotificationRequest()
                {
                    Ids = collaboratorId,
                    Title = NotificationTypeEnum.RECRUITMENT_POSITION_REMOVED.GetDisplayName(),
                    Body = "Your recruitment registration is canceled by the admission removed position",
                    NotificationsType = (int)NotificationTypeEnum.RECRUITMENT_POSITION_REMOVED
                };

                await _notificationService.PushNotification(notificationRequest);

                await _unitOfWork.CommitAsync();


                //call to get post
                var result = await _unitOfWork.Repository<Post>().FindAsync(x => x.Id == postPosition.PostId);

                return new BaseResponseViewModel<AdmissionPostResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdmissionPostResponse>(result)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AdmissionPostResponse>> DeletePost(int accountId, int postId)
        {
            try
            {
                //define collabroator accountId to send notification
                List<int> collaboratorId = new List<int>();

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

                //make sure post is update status by someone who created them
                var post = await _unitOfWork.Repository<Post>()
                                    .FindAsync(x => x.Id == postId && x.AccountId == accountId);

                if (post == null)
                {
                    throw new ErrorResponse(404, (int)PostErrorEnum.NOT_FOUND_ID,
                                         PostErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                //validate to makesure there is no applied in this position
                var postPositionIds = post.PostPositions.Select(p => p.Id).ToList();

                //get registration relevant to this post

                var checkApplied = _unitOfWork.Repository<PostRegistration>()
                                                    .GetAll()
                                                    .Where(x => postPositionIds.Contains(x.PositionId));

                //check if there is any registration is confirmed or more
                if (checkApplied.Any(x => x.Status == (int)PostRegistrationStatusEnum.Confirm
                                         || x.Status == (int)PostRegistrationStatusEnum.CheckIn
                                         || x.Status == (int)PostRegistrationStatusEnum.CheckOut))
                {
                    throw new ErrorResponse(400, (int)PostErrorEnum.DELETE_POST_FAIL,
                        PostErrorEnum.DELETE_POST_FAIL.GetDisplayName());
                }

                //remove registration 
                if (checkApplied != null)
                {
                    foreach (var registration in checkApplied)
                    {
                        if (registration.Status == (int)PostRegistrationStatusEnum.Pending)
                        {
                            collaboratorId.Add(registration.AccountId);
                            registration.Status = (int)PostRegistrationStatusEnum.Cancel;
                            registration.CancelTime = Ultils.GetCurrentDatetime();

                            await _unitOfWork.Repository<PostRegistration>().UpdateDetached(registration);
                        }
                    }
                }

                //if no one is applied so remove the registration too
                foreach (var position in post.PostPositions)
                {
                    position.Status = (int)PostPositionStatusEnum.Delete;
                    await _unitOfWork.Repository<PostPosition>().UpdateDetached(position);
                }

                post.Status = (int)PostStatusEnum.Delete;
                post.UpdateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<Post>().UpdateDetached(post);

                //create notification request 
                PushNotificationRequest notificationRequest = new PushNotificationRequest()
                {
                    Ids = collaboratorId,
                    Title = NotificationTypeEnum.RECRUITMENT_POST_REMOVED.GetDisplayName(),
                    Body = "Your recruitment registration is canceled by the admission removed post",
                    NotificationsType = (int)NotificationTypeEnum.RECRUITMENT_POST_REMOVED
                };

                await _notificationService.PushNotification(notificationRequest);

                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AdmissionPostResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdmissionPostResponse>(post)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AdmissionPostResponse>> AdmissionSearchPost(int accountId, string searchPost, PagingRequest paging)
        {
            try
            {
                var post = _unitOfWork.Repository<Post>().GetAll()
                                        .ProjectTo<AdmissionPostResponse>(_mapper.ConfigurationProvider)
                                        .Where(x => x.PostCode.Contains(searchPost) || x.PostCategory.PostCategoryDescription.Contains(searchPost)
                                                                                    || x.PostDescription.Contains(searchPost)
                                                                                    || x.PostPositions.Any(x => x.SchoolName.Contains(searchPost))
                                                                                    || x.PostPositions.Any(x => x.Location.Contains(searchPost)))
                                        .OrderByDescending(x => x.CreateAt)
                                        .OrderByDescending(x => x.Priority)
                                        .PagingQueryable(paging.Page, paging.PageSize);

                return new BaseResponseViewModel<AdmissionPostResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdmissionPostResponse>(post)
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponseViewModel<AdmissionPostResponse>> CreatePostPosition(int accountId, int postId, CreatePostPositionRequest request)
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

                //check post

                var post = await _unitOfWork.Repository<Post>().FindAsync(x => x.Id == postId);

                if (post == null)
                {
                    throw new ErrorResponse(404, (int)PostErrorEnum.NOT_FOUND_ID,
                                        PostErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                if (request == null)
                {
                    throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_NEW_POSITION,
                                        PostErrorEnum.INVALID_NEW_POSITION.GetDisplayName());
                }

                var position = _mapper.Map<CreatePostPositionRequest, PostPosition>(request);
                position.Status = (int)PostPositionStatusEnum.Active;

                post.PostPositions.Add(position);

                await _unitOfWork.Repository<Post>().UpdateDetached(post);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AdmissionPostResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdmissionPostResponse>(post)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region Collaborator Post Service

        public async Task<BaseResponsePagingViewModel<PostResponse>> GetPosts(int accountId, string search, PostResponse filter, TimeFromFilter timeFromFilter, PagingRequest paging)
        {
            try
            {
                var checkAccount = await _unitOfWork.Repository<Account>().GetAll().FirstOrDefaultAsync(a => a.Id == accountId);

                if (checkAccount == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                         AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                if (checkAccount.IsActive == false)
                {
                    throw new ErrorResponse(400, (int)AccountErrorEnums.ACCOUNT_DISABLE,
                                         AccountErrorEnums.ACCOUNT_DISABLE.GetDisplayName());
                }

                if (checkAccount.IsPremium == true)
                {
                    if (!string.IsNullOrEmpty(search))
                    {
                        var searchPremiumPost = _unitOfWork.Repository<Post>().GetAll()
                                      .Where(x => x.Status >= (int)PostStatusEnum.Opening && x.Status <= (int)PostStatusEnum.Avoid_Regist)
                                      .Include(x => x.PostPositions.Where(x => x.Status == (int)PostPositionStatusEnum.Active))
                                      .ProjectTo<PostResponse>(_mapper.ConfigurationProvider)
                                      .DynamicFilter(filter)
                                      .DynamicSort(paging.Sort, paging.Order)
                                      .PagingQueryable(paging.Page, paging.PageSize);

                        var filterStatus = searchPremiumPost.Item2.Where(x => x.PostCode.Contains(search) || x.PostCategory.PostCategoryDescription.Contains(search)
                                                                                    || x.PostDescription.Contains(search)
                                                                                    || x.PostPositions.Any(x => x.SchoolName.Contains(search))
                                                                                    || x.PostPositions.Any(x => x.Location.Contains(search)));

                        var postPremiumSearchResponses = await filterStatus.ToListAsync();

                        var postSearchAfterCounting = await CountPostInformation(postPremiumSearchResponses);

                        return new BaseResponsePagingViewModel<PostResponse>()
                        {
                            Metadata = new PagingsMetadata()
                            {
                                Page = paging.Page,
                                Size = paging.PageSize,
                                Total = searchPremiumPost.Item1
                            },
                            //Data = postPremiumResponses.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Priority).ToList()
                            Data = postSearchAfterCounting
                        };
                    }

                    var premiumPost = _unitOfWork.Repository<Post>().GetAll()
                                      .Where(x => x.Status >= (int)PostStatusEnum.Opening && x.Status <= (int)PostStatusEnum.Avoid_Regist)
                                       .Include(x => x.PostPositions.Where(x => x.Status == (int)PostPositionStatusEnum.Active))
                                      .ProjectTo<PostResponse>(_mapper.ConfigurationProvider)
                                      .DynamicFilter(filter)
                                      .DynamicSort(paging.Sort, paging.Order);

                    var premiumList = FilterPostDateFrom(premiumPost, timeFromFilter).PagingQueryable(paging.Page, paging.PageSize);
                    var response = await premiumList.Item2.ToListAsync();

                    var premiumPostCounting = await CountPostInformation(response);

                    return new BaseResponsePagingViewModel<PostResponse>()
                    {
                        Metadata = new PagingsMetadata()
                        {
                            Page = paging.Page,
                            Size = paging.PageSize,
                            Total = premiumList.Item1
                        },
                        //Data = postPremiumResponses.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Priority).ToList()
                        Data = premiumPostCounting
                    };
                }

                if (!string.IsNullOrEmpty(search))
                {
                    var searchPost = _unitOfWork.Repository<Post>().GetAll()
                                    .Where(x => x.Status <= (int)PostStatusEnum.Avoid_Regist && x.IsPremium == false)
                                    .Include(x => x.PostPositions.Where(x => x.Status == (int)PostPositionStatusEnum.Active))
                                    .ProjectTo<PostResponse>(_mapper.ConfigurationProvider)
                                    .DynamicFilter(filter)
                                    .DynamicSort(paging.Sort, paging.Order)
                                    .PagingQueryable(paging.Page, paging.PageSize);

                    var filterStatus = searchPost.Item2.Where(x => x.PostCode.Contains(search) || x.PostCategory.PostCategoryDescription.Contains(search)
                                                                                            || x.PostDescription.Contains(search)
                                                                                            || x.PostPositions.Any(x => x.SchoolName.Contains(search))
                                                                                            || x.PostPositions.Any(x => x.Location.Contains(search)));

                    var postSearchResponses = await filterStatus.ToListAsync();

                    var regularSearchPostAfterCounting = await CountPostInformation(postSearchResponses);

                    return new BaseResponsePagingViewModel<PostResponse>()
                    {
                        Metadata = new PagingsMetadata()
                        {
                            Page = paging.Page,
                            Size = paging.PageSize,
                            Total = searchPost.Item1
                        },
                        //Data = postResponses.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Priority).ToList()
                        Data = regularSearchPostAfterCounting
                    };
                }

                var posts = _unitOfWork.Repository<Post>().GetAll()
                                    .Where(x => x.Status >= (int)PostStatusEnum.Opening
                                             && x.Status <= (int)PostStatusEnum.Avoid_Regist && x.IsPremium == false)
                                    .Include(x => x.PostPositions.Where(x => x.Status == (int)PostPositionStatusEnum.Active))
                                    .ProjectTo<PostResponse>(_mapper.ConfigurationProvider)
                                    .DynamicFilter(filter)
                                    .DynamicSort(paging.Sort, paging.Order);

                var regularPostsFilter = FilterPostDateFrom(posts, timeFromFilter).PagingQueryable(paging.Page, paging.PageSize);
                var regularPosts = await regularPostsFilter.Item2.ToListAsync();

                var regularPostsAfterCounting = await CountPostInformation(regularPosts);

                return new BaseResponsePagingViewModel<PostResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = regularPostsFilter.Item1
                    },
                    //Data = postResponses.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Priority).ToList()
                    Data = regularPostsAfterCounting
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<List<PostResponse>> CountPostInformation(List<PostResponse> listPosts)
        {
            int totalCount = 0;
            int totalPositionCount = 0;
            int? totalAmountPosition = 0;

            foreach (var item in listPosts)
            {
                //lấy thời gian thấp nhất và cao nhất để hiển thị trên UI
                item.TimeFrom = item.PostPositions.Min(p => p.TimeFrom).ToString();
                item.TimeTo = item.PostPositions.Max(p => p.TimeTo).ToString();

                // lấy tất cả các position Id của bài post hiện tại
                var postPositionIds = item.PostPositions.Select(p => p.Id).ToList();

                // tìm post Registration có position Id trung với các bài post
                var postRegistrations = await _unitOfWork.Repository<PostRegistration>()
                        .GetAll()
                        .Where(reg => postPositionIds.Contains(reg.PositionId))
                        .ToListAsync();

                //lấy những position pending, confirm , check in và check out
                var postRegistrationsTotal = postRegistrations.Where(reg => reg.Status != (int)PostRegistrationStatusEnum.Cancel
                                                                        && reg.Status != (int)PostRegistrationStatusEnum.Quit
                                                                        && reg.Status != (int)PostRegistrationStatusEnum.Reject);

                item.TotalRegisterAmount = postRegistrationsTotal.Count();

                var postRegistrationsFiltering = postRegistrationsTotal.Where(reg => reg.Status != (int)PostRegistrationStatusEnum.Pending);

                // tính tổng các registration đã được confirm
                item.RegisterAmount = postRegistrationsFiltering.Count();

                foreach (var itemDetail in item.PostPositions)
                {
                    if (itemDetail.Status == (int)PostPositionStatusEnum.Delete)
                    {
                        continue;
                    }

                    //count register amount in post attendee based on position
                    totalCount += CountRegisterAmount(itemDetail.Id, postRegistrationsFiltering);
                    totalPositionCount += CountRegisterAmount(itemDetail.Id, postRegistrationsTotal);

                    //transafer data to field in post position
                    itemDetail.PositionRegisterAmount = totalCount;
                    itemDetail.TotalPositionRegisterAmount = totalPositionCount;

                    //add number of amount required to total amount of a specific post
                    totalAmountPosition += itemDetail.Amount;

                    //reset temp count
                    totalCount = 0;
                    totalPositionCount = 0;
                }
                //transfer data from position after add to field in post
                item.TotalAmountPosition = totalAmountPosition;

                // Reset temp variable
                totalAmountPosition = 0;
            }

            return listPosts;
        }

        public async Task<BaseResponseViewModel<PostResponse>> GetPostById(int accountId, int postId)
        {
            try
            {
                int totalCount = 0;
                int? totalAmountPosition = 0;
                int? totalPositionRegister = 0;
                var checkAccount = await _unitOfWork.Repository<Account>().GetAll().FirstOrDefaultAsync(a => a.Id == accountId);

                if (checkAccount == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                         AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                if (checkAccount.IsActive == false)
                {
                    throw new ErrorResponse(400, (int)AccountErrorEnums.ACCOUNT_DISABLE,
                                         AccountErrorEnums.ACCOUNT_DISABLE.GetDisplayName());
                }

                var post = await _unitOfWork.Repository<Post>().GetAll()
                                            .Include(x => x.PostPositions.Where(x => x.Status == (int)PostPositionStatusEnum.Active))
                                            .FirstOrDefaultAsync(x => x.Id == postId);

                if (post == null)
                {
                    throw new ErrorResponse(404, (int)PostErrorEnum.NOT_FOUND_ID,
                                         PostErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                if (post.IsPremium == true && checkAccount.IsPremium == false)
                {
                    throw new ErrorResponse(403, (int)PostErrorEnum.PREMIUM_REQUIRED,
                                         PostErrorEnum.PREMIUM_REQUIRED.GetDisplayName());
                }

                var postMapping = _mapper.Map<PostResponse>(post);

                //lấy thời gian thấp nhất và cao nhất để hiển thị trên UI
                postMapping.TimeFrom = post.PostPositions.Min(p => p.TimeFrom).ToString();
                postMapping.TimeTo = post.PostPositions.Max(p => p.TimeTo).ToString();

                // lấy tất cả các position Id của bài post hiện tại
                var postPositionIds = postMapping.PostPositions.Select(p => p.Id).ToList();

                // tìm post Registration có position Id trung với các bài post
                var postRegistrations = await _unitOfWork.Repository<PostRegistration>()
                                                    .GetAll()
                                                    .Where(reg => postPositionIds.Contains(reg.PositionId))
                                                    .ToListAsync();

                var postRegistrationsTotal = postRegistrations.Where(reg => reg.Status != (int)PostRegistrationStatusEnum.Cancel
                                                                            && reg.Status != (int)PostRegistrationStatusEnum.Quit
                                                                            && reg.Status != (int)PostRegistrationStatusEnum.Reject);

                postMapping.TotalRegisterAmount = postRegistrationsTotal.Count();

                var postRegistrationsFiltering = postRegistrationsTotal.Where(reg => reg.Status != (int)PostRegistrationStatusEnum.Pending);

                // tính tổng các registration đã được confirm
                postMapping.RegisterAmount = postRegistrationsFiltering.Count();

                foreach (var itemDetail in postMapping.PostPositions)
                {
                    //count register amount in post attendee based on position
                    totalCount += CountRegisterAmount(itemDetail.Id, postRegistrationsFiltering);
                    totalPositionRegister += CountRegisterAmount(itemDetail.Id, postRegistrationsTotal);

                    //transafer data to field in post position
                    itemDetail.PositionRegisterAmount = totalCount;
                    itemDetail.TotalPositionRegisterAmount = totalPositionRegister;

                    //add number of amount required to total amount of a specific post
                    totalAmountPosition += itemDetail.Amount;

                    //reset temp count
                    totalCount = 0;
                    totalPositionRegister = 0;
                }
                //transfer data from position after add to field in post
                postMapping.TotalAmountPosition = totalAmountPosition;

                // Reset temp variable
                totalAmountPosition = 0;

                return new BaseResponseViewModel<PostResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = postMapping
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<PostResponse>> SearchPost(string searchPost, PagingRequest paging)
        {
            try
            {
                var post = _unitOfWork.Repository<Post>().GetAll()
                                        .ProjectTo<PostResponse>(_mapper.ConfigurationProvider)
                                        .Where(x => x.PostCode.Contains(searchPost) || x.PostCategory.PostCategoryDescription.Contains(searchPost)
                                                                                    || x.PostDescription.Contains(searchPost)
                                                                                    || x.PostPositions.Any(x => x.SchoolName.Contains(searchPost))
                                                                                    || x.PostPositions.Any(x => x.Location.Contains(searchPost)))
                                        .OrderByDescending(x => x.CreateAt)
                                        .OrderByDescending(x => x.Priority)
                                        .PagingQueryable(paging.Page, paging.PageSize);

                return new BaseResponsePagingViewModel<PostResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = post.Item1
                    },
                    Data = post.Item2.ToList()
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        private static int CountRegisterAmount(int positionId, IEnumerable<PostRegistration> postRegistrations)
        {
            return postRegistrations.Count(x => x.PositionId == positionId);
        }

        public async Task<BaseResponsePagingViewModel<PostResponse>> GetPostReOpen(int accountId, PostResponse filter, string search, TimeFromFilter timeFromFilter, PagingRequest paging)
        {
            try
            {
                int totalCount = 0;
                int? totalAmountPosition = 0;
                int totalPositionCount = 0;
                var checkAccount = _unitOfWork.Repository<Account>().GetAll().FirstOrDefault(a => a.Id == accountId);

                if (checkAccount == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                         AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                if (checkAccount.IsActive == false)
                {
                    throw new ErrorResponse(400, (int)AccountErrorEnums.ACCOUNT_DISABLE,
                                         AccountErrorEnums.ACCOUNT_DISABLE.GetDisplayName());
                }

                //check nếu tài khoảng premium mới cho coi post premium
                else if (checkAccount.IsPremium == true)
                {
                    if (!string.IsNullOrEmpty(search))
                    {
                        var searchPremiumPost = _unitOfWork.Repository<Post>().GetAll()
                                       .Where(x => x.Status == (int)PostStatusEnum.Re_Open)
                                       .Include(x => x.PostPositions.Where(x => x.Status == (int)PostPositionStatusEnum.Active))
                                       .ProjectTo<PostResponse>(_mapper.ConfigurationProvider)
                                       .DynamicFilter(filter)
                                       .DynamicSort(paging.Sort, paging.Order)
                                       .PagingQueryable(paging.Page, paging.PageSize);

                        var filterStatus = searchPremiumPost.Item2.Where(x => x.PostCode.Contains(search) || x.PostCategory.PostCategoryDescription.Contains(search)
                                                                                    || x.PostDescription.Contains(search)
                                                                                    || x.PostPositions.Any(x => x.SchoolName.Contains(search))
                                                                                    || x.PostPositions.Any(x => x.Location.Contains(search)));

                        var postPremiumSearchResponses = await filterStatus.ToListAsync();

                        var postSearchAfterCounting = await CountPostInformation(postPremiumSearchResponses);

                        return new BaseResponsePagingViewModel<PostResponse>()
                        {
                            Metadata = new PagingsMetadata()
                            {
                                Page = paging.Page,
                                Size = paging.PageSize,
                                Total = searchPremiumPost.Item1
                            },
                            //Data = postPremiumResponses.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Priority).ToList()
                            Data = postSearchAfterCounting
                        };
                    }

                    var premiumPost = _unitOfWork.Repository<Post>().GetAll()
                                       .Where(x => x.Status == (int)PostStatusEnum.Re_Open)
                                       .Include(x => x.PostPositions.Where(x => x.Status == (int)PostPositionStatusEnum.Active))
                                       .ProjectTo<PostResponse>(_mapper.ConfigurationProvider)
                                       .DynamicFilter(filter)
                                       .DynamicSort(paging.Sort, paging.Order);

                    var premiumList = FilterPostDateFrom(premiumPost, timeFromFilter).PagingQueryable(paging.Page, paging.PageSize);
                    var premiumResponses = await premiumList.Item2.ToListAsync();

                    var premiumPostCounting = await CountPostInformation(premiumResponses);

                    return new BaseResponsePagingViewModel<PostResponse>()
                    {
                        Metadata = new PagingsMetadata()
                        {
                            Page = paging.Page,
                            Size = paging.PageSize,
                            Total = premiumList.Item1
                        },
                        //Data = postPremiumResponses.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Priority).ToList()
                        Data = premiumPostCounting
                    };
                }

                if (!string.IsNullOrEmpty(search))
                {
                    var searchPost = _unitOfWork.Repository<Post>().GetAll()
                                    .Where(x => x.Status == (int)PostStatusEnum.Re_Open)
                                    .Include(x => x.PostPositions.Where(x => x.Status == (int)PostPositionStatusEnum.Active))
                                    .ProjectTo<PostResponse>(_mapper.ConfigurationProvider)
                                    .DynamicFilter(filter)
                                    .DynamicSort(paging.Sort, paging.Order)
                                    .PagingQueryable(paging.Page, paging.PageSize);

                    var filterStatus = searchPost.Item2.Where(x => x.PostCode.Contains(search) || x.PostCategory.PostCategoryDescription.Contains(search)
                                                                                            || x.PostDescription.Contains(search)
                                                                                            || x.PostPositions.Any(x => x.SchoolName.Contains(search))
                                                                                            || x.PostPositions.Any(x => x.Location.Contains(search)));

                    var postPremiumSearchResponses = await filterStatus.ToListAsync();

                    var postSearchResponses = await searchPost.Item2.ToListAsync();

                    var regularSearchPostAfterCounting = await CountPostInformation(postSearchResponses);

                    return new BaseResponsePagingViewModel<PostResponse>()
                    {
                        Metadata = new PagingsMetadata()
                        {
                            Page = paging.Page,
                            Size = paging.PageSize,
                            Total = searchPost.Item1
                        },
                        //Data = postResponses.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Priority).ToList()
                        Data = regularSearchPostAfterCounting
                    };
                }

                var posts = _unitOfWork.Repository<Post>().GetAll()
                                        .Where(x => x.Status == (int)PostStatusEnum.Re_Open)
                                        .Include(x => x.PostPositions.Where(x => x.Status == (int)PostPositionStatusEnum.Active))
                                        .ProjectTo<PostResponse>(_mapper.ConfigurationProvider)
                                        .DynamicFilter(filter)
                                        .DynamicSort(paging.Sort, paging.Order);
                //.PagingQueryable(paging.Page, paging.PageSize);

                var regularPostsFilter = FilterPostDateFrom(posts, timeFromFilter).PagingQueryable(paging.Page, paging.PageSize);
                var responses = await regularPostsFilter.Item2.ToListAsync();

                var regularPostsAfterCounting = await CountPostInformation(responses);

                return new BaseResponsePagingViewModel<PostResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = regularPostsFilter.Item1
                    },
                    //Data = postResponses.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Priority).ToList()
                    Data = regularPostsAfterCounting
                };

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region Private logic 

        private static IQueryable<PostResponse> FilterPostDateFrom(IQueryable<PostResponse> list, TimeFromFilter filter)
        {
            if (filter.DateFromEnd != null && filter.DateFromEnd.HasValue && filter.DateFromStart != null && filter.DateFromStart.HasValue)
            {
                //filter here
                list = list.Where(post => post.DateFrom >= filter.DateFromStart && post.DateFrom <= filter.DateFromEnd);

            }

            if (filter.CreateAtEnd != null && filter.CreateAtEnd.HasValue && filter.CreateAtStart != null && filter.CreateAtStart.HasValue)
            {
                //filter here
                list = list.Where(post => post.CreateAt >= filter.CreateAtStart && post.DateFrom <= filter.CreateAtEnd);

            }

            //int size = list.Count();
            return list;
        }

        #endregion
    }
}
