using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class DocumentTemplate
    {
        public DocumentTemplate()
        {
            PostPositions = new HashSet<PostPosition>();
        }

        public int Id { get; set; }
        public string DocName { get; set; } = null!;
        public string DocUrl { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual ICollection<PostPosition> PostPositions { get; set; }
    }
}
