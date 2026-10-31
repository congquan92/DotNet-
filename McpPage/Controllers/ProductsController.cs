using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using McpPage.Data;
using McpPage.Models;

namespace McpPage.Controllers;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Products
    public async Task<IActionResult> Index(int? categoryId, string? keyword, string? priceRange, string? sortBy)
    {
        var query = _context.Products.Include(p => p.Category).AsNoTracking().AsQueryable();

        // 1. Lọc theo danh mục
        if (categoryId.HasValue && categoryId.Value > 0)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        // 2. Tìm kiếm theo từ khóa
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var term = keyword.Trim();
            query = query.Where(p => EF.Functions.ILike(p.Name, $"%{term}%") || 
                                     EF.Functions.ILike(p.Description, $"%{term}%"));
        }

        // 3. Lọc theo khoảng giá
        if (!string.IsNullOrWhiteSpace(priceRange))
        {
            query = priceRange switch
            {
                "under5m" => query.Where(p => p.Price < 5000000),
                "5m-15m" => query.Where(p => p.Price >= 5000000 && p.Price <= 15000000),
                "15m-30m" => query.Where(p => p.Price >= 15000000 && p.Price <= 30000000),
                "above30m" => query.Where(p => p.Price > 30000000),
                _ => query
            };
        }

        // 4. Sắp xếp
        query = sortBy switch
        {
            "price_asc" => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            "name_asc" => query.OrderBy(p => p.Name),
            _ => query.OrderByDescending(p => p.Id)
        };

        var products = await query.ToListAsync();
        var categories = await _context.Categories.AsNoTracking().ToListAsync();

        var selectedCategoryName = categories.FirstOrDefault(c => c.Id == categoryId)?.Name;

        var viewModel = new ProductListViewModel
        {
            Products = products,
            Categories = categories,
            SelectedCategoryId = categoryId,
            SelectedCategoryName = selectedCategoryName,
            Keyword = keyword,
            PriceRange = priceRange,
            SortBy = sortBy,
            TotalCount = products.Count
        };

        return View(viewModel);
    }

    // GET: /Products/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        // Lấy 4 sản phẩm liên quan cùng danh mục
        var relatedProducts = await _context.Products
            .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
            .Take(4)
            .AsNoTracking()
            .ToListAsync();

        ViewBag.RelatedProducts = relatedProducts;

        return View(product);
    }
}
