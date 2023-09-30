using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class TrainingCertificate
    {
        public TrainingCertificate()
        {
            AccountCertificates = new HashSet<AccountCertificate>();
            PostPositions = new HashSet<PostPosition>();
            PostTrainingCertificates = new HashSet<PostTrainingCertificate>();
        }

        public int Id { get; set; }
        public string TrainingTypeId { get; set; } = null!;
        public string CertificateName { get; set; } = null!;
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual ICollection<AccountCertificate> AccountCertificates { get; set; }
        public virtual ICollection<PostPosition> PostPositions { get; set; }
        public virtual ICollection<PostTrainingCertificate> PostTrainingCertificates { get; set; }
    }
}
