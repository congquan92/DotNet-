using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ECommerceWeb.Data;
using ECommerceWeb.Entities;

namespace ECommerceWeb.Pages.Admin;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public int TotalProducts { get; set; }
    public int TotalCustomers { get; set; }

    public List<Order> RecentOrders { get; set; } = new();
    public List<Product> Products { get; set; } = new();
    public List<Product> LowStockProducts { get; set; } = new();

    public async Task OnGetAsync()
    {
        TotalOrders = await _context.Orders.CountAsync();
        TotalProducts = await _context.Products.CountAsync();
        TotalCustomers = await _context.Customers.CountAsync();
        TotalRevenue = await _context.Orders
            .Where(o => o.Status != "Cancelled")
            .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

        RecentOrders = await _context.Orders
            .Include(o => o.Customer)
            .OrderByDescending(o => o.OrderDate)
            .Take(5)
            .ToListAsync();

        Products = await _context.Products
            .Include(p => p.Category)
            .OrderByDescending(p => p.Id)
            .ToListAsync();

        LowStockProducts = await _context.Products
            .OrderBy(p => p.Stock)
            .Take(4)
            .ToListAsync();
    }
}
