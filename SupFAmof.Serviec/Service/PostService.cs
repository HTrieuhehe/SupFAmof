using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;
using NTQ.Sdk.Core.Utilities;
using Service.Commons;
using SupFAmof.Data.Entity;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.DTO.Request;
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

                foreach (var item in post.PostPositions)
                {
                    //tìm kiếm các post Regist có cùng position Id để so sánh giữa amount và số lượng đk
                    var totalPostRegist = _unitOfWork.Repository<PostRegistration>().GetAll()
                                                     .Include(x => x.PostRegistrationDetails)
                                                     .Where(x => x.PostRegistrationDetails.Any(pd => pd.PositionId == item.Id) && x.Status == (int)PostRegistrationStatusEnum.Confirm);

                    if (totalPostRegist.Count() != item.Amount)
                    {
                        throw new ErrorResponse(404, (int)PostErrorEnum.INVALID_ENDING_POST,
                                            PostErrorEnum.INVALID_ENDING_POST.GetDisplayName());
                    }
                }

                //nếu dữ liệu trùng khớp thì tiến hành close form đăng ký
                post.Status = (int)PostStatusEnum.Closed;
                post.UpdateAt = DateTime.Now;

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

        public async Task<BaseResponseViewModel<AdmissionPostResponse>> ClosePost(int accountId, int postId)
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

                post.Status = (int)PostStatusEnum.Closed;
                post.UpdateAt = DateTime.Now;

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

                //validate Date
                //request DateFrom must be greater than Current time or before 12 hours before event start
                if (request.DateFrom <= DateTime.Now)
                {
                    throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_DATE_CREATE_POST,
                                         PostErrorEnum.INVALID_DATE_CREATE_POST.GetDisplayName());
                }

                else if (request.DateTo.HasValue && request.DateTo < request.DateFrom)
                {
                    throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_DATETIME_CREATE_POST,
                                         PostErrorEnum.INVALID_DATETIME_CREATE_POST.GetDisplayName());
                }

                //validate Time
                if (request.TimeFrom < TimeSpan.FromHours(3) || request.TimeFrom > TimeSpan.FromHours(20))
                {
                    throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_TIME_CREATE_POST,
                                         PostErrorEnum.INVALID_TIME_CREATE_POST.GetDisplayName());
                }

                if (request.TimeTo.HasValue)
                {
                    if (request.TimeTo <= request.TimeFrom)
                    {
                        throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_TIME_CREATE_POST,
                                         PostErrorEnum.INVALID_TIME_CREATE_POST.GetDisplayName());
                    }
                }

                var post = _mapper.Map<Post>(request);

                post.PostCode = Ultils.GenerateRandomCode();
                post.AccountId = accountId;
                post.AttendanceComplete = false;
                //post.IsActive = true;
                //post.IsEnd = false;
                post.CreateAt = Ultils.GetCurrentTime();

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

        public async Task<BaseResponsePagingViewModel<AdmissionPostResponse>> GetAdmissionPosts(AdmissionPostResponse filter, PagingRequest paging)
        {
            try
            {
                var post = _unitOfWork.Repository<Post>().GetAll()
                                    .ProjectTo<AdmissionPostResponse>(_mapper.ConfigurationProvider)
                                    .DynamicFilter(filter)
                                    .DynamicSort(filter)
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

        public Task<BaseResponseViewModel<AdmissionPostResponse>> UpdateAdmissionPost
            (int accountId, int postId, UpdatePostRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponsePagingViewModel<CollaboratorAccountReponse>> GetAccountByPostPositionId(int positionId, PagingRequest paging)
        {
            try
            {
                var account = _unitOfWork.Repository<Account>().GetAll()
                                        .Include(a => a.PostRegistrations) // Nạp danh sách PostRegistrations của mỗi Account
                                            .ThenInclude(pr => pr.PostRegistrationDetails) // Nạp thông tin chi tiết đăng ký
                                                .ThenInclude(prd => prd.Post) // Nạp thông tin Post của chi tiết đăng ký
                                                    .Where(account => account.PostRegistrations.Any(pr => pr.PostRegistrationDetails
                                                                        .Any(prd => prd.PositionId == positionId) || pr.Status == (int)PostRegistrationStatusEnum.Pending))
                                                    .OrderByDescending(account => account.PostRegistrations.Max(pr => pr.CreateAt)) // Sắp xếp theo CreateAt của PostRegistration
                                                    .ProjectTo<CollaboratorAccountReponse>(_mapper.ConfigurationProvider)
                                                    .PagingQueryable(paging.Page, paging.PageSize,
                                                    Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<CollaboratorAccountReponse>()
                {

                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = account.Item1
                    },
                    Data = account.Item2.ToList(),
                };
            }
            catch (Exception ex)
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
                var checkAccount = _unitOfWork.Repository<Account>().GetAll().FirstOrDefault(a => a.Id == accountId);

                if (checkAccount == null)
                {
                    throw new ErrorResponse(400, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                         AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                //check nếu tài khoảng premium mới cho coi post premium
                else if (checkAccount.IsPremium == true)
                {
                    var premiumPost = _unitOfWork.Repository<Post>().GetAll()
                                        .Where(x => x.Status == (int)PostStatusEnum.Opening)
                                        .ProjectTo<PostResponse>(_mapper.ConfigurationProvider)
                                        .DynamicFilter(filter)
                                        .DynamicSort(filter)
                                        .OrderByDescending(x => x.Priority)
                                        .PagingQueryable(paging.Page, paging.PageSize,
                                        Constants.LimitPaging, Constants.DefaultPaging);

                    List<PostResponse> premiumPostResponseList = new();
                    foreach (var item in premiumPost.Item2)
                    {
                        //tìm kiếm các post Regist có cùng post Id để lọc là số lượng đăng kí post này
                        var totalPostRegist = _unitOfWork.Repository<PostRegistration>().GetAll()
                                                         .Include(x => x.PostRegistrationDetails)
                                                         .Where(x => x.PostRegistrationDetails.Any(pd => pd.PostId == item.Id) && x.Status == (int)PostRegistrationStatusEnum.Confirm);

                        totalCount = totalPostRegist.Count();
                        item.RegisterAmount = totalCount;
                        totalCount = 0;

                        foreach (var itemDetail in item.PostPositions)
                        {
                            var postregistDetail = _unitOfWork.Repository<PostRegistrationDetail>().GetAll()
                                .Include(x => x.PostRegistration)
                                .Where(x => x.PositionId == itemDetail.Id && x.PostRegistration.Status == (int)PostRegistrationStatusEnum.Confirm);
                            if (postregistDetail.Any(x => x.PositionId == itemDetail.Id))
                            {
                                itemDetail.RegisterAmount = postregistDetail.Count();
                            }
                        }

                        premiumPostResponseList.Add(item);
                    }

                    return new BaseResponsePagingViewModel<PostResponse>()
                    {
                        Metadata = new PagingsMetadata()
                        {
                            Page = paging.Page,
                            Size = paging.PageSize,
                            Total = premiumPost.Item1
                        },
                        Data = premiumPostResponseList.ToList()
                    };
                }

                var post = _unitOfWork.Repository<Post>().GetAll()
                                        .Where(x => x.IsPremium != true || x.Status == (int)PostStatusEnum.Opening)
                                        .OrderByDescending(x => x.Priority)
                                        .ProjectTo<PostResponse>(_mapper.ConfigurationProvider)
                                        .DynamicFilter(filter)
                                        .DynamicSort(filter)
                                        .PagingQueryable(paging.Page, paging.PageSize,
                                        Constants.LimitPaging, Constants.DefaultPaging);

                List<PostResponse> postResponseList = new();
                foreach (var item in post.Item2)
                {
                    //tìm kiếm các post Regist có cùng post Id để lọc là số lượng đăng kí post này
                    var totalPostRegist = _unitOfWork.Repository<PostRegistration>().GetAll()
                                                     .Include(x => x.PostRegistrationDetails)
                                                     .Where(x => x.PostRegistrationDetails.Any(pd => pd.PostId == item.Id) && x.Status == (int)PostRegistrationStatusEnum.Confirm);

                    totalCount = totalPostRegist.Count();
                    item.RegisterAmount = totalCount;
                    totalCount = 0;

                    foreach (var itemDetail in item.PostPositions)
                    {
                        var postregistDetail = _unitOfWork.Repository<PostRegistrationDetail>().GetAll()
                            .Include(x => x.PostRegistration)
                            .Where(x => x.PositionId == itemDetail.Id && x.PostRegistration.Status == (int)PostRegistrationStatusEnum.Confirm);
                        if (postregistDetail.Any(x => x.PositionId == itemDetail.Id))
                        {
                            itemDetail.RegisterAmount = postregistDetail.Count();
                        }
                    }

                    postResponseList.Add(item);
                }

                return new BaseResponsePagingViewModel<PostResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = post.Item1
                    },
                    Data = postResponseList.ToList()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<PostResponse>> GetPostByCode(string searchPost, PagingRequest paging)
        {
            try
            {
                var post = _unitOfWork.Repository<Post>().GetAll()
                                                         .ProjectTo<PostResponse>(_mapper.ConfigurationProvider)
                                                         .Where(x => x.PostCode.Contains(searchPost) || x.PostTitle.PostTitleDescription.Contains(searchPost))
                                                         .OrderByDescending(x => x.CreateAt)
                                                         .OrderByDescending(x => x.Priority)
                                                         .PagingQueryable(paging.Page, paging.PageSize,
                                                            Constants.LimitPaging, Constants.DefaultPaging);

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

        #endregion
    }
}
