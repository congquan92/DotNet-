using ECommerceWeb.Data;
using ECommerceWeb.Extensions;
using ECommerceWeb.Models;

namespace ECommerceWeb.Pages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


public class CartModel : PageModel
{
    public List<CartRow> Items { get; set; } = new();

    public decimal Total => Items.Sum(x => x.Subtotal);

    public void OnGet()
    {
        LoadCart();
    }

    private void LoadCart()
    {
        var cart = HttpContext.Session.GetCart();

        Items = new List<CartRow>();

        foreach (var item in cart)
        {
            var product = ProductCatalog.Find(item.Key);

            if (product != null && item.Value > 0)
            {
                Items.Add(new CartRow(product, item.Value));
            }
        }
    }
    
    public IActionResult OnPostUpdate(int id, int quantity)
    {
        var cart = HttpContext.Session.GetCart();

        if (!cart.ContainsKey(id))
            return RedirectToPage();

        if (quantity <= 0)
        {
            cart.Remove(id);
        }
        else
        {
            cart[id] = Math.Min(quantity, 99);
        }

        HttpContext.Session.SetCart(cart);

        return RedirectToPage();
    }
    
    public IActionResult OnPostRemove(int id)
    {
        var cart = HttpContext.Session.GetCart();

        cart.Remove(id);

        HttpContext.Session.SetCart(cart);

        return RedirectToPage();
    }
    
    public IActionResult OnPostCheckout()
    {
        var cart = HttpContext.Session.GetCart();

        if (cart.Count == 0)
            return RedirectToPage();

        HttpContext.Session.Remove("Cart");

        TempData["Success"] =
            "Đặt hàng giả lập thành công!";

        return RedirectToPage();
    }
}

public record CartRow(Product Product, int Quantity)
{
    public decimal Subtotal => Product.Price * Quantity;
}