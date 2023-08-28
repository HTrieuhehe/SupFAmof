using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
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

                var post = _mapper.Map<Post>(request);

                post.AccountId = accountId;
                post.AttendanceComplete = false;
                post.IsActive = true;
                post.IsEnd = false;
                post.CreateAt = DateTime.Now;

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
            catch(Exception)
            {
                throw new ErrorResponse(400, (int)PostErrorEnum.INVALID_POST,
                                         PostErrorEnum.INVALID_POST.GetDisplayName());
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

        public async Task<BaseResponseViewModel<AdmissionPostResponse>> GetPostByPostcode(int postCode)
        {
            try
            {
                var post = _unitOfWork.Repository<Post>().GetAll().FirstOrDefault(x => x.PostCode == postCode);

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
                                        .ProjectTo<PostResponse>(_mapper.ConfigurationProvider)
                                        .DynamicFilter(filter)
                                        .DynamicSort(filter)
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
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<PostResponse>> GetPostByCode(int postCode)
        {
            try
            {
                var post = _unitOfWork.Repository<Post>().GetAll().FirstOrDefault(x => x.PostCode == postCode);

                if (post == null)
                {
                    throw new ErrorResponse(404, (int)PostErrorEnum.NOT_FOUND_ID,
                                         PostErrorEnum.NOT_FOUND_ID.GetDisplayName());
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
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
