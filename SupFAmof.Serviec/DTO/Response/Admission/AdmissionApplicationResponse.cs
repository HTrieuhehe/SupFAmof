using SupFAmof.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response.Admission
{
    public class AdmissionApplicationResponse
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

    public class AdmissionReplyAccountReponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? ImgUrl { get; set; }
    }
}
