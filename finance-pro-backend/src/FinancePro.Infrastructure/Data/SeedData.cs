using FinancePro.Domain.Entities;
using FinancePro.Domain.Enums;

namespace FinancePro.Infrastructure.Data;

public static class SeedData
{
    /// <summary>
    /// Swiss Kontenrahmen KMU — standard chart of accounts for SMEs in Switzerland
    /// </summary>
    public static List<LedgerAccount> GetSwissChartOfAccounts(int organizationId) => new()
    {
        // Assets (1xxx)
        new() { AccountNumber = "1000", Name = "Kasse / Cash", AccountType = AccountType.Asset, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "1020", Name = "Bank / Bank account", AccountType = AccountType.Asset, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "1100", Name = "Debitoren / Accounts receivable", AccountType = AccountType.Asset, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "1150", Name = "Settlement receivables", AccountType = AccountType.Asset, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "1170", Name = "Vorsteuer MwSt / Input VAT", AccountType = AccountType.Asset, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "1200", Name = "Warenvorrat / Inventory", AccountType = AccountType.Asset, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "1500", Name = "Maschinen / Equipment", AccountType = AccountType.Asset, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "1510", Name = "Mobiliar / Furniture", AccountType = AccountType.Asset, OrganizationId = organizationId, IsSystemAccount = true },

        // Liabilities (2xxx)
        new() { AccountNumber = "2000", Name = "Kreditoren / Accounts payable", AccountType = AccountType.Liability, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "2030", Name = "Gift card liability / Deferred revenue", AccountType = AccountType.Liability, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "2200", Name = "MwSt-Schuld / VAT payable", AccountType = AccountType.Liability, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "2300", Name = "Kurzfristige Bankverbindlichkeiten", AccountType = AccountType.Liability, OrganizationId = organizationId, IsSystemAccount = true },

        // Equity (28xx)
        new() { AccountNumber = "2800", Name = "Eigenkapital / Equity", AccountType = AccountType.Equity, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "2900", Name = "Jahresgewinn / Profit for the year", AccountType = AccountType.Equity, OrganizationId = organizationId, IsSystemAccount = true },

        // Revenue (3xxx)
        new() { AccountNumber = "3000", Name = "Warenertrag / Revenue from sales", AccountType = AccountType.Revenue, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "3200", Name = "Übrige Erlöse / Other revenue", AccountType = AccountType.Revenue, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "3400", Name = "Dienstleistungsertrag / Service revenue", AccountType = AccountType.Revenue, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "3800", Name = "Rabatte / Discounts given", AccountType = AccountType.Revenue, OrganizationId = organizationId, IsSystemAccount = true },

        // COGS (4xxx)
        new() { AccountNumber = "4000", Name = "Materialaufwand / Material costs", AccountType = AccountType.Expense, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "4200", Name = "Warenaufwand Getränke / Beverage costs", AccountType = AccountType.Expense, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "4400", Name = "Einkauf Verbrauchsmaterial / Supply costs", AccountType = AccountType.Expense, OrganizationId = organizationId, IsSystemAccount = true },

        // Personnel (5xxx)
        new() { AccountNumber = "5000", Name = "Lohnaufwand / Wages", AccountType = AccountType.Expense, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "5700", Name = "Sozialversicherungen / Social security", AccountType = AccountType.Expense, OrganizationId = organizationId, IsSystemAccount = true },

        // Operating expenses (6xxx)
        new() { AccountNumber = "6000", Name = "Raumaufwand / Premises costs (rent)", AccountType = AccountType.Expense, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "6100", Name = "Unterhalt / Maintenance", AccountType = AccountType.Expense, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "6200", Name = "Fahrzeugaufwand / Vehicle expenses", AccountType = AccountType.Expense, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "6300", Name = "Sachversicherungen / Property insurance", AccountType = AccountType.Expense, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "6400", Name = "Energie / Utilities", AccountType = AccountType.Expense, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "6500", Name = "Verwaltungsaufwand / Admin expenses", AccountType = AccountType.Expense, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "6600", Name = "Werbeaufwand / Marketing", AccountType = AccountType.Expense, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "6800", Name = "Abschreibungen / Depreciation", AccountType = AccountType.Expense, OrganizationId = organizationId, IsSystemAccount = true },

        // Finance (6900)
        new() { AccountNumber = "6900", Name = "Finanzaufwand / Finance expenses", AccountType = AccountType.Expense, OrganizationId = organizationId, IsSystemAccount = true },
        new() { AccountNumber = "6950", Name = "Finanzertrag / Finance income", AccountType = AccountType.Revenue, OrganizationId = organizationId, IsSystemAccount = true },
    };

    public static List<TaxRate> GetSwissTaxRates(int organizationId) => new()
    {
        new() { Name = "Standard (8.1%)", RateType = VatRate.Standard, Rate = 8.1m, OrganizationId = organizationId, EffectiveFrom = new DateTime(2024, 1, 1) },
        new() { Name = "Reduced (2.6%)", RateType = VatRate.Reduced, Rate = 2.6m, OrganizationId = organizationId, EffectiveFrom = new DateTime(2024, 1, 1) },
        new() { Name = "Hospitality (3.8%)", RateType = VatRate.Hospitality, Rate = 3.8m, OrganizationId = organizationId, EffectiveFrom = new DateTime(2024, 1, 1) },
        new() { Name = "Exempt (0%)", RateType = VatRate.Exempt, Rate = 0m, OrganizationId = organizationId, EffectiveFrom = new DateTime(2024, 1, 1) },
    };
}
