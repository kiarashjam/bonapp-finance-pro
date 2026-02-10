# Finance Pro - Frontend Development Report

**Generated:** February 9, 2026  
**Project:** Finance Pro — Standalone Restaurant Financial Management System  
**Framework:** React 18 | Vite 5 | TypeScript  
**Architecture:** Standalone React application (NOT part of BonApp monorepo)  
**Repository:** `finance-pro-frontend` (separate Git repository)  
**Status:** Planning Phase

---

## 1. Executive Summary

This report defines the complete frontend development plan for **Finance Pro**, a fully **autonomous, standalone** restaurant financial management frontend. This is a **completely independent application** — it has its own codebase, dependencies, design system, authentication UI, and deployment. It does NOT live inside the BonApp frontend monorepo.

Finance Pro is the **"money brain"** dashboard for restaurant owners, accountants, and managers. It visualizes profitability, handles invoicing, provides bank reconciliation tools, and generates tax-compliant reports. The UI is designed for financial clarity — numbers must be precise, layouts must be dense yet readable, and workflows must guide users through complex financial processes step by step.

The application is designed for **any restaurant** regardless of their POS system:
- Restaurants using **BonApp POS** (integrated financial data)
- Restaurants using **Lightspeed, Toast, Square, Orderbird**
- Restaurants using **any POS system** with API support
- Restaurants with **no POS system** at all (manual entry + invoicing + bank reconciliation)

### Core Features at a Glance (MVP Only — P0)

These are the **only features in the first frontend release**. Everything else comes later.

| # | Page/Module | What the User Sees |
|---|-----------|-------------------|
| 1 | Login / Register / Reset Password | Standard auth flow |
| 2 | Dashboard | KPI cards (revenue, prime cost %, cash position), revenue trend chart, reconciliation status |
| 3 | Sales & Payments | Payment list, manual sales entry form, CSV import wizard |
| 4 | Chart of Accounts | Tree view of Swiss accounts, add/edit accounts |
| 5 | Journal Entries | Journal list, new entry form (debit/credit lines), trial balance |
| 6 | Close Day Wizard | Step-by-step: preview totals → verify cash → confirm → lock the day |
| 7 | Expenses | Expense list, new expense form with file upload, vendor management |
| 8 | Invoicing | Invoice list, create/edit invoice, live PDF preview with QR-bill, customer management |
| 9 | Bank Reconciliation | Import bank CSV, payout tracking list, reconciliation dashboard (expected vs. received vs. missing) |
| 10 | VAT Reports | Tax rate overview, generate VAT return, ESTV export |
| 11 | P&L Report | Revenue → COGS → Gross Profit → Labor → OpEx → Net Profit. Prime cost % |
| 12 | Settings | Organization profile, general preferences, user management |

> **That's it for v1.** No AI, no multi-location switcher, no delivery dashboards, no gift cards, no budget editor. Those come in Phase 2-4.

### Architectural Independence

- **Own Git repository** — `finance-pro-frontend/` is a standalone project
- **Own design system** — Based on Ant Design 5, NOT `@repo/ui`
- **Own authentication** — Login, register, forgot password (talks to own Finance Pro backend)
- **Own deployment** — Separate Docker build, separate CDN, separate CI/CD pipeline
- **No shared code** — No `@repo/ui`, `@repo/configs`, or `@repo/eslint-config` dependencies
- **No cross-app navigation** — No links to/from BonApp Manager, Waiter, or Customer apps

### Design Principles

- **Financial clarity first** — Numbers are the hero. Tables, charts, and KPI cards must be immediately readable. Use accounting conventions: negative amounts in red with parentheses, consistent decimal places, right-aligned numbers.
- **Guided workflows** — Complex processes (close-day, reconciliation, VAT reporting) use step-by-step wizards with preview-before-commit patterns.
- **Dense but organized** — Financial users expect data density. Use compact table rows, collapsible sections, and tabbed interfaces — not excessive whitespace.
- **Desktop-optimized, mobile-accessible** — Primary use is desktop/tablet (accountants at desks). Key mobile functions: approve expenses, check KPIs.
- **Print-ready layouts** — Reports, invoices, and tax documents must have clean print stylesheets.
- **Dark mode** — Accountants often work long hours. Dark mode reduces eye strain.
- **(Phase 2) Daily flash report at a glance** — One screen that answers "how did we do yesterday?" Added after close-day workflow is stable.
- **(Phase 2) Role-based home screens** — Owner sees P&L + cash flow. Accountant sees ledger + reconciliation. Manager sees daily flash. Staff sees expenses only.
- **(Phase 3) Multi-location aware** — Location switcher in header. Consolidated vs. per-location views. Added after single-location is proven.
- **(Phase 3) Real-time alerts** — Financial anomalies surface as in-app notifications. Added after core features are stable.

### What to Build First vs. Later

**The MVP frontend must deliver a complete, usable finance app without the advanced features.** Advanced features (AI, multi-location, delivery tracking) are planned but NOT built until MVP is stable.

| Phase | Frontend Scope | Pages to Build |
|-------|---------------|----------------|
| **MVP (P0)** | Auth, dashboard with KPIs, sales overview + manual entry, chart of accounts, journal entries, close-day wizard, basic P&L + prime cost reports, expense entry + vendor management, invoice creation + PDF/QR-bill, customer management, bank statement import (CSV), reconciliation dashboard + payout tracking, VAT reports, settings, audit log | ~45 pages |
| **Phase 2 (P1)** | Daily flash report, auto/manual reconciliation matching, cash drawer management, close-month wizard, fiscal periods, revenue analytics, multi-period comparison, tip summary, reconciliation alerts, recurring expenses, bill tracking | ~20 pages |
| **Phase 3 (P2)** | Delivery platform dashboard, budget editor + variance, invoice OCR scanner, multi-location (location switcher, comparison, consolidated), scheduled reports, variance alerts center, payment reminders, Abacus/Sage export, controllable P&L | ~25 pages |
| **Phase 4 (P3)** | AI cash flow forecast, AI revenue forecast, gift card management, fixed asset register + depreciation, above-store dashboard, break-even analysis | ~15 pages |

### What Makes This Best-in-Class (Future Phases — Based on Industry Research)

| Feature | Source | Phase | Impact |
|---------|--------|-------|--------|
| **Daily Flash Report** | Restaurant365 #1 feature | Phase 2 | Managers get yesterday's performance at 6 AM — revenue, costs, prime cost %, vs. last week/year |
| **AI Invoice OCR** | MarginEdge, R365 AP Capture AI | Phase 3 | Snap photo of supplier invoice → AI fills in vendor, amount, line items, VAT → 50% less data entry |
| **Budget vs. Actual** | Standard for multi-unit operators | Phase 3 | Real-time variance tracking prevents month-end surprises |
| **Delivery Platform Dashboard** | Pain point for 80%+ of restaurants | Phase 3 | See true profitability of Uber Eats/DoorDash after 15-25% commissions |
| **Gift Card Management** | ASC 606 compliance | Phase 4 | Track liability, redemption, breakage — most restaurants get this wrong |
| **Multi-Location Comparison** | Above-store reporting (CrunchTime, Forte) | Phase 3 | Side-by-side KPIs: which location is best/worst? |
| **Automated Report Email** | Distil.ai, OneDataSource | Phase 3 | Daily flash, weekly P&L, monthly package — auto-emailed to stakeholders |
| **AI Cash Flow Forecasting** | Distil.ai (98-99% accuracy) | Phase 4 | Predict cash position forward 7/30/90 days |

### MVP Frontend Implementation Order (Build in This Sequence)

| Step | What to Build | Pages | Depends On (Backend) |
|------|--------------|-------|---------------------|
| 1 | **Project setup** — Vite, React, TypeScript, Ant Design theme, Redux store shell, router shell, Axios client | 0 | Project setup |
| 2 | **Auth pages** — Login, register, forgot/reset password | 4 | Auth API |
| 3 | **App layout** — Sidebar, header, content wrapper, page header | 0 (layout) | Auth working |
| 4 | **Settings pages** — Organization settings, general settings, user management | 3 | Settings API |
| 5 | **Chart of Accounts** — Tree view, account form, account detail | 4 | Accounts API |
| 6 | **Journal entries** — List, form (debit/credit lines), detail, trial balance | 4 | Journal API |
| 7 | **Expense module** — Expense list, form (with file upload), detail, vendor list/form/detail | 6 | Expenses + Vendors API |
| 8 | **Invoice module** — Invoice list, form (with line editor + live preview), detail, PDF preview with QR-bill, customer list/form/detail | 7 | Invoices + Customers API |
| 9 | **Sales & payments** — Sales overview, manual entry form, CSV import wizard, payment list/detail | 6 | Payments API |
| 10 | **Close-day wizard** — Multi-step wizard with preview | 3 | Close-day API |
| 11 | **Bank reconciliation** — Bank account list/form, statement import wizard, transaction list, payout list, reconciliation dashboard | 8 | Reconciliation API |
| 12 | **VAT reports** — Tax overview, rate table, report generator, report detail | 4 | Tax API |
| 13 | **P&L + reports** — Profit & Loss, prime cost report, cash flow, balance sheet, reports hub, export center | 6 | Reports API |
| 14 | **Dashboard** — KPI cards, revenue chart, payment method pie, alerts widget, reconciliation status, quick actions | 1 (complex) | Dashboard API |

> **Total MVP: ~45 pages + layout components + shared components + ~12 Redux slices + ~16 API modules**
> This is a real, shippable product. Everything after this is enhancement.

---

## 2. Application Configuration

### Standalone Project Setup

| Setting | Value |
|---------|-------|
| **App Name** | `finance-pro` |
| **Location** | `finance-pro-frontend/` (own Git repository) |
| **Dev Port** | 3100 |
| **Base Path** | `/` |
| **Language** | TypeScript (.tsx) |
| **React Version** | ^18.3.1 |
| **Vite Version** | ^5.3.1 |
| **UI Library** | Ant Design 5 (own theme — NOT @repo/ui) |
| **State Management** | Redux Toolkit |
| **Redux Persist** | Yes (auth, settings, ui) |
| **Router** | React Router ^7.x |
| **Forms** | Formik + Yup |
| **i18n Fallback** | English ("en"), supports German, French, Italian (Swiss quadrilingual) |
| **Charts** | Chart.js ^4.x + react-chartjs-2 (main), Recharts for complex visualizations |
| **Tables** | Ant Design Table + custom virtual scrolling for large ledger datasets |
| **Date Handling** | dayjs |
| **Animation** | Framer Motion (subtle transitions) |
| **PDF Preview** | react-pdf for in-app invoice/report preview |
| **PDF Generation** | Server-side (backend API returns PDF) |
| **CSV/Excel** | SheetJS (xlsx) for import/export |
| **Number Formatting** | Intl.NumberFormat with Swiss locale (1'234.56 CHF) |
| **Notifications** | react-hot-toast |

### Package Manager

```bash
# Standalone project — uses npm or pnpm directly (no monorepo)
npm create vite@latest finance-pro-frontend -- --template react-ts
cd finance-pro-frontend
npm install
```

### CI/CD Pipeline

GitHub Actions or Azure DevOps (standalone pipeline):
- Trigger: `main`, `develop`, `feature/*`
- Steps: Install → Lint → Type Check → Test → Build → Deploy
- Deployment: Docker → Azure App Service / Vercel / Netlify

---

## 3. Dependencies

### Production Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| `react` | ^18.3.1 | UI framework |
| `react-dom` | ^18.3.1 | DOM rendering |
| `react-router` | ^7.1.3 | Routing (v7) |
| `@reduxjs/toolkit` | ^2.2.6 | State management |
| `react-redux` | ^9.1.2 | React-Redux bindings |
| `redux-persist` | ^6.0.0 | State persistence |
| `axios` | ^1.6.8 | HTTP client |
| `formik` | ^2.4.6 | Form handling |
| `yup` | ^1.4.0 | Schema validation |
| `antd` | ^5.22.1 | UI component library |
| `@ant-design/icons` | ^5.5.2 | Icons |
| `@ant-design/pro-components` | ^2.7.0 | ProTable, ProForm, ProLayout — finance-grade data components |
| `chart.js` | ^4.4.3 | Charts for reports and dashboards |
| `react-chartjs-2` | ^5.2.0 | Chart.js React bindings |
| `recharts` | ^2.12.0 | Complex visualizations (waterfall, treemap) |
| `framer-motion` | ^11.3.31 | Animations |
| `react-hot-toast` | ^2.4.1 | Toast notifications |
| `dayjs` | ^1.11.13 | Date handling |
| `i18next` | ^23.11.4 | Internationalization |
| `i18next-browser-languagedetector` | ^8.0.0 | Language detection |
| `i18next-http-backend` | ^2.5.1 | Translation loading |
| `react-i18next` | ^14.1.1 | React i18n bindings |
| `xlsx` | ^0.18.5 | CSV/Excel import/export |
| `file-saver` | ^2.0.5 | File downloads |
| `react-pdf` | ^7.7.0 | PDF preview in browser |
| `lodash` | ^4.17.21 | Utility functions |
| `numeral` | ^2.0.6 | Number formatting for accounting |
| `react-grid-layout` | ^1.4.4 | Drag-and-drop dashboard widget layout |
| `@hello-pangea/dnd` | ^17.0.0 | Drag and drop for list reordering |
| `react-dropzone` | ^14.2.3 | File upload drag-and-drop (bank statements, invoices, receipts) |
| `react-to-print` | ^2.15.1 | Print reports/invoices directly from browser |
| `qrcode.react` | ^3.1.0 | QR code generation (Swiss QR-bill display) |
| `zustand` | ^4.5.0 | Lightweight state for UI-only state (modals, drawers) — optional |

### Dev Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| `vite` | ^5.3.1 | Build tool |
| `@vitejs/plugin-react` | ^4.3.1 | React Vite plugin |
| `typescript` | ^5.5.0 | TypeScript compiler |
| `vite-plugin-svgr` | ^4.2.0 | SVG as React components |
| `sass` | ^1.77.1 | SCSS preprocessor |
| `vitest` | ^1.6.0 | Unit testing |
| `@testing-library/react` | ^15.0.7 | Component testing |
| `@testing-library/jest-dom` | ^6.4.5 | Jest DOM matchers |
| `jsdom` | ^24.0.0 | Test DOM environment |
| `msw` | ^2.3.0 | Mock Service Worker for API mocking in tests |
| `@playwright/test` | ^1.44.0 | E2E testing |

---

## 4. Application Structure

### Router Configuration

```
basename: "/"

/login → LoginPage (own auth — NOT BonApp's Login app)
/register → RegisterPage (create account + organization)
/forgot-password → ForgotPasswordPage
/reset-password → ResetPasswordPage

/ → redirect to /dashboard (if authenticated)
/dashboard → FinanceDashboard (KPI cards + customizable widget grid)

— DAILY FLASH REPORT —
/flash → DailyFlashReport (yesterday's one-page operational snapshot — THE key daily view)
  /today → LiveFlashReport (today's running numbers, incomplete)
  /:date → HistoricalFlashReport (any past date)
  /trend → FlashReportTrend (multi-day flash report comparison table)

— SALES & PAYMENTS —
/sales → SalesOverview (daily/weekly/monthly revenue, payment method breakdown)
  /daily/:date → DailyDetail (all transactions for a day)
  /manual-entry → ManualSalesEntry (standalone mode: enter daily totals)
  /import → SalesImport (CSV upload for historical data)

/payments → PaymentList (all payment records, filterable)
  /:paymentId → PaymentDetail

/cash-drawers → CashDrawerList
  /:drawerId → CashDrawerDetail (opening, sales, drops, closing, variance)
  /close → CashDrawerCloseForm

— BOOKKEEPING —
/ledger → LedgerOverview (chart of accounts + trial balance)
  /accounts → ChartOfAccounts (tree view of all accounts)
  /accounts/new → LedgerAccountForm (create)
  /accounts/:accountId → AccountDetail (balance, transaction history)
  /accounts/:accountId/edit → LedgerAccountForm (edit)

/journal → JournalEntryList (all journal entries, filterable)
  /new → JournalEntryForm (manual entry — debit/credit lines)
  /:journalId → JournalEntryDetail (lines, source, audit)
  /:journalId/edit → JournalEntryForm (draft only)

/close-day → CloseDayWizard (step-by-step guided day closing)
  /preview → CloseDayPreview (review before committing)
  /history → CloseDayHistory (past day closings)

/close-month → CloseMonthWizard (step-by-step month closing)
  /history → CloseMonthHistory

— BANK RECONCILIATION —
/reconciliation → ReconciliationDashboard (expected vs. received vs. missing)
  /bank-accounts → BankAccountList
  /bank-accounts/new → BankAccountForm
  /bank-accounts/:bankAccountId → BankAccountDetail

  /import → BankStatementImport (CSV / CAMT.053 upload with preview)
  /transactions → BankTransactionList (all imported transactions)
  /transactions/:transactionId → BankTransactionDetail

  /payouts → PayoutList (expected payouts from payment providers)
  /payouts/:payoutId → PayoutDetail

  /matching → ReconciliationMatching (side-by-side matching UI)
    /auto → AutoMatchResults (review auto-matches)
    /manual → ManualMatchingWorkbench (drag-drop matching)

  /providers → PaymentProviderList (registered payment provider accounts)
  /providers/new → PaymentProviderForm
  /providers/:providerId → PaymentProviderDetail

— INVOICING —
/invoices → InvoiceList (all invoices with status tabs)
  /new → InvoiceForm (create invoice with line items)
  /:invoiceId → InvoiceDetail (view with PDF preview)
  /:invoiceId/edit → InvoiceForm (draft only)
  /:invoiceId/pdf → InvoicePdfPreview (full-screen PDF with print)

/customers → CustomerList (B2B clients / debtors)
  /new → CustomerForm
  /:customerId → CustomerDetail (open invoices, payment history, aging)
  /:customerId/edit → CustomerForm

— BUDGET —
/budgets → BudgetList (annual/monthly budgets, status)
  /new → BudgetWizard (create new budget, copy from last year, or blank)
  /:budgetId → BudgetDetail (all budget lines with amounts)
  /:budgetId/edit → BudgetEditor (spreadsheet-style budget line editing)
  /variance → BudgetVarianceReport (budget vs. actual, YTD summary, per-line drill-down)

— 3RD-PARTY DELIVERY —
/delivery → DeliveryDashboard (gross vs. commission vs. net by platform, profitability analysis)
  /platforms → DeliveryPlatformList (registered platforms with commission rates)
  /platforms/new → DeliveryPlatformForm
  /platforms/:platformId → DeliveryPlatformDetail (payout history, effective cost analysis)
  /platforms/:platformId/import → DeliveryPayoutImport (CSV upload from platform)
  /reconciliation → DeliveryReconciliation (match platform payouts to bank deposits)

— GIFT CARDS —
/gift-cards → GiftCardOverview (outstanding liability, issued/redeemed/expired counts)
  /issue → IssueGiftCardForm
  /:cardId → GiftCardDetail (transaction history, balance)
  /redeem → RedeemGiftCardForm (quick redeem by card number)
  /liability → GiftCardLiabilityReport (total outstanding, breakage analysis)
  /breakage → BreakageCalculation (calculate and recognize breakage revenue)

— FIXED ASSETS —
/assets → FixedAssetList (all equipment with book value, depreciation status)
  /new → FixedAssetForm
  /:assetId → FixedAssetDetail (depreciation schedule, history)
  /:assetId/edit → FixedAssetForm
  /depreciation → DepreciationSchedule (full schedule for all assets, run monthly)
  /depreciation/run → RunDepreciationWizard (preview + execute monthly depreciation)

— MULTI-LOCATION —
/locations → LocationOverview (list of all locations with status)
  /new → LocationForm
  /:locationId → LocationDetail (location-specific financials)
  /:locationId/edit → LocationForm
  /comparison → LocationComparison (side-by-side KPI comparison — the above-store dashboard)
  /consolidated → ConsolidatedDashboard (rolled-up financials across all locations)

— EXPENSES & BILLS —
/expenses → ExpenseList (all expenses with status filters)
  /new → ExpenseForm (create with file upload)
  /:expenseId → ExpenseDetail (with attachments, approval status)
  /:expenseId/edit → ExpenseForm
  /import → ExpenseImport (CSV upload)

/vendors → VendorList (suppliers / creditors)
  /new → VendorForm
  /:vendorId → VendorDetail (open bills, payment history, aging)
  /:vendorId/edit → VendorForm

— TAX & COMPLIANCE —
/tax → TaxOverview (VAT summary, filing status)
  /rates → TaxRateList (Swiss VAT rates)
  /reports → TaxReportList (quarterly/annual VAT returns)
  /reports/:periodId → TaxReportDetail (breakdown by rate, exportable)
  /reports/generate → TaxReportGenerator (select period, preview, generate)

— AI TOOLS —
/ai → AiToolsHub (AI-powered features overview)
  /invoice-scanner → InvoiceOcrScanner (upload photo/PDF → AI extracts data → confirm → create expense)
  /cash-flow-forecast → AiCashFlowForecast (7/30/90 day AI-powered cash flow projection)
  /revenue-forecast → AiRevenueForecast (AI-powered revenue prediction with confidence intervals)

— ALERTS & NOTIFICATIONS —
/alerts → AlertCenter (all financial alerts: variance, overdue, missing payouts, anomalies)
  /settings → AlertSettings (configure thresholds per metric, per location)
  /scheduled-reports → ScheduledReportList (manage automated report delivery)
  /scheduled-reports/new → ScheduledReportForm

— REPORTS —
/reports → ReportsHub (overview of all available reports)
  /profit-loss → ProfitLossReport (P&L statement with period comparison)
  /prime-cost → PrimeCostReport (COGS + Labor analysis)
  /revenue → RevenueAnalyticsReport (by method, period, category)
  /cash-flow → CashFlowReport (statement + forecast)
  /balance-sheet → BalanceSheetReport (simplified)
  /ar-aging → AccountsReceivableAging (invoice aging buckets)
  /ap-aging → AccountsPayableAging (expense/bill aging buckets)
  /reconciliation-summary → ReconciliationSummaryReport
  /budget-variance → BudgetVarianceReport (budget vs. actual by line, YTD)
  /controllable-pl → ControllableProfitLossReport (controllable vs. non-controllable P&L)
  /delivery-profitability → DeliveryProfitabilityReport (by platform, effective cost analysis)
  /location-comparison → LocationComparisonReport (multi-location KPI comparison)
  /flash-trend → FlashReportTrendAnalysis (flash report metrics over 30/90/365 days)
  /export → ExportCenter (Abacus, Sage, generic CSV exports)

— SETTINGS —
/settings → SettingsOverview
  /general → GeneralSettings (fiscal year, prime cost targets, VAT defaults)
  /organization → OrganizationSettings (name, address, VAT number, logo)
  /users → UserManagement (invite users, manage roles: Owner/Accountant/Manager/Staff)
  /chart-of-accounts → ChartOfAccountsSettings (customize default accounts)
  /payment-sources → PaymentSourceList (register POS/terminals/online)
    /new → PaymentSourceForm
    /:sourceId → PaymentSourceDetail
  /integrations → IntegrationSettings
    /connections → IntegrationConnectionList (Inventory Pro, Staff Pro, etc.)
    /connections/new → IntegrationConnectionForm
    /connections/:connId → IntegrationConnectionDetail
    /connections/:connId/webhooks → WebhookSubscriptionManager
  /invoice-template → InvoiceTemplateSettings (logo, colors, default text)
  /audit-log → AuditLogViewer (immutable financial audit trail)

* → NotFound / redirect to /dashboard
```

### Router Files (24 files — all .tsx)

| File | Routes |
|------|--------|
| `AuthRouter.tsx` | `/login`, `/register`, `/forgot-password`, `/reset-password` |
| `DashboardRouter.tsx` | `/dashboard` |
| `FlashReportRouter.tsx` | `/flash/*` |
| `SalesRouter.tsx` | `/sales/*`, `/payments/*`, `/cash-drawers/*` |
| `LedgerRouter.tsx` | `/ledger/*` |
| `JournalRouter.tsx` | `/journal/*` |
| `CloseDayRouter.tsx` | `/close-day/*` |
| `CloseMonthRouter.tsx` | `/close-month/*` |
| `BudgetRouter.tsx` | `/budgets/*` |
| `ReconciliationRouter.tsx` | `/reconciliation/*` |
| `DeliveryRouter.tsx` | `/delivery/*` |
| `InvoiceRouter.tsx` | `/invoices/*` |
| `CustomerRouter.tsx` | `/customers/*` |
| `GiftCardRouter.tsx` | `/gift-cards/*` |
| `ExpenseRouter.tsx` | `/expenses/*` |
| `VendorRouter.tsx` | `/vendors/*` |
| `FixedAssetRouter.tsx` | `/assets/*` |
| `LocationRouter.tsx` | `/locations/*` |
| `TaxRouter.tsx` | `/tax/*` |
| `AiToolsRouter.tsx` | `/ai/*` |
| `AlertRouter.tsx` | `/alerts/*` |
| `ReportsRouter.tsx` | `/reports/*` |
| `SettingsRouter.tsx` | `/settings/*` |

---

## 5. Redux Store Design

### Store Configuration

```typescript
const store = configureStore({
  reducer: {
    auth: authReducer,                     // OWN auth (register, login, JWT tokens)
    dashboard: dashboardReducer,           // KPI data, widget layout
    sales: salesReducer,                   // payments, daily summaries, tips
    ledger: ledgerReducer,                 // chart of accounts, account balances
    journal: journalReducer,              // journal entries and lines
    reconciliation: reconciliationReducer, // bank transactions, payouts, matches
    invoices: invoicesReducer,            // invoices, customers, AR
    expenses: expensesReducer,            // expenses, vendors, AP
    tax: taxReducer,                      // VAT rates, tax reports
    reports: reportsReducer,              // P&L, prime cost, analytics
    budget: budgetReducer,                 // budgets, budget lines, variance
    delivery: deliveryReducer,             // delivery platforms, payouts, commission tracking
    giftCards: giftCardReducer,            // gift cards, transactions, liability
    assets: assetsReducer,                 // fixed assets, depreciation
    locations: locationsReducer,           // multi-location management, comparison
    flashReport: flashReportReducer,       // daily flash report data
    alerts: alertsReducer,                 // variance alerts, scheduled reports
    ai: aiReducer,                         // OCR results, forecasts
    settings: settingsReducer,            // org settings, integrations
    ui: uiReducer,                        // sidebar collapsed, theme, dashboard widget layout
  },
});
```

**Redux Persist:** Persist `auth`, `settings`, and `ui` (dashboard widget layout, theme preference) slices.

### Slice Definitions

#### auth (own auth system — NOT shared with BonApp)

| State Property | Type | Description |
|---------------|------|-------------|
| `user` | object | User profile (name, email, role: Owner/Accountant/Manager/Staff) |
| `accessToken` | string | JWT Bearer token (from own backend) |
| `refreshToken` | string | Refresh token |
| `isAuthenticated` | bool | Auth status |
| `organizationId` | int | Current organization (restaurant) context |
| `organizationName` | string | Restaurant name |
| `organizations` | array | List of organizations user belongs to (for org switcher) |
| `lang` | string | Current language (en/de/fr/it) |
| `status` | string | idle / loading / succeeded / failed |
| `error` | string | Auth error message |

#### dashboard

| State Property | Type | Description |
|---------------|------|-------------|
| `kpis` | object | Today's KPIs: revenue, prime cost %, food cost %, labor cost %, net profit margin, cash position, outstanding AR, outstanding AP, reconciliation rate |
| `revenueChart` | array | Revenue data for chart (last 30 days / 12 months) |
| `primeCostTrend` | array | Prime cost trend data |
| `widgetLayout` | array | Drag-and-drop widget positions (persisted) |
| `alerts` | array | Active financial alerts (overdue invoices, missing payouts, cash variance) |
| `status` | string | Loading state |

#### sales

| State Property | Type | Description |
|---------------|------|-------------|
| `payments` | array | Paginated payment records |
| `selectedPayment` | object | Currently viewed payment detail |
| `dailySummaries` | array | Daily sales summaries for the period |
| `selectedDaySummary` | object | Currently viewed day detail |
| `cashDrawers` | array | Cash drawer list |
| `selectedCashDrawer` | object | Currently viewed drawer |
| `filters` | object | Active filters (date range, method, source, type) |
| `pagination` | object | Page, pageSize, totalCount |
| `status` | string | Loading state |

#### ledger

| State Property | Type | Description |
|---------------|------|-------------|
| `accounts` | array | Chart of accounts (tree structure) |
| `selectedAccount` | object | Currently viewed account with transaction history |
| `trialBalance` | object | Trial balance data (debit/credit totals per account) |
| `status` | string | Loading state |

#### journal

| State Property | Type | Description |
|---------------|------|-------------|
| `entries` | array | Paginated journal entries |
| `selectedEntry` | object | Currently viewed entry with lines |
| `filters` | object | Date range, source, status filters |
| `pagination` | object | Page, pageSize, totalCount |
| `status` | string | Loading state |

#### reconciliation

| State Property | Type | Description |
|---------------|------|-------------|
| `bankAccounts` | array | Registered bank accounts |
| `bankTransactions` | array | Imported bank transactions |
| `payouts` | array | Expected payouts from providers |
| `matches` | array | Reconciliation matches |
| `dashboard` | object | Expected, received, missing, unidentified totals |
| `importResult` | object | Last import result (success count, errors) |
| `autoMatchResults` | array | Pending auto-match results to review |
| `selectedBankTransaction` | object | Currently viewing |
| `selectedPayout` | object | Currently viewing |
| `filters` | object | Date range, status, bank account, provider |
| `status` | string | Loading state |

#### invoices

| State Property | Type | Description |
|---------------|------|-------------|
| `invoices` | array | Paginated invoice list |
| `selectedInvoice` | object | Currently viewed invoice with lines |
| `customers` | array | Customer / debtor list |
| `selectedCustomer` | object | Currently viewed customer with open invoices |
| `agingReport` | object | AR aging buckets (current, 30, 60, 90+) |
| `filters` | object | Status, customer, date range |
| `pagination` | object | Page, pageSize, totalCount |
| `status` | string | Loading state |

#### expenses

| State Property | Type | Description |
|---------------|------|-------------|
| `expenses` | array | Paginated expense list |
| `selectedExpense` | object | Currently viewed expense with attachments |
| `vendors` | array | Vendor / creditor list |
| `selectedVendor` | object | Currently viewed vendor with open bills |
| `agingReport` | object | AP aging buckets |
| `filters` | object | Category, vendor, status, date range |
| `pagination` | object | Page, pageSize, totalCount |
| `status` | string | Loading state |

#### tax

| State Property | Type | Description |
|---------------|------|-------------|
| `taxRates` | array | Swiss VAT rates (active + historical) |
| `taxReports` | array | Tax report periods |
| `selectedReport` | object | Currently viewed report detail (breakdown by rate) |
| `status` | string | Loading state |

#### reports

| State Property | Type | Description |
|---------------|------|-------------|
| `profitLoss` | object | P&L data with optional comparison period |
| `primeCost` | object | Prime cost analysis data |
| `revenueAnalytics` | object | Revenue breakdown data |
| `cashFlow` | object | Cash flow data + forecast |
| `balanceSheet` | object | Balance sheet data |
| `selectedReportType` | string | Which report is currently loaded |
| `dateRange` | object | Selected date range for reports |
| `comparisonDateRange` | object | Optional comparison period |
| `status` | string | Loading state |

#### settings

| State Property | Type | Description |
|---------------|------|-------------|
| `organization` | object | Organization settings (name, VAT, targets, fiscal year) |
| `paymentSources` | array | Registered payment sources |
| `integrations` | array | Integration connections |
| `users` | array | Organization users |
| `status` | string | Loading state |

#### budget

| State Property | Type | Description |
|---------------|------|-------------|
| `budgets` | array | List of budgets (name, year, status) |
| `selectedBudget` | object | Budget detail with all budget lines |
| `varianceReport` | object | Budget vs. actual variance data |
| `ytdSummary` | object | Year-to-date budget variance summary |
| `status` | string | Loading state |

#### delivery

| State Property | Type | Description |
|---------------|------|-------------|
| `platforms` | array | Registered delivery platforms |
| `selectedPlatform` | object | Platform detail with payout history |
| `payouts` | array | Delivery platform payouts |
| `summary` | object | Delivery revenue summary (gross, commission, net by platform) |
| `importResult` | object | Last CSV import result |
| `status` | string | Loading state |

#### giftCards

| State Property | Type | Description |
|---------------|------|-------------|
| `giftCards` | array | Paginated gift card list |
| `selectedGiftCard` | object | Gift card detail with transaction history |
| `liability` | object | Total outstanding gift card liability |
| `breakage` | object | Breakage calculation data |
| `filters` | object | Status, search filters |
| `pagination` | object | Page, pageSize, totalCount |
| `status` | string | Loading state |

#### assets

| State Property | Type | Description |
|---------------|------|-------------|
| `assets` | array | Fixed asset list |
| `selectedAsset` | object | Asset detail with depreciation schedule |
| `depreciationSchedule` | array | Full depreciation schedule for all assets |
| `depreciationPreview` | object | Preview of monthly depreciation run |
| `filters` | object | Category, location, status filters |
| `status` | string | Loading state |

#### locations

| State Property | Type | Description |
|---------------|------|-------------|
| `locations` | array | All locations for the organization |
| `selectedLocation` | object | Location detail |
| `currentLocationId` | int/null | Currently selected location context (null = all locations / consolidated) |
| `comparison` | object | Side-by-side location KPI comparison data |
| `consolidated` | object | Consolidated dashboard data |
| `status` | string | Loading state |

#### flashReport

| State Property | Type | Description |
|---------------|------|-------------|
| `todayFlash` | object | Today's live (incomplete) flash report |
| `selectedFlash` | object | Flash report for a selected date |
| `flashRange` | array | Flash reports for a date range (trend view) |
| `selectedDate` | string | Currently selected date |
| `status` | string | Loading state |

#### alerts

| State Property | Type | Description |
|---------------|------|-------------|
| `alerts` | array | Paginated list of variance alerts |
| `unreadCount` | int | Number of unread alerts (displayed on AlertBell) |
| `scheduledReports` | array | Configured scheduled report deliveries |
| `selectedAlert` | object | Currently viewed alert detail |
| `filters` | object | Type, severity, isRead filters |
| `status` | string | Loading state |

#### ai

| State Property | Type | Description |
|---------------|------|-------------|
| `ocrResult` | object | Latest OCR extraction result (vendor, lines, amounts, confidence) |
| `ocrHistory` | array | Previous OCR extractions |
| `cashFlowForecast` | object | AI cash flow prediction data (7/30/90 days) |
| `revenueForecast` | object | AI revenue prediction data (7/30/90 days) |
| `isProcessing` | bool | OCR processing in progress |
| `status` | string | Loading state |

#### ui

| State Property | Type | Description |
|---------------|------|-------------|
| `sidebarCollapsed` | bool | Sidebar state |
| `theme` | string | "light" / "dark" |
| `dashboardLayout` | array | Widget grid positions (persisted) |
| `compactMode` | bool | Dense table rows |
| `numberFormat` | object | Decimal places, thousand separator, currency position |
| `activeTab` | object | Per-page active tab tracking |

---

## 6. API Client Layer

### Axios Instance Configuration

```typescript
// src/api/client.ts
const financeApi = axios.create({
  baseURL: import.meta.env.VITE_FINANCE_API_URL, // e.g., "https://api.financepro.app"
  timeout: 30000,
  headers: { "Content-Type": "application/json" },
});

// Request interceptor — attach JWT token
financeApi.interceptors.request.use((config) => {
  const token = store.getState().auth.accessToken;
  if (token) config.headers.Authorization = `Bearer ${token}`;
  
  // Inject organizationId into URL (replace {orgId})
  const orgId = store.getState().auth.organizationId;
  if (orgId && config.url) {
    config.url = config.url.replace("{orgId}", String(orgId));
  }
  return config;
});

// Response interceptor — handle 401 (refresh token), 403 (redirect), 500 (toast)
financeApi.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      // Try refresh token, if fail → redirect to /login
    }
    if (error.response?.status === 403) {
      toast.error("You don't have permission for this action.");
    }
    if (error.response?.status >= 500) {
      toast.error("Server error. Please try again.");
    }
    return Promise.reject(error);
  }
);
```

### API Modules (24 dedicated modules)

| Module | File | Endpoints Covered |
|--------|------|-------------------|
| `authApi` | `src/api/auth.ts` | Login, register, refresh, forgot-password, reset-password |
| `accountsApi` | `src/api/accounts.ts` | Chart of accounts CRUD, trial balance |
| `journalApi` | `src/api/journal.ts` | Journal entries CRUD, post, void |
| `paymentsApi` | `src/api/payments.ts` | Payment records, daily summaries, manual entry |
| `disputesApi` | `src/api/disputes.ts` | Refunds, chargebacks |
| `closeDayApi` | `src/api/closeDay.ts` | Close-day preview + execute, reopen, close-month |
| `reconciliationApi` | `src/api/reconciliation.ts` | Bank accounts, import, transactions, payouts, matching |
| `invoicesApi` | `src/api/invoices.ts` | Invoice CRUD, PDF, send, payments, reminders |
| `customersApi` | `src/api/customers.ts` | Customer CRUD, AR aging |
| `expensesApi` | `src/api/expenses.ts` | Expense CRUD, approve/reject, attachments, import |
| `vendorsApi` | `src/api/vendors.ts` | Vendor CRUD, AP aging |
| `taxApi` | `src/api/tax.ts` | Tax rates, VAT reports, generate, export |
| `reportsApi` | `src/api/reports.ts` | P&L, prime cost, revenue, cash flow, balance sheet, exports |
| `dashboardApi` | `src/api/dashboard.ts` | KPI data, alerts, chart data |
| `integrationsApi` | `src/api/integrations.ts` | Connections, webhooks, payment sources |
| `settingsApi` | `src/api/settings.ts` | Org settings, users, audit log, fiscal periods |
| `flashReportApi` | `src/api/flashReport.ts` | Daily flash report generation, history, email delivery |
| `budgetApi` | `src/api/budget.ts` | Budget CRUD, budget lines, variance reports |
| `deliveryApi` | `src/api/delivery.ts` | Delivery platforms, commission tracking, payout import/reconciliation |
| `giftCardApi` | `src/api/giftCards.ts` | Gift card lifecycle, liability tracking, breakage |
| `assetsApi` | `src/api/assets.ts` | Fixed asset CRUD, depreciation schedule, run depreciation |
| `locationsApi` | `src/api/locations.ts` | Multi-location management, comparison, consolidated views |
| `alertsApi` | `src/api/alerts.ts` | Variance alerts, scheduled reports |
| `aiApi` | `src/api/ai.ts` | Invoice OCR, cash flow forecast, revenue forecast |

---

## 7. Component Architecture

### 7.1 Layout Components

| Component | Description |
|-----------|-------------|
| `AppLayout` | Main layout: collapsible sidebar + header bar + content area. ProLayout from `@ant-design/pro-components`. |
| `FinanceSidebar` | Navigation sidebar. **MVP sections:** Dashboard, Sales & Payments, Bookkeeping (Accounts, Journal Entries), Reconciliation, Invoicing, Expenses & Bills, Tax & Compliance, Reports, Settings. **Phase 2 additions:** Daily Flash, Close Month. **Phase 3 additions:** Budget, Delivery Platforms, Multi-Location, Alerts, AI Tools. **Phase 4 additions:** Gift Cards, Fixed Assets. Collapsible with icon-only mode. Active section highlighting. Grouped sections with dividers. Sidebar items for later phases are simply not rendered until the phase is built. |
| `HeaderBar` | Top bar: organization name, global search, user avatar (profile, language selector, theme toggle, logout). **Phase 3 additions:** LocationSwitcher (dropdown to switch between locations), AlertBell (notification bell with unread count). |
| `PageHeader` | Page title + breadcrumb + action buttons (e.g., "New Invoice", "Export PDF"). |
| `ContentWrapper` | Max-width content container with consistent padding. |

### 7.2 Dashboard Components

| Component | Description |
|-----------|-------------|
| `FinanceDashboard` | Main dashboard with drag-and-drop widget grid (`react-grid-layout`). Customizable layout persisted in Redux. |
| `KpiCard` | Single KPI display: value, label, trend arrow (up/down), comparison to target. Color-coded: green (on target), yellow (warning), red (above threshold). |
| `KpiRow` | Row of KPI cards: Daily Revenue, Prime Cost %, Food Cost %, Labor Cost %, Net Profit Margin, Cash Position. |
| `RevenueChart` | Line/bar chart: daily revenue for the last 30 days. Toggleable by payment method. |
| `PrimeCostTrendChart` | Line chart: prime cost % trend (daily/weekly) with target line. |
| `PaymentMethodPieChart` | Doughnut chart: revenue breakdown by payment method (card, cash, online, etc.). |
| `AlertsWidget` | List of active financial alerts: overdue invoices, missing payouts, cash variance, VAT report due. Click to navigate to detail. |
| `ReconciliationStatusWidget` | Mini reconciliation dashboard: expected/received/missing amounts with progress bar. |
| `OutstandingWidget` | AR (outstanding invoices) and AP (outstanding bills) summary with aging highlights. |
| `CashFlowMiniChart` | Small cash flow forecast chart (7-day projection). |
| `QuickActionsWidget` | Quick action buttons: Close Day, New Invoice, Record Expense, Import Statement. |

### 7.3 Sales & Payment Components

| Component | Description |
|-----------|-------------|
| `SalesOverview` | Revenue dashboard with date range selector. Totals by payment method. Chart + table view toggle. |
| `DailySalesCard` | Card for a single day: gross, net, refunds, tips, transaction count, average ticket. Click for detail. |
| `DailyDetail` | All transactions for a specific day. Grouped by payment source. Totals at bottom. |
| `PaymentList` | Paginated table of all payments. Columns: date, amount, method, provider, type, status, tip, order ref. Filterable, sortable, exportable. |
| `PaymentDetail` | Full payment detail: amount, VAT breakdown, linked journal entry, linked payout, refund history. |
| `ManualSalesEntryForm` | Form for standalone mode: enter daily totals with payment method breakdown, VAT split, tip amounts. Bulk entry for multiple days (table-style input). |
| `SalesImportWizard` | Step-by-step CSV import: upload → column mapping → preview → validate → import. |
| `CashDrawerPanel` | Cash drawer management: open drawer, record drops, close with count, variance display. |
| `TipSummaryTable` | Tips by period, payment method, and staff member. |

### 7.4 Bookkeeping Components

| Component | Description |
|-----------|-------------|
| `ChartOfAccountsTree` | Tree view of ledger accounts with account numbers, names, types, and balances. Expandable/collapsible hierarchy. Inline search. Swiss Kontenrahmen layout. |
| `LedgerAccountForm` | Create/edit account: number, name, type, parent account, default VAT rate. Validates Swiss account number conventions. |
| `AccountTransactionHistory` | Table of all journal lines for a specific account. Running balance column. Date range filter. |
| `JournalEntryList` | Paginated journal entries table: entry #, date, description, source (auto/manual), debit total, credit total, status. Color-coded by source type. |
| `JournalEntryForm` | Manual journal entry creation: date, description, dynamic debit/credit line rows. Add/remove lines. Real-time balance check (must be zero). Account search dropdown. |
| `JournalEntryDetail` | View entry with all lines in debit/credit T-account format. Source reference link. Audit info. Void button (Owner only). |
| `TrialBalance` | Two-column table: all accounts with debit balance and credit balance. Totals must match. As-of-date selector. |

### 7.5 Close Day/Month Components

| Component | Description |
|-----------|-------------|
| `CloseDayWizard` | Multi-step wizard for day closing. Steps: 1) Select date, 2) Review sales summary, 3) Verify cash drawer(s), 4) Preview journal entries to be created, 5) Confirm and close. Each step has a validation status (complete/incomplete/error). |
| `CloseDayPreview` | Preview page: shows all journal entries that will be auto-generated. Editable amounts for corrections before commit. Warnings for anomalies (unusually high/low revenue, cash variance). |
| `CloseDayHistory` | Table of past day closings with date, revenue total, status, who closed, when. Click to view details. |
| `CloseMonthWizard` | Multi-step: 1) Verify all days are closed, 2) Review unclosed days, 3) Accruals entry, 4) Preview monthly financial summary, 5) Close month. |

### 7.6 Reconciliation Components

| Component | Description |
|-----------|-------------|
| `ReconciliationDashboard` | Visual dashboard with four columns: Expected Payouts (from providers), Received (matched to bank), Missing (expected but not received), Unidentified (in bank but not expected). Summary amounts and counts. Drill-down on each column. |
| `BankStatementImportWizard` | Step-by-step import: 1) Select bank account + upload file, 2) Detect format (CSV type or CAMT.053), 3) Preview parsed transactions, 4) Confirm import. Column mapping for CSV. Error highlighting. |
| `BankTransactionList` | Table of imported bank transactions: date, amount, description, counterparty, status (matched/unmatched/identified). Filterable by bank account, date range, status. |
| `PayoutList` | Table of expected payouts: provider, amount, expected date, actual date, status. Color-coded status badges. |
| `AutoMatchResults` | Review page: list of auto-matched pairs (bank transaction ↔ payout/invoice) with confidence score. Accept/reject each match. Batch accept for exact matches. |
| `ManualMatchingWorkbench` | Split-screen UI: left panel = unmatched bank transactions, right panel = unmatched payouts/invoices. Click to select, click "Match" to create reconciliation match. Support split matching (one-to-many). |
| `ReconciliationTimeline` | Visual timeline showing settlement flow: sale → provider captures → payout issued → bank received → matched. Highlights where the chain breaks. |
| `PaymentProviderForm` | Register payment provider account: name, merchant ID, settlement bank account, typical settlement days, fee structure. |

### 7.7 Invoice Components

| Component | Description |
|-----------|-------------|
| `InvoiceList` | Table with status tabs: All, Draft, Sent, Overdue, Paid, Cancelled. Columns: invoice #, customer, date, due date, amount, paid, outstanding, status. Batch actions (send reminders for overdue). |
| `InvoiceForm` | Create/edit invoice: customer selector, invoice date, payment terms (auto-calculates due date), line items (description, qty, price, VAT rate). Live preview panel on the right. Running total with VAT breakdown. |
| `InvoiceDetail` | Full invoice view: header info, line items, payment history, reminder history. Actions: Send, Record Payment, Remind, Cancel, Download PDF, Print. |
| `InvoicePdfPreview` | Full-screen PDF preview using react-pdf. Shows the actual PDF that would be sent/printed. Includes Swiss QR-bill at the bottom. Print button. |
| `InvoiceLineEditor` | Reusable line item editor: add rows, remove rows, reorder. Each line: description input, quantity, unit price, VAT rate dropdown, line total (auto-calculated). |
| `CustomerSelect` | Searchable dropdown for customer selection. Shows recent customers. Option to create new inline. |
| `RecordPaymentModal` | Modal to record payment against invoice: amount, date, payment method, bank transaction reference. Partial payment support (remaining balance shown). |
| `ReminderModal` | Modal to send payment reminder: preview text, select reminder level (1st, 2nd, final), email or download PDF. |
| `AgingTable` | AR/AP aging table: rows = customers/vendors, columns = Current, 1-30, 31-60, 61-90, 90+, Total. Color-coded cells (older = redder). Click customer to see detail. |
| `QrBillPreview` | Swiss QR-bill component: displays the QR code, IBAN, reference, amount. Rendered on invoices. |

### 7.8 Expense Components

| Component | Description |
|-----------|-------------|
| `ExpenseList` | Table with filters: category, vendor, status (draft, pending approval, approved, posted), date range. Columns: date, vendor, category, amount, VAT, payment status, approval status. |
| `ExpenseForm` | Create/edit expense: date, vendor, category, account, amount, VAT rate, description. File upload zone (react-dropzone) for receipts/invoices. Recurring toggle with schedule. |
| `ExpenseDetail` | Full expense view: info, attachments (expandable image/PDF preview), approval timeline, linked journal entry. Approve/Reject buttons for Owner. |
| `ExpenseApprovalQueue` | List of expenses pending approval. Batch approve/reject. Quick preview (expand row). |
| `VendorSelect` | Searchable vendor dropdown with create-new inline. |
| `AttachmentViewer` | View uploaded files: image zoom, PDF preview, download. Delete option. |

### 7.9 Tax & Compliance Components

| Component | Description |
|-----------|-------------|
| `TaxOverview` | Summary: current period VAT liability, upcoming filing deadline, last filed report. Quick link to generate report. |
| `TaxRateTable` | List of VAT rates: name, rate %, type, valid from/to, active status. Historical rates shown in collapsed section. |
| `TaxReportGenerator` | Generate VAT report: select period type (quarterly/annual), date range, preview data, generate. Step-by-step wizard. |
| `TaxReportDetail` | Detailed VAT report: revenue by rate, output VAT totals, input VAT (from expenses), net payable. Exportable (CSV/PDF for ESTV). Formatted for Swiss MWST-Abrechnung. |
| `TaxReportComparison` | Side-by-side comparison of two tax periods. Highlights variances. |

### 7.10 Daily Flash Report Components (NEW — Industry's #1 Feature)

| Component | Description |
|-----------|-------------|
| `DailyFlashReport` | **The key daily view.** One-page operational snapshot for yesterday (or any selected date). Top row: Net Sales, Guest Count, Average Check. Second row: Food Cost %, Beverage Cost %, Labor Cost %, Prime Cost %. Each metric shows: value, % of sales, vs. same day last week (↑/↓), vs. same day last year (↑/↓). Color-coded: green if on/below target, red if above. Bottom: mini chart showing the metric trend for last 7 days. Auto-generated after close-day, emailed to managers at 6 AM. |
| `FlashReportHeader` | Date selector + location selector (for multi-location). "Email This Report" button. Print button. |
| `FlashKpiGrid` | Grid of KPI cards specifically for the flash report. 2 rows × 4 columns. Each card: metric name, value, % of sales, comparison arrows, color background. |
| `FlashComparisonTable` | Detailed table: rows = metrics (Revenue, COGS, Labor, Prime Cost, Controllable Profit, etc.), columns = Today, Same Day Last Week, Same Day Last Year, 7-Day Average, Budget. Variance columns with color coding. |
| `FlashTrendChart` | Small multi-line chart: last 30 days of flash report data. Toggleable lines for revenue, prime cost %, food cost %, labor cost %. Shows target lines for each metric. |
| `FlashReportTrendTable` | Multi-day view: rows = dates (last 30 days), columns = all flash metrics. Sparklines in each cell. Highlights outlier days in red/amber. Sort by any column. Export to CSV. |

### 7.11 Budget Components (NEW)

| Component | Description |
|-----------|-------------|
| `BudgetList` | Table of budgets: name, fiscal year, status (Draft/Approved/Locked), created by, actions. |
| `BudgetWizard` | Create budget: Step 1) Name + fiscal year + location. Step 2) Choose method: blank, copy from last year (+X% adjustment), or use industry benchmarks. Step 3) Review and save. |
| `BudgetEditor` | **Spreadsheet-style** budget line editor: rows = P&L accounts (Revenue, Food COGS, Beverage COGS, Labor, Rent, etc.), columns = Jan–Dec + Annual Total. Inline editing. Auto-sum. Percentage of revenue shown per cell. Color-coded cells based on industry benchmarks. Import from CSV. |
| `BudgetVarianceReport` | Budget vs. actual comparison: rows = P&L accounts, columns = Budgeted, Actual, Variance ($), Variance (%). YTD aggregation. Drill-down from variance to underlying transactions. Bar chart showing top 5 over-budget categories. |
| `BudgetVarianceMiniWidget` | Dashboard widget: this month's budget variance summary. Top 3 over-budget items highlighted. |

### 7.12 Delivery Platform Components (NEW)

| Component | Description |
|-----------|-------------|
| `DeliveryDashboard` | Overview: total delivery revenue (gross), total commissions paid, net delivery revenue, effective commission %, orders by platform. Pie chart: revenue share by platform. Bar chart: gross vs. net per platform. **Key insight card: "Your delivery platforms cost you X CHF this month (Y% of delivery revenue)"**. |
| `DeliveryPlatformList` | Table of registered platforms: name, commission %, active orders, last payout, status. |
| `DeliveryPlatformForm` | Register/edit platform: name, commission %, fixed fee, marketing fee, account ID. |
| `DeliveryPlatformDetail` | Platform detail: payout history, commission analysis, effective cost trend over time. |
| `DeliveryPayoutImport` | Import payout CSV from Uber Eats / DoorDash / Just Eat: upload → column mapping → preview → validate → import. Platform-specific templates. |
| `DeliveryReconciliation` | Match delivery platform payouts to bank deposits. Similar to payment reconciliation but specific to delivery platforms. |
| `DeliveryProfitabilityTable` | Table: rows = delivery platforms, columns = Gross Orders, Commission, Marketing Fee, Net Payout, Effective Cost %, Avg Commission Per Order. Compare: is delivery profitable after commissions? |

### 7.13 Gift Card Components (NEW)

| Component | Description |
|-----------|-------------|
| `GiftCardOverview` | Dashboard: total outstanding liability (all unredeemed balances), cards issued this month, cards redeemed this month, breakage revenue recognized. Donut chart: Active vs. Fully Redeemed vs. Expired. |
| `IssueGiftCardForm` | Issue new card: amount, purchaser, recipient, expiry (optional). Auto-generate card number. Print gift card receipt. |
| `GiftCardDetail` | Card detail: number, initial value, current balance, status, transaction history (issue, redeem, top-up, adjust). |
| `RedeemGiftCardForm` | Quick redeem: enter card number → show balance → enter redemption amount → confirm. Partial redemption supported. |
| `GiftCardLiabilityReport` | Accounting report: total cards issued, total redeemed, total outstanding, breakage estimate, aging of outstanding cards. |

### 7.14 Fixed Asset Components (NEW)

| Component | Description |
|-----------|-------------|
| `FixedAssetList` | Table: asset name, category, purchase date, purchase cost, accumulated depreciation, book value, status. Filter by category, location, status. |
| `FixedAssetForm` | Register asset: name, category, purchase date, cost, useful life, depreciation method, salvage value, location, vendor, serial number. |
| `FixedAssetDetail` | Asset detail with depreciation schedule table: month-by-month depreciation amount, accumulated depreciation, remaining book value. Chart: book value decline over time. |
| `RunDepreciationWizard` | Monthly depreciation: preview all assets to be depreciated, total depreciation amount, journal entries to be created. Confirm and run. |
| `DepreciationSchedule` | Full schedule for all assets: Gantt-style view showing asset useful life, current position, remaining months. Summary: total monthly depreciation, total book value. |

### 7.15 Multi-Location Components (NEW)

| Component | Description |
|-----------|-------------|
| `LocationSwitcher` | Dropdown in header bar: switch between locations or select "All Locations" for consolidated view. Persisted in Redux. All data throughout the app filters by selected location. |
| `LocationOverview` | Card grid of all locations: name, address, manager, today's revenue, prime cost %, status indicator (green/yellow/red based on KPIs). |
| `LocationComparison` | **The above-store dashboard.** Side-by-side KPI comparison table: rows = KPI metrics (revenue, food cost %, labor cost %, prime cost %, controllable profit, guest count, avg check), columns = locations. Color-coded cells: best performer highlighted green, worst in red. Rank column. Date range selector. |
| `ConsolidatedDashboard` | Rolled-up financials: total revenue across all locations, consolidated P&L, aggregate prime cost, total cash position, total AR/AP. Chart: revenue by location (stacked bar). Map view of locations with KPI indicators (if desired). |
| `LocationForm` | Create/edit location: name, code, address, manager, headquarters flag. |

### 7.16 AI Tools Components (NEW)

| Component | Description |
|-----------|-------------|
| `InvoiceOcrScanner` | **Upload supplier invoice photo/PDF → AI processes → review extracted data → confirm and create expense.** Step 1: drag-and-drop upload zone with camera capture option (mobile). Step 2: side-by-side view — original invoice image on left, extracted fields on right (vendor, date, amount, line items, VAT). User can correct any field. Step 3: confirm → auto-creates expense with attachment. Duplicate detection warning. |
| `OcrResultReview` | Review/correction form for OCR results: vendor (dropdown with suggestion), date, invoice number, line items (editable table), VAT rate, total. "Looks good" / "Needs correction" feedback for ML improvement. |
| `AiCashFlowForecast` | Interactive cash flow projection chart: historical data (solid line) + forecast (dashed line with confidence interval shading). Toggle 7/30/90 day view. Shows: projected low point, projected end balance, key inflection points. Table below: day-by-day forecast with expected inflows (revenue, collections) and outflows (bills, payroll, rent). Warning banner if cash is projected to go below safety threshold. |
| `AiRevenueForecast` | Revenue prediction: next 7/30/90 days based on historical patterns. Shows day-of-week patterns, seasonal adjustments, holiday effects. Comparison: last year same period vs. forecast. Confidence intervals. |

### 7.17 Alert & Notification Components (NEW)

| Component | Description |
|-----------|-------------|
| `AlertCenter` | Full-page alert management: list of all active alerts with severity badges (Info/Warning/Critical), alert type, message, timestamp. Filter by type, severity, location. Mark read, dismiss, or click to navigate to related entity. |
| `AlertBell` | Header notification bell with unread count badge. Click opens dropdown with latest 5 alerts. "View All" link to Alert Center. |
| `AlertSettingsForm` | Configure alert thresholds: prime cost % threshold, food cost % threshold, labor cost % threshold, cash variance tolerance, budget variance threshold, payout delay days. Per-location overrides. Enable/disable email notifications per alert type. |
| `ScheduledReportManager` | Manage automated report delivery: table of schedules with report type, frequency, delivery time, recipients, last sent. Create/edit/delete schedules. Test send button. |
| `ScheduledReportForm` | Create schedule: report type (Daily Flash, Weekly P&L, Monthly Financials, etc.), frequency (daily/weekly/monthly), delivery time, day of week/month, recipients (email list), format (PDF/CSV), location (specific or all). |

### 7.18 Report Components (Updated)

| Component | Description |
|-----------|-------------|
| `ReportsHub` | Card grid of all available reports with icons, descriptions, and "last generated" timestamp. Quick access to favorites. |
| `ProfitLossReport` | Standard P&L layout: Revenue → COGS → Gross Profit → Labor → Operating Expenses → Net Profit. Each line expandable to sub-accounts. Period comparison (side-by-side columns). Bar chart + waterfall chart visualization. |
| `PrimeCostReport` | Prime cost dashboard: COGS + Labor = Prime Cost. Trend chart with target line. Breakdown: food cost %, beverage cost %, labor cost %. Daily/weekly/monthly granularity toggle. Alert when exceeding threshold. |
| `RevenueAnalyticsReport` | Revenue breakdown: by payment method (pie), by day of week (bar), by time period (line), by category (treemap if POS provides order data). Comparison periods. |
| `CashFlowReport` | Cash flow statement: operating activities + investing + financing. Forward projection chart (7/30/90 days). Based on scheduled payments, expected collections, historical revenue. |
| `BalanceSheetReport` | Simplified balance sheet: Assets = Liabilities + Equity. Each section expandable to accounts. As-of-date selector. |
| `ArAgingReport` | Accounts Receivable aging: by customer, by aging bucket. Summary totals. Drill down to individual invoices. Export to CSV. |
| `ApAgingReport` | Accounts Payable aging: by vendor, by aging bucket. Summary totals. Drill down to individual bills. |
| `ReconciliationSummaryReport` | Reconciliation metrics: rate %, average settlement delay, outstanding by provider, monthly trend. |
| `BudgetVarianceReportPage` | Full budget vs. actual report: P&L format with Budget, Actual, Variance columns. YTD aggregation. Top-5 over-budget categories chart. Drill-through to transactions. |
| `ControllableProfitReport` | P&L split into two sections: **Controllable** (COGS, labor, direct OpEx, marketing, utilities) and **Non-Controllable** (rent, insurance, depreciation, interest). Controllable Profit highlighted as the key metric. Location comparison for multi-unit. |
| `DeliveryProfitabilityReport` | Delivery-specific P&L: Gross delivery revenue → Commission by platform → Net delivery revenue → COGS for delivery orders → **True delivery profit**. Is delivery worth it? Per-platform analysis. Trend over time. |
| `LocationComparisonReport` | Multi-location side-by-side: rows = KPIs, columns = locations. Sparklines, rank badges, color coding. Export for board/investor presentation. |
| `FlashReportTrendAnalysis` | Long-range flash report analysis: 30/90/365 day trend lines for all flash metrics. Moving averages. Seasonal pattern identification. Anomaly highlighting. |
| `ExportCenter` | Export hub: select export format (Abacus TAF, Sage CSV, generic CSV), date range, data type (journal entries, invoices, expenses). Download. |
| `ReportDateRangePicker` | Reusable date range component with presets: This Month, Last Month, This Quarter, Last Quarter, This Year, Last Year, Custom Range. Comparison period toggle. |
| `ReportExportBar` | Sticky bar at top of reports: export to CSV, PDF, print. |

### 7.19 Settings Components

| Component | Description |
|-----------|-------------|
| `GeneralSettings` | Form: fiscal year start month, prime cost target %, food cost target %, labor cost target %, default VAT rate, auto-close day toggle + time, default payment terms. |
| `OrganizationSettings` | Form: name, address, canton, VAT number (UID), logo upload, currency, timezone. |
| `UserManagement` | Table of organization users: name, email, role, last login, status. Invite new users by email. Edit role. Deactivate. |
| `PaymentSourceManager` | List of registered payment sources (POS systems, terminals, manual). Create/edit/deactivate. API key display (masked, with copy button). |
| `IntegrationConnectionManager` | Manage connections to Inventory Pro, Staff Pro, etc. Connection status indicator (last sync). API key rotation. Webhook configuration. |
| `InvoiceTemplateSettings` | Configure invoice appearance: logo, header text, footer text, default notes, bank details (IBAN), QR-bill settings. Live preview. |
| `AuditLogViewer` | Immutable audit trail: table with user, action, entity, timestamp, before/after values. Filterable. Cannot be edited/deleted. Export to CSV. |

### 7.20 Shared / Reusable Components

| Component | Description |
|-----------|-------------|
| `MoneyDisplay` | Formatted money display: right-aligned, Swiss locale (1'234.56 CHF), negative in red with parentheses. Configurable decimal places. |
| `MoneyInput` | Money input field: formatted with thousand separators, configurable decimal places, currency suffix. Validates non-negative (unless explicitly allowed). |
| `PercentDisplay` | Percentage display: colored based on threshold (green/yellow/red). Configurable thresholds per context. |
| `StatusBadge` | Colored badge for entity statuses: Draft (gray), Sent (blue), Paid (green), Overdue (red), Cancelled (dark gray), etc. |
| `DateRangePicker` | Ant Design RangePicker with custom presets: Today, This Week, This Month, Last Month, This Quarter, This Year, Custom. |
| `DataTable` | Base table component wrapping Ant Design Table: pagination, sorting, filtering, column visibility toggle, export button, print button. Virtual scrolling for large datasets. |
| `EmptyState` | Empty state illustration with call-to-action. E.g., "No invoices yet. Create your first invoice." |
| `ConfirmModal` | Confirmation dialog for destructive actions with severity levels (info, warning, danger). |
| `FileUploadZone` | Drag-and-drop file upload area using react-dropzone. Shows file preview, size, type validation. |
| `StepWizard` | Multi-step wizard wrapper: step indicators, next/back buttons, step validation, progress tracking. Used by Close Day, Import, Tax Report generation. |
| `AccountPicker` | Searchable ledger account selector with account number, name, and type. Tree hierarchy browsable. |
| `SearchInput` | Debounced search input with clear button. Used across all list pages. |
| `PrintLayout` | Wrapper component that applies print-optimized CSS. Hides navigation, headers. Used for reports and invoices. |
| `TrendIndicator` | Small arrow + percentage: "↑ 12.5%" (green) or "↓ 3.2%" (red). Used on KPI cards and comparison columns. |
| `OrganizationSwitcher` | Dropdown in header for users with multiple organizations. Switches context and reloads data. |
| `ThemeToggle` | Light/dark mode toggle in header. Persisted in Redux `ui` slice. |

---

## 8. Theming & Design System

### Ant Design 5 Theme Tokens

```typescript
// src/theme/financeTheme.ts
const financeTheme = {
  token: {
    // Brand colors — professional finance palette
    colorPrimary: "#1B4F72",           // Deep navy blue — trust, professionalism
    colorSuccess: "#27AE60",           // Green — positive numbers, paid status
    colorWarning: "#F39C12",           // Amber — warnings, approaching limits
    colorError: "#E74C3C",             // Red — negative numbers, overdue, errors
    colorInfo: "#3498DB",              // Blue — informational, links
    
    // Layout
    borderRadius: 6,
    fontSize: 13,                       // Slightly smaller for data density
    
    // Typography — clean financial readability
    fontFamily: "'Inter', -apple-system, BlinkMacSystemFont, sans-serif",
    fontSizeHeading1: 28,
    fontSizeHeading2: 22,
    fontSizeHeading3: 18,
    fontSizeHeading4: 15,
    
    // Table — compact for financial data
    // These are set via component tokens below
  },
  components: {
    Table: {
      cellPaddingBlock: 8,             // Compact rows
      cellPaddingInline: 12,
      headerBg: "#F8F9FA",
      headerColor: "#2C3E50",
      rowHoverBg: "#EBF5FB",
    },
    Card: {
      paddingLG: 16,
    },
    Menu: {
      itemHeight: 36,                   // Compact sidebar
    },
  },
};

// Dark mode overrides
const financeDarkTheme = {
  token: {
    ...financeTheme.token,
    colorBgContainer: "#1A1A2E",
    colorBgLayout: "#16213E",
    colorBgElevated: "#1A1A2E",
    colorText: "#E8E8E8",
    colorTextSecondary: "#A0A0A0",
  },
};
```

### Financial Color System

| Color | Hex | Usage |
|-------|-----|-------|
| **Navy** | `#1B4F72` | Primary brand, headers, sidebar |
| **Success Green** | `#27AE60` | Positive amounts, "Paid" status, on-target KPIs |
| **Warning Amber** | `#F39C12` | "Partially Paid", approaching thresholds, "Pending" |
| **Error Red** | `#E74C3C` | Negative amounts, "Overdue" status, above-threshold KPIs |
| **Info Blue** | `#3498DB` | Links, "Sent" status, informational badges |
| **Neutral Gray** | `#95A5A6` | "Draft" status, disabled states, secondary text |
| **Dark Slate** | `#2C3E50` | Text, headings |
| **Light BG** | `#F8F9FA` | Table headers, card backgrounds |

### Number Formatting Rules

| Rule | Example | Implementation |
|------|---------|---------------|
| **Swiss thousands separator** | 1'234'567.89 | `Intl.NumberFormat('de-CH')` |
| **Currency suffix** | 1'234.56 CHF | Custom MoneyDisplay component |
| **Negative amounts** | (1'234.56) in red | Parentheses + `color: #E74C3C` |
| **Percentages** | 32.5% | One decimal place, colored by threshold |
| **Right-aligned numbers** | All numeric columns | CSS `text-align: right` |
| **Consistent decimals** | Always 2 decimal places for money | `minimumFractionDigits: 2, maximumFractionDigits: 2` |

---

## 9. Key Page Wireframes (Detailed)

### 9.1 Daily Flash Report (NEW — The #1 Screen)

```
┌──────────────────────────────────────────────────────────────────────┐
│ Daily Flash Report — February 8, 2026    [← Prev] [Next →]         │
│ Restaurant ABC — Downtown Zürich    [📧 Email Report] [🖨 Print]   │
│                                                                      │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐               │
│  │ NET SALES│ │ GUESTS   │ │ AVG CHECK│ │ TIPS     │               │
│  │ 3'720.50 │ │    87    │ │  42.76   │ │  285.00  │               │
│  │ ↑8% vs LW│ │ ↑12% LW │ │ ↓3% LW  │ │ ↑5% LW  │               │
│  │ ↑15% vs LY│ │ ↑18% LY │ │ ↓2% LY  │ │ ↑8% LY  │               │
│  └──────────┘ └──────────┘ └──────────┘ └──────────┘               │
│                                                                      │
│  ┌─── Cost Analysis ─────────────────────────────────────────────┐  │
│  │              │ Today    │ % Sales │ vs LW  │ vs LY  │ Target │  │
│  │──────────────┼──────────┼─────────┼────────┼────────┼────────│  │
│  │ Food Cost    │   980.00 │  26.3%  │ ↓ 1.2% │ ↓ 0.8% │ 30%  │  │
│  │ Bev Cost     │   320.00 │   8.6%  │ ↑ 0.3% │ → 0.0% │ 20%  │  │
│  │ Total COGS   │ 1'300.00 │  34.9%  │ ↓ 0.9% │ ↓ 0.8% │      │  │
│  │ Labor Cost   │   890.00 │  23.9%  │ ↓ 1.5% │ ↓ 2.1% │ 30%  │  │
│  │──────────────┼──────────┼─────────┼────────┼────────┼────────│  │
│  │ ★ PRIME COST │ 2'190.00 │  58.9%  │ ↓ 2.4% │ ↓ 2.9% │ 60%  │  │
│  │ Controllable │ 2'690.00 │  72.3%  │ ↓ 1.8% │ ↓ 2.2% │      │  │
│  │ Ctrl Profit  │ 1'030.50 │  27.7%  │ ↑ 1.8% │ ↑ 2.2% │      │  │
│  └────────────────────────────────────────────────────────────────┘  │
│                                                                      │
│  ┌─── 7-Day Trend ──────────────────┐ ┌─── Notes ────────────────┐  │
│  │ [Sparkline charts for revenue,   │ │ ✅ Cash variance: 0 CHF  │  │
│  │  prime cost %, food cost %,      │ │ ⚠ Revenue 18% above avg  │  │
│  │  labor cost %]                   │ │ ✅ All payments reconciled│  │
│  └──────────────────────────────────┘ └───────────────────────────┘  │
└──────────────────────────────────────────────────────────────────────┘
```

### 9.2 Finance Dashboard

```
┌──────────────────────────────────────────────────────────────────────┐
│ [☰] Finance Pro     [🏠 Restaurant ABC ▾]  [🔔 3]  [JD ▾]  [🌙]  │
├──────────┬───────────────────────────────────────────────────────────┤
│          │  Dashboard                            [Customize] [↻]   │
│ Dashboard│                                                          │
│          │  ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐ │
│ Sales    │  │Today │ │Prime │ │Food  │ │Labor │ │Net   │ │Cash  │ │
│          │  │Rev   │ │Cost  │ │Cost  │ │Cost  │ │Profit│ │Pos.  │ │
│ Book-    │  │3'245 │ │58.2% │ │30.1% │ │28.1% │ │12.5% │ │45'230│ │
│ keeping  │  │↑ 12% │ │↓ 1.2 │ │→ 0.1 │ │↓ 1.1 │ │↑ 2.3 │ │      │ │
│          │  └──────┘ └──────┘ └──────┘ └──────┘ └──────┘ └──────┘ │
│ Reconcil.│                                                          │
│          │  ┌─── Revenue (30 days) ──────┐ ┌── Payment Methods ──┐ │
│ Invoices │  │ [Line chart with bars]     │ │ [Doughnut chart]    │ │
│          │  │                            │ │ Card: 62%           │ │
│ Expenses │  │                            │ │ Cash: 24%           │ │
│          │  │                            │ │ Online: 14%         │ │
│ Tax      │  └────────────────────────────┘ └─────────────────────┘ │
│          │                                                          │
│ Reports  │  ┌─── Alerts (3) ──────────┐ ┌── Reconciliation ──────┐│
│          │  │ ⚠ INV-2026-0042 overdue │ │ Expected: 12'450 CHF  ││
│ Settings │  │ ⚠ Payout missing: WL-33 │ │ Received: 11'200 CHF  ││
│          │  │ ⚠ Cash variance: -45 CHF│ │ Missing:   1'250 CHF  ││
│          │  └─────────────────────────┘ │ [━━━━━━━━━░░] 89.9%   ││
│          │                               └────────────────────────┘│
└──────────┴───────────────────────────────────────────────────────────┘
```

### 9.3 Close Day Wizard

```
┌──────────────────────────────────────────────────────────────────────┐
│ Close Day — February 8, 2026                                        │
│                                                                      │
│  ① Select Date  ━━  ② Sales Summary  ━━  ③ Cash Drawers  ━━        │
│  ④ Preview Entries  ━━  ⑤ Confirm                                   │
│                                                                      │
│  ┌─────────────────────────────────────────────────────────────────┐ │
│  │ Step 2: Sales Summary                                          │ │
│  │                                                                 │ │
│  │  Gross Revenue ................ 3'845.50 CHF                   │ │
│  │  Refunds ......................   (125.00) CHF                  │ │
│  │  Net Revenue .................. 3'720.50 CHF                   │ │
│  │                                                                 │ │
│  │  By Payment Method:                                             │ │
│  │    Card (Worldline) ........... 2'340.00 CHF  (62.9%)         │ │
│  │    Cash ....................... 1'005.50 CHF  (27.0%)          │ │
│  │    Twint ......................   375.00 CHF  (10.1%)          │ │
│  │                                                                 │ │
│  │  Tips: 285.00 CHF (Card: 195.00 | Cash: 90.00)               │ │
│  │                                                                 │ │
│  │  Transactions: 87 | Average Ticket: 42.76 CHF                 │ │
│  │                                                                 │ │
│  │  VAT Breakdown:                                                 │ │
│  │    8.1% (standard) ..... 2'890.50 net / 234.13 VAT            │ │
│  │    2.6% (reduced) ......   830.00 net /  21.58 VAT            │ │
│  │                                                                 │ │
│  │  ⚠ Revenue is 18% above 30-day average. Please verify.       │ │
│  └─────────────────────────────────────────────────────────────────┘ │
│                                                                      │
│  [← Back]                                              [Next Step →] │
└──────────────────────────────────────────────────────────────────────┘
```

### 9.4 Reconciliation Matching Workbench

```
┌──────────────────────────────────────────────────────────────────────┐
│ Reconciliation — Manual Matching                                     │
│                                                                      │
│  ┌─── Bank Transactions (Unmatched) ──┐ ┌── Expected Payouts ─────┐ │
│  │ ☐ Feb 7 | +3'150.00 | WORLDLINE   │ │ ☑ Payout WL-0207       │ │
│  │          | Ref: WL-BATCH-0207      │ │   Amount: 3'200.00     │ │
│  │                                     │ │   Provider: Worldline  │ │
│  │ ☐ Feb 7 | +1'245.80 | STRIPE      │ │   Expected: Feb 7      │ │
│  │          | Ref: pi_3Ox...          │ │   Fees: 50.00          │ │
│  │                                     │ │   Net: 3'150.00       │ │
│  │ ☐ Feb 8 | +450.00   | UNKNOWN     │ │                         │ │
│  │          | Ref: 029384756          │ │ ☐ Payout STR-0207      │ │
│  │                                     │ │   Amount: 1'245.80    │ │
│  │ ☐ Feb 8 | -89.50    | BANK FEE    │ │   Provider: Stripe     │ │
│  │          | Monthly account fee     │ │   Expected: Feb 8      │ │
│  └─────────────────────────────────────┘ └─────────────────────────┘ │
│                                                                      │
│  Selected: 1 bank transaction, 1 payout                              │
│  Bank: +3'150.00 CHF  ←→  Payout net: 3'150.00 CHF  ✓ Exact match │
│                                                                      │
│  [Match Selected]  [Mark Bank Txn as Identified]  [Skip]            │
└──────────────────────────────────────────────────────────────────────┘
```

### 9.5 Invoice Form

```
┌──────────────────────────────────────────────────────────────────────┐
│ New Invoice                                        [Save Draft] [✕] │
│                                                                      │
│  ┌─── Invoice Details ──────────────┐ ┌── Live Preview ───────────┐ │
│  │                                   │ │  ┌────────────────────┐  │ │
│  │ Customer: [Zurich Events AG ▾]   │ │  │ [LOGO]             │  │ │
│  │ Invoice Date: [09/02/2026]       │ │  │ Restaurant ABC      │  │ │
│  │ Payment Terms: [Net 30 ▾]       │ │  │ Bahnhofstr. 42      │  │ │
│  │ Due Date: 11/03/2026 (auto)     │ │  │ 8001 Zürich         │  │ │
│  │                                   │ │  │                     │  │ │
│  │ Line Items:                       │ │  │ INVOICE INV-2026-15 │  │ │
│  │ ┌──┬────────────┬───┬──────┬───┐ │ │  │ Date: 09/02/2026   │  │ │
│  │ │# │Description │Qty│ Price│VAT│ │ │  │                     │  │ │
│  │ ├──┼────────────┼───┼──────┼───┤ │ │  │ Catering Lunch  x40│  │ │
│  │ │1 │Catering    │ 40│ 35.00│8.1│ │ │  │ 1'400.00 CHF       │  │ │
│  │ │  │Corporate   │   │      │ % │ │ │  │                     │  │ │
│  │ │  │Lunch       │   │      │   │ │ │  │ Subtotal: 1'400.00 │  │ │
│  │ ├──┼────────────┼───┼──────┼───┤ │ │  │ VAT 8.1%:   113.40 │  │ │
│  │ │2 │Delivery    │  1│ 50.00│8.1│ │ │  │ Delivery:     50.00│  │ │
│  │ │  │            │   │      │ % │ │ │  │ Total:    1'563.40  │  │ │
│  │ └──┴────────────┴───┴──────┴───┘ │ │  │                     │  │ │
│  │ [+ Add Line]                      │ │  │ ┌─── QR-Bill ────┐ │  │ │
│  │                                   │ │  │ │ [QR Code]      │ │  │ │
│  │ Notes: [                        ]│ │  │ │ CH12 3456 7890 │ │  │ │
│  │                                   │ │  │ │ 1234 5         │ │  │ │
│  │ Subtotal:   1'450.00 CHF        │ │  │ └────────────────┘ │  │ │
│  │ VAT (8.1%):   117.45 CHF        │ │  └────────────────────┘  │ │
│  │ Total:      1'567.45 CHF        │ │                           │ │
│  └───────────────────────────────────┘ └───────────────────────────┘ │
│                                                                      │
│  [Save as Draft]  [Save & Send by Email]  [Save & Download PDF]     │
└──────────────────────────────────────────────────────────────────────┘
```

### 9.6 Profit & Loss Report

```
┌──────────────────────────────────────────────────────────────────────┐
│ Profit & Loss                  [This Month ▾] vs [Last Month ▾]     │
│                                [Export CSV] [Export PDF] [Print]     │
│                                                                      │
│  ┌────────────────────────────────────────────────────────────────┐  │
│  │                         │  Feb 2026  │  Jan 2026  │  Change   │  │
│  │─────────────────────────┼────────────┼────────────┼───────────│  │
│  │ REVENUE                 │            │            │           │  │
│  │  ▸ Food Revenue         │  52'340.00 │  48'120.00 │  ↑  8.8% │  │
│  │  ▸ Beverage Revenue     │  18'450.00 │  17'890.00 │  ↑  3.1% │  │
│  │  ▸ Catering Revenue     │   4'200.00 │   2'800.00 │  ↑ 50.0% │  │
│  │    Less: Refunds        │    (920.00)│    (650.00)│  ↑ 41.5% │  │
│  │ NET REVENUE             │  74'070.00 │  68'160.00 │  ↑  8.7% │  │
│  │─────────────────────────┼────────────┼────────────┼───────────│  │
│  │ COST OF GOODS SOLD      │            │            │           │  │
│  │  ▸ Food Purchases       │  16'500.00 │  15'200.00 │  ↑  8.6% │  │
│  │  ▸ Beverage Purchases   │   3'200.00 │   3'100.00 │  ↑  3.2% │  │
│  │    Inventory Change     │    (500.00)│    (200.00)│           │  │
│  │ TOTAL COGS              │  19'200.00 │  18'100.00 │  ↑  6.1% │  │
│  │ COGS %                  │     25.9%  │     26.6%  │  ↓  0.7  │  │
│  │─────────────────────────┼────────────┼────────────┼───────────│  │
│  │ GROSS PROFIT            │  54'870.00 │  50'060.00 │  ↑  9.6% │  │
│  │─────────────────────────┼────────────┼────────────┼───────────│  │
│  │ LABOR                   │            │            │           │  │
│  │  ▸ Wages & Salaries     │  18'000.00 │  17'500.00 │  ↑  2.9% │  │
│  │  ▸ Social Security      │   2'800.00 │   2'720.00 │  ↑  2.9% │  │
│  │  ▸ Other Personnel      │   1'200.00 │   1'100.00 │  ↑  9.1% │  │
│  │ TOTAL LABOR             │  22'000.00 │  21'320.00 │  ↑  3.2% │  │
│  │ LABOR %                 │     29.7%  │     31.3%  │  ↓  1.6  │  │
│  │─────────────────────────┼────────────┼────────────┼───────────│  │
│  │ ★ PRIME COST            │  41'200.00 │  39'420.00 │  ↑  4.5% │  │
│  │ ★ PRIME COST %          │     55.6%  │     57.8%  │  ↓  2.2  │  │
│  │─────────────────────────┼────────────┼────────────┼───────────│  │
│  │ OPERATING EXPENSES      │            │            │           │  │
│  │  ▸ Rent                 │   5'500.00 │   5'500.00 │     0.0% │  │
│  │  ▸ Utilities            │   1'200.00 │   1'350.00 │  ↓ 11.1% │  │
│  │  ▸ Insurance            │     800.00 │     800.00 │     0.0% │  │
│  │  ▸ Marketing            │   1'500.00 │     900.00 │  ↑ 66.7% │  │
│  │  ▸ Other                │   2'100.00 │   1'950.00 │  ↑  7.7% │  │
│  │ TOTAL OPEX              │  11'100.00 │  10'500.00 │  ↑  5.7% │  │
│  │═════════════════════════╪════════════╪════════════╪═══════════│  │
│  │ NET OPERATING PROFIT    │  21'770.00 │  18'240.00 │  ↑ 19.4% │  │
│  │ NET PROFIT MARGIN       │     29.4%  │     26.8%  │  ↑  2.6  │  │
│  └────────────────────────────────────────────────────────────────┘  │
│                                                                      │
│  ┌─── Visual Breakdown ───────────────────────────────────────────┐  │
│  │ [Waterfall chart: Revenue → -COGS → -Labor → -OpEx → Profit] │  │
│  └────────────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────────────┘
```

---

## 10. Offline & Performance

### 10.1 Caching Strategy

| Data | Cache Strategy | TTL |
|------|---------------|-----|
| **Chart of Accounts** | Redux + localStorage | Until org settings change |
| **Tax Rates** | Redux + localStorage | 24 hours |
| **Dashboard KPIs** | Redux only | 5 minutes (auto-refresh) |
| **Payment list** | Redux only | Per-session (refetch on filter change) |
| **Reports** | Redux only | Per-generation (explicit refresh) |

### 10.2 Performance Optimizations

| Technique | Where Applied |
|-----------|---------------|
| **Virtual scrolling** | Journal entry list, bank transaction list, payment list (can have 10K+ rows) |
| **Pagination** | All list pages (server-side pagination, 25/50/100 per page) |
| **Debounced search** | 300ms debounce on search inputs |
| **Lazy loading** | Report pages loaded on demand (React.lazy + Suspense) |
| **Memoization** | Heavy calculations (P&L aggregation, aging buckets) memoized with useMemo |
| **Chart optimization** | Chart.js decimation plugin for large datasets (daily data over years) |
| **Print stylesheet** | Separate CSS for @media print — hides nav, optimizes layout |

### 10.3 Code Splitting

```
Route-based splitting (React.lazy + Suspense):
/flash/*                     → FlashReportChunk        (daily flash report — fastest load priority)
/dashboard                   → DashboardChunk
/sales/* + /payments/* + /cash-drawers/* → SalesChunk
/ledger/* + /journal/* + /close-day/* + /close-month/*  → BookkeepingChunk
/reconciliation/*            → ReconciliationChunk
/invoices/* + /customers/*   → InvoicingChunk
/budgets/*                   → BudgetChunk
/delivery/*                  → DeliveryChunk
/gift-cards/*                → GiftCardChunk
/expenses/* + /vendors/*     → ExpensesChunk
/assets/*                    → FixedAssetChunk
/tax/*                       → TaxChunk
/locations/*                 → LocationChunk
/ai/*                        → AiToolsChunk
/alerts/*                    → AlertsChunk
/reports/*                   → ReportsChunk
/settings/*                  → SettingsChunk

Total: 17 lazy-loaded chunks (initial bundle ~120 KB gzipped: React + Ant Design core + Redux + router)
```

---

## 11. Internationalization (i18n)

### Supported Languages

| Language | Code | Usage |
|----------|------|-------|
| **English** | `en` | Default fallback. Primary for international users. |
| **German** | `de` | German-speaking Switzerland (Zurich, Bern, Basel, Lucerne) |
| **French** | `fr` | French-speaking Switzerland (Geneva, Lausanne, Fribourg) |
| **Italian** | `it` | Italian-speaking Switzerland (Ticino, Graubünden) |

### Translation Namespaces

| Namespace | Content |
|-----------|---------|
| `common` | Shared terms: Save, Cancel, Delete, Back, Next, Loading, Error |
| `auth` | Login, Register, Forgot Password, Reset Password |
| `dashboard` | KPI labels, widget titles, alert messages |
| `sales` | Payment methods, payment types, daily summary labels |
| `ledger` | Account types, journal entry sources, trial balance |
| `reconciliation` | Match types, payout statuses, reconciliation terms |
| `invoices` | Invoice statuses, reminder levels, aging buckets |
| `expenses` | Expense categories, approval statuses, vendor terms |
| `tax` | VAT rate names, tax report labels, ESTV terminology |
| `reports` | Report names, P&L line items, KPI definitions |
| `flash` | Flash report labels, comparison terms, trend descriptions |
| `budget` | Budget terms, variance labels, budget status |
| `delivery` | Platform names, commission terms, delivery reconciliation |
| `giftCards` | Gift card lifecycle terms, breakage, liability |
| `assets` | Fixed asset terms, depreciation, disposal |
| `locations` | Location terms, comparison labels, consolidated views |
| `alerts` | Alert types, severity levels, threshold descriptions |
| `settings` | Setting labels, user roles, integration types |

### Financial Term Localization

| English | German | French | Italian |
|---------|--------|--------|---------|
| Revenue | Umsatz | Chiffre d'affaires | Fatturato |
| Cost of Goods Sold | Warenaufwand | Coût des marchandises | Costo del venduto |
| Gross Profit | Bruttogewinn | Bénéfice brut | Utile lordo |
| Prime Cost | Hauptkosten | Coût principal | Costo primo |
| VAT | MWST | TVA | IVA |
| Invoice | Rechnung | Facture | Fattura |
| Accounts Receivable | Debitoren | Débiteurs | Debitori |
| Accounts Payable | Kreditoren | Créanciers | Creditori |
| Balance Sheet | Bilanz | Bilan | Bilancio |
| Profit & Loss | Erfolgsrechnung | Compte de résultat | Conto economico |

---

## 12. Testing Strategy

### 12.1 Unit Tests (Vitest + Testing Library)

| Test Area | Examples |
|-----------|---------|
| **MoneyDisplay** | Correct Swiss formatting. Negative amounts in parentheses. Currency suffix. |
| **PercentDisplay** | Correct threshold coloring. Edge cases (0%, 100%, negative). |
| **VatCalculation** | Client-side VAT calculations match expected. Multi-rate support. |
| **AgingBuckets** | Correct bucket allocation. Edge cases (due today, future). |
| **JournalEntryForm** | Debit/credit balance validation. Add/remove lines. Account selection. |
| **InvoiceLineEditor** | Add lines, calculate totals, VAT per line, remove lines. |
| **DateRangePicker** | Preset values. Custom range. Comparison period toggle. |
| **Redux slices** | Each reducer handles all action types correctly (all 20 slices). |
| **Thunks** | API calls with correct parameters. Success and error handling. |
| **FlashKpiGrid** | Correct KPI rendering. Color-coded thresholds. Comparison arrows. |
| **BudgetEditor** | Spreadsheet-style editing. Auto-sum. Tab navigation. |
| **LocationSwitcher** | Switching location updates Redux. "All Locations" shows consolidated. |
| **AlertBell** | Unread count badge. Dropdown shows latest alerts. |
| **DeliveryProfitabilityTable** | Commission calculation. Effective cost %. Platform comparison. |
| **GiftCardLiabilityReport** | Liability total. Breakage estimate. Aging of outstanding cards. |

### 12.2 Integration / E2E Tests (Playwright)

| Test Flow | Steps |
|-----------|-------|
| **Login → Dashboard** | Login with credentials → verify KPI cards load → verify charts render |
| **Create Invoice** | Navigate → fill form → add lines → save draft → send → verify status change |
| **Close Day** | Navigate → select date → review summary → verify cash → preview entries → confirm → verify lock |
| **Import Bank Statement** | Upload CSV → verify preview → confirm import → verify transactions appear |
| **Manual Reconciliation** | Select bank transaction → select payout → match → verify status update |
| **P&L Report** | Select date range → verify totals → compare periods → export CSV |
| **Expense Approval** | Staff creates expense → Manager sees in queue → Owner approves → verify posted |
| **Daily Flash Report** | Navigate to /flash → verify all KPIs render → verify comparison arrows → verify 7-day trend chart |
| **Budget Variance** | Navigate to budget → verify budget vs. actual → check color coding → drill through to transactions |
| **Invoice OCR** | Upload invoice image → verify OCR fields populated → correct a field → confirm → verify expense created |
| **Location Switching** | Switch location in header → verify dashboard KPIs change → verify P&L filters by location |
| **Gift Card Redeem** | Navigate to /gift-cards/redeem → enter card number → verify balance → redeem partial amount → verify new balance |
| **Delivery Dashboard** | Navigate → verify gross/commission/net by platform → verify profitability insight card |

---

## 13. Project Structure

```
finance-pro-frontend/
├── public/
│   ├── favicon.ico
│   ├── logo.svg
│   └── locales/
│       ├── en/
│       │   ├── common.json
│       │   ├── auth.json
│       │   ├── dashboard.json
│       │   ├── sales.json
│       │   ├── ledger.json
│       │   ├── reconciliation.json
│       │   ├── invoices.json
│       │   ├── expenses.json
│       │   ├── tax.json
│       │   ├── reports.json
│       │   ├── flash.json
│       │   ├── budget.json
│       │   ├── delivery.json
│       │   ├── giftCards.json
│       │   ├── assets.json
│       │   ├── locations.json
│       │   ├── alerts.json
│       │   └── settings.json
│       ├── de/ (same 18 files)
│       ├── fr/ (same 18 files)
│       └── it/ (same 18 files)
│
├── src/
│   ├── main.tsx                    — Entry point
│   ├── App.tsx                     — Root component (providers, router)
│   ├── vite-env.d.ts
│   │
│   ├── api/                        — API client modules (24 files)
│   │   ├── client.ts               — Axios instance + interceptors
│   │   ├── auth.ts
│   │   ├── accounts.ts
│   │   ├── journal.ts
│   │   ├── payments.ts
│   │   ├── disputes.ts
│   │   ├── closeDay.ts
│   │   ├── reconciliation.ts
│   │   ├── invoices.ts
│   │   ├── customers.ts
│   │   ├── expenses.ts
│   │   ├── vendors.ts
│   │   ├── tax.ts
│   │   ├── reports.ts
│   │   ├── dashboard.ts
│   │   ├── integrations.ts
│   │   ├── settings.ts
│   │   ├── flashReport.ts
│   │   ├── budget.ts
│   │   ├── delivery.ts
│   │   ├── giftCards.ts
│   │   ├── assets.ts
│   │   ├── locations.ts
│   │   ├── alerts.ts
│   │   └── ai.ts
│   │
│   ├── store/                      — Redux store
│   │   ├── index.ts                — Store configuration
│   │   ├── hooks.ts                — Typed useAppSelector, useAppDispatch
│   │   └── slices/
│   │       ├── authSlice.ts
│   │       ├── dashboardSlice.ts
│   │       ├── salesSlice.ts
│   │       ├── ledgerSlice.ts
│   │       ├── journalSlice.ts
│   │       ├── reconciliationSlice.ts
│   │       ├── invoicesSlice.ts
│   │       ├── expensesSlice.ts
│   │       ├── taxSlice.ts
│   │       ├── reportsSlice.ts
│   │       ├── budgetSlice.ts
│   │       ├── deliverySlice.ts
│   │       ├── giftCardSlice.ts
│   │       ├── assetsSlice.ts
│   │       ├── locationsSlice.ts
│   │       ├── flashReportSlice.ts
│   │       ├── alertsSlice.ts
│   │       ├── aiSlice.ts
│   │       ├── settingsSlice.ts
│   │       └── uiSlice.ts
│   │
│   ├── router/                     — Route definitions (24 files)
│   │   ├── index.tsx               — Root router with lazy loading
│   │   ├── AuthRouter.tsx
│   │   ├── DashboardRouter.tsx
│   │   ├── SalesRouter.tsx
│   │   ├── LedgerRouter.tsx
│   │   ├── JournalRouter.tsx
│   │   ├── CloseDayRouter.tsx
│   │   ├── CloseMonthRouter.tsx
│   │   ├── ReconciliationRouter.tsx
│   │   ├── InvoiceRouter.tsx
│   │   ├── CustomerRouter.tsx
│   │   ├── ExpenseRouter.tsx
│   │   ├── VendorRouter.tsx
│   │   ├── TaxRouter.tsx
│   │   ├── FlashReportRouter.tsx
│   │   ├── BudgetRouter.tsx
│   │   ├── DeliveryRouter.tsx
│   │   ├── GiftCardRouter.tsx
│   │   ├── FixedAssetRouter.tsx
│   │   ├── LocationRouter.tsx
│   │   ├── AiToolsRouter.tsx
│   │   ├── AlertRouter.tsx
│   │   ├── ReportsRouter.tsx
│   │   └── SettingsRouter.tsx
│   │
│   ├── pages/                      — Page components (organized by domain)
│   │   ├── auth/
│   │   │   ├── LoginPage.tsx
│   │   │   ├── RegisterPage.tsx
│   │   │   ├── ForgotPasswordPage.tsx
│   │   │   └── ResetPasswordPage.tsx
│   │   ├── dashboard/
│   │   │   └── FinanceDashboard.tsx
│   │   ├── sales/
│   │   │   ├── SalesOverview.tsx
│   │   │   ├── DailyDetail.tsx
│   │   │   ├── ManualSalesEntry.tsx
│   │   │   ├── SalesImport.tsx
│   │   │   ├── PaymentList.tsx
│   │   │   ├── PaymentDetail.tsx
│   │   │   └── CashDrawerList.tsx
│   │   ├── ledger/
│   │   │   ├── LedgerOverview.tsx
│   │   │   ├── ChartOfAccounts.tsx
│   │   │   ├── LedgerAccountForm.tsx
│   │   │   ├── AccountDetail.tsx
│   │   │   └── TrialBalance.tsx
│   │   ├── journal/
│   │   │   ├── JournalEntryList.tsx
│   │   │   ├── JournalEntryForm.tsx
│   │   │   └── JournalEntryDetail.tsx
│   │   ├── closeDay/
│   │   │   ├── CloseDayWizard.tsx
│   │   │   ├── CloseDayPreview.tsx
│   │   │   └── CloseDayHistory.tsx
│   │   ├── closeMonth/
│   │   │   ├── CloseMonthWizard.tsx
│   │   │   └── CloseMonthHistory.tsx
│   │   ├── reconciliation/
│   │   │   ├── ReconciliationDashboard.tsx
│   │   │   ├── BankStatementImport.tsx
│   │   │   ├── BankTransactionList.tsx
│   │   │   ├── PayoutList.tsx
│   │   │   ├── AutoMatchResults.tsx
│   │   │   ├── ManualMatchingWorkbench.tsx
│   │   │   ├── BankAccountList.tsx
│   │   │   └── PaymentProviderList.tsx
│   │   ├── invoices/
│   │   │   ├── InvoiceList.tsx
│   │   │   ├── InvoiceForm.tsx
│   │   │   ├── InvoiceDetail.tsx
│   │   │   └── InvoicePdfPreview.tsx
│   │   ├── customers/
│   │   │   ├── CustomerList.tsx
│   │   │   ├── CustomerForm.tsx
│   │   │   └── CustomerDetail.tsx
│   │   ├── expenses/
│   │   │   ├── ExpenseList.tsx
│   │   │   ├── ExpenseForm.tsx
│   │   │   ├── ExpenseDetail.tsx
│   │   │   ├── ExpenseApprovalQueue.tsx
│   │   │   └── ExpenseImport.tsx
│   │   ├── vendors/
│   │   │   ├── VendorList.tsx
│   │   │   ├── VendorForm.tsx
│   │   │   └── VendorDetail.tsx
│   │   ├── tax/
│   │   │   ├── TaxOverview.tsx
│   │   │   ├── TaxRateTable.tsx
│   │   │   ├── TaxReportList.tsx
│   │   │   ├── TaxReportDetail.tsx
│   │   │   └── TaxReportGenerator.tsx
│   │   ├── reports/
│   │   │   ├── ReportsHub.tsx
│   │   │   ├── ProfitLossReport.tsx
│   │   │   ├── PrimeCostReport.tsx
│   │   │   ├── RevenueAnalyticsReport.tsx
│   │   │   ├── CashFlowReport.tsx
│   │   │   ├── BalanceSheetReport.tsx
│   │   │   ├── ArAgingReport.tsx
│   │   │   ├── ApAgingReport.tsx
│   │   │   ├── ReconciliationSummaryReport.tsx
│   │   │   ├── BudgetVarianceReportPage.tsx
│   │   │   ├── ControllableProfitReport.tsx
│   │   │   ├── DeliveryProfitabilityReport.tsx
│   │   │   ├── LocationComparisonReport.tsx
│   │   │   ├── FlashReportTrendAnalysis.tsx
│   │   │   └── ExportCenter.tsx
│   │   ├── flash/                       (NEW)
│   │   │   ├── DailyFlashReport.tsx
│   │   │   ├── LiveFlashReport.tsx
│   │   │   └── FlashReportTrend.tsx
│   │   ├── budget/                      (NEW)
│   │   │   ├── BudgetList.tsx
│   │   │   ├── BudgetWizard.tsx
│   │   │   ├── BudgetEditor.tsx
│   │   │   └── BudgetVarianceReport.tsx
│   │   ├── delivery/                    (NEW — 6 pages)
│   │   │   ├── DeliveryDashboard.tsx         — gross vs. commission vs. net overview
│   │   │   ├── DeliveryPlatformList.tsx      — list of registered platforms
│   │   │   ├── DeliveryPlatformForm.tsx      — create/edit platform
│   │   │   ├── DeliveryPlatformDetail.tsx    — payout history, effective cost
│   │   │   ├── DeliveryPayoutImport.tsx      — CSV upload from platform
│   │   │   └── DeliveryReconciliation.tsx    — match payouts to bank deposits
│   │   ├── giftCards/                   (NEW)
│   │   │   ├── GiftCardOverview.tsx
│   │   │   ├── IssueGiftCardForm.tsx
│   │   │   ├── GiftCardDetail.tsx
│   │   │   ├── RedeemGiftCardForm.tsx
│   │   │   ├── GiftCardLiabilityReport.tsx
│   │   │   └── BreakageCalculation.tsx
│   │   ├── assets/                      (NEW)
│   │   │   ├── FixedAssetList.tsx
│   │   │   ├── FixedAssetForm.tsx
│   │   │   ├── FixedAssetDetail.tsx
│   │   │   ├── RunDepreciationWizard.tsx
│   │   │   └── DepreciationSchedule.tsx
│   │   ├── locations/                   (NEW)
│   │   │   ├── LocationOverview.tsx
│   │   │   ├── LocationForm.tsx
│   │   │   ├── LocationDetail.tsx
│   │   │   ├── LocationComparison.tsx
│   │   │   └── ConsolidatedDashboard.tsx
│   │   ├── ai/                          (NEW)
│   │   │   ├── InvoiceOcrScanner.tsx
│   │   │   ├── OcrResultReview.tsx
│   │   │   ├── AiCashFlowForecast.tsx
│   │   │   └── AiRevenueForecast.tsx
│   │   ├── alerts/                      (NEW)
│   │   │   ├── AlertCenter.tsx
│   │   │   ├── AlertSettings.tsx
│   │   │   ├── ScheduledReportList.tsx
│   │   │   └── ScheduledReportForm.tsx
│   │   └── settings/
│   │       ├── SettingsOverview.tsx
│   │       ├── GeneralSettings.tsx
│   │       ├── OrganizationSettings.tsx
│   │       ├── UserManagement.tsx
│   │       ├── PaymentSourceManager.tsx
│   │       ├── IntegrationConnectionManager.tsx
│   │       ├── InvoiceTemplateSettings.tsx
│   │       └── AuditLogViewer.tsx
│   │
│   ├── components/                 — Reusable components
│   │   ├── layout/
│   │   │   ├── AppLayout.tsx
│   │   │   ├── FinanceSidebar.tsx
│   │   │   ├── HeaderBar.tsx
│   │   │   ├── PageHeader.tsx
│   │   │   ├── ContentWrapper.tsx
│   │   │   └── LocationSwitcher.tsx    (NEW — multi-location context switcher)
│   │   ├── dashboard/
│   │   │   ├── KpiCard.tsx
│   │   │   ├── KpiRow.tsx
│   │   │   ├── RevenueChart.tsx
│   │   │   ├── PrimeCostTrendChart.tsx
│   │   │   ├── PaymentMethodPieChart.tsx
│   │   │   ├── AlertsWidget.tsx
│   │   │   ├── ReconciliationStatusWidget.tsx
│   │   │   ├── OutstandingWidget.tsx
│   │   │   ├── CashFlowMiniChart.tsx
│   │   │   └── QuickActionsWidget.tsx
│   │   ├── finance/
│   │   │   ├── MoneyDisplay.tsx
│   │   │   ├── MoneyInput.tsx
│   │   │   ├── PercentDisplay.tsx
│   │   │   ├── TrendIndicator.tsx
│   │   │   ├── StatusBadge.tsx
│   │   │   ├── AgingTable.tsx
│   │   │   ├── QrBillPreview.tsx
│   │   │   └── AccountPicker.tsx
│   │   ├── forms/
│   │   │   ├── InvoiceLineEditor.tsx
│   │   │   ├── JournalLineEditor.tsx
│   │   │   ├── CustomerSelect.tsx
│   │   │   ├── VendorSelect.tsx
│   │   │   └── FileUploadZone.tsx
│   │   ├── shared/
│   │   │   ├── DataTable.tsx
│   │   │   ├── DateRangePicker.tsx
│   │   │   ├── SearchInput.tsx
│   │   │   ├── StepWizard.tsx
│   │   │   ├── ConfirmModal.tsx
│   │   │   ├── EmptyState.tsx
│   │   │   ├── PrintLayout.tsx
│   │   │   ├── ReportDateRangePicker.tsx
│   │   │   ├── ReportExportBar.tsx
│   │   │   ├── OrganizationSwitcher.tsx
│   │   │   ├── ThemeToggle.tsx
│   │   │   └── AttachmentViewer.tsx
│   │   ├── reconciliation/
│   │   │   ├── MatchingCard.tsx
│   │   │   ├── ReconciliationTimeline.tsx
│   │   │   └── MatchConfidenceBadge.tsx
│   │   ├── flash/                       (NEW)
│   │   │   ├── FlashKpiGrid.tsx
│   │   │   ├── FlashComparisonTable.tsx
│   │   │   └── FlashTrendChart.tsx
│   │   ├── budget/                      (NEW)
│   │   │   └── BudgetVarianceMiniWidget.tsx
│   │   ├── delivery/                    (NEW)
│   │   │   └── DeliveryProfitabilityTable.tsx
│   │   ├── alerts/                      (NEW)
│   │   │   └── AlertBell.tsx
│   │   └── ai/                          (NEW)
│   │       └── OcrSideBySideView.tsx
│   │
│   ├── hooks/                      — Custom hooks (11 files)
│   │   ├── useFinanceApi.ts        — API call wrappers with loading/error state
│   │   ├── useMoney.ts             — Swiss money formatting hook
│   │   ├── usePermission.ts        — Role-based UI permission checks
│   │   ├── useDateRange.ts         — Date range with presets
│   │   ├── usePagination.ts        — Server-side pagination hook
│   │   ├── useDebounce.ts          — Debounced value hook
│   │   ├── usePrint.ts             — Print functionality hook
│   │   ├── useLocationContext.ts   — Current location from Redux, location switcher logic
│   │   ├── useAlerts.ts            — Unread alert count, alert polling, mark-as-read
│   │   ├── useThreshold.ts         — KPI threshold color determination (green/amber/red)
│   │   └── useExport.ts            — CSV/PDF export trigger with loading state
│   │
│   ├── utils/                      — Utility functions (11 files)
│   │   ├── formatMoney.ts          — Swiss locale money formatting (1'234.56 CHF)
│   │   ├── formatDate.ts           — Date formatting with dayjs (Swiss formats)
│   │   ├── formatPercent.ts        — Percentage formatting with threshold coloring
│   │   ├── vatCalculation.ts       — Client-side VAT calculations (8.1%, 2.6%, 3.8%)
│   │   ├── agingBuckets.ts         — AR/AP aging bucket calculation
│   │   ├── colorByThreshold.ts     — KPI color determination (green/amber/red)
│   │   ├── deliveryCalc.ts         — Delivery platform profitability calculations (commission %, effective cost)
│   │   ├── giftCardCalc.ts         — Gift card liability and breakage calculations
│   │   ├── primeCostCalc.ts        — Prime cost calculation and threshold checking
│   │   ├── exportCsv.ts            — Client-side CSV generation
│   │   └── constants.ts            — Swiss VAT rates, account types, enums, role permissions
│   │
│   ├── theme/                      — Theme configuration
│   │   ├── financeTheme.ts         — Light theme tokens
│   │   ├── financeDarkTheme.ts     — Dark theme tokens
│   │   └── printStyles.css         — Print-specific stylesheet
│   │
│   ├── types/                      — TypeScript type definitions
│   │   ├── api.ts                  — API response types
│   │   ├── entities.ts             — Entity types (mirror backend DTOs)
│   │   ├── forms.ts                — Form value types
│   │   └── enums.ts                — Enum types
│   │
│   └── styles/                     — Global styles
│       ├── global.scss             — Global styles, CSS reset
│       ├── variables.scss          — SCSS variables
│       ├── tables.scss             — Table-specific styles (accounting format)
│       └── print.scss              — Print stylesheet
│
├── index.html
├── vite.config.ts
├── tsconfig.json
├── tsconfig.node.json
├── package.json
├── .env.example                    — VITE_FINANCE_API_URL=http://localhost:5200
├── .eslintrc.cjs
├── .prettierrc
├── vitest.config.ts
├── playwright.config.ts
└── Dockerfile
```

---

## 14. Estimated Component Count

| Category | Count |
|----------|-------|
| **Pages** | ~87 (up from 55) |
| **Layout Components** | 6 (+ LocationSwitcher) |
| **Dashboard Components** | 10 |
| **Daily Flash Report Components** | 6 (NEW) |
| **Budget Components** | 5 (NEW) |
| **Delivery Platform Components** | 7 (NEW) |
| **Gift Card Components** | 5 (NEW) |
| **Fixed Asset Components** | 5 (NEW) |
| **Multi-Location Components** | 5 (NEW) |
| **AI Tools Components** | 4 (NEW) |
| **Alert/Notification Components** | 5 (NEW) |
| **Finance Components** | 8 |
| **Form Components** | 5 |
| **Shared Components** | 12 |
| **Reconciliation Components** | 3 |
| **Report Components** | 16 (up from 10) |
| **Custom Hooks** | 11 (useFinanceApi, useMoney, usePermission, useDateRange, usePagination, useDebounce, usePrint, useLocationContext, useAlerts, useThreshold, useExport) |
| **API Modules** | 24 (up from 16) |
| **Redux Slices** | 20 (up from 12) |
| **Router Files** | 24 (up from 15) |
| **Utility Files** | 11 (formatMoney, formatDate, formatPercent, vatCalculation, agingBuckets, colorByThreshold, deliveryCalc, giftCardCalc, primeCostCalc, exportCsv, constants) |
| **Translation Namespaces** | 18 × 4 languages = 72 files (up from 44) |
| **Total Estimated TSX/TS Files** | ~270 (up from 155) |

---

## 15. Key UX Patterns

| Pattern | Usage | Why |
|---------|-------|-----|
| **Preview-before-commit** | Close Day, Close Month, Import, Tax Report | Financial actions are irreversible. Users must review before committing. |
| **Step Wizard** | Close Day (5 steps), Import (4 steps), Tax Report (3 steps) | Complex workflows broken into manageable steps with validation at each. |
| **Inline editing** | Manual sales entry (table-style), journal line editing | Fast data entry for financial professionals. |
| **Split-screen matching** | Reconciliation matching workbench | Side-by-side comparison is the natural way to match bank items to payouts. |
| **Status tabs** | Invoice list, Expense list, Payout list | Quick filtering by status is the primary navigation in financial lists. |
| **Expandable rows** | P&L (expand to sub-accounts), Chart of Accounts (tree) | Progressive disclosure — show summary first, detail on demand. |
| **Comparison columns** | P&L report, Tax report, Dashboard KPIs | Period-over-period comparison is essential for financial analysis. |
| **Color-coded thresholds** | KPI cards, aging buckets, prime cost | Immediate visual signal — green = on target, amber = warning, red = problem. |
| **Persistent filters** | All list pages | Financial users set complex filters and don't want to re-apply on return. |
| **Keyboard shortcuts** | N = New, S = Save, E = Export, P = Print | Power users (accountants) expect keyboard shortcuts for efficiency. |
| **Spreadsheet-style editing** | Budget editor, manual sales entry | Financial professionals think in spreadsheets. Table-style inline editing with tab navigation between cells. |
| **Side-by-side OCR review** | Invoice OCR scanner | Original document on left, extracted data on right. Users validate and correct AI output before creating expense. |
| **Location context** | Header location switcher, all data pages | Every page respects location context — single location data or consolidated. Switching location refreshes all data. |
| **Morning email report** | Daily flash report | Auto-emailed PDF at 6 AM — managers check on phone before arriving at restaurant. The #1 engagement feature. |
| **Anomaly highlighting** | Flash report, budget variance, delivery dashboard | Statistical outliers automatically highlighted in red/amber. Users' attention drawn to exceptions, not normal data. |
| **Platform comparison** | Delivery dashboard | "Is Uber Eats worth it?" — show true profitability after commissions. Side-by-side platform comparison. |

---

*This is a standalone application. It communicates with the Finance Pro backend exclusively through REST APIs. No shared code with BonApp frontend monorepo. No shared UI components. No cross-app navigation.*
