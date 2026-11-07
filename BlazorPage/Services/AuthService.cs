using System.Security.Claims;
using BlazorPage.Data;
using BlazorPage.Data.Entities;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace BlazorPage.Services;

public class CurrentUserInfo
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
}

public class AuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceProvider _serviceProvider;
    private readonly AuthenticationStateProvider? _authStateProvider;
    private bool _isInitialized = false;

    public CurrentUserInfo? CurrentUser { get; private set; }

    public bool IsLoggedIn => CurrentUser != null;
    public bool IsAdmin => CurrentUser?.Role == "Admin";

    public event Action? OnChange;

    public AuthService(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
    {
        _httpContextAccessor = httpContextAccessor;
        _serviceProvider = serviceProvider;
        _authStateProvider = serviceProvider.GetService<AuthenticationStateProvider>();
    }

    public async Task InitializeAsync()
    {
        try
        {
            ClaimsPrincipal? user = null;

            // 1. Kiểm tra từ HttpContext (SSR & initial HTTP request)
            if (_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true)
            {
                user = _httpContextAccessor.HttpContext.User;
            }
            // 2. Kiểm tra từ AuthenticationStateProvider (WebSocket SignalR circuit)
            else if (_authStateProvider != null)
            {
                var authState = await _authStateProvider.GetAuthenticationStateAsync();
                if (authState.User.Identity?.IsAuthenticated == true)
                {
                    user = authState.User;
                }
            }

            if (user != null && user.Identity?.IsAuthenticated == true)
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                var username = user.Identity.Name ?? string.Empty;
                var role = user.FindFirst(ClaimTypes.Role)?.Value ?? "User";

                Customer? customer = null;
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
                    await using var db = await dbFactory.CreateDbContextAsync();
                    customer = await db.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
                }
                catch (Exception dbEx)
                {
                    Console.WriteLine($"[AuthService DB Error]: {dbEx.Message}");
                }

                CurrentUser = new CurrentUserInfo
                {
                    Id = userId,
                    Username = username,
                    Role = role,
                    FullName = customer?.FullName ?? user.FindFirst(ClaimTypes.GivenName)?.Value ?? username,
                    Email = customer?.Email ?? user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
                    Phone = customer?.Phone ?? user.FindFirst(ClaimTypes.MobilePhone)?.Value ?? string.Empty,
                    Address = customer?.Address ?? user.FindFirst(ClaimTypes.StreetAddress)?.Value ?? string.Empty,
                    CustomerId = customer?.Id
                };
            }
            else
            {
                CurrentUser = null;
            }
            _isInitialized = true;
            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthService Init Error]: {ex.Message}");
            CurrentUser = null;
        }
    }

    public Task InitializeAsync(IJSRuntime? js) => InitializeAsync();

    public async Task EnsureInitializedAsync()
    {
        if (!_isInitialized)
        {
            await InitializeAsync();
        }
    }

    public void Login(ApplicationUser user, string? role = null, string? customerFullName = null)
    {
        CurrentUser = new CurrentUserInfo
        {
            Id = user.Id,
            Username = user.UserName ?? string.Empty,
            Role = role ?? "User",
            FullName = !string.IsNullOrWhiteSpace(customerFullName) ? customerFullName : (user.FullName ?? user.UserName ?? string.Empty)
        };
        NotifyStateChanged();
    }

    public Task UpdateProfileAsync(string fullName, string email, string phone, string address, IJSRuntime? js = null)
    {
        if (CurrentUser != null)
        {
            CurrentUser.FullName = fullName;
            CurrentUser.Email = email;
            CurrentUser.Phone = phone;
            CurrentUser.Address = address;
            NotifyStateChanged();
        }
        return Task.CompletedTask;
    }

    public Task LogoutAsync(IJSRuntime? js = null)
    {
        CurrentUser = null;
        NotifyStateChanged();
        return Task.CompletedTask;
    }

    public void Logout()
    {
        CurrentUser = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
