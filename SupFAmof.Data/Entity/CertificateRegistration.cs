using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class CertificateRegistration
    {
        public int Id { get; set; }
        public int CertificateId { get; set; }
        public int? InterviewDayId { get; set; }
        public int AccountId { get; set; }
        public int Status { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Certificate Certificate { get; set; } = null!;
        public virtual InterviewDay? InterviewDay { get; set; }
    }
}
