namespace FinancePro.Application.DTOs;

public record ProfitAndLossDto(
    DateTime StartDate,
    DateTime EndDate,
    decimal Revenue,
    decimal Cogs,
    decimal GrossProfit,
    decimal GrossProfitMargin,
    decimal LaborCost,
    decimal PrimeCost,
    decimal PrimeCostPercent,
    decimal OperatingExpenses,
    decimal NetOperatingProfit,
    decimal NetProfitMargin,
    List<PnlLineItemDto> RevenueItems,
    List<PnlLineItemDto> CogsItems,
    List<PnlLineItemDto> LaborItems,
    List<PnlLineItemDto> OpExItems);

public record PnlLineItemDto(string AccountNumber, string AccountName, decimal Amount, decimal Percent);

public record VatReportDto(
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalRevenue,
    decimal OutputVatStandard,
    decimal OutputVatReduced,
    decimal OutputVatHospitality,
    decimal TotalOutputVat,
    decimal TotalInputVat,
    decimal NetVatPayable,
    List<VatReportLineDto> Lines);

public record VatReportLineDto(string Description, string VatRateName, decimal Rate, decimal TaxableAmount, decimal VatAmount);

public record TrialBalanceDto(DateTime AsOfDate, List<TrialBalanceLineDto> Lines, decimal TotalDebit, decimal TotalCredit);
public record TrialBalanceLineDto(string AccountNumber, string AccountName, string AccountType, decimal DebitBalance, decimal CreditBalance);

public record DashboardDto(
    decimal TodayRevenue,
    decimal WeekRevenue,
    decimal MonthRevenue,
    decimal PrimeCostPercent,
    decimal FoodCostPercent,
    decimal LaborCostPercent,
    decimal NetProfitMargin,
    decimal CashPosition,
    decimal OutstandingAR,
    decimal OutstandingAP,
    decimal ReconciliationRate,
    decimal VatLiability,
    List<RevenueDataPointDto> RevenueChart);

public record RevenueDataPointDto(DateTime Date, decimal Amount);

public record PaymentSourceDto(int Id, string Name, string SourceType, bool IsActive, string? Description, DateTime CreatedAt);
public record CreatePaymentSourceRequest(string Name, string SourceType, string? WebhookUrl, string? Description);

public record OrganizationSettingsDto(int Id, string Name, string? LegalName, string? Street, string? HouseNumber, string? PostalCode, string? City, string? Country, string? Phone, string? Email, string? Website, string? VatId, string? Iban, string? QrIban, string? LogoUrl, string Currency, int FiscalYearStartMonth, string DefaultLanguage);
public record UpdateOrganizationRequest(string Name, string? LegalName, string? Street, string? HouseNumber, string? PostalCode, string? City, string? Phone, string? Email, string? Website, string? VatId, string? Iban, string? QrIban, string Currency, int FiscalYearStartMonth, string DefaultLanguage);

public record TaxRateDto(int Id, string Name, string RateType, decimal Rate, bool IsActive, DateTime EffectiveFrom, DateTime? EffectiveTo);

public record AuditLogDto(long Id, string EntityType, int EntityId, string Action, string? UserName, DateTime Timestamp);

public record PagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize);
