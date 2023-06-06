using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class Postition
    {
        public Postition()
        {
            PostPositions = new HashSet<PostPosition>();
            Salaries = new HashSet<Salary>();
        }

        public int Id { get; set; }
        public int PostTitleId { get; set; }
        public int Position { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual PostTitle PostTitle { get; set; } = null!;
        public virtual ICollection<PostPosition> PostPositions { get; set; }
        public virtual ICollection<Salary> Salaries { get; set; }
    }
}
