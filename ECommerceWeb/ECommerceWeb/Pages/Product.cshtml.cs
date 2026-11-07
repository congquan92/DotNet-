using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ECommerceWeb.Data;
using ECommerceWeb.Entities;
using ECommerceWeb.Extensions;
using ECommerceWeb.Models;

namespace ECommerceWeb.Pages;

public class ProductModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public ProductModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Product Product { get; private set; } = null!;
    public List<Product> RelatedProducts { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound();

        Product = product;
        RelatedProducts = await _context.Products
            .Where(p => p.Id != id && (p.Brand == product.Brand || p.CategoryId == product.CategoryId))
            .Take(4)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAddAsync(int id, int quantity = 1, string? actionType = null)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        if (quantity < 1) quantity = 1;

        var cart = HttpContext.Session.GetCart();
        var item = cart.FirstOrDefault(i => i.ProductId == id);
        if (item != null)
        {
            item.Quantity = Math.Min(item.Quantity + quantity, 99);
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
                Quantity = quantity
            });
        }

        HttpContext.Session.SetCart(cart);
        TempData["SuccessMessage"] = $"Đã thêm \"{product.Name}\" ({quantity} sản phẩm) vào giỏ hàng!";

        if (actionType == "buyNow")
        {
            return RedirectToPage("/Cart");
        }

        return RedirectToPage("/Product", new { id });
    }
}
