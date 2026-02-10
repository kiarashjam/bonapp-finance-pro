// ───── Auth ─────
export interface LoginRequest { email: string; password: string; }
export interface RegisterRequest { email: string; password: string; firstName: string; lastName: string; organizationName: string; }
export interface AuthResponse { token: string; refreshToken: string; expiration: string; user: UserDto; }
export interface UserDto { id: string; email: string; firstName: string; lastName: string; role: string; organizationId: number; organizationName: string; }

// ───── Accounts ─────
export interface LedgerAccountDto { id: number; accountNumber: string; name: string; description?: string; accountType: string; parentAccountId?: number; isActive: boolean; isSystemAccount: boolean; }
export interface CreateLedgerAccountRequest { accountNumber: string; name: string; description?: string; accountType: string; parentAccountId?: number; }

// ───── Journal Entries ─────
export interface JournalEntryDto { id: number; referenceNumber: string; entryDate: string; description?: string; status: string; source: string; totalDebit: number; totalCredit: number; lines: JournalLineDto[]; createdAt: string; }
export interface JournalLineDto { id: number; ledgerAccountId: number; accountNumber: string; accountName: string; debitAmount: number; creditAmount: number; description?: string; }
export interface CreateJournalEntryRequest { entryDate: string; description?: string; lines: { ledgerAccountId: number; debitAmount: number; creditAmount: number; description?: string; }[]; }

// ───── Invoices ─────
export interface InvoiceDto { id: number; invoiceNumber: string; customerId: number; customerName: string; invoiceDate: string; dueDate: string; status: string; subTotal: number; vatTotal: number; total: number; paidAmount: number; outstandingAmount: number; notes?: string; lines: InvoiceLineDto[]; createdAt: string; }
export interface InvoiceLineDto { id: number; description: string; quantity: number; unitPrice: number; vatRate: string; vatAmount: number; lineTotal: number; sortOrder: number; }
export interface CreateInvoiceRequest { customerId: number; invoiceDate: string; dueDate: string; notes?: string; paymentTerms?: string; lines: { description: string; quantity: number; unitPrice: number; vatRate: string; sortOrder: number; }[]; }

// ───── Customers ─────
export interface CustomerDto { id: number; name: string; contactPerson?: string; email?: string; phone?: string; street?: string; houseNumber?: string; postalCode?: string; city?: string; country?: string; vatId?: string; paymentTermDays: number; creditLimit?: number; outstandingBalance: number; }

// ───── Expenses ─────
export interface ExpenseDto { id: number; expenseDate: string; description: string; amount: number; vatAmount: number; netAmount: number; vatRate: string; category: string; paymentMethod: string; receiptUrl?: string; notes?: string; vendorId?: number; vendorName?: string; createdAt: string; }
export interface CreateExpenseRequest { expenseDate: string; description: string; amount: number; vatRate: string; category: string; paymentMethod: string; notes?: string; vendorId?: number; }

// ───── Vendors ─────
export interface VendorDto { id: number; name: string; contactPerson?: string; email?: string; phone?: string; address?: string; iban?: string; paymentTermDays: number; }

// ───── Sales ─────
export interface DailySalesSummaryDto { id: number; date: string; grossSales: number; netSales: number; totalRefunds: number; totalTips: number; cashSales: number; cardSales: number; onlineSales: number; transactionCount: number; guestCount: number; dayStatus: string; closedAt?: string; averageCheck: number; }
export interface ManualSalesEntryRequest { date: string; grossSales: number; cashSales: number; cardSales: number; onlineSales: number; otherSales: number; totalTips: number; vatStandard: number; vatReduced: number; vatHospitality: number; transactionCount: number; guestCount: number; }
export interface CloseDayPreviewDto { date: string; grossSales: number; netSales: number; totalRefunds: number; totalTips: number; transactionCount: number; cashSales: number; cardSales: number; proposedJournalLines: JournalLineDto[]; }

// ───── Reconciliation ─────
export interface BankAccountDto { id: number; name: string; iban: string; bankName?: string; currency: string; currentBalance: number; isActive: boolean; }
export interface BankTransactionDto { id: number; bankAccountId: number; transactionDate: string; valueDate?: string; amount: number; description?: string; reference?: string; isMatched: boolean; }
export interface PayoutDto { id: number; paymentSourceId: number; paymentSourceName: string; expectedAmount: number; actualAmount?: number; fees?: number; expectedDate: string; actualDate?: string; status: string; providerReference?: string; }
export interface ReconciliationDashboardDto { totalExpectedPayouts: number; totalReceivedPayouts: number; totalMissingPayouts: number; totalDisputedPayouts: number; unmatchedBankTransactions: number; reconciliationRate: number; recentPayouts: PayoutDto[]; unmatchedTransactions: BankTransactionDto[]; }

// ───── Reports ─────
export interface ProfitAndLossDto { startDate: string; endDate: string; revenue: number; cogs: number; grossProfit: number; grossProfitMargin: number; laborCost: number; primeCost: number; primeCostPercent: number; operatingExpenses: number; netOperatingProfit: number; netProfitMargin: number; revenueItems: PnlLineItemDto[]; cogsItems: PnlLineItemDto[]; laborItems: PnlLineItemDto[]; opExItems: PnlLineItemDto[]; }
export interface PnlLineItemDto { accountNumber: string; accountName: string; amount: number; percent: number; }
export interface DashboardDto { todayRevenue: number; weekRevenue: number; monthRevenue: number; primeCostPercent: number; foodCostPercent: number; laborCostPercent: number; netProfitMargin: number; cashPosition: number; outstandingAR: number; outstandingAP: number; reconciliationRate: number; vatLiability: number; revenueChart: { date: string; amount: number; }[]; }
export interface VatReportDto { startDate: string; endDate: string; totalRevenue: number; outputVatStandard: number; outputVatReduced: number; outputVatHospitality: number; totalOutputVat: number; totalInputVat: number; netVatPayable: number; }

// ───── Settings ─────
export interface OrganizationSettingsDto { id: number; name: string; legalName?: string; street?: string; houseNumber?: string; postalCode?: string; city?: string; country?: string; phone?: string; email?: string; website?: string; vatId?: string; iban?: string; qrIban?: string; logoUrl?: string; currency: string; fiscalYearStartMonth: number; defaultLanguage: string; }
export interface PaymentSourceDto { id: number; name: string; sourceType: string; isActive: boolean; description?: string; createdAt: string; }
export interface TaxRateDto { id: number; name: string; rateType: string; rate: number; isActive: boolean; effectiveFrom: string; effectiveTo?: string; }
export interface AuditLogDto { id: number; entityType: string; entityId: number; action: string; userName?: string; timestamp: string; }

// ───── Pagination ─────
export interface PagedResult<T> { items: T[]; totalCount: number; page: number; pageSize: number; }
