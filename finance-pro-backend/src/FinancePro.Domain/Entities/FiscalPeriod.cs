using FinancePro.Domain.Enums;

namespace FinancePro.Domain.Entities;

public class FiscalPeriod : BaseEntity
{
    public string Name { get; set; } = string.Empty; // e.g., "2026-01", "2026-Q1"
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public FiscalPeriodStatus Status { get; set; } = FiscalPeriodStatus.Open;
    public DateTime? ClosedAt { get; set; }
    public string? ClosedBy { get; set; }
    public int OrganizationId { get; set; }
}
