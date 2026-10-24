using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ECommerceWeb.Pages.Admin;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class LoginModel(IConfiguration configuration, ILogger<LoginModel> logger) : PageModel
{
    [BindProperty]
    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
    [StringLength(100, ErrorMessage = "Tên đăng nhập quá dài.")]
    public string Username { get; set; } = "";

    [BindProperty]
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [StringLength(256, ErrorMessage = "Mật khẩu quá dài.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public IActionResult OnGet()
    {
        return User.IsInRole("Admin") ? RedirectToPage("/Admin/Index") : Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var adminUsername = configuration["Admin:Username"];
        var passwordHash = configuration["Admin:PasswordHash"];
        var result = PasswordVerificationResult.Failed;

        if (!string.IsNullOrWhiteSpace(adminUsername) && !string.IsNullOrWhiteSpace(passwordHash))
        {
            try
            {
                result = new PasswordHasher<string>().VerifyHashedPassword(adminUsername, passwordHash, Password);
            }
            catch (FormatException)
            {
                logger.LogError("The configured admin password hash is invalid.");
            }
        }

        if (result == PasswordVerificationResult.Failed ||
            !string.Equals(Username, adminUsername, StringComparison.Ordinal))
        {
            ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
            return Page();
        }

        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.Name, adminUsername!), new Claim(ClaimTypes.Role, "Admin")],
            CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = false });

        return Url.IsLocalUrl(ReturnUrl)
            ? LocalRedirect(ReturnUrl!)
            : RedirectToPage("/Admin/Index");
    }
}
