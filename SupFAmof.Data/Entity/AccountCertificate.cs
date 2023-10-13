using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class AccountCertificate
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int? CertificateIssuerId { get; set; }
        public int TrainingCertificateId { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Account? CertificateIssuer { get; set; }
        public virtual TrainingCertificate TrainingCertificate { get; set; } = null!;
    }
}
