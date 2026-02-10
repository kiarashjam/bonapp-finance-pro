using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FinancePro.Application.DTOs;
using FinancePro.Application.Interfaces;
using FinancePro.Domain.Entities;
using FinancePro.Domain.Enums;
using FinancePro.Domain.Interfaces;
using FinancePro.Infrastructure.Data;
using FinancePro.Infrastructure.Repositories;
using FinancePro.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ───── Database ─────
builder.Services.AddDbContext<FinanceProDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ───── Identity ─────
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<FinanceProDbContext>()
.AddDefaultTokenProviders();

// ───── JWT Authentication ─────
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});
builder.Services.AddAuthorization();

// ───── CORS ─────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(builder.Configuration["Cors:Origins"]?.Split(',') ?? ["http://localhost:5173"])
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// ───── Dependency Injection ─────
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IJournalService, JournalService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<ISalesService, SalesService>();
builder.Services.AddScoped<IReconciliationService, ReconciliationService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IAuditService, AuditService>();

// ───── Swagger ─────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ───── Middleware ─────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ───── Global Error Handler ─────
app.UseExceptionHandler(error => error.Run(async ctx =>
{
    ctx.Response.ContentType = "application/json";
    var ex = ctx.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
    var (status, message) = ex switch
    {
        UnauthorizedAccessException => (401, ex.Message),
        InvalidOperationException => (400, ex.Message),
        KeyNotFoundException => (404, ex.Message),
        _ => (500, app.Environment.IsDevelopment() ? ex?.Message ?? "Internal error" : "Internal server error")
    };
    ctx.Response.StatusCode = status;
    await ctx.Response.WriteAsJsonAsync(new { error = message });
}));

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

// ───── Helper to extract user claims ─────
static int GetOrgId(ClaimsPrincipal user) => int.Parse(user.FindFirst("organizationId")?.Value ?? "0");
static string GetUserId(ClaimsPrincipal user) => user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

// ═══════════════════════════════════════════════
// AUTH ENDPOINTS
// ═══════════════════════════════════════════════
var auth = app.MapGroup("/api/auth").WithTags("Authentication");

auth.MapPost("/register", async (RegisterRequest request, IAuthService svc) =>
    Results.Ok(await svc.RegisterAsync(request)));

auth.MapPost("/login", async (LoginRequest request, IAuthService svc) =>
    Results.Ok(await svc.LoginAsync(request)));

auth.MapPost("/refresh", async (RefreshTokenRequest request, IAuthService svc) =>
    Results.Ok(await svc.RefreshTokenAsync(request)));

auth.MapGet("/me", async (ClaimsPrincipal user, IAuthService svc) =>
    Results.Ok(await svc.GetCurrentUserAsync(GetUserId(user))))
    .RequireAuthorization();

auth.MapPost("/change-password", async (ChangePasswordRequest request, ClaimsPrincipal user, IAuthService svc) =>
{
    await svc.ChangePasswordAsync(GetUserId(user), request);
    return Results.Ok(new { message = "Password changed successfully." });
}).RequireAuthorization();

// ═══════════════════════════════════════════════
// CHART OF ACCOUNTS
// ═══════════════════════════════════════════════
var accounts = app.MapGroup("/api/accounts").WithTags("Chart of Accounts").RequireAuthorization();

accounts.MapGet("/", async (ClaimsPrincipal user, IAccountService svc) =>
    Results.Ok(await svc.GetAllAsync(GetOrgId(user))));

accounts.MapGet("/{id}", async (int id, ClaimsPrincipal user, IAccountService svc) =>
{
    var result = await svc.GetByIdAsync(GetOrgId(user), id);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

accounts.MapPost("/", async (CreateLedgerAccountRequest request, ClaimsPrincipal user, IAccountService svc) =>
    Results.Created($"/api/accounts", await svc.CreateAsync(GetOrgId(user), request)));

accounts.MapPut("/{id}", async (int id, UpdateLedgerAccountRequest request, ClaimsPrincipal user, IAccountService svc) =>
    Results.Ok(await svc.UpdateAsync(GetOrgId(user), id, request)));

accounts.MapDelete("/{id}", async (int id, ClaimsPrincipal user, IAccountService svc) =>
{
    await svc.DeleteAsync(GetOrgId(user), id);
    return Results.NoContent();
});

// ═══════════════════════════════════════════════
// JOURNAL ENTRIES
// ═══════════════════════════════════════════════
var journals = app.MapGroup("/api/journals").WithTags("Journal Entries").RequireAuthorization();

journals.MapGet("/", async (int? page, int? pageSize, ClaimsPrincipal user, IJournalService svc) =>
    Results.Ok(await svc.GetAllAsync(GetOrgId(user), page is > 0 ? page.Value : 1, pageSize is > 0 ? pageSize.Value : 20)));

journals.MapGet("/{id}", async (int id, ClaimsPrincipal user, IJournalService svc) =>
{
    var result = await svc.GetByIdAsync(GetOrgId(user), id);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

journals.MapPost("/", async (CreateJournalEntryRequest request, ClaimsPrincipal user, IJournalService svc) =>
    Results.Created($"/api/journals", await svc.CreateAsync(GetOrgId(user), GetUserId(user), request)));

journals.MapPost("/{id}/post", async (int id, ClaimsPrincipal user, IJournalService svc) =>
    Results.Ok(await svc.PostAsync(GetOrgId(user), id, GetUserId(user))));

journals.MapPost("/{id}/void", async (int id, ClaimsPrincipal user, IJournalService svc) =>
    Results.Ok(await svc.VoidAsync(GetOrgId(user), id, GetUserId(user))));

journals.MapGet("/trial-balance", async (DateTime asOfDate, ClaimsPrincipal user, IJournalService svc) =>
    Results.Ok(await svc.GetTrialBalanceAsync(GetOrgId(user), asOfDate)));

// ═══════════════════════════════════════════════
// INVOICES
// ═══════════════════════════════════════════════
var invoices = app.MapGroup("/api/invoices").WithTags("Invoices").RequireAuthorization();

invoices.MapGet("/", async (int? page, int? pageSize, string? status, ClaimsPrincipal user, IInvoiceService svc) =>
    Results.Ok(await svc.GetAllAsync(GetOrgId(user), page is > 0 ? page.Value : 1, pageSize is > 0 ? pageSize.Value : 20, status)));

invoices.MapGet("/{id}", async (int id, ClaimsPrincipal user, IInvoiceService svc) =>
{
    var result = await svc.GetByIdAsync(GetOrgId(user), id);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

invoices.MapPost("/", async (CreateInvoiceRequest request, ClaimsPrincipal user, IInvoiceService svc) =>
    Results.Created($"/api/invoices", await svc.CreateAsync(GetOrgId(user), GetUserId(user), request)));

invoices.MapPut("/{id}/status", async (int id, string status, ClaimsPrincipal user, IInvoiceService svc) =>
    Results.Ok(await svc.UpdateStatusAsync(GetOrgId(user), id, status)));

invoices.MapDelete("/{id}", async (int id, ClaimsPrincipal user, IInvoiceService svc) =>
{
    await svc.DeleteAsync(GetOrgId(user), id);
    return Results.NoContent();
});

// ═══════════════════════════════════════════════
// CUSTOMERS
// ═══════════════════════════════════════════════
var customers = app.MapGroup("/api/customers").WithTags("Customers").RequireAuthorization();

customers.MapGet("/", async (ClaimsPrincipal user, ICustomerService svc) =>
    Results.Ok(await svc.GetAllAsync(GetOrgId(user))));

customers.MapGet("/{id}", async (int id, ClaimsPrincipal user, ICustomerService svc) =>
{
    var result = await svc.GetByIdAsync(GetOrgId(user), id);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

customers.MapPost("/", async (CreateCustomerRequest request, ClaimsPrincipal user, ICustomerService svc) =>
    Results.Created($"/api/customers", await svc.CreateAsync(GetOrgId(user), request)));

customers.MapPut("/{id}", async (int id, CreateCustomerRequest request, ClaimsPrincipal user, ICustomerService svc) =>
    Results.Ok(await svc.UpdateAsync(GetOrgId(user), id, request)));

customers.MapDelete("/{id}", async (int id, ClaimsPrincipal user, ICustomerService svc) =>
{
    await svc.DeleteAsync(GetOrgId(user), id);
    return Results.NoContent();
});

// ═══════════════════════════════════════════════
// EXPENSES
// ═══════════════════════════════════════════════
var expenses = app.MapGroup("/api/expenses").WithTags("Expenses").RequireAuthorization();

expenses.MapGet("/", async (int? page, int? pageSize, DateTime? startDate, DateTime? endDate, int? vendorId, ClaimsPrincipal user, IExpenseService svc) =>
    Results.Ok(await svc.GetAllAsync(GetOrgId(user), page is > 0 ? page.Value : 1, pageSize is > 0 ? pageSize.Value : 20, new ExpenseFilterDto(startDate, endDate, null, vendorId))));

expenses.MapGet("/{id}", async (int id, ClaimsPrincipal user, IExpenseService svc) =>
{
    var result = await svc.GetByIdAsync(GetOrgId(user), id);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

expenses.MapPost("/", async (CreateExpenseRequest request, ClaimsPrincipal user, IExpenseService svc) =>
    Results.Created($"/api/expenses", await svc.CreateAsync(GetOrgId(user), GetUserId(user), request)));

expenses.MapPut("/{id}", async (int id, UpdateExpenseRequest request, ClaimsPrincipal user, IExpenseService svc) =>
    Results.Ok(await svc.UpdateAsync(GetOrgId(user), id, request)));

expenses.MapDelete("/{id}", async (int id, ClaimsPrincipal user, IExpenseService svc) =>
{
    await svc.DeleteAsync(GetOrgId(user), id);
    return Results.NoContent();
});

// ═══════════════════════════════════════════════
// VENDORS
// ═══════════════════════════════════════════════
var vendors = app.MapGroup("/api/vendors").WithTags("Vendors").RequireAuthorization();

vendors.MapGet("/", async (ClaimsPrincipal user, IVendorService svc) =>
    Results.Ok(await svc.GetAllAsync(GetOrgId(user))));

vendors.MapGet("/{id}", async (int id, ClaimsPrincipal user, IVendorService svc) =>
{
    var result = await svc.GetByIdAsync(GetOrgId(user), id);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

vendors.MapPost("/", async (CreateVendorRequest request, ClaimsPrincipal user, IVendorService svc) =>
    Results.Created($"/api/vendors", await svc.CreateAsync(GetOrgId(user), request)));

vendors.MapPut("/{id}", async (int id, CreateVendorRequest request, ClaimsPrincipal user, IVendorService svc) =>
    Results.Ok(await svc.UpdateAsync(GetOrgId(user), id, request)));

vendors.MapDelete("/{id}", async (int id, ClaimsPrincipal user, IVendorService svc) =>
{
    await svc.DeleteAsync(GetOrgId(user), id);
    return Results.NoContent();
});

// ═══════════════════════════════════════════════
// SALES & CLOSE DAY
// ═══════════════════════════════════════════════
var sales = app.MapGroup("/api/sales").WithTags("Sales & Close Day").RequireAuthorization();

sales.MapGet("/daily", async (DateTime startDate, DateTime endDate, ClaimsPrincipal user, ISalesService svc) =>
    Results.Ok(await svc.GetDailySummariesAsync(GetOrgId(user), startDate, endDate)));

sales.MapPost("/manual-entry", async (ManualSalesEntryRequest request, ClaimsPrincipal user, ISalesService svc) =>
    Results.Created($"/api/sales/daily", await svc.CreateManualEntryAsync(GetOrgId(user), GetUserId(user), request)));

sales.MapGet("/close-day/preview", async (DateTime date, ClaimsPrincipal user, ISalesService svc) =>
    Results.Ok(await svc.GetCloseDayPreviewAsync(GetOrgId(user), date)));

sales.MapPost("/close-day", async (CloseDayRequest request, ClaimsPrincipal user, ISalesService svc) =>
    Results.Ok(await svc.CloseDayAsync(GetOrgId(user), GetUserId(user), request)));

// ═══════════════════════════════════════════════
// BANK RECONCILIATION
// ═══════════════════════════════════════════════
var recon = app.MapGroup("/api/reconciliation").WithTags("Bank Reconciliation").RequireAuthorization();

recon.MapGet("/dashboard", async (ClaimsPrincipal user, IReconciliationService svc) =>
    Results.Ok(await svc.GetDashboardAsync(GetOrgId(user))));

recon.MapGet("/bank-accounts", async (ClaimsPrincipal user, IReconciliationService svc) =>
    Results.Ok(await svc.GetBankAccountsAsync(GetOrgId(user))));

recon.MapPost("/bank-accounts", async (CreateBankAccountRequest request, ClaimsPrincipal user, IReconciliationService svc) =>
    Results.Created($"/api/reconciliation/bank-accounts", await svc.CreateBankAccountAsync(GetOrgId(user), request)));

recon.MapPost("/bank-accounts/{bankAccountId}/import", async (int bankAccountId, HttpRequest req, ClaimsPrincipal user, IReconciliationService svc) =>
{
    if (!req.HasFormContentType) return Results.BadRequest("Expected multipart form data.");
    var form = await req.ReadFormAsync();
    var file = form.Files.FirstOrDefault() ?? throw new InvalidOperationException("No file uploaded.");
    var count = await svc.ImportBankStatementAsync(GetOrgId(user), bankAccountId, file.OpenReadStream());
    return Results.Ok(new { imported = count });
}).DisableAntiforgery();

recon.MapGet("/bank-accounts/{bankAccountId}/transactions", async (int bankAccountId, int? page, int? pageSize, ClaimsPrincipal user, IReconciliationService svc) =>
    Results.Ok(await svc.GetTransactionsAsync(GetOrgId(user), bankAccountId, page is > 0 ? page.Value : 1, pageSize is > 0 ? pageSize.Value : 50)));

recon.MapGet("/payouts", async (int? page, int? pageSize, string? status, ClaimsPrincipal user, IReconciliationService svc) =>
    Results.Ok(await svc.GetPayoutsAsync(GetOrgId(user), page is > 0 ? page.Value : 1, pageSize is > 0 ? pageSize.Value : 20, status)));

recon.MapPost("/payouts", async (CreatePayoutRequest request, ClaimsPrincipal user, IReconciliationService svc) =>
    Results.Created($"/api/reconciliation/payouts", await svc.CreatePayoutAsync(GetOrgId(user), request)));

// ═══════════════════════════════════════════════
// REPORTS
// ═══════════════════════════════════════════════
var reports = app.MapGroup("/api/reports").WithTags("Reports").RequireAuthorization();

reports.MapGet("/profit-and-loss", async (DateTime startDate, DateTime endDate, ClaimsPrincipal user, IReportService svc) =>
    Results.Ok(await svc.GetProfitAndLossAsync(GetOrgId(user), startDate, endDate)));

reports.MapGet("/vat", async (DateTime startDate, DateTime endDate, ClaimsPrincipal user, IReportService svc) =>
    Results.Ok(await svc.GetVatReportAsync(GetOrgId(user), startDate, endDate)));

reports.MapGet("/dashboard", async (ClaimsPrincipal user, IReportService svc) =>
    Results.Ok(await svc.GetDashboardAsync(GetOrgId(user))));

// ═══════════════════════════════════════════════
// TAX RATES
// ═══════════════════════════════════════════════
var tax = app.MapGroup("/api/tax-rates").WithTags("Tax Rates").RequireAuthorization();

tax.MapGet("/", async (ClaimsPrincipal user, FinanceProDbContext db) =>
{
    var orgId = GetOrgId(user);
    var rates = await db.TaxRates.Where(t => t.OrganizationId == orgId && t.IsActive).OrderBy(t => t.Rate)
        .Select(t => new TaxRateDto(t.Id, t.Name, t.RateType.ToString(), t.Rate, t.IsActive, t.EffectiveFrom, t.EffectiveTo))
        .ToListAsync();
    return Results.Ok(rates);
});

// ═══════════════════════════════════════════════
// SETTINGS & INTEGRATION
// ═══════════════════════════════════════════════
var settings = app.MapGroup("/api/settings").WithTags("Settings").RequireAuthorization();

settings.MapGet("/organization", async (ClaimsPrincipal user, IOrganizationService svc) =>
    Results.Ok(await svc.GetSettingsAsync(GetOrgId(user))));

settings.MapPut("/organization", async (UpdateOrganizationRequest request, ClaimsPrincipal user, IOrganizationService svc) =>
    Results.Ok(await svc.UpdateSettingsAsync(GetOrgId(user), request)));

settings.MapGet("/payment-sources", async (ClaimsPrincipal user, IOrganizationService svc) =>
    Results.Ok(await svc.GetPaymentSourcesAsync(GetOrgId(user))));

settings.MapPost("/payment-sources", async (CreatePaymentSourceRequest request, ClaimsPrincipal user, IOrganizationService svc) =>
    Results.Created($"/api/settings/payment-sources", await svc.CreatePaymentSourceAsync(GetOrgId(user), request)));

settings.MapDelete("/payment-sources/{id}", async (int id, ClaimsPrincipal user, IOrganizationService svc) =>
{
    await svc.DeletePaymentSourceAsync(GetOrgId(user), id);
    return Results.NoContent();
});

// ═══════════════════════════════════════════════
// AUDIT LOG
// ═══════════════════════════════════════════════
var audit = app.MapGroup("/api/audit").WithTags("Audit").RequireAuthorization();

audit.MapGet("/", async (int? page, int? pageSize, string? entityType, ClaimsPrincipal user, IAuditService svc) =>
    Results.Ok(await svc.GetLogsAsync(GetOrgId(user), page is > 0 ? page.Value : 1, pageSize is > 0 ? pageSize.Value : 50, entityType)));

// ═══════════════════════════════════════════════
// INBOUND INTEGRATION API (API Key Auth)
// ═══════════════════════════════════════════════
var posApi = app.MapGroup("/api/pos/v1").WithTags("POS Integration");

posApi.MapPost("/payments", async (HttpContext ctx, CreatePaymentRequest request, FinanceProDbContext db, ISalesService svc) =>
{
    var apiKey = ctx.Request.Headers["X-Api-Key"].FirstOrDefault();
    if (string.IsNullOrEmpty(apiKey)) return Results.Unauthorized();

    var source = await db.PaymentSources.FirstOrDefaultAsync(s => s.ApiKey == apiKey && s.IsActive);
    if (source == null) return Results.Unauthorized();

    await svc.IngestPaymentAsync(source.OrganizationId, source.Id, request);
    return Results.Ok(new { message = "Payment ingested." });
});

// ═══════════════════════════════════════════════
// DATABASE MIGRATION ON STARTUP
// ═══════════════════════════════════════════════
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FinanceProDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();
