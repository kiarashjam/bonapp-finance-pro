using Microsoft.EntityFrameworkCore;
using FinancePro.Application.DTOs;
using FinancePro.Application.Interfaces;
using FinancePro.Domain.Entities;
using FinancePro.Domain.Enums;
using FinancePro.Infrastructure.Data;

namespace FinancePro.Infrastructure.Services;

public class JournalService : IJournalService
{
    private readonly FinanceProDbContext _db;

    public JournalService(FinanceProDbContext db) => _db = db;

    public async Task<PagedResult<JournalEntryDto>> GetAllAsync(int orgId, int page, int pageSize)
    {
        var query = _db.JournalEntries
            .Include(j => j.Lines).ThenInclude(l => l.LedgerAccount)
            .Where(j => j.OrganizationId == orgId)
            .OrderByDescending(j => j.EntryDate);

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<JournalEntryDto>(items.Select(MapToDto).ToList(), total, page, pageSize);
    }

    public async Task<JournalEntryDto?> GetByIdAsync(int orgId, int id)
    {
        var j = await _db.JournalEntries
            .Include(j => j.Lines).ThenInclude(l => l.LedgerAccount)
            .FirstOrDefaultAsync(j => j.Id == id && j.OrganizationId == orgId);
        return j == null ? null : MapToDto(j);
    }

    public async Task<JournalEntryDto> CreateAsync(int orgId, string userId, CreateJournalEntryRequest request)
    {
        var totalDebit = request.Lines.Sum(l => l.DebitAmount);
        var totalCredit = request.Lines.Sum(l => l.CreditAmount);
        if (totalDebit != totalCredit)
            throw new InvalidOperationException($"Journal entry must balance. Debit: {totalDebit}, Credit: {totalCredit}");

        var refNumber = await GenerateReferenceNumber(orgId);

        var entry = new JournalEntry
        {
            ReferenceNumber = refNumber,
            EntryDate = request.EntryDate,
            Description = request.Description,
            Source = JournalEntrySource.Manual,
            OrganizationId = orgId,
            CreatedBy = userId,
            Lines = request.Lines.Select(l => new JournalLine
            {
                LedgerAccountId = l.LedgerAccountId,
                DebitAmount = l.DebitAmount,
                CreditAmount = l.CreditAmount,
                Description = l.Description,
                VatRate = l.VatRate,
                VatAmount = l.VatAmount
            }).ToList()
        };

        _db.JournalEntries.Add(entry);
        await _db.SaveChangesAsync();

        return await GetByIdAsync(orgId, entry.Id) ?? throw new InvalidOperationException("Failed to create journal entry.");
    }

    public async Task<JournalEntryDto> PostAsync(int orgId, int id, string userId)
    {
        var entry = await _db.JournalEntries.Include(j => j.Lines)
            .FirstOrDefaultAsync(j => j.Id == id && j.OrganizationId == orgId)
            ?? throw new InvalidOperationException("Journal entry not found.");

        if (entry.Status != JournalEntryStatus.Draft)
            throw new InvalidOperationException("Only draft entries can be posted.");

        entry.Status = JournalEntryStatus.Posted;
        entry.PostedAt = DateTime.UtcNow;
        entry.PostedBy = userId;
        await _db.SaveChangesAsync();

        return await GetByIdAsync(orgId, id) ?? throw new InvalidOperationException();
    }

    public async Task<JournalEntryDto> VoidAsync(int orgId, int id, string userId)
    {
        var entry = await _db.JournalEntries
            .FirstOrDefaultAsync(j => j.Id == id && j.OrganizationId == orgId)
            ?? throw new InvalidOperationException("Journal entry not found.");

        if (entry.Status != JournalEntryStatus.Posted)
            throw new InvalidOperationException("Only posted entries can be voided.");

        entry.Status = JournalEntryStatus.Voided;
        entry.UpdatedBy = userId;
        await _db.SaveChangesAsync();

        return await GetByIdAsync(orgId, id) ?? throw new InvalidOperationException();
    }

    public async Task<TrialBalanceDto> GetTrialBalanceAsync(int orgId, DateTime asOfDate)
    {
        var accounts = await _db.LedgerAccounts
            .Where(a => a.OrganizationId == orgId && a.IsActive)
            .Include(a => a.JournalLines)
                .ThenInclude(l => l.JournalEntry)
            .ToListAsync();

        var lines = accounts.Select(a =>
        {
            var postedLines = a.JournalLines
                .Where(l => l.JournalEntry.Status == JournalEntryStatus.Posted && l.JournalEntry.EntryDate <= asOfDate);
            var debit = postedLines.Sum(l => l.DebitAmount);
            var credit = postedLines.Sum(l => l.CreditAmount);
            return new TrialBalanceLineDto(a.AccountNumber, a.Name, a.AccountType.ToString(), debit, credit);
        })
        .Where(l => l.DebitBalance != 0 || l.CreditBalance != 0)
        .OrderBy(l => l.AccountNumber)
        .ToList();

        return new TrialBalanceDto(asOfDate, lines, lines.Sum(l => l.DebitBalance), lines.Sum(l => l.CreditBalance));
    }

    private async Task<string> GenerateReferenceNumber(int orgId)
    {
        var count = await _db.JournalEntries.CountAsync(j => j.OrganizationId == orgId);
        return $"JE-{DateTime.UtcNow.Year}-{(count + 1):D5}";
    }

    private static JournalEntryDto MapToDto(JournalEntry j) => new(
        j.Id, j.ReferenceNumber, j.EntryDate, j.Description, j.Status, j.Source,
        j.Lines.Sum(l => l.DebitAmount), j.Lines.Sum(l => l.CreditAmount),
        j.Lines.Select(l => new JournalLineDto(l.Id, l.LedgerAccountId, l.LedgerAccount.AccountNumber, l.LedgerAccount.Name, l.DebitAmount, l.CreditAmount, l.Description, l.VatRate, l.VatAmount)).ToList(),
        j.CreatedAt);
}
