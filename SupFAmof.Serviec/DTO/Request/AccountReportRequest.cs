using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Request
{
    public class CreateAccountReportRequest
    {
        //public int Id { get; set; }
        public int AccountId { get; set; }
        public int PostId { get; set; }
        public int PositionId { get; set; }
        public double? Salary { get; set; }
        //public DateTime? Date { get; set; }
    }
}
