using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class AccountInformation
    {
        public AccountInformation()
        {
            Accounts = new HashSet<Account>();
        }

        public int Id { get; set; }
        public int AccountId { get; set; }
        public string? PersonalId { get; set; }
        public string? IdStudent { get; set; }
        public string? FbUrl { get; set; }
        public string? Address { get; set; }
        public DateTime? PersonalIdDate { get; set; }
        public string? PlaceOfIssue { get; set; }
        public byte[]? PersonalIdFrontImg { get; set; }
        public byte[]? PersonalIdBackImg { get; set; }
        public string? TaxNumber { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual ICollection<Account> Accounts { get; set; }
    }
}
