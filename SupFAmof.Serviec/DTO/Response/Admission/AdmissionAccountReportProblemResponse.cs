using SupFAmof.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response.Admission
{
    public class AdmissionAccountReportProblemResponse
    {
        public int? Id { get; set; }
        public int? AccountId { get; set; }
        public DateTime? ReportDate { get; set; }
        public DateTime? ReplyDate { get; set; }
        public string? ProblemNote { get; set; }
        public string? ReplyNote { get; set; }
        public int? Status { get; set; }

        public virtual AccountResponse? Account { get; set; }
    }
}
