using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public double Amount { get; set; }
        public string? Notes { get; set; }
        public int Status { get; set; }
        public DateTime PaymentDate { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
