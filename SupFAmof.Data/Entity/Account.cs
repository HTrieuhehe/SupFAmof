using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class Account
    {
        public Account()
        {
            AccountBanneds = new HashSet<AccountBanned>();
            AccountReports = new HashSet<AccountReport>();
            PostRegistrations = new HashSet<PostRegistration>();
            Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public int RoleId { get; set; }
        public string? Name { get; set; }
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string IdStudent { get; set; } = null!;
        public string FbUrl { get; set; } = null!;
        public DateTime? DateOfBirth { get; set; }
        public string? ImgUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<AccountBanned> AccountBanneds { get; set; }
        public virtual ICollection<AccountReport> AccountReports { get; set; }
        public virtual ICollection<PostRegistration> PostRegistrations { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
