using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ECommerceWeb.Entities;

namespace ECommerceWeb.Pages.Admin;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [BindProperty]
    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public IActionResult OnGet()
    {
        if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
        {
            return RedirectToPage("/Admin/Index");
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = await _userManager.FindByNameAsync(Username);
        if (user != null && await _userManager.IsLockedOutAsync(user))
        {
            ModelState.AddModelError(string.Empty, "⚠️ Tài khoản này hiện đang bị KHÓA bởi Quản trị viên.");
            return Page();
        }

        var result = await _signInManager.PasswordSignInAsync(
            Username,
            Password,
            isPersistent: true,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
            {
                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                return RedirectToPage("/Admin/Index");
            }

            // Nếu không phải admin
            await _signInManager.SignOutAsync();
            ModelState.AddModelError(string.Empty, "Tài khoản của bạn không có quyền truy cập trang quản trị.");
            return Page();
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "⚠️ Tài khoản này đã bị khóa do đăng nhập sai nhiều lần.");
            return Page();
        }

        ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác.");
        return Page();
    }
}
