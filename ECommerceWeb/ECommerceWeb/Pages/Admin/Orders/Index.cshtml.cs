using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ECommerceWeb.Data;
using ECommerceWeb.Entities;

namespace ECommerceWeb.Pages.Admin.Orders;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Order> Orders { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string Status { get; set; } = "All";

    public async Task OnGetAsync()
    {
        var query = _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(Status) && Status != "All")
        {
            query = query.Where(o => o.Status == Status);
        }

        Orders = await query.OrderByDescending(o => o.OrderDate).ToListAsync();
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(int id, string newStatus)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order != null)
        {
            order.Status = newStatus;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã cập nhật đơn hàng #{order.Id} sang trạng thái: {newStatus}!";
        }
        return RedirectToPage(new { status = Status });
    }
}
