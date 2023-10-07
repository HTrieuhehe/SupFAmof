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
            AccountCertificateAccounts = new HashSet<AccountCertificate>();
            AccountCertificateCreatePeople = new HashSet<AccountCertificate>();
            AccountContracts = new HashSet<AccountContract>();
            AccountInformations = new HashSet<AccountInformation>();
            AccountReactivations = new HashSet<AccountReactivation>();
            AccountReports = new HashSet<AccountReport>();
            CheckAttendances = new HashSet<CheckAttendance>();
            Contracts = new HashSet<Contract>();
            Fcmtokens = new HashSet<Fcmtoken>();
            PostAttendees = new HashSet<PostAttendee>();
            PostRegistrations = new HashSet<PostRegistration>();
            Posts = new HashSet<Post>();
            Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public int RoleId { get; set; }
        public int? AccountInformationId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ImgUrl { get; set; }
        public bool PostPermission { get; set; }
        public bool? IsPremium { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual AccountInformation? AccountInformation { get; set; }
        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<AccountBanking> AccountBankings { get; set; }
        public virtual ICollection<AccountBanned> AccountBanneds { get; set; }
        public virtual ICollection<AccountCertificate> AccountCertificateAccounts { get; set; }
        public virtual ICollection<AccountCertificate> AccountCertificateCreatePeople { get; set; }
        public virtual ICollection<AccountContract> AccountContracts { get; set; }
        public virtual ICollection<AccountInformation> AccountInformations { get; set; }
        public virtual ICollection<AccountReactivation> AccountReactivations { get; set; }
        public virtual ICollection<AccountReport> AccountReports { get; set; }
        public virtual ICollection<CheckAttendance> CheckAttendances { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<Fcmtoken> Fcmtokens { get; set; }
        public virtual ICollection<PostAttendee> PostAttendees { get; set; }
        public virtual ICollection<PostRegistration> PostRegistrations { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
