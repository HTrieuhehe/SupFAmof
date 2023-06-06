using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class PostReport
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int ApplyNumber { get; set; }

        public virtual Post Post { get; set; } = null!;
    }
}
