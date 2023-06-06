using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class Post
    {
        public Post()
        {
            PostPositions = new HashSet<PostPosition>();
            PostReports = new HashSet<PostReport>();
        }

        public int Id { get; set; }
        public int PostTitleId { get; set; }
        public string PostDescription { get; set; } = null!;
        public DateTime DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public TimeSpan TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual PostTitle PostTitle { get; set; } = null!;
        public virtual ICollection<PostPosition> PostPositions { get; set; }
        public virtual ICollection<PostReport> PostReports { get; set; }
    }
}
