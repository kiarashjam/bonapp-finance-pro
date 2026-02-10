# Finance Pro Backend – Code Review: Bugs and Issues

**Scope:** All `.cs` files under `src/` (excluding auto-generated migration Designer/Snapshot).  
**Focus:** Null reference risks, LINQ/runtime errors, entity/DTO/API consistency, validation, security, DbContext, and API contracts.

---

## 1. Null reference / missing null checks

### 1.1 **Program.cs – JWT key can be null**
- **Location:** `Program.cs` line 50  
- **Issue:** `builder.Configuration["Jwt:Key"]!` uses null-forgiving operator. If `Jwt:Key` is missing, `Encoding.UTF8.GetBytes(null)` throws `ArgumentNullException` at startup.  
- **Fix:** Validate config at startup (e.g. `if (string.IsNullOrEmpty(builder.Configuration["Jwt:Key"])) throw new InvalidOperationException("Jwt:Key is required.");`) or use a fallback and document that key must be set.

### 1.2 **AuthService – JWT key and expiry**
- **Location:** `AuthService.cs` lines 131, 133, 172  
- **Issue:** `_config["Jwt:Key"]!` and `_config["Jwt:ExpireHours"]` – key can be null; `double.Parse(_config["Jwt:ExpireHours"] ?? "24")` is safe but key is not.  
- **Fix:** Validate `Jwt:Key` in constructor or first use; consider `double.TryParse` for expiry if you allow invalid values to fall back to default.

### 1.3 **InvoiceService.CreateAsync – request.Lines can be null**
- **Location:** `InvoiceService.cs` line 54  
- **Issue:** `request.Lines.Select(...)` throws `NullReferenceException` if `CreateInvoiceRequest.Lines` is null.  
- **Fix:** Guard at start of method:  
  `if (request.Lines == null || !request.Lines.Any()) throw new ArgumentException("At least one invoice line is required.");`

### 1.4 **JournalService.CreateAsync – request.Lines can be null**
- **Location:** `JournalService.cs` lines 36–37, 51  
- **Issue:** `request.Lines.Sum(...)` and `request.Lines.Select(...)` throw if `CreateJournalEntryRequest.Lines` is null.  
- **Fix:**  
  `if (request.Lines == null || !request.Lines.Any()) throw new ArgumentException("Journal entry must have at least one line.");`

### 1.5 **GetOrgId returns 0 when claim missing**
- **Location:** `Program.cs` line 98  
- **Issue:** `int.Parse(user.FindFirst("organizationId")?.Value ?? "0")` returns `0` when the claim is missing. All subsequent queries use `OrganizationId == 0`, which can return no data or, if org 0 exists, wrong tenant.  
- **Fix:** For authorized endpoints, treat missing/zero org as unauthorized:  
  e.g. `if (orgId <= 0) return Results.Unauthorized();` in a filter or helper, or throw and map to 401.

### 1.6 **JournalEntry computed properties if Lines is null**
- **Location:** `JournalEntry.cs` lines 20–22  
- **Issue:** `TotalDebit`, `TotalCredit`, and `IsBalanced` use `Lines.Sum(...)`. If `Lines` were ever null (e.g. after deserialization or manual set), this would throw.  
- **Fix:** Either keep `Lines` never null (current default `= new List<>()`) and document it, or use null-conditional:  
  `Lines?.Sum(l => l.DebitAmount) ?? 0` (and same for credit).

---

## 2. Missing validation

### 2.1 **ReconciliationService.CreatePayoutAsync – PaymentSource not validated for org**
- **Location:** `ReconciliationService.cs` lines 117–123  
- **Issue:** Payout is created with `request.PaymentSourceId` and `orgId` but the code does not verify that the payment source belongs to the organization. A client could pass another org’s `PaymentSourceId` and create a payout linked to the wrong source (cross-tenant).  
- **Fix:** Load source scoped to org and reject if not found:  
  `var source = await _db.PaymentSources.FirstOrDefaultAsync(s => s.Id == request.PaymentSourceId && s.OrganizationId == orgId) ?? throw new InvalidOperationException("Payment source not found.");`  
  Then use `source.Name` in the DTO and optionally set `payout` from `source` if needed.

### 2.2 **Invoice CreateAsync – empty or invalid Lines**
- **Location:** `InvoiceService.cs`  
- **Issue:** Beyond null, there is no check for empty `Lines` or invalid quantities/prices (e.g. negative).  
- **Fix:** Validate `request.Lines` not null/empty; validate `Quantity > 0`, `UnitPrice >= 0` (or your business rules) and throw `ArgumentException` with a clear message.

### 2.3 **Journal CreateAsync – empty Lines or unbalanced entry**
- **Location:** `JournalService.cs`  
- **Issue:** Balance check exists; no explicit check that `Lines` is not empty.  
- **Fix:** Add validation that `request.Lines` has at least one line and that no line has both Debit and Credit zero (or apply your business rules).

---

## 3. Incorrect decimal / money handling

### 3.1 **InvoiceService – LineTotal vs SubTotal**
- **Location:** `InvoiceService.cs` lines 65, 71  
- **Issue:** `LineTotal = lineTotal + vatAmount` (line 65) is correct. `invoice.SubTotal = invoice.Lines.Sum(l => l.Quantity * l.UnitPrice)` (line 71) is net. `Total = SubTotal + VatTotal` is correct. No bug here; ensure rounding is consistent (e.g. round each line and then sum, or document that you round only totals).

### 3.2 **ExpenseService – VAT calculation**
- **Location:** `ExpenseService.cs` lines 45–46, 76–77  
- **Issue:** `netAmount = request.Amount / (1 + vatRate / 100)` assumes `request.Amount` is gross. If `vatRate` is 0 (Exempt), denominator is 1, so no division by zero. Logic is consistent.  
- **Suggestion:** Use `decimal` rounding explicitly (e.g. `Math.Round(..., 2)`) everywhere for money; already done in most places.

---

## 4. Division by zero risks

### 4.1 **ReportService.GetProfitAndLossAsync**
- **Location:** `ReportService.cs` lines 52–55  
- **Issue:** `revenue > 0 ? ... grossProfit / revenue * 100 ...` and similar – guarded, so no division by zero.  
- **Status:** OK.

### 4.2 **ReportService.GetDashboardAsync**
- **Location:** `ReportService.cs` lines 131–132  
- **Issue:** `pnl.Revenue > 0 ? Math.Round(pnl.Cogs / pnl.Revenue * 100, 1) : 0` – guarded.  
- **Status:** OK.

### 4.3 **ReconciliationService.GetDashboardAsync**
- **Location:** `ReconciliationService.cs` line 28  
- **Issue:** `reconRate = totalExpected > 0 ? (totalReceived / totalExpected) * 100 : 0` – guarded.  
- **Status:** OK.

### 4.4 **DailySalesSummary.AverageCheck**
- **Location:** `DailySalesSummary.cs` line 31  
- **Issue:** `TransactionCount > 0 ? NetSales / TransactionCount : 0` – guarded.  
- **Status:** OK.

---

## 5. Entity / DbContext configuration

### 5.1 **ReconciliationMatch – BankTransactionId and PayoutId ignored**
- **Location:** `FinanceProDbContext.cs` lines 188–191  
- **Issue:** `e.Ignore(r => r.BankTransactionId); e.Ignore(r => r.PayoutId);` – the FK is on the other side (BankTransaction.ReconciliationMatchId, Payout.ReconciliationMatchId), so the relationship is correct. The entity still has `BankTransactionId` and `PayoutId` in code but they are not persisted.  
- **Fix (optional):** Remove `BankTransactionId` and `PayoutId` from the `ReconciliationMatch` entity to avoid confusion, or document that they are not stored and only navigation properties are used.

### 5.2 **DailySalesSummary – no FK to Organization**
- **Location:** `FinanceProDbContext.cs`; entity `DailySalesSummary`  
- **Issue:** `DailySalesSummary` has `OrganizationId` but no navigation to `Organization`, and there is no `HasOne<Organization>().WithMany().HasForeignKey(d => d.OrganizationId)` (and `Organization` has no `DailySalesSummaries` collection). The migration does not add an FK from `DailySalesSummaries` to `Organizations`, so referential integrity is not enforced.  
- **Fix:** Add in `OnModelCreating`:  
  `builder.Entity<DailySalesSummary>().HasOne<Organization>().WithMany().HasForeignKey(d => d.OrganizationId).OnDelete(DeleteBehavior.Restrict);`  
  (Add a new migration after changing the model if you add a navigation or explicit FK.)

### 5.3 **PaymentRecord – no FK to Organization**
- **Location:** `FinanceProDbContext.cs`  
- **Issue:** Same pattern: `PaymentRecord` has `OrganizationId` but no configured relationship to `Organization` in the current model, and the initial migration does not create an FK.  
- **Fix:** Add explicit FK configuration for `PaymentRecord` to `Organization` (and migrate if needed).

### 5.4 **BankAccount – Organization relationship**
- **Location:** `FinanceProDbContext.cs`  
- **Issue:** Only `CurrentBalance` precision is configured; relationship to `Organization` is by convention. Migration already has `FK_BankAccounts_Organizations_OrganizationId`.  
- **Status:** OK; optionally add explicit `HasOne(b => b.Organization).WithMany(o => o.BankAccounts).HasForeignKey(b => b.OrganizationId)` for clarity.

### 5.5 **TaxRate – Organization**
- **Location:** Migration and model  
- **Issue:** TaxRates have FK to Organizations in migration. No explicit configuration in DbContext; convention is enough.  
- **Status:** OK.

### 5.6 **AuditLog – not BaseEntity**
- **Location:** `AuditLog.cs`, `FinanceProDbContext.cs`  
- **Issue:** `AuditLog` does not inherit `BaseEntity` (different key type and no soft delete). Global soft-delete filter only applies to `BaseEntity` types, so `AuditLog` is excluded. Intentional.  
- **Status:** OK.

---

## 6. DTOs vs entities / API contracts

### 6.1 **UpdateOrganizationRequest – Country not included**
- **Location:** `ReportDtos.cs` (UpdateOrganizationRequest); `OrganizationService.UpdateSettingsAsync`  
- **Issue:** `UpdateOrganizationRequest` does not include `Country`. Organization entity has `Country`; it is never updated from the API.  
- **Fix:** If the API should allow updating country, add `string? Country` to `UpdateOrganizationRequest` and set `o.Country = r.Country ?? o.Country` (or your rule). If not, document that country is set only at creation.

### 6.2 **CreatePaymentSourceRequest.SourceType as string**
- **Location:** `ReportDtos.cs`, `OrganizationService.CreatePaymentSourceAsync`  
- **Issue:** DTO uses `string SourceType`; entity uses `IntegrationSourceType`. Service uses `Enum.TryParse<IntegrationSourceType>(request.SourceType, true, out var st) ? st : IntegrationSourceType.Other`.  
- **Status:** OK; invalid values default to `Other`. Optionally validate and return 400 for invalid values.

### 6.3 **IAuditService.LogAsync – action as string**
- **Location:** `IAuditService.cs`, `AuditService.LogAsync`  
- **Issue:** Interface uses `string action`; entity uses `AuditAction` enum. Service parses with `Enum.TryParse<AuditAction>(action, out var a) ? a : AuditAction.Create`.  
- **Status:** OK; consider taking `AuditAction` in the interface for type safety.

---

## 7. Potential SQL injection / raw queries

- **Location:** All reviewed code  
- **Finding:** No raw SQL or string-concatenated queries; all use EF Core LINQ and parameters.  
- **Status:** No SQL injection issues found.

---

## 8. Missing async/await

- **Location:** All services  
- **Finding:** Async methods are awaited; no async void or fire-and-forget of async code.  
- **Status:** OK.  
- **Note:** `InvoiceService.GeneratePdfAsync` returns `Task.FromResult(Array.Empty<byte>())` (placeholder); when implemented, ensure PDF generation is non-blocking or offloaded if needed.

---

## 9. Unused variables / dead code

- **Location:** General  
- **Finding:** No obvious unused variables or dead code in the reviewed files.  
- **Status:** OK.

---

## 10. API endpoints – parameters and types

### 10.1 **invoices.MapPut("/{id}/status", ... string status)**
- **Location:** `Program.cs` line 194  
- **Issue:** Minimal APIs bind `status` from query or route. Route template is `"/{id}/status"`, so `status` is not in the route; it must come from query string (e.g. `?status=Paid`).  
- **Fix:** Document that `status` is a query parameter, or change to a request body (e.g. `UpdateInvoiceStatusRequest { Status = "Paid" }`) for clarity and to support more complex updates.

### 10.2 **reports – DateTime parameters**
- **Location:** `Program.cs` lines 327–328  
- **Issue:** `reports.MapGet("/profit-and-loss", async (DateTime startDate, DateTime endDate, ...)` – DateTime from query string may be culture-dependent and can be invalid.  
- **Fix:** Validate and parse (e.g. require ISO format), or use a DTO and model binding with validation attributes; return 400 with a clear message for invalid dates.

### 10.3 **sales.MapGet("/daily", DateTime startDate, DateTime endDate)**
- **Location:** `Program.cs` line 285  
- **Issue:** Same as above; no explicit validation of date range (e.g. startDate <= endDate).  
- **Fix:** Validate date range and format; return 400 if invalid.

### 10.4 **journals.MapGet("/trial-balance", DateTime asOfDate)**
- **Location:** `Program.cs` line 174  
- **Issue:** Same for single date.  
- **Fix:** Validate or document expected format and reject invalid values with 400.

---

## 11. Security

### 11.1 **PaymentSource – API key stored and compared in plain text**
- **Location:** `PaymentSource` entity (ApiKey), `Program.cs` line 368  
- **Issue:** Lookup uses `s.ApiKey == apiKey`. API key is stored in plain text. If the database is compromised, keys are exposed.  
- **Fix:** Store only a hash (e.g. SHA256) and compare `ApiKeyHash` with `Hash(providedKey)`. Keep generating a plain key once for the client to copy; do not persist the plain key (or restrict access to it).

### 11.2 **POS API – no rate limiting**
- **Location:** `Program.cs` posApi.MapPost("/payments", ...)  
- **Issue:** API key auth only; no rate limiting mentioned.  
- **Fix:** Add rate limiting (e.g. by API key or IP) to reduce abuse and brute force.

---

## 12. AuthService – token validation

### 12.1 **GetPrincipalFromExpiredToken can throw**
- **Location:** `AuthService.cs` line 166  
- **Issue:** `ValidateToken` can throw (e.g. `SecurityTokenException`) for malformed or invalid tokens. `RefreshTokenAsync` does not catch, so the exception can bubble as 500.  
- **Fix:** Wrap in try/catch and throw `UnauthorizedAccessException("Invalid or expired token.")` so the API returns 401 instead of 500.

---

## 13. Invoice line totals and rounding

### 13.1 **InvoiceService – LineTotal rounding**
- **Location:** `InvoiceService.cs` lines 56–66  
- **Issue:** `vatAmount = Math.Round(lineTotal * vatRateValue / 100, 2)`; `LineTotal = lineTotal + vatAmount`. If you round only per line, sum of line totals can differ slightly from `SubTotal + VatTotal` due to rounding.  
- **Fix:** Either define that line totals are source of truth and set `invoice.SubTotal`/`VatTotal`/`Total` from sums of line values (with optional final rounding), or document the current rounding policy.

---

## 14. ReportService – VAT report grouping

### 14.1 **GetVatReportAsync – GroupBy LedgerAccount**
- **Location:** `ReportService.cs` lines 71–74  
- **Issue:** `outputVat = journalLines.Where(l => l.LedgerAccount.AccountNumber.StartsWith("3")).GroupBy(l => l.VatRate)...` – `LedgerAccount` is included so it’s loaded. Grouping by `VatRate` (nullable) is fine.  
- **Status:** OK; ensure all journal lines have `LedgerAccount` loaded (current `Include` does).

---

## 15. UnitOfWork registration but limited use

### 15.1 **Program.cs – IUnitOfWork and UnitOfWork**
- **Location:** `Program.cs` line 67; `Repository.cs`  
- **Issue:** `UnitOfWork` is registered but most services use `FinanceProDbContext` directly and call `SaveChangesAsync` on it. Repository’s `AddAsync`/`UpdateAsync`/`DeleteAsync` call `_context.SaveChangesAsync` themselves, so each operation commits.  
- **Fix:** Either use `IUnitOfWork` in services when you need a single transaction across multiple operations (and stop saving inside repository methods when using UoW), or remove `UnitOfWork` registration if you do not need it.

---

## Summary table

| # | Category              | File(s)                    | Severity | One-line description |
|---|------------------------|----------------------------|----------|-----------------------|
| 1.1 | Null/Config           | Program.cs                 | High     | Jwt:Key null → startup crash |
| 1.2 | Null/Config           | AuthService.cs             | Medium   | JWT config not validated |
| 1.3 | Null reference        | InvoiceService.cs          | High     | request.Lines null → NRE |
| 1.4 | Null reference        | JournalService.cs          | High     | request.Lines null → NRE |
| 1.5 | Auth/tenant            | Program.cs                 | Medium   | GetOrgId 0 when claim missing |
| 1.6 | Null reference         | JournalEntry.cs            | Low      | Lines null on computed props |
| 2.1 | Validation/security   | ReconciliationService.cs   | High     | CreatePayoutAsync no org check on PaymentSource |
| 2.2 | Validation            | InvoiceService.cs          | Medium   | No validation of Lines content |
| 2.3 | Validation            | JournalService.cs          | Low      | No explicit empty Lines check |
| 5.1 | DbContext              | FinanceProDbContext.cs     | Low      | ReconciliationMatch ignored FKs |
| 5.2 | DbContext              | FinanceProDbContext.cs     | Medium   | DailySalesSummary no FK to Organization |
| 5.3 | DbContext              | FinanceProDbContext.cs     | Medium   | PaymentRecord no FK to Organization |
| 6.1 | DTO                    | ReportDtos / OrganizationService | Low  | UpdateOrganizationRequest missing Country |
| 10.1 | API                    | Program.cs                 | Low      | status parameter binding for invoice status |
| 10.2–10.4 | API                 | Program.cs                 | Medium   | DateTime query params not validated |
| 11.1 | Security               | PaymentSource / Program    | High     | API key stored and compared in plain text |
| 11.2 | Security               | Program.cs                 | Low      | No rate limiting on POS API |
| 12.1 | Auth                   | AuthService.cs             | Medium   | Refresh token validation throws → 500 |
| 15.1 | Design                 | Program / Repository       | Low      | UnitOfWork registered but not used consistently |

---

**Recommended order of fixes:**  
1) Null checks for `request.Lines` and JWT config (1.1, 1.2, 1.3, 1.4).  
2) CreatePayoutAsync PaymentSource org validation (2.1).  
3) API key hashing and refresh-token exception handling (11.1, 12.1).  
4) GetOrgId/authorization when org is missing (1.5).  
5) DbContext FK configuration for DailySalesSummary and PaymentRecord (5.2, 5.3).  
6) Date validation and API contract clarity (10.2–10.4, 10.1).
