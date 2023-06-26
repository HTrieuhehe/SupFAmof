using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Test
{
    public partial class AccountReport
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int TotalPost { get; set; }
        public double? TotalSalary { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
