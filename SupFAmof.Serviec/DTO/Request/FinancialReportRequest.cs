using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Bibliography;

namespace SupFAmof.Service.DTO.Request
{
    public class FinancialReportRequest
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
    }
}
