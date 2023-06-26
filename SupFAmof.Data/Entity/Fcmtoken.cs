using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Test
{
    public partial class Fcmtoken
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public int? StaffId { get; set; }
        public string Token { get; set; } = null!;
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Account? Account { get; set; }
        public virtual staff? Staff { get; set; }
    }
}
