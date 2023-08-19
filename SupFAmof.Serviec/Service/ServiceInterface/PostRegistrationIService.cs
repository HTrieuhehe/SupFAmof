using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface PostRegistrationIService
    {
        Task<BaseResponseViewModel<List<PostRegistrationResponse>>> GetPostRegistrationByAccountId(int accountId);
        Task<BaseResponseViewModel<PostRegistrationResponse>> CreatePostRegistration(PostRegistrationRequest request);
        Task<BaseResponseViewModel<PostRegistrationDetailResponse>> CreatePostRegistrationDetail(PostRegistrationDetailRequest request);
        Task CancelPostregistration(int postRegistrationId);
    }
}
