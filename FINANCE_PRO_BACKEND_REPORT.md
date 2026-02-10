# Finance Pro - Backend Development Report

**Generated:** February 9, 2026  
**Project:** Finance Pro — Standalone Restaurant Financial Management System  
**Framework:** .NET 8.0 | ASP.NET Core  
**Architecture:** Clean Architecture (standalone project — NOT part of BonApp backend)  
**Repository:** `finance-pro-backend` (separate Git repository)  
**Database:** Own SQL Server database (`FinanceProDb`)  
**Status:** Planning Phase

---

## 1. Executive Summary

This report defines the complete backend development plan for **Finance Pro**, a fully **autonomous, standalone** restaurant financial management system. This is an **independent application** — it has its own codebase, database, authentication, and deployment. It does NOT live inside the BonApp backend.

Finance Pro is the **"money brain"** of restaurant operations. While Inventory Pro covers COGS, Staff Pro covers labor, and POS covers sales — Finance Pro ties everything together into **profitability, bookkeeping, and compliance**. It provides the complete financial picture that restaurant owners and accountants need.

The application is designed for any restaurant or hospitality business, regardless of which POS system they use:

- **BonApp POS** — Connected via REST API / webhooks (future integration)
- **Lightspeed, Toast, Square, Orderbird** — Connected via REST API / webhooks
- **Any POS system** — Connected via the documented External Integration API (`/api/pos/v1/`, `/api/inventory/v1/`, `/api/staff/v1/`)
- **No POS at all** — Standalone finance management with manual daily sales entry, CSV import, invoicing, and bank reconciliation

### Core Features at a Glance (MVP Only — P0)

These are the **only features in the first release**. Everything else comes later.

| # | Feature | Description |
|---|---------|-------------|
| 1 | Auth + user management | Register, login, JWT tokens, roles (Owner/Accountant/Manager/Staff) |
| 2 | Organization setup | Create restaurant profile, settings, branding |
| 3 | Chart of Accounts | Swiss Kontenrahmen KMU pre-seeded, customizable |
| 4 | Journal entries | Double-entry bookkeeping, auto + manual journal entries |
| 5 | Expense tracking | Record expenses, categorize, attach files, vendor management |
| 6 | Invoicing | Create invoices, PDF generation, Swiss QR-bill, customer management |
| 7 | Sales & payments | Payment ingestion API, manual entry, CSV import, daily summaries |
| 8 | Close-day workflow | End-of-day closing with preview, auto journal generation, lock |
| 9 | Bank reconciliation | CSV import, payout tracking, reconciliation dashboard |
| 10 | VAT reporting | Swiss VAT rates (8.1/2.6/3.8%), VAT returns, ESTV export |
| 11 | P&L + prime cost | Restaurant P&L, prime cost %, COGS/labor integration |
| 12 | Dashboard | KPI cards, revenue trends, alerts, reconciliation status |
| 13 | Audit trail | Immutable log (GeBüV compliance), 10-year retention |
| 14 | Settings | Org settings, API key management, webhook config |

> **That's it for v1.** No AI, no multi-location, no gift cards, no delivery tracking, no budgets. Those come in Phase 2-4 after the core is solid.

### Architectural Independence

This system is **completely decoupled** from BonApp:
- **Own database** — `FinanceProDb` with its own schema (no shared tables)
- **Own authentication** — ASP.NET Identity with JWT Bearer tokens (independent user accounts)
- **Own entities** — `Organization` (restaurant), `LedgerAccount` (chart of accounts), `AppUser` (users) — NOT BonApp's `YumYumYard`, `Product`, `ApplicationUser`
- **Own deployment** — Separate Docker container, separate Azure App Service, separate CI/CD pipeline
- **API-first integration** — Any future connection to BonApp, Inventory Pro, Staff Pro, or other systems happens exclusively through the Integration API endpoints using API keys

### Operating Modes

**Standalone Mode (No POS):**
- Manual daily sales entry or CSV import
- Manual expense entry + invoicing
- Bank reconciliation (CSV import, match payouts)
- VAT reports + P&L basics
- Works for small cafes, bakeries, food trucks that just want "finance clarity"

**Integrated Mode (with BonApp POS + other Pro apps):**
- POS feeds real transactions automatically via API/webhooks
- Inventory Pro feeds COGS + supplier bills via API
- Staff Pro feeds payroll exports + labor cost by shift/day via API
- Finance Pro becomes the **"single financial truth"**

### Future BonApp Integration (Phase 2)

When the time comes to connect Finance Pro to BonApp and other Pro modules, it will happen through:
1. BonApp POS registers as a Payment Source in Finance Pro (gets an API key)
2. BonApp calls `POST /api/pos/v1/payments` when payments are captured, refunded, or tips added
3. BonApp calls `POST /api/pos/v1/orders` to send order-level gross/net/tax data
4. Inventory Pro calls `POST /api/inventory/v1/cogs` with period COGS summaries
5. Staff Pro calls `POST /api/staff/v1/labor-summary` with period labor summaries
6. Finance Pro sends outbound webhooks on events like `tax.report_ready`, `payout.mismatch_detected`
7. **No shared database. No shared code. No shared authentication. API calls only.**

### Multi-Location Support (Phase 3 — P2)

Finance Pro is **designed** for multi-location restaurant groups, but this is built **after the single-location MVP is stable**:

**MVP (single location):** The `LocationId` column exists on all relevant entities but is always NULL. The app works as a single-location finance system. No location switcher in the UI.

**Phase 3 (multi-location enabled):**
- **Organization hierarchy** — Parent organization with child locations. Each location has its own daily sales, expenses, and cash drawers.
- **Consolidated reporting** — P&L, prime cost, and cash flow rolled up across all locations.
- **Location comparison** — Side-by-side KPI comparison: which location has the best food cost %, highest revenue, worst cash variance.
- **Above-store dashboard** (Phase 4) — High-level view for operators/CFOs managing multiple units.
- **Standardized chart of accounts** — Enforced across all locations for accurate consolidation.
- **Per-location and consolidated tax reporting** — Roll-up VAT returns or per-location filings.

> **Why the data model includes LocationId from day one:** Adding a nullable FK later requires database migrations and code changes across every query. Including it as nullable from the start is zero-cost and prevents a painful migration.

### What Makes Finance Pro Best-in-Class (Industry Research — Phased Delivery)

Based on analysis of the leading restaurant finance platforms (Restaurant365, MarginEdge, CrunchTime, Distil.ai, Toast, QuickBooks for Restaurants), Finance Pro incorporates the following differentiating capabilities **across phases**:

| Capability | Phase | Industry Source | Our Implementation |
|------------|-------|----------------|--------------------|
| **Swiss Compliance (QR-Bill, GeBüV, Kontenrahmen)** | **MVP** | Swiss legal requirement | Pre-seeded Swiss chart of accounts, immutable audit trail, QR-bill invoices with structured address (Type S). This is what makes Finance Pro uniquely Swiss. |
| **Close-Day Workflow** | **MVP** | Industry standard | Preview-before-commit day closing with auto-generated journal entries. Locks data from edits. Foundation for all reporting. |
| **Basic P&L + Prime Cost** | **MVP** | Every restaurant needs this | Restaurant-specific P&L: Revenue → COGS → Gross Profit → Labor → OpEx → Net Profit. Prime cost = COGS + Labor (the #1 restaurant KPI). |
| **Daily Flash Report** | Phase 2 | Restaurant365's most popular feature | Auto-generated daily snapshot: revenue, labor %, food cost %, prime cost %, guest count, average check — emailed to managers at configurable time. |
| **AI-Powered Invoice OCR** | Phase 3 | R365 AP Capture AI, MarginEdge | Upload supplier invoice photo/PDF → AI extracts vendor, date, amount, line items, VAT → auto-populates expense form. |
| **Budget vs. Actual Variance** | Phase 3 | Industry standard for multi-unit operators | Set monthly/weekly budgets per P&L line item. Real-time budget vs. actual with variance % and dollar amount. |
| **3rd-Party Delivery Reconciliation** | Phase 3 | Major pain point for 80%+ of restaurants | Track Uber Eats, DoorDash commissions (15-25%). Reconcile net payouts against gross orders. |
| **Multi-Location Comparison** | Phase 3 | CrunchTime, Forte | Side-by-side KPIs across locations. Consolidated reporting. |
| **Real-Time Variance Alerts** | Phase 3 | CrunchTime Operational Intelligence | Push alerts when KPIs exceed thresholds. Configurable per metric, per location. |
| **AI Cash Flow Forecasting** | Phase 4 | Distil.ai (98-99% accuracy) | Predict next 7/30/90 days cash position based on historical patterns. |
| **Gift Card Liability Tracking** | Phase 4 | ASC 606 / Swiss GAAP FER compliance | Gift card sales as liability. Revenue on redemption. Breakage calculation. |
| **Fixed Asset Management** | Phase 4 | R365 Plan Comparison | Track equipment with depreciation. Auto-generate depreciation journal entries. |

### MVP Scope (Realistic Phasing — Build What Works First)

**Principle:** Ship a working single-location finance app before adding advanced features. Every phase must be fully usable on its own.

| Phase | Features | What the User Gets | Complexity |
|-------|----------|---------------------|------------|
| **MVP (P0)** | Auth + org setup, chart of accounts (Swiss pre-seeded), manual sales entry + CSV import, payment ingestion API, daily sales summary, close-day workflow, basic P&L, prime cost %, expense entry + categories, vendor management, invoice creation + PDF + Swiss QR-bill, customer management, bank statement import (CSV only), basic reconciliation dashboard, payout tracking, VAT reporting, audit trail, dashboard KPIs | **A working restaurant finance app** — enter sales, track expenses, create invoices, reconcile bank, file VAT, see P&L. Single location. | Medium |
| **Phase 2 (P1)** | Daily flash report (email), auto-matching engine for reconciliation, manual matching workbench, reconciliation alerts, close-month workflow, fiscal period management, cash management (drawers), chargeback handling, partial invoice payments, recurring expenses, bill payment tracking, multi-period comparison, revenue analytics, operating expense analysis, tip tracking by staff | **Automation + polish** — less manual work, smarter reconciliation, deeper reports. Still single location. | Medium-High |
| **Phase 3 (P2)** | 3rd-party delivery platform tracking, budget management + variance, AI invoice OCR, multi-location support, location comparison, scheduled report emails, real-time variance alerts, payment reminders, Abacus/Sage export, CAMT.053 bank import, controllable profit tracking, expense approval workflow | **Scale + intelligence** — multi-location, delivery tracking, OCR, budgets, automated alerts. | High |
| **Phase 4 (P3)** | AI cash flow forecasting, AI revenue forecasting, gift card liability tracking + breakage, fixed asset management + depreciation, above-store dashboard, break-even analysis, purchase order matching (Inventory Pro integration) | **Advanced analytics** — AI predictions, asset tracking, gift cards. Only for mature deployments. | Very High |

> **Important:** Do NOT build Phase 2-4 features until Phase 1 (MVP) is shipped, tested, and stable. Each feature in MVP must work end-to-end before moving on.

### MVP Implementation Order (Build in This Sequence)

This is the recommended build order within the MVP. Each step depends on the previous one.

| Step | What to Build | Why This Order |
|------|--------------|----------------|
| 1 | **Project setup** — Solution structure, EF Core DbContext, migrations, Docker, CI pipeline | Foundation for everything |
| 2 | **Auth + Organization** — Register, login, JWT, org creation, user roles | Everything requires authentication |
| 3 | **Chart of Accounts + Swiss seed data** — LedgerAccount CRUD, Kontenrahmen KMU seeder | Bookkeeping foundation — every transaction needs accounts |
| 4 | **Journal Entry system** — Create, post, void, debit=credit validation, fiscal periods | Core accounting engine — everything writes journal entries |
| 5 | **Expense module** — Expense CRUD, vendor management, categories, file upload | First real user feature — simple and testable |
| 6 | **Invoice module** — Invoice CRUD, customer management, PDF generation, Swiss QR-bill | Second real user feature — generates revenue entries |
| 7 | **Payment ingestion + daily sales** — Payment records, daily summaries, manual entry, CSV import | Sales data flows into the system |
| 8 | **Close-day workflow** — Aggregate, preview, close, auto-generate journal entries | Ties sales into bookkeeping |
| 9 | **Bank reconciliation (basic)** — Bank account registration, CSV import, payout tracking, reconciliation dashboard | Match what the bank shows vs. what the system expects |
| 10 | **VAT reporting** — Tax rates, VAT report generation, ESTV export | Compliance requirement — needed from day one |
| 11 | **P&L + prime cost** — Profit & Loss from journal data, COGS/labor integration (manual entry), prime cost KPI | The key financial report |
| 12 | **Dashboard** — KPI cards, revenue chart, alerts widget, quick actions | The home screen that ties everything together |
| 13 | **Audit trail** — Immutable log of all actions (GeBüV compliance) | Swiss legal requirement |
| 14 | **Settings + integration management** — Org settings, API key management, webhook config | Configuration for future integrations |

> After these 14 steps, you have a **fully functional single-location restaurant finance app**. Ship it. Get user feedback. Then build Phase 2.

### Priority Legend

| Priority | Meaning | When to Build |
|----------|---------|---------------|
| **P0** | Core — app doesn't work without it | MVP (ship first) |
| **P1** | Important — makes the app significantly better | Phase 2 (after MVP is stable) |
| **P2** | Valuable — for scaling and advanced users | Phase 3 (after Phase 2 feedback) |
| **P3** | Nice-to-have — impressive but only for mature deployments | Phase 4 (only if demand exists) |

---

## 2. Core Features to Build

### 2.1 Sales & Settlement (Multi-Payment Reality)

| Feature | Description | Priority |
|---------|-------------|----------|
| **Payment Source Management** | Register any payment source — BonApp POS, Worldline/Saferpay terminals, Stripe, Wallee, cash registers, or manual entry. Each source gets a unique API key. Track which source generated each payment for reconciliation. | P0 |
| **Payment Ingestion** | Receive and normalize payment data from all sources. Each payment captures: amount, method (card, cash, online, mobile), provider, timestamp, order reference, VAT breakdown. Supports capture, authorization, refund, and chargeback events. | P0 |
| **Multi-Payment Support** | Handle split payments on a single order (e.g., part card + part cash). Track partial payments, combined payments, and group payments. Link multiple payment records to a single order. | P0 |
| **Tip Tracking** | Track tips by payment method — card tips (from terminal/online), cash tips (manual entry), or service charge. Attribute tips to staff (optional, for labor cost analysis). Tip aggregation by period, payment method, and staff member. | P1 |
| **Refund Management** | Process and track refunds: full refunds, partial refunds, void transactions. Link refunds to original payments. Track refund reasons (customer complaint, order error, quality issue). Refunds auto-create reversal journal entries. | P0 |
| **Chargeback Handling** | Record and track chargebacks from payment providers. Alert on new chargebacks. Track dispute status (Open, Won, Lost). Chargebacks create provisional journal entries (reversed if won). Chargeback rate monitoring as a KPI. | P1 |
| **Cash Management** | Track cash drawer: opening float, sales, tips, payouts, closing count. Cash variance detection (expected vs. actual). Support multiple cash drawers per shift. Cash skimming/drops during shift. | P1 |
| **Daily Sales Aggregation** | Auto-aggregate all payments into daily summaries: gross sales, net sales (after refunds), tips, by payment method. Serves as the foundation for the close-day workflow. | P0 |
| **Manual Sales Entry** | For standalone mode: manual entry of daily sales totals with payment method breakdown and VAT split. CSV import for bulk historical data. Templates for recurring patterns. | P0 |
| **Daily Flash Report** | Auto-generated daily operational snapshot — the #1 most-used report in restaurant finance (Restaurant365's most popular feature). Includes: net sales, guest count, average check, labor cost %, food cost %, prime cost %, same-day-last-year comparison, same-day-last-week comparison. Auto-emailed to managers/owners at a configurable time (e.g., 6 AM next morning). Requires previous day to be closed. Available per location and consolidated. **Depends on: close-day workflow + COGS/labor data.** | P1 |
| **3rd-Party Delivery Platform Tracking** | Track revenue from delivery platforms (Uber Eats, DoorDash, Just Eat, eat.ch, Smood). Each platform registered as a payment source with commission rate (typically 15-25%). Import platform payout reports (CSV). Reconcile: gross order value vs. commission deducted vs. net payout received. Track effective delivery cost per order. Principal vs. agent revenue recognition (GAAP Topic 606 / Swiss GAAP FER). This is the #1 pain point for modern restaurants — most lose money on delivery without knowing it. **Important but not essential for MVP — many restaurants don't use delivery platforms.** | P2 |
| **Gift Card & Voucher Tracking** | Gift card sales recorded as **liability** (deferred revenue), not immediate income. Revenue recognized only on redemption. Track: cards issued, redeemed, outstanding balance, expired. Breakage calculation (industry average 10-15% of gift cards are never redeemed — recognized as revenue over time). Multi-location cross-redemption tracking. Compliant with ASC 606 / Swiss GAAP FER revenue recognition standards. **Niche feature — only for restaurants that sell gift cards.** | P3 |

### 2.2 Bookkeeping-Ready Ledger (Finance-Grade, Not Full ERP)

| Feature | Description | Priority |
|---------|-------------|----------|
| **Chart of Accounts** | Pre-seeded Swiss chart of accounts (Kontenrahmen KMU) aligned with Swiss GAAP FER. Customizable — add/edit accounts, maintain account hierarchy. Account types: Asset, Liability, Equity, Revenue, Expense. Account numbering follows Swiss conventions (e.g., 1000 Cash, 1020 Bank, 3000 Revenue from sales, 4000 Material purchases, 5000 Personnel expenses, 6000 Other operating expenses). | P0 |
| **Auto Journal Entries** | Automatically generate double-entry journal entries from: POS sales (debit cash/bank, credit revenue + VAT payable), refunds (reverse the sale entry), payouts received (debit bank, credit settlement receivable), expenses entered (debit expense category, credit accounts payable/cash). No manual bookkeeping needed for standard flows. | P0 |
| **Manual Journal Entries** | Support manual journal entries for non-standard transactions: corrections, accruals, prepayments, owner draws, loan payments, depreciation. Debit/credit must balance. Approval workflow optional. | P0 |
| **Multi-VAT Rate Support** | Swiss VAT rates: 8.1% (standard), 2.6% (reduced — food/beverages for takeaway), 3.8% (hospitality/accommodation). Track VAT on every transaction line. Auto-calculate VAT from gross or net amounts. Support VAT-exempt transactions. | P0 |
| **VAT Reporting** | Generate VAT returns (Swiss MWST-Abrechnung): total revenue by VAT rate, input VAT (from supplier invoices), output VAT (from sales), net VAT payable. Export-ready for Swiss Federal Tax Administration (ESTV). Quarterly and annual summaries. | P0 |
| **Close Day Workflow** | End-of-day financial closing: aggregate all sales, verify cash counts, reconcile card payments, generate daily journal entries, lock the day's transactions from further edits. Preview before closing. Reopen with manager approval if corrections needed. | P0 |
| **Close Month Workflow** | End-of-month closing: verify all days are closed, run accruals, generate monthly financial statements (P&L, balance sheet preview), lock the month. Export for accountant. | P1 |
| **Fiscal Period Management** | Define fiscal year start/end (Swiss default: Jan 1 - Dec 31, but configurable). Track open/closed periods. Prevent posting to closed periods without override. Year-end closing entries. | P1 |
| **Audit Trail** | Every journal entry, modification, deletion, and approval is logged with user, timestamp, IP address, and before/after values. Immutable audit log. Required for Swiss compliance (Geschäftsbücherverordnung — GeBüV). Invoices and financial records archived for **10 years** (Swiss legal requirement). | P0 |
| **Budget Management** | Create annual/monthly budgets per P&L line item (revenue targets, COGS budget, labor budget, OpEx budget). Budget templates for quick setup. Copy last year's budget as starting point. Flexible budget that adjusts targets based on actual revenue (e.g., if revenue is 10% above budget, COGS budget auto-adjusts proportionally). Lock approved budgets. **Useful but requires at least 1-2 months of historical data to be meaningful.** | P2 |
| **Budget vs. Actual Variance Reporting** | Real-time comparison of budgeted vs. actual figures for every P&L line. Variance shown in dollars and percentage. Color-coded: green (under budget), yellow (within 5%), red (over budget). Drill-down from variance to specific transactions. Weekly and monthly variance summaries. The industry standard for multi-unit operators — crucial for catching cost overruns before month-end. **Depends on: Budget Management.** | P2 |
| **Controllable Profit Tracking** | Separate P&L into **controllable** vs. **non-controllable** expenses. Controllable: COGS, labor, direct operating expenses, marketing, utilities, repairs. Non-controllable: rent, insurance, depreciation, interest, taxes. **Controllable Profit = Revenue - Controllable Expenses**. This is the best indicator of management effectiveness and is commonly used for manager bonuses in restaurant groups. Track per location for comparison. **Nice for restaurant groups, not essential for single-location MVP.** | P2 |
| **Fixed Asset Register** | Track restaurant equipment: kitchen equipment, POS terminals, furniture, signage. Record purchase date, cost, useful life, depreciation method (straight-line or declining balance). Auto-generate monthly depreciation journal entries. Track accumulated depreciation and book value. Required for accurate balance sheet and tax depreciation claims. **Most small restaurants handle this via their accountant, not via software.** | P3 |

### 2.3 Bank & Payout Reconciliation

| Feature | Description | Priority |
|---------|-------------|----------|
| **Bank Account Management** | Register bank accounts (IBAN, bank name, currency). Support multiple accounts per organization. Track expected vs. actual balance. | P0 |
| **Bank Statement Import (CSV)** | Import bank statements from CSV files (UBS, Credit Suisse, PostFinance, Raiffeisen common formats). Parse date, description, amount, reference. Configurable column mapping for different bank formats. | P0 |
| **Bank Statement Import (CAMT.053)** | Import ISO 20022 CAMT.053 (Swiss standard for bank-to-customer statements). Parse structured remittance data, creditor/debtor info, payment references. Widely used by Swiss banks. **Complex XML parsing — CSV is sufficient for MVP.** | P2 |
| **Payment Provider Payout Tracking** | Track expected payouts from each payment provider (Worldline, Stripe, Wallee). Each payout has: provider, expected amount, expected date, actual amount, actual date, status (Pending, Received, Partial, Missing, Disputed). Fees deducted by provider are tracked separately. | P0 |
| **Auto-Matching Engine** | Automatically match bank transactions to expected payouts using: amount (exact or within tolerance), date (within window), reference number, payment provider batch ID. Confidence scoring: Exact Match, Probable Match, Manual Review Required. | P1 |
| **Reconciliation Dashboard** | Visual dashboard showing: "Expected Payouts" (from payment providers), "Received Payouts" (matched to bank transactions), "Missing/Unmatched" (expected but not received), "Disputed" (amount mismatch or timing issue), "Unidentified Bank Transactions" (received but not expected). Summary KPIs: reconciliation rate %, average settlement delay, outstanding amount. | P0 |
| **Manual Matching** | UI for manually matching unmatched bank transactions to expected payouts. Split matching (one bank transaction = multiple payouts, or vice versa). Mark as "identified" with notes for non-payout deposits (owner injection, loan, interest, etc.). | P1 |
| **Reconciliation Alerts** | Auto-alert when: payout is overdue (configurable days past expected), payout amount differs from expected by more than threshold, chargeback detected, unmatched transactions older than X days. Webhook: `payout.mismatch_detected`. | P1 |

### 2.4 Invoicing (B2B / Events / Catering)

| Feature | Description | Priority |
|---------|-------------|----------|
| **Invoice Creation** | Create professional invoices for B2B clients, catering orders, event bookings, corporate accounts. Line items with description, quantity, unit price, VAT rate. Auto-calculate totals and VAT. | P0 |
| **Customer (Debtor) Management** | Manage B2B customers / debtors: company name, contact, address, VAT ID, payment terms (default: 30 days), credit limit. Link invoices to customers. Track customer outstanding balance. | P0 |
| **Invoice Lifecycle** | Status workflow: Draft → Sent → Partially Paid → Paid → Overdue → Cancelled → Written Off. Automatic status transitions based on payment receipt. | P0 |
| **Partial Payments** | Track partial payments against invoices. Running balance per invoice. Multiple payments on a single invoice. Allocate incoming bank transactions to open invoices. | P1 |
| **Due Date & Payment Terms** | Configurable payment terms per customer: Net 10, Net 30, Net 60, COD, custom. Auto-calculate due date from invoice date. Track aging: Current, 1-30 days, 31-60 days, 61-90 days, 90+ days. | P0 |
| **Payment Reminders** | Auto-generate payment reminders at configurable intervals: first reminder (due date + 10 days), second reminder (+ 20 days), final notice (+ 30 days). Email delivery or PDF download. Reminder log with timestamps. | P2 |
| **PDF Invoice Generator** | Generate professional PDF invoices with: organization logo, address, customer address, invoice number (auto-sequenced), line items, VAT breakdown, bank details (IBAN, QR-bill reference), payment terms, ESR/QR reference number for Swiss bank payments. | P0 |
| **QR-Bill Support (2025 Compliant)** | Generate Swiss QR-bills (QR-Rechnung) on invoices — mandatory payment slip format in Switzerland. Includes QR code with IBAN, amount, reference number (QR-IBAN + QR reference or SCOR reference). **CRITICAL: As of November 21, 2025, only structured addresses (Type S) are permitted in QR invoices.** Unstructured addresses (Type K) are no longer valid. Finance Pro uses Type S from day one. Transition deadline for existing systems: September 30, 2026. Compatible with all Swiss bank e-banking. | P0 |
| **Invoice Numbering** | Auto-sequential invoice numbering with configurable prefix: e.g., `INV-2026-0001`. Separate sequences per fiscal year. Gaps detection for compliance. | P0 |
| **Export for Abacus / Sage** | Export invoices and journal entries in formats compatible with popular Swiss accounting software: Abacus (TAF format), Sage (CSV/XML export). Mapping configuration for account codes. | P2 |

### 2.5 Expense & Bills (Light Accounts Payable)

| Feature | Description | Priority |
|---------|-------------|----------|
| **Expense Entry** | Record business expenses: rent, utilities, insurance, marketing, maintenance, licenses, professional services, miscellaneous. Amount, date, category, VAT rate, payment method, description. | P0 |
| **Supplier Invoice Upload** | Upload supplier invoices (PDF, image). Store as attachments linked to expense records. | P0 |
| **AI-Powered Invoice OCR (AP Capture AI)** | Upload invoice photo/PDF → AI/OCR automatically extracts: vendor name, invoice number, date, line items, amounts, VAT rate, total. Auto-populates expense form. Learns from corrections — accuracy improves per vendor over time (R365 AP Capture AI model). Supports European date formats and Swiss number formats. Catch-weight column support for food invoices. GL account mapping memorization (remembers which expense category for recurring vendor items). Reduces AP data entry by up to 50% (industry benchmark from Restaurant365). Handles multi-page invoices. Rejects duplicates automatically. **Requires Azure AI Document Intelligence — adds infrastructure cost and complexity.** | P2 |
| **Expense Categories** | Pre-seeded Swiss expense categories aligned with chart of accounts: Material (4000), Personnel (5000), Premises/Rent (6000), Administrative (6500), Marketing (6600), Depreciation (6800), Finance (6900). Customizable sub-categories. | P0 |
| **Vendor Management** | Manage vendors/creditors: name, contact, IBAN, payment terms. Link expenses to vendors. Track vendor outstanding balance (AP aging). | P0 |
| **Purchase Order Matching** | Match supplier invoices to purchase orders from Inventory Pro (when integrated). Three-way matching: PO → Goods Receipt → Invoice. Variance detection: price differences, quantity differences. | P2 |
| **Recurring Expenses** | Define recurring expenses (monthly rent, weekly cleaning, quarterly insurance). Auto-generate expense entries on schedule. Editable before posting. | P1 |
| **Expense Approval Workflow** | Optional approval workflow: Staff submits → Manager reviews → Owner approves → Posted. Configurable thresholds (e.g., auto-approve under CHF 200, require approval above). | P2 |
| **Bill Payment Tracking** | Track payment status of supplier bills: Unpaid → Scheduled → Paid. Due date tracking. Cash flow planning based on upcoming bills. | P1 |

### 2.6 Profitability — The Complete Picture

| Feature | Description | Priority |
|---------|-------------|----------|
| **Profit & Loss Statement** | Industry-standard P&L for restaurants: **Revenue** (from POS or manual entry) → **- COGS** (from Inventory Pro or manual entry) = **Gross Profit** → **- Labor** (from Staff Pro or manual entry) → **- Operating Expenses** (rent, utilities, etc. from expenses module) = **Net Operating Profit**. Period comparison (this month vs. last month, this year vs. last year). | P0 |
| **Prime Cost Calculation** | **Prime Cost = COGS + Labor** — the single most important KPI in restaurant management. Target: 55-65% of revenue. Tracked daily, weekly, monthly. Alert when prime cost exceeds threshold. Breakdown by category (food COGS + beverage COGS + labor). | P0 |
| **COGS Integration** | Receive COGS summaries from Inventory Pro via API, or enter manually. Track food cost %, beverage cost %, supply cost % separately. Industry benchmarks: food 28-35%, beverage 18-24%. | P0 |
| **Labor Cost Integration** | Receive labor summaries from Staff Pro via API, or enter manually. Track labor cost % of revenue. Include wages, benefits, payroll taxes. Industry benchmark: 25-35%. | P0 |
| **Operating Expense Analysis** | Categorize and track all non-COGS, non-labor expenses. Controllable (marketing, supplies) vs. non-controllable (rent, insurance, depreciation). Period-over-period trend analysis. | P1 |
| **Revenue Analytics** | Revenue breakdown by: payment method, day of week, time of day (meal periods), product category (if POS provides order details). Average ticket size. Covers per labor hour. Revenue per available seat hour (RevPASH). | P1 |
| **Dashboard KPIs** | Real-time financial dashboard: daily/weekly/monthly revenue, prime cost %, food cost %, labor cost %, net profit margin, cash position, outstanding invoices (AR), outstanding bills (AP), reconciliation status, VAT liability. | P0 |
| **Break-Even Analysis** | Calculate break-even point: fixed costs / (1 - variable cost ratio). Show how many covers/day needed to break even. Sensitivity analysis with cost changes. **Nice but not essential — a simple calculator, low effort to add later.** | P3 |
| **AI Cash Flow Forecasting** | Machine-learning forecast of cash position forward based on: historical revenue patterns (day-of-week, seasonality, holidays, weather), scheduled bill payments, upcoming payout settlements, pending invoice collections, recurring expenses. 7-day, 30-day, 90-day projections. Industry leaders (Distil.ai) achieve 98-99% accuracy vs. 60% with manual forecasting. Proactive alerts when projected cash drops below safety threshold. **Requires months of historical data to be useful. ML model development is complex and time-consuming.** | P3 |
| **Multi-Period Comparison** | Compare financial metrics across periods: P&L comparison (monthly, quarterly, yearly), budget vs. actual, same-period-last-year. Variance highlighting. | P1 |
| **Multi-Location Comparison** | Side-by-side financial comparison across locations: revenue, prime cost %, food cost %, labor cost %, controllable profit, cash variance. Rank locations by each KPI. Identify top performers and underperformers. Drill-down from consolidated to location-specific data. Essential for multi-unit operators. **Requires multi-location support (P2) to be built first.** | P2 |
| **Above-Store Dashboard** | Executive-level consolidated view for owners/CFOs managing multiple locations. Total portfolio revenue, consolidated P&L, aggregate prime cost, cash position across all accounts, location performance heatmap. Auto-emailed weekly/monthly financial summary. **Only relevant for restaurant groups with 3+ locations.** | P3 |
| **Automated Report Scheduling** | Configure scheduled report delivery: daily flash report (every morning), weekly P&L summary (every Monday), monthly financial package (1st of month). Recipients configurable per report. Email delivery with PDF attachment. Per-location or consolidated. Reduces accountant/manager time by 30% (industry benchmark from Distil.ai). **Requires email infrastructure and background job scheduler.** | P2 |
| **Real-Time Variance Alerts** | Push notifications / webhook alerts when: prime cost exceeds threshold (e.g., > 65%), food cost spikes > X% vs. 7-day average, labor cost deviating from budget by > Y%, cash variance detected in drawer closing, unusual expense pattern (e.g., same vendor billed twice), payout settlement delayed beyond expected window. Configurable thresholds per metric, per location. Alert channels: in-app, email, webhook (for integration with Slack/Teams). **Requires background monitoring jobs and notification infrastructure.** | P2 |
| **Manual Data Entry (Standalone)** | For standalone mode: enter daily revenue totals, COGS estimates, labor costs, and operating expenses manually. CSV import for historical data. Templates for recurring entries. | P0 |

---

## 3. Domain Entities (Data Model)

### 3.1 Core Entities (Own Database)

**Important:** All entities belong to this standalone system. `Organization` replaces BonApp's `YumYumYard`. `AppUser` is managed by this system's own ASP.NET Identity. All monetary fields use `decimal` for precision.

```
Organization (the restaurant — own entity, NOT BonApp's YumYumYard)
├── Id (int, PK)
├── Name (string, required) — restaurant name
├── Slug (string, unique) — URL-friendly identifier
├── Address (string, nullable)
├── City (string, nullable)
├── PostalCode (string, nullable)
├── Country (string, nullable) default "CH"
├── Canton (string, nullable) — Swiss canton (e.g., "ZH", "GE", "VD")
├── Phone (string, nullable)
├── Email (string, nullable)
├── LogoUrl (string, nullable)
├── Currency (string) default "CHF"
├── Timezone (string) default "Europe/Zurich"
├── VatNumber (string, nullable) — Swiss UID (e.g., "CHE-123.456.789 MWST")
├── FiscalYearStartMonth (int) default 1 — January
├── DefaultPaymentTermsDays (int) default 30
├── InvoiceNumberPrefix (string) default "INV"
├── NextInvoiceNumber (int) default 1
├── PrimeCostTargetPercent (decimal) default 60
├── FoodCostTargetPercent (decimal) default 30
├── LaborCostTargetPercent (decimal) default 30
├── DefaultVatRate (decimal) default 8.1 — Swiss standard rate
├── AutoCloseDay (bool) default false — auto-close at midnight
├── AutoCloseDayTime (TimeSpan, nullable) — e.g., 02:00 AM
├── IsActive (bool)
├── CreatedAt (DateTime)
├── SubscriptionPlan (enum: Free, Starter, Professional, Enterprise) default Free

AppUser (own ASP.NET Identity user — NOT BonApp's ApplicationUser)
├── Id (string, PK) — IdentityUser base
├── OrganizationId (int, FK → Organization)
├── FirstName (string, required)
├── LastName (string, required)
├── Role (enum: Owner, Accountant, Manager, Staff) 
├── IsActive (bool)
├── CreatedAt (DateTime)
├── LastLoginAt (DateTime, nullable)
```

### 3.2 Chart of Accounts & Ledger Entities

```
LedgerAccount (Chart of Accounts — Swiss Kontenrahmen KMU)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── AccountNumber (string, required) — e.g., "1000", "1020", "3000"
├── Name (string, required) — e.g., "Cash", "Bank UBS", "Revenue Food"
├── AccountType (enum: Asset, Liability, Equity, Revenue, Expense)
├── AccountSubType (string, nullable) — e.g., "Current Asset", "Long-Term Liability"
├── ParentAccountId (int, FK → LedgerAccount, nullable) — for account hierarchy
├── NormalBalance (enum: Debit, Credit) — which side increases the account
├── IsSystemAccount (bool) — seeded accounts cannot be deleted
├── IsActive (bool)
├── Description (string, nullable)
├── TaxRateId (int, FK → TaxRate, nullable) — default VAT rate for this account
├── DisplayOrder (int)
├── CurrentBalance (decimal) default 0 — running balance (updated on journal post)
├── CreatedAt (DateTime)

JournalEntry (double-entry bookkeeping transaction)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── LocationId (int, FK → Location, nullable) — null for single-location orgs or org-level entries
├── EntryNumber (string, auto-generated) — sequential: "JE-2026-00001"
├── EntryDate (DateTime) — the financial date (may differ from CreatedAt)
├── Description (string, required) — e.g., "Daily sales 2026-02-09"
├── Source (enum: Auto_POS_Sale, Auto_Refund, Auto_Payout, Auto_Expense, Auto_Invoice, Auto_CloseDay, Auto_Depreciation, Auto_GiftCard_Issue, Auto_GiftCard_Redeem, Auto_Breakage, Auto_Chargeback, Manual)
├── SourceReferenceType (string, nullable) — e.g., "DailySalesSummary", "Refund", "Payout", "Expense", "Invoice"
├── SourceReferenceId (int, nullable) — FK to the record that generated this entry
├── Status (enum: Draft, Posted, Voided)
├── IsReversed (bool) — if this entry has been reversed
├── ReversalOfId (int, FK → JournalEntry, nullable) — if this is a reversal, points to original
├── FiscalPeriodId (int, FK → FiscalPeriod)
├── PostedAt (DateTime, nullable)
├── PostedBy (string, FK → AppUser, nullable)
├── CreatedAt (DateTime)
├── CreatedBy (string, FK → AppUser)
├── Notes (string, nullable)

JournalLine (individual debit or credit line within a journal entry)
├── Id (int, PK)
├── JournalEntryId (int, FK → JournalEntry)
├── LedgerAccountId (int, FK → LedgerAccount)
├── DebitAmount (decimal) default 0
├── CreditAmount (decimal) default 0
├── Description (string, nullable) — line-level description
├── TaxRateId (int, FK → TaxRate, nullable)
├── TaxAmount (decimal) default 0
├── CostCenterId (int, FK → CostCenter, nullable) — optional cost allocation
├── DisplayOrder (int)

FiscalPeriod (accounting periods)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── PeriodName (string) — e.g., "January 2026", "Q1 2026", "FY 2026"
├── PeriodType (enum: Month, Quarter, Year)
├── StartDate (DateTime)
├── EndDate (DateTime)
├── Status (enum: Open, Closed, Locked)
├── ClosedAt (DateTime, nullable)
├── ClosedBy (string, FK → AppUser, nullable)

CostCenter (optional cost allocation)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── Name (string) — e.g., "Kitchen", "Bar", "Terrace", "Catering"
├── Code (string) — e.g., "CC01", "CC02"
├── IsActive (bool)
```

### 3.3 Payment & Sales Entities

```
PaymentSource (any system that sends payment data — POS, terminal, online, manual)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── SourceName (string, required) — e.g., "BonApp POS", "Worldline Terminal", "Stripe Online", "Cash Register", "Manual"
├── SourceType (enum: POS, Terminal, OnlinePayment, CashRegister, Manual)
├── ApiKey (string, encrypted) — for API authentication
├── WebhookSecret (string, encrypted, nullable) — for verifying incoming webhooks
├── IsActive (bool)
├── LastSyncAt (DateTime, nullable)
├── CreatedAt (DateTime)

PaymentRecord (individual payment transaction)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── LocationId (int, FK → Location, nullable) — null for single-location orgs
├── PaymentSourceId (int, FK → PaymentSource)
├── ExternalPaymentId (string, nullable) — ID from the payment provider
├── ExternalOrderId (string, nullable) — order reference from POS
├── PaymentDate (DateTime)
├── Amount (decimal) — gross payment amount
├── Currency (string) default "CHF"
├── PaymentMethod (enum: Card, Cash, OnlineCard, Twint, ApplePay, GooglePay, BankTransfer, Voucher, Other)
├── PaymentProvider (string, nullable) — e.g., "Worldline", "Stripe", "Wallee"
├── PaymentType (enum: Capture, Authorization, Refund, Void, Chargeback)
├── Status (enum: Pending, Completed, Failed, Cancelled)
├── NetAmount (decimal) — amount after VAT
├── VatAmount (decimal) — total VAT on this payment
├── TipAmount (decimal) default 0
├── ProviderFeeAmount (decimal, nullable) — processing fee charged by provider
├── CardBrand (string, nullable) — "Visa", "Mastercard", "Amex"
├── CardLastFour (string, nullable) — "****1234"
├── OriginalPaymentId (int, FK → PaymentRecord, nullable) — for refunds: points to original
├── JournalEntryId (int, FK → JournalEntry, nullable) — the auto-generated journal entry
├── RowVersion (byte[]) — optimistic concurrency token
├── CreatedAt (DateTime)
├── Notes (string, nullable)

PaymentVatLine (VAT breakdown per payment — supports multi-rate)
├── Id (int, PK)
├── PaymentRecordId (int, FK → PaymentRecord)
├── TaxRateId (int, FK → TaxRate)
├── NetAmount (decimal) — net amount at this rate
├── VatAmount (decimal) — VAT at this rate
├── GrossAmount (decimal) — computed: Net + VAT

DailySalesSummary (aggregated daily figures — foundation for close-day)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── LocationId (int, FK → Location, nullable) — null for single-location orgs
├── SaleDate (DateTime) — date only
├── GrossRevenue (decimal) — total before refunds
├── RefundTotal (decimal) — total refunds for the day
├── NetRevenue (decimal) — gross - refunds
├── CashTotal (decimal) — cash payments
├── CardTotal (decimal) — card payments (all types)
├── OnlineTotal (decimal) — online payments
├── OtherTotal (decimal) — vouchers, bank transfers, etc.
├── TipTotal (decimal) — total tips
├── TipCash (decimal) — cash tips
├── TipCard (decimal) — card tips
├── TransactionCount (int) — number of transactions
├── AverageTicket (decimal) — NetRevenue / TransactionCount
├── VatStandard (decimal) — VAT collected at 8.1%
├── VatReduced (decimal) — VAT collected at 2.6%
├── VatAccommodation (decimal) — VAT collected at 3.8%
├── Status (enum: Open, Closed, Locked)
├── ClosedAt (DateTime, nullable)
├── ClosedBy (string, FK → AppUser, nullable)
├── JournalEntryId (int, FK → JournalEntry, nullable)
├── CreatedAt (DateTime)

Refund (detailed refund tracking)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── OriginalPaymentId (int, FK → PaymentRecord) — the original payment being refunded
├── RefundPaymentId (int, FK → PaymentRecord) — the refund payment record
├── Amount (decimal)
├── Reason (enum: CustomerComplaint, OrderError, QualityIssue, Duplicate, Fraud, Other)
├── ReasonDescription (string, nullable)
├── InitiatedBy (string, FK → AppUser)
├── ApprovedBy (string, FK → AppUser, nullable)
├── Status (enum: Pending, Approved, Processed, Rejected)
├── ProcessedAt (DateTime, nullable)
├── CreatedAt (DateTime)

Chargeback (dispute tracking)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── PaymentRecordId (int, FK → PaymentRecord) — the disputed payment
├── ChargebackAmount (decimal)
├── ChargebackFee (decimal, nullable) — fee from provider
├── Reason (string, nullable) — chargeback reason code from provider
├── ExternalDisputeId (string, nullable) — dispute ID from provider
├── Status (enum: Open, UnderReview, Won, Lost)
├── EvidenceDueDate (DateTime, nullable) — deadline for evidence submission
├── ResolvedAt (DateTime, nullable)
├── JournalEntryId (int, FK → JournalEntry, nullable)
├── CreatedAt (DateTime)
├── Notes (string, nullable)

CashDrawer (cash management per shift)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── LocationId (int, FK → Location, nullable) — null for single-location orgs
├── DrawerName (string) — e.g., "Register 1", "Bar Drawer"
├── OpenedAt (DateTime)
├── ClosedAt (DateTime, nullable)
├── OpenedBy (string, FK → AppUser)
├── ClosedBy (string, FK → AppUser, nullable)
├── OpeningFloat (decimal) — starting cash
├── ExpectedClosing (decimal, nullable) — calculated: float + cash sales - cash refunds - drops
├── ActualClosing (decimal, nullable) — physically counted
├── Variance (decimal, nullable) — actual - expected
├── CashSalesTotal (decimal) default 0
├── CashRefundsTotal (decimal) default 0
├── CashDropsTotal (decimal) default 0 — mid-shift cash removals
├── CashTipsTotal (decimal) default 0
├── Status (enum: Open, Closed, Reconciled)
├── Notes (string, nullable)
```

### 3.4 Payout & Bank Reconciliation Entities

```
BankAccount (registered bank accounts)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── BankName (string) — e.g., "UBS", "Credit Suisse", "PostFinance", "Raiffeisen"
├── AccountName (string) — e.g., "Main Business Account"
├── IBAN (string, required) — Swiss IBAN (CH...)
├── Currency (string) default "CHF"
├── LedgerAccountId (int, FK → LedgerAccount) — linked chart of accounts entry
├── IsActive (bool)
├── CreatedAt (DateTime)

PaymentProviderAccount (payment processor accounts for payout tracking)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── ProviderName (string) — "Worldline", "Stripe", "Wallee", "SumUp"
├── AccountIdentifier (string, nullable) — merchant ID or account reference
├── SettlementBankAccountId (int, FK → BankAccount) — where payouts land
├── TypicalSettlementDays (int) default 2 — expected days to receive payout
├── FeePercentage (decimal, nullable) — typical processing fee %
├── FeeFixed (decimal, nullable) — fixed fee per transaction
├── IsActive (bool)
├── CreatedAt (DateTime)

Payout (expected payout from payment provider)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── PaymentProviderAccountId (int, FK → PaymentProviderAccount)
├── PayoutReference (string, nullable) — payout/batch ID from provider
├── ExpectedAmount (decimal) — sum of transactions minus provider fees
├── ActualAmount (decimal, nullable) — actual amount received (after matching)
├── ProviderFees (decimal) default 0 — fees deducted by provider
├── GrossAmount (decimal) — total before fees
├── PayoutDate (DateTime) — date the payout was initiated
├── ExpectedArrivalDate (DateTime, nullable)
├── ActualArrivalDate (DateTime, nullable)
├── Status (enum: Expected, Received, Partial, Missing, Disputed, Written_Off)
├── BankTransactionId (int, FK → BankTransaction, nullable) — matched bank entry
├── Notes (string, nullable)
├── CreatedAt (DateTime)

PayoutLine (individual transactions in a payout batch)
├── Id (int, PK)
├── PayoutId (int, FK → Payout)
├── PaymentRecordId (int, FK → PaymentRecord) — the original payment
├── Amount (decimal) — amount of this transaction in the payout
├── FeeAmount (decimal) default 0 — fee on this specific transaction
├── NetAmount (decimal) — amount - fee

BankTransaction (imported bank statement lines)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── BankAccountId (int, FK → BankAccount)
├── TransactionDate (DateTime)
├── ValueDate (DateTime, nullable) — bank value date
├── Amount (decimal) — positive = credit, negative = debit
├── Currency (string) default "CHF"
├── Description (string) — bank's description text
├── Reference (string, nullable) — payment reference / structured remittance
├── CounterpartyName (string, nullable)
├── CounterpartyIBAN (string, nullable)
├── ImportBatchId (string, nullable) — groups transactions from same import
├── ImportSource (enum: CSV, CAMT053, Manual)
├── Status (enum: Unmatched, Matched, PartiallyMatched, Identified, Excluded)
├── CreatedAt (DateTime)

ReconciliationMatch (links bank transactions to expected payouts/invoices)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── BankTransactionId (int, FK → BankTransaction)
├── MatchedEntityType (enum: Payout, InvoicePayment, Expense, Manual)
├── MatchedEntityId (int) — FK to the matched record
├── MatchedAmount (decimal) — amount allocated to this match
├── ConfidenceScore (enum: Exact, Probable, Manual)
├── MatchMethod (enum: Auto_Amount_Reference, Auto_Amount_Date, Manual)
├── MatchedAt (DateTime)
├── MatchedBy (string, FK → AppUser, nullable) — null if auto-matched
├── Notes (string, nullable)
├── IsConfirmed (bool) — user confirmed the match
```

### 3.5 Invoice & Receivables Entities

```
Customer (B2B clients / debtors)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── CompanyName (string, required) — e.g., "Zurich Events AG"
├── ContactPerson (string, nullable)
├── Email (string, nullable)
├── Phone (string, nullable)
├── Address (string, nullable)
├── City (string, nullable)
├── PostalCode (string, nullable)
├── Country (string, nullable) default "CH"
├── VatNumber (string, nullable) — customer's VAT/UID number
├── PaymentTermsDays (int) default 30
├── CreditLimit (decimal, nullable)
├── OutstandingBalance (decimal) default 0 — running total of unpaid invoices
├── LedgerAccountId (int, FK → LedgerAccount, nullable) — AR sub-account
├── IsActive (bool)
├── CreatedAt (DateTime)
├── Notes (string, nullable)

Invoice (accounts receivable invoices)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── LocationId (int, FK → Location, nullable) — issuing location
├── CustomerId (int, FK → Customer)
├── InvoiceNumber (string, required, unique per org) — e.g., "INV-2026-0001"
├── InvoiceDate (DateTime)
├── DueDate (DateTime) — auto-calculated from payment terms
├── Status (enum: Draft, Sent, PartiallyPaid, Paid, Overdue, Cancelled, WrittenOff)
├── SubTotal (decimal) — sum of line items before VAT
├── VatTotal (decimal) — total VAT
├── GrossTotal (decimal) — subtotal + VAT
├── PaidAmount (decimal) default 0 — total payments received
├── OutstandingAmount (decimal) — computed: GrossTotal - PaidAmount
├── Currency (string) default "CHF"
├── PaymentReference (string, nullable) — QR reference / ESR number
├── Notes (string, nullable)
├── InternalNotes (string, nullable) — not shown to customer
├── ReminderCount (int) default 0 — number of reminders sent
├── LastReminderDate (DateTime, nullable)
├── JournalEntryId (int, FK → JournalEntry, nullable)
├── PdfUrl (string, nullable) — stored PDF
├── CreatedBy (string, FK → AppUser)
├── CreatedAt (DateTime)
├── UpdatedAt (DateTime)

InvoiceLine (line items on an invoice)
├── Id (int, PK)
├── InvoiceId (int, FK → Invoice)
├── Description (string, required) — e.g., "Catering service - Corporate Lunch"
├── Quantity (decimal)
├── UnitPrice (decimal)
├── LineTotal (decimal) — computed: Quantity * UnitPrice
├── TaxRateId (int, FK → TaxRate)
├── VatAmount (decimal) — computed from line total and tax rate
├── CostCenterId (int, FK → CostCenter, nullable)
├── DisplayOrder (int)

InvoicePayment (payments received against invoices)
├── Id (int, PK)
├── InvoiceId (int, FK → Invoice)
├── PaymentDate (DateTime)
├── Amount (decimal)
├── PaymentMethod (enum: BankTransfer, Cash, Card, Other)
├── BankTransactionId (int, FK → BankTransaction, nullable) — matched bank entry
├── Reference (string, nullable)
├── Notes (string, nullable)
├── RecordedBy (string, FK → AppUser)
├── CreatedAt (DateTime)

InvoiceReminder (reminder history)
├── Id (int, PK)
├── InvoiceId (int, FK → Invoice)
├── ReminderLevel (int) — 1st, 2nd, 3rd reminder
├── SentAt (DateTime)
├── SentTo (string) — email address
├── SentBy (string, FK → AppUser, nullable) — null if auto-sent
├── Method (enum: Email, PDF_Download, Print)
├── PdfUrl (string, nullable) — reminder PDF
```

### 3.6 Expense & Vendor Entities

```
Vendor (suppliers / creditors)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── Name (string, required) — e.g., "Swiss Gastro Supplies AG"
├── ContactPerson (string, nullable)
├── Email (string, nullable)
├── Phone (string, nullable)
├── Address (string, nullable)
├── City (string, nullable)
├── PostalCode (string, nullable)
├── Country (string, nullable) default "CH"
├── IBAN (string, nullable) — vendor's bank account
├── VatNumber (string, nullable)
├── PaymentTermsDays (int) default 30
├── OutstandingBalance (decimal) default 0 — running total of unpaid bills
├── LedgerAccountId (int, FK → LedgerAccount, nullable) — AP sub-account
├── IsActive (bool)
├── CreatedAt (DateTime)
├── Notes (string, nullable)

Expense (recorded business expense / supplier bill)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── LocationId (int, FK → Location, nullable) — null for single-location orgs
├── VendorId (int, FK → Vendor, nullable)
├── ExpenseNumber (string, auto-generated) — "EXP-2026-00001"
├── ExpenseDate (DateTime)
├── DueDate (DateTime, nullable)
├── Category (enum: Rent, Utilities, Insurance, Marketing, Maintenance, Supplies, ProfessionalServices, Technology, Licenses, Transportation, BankFees, CreditCardFees, DeliveryCommission, Depreciation, FoodPurchase, BeveragePurchase, Miscellaneous)
├── LedgerAccountId (int, FK → LedgerAccount) — expense account to debit
├── SubTotal (decimal)
├── VatAmount (decimal)
├── GrossTotal (decimal)
├── Currency (string) default "CHF"
├── PaymentStatus (enum: Unpaid, Scheduled, Paid)
├── PaymentDate (DateTime, nullable) — when actually paid
├── PaymentMethod (enum: BankTransfer, Cash, Card, DirectDebit, Other, nullable)
├── IsRecurring (bool) default false
├── RecurrenceRule (string, nullable) — e.g., "MONTHLY", "WEEKLY"
├── PurchaseOrderReference (string, nullable) — PO # from Inventory Pro
├── Status (enum: Draft, PendingApproval, Approved, Posted, Rejected)
├── ApprovedBy (string, FK → AppUser, nullable)
├── ApprovedAt (DateTime, nullable)
├── JournalEntryId (int, FK → JournalEntry, nullable)
├── Notes (string, nullable)
├── CreatedBy (string, FK → AppUser)
├── CreatedAt (DateTime)
├── UpdatedAt (DateTime)

Attachment (file attachments for expenses, invoices, etc.)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── EntityType (string) — "Expense", "Invoice", "Chargeback"
├── EntityId (int) — FK to the parent record
├── FileName (string)
├── FileUrl (string) — Azure Blob Storage URL
├── FileSize (long) — in bytes
├── ContentType (string) — MIME type
├── UploadedBy (string, FK → AppUser)
├── UploadedAt (DateTime)
```

### 3.7 Tax & Compliance Entities

```
TaxRate (VAT rates)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization, nullable) — null for system defaults
├── Name (string) — e.g., "Standard 8.1%", "Reduced 2.6%", "Accommodation 3.8%"
├── Rate (decimal) — e.g., 8.1, 2.6, 3.8, 0
├── TaxType (enum: Standard, Reduced, Accommodation, Exempt, ZeroRated)
├── ValidFrom (DateTime) — Swiss rates can change (last changed Jan 2024)
├── ValidTo (DateTime, nullable) — null = currently active
├── IsDefault (bool) — default rate for new transactions
├── IsActive (bool)

TaxReportPeriod (VAT return periods)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── PeriodType (enum: Monthly, Quarterly, SemiAnnual, Annual)
├── StartDate (DateTime)
├── EndDate (DateTime)
├── TotalRevenue (decimal) — total revenue for the period
├── TotalOutputVat (decimal) — VAT collected on sales (output tax)
├── TotalInputVat (decimal) — VAT paid on purchases (input tax)
├── NetVatPayable (decimal) — output - input
├── Revenue_Standard (decimal) — revenue at standard rate
├── Revenue_Reduced (decimal) — revenue at reduced rate
├── Revenue_Accommodation (decimal) — revenue at accommodation rate
├── Revenue_Exempt (decimal) — exempt revenue
├── Vat_Standard (decimal) — VAT at standard rate
├── Vat_Reduced (decimal) — VAT at reduced rate
├── Vat_Accommodation (decimal) — VAT at accommodation rate
├── Status (enum: Draft, Submitted, Filed, Corrected)
├── FiledAt (DateTime, nullable)
├── CreatedAt (DateTime)
├── CreatedBy (string, FK → AppUser)

AuditLog (immutable audit trail — required by Swiss GeBüV)
├── Id (long, PK)
├── OrganizationId (int, FK → Organization)
├── UserId (string, FK → AppUser)
├── Action (string) — e.g., "JournalEntry.Created", "Invoice.Sent", "Day.Closed"
├── EntityType (string) — e.g., "JournalEntry", "Invoice", "Expense"
├── EntityId (int)
├── OldValues (string, JSON, nullable) — serialized before-state
├── NewValues (string, JSON, nullable) — serialized after-state
├── IpAddress (string, nullable)
├── UserAgent (string, nullable)
├── Timestamp (DateTime) — UTC
```

### 3.8 New Entities from Research (Daily Flash, Budget, Delivery, Gift Cards, Fixed Assets, Multi-Location)

```
DailyFlashReport (auto-generated daily operational snapshot — industry's #1 report)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── LocationId (int, FK → Location, nullable) — null for single-location orgs
├── ReportDate (DateTime) — date of the business day
├── NetSales (decimal)
├── GrossRevenue (decimal)
├── RefundTotal (decimal)
├── GuestCount (int, nullable) — from POS or manual
├── AverageCheck (decimal, nullable) — NetSales / GuestCount
├── FoodCost (decimal, nullable) — from Inventory Pro or manual
├── FoodCostPercent (decimal, nullable)
├── BeverageCost (decimal, nullable)
├── BeverageCostPercent (decimal, nullable)
├── LaborCost (decimal, nullable) — from Staff Pro or manual
├── LaborCostPercent (decimal, nullable)
├── PrimeCost (decimal, nullable) — COGS + Labor
├── PrimeCostPercent (decimal, nullable)
├── ControllableProfit (decimal, nullable)
├── ControllableProfitPercent (decimal, nullable)
├── CashVariance (decimal, nullable) — from cash drawer closing
├── SameDayLastWeekSales (decimal, nullable) — for comparison
├── SameDayLastYearSales (decimal, nullable) — for comparison
├── WeekOverWeekChange (decimal, nullable) — % change
├── YearOverYearChange (decimal, nullable) — % change
├── Alerts (string, JSON, nullable) — any anomalies flagged
├── GeneratedAt (DateTime)
├── EmailedTo (string, nullable) — recipients who received this report
├── EmailedAt (DateTime, nullable)

Location (for multi-location restaurant groups)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization) — parent org
├── Name (string, required) — e.g., "Downtown Zürich", "Airport Location"
├── Code (string) — short code e.g., "ZRH-01"
├── Address (string, nullable)
├── City (string, nullable)
├── PostalCode (string, nullable)
├── ManagerUserId (string, FK → AppUser, nullable)
├── IsHeadquarters (bool) default false
├── IsActive (bool)
├── CreatedAt (DateTime)

Budget (annual/monthly financial budget per P&L line)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── LocationId (int, FK → Location, nullable)
├── BudgetName (string) — e.g., "2026 Annual Budget", "Q1 2026"
├── FiscalYear (int) — e.g., 2026
├── Status (enum: Draft, Approved, Locked)
├── ApprovedBy (string, FK → AppUser, nullable)
├── ApprovedAt (DateTime, nullable)
├── CreatedBy (string, FK → AppUser)
├── CreatedAt (DateTime)

BudgetLine (individual budget allocation per account per month)
├── Id (int, PK)
├── BudgetId (int, FK → Budget)
├── LedgerAccountId (int, FK → LedgerAccount)
├── Month (int) — 1-12
├── BudgetedAmount (decimal)
├── ActualAmount (decimal, nullable) — populated on month close
├── VarianceAmount (decimal, nullable) — computed: Actual - Budgeted
├── VariancePercent (decimal, nullable) — computed: (Variance / Budgeted) * 100
├── Notes (string, nullable)

DeliveryPlatform (3rd-party delivery tracking — Uber Eats, DoorDash, eat.ch, Smood, etc.)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── PlatformName (string) — "Uber Eats", "DoorDash", "Just Eat", "eat.ch", "Smood"
├── CommissionPercent (decimal) — typical 15-25%
├── FixedFeePerOrder (decimal, nullable) — some platforms charge a fixed fee per order
├── MarketingFeePercent (decimal, nullable) — promotional/marketing surcharge
├── AccountIdentifier (string, nullable) — restaurant's ID on the platform
├── PaymentSourceId (int, FK → PaymentSource, nullable) — linked payment source for payouts
├── IsActive (bool)
├── CreatedAt (DateTime)

DeliveryPlatformPayout (reconciliation of delivery platform settlements)
├── Id (int, PK)
├── DeliveryPlatformId (int, FK → DeliveryPlatform)
├── OrganizationId (int, FK → Organization)
├── PayoutPeriodStart (DateTime)
├── PayoutPeriodEnd (DateTime)
├── GrossOrderTotal (decimal) — total customer order value
├── CommissionDeducted (decimal) — platform commission
├── MarketingFeeDeducted (decimal, nullable)
├── TipAmount (decimal, nullable) — driver tips passed through or withheld
├── AdjustmentAmount (decimal, nullable) — refunds, error corrections from platform
├── NetPayoutExpected (decimal) — what restaurant should receive
├── NetPayoutActual (decimal, nullable) — what was actually deposited
├── Variance (decimal, nullable) — Expected - Actual
├── OrderCount (int) — number of orders in this payout
├── EffectiveCostPerOrder (decimal, nullable) — computed: CommissionDeducted / OrderCount
├── EffectiveCostPercent (decimal, nullable) — computed: CommissionDeducted / GrossOrderTotal * 100
├── BankTransactionId (int, FK → BankTransaction, nullable) — matched bank entry
├── Status (enum: Expected, Received, Partial, Missing, Disputed)
├── ImportSource (string, nullable) — "CSV", "API", "Manual"
├── CreatedAt (DateTime)

GiftCard (gift card / voucher liability tracking)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── CardNumber (string, unique) — gift card code/number
├── IssuedDate (DateTime)
├── ExpiryDate (DateTime, nullable) — Swiss law: generally no expiry required
├── InitialValue (decimal) — face value when issued
├── CurrentBalance (decimal) — remaining balance
├── Status (enum: Active, FullyRedeemed, Expired, Cancelled)
├── PurchaserName (string, nullable)
├── RecipientName (string, nullable)
├── PaymentRecordId (int, FK → PaymentRecord, nullable) — original purchase payment
├── JournalEntryId (int, FK → JournalEntry, nullable) — liability journal entry
├── CreatedAt (DateTime)

GiftCardTransaction (redemption / top-up / adjustment history)
├── Id (int, PK)
├── GiftCardId (int, FK → GiftCard)
├── TransactionType (enum: Issue, Redeem, TopUp, Adjust, Expire, Cancel)
├── Amount (decimal) — positive = add balance, negative = redeem
├── BalanceAfter (decimal)
├── LocationId (int, FK → Location, nullable) — which location redeemed
├── PaymentRecordId (int, FK → PaymentRecord, nullable) — linked payment
├── JournalEntryId (int, FK → JournalEntry, nullable) — revenue recognition on redemption
├── CreatedAt (DateTime)
├── CreatedBy (string, FK → AppUser)
├── Notes (string, nullable)

GiftCardBreakage (periodic breakage revenue recognition)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── PeriodStart (DateTime)
├── PeriodEnd (DateTime)
├── TotalOutstandingBalance (decimal) — total unredeemed gift card balance
├── EstimatedBreakageRate (decimal) — typically 10-15%
├── BreakageRevenueRecognized (decimal) — amount recognized as revenue
├── JournalEntryId (int, FK → JournalEntry, nullable)
├── CreatedAt (DateTime)

FixedAsset (restaurant equipment and depreciation tracking)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── LocationId (int, FK → Location, nullable)
├── AssetName (string) — e.g., "Commercial Oven", "POS Terminal #3"
├── AssetCategory (enum: KitchenEquipment, Furniture, Technology, Vehicle, BuildingImprovement, Signage, Other)
├── PurchaseDate (DateTime)
├── PurchaseCost (decimal)
├── UsefulLifeMonths (int) — e.g., 60 months (5 years)
├── DepreciationMethod (enum: StraightLine, DecliningBalance)
├── SalvageValue (decimal) default 0
├── AccumulatedDepreciation (decimal) default 0
├── BookValue (decimal) — computed: PurchaseCost - AccumulatedDepreciation
├── LedgerAccountId (int, FK → LedgerAccount) — asset account
├── DepreciationAccountId (int, FK → LedgerAccount) — depreciation expense account
├── AccumulatedDepreciationAccountId (int, FK → LedgerAccount) — contra-asset account
├── VendorId (int, FK → Vendor, nullable) — who supplied it
├── SerialNumber (string, nullable)
├── WarrantyExpiry (DateTime, nullable)
├── Status (enum: Active, Disposed, WrittenOff)
├── DisposedDate (DateTime, nullable)
├── DisposalAmount (decimal, nullable)
├── Notes (string, nullable)
├── CreatedAt (DateTime)

ScheduledReport (automated report email delivery)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── ReportType (enum: DailyFlash, WeeklyPL, MonthlyFinancials, PrimeCostDaily, ReconciliationSummary, BudgetVariance)
├── Frequency (enum: Daily, Weekly, Monthly)
├── DeliveryTime (TimeSpan) — e.g., 06:00 (6 AM)
├── DeliveryDayOfWeek (int, nullable) — for weekly: 1=Monday
├── DeliveryDayOfMonth (int, nullable) — for monthly: 1-28
├── Recipients (string) — comma-separated email addresses
├── IncludeAllLocations (bool) default true
├── LocationId (int, FK → Location, nullable) — specific location, or null for consolidated
├── Format (enum: PDF, CSV, Both)
├── IsActive (bool)
├── LastSentAt (DateTime, nullable)
├── CreatedAt (DateTime)
├── CreatedBy (string, FK → AppUser)

VarianceAlert (real-time financial anomaly alerts)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── LocationId (int, FK → Location, nullable)
├── AlertType (enum: PrimeCostExceeded, FoodCostSpike, LaborCostOverBudget, CashVariance, UnusualExpense, PayoutDelayed, InvoiceOverdue, BudgetVarianceExceeded)
├── Severity (enum: Info, Warning, Critical)
├── Title (string) — e.g., "Prime cost exceeded 65% threshold"
├── Message (string) — detailed description with numbers
├── MetricValue (decimal, nullable) — the actual value that triggered the alert
├── ThresholdValue (decimal, nullable) — the threshold that was exceeded
├── ReferenceEntityType (string, nullable) — e.g., "DailySalesSummary", "CashDrawer"
├── ReferenceEntityId (int, nullable)
├── IsRead (bool) default false
├── IsDismissed (bool) default false
├── CreatedAt (DateTime)
├── ReadAt (DateTime, nullable)
├── ReadBy (string, FK → AppUser, nullable)
```

### 3.9 Integration Entities

```
IntegrationConnection (any external system that sends/receives data)
├── Id (int, PK)
├── OrganizationId (int, FK → Organization)
├── SystemName (string) — "BonApp POS", "Inventory Pro", "Staff Pro", "Lightspeed", etc.
├── SystemType (enum: POS, InventorySystem, StaffSystem, AccountingExport, Custom)
├── ApiKey (string, encrypted) — for inbound API authentication
├── WebhookSecret (string, encrypted, nullable) — HMAC signing
├── WebhookUrl (string, nullable) — their endpoint for outbound webhooks
├── IsActive (bool)
├── LastSyncAt (DateTime, nullable)
├── Config (string, JSON, nullable) — system-specific configuration
├── CreatedAt (DateTime)

OutboundWebhookSubscription (webhooks we send out)
├── Id (int, PK)
├── IntegrationConnectionId (int, FK → IntegrationConnection)
├── EventType (enum: TaxReportReady, PayoutMismatchDetected, DayClosed, InvoiceOverdue, CashVariance)
├── TargetUrl (string) — external system's webhook URL
├── Secret (string, encrypted) — HMAC signing secret
├── IsActive (bool)
├── CreatedAt (DateTime)

OutboundWebhookDeliveryLog (delivery tracking)
├── Id (int, PK)
├── SubscriptionId (int, FK → OutboundWebhookSubscription)
├── EventType (string)
├── Payload (string, JSON)
├── ResponseStatusCode (int, nullable)
├── ResponseBody (string, nullable)
├── AttemptNumber (int)
├── DeliveredAt (DateTime)
├── IsSuccess (bool)
```

### 3.10 Entity Relationship Summary (Updated)

```
Organization ─┬── Location (many) ─┬── DailyFlashReport (many)
              │                     ├── DailySalesSummary (many) ← also at org level
              │                     ├── PaymentRecord (many) ← also at org level
              │                     ├── JournalEntry (many) ← also at org level
              │                     ├── Invoice (many) ← also at org level
              │                     ├── Expense (many) ← also at org level
              │                     ├── CashDrawer (many) ← also at org level
              │                     ├── Budget (many) ← also at org level
              │                     ├── FixedAsset (many) ← also at org level
              │                     ├── VarianceAlert (many) ← also at org level
              │                     ├── ScheduledReport (many) ← also at org level
              │                     └── GiftCardTransaction (many) ← cross-location redemption
              │
              ├── AppUser (many)
              ├── LedgerAccount (many) ─── JournalLine (many)
              ├── JournalEntry (many) ──── JournalLine (many)
              ├── FiscalPeriod (many)
              ├── CostCenter (many)
              ├── Budget (many) ──── BudgetLine (many)
              ├── PaymentSource (many) ─── PaymentRecord (many) ─── PaymentVatLine (many)
              ├── DailySalesSummary (many)
              ├── DailyFlashReport (many)
              ├── Refund (many)
              ├── Chargeback (many)
              ├── CashDrawer (many)
              ├── BankAccount (many) ──── BankTransaction (many)
              ├── PaymentProviderAccount (many) ── Payout (many) ── PayoutLine (many)
              ├── ReconciliationMatch (many)
              ├── DeliveryPlatform (many) ── DeliveryPlatformPayout (many)
              ├── GiftCard (many) ──── GiftCardTransaction (many)
              ├── GiftCardBreakage (many)
              ├── Customer (many) ──── Invoice (many) ── InvoiceLine (many)
              │                                       ── InvoicePayment (many)
              │                                       ── InvoiceReminder (many)
              ├── Vendor (many) ──── Expense (many) ── Attachment (many)
              ├── FixedAsset (many)
              ├── TaxRate (many)
              ├── TaxReportPeriod (many)
              ├── IntegrationConnection (many) ── OutboundWebhookSubscription (many)
              ├── ScheduledReport (many)
              ├── VarianceAlert (many)
              └── AuditLog (many)

Key: LocationId is NULLABLE on entities marked "← also at org level".
     When LocationId is NULL, the record applies to the entire organization (single-location mode).
     When LocationId is set, the record belongs to a specific location (multi-location mode).

Total Entities: ~55+ (up from ~40 in initial version)
```

---

## 4. API Endpoints to Build

### 4.0 Authentication API (`/api/auth`)

These endpoints handle user authentication, registration, and token management for the Finance Pro frontend.

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | POST | `/register` | None | Register new user + create organization | `RegisterDto` | `AuthResponseDto` |
| 2 | POST | `/login` | None | Login with email/password | `LoginDto` | `AuthResponseDto` (access + refresh tokens) |
| 3 | POST | `/refresh` | Refresh Token | Refresh expired access token | `RefreshTokenDto` | `AuthResponseDto` |
| 4 | POST | `/forgot-password` | None | Send password reset email | `ForgotPasswordDto` | `ServiceResponseDto` |
| 5 | POST | `/reset-password` | None | Reset password with token | `ResetPasswordDto` | `ServiceResponseDto` |
| 6 | GET | `/me` | Bearer | Get current user profile | - | `UserProfileDto` |
| 7 | PUT | `/me` | Bearer | Update profile (name, language) | `UpdateProfileDto` | `UserProfileDto` |
| 8 | POST | `/invite` | Owner | Invite user to organization | `InviteUserDto` | `ServiceResponseDto` |
| 9 | POST | `/accept-invite` | None | Accept invitation + set password | `AcceptInviteDto` | `AuthResponseDto` |

### 4.1 Inbound Integration API — Payment Sources (`/api/pos/v1`)

These endpoints are called by external POS systems (BonApp, Lightspeed, etc.) to push payment and order data into Finance Pro. Authenticated via API key (not Bearer token).

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | POST | `/payments` | API Key | Ingest a payment event (capture, refund, tip, chargeback) | `IngestPaymentDto` | `PaymentReceiptDto` |
| 2 | POST | `/payments/batch` | API Key | Ingest multiple payments at once | `List<IngestPaymentDto>` | `BatchResultDto` |
| 3 | POST | `/orders` | API Key | Ingest order with gross/net/tax line breakdown | `IngestOrderDto` | `OrderReceiptDto` |
| 4 | POST | `/daily-sales` | API Key | Submit aggregated daily sales summary | `DailySalesSummaryDto` | `ServiceResponseDto` |
| 5 | GET | `/status` | API Key | Health check for integration connectivity | - | `IntegrationStatusDto` |

### 4.2 Inbound Integration API — Inventory System (`/api/inventory/v1`)

Called by Inventory Pro or any inventory system to push COGS data.

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | POST | `/cogs` | API Key | Submit COGS summary for a period | `CogsSummaryDto` | `ServiceResponseDto` |
| 2 | POST | `/supplier-bills` | API Key | Push supplier bill data for expense matching | `SupplierBillDto` | `ServiceResponseDto` |

### 4.3 Inbound Integration API — Staff System (`/api/staff/v1`)

Called by Staff Pro or any staff/payroll system to push labor cost data.

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | POST | `/labor-summary` | API Key | Submit labor cost summary for a period | `LaborSummaryDto` | `ServiceResponseDto` |
| 2 | POST | `/payroll` | API Key | Push payroll export data | `PayrollExportDto` | `ServiceResponseDto` |

### 4.4 Finance API — Chart of Accounts (`/api/finance/accounts`)

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | GET | `/{orgId}` | Manager | List all ledger accounts (tree structure) | Query: accountType?, search?, isActive? | `List<LedgerAccountDto>` |
| 2 | GET | `/{orgId}/{accountId}` | Manager | Get account detail with running balance | - | `LedgerAccountDetailDto` |
| 3 | POST | `/{orgId}` | Owner/Accountant | Create ledger account | `CreateLedgerAccountDto` | `LedgerAccountDto` |
| 4 | PUT | `/{orgId}/{accountId}` | Owner/Accountant | Update ledger account | `UpdateLedgerAccountDto` | `LedgerAccountDto` |
| 5 | DELETE | `/{orgId}/{accountId}` | Owner | Deactivate account (only if no journal lines) | - | `ServiceResponseDto` |
| 6 | POST | `/{orgId}/seed` | Owner | Re-seed Swiss default chart of accounts | - | `ServiceResponseDto` |
| 7 | GET | `/{orgId}/{accountId}/transactions` | Manager | Get all journal lines for this account | Query: startDate, endDate, page, pageSize | `PaginatedResponse<JournalLineDto>` |

### 4.5 Finance API — Journal Entries (`/api/finance/journal`)

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | GET | `/{orgId}` | Manager | List journal entries (paginated, filterable) | Query: startDate, endDate, source?, status?, page, pageSize | `PaginatedResponse<JournalEntryDto>` |
| 2 | GET | `/{orgId}/{journalId}` | Manager | Get journal entry with all lines | - | `JournalEntryDetailDto` |
| 3 | POST | `/{orgId}` | Accountant | Create manual journal entry | `CreateJournalEntryDto` | `JournalEntryDto` |
| 4 | PUT | `/{orgId}/{journalId}` | Accountant | Update draft journal entry | `UpdateJournalEntryDto` | `JournalEntryDto` |
| 5 | POST | `/{orgId}/{journalId}/post` | Accountant | Post a draft entry (makes it permanent) | - | `JournalEntryDto` |
| 6 | POST | `/{orgId}/{journalId}/void` | Owner | Void a posted entry (creates reversal) | `VoidReasonDto` | `JournalEntryDto` |
| 7 | GET | `/{orgId}/trial-balance` | Accountant | Get trial balance for a period | Query: asOfDate | `TrialBalanceDto` |

### 4.6 Finance API — Payments (`/api/finance/payments`)

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | GET | `/{orgId}` | Manager | List payments (paginated, filterable) | Query: startDate, endDate, method?, source?, type?, page, pageSize | `PaginatedResponse<PaymentRecordDto>` |
| 2 | GET | `/{orgId}/{paymentId}` | Manager | Get payment detail | - | `PaymentRecordDetailDto` |
| 3 | POST | `/{orgId}/manual` | Manager | Create manual payment entry (standalone mode) | `CreateManualPaymentDto` | `PaymentRecordDto` |
| 4 | GET | `/{orgId}/daily-summary` | Manager | Get daily sales summary | Query: date | `DailySalesSummaryDto` |
| 5 | GET | `/{orgId}/daily-summaries` | Manager | List daily summaries for period | Query: startDate, endDate | `List<DailySalesSummaryDto>` |

### 4.7 Finance API — Refunds & Chargebacks (`/api/finance/disputes`)

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | GET | `/{orgId}/refunds` | Manager | List refunds | Query: startDate, endDate, status?, page, pageSize | `PaginatedResponse<RefundDto>` |
| 2 | POST | `/{orgId}/refunds` | Manager | Create manual refund | `CreateRefundDto` | `RefundDto` |
| 3 | PUT | `/{orgId}/refunds/{refundId}/approve` | Owner | Approve pending refund | - | `RefundDto` |
| 4 | GET | `/{orgId}/chargebacks` | Manager | List chargebacks | Query: status?, page, pageSize | `PaginatedResponse<ChargebackDto>` |
| 5 | PUT | `/{orgId}/chargebacks/{chargebackId}` | Manager | Update chargeback status | `UpdateChargebackDto` | `ChargebackDto` |

### 4.8 Finance API — Close Day / Close Month (`/api/finance/close`)

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | GET | `/{orgId}/close-day/preview` | Manager | Preview close-day: totals, journal entries to generate | Query: date | `CloseDayPreviewDto` |
| 2 | POST | `/{orgId}/close-day` | Manager | Execute close-day for a date | `CloseDayRequestDto` | `CloseDayResultDto` |
| 3 | POST | `/{orgId}/reopen-day` | Owner | Reopen a closed day for corrections | `ReopenDayRequestDto` | `ServiceResponseDto` |
| 4 | GET | `/{orgId}/close-month/preview` | Accountant | Preview close-month | Query: year, month | `CloseMonthPreviewDto` |
| 5 | POST | `/{orgId}/close-month` | Owner | Execute close-month | `CloseMonthRequestDto` | `CloseMonthResultDto` |

### 4.9 Finance API — Bank Reconciliation (`/api/finance/reconciliation`)

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | GET | `/{orgId}/bank-accounts` | Manager | List registered bank accounts | - | `List<BankAccountDto>` |
| 2 | POST | `/{orgId}/bank-accounts` | Owner | Register a bank account | `CreateBankAccountDto` | `BankAccountDto` |
| 3 | POST | `/{orgId}/bank-transactions/import` | Accountant | Import bank statement (CSV/CAMT.053) | `IFormFile` + format | `ImportResultDto` |
| 4 | GET | `/{orgId}/bank-transactions` | Accountant | List bank transactions | Query: bankAccountId, startDate, endDate, status?, page, pageSize | `PaginatedResponse<BankTransactionDto>` |
| 5 | GET | `/{orgId}/payouts` | Accountant | List expected payouts | Query: providerId?, status?, startDate, endDate | `PaginatedResponse<PayoutDto>` |
| 6 | POST | `/{orgId}/payouts/manual` | Accountant | Create manual payout expectation | `CreatePayoutDto` | `PayoutDto` |
| 7 | POST | `/{orgId}/reconcile/auto` | Accountant | Run auto-matching engine | Query: bankAccountId | `ReconciliationResultDto` |
| 8 | POST | `/{orgId}/reconcile/manual` | Accountant | Manually match bank transaction to payout/invoice | `ManualMatchDto` | `ReconciliationMatchDto` |
| 9 | GET | `/{orgId}/reconciliation/dashboard` | Manager | Reconciliation overview (expected, received, missing, unidentified) | Query: startDate, endDate | `ReconciliationDashboardDto` |
| 10 | POST | `/{orgId}/reconcile/{matchId}/confirm` | Accountant | Confirm an auto-match | - | `ReconciliationMatchDto` |

### 4.10 Finance API — Invoicing (`/api/finance/invoices`)

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | GET | `/{orgId}` | Manager | List invoices | Query: status?, customerId?, startDate, endDate, page, pageSize | `PaginatedResponse<InvoiceDto>` |
| 2 | GET | `/{orgId}/{invoiceId}` | Manager | Get invoice detail with lines | - | `InvoiceDetailDto` |
| 3 | POST | `/{orgId}` | Manager | Create invoice | `CreateInvoiceDto` | `InvoiceDto` |
| 4 | PUT | `/{orgId}/{invoiceId}` | Manager | Update draft invoice | `UpdateInvoiceDto` | `InvoiceDto` |
| 5 | POST | `/{orgId}/{invoiceId}/send` | Manager | Mark as sent (triggers email if configured) | - | `InvoiceDto` |
| 6 | POST | `/{orgId}/{invoiceId}/cancel` | Owner | Cancel invoice | `CancelReasonDto` | `InvoiceDto` |
| 7 | GET | `/{orgId}/{invoiceId}/pdf` | Manager | Download invoice PDF (with QR-bill) | - | `File` (PDF) |
| 8 | POST | `/{orgId}/{invoiceId}/payments` | Accountant | Record payment against invoice | `RecordInvoicePaymentDto` | `InvoicePaymentDto` |
| 9 | POST | `/{orgId}/{invoiceId}/remind` | Manager | Send payment reminder | - | `InvoiceReminderDto` |
| 10 | GET | `/{orgId}/aging` | Accountant | Accounts receivable aging report | - | `AgingReportDto` |

### 4.11 Finance API — Customers / Debtors (`/api/finance/customers`)

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | GET | `/{orgId}` | Manager | List customers | Query: search?, isActive? | `List<CustomerDto>` |
| 2 | GET | `/{orgId}/{customerId}` | Manager | Get customer detail with open invoices | - | `CustomerDetailDto` |
| 3 | POST | `/{orgId}` | Manager | Create customer | `CreateCustomerDto` | `CustomerDto` |
| 4 | PUT | `/{orgId}/{customerId}` | Manager | Update customer | `UpdateCustomerDto` | `CustomerDto` |
| 5 | DELETE | `/{orgId}/{customerId}` | Owner | Deactivate customer | - | `ServiceResponseDto` |

### 4.12 Finance API — Expenses (`/api/finance/expenses`)

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | GET | `/{orgId}` | Manager | List expenses | Query: category?, vendorId?, status?, startDate, endDate, page, pageSize | `PaginatedResponse<ExpenseDto>` |
| 2 | GET | `/{orgId}/{expenseId}` | Manager | Get expense detail with attachments | - | `ExpenseDetailDto` |
| 3 | POST | `/{orgId}` | Staff | Create expense | `CreateExpenseDto` | `ExpenseDto` |
| 4 | PUT | `/{orgId}/{expenseId}` | Manager | Update expense | `UpdateExpenseDto` | `ExpenseDto` |
| 5 | POST | `/{orgId}/{expenseId}/approve` | Owner | Approve expense | - | `ExpenseDto` |
| 6 | POST | `/{orgId}/{expenseId}/reject` | Owner | Reject expense | `RejectReasonDto` | `ExpenseDto` |
| 7 | POST | `/{orgId}/{expenseId}/attachments` | Staff | Upload attachment | `IFormFile` | `AttachmentDto` |
| 8 | DELETE | `/{orgId}/{expenseId}/attachments/{attachmentId}` | Manager | Delete attachment | - | `ServiceResponseDto` |
| 9 | POST | `/{orgId}/import` | Manager | Bulk import expenses from CSV | `IFormFile` | `ImportResultDto` |

### 4.13 Finance API — Vendors / Creditors (`/api/finance/vendors`)

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | GET | `/{orgId}` | Manager | List vendors | Query: search?, isActive? | `List<VendorDto>` |
| 2 | GET | `/{orgId}/{vendorId}` | Manager | Get vendor detail with open bills | - | `VendorDetailDto` |
| 3 | POST | `/{orgId}` | Manager | Create vendor | `CreateVendorDto` | `VendorDto` |
| 4 | PUT | `/{orgId}/{vendorId}` | Manager | Update vendor | `UpdateVendorDto` | `VendorDto` |
| 5 | DELETE | `/{orgId}/{vendorId}` | Owner | Deactivate vendor | - | `ServiceResponseDto` |
| 6 | GET | `/{orgId}/aging` | Accountant | Accounts payable aging report | - | `AgingReportDto` |

### 4.14 Finance API — Tax / VAT (`/api/finance/tax`)

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | GET | `/{orgId}/rates` | Manager | List tax rates (active and historical) | - | `List<TaxRateDto>` |
| 2 | POST | `/{orgId}/rates` | Owner | Create custom tax rate | `CreateTaxRateDto` | `TaxRateDto` |
| 3 | GET | `/{orgId}/reports` | Accountant | List tax report periods | Query: year? | `List<TaxReportPeriodDto>` |
| 4 | GET | `/{orgId}/reports/{periodId}` | Accountant | Get detailed VAT report for period | - | `TaxReportDetailDto` |
| 5 | POST | `/{orgId}/reports/generate` | Accountant | Generate VAT report for a period | `GenerateTaxReportDto` | `TaxReportPeriodDto` |
| 6 | POST | `/{orgId}/reports/{periodId}/file` | Owner | Mark tax report as filed | - | `TaxReportPeriodDto` |
| 7 | GET | `/{orgId}/reports/{periodId}/export` | Accountant | Export VAT report (CSV/PDF for ESTV) | Query: format | `File` |

### 4.15 Finance API — Reports & Analytics (`/api/finance/reports`)

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | GET | `/{orgId}/profit-loss` | Manager | Profit & Loss statement | Query: startDate, endDate, compareStartDate?, compareEndDate? | `ProfitLossDto` |
| 2 | GET | `/{orgId}/prime-cost` | Manager | Prime cost analysis (COGS + Labor) | Query: startDate, endDate, granularity (day/week/month) | `PrimeCostDto` |
| 3 | GET | `/{orgId}/revenue-analytics` | Manager | Revenue breakdown by method, period, category | Query: startDate, endDate, groupBy | `RevenueAnalyticsDto` |
| 4 | GET | `/{orgId}/cash-flow` | Accountant | Cash flow statement/forecast | Query: startDate, endDate | `CashFlowDto` |
| 5 | GET | `/{orgId}/balance-sheet` | Accountant | Balance sheet (simplified) | Query: asOfDate | `BalanceSheetDto` |
| 6 | GET | `/{orgId}/kpi-dashboard` | Manager | Dashboard KPIs (revenue, costs, margins, alerts) | Query: date? | `KpiDashboardDto` |
| 7 | GET | `/{orgId}/export/abacus` | Accountant | Export for Abacus (TAF format) | Query: startDate, endDate | `File` |
| 8 | GET | `/{orgId}/export/sage` | Accountant | Export for Sage | Query: startDate, endDate | `File` |
| 9 | GET | `/{orgId}/export/csv` | Accountant | Generic CSV export (journal entries, invoices, expenses) | Query: type, startDate, endDate | `File` |

### 4.16 Finance API — Integration Management (`/api/finance/integrations`)

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | GET | `/{orgId}/connections` | Owner | List integration connections | - | `List<IntegrationConnectionDto>` |
| 2 | POST | `/{orgId}/connections` | Owner | Register a new integration | `CreateIntegrationConnectionDto` | `IntegrationConnectionDto` |
| 3 | PUT | `/{orgId}/connections/{connId}` | Owner | Update integration settings | `UpdateIntegrationConnectionDto` | `IntegrationConnectionDto` |
| 4 | DELETE | `/{orgId}/connections/{connId}` | Owner | Deactivate integration | - | `ServiceResponseDto` |
| 5 | POST | `/{orgId}/connections/{connId}/rotate-key` | Owner | Regenerate API key | - | `ApiKeyDto` |
| 6 | GET | `/{orgId}/connections/{connId}/webhooks` | Owner | List outbound webhook subscriptions | - | `List<WebhookSubscriptionDto>` |
| 7 | POST | `/{orgId}/connections/{connId}/webhooks` | Owner | Create webhook subscription | `CreateWebhookDto` | `WebhookSubscriptionDto` |
| 8 | GET | `/{orgId}/connections/{connId}/webhooks/logs` | Owner | View delivery logs | Query: page, pageSize | `PaginatedResponse<WebhookDeliveryLogDto>` |

### 4.17 Finance API — Settings & Admin (`/api/finance/settings`)

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | GET | `/{orgId}` | Manager | Get organization financial settings | - | `OrganizationSettingsDto` |
| 2 | PUT | `/{orgId}` | Owner | Update financial settings | `UpdateSettingsDto` | `OrganizationSettingsDto` |
| 3 | GET | `/{orgId}/audit-log` | Owner | View audit log | Query: entityType?, action?, userId?, startDate, endDate, page, pageSize | `PaginatedResponse<AuditLogDto>` |
| 4 | GET | `/{orgId}/fiscal-periods` | Accountant | List fiscal periods | Query: year? | `List<FiscalPeriodDto>` |
| 5 | POST | `/{orgId}/fiscal-periods` | Owner | Create/adjust fiscal periods | `FiscalPeriodSetupDto` | `List<FiscalPeriodDto>` |

### 4.18 Finance API — Daily Flash Report (`/api/finance/flash`)

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | GET | `/{orgId}/today` | Manager | Get today's flash report (live, may be incomplete) | - | `DailyFlashReportDto` |
| 2 | GET | `/{orgId}/{date}` | Manager | Get flash report for a specific date | - | `DailyFlashReportDto` |
| 3 | GET | `/{orgId}/range` | Manager | Get flash reports for date range | Query: startDate, endDate, locationId? | `List<DailyFlashReportDto>` |
| 4 | POST | `/{orgId}/generate` | Accountant | Manually trigger flash report generation for a date | `GenerateFlashDto` | `DailyFlashReportDto` |
| 5 | POST | `/{orgId}/email` | Manager | Manually email flash report to recipients | `EmailFlashDto` | `ServiceResponseDto` |

### 4.19 Finance API — Budget Management (`/api/finance/budgets`)

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | GET | `/{orgId}` | Manager | List budgets | Query: year?, status? | `List<BudgetDto>` |
| 2 | GET | `/{orgId}/{budgetId}` | Manager | Get budget with all lines | - | `BudgetDetailDto` |
| 3 | POST | `/{orgId}` | Owner | Create budget (blank or copy from previous year) | `CreateBudgetDto` | `BudgetDto` |
| 4 | PUT | `/{orgId}/{budgetId}` | Owner | Update budget lines | `UpdateBudgetDto` | `BudgetDto` |
| 5 | POST | `/{orgId}/{budgetId}/approve` | Owner | Approve and lock budget | - | `BudgetDto` |
| 6 | GET | `/{orgId}/{budgetId}/variance` | Manager | Get budget vs. actual variance report | Query: month? | `BudgetVarianceDto` |
| 7 | GET | `/{orgId}/variance/summary` | Manager | Get YTD budget variance summary | Query: year | `BudgetVarianceSummaryDto` |

### 4.20 Finance API — 3rd-Party Delivery Platforms (`/api/finance/delivery`)

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | GET | `/{orgId}/platforms` | Manager | List registered delivery platforms | - | `List<DeliveryPlatformDto>` |
| 2 | POST | `/{orgId}/platforms` | Owner | Register delivery platform | `CreateDeliveryPlatformDto` | `DeliveryPlatformDto` |
| 3 | PUT | `/{orgId}/platforms/{platformId}` | Owner | Update platform settings (commission %) | `UpdateDeliveryPlatformDto` | `DeliveryPlatformDto` |
| 4 | POST | `/{orgId}/platforms/{platformId}/payouts/import` | Accountant | Import platform payout report (CSV) | `IFormFile` | `ImportResultDto` |
| 5 | GET | `/{orgId}/platforms/{platformId}/payouts` | Manager | List delivery payouts | Query: startDate, endDate, status? | `PaginatedResponse<DeliveryPlatformPayoutDto>` |
| 6 | GET | `/{orgId}/delivery-summary` | Manager | Delivery revenue summary (gross, commission, net by platform) | Query: startDate, endDate | `DeliverySummaryDto` |

### 4.21 Finance API — Gift Cards (`/api/finance/gift-cards`)

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | GET | `/{orgId}` | Manager | List gift cards | Query: status?, search?, page, pageSize | `PaginatedResponse<GiftCardDto>` |
| 2 | POST | `/{orgId}` | Manager | Issue new gift card | `IssueGiftCardDto` | `GiftCardDto` |
| 3 | GET | `/{orgId}/{cardId}` | Manager | Get gift card detail with transaction history | - | `GiftCardDetailDto` |
| 4 | POST | `/{orgId}/{cardId}/redeem` | Staff | Record redemption | `RedeemGiftCardDto` | `GiftCardTransactionDto` |
| 5 | GET | `/{orgId}/liability` | Accountant | Get total outstanding gift card liability | - | `GiftCardLiabilityDto` |
| 6 | POST | `/{orgId}/breakage/calculate` | Accountant | Calculate and record breakage revenue | `BreakageCalculationDto` | `GiftCardBreakageDto` |

### 4.22 Finance API — Fixed Assets (`/api/finance/assets`)

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | GET | `/{orgId}` | Manager | List fixed assets | Query: category?, status?, locationId? | `List<FixedAssetDto>` |
| 2 | POST | `/{orgId}` | Owner | Register new fixed asset | `CreateFixedAssetDto` | `FixedAssetDto` |
| 3 | PUT | `/{orgId}/{assetId}` | Owner | Update asset details | `UpdateFixedAssetDto` | `FixedAssetDto` |
| 4 | POST | `/{orgId}/depreciation/run` | Accountant | Run monthly depreciation for all active assets | `RunDepreciationDto` | `DepreciationResultDto` |
| 5 | POST | `/{orgId}/{assetId}/dispose` | Owner | Dispose of asset (sell, write off) | `DisposeAssetDto` | `FixedAssetDto` |
| 6 | GET | `/{orgId}/depreciation-schedule` | Accountant | Full depreciation schedule for all assets | - | `DepreciationScheduleDto` |

### 4.23 Finance API — Multi-Location (`/api/finance/locations`)

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | GET | `/{orgId}` | Manager | List locations | - | `List<LocationDto>` |
| 2 | POST | `/{orgId}` | Owner | Create location | `CreateLocationDto` | `LocationDto` |
| 3 | PUT | `/{orgId}/{locationId}` | Owner | Update location | `UpdateLocationDto` | `LocationDto` |
| 4 | GET | `/{orgId}/comparison` | Manager | Side-by-side KPI comparison across locations | Query: startDate, endDate | `LocationComparisonDto` |
| 5 | GET | `/{orgId}/consolidated/pl` | Owner | Consolidated P&L across all locations | Query: startDate, endDate | `ProfitLossDto` |

### 4.24 Finance API — Alerts & Scheduling (`/api/finance/alerts`)

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | GET | `/{orgId}` | Manager | List active alerts | Query: type?, severity?, isRead? | `PaginatedResponse<VarianceAlertDto>` |
| 2 | PUT | `/{orgId}/{alertId}/read` | Manager | Mark alert as read | - | `VarianceAlertDto` |
| 3 | PUT | `/{orgId}/{alertId}/dismiss` | Manager | Dismiss alert | - | `VarianceAlertDto` |
| 4 | GET | `/{orgId}/scheduled-reports` | Owner | List scheduled reports | - | `List<ScheduledReportDto>` |
| 5 | POST | `/{orgId}/scheduled-reports` | Owner | Create scheduled report | `CreateScheduledReportDto` | `ScheduledReportDto` |
| 6 | PUT | `/{orgId}/scheduled-reports/{scheduleId}` | Owner | Update schedule | `UpdateScheduledReportDto` | `ScheduledReportDto` |
| 7 | DELETE | `/{orgId}/scheduled-reports/{scheduleId}` | Owner | Delete schedule | - | `ServiceResponseDto` |

### 4.25 Finance API — AI Services (`/api/finance/ai`)

| # | Method | Route | Auth | Description | Request | Response |
|---|--------|-------|------|-------------|---------|----------|
| 1 | POST | `/{orgId}/ocr/invoice` | Staff | Upload invoice image/PDF for AI OCR extraction | `IFormFile` | `OcrExtractionResultDto` |
| 2 | POST | `/{orgId}/ocr/invoice/{extractionId}/confirm` | Manager | Confirm/correct OCR results and create expense | `ConfirmOcrDto` | `ExpenseDto` |
| 3 | GET | `/{orgId}/forecast/cash-flow` | Manager | AI cash flow forecast | Query: days (7/30/90) | `CashFlowForecastDto` |
| 4 | GET | `/{orgId}/forecast/revenue` | Manager | AI revenue forecast | Query: days (7/30/90) | `RevenueForecastDto` |

### Endpoint Summary

| Category | Endpoints | Auth Type | Phase |
|----------|-----------|-----------|-------|
| Authentication (`/api/auth/`) | 9 | None / Bearer | **MVP** |
| Chart of Accounts | 7 | JWT Bearer | **MVP** |
| Journal Entries | 7 | JWT Bearer | **MVP** |
| Payments | 5 | JWT Bearer | **MVP** |
| Close Day | 3 | JWT Bearer | **MVP** |
| Bank Reconciliation | 10 | JWT Bearer | **MVP** |
| Invoicing | 10 | JWT Bearer | **MVP** |
| Customers | 5 | JWT Bearer | **MVP** |
| Expenses | 9 | JWT Bearer | **MVP** |
| Vendors | 6 | JWT Bearer | **MVP** |
| Tax/VAT | 7 | JWT Bearer | **MVP** |
| Reports & Analytics | 9 | JWT Bearer | **MVP** |
| Integration Management | 8 | JWT Bearer | **MVP** |
| Settings & Admin | 5 | JWT Bearer | **MVP** |
| Inbound POS API (`/api/pos/v1/`) | 5 | API Key | **MVP** |
| Inbound Inventory API (`/api/inventory/v1/`) | 2 | API Key | **MVP** |
| Inbound Staff API (`/api/staff/v1/`) | 2 | API Key | **MVP** |
| **MVP Subtotal** | **108** | | |
| Disputes (Refunds/Chargebacks) | 5 | JWT Bearer | Phase 2 |
| Close Month | 2 | JWT Bearer | Phase 2 |
| Daily Flash Report | 5 | JWT Bearer | Phase 2 |
| **Phase 2 Subtotal** | **12** | | |
| Budget Management | 7 | JWT Bearer | Phase 3 |
| 3rd-Party Delivery | 6 | JWT Bearer | Phase 3 |
| Multi-Location | 5 | JWT Bearer | Phase 3 |
| Alerts & Scheduling | 7 | JWT Bearer | Phase 3 |
| AI Services (OCR) | 2 | JWT Bearer | Phase 3 |
| **Phase 3 Subtotal** | **27** | | |
| Gift Cards | 6 | JWT Bearer | Phase 4 |
| Fixed Assets | 6 | JWT Bearer | Phase 4 |
| AI Services (Forecast) | 2 | JWT Bearer | Phase 4 |
| Above-Store Dashboard | 1 | JWT Bearer | Phase 4 |
| **Phase 4 Subtotal** | **15** | | |
| **Grand Total** | **162** | | |

---

## 5. Service Layer Design

### 5.1 Application Services (Business Logic Orchestration)

Each service implements an interface defined in the Application layer. The Infrastructure layer provides the concrete implementations.

| Service Interface | Key Responsibilities |
|-------------------|---------------------|
| `IAuthService` | User registration, login, JWT token generation/refresh, password reset, invitation management. |
| `IEmailService` | Email delivery for: invoice sends, payment reminders, flash reports, scheduled reports, password resets, invitations. Uses Azure Communication Services. |
| `IPaymentIngestionService` | Normalize and store payment data from any source. Validate payment records. Handle idempotency (prevent duplicate ingestion). Route to appropriate handlers based on payment type. |
| `IPaymentService` | Query payments, create manual entries, aggregate daily summaries. |
| `IJournalEntryService` | Create, post, void journal entries. Validate debit/credit balance. Enforce fiscal period constraints. Update ledger account running balances. |
| `IAutoJournalService` | Auto-generate journal entries from: POS sales, refunds, payouts, expenses, invoices. Uses configurable mapping rules to determine which accounts to debit/credit. |
| `ICloseDayService` | Orchestrate close-day workflow: aggregate payments, verify cash drawers, generate daily summary, create journal entries, lock the day. Preview before executing. |
| `ICloseMonthService` | Orchestrate close-month: verify all days closed, run accruals, generate monthly reports, close fiscal period. |
| `IReconciliationService` | Import bank statements. Run auto-matching engine. Manual matching. Dashboard aggregation. Mismatch detection and alerting. |
| `IPayoutTrackingService` | Track expected payouts from payment providers. Match to bank transactions. Calculate settlement delays and missing amounts. |
| `IInvoiceService` | Full invoice lifecycle: create, send, track payments, reminders, cancellation. Invoice numbering. |
| `IPdfGeneratorService` | Generate PDF invoices with QR-bill. Generate PDF reports, reminders, tax exports. |
| `IQrBillService` | Generate Swiss QR-bill payment slips with QR-IBAN, reference numbers, and structured data. |
| `IExpenseService` | Expense CRUD, approval workflow, recurring expense generation. |
| `ITaxService` | VAT calculation, tax report generation, ESTV export formatting. |
| `IProfitLossService` | Generate P&L from journal data. Prime cost calculation. Revenue analytics. Period comparison. |
| `ICashFlowService` | Cash flow statement generation. Forward projection based on historical data and scheduled payments. |
| `IBankStatementParserService` | Parse CSV (multiple Swiss bank formats) and CAMT.053 XML bank statements into normalized `BankTransaction` records. |
| `IExportService` | Export data for external accounting systems: Abacus TAF format, Sage CSV/XML, generic CSV. |
| `IRefundService` | Process refunds with approval workflow. Create reversal journal entries. |
| `IChargebackService` | Track chargebacks, manage dispute lifecycle, create provisional journal entries. |
| `IAuditLogService` | Log all financial actions to immutable audit trail. Query audit log with filters. |
| `IWebhookDispatcherService` | Send outbound webhooks with HMAC signing, retry logic (3 attempts with exponential backoff), and delivery logging. |
| `IIntegrationService` | Manage integration connections, API keys, webhook subscriptions. |
| `IDailyFlashReportService` | Generate daily flash reports: aggregate sales + labor + COGS data, calculate KPIs, compare to prior periods, detect anomalies. Email delivery to configured recipients. |
| `IBudgetService` | Budget CRUD, budget line management, budget vs. actual calculation. Flexible budget adjustment based on actual revenue. Year-over-year budget copying. |
| `IDeliveryPlatformService` | Delivery platform management. Import platform payout CSVs. Reconcile gross vs. commission vs. net. Calculate effective delivery cost metrics. |
| `IGiftCardService` | Gift card lifecycle: issue, redeem, top-up, expire. Liability tracking. Breakage calculation based on historical redemption patterns. Cross-location redemption. |
| `IFixedAssetService` | Asset register management. Monthly depreciation calculation (straight-line or declining balance). Disposal processing with gain/loss journal entry. Depreciation schedule generation. |
| `ILocationService` | Multi-location management. Location-level data isolation. Consolidated reporting across locations. Location KPI comparison. |
| `IVarianceAlertService` | Real-time monitoring of financial KPIs against configured thresholds. Alert generation. Alert delivery (in-app, email, webhook). Anomaly detection using statistical deviation from rolling averages. |
| `IScheduledReportService` | Manage report schedules. Trigger report generation at configured times. Email delivery with PDF/CSV attachment. Retry on failure. |
| `IOcrService` | Invoice OCR processing: accept image/PDF upload, extract vendor, date, amounts, line items, VAT. Return structured result for user confirmation. Learn from corrections per vendor (ML feedback loop). Duplicate detection. |
| `IAiForecasterService` | Cash flow and revenue forecasting using historical data analysis: day-of-week patterns, seasonal trends, holiday effects. Confidence intervals on predictions. 7/30/90 day forward projections. |

### 5.2 Domain Services (Pure Business Rules)

| Service | Rules |
|---------|-------|
| `VatCalculator` | Calculate VAT from gross or net amounts. Swiss rate lookup by date (rates change). Reverse calculation (extract VAT from gross). |
| `JournalValidator` | Validate that debits = credits. Validate fiscal period is open. Validate account types are correct for the transaction. |
| `ReconciliationMatcher` | Matching algorithm: exact amount + reference → Exact. Amount within tolerance + date window → Probable. Else → Manual Review. |
| `InvoiceNumberGenerator` | Sequential numbering with prefix. Gap detection. Thread-safe (database sequence). |
| `PrimeCostCalculator` | Prime Cost = COGS + Labor. Calculate percentages. Compare to thresholds. |
| `AgingCalculator` | Calculate AR/AP aging buckets: Current, 1-30, 31-60, 61-90, 90+. |

### 5.3 Background Services (Hosted Services)

| Service | Schedule | Purpose |
|---------|----------|---------|
| `AutoCloseDayJob` | Configurable (default 2 AM) | Auto-close previous day if `Organization.AutoCloseDay` is enabled |
| `PayoutMismatchDetectorJob` | Every 6 hours | Check for overdue payouts, amount mismatches. Send alerts via webhook. |
| `InvoiceOverdueCheckerJob` | Daily at 8 AM | Update invoice status to "Overdue" when past due date. Trigger reminder workflow. |
| `RecurringExpenseGeneratorJob` | Daily at 6 AM | Generate expense entries for recurring expenses due today. |
| `AutoReconciliationJob` | Every 4 hours | Run auto-matching on new unmatched bank transactions. |
| `DailyFlashReportJob` | Configurable (default 6 AM) | Generate and email previous day's flash report to all configured recipients. |
| `VarianceAlertCheckerJob` | Every hour | Check all financial KPIs against configured thresholds. Generate alerts for violations. |
| `ScheduledReportDispatcherJob` | Every 15 minutes | Check for due scheduled reports. Generate and email to recipients. |
| `MonthlyDepreciationJob` | 1st of month, 3 AM | Run depreciation calculation for all active fixed assets. Generate journal entries. |
| `GiftCardExpiryJob` | Daily at midnight | Check for expired gift cards. Update status. Generate breakage revenue recognition if configured. |
| `DeliveryPlatformPayoutCheckerJob` | Every 6 hours | Check for expected delivery platform payouts that haven't been received. Generate alerts. |
| `BudgetVarianceAlertJob` | Daily at 7 AM | Check budget vs. actual for current month. Alert when any line exceeds variance threshold. |

---

## 6. Security & Authentication

### 6.1 Authentication

| Feature | Implementation |
|---------|---------------|
| **User Authentication** | ASP.NET Identity with JWT Bearer tokens. Own user database — NOT shared with BonApp. |
| **Registration** | Self-registration creates user + organization. Invite flow for adding users to existing org. |
| **JWT Tokens** | Access token (15 min TTL) + Refresh token (7 day TTL). Signed with RS256 (asymmetric keys). |
| **API Key Authentication** | Separate auth handler for integration endpoints (`/api/pos/v1/`, `/api/inventory/v1/`, `/api/staff/v1/`). API keys are hashed (SHA-256) in database, never stored in plain text. |
| **Password Policy** | Minimum 8 characters, require uppercase + lowercase + digit + special character. |

### 6.2 Authorization (Role-Based)

| Role | Permissions |
|------|-------------|
| **Owner** | Full access. Close month, void entries, manage users, approve expenses, manage integrations, view audit log. |
| **Accountant** | Journal entries, reconciliation, tax reports, close day, import statements, generate reports. Cannot manage users or integrations. |
| **Manager** | View all financial data, create invoices, create expenses, view reports. Cannot post/void journal entries or close month. |
| **Staff** | Submit expenses only. View own submissions. Cannot access financial reports or ledger. |

### 6.3 Security Requirements

| Requirement | Implementation |
|-------------|---------------|
| **Secrets Management** | Azure Key Vault for production. User Secrets for development. NEVER in appsettings.json. |
| **Data Encryption** | API keys and webhook secrets encrypted at rest (AES-256). Bank account details encrypted. |
| **Audit Trail** | Every financial action logged with user, timestamp, IP, before/after values. Immutable — no deletes. Swiss GeBüV compliance. |
| **Rate Limiting** | Integration API endpoints: 100 requests/minute per API key. Finance API: 300 requests/minute per user. |
| **CORS** | Configured per environment. Production: only the Finance Pro frontend domain. |
| **Input Validation** | FluentValidation on all DTOs. Monetary amounts validated as non-negative, reasonable range. Account numbers validated against Swiss Kontenrahmen format. |
| **Webhook Security** | All inbound webhooks verified via HMAC-SHA256 signature. All outbound webhooks signed with HMAC-SHA256. |

---

## 7. Database Design

### 7.1 Database Configuration

| Setting | Value |
|---------|-------|
| **Database** | SQL Server (Azure SQL in production) |
| **Database Name** | `FinanceProDb` |
| **ORM** | Entity Framework Core 8 |
| **Migrations** | Code-first migrations, auto-apply on startup (dev), manual in production |
| **Connection String** | Azure Key Vault (prod), User Secrets (dev) |
| **Concurrency** | Optimistic concurrency on `PaymentRecord` (RowVersion), `Invoice`, `BankTransaction` |

### 7.2 Key Indexes

| Table | Index | Type | Purpose |
|-------|-------|------|---------|
| `JournalEntry` | `IX_OrgId_EntryDate` | Non-clustered | Period queries |
| `JournalEntry` | `IX_OrgId_Source_SourceRefId` | Non-clustered | Find auto-generated entries |
| `JournalLine` | `IX_LedgerAccountId_JournalEntryId` | Non-clustered | Account transaction lookups |
| `PaymentRecord` | `IX_OrgId_PaymentDate` | Non-clustered | Daily aggregation |
| `PaymentRecord` | `IX_ExternalPaymentId` | Unique | Idempotency check |
| `BankTransaction` | `IX_OrgId_BankAccountId_TransactionDate` | Non-clustered | Statement queries |
| `BankTransaction` | `IX_OrgId_Status` | Non-clustered | Unmatched transaction queries |
| `Payout` | `IX_OrgId_Status_ExpectedArrivalDate` | Non-clustered | Missing payout detection |
| `Invoice` | `IX_OrgId_Status_DueDate` | Non-clustered | Overdue invoice detection |
| `Invoice` | `IX_InvoiceNumber` | Unique per org | Invoice lookup |
| `Expense` | `IX_OrgId_Category_ExpenseDate` | Non-clustered | Expense reports |
| `AuditLog` | `IX_OrgId_Timestamp` | Non-clustered | Audit trail queries |
| `AuditLog` | `IX_EntityType_EntityId` | Non-clustered | Entity history lookup |
| `DailyFlashReport` | `IX_OrgId_LocationId_ReportDate` | Unique | One flash report per location per day |
| `DailySalesSummary` | `IX_OrgId_LocationId_SaleDate` | Unique | One summary per location per day |
| `Budget` | `IX_OrgId_FiscalYear` | Non-clustered | Budget lookup by year |
| `BudgetLine` | `IX_BudgetId_LedgerAccountId_Month` | Unique | One budget line per account per month |
| `GiftCard` | `IX_CardNumber` | Unique | Fast gift card lookup by number |
| `FixedAsset` | `IX_OrgId_Status` | Non-clustered | Active asset queries for depreciation |
| `VarianceAlert` | `IX_OrgId_IsRead_CreatedAt` | Non-clustered | Unread alerts sorted by recency |
| `DeliveryPlatformPayout` | `IX_OrgId_PlatformId_Status` | Non-clustered | Platform payout reconciliation |
| `Location` | `IX_OrgId_IsActive` | Non-clustered | Active location listing |

### 7.3 Swiss Chart of Accounts Seed Data (Kontenrahmen KMU)

The following accounts are seeded on organization creation:

```
Assets (1xxx)
├── 1000 Cash (Asset) — cash register
├── 1010 PostFinance (Asset)
├── 1020 Bank UBS (Asset)
├── 1021 Bank Credit Suisse (Asset)
├── 1022 Bank Raiffeisen (Asset)
├── 1050 Short-term investments (Asset)
├── 1100 Trade receivables (Asset) — accounts receivable
├── 1109 Provision for doubtful debts (Asset, contra)
├── 1150 Settlement receivables (Asset) — pending payouts from Worldline, Stripe, etc.
├── 1170 VAT receivable (input tax) (Asset)
├── 1200 Inventory - food (Asset)
├── 1201 Inventory - beverages (Asset)
├── 1300 Prepaid expenses (Asset)
├── 1500 Equipment (Asset)
├── 1509 Accumulated depreciation - equipment (Asset, contra)
├── 1520 Kitchen equipment (Asset)
├── 1529 Accumulated depreciation - kitchen (Asset, contra)

Liabilities (2xxx)
├── 2000 Trade payables (Liability) — accounts payable
├── 2100 Bank overdraft / credit line (Liability)
├── 2200 VAT payable (output tax) (Liability)
├── 2201 VAT settlement account (Liability)
├── 2270 Social security payable (Liability)
├── 2030 Gift card liability / deferred revenue (Liability) — gift card balances owed to holders
├── 2300 Accrued expenses (Liability)
├── 2400 Long-term loans (Liability)

Equity (28xx)
├── 2800 Share capital (Equity)
├── 2900 Retained earnings (Equity)
├── 2990 Profit/loss for the year (Equity)

Revenue (3xxx)
├── 3000 Revenue - food (Revenue)
├── 3001 Revenue - beverages (Revenue)
├── 3002 Revenue - takeaway (Revenue)
├── 3003 Revenue - catering (Revenue)
├── 3004 Revenue - delivery (Revenue)
├── 3010 Revenue - other (Revenue)
├── 3100 Revenue - service charge / tips declared (Revenue)
├── 3200 Gift card breakage revenue (Revenue) — recognized from unredeemed gift cards
├── 3800 Discounts given (Revenue, contra)
├── 3900 Refunds / returns (Revenue, contra)

COGS / Material (4xxx)
├── 4000 Material purchases - food (Expense)
├── 4001 Material purchases - beverages (Expense)
├── 4002 Material purchases - supplies/packaging (Expense)
├── 4090 Purchase discounts received (Expense, contra)
├── 4200 Inventory change (Expense)

Personnel (5xxx)
├── 5000 Wages and salaries (Expense)
├── 5010 Overtime / bonus (Expense)
├── 5070 Social security employer contributions (Expense)
├── 5080 Pension fund contributions (Expense)
├── 5090 Other personnel expenses (Expense)

Operating Expenses (6xxx)
├── 6000 Rent (Expense)
├── 6010 Maintenance & repairs (Expense)
├── 6020 Cleaning (Expense)
├── 6100 Energy (electricity, gas) (Expense)
├── 6110 Water (Expense)
├── 6200 Insurance (Expense)
├── 6300 Administrative expenses (Expense)
├── 6400 IT & software (Expense)
├── 6500 Marketing & advertising (Expense)
├── 6510 Music license (SUISA) (Expense)
├── 6570 Decoration / flowers (Expense)
├── 6600 Bank charges (Expense)
├── 6601 Credit card processing fees (Expense)
├── 6602 Delivery platform commissions (Expense) — Uber Eats, DoorDash, Just Eat, eat.ch, Smood
├── 6700 Depreciation (Expense)
├── 6800 Licenses & permits (Expense)
├── 6900 Other operating expenses (Expense)

Financial Result (8xxx/9xxx)
├── 8000 Interest income (Revenue)
├── 8100 Interest expense (Expense)
├── 8500 Extraordinary income (Revenue)
├── 8900 Extraordinary expense (Expense)
├── 9000 Direct taxes (Expense)
```

---

## 8. Testing Strategy

### 8.1 Unit Tests (xUnit + Moq + FluentAssertions)

| Test Area | Examples |
|-----------|---------|
| **VatCalculator** | Correct calculation for each Swiss rate. Rate lookup by date. Gross-to-net and net-to-gross conversions. |
| **JournalValidator** | Debit/credit balance enforcement. Fiscal period validation. Account type validation. |
| **ReconciliationMatcher** | Exact match detection. Probable match with tolerance. No match scenarios. Split matching. |
| **PrimeCostCalculator** | Correct prime cost calculation. Threshold breach detection. Edge cases (zero revenue). |
| **AgingCalculator** | Correct aging bucket allocation. Edge cases (due today, negative days). |
| **AutoJournalService** | Correct journal entries generated for: POS sale, refund, payout, expense, invoice, depreciation, gift card issue/redeem. |
| **InvoiceNumberGenerator** | Sequential numbering. Year rollover. Thread safety. |
| **BankStatementParser** | Parse UBS CSV format. Parse PostFinance CSV format. Parse CAMT.053 XML. Invalid file handling. |
| **DailyFlashReportService** | Flash report calculation accuracy. Same-day-last-week/year comparison. Anomaly detection. Missing data handling. |
| **BudgetVarianceCalculator** | Correct variance $ and %. Flexible budget adjustment when revenue differs. Color-code threshold logic. |
| **GiftCardService** | Liability correctly tracked on issue. Revenue recognized on redeem. Breakage calculation with configurable rate. Cross-location redemption. |
| **FixedAssetService** | Straight-line depreciation over useful life. Declining balance method. Disposal gain/loss calculation. Zero salvage value edge case. |
| **DeliveryPlatformService** | Commission calculation. Effective cost per order. Variance detection (expected vs. actual payout). |
| **VarianceAlertService** | Alert triggered when threshold exceeded. No false positive below threshold. Correct severity assignment. |
| **QrBillService** | Structured address (Type S) generation. IBAN validation. QR reference number formatting. |

### 8.2 Integration Tests (Testcontainers + WebApplicationFactory)

| Test Area | Scope |
|-----------|-------|
| **Payment Ingestion API** | Full flow: POST payment → stored → daily summary updated → journal entry created |
| **Close Day Flow** | Aggregate → preview → close → verify journal entries → verify lock |
| **Reconciliation Flow** | Import statement → auto-match → verify matches → confirm |
| **Invoice Lifecycle** | Create → send → record payment → mark paid → verify journal entries |
| **VAT Report** | Ingest payments with different VAT rates → generate report → verify totals |
| **Multi-Org Isolation** | Verify data isolation between organizations |
| **Multi-Location Isolation** | Verify location-level data isolation within an organization. Consolidated report rolls up correctly. |
| **Daily Flash Report Flow** | Close day → flash report generated → verify all KPIs → email dispatch |
| **Gift Card Lifecycle** | Issue → partial redeem → check balance → full redeem → verify liability journal entries |
| **Budget Variance Flow** | Create budget → post expenses → check variance report → verify threshold alerts |
| **Delivery Reconciliation** | Register platform → import payout CSV → match to bank → verify commission tracking |
| **Fixed Asset Depreciation** | Register asset → run monthly depreciation → verify journal entries → verify book value |
| **OCR Invoice Flow** | Upload invoice image → receive extraction → confirm → verify expense created with correct data |

### 8.3 Test Data Seeding

- Use **Bogus** for realistic Swiss restaurant financial data
- Pre-built scenarios: "Café with cash-only", "Restaurant with Worldline + Stripe", "Catering with B2B invoicing"
- Seed Swiss chart of accounts, tax rates, sample customers and vendors

---

## 9. Deployment Architecture

### 9.1 Containerization

```dockerfile
# Multi-stage build
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "FinancePro.Api/FinancePro.Api.csproj" -c Release -o /app/publish
FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FinancePro.Api.dll"]
```

### 9.2 Azure Infrastructure

| Resource | Purpose |
|----------|---------|
| **Azure App Service** | Host the .NET 8 API |
| **Azure SQL Database** | `FinanceProDb` database |
| **Azure Key Vault** | Secrets (DB connection, JWT keys, API keys encryption key) |
| **Azure Blob Storage** | Invoice PDFs, expense attachments, bank statement archives |
| **Azure Application Insights** | Monitoring, telemetry, error tracking |
| **Azure AI Document Intelligence** | Invoice OCR processing (prebuilt invoice model). Extracts vendor, line items, amounts, VAT. |
| **Azure Communication Services** | Email delivery for flash reports, invoice reminders, scheduled reports |
| **Azure Service Bus** (Phase 2) | Async event processing for webhooks and background jobs |

### 9.3 CI/CD Pipeline

GitHub Actions or Azure DevOps:
- **Trigger:** `main`, `develop`, `feature/*`
- **Steps:** Restore → Build → Unit Tests → Integration Tests (Testcontainers) → Publish → Docker Build → Push to Container Registry → Deploy to App Service
- **Environments:** Development → Staging → Production
- **Database:** EF Core migrations applied via deployment pipeline (not auto-migrate in production)

---

## 10. Project Structure

```
finance-pro-backend/
├── FinancePro.sln
├── FinancePro.Api/                    — Presentation Layer
│   ├── Program.cs                     — Service registration, middleware, endpoints
│   ├── Endpoints/                     — Minimal API endpoint handlers
│   │   ├── AuthEndpoints.cs               — /api/auth/*
│   │   ├── PosIntegrationEndpoints.cs     — /api/pos/v1/*
│   │   ├── InventoryIntegrationEndpoints.cs — /api/inventory/v1/*
│   │   ├── StaffIntegrationEndpoints.cs   — /api/staff/v1/*
│   │   ├── AccountEndpoints.cs            — /api/finance/accounts/*
│   │   ├── JournalEndpoints.cs            — /api/finance/journal/*
│   │   ├── PaymentEndpoints.cs            — /api/finance/payments/*
│   │   ├── DisputeEndpoints.cs            — /api/finance/disputes/*
│   │   ├── CloseEndpoints.cs              — /api/finance/close/*
│   │   ├── ReconciliationEndpoints.cs     — /api/finance/reconciliation/*
│   │   ├── InvoiceEndpoints.cs            — /api/finance/invoices/*
│   │   ├── CustomerEndpoints.cs           — /api/finance/customers/*
│   │   ├── ExpenseEndpoints.cs            — /api/finance/expenses/*
│   │   ├── VendorEndpoints.cs             — /api/finance/vendors/*
│   │   ├── TaxEndpoints.cs                — /api/finance/tax/*
│   │   ├── ReportEndpoints.cs             — /api/finance/reports/*
│   │   ├── IntegrationEndpoints.cs        — /api/finance/integrations/*
│   │   ├── SettingsEndpoints.cs           — /api/finance/settings/*
│   │   ├── FlashReportEndpoints.cs        — /api/finance/flash/*
│   │   ├── BudgetEndpoints.cs             — /api/finance/budgets/*
│   │   ├── DeliveryPlatformEndpoints.cs   — /api/finance/delivery/*
│   │   ├── GiftCardEndpoints.cs           — /api/finance/gift-cards/*
│   │   ├── FixedAssetEndpoints.cs         — /api/finance/assets/*
│   │   ├── LocationEndpoints.cs           — /api/finance/locations/*
│   │   ├── AlertEndpoints.cs              — /api/finance/alerts/*
│   │   └── AiEndpoints.cs                 — /api/finance/ai/*
│   ├── Middleware/
│   │   ├── ApiKeyAuthenticationHandler.cs
│   │   ├── ExceptionMiddleware.cs
│   │   ├── AuditLogMiddleware.cs
│   │   └── RateLimitingMiddleware.cs
│   ├── Filters/
│   │   └── OrganizationContextFilter.cs   — Inject org context from route
│   └── appsettings.json / appsettings.Development.json
│
├── FinancePro.Application/            — Application Layer
│   ├── Interfaces/                    — Service contracts (30+ interfaces)
│   ├── DTOs/                          — Data Transfer Objects
│   │   ├── Auth/
│   │   ├── Payments/
│   │   ├── Journal/
│   │   ├── Reconciliation/
│   │   ├── Invoicing/
│   │   ├── Expenses/
│   │   ├── Tax/
│   │   ├── Reports/
│   │   ├── Integration/
│   │   ├── Settings/
│   │   ├── FlashReport/
│   │   ├── Budget/
│   │   ├── Delivery/
│   │   ├── GiftCards/
│   │   ├── Assets/
│   │   ├── Locations/
│   │   ├── Alerts/
│   │   └── Ai/
│   ├── Validators/                    — FluentValidation validators
│   ├── Mappers/                       — AutoMapper profiles
│   └── Helpers/
│       ├── VatCalculator.cs
│       ├── PrimeCostCalculator.cs
│       ├── AgingCalculator.cs
│       └── InvoiceNumberGenerator.cs
│
├── FinancePro.Domain/                 — Domain Layer
│   ├── Entities/                      — All entity classes (55+ entities)
│   │   ├── Organization.cs
│   │   ├── LedgerAccount.cs
│   │   ├── JournalEntry.cs
│   │   ├── JournalLine.cs
│   │   ├── PaymentRecord.cs
│   │   ├── PaymentSource.cs
│   │   ├── BankTransaction.cs
│   │   ├── Payout.cs
│   │   ├── Invoice.cs
│   │   ├── Expense.cs
│   │   ├── ... (all entities from Section 3)
│   ├── Enums/
│   │   ├── AccountType.cs
│   │   ├── PaymentMethod.cs
│   │   ├── PaymentType.cs
│   │   ├── InvoiceStatus.cs
│   │   ├── ReconciliationStatus.cs
│   │   ├── ... (all enums)
│   ├── Interfaces/                    — Repository interfaces
│   │   ├── ILedgerAccountRepository.cs
│   │   ├── IJournalEntryRepository.cs
│   │   ├── IPaymentRecordRepository.cs
│   │   ├── IBankTransactionRepository.cs
│   │   ├── IInvoiceRepository.cs
│   │   ├── IExpenseRepository.cs
│   │   └── ... (one per aggregate root)
│   └── Constants/
│       ├── SwissVatRates.cs
│       └── SwissChartOfAccounts.cs
│
├── FinancePro.DataAccess/             — Data Access Layer
│   ├── FinanceProDbContext.cs          — EF Core DbContext (55+ DbSets)
│   ├── Repositories/                  — Repository implementations
│   ├── Migrations/
│   ├── Configuration/                 — Entity type configurations (Fluent API)
│   │   ├── LedgerAccountConfig.cs
│   │   ├── JournalEntryConfig.cs
│   │   ├── PaymentRecordConfig.cs
│   │   └── ... (all entity configs)
│   └── Seeders/
│       ├── SwissChartOfAccountsSeeder.cs
│       ├── SwissVatRateSeeder.cs
│       └── RoleSeeder.cs
│
├── FinancePro.Infrastructure/         — Infrastructure Layer
│   ├── Services/                      — Service implementations
│   │   ├── PaymentIngestionService.cs
│   │   ├── AutoJournalService.cs
│   │   ├── CloseDayService.cs
│   │   ├── ReconciliationService.cs
│   │   ├── InvoiceService.cs
│   │   ├── PdfGeneratorService.cs
│   │   ├── QrBillService.cs
│   │   ├── BankStatementParserService.cs
│   │   ├── ExportService.cs
│   │   ├── WebhookDispatcherService.cs
│   │   ├── DailyFlashReportService.cs
│   │   ├── BudgetService.cs
│   │   ├── DeliveryPlatformService.cs
│   │   ├── GiftCardService.cs
│   │   ├── FixedAssetService.cs
│   │   ├── LocationService.cs
│   │   ├── VarianceAlertService.cs
│   │   ├── ScheduledReportService.cs
│   │   ├── OcrService.cs (Azure AI Document Intelligence / custom ML)
│   │   ├── AiForecasterService.cs
│   │   └── ... (all service implementations)
│   ├── BackgroundJobs/
│   │   ├── AutoCloseDayJob.cs
│   │   ├── PayoutMismatchDetectorJob.cs
│   │   ├── InvoiceOverdueCheckerJob.cs
│   │   ├── RecurringExpenseGeneratorJob.cs
│   │   ├── AutoReconciliationJob.cs
│   │   ├── DailyFlashReportJob.cs
│   │   ├── VarianceAlertCheckerJob.cs
│   │   ├── ScheduledReportDispatcherJob.cs
│   │   ├── MonthlyDepreciationJob.cs
│   │   ├── GiftCardExpiryJob.cs
│   │   ├── DeliveryPlatformPayoutCheckerJob.cs
│   │   └── BudgetVarianceAlertJob.cs
│   ├── Parsers/
│   │   ├── UbsCsvParser.cs
│   │   ├── PostFinanceCsvParser.cs
│   │   ├── RaiffeisenCsvParser.cs
│   │   ├── CreditSuisseCsvParser.cs
│   │   └── Camt053Parser.cs
│   └── Exporters/
│       ├── AbacusTafExporter.cs
│       ├── SageCsvExporter.cs
│       └── GenericCsvExporter.cs
│
├── FinancePro.UnitTests/              — Unit Tests
│   ├── Services/
│   ├── Validators/
│   ├── Helpers/
│   └── Parsers/
│
├── FinancePro.IntegrationTests/       — Integration Tests
│   ├── Endpoints/
│   ├── Flows/
│   └── Fixtures/
│
└── Dockerfile
```

---

## 11. Key Technical Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| **All monetary fields** | `decimal` (not `double`) | Avoid floating-point precision errors. Financial data demands exact arithmetic. |
| **Journal entry validation** | Debits must equal credits | Core double-entry bookkeeping constraint. Enforced at application and database level. |
| **Idempotent payment ingestion** | Unique `ExternalPaymentId` per source | Prevents duplicate processing when POS retries. |
| **Immutable audit log** | Append-only table, no UPDATE/DELETE | Swiss compliance (GeBüV). |
| **API key hashing** | SHA-256 hash stored, never plain text | If database is compromised, API keys are not exposed. |
| **Bank statement parsing** | Pluggable parser pattern | Each Swiss bank has a different CSV format. CAMT.053 is standardized but complex. Strategy pattern allows adding new parsers. |
| **PDF generation** | QuestPDF or iTextSharp | QuestPDF is open-source and .NET-native. Supports Swiss QR-bill generation. |
| **Concurrency** | Optimistic concurrency (RowVersion) on PaymentRecord, Invoice, BankTransaction | Prevents lost updates during concurrent operations. |
| **Multi-tenancy** | Row-level isolation (OrganizationId on every table) | Simpler than schema-per-tenant. Query filter applied globally via EF Core. |
| **Background jobs** | .NET `IHostedService` (MVP), Hangfire (Phase 2) | Simple for MVP. Hangfire adds dashboard, persistence, and retry for production. |

---

## 12. Integration Contract Reference

### Inbound: Payment Ingestion (`POST /api/pos/v1/payments`)

```json
{
  "externalPaymentId": "WL-TXN-20260209-001",
  "externalOrderId": "ORD-1234",
  "paymentDate": "2026-02-09T14:30:00Z",
  "amount": 45.50,
  "currency": "CHF",
  "paymentMethod": "Card",
  "paymentProvider": "Worldline",
  "paymentType": "Capture",
  "tipAmount": 5.00,
  "cardBrand": "Visa",
  "cardLastFour": "1234",
  "vatLines": [
    { "taxRate": 8.1, "netAmount": 35.50, "vatAmount": 2.88 },
    { "taxRate": 2.6, "netAmount": 10.00, "vatAmount": 0.26 }
  ]
}
```

### Inbound: COGS Summary (`POST /api/inventory/v1/cogs`)

```json
{
  "periodStart": "2026-02-01",
  "periodEnd": "2026-02-28",
  "foodCogs": 12500.00,
  "beverageCogs": 3200.00,
  "supplyCogs": 800.00,
  "totalCogs": 16500.00,
  "openingInventoryValue": 8000.00,
  "closingInventoryValue": 7500.00,
  "purchasesTotal": 16000.00
}
```

### Inbound: Labor Summary (`POST /api/staff/v1/labor-summary`)

```json
{
  "periodStart": "2026-02-01",
  "periodEnd": "2026-02-28",
  "totalWages": 22000.00,
  "totalOvertime": 1500.00,
  "totalBenefits": 4200.00,
  "socialSecurityEmployer": 2800.00,
  "pensionContributions": 1600.00,
  "totalLaborCost": 32100.00,
  "laborHours": 1200,
  "fullTimeEquivalent": 7.5
}
```

### Outbound: Webhook Events

```json
// Event: payout.mismatch_detected
{
  "event": "payout.mismatch_detected",
  "timestamp": "2026-02-09T06:00:00Z",
  "organizationId": 1,
  "data": {
    "payoutId": 456,
    "provider": "Worldline",
    "expectedAmount": 3200.00,
    "actualAmount": 3150.00,
    "variance": -50.00,
    "payoutDate": "2026-02-07"
  }
}
```

```json
// Event: tax.report_ready
{
  "event": "tax.report_ready",
  "timestamp": "2026-04-01T08:00:00Z",
  "organizationId": 1,
  "data": {
    "periodId": 12,
    "periodType": "Quarterly",
    "startDate": "2026-01-01",
    "endDate": "2026-03-31",
    "netVatPayable": 4520.00
  }
}
```

```json
// Event: day.closed (triggers flash report generation)
{
  "event": "day.closed",
  "timestamp": "2026-02-10T02:00:00Z",
  "organizationId": 1,
  "data": {
    "date": "2026-02-09",
    "locationId": null,
    "netRevenue": 3720.50,
    "transactionCount": 87,
    "primeCostPercent": 58.9,
    "closedBy": "john.doe@restaurant.ch"
  }
}
```

### Delivery Platform Payout CSV Import Format (Expected Columns)

```csv
# Uber Eats / DoorDash / eat.ch payout report — expected columns after mapping
payout_date,period_start,period_end,gross_orders,commission,marketing_fee,tips,adjustments,net_payout,order_count
2026-02-07,2026-01-27,2026-02-02,4250.00,850.00,127.50,185.00,-45.00,3412.50,142
```

### OCR Invoice Extraction Result (`OcrExtractionResultDto`)

```json
{
  "extractionId": "ocr-abc123",
  "confidence": 0.94,
  "vendor": { "name": "Swiss Gastro Supplies AG", "confidence": 0.97 },
  "invoiceNumber": { "value": "RE-2026-0842", "confidence": 0.99 },
  "invoiceDate": { "value": "2026-02-05", "confidence": 0.95 },
  "dueDate": { "value": "2026-03-07", "confidence": 0.88 },
  "lineItems": [
    { "description": "Organic flour 25kg", "quantity": 4, "unitPrice": 32.50, "total": 130.00, "confidence": 0.92 },
    { "description": "Olive oil 5L", "quantity": 6, "unitPrice": 18.90, "total": 113.40, "confidence": 0.90 }
  ],
  "subTotal": { "value": 243.40, "confidence": 0.96 },
  "vatRate": { "value": 8.1, "confidence": 0.93 },
  "vatAmount": { "value": 19.72, "confidence": 0.93 },
  "total": { "value": 263.12, "confidence": 0.97 },
  "suggestedCategory": "FoodPurchase",
  "suggestedLedgerAccount": "4000",
  "isDuplicate": false,
  "originalFileUrl": "https://blob.azure.com/invoices/ocr-abc123.pdf"
}
```

---

*This is a standalone application. It communicates with BonApp POS, Inventory Pro, Staff Pro, and any other system exclusively through APIs. No shared database. No shared code. No shared authentication.*
