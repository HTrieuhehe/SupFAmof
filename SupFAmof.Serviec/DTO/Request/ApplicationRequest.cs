using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SupFAmof.Service.DTO.Request
{
    public class CreateAccountApplicationRequest
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

    public class UpdateAdmissionAccountApplicationRequest
    {
        //public int Id { get; set; }
        //public int AccountId { get; set; }
        //public DateTime ReportDate { get; set; }
        //public DateTime? ReplyDate { get; set; }

        [Required(ErrorMessage = "Reply Note is required.")]
        [MaxLength(500, ErrorMessage = "Reply Note cannot exceed 500 characters.")]
        public string? ReplyNote { get; set; }

        //public string? ProblemNote { get; set; }
        //public int Status { get; set; }
    }
}
