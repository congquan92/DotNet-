using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ECommerceWeb.Data;
using ECommerceWeb.Entities;
using ECommerceWeb.Extensions;
using ECommerceWeb.Models;

namespace ECommerceWeb.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Product> Products { get; set; } = new();

    public string? SearchKeyword { get; set; }

    public async Task OnGetAsync(string? search = null)
    {
        SearchKeyword = search;
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var kw = search.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(kw) || p.Brand.ToLower().Contains(kw));
        }

        Products = await query.OrderBy(p => p.Id).ToListAsync();
    }

    public async Task<IActionResult> OnPostAddAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        var cart = HttpContext.Session.GetCart();
        var item = cart.FirstOrDefault(i => i.ProductId == id);
        if (item != null)
        {
            item.Quantity = Math.Min(item.Quantity + 1, 99);
        }
        else
        {
            cart.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Brand = product.Brand,
                Storage = product.Storage,
                Color = product.Color,
                Quantity = 1
            });
        }

        HttpContext.Session.SetCart(cart);
        TempData["SuccessMessage"] = $"Đã thêm \"{product.Name}\" vào giỏ hàng thành công!";
        return RedirectToPage("/Cart");
    }
}
