using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class Post
    {
        public Post()
        {
            PostPositions = new HashSet<PostPosition>();
            PostRegistrationDetails = new HashSet<PostRegistrationDetail>();
            PostRgupdateHistories = new HashSet<PostRgupdateHistory>();
            TrainingPositions = new HashSet<TrainingPosition>();
        }

        public int Id { get; set; }
        public int AccountId { get; set; }
        public int PostTitleId { get; set; }
        public string PostCode { get; set; } = null!;
        public string PostDescription { get; set; } = null!;
        public int? Priority { get; set; }
        public bool IsPremium { get; set; }
        public int Status { get; set; }
        public bool AttendanceComplete { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual PostTitle PostTitle { get; set; } = null!;
        public virtual ICollection<PostPosition> PostPositions { get; set; }
        public virtual ICollection<PostRegistrationDetail> PostRegistrationDetails { get; set; }
        public virtual ICollection<PostRgupdateHistory> PostRgupdateHistories { get; set; }
        public virtual ICollection<TrainingPosition> TrainingPositions { get; set; }
    }
}
