﻿using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class PostRegistrationDetail
    {
        public int Id { get; set; }
        public int PostRegistrationId { get; set; }
        public int PostId { get; set; }
        public int PositionId { get; set; }
        public string? Note { get; set; }
        public double Salary { get; set; }

        public virtual Post Post { get; set; } = null!;
        public virtual PostRegistration PostRegistration { get; set; } = null!;
    }
}
