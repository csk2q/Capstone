using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ServerBee.Data.Models;

namespace ServerBee.Data
{
    public partial class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {


        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<CreditHistory> CreditHistories { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerCredential> CustomerCredentials { get; set; }
        public virtual DbSet<Loan> Loans { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.AccountId);
                entity.ToTable("account");

                entity.Property(e => e.AccountId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("account_id");
                entity.Property(e => e.AccountNumber)
                    .HasMaxLength(20)
                    .HasColumnName("account_number");
                entity.Property(e => e.AccountType)
                    .HasMaxLength(20)
                    .HasColumnName("account_type");
                entity.Property(e => e.CurrentBalance)
                    .HasPrecision(15, 2)
                    .HasColumnName("current_balance");
                entity.Property(e => e.CustomerId).HasColumnName("customer_id");
                entity.Property(e => e.OpeningBalance)
                    .HasPrecision(15, 2)
                    .HasColumnName("opening_balance");
            });

            modelBuilder.Entity<CreditHistory>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("credit_history");

                entity.Property(e => e.CreditHistoryId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("credit_history_id");
                entity.Property(e => e.CreditLines).HasColumnName("credit_lines");
                entity.Property(e => e.CreditScore).HasColumnName("credit_score");
                entity.Property(e => e.CustomerId).HasColumnName("customer_id");
                entity.Property(e => e.PastPaymentHistory).HasColumnName("past_payment_history");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("customer");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .HasColumnName("address");
                entity.Property(e => e.CustomerId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("customer_id");
                entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
                entity.Property(e => e.EmailAddress)
                    .HasMaxLength(100)
                    .HasColumnName("email_address");
                entity.Property(e => e.FullName)
                    .HasMaxLength(100)
                    .HasColumnName("full_name");
                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20)
                    .HasColumnName("phone_number");
                entity.Property(e => e.Ssn)
                    .HasMaxLength(20)
                    .HasColumnName("ssn");
            });

            modelBuilder.Entity<CustomerCredential>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("customer_credentials");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");
                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("password");
                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .HasColumnName("username");
            });

            modelBuilder.Entity<Loan>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("loan");

                entity.Property(e => e.CollateralDetails).HasColumnName("collateral_details");
                entity.Property(e => e.CustomerId).HasColumnName("customer_id");
                entity.Property(e => e.InterestRate)
                    .HasPrecision(5, 2)
                    .HasColumnName("interest_rate");
                entity.Property(e => e.LoanAmount)
                    .HasPrecision(15, 2)
                    .HasColumnName("loan_amount");
                entity.Property(e => e.LoanId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("loan_id");
                entity.Property(e => e.RepaymentSchedule)
                    .HasMaxLength(100)
                    .HasColumnName("repayment_schedule");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId);
                entity.ToTable("transaction");

                entity.Property(e => e.AccountId).HasColumnName("account_id");
                entity.Property(e => e.Amount)
                    .HasPrecision(10, 2)
                    .HasColumnName("amount");
                entity.Property(e => e.Date).HasColumnName("date");
                entity.Property(e => e.PayeePayerAccountNumber)
                    .HasMaxLength(20)
                    .HasColumnName("payee_payer_account_number");
                entity.Property(e => e.PayeePayerName)
                    .HasMaxLength(100)
                    .HasColumnName("payee_payer_name");
                entity.Property(e => e.Time).HasColumnName("time");
                entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
                entity.Property(e => e.TransactionType)
                    .HasMaxLength(50)
                    .HasColumnName("transaction_type");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
