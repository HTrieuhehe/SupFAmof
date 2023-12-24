using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class Certificate
    {
        public Certificate()
        {
            AccountCertificates = new HashSet<AccountCertificate>();
            CertificateRegistrations = new HashSet<CertificateRegistration>();
            PostPositions = new HashSet<PostPosition>();
        }

        public int Id { get; set; }
        public string CertificateTypeId { get; set; } = null!;
        public string CertificateName { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual ICollection<AccountCertificate> AccountCertificates { get; set; }
        public virtual ICollection<CertificateRegistration> CertificateRegistrations { get; set; }
        public virtual ICollection<PostPosition> PostPositions { get; set; }
    }
}
