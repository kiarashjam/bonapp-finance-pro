using FinancePro.Domain.Enums;

namespace FinancePro.Application.DTOs;

public record BankAccountDto(int Id, string Name, string Iban, string? BankName, string Currency, decimal CurrentBalance, bool IsActive);
public record CreateBankAccountRequest(string Name, string Iban, string? BankName, string Currency);
public record BankTransactionDto(int Id, int BankAccountId, DateTime TransactionDate, DateTime? ValueDate, decimal Amount, string? Description, string? Reference, bool IsMatched);
public record PayoutDto(int Id, int PaymentSourceId, string PaymentSourceName, decimal ExpectedAmount, decimal? ActualAmount, decimal? Fees, DateTime ExpectedDate, DateTime? ActualDate, PayoutStatus Status, string? ProviderReference);
public record CreatePayoutRequest(int PaymentSourceId, decimal ExpectedAmount, DateTime ExpectedDate, string? ProviderReference);
public record ReconciliationDashboardDto(decimal TotalExpectedPayouts, decimal TotalReceivedPayouts, decimal TotalMissingPayouts, decimal TotalDisputedPayouts, int UnmatchedBankTransactions, decimal ReconciliationRate, List<PayoutDto> RecentPayouts, List<BankTransactionDto> UnmatchedTransactions);
