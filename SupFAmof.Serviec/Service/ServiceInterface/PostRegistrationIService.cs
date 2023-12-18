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
        Task<BaseResponsePagingViewModel<CollabRegistrationUpdateViewResponse>> GetPostRegistrationByAccountId
            (int accountId, PagingRequest paging, CollabRegistrationUpdateViewResponse filter, FilterPostRegistrationResponse statusFilter);

        Task<BaseResponsePagingViewModel<RegistrationMapResponse>> GetPostRegistrationGoogleMapByAccountId
            (int accountId, PagingRequest paging, RegistrationMapResponse filter);

        Task<BaseResponseViewModel<CollabRegistrationResponse>> CreatePostRegistration(int accountId,PostRegistrationRequest request);
        Task<BaseResponseViewModel<dynamic>> CancelPostregistration(int accountId,int postRegistrationId);
        Task<BaseResponseViewModel<dynamic>> UpdatePostRegistration(int accountId,PostRegistrationUpdateRequest request);
        Task<BaseResponsePagingViewModel<AdmissionPostsResponse>> AdmssionPostRegistrations(int admissionAccountId, PagingRequest paging);
        Task<BaseResponseViewModel<List<PostRegistrationResponse>>> ApproveUpdateRequest(List<int> Ids, bool approve);
        Task<BaseResponseViewModel<dynamic>> ApprovePostRegistrationRequest(int accountId,List<int> postRegistrationIds, bool approve);

        Task<BaseResponsePagingViewModel<PostRegistrationResponse>> GetAccountByPostPositionId(int accountId, int positionId, string searchEmail, PostRegistrationResponse filter,  PagingRequest paging);
        Task<BaseResponsePagingViewModel<PostRgupdateHistoryResponse>> GetUpdateRequestByAccountId(int accountId, PostRgupdateHistoryResponse filter, PagingRequest paging);

        Task<BaseResponsePagingViewModel<CollabRegistrationResponse>> GetPostRegistrationCheckIn(int accountId, PagingRequest paging);
        Task<BaseResponseViewModel<dynamic>> UpdateSchoolBus(int accountId, UpdateSchoolBusRequest request);
        Task<BaseResponseViewModel<List<PostRegistrationResponse>>> CancelPostRegistrationAdmission(List<int> Ids, int accountId);
        Task<BaseResponsePagingViewModel<AdmissionUpdateRequestResponse>> AdmissionUpdateRequests(int admissionAccountId, PagingRequest paging, AdmissionUpdateRequestResponse fitler, FilterUpdateRequestResponse IdFilter);
        Task<BaseResponsePagingViewModel<PostRegistrationResponse>> GetRegistrationByPositionIdAdmission(int admissionAccountId, PagingRequest paging, int? positionId);


        //code của dashboard
        Task<BaseResponseViewModel<DashboardPostRegistrationResponse>> GetTotalRegistrationInCurrentMonth(int accountId);
        Task<BaseResponseViewModel<DashboardRegistrationAnalyticsResponse>> GetAnalyticsInMonth(int accountId, DashBoardAnalyticsTimeRequest request);
    }

}
