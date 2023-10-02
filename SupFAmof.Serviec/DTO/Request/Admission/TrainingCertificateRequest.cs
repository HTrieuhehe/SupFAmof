using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Request.Admission
{
    public class CreateTrainingCertificateRequest
    {
        [Required]
        [MaxLength(10, ErrorMessage = "TrainingTypeId cannot exceed 50 characters.")]
        public string? TrainingTypeId { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "CertificateName cannot exceed 50 characters.")]
        public string? CertificateName { get; set; }
    }

    public class UpdateTrainingCertificateRequest
    {
        [Required]
        [MaxLength(10, ErrorMessage = "TrainingTypeId cannot exceed 50 characters.")]
        public string? TrainingTypeId { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "CertificateName cannot exceed 50 characters.")]
        public string? CertificateName { get; set; }
    }
}
