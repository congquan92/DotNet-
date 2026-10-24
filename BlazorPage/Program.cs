using System.Security.Claims;
using BlazorPage.Components;
using Microsoft.EntityFrameworkCore;
using BlazorPage.Data;
using BlazorPage.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = ".BlazorShop.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.LoginPath = "/login";
        options.LogoutPath = "/api/auth/logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register State Services
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

// Seed Database automatically on startup
try
{
    using var scope = app.Services.CreateScope();
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    await using var db = await dbFactory.CreateDbContextAsync();
    await DbInitializer.SeedAsync(db);
}
catch (Exception ex)
{
    Console.WriteLine($"[Database Init Error]: {ex.Message}");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// Minimal API Endpoints for Cookie Authentication
app.MapPost("/api/auth/login", async (HttpContext httpContext, LoginRequest request, IDbContextFactory<AppDbContext> dbFactory) =>
{
    if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.BadRequest(new { message = "Vui lòng nhập tên đăng nhập và mật khẩu!" });
    }

    await using var db = await dbFactory.CreateDbContextAsync();
    var account = await db.Accounts
        .Include(a => a.Customer)
        .FirstOrDefaultAsync(a => a.Username == request.Username.Trim() && a.Password == request.Password.Trim());

    if (account == null)
    {
        return Results.BadRequest(new { message = "Tên đăng nhập hoặc mật khẩu không chính xác!" });
    }

    if (account.Status == "Locked")
    {
        return Results.BadRequest(new { message = "Tài khoản của bạn đã bị khóa! Vui lòng liên hệ quản trị viên." });
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, account.Id.ToString()),
        new(ClaimTypes.Name, account.Username),
        new(ClaimTypes.Role, account.Role),
        new(ClaimTypes.GivenName, account.Customer?.FullName ?? account.Username),
        new(ClaimTypes.Email, account.Customer?.Email ?? string.Empty),
        new(ClaimTypes.MobilePhone, account.Customer?.Phone ?? string.Empty),
        new(ClaimTypes.StreetAddress, account.Customer?.Address ?? string.Empty),
        new("CustomerId", account.Customer?.Id.ToString() ?? string.Empty)
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);

    var authProperties = new AuthenticationProperties
    {
        IsPersistent = true,
        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
    };

    await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

    var redirectUrl = account.Role == "Admin" ? "/admin/dashboard" : "/profile";
    return Results.Ok(new { success = true, role = account.Role, redirectUrl });
}).DisableAntiforgery();

app.MapGet("/api/auth/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});

app.MapPost("/api/auth/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
}).DisableAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

public record LoginRequest(string Username, string Password);
