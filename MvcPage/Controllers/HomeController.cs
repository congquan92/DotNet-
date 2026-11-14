using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using McpPage.Data;
using McpPage.Models;

namespace McpPage.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories.AsNoTracking().ToListAsync();
        var allProducts = await _context.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .ToListAsync();

        // Flash Sale: Lấy 5 sản phẩm hot giảm giá sốc
        var flashSaleProducts = allProducts.Take(5).ToList();

        // Sản phẩm nổi bật
        var featuredProducts = allProducts.Skip(5).Take(8).ToList();

        // Phân loại sản phẩm theo từng danh mục
        var categorizedProducts = new Dictionary<string, List<Data.Entities.Product>>();
        foreach (var cat in categories)
        {
            var prods = allProducts.Where(p => p.CategoryId == cat.Id).Take(4).ToList();
            if (prods.Any())
            {
                categorizedProducts[cat.Name] = prods;
            }
        }

        var viewModel = new HomeViewModel
        {
            Categories = categories,
            FlashSaleProducts = flashSaleProducts,
            FeaturedProducts = featuredProducts,
            CategorizedProducts = categorizedProducts
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
