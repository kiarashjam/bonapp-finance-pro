using FinancePro.Domain.Enums;

namespace FinancePro.Domain.Entities;

public class PaymentRecord : BaseEntity
{
    public string? ExternalId { get; set; }
    public string? OrderReference { get; set; }
    public decimal Amount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal TipAmount { get; set; }
    public decimal VatAmount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentEventType EventType { get; set; }
    public VatRate VatRate { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? Description { get; set; }
    public int PaymentSourceId { get; set; }
    public PaymentSource PaymentSource { get; set; } = null!;
    public int OrganizationId { get; set; }
    public int? LocationId { get; set; }
    public int? DailySalesSummaryId { get; set; }
    public DailySalesSummary? DailySalesSummary { get; set; }
}
