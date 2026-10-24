namespace BlazorPage.Data.Entities;

public class Customer
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public int? AccountId { get; set; }
    public Account? Account { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
