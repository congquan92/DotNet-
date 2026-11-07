# Chuẩn hóa Auth, Cart và Cấu hình EF Core cho ECommerceWeb và BlazorPage theo McpPage

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking. (Do NOT run `git commit` or `git push` per workspace rules).

**Goal:** Chuẩn hóa toàn diện 2 dự án `ECommerceWeb` (Razor Pages) và `BlazorPage` (Blazor Server) về Quản lý Authentication (ASP.NET Core Identity băm mật khẩu PBKDF2, Roles, Lockout), Giỏ hàng & State (CartItem JSON / Session / đồng bộ model), và Cấu hình EF Core (Fluent API tách rời `IEntityTypeConfiguration<T>`, `ApplyConfigurationsFromAssembly`, Entity đồng nhất) giống như `McpPage`.

**Architecture:** 
- **Database & EF Core:** Đưa `ApplicationUser : IdentityUser`, `Customer`, `Category`, `Product`, `Order`, `OrderDetail` làm bộ Entity chuẩn; tách biệt từng cấu hình entity thành class triển khai `IEntityTypeConfiguration<T>` trong `Data/Configurations/`; DbContext kế thừa `IdentityDbContext<ApplicationUser>`.
- **Authentication:** Thay thế cookie auth thủ công/mật khẩu plaintext bằng chuẩn ASP.NET Core Identity (`UserManager`, `SignInManager`, bảo mật băm PBKDF2, phân quyền Role Admin/User, khóa tài khoản). ECommerceWeb dùng Razor Pages Identity, BlazorPage kết hợp Minimal API + `SignInManager` + `CascadingAuthenticationState`.
- **Cart & State:** ECommerceWeb dùng `ISession` lưu JSON `List<CartItem>` đọc từ DbContext; BlazorPage chuẩn hóa `CartItem` và logic giỏ hàng tương thích hoàn toàn mô hình của McpPage.

**Tech Stack:** .NET 10, ASP.NET Core Identity, Entity Framework Core 10, Npgsql PostgreSQL, Razor Pages, Blazor Server (Interactive Server).

---

## Danh sách Task triển khai

### Task 1: Chuẩn hóa EF Core & Identity Packages cho ECommerceWeb
- [ ] Thêm Package `Microsoft.AspNetCore.Identity.EntityFrameworkCore` vào `ECommerceWeb/ECommerceWeb/ECommerceWeb.csproj`.
- [ ] Cập nhật Entity đồng nhất: `ApplicationUser.cs`, `Customer.cs`, `Category.cs`, `Product.cs`, `Order.cs`, `OrderDetail.cs`.
- [ ] Tạo các class Fluent API tách rời trong `Data/Configurations/`: `CategoryConfiguration`, `ProductConfiguration`, `CustomerConfiguration`, `OrderConfiguration`, `OrderDetailConfiguration`.
- [ ] Cập nhật `ApplicationDbContext` kế thừa `IdentityDbContext<ApplicationUser>` và gọi `ApplyConfigurationsFromAssembly`.
- [ ] Viết `DbInitializer.cs` tự động seed Roles, Admin/User Identity, Categories, Products mẫu.

### Task 2: Chuẩn hóa Authentication & Program.cs cho ECommerceWeb
- [ ] Cấu hình ASP.NET Core Identity và Session trong `Program.cs`.
- [ ] Tạo/cập nhật các trang Auth: `/Account/Login.cshtml`, `/Account/Register.cshtml`, `/Account/Logout.cshtml` dùng `SignInManager` và `UserManager`.
- [ ] Cập nhật phân quyền Admin trong `Pages/Admin/` và Navbar `_Layout.cshtml`.

### Task 3: Chuẩn hóa Giỏ hàng & Dữ liệu sản phẩm cho ECommerceWeb
- [ ] Cập nhật Model `CartItem.cs` và `CartViewModel.cs` giống McpPage.
- [ ] Cập nhật `CartSessionExtensions.cs` để serialize/deserialize `List<CartItem>` thay cho `Dictionary<int, int>`.
- [ ] Cập nhật `Pages/Cart.cshtml` và `Cart.cshtml.cs` để thao tác trực tiếp với Cart JSON và `ApplicationDbContext`.
- [ ] Cập nhật `Pages/Index.cshtml` và `Pages/Product.cshtml` để truy vấn sản phẩm trực tiếp từ PostgreSQL DbContext thay vì mock `ProductCatalog`.
- [ ] Chạy build kiểm tra `ECommerceWeb`.

### Task 4: Chuẩn hóa EF Core & Identity cho BlazorPage
- [ ] Thêm Package `Microsoft.AspNetCore.Identity.EntityFrameworkCore` vào `BlazorPage/BlazorPage.csproj`.
- [ ] Cập nhật bộ Entity: Tạo `ApplicationUser.cs`, cập nhật quan hệ giữa `Customer` và `ApplicationUser` (thay thế entity `Account` thủ công).
- [ ] Tạo các class cấu hình `Data/Configurations/` (`CategoryConfiguration`, `ProductConfiguration`, `CustomerConfiguration`, `OrderConfiguration`, `OrderDetailConfiguration`).
- [ ] Cập nhật `AppDbContext` kế thừa `IdentityDbContext<ApplicationUser>` và dùng `ApplyConfigurationsFromAssembly`.
- [ ] Cập nhật `DbInitializer.cs` của BlazorPage để seed Identity Roles, Users với password băm an toàn qua `UserManager`.

### Task 5: Chuẩn hóa Auth Minimal API, AuthService & Blazor Admin
- [ ] Cấu hình Identity trong `BlazorPage/Program.cs`.
- [ ] Cập nhật Endpoint Minimal API `/api/auth/login` dùng `SignInManager<ApplicationUser>` / `UserManager<ApplicationUser>` để xác thực mật khẩu băm và kiểm tra Lockout.
- [ ] Cập nhật `AuthService.cs` tương thích với Identity Claims của `ApplicationUser`.
- [ ] Cập nhật `Login.razor`, `Profile.razor`, và trang quản trị `Admin/Accounts.razor` để quản lý người dùng Identity (khóa/mở khóa tài khoản, phân quyền Role) tương tự McpPage.

### Task 6: Chuẩn hóa Giỏ hàng cho BlazorPage & Nghiệm thu
- [ ] Cập nhật `CartService.cs` và `CartItem` model trong BlazorPage đồng bộ với McpPage (`ProductId`, `ProductName`, `Price`, `ImageUrl`, `Quantity`, `SubTotal`).
- [ ] Cập nhật `Components/Pages/Cart.razor`, `ProductDetail.razor` tương ứng.
- [ ] Build toàn bộ solution, kiểm tra không có lỗi biên dịch trên cả `ECommerceWeb` và `BlazorPage`.
