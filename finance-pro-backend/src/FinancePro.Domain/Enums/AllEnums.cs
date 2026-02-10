namespace FinancePro.Domain.Enums;

public enum UserRole
{
    Owner = 1,
    Accountant = 2,
    Manager = 3,
    Staff = 4
}

public enum AccountType
{
    Asset = 1,
    Liability = 2,
    Equity = 3,
    Revenue = 4,
    Expense = 5
}

public enum PaymentMethod
{
    Cash = 1,
    Card = 2,
    Online = 3,
    Mobile = 4,
    BankTransfer = 5,
    Other = 6
}

public enum PaymentEventType
{
    Capture = 1,
    Authorization = 2,
    Refund = 3,
    Void = 4,
    Chargeback = 5
}

public enum JournalEntryStatus
{
    Draft = 1,
    Posted = 2,
    Voided = 3
}

public enum JournalEntrySource
{
    Manual = 1,
    Auto_Sale = 2,
    Auto_Refund = 3,
    Auto_Payout = 4,
    Auto_Expense = 5,
    Auto_CloseDay = 6,
    Auto_Invoice = 7
}

public enum DayStatus
{
    Open = 1,
    Closed = 2,
    Reopened = 3
}

public enum InvoiceStatus
{
    Draft = 1,
    Sent = 2,
    PartiallyPaid = 3,
    Paid = 4,
    Overdue = 5,
    Cancelled = 6,
    WrittenOff = 7
}

public enum ExpenseCategory
{
    Material = 1,
    Personnel = 2,
    Rent = 3,
    Utilities = 4,
    Insurance = 5,
    Marketing = 6,
    Administrative = 7,
    Maintenance = 8,
    Licenses = 9,
    ProfessionalServices = 10,
    CreditCardFees = 11,
    Miscellaneous = 99
}

public enum PayoutStatus
{
    Pending = 1,
    Received = 2,
    Partial = 3,
    Missing = 4,
    Disputed = 5
}

public enum ReconciliationMatchType
{
    ExactMatch = 1,
    ProbableMatch = 2,
    ManualMatch = 3,
    Unmatched = 4
}

public enum VatRate
{
    Standard = 1,     // 8.1%
    Reduced = 2,      // 2.6%
    Hospitality = 3,  // 3.8%
    Exempt = 4        // 0%
}

public enum FiscalPeriodStatus
{
    Open = 1,
    Closed = 2
}

public enum IntegrationSourceType
{
    BonAppPOS = 1,
    Lightspeed = 2,
    Toast = 3,
    Square = 4,
    Orderbird = 5,
    Stripe = 6,
    Worldline = 7,
    Wallee = 8,
    Manual = 9,
    CsvImport = 10,
    Other = 99
}

public enum AuditAction
{
    Create = 1,
    Update = 2,
    Delete = 3,
    CloseDay = 4,
    ReopenDay = 5,
    PostJournal = 6,
    VoidJournal = 7,
    Login = 8,
    Export = 9
}
