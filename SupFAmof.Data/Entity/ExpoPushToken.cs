using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class ExpoPushToken
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public int? AdminId { get; set; }
        public string Token { get; set; } = null!;
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Admin? Admin { get; set; }
    }
}
