using AutoMapper;
using AutoMapper.QueryableExtensions;
using LAK.Sdk.Core.Utilities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.HPRtree;
using Service.Commons;
using SupFAmof.Data.Entity;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Account;
using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Service.ServiceInterface;
using SupFAmof.Service.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using static SupFAmof.Service.Helpers.Enum;
using static SupFAmof.Service.Helpers.ErrorEnum;

namespace SupFAmof.Service.Service
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PostService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponseViewModel<AdmissionPostResponse>> EndPost(int accountId, int postId)
        {
            try
            {
                var account = _unitOfWork.Repository<Account>().GetAll().FirstOrDefault(x => x.Id == accountId);

                if (account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                var post = _unitOfWork.Repository<Post>().Find(x => x.Id == postId && x.AccountId == accountId);

                if (post == null)
                {
                    throw new ErrorResponse(404, (int)PostErrorEnum.NOT_FOUND_ID,
                                        PostErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                //validate nếu post chưa xong thì không được end
                if (post.DateFrom > Ultils.GetCurrentDatetime() 
                    || post.DateTo > Ultils.GetCurrentDatetime() 
                    || post.PostPositions.Any(x => x.TimeFrom >= Ultils.GetCurrentDatetime().TimeOfDay 
                                                || x.TimeTo > Ultils.GetCurrentDatetime().TimeOfDay))
                {
                    throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_END_POST,
                                        PostErrorEnum.INVALID_END_POST.GetDisplayName());
                }

                post.Status = (int)PostStatusEnum.Ended;
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

                var timeCheck = post.PostPositions.Min(p => p.TimeFrom);

                if (timeCheck >= Ultils.GetCurrentDatetime().TimeOfDay || timeCheck < Ultils.GetCurrentDatetime().AddMinutes(-30).TimeOfDay)
                {
                    throw new ErrorResponse(400, (int)PostErrorEnum.RE_OPEN_POST_FAIL,
                                            PostErrorEnum.RE_OPEN_POST_FAIL.GetDisplayName());
                }

                //reopen event để cho phép apply đăng ký
                post.Status = (int)PostStatusEnum.Opening;
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

        public async Task<BaseResponseViewModel<AdmissionPostResponse>> CreateAdmissionPost
            (int accountId, CreatePostRequest request)
        {
            try
            {
                var checkAccount = _unitOfWork.Repository<Account>()
                                              .GetAll()
                                              .FirstOrDefault(a => a.Id == accountId);

                if (checkAccount == null)
                {
                    throw new ErrorResponse(400, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                         AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                else if (checkAccount.PostPermission == false)
                {
                    throw new ErrorResponse(400, (int)AccountErrorEnums.POST_PERMIT_NOT_ALLOWED,
                                         AccountErrorEnums.POST_PERMIT_NOT_ALLOWED.GetDisplayName());
                }

                //check document
                foreach (var position in request.PostPositions)
                {
                    var checkdocument = await _unitOfWork.Repository<DocumentTemplate>().GetAll().FirstOrDefaultAsync(x => x.Id == position.DocumentId);

                    if (position.DocumentId == null)
                    {
                        continue;
                    }

                    else if(checkdocument == null)
                    {
                        throw new ErrorResponse(400, (int)DocumentErrorEnum.NOT_FOUND_DOCUMENT,
                                         DocumentErrorEnum.NOT_FOUND_DOCUMENT.GetDisplayName() + $"in Position Name: {position.PositionName}");
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
                    // Lấy danh sách PostAttendee 
                    var totalPostAttendee = _unitOfWork.Repository<PostAttendee>().GetAll()
                                             .Where(x => x.PostId == item.Id)
                                             .ToList();

                    // Tính toán các trường cần thiết
                    item.RegisterAmount = totalPostAttendee.Count;

                    foreach (var itemDetail in item.PostPositions)
                    {
                        //count register amount in post attendee based on position
                        totalCount += CountRegisterAmount(itemDetail.Id, totalPostAttendee);

                        //transafer data to field in post position
                        itemDetail.RegisterAmount = totalCount;

                        //add number of amount required to total amount of a specific post
                        totalAmountPosition += itemDetail.Amount;
                    }

                    //transfer data from position after add to field in post
                    item.TotalAmountPosition = totalAmountPosition;

                    // Reset temp variable
                    totalCount = 0;
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

        public async Task<BaseResponsePagingViewModel<AdmissionPostResponse>> GetPostByAccountId(int accountId, PagingRequest paging)
        {
            try
            {
                var post = _unitOfWork.Repository<Post>().GetAll()
                                      .ProjectTo<AdmissionPostResponse>(_mapper.ConfigurationProvider)
                                      .Where(x => x.AccountId == accountId)
                                      .OrderByDescending(x => x.CreateAt)
                                      .PagingQueryable(paging.Page, paging.PageSize,
                                            Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<AdmissionPostResponse>()
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
                var checkPost = _unitOfWork.Repository<Post>()
                                        .Find(x => x.Id == postId && x.AccountId == accountId);

                if (checkPost == null)
                {
                    throw new ErrorResponse(404, (int)PostErrorEnum.NOT_FOUND_ID,
                                         PostErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                //check if anyone apply to any position
                var postRegistration = _unitOfWork.Repository<PostRegistrationDetail>().GetAll()
                                                    .FirstOrDefault(x => x.PostId == postId);
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
                            throw new ErrorResponse(404, (int)PostErrorEnum.POSITION_NOT_FOUND,
                                         PostErrorEnum.POSITION_NOT_FOUND.GetDisplayName());
                        }

                        item.Id = item.Id;
                        item.PositionName = checkPosition.PositionName;
                        item.SchoolName = checkPosition.SchoolName;
                        item.Location = checkPosition.Location;
                        item.Latitude = checkPosition.Latitude;
                        item.Longtitude = checkPosition.Longtitude;
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
                        throw new ErrorResponse(404, (int)PostErrorEnum.POSITION_NOT_FOUND,
                                     PostErrorEnum.POSITION_NOT_FOUND.GetDisplayName());
                    }

                    item.Id = item.Id;
                    item.PositionName = checkPosition.PositionName;
                    item.SchoolName = checkPosition.SchoolName;
                    item.Location = checkPosition.Location;
                    item.Latitude = checkPosition.Latitude;
                    item.Longtitude = checkPosition.Longtitude;
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

                var checkApplied = await _unitOfWork.Repository<PostRegistrationDetail>().GetAll().FirstOrDefaultAsync(x => x.PositionId == positionId);

                if (checkApplied != null)
                {
                    throw new ErrorResponse(400, (int)PostErrorEnum.UPDATE_FAIl,
                                         PostErrorEnum.UPDATE_FAIl.GetDisplayName());
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

                var checkApplied = await _unitOfWork.Repository<PostRegistrationDetail>().GetAll().FirstOrDefaultAsync(x => x.PostId == postId);

                if (checkApplied != null)
                {
                    throw new ErrorResponse(400, (int)PostErrorEnum.UPDATE_FAIl,
                                         PostErrorEnum.UPDATE_FAIl.GetDisplayName());
                }

                post.Status = (int)PostStatusEnum.Delete;

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

        #endregion

        #region Collaborator Post Service

        public async Task<BaseResponsePagingViewModel<PostResponse>> GetPosts(int accountId, PostResponse filter, PagingRequest paging)
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
                    var premiumPost = _unitOfWork.Repository<Post>().GetAll()
                                       .Where(x => x.Status == (int)PostStatusEnum.Opening && x.Status == (int)PostStatusEnum.Avoid_Regist)
                                       .Include(x => x.PostPositions.Where(x => x.Status == (int)PostPositionStatusEnum.Active))
                                       .ProjectTo<PostResponse>(_mapper.ConfigurationProvider)
                                       .DynamicFilter(filter)
                                       .DynamicSort(paging.Sort, paging.Order)
                                       .PagingQueryable(paging.Page, paging.PageSize);

                    var postPremiumResponses = premiumPost.Item2.ToList();

                    foreach (var item in postPremiumResponses)
                    {
                        // Lấy danh sách PostAttendee 
                        var totalPostPremiumAttendee = _unitOfWork.Repository<PostAttendee>().GetAll()
                                                 .Where(x => x.PostId == item.Id)
                                                 .ToList();

                        // Tính toán các trường cần thiết
                        item.RegisterAmount = totalPostPremiumAttendee.Count;

                        item.TimeFrom = item.PostPositions.Min(p => p.TimeFrom).ToString();
                        item.TimeTo = item.PostPositions.Max(p => p.TimeTo).ToString();

                        foreach (var itemDetail in item.PostPositions)
                        {
                            //count register amount in post attendee based on position
                            totalCount += CountRegisterAmount(itemDetail.Id, totalPostPremiumAttendee);

                            //transafer data to field in post position
                            itemDetail.RegisterAmount = totalCount;

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
                            Total = premiumPost.Item1
                        },
                        //Data = postPremiumResponses.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Priority).ToList()
                        Data = postPremiumResponses.ToList()
                    };
                }

                var post = _unitOfWork.Repository<Post>().GetAll()
                                        .Where(x => x.Status >= (int)PostStatusEnum.Opening && x.Status <= (int)PostStatusEnum.Avoid_Regist)
                                        .Include(x => x.PostPositions.Where(x => x.Status == (int)PostPositionStatusEnum.Active))
                                        .ProjectTo<PostResponse>(_mapper.ConfigurationProvider)
                                        .DynamicFilter(filter)
                                        .DynamicSort(paging.Sort, paging.Order)
                                        .PagingQueryable(paging.Page, paging.PageSize);

                #region Old Code

                //List<PostResponse> postResponseList = new();
                //foreach (var item in post.Item2)
                //{
                //    //tìm kiếm các post Attendee có cùng post Id để lọc là số lượng đăng kí post này
                //    var totalPostAttendee = _unitOfWork.Repository<PostAttendee>().GetAll()
                //                                     .Where(x => x.PostId == item.Id);

                //    totalCount = totalPostAttendee.Count();
                //    //item.RegisterAmount = 0;
                //    item.RegisterAmount = totalCount;
                //    totalCount = 0;

                //    foreach (var itemDetail in item.PostPositions)
                //    {
                //        //vào từng obj của attendee để count
                //        foreach (var attendeeDetail in totalPostAttendee)
                //        {
                //            if (attendeeDetail.PositionId == itemDetail.Id)
                //            {
                //                totalCount++;
                //            }
                //        }
                //        itemDetail.RegisterAmount = totalCount;
                //        totalAmountPosition += itemDetail.Amount;
                //    }
                //    item.TotalAmountPosition = totalAmountPosition;
                //    totalAmountPosition = 0;
                //    postResponseList.Add(item);
                //}

                #endregion

                var postResponses = post.Item2.ToList();

                foreach (var item in postResponses)
                {
                    // Lấy danh sách PostAttendee 
                    var totalPostAttendee = _unitOfWork.Repository<PostAttendee>().GetAll()
                                             .Where(x => x.PostId == item.Id)
                                             .ToList();

                    // Tính toán các trường cần thiết
                    item.RegisterAmount = totalPostAttendee.Count;

                    item.TimeFrom = item.PostPositions.Min(p => p.TimeFrom).ToString();
                    item.TimeTo = item.PostPositions.Max(p => p.TimeTo).ToString();

                    foreach (var itemDetail in item.PostPositions)
                    {
                        //count register amount in post attendee based on position
                        totalCount += CountRegisterAmount(itemDetail.Id, totalPostAttendee);

                        //transafer data to field in post position
                        itemDetail.RegisterAmount = totalCount;

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

                switch(status)
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

        private static int CountRegisterAmount(int positionId, List<PostAttendee> postAttendees)
        {
            return postAttendees.Count(x => x.PositionId == positionId);
        }

        public async Task<BaseResponsePagingViewModel<PostResponse>> GetPostsMissingSlot(int accountId, PostResponse filter, PagingRequest paging)
        {
            try
            {
                var checkAccount = _unitOfWork.Repository<Account>()
                                              .GetAll()
                                              .FirstOrDefault(a => a.Id == accountId);

                if (checkAccount == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                         AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                //premium account check

                if (checkAccount.IsPremium == true)
                {
                    return null;
                }

                //regular account check

                var post = _unitOfWork.Repository<Post>().GetAll()
                                       .Where(x => x.IsPremium != true && x.Status == (int)PostStatusEnum.Opening)
                                       .Include(x => x.PostPositions.Where(x => x.Status == (int)PostPositionStatusEnum.Active))
                                       .ProjectTo<PostResponse>(_mapper.ConfigurationProvider)
                                       .DynamicFilter(filter)
                                       .DynamicSort(paging.Sort, paging.Order)
                                       .PagingQueryable(paging.Page, paging.PageSize);

                //calculating to remove any post enough position

                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion
    }
}
