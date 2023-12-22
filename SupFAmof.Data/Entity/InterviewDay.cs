using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class InterviewDay
    {
        public InterviewDay()
        {
            InterviewRegistrations = new HashSet<InterviewRegistration>();
        }

        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string? Room { get; set; }
        public TimeSpan? TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? Updateat { get; set; }

        public virtual ICollection<InterviewRegistration> InterviewRegistrations { get; set; }
    }
}
