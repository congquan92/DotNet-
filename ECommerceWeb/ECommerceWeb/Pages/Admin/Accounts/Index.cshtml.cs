using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ECommerceWeb.Entities;

namespace ECommerceWeb.Pages.Admin.Accounts;

public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public List<ApplicationUser> Users { get; set; } = new();
    public Dictionary<string, IList<string>> UserRoles { get; set; } = new();
    public Dictionary<string, bool> UserLockoutStatus { get; set; } = new();

    public async Task OnGetAsync()
    {
        Users = await _userManager.Users.AsNoTracking().ToListAsync();

        foreach (var u in Users)
        {
            UserRoles[u.Id] = await _userManager.GetRolesAsync(u);
            UserLockoutStatus[u.Id] = await _userManager.IsLockedOutAsync(u);
        }
    }

    public async Task<IActionResult> OnPostToggleLockAsync(string id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser != null && currentUser.Id == id)
        {
            TempData["ErrorMessage"] = "Bạn không thể tự khóa tài khoản của chính mình!";
            return RedirectToPage();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy người dùng!";
            return RedirectToPage();
        }

        var isLocked = await _userManager.IsLockedOutAsync(user);
        if (isLocked)
        {
            await _userManager.SetLockoutEndDateAsync(user, null);
            TempData["SuccessMessage"] = $"Đã mở khóa tài khoản @{user.UserName} thành công!";
        }
        else
        {
            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
            TempData["SuccessMessage"] = $"Đã khóa tài khoản @{user.UserName}!";
        }

        return RedirectToPage();
    }
}
