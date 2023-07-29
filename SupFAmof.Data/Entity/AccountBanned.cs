using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class AccountBanned
    {
        public int Id { get; set; }
        public int AccountIdBanned { get; set; }
        public int BannedPersonId { get; set; }
        public DateTime DayStart { get; set; }
        public DateTime DayEnd { get; set; }

        public virtual Account BannedPerson { get; set; } = null!;
    }
}
