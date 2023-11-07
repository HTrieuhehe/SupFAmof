using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Request
{
    public class CreateAccountReportRequest
    {
        [Required]
        [RegularExpression("^-?[0-9]+$", ErrorMessage = "AccountId must be a valid numeric value.")]
        public int AccountId { get; set; }

        [Required]
        [RegularExpression("^-?[0-9]+$", ErrorMessage = "AccountId must be a valid numeric value.")]
        public int PositionId { get; set; }

        [Required]
        [RegularExpression(@"^[+]?[0-9]*\.?[0-9]+$", ErrorMessage = "Salary must be a valid numeric value.")]
        public double? Salary { get; set; }
    }
}
