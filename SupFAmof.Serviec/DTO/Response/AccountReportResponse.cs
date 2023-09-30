using SupFAmof.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response
{
    public class AccountReportResponse
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int PostId { get; set; }
        public double? Salary { get; set; }
        public DateTime? Date { get; set; }

        public double? TotalSalary { get; set; }

        public virtual PostResponse? Post { get; set; }
    }
}
