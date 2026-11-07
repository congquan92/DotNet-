using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ECommerceWeb.Entities;

namespace ECommerceWeb.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Đảm bảo database đã được tạo
        await context.Database.EnsureCreatedAsync();

        // 1. Seed Roles
        string[] roles = ["Admin", "Manager", "Staff", "User"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2. Seed Admin User
        var adminUser = await userManager.FindByNameAsync("admin");
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@ecommerce.com",
                FullName = "Quản Trị Viên Hệ Thống",
                PhoneNumber = "0900000001",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");

                var adminCustomer = new Customer
                {
                    FullName = adminUser.FullName,
                    Email = adminUser.Email,
                    Phone = adminUser.PhoneNumber,
                    Address = "Hà Nội, Việt Nam",
                    UserId = adminUser.Id
                };
                context.Customers.Add(adminCustomer);
                await context.SaveChangesAsync();
            }
        }

        // 3. Seed Demo Customer User
        var demoUser = await userManager.FindByNameAsync("user");
        if (demoUser == null)
        {
            demoUser = new ApplicationUser
            {
                UserName = "user",
                Email = "user@ecommerce.com",
                FullName = "Nguyễn Văn A",
                PhoneNumber = "0901234567",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(demoUser, "User@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(demoUser, "User");

                var customer = new Customer
                {
                    FullName = demoUser.FullName,
                    Email = demoUser.Email,
                    Phone = demoUser.PhoneNumber,
                    Address = "123 Đường Cầu Giấy, Hà Nội",
                    UserId = demoUser.Id
                };
                context.Customers.Add(customer);
                await context.SaveChangesAsync();
            }
        }

        var demoUser1 = await userManager.FindByNameAsync("user1");
        if (demoUser1 == null)
        {
            demoUser1 = new ApplicationUser
            {
                UserName = "user1",
                Email = "user1@ecommerce.com",
                FullName = "Trần Thị B",
                PhoneNumber = "0902345678",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(demoUser1, "User@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(demoUser1, "User");

                var customer = new Customer
                {
                    FullName = demoUser1.FullName,
                    Email = demoUser1.Email,
                    Phone = demoUser1.PhoneNumber,
                    Address = "456 Đường Nguyễn Huệ, TP.HCM",
                    UserId = demoUser1.Id
                };
                context.Customers.Add(customer);
                await context.SaveChangesAsync();
            }
        }

        // 4. Seed Categories
        if (!await context.Categories.AnyAsync())
        {
            var appleCategory = new Category
            {
                Name = "Apple",
                Description = "Các thiết bị iPhone và hệ sinh thái Apple đỉnh cao."
            };
            var samsungCategory = new Category
            {
                Name = "Samsung",
                Description = "Các thiết bị Samsung Galaxy với công nghệ đột phá."
            };

            context.Categories.AddRange(appleCategory, samsungCategory);
            await context.SaveChangesAsync();

            // 5. Seed Products
            var products = new List<Product>
            {
                new()
                {
                    Name = "iPhone 16 Pro Max",
                    Price = 30990000,
                    OriginalPrice = 34990000,
                    ImageUrl = "/images/products/iphone-16-pro-max.png",
                    Description = "Màn hình rộng, thiết kế titanium và camera Pro cho những khoảnh khắc đáng nhớ.",
                    Brand = "Apple",
                    Storage = "256 GB",
                    Color = "Titanium tự nhiên",
                    Display = "6,9 inch Super Retina XDR · ProMotion 120 Hz",
                    Chip = "Apple A18 Pro",
                    Camera = "48 MP Fusion + 48 MP góc siêu rộng + 12 MP telephoto 5x",
                    Battery = "Xem video đến 33 giờ (theo Apple)",
                    Stock = 50,
                    CategoryId = appleCategory.Id
                },
                new()
                {
                    Name = "iPhone 16 Pro",
                    Price = 25990000,
                    OriginalPrice = 28990000,
                    ImageUrl = "/images/products/iphone-16-pro.png",
                    Description = "Hiệu năng A18 Pro và hệ thống camera Pro trong một thiết kế gọn gàng.",
                    Brand = "Apple",
                    Storage = "256 GB",
                    Color = "Titanium sa mạc",
                    Display = "6,3 inch Super Retina XDR · ProMotion 120 Hz",
                    Chip = "Apple A18 Pro",
                    Camera = "48 MP Fusion + 48 MP góc siêu rộng + 12 MP telephoto 5x",
                    Battery = "Xem video đến 27 giờ (theo Apple)",
                    Stock = 60,
                    CategoryId = appleCategory.Id
                },
                new()
                {
                    Name = "iPhone 16",
                    Price = 19990000,
                    OriginalPrice = 22990000,
                    ImageUrl = "/images/products/iphone-16.png",
                    Description = "Sắc màu nổi bật, chip A18 và camera 48 MP để ghi lại từng ngày theo cách của bạn.",
                    Brand = "Apple",
                    Storage = "128 GB",
                    Color = "Xanh lưu ly",
                    Display = "6,1 inch Super Retina XDR",
                    Chip = "Apple A18",
                    Camera = "48 MP Fusion + 12 MP góc siêu rộng",
                    Battery = "Xem video đến 22 giờ (theo Apple)",
                    Stock = 80,
                    CategoryId = appleCategory.Id
                },
                new()
                {
                    Name = "Samsung Galaxy S25 Ultra",
                    Price = 29990000,
                    OriginalPrice = 33990000,
                    ImageUrl = "/images/products/galaxy-s25-ultra.png",
                    Description = "Camera 200 MP, màn hình lớn và bút S Pen cho công việc lẫn sáng tạo.",
                    Brand = "Samsung",
                    Storage = "256 GB",
                    Color = "Titanium xanh bạc",
                    Display = "6,9 inch Dynamic AMOLED 2X · QHD+ · 120 Hz",
                    Chip = "Snapdragon 8 Elite for Galaxy",
                    Camera = "200 MP chính + 50 MP góc siêu rộng + 50 MP telephoto 5x + 10 MP telephoto 3x",
                    Battery = "5.000 mAh (dung lượng điển hình)",
                    Stock = 45,
                    CategoryId = samsungCategory.Id
                },
                new()
                {
                    Name = "Samsung Galaxy S25+",
                    Price = 22990000,
                    OriginalPrice = 25990000,
                    ImageUrl = "/images/products/galaxy-s25-plus.png",
                    Description = "Màn hình QHD+ rộng rãi và pin 4.900 mAh, sẵn sàng cho nhịp sống mỗi ngày.",
                    Brand = "Samsung",
                    Storage = "256 GB",
                    Color = "Xanh navy",
                    Display = "6,7 inch Dynamic AMOLED 2X · QHD+ · 120 Hz",
                    Chip = "Snapdragon 8 Elite for Galaxy",
                    Camera = "50 MP chính + 12 MP góc siêu rộng + 10 MP telephoto 3x",
                    Battery = "4.900 mAh (dung lượng điển hình)",
                    Stock = 55,
                    CategoryId = samsungCategory.Id
                },
                new()
                {
                    Name = "Samsung Galaxy S25",
                    Price = 18990000,
                    OriginalPrice = 21990000,
                    ImageUrl = "/images/products/galaxy-s25.png",
                    Description = "Nhỏ gọn, màn hình 120 Hz mượt mà và hiệu năng Snapdragon 8 Elite for Galaxy.",
                    Brand = "Samsung",
                    Storage = "256 GB",
                    Color = "Xanh băng",
                    Display = "6,2 inch Dynamic AMOLED 2X · FHD+ · 120 Hz",
                    Chip = "Snapdragon 8 Elite for Galaxy",
                    Camera = "50 MP chính + 12 MP góc siêu rộng + 10 MP telephoto 3x",
                    Battery = "4.000 mAh (dung lượng điển hình)",
                    Stock = 70,
                    CategoryId = samsungCategory.Id
                }
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
    }
}
