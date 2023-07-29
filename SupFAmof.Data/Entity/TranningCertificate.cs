using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class TranningCertificate
    {
        public TranningCertificate()
        {
            AccountCertificates = new HashSet<AccountCertificate>();
        }

        public int Id { get; set; }
        public int TrainingTypeId { get; set; }
        public string CertificateName { get; set; } = null!;
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual ICollection<AccountCertificate> AccountCertificates { get; set; }
    }
}
