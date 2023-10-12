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
    public interface IAccountReportProblemService
    {
        Task<BaseResponsePagingViewModel<AccountReportProblemResponse>> GetAccountReportProblemsByToken(int accountId, AccountReportProblemResponse filter, PagingRequest paging);
        Task<BaseResponsePagingViewModel<AdmissionAccountReportProblemResponse>> GetAdmissionAccountReportProblems(int accountId, AdmissionAccountReportProblemResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<AccountReportProblemResponse>> CreateAccountReportProblem(int accountId, CreateAccountReportProblemRequest request);

        Task<BaseResponseViewModel<AdmissionAccountReportProblemResponse>> ApproveReportProblem(int accountId, UpdateAdmissionAccountReportProblemRequest request);
        Task<BaseResponseViewModel<AdmissionAccountReportProblemResponse>> RejectReportProblem(int accountId, UpdateAdmissionAccountReportProblemRequest request);
    }
}
