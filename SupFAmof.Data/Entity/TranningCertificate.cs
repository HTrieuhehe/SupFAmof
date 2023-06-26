using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Test
{
    public partial class TranningCertificate
    {
        public int Id { get; set; }
        public int TrainingTypeId { get; set; }
        public string CertificateName { get; set; } = null!;
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
