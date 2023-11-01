using System;
using System.Linq;
using System.Text;
using OfficeOpenXml;
using System.Threading.Tasks;
using System.Collections.Generic;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IFinancialReportService
    {
        Task<BaseResponseViewModel<CollabInfoReportResponse>> GetAdmissionFinancialReport(int accountId);
        Task<byte[]> GenerateAccountExcel();
        BaseResponsePagingViewModel<CollabReportResponse> AccountReportList(PagingRequest paging);
    }
}
