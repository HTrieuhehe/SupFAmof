using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class PostTitle
    {
        public PostTitle()
        {
            Postitions = new HashSet<Postition>();
            Posts = new HashSet<Post>();
            TranningTypes = new HashSet<TranningType>();
        }

        public int Id { get; set; }
        public string PostTitleDescription { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreatAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual ICollection<Postition> Postitions { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<TranningType> TranningTypes { get; set; }
    }
}
