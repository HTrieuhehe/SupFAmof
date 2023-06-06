using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class PostPosition
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int PositionId { get; set; }
        public int Amount { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Postition Position { get; set; } = null!;
        public virtual Post Post { get; set; } = null!;
    }
}
