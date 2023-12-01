using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using static SupFAmof.Service.Utilities.Ultils;

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

    public class EventDaysCertificate
    {
        public DateTime Date { get; set; }
        public string? Class { get; set; }
        public TimeSpan? TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        [JsonIgnore]
        public DateTime CreateAt { get; set; } = GetCurrentDatetime();
        [JsonIgnore]
        public int? Status { get; set; } = 1;

    }
    public class UpdateDaysCertifcate
    {
        public DateTime Date { get; set; }
        public string? Class { get; set; }
        public TimeSpan? TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        [JsonIgnore]
        public DateTime Updateat { get; set; } = GetCurrentDatetime();

    }
    public class TrainingCertificateRegistration
    {
        public int TrainingCertificateId { get; set; }
        [JsonIgnore]
        public int Status { get; set; } = 1;
        [JsonIgnore]
        public DateTime CreateAt { get; set; } = GetCurrentDatetime();
    }
    public class AssignEventDayToAccount
    {
        public int? TrainingRegistrationId { get; set; }
        public int? EventDayId { get; set; }
    }
}