using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class PostTitle
    {
        public PostTitle()
        {
            Contracts = new HashSet<Contract>();
            Posts = new HashSet<Post>();
        }

        public int Id { get; set; }
        public string PostTitleDescription { get; set; } = null!;
        public string PostTitleType { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
