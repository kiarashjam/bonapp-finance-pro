using FinancePro.Domain.Enums;

namespace FinancePro.Domain.Entities;

public class PaymentSource : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public IntegrationSourceType SourceType { get; set; }
    public string ApiKey { get; set; } = string.Empty;
    public string? ApiKeyHash { get; set; }
    public bool IsActive { get; set; } = true;
    public string? WebhookUrl { get; set; }
    public string? Description { get; set; }
    public int OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    public ICollection<PaymentRecord> PaymentRecords { get; set; } = new List<PaymentRecord>();
    public ICollection<Payout> Payouts { get; set; } = new List<Payout>();
}
