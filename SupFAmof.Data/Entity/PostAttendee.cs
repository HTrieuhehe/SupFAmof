using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class PostAttendee
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int? PositionId { get; set; }
        public int PostId { get; set; }
        public DateTime ConfirmAt { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual PostPosition? Position { get; set; }
        public virtual Post Post { get; set; } = null!;
    }
}
