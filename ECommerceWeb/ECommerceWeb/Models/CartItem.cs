namespace ECommerceWeb.Models;

public class CartItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public string Brand { get; set; } = string.Empty;
    public string Storage { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal SubTotal => Price * Quantity;
}
