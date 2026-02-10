using Microsoft.EntityFrameworkCore;
using FinancePro.Application.DTOs;
using FinancePro.Application.Interfaces;
using FinancePro.Domain.Entities;
using FinancePro.Infrastructure.Data;

namespace FinancePro.Infrastructure.Services;

public class AccountService : IAccountService
{
    private readonly FinanceProDbContext _db;

    public AccountService(FinanceProDbContext db) => _db = db;

    public async Task<List<LedgerAccountDto>> GetAllAsync(int orgId)
    {
        return await _db.LedgerAccounts
            .Where(a => a.OrganizationId == orgId)
            .OrderBy(a => a.AccountNumber)
            .Select(a => new LedgerAccountDto(a.Id, a.AccountNumber, a.Name, a.Description, a.AccountType, a.ParentAccountId, a.IsActive, a.IsSystemAccount))
            .ToListAsync();
    }

    public async Task<LedgerAccountDto?> GetByIdAsync(int orgId, int id)
    {
        var a = await _db.LedgerAccounts.FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == orgId);
        return a == null ? null : new LedgerAccountDto(a.Id, a.AccountNumber, a.Name, a.Description, a.AccountType, a.ParentAccountId, a.IsActive, a.IsSystemAccount);
    }

    public async Task<LedgerAccountDto> CreateAsync(int orgId, CreateLedgerAccountRequest request)
    {
        var exists = await _db.LedgerAccounts.AnyAsync(a => a.OrganizationId == orgId && a.AccountNumber == request.AccountNumber);
        if (exists) throw new InvalidOperationException($"Account number {request.AccountNumber} already exists.");

        var account = new LedgerAccount
        {
            AccountNumber = request.AccountNumber,
            Name = request.Name,
            Description = request.Description,
            AccountType = request.AccountType,
            ParentAccountId = request.ParentAccountId,
            OrganizationId = orgId
        };
        _db.LedgerAccounts.Add(account);
        await _db.SaveChangesAsync();
        return new LedgerAccountDto(account.Id, account.AccountNumber, account.Name, account.Description, account.AccountType, account.ParentAccountId, account.IsActive, account.IsSystemAccount);
    }

    public async Task<LedgerAccountDto> UpdateAsync(int orgId, int id, UpdateLedgerAccountRequest request)
    {
        var account = await _db.LedgerAccounts.FirstOrDefaultAsync(a => a.Id == id && a.OrganizationId == orgId)
            ?? throw new InvalidOperationException("Account not found.");

        if (account.IsSystemAccount)
            throw new InvalidOperationException("System accounts cannot be modified.");

        account.Name = request.Name;
        account.Description = request.Description;
        account.IsActive = request.IsActive;
        account.ParentAccountId = request.ParentAccountId;
        await _db.SaveChangesAsync();
        return new LedgerAccountDto(account.Id, account.AccountNumber, account.Name, account.Description, account.AccountType, account.ParentAccountId, account.IsActive, account.IsSystemAccount);
    }

    public async Task DeleteAsync(int orgId, int id)
    {
        var account = await _db.LedgerAccounts.FirstOrDefaultAsync(a => a.Id == id && a.OrganizationId == orgId)
            ?? throw new InvalidOperationException("Account not found.");
        if (account.IsSystemAccount)
            throw new InvalidOperationException("System accounts cannot be deleted.");

        var hasJournalLines = await _db.JournalLines.AnyAsync(l => l.LedgerAccountId == id);
        if (hasJournalLines)
            throw new InvalidOperationException("Cannot delete account with existing journal entries.");

        account.IsDeleted = true;
        await _db.SaveChangesAsync();
    }

    public async Task SeedChartOfAccountsAsync(int orgId)
    {
        var existing = await _db.LedgerAccounts.AnyAsync(a => a.OrganizationId == orgId);
        if (existing) return;

        var accounts = SeedData.GetSwissChartOfAccounts(orgId);
        _db.LedgerAccounts.AddRange(accounts);
        await _db.SaveChangesAsync();
    }
}
