using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class Contract
    {
        public int Id { get; set; }
        public int PostTitleId { get; set; }
        public byte[]? ContractFile { get; set; }
        public string ContractDescription { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual PostTitle PostTitle { get; set; } = null!;
    }
}
