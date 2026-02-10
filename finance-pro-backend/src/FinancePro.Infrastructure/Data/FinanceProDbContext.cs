using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FinancePro.Domain.Entities;

namespace FinancePro.Infrastructure.Data;

public class FinanceProDbContext : IdentityDbContext<AppUser>
{
    public FinanceProDbContext(DbContextOptions<FinanceProDbContext> options) : base(options) { }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<LedgerAccount> LedgerAccounts => Set<LedgerAccount>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalLine> JournalLines => Set<JournalLine>();
    public DbSet<PaymentSource> PaymentSources => Set<PaymentSource>();
    public DbSet<PaymentRecord> PaymentRecords => Set<PaymentRecord>();
    public DbSet<DailySalesSummary> DailySalesSummaries => Set<DailySalesSummary>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceLine> InvoiceLines => Set<InvoiceLine>();
    public DbSet<InvoicePayment> InvoicePayments => Set<InvoicePayment>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<BankTransaction> BankTransactions => Set<BankTransaction>();
    public DbSet<Payout> Payouts => Set<Payout>();
    public DbSet<ReconciliationMatch> ReconciliationMatches => Set<ReconciliationMatch>();
    public DbSet<TaxRate> TaxRates => Set<TaxRate>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<FiscalPeriod> FiscalPeriods => Set<FiscalPeriod>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Organization
        builder.Entity<Organization>(e =>
        {
            e.HasIndex(o => o.Name);
            e.Property(o => o.Currency).HasMaxLength(3);
        });

        // AppUser
        builder.Entity<AppUser>(e =>
        {
            e.HasOne(u => u.Organization)
                .WithMany(o => o.Users)
                .HasForeignKey(u => u.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // LedgerAccount
        builder.Entity<LedgerAccount>(e =>
        {
            e.HasIndex(a => new { a.OrganizationId, a.AccountNumber }).IsUnique();
            e.HasOne(a => a.ParentAccount)
                .WithMany(a => a.ChildAccounts)
                .HasForeignKey(a => a.ParentAccountId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(a => a.Organization)
                .WithMany(o => o.LedgerAccounts)
                .HasForeignKey(a => a.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // JournalEntry
        builder.Entity<JournalEntry>(e =>
        {
            e.HasIndex(j => new { j.OrganizationId, j.ReferenceNumber }).IsUnique();
            e.HasIndex(j => new { j.OrganizationId, j.EntryDate });
            e.HasOne(j => j.Organization)
                .WithMany(o => o.JournalEntries)
                .HasForeignKey(j => j.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // JournalLine
        builder.Entity<JournalLine>(e =>
        {
            e.Property(l => l.DebitAmount).HasPrecision(18, 2);
            e.Property(l => l.CreditAmount).HasPrecision(18, 2);
            e.Property(l => l.VatAmount).HasPrecision(18, 2);
            e.HasOne(l => l.JournalEntry)
                .WithMany(j => j.Lines)
                .HasForeignKey(l => l.JournalEntryId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(l => l.LedgerAccount)
                .WithMany(a => a.JournalLines)
                .HasForeignKey(l => l.LedgerAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // PaymentRecord
        builder.Entity<PaymentRecord>(e =>
        {
            e.Property(p => p.Amount).HasPrecision(18, 2);
            e.Property(p => p.NetAmount).HasPrecision(18, 2);
            e.Property(p => p.TipAmount).HasPrecision(18, 2);
            e.Property(p => p.VatAmount).HasPrecision(18, 2);
            e.HasIndex(p => new { p.OrganizationId, p.TransactionDate });
            e.HasOne(p => p.PaymentSource)
                .WithMany(s => s.PaymentRecords)
                .HasForeignKey(p => p.PaymentSourceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // DailySalesSummary
        builder.Entity<DailySalesSummary>(e =>
        {
            e.HasIndex(d => new { d.OrganizationId, d.Date, d.LocationId }).IsUnique();
            e.Property(d => d.GrossSales).HasPrecision(18, 2);
            e.Property(d => d.NetSales).HasPrecision(18, 2);
            e.Property(d => d.TotalRefunds).HasPrecision(18, 2);
            e.Property(d => d.TotalTips).HasPrecision(18, 2);
            e.Property(d => d.CashSales).HasPrecision(18, 2);
            e.Property(d => d.CardSales).HasPrecision(18, 2);
            e.Property(d => d.OnlineSales).HasPrecision(18, 2);
            e.Property(d => d.OtherSales).HasPrecision(18, 2);
            e.Property(d => d.VatStandard).HasPrecision(18, 2);
            e.Property(d => d.VatReduced).HasPrecision(18, 2);
            e.Property(d => d.VatHospitality).HasPrecision(18, 2);
        });

        // Invoice
        builder.Entity<Invoice>(e =>
        {
            e.HasIndex(i => new { i.OrganizationId, i.InvoiceNumber }).IsUnique();
            e.Property(i => i.SubTotal).HasPrecision(18, 2);
            e.Property(i => i.VatTotal).HasPrecision(18, 2);
            e.Property(i => i.Total).HasPrecision(18, 2);
            e.Property(i => i.PaidAmount).HasPrecision(18, 2);
            e.HasOne(i => i.Customer)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(i => i.Organization)
                .WithMany(o => o.Invoices)
                .HasForeignKey(i => i.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // InvoiceLine
        builder.Entity<InvoiceLine>(e =>
        {
            e.Property(l => l.Quantity).HasPrecision(18, 4);
            e.Property(l => l.UnitPrice).HasPrecision(18, 2);
            e.Property(l => l.VatAmount).HasPrecision(18, 2);
            e.Property(l => l.LineTotal).HasPrecision(18, 2);
        });

        // InvoicePayment
        builder.Entity<InvoicePayment>(e =>
        {
            e.Property(p => p.Amount).HasPrecision(18, 2);
        });

        // Expense
        builder.Entity<Expense>(e =>
        {
            e.Property(x => x.Amount).HasPrecision(18, 2);
            e.Property(x => x.VatAmount).HasPrecision(18, 2);
            e.Property(x => x.NetAmount).HasPrecision(18, 2);
            e.HasIndex(x => new { x.OrganizationId, x.ExpenseDate });
            e.HasOne(x => x.Vendor)
                .WithMany(v => v.Expenses)
                .HasForeignKey(x => x.VendorId)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.Organization)
                .WithMany(o => o.Expenses)
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // BankTransaction
        builder.Entity<BankTransaction>(e =>
        {
            e.Property(t => t.Amount).HasPrecision(18, 2);
            e.HasIndex(t => new { t.BankAccountId, t.TransactionDate });
            e.HasOne(t => t.ReconciliationMatch)
                .WithOne(r => r.BankTransaction)
                .HasForeignKey<BankTransaction>(t => t.ReconciliationMatchId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ReconciliationMatch
        builder.Entity<ReconciliationMatch>(e =>
        {
            e.Property(r => r.ConfidenceScore).HasPrecision(5, 2);
            e.Ignore(r => r.BankTransactionId);
            e.Ignore(r => r.PayoutId);
        });

        // Payout
        builder.Entity<Payout>(e =>
        {
            e.Property(p => p.ExpectedAmount).HasPrecision(18, 2);
            e.Property(p => p.ActualAmount).HasPrecision(18, 2);
            e.Property(p => p.Fees).HasPrecision(18, 2);
            e.HasOne(p => p.PaymentSource)
                .WithMany(s => s.Payouts)
                .HasForeignKey(p => p.PaymentSourceId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.ReconciliationMatch)
                .WithOne(r => r.Payout)
                .HasForeignKey<Payout>(p => p.ReconciliationMatchId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // BankAccount
        builder.Entity<BankAccount>(e =>
        {
            e.Property(b => b.CurrentBalance).HasPrecision(18, 2);
        });

        // Customer
        builder.Entity<Customer>(e =>
        {
            e.Property(c => c.CreditLimit).HasPrecision(18, 2);
        });

        // TaxRate
        builder.Entity<TaxRate>(e =>
        {
            e.Property(t => t.Rate).HasPrecision(8, 4);
        });

        // AuditLog - immutable, no soft delete
        builder.Entity<AuditLog>(e =>
        {
            e.HasIndex(a => new { a.OrganizationId, a.Timestamp });
            e.HasIndex(a => new { a.EntityType, a.EntityId });
        });

        // Global query filter: soft delete
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                builder.Entity(entityType.ClrType).HasQueryFilter(
                    CreateSoftDeleteFilter(entityType.ClrType));
            }
        }
    }

    private static System.Linq.Expressions.LambdaExpression CreateSoftDeleteFilter(Type entityType)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(entityType, "e");
        var property = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
        var condition = System.Linq.Expressions.Expression.Equal(property,
            System.Linq.Expressions.Expression.Constant(false));
        return System.Linq.Expressions.Expression.Lambda(condition, parameter);
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
        return base.SaveChangesAsync(ct);
    }
}
