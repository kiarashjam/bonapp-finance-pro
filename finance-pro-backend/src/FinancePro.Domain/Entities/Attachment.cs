namespace FinancePro.Domain.Entities;

public class Attachment : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string StorageUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int? ExpenseId { get; set; }
    public Expense? Expense { get; set; }
    public int OrganizationId { get; set; }
}
