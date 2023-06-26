using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Test
{
    public partial class AccountBanned
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public DateTime DayStart { get; set; }
        public DateTime DayEnd { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
