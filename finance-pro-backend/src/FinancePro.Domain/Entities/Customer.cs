namespace FinancePro.Domain.Entities;

public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Street { get; set; }
    public string? HouseNumber { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; } = "CH";
    public string? VatId { get; set; }
    public int PaymentTermDays { get; set; } = 30;
    public decimal? CreditLimit { get; set; }
    public int OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
