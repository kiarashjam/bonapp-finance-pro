using Microsoft.EntityFrameworkCore;
using FinancePro.Application.DTOs;
using FinancePro.Application.Interfaces;
using FinancePro.Domain.Enums;
using FinancePro.Infrastructure.Data;

namespace FinancePro.Infrastructure.Services;

public class ReportService : IReportService
{
    private readonly FinanceProDbContext _db;
    public ReportService(FinanceProDbContext db) => _db = db;

    public async Task<ProfitAndLossDto> GetProfitAndLossAsync(int orgId, DateTime startDate, DateTime endDate)
    {
        var journalLines = await _db.JournalLines
            .Include(l => l.JournalEntry).Include(l => l.LedgerAccount)
            .Where(l => l.JournalEntry.OrganizationId == orgId
                && l.JournalEntry.Status == JournalEntryStatus.Posted
                && l.JournalEntry.EntryDate >= startDate
                && l.JournalEntry.EntryDate <= endDate)
            .ToListAsync();

        var grouped = journalLines.GroupBy(l => l.LedgerAccount)
            .Select(g => new { Account = g.Key, Balance = g.Sum(l => l.CreditAmount - l.DebitAmount) })
            .ToList();

        // Revenue accounts (3xxx): credit balance is positive revenue
        var revenueItems = grouped.Where(g => g.Account.AccountNumber.StartsWith("3"))
            .Select(g => new PnlLineItemDto(g.Account.AccountNumber, g.Account.Name, g.Balance, 0)).ToList();
        var revenue = revenueItems.Sum(r => r.Amount);

        // COGS (4xxx): debit balance is positive cost
        var cogsItems = grouped.Where(g => g.Account.AccountNumber.StartsWith("4"))
            .Select(g => new PnlLineItemDto(g.Account.AccountNumber, g.Account.Name, -g.Balance, 0)).ToList();
        var cogs = cogsItems.Sum(c => c.Amount);

        // Labor (5xxx)
        var laborItems = grouped.Where(g => g.Account.AccountNumber.StartsWith("5"))
            .Select(g => new PnlLineItemDto(g.Account.AccountNumber, g.Account.Name, -g.Balance, 0)).ToList();
        var labor = laborItems.Sum(l => l.Amount);

        // OpEx (6xxx)
        var opExItems = grouped.Where(g => g.Account.AccountNumber.StartsWith("6"))
            .Select(g => new PnlLineItemDto(g.Account.AccountNumber, g.Account.Name, -g.Balance, 0)).ToList();
        var opEx = opExItems.Sum(o => o.Amount);

        var grossProfit = revenue - cogs;
        var primeCost = cogs + labor;
        var netProfit = revenue - cogs - labor - opEx;

        return new ProfitAndLossDto(
            startDate, endDate, revenue, cogs, grossProfit,
            revenue > 0 ? Math.Round(grossProfit / revenue * 100, 1) : 0,
            labor, primeCost,
            revenue > 0 ? Math.Round(primeCost / revenue * 100, 1) : 0,
            opEx, netProfit,
            revenue > 0 ? Math.Round(netProfit / revenue * 100, 1) : 0,
            revenueItems, cogsItems, laborItems, opExItems);
    }

    public async Task<VatReportDto> GetVatReportAsync(int orgId, DateTime startDate, DateTime endDate)
    {
        var journalLines = await _db.JournalLines
            .Include(l => l.JournalEntry).Include(l => l.LedgerAccount)
            .Where(l => l.JournalEntry.OrganizationId == orgId
                && l.JournalEntry.Status == JournalEntryStatus.Posted
                && l.JournalEntry.EntryDate >= startDate
                && l.JournalEntry.EntryDate <= endDate
                && l.VatRate != null && l.VatAmount != null)
            .ToListAsync();

        var outputVat = journalLines.Where(l => l.LedgerAccount.AccountNumber.StartsWith("3"))
            .GroupBy(l => l.VatRate)
            .Select(g => new { Rate = g.Key, TaxableAmount = g.Sum(l => l.CreditAmount - l.DebitAmount), VatAmount = g.Sum(l => l.VatAmount ?? 0) })
            .ToList();

        var inputVat = journalLines.Where(l => l.LedgerAccount.AccountNumber.StartsWith("4") || l.LedgerAccount.AccountNumber.StartsWith("6"))
            .Sum(l => l.VatAmount ?? 0);

        var totalOutputVat = outputVat.Sum(o => o.VatAmount);
        var totalRevenue = outputVat.Sum(o => o.TaxableAmount);

        var lines = outputVat.Select(o => new VatReportLineDto(
            "Output VAT", o.Rate.ToString() ?? "", GetVatRatePercent(o.Rate), o.TaxableAmount, o.VatAmount)).ToList();

        return new VatReportDto(startDate, endDate, totalRevenue,
            outputVat.FirstOrDefault(o => o.Rate == VatRate.Standard)?.VatAmount ?? 0,
            outputVat.FirstOrDefault(o => o.Rate == VatRate.Reduced)?.VatAmount ?? 0,
            outputVat.FirstOrDefault(o => o.Rate == VatRate.Hospitality)?.VatAmount ?? 0,
            totalOutputVat, inputVat, totalOutputVat - inputVat, lines);
    }

    public async Task<DashboardDto> GetDashboardAsync(int orgId)
    {
        var today = DateTime.UtcNow.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek + 1);
        var monthStart = new DateTime(today.Year, today.Month, 1);

        var dailySummaries = await _db.DailySalesSummaries
            .Where(d => d.OrganizationId == orgId && d.Date >= monthStart.AddMonths(-1))
            .OrderBy(d => d.Date).ToListAsync();

        var todayData = dailySummaries.FirstOrDefault(d => d.Date == today);
        var weekData = dailySummaries.Where(d => d.Date >= weekStart && d.Date <= today);
        var monthData = dailySummaries.Where(d => d.Date >= monthStart && d.Date <= today);

        var pnl = await GetProfitAndLossAsync(orgId, monthStart, today);

        var outstandingAR = await _db.Invoices
            .Where(i => i.OrganizationId == orgId && i.Status != InvoiceStatus.Paid && i.Status != InvoiceStatus.Cancelled)
            .SumAsync(i => i.Total - i.PaidAmount);

        var outstandingAP = await _db.Expenses
            .Where(e => e.OrganizationId == orgId && e.JournalEntryId == null)
            .SumAsync(e => e.Amount);

        var bankBalance = await _db.BankAccounts
            .Where(b => b.OrganizationId == orgId && b.IsActive)
            .SumAsync(b => b.CurrentBalance);

        var revenueChart = dailySummaries
            .Where(d => d.Date >= monthStart.AddMonths(-1))
            .Select(d => new RevenueDataPointDto(d.Date, d.NetSales)).ToList();

        return new DashboardDto(
            todayData?.NetSales ?? 0,
            weekData.Sum(d => d.NetSales),
            monthData.Sum(d => d.NetSales),
            pnl.PrimeCostPercent,
            pnl.Revenue > 0 ? Math.Round(pnl.Cogs / pnl.Revenue * 100, 1) : 0,
            pnl.Revenue > 0 ? Math.Round(pnl.LaborCost / pnl.Revenue * 100, 1) : 0,
            pnl.NetProfitMargin,
            bankBalance,
            outstandingAR,
            outstandingAP,
            0, // Reconciliation rate calculated separately
            0, // VAT liability calculated separately
            revenueChart);
    }

    private static decimal GetVatRatePercent(VatRate? rate) => rate switch
    {
        VatRate.Standard => 8.1m,
        VatRate.Reduced => 2.6m,
        VatRate.Hospitality => 3.8m,
        _ => 0m
    };
}
