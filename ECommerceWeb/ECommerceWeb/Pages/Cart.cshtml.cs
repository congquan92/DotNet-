using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ECommerceWeb.Data;
using ECommerceWeb.Entities;
using ECommerceWeb.Extensions;
using ECommerceWeb.Models;

namespace ECommerceWeb.Pages;

public class CartModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CartModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<CartItem> Items { get; set; } = new();

    public decimal TotalAmount => Items.Sum(x => x.SubTotal);
    public int TotalItems => Items.Sum(x => x.Quantity);

    [BindProperty]
    [Required(ErrorMessage = "Vui lòng nhập họ và tên người nhận.")]
    public string FullName { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    public string Phone { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng.")]
    public string Address { get; set; } = string.Empty;

    [BindProperty]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    public string? Email { get; set; }

    [BindProperty]
    public string? Note { get; set; }

    public async Task OnGetAsync()
    {
        Items = HttpContext.Session.GetCart();

        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);
                FullName = customer?.FullName ?? user.FullName ?? user.UserName ?? string.Empty;
                Phone = customer?.Phone ?? user.PhoneNumber ?? string.Empty;
                Address = customer?.Address ?? string.Empty;
                Email = customer?.Email ?? user.Email;
            }
        }
    }

    public IActionResult OnPostUpdate(int id, int quantity)
    {
        var cart = HttpContext.Session.GetCart();
        var item = cart.FirstOrDefault(i => i.ProductId == id);

        if (item != null)
        {
            if (quantity <= 0)
            {
                cart.Remove(item);
            }
            else
            {
                item.Quantity = Math.Min(quantity, 99);
            }
            HttpContext.Session.SetCart(cart);
        }

        return RedirectToPage();
    }

    public IActionResult OnPostRemove(int id)
    {
        var cart = HttpContext.Session.GetCart();
        var item = cart.FirstOrDefault(i => i.ProductId == id);
        if (item != null)
        {
            cart.Remove(item);
            HttpContext.Session.SetCart(cart);
        }

        TempData["SuccessMessage"] = "Đã xóa sản phẩm khỏi giỏ hàng!";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCheckoutAsync()
    {
        Items = HttpContext.Session.GetCart();
        if (Items.Count == 0)
        {
            TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống!";
            return RedirectToPage();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        Customer? customer = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);
            }
        }

        if (customer == null)
        {
            customer = await _context.Customers.FirstOrDefaultAsync(c => c.Phone == Phone.Trim());
        }

        if (customer == null)
        {
            customer = new Customer
            {
                FullName = FullName.Trim(),
                Phone = Phone.Trim(),
                Address = Address.Trim(),
                Email = !string.IsNullOrWhiteSpace(Email) ? Email.Trim() : $"{Phone.Trim()}@guest.mobi"
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }
        else
        {
            customer.FullName = FullName.Trim();
            customer.Phone = Phone.Trim();
            customer.Address = Address.Trim();
            if (!string.IsNullOrWhiteSpace(Email))
            {
                customer.Email = Email.Trim();
            }
            await _context.SaveChangesAsync();
        }

        var order = new Order
        {
            CustomerId = customer.Id,
            OrderDate = DateTime.UtcNow,
            TotalAmount = Items.Sum(i => i.SubTotal),
            Status = "Pending",
            ShippingAddress = Address.Trim()
        };

        foreach (var item in Items)
        {
            order.OrderDetails.Add(new OrderDetail
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.Price
            });
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Xóa giỏ hàng sau khi đặt thành công
        HttpContext.Session.SetCart(new List<CartItem>());

        TempData["SuccessMessage"] = $"🎉 Đặt hàng thành công! Mã đơn hàng của bạn là #{order.Id}. Đội ngũ Mobi Shop sẽ liên hệ xác nhận sớm nhất.";
        return RedirectToPage();
    }
}
