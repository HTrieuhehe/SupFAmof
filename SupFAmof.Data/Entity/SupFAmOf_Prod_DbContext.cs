﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SupFAmof.Data.Entity
{
    public partial class SupFAmOf_Prod_DbContext : DbContext
    {
        public SupFAmOf_Prod_DbContext()
        {
        }

        public SupFAmOf_Prod_DbContext(DbContextOptions<SupFAmOf_Prod_DbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<AccountBanking> AccountBankings { get; set; } = null!;
        public virtual DbSet<AccountBanned> AccountBanneds { get; set; } = null!;
        public virtual DbSet<AccountCertificate> AccountCertificates { get; set; } = null!;
        public virtual DbSet<AccountContract> AccountContracts { get; set; } = null!;
        public virtual DbSet<AccountInformation> AccountInformations { get; set; } = null!;
        public virtual DbSet<AccountReactivation> AccountReactivations { get; set; } = null!;
        public virtual DbSet<AccountReport> AccountReports { get; set; } = null!;
        public virtual DbSet<Admin> Admins { get; set; } = null!;
        public virtual DbSet<Application> Applications { get; set; } = null!;
        public virtual DbSet<CheckAttendance> CheckAttendances { get; set; } = null!;
        public virtual DbSet<Contract> Contracts { get; set; } = null!;
        public virtual DbSet<DocumentTemplate> DocumentTemplates { get; set; } = null!;
        public virtual DbSet<ExpoPushToken> ExpoPushTokens { get; set; } = null!;
        public virtual DbSet<NotificationHistory> NotificationHistories { get; set; } = null!;
        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<PostCategory> PostCategories { get; set; } = null!;
        public virtual DbSet<PostPosition> PostPositions { get; set; } = null!;
        public virtual DbSet<PostRegistration> PostRegistrations { get; set; } = null!;
        public virtual DbSet<PostRgupdateHistory> PostRgupdateHistories { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<TrainingCertificate> TrainingCertificates { get; set; } = null!;
        public virtual DbSet<TrainingEventDay> TrainingEventDays { get; set; } = null!;
        public virtual DbSet<TrainingRegistration> TrainingRegistrations { get; set; } = null!;

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                optionsBuilder.UseSqlServer("Server=3.0.33.161;Database=SupFAmOf_Prod_1_Db;User ID=sa;Password=QW0%mG0#%jRC3Z7&T4fL38ygt5Jhhx;MultipleActiveResultSets=true;Integrated Security=true;Trusted_Connection=False;Encrypt=True;TrustServerCertificate=True", x => x.UseNetTopologySuite());
//            }
//        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.DateOfBirth).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Phone).HasMaxLength(15);

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.AccountInformation)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.AccountInformationId)
                    .HasConstraintName("FK_Account_AccountInformation");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Account_Role");
            });

            modelBuilder.Entity<AccountBanking>(entity =>
            {
                entity.ToTable("AccountBanking");

                entity.Property(e => e.AccountNumber).HasMaxLength(50);

                entity.Property(e => e.BankName).HasMaxLength(50);

                entity.Property(e => e.Beneficiary).HasMaxLength(50);

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountBankings)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountBanking_Account");
            });

            modelBuilder.Entity<AccountBanned>(entity =>
            {
                entity.ToTable("AccountBanned");

                entity.Property(e => e.DayEnd).HasColumnType("date");

                entity.Property(e => e.DayStart).HasColumnType("date");

                entity.Property(e => e.Note).HasMaxLength(256);

                entity.HasOne(d => d.BannedPerson)
                    .WithMany(p => p.AccountBanneds)
                    .HasForeignKey(d => d.BannedPersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountBanned_Account");
            });

            modelBuilder.Entity<AccountCertificate>(entity =>
            {
                entity.ToTable("AccountCertificate");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountCertificateAccounts)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountCertificate_Account");

                entity.HasOne(d => d.CertificateIssuer)
                    .WithMany(p => p.AccountCertificateCertificateIssuers)
                    .HasForeignKey(d => d.CertificateIssuerId)
                    .HasConstraintName("FK_AccountCertificate_Account1");

                entity.HasOne(d => d.TrainingCertificate)
                    .WithMany(p => p.AccountCertificates)
                    .HasForeignKey(d => d.TrainingCertificateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountCertificate_TranningCertificate");
            });

            modelBuilder.Entity<AccountContract>(entity =>
            {
                entity.ToTable("AccountContract");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountContracts)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountContract_Account");

                entity.HasOne(d => d.Contract)
                    .WithMany(p => p.AccountContracts)
                    .HasForeignKey(d => d.ContractId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountContract_Contract");
            });

            modelBuilder.Entity<AccountInformation>(entity =>
            {
                entity.ToTable("AccountInformation");

                entity.Property(e => e.Address).HasMaxLength(225);

                entity.Property(e => e.FbUrl).HasMaxLength(225);

                entity.Property(e => e.IdStudent).HasMaxLength(10);

                entity.Property(e => e.IdentityIssueDate).HasColumnType("date");

                entity.Property(e => e.IdentityNumber).HasMaxLength(20);

                entity.Property(e => e.PlaceOfIssue).HasMaxLength(100);

                entity.Property(e => e.TaxNumber).HasMaxLength(50);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountInformations)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountInformation_Account");
            });

            modelBuilder.Entity<AccountReactivation>(entity =>
            {
                entity.ToTable("AccountReactivation");

                entity.Property(e => e.DeactivateDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.ExpirationDate).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountReactivations)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountReactivation_Account");
            });

            modelBuilder.Entity<AccountReport>(entity =>
            {
                entity.ToTable("AccountReport");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountReports)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountReport_Account");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.AccountReports)
                    .HasForeignKey(d => d.PositionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountReport_PostPosition");
            });

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("Admin");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Password).HasMaxLength(256);

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.Property(e => e.Username).HasMaxLength(100);
            });

            modelBuilder.Entity<Application>(entity =>
            {
                entity.ToTable("Application");

                entity.Property(e => e.ProblemNote).HasMaxLength(500);

                entity.Property(e => e.ReplyDate).HasColumnType("datetime");

                entity.Property(e => e.ReplyNote).HasMaxLength(500);

                entity.Property(e => e.ReportDate).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.ApplicationAccounts)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountReportProblem_Account");

                entity.HasOne(d => d.AccountReply)
                    .WithMany(p => p.ApplicationAccountReplies)
                    .HasForeignKey(d => d.AccountReplyId)
                    .HasConstraintName("FK_Application_Account");
            });

            modelBuilder.Entity<CheckAttendance>(entity =>
            {
                entity.ToTable("CheckAttendance");

                entity.Property(e => e.CheckInTime).HasColumnType("datetime");

                entity.Property(e => e.CheckOutTime).HasColumnType("datetime");

                entity.Property(e => e.ConfirmTime).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(100);

                entity.HasOne(d => d.PostRegistration)
                    .WithMany(p => p.CheckAttendances)
                    .HasForeignKey(d => d.PostRegistrationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CheckAttendance_PostRegistration");
            });

            modelBuilder.Entity<Contract>(entity =>
            {
                entity.ToTable("Contract");

                entity.Property(e => e.ContractName).HasMaxLength(100);

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.SigningDate).HasColumnType("date");

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.CreatePerson)
                    .WithMany(p => p.Contracts)
                    .HasForeignKey(d => d.CreatePersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Contract_Account");
            });

            modelBuilder.Entity<DocumentTemplate>(entity =>
            {
                entity.ToTable("DocumentTemplate");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.DocName).HasMaxLength(100);
            });

            modelBuilder.Entity<ExpoPushToken>(entity =>
            {
                entity.ToTable("ExpoPushToken");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.ExpoPushTokens)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_FCMToken_Account");
            });

            modelBuilder.Entity<NotificationHistory>(entity =>
            {
                entity.ToTable("NotificationHistory");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.Text).HasMaxLength(500);

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.HasOne(d => d.Recipient)
                    .WithMany(p => p.NotificationHistories)
                    .HasForeignKey(d => d.RecipientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NotificationHistory_Account");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("Post");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.DateFrom).HasColumnType("date");

                entity.Property(e => e.DateTo).HasColumnType("date");

                entity.Property(e => e.PostCode).HasMaxLength(15);

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Post_Account");

                entity.HasOne(d => d.PostCategory)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.PostCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Post_PostTitle");
            });

            modelBuilder.Entity<PostCategory>(entity =>
            {
                entity.ToTable("PostCategory");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.PostCategoryDescription).HasMaxLength(50);

                entity.Property(e => e.PostCategoryType).HasMaxLength(10);

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<PostPosition>(entity =>
            {
                entity.ToTable("PostPosition");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");

                entity.Property(e => e.Location).HasMaxLength(500);

                entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");

                entity.Property(e => e.PositionDescription).HasMaxLength(100);

                entity.Property(e => e.PositionName).HasMaxLength(30);

                entity.Property(e => e.SchoolName).HasMaxLength(100);

                entity.HasOne(d => d.Document)
                    .WithMany(p => p.PostPositions)
                    .HasForeignKey(d => d.DocumentId)
                    .HasConstraintName("FK_PostPosition_DocumentTemplate");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.PostPositions)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostPosition_Post");

                entity.HasOne(d => d.TrainingCertificate)
                    .WithMany(p => p.PostPositions)
                    .HasForeignKey(d => d.TrainingCertificateId)
                    .HasConstraintName("FK_PostPosition_TranningCertificate");
            });

            modelBuilder.Entity<PostRegistration>(entity =>
            {
                entity.ToTable("PostRegistration");

                entity.Property(e => e.CancelTime).HasColumnType("datetime");

                entity.Property(e => e.ConfirmTime).HasColumnType("datetime");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(256);

                entity.Property(e => e.RegistrationCode).HasMaxLength(30);

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.PostRegistrations)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostRegistation_Account");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.PostRegistrations)
                    .HasForeignKey(d => d.PositionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostRegistration_PostPosition");
            });

            modelBuilder.Entity<PostRgupdateHistory>(entity =>
            {
                entity.ToTable("PostRGUpdateHistory");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(100);

                entity.HasOne(d => d.OriginalPosition)
                    .WithMany(p => p.PostRgupdateHistoryOriginalPositions)
                    .HasForeignKey(d => d.OriginalPositionId)
                    .HasConstraintName("FK_PostRGUpdateHistory_PostPosition");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.PostRgupdateHistoryPositions)
                    .HasForeignKey(d => d.PositionId)
                    .HasConstraintName("FK_PostTGupdateHistory_PostPosition");

                entity.HasOne(d => d.PostRegistration)
                    .WithMany(p => p.PostRgupdateHistories)
                    .HasForeignKey(d => d.PostRegistrationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostTGupdateHistory_PostRegistration");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.RoleEmail).HasMaxLength(20);

                entity.Property(e => e.RoleName).HasMaxLength(20);

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<TrainingCertificate>(entity =>
            {
                entity.ToTable("TrainingCertificate");

                entity.Property(e => e.CertificateName).HasMaxLength(50);

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.TrainingTypeId).HasMaxLength(10);

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<TrainingEventDay>(entity =>
            {
                entity.ToTable("TrainingEventDay");

                entity.Property(e => e.Class).HasMaxLength(100);

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Updateat).HasColumnType("datetime");
            });

            modelBuilder.Entity<TrainingRegistration>(entity =>
            {
                entity.ToTable("TrainingRegistration");

                entity.Property(e => e.ConfirmedAt).HasColumnType("datetime");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.TrainingRegistrations)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrainingRegistration_Account");

                entity.HasOne(d => d.EventDay)
                    .WithMany(p => p.TrainingRegistrations)
                    .HasForeignKey(d => d.EventDayId)
                    .HasConstraintName("FK_TrainingRegistration_TrainingEventDay");

                entity.HasOne(d => d.TrainingCertificate)
                    .WithMany(p => p.TrainingRegistrations)
                    .HasForeignKey(d => d.TrainingCertificateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrainingRegistration_TrainingCertificate");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
