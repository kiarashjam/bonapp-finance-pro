using Microsoft.EntityFrameworkCore;
using FinancePro.Application.DTOs;
using FinancePro.Application.Interfaces;
using FinancePro.Domain.Entities;
using FinancePro.Domain.Enums;
using FinancePro.Infrastructure.Data;

namespace FinancePro.Infrastructure.Services;

public class ExpenseService : IExpenseService
{
    private readonly FinanceProDbContext _db;

    public ExpenseService(FinanceProDbContext db) => _db = db;

    public async Task<PagedResult<ExpenseDto>> GetAllAsync(int orgId, int page, int pageSize, ExpenseFilterDto? filter)
    {
        var query = _db.Expenses
            .Include(e => e.Vendor)
            .Where(e => e.OrganizationId == orgId);

        if (filter?.StartDate != null) query = query.Where(e => e.ExpenseDate >= filter.StartDate);
        if (filter?.EndDate != null) query = query.Where(e => e.ExpenseDate <= filter.EndDate);
        if (filter?.VendorId != null) query = query.Where(e => e.VendorId == filter.VendorId);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(e => e.ExpenseDate)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(e => new ExpenseDto(e.Id, e.ExpenseDate, e.Description, e.Amount, e.VatAmount, e.NetAmount, e.VatRate, e.Category, e.PaymentMethod, e.ReceiptUrl, e.Notes, e.VendorId, e.Vendor != null ? e.Vendor.Name : null, e.CreatedAt))
            .ToListAsync();

        return new PagedResult<ExpenseDto>(items, total, page, pageSize);
    }

    public async Task<ExpenseDto?> GetByIdAsync(int orgId, int id)
    {
        var e = await _db.Expenses.Include(x => x.Vendor)
            .FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == orgId);
        return e == null ? null : new ExpenseDto(e.Id, e.ExpenseDate, e.Description, e.Amount, e.VatAmount, e.NetAmount, e.VatRate, e.Category, e.PaymentMethod, e.ReceiptUrl, e.Notes, e.VendorId, e.Vendor?.Name, e.CreatedAt);
    }

    public async Task<ExpenseDto> CreateAsync(int orgId, string userId, CreateExpenseRequest request)
    {
        var vatRate = await GetVatRateValue(orgId, request.VatRate);
        var netAmount = request.Amount / (1 + vatRate / 100);
        var vatAmount = request.Amount - netAmount;

        var expense = new Expense
        {
            ExpenseDate = request.ExpenseDate,
            Description = request.Description,
            Amount = request.Amount,
            VatAmount = Math.Round(vatAmount, 2),
            NetAmount = Math.Round(netAmount, 2),
            VatRate = request.VatRate,
            Category = request.Category,
            PaymentMethod = request.PaymentMethod,
            Notes = request.Notes,
            VendorId = request.VendorId,
            OrganizationId = orgId,
            CreatedBy = userId
        };

        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();
        return (await GetByIdAsync(orgId, expense.Id))!;
    }

    public async Task<ExpenseDto> UpdateAsync(int orgId, int id, UpdateExpenseRequest request)
    {
        var expense = await _db.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.OrganizationId == orgId)
            ?? throw new InvalidOperationException("Expense not found.");

        var vatRate = await GetVatRateValue(orgId, request.VatRate);
        var netAmount = request.Amount / (1 + vatRate / 100);

        expense.ExpenseDate = request.ExpenseDate;
        expense.Description = request.Description;
        expense.Amount = request.Amount;
        expense.VatAmount = Math.Round(request.Amount - netAmount, 2);
        expense.NetAmount = Math.Round(netAmount, 2);
        expense.VatRate = request.VatRate;
        expense.Category = request.Category;
        expense.PaymentMethod = request.PaymentMethod;
        expense.Notes = request.Notes;
        expense.VendorId = request.VendorId;

        await _db.SaveChangesAsync();
        return (await GetByIdAsync(orgId, id))!;
    }

    public async Task DeleteAsync(int orgId, int id)
    {
        var expense = await _db.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.OrganizationId == orgId)
            ?? throw new InvalidOperationException("Expense not found.");
        expense.IsDeleted = true;
        await _db.SaveChangesAsync();
    }

    private async Task<decimal> GetVatRateValue(int orgId, VatRate vatRate)
    {
        var rate = await _db.TaxRates.FirstOrDefaultAsync(r => r.OrganizationId == orgId && r.RateType == vatRate && r.IsActive);
        return rate?.Rate ?? 0;
    }
}

public class VendorService : IVendorService
{
    private readonly FinanceProDbContext _db;
    public VendorService(FinanceProDbContext db) => _db = db;

    public async Task<List<VendorDto>> GetAllAsync(int orgId)
        => await _db.Vendors.Where(v => v.OrganizationId == orgId).OrderBy(v => v.Name)
            .Select(v => new VendorDto(v.Id, v.Name, v.ContactPerson, v.Email, v.Phone, v.Address, v.Iban, v.PaymentTermDays))
            .ToListAsync();

    public async Task<VendorDto?> GetByIdAsync(int orgId, int id)
    {
        var v = await _db.Vendors.FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == orgId);
        return v == null ? null : new VendorDto(v.Id, v.Name, v.ContactPerson, v.Email, v.Phone, v.Address, v.Iban, v.PaymentTermDays);
    }

    public async Task<VendorDto> CreateAsync(int orgId, CreateVendorRequest request)
    {
        var vendor = new Vendor { Name = request.Name, ContactPerson = request.ContactPerson, Email = request.Email, Phone = request.Phone, Address = request.Address, Iban = request.Iban, PaymentTermDays = request.PaymentTermDays, OrganizationId = orgId };
        _db.Vendors.Add(vendor);
        await _db.SaveChangesAsync();
        return new VendorDto(vendor.Id, vendor.Name, vendor.ContactPerson, vendor.Email, vendor.Phone, vendor.Address, vendor.Iban, vendor.PaymentTermDays);
    }

    public async Task<VendorDto> UpdateAsync(int orgId, int id, CreateVendorRequest request)
    {
        var v = await _db.Vendors.FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == orgId)
            ?? throw new InvalidOperationException("Vendor not found.");
        v.Name = request.Name; v.ContactPerson = request.ContactPerson; v.Email = request.Email;
        v.Phone = request.Phone; v.Address = request.Address; v.Iban = request.Iban; v.PaymentTermDays = request.PaymentTermDays;
        await _db.SaveChangesAsync();
        return new VendorDto(v.Id, v.Name, v.ContactPerson, v.Email, v.Phone, v.Address, v.Iban, v.PaymentTermDays);
    }

    public async Task DeleteAsync(int orgId, int id)
    {
        var v = await _db.Vendors.FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == orgId)
            ?? throw new InvalidOperationException("Vendor not found.");
        v.IsDeleted = true;
        await _db.SaveChangesAsync();
    }
}
