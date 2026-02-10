using Microsoft.EntityFrameworkCore;
using FinancePro.Application.DTOs;
using FinancePro.Application.Interfaces;
using FinancePro.Domain.Entities;
using FinancePro.Domain.Enums;
using FinancePro.Infrastructure.Data;

namespace FinancePro.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly FinanceProDbContext _db;
    public AuditService(FinanceProDbContext db) => _db = db;

    public async Task LogAsync(int orgId, string entityType, int entityId, string action, string? userId, string? oldValues, string? newValues, string? ipAddress)
    {
        var log = new AuditLog
        {
            OrganizationId = orgId,
            EntityType = entityType,
            EntityId = entityId,
            Action = Enum.TryParse<AuditAction>(action, out var a) ? a : AuditAction.Create,
            UserId = userId,
            OldValues = oldValues,
            NewValues = newValues,
            IpAddress = ipAddress
        };
        _db.AuditLogs.Add(log);
        await _db.SaveChangesAsync();
    }

    public async Task<PagedResult<AuditLogDto>> GetLogsAsync(int orgId, int page, int pageSize, string? entityType)
    {
        var query = _db.AuditLogs.Where(a => a.OrganizationId == orgId);
        if (!string.IsNullOrEmpty(entityType))
            query = query.Where(a => a.EntityType == entityType);

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(a => a.Timestamp)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(a => new AuditLogDto(a.Id, a.EntityType, a.EntityId, a.Action.ToString(), a.UserName, a.Timestamp))
            .ToListAsync();

        return new PagedResult<AuditLogDto>(items, total, page, pageSize);
    }
}

public class OrganizationService : IOrganizationService
{
    private readonly FinanceProDbContext _db;
    public OrganizationService(FinanceProDbContext db) => _db = db;

    public async Task<OrganizationSettingsDto> GetSettingsAsync(int orgId)
    {
        var o = await _db.Organizations.FindAsync(orgId) ?? throw new InvalidOperationException("Organization not found.");
        return new OrganizationSettingsDto(o.Id, o.Name, o.LegalName, o.Street, o.HouseNumber, o.PostalCode, o.City, o.Country, o.Phone, o.Email, o.Website, o.VatId, o.Iban, o.QrIban, o.LogoUrl, o.Currency, o.FiscalYearStartMonth, o.DefaultLanguage);
    }

    public async Task<OrganizationSettingsDto> UpdateSettingsAsync(int orgId, UpdateOrganizationRequest r)
    {
        var o = await _db.Organizations.FindAsync(orgId) ?? throw new InvalidOperationException("Organization not found.");
        o.Name = r.Name; o.LegalName = r.LegalName; o.Street = r.Street; o.HouseNumber = r.HouseNumber;
        o.PostalCode = r.PostalCode; o.City = r.City; o.Phone = r.Phone; o.Email = r.Email;
        o.Website = r.Website; o.VatId = r.VatId; o.Iban = r.Iban; o.QrIban = r.QrIban;
        o.Currency = r.Currency; o.FiscalYearStartMonth = r.FiscalYearStartMonth; o.DefaultLanguage = r.DefaultLanguage;
        await _db.SaveChangesAsync();
        return await GetSettingsAsync(orgId);
    }

    public async Task<List<PaymentSourceDto>> GetPaymentSourcesAsync(int orgId)
        => await _db.PaymentSources.Where(s => s.OrganizationId == orgId).OrderBy(s => s.Name)
            .Select(s => new PaymentSourceDto(s.Id, s.Name, s.SourceType.ToString(), s.IsActive, s.Description, s.CreatedAt))
            .ToListAsync();

    public async Task<PaymentSourceDto> CreatePaymentSourceAsync(int orgId, CreatePaymentSourceRequest request)
    {
        var apiKey = Guid.NewGuid().ToString("N");
        var source = new Domain.Entities.PaymentSource
        {
            Name = request.Name,
            SourceType = Enum.TryParse<IntegrationSourceType>(request.SourceType, true, out var st) ? st : IntegrationSourceType.Other,
            ApiKey = apiKey,
            ApiKeyHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(apiKey))),
            WebhookUrl = request.WebhookUrl,
            Description = request.Description,
            OrganizationId = orgId
        };
        _db.PaymentSources.Add(source);
        await _db.SaveChangesAsync();
        return new PaymentSourceDto(source.Id, source.Name, source.SourceType.ToString(), source.IsActive, source.Description, source.CreatedAt);
    }

    public async Task DeletePaymentSourceAsync(int orgId, int id)
    {
        var source = await _db.PaymentSources.FirstOrDefaultAsync(s => s.Id == id && s.OrganizationId == orgId)
            ?? throw new InvalidOperationException("Payment source not found.");
        source.IsDeleted = true;
        await _db.SaveChangesAsync();
    }
}
