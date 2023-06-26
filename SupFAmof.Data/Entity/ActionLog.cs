using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class ActionLog
    {
        public int Id { get; set; }
        public string ActionLog1 { get; set; } = null!;
        public DateTime CreateAt { get; set; }
    }
}
