using FinancePro.Domain.Enums;

namespace FinancePro.Application.DTOs;

public record PaymentRecordDto(int Id, string? ExternalId, string? OrderReference, decimal Amount, decimal NetAmount, decimal TipAmount, PaymentMethod Method, PaymentEventType EventType, DateTime TransactionDate, string? Description, int PaymentSourceId, string PaymentSourceName);
public record CreatePaymentRequest(string? ExternalId, string? OrderReference, decimal Amount, decimal TipAmount, decimal VatAmount, PaymentMethod Method, PaymentEventType EventType, VatRate VatRate, DateTime TransactionDate, string? Description);
public record DailySalesSummaryDto(int Id, DateTime Date, decimal GrossSales, decimal NetSales, decimal TotalRefunds, decimal TotalTips, decimal CashSales, decimal CardSales, decimal OnlineSales, int TransactionCount, int GuestCount, DayStatus DayStatus, DateTime? ClosedAt, decimal AverageCheck);
public record ManualSalesEntryRequest(DateTime Date, decimal GrossSales, decimal CashSales, decimal CardSales, decimal OnlineSales, decimal OtherSales, decimal TotalTips, decimal VatStandard, decimal VatReduced, decimal VatHospitality, int TransactionCount, int GuestCount);
public record CloseDayPreviewDto(DateTime Date, decimal GrossSales, decimal NetSales, decimal TotalRefunds, decimal TotalTips, int TransactionCount, decimal CashSales, decimal CardSales, List<JournalLineDto> ProposedJournalLines);
public record CloseDayRequest(DateTime Date, bool Confirmed);
