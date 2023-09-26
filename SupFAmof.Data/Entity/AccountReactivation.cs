using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class AccountReactivation
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Email { get; set; } = null!;
        public int? VerifyCode { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime DeactivateDate { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
