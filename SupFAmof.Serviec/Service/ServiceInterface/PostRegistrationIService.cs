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
        Task<BaseResponseViewModel<PostRegistrationResponse>> CreatePostRegistration(int accountId,PostRegistrationRequest request);
        Task<BaseResponseViewModel<dynamic>> CancelPostregistration(int accountId,int postRegistrationId);
        Task<BaseResponseViewModel<dynamic>> UpdatePostRegistration(int accountId,int PostRegistrationId, PostRegistrationUpdateRequest request);
        Task<BaseResponsePagingViewModel<AdmissionPostsResponse>> AdmssionPostRegistrations(int admissionAccountId, PagingRequest paging);
        Task<BaseResponseViewModel<List<PostRegistrationResponse>>> ApproveUpdateRequest(List<int> Ids, bool approve);
        Task<BaseResponseViewModel<dynamic>> ApprovePostRegistrationRequest(int accountId,List<int> postRegistrationIds, bool approve);
    }

}
