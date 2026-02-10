using FinancePro.Domain.Enums;

namespace FinancePro.Domain.Entities;

public class Expense : BaseEntity
{
    public DateTime ExpenseDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal VatAmount { get; set; }
    public decimal NetAmount { get; set; }
    public VatRate VatRate { get; set; }
    public ExpenseCategory Category { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? ReceiptUrl { get; set; }
    public string? Notes { get; set; }
    public bool IsRecurring { get; set; }
    public int? VendorId { get; set; }
    public Vendor? Vendor { get; set; }
    public int OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public int? LocationId { get; set; }
    public int? JournalEntryId { get; set; }
    public JournalEntry? JournalEntry { get; set; }

    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}
