using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Test
{
    public partial class CheckAttendance
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string IdStudent { get; set; } = null!;
        public bool IsPresent { get; set; }

        public virtual Post Post { get; set; } = null!;
    }
}
