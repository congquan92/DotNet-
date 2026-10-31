using McpPage.Data.Entities;

namespace McpPage.Models;

public class ProductListViewModel
{
    public List<Product> Products { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public int? SelectedCategoryId { get; set; }
    public string? SelectedCategoryName { get; set; }
    public string? Keyword { get; set; }
    public string? PriceRange { get; set; }
    public string? SortBy { get; set; }
    public int TotalCount { get; set; }
}
