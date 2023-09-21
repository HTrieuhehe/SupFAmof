using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class PostCategory
    {
        public PostCategory()
        {
            PostTrainingCertificates = new HashSet<PostTrainingCertificate>();
            Posts = new HashSet<Post>();
        }

        public int Id { get; set; }
        public string PostCategoryDescription { get; set; } = null!;
        public string PostCategoryType { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual ICollection<PostTrainingCertificate> PostTrainingCertificates { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
