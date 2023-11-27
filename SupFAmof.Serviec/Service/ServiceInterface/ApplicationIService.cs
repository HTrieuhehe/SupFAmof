using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IApplicationService
    {
        Task<BaseResponsePagingViewModel<ApplicationResponse>> GetAccountApplicationsByToken(int accountId, ApplicationResponse filter, PagingRequest paging);
        Task<BaseResponsePagingViewModel<AdmissionApplicationResponse>> GetAdmissionAccountApplications(int accountId, AdmissionApplicationResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<ApplicationResponse>> CreateAccountApplication(int accountId, CreateAccountApplicationRequest request);

        Task<BaseResponseViewModel<AdmissionApplicationResponse>> ApproveApplication(int accountId, int reportId, UpdateAdmissionAccountApplicationRequest request);
        Task<BaseResponseViewModel<AdmissionApplicationResponse>> RejectApplication(int accountId, int reportId, UpdateAdmissionAccountApplicationRequest request);
    }
}
