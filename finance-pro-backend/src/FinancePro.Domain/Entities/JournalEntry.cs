using FinancePro.Domain.Enums;

namespace FinancePro.Domain.Entities;

public class JournalEntry : BaseEntity
{
    public string ReferenceNumber { get; set; } = string.Empty;
    public DateTime EntryDate { get; set; }
    public string? Description { get; set; }
    public JournalEntryStatus Status { get; set; } = JournalEntryStatus.Draft;
    public JournalEntrySource Source { get; set; } = JournalEntrySource.Manual;
    public string? SourceReferenceId { get; set; }
    public int OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public int? LocationId { get; set; }
    public DateTime? PostedAt { get; set; }
    public string? PostedBy { get; set; }

    public ICollection<JournalLine> Lines { get; set; } = new List<JournalLine>();

    public decimal TotalDebit => Lines.Sum(l => l.DebitAmount);
    public decimal TotalCredit => Lines.Sum(l => l.CreditAmount);
    public bool IsBalanced => TotalDebit == TotalCredit;
}

public class JournalLine : BaseEntity
{
    public int JournalEntryId { get; set; }
    public JournalEntry JournalEntry { get; set; } = null!;
    public int LedgerAccountId { get; set; }
    public LedgerAccount LedgerAccount { get; set; } = null!;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string? Description { get; set; }
    public VatRate? VatRate { get; set; }
    public decimal? VatAmount { get; set; }
}
