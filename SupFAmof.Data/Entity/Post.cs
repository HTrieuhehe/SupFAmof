using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class Post
    {
        public Post()
        {
            PostPositions = new HashSet<PostPosition>();
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
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual PostCategory PostCategory { get; set; } = null!;
        public virtual ICollection<PostPosition> PostPositions { get; set; }
    }
}
