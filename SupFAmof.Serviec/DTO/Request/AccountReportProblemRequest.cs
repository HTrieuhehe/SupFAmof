using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Request
{
    public class CreateAccountReportProblemRequest
    {
        //public int Id { get; set; }
        //public int AccountId { get; set; }
        //public DateTime ReportDate { get; set; }
        //public DateTime? ReplyDate { get; set; }

        [Required(ErrorMessage = "Problem Note is required.")]
        [MaxLength(500, ErrorMessage = "Problem Note cannot exceed 500 characters.")]
        public string? ProblemNote { get; set; }

        //public string? ReplyNote { get; set; }
        //public int Status { get; set; }
    }
}
