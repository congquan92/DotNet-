using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ECommerceWeb.Entities;
using ECommerceWeb.Models;

namespace ECommerceWeb.Pages.Account;

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
    public LoginViewModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public IActionResult OnGet(string? returnUrl = null)
    {
        if (_signInManager.IsSignedIn(User))
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToPage("/Admin/Index");
            }
            return RedirectToPage("/Index");
        }

        ReturnUrl = returnUrl;
        Input.ReturnUrl = returnUrl;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Input.ReturnUrl;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.FindByNameAsync(Input.Username);
        if (user != null && await _userManager.IsLockedOutAsync(user))
        {
            ModelState.AddModelError(string.Empty, "⚠️ Tài khoản này hiện đang bị KHÓA bởi Quản trị viên.");
            return Page();
        }

        var result = await _signInManager.PasswordSignInAsync(
            Input.Username,
            Input.Password,
            Input.RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = $"Đăng nhập thành công! Chào mừng {Input.Username}.";

            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }

            if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return RedirectToPage("/Admin/Index");
            }

            return RedirectToPage("/Index");
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
