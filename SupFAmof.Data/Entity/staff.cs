﻿using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Test
{
    public partial class staff
    {
        public staff()
        {
            Fcmtokens = new HashSet<Fcmtoken>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public byte[] Password { get; set; } = null!;
        public bool IsAvailable { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual ICollection<Fcmtoken> Fcmtokens { get; set; }
    }
}
