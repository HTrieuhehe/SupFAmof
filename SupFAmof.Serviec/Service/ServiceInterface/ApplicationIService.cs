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
        Task<BaseResponsePagingViewModel<AdmissionComplaintResponse>> GetAdmissionAccountApplications(int accountId, AdmissionComplaintResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<ApplicationResponse>> CreateAccountApplication(int accountId, CreateAccountApplicationRequest request);

        Task<BaseResponseViewModel<AdmissionComplaintResponse>> ApproveApplication(int accountId, int reportId, UpdateAdmissionAccountApplicationRequest request);
        Task<BaseResponseViewModel<AdmissionComplaintResponse>> RejectApplication(int accountId, int reportId, UpdateAdmissionAccountApplicationRequest request);
    }
}
