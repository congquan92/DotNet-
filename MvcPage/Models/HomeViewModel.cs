using McpPage.Data.Entities;

namespace McpPage.Models;

public class HomeViewModel
{
    public List<Category> Categories { get; set; } = new();
    public List<Product> FlashSaleProducts { get; set; } = new();
    public List<Product> FeaturedProducts { get; set; } = new();
    public Dictionary<string, List<Product>> CategorizedProducts { get; set; } = new();
}
