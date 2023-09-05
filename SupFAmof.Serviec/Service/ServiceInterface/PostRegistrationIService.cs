using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IPostRegistrationService
    {
        Task<BaseResponsePagingViewModel<PostRegistrationResponse>> GetPostRegistrationByAccountId(int accountId, PagingRequest paging);
        Task<BaseResponseViewModel<PostRegistrationResponse>> CreatePostRegistration(PostRegistrationRequest request);
        Task<BaseResponseViewModel<dynamic>> CancelPostregistration(int postRegistrationId);
        Task<BaseResponseViewModel<PostRegistrationResponse>> UpdatePostRegistration(int PostRegistrationId, PostRegistrationUpdateRequest request);
        Task<BaseResponseViewModel<PostRegistrationResponse>> ApproveUpdateRequest(int Id, bool approve);
        Task<BaseResponsePagingViewModel<PostRegistrationResponse>> AdmssionPostRegistrations(int admissionAccountId, PagingRequest paging);
    }
}
