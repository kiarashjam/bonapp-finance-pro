using FinancePro.Domain.Enums;

namespace FinancePro.Domain.Entities;

public class LedgerAccount : BaseEntity
{
    public string AccountNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AccountType AccountType { get; set; }
    public int? ParentAccountId { get; set; }
    public LedgerAccount? ParentAccount { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsSystemAccount { get; set; }
    public int OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    public ICollection<LedgerAccount> ChildAccounts { get; set; } = new List<LedgerAccount>();
    public ICollection<JournalLine> JournalLines { get; set; } = new List<JournalLine>();
}
