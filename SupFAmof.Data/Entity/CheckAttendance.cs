using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class CheckAttendance
    {
        public int Id { get; set; }
        public int PostRegistrationId { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public DateTime? ConfirmTime { get; set; }
        public int Status { get; set; }
        public string? Note { get; set; }

        public virtual PostRegistration PostRegistration { get; set; } = null!;
    }
}
