using FinancePro.Domain.Enums;

namespace FinancePro.Domain.Entities;

public class Payout : BaseEntity
{
    public int PaymentSourceId { get; set; }
    public PaymentSource PaymentSource { get; set; } = null!;
    public decimal ExpectedAmount { get; set; }
    public decimal? ActualAmount { get; set; }
    public decimal? Fees { get; set; }
    public DateTime ExpectedDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public PayoutStatus Status { get; set; } = PayoutStatus.Pending;
    public string? ProviderReference { get; set; }
    public string? Notes { get; set; }
    public int OrganizationId { get; set; }
    public int? ReconciliationMatchId { get; set; }
    public ReconciliationMatch? ReconciliationMatch { get; set; }
}
