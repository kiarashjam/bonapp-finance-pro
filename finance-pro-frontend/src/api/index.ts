import client from './client';
import type { AuthResponse, LoginRequest, RegisterRequest, LedgerAccountDto, CreateLedgerAccountRequest, JournalEntryDto, CreateJournalEntryRequest, InvoiceDto, CreateInvoiceRequest, CustomerDto, ExpenseDto, CreateExpenseRequest, VendorDto, DailySalesSummaryDto, ManualSalesEntryRequest, CloseDayPreviewDto, BankAccountDto, ReconciliationDashboardDto, PayoutDto, ProfitAndLossDto, VatReportDto, DashboardDto, OrganizationSettingsDto, PaymentSourceDto, TaxRateDto, AuditLogDto, PagedResult, UserDto } from '../types';

// Auth
export const authApi = {
  register: (data: RegisterRequest) => client.post<AuthResponse>('/api/auth/register', data),
  login: (data: LoginRequest) => client.post<AuthResponse>('/api/auth/login', data),
  me: () => client.get<UserDto>('/api/auth/me'),
};

// Accounts
export const accountsApi = {
  getAll: () => client.get<LedgerAccountDto[]>('/api/accounts'),
  getById: (id: number) => client.get<LedgerAccountDto>(`/api/accounts/${id}`),
  create: (data: CreateLedgerAccountRequest) => client.post<LedgerAccountDto>('/api/accounts', data),
  update: (id: number, data: Partial<LedgerAccountDto>) => client.put<LedgerAccountDto>(`/api/accounts/${id}`, data),
  delete: (id: number) => client.delete(`/api/accounts/${id}`),
};

// Journals
export const journalsApi = {
  getAll: (page = 1, pageSize = 20) => client.get<PagedResult<JournalEntryDto>>(`/api/journals?page=${page}&pageSize=${pageSize}`),
  getById: (id: number) => client.get<JournalEntryDto>(`/api/journals/${id}`),
  create: (data: CreateJournalEntryRequest) => client.post<JournalEntryDto>('/api/journals', data),
  post: (id: number) => client.post<JournalEntryDto>(`/api/journals/${id}/post`),
  void: (id: number) => client.post<JournalEntryDto>(`/api/journals/${id}/void`),
  trialBalance: (asOfDate: string) => client.get(`/api/journals/trial-balance?asOfDate=${asOfDate}`),
};

// Invoices
export const invoicesApi = {
  getAll: (page = 1, pageSize = 20, status?: string) => client.get<PagedResult<InvoiceDto>>(`/api/invoices?page=${page}&pageSize=${pageSize}${status ? `&status=${status}` : ''}`),
  getById: (id: number) => client.get<InvoiceDto>(`/api/invoices/${id}`),
  create: (data: CreateInvoiceRequest) => client.post<InvoiceDto>('/api/invoices', data),
  updateStatus: (id: number, status: string) => client.put<InvoiceDto>(`/api/invoices/${id}/status?status=${status}`),
  delete: (id: number) => client.delete(`/api/invoices/${id}`),
};

// Customers
export const customersApi = {
  getAll: () => client.get<CustomerDto[]>('/api/customers'),
  getById: (id: number) => client.get<CustomerDto>(`/api/customers/${id}`),
  create: (data: Partial<CustomerDto>) => client.post<CustomerDto>('/api/customers', data),
  update: (id: number, data: Partial<CustomerDto>) => client.put<CustomerDto>(`/api/customers/${id}`, data),
  delete: (id: number) => client.delete(`/api/customers/${id}`),
};

// Expenses
export const expensesApi = {
  getAll: (page = 1, pageSize = 20) => client.get<PagedResult<ExpenseDto>>(`/api/expenses?page=${page}&pageSize=${pageSize}`),
  getById: (id: number) => client.get<ExpenseDto>(`/api/expenses/${id}`),
  create: (data: CreateExpenseRequest) => client.post<ExpenseDto>('/api/expenses', data),
  update: (id: number, data: CreateExpenseRequest) => client.put<ExpenseDto>(`/api/expenses/${id}`, data),
  delete: (id: number) => client.delete(`/api/expenses/${id}`),
};

// Vendors
export const vendorsApi = {
  getAll: () => client.get<VendorDto[]>('/api/vendors'),
  getById: (id: number) => client.get<VendorDto>(`/api/vendors/${id}`),
  create: (data: Partial<VendorDto>) => client.post<VendorDto>('/api/vendors', data),
  update: (id: number, data: Partial<VendorDto>) => client.put<VendorDto>(`/api/vendors/${id}`, data),
  delete: (id: number) => client.delete(`/api/vendors/${id}`),
};

// Sales
export const salesApi = {
  getDailySummaries: (start: string, end: string) => client.get<DailySalesSummaryDto[]>(`/api/sales/daily?startDate=${start}&endDate=${end}`),
  createManualEntry: (data: ManualSalesEntryRequest) => client.post<DailySalesSummaryDto>('/api/sales/manual-entry', data),
  getCloseDayPreview: (date: string) => client.get<CloseDayPreviewDto>(`/api/sales/close-day/preview?date=${date}`),
  closeDay: (date: string) => client.post<DailySalesSummaryDto>('/api/sales/close-day', { date, confirmed: true }),
};

// Reconciliation
export const reconciliationApi = {
  getDashboard: () => client.get<ReconciliationDashboardDto>('/api/reconciliation/dashboard'),
  getBankAccounts: () => client.get<BankAccountDto[]>('/api/reconciliation/bank-accounts'),
  createBankAccount: (data: Partial<BankAccountDto>) => client.post<BankAccountDto>('/api/reconciliation/bank-accounts', data),
  importStatement: (bankAccountId: number, file: File) => { const fd = new FormData(); fd.append('file', file); return client.post(`/api/reconciliation/bank-accounts/${bankAccountId}/import`, fd, { headers: { 'Content-Type': 'multipart/form-data' } }); },
  getPayouts: (page = 1, pageSize = 20, status?: string) => client.get<PayoutDto[]>(`/api/reconciliation/payouts?page=${page}&pageSize=${pageSize}${status ? `&status=${status}` : ''}`),
};

// Reports
export const reportsApi = {
  getProfitAndLoss: (start: string, end: string) => client.get<ProfitAndLossDto>(`/api/reports/profit-and-loss?startDate=${start}&endDate=${end}`),
  getVatReport: (start: string, end: string) => client.get<VatReportDto>(`/api/reports/vat?startDate=${start}&endDate=${end}`),
  getDashboard: () => client.get<DashboardDto>('/api/reports/dashboard'),
};

// Tax Rates
export const taxApi = {
  getRates: () => client.get<TaxRateDto[]>('/api/tax-rates'),
};

// Settings
export const settingsApi = {
  getOrganization: () => client.get<OrganizationSettingsDto>('/api/settings/organization'),
  updateOrganization: (data: Partial<OrganizationSettingsDto>) => client.put<OrganizationSettingsDto>('/api/settings/organization', data),
  getPaymentSources: () => client.get<PaymentSourceDto[]>('/api/settings/payment-sources'),
  createPaymentSource: (data: Partial<PaymentSourceDto>) => client.post<PaymentSourceDto>('/api/settings/payment-sources', data),
  deletePaymentSource: (id: number) => client.delete(`/api/settings/payment-sources/${id}`),
};

// Audit
export const auditApi = {
  getLogs: (page = 1, pageSize = 50, entityType?: string) => client.get<PagedResult<AuditLogDto>>(`/api/audit?page=${page}&pageSize=${pageSize}${entityType ? `&entityType=${entityType}` : ''}`),
};
