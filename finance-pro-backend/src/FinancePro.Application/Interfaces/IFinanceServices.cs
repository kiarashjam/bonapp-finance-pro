using FinancePro.Application.DTOs;

namespace FinancePro.Application.Interfaces;

public interface IAccountService
{
    Task<List<LedgerAccountDto>> GetAllAsync(int orgId);
    Task<LedgerAccountDto?> GetByIdAsync(int orgId, int id);
    Task<LedgerAccountDto> CreateAsync(int orgId, CreateLedgerAccountRequest request);
    Task<LedgerAccountDto> UpdateAsync(int orgId, int id, UpdateLedgerAccountRequest request);
    Task DeleteAsync(int orgId, int id);
    Task SeedChartOfAccountsAsync(int orgId);
}

public interface IJournalService
{
    Task<PagedResult<JournalEntryDto>> GetAllAsync(int orgId, int page, int pageSize);
    Task<JournalEntryDto?> GetByIdAsync(int orgId, int id);
    Task<JournalEntryDto> CreateAsync(int orgId, string userId, CreateJournalEntryRequest request);
    Task<JournalEntryDto> PostAsync(int orgId, int id, string userId);
    Task<JournalEntryDto> VoidAsync(int orgId, int id, string userId);
    Task<TrialBalanceDto> GetTrialBalanceAsync(int orgId, DateTime asOfDate);
}

public interface IInvoiceService
{
    Task<PagedResult<InvoiceDto>> GetAllAsync(int orgId, int page, int pageSize, string? status);
    Task<InvoiceDto?> GetByIdAsync(int orgId, int id);
    Task<InvoiceDto> CreateAsync(int orgId, string userId, CreateInvoiceRequest request);
    Task<InvoiceDto> UpdateStatusAsync(int orgId, int id, string newStatus);
    Task<byte[]> GeneratePdfAsync(int orgId, int id);
    Task DeleteAsync(int orgId, int id);
}

public interface ICustomerService
{
    Task<List<CustomerDto>> GetAllAsync(int orgId);
    Task<CustomerDto?> GetByIdAsync(int orgId, int id);
    Task<CustomerDto> CreateAsync(int orgId, CreateCustomerRequest request);
    Task<CustomerDto> UpdateAsync(int orgId, int id, CreateCustomerRequest request);
    Task DeleteAsync(int orgId, int id);
}

public interface IExpenseService
{
    Task<PagedResult<ExpenseDto>> GetAllAsync(int orgId, int page, int pageSize, ExpenseFilterDto? filter);
    Task<ExpenseDto?> GetByIdAsync(int orgId, int id);
    Task<ExpenseDto> CreateAsync(int orgId, string userId, CreateExpenseRequest request);
    Task<ExpenseDto> UpdateAsync(int orgId, int id, UpdateExpenseRequest request);
    Task DeleteAsync(int orgId, int id);
}

public record ExpenseFilterDto(DateTime? StartDate, DateTime? EndDate, string? Category, int? VendorId);

public interface IVendorService
{
    Task<List<VendorDto>> GetAllAsync(int orgId);
    Task<VendorDto?> GetByIdAsync(int orgId, int id);
    Task<VendorDto> CreateAsync(int orgId, CreateVendorRequest request);
    Task<VendorDto> UpdateAsync(int orgId, int id, CreateVendorRequest request);
    Task DeleteAsync(int orgId, int id);
}

public interface ISalesService
{
    Task<List<DailySalesSummaryDto>> GetDailySummariesAsync(int orgId, DateTime startDate, DateTime endDate);
    Task<DailySalesSummaryDto> CreateManualEntryAsync(int orgId, string userId, ManualSalesEntryRequest request);
    Task<CloseDayPreviewDto> GetCloseDayPreviewAsync(int orgId, DateTime date);
    Task<DailySalesSummaryDto> CloseDayAsync(int orgId, string userId, CloseDayRequest request);
    Task IngestPaymentAsync(int orgId, int sourceId, CreatePaymentRequest request);
}

public interface IReconciliationService
{
    Task<ReconciliationDashboardDto> GetDashboardAsync(int orgId);
    Task<List<BankAccountDto>> GetBankAccountsAsync(int orgId);
    Task<BankAccountDto> CreateBankAccountAsync(int orgId, CreateBankAccountRequest request);
    Task<int> ImportBankStatementAsync(int orgId, int bankAccountId, Stream csvStream);
    Task<List<BankTransactionDto>> GetTransactionsAsync(int orgId, int bankAccountId, int page, int pageSize);
    Task<List<PayoutDto>> GetPayoutsAsync(int orgId, int page, int pageSize, string? status);
    Task<PayoutDto> CreatePayoutAsync(int orgId, CreatePayoutRequest request);
}

public interface IReportService
{
    Task<ProfitAndLossDto> GetProfitAndLossAsync(int orgId, DateTime startDate, DateTime endDate);
    Task<VatReportDto> GetVatReportAsync(int orgId, DateTime startDate, DateTime endDate);
    Task<DashboardDto> GetDashboardAsync(int orgId);
}

public interface IOrganizationService
{
    Task<OrganizationSettingsDto> GetSettingsAsync(int orgId);
    Task<OrganizationSettingsDto> UpdateSettingsAsync(int orgId, UpdateOrganizationRequest request);
    Task<List<PaymentSourceDto>> GetPaymentSourcesAsync(int orgId);
    Task<PaymentSourceDto> CreatePaymentSourceAsync(int orgId, CreatePaymentSourceRequest request);
    Task DeletePaymentSourceAsync(int orgId, int id);
}

public interface IAuditService
{
    Task LogAsync(int orgId, string entityType, int entityId, string action, string? userId, string? oldValues, string? newValues, string? ipAddress);
    Task<PagedResult<AuditLogDto>> GetLogsAsync(int orgId, int page, int pageSize, string? entityType);
}
