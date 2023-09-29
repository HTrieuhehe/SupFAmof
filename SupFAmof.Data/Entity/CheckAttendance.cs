using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class CheckAttendance
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int PostId { get; set; }
        public int PositionId { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Post Post { get; set; } = null!;
    }
}
