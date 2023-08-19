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
        Task<BaseResponsePagingViewModel<PostTitleResponse>> GetPostTitles(PostTitleResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<PostTitleResponse>> GetPostTitleById(int postTitleId);
        Task<BaseResponseViewModel<PostTitleResponse>> CreatePostTitle(CreatePostTitleRequest request);
        Task<BaseResponseViewModel<PostTitleResponse>> UpdatePostTitle(int postTitleId, UpdatePostTitleRequest request);
    }
}
