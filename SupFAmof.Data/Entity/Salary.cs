using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class Salary
    {
        public int Id { get; set; }
        public int PositionId { get; set; }
        public double Salary1 { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Postition Position { get; set; } = null!;
    }
}
