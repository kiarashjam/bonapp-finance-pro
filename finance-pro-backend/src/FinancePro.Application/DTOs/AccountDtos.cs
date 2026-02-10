using FinancePro.Domain.Enums;

namespace FinancePro.Application.DTOs;

public record LedgerAccountDto(int Id, string AccountNumber, string Name, string? Description, AccountType AccountType, int? ParentAccountId, bool IsActive, bool IsSystemAccount);
public record CreateLedgerAccountRequest(string AccountNumber, string Name, string? Description, AccountType AccountType, int? ParentAccountId);
public record UpdateLedgerAccountRequest(string Name, string? Description, bool IsActive, int? ParentAccountId);
