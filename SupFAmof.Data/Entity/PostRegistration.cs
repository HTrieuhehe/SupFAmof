﻿using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class PostRegistration
    {
        public PostRegistration()
        {
            CheckAttendances = new HashSet<CheckAttendance>();
            PostRegistrationDetails = new HashSet<PostRegistrationDetail>();
            PostTgupdateHistories = new HashSet<PostTgupdateHistory>();
        }

        public int Id { get; set; }
        public string RegistrationCode { get; set; } = null!;
        public int AccountId { get; set; }
        public int Status { get; set; }
        public bool? SchoolBusOption { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual ICollection<CheckAttendance> CheckAttendances { get; set; }
        public virtual ICollection<PostRegistrationDetail> PostRegistrationDetails { get; set; }
        public virtual ICollection<PostTgupdateHistory> PostTgupdateHistories { get; set; }
    }
}
