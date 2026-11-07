using System.Security.Claims;
using BlazorPage.Components;
using Microsoft.EntityFrameworkCore;
using BlazorPage.Data;
using BlazorPage.Data.Entities;
using BlazorPage.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// 1. Cấu hình DbContextFactory & Scoped DbContext cho Identity
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped(sp => 
    sp.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext());

// 2. Cấu hình ASP.NET Core Identity chuẩn giống McpPage
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;

    options.User.RequireUniqueEmail = false;

    // Cấu hình Khóa tài khoản (Lockout)
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(36500);
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".BlazorShop.Identity";
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

// Register State Services
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

// Seed Database tự động khi khởi chạy
try
{
    using var scope = app.Services.CreateScope();
    await DbInitializer.SeedAsync(scope.ServiceProvider);
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

// Minimal API Endpoints xác thực Identity Cookie
app.MapPost("/api/auth/login", async (
    HttpContext httpContext, 
    LoginRequest request, 
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager) =>
{
    if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.BadRequest(new { message = "Vui lòng nhập tên đăng nhập và mật khẩu!" });
    }

    var user = await userManager.FindByNameAsync(request.Username.Trim());
    if (user == null)
    {
        return Results.BadRequest(new { message = "Tên đăng nhập hoặc mật khẩu không chính xác!" });
    }

    if (await userManager.IsLockedOutAsync(user))
    {
        return Results.BadRequest(new { message = "⚠️ Tài khoản này hiện đang bị KHÓA bởi Quản trị viên." });
    }

    var result = await signInManager.PasswordSignInAsync(
        request.Username.Trim(),
        request.Password.Trim(),
        isPersistent: true,
        lockoutOnFailure: true);

    if (result.Succeeded)
    {
        var roles = await userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "User";
        var redirectUrl = role == "Admin" ? "/admin/dashboard" : "/profile";

        return Results.Ok(new { success = true, role = role, redirectUrl });
    }

    if (result.IsLockedOut)
    {
        return Results.BadRequest(new { message = "⚠️ Tài khoản này hiện đang bị KHÓA bởi Quản trị viên." });
    }

    return Results.BadRequest(new { message = "Tên đăng nhập hoặc mật khẩu không chính xác!" });
}).DisableAntiforgery();

app.MapGet("/api/auth/logout", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
});

app.MapPost("/api/auth/logout", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
}).DisableAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

public record LoginRequest(string Username, string Password);
