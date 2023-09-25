using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class Contract
    {
        public Contract()
        {
            AccountContracts = new HashSet<AccountContract>();
        }

        public int Id { get; set; }
        public string ContractName { get; set; } = null!;
        public string ContractDescription { get; set; } = null!;
        public byte[] SampleFile { get; set; } = null!;
        public DateTime SigningDate { get; set; }
        public DateTime StartDate { get; set; }
        public double TotalSalary { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAtd { get; set; }

        public virtual ICollection<AccountContract> AccountContracts { get; set; }
    }
}
