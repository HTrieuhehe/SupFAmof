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
    public interface IComplaintService
    {
        Task<BaseResponsePagingViewModel<CompaintResponse>> GetAccountReportProblemsByToken(int accountId, CompaintResponse filter, PagingRequest paging);
        Task<BaseResponsePagingViewModel<AdmissionComplaintResponse>> GetAdmissionAccountReportProblems(int accountId, AdmissionComplaintResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<CompaintResponse>> CreateAccountReportProblem(int accountId, CreateAccountReportProblemRequest request);

        Task<BaseResponseViewModel<AdmissionComplaintResponse>> ApproveReportProblem(int accountId, int reportId, UpdateAdmissionAccountReportProblemRequest request);
        Task<BaseResponseViewModel<AdmissionComplaintResponse>> RejectReportProblem(int accountId, int reportId, UpdateAdmissionAccountReportProblemRequest request);
    }
}
