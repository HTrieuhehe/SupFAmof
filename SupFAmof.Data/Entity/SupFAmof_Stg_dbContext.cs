using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SupFAmof.Data.Entity;

namespace SupFAmof.Data.Entity
{
    public partial class SupFAmof_Stg_dbContext : DbContext
    {
        public SupFAmof_Stg_dbContext()
        {
        }

        public SupFAmof_Stg_dbContext(DbContextOptions<SupFAmof_Stg_dbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<AccountBanking> AccountBankings { get; set; } = null!;
        public virtual DbSet<AccountBanned> AccountBanneds { get; set; } = null!;
        public virtual DbSet<AccountCertificate> AccountCertificates { get; set; } = null!;
        public virtual DbSet<AccountReport> AccountReports { get; set; } = null!;
        public virtual DbSet<ActionLog> ActionLogs { get; set; } = null!;
        public virtual DbSet<CheckAttendance> CheckAttendances { get; set; } = null!;
        public virtual DbSet<Contract> Contracts { get; set; } = null!;
        public virtual DbSet<Fcmtoken> Fcmtokens { get; set; } = null!;
        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<PostPosition> PostPositions { get; set; } = null!;
        public virtual DbSet<PostRegistration> PostRegistrations { get; set; } = null!;
        public virtual DbSet<PostRegistrationDetail> PostRegistrationDetails { get; set; } = null!;
        public virtual DbSet<PostTitle> PostTitles { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<TranningCertificate> TranningCertificates { get; set; } = null!;
        public virtual DbSet<Transaction> Transactions { get; set; } = null!;
        public virtual DbSet<staff> staff { get; set; } = null!;

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                optionsBuilder.UseSqlServer("Server=54.179.129.105;Database=SupFAmof_Stg_db;User ID=sa;Password=lWN5!fI98WG02zE26ix$;MultipleActiveResultSets=true;Integrated Security=true;Trusted_Connection=False;Encrypt=True;TrustServerCertificate=True", x => x.UseNetTopologySuite());
//            }
//        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.DateOfBirth).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.FbUrl).HasMaxLength(225);

                entity.Property(e => e.IdStudent).HasMaxLength(10);

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.PersonalId).HasMaxLength(20);

                entity.Property(e => e.PersonalIdDate).HasColumnType("date");

                entity.Property(e => e.PersonalIdDestination).HasMaxLength(225);

                entity.Property(e => e.Phone).HasMaxLength(15);

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Account_Role");
            });

            modelBuilder.Entity<AccountBanking>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("AccountBanking");

                entity.Property(e => e.AccountNumber).HasMaxLength(50);

                entity.Property(e => e.BankName).HasMaxLength(50);

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany()
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountBanking_Account");
            });

            modelBuilder.Entity<AccountBanned>(entity =>
            {
                entity.ToTable("AccountBanned");

                entity.Property(e => e.DayEnd).HasColumnType("date");

                entity.Property(e => e.DayStart).HasColumnType("date");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountBanneds)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountBanned_Account");
            });

            modelBuilder.Entity<AccountCertificate>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("AccountCertificate");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany()
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountCertificate_Account");

                entity.HasOne(d => d.TraningCertificate)
                    .WithMany()
                    .HasForeignKey(d => d.TraningCertificateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountCertificate_TranningCertificate");
            });

            modelBuilder.Entity<AccountReport>(entity =>
            {
                entity.ToTable("AccountReport");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountReports)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountReport_Account");
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

                entity.Property(e => e.IdStudent).HasMaxLength(15);

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.CheckAttendances)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CheckAttendance_Post");
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

                entity.Property(e => e.PostCode).HasMaxLength(50);

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.PostTitle)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.PostTitleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Post_PostTitle");
            });

            modelBuilder.Entity<PostPosition>(entity =>
            {
                entity.ToTable("PostPosition");

                entity.Property(e => e.Position).HasMaxLength(50);

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.PostPositions)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostPosition_Post");
            });

            modelBuilder.Entity<PostRegistration>(entity =>
            {
                entity.ToTable("PostRegistration");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.RegistationCode).HasMaxLength(30);

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.PostRegistrations)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostRegistation_Account");
            });

            modelBuilder.Entity<PostRegistrationDetail>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("PostRegistrationDetail");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Note).HasMaxLength(256);

                entity.HasOne(d => d.Post)
                    .WithMany()
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostRegistrationDetail_Post");

                entity.HasOne(d => d.PostRegistration)
                    .WithMany()
                    .HasForeignKey(d => d.PostRegistrationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostRegistrationDetail_PostRegistration1");
            });

            modelBuilder.Entity<PostTitle>(entity =>
            {
                entity.ToTable("PostTitle");

                entity.Property(e => e.CreatAt).HasColumnType("datetime");

                entity.Property(e => e.PostTitleDescription).HasMaxLength(50);

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.RoleEmail).HasMaxLength(20);

                entity.Property(e => e.RoleName).HasMaxLength(20);

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<TranningCertificate>(entity =>
            {
                entity.ToTable("TranningCertificate");

                entity.Property(e => e.CertificateName).HasMaxLength(50);

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");
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
