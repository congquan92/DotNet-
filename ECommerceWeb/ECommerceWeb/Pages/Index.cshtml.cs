using ECommerceWeb.Data;
using ECommerceWeb.Extensions;
using ECommerceWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ECommerceWeb.Pages;


public class IndexModel : PageModel
{
    public IReadOnlyList<Product> Products =>
        ProductCatalog.All;

    public IActionResult OnPostAdd(int id)
    {
        var product = ProductCatalog.Find(id);

        if (product == null)
            return NotFound();
        
        var cart = HttpContext.Session.GetCart();
        
        cart[id] = Math.Min(cart.GetValueOrDefault(id) + 1, 99);
        
        HttpContext.Session.SetCart(cart);

        return RedirectToPage("/Cart");
    }
}