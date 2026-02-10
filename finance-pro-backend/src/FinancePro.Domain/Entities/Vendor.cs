namespace FinancePro.Domain.Entities;

public class Vendor : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Iban { get; set; }
    public int PaymentTermDays { get; set; } = 30;
    public int OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
