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
        [RegularExpression("^[0-9]+$", ErrorMessage = "Account Id must contain only numeric characters.")]
        public int AccountId { get; set; }

        [Required]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Traning Certificate Id must contain only numeric characters.")]
        public int TrainingCertificateId { get; set; }
    }

    public class UpdateAccountCertificateRequest
    {
        [Required]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Account Id must contain only numeric characters.")]
        public int AccountId { get; set; }

        [Required]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Traning Certificate Id must contain only numeric characters.")]
        public int TrainingCertificateId { get; set; }

        [Required]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Status must containt only numeric character")]
        public int Status { get; set; }

    }
}
