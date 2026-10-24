using ECommerceWeb.Data;
using ECommerceWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ECommerceWeb.Pages.Admin;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class IndexModel : PageModel
{
    public IReadOnlyList<Product> Products => ProductCatalog.All;
    public int BrandCount => Products.Select(product => product.Brand).Distinct().Count();
    public decimal LowestPrice => Products.Min(product => product.Price);
    public decimal HighestPrice => Products.Max(product => product.Price);
}
