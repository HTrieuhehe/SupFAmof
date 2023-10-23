using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IAccountReportService
    {
        Task<BaseResponsePagingViewModel<AccountReportResponse>> GetAccountReports(AccountReportResponse filter, PagingRequest paging);
        Task<BaseResponsePagingViewModel<AccountReportResponse>> GetAccountReportByToken(int accountId, AccountReportResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<ReportPostRegistrationResponse>> GetReportRegistrationById(int accountId, int accountReportId);
    }
}
