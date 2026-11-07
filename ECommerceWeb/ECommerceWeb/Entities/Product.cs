namespace ECommerceWeb.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int Stock { get; set; } = 100;

    public string Brand { get; set; } = string.Empty;
    public string Storage { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Display { get; set; } = string.Empty;
    public string Chip { get; set; } = string.Empty;
    public string Camera { get; set; } = string.Empty;
    public string Battery { get; set; } = string.Empty;

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
