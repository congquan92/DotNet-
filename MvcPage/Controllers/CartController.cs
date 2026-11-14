using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using McpPage.Data;
using McpPage.Data.Entities;
using McpPage.Models;

namespace McpPage.Controllers;

public class CartController : Controller
{
    private const string CartSessionKey = "CartSession";
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Helper: Đọc giỏ hàng từ Session
    private List<CartItem> GetCart()
    {
        var json = HttpContext.Session.GetString(CartSessionKey);
        if (string.IsNullOrEmpty(json))
        {
            return new List<CartItem>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
        }
        catch
        {
            return new List<CartItem>();
        }
    }

    // Helper: Lưu giỏ hàng vào Session
    private void SaveCart(List<CartItem> cart)
    {
        var json = JsonSerializer.Serialize(cart);
        HttpContext.Session.SetString(CartSessionKey, json);
    }

    // GET: /Cart
    public IActionResult Index()
    {
        var cart = GetCart();
        var model = new CheckoutViewModel
        {
            Cart = new CartViewModel { Items = cart }
        };

        return View(model);
    }

    // POST: /Cart/AddToCart
    [HttpPost]
    public async Task<IActionResult> AddToCart(int productId, int quantity = 1, string? actionType = null)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy sản phẩm!";
            return RedirectToAction("Index", "Products");
        }

        if (quantity < 1) quantity = 1;

        var cart = GetCart();
        var existingItem = cart.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            cart.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Quantity = quantity
            });
        }

        SaveCart(cart);
        TempData["SuccessMessage"] = $"Đã thêm \"{product.Name}\" vào giỏ hàng!";

        if (actionType == "buyNow")
        {
            return RedirectToAction("Index");
        }

        var referer = Request.Headers["Referer"].ToString();
        return !string.IsNullOrEmpty(referer) ? Redirect(referer) : RedirectToAction("Index");
    }

    // POST: /Cart/UpdateQuantity
    [HttpPost]
    public IActionResult UpdateQuantity(int productId, int quantity)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            if (quantity <= 0)
            {
                cart.Remove(item);
                TempData["SuccessMessage"] = "Đã xóa sản phẩm khỏi giỏ hàng!";
            }
            else
            {
                item.Quantity = quantity;
                TempData["SuccessMessage"] = "Đã cập nhật số lượng thành công!";
            }
            SaveCart(cart);
        }

        return RedirectToAction("Index");
    }

    // POST: /Cart/RemoveFromCart
    [HttpPost]
    public IActionResult RemoveFromCart(int productId)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            cart.Remove(item);
            SaveCart(cart);
            TempData["SuccessMessage"] = "Đã xóa sản phẩm khỏi giỏ hàng!";
        }

        return RedirectToAction("Index");
    }

    // POST: /Cart/Checkout
    [HttpPost]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        var cart = GetCart();
        if (!cart.Any())
        {
            TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống!";
            return RedirectToAction("Index");
        }

        model.Cart = new CartViewModel { Items = cart };

        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        // 1. Kiểm tra hoặc tạo Customer mới
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Phone == model.Phone);
        if (customer == null)
        {
            customer = new Customer
            {
                FullName = model.FullName,
                Phone = model.Phone,
                Address = model.Address,
                Email = model.Email ?? string.Empty
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }

        // 2. Tạo đơn hàng Order và OrderDetail
        var order = new Order
        {
            CustomerId = customer.Id,
            OrderDate = DateTime.UtcNow,
            ShippingAddress = model.Address,
            Status = "Pending",
            TotalAmount = cart.Sum(i => i.SubTotal),
            OrderDetails = cart.Select(i => new OrderDetail
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.Price
            }).ToList()
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // 3. Xóa giỏ hàng sau khi đặt thành công
        HttpContext.Session.Remove(CartSessionKey);

        TempData["SuccessMessage"] = "Đặt hàng thành công! Cảm ơn bạn đã mua hàng tại MCP Shop.";
        return RedirectToAction("OrderSuccess", new { id = order.Id });
    }

    // GET: /Cart/OrderSuccess/5
    public async Task<IActionResult> OrderSuccess(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(order);
    }
}
