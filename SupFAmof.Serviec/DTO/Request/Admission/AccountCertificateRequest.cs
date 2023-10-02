using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Request.Admission
{
    public class CreateAccountCertificateRequest
    {
        [Required]
        [RegularExpression("^[0-9]+$", ErrorMessage = "AccountId must contain only numeric characters.")]
        public int AccountId { get; set; }

        [Required]
        [RegularExpression("^[0-9]+$", ErrorMessage = "TraningCertificateId must contain only numeric characters.")]
        public int TraningCertificateId { get; set; }
    }
}
