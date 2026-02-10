using FinancePro.Domain.Enums;

namespace FinancePro.Application.DTOs;

public record ExpenseDto(int Id, DateTime ExpenseDate, string Description, decimal Amount, decimal VatAmount, decimal NetAmount, VatRate VatRate, ExpenseCategory Category, PaymentMethod PaymentMethod, string? ReceiptUrl, string? Notes, int? VendorId, string? VendorName, DateTime CreatedAt);
public record CreateExpenseRequest(DateTime ExpenseDate, string Description, decimal Amount, VatRate VatRate, ExpenseCategory Category, PaymentMethod PaymentMethod, string? Notes, int? VendorId);
public record UpdateExpenseRequest(DateTime ExpenseDate, string Description, decimal Amount, VatRate VatRate, ExpenseCategory Category, PaymentMethod PaymentMethod, string? Notes, int? VendorId);
public record VendorDto(int Id, string Name, string? ContactPerson, string? Email, string? Phone, string? Address, string? Iban, int PaymentTermDays);
public record CreateVendorRequest(string Name, string? ContactPerson, string? Email, string? Phone, string? Address, string? Iban, int PaymentTermDays);
