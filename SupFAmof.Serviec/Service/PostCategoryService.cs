using AutoMapper;
using AutoMapper.QueryableExtensions;
using LAK.Sdk.Core.Utilities;
using Service.Commons;
using SupFAmof.Data.Entity;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Request.Role;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Service.ServiceInterface;
using SupFAmof.Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static SupFAmof.Service.Helpers.ErrorEnum;

namespace SupFAmof.Service.Service
{
    public class PostCategoryService : IPostCategoryService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public PostCategoryService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponsePagingViewModel<PostCategoryResponse>> GetPostCategories(PostCategoryResponse filter, PagingRequest paging)
        {
            try
            {
                var postCate = _unitOfWork.Repository<PostCategory>().GetAll()
                                            .ProjectTo<PostCategoryResponse>(_mapper.ConfigurationProvider)
                                            .DynamicFilter(filter)
                                            .DynamicSort(paging.Sort, paging.Order)
                                            .Where(x => x.IsActive == true)
                                            .PagingQueryable(paging.Page, paging.PageSize);

                return new BaseResponsePagingViewModel<PostCategoryResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = postCate.Item1
                    },
                    Data = postCate.Item2.ToList()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<PostCategoryResponse>> GetPostCategoryById(int postCategoryId)
        {
            try
            {
                var postTitle = _unitOfWork.Repository<PostCategory>().GetAll()
                                      .FirstOrDefault(x => x.Id == postCategoryId && x.IsActive == true);

                if (postTitle == null)
                {
                    throw new ErrorResponse(404, (int)PostCategoryErrorEnum.NOT_FOUND_ID,
                                         PostCategoryErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                return new BaseResponseViewModel<PostCategoryResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<PostCategoryResponse>(postTitle)
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponseViewModel<PostCategoryResponse>> CreatePostCategory(int accountId, CreatePostCategoryRequest request)
        {
            try
            {
                //check account
                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);

                if (account == null || account.PostPermission == false) 
                {
                    throw new ErrorResponse(403, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }


                if (request.PostCategoryType == null || request.PostCategoryType == "")
                {
                    throw new ErrorResponse(400, (int)PostCategoryErrorEnum.POST_CATEGORY_TYPE_DUPLICATE,
                                        PostCategoryErrorEnum.POST_CATEGORY_TYPE_DUPLICATE.GetDisplayName());
                }

                var postCategory = _unitOfWork.Repository<PostCategory>()
                                           .Find(x => x.PostCategoryType.Contains(request.PostCategoryType) && x.IsActive == true);

                if (postCategory != null)
                {
                    throw new ErrorResponse(400, (int)PostCategoryErrorEnum.POST_CATEGORY_TYPE_EXISTED,
                                        PostCategoryErrorEnum.POST_CATEGORY_TYPE_EXISTED.GetDisplayName());
                }
                var result = _mapper.Map<CreatePostCategoryRequest, PostCategory>(request);

                result.PostCategoryType = result.PostCategoryType.ToUpper();
                result.IsActive = true;
                result.CreateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<PostCategory>().InsertAsync(result);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<PostCategoryResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<PostCategoryResponse>(result)
                };
            }
            catch (ErrorResponse ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<PostCategoryResponse>> UpdatePostCategory(int accountId, int postCategoryId, UpdatePostCategoryRequest request)
        {
            try
            {
                var postCategory = _unitOfWork.Repository<PostCategory>().Find(x => x.Id == postCategoryId);

                if (postCategory == null)
                {
                    throw new ErrorResponse(404, (int)PostCategoryErrorEnum.NOT_FOUND_ID,
                                             PostCategoryErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                if (request.PostCategoryType == null || request.PostCategoryType == "")
                {
                    throw new ErrorResponse(400, (int)PostCategoryErrorEnum.POST_CATEGORY_TYPE_DUPLICATE,
                                        PostCategoryErrorEnum.POST_CATEGORY_TYPE_DUPLICATE.GetDisplayName());
                }

                var updatePostCategory = _mapper.Map<UpdatePostCategoryRequest, PostCategory>(request, postCategory);

                updatePostCategory.PostCategoryType = updatePostCategory.PostCategoryType.ToUpper();
                updatePostCategory.UpdateAt = DateTime.Now;

                await _unitOfWork.Repository<PostCategory>().UpdateDetached(updatePostCategory);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<PostCategoryResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<PostCategoryResponse>(updatePostCategory)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<PostCategoryResponse>> SearchPostCategory(string search, PagingRequest paging)
        {
            //Search by Description or Type
            try
            {

                if (search == null || search.Length == 0)
                {
                    throw new ErrorResponse(404, (int)PostCategoryErrorEnum.NOT_FOUND_ID,
                                        PostCategoryErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                var postCate = _unitOfWork.Repository<PostCategory>().GetAll()
                                    .ProjectTo<PostCategoryResponse>(_mapper.ConfigurationProvider)
                                    .Where(x => x.PostCategoryDescription.Contains(search) || x.PostCategoryType.Contains(search.ToUpper()))
                                    .PagingQueryable(paging.Page, paging.PageSize);

                if (!postCate.Item2.Any())
                {
                    throw new ErrorResponse(404, (int)PostCategoryErrorEnum.NOT_FOUND_ID,
                                        PostCategoryErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                return new BaseResponsePagingViewModel<PostCategoryResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = postCate.Item1
                    },
                    Data = postCate.Item2.ToList()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<PostCategoryResponse>> DisablePostCategory(int accountId, int postCategoryId)
        {
            try
            {
                //check account
                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);

                if (account == null || account.PostPermission == false)
                {
                    throw new ErrorResponse(403, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                //find Post Category
                var postCategory = await _unitOfWork.Repository<PostCategory>().FindAsync(pc => pc.Id == postCategoryId);

                if (postCategory == null)
                {
                    throw new ErrorResponse(404, (int)PostCategoryErrorEnum.NOT_FOUND_ID,
                                             PostCategoryErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                postCategory.IsActive = false;
                postCategory.UpdateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<PostCategory>().UpdateDetached(postCategory);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<PostCategoryResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<PostCategoryResponse>(postCategory)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
