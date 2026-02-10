using Microsoft.EntityFrameworkCore;
using FinancePro.Application.DTOs;
using FinancePro.Application.Interfaces;
using FinancePro.Domain.Entities;
using FinancePro.Domain.Enums;
using FinancePro.Infrastructure.Data;

namespace FinancePro.Infrastructure.Services;

public class InvoiceService : IInvoiceService
{
    private readonly FinanceProDbContext _db;

    public InvoiceService(FinanceProDbContext db) => _db = db;

    public async Task<PagedResult<InvoiceDto>> GetAllAsync(int orgId, int page, int pageSize, string? status)
    {
        var query = _db.Invoices
            .Include(i => i.Lines).Include(i => i.Customer)
            .Where(i => i.OrganizationId == orgId);

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<InvoiceStatus>(status, true, out var s))
            query = query.Where(i => i.Status == s);

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(i => i.InvoiceDate)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<InvoiceDto>(items.Select(MapToDto).ToList(), total, page, pageSize);
    }

    public async Task<InvoiceDto?> GetByIdAsync(int orgId, int id)
    {
        var i = await _db.Invoices.Include(x => x.Lines).Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == orgId);
        return i == null ? null : MapToDto(i);
    }

    public async Task<InvoiceDto> CreateAsync(int orgId, string userId, CreateInvoiceRequest request)
    {
        var count = await _db.Invoices.CountAsync(i => i.OrganizationId == orgId && i.InvoiceDate.Year == request.InvoiceDate.Year);
        var invoiceNumber = $"INV-{request.InvoiceDate.Year}-{(count + 1):D4}";

        var invoice = new Invoice
        {
            InvoiceNumber = invoiceNumber,
            CustomerId = request.CustomerId,
            InvoiceDate = request.InvoiceDate,
            DueDate = request.DueDate,
            Notes = request.Notes,
            PaymentTerms = request.PaymentTerms,
            OrganizationId = orgId,
            CreatedBy = userId,
            Lines = request.Lines.Select(l =>
            {
                var lineTotal = l.Quantity * l.UnitPrice;
                var vatRateValue = GetVatPercent(l.VatRate);
                var vatAmount = Math.Round(lineTotal * vatRateValue / 100, 2);
                return new InvoiceLine
                {
                    Description = l.Description,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice,
                    VatRate = l.VatRate,
                    VatAmount = vatAmount,
                    LineTotal = lineTotal + vatAmount,
                    SortOrder = l.SortOrder
                };
            }).ToList()
        };

        invoice.SubTotal = invoice.Lines.Sum(l => l.Quantity * l.UnitPrice);
        invoice.VatTotal = invoice.Lines.Sum(l => l.VatAmount);
        invoice.Total = invoice.SubTotal + invoice.VatTotal;

        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();
        return (await GetByIdAsync(orgId, invoice.Id))!;
    }

    public async Task<InvoiceDto> UpdateStatusAsync(int orgId, int id, string newStatus)
    {
        var invoice = await _db.Invoices.FirstOrDefaultAsync(i => i.Id == id && i.OrganizationId == orgId)
            ?? throw new InvalidOperationException("Invoice not found.");

        if (!Enum.TryParse<InvoiceStatus>(newStatus, true, out var status))
            throw new InvalidOperationException($"Invalid status: {newStatus}");

        invoice.Status = status;
        await _db.SaveChangesAsync();
        return (await GetByIdAsync(orgId, id))!;
    }

    public Task<byte[]> GeneratePdfAsync(int orgId, int id)
    {
        // PDF generation placeholder — in production, use QuestPDF or similar
        return Task.FromResult(Array.Empty<byte>());
    }

    public async Task DeleteAsync(int orgId, int id)
    {
        var invoice = await _db.Invoices.FirstOrDefaultAsync(i => i.Id == id && i.OrganizationId == orgId)
            ?? throw new InvalidOperationException("Invoice not found.");
        if (invoice.Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Only draft invoices can be deleted.");
        invoice.IsDeleted = true;
        await _db.SaveChangesAsync();
    }

    private static decimal GetVatPercent(VatRate rate) => rate switch
    {
        VatRate.Standard => 8.1m,
        VatRate.Reduced => 2.6m,
        VatRate.Hospitality => 3.8m,
        _ => 0m
    };

    private static InvoiceDto MapToDto(Invoice i) => new(
        i.Id, i.InvoiceNumber, i.CustomerId, i.Customer?.Name ?? "", i.InvoiceDate, i.DueDate,
        i.Status, i.SubTotal, i.VatTotal, i.Total, i.PaidAmount, i.OutstandingAmount, i.Notes,
        i.Lines.OrderBy(l => l.SortOrder).Select(l => new InvoiceLineDto(l.Id, l.Description, l.Quantity, l.UnitPrice, l.VatRate, l.VatAmount, l.LineTotal, l.SortOrder)).ToList(),
        i.CreatedAt);
}

public class CustomerService : ICustomerService
{
    private readonly FinanceProDbContext _db;
    public CustomerService(FinanceProDbContext db) => _db = db;

    public async Task<List<CustomerDto>> GetAllAsync(int orgId)
        => await _db.Customers.Where(c => c.OrganizationId == orgId).OrderBy(c => c.Name)
            .Select(c => new CustomerDto(c.Id, c.Name, c.ContactPerson, c.Email, c.Phone, c.Street, c.HouseNumber, c.PostalCode, c.City, c.Country, c.VatId, c.PaymentTermDays, c.CreditLimit, c.Invoices.Where(i => i.Status != InvoiceStatus.Paid && i.Status != InvoiceStatus.Cancelled).Sum(i => i.Total - i.PaidAmount)))
            .ToListAsync();

    public async Task<CustomerDto?> GetByIdAsync(int orgId, int id)
    {
        var c = await _db.Customers.Include(x => x.Invoices).FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == orgId);
        if (c == null) return null;
        var outstanding = c.Invoices.Where(i => i.Status != InvoiceStatus.Paid && i.Status != InvoiceStatus.Cancelled).Sum(i => i.Total - i.PaidAmount);
        return new CustomerDto(c.Id, c.Name, c.ContactPerson, c.Email, c.Phone, c.Street, c.HouseNumber, c.PostalCode, c.City, c.Country, c.VatId, c.PaymentTermDays, c.CreditLimit, outstanding);
    }

    public async Task<CustomerDto> CreateAsync(int orgId, CreateCustomerRequest request)
    {
        var customer = new Customer { Name = request.Name, ContactPerson = request.ContactPerson, Email = request.Email, Phone = request.Phone, Street = request.Street, HouseNumber = request.HouseNumber, PostalCode = request.PostalCode, City = request.City, Country = request.Country, VatId = request.VatId, PaymentTermDays = request.PaymentTermDays, CreditLimit = request.CreditLimit, OrganizationId = orgId };
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();
        return new CustomerDto(customer.Id, customer.Name, customer.ContactPerson, customer.Email, customer.Phone, customer.Street, customer.HouseNumber, customer.PostalCode, customer.City, customer.Country, customer.VatId, customer.PaymentTermDays, customer.CreditLimit, 0);
    }

    public async Task<CustomerDto> UpdateAsync(int orgId, int id, CreateCustomerRequest r)
    {
        var c = await _db.Customers.FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == orgId) ?? throw new InvalidOperationException("Customer not found.");
        c.Name = r.Name; c.ContactPerson = r.ContactPerson; c.Email = r.Email; c.Phone = r.Phone;
        c.Street = r.Street; c.HouseNumber = r.HouseNumber; c.PostalCode = r.PostalCode; c.City = r.City;
        c.Country = r.Country; c.VatId = r.VatId; c.PaymentTermDays = r.PaymentTermDays; c.CreditLimit = r.CreditLimit;
        await _db.SaveChangesAsync();
        return (await GetByIdAsync(orgId, id))!;
    }

    public async Task DeleteAsync(int orgId, int id)
    {
        var c = await _db.Customers.FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == orgId) ?? throw new InvalidOperationException("Customer not found.");
        c.IsDeleted = true;
        await _db.SaveChangesAsync();
    }
}
