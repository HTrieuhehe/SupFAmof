using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class Post
    {
        public Post()
        {
            AccountReports = new HashSet<AccountReport>();
            CheckAttendances = new HashSet<CheckAttendance>();
            PostAttendees = new HashSet<PostAttendee>();
            PostPositions = new HashSet<PostPosition>();
            PostRegistrationDetails = new HashSet<PostRegistrationDetail>();
            PostRgupdateHistories = new HashSet<PostRgupdateHistory>();
        }

        public int Id { get; set; }
        public int AccountId { get; set; }
        public int PostCategoryId { get; set; }
        public string PostCode { get; set; } = null!;
        public string PostDescription { get; set; } = null!;
        public string? PostImg { get; set; }
        public int Priority { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool IsPremium { get; set; }
        public int Status { get; set; }
        public bool AttendanceComplete { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual PostCategory PostCategory { get; set; } = null!;
        public virtual ICollection<AccountReport> AccountReports { get; set; }
        public virtual ICollection<CheckAttendance> CheckAttendances { get; set; }
        public virtual ICollection<PostAttendee> PostAttendees { get; set; }
        public virtual ICollection<PostPosition> PostPositions { get; set; }
        public virtual ICollection<PostRegistrationDetail> PostRegistrationDetails { get; set; }
        public virtual ICollection<PostRgupdateHistory> PostRgupdateHistories { get; set; }
    }
}
