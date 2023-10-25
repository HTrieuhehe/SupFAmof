using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IFinancialReportService
    {
        Task<BaseResponseViewModel<CollabInfoReportResponse>> GetAdmissionFinancialReport(int accountId);
    }
}
