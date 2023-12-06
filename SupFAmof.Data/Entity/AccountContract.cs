using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class AccountContract
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int AccountId { get; set; }
        public byte[]? SubmittedBinaryFile { get; set; }
        public string? SubmittedFile { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Contract Contract { get; set; } = null!;
    }
}
