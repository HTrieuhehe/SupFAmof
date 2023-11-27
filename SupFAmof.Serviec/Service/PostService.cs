﻿using System;
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

                //check document
                foreach (var position in request.PostPositions)
                {
                    var checkdocument = await _unitOfWork.Repository<DocumentTemplate>().GetAll().FirstOrDefaultAsync(x => x.Id == position.DocumentId);

                    if (position.DocumentId == null)
                    {
                        continue;
                    }

                    else if (checkdocument == null)
                    {
                        throw new ErrorResponse(400, (int)DocumentErrorEnum.NOT_FOUND_DOCUMENT,
                                         DocumentErrorEnum.NOT_FOUND_DOCUMENT.GetDisplayName() + $"in Position Name: {position.PositionName}");
                    }

                    //validate date in range
                    if (request.DateFrom > position.Date || request.DateTo < position.Date)
                    {
                        throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_POSITION_DATE,
                                         PostErrorEnum.INVALID_POSITION_DATE.GetDisplayName());
                    }
                    continue;
                }

                //validate Date
                //request DateFrom must be greater than Current time or before 12 hours before event start
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

                foreach (var item in request.PostPositions)
                {
                    //validate Certificate
                    var checkCerti = _unitOfWork.Repository<TrainingCertificate>().GetAll().FirstOrDefault(x => x.Id == item.TrainingCertificateId);

                    if (item.TrainingCertificateId > 0 && checkCerti == null)
                    {
                        throw new ErrorResponse(400, (int)TrainingCertificateErrorEnum.NOT_FOUND_ID,
                                             TrainingCertificateErrorEnum.NOT_FOUND_ID.GetDisplayName());
                    }

                    //validate Time
                    if (item.TimeFrom < TimeSpan.FromHours(3) || item.TimeFrom > TimeSpan.FromHours(20))
                    {
                        throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_TIME_CREATE_POST,
                                             PostErrorEnum.INVALID_TIME_CREATE_POST.GetDisplayName());
                    }

                    if (item.TimeTo.HasValue)
                    {
                        if (item.TimeTo <= item.TimeFrom)
                        {
                            throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_TIME_CREATE_POST,
                                             PostErrorEnum.INVALID_TIME_CREATE_POST.GetDisplayName());
                        }
                    }
                    item.Status = (int)PostPositionStatusEnum.Active;
                }

                var post = _mapper.Map<Post>(request);

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
                int? totalCount = 0;
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
                    var postRegistrations = _unitOfWork.Repository<PostRegistration>()
                                                        .GetAll()
                                                        .Where(reg => postPositionIds.Contains(reg.PositionId) && reg.Status == (int)PostRegistrationStatusEnum.Confirm)
                                                        .ToList();

                    // Tính toán các trường cần thiết
                    item.RegisterAmount = postRegistrations.Count;

                    foreach (var itemDetail in item.PostPositions)
                    {
                        //count register amount in post attendee based on position
                        totalCount += CountRegisterAmount(itemDetail.Id, postRegistrations);

                        //transafer data to field in post position
                        itemDetail.PositionRegisterAmount = totalCount;

                        //add number of amount required to total amount of a specific post
                        totalAmountPosition += itemDetail.Amount;

                        // Reset temp variable
                        totalCount = 0;
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
                int? totalCount = 0;
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
                    var postRegistrations = _unitOfWork.Repository<PostRegistration>()
                                                        .GetAll()
                                                        .Where(reg => postPositionIds.Contains(reg.PositionId) && reg.Status == (int)PostRegistrationStatusEnum.Confirm)
                                                        .ToList();

                    // Tính toán các trường cần thiết
                    item.RegisterAmount = postRegistrations.Count;

                    foreach (var itemDetail in item.PostPositions)
                    {
                        //count register amount in post attendee based on position
                        totalCount += CountRegisterAmount(itemDetail.Id, postRegistrations);

                        //transafer data to field in post position
                        itemDetail.PositionRegisterAmount = totalCount;

                        //add number of amount required to total amount of a specific post
                        totalAmountPosition += itemDetail.Amount;

                        // Reset temp variable
                        totalCount = 0;

                        //tìm post update history có status pending
                        var updateRegistration = _unitOfWork.Repository<PostRgupdateHistory>()
                                .GetAll().Where(x => x.PositionId == itemDetail.Id && x.Status == (int)PostRGUpdateHistoryEnum.Pending);

                        item.TotalUpdateRegisterAmount += updateRegistration.Count();
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

                var postPositionIds = checkPost.PostPositions.Select(p => p.Id).ToList();

                //check if anyone apply to any position
                var postRegistration = _unitOfWork.Repository<PostRegistration>().GetAll()
                                                    .FirstOrDefault(x => postPositionIds.Contains(x.PositionId));
                checkPost.Id = postId;
                checkPost.AccountId = accountId;
                checkPost.PostCategoryId = request.PostCategoryId;
                checkPost.PostDescription = request.PostDescription;
                checkPost.PostImg = request.PostImg;
                checkPost.UpdateAt = Ultils.GetCurrentDatetime();

                if (postRegistration == null)
                {
                    // allow update salary because there is a post registration in that post
                    foreach (var item in checkPost.PostPositions)
                    {
                        var checkPosition = request.PostPositions
                                    .FirstOrDefault(x => x.Id == item.Id);
                        if (checkPosition == null)
                        {
                            continue;
                        }

                        item.Id = item.Id;
                        item.PositionName = checkPosition.PositionName;
                        item.SchoolName = checkPosition.SchoolName;
                        item.Location = checkPosition.Location;
                        item.Latitude = checkPosition.Latitude;
                        item.Longitude = checkPosition.Longitude;
                        item.Amount = checkPosition.Amount;
                        item.Salary = checkPosition.Salary;
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

                foreach (var item in checkPost.PostPositions)
                {
                    var checkPosition = request.PostPositions
                                .FirstOrDefault(x => x.Id == item.Id);
                    if (checkPosition == null)
                    {
                        continue;
                    }

                    item.Id = item.Id;
                    item.PositionName = checkPosition.PositionName;
                    item.SchoolName = checkPosition.SchoolName;
                    item.Location = checkPosition.Location;
                    item.Latitude = checkPosition.Latitude;
                    item.Longitude = checkPosition.Longitude;
                    item.Amount = checkPosition.Amount;
                    item.Salary = checkPosition.Salary;
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
                //make sure position is update status by someone who created them
                var postPosition = await _unitOfWork.Repository<PostPosition>()
                                    .FindAsync(x => x.Id == positionId && x.Post.AccountId == accountId && x.Post.Id == x.PostId);

                if (postPosition == null)
                {
                    throw new ErrorResponse(404, (int)PostErrorEnum.NOT_FOUND_ID,
                                         PostErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                //validate to makesure there is no applied in this position

                var checkApplied = await _unitOfWork.Repository<PostRegistration>().GetAll().FirstOrDefaultAsync(x => x.PositionId == positionId);

                if (checkApplied != null)
                {
                    throw new ErrorResponse(400, (int)PostErrorEnum.UPDATE_FAIL,
                                         PostErrorEnum.UPDATE_FAIL.GetDisplayName());
                }

                postPosition.Status = (int)PostPositionStatusEnum.Delete;

                await _unitOfWork.Repository<PostPosition>().UpdateDetached(postPosition);
                await _unitOfWork.CommitAsync();

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

                var checkApplied = await _unitOfWork.Repository<PostRegistration>()
                                                    .GetAll()
                                                    .FirstOrDefaultAsync(x => postPositionIds.Contains(x.PositionId));

                if (checkApplied != null)
                {
                    throw new ErrorResponse(400, (int)PostErrorEnum.UPDATE_FAIL,
                                         PostErrorEnum.UPDATE_FAIL.GetDisplayName());
                }

                post.Status = (int)PostStatusEnum.Delete;
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
                int totalCount = 0;
                int? totalAmountPosition = 0;
                var checkAccount = await _unitOfWork.Repository<Account>().GetAll().FirstOrDefaultAsync(a => a.Id == accountId);

                if (checkAccount == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                         AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                //check nếu tài khoảng premium mới cho coi post premium
                else if (checkAccount.IsPremium == true)
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

                        foreach (var item in postPremiumSearchResponses)
                        {
                            // lấy tất cả các position Id của bài post hiện tại
                            var premiumPostPositionIds = item.PostPositions.Select(p => p.Id).ToList();

                            // tìm post Registration có position Id trung với các bài post
                            var premiumPostRegistrations = await _unitOfWork.Repository<PostRegistration>()
                                .GetAll()
                                .Where(reg => premiumPostPositionIds.Contains(reg.PositionId) && reg.Status == (int)PostRegistrationStatusEnum.Confirm)
                                .ToListAsync();

                            // tính tổng các registration đã được confirm
                            item.RegisterAmount = premiumPostRegistrations.Count;

                            //lấy thời gian min max
                            item.TimeFrom = item.PostPositions.Min(p => p.TimeFrom).ToString();
                            item.TimeTo = item.PostPositions.Max(p => p.TimeTo).ToString();

                            foreach (var itemDetail in item.PostPositions)
                            {
                                //count register amount in post attendee based on position
                                totalCount += CountRegisterAmount(itemDetail.Id, premiumPostRegistrations);

                                //transafer data to field in post position
                                itemDetail.PositionRegisterAmount = totalCount;

                                //add number of amount required to total amount of a specific post
                                totalAmountPosition += itemDetail.Amount;

                                // Reset temp variable
                                totalCount = 0;
                            }
                            //transfer data from position after add to field in post
                            item.TotalAmountPosition = totalAmountPosition;

                            // Reset temp variable
                            totalAmountPosition = 0;
                        }

                        return new BaseResponsePagingViewModel<PostResponse>()
                        {
                            Metadata = new PagingsMetadata()
                            {
                                Page = paging.Page,
                                Size = paging.PageSize,
                                Total = postPremiumSearchResponses.Count()
                            },
                            //Data = postPremiumResponses.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Priority).ToList()
                            Data = postPremiumSearchResponses
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

                    foreach (var item in response)
                    {
                        // lấy tất cả các position Id của bài post hiện tại
                        var premiumPostPositionIds = item.PostPositions.Select(p => p.Id).ToList();

                        // tìm post Registration có position Id trung với các bài post
                        var premiumPostRegistrations = await _unitOfWork.Repository<PostRegistration>()
                            .GetAll()
                            .Where(reg => premiumPostPositionIds.Contains(reg.PositionId) && reg.Status == (int)PostRegistrationStatusEnum.Confirm)
                            .ToListAsync();

                        // tính tổng các registration đã được confirm
                        item.RegisterAmount = premiumPostRegistrations.Count;

                        //lấy thời gian min max
                        item.TimeFrom = item.PostPositions.Min(p => p.TimeFrom).ToString();
                        item.TimeTo = item.PostPositions.Max(p => p.TimeTo).ToString();

                        foreach (var itemDetail in item.PostPositions)
                        {
                            //count register amount in post attendee based on position
                            totalCount += CountRegisterAmount(itemDetail.Id, premiumPostRegistrations);

                            //transafer data to field in post position
                            itemDetail.PositionRegisterAmount = totalCount;

                            //add number of amount required to total amount of a specific post
                            totalAmountPosition += itemDetail.Amount;

                            // Reset temp variable
                            totalCount = 0;
                        }
                        //transfer data from position after add to field in post
                        item.TotalAmountPosition = totalAmountPosition;

                        // Reset temp variable
                        totalAmountPosition = 0;
                    }

                   

                    return new BaseResponsePagingViewModel<PostResponse>()
                    {
                        Metadata = new PagingsMetadata()
                        {
                            Page = paging.Page,
                            Size = paging.PageSize,
                            Total = premiumList.Item1,
                        },
                        //Data = postPremiumResponses.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Priority).ToList()
                        Data = response
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

                    foreach (var item in postSearchResponses)
                    {
                        //lấy thời gian thấp nhất và cao nhất để hiển thị trên UI
                        item.TimeFrom = item.PostPositions.Min(p => p.TimeFrom).ToString();
                        item.TimeTo = item.PostPositions.Max(p => p.TimeTo).ToString();

                        // lấy tất cả các position Id của bài post hiện tại
                        var postPositionIds = item.PostPositions.Select(p => p.Id).ToList();

                        // tìm post Registration có position Id trung với các bài post
                        var postRegistrations = await _unitOfWork.Repository<PostRegistration>()
                                                            .GetAll()
                                                            .Where(reg => postPositionIds.Contains(reg.PositionId) && reg.Status == (int)PostRegistrationStatusEnum.Confirm)
                                                            .ToListAsync();

                        // tính tổng các registration đã được confirm
                        item.RegisterAmount = postRegistrations.Count;

                        foreach (var itemDetail in item.PostPositions)
                        {
                            //count register amount in post attendee based on position
                            totalCount += CountRegisterAmount(itemDetail.Id, postRegistrations);

                            //transafer data to field in post position
                            itemDetail.PositionRegisterAmount = totalCount;

                            //add number of amount required to total amount of a specific post
                            totalAmountPosition += itemDetail.Amount;
                        }
                        //transfer data from position after add to field in post
                        item.TotalAmountPosition = totalAmountPosition;

                        // Reset temp variable
                        totalCount = 0;
                        totalAmountPosition = 0;
                    }

                    return new BaseResponsePagingViewModel<PostResponse>()
                    {
                        Metadata = new PagingsMetadata()
                        {
                            Page = paging.Page,
                            Size = paging.PageSize,
                            Total = postSearchResponses.Count()
                        },
                        //Data = postResponses.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Priority).ToList()
                        Data = postSearchResponses.ToList()
                    };
                }

                var posts = _unitOfWork.Repository<Post>().GetAll()
                                    .Where(x => x.Status >= (int)PostStatusEnum.Opening
                                             && x.Status <= (int)PostStatusEnum.Avoid_Regist && x.IsPremium == false)
                                    .Include(x => x.PostPositions.Where(x => x.Status == (int)PostPositionStatusEnum.Active))
                                    .ProjectTo<PostResponse>(_mapper.ConfigurationProvider)
                                    .DynamicFilter(filter)
                                    .DynamicSort(paging.Sort, paging.Order);

                var dateFilter = FilterPostDateFrom(posts, timeFromFilter).PagingQueryable(paging.Page, paging.PageSize);
                var data = await dateFilter.Item2.ToListAsync();

                foreach (var item in data)
                {
                    //lấy thời gian thấp nhất và cao nhất để hiển thị trên UI
                    item.TimeFrom = item.PostPositions.Min(p => p.TimeFrom).ToString();
                    item.TimeTo = item.PostPositions.Max(p => p.TimeTo).ToString();

                    // lấy tất cả các position Id của bài post hiện tại
                    var postPositionIds = item.PostPositions.Select(p => p.Id).ToList();

                    // tìm post Registration có position Id trung với các bài post
                    var postRegistrations = await _unitOfWork.Repository<PostRegistration>()
                                                        .GetAll()
                                                        .Where(reg => postPositionIds.Contains(reg.PositionId) && reg.Status == (int)PostRegistrationStatusEnum.Confirm)
                                                        .ToListAsync();

                    // tính tổng các registration đã được confirm
                    item.RegisterAmount = postRegistrations.Count;

                    foreach (var itemDetail in item.PostPositions)
                    {
                        //count register amount in post attendee based on position
                        totalCount += CountRegisterAmount(itemDetail.Id, postRegistrations);

                        //transafer data to field in post position
                        itemDetail.PositionRegisterAmount = totalCount;

                        //add number of amount required to total amount of a specific post
                        totalAmountPosition += itemDetail.Amount;

                        //reset temp count
                        totalCount = 0;
                    }
                    //transfer data from position after add to field in post
                    item.TotalAmountPosition = totalAmountPosition;

                    // Reset temp variable
                    totalAmountPosition = 0;
                }

                return new BaseResponsePagingViewModel<PostResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = dateFilter.Item1
                    },
                    //Data = postResponses.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Priority).ToList()
                    Data = data
                };
            }
            catch (Exception ex)
            {
                throw new ErrorResponse(408, (int)PostErrorEnum.REQUEST_TIME_OUT,
                                         PostErrorEnum.REQUEST_TIME_OUT.GetDisplayName());
            }
        }

        public async Task<BaseResponseViewModel<PostResponse>> GetPostById(int accountId, int postId)
        {
            try
            {
                var checkAccount = await _unitOfWork.Repository<Account>().GetAll().FirstOrDefaultAsync(a => a.Id == accountId);

                if (checkAccount == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                         AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                var post = await _unitOfWork.Repository<Post>().GetAll().FirstOrDefaultAsync(x => x.Id == postId);

                if (post == null)
                {
                    if (post == null)
                    {
                        throw new ErrorResponse(404, (int)PostErrorEnum.NOT_FOUND_ID,
                                             PostErrorEnum.NOT_FOUND_ID.GetDisplayName());
                    }
                }

                return new BaseResponseViewModel<PostResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<PostResponse>(post)
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

        private async Task<List<PostResponse>> GetPostsByStatus(int status, PostResponse filter, PagingRequest paging)
        {
            try
            {
                //1: premium
                //2: regular

                switch (status)
                {
                    case 1:

                        #region Code here

                        var premiumPost = _unitOfWork.Repository<Post>().GetAll()
                                        .Where(x => x.Status == (int)PostStatusEnum.Opening)
                                        .Include(x => x.PostPositions.Where(x => x.Status == (int)PostPositionStatusEnum.Active))
                                        .OrderByDescending(x => x.CreateAt)
                                        .OrderByDescending(x => x.Priority)
                                        .ProjectTo<PostResponse>(_mapper.ConfigurationProvider)
                                        .DynamicFilter(filter)
                                        .DynamicSort(paging.Sort, paging.Order)
                                        .PagingQueryable(paging.Page, paging.PageSize);

                        return premiumPost.Item2.ToList();

                    #endregion

                    case 2:

                        #region Code here

                        var post = _unitOfWork.Repository<Post>().GetAll()
                                        .Where(x => x.IsPremium != true || x.Status == (int)PostStatusEnum.Opening)
                                        .Include(x => x.PostPositions.Where(x => x.Status == (int)PostPositionStatusEnum.Active))
                                        .ProjectTo<PostResponse>(_mapper.ConfigurationProvider)
                                        .OrderByDescending(x => x.CreateAt)
                                        .OrderByDescending(x => x.Priority)
                                        .DynamicFilter(filter)
                                        .DynamicSort(paging.Sort, paging.Order)
                                        .PagingQueryable(paging.Page, paging.PageSize);

                        return post.Item2.ToList();

                    #endregion

                    default:
                        throw new Exception();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static int CountRegisterAmount(int positionId, List<PostRegistration> postRegistrations)
        {
            return postRegistrations.Count(x => x.PositionId == positionId);
        }

        public async Task<BaseResponsePagingViewModel<PostResponse>> GetPostReOpen(int accountId, PostResponse filter, string search, TimeFromFilter timeFromFilter, PagingRequest paging)
        {
            try
            {
                int totalCount = 0;
                int? totalAmountPosition = 0;
                var checkAccount = _unitOfWork.Repository<Account>().GetAll().FirstOrDefault(a => a.Id == accountId);

                if (checkAccount == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                         AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
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

                        foreach (var item in postPremiumSearchResponses)
                        {
                            // lấy tất cả các position Id của bài post hiện tại
                            var premiumPostPositionIds = item.PostPositions.Select(p => p.Id).ToList();

                            // tìm post Registration có position Id trung với các bài post
                            var premiumPostRegistrations = _unitOfWork.Repository<PostRegistration>()
                                .GetAll()
                                .Where(reg => premiumPostPositionIds.Contains(reg.PositionId) && reg.Status == (int)PostRegistrationStatusEnum.Confirm)
                                .ToList();

                            // tính tổng các registration đã được confirm
                            item.RegisterAmount = premiumPostRegistrations.Count;

                            //lấy thời gian min max
                            item.TimeFrom = item.PostPositions.Min(p => p.TimeFrom).ToString();
                            item.TimeTo = item.PostPositions.Max(p => p.TimeTo).ToString();

                            foreach (var itemDetail in item.PostPositions)
                            {
                                //count register amount in post attendee based on position
                                totalCount += CountRegisterAmount(itemDetail.Id, premiumPostRegistrations);

                                //transafer data to field in post position
                                itemDetail.PositionRegisterAmount = totalCount;

                                //add number of amount required to total amount of a specific post
                                totalAmountPosition += itemDetail.Amount;

                                // Reset temp variable
                                totalCount = 0;
                            }
                            //transfer data from position after add to field in post
                            item.TotalAmountPosition = totalAmountPosition;

                            // Reset temp variable
                            totalAmountPosition = 0;
                        }

                        return new BaseResponsePagingViewModel<PostResponse>()
                        {
                            Metadata = new PagingsMetadata()
                            {
                                Page = paging.Page,
                                Size = paging.PageSize,
                                Total = postPremiumSearchResponses.Count()
                            },
                            //Data = postPremiumResponses.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Priority).ToList()
                            Data = postPremiumSearchResponses
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

                    foreach (var item in premiumResponses)
                    {
                        //lấy thời gian thấp nhất và cao nhất để hiển thị trên UI
                        item.TimeFrom = item.PostPositions.Min(p => p.TimeFrom).ToString();
                        item.TimeTo = item.PostPositions.Max(p => p.TimeTo).ToString();

                        // lấy tất cả các position Id của bài post hiện tại
                        var premiumPostPositionIds = item.PostPositions.Select(p => p.Id).ToList();

                        // tìm post Registration có position Id trung với các bài post
                        var premiumPostRegistrations = _unitOfWork.Repository<PostRegistration>()
                            .GetAll()
                            .Where(reg => premiumPostPositionIds.Contains(reg.PositionId) && reg.Status == (int)PostRegistrationStatusEnum.Confirm)
                            .ToList();

                        // tính tổng các registration đã được confirm
                        item.RegisterAmount = premiumPostRegistrations.Count;

                        item.TimeFrom = item.PostPositions.Min(p => p.TimeFrom).ToString();
                        item.TimeTo = item.PostPositions.Max(p => p.TimeTo).ToString();

                        foreach (var itemDetail in item.PostPositions)
                        {
                            //count register amount in post attendee based on position
                            totalCount += CountRegisterAmount(itemDetail.Id, premiumPostRegistrations);

                            //transafer data to field in post position
                            itemDetail.PositionRegisterAmount = totalCount;

                            //add number of amount required to total amount of a specific post
                            totalAmountPosition += itemDetail.Amount;

                            // Reset temp variable
                            totalCount = 0;
                        }
                        //transfer data from position after add to field in post
                        item.TotalAmountPosition = totalAmountPosition;

                        // Reset temp variable
                        totalAmountPosition = 0;
                    }

                    return new BaseResponsePagingViewModel<PostResponse>()
                    {
                        Metadata = new PagingsMetadata()
                        {
                            Page = paging.Page,
                            Size = paging.PageSize,
                            Total = premiumResponses.Count(),
                        },
                        //Data = postPremiumResponses.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Priority).ToList()
                        Data = premiumResponses
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

                    foreach (var item in postSearchResponses)
                    {
                        //lấy thời gian thấp nhất và cao nhất để hiển thị trên UI
                        item.TimeFrom = item.PostPositions.Min(p => p.TimeFrom).ToString();
                        item.TimeTo = item.PostPositions.Max(p => p.TimeTo).ToString();

                        // lấy tất cả các position Id của bài post hiện tại
                        var postPositionIds = item.PostPositions.Select(p => p.Id).ToList();

                        // tìm post Registration có position Id trung với các bài post
                        var postRegistrations = _unitOfWork.Repository<PostRegistration>()
                                                            .GetAll()
                                                            .Where(reg => postPositionIds.Contains(reg.PositionId) && reg.Status == (int)PostRegistrationStatusEnum.Confirm)
                                                            .ToList();

                        // tính tổng các registration đã được confirm
                        item.RegisterAmount = postRegistrations.Count;

                        foreach (var itemDetail in item.PostPositions)
                        {
                            //count register amount in post attendee based on position
                            totalCount += CountRegisterAmount(itemDetail.Id, postRegistrations);

                            //transafer data to field in post position
                            itemDetail.PositionRegisterAmount = totalCount;

                            //add number of amount required to total amount of a specific post
                            totalAmountPosition += itemDetail.Amount;
                        }
                        //transfer data from position after add to field in post
                        item.TotalAmountPosition = totalAmountPosition;

                        // Reset temp variable
                        totalCount = 0;
                        totalAmountPosition = 0;
                    }

                    return new BaseResponsePagingViewModel<PostResponse>()
                    {
                        Metadata = new PagingsMetadata()
                        {
                            Page = paging.Page,
                            Size = paging.PageSize,
                            Total = postSearchResponses.Count()
                        },
                        //Data = postResponses.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Priority).ToList()
                        Data = postSearchResponses.ToList()
                    };
                }

                var posts = _unitOfWork.Repository<Post>().GetAll()
                                        .Where(x => x.Status == (int)PostStatusEnum.Re_Open)
                                        .Include(x => x.PostPositions.Where(x => x.Status == (int)PostPositionStatusEnum.Active))
                                        .ProjectTo<PostResponse>(_mapper.ConfigurationProvider)
                                        .DynamicFilter(filter)
                                        .DynamicSort(paging.Sort, paging.Order);
                                        //.PagingQueryable(paging.Page, paging.PageSize);

                var dateFilter = FilterPostDateFrom(posts, timeFromFilter).PagingQueryable(paging.Page, paging.PageSize);
                var responses = await dateFilter.Item2.ToListAsync();

                foreach (var item in responses)
                {
                    //lấy thời gian thấp nhất và cao nhất để hiển thị trên UI
                    item.TimeFrom = item.PostPositions.Min(p => p.TimeFrom).ToString();
                    item.TimeTo = item.PostPositions.Max(p => p.TimeTo).ToString();

                    // lấy tất cả các position Id của bài post hiện tại
                    var postPositionIds = item.PostPositions.Select(p => p.Id).ToList();

                    // tìm post Registration có position Id trung với các bài post
                    var postRegistrations = _unitOfWork.Repository<PostRegistration>()
                                                        .GetAll()
                                                        .Where(reg => postPositionIds.Contains(reg.PositionId) && reg.Status == (int)PostRegistrationStatusEnum.Confirm)
                                                        .ToList();

                    // tính tổng các registration đã được confirm
                    item.RegisterAmount = postRegistrations.Count;

                    foreach (var itemDetail in item.PostPositions)
                    {
                        //count register amount in post attendee based on position
                        totalCount += CountRegisterAmount(itemDetail.Id, postRegistrations);

                        //transafer data to field in post position
                        itemDetail.PositionRegisterAmount = totalCount;

                        //add number of amount required to total amount of a specific post
                        totalAmountPosition += itemDetail.Amount;

                        //reset temp count
                        totalCount = 0;
                    }
                    //transfer data from position after add to field in post
                    item.TotalAmountPosition = totalAmountPosition;

                    // Reset temp variable
                    totalAmountPosition = 0;
                }

                return new BaseResponsePagingViewModel<PostResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = responses.Count()
                    },
                    //Data = postResponses.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Priority).ToList()
                    Data = responses
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
