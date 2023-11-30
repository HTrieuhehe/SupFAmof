using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class TrainingEventDay
    {
        public TrainingEventDay()
        {
            TrainingRegistrations = new HashSet<TrainingRegistration>();
        }

        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string? Class { get; set; }
        public TimeSpan? TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? Updateat { get; set; }

        public virtual ICollection<TrainingRegistration> TrainingRegistrations { get; set; }
    }
}
