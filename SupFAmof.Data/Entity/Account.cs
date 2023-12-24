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
            AccountCertificateCertificateIssuers = new HashSet<AccountCertificate>();
            AccountContracts = new HashSet<AccountContract>();
            AccountInformations = new HashSet<AccountInformation>();
            AccountReactivations = new HashSet<AccountReactivation>();
            AccountReports = new HashSet<AccountReport>();
            ApplicationAccountReplies = new HashSet<Application>();
            ApplicationAccounts = new HashSet<Application>();
            CertificateRegistrations = new HashSet<CertificateRegistration>();
            Contracts = new HashSet<Contract>();
            ExpoPushTokens = new HashSet<ExpoPushToken>();
            NotificationHistories = new HashSet<NotificationHistory>();
            PostRegistrations = new HashSet<PostRegistration>();
            Posts = new HashSet<Post>();
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
        public virtual ICollection<AccountCertificate> AccountCertificateCertificateIssuers { get; set; }
        public virtual ICollection<AccountContract> AccountContracts { get; set; }
        public virtual ICollection<AccountInformation> AccountInformations { get; set; }
        public virtual ICollection<AccountReactivation> AccountReactivations { get; set; }
        public virtual ICollection<AccountReport> AccountReports { get; set; }
        public virtual ICollection<Application> ApplicationAccountReplies { get; set; }
        public virtual ICollection<Application> ApplicationAccounts { get; set; }
        public virtual ICollection<CertificateRegistration> CertificateRegistrations { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<ExpoPushToken> ExpoPushTokens { get; set; }
        public virtual ICollection<NotificationHistory> NotificationHistories { get; set; }
        public virtual ICollection<PostRegistration> PostRegistrations { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
