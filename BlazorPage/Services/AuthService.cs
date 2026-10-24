using System.Security.Claims;
using BlazorPage.Data.Entities;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace BlazorPage.Services;

public class CurrentUserInfo
{
    public int Id { get; set; }
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
    private readonly AuthenticationStateProvider? _authStateProvider;
    private bool _isInitialized = false;

    public CurrentUserInfo? CurrentUser { get; private set; }

    public bool IsLoggedIn => CurrentUser != null;
    public bool IsAdmin => CurrentUser?.Role == "Admin";

    public event Action? OnChange;

    public AuthService(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
    {
        _httpContextAccessor = httpContextAccessor;
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
                CurrentUser = new CurrentUserInfo
                {
                    Id = int.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0,
                    Username = user.Identity.Name ?? string.Empty,
                    Role = user.FindFirst(ClaimTypes.Role)?.Value ?? "User",
                    FullName = user.FindFirst(ClaimTypes.GivenName)?.Value ?? user.Identity.Name ?? string.Empty,
                    Email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
                    Phone = user.FindFirst(ClaimTypes.MobilePhone)?.Value ?? string.Empty,
                    Address = user.FindFirst(ClaimTypes.StreetAddress)?.Value ?? string.Empty,
                    CustomerId = int.TryParse(user.FindFirst("CustomerId")?.Value, out var cid) ? cid : null
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

    // Overload tương thích ngược
    public Task InitializeAsync(IJSRuntime? js) => InitializeAsync();

    public async Task EnsureInitializedAsync()
    {
        if (!_isInitialized)
        {
            await InitializeAsync();
        }
    }

    public void Login(Account account, string? customerFullName = null)
    {
        CurrentUser = new CurrentUserInfo
        {
            Id = account.Id,
            Username = account.Username,
            Role = account.Role,
            FullName = !string.IsNullOrWhiteSpace(customerFullName) ? customerFullName : account.Username
        };
        NotifyStateChanged();
    }

    public Task LoginAsync(Account account, Customer? customer, IJSRuntime? js = null)
    {
        CurrentUser = new CurrentUserInfo
        {
            Id = account.Id,
            Username = account.Username,
            Role = account.Role,
            FullName = !string.IsNullOrWhiteSpace(customer?.FullName) ? customer.FullName : account.Username,
            Email = customer?.Email ?? string.Empty,
            Phone = customer?.Phone ?? string.Empty,
            Address = customer?.Address ?? string.Empty,
            CustomerId = customer?.Id
        };
        NotifyStateChanged();
        return Task.CompletedTask;
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
