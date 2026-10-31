namespace McpPage.Models;

public class CartViewModel
{
    public List<CartItem> Items { get; set; } = new();
    public decimal TotalAmount => Items.Sum(i => i.SubTotal);
    public int TotalItems => Items.Sum(i => i.Quantity);
}
