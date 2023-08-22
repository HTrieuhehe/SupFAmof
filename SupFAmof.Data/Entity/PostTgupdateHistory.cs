﻿using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class PostTgupdateHistory
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int PostRegistrationId { get; set; }
        public int? PositionId { get; set; }
        public bool? BusOption { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual PostPosition? Position { get; set; }
        public virtual Post Post { get; set; } = null!;
        public virtual PostRegistration PostRegistration { get; set; } = null!;
    }
}
