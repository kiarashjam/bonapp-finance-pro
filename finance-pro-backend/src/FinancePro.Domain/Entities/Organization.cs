namespace FinancePro.Domain.Entities;

public class Organization : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? Address { get; set; }
    public string? Street { get; set; }
    public string? HouseNumber { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; } = "CH";
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? VatId { get; set; }
    public string? Iban { get; set; }
    public string? QrIban { get; set; }
    public string? LogoUrl { get; set; }
    public string Currency { get; set; } = "CHF";
    public int FiscalYearStartMonth { get; set; } = 1; // January
    public string DefaultLanguage { get; set; } = "de";

    // Navigation
    public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
    public ICollection<LedgerAccount> LedgerAccounts { get; set; } = new List<LedgerAccount>();
    public ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
    public ICollection<PaymentSource> PaymentSources { get; set; } = new List<PaymentSource>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    public ICollection<Vendor> Vendors { get; set; } = new List<Vendor>();
    public ICollection<TaxRate> TaxRates { get; set; } = new List<TaxRate>();
}
