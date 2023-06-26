using System;
using System.Collections.Generic;
using SupFAmof.Data.Entity;

namespace SupFAmof.Data.Test
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
        public int PostTitleType { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
