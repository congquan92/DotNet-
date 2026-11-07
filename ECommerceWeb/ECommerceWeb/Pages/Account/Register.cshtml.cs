using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ECommerceWeb.Data;
using ECommerceWeb.Entities;
using ECommerceWeb.Models;

namespace ECommerceWeb.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public RegisterModel(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _context = context;
    }

    [BindProperty]
    public RegisterViewModel Input { get; set; } = new();

    public IActionResult OnGet()
    {
        if (_signInManager.IsSignedIn(User))
        {
            return RedirectToPage("/Index");
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var existingUser = await _userManager.FindByNameAsync(Input.Username);
        if (existingUser != null)
        {
            ModelState.AddModelError("Input.Username", "Tên đăng nhập này đã tồn tại.");
            return Page();
        }

        var user = new ApplicationUser
        {
            UserName = Input.Username.Trim(),
            Email = Input.Email.Trim(),
            PhoneNumber = Input.Phone.Trim(),
            FullName = Input.FullName.Trim(),
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, Input.Password);
        if (result.Succeeded)
        {
            // Mặc định gán Role "User"
            await _userManager.AddToRoleAsync(user, "User");

            // Tạo hồ sơ Customer tương ứng
            var customer = new Customer
            {
                FullName = Input.FullName.Trim(),
                Email = Input.Email.Trim(),
                Phone = Input.Phone.Trim(),
                Address = Input.Address?.Trim() ?? string.Empty,
                UserId = user.Id
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Tự động đăng nhập
            await _signInManager.SignInAsync(user, isPersistent: true);
            TempData["SuccessMessage"] = $"Đăng ký thành công! Chào mừng {user.FullName} gia nhập Mobi.";
            return RedirectToPage("/Index");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return Page();
    }
}
