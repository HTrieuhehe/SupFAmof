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

        public async Task<BaseResponseViewModel<AdmissionPostResponse>> ConfirmEndingPost(int accountId, int postId)
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

                //else if (post.IsConfirm != true)
                {
                    throw new ErrorResponse(404, (int)PostErrorEnum.INVALID_ENDING_POST,
                                        PostErrorEnum.INVALID_ENDING_POST.GetDisplayName());
                }

                //post.IsEnd = true;
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

        public async Task<BaseResponseViewModel<AdmissionPostResponse>> ConfirmPost(int accountId, int postId)
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

                //post.IsConfirm = true;
                //post.IsEnd = true;
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
                                        .ProjectTo<PostResponse>(_mapper.ConfigurationProvider)
                                        .DynamicFilter(filter)
                                        .DynamicSort(filter)
                                        .OrderByDescending(x => x.Priority)
                                        .PagingQueryable(paging.Page, paging.PageSize,
                                        Constants.LimitPaging, Constants.DefaultPaging);

                    foreach (var register in premiumPost.Item2)
                    {
                        //tìm kiếm các post Regist có cùng post Id để lọc là số lượng đăng kí post này
                        var totalPostRegist = _unitOfWork.Repository<PostRegistration>().GetAll()
                                                    .Where(x => x.PostRegistrationDetails.Any(pd => pd.PostId == register.Id));
                        //register.RegisterAmount = totalPostRegist.Count();

                        //lấy từng Post Registration ra
                        //foreach (var postRegist in totalPostRegist)
                        //{
                        //    //lấy từng Post Registration Detail ra
                        //    foreach (var postRegistDetail in postRegist.PostRegistrationDetails)
                        //    {

                        //    }
                        //}
                    }

                    return new BaseResponsePagingViewModel<PostResponse>()
                    {
                        Metadata = new PagingsMetadata()
                        {
                            Page = paging.Page,
                            Size = paging.PageSize,
                            Total = premiumPost.Item1
                        },
                        Data = premiumPost.Item2.ToList()
                    };
                }

                var post = _unitOfWork.Repository<Post>().GetAll()
                                        .Where(x => x.IsPremium != true)
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
