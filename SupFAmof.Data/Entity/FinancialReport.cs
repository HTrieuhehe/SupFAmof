using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class FinancialReport
    {
        public int Id { get; set; }
        public DateTime DateReport { get; set; }
        public double TotalAmount { get; set; }
    }
}
