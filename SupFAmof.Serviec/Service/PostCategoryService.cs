using AutoMapper;
using AutoMapper.QueryableExtensions;
using NTQ.Sdk.Core.Utilities;
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

        public async Task<BaseResponsePagingViewModel<PostCategoryResponse>> GetPostTitles(PostCategoryResponse filter, PagingRequest paging)
        {
            try
            {
                var role = _unitOfWork.Repository<PostCategory>().GetAll()
                                    .ProjectTo<PostCategoryResponse>(_mapper.ConfigurationProvider)
                                    .DynamicFilter(filter)
                                    .DynamicSort(filter)
                                    .PagingQueryable(paging.Page, paging.PageSize,
                                    Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<PostCategoryResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = role.Item1
                    },
                    Data = role.Item2.ToList()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<PostCategoryResponse>> GetPostTitleById(int postTitleId)
        {
            try
            {
                var postTitle = _unitOfWork.Repository<PostCategory>().GetAll()
                                      .FirstOrDefault(x => x.Id == postTitleId);

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

        public async Task<BaseResponseViewModel<PostCategoryResponse>> CreatePostTitle(CreatePostCategoryRequest request)
        {
            try
            {
                if (request.PostTitleType == null || request.PostTitleType == "")
                {
                    throw new ErrorResponse(400, (int)PostCategoryErrorEnum.POST_TITLE_TYPE_DUPLICATE,
                                        PostCategoryErrorEnum.POST_TITLE_TYPE_DUPLICATE.GetDisplayName());
                }

                var postTitle = _unitOfWork.Repository<PostCategory>()
                                           .Find(x => x.PostCategoryType.Contains(request.PostTitleType));

                if (postTitle != null)
                {
                    throw new ErrorResponse(400, (int)PostCategoryErrorEnum.POST_TITLE_TYPE_EXISTED,
                                        PostCategoryErrorEnum.POST_TITLE_TYPE_EXISTED.GetDisplayName());
                }
                var result = _mapper.Map<CreatePostCategoryRequest, PostCategory>(request);

                result.PostCategoryType = result.PostCategoryType.ToUpper();
                result.IsActive = true;
                result.CreateAt = Ultils.GetCurrentTime();

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

        public async Task<BaseResponseViewModel<PostCategoryResponse>> UpdatePostTitle(int postTitleId, UpdatePostCategoryRequest request)
        {
            try
            {
                var postTitle = _unitOfWork.Repository<PostCategory>().Find(x => x.Id == postTitleId);

                if (postTitle == null)
                {
                    throw new ErrorResponse(404, (int)PostCategoryErrorEnum.NOT_FOUND_ID,
                                             PostCategoryErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                if (request.PostTitleType == null || request.PostTitleType == "")
                {
                    throw new ErrorResponse(400, (int)PostCategoryErrorEnum.POST_TITLE_TYPE_DUPLICATE,
                                        PostCategoryErrorEnum.POST_TITLE_TYPE_DUPLICATE.GetDisplayName());
                }

                var updatePostTitle = _mapper.Map<UpdatePostCategoryRequest, PostCategory>(request, postTitle);

                updatePostTitle.PostCategoryType = updatePostTitle.PostCategoryType.ToUpper();
                updatePostTitle.UpdateAt = DateTime.Now;

                await _unitOfWork.Repository<PostCategory>().UpdateDetached(updatePostTitle);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<PostCategoryResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<PostCategoryResponse>(updatePostTitle)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
