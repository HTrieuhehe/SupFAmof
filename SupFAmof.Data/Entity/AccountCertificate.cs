using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class AccountCertificate
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int TraningCertificateId { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual TranningCertificate TraningCertificate { get; set; } = null!;
    }
}
