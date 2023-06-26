using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class PostPosition
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string Position { get; set; } = null!;
        public int Amount { get; set; }
        public double Salary { get; set; }

        public virtual Post Post { get; set; } = null!;
    }
}
