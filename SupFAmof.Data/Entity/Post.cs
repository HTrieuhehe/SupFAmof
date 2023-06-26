using System;
using System.Collections.Generic;
using SupFAmof.Data.Entity;

namespace SupFAmof.Data.Test
{
    public partial class Post
    {
        public Post()
        {
            CheckAttendances = new HashSet<CheckAttendance>();
            PostPositions = new HashSet<PostPosition>();
        }

        public int Id { get; set; }
        public int PostTitleId { get; set; }
        public string PostCode { get; set; } = null!;
        public string PostDescription { get; set; } = null!;
        public DateTime DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public TimeSpan TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public bool IsPremium { get; set; }
        public bool AttendanceComplete { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual PostTitle PostTitle { get; set; } = null!;
        public virtual ICollection<CheckAttendance> CheckAttendances { get; set; }
        public virtual ICollection<PostPosition> PostPositions { get; set; }
    }
}
