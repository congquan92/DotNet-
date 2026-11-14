using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using McpPage.Data;
using McpPage.Data.Entities;
using McpPage.Models;

namespace McpPage.Controllers;

[Authorize(Roles = "Admin,Manager")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: /Admin (Dashboard tổng quan)
    public async Task<IActionResult> Index()
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .OrderByDescending(o => o.OrderDate)
            .AsNoTracking()
            .ToListAsync();

        var totalRevenue = orders.Where(o => o.Status != "Cancelled").Sum(o => o.TotalAmount);
        var totalOrders = orders.Count;
        var totalProducts = await _context.Products.CountAsync();
        var totalCustomers = await _context.Customers.CountAsync();

        var recentOrders = orders.Take(6).ToList();
        var lowStockProducts = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Stock <= 15)
            .OrderBy(p => p.Stock)
            .Take(5)
            .AsNoTracking()
            .ToListAsync();

        var model = new AdminDashboardViewModel
        {
            TotalRevenue = totalRevenue,
            TotalOrders = totalOrders,
            TotalProducts = totalProducts,
            TotalCustomers = totalCustomers,
            RecentOrders = recentOrders,
            LowStockProducts = lowStockProducts
        };

        return View(model);
    }

    // GET: /Admin/Products (Quản lý sản phẩm)
    public async Task<IActionResult> Products(string? search, int? categoryId)
    {
        var query = _context.Products.Include(p => p.Category).AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p => EF.Functions.ILike(p.Name, $"%{term}%"));
        }

        if (categoryId.HasValue && categoryId > 0)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        var products = await query.OrderByDescending(p => p.Id).ToListAsync();
        ViewBag.Categories = await _context.Categories.AsNoTracking().ToListAsync();
        ViewBag.Search = search;
        ViewBag.CategoryId = categoryId;

        return View(products);
    }

    // GET: /Admin/CreateProduct
    [HttpGet]
    public async Task<IActionResult> CreateProduct()
    {
        ViewBag.Categories = await _context.Categories.AsNoTracking().ToListAsync();
        return View(new Product());
    }

    // POST: /Admin/CreateProduct
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProduct(Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã thêm sản phẩm \"{product.Name}\" thành công!";
            return RedirectToAction(nameof(Products));
        }

        ViewBag.Categories = await _context.Categories.AsNoTracking().ToListAsync();
        return View(product);
    }

    // GET: /Admin/EditProduct/5
    [HttpGet]
    public async Task<IActionResult> EditProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        ViewBag.Categories = await _context.Categories.AsNoTracking().ToListAsync();
        return View(product);
    }

    // POST: /Admin/EditProduct/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(int id, Product model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã cập nhật sản phẩm \"{model.Name}\" thành công!";
                return RedirectToAction(nameof(Products));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                throw;
            }
        }

        ViewBag.Categories = await _context.Categories.AsNoTracking().ToListAsync();
        return View(model);
    }

    // POST: /Admin/DeleteProduct/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã xóa sản phẩm \"{product.Name}\"!";
        }

        return RedirectToAction(nameof(Products));
    }

    // GET: /Admin/Orders (Quản lý đơn hàng)
    public async Task<IActionResult> Orders(string? status)
    {
        var query = _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) && status != "All")
        {
            query = query.Where(o => o.Status == status);
        }

        var orders = await query.OrderByDescending(o => o.OrderDate).ToListAsync();
        ViewBag.Status = status ?? "All";

        return View(orders);
    }

    // POST: /Admin/UpdateOrderStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateOrderStatus(int id, string newStatus)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order != null)
        {
            order.Status = newStatus;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã cập nhật đơn hàng #{order.Id} sang trạng thái: {newStatus}!";
        }

        return RedirectToAction(nameof(Orders));
    }

    // GET: /Admin/Accounts (Quản lý tài khoản Identity)
    public async Task<IActionResult> Accounts()
    {
        var users = await _userManager.Users.AsNoTracking().ToListAsync();
        var userRoles = new Dictionary<string, IList<string>>();
        var userLockoutStatus = new Dictionary<string, bool>();

        foreach (var user in users)
        {
            userRoles[user.Id] = await _userManager.GetRolesAsync(user);
            userLockoutStatus[user.Id] = await _userManager.IsLockedOutAsync(user);
        }

        ViewBag.UserRoles = userRoles;
        ViewBag.UserLockoutStatus = userLockoutStatus;
        return View(users);
    }

    // POST: /Admin/ToggleUserLock
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleUserLock(string id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser != null && currentUser.Id == id)
        {
            TempData["ErrorMessage"] = "Bạn không thể tự khóa tài khoản của chính mình!";
            return RedirectToAction(nameof(Accounts));
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy người dùng!";
            return RedirectToAction(nameof(Accounts));
        }

        var isLocked = await _userManager.IsLockedOutAsync(user);
        if (isLocked)
        {
            // Mở khóa
            await _userManager.SetLockoutEndDateAsync(user, null);
            TempData["SuccessMessage"] = $"Đã mở khóa tài khoản @{user.UserName} thành công!";
        }
        else
        {
            // Khóa tài khoản
            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
            TempData["SuccessMessage"] = $"Đã khóa tài khoản @{user.UserName}!";
        }

        return RedirectToAction(nameof(Accounts));
    }
}
