using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class Account
    {
        public Account()
        {
            AccountBankings = new HashSet<AccountBanking>();
            AccountBanneds = new HashSet<AccountBanned>();
            AccountCertificates = new HashSet<AccountCertificate>();
            AccountInformations = new HashSet<AccountInformation>();
            Fcmtokens = new HashSet<Fcmtoken>();
            PostRegistrations = new HashSet<PostRegistration>();
            Posts = new HashSet<Post>();
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
        public bool PostPermission { get; set; }
        public bool? IsPremium { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<AccountBanking> AccountBankings { get; set; }
        public virtual ICollection<AccountBanned> AccountBanneds { get; set; }
        public virtual ICollection<AccountCertificate> AccountCertificates { get; set; }
        public virtual ICollection<AccountInformation> AccountInformations { get; set; }
        public virtual ICollection<Fcmtoken> Fcmtokens { get; set; }
        public virtual ICollection<PostRegistration> PostRegistrations { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
