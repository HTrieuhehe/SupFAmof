using SupFAmof.Service.DTO.Response.Admission;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response
{
    public class ApplicationResponse
    {
        public int? Id { get; set; }
        public int? AccountId { get; set; }
        public DateTime? ReportDate { get; set; }
        public DateTime? ReplyDate { get; set; }
        public string? ProblemNote { get; set; }
        public string? ReplyNote { get; set; }
        public int? Status { get; set; }

        public virtual AccountResponse? Account { get; set; }
        public virtual AdmissionReplyAccountReponse? AccountReply { get; set; }
    }
}
