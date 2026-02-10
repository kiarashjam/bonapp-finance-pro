using FinancePro.Domain.Enums;

namespace FinancePro.Application.DTOs;

public record JournalEntryDto(int Id, string ReferenceNumber, DateTime EntryDate, string? Description, JournalEntryStatus Status, JournalEntrySource Source, decimal TotalDebit, decimal TotalCredit, List<JournalLineDto> Lines, DateTime CreatedAt);
public record JournalLineDto(int Id, int LedgerAccountId, string AccountNumber, string AccountName, decimal DebitAmount, decimal CreditAmount, string? Description, VatRate? VatRate, decimal? VatAmount);
public record CreateJournalEntryRequest(DateTime EntryDate, string? Description, List<CreateJournalLineRequest> Lines);
public record CreateJournalLineRequest(int LedgerAccountId, decimal DebitAmount, decimal CreditAmount, string? Description, VatRate? VatRate, decimal? VatAmount);
