namespace FinancePro.Domain.Entities;

public class BankAccount : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Iban { get; set; } = string.Empty;
    public string? BankName { get; set; }
    public string Currency { get; set; } = "CHF";
    public decimal CurrentBalance { get; set; }
    public bool IsActive { get; set; } = true;
    public int OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    public ICollection<BankTransaction> Transactions { get; set; } = new List<BankTransaction>();
}

public class BankTransaction : BaseEntity
{
    public int BankAccountId { get; set; }
    public BankAccount BankAccount { get; set; } = null!;
    public DateTime TransactionDate { get; set; }
    public DateTime? ValueDate { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? Reference { get; set; }
    public bool IsMatched { get; set; }
    public int? ReconciliationMatchId { get; set; }
    public ReconciliationMatch? ReconciliationMatch { get; set; }
    public string? ImportBatchId { get; set; }
}
