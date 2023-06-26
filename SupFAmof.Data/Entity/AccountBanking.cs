using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Test
{
    public partial class AccountBanking
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string AccountNumber { get; set; } = null!;
        public string BankName { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
