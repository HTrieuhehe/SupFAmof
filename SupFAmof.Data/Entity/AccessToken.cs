using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class AccessToken
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public string AccessToken1 { get; set; } = null!;
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual staff Staff { get; set; } = null!;
    }
}
