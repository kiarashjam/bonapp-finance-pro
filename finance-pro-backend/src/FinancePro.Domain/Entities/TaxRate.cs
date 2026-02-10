using FinancePro.Domain.Enums;

namespace FinancePro.Domain.Entities;

public class TaxRate : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public VatRate RateType { get; set; }
    public decimal Rate { get; set; } // 8.1, 2.6, 3.8, 0
    public bool IsActive { get; set; } = true;
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public int OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
}
