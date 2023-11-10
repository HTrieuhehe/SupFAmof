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
    public interface IPostCategoryService
    {
        Task<BaseResponsePagingViewModel<PostCategoryResponse>> GetPostCategories(PostCategoryResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<PostCategoryResponse>> GetPostCategoryById(int postCategoryId);
        Task<BaseResponseViewModel<PostCategoryResponse>> CreatePostCategory(int accountId, CreatePostCategoryRequest request);
        Task<BaseResponseViewModel<PostCategoryResponse>> UpdatePostCategory(int accountId, int postCategoryId, UpdatePostCategoryRequest request);
        Task<BaseResponsePagingViewModel<PostCategoryResponse>> SearchPostCategory(string search, PagingRequest paging);
        Task<BaseResponseViewModel<PostCategoryResponse>> DisablePostCategory(int accountId, int postCategoryId);
    }
}
