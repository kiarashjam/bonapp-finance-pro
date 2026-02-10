using FinancePro.Domain.Enums;

namespace FinancePro.Domain.Entities;

public class ReconciliationMatch : BaseEntity
{
    public int? BankTransactionId { get; set; }
    public BankTransaction? BankTransaction { get; set; }
    public int? PayoutId { get; set; }
    public Payout? Payout { get; set; }
    public ReconciliationMatchType MatchType { get; set; }
    public decimal? ConfidenceScore { get; set; }
    public string? Notes { get; set; }
    public string? MatchedBy { get; set; }
    public DateTime MatchedAt { get; set; } = DateTime.UtcNow;
    public int OrganizationId { get; set; }
}
