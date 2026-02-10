using FinancePro.Domain.Enums;

namespace FinancePro.Application.DTOs;

public record InvoiceDto(int Id, string InvoiceNumber, int CustomerId, string CustomerName, DateTime InvoiceDate, DateTime DueDate, InvoiceStatus Status, decimal SubTotal, decimal VatTotal, decimal Total, decimal PaidAmount, decimal OutstandingAmount, string? Notes, List<InvoiceLineDto> Lines, DateTime CreatedAt);
public record InvoiceLineDto(int Id, string Description, decimal Quantity, decimal UnitPrice, VatRate VatRate, decimal VatAmount, decimal LineTotal, int SortOrder);
public record CreateInvoiceRequest(int CustomerId, DateTime InvoiceDate, DateTime DueDate, string? Notes, string? PaymentTerms, List<CreateInvoiceLineRequest> Lines);
public record CreateInvoiceLineRequest(string Description, decimal Quantity, decimal UnitPrice, VatRate VatRate, int SortOrder);
public record CustomerDto(int Id, string Name, string? ContactPerson, string? Email, string? Phone, string? Street, string? HouseNumber, string? PostalCode, string? City, string? Country, string? VatId, int PaymentTermDays, decimal? CreditLimit, decimal OutstandingBalance);
public record CreateCustomerRequest(string Name, string? ContactPerson, string? Email, string? Phone, string? Street, string? HouseNumber, string? PostalCode, string? City, string? Country, string? VatId, int PaymentTermDays, decimal? CreditLimit);
