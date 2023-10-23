using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class AccountReport
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int PostId { get; set; }
        public int PositionId { get; set; }
        public double? Salary { get; set; }
        public DateTime? CreateAt { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual PostPosition Position { get; set; } = null!;
        public virtual Post Post { get; set; } = null!;
    }
}
