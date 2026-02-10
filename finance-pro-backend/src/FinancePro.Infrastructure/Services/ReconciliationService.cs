using System.Globalization;
using Microsoft.EntityFrameworkCore;
using FinancePro.Application.DTOs;
using FinancePro.Application.Interfaces;
using FinancePro.Domain.Entities;
using FinancePro.Domain.Enums;
using FinancePro.Infrastructure.Data;

namespace FinancePro.Infrastructure.Services;

public class ReconciliationService : IReconciliationService
{
    private readonly FinanceProDbContext _db;
    public ReconciliationService(FinanceProDbContext db) => _db = db;

    public async Task<ReconciliationDashboardDto> GetDashboardAsync(int orgId)
    {
        var payouts = await _db.Payouts.Include(p => p.PaymentSource)
            .Where(p => p.OrganizationId == orgId).ToListAsync();

        var totalExpected = payouts.Sum(p => p.ExpectedAmount);
        var totalReceived = payouts.Where(p => p.Status == PayoutStatus.Received).Sum(p => p.ActualAmount ?? 0);
        var totalMissing = payouts.Where(p => p.Status == PayoutStatus.Missing || p.Status == PayoutStatus.Pending).Sum(p => p.ExpectedAmount);
        var totalDisputed = payouts.Where(p => p.Status == PayoutStatus.Disputed).Sum(p => p.ExpectedAmount);

        var bankAccountIds = await _db.BankAccounts.Where(b => b.OrganizationId == orgId).Select(b => b.Id).ToListAsync();
        var unmatchedCount = await _db.BankTransactions.CountAsync(t => bankAccountIds.Contains(t.BankAccountId) && !t.IsMatched);

        var reconRate = totalExpected > 0 ? (totalReceived / totalExpected) * 100 : 0;

        var recentPayouts = payouts.OrderByDescending(p => p.ExpectedDate).Take(10)
            .Select(p => new PayoutDto(p.Id, p.PaymentSourceId, p.PaymentSource.Name, p.ExpectedAmount, p.ActualAmount, p.Fees, p.ExpectedDate, p.ActualDate, p.Status, p.ProviderReference)).ToList();

        var unmatched = await _db.BankTransactions
            .Where(t => bankAccountIds.Contains(t.BankAccountId) && !t.IsMatched)
            .OrderByDescending(t => t.TransactionDate).Take(20)
            .Select(t => new BankTransactionDto(t.Id, t.BankAccountId, t.TransactionDate, t.ValueDate, t.Amount, t.Description, t.Reference, t.IsMatched))
            .ToListAsync();

        return new ReconciliationDashboardDto(totalExpected, totalReceived, totalMissing, totalDisputed, unmatchedCount, Math.Round(reconRate, 1), recentPayouts, unmatched);
    }

    public async Task<List<BankAccountDto>> GetBankAccountsAsync(int orgId)
        => await _db.BankAccounts.Where(b => b.OrganizationId == orgId).OrderBy(b => b.Name)
            .Select(b => new BankAccountDto(b.Id, b.Name, b.Iban, b.BankName, b.Currency, b.CurrentBalance, b.IsActive))
            .ToListAsync();

    public async Task<BankAccountDto> CreateBankAccountAsync(int orgId, CreateBankAccountRequest request)
    {
        var bank = new BankAccount { Name = request.Name, Iban = request.Iban, BankName = request.BankName, Currency = request.Currency, OrganizationId = orgId };
        _db.BankAccounts.Add(bank);
        await _db.SaveChangesAsync();
        return new BankAccountDto(bank.Id, bank.Name, bank.Iban, bank.BankName, bank.Currency, bank.CurrentBalance, bank.IsActive);
    }

    public async Task<int> ImportBankStatementAsync(int orgId, int bankAccountId, Stream csvStream)
    {
        var bank = await _db.BankAccounts.FirstOrDefaultAsync(b => b.Id == bankAccountId && b.OrganizationId == orgId)
            ?? throw new InvalidOperationException("Bank account not found.");

        var batchId = Guid.NewGuid().ToString("N")[..12];
        var count = 0;

        using var reader = new StreamReader(csvStream);
        var header = await reader.ReadLineAsync(); // Skip header
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(';');
            if (parts.Length < 4) continue;

            var transaction = new BankTransaction
            {
                BankAccountId = bankAccountId,
                TransactionDate = DateTime.TryParse(parts[0].Trim('"'), CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt) ? dt : DateTime.UtcNow,
                Description = parts[1].Trim('"'),
                Amount = decimal.TryParse(parts[2].Trim('"').Replace("'", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out var amt) ? amt : 0,
                Reference = parts.Length > 3 ? parts[3].Trim('"') : null,
                ImportBatchId = batchId
            };

            _db.BankTransactions.Add(transaction);
            count++;
        }

        await _db.SaveChangesAsync();
        return count;
    }

    public async Task<List<BankTransactionDto>> GetTransactionsAsync(int orgId, int bankAccountId, int page, int pageSize)
    {
        var bank = await _db.BankAccounts.FirstOrDefaultAsync(b => b.Id == bankAccountId && b.OrganizationId == orgId)
            ?? throw new InvalidOperationException("Bank account not found.");

        return await _db.BankTransactions
            .Where(t => t.BankAccountId == bankAccountId)
            .OrderByDescending(t => t.TransactionDate)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(t => new BankTransactionDto(t.Id, t.BankAccountId, t.TransactionDate, t.ValueDate, t.Amount, t.Description, t.Reference, t.IsMatched))
            .ToListAsync();
    }

    public async Task<List<PayoutDto>> GetPayoutsAsync(int orgId, int page, int pageSize, string? status)
    {
        var query = _db.Payouts.Include(p => p.PaymentSource).Where(p => p.OrganizationId == orgId);
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<PayoutStatus>(status, true, out var s))
            query = query.Where(p => p.Status == s);

        return await query.OrderByDescending(p => p.ExpectedDate)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(p => new PayoutDto(p.Id, p.PaymentSourceId, p.PaymentSource.Name, p.ExpectedAmount, p.ActualAmount, p.Fees, p.ExpectedDate, p.ActualDate, p.Status, p.ProviderReference))
            .ToListAsync();
    }

    public async Task<PayoutDto> CreatePayoutAsync(int orgId, CreatePayoutRequest request)
    {
        var payout = new Payout { PaymentSourceId = request.PaymentSourceId, ExpectedAmount = request.ExpectedAmount, ExpectedDate = request.ExpectedDate, ProviderReference = request.ProviderReference, OrganizationId = orgId };
        _db.Payouts.Add(payout);
        await _db.SaveChangesAsync();
        var source = await _db.PaymentSources.FindAsync(request.PaymentSourceId);
        return new PayoutDto(payout.Id, payout.PaymentSourceId, source?.Name ?? "", payout.ExpectedAmount, null, null, payout.ExpectedDate, null, payout.Status, payout.ProviderReference);
    }
}
