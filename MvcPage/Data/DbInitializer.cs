using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using McpPage.Data.Entities;

namespace McpPage.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Đảm bảo database và các bảng đã được khởi tạo
        await context.Database.EnsureCreatedAsync();

        // ==========================================
        // 1. SEED ROLES
        // ==========================================
        string[] roles = ["Admin", "Manager", "Staff", "User"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // ==========================================
        // 2. SEED ADMIN & ACCOUNTS THEO CÁC ROLE
        // ==========================================

        // Tài khoản Admin
        var adminUser = await userManager.FindByNameAsync("admin");
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@mcp.com",
                FullName = "Hệ Thống Quản Trị Viên",
                PhoneNumber = "0900000001",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Tài khoản Manager
        var managerUser = await userManager.FindByNameAsync("manager");
        if (managerUser == null)
        {
            managerUser = new ApplicationUser
            {
                UserName = "manager",
                Email = "manager@mcp.com",
                FullName = "Quản Lý Cửa Hàng",
                PhoneNumber = "0900000002",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(managerUser, "Manager@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(managerUser, "Manager");
            }
        }

        // Tài khoản Staff
        var staffUser = await userManager.FindByNameAsync("staff");
        if (staffUser == null)
        {
            staffUser = new ApplicationUser
            {
                UserName = "staff",
                Email = "staff@mcp.com",
                FullName = "Nhân Viên Bán Hàng",
                PhoneNumber = "0900000003",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(staffUser, "Staff@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(staffUser, "Staff");
            }
        }

        // Tài khoản Khách hàng User 1
        var user1 = await userManager.FindByNameAsync("user1");
        if (user1 == null)
        {
            user1 = new ApplicationUser
            {
                UserName = "user1",
                Email = "an.nguyen@example.com",
                FullName = "Nguyễn Văn An",
                PhoneNumber = "0901234567",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user1, "User@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user1, "User");
            }
        }

        // Tài khoản Khách hàng User 2
        var user2 = await userManager.FindByNameAsync("user2");
        if (user2 == null)
        {
            user2 = new ApplicationUser
            {
                UserName = "user2",
                Email = "binh.tran@example.com",
                FullName = "Trần Thị Bình",
                PhoneNumber = "0987654321",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user2, "User@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user2, "User");
            }
        }

        // ==========================================
        // 3. SEED CUSTOMERS (Gắn liền với Identity User)
        // ==========================================
        Customer? customer1 = await context.Customers.FirstOrDefaultAsync(c => c.UserId == user1.Id);
        if (customer1 == null && user1 != null)
        {
            customer1 = new Customer
            {
                FullName = user1.FullName ?? "Nguyễn Văn An",
                Email = user1.Email ?? "an.nguyen@example.com",
                Phone = user1.PhoneNumber ?? "0901234567",
                Address = "123 Lê Lợi, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh",
                UserId = user1.Id
            };
            context.Customers.Add(customer1);
            await context.SaveChangesAsync();
        }

        Customer? customer2 = await context.Customers.FirstOrDefaultAsync(c => c.UserId == user2.Id);
        if (customer2 == null && user2 != null)
        {
            customer2 = new Customer
            {
                FullName = user2.FullName ?? "Trần Thị Bình",
                Email = user2.Email ?? "binh.tran@example.com",
                Phone = user2.PhoneNumber ?? "0987654321",
                Address = "456 Nguyễn Huệ, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh",
                UserId = user2.Id
            };
            context.Customers.Add(customer2);
            await context.SaveChangesAsync();
        }

        // Kiểm tra dữ liệu sản phẩm đã có chưa, nếu có thì bỏ qua phần seed sản phẩm
        if (await context.Categories.AnyAsync())
        {
            return;
        }

        // ==========================================
        // 4. SEED CATEGORIES (DANH MỤC SẢN PHẨM)
        // ==========================================
        var catLaptop = new Category 
        { 
            Name = "Laptop & Máy tính", 
            Description = "Các dòng laptop văn phòng cao cấp, đồ họa chuyên nghiệp và gaming mạnh mẽ" 
        };
        var catPhone = new Category 
        { 
            Name = "Điện thoại & Tablet", 
            Description = "Smartphone cao cấp, máy tính bảng thông minh thế hệ mới nhất" 
        };
        var catAccessory = new Category 
        { 
            Name = "Phụ kiện công nghệ", 
            Description = "Bàn phím cơ, chuột công thái học, củ sạc nhanh và phụ kiện cao cấp" 
        };
        var catAudio = new Category 
        { 
            Name = "Âm thanh & Loa", 
            Description = "Tai nghe chống ồn chủ động, loa bluetooth Hi-Res chuẩn phòng thu" 
        };
        var catSmartwatch = new Category 
        { 
            Name = "Thiết bị đeo thông minh", 
            Description = "Đồng hồ thông minh theo dõi sức khỏe và thể thao chuyên nghiệp" 
        };

        context.Categories.AddRange(catLaptop, catPhone, catAccessory, catAudio, catSmartwatch);
        await context.SaveChangesAsync();

        // ==========================================
        // 5. SEED PRODUCTS (20+ SẢN PHẨM CÔNG NGHỆ CHẤT LƯỢNG CAO)
        // ==========================================
        var products = new List<Product>
        {
            // --- Nhóm Laptop & Máy tính ---
            new Product
            {
                Name = "MacBook Air M2 13.6 inch",
                Description = "Thiết kế vuông vức siêu mỏng 11.3mm màu Starlight tinh tế, màn hình Liquid Retina tai thỏ sắc nét, chip Apple M2 vượt trội.",
                Price = 24990000,
                Stock = 25,
                CategoryId = catLaptop.Id,
                ImageUrl = "https://thumb.wikimedia.org/wikipedia/commons/thumb/9/9f/M2_Macbook_Air_Starlight_model.jpg/960px-M2_Macbook_Air_Starlight_model.jpg"
            },
            new Product
            {
                Name = "MacBook Pro 14 inch M3 Pro",
                Description = "Hiệu năng chuyên nghiệp với chip Apple M3 Pro, màn hình Liquid Retina XDR Mini-LED 120Hz ProMotion và hệ thống âm thanh 6 loa đỉnh cao.",
                Price = 42900000,
                Stock = 10,
                CategoryId = catLaptop.Id,
                ImageUrl = "https://cdn.dummyjson.com/product-images/laptops/apple-macbook-pro-14-inch-space-grey/thumbnail.webp"
            },
            new Product
            {
                Name = "Dell XPS 13 Plus 9320",
                Description = "Màn hình tràn viền InfinityEdge 4 cạnh sắc nét, Intel Core i7 thế hệ mới, bàn phím tràn viền liền mạch và touchpad kính tàng hình.",
                Price = 38500000,
                Stock = 12,
                CategoryId = catLaptop.Id,
                ImageUrl = "https://cdn.dummyjson.com/product-images/laptops/new-dell-xps-13-9300-laptop/1.webp"
            },
            new Product
            {
                Name = "ASUS ROG Zephyrus G14 Gaming",
                Description = "Laptop gaming mỏng nhẹ cao cấp, màn hình ROG Nebula OLED 120Hz, vi xử lý AMD Ryzen 9 kèm card đồ họa RTX 4060 mạnh mẽ.",
                Price = 36990000,
                Stock = 15,
                CategoryId = catLaptop.Id,
                ImageUrl = "https://cdn.dummyjson.com/product-images/laptops/asus-zenbook-pro-duo/1.webp"
            },

            // --- Nhóm Điện thoại & Tablet ---
            new Product
            {
                Name = "iPhone 16 Pro Max 256GB",
                Description = "Khung viền Titan sa mạc đẳng cấp, chip A18 Pro hỗ trợ Apple Intelligence, nút Camera Control thế hệ mới và zoom quang học 5x.",
                Price = 32990000,
                Stock = 35,
                CategoryId = catPhone.Id,
                ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/ae/IPhone_15_pro.png/960px-IPhone_15_pro.png"
            },
            new Product
            {
                Name = "iPhone 15 Pro Max 256GB",
                Description = "Khung viền Titan chuẩn hàng không vũ trụ, chip A17 Pro mạnh mẽ với GPU 6 lõi, camera tiềm vọng 5x, màn hình Super Retina XDR 120Hz.",
                Price = 28490000,
                Stock = 30,
                CategoryId = catPhone.Id,
                ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/ae/IPhone_15_pro.png/960px-IPhone_15_pro.png"
            },
            new Product
            {
                Name = "Samsung Galaxy S24 Ultra 5G",
                Description = "Khung viền Titanium cao cấp, màn hình phẳng Dynamic AMOLED 2X 6.8 inch, tích hợp bút S-Pen thông minh và quyền năng Galaxy AI.",
                Price = 27990000,
                Stock = 20,
                CategoryId = catPhone.Id,
                ImageUrl = "https://thumb.wikimedia.org/wikipedia/commons/thumb/4/44/SAMSUNG_Galaxy_S24_Ultra_%286%29.jpg/960px-SAMSUNG_Galaxy_S24_Ultra_%286%29.jpg"
            },
            new Product
            {
                Name = "Samsung Galaxy Tab S8 Plus 5G",
                Description = "Màn hình Super AMOLED 12.4 inch 120Hz siêu nét, Snapdragon 8 Gen 1 đỉnh cao, kèm bút S-Pen độ trễ 2.8ms cho làm việc sáng tạo.",
                Price = 15990000,
                Stock = 15,
                CategoryId = catPhone.Id,
                ImageUrl = "https://cdn.dummyjson.com/product-images/tablets/samsung-galaxy-tab-s8-plus-grey/thumbnail.webp"
            },
            new Product
            {
                Name = "iPad Pro M4 11 inch Wi-Fi 256GB",
                Description = "Độ mỏng kỷ lục 5.3mm, màn hình Ultra Retina XDR công nghệ Tandem OLED đột phá, sức mạnh không tưởng từ vi xử lý Apple M4.",
                Price = 26990000,
                Stock = 18,
                CategoryId = catPhone.Id,
                ImageUrl = "https://cdn.dummyjson.com/product-images/tablets/ipad-mini-2021-starlight/thumbnail.webp"
            },

            // --- Nhóm Phụ kiện công nghệ ---
            new Product
            {
                Name = "Bàn phím không dây Apple Magic Keyboard",
                Description = "Bàn phím không dây chính hãng Apple với cơ chế cắt kéo tối ưu, hành trình phím mượt mà, kết nối Bluetooth tự động và pin sạc tiện lợi.",
                Price = 2490000,
                Stock = 50,
                CategoryId = catAccessory.Id,
                ImageUrl = "https://thumb.wikimedia.org/wikipedia/commons/thumb/c/c0/Apple_Magic_Keyboard_-_US_remix_transparent.png/960px-Apple_Magic_Keyboard_-_US_remix_transparent.png"
            },
            new Product
            {
                Name = "Chuột không dây Logitech MX Master 3S",
                Description = "Cảm biến Darkfield 8000 DPI lướt trên mọi bề mặt kể cả mặt kính, cuộn MagSpeed siêu nhanh êm ái, click chuột tĩnh lặng 90%.",
                Price = 2290000,
                Stock = 45,
                CategoryId = catAccessory.Id,
                ImageUrl = "https://thumb.wikimedia.org/wikipedia/commons/thumb/2/25/Logitech_MX_Master_3S_HS03.jpg/960px-Logitech_MX_Master_3S_HS03.jpg"
            },
            new Product
            {
                Name = "Củ sạc nhanh Anker 735 GaNPrime 65W",
                Description = "Công nghệ GaNPrime tiên tiến, 3 cổng sạc đồng thời (2 USB-C + 1 USB-A), tự động phân bổ dòng điện an toàn thông minh PowerIQ 4.0.",
                Price = 1190000,
                Stock = 60,
                CategoryId = catAccessory.Id,
                ImageUrl = "https://cdn.dummyjson.com/product-images/mobile-accessories/apple-airpods-max-silver/thumbnail.webp"
            },
            new Product
            {
                Name = "Máy chơi game Nintendo Switch OLED Model",
                Description = "Màn hình OLED 7 inch rực rỡ, kèm đế dock tích hợp cổng LAN có dây, chân đế bản rộng đa góc độ cùng cặp tay cầm Joy-Con Neon độc đáo.",
                Price = 7490000,
                Stock = 20,
                CategoryId = catAccessory.Id,
                ImageUrl = "https://thumb.wikimedia.org/wikipedia/commons/thumb/7/76/Nintendo-Switch-Console-Docked-wJoyConRB.jpg/960px-Nintendo-Switch-Console-Docked-wJoyConRB.jpg"
            },

            // --- Nhóm Âm thanh & Loa ---
            new Product
            {
                Name = "Tai nghe chống ồn Sony WH-1000XM5",
                Description = "Khử tiếng ồn kỹ thuật số chuẩn phòng thu thế hệ mới với 8 micro, công nghệ Auto NC Optimizer, codec LDAC và thời lượng pin 30 giờ.",
                Price = 7490000,
                Stock = 22,
                CategoryId = catAudio.Id,
                ImageUrl = "https://thumb.wikimedia.org/wikipedia/commons/thumb/4/4b/Sony-WH-1000XM3-kabellose-Bluetooth-Noise-Cancelling-Kopfhoerer.jpg/960px-Sony-WH-1000XM3-kabellose-Bluetooth-Noise-Cancelling-Kopfhoerer.jpg"
            },
            new Product
            {
                Name = "Loa Bluetooth Marshall Acton III",
                Description = "Thiết kế rock 'n' roll cổ điển biểu tượng của thương hiệu Marshall Anh Quốc, âm trường rộng hơn với củ loa tweeter hướng ra ngoài.",
                Price = 6290000,
                Stock = 15,
                CategoryId = catAudio.Id,
                ImageUrl = "https://thumb.wikimedia.org/wikipedia/commons/thumb/c/ca/Marshall_Speaker.jpg/960px-Marshall_Speaker.jpg"
            },
            new Product
            {
                Name = "Tai nghe không dây Apple AirPods Pro (Gen 2) USB-C",
                Description = "Trang bị chip Apple H2 khử tiếng ồn chủ động mạnh gấp đôi, tính năng Thích Ứng Âm Thanh (Adaptive Audio) và hộp sạc MagSafe cổng USB-C.",
                Price = 5490000,
                Stock = 40,
                CategoryId = catAudio.Id,
                ImageUrl = "https://thumb.wikimedia.org/wikipedia/commons/thumb/2/2f/AirPods_Pro_%282nd_generation%29.jpg/960px-AirPods_Pro_%282nd_generation%29.jpg"
            },
            new Product
            {
                Name = "Tai nghe chụp tai Apple AirPods Max Silver",
                Description = "Vòm chụp bằng lưới thoáng khí phân tán áp lực, chụp tai bằng nhôm anodized sang trọng, hệ thống xử lý âm thanh điện toán Computational Audio đỉnh cao.",
                Price = 12990000,
                Stock = 8,
                CategoryId = catAudio.Id,
                ImageUrl = "https://cdn.dummyjson.com/product-images/mobile-accessories/apple-airpods-max-silver/thumbnail.webp"
            },

            // --- Nhóm Thiết bị đeo thông minh ---
            new Product
            {
                Name = "Apple Watch Ultra 2 GPS + Cellular 49mm",
                Description = "Vỏ Titanium 49mm chống va đập chuẩn quân sự, màn hình Retina sáng 3000 nits ngoài trời nắng gắt, GPS tần số kép chuyên sâu cho thể thao mạo hiểm.",
                Price = 20490000,
                Stock = 14,
                CategoryId = catSmartwatch.Id,
                ImageUrl = "https://thumb.wikimedia.org/wikipedia/commons/thumb/3/33/Apple_Watch_Ultra_2.jpg/960px-Apple_Watch_Ultra_2.jpg"
            },
            new Product
            {
                Name = "Samsung Galaxy Watch 6 Classic 47mm",
                Description = "Thiết kế vòng xoay bezel vật lý trứ danh, mặt kính Sapphire chống trầy xước, phân tích thành phần cơ thể BIA và theo dõi giấc ngủ chuyên sâu.",
                Price = 6990000,
                Stock = 16,
                CategoryId = catSmartwatch.Id,
                ImageUrl = "https://cdn.dummyjson.com/product-images/mens-watches/rolex-cellini-date/thumbnail.webp"
            }
        };

        context.Products.AddRange(products);
        await context.SaveChangesAsync();

        // ==========================================
        // 6. SEED ORDERS & ORDER DETAILS MẪU
        // ==========================================
        if (customer1 != null)
        {
            var sampleOrder1 = new Order
            {
                CustomerId = customer1.Id,
                OrderDate = DateTime.UtcNow.AddDays(-2),
                ShippingAddress = customer1.Address,
                Status = "Processing",
                TotalAmount = products[0].Price + products[10].Price,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        ProductId = products[0].Id,
                        Quantity = 1,
                        UnitPrice = products[0].Price
                    },
                    new OrderDetail
                    {
                        ProductId = products[10].Id,
                        Quantity = 1,
                        UnitPrice = products[10].Price
                    }
                }
            };
            context.Orders.Add(sampleOrder1);
        }

        if (customer2 != null)
        {
            var sampleOrder2 = new Order
            {
                CustomerId = customer2.Id,
                OrderDate = DateTime.UtcNow.AddDays(-5),
                ShippingAddress = customer2.Address,
                Status = "Completed",
                TotalAmount = products[4].Price,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        ProductId = products[4].Id,
                        Quantity = 1,
                        UnitPrice = products[4].Price
                    }
                }
            };
            context.Orders.Add(sampleOrder2);
        }

        await context.SaveChangesAsync();
    }
}
