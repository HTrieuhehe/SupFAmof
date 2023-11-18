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
        Task<BaseResponsePagingViewModel<ApplicationResponse>> GetAccountReportProblemsByToken(int accountId, ApplicationResponse filter, PagingRequest paging);
        Task<BaseResponsePagingViewModel<AdmissionComplaintResponse>> GetAdmissionAccountReportProblems(int accountId, AdmissionComplaintResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<ApplicationResponse>> CreateAccountReportProblem(int accountId, CreateAccountApplicationRequest request);

        Task<BaseResponseViewModel<AdmissionComplaintResponse>> ApproveReportProblem(int accountId, int reportId, UpdateAdmissionAccountApplicationRequest request);
        Task<BaseResponseViewModel<AdmissionComplaintResponse>> RejectReportProblem(int accountId, int reportId, UpdateAdmissionAccountApplicationRequest request);
    }
}
