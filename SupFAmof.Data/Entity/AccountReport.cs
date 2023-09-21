using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class AccountReport
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int PostId { get; set; }
        public int? PostApplied { get; set; }
        public double? Salary { get; set; }
        public DateTime? Date { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Post Post { get; set; } = null!;
    }
}
