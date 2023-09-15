using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace SupFAmof.Data.Entity
{
    public partial class SupFAmOf_Stg_Db_Ver_2Context : DbContext
    {
        public SupFAmOf_Stg_Db_Ver_2Context()
        {
        }

        public SupFAmOf_Stg_Db_Ver_2Context(DbContextOptions<SupFAmOf_Stg_Db_Ver_2Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<AccountBanking> AccountBankings { get; set; } = null!;
        public virtual DbSet<AccountBanned> AccountBanneds { get; set; } = null!;
        public virtual DbSet<AccountCertificate> AccountCertificates { get; set; } = null!;
        public virtual DbSet<AccountInformation> AccountInformations { get; set; } = null!;
        public virtual DbSet<ActionLog> ActionLogs { get; set; } = null!;
        public virtual DbSet<CheckAttendance> CheckAttendances { get; set; } = null!;
        public virtual DbSet<Contract> Contracts { get; set; } = null!;
        public virtual DbSet<Fcmtoken> Fcmtokens { get; set; } = null!;
        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<PostPosition> PostPositions { get; set; } = null!;
        public virtual DbSet<PostRegistration> PostRegistrations { get; set; } = null!;
        public virtual DbSet<PostRegistrationDetail> PostRegistrationDetails { get; set; } = null!;
        public virtual DbSet<PostRgupdateHistory> PostRgupdateHistories { get; set; } = null!;
        public virtual DbSet<PostTitle> PostTitles { get; set; } = null!;
        public virtual DbSet<PostTrainingCertificate> PostTrainingCertificates { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<TrainingCertificate> TrainingCertificates { get; set; } = null!;
        public virtual DbSet<TrainingPosition> TrainingPositions { get; set; } = null!;
        public virtual DbSet<Transaction> Transactions { get; set; } = null!;
        public virtual DbSet<staff> staff { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfiguration config = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json").Build();
                string connectionString = config.GetConnectionString("SQLServerDatabase");
                optionsBuilder.UseSqlServer(connectionString);
            }

            base.OnConfiguring(optionsBuilder);
        }

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

                entity.HasOne(d => d.CreatePerson)
                    .WithMany(p => p.AccountCertificateCreatePeople)
                    .HasForeignKey(d => d.CreatePersonId)
                    .HasConstraintName("FK_AccountCertificate_Account1");

                entity.HasOne(d => d.TraningCertificate)
                    .WithMany(p => p.AccountCertificates)
                    .HasForeignKey(d => d.TraningCertificateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountCertificate_TranningCertificate");
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

            modelBuilder.Entity<ActionLog>(entity =>
            {
                entity.ToTable("ActionLog");

                entity.Property(e => e.ActionLog1).HasColumnName("ActionLog");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<CheckAttendance>(entity =>
            {
                entity.ToTable("CheckAttendance");

                entity.Property(e => e.CheckInDate).HasColumnType("datetime");

                entity.HasOne(d => d.PostRegistration)
                    .WithMany(p => p.CheckAttendances)
                    .HasForeignKey(d => d.PostRegistrationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CheckAttendance_PostRegistration");
            });

            modelBuilder.Entity<Contract>(entity =>
            {
                entity.ToTable("Contract");

                entity.Property(e => e.ContractDescription).HasMaxLength(225);

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.PostTitle)
                    .WithMany(p => p.Contracts)
                    .HasForeignKey(d => d.PostTitleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Contract_PostTitle");
            });

            modelBuilder.Entity<Fcmtoken>(entity =>
            {
                entity.ToTable("FCMToken");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Fcmtokens)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_FCMToken_Account");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.Fcmtokens)
                    .HasForeignKey(d => d.StaffId)
                    .HasConstraintName("FK_AccessToken_Staff");
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

                entity.HasOne(d => d.PostTitle)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.PostTitleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Post_PostTitle");
            });

            modelBuilder.Entity<PostPosition>(entity =>
            {
                entity.ToTable("PostPosition");

                entity.Property(e => e.Location).HasMaxLength(500);

                entity.Property(e => e.PositionName).HasMaxLength(50);

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.PostPositions)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostPosition_Post");

                entity.HasOne(d => d.TrainingCertificate)
                    .WithMany(p => p.PostPositions)
                    .HasForeignKey(d => d.TrainingCertificateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostPosition_TranningCertificate");
            });

            modelBuilder.Entity<PostRegistration>(entity =>
            {
                entity.ToTable("PostRegistration");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.RegistrationCode).HasMaxLength(30);

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.PostRegistrations)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostRegistation_Account");
            });

            modelBuilder.Entity<PostRegistrationDetail>(entity =>
            {
                entity.ToTable("PostRegistrationDetail");

                entity.Property(e => e.Note).HasMaxLength(256);

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.PostRegistrationDetails)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostRegistrationDetail_Post");

                entity.HasOne(d => d.PostRegistration)
                    .WithMany(p => p.PostRegistrationDetails)
                    .HasForeignKey(d => d.PostRegistrationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostRegistrationDetail_PostRegistration1");
            });

            modelBuilder.Entity<PostRgupdateHistory>(entity =>
            {
                entity.ToTable("PostRGUpdateHistory");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.PostRgupdateHistories)
                    .HasForeignKey(d => d.PositionId)
                    .HasConstraintName("FK_PostTGupdateHistory_PostPosition");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.PostRgupdateHistories)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostTGupdateHistory_Post");

                entity.HasOne(d => d.PostRegistration)
                    .WithMany(p => p.PostRgupdateHistories)
                    .HasForeignKey(d => d.PostRegistrationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostTGupdateHistory_PostRegistration");
            });

            modelBuilder.Entity<PostTitle>(entity =>
            {
                entity.ToTable("PostTitle");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.PostTitleDescription).HasMaxLength(50);

                entity.Property(e => e.PostTitleType).HasMaxLength(10);

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<PostTrainingCertificate>(entity =>
            {
                entity.ToTable("PostTrainingCertificate");

                entity.HasOne(d => d.PostTitle)
                    .WithMany(p => p.PostTrainingCertificates)
                    .HasForeignKey(d => d.PostTitleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostTrainingCertificate_PostTitle");

                entity.HasOne(d => d.TrainingCerti)
                    .WithMany(p => p.PostTrainingCertificates)
                    .HasForeignKey(d => d.TrainingCertiId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostTrainingCertificate_TranningCertificate");
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

            modelBuilder.Entity<TrainingPosition>(entity =>
            {
                entity.ToTable("TrainingPosition");

                entity.Property(e => e.Location).HasMaxLength(500);

                entity.Property(e => e.PositionName).HasMaxLength(50);

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.TrainingPositions)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrainingPosition_Post");

                entity.HasOne(d => d.TrainingCertificate)
                    .WithMany(p => p.TrainingPositions)
                    .HasForeignKey(d => d.TrainingCertificateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrainingPosition_TranningCertificate");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("Transaction");

                entity.Property(e => e.Notes).HasMaxLength(256);

                entity.Property(e => e.PaymentDate).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_Account");
            });

            modelBuilder.Entity<staff>(entity =>
            {
                entity.ToTable("Staff");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Password).HasMaxLength(256);

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.Property(e => e.Username).HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
