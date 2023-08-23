using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class CheckAttendance
    {
        public int Id { get; set; }
        public int PostRegistrationId { get; set; }
        public DateTime CheckInDate { get; set; }
        public bool IsPresent { get; set; }

        public virtual PostRegistration PostRegistration { get; set; } = null!;
    }
}
