using Microsoft.EntityFrameworkCore;
using FinancePro.Application.DTOs;
using FinancePro.Application.Interfaces;
using FinancePro.Domain.Entities;
using FinancePro.Domain.Enums;
using FinancePro.Infrastructure.Data;

namespace FinancePro.Infrastructure.Services;

public class SalesService : ISalesService
{
    private readonly FinanceProDbContext _db;
    private readonly IJournalService _journalService;

    public SalesService(FinanceProDbContext db, IJournalService journalService)
    {
        _db = db;
        _journalService = journalService;
    }

    public async Task<List<DailySalesSummaryDto>> GetDailySummariesAsync(int orgId, DateTime startDate, DateTime endDate)
    {
        return await _db.DailySalesSummaries
            .Where(d => d.OrganizationId == orgId && d.Date >= startDate && d.Date <= endDate)
            .OrderByDescending(d => d.Date)
            .Select(d => new DailySalesSummaryDto(d.Id, d.Date, d.GrossSales, d.NetSales, d.TotalRefunds, d.TotalTips, d.CashSales, d.CardSales, d.OnlineSales, d.TransactionCount, d.GuestCount, d.DayStatus, d.ClosedAt, d.TransactionCount > 0 ? d.NetSales / d.TransactionCount : 0))
            .ToListAsync();
    }

    public async Task<DailySalesSummaryDto> CreateManualEntryAsync(int orgId, string userId, ManualSalesEntryRequest request)
    {
        var exists = await _db.DailySalesSummaries.AnyAsync(d => d.OrganizationId == orgId && d.Date == request.Date.Date);
        if (exists) throw new InvalidOperationException($"Sales summary already exists for {request.Date:yyyy-MM-dd}.");

        var summary = new DailySalesSummary
        {
            Date = request.Date.Date,
            GrossSales = request.GrossSales,
            NetSales = request.GrossSales, // Will be adjusted after refunds
            CashSales = request.CashSales,
            CardSales = request.CardSales,
            OnlineSales = request.OnlineSales,
            OtherSales = request.OtherSales,
            TotalTips = request.TotalTips,
            VatStandard = request.VatStandard,
            VatReduced = request.VatReduced,
            VatHospitality = request.VatHospitality,
            TransactionCount = request.TransactionCount,
            GuestCount = request.GuestCount,
            OrganizationId = orgId,
            CreatedBy = userId
        };

        _db.DailySalesSummaries.Add(summary);
        await _db.SaveChangesAsync();

        return new DailySalesSummaryDto(summary.Id, summary.Date, summary.GrossSales, summary.NetSales, summary.TotalRefunds, summary.TotalTips, summary.CashSales, summary.CardSales, summary.OnlineSales, summary.TransactionCount, summary.GuestCount, summary.DayStatus, summary.ClosedAt, summary.AverageCheck);
    }

    public async Task<CloseDayPreviewDto> GetCloseDayPreviewAsync(int orgId, DateTime date)
    {
        var summary = await _db.DailySalesSummaries
            .FirstOrDefaultAsync(d => d.OrganizationId == orgId && d.Date == date.Date)
            ?? throw new InvalidOperationException($"No sales data found for {date:yyyy-MM-dd}. Enter manual sales or wait for POS data.");

        if (summary.DayStatus == DayStatus.Closed)
            throw new InvalidOperationException($"Day {date:yyyy-MM-dd} is already closed.");

        // Build proposed journal lines
        var accounts = await _db.LedgerAccounts.Where(a => a.OrganizationId == orgId).ToListAsync();
        var cashAccount = accounts.FirstOrDefault(a => a.AccountNumber == "1000");
        var bankAccount = accounts.FirstOrDefault(a => a.AccountNumber == "1020");
        var revenueAccount = accounts.FirstOrDefault(a => a.AccountNumber == "3000");
        var vatPayable = accounts.FirstOrDefault(a => a.AccountNumber == "2200");

        var lines = new List<JournalLineDto>();
        if (cashAccount != null && summary.CashSales > 0)
            lines.Add(new JournalLineDto(0, cashAccount.Id, "1000", "Cash", summary.CashSales, 0, "Cash sales", null, null));
        if (bankAccount != null && (summary.CardSales + summary.OnlineSales) > 0)
            lines.Add(new JournalLineDto(0, bankAccount.Id, "1020", "Bank", summary.CardSales + summary.OnlineSales, 0, "Card + online sales", null, null));
        if (revenueAccount != null)
        {
            var totalVat = summary.VatStandard + summary.VatReduced + summary.VatHospitality;
            var netRevenue = summary.GrossSales - totalVat;
            lines.Add(new JournalLineDto(0, revenueAccount.Id, "3000", "Revenue", 0, netRevenue, "Daily revenue", null, null));
            if (vatPayable != null && totalVat > 0)
                lines.Add(new JournalLineDto(0, vatPayable.Id, "2200", "VAT payable", 0, totalVat, "Daily VAT", null, null));
        }

        return new CloseDayPreviewDto(date.Date, summary.GrossSales, summary.NetSales, summary.TotalRefunds, summary.TotalTips, summary.TransactionCount, summary.CashSales, summary.CardSales, lines);
    }

    public async Task<DailySalesSummaryDto> CloseDayAsync(int orgId, string userId, CloseDayRequest request)
    {
        if (!request.Confirmed)
            throw new InvalidOperationException("Please confirm the close-day action.");

        var summary = await _db.DailySalesSummaries
            .FirstOrDefaultAsync(d => d.OrganizationId == orgId && d.Date == request.Date.Date)
            ?? throw new InvalidOperationException($"No sales data found for {request.Date:yyyy-MM-dd}.");

        if (summary.DayStatus == DayStatus.Closed)
            throw new InvalidOperationException($"Day {request.Date:yyyy-MM-dd} is already closed.");

        // Generate journal entry
        var preview = await GetCloseDayPreviewAsync(orgId, request.Date);
        var journalLines = preview.ProposedJournalLines.Select(l => new CreateJournalLineRequest(l.LedgerAccountId, l.DebitAmount, l.CreditAmount, l.Description, l.VatRate, l.VatAmount)).ToList();

        if (journalLines.Any())
        {
            var journalEntry = await _journalService.CreateAsync(orgId, userId, new CreateJournalEntryRequest(request.Date.Date, $"Close day {request.Date:yyyy-MM-dd}", journalLines));
            await _journalService.PostAsync(orgId, journalEntry.Id, userId);
            summary.JournalEntryId = journalEntry.Id;
        }

        summary.DayStatus = DayStatus.Closed;
        summary.ClosedAt = DateTime.UtcNow;
        summary.ClosedBy = userId;
        await _db.SaveChangesAsync();

        return new DailySalesSummaryDto(summary.Id, summary.Date, summary.GrossSales, summary.NetSales, summary.TotalRefunds, summary.TotalTips, summary.CashSales, summary.CardSales, summary.OnlineSales, summary.TransactionCount, summary.GuestCount, summary.DayStatus, summary.ClosedAt, summary.AverageCheck);
    }

    public async Task IngestPaymentAsync(int orgId, int sourceId, CreatePaymentRequest request)
    {
        var source = await _db.PaymentSources.FirstOrDefaultAsync(s => s.Id == sourceId && s.OrganizationId == orgId)
            ?? throw new InvalidOperationException("Payment source not found.");

        var record = new PaymentRecord
        {
            ExternalId = request.ExternalId,
            OrderReference = request.OrderReference,
            Amount = request.Amount,
            NetAmount = request.Amount - request.TipAmount,
            TipAmount = request.TipAmount,
            VatAmount = request.VatAmount,
            Method = request.Method,
            EventType = request.EventType,
            VatRate = request.VatRate,
            TransactionDate = request.TransactionDate,
            Description = request.Description,
            PaymentSourceId = sourceId,
            OrganizationId = orgId
        };

        _db.PaymentRecords.Add(record);

        // Update or create daily summary
        var date = request.TransactionDate.Date;
        var summary = await _db.DailySalesSummaries.FirstOrDefaultAsync(d => d.OrganizationId == orgId && d.Date == date);
        if (summary == null)
        {
            summary = new DailySalesSummary { Date = date, OrganizationId = orgId };
            _db.DailySalesSummaries.Add(summary);
        }

        if (request.EventType == PaymentEventType.Capture)
        {
            summary.GrossSales += request.Amount;
            summary.NetSales += request.Amount;
            summary.TotalTips += request.TipAmount;
            summary.TransactionCount++;
            switch (request.Method)
            {
                case PaymentMethod.Cash: summary.CashSales += request.Amount; break;
                case PaymentMethod.Card: summary.CardSales += request.Amount; break;
                case PaymentMethod.Online: summary.OnlineSales += request.Amount; break;
                default: summary.OtherSales += request.Amount; break;
            }
        }
        else if (request.EventType == PaymentEventType.Refund)
        {
            summary.TotalRefunds += request.Amount;
            summary.NetSales -= request.Amount;
        }

        await _db.SaveChangesAsync();
    }
}
