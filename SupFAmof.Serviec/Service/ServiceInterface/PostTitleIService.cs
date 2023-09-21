using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IPostTitleService
    {
        Task<BaseResponsePagingViewModel<PostCategoryResponse>> GetPostTitles(PostCategoryResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<PostCategoryResponse>> GetPostTitleById(int postTitleId);
        Task<BaseResponseViewModel<PostCategoryResponse>> CreatePostTitle(CreatePostCategoryRequest request);
        Task<BaseResponseViewModel<PostCategoryResponse>> UpdatePostTitle(int postTitleId, UpdatePostCategoryRequest request);
    }
}
