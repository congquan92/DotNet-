using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ECommerceWeb.Data;
using ECommerceWeb.Entities;

namespace ECommerceWeb.Pages.Admin.Products;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Product Product { get; set; } = new() { Stock = 50 };

    public List<Category> Categories { get; set; } = new();

    public async Task OnGetAsync()
    {
        Categories = await _context.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Categories = await _context.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();
            return Page();
        }

        if (string.IsNullOrWhiteSpace(Product.ImageUrl))
        {
            Product.ImageUrl = "https://placehold.co/500x500?text=" + Uri.EscapeDataString(Product.Name);
        }

        _context.Products.Add(Product);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Đã thêm sản phẩm \"{Product.Name}\" thành công!";
        return RedirectToPage("/Admin/Products/Index");
    }
}
