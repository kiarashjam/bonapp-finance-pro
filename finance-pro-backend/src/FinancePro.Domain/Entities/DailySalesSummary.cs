using FinancePro.Domain.Enums;

namespace FinancePro.Domain.Entities;

public class DailySalesSummary : BaseEntity
{
    public DateTime Date { get; set; }
    public decimal GrossSales { get; set; }
    public decimal NetSales { get; set; }
    public decimal TotalRefunds { get; set; }
    public decimal TotalTips { get; set; }
    public decimal CashSales { get; set; }
    public decimal CardSales { get; set; }
    public decimal OnlineSales { get; set; }
    public decimal OtherSales { get; set; }
    public decimal VatStandard { get; set; }   // 8.1%
    public decimal VatReduced { get; set; }    // 2.6%
    public decimal VatHospitality { get; set; }// 3.8%
    public int TransactionCount { get; set; }
    public int GuestCount { get; set; }
    public DayStatus DayStatus { get; set; } = DayStatus.Open;
    public DateTime? ClosedAt { get; set; }
    public string? ClosedBy { get; set; }
    public int? JournalEntryId { get; set; }
    public JournalEntry? JournalEntry { get; set; }
    public int OrganizationId { get; set; }
    public int? LocationId { get; set; }

    public ICollection<PaymentRecord> PaymentRecords { get; set; } = new List<PaymentRecord>();

    public decimal AverageCheck => TransactionCount > 0 ? NetSales / TransactionCount : 0;
}
