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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SupFAmof.Service.Helpers.ErrorEnum;

namespace SupFAmof.Service.Service
{
    public class PostTitleService : IPostTitleService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITimezoneService _timeZone;

        public PostTitleService(IMapper mapper, IUnitOfWork unitOfWork, ITimezoneService timeZone)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _timeZone = timeZone;
        }

        public async Task<BaseResponsePagingViewModel<PostTitleResponse>> GetPostTitles(PostTitleResponse filter, PagingRequest paging)
        {
            try
            {
                var role = _unitOfWork.Repository<PostTitle>().GetAll()
                                    .ProjectTo<PostTitleResponse>(_mapper.ConfigurationProvider)
                                    .DynamicFilter(filter)
                                    .DynamicSort(filter)
                                    .PagingQueryable(paging.Page, paging.PageSize,
                                    Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<PostTitleResponse>()
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

        public async Task<BaseResponseViewModel<PostTitleResponse>> GetPostTitleById(int postTitleId)
        {
            try
            {
                var postTitle = _unitOfWork.Repository<PostTitle>().GetAll()
                                      .FirstOrDefault(x => x.Id == postTitleId);

                if (postTitle == null)
                {
                    throw new ErrorResponse(404, (int)PostTitleErrorEnum.NOT_FOUND_ID,
                                         PostTitleErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                return new BaseResponseViewModel<PostTitleResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<PostTitleResponse>(postTitle)
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponseViewModel<PostTitleResponse>> CreatePostTitle(CreatePostTitleRequest request)
        {
            try
            {
                if (request.PostTitleType == null || request.PostTitleType == "")
                {
                    throw new ErrorResponse(400, (int)PostTitleErrorEnum.INVALID_POST_TITLE_TYPE,
                                        PostTitleErrorEnum.INVALID_POST_TITLE_TYPE.GetDisplayName());
                }

                var postTitle = _unitOfWork.Repository<PostTitle>()
                                           .Find(x => x.PostTitleType.Contains(request.PostTitleType));

                if (postTitle != null)
                {
                    throw new ErrorResponse(400, (int)PostTitleErrorEnum.POST_TITLE_TYPE_EXISTED,
                                        PostTitleErrorEnum.POST_TITLE_TYPE_EXISTED.GetDisplayName());
                }
                var result = _mapper.Map<CreatePostTitleRequest, PostTitle>(request);

                result.PostTitleType = result.PostTitleType.ToUpper();
                result.IsActive = true;
                result.CreateAt = _timeZone.GetCurrentTime();

                await _unitOfWork.Repository<PostTitle>().InsertAsync(result);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<PostTitleResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<PostTitleResponse>(result)
                };
            }
            catch (ErrorResponse ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<PostTitleResponse>> UpdatePostTitle(int postTitleId, UpdatePostTitleRequest request)
        {
            try
            {
                var postTitle = _unitOfWork.Repository<PostTitle>().Find(x => x.Id == postTitleId);

                if (postTitle == null)
                {
                    throw new ErrorResponse(404, (int)PostTitleErrorEnum.NOT_FOUND_ID,
                                             PostTitleErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                if (request.PostTitleType == null || request.PostTitleType == "")
                {
                    throw new ErrorResponse(400, (int)PostTitleErrorEnum.INVALID_POST_TITLE_TYPE,
                                        PostTitleErrorEnum.INVALID_POST_TITLE_TYPE.GetDisplayName());
                }

                var updatePostTitle = _mapper.Map<UpdatePostTitleRequest, PostTitle>(request, postTitle);

                updatePostTitle.PostTitleType = updatePostTitle.PostTitleType.ToUpper();
                updatePostTitle.UpdateAt = DateTime.Now;

                await _unitOfWork.Repository<PostTitle>().UpdateDetached(updatePostTitle);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<PostTitleResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<PostTitleResponse>(updatePostTitle)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
