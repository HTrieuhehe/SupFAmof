using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class PostPosition
    {
        public PostPosition()
        {
            AccountReports = new HashSet<AccountReport>();
            PostAttendees = new HashSet<PostAttendee>();
            PostRegistrationDetails = new HashSet<PostRegistrationDetail>();
            PostRgupdateHistories = new HashSet<PostRgupdateHistory>();
        }

        public int Id { get; set; }
        public int PostId { get; set; }
        public int? TrainingCertificateId { get; set; }
        public int? DocumentId { get; set; }
        public string PositionName { get; set; } = null!;
        public string? PositionDescription { get; set; }
        public string? SchoolName { get; set; }
        public string? Location { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longtitude { get; set; }
        public TimeSpan TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public bool? IsBusService { get; set; }
        public int Status { get; set; }
        public int Amount { get; set; }
        public double Salary { get; set; }

        public virtual DocumentTemplate? Document { get; set; }
        public virtual Post Post { get; set; } = null!;
        public virtual TrainingCertificate? TrainingCertificate { get; set; }
        public virtual ICollection<AccountReport> AccountReports { get; set; }
        public virtual ICollection<PostAttendee> PostAttendees { get; set; }
        public virtual ICollection<PostRegistrationDetail> PostRegistrationDetails { get; set; }
        public virtual ICollection<PostRgupdateHistory> PostRgupdateHistories { get; set; }
    }
}
