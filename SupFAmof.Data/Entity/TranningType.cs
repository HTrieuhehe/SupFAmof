using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class TranningType
    {
        public TranningType()
        {
            TranningCertificates = new HashSet<TranningCertificate>();
        }

        public int Id { get; set; }
        public int PostTitleId { get; set; }
        public string TrainingType { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual PostTitle PostTitle { get; set; } = null!;
        public virtual ICollection<TranningCertificate> TranningCertificates { get; set; }
    }
}
