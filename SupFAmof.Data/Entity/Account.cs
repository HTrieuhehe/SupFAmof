using System;
using System.Collections.Generic;
using SupFAmof.Data.Entity;

namespace SupFAmof.Data.Entity
{
    public partial class Account
    {
        public Account()
        {
            AccountBanneds = new HashSet<AccountBanned>();
            AccountReports = new HashSet<AccountReport>();
            Fcmtokens = new HashSet<Fcmtoken>();
            PostRegistrations = new HashSet<PostRegistration>();
            Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public int RoleId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? IdStudent { get; set; }
        public string? FbUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ImgUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<AccountBanned> AccountBanneds { get; set; }
        public virtual ICollection<AccountReport> AccountReports { get; set; }
        public virtual ICollection<Fcmtoken> Fcmtokens { get; set; }
        public virtual ICollection<PostRegistration> PostRegistrations { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
