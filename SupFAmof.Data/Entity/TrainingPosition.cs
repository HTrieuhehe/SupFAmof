using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class TrainingPosition
    {
        public TrainingPosition()
        {
            PostAttendees = new HashSet<PostAttendee>();
        }

        public int Id { get; set; }
        public int PostId { get; set; }
        public int TrainingCertificateId { get; set; }
        public int? DocumentId { get; set; }
        public string PositionName { get; set; } = null!;
        public string? SchoolName { get; set; }
        public string? Location { get; set; }
        public TimeSpan TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public bool? IsBusService { get; set; }
        public int Amount { get; set; }
        public double Salary { get; set; }

        public virtual DocumentTemplate? Document { get; set; }
        public virtual Post Post { get; set; } = null!;
        public virtual TrainingCertificate TrainingCertificate { get; set; } = null!;
        public virtual ICollection<PostAttendee> PostAttendees { get; set; }
    }
}
