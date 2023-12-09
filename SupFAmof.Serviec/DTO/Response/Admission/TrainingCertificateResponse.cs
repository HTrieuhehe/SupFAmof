using System;
using System.Linq;
using System.Text;
using SupFAmof.Data.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SupFAmof.Service.DTO.Response.Admission
{
    public class TrainingCertificateResponse
    {
        public int? Id { get; set; }
        public string? TrainingTypeId { get; set; }
        public string? CertificateName { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }


    public class ViewCollabInterviewClassResponse
    {
        public int? Id { get; set; }
        public DateTime? Date { get; set; }
        public string? Class { get; set; }
        public TimeSpan? TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? Updateat { get; set; }

        public virtual ICollection<TrainingRegistrationResponse>? TrainingRegistrations { get; set; }

    }

    public class TrainingRegistrationResponse
    {
        public int? Id { get; set; }
        public int? Status { get; set; }
        public virtual AccountCertificateRegistrationResponse? Account { get; set; }
        public virtual TrainingCertificateRegistrationResponse? TrainingCertificate { get; set; }


    }
    public class AccountCertificateRegistrationResponse
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? ImgUrl { get; set; }
        public bool? IsPremium { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? IdStudent { get; set; }
        public int? Status { get; set; }
    }
    public class TrainingCertificateRegistrationResponse
    {
        public int? Id { get; set; }
        public string? TrainingTypeId { get; set; } = null!;
        public string? CertificateName { get; set; } = null!;
        public bool? IsActive { get; set; }
    }

    public class TrainingEventDayResponse
    {
        public int? Id { get; set; }
        public DateTime? Date { get; set; }
        public string? Class { get; set; }
        public TimeSpan? TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? Updateat { get; set; }
    }


    public class AdmissionGetCertificateRegistrationResponse
    {
        public int? Id { get; set; }
        public string? TrainingTypeId { get; set; } = null!;
        public string? CertificateName { get; set; } = null!;
        public bool? IsActive { get; set; }
        public DateTime? CreateAt { get; set; }

        public virtual ICollection<AccountCertificateRegistrationResponse>? Registrations { get; set; }
        public int? RegisterAmount { get; set; }
    }

    public class CollabRegistrationsResponse
    {
        public int? Id { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public DateTime? ConfirmAt { get; set; }

        public virtual TrainingCertificateRegistrationResponse? TrainingCertificate { get; set; }
        public virtual TrainingEventDayResponse? EventDay { get; set; }

    }
    public class FilterStatusRegistrationResponse
    {
        public int? Status { get; set; }
    }


}
