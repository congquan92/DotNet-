using ECommerceWeb.Data;
using ECommerceWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ECommerceWeb.Pages;

public class ProductModel : PageModel
{
    public Product Product { get; private set; } = null!;

    public IActionResult OnGet(int id)
    {
        var product = ProductCatalog.Find(id);

        if (product == null)
            return NotFound();

        Product = product;
        return Page();
    }
}
