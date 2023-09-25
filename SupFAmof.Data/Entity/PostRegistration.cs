using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class PostRegistration
    {
        public PostRegistration()
        {
            PostRegistrationDetails = new HashSet<PostRegistrationDetail>();
            PostRgupdateHistories = new HashSet<PostRgupdateHistory>();
        }

        public int Id { get; set; }
        public string RegistrationCode { get; set; } = null!;
        public int AccountId { get; set; }
        public int Status { get; set; }
        public bool? SchoolBusOption { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual ICollection<PostRegistrationDetail> PostRegistrationDetails { get; set; }
        public virtual ICollection<PostRgupdateHistory> PostRgupdateHistories { get; set; }
    }
}
