using Microsoft.EntityFrameworkCore;
using BlazorPage.Data.Entities;

namespace BlazorPage.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Đảm bảo DB và các bảng đã được tạo
        await context.Database.EnsureCreatedAsync();

        if (await context.Categories.AnyAsync())
        {
            return; // Đã có dữ liệu, không seed lại
        }

        // 1. Seed Accounts
        var adminAccount = new Account
        {
            Username = "admin",
            Password = "123",
            Role = "Admin",
            Status = "Active"
        };

        var userAccount = new Account
        {
            Username = "user1",
            Password = "123",
            Role = "User",
            Status = "Active"
        };

        context.Accounts.AddRange(adminAccount, userAccount);
        await context.SaveChangesAsync();

        // 2. Seed Customers (Chỉ tài khoản người dùng mới có hồ sơ khách hàng)
        var customer1 = new Customer
        {
            FullName = "Nguyễn Văn An",
            Email = "an.nguyen@example.com",
            Phone = "0901234567",
            Address = "123 Lê Lợi, Q.1, TP.HCM",
            AccountId = userAccount.Id
        };

        context.Customers.Add(customer1);
        await context.SaveChangesAsync();

        // 3. Seed Categories
        var catLaptop = new Category { Name = "Laptop & Máy tính", Description = "Các dòng laptop văn phòng, đồ họa, gaming" };
        var catPhone = new Category { Name = "Điện thoại & Tablet", Description = "Smartphone cao cấp, máy tính bảng thông minh" };
        var catAccessory = new Category { Name = "Phụ kiện công nghệ", Description = "Chuột, bàn phím, tai nghe, sạc cáp" };
        var catAudio = new Category { Name = "Âm thanh & Loa", Description = "Loa bluetooth, tai nghe chống ồn" };

        context.Categories.AddRange(catLaptop, catPhone, catAccessory, catAudio);
        await context.SaveChangesAsync();

        // 4. Seed Products
        var products = new List<Product>
        {
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
                Name = "Dell XPS 13 Plus 9320",
                Description = "Màn hình tràn viền InfinityEdge 4 cạnh sắc nét, Intel Core i7 thế hệ mới, bàn phím tràn viền liền mạch và touchpad kính tàng hình.",
                Price = 38500000,
                Stock = 12,
                CategoryId = catLaptop.Id,
                ImageUrl = "https://cdn.dummyjson.com/product-images/laptops/new-dell-xps-13-9300-laptop/1.webp"
            },
            new Product
            {
                Name = "iPhone 15 Pro Max 256GB",
                Description = "Khung viền Titan chuẩn hàng không vũ trụ, chip A17 Pro mạnh mẽ với GPU 6 lõi, camera tiềm vọng 5x, màn hình Super Retina XDR 120Hz.",
                Price = 29490000,
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
                Name = "Tai nghe chống ồn Sony WH-1000XM",
                Description = "Khử tiếng ồn kỹ thuật số chuẩn phòng thu, codec LDAC truyền tải âm thanh Hi-Res Audio không dây, đệm tai bọc da êm ái kèm hộp đựng cao cấp.",
                Price = 6490000,
                Stock = 18,
                CategoryId = catAudio.Id,
                ImageUrl = "https://thumb.wikimedia.org/wikipedia/commons/thumb/4/4b/Sony-WH-1000XM3-kabellose-Bluetooth-Noise-Cancelling-Kopfhoerer.jpg/960px-Sony-WH-1000XM3-kabellose-Bluetooth-Noise-Cancelling-Kopfhoerer.jpg"
            },
            new Product
            {
                Name = "Loa Bluetooth Marshall Acton / Emberton",
                Description = "Thiết kế rock 'n' roll cổ điển biểu tượng của thương hiệu Marshall Anh Quốc, âm thanh đa hướng True Stereophonic sống động và độ bền cao.",
                Price = 6290000,
                Stock = 15,
                CategoryId = catAudio.Id,
                ImageUrl = "https://thumb.wikimedia.org/wikipedia/commons/thumb/c/ca/Marshall_Speaker.jpg/960px-Marshall_Speaker.jpg"
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
                Name = "Apple MacBook Pro 14 inch Space Grey",
                Description = "Hiệu năng chuyên nghiệp với chip Apple M-Series, màn hình Liquid Retina XDR Mini-LED 120Hz ProMotion và hệ thống âm thanh 6 loa.",
                Price = 42900000,
                Stock = 10,
                CategoryId = catLaptop.Id,
                ImageUrl = "https://cdn.dummyjson.com/product-images/laptops/apple-macbook-pro-14-inch-space-grey/thumbnail.webp"
            },
            new Product
            {
                Name = "Đồng hồ thông minh Apple Watch Ultra 2",
                Description = "Vỏ Titanium 49mm chống va đập chuẩn quân sự, màn hình Retina sáng 3000 nits ngoài trời nắng gắt, GPS tần số kép chuyên sâu cho thể thao mạo hiểm.",
                Price = 20490000,
                Stock = 14,
                CategoryId = catAccessory.Id,
                ImageUrl = "https://thumb.wikimedia.org/wikipedia/commons/thumb/3/33/Apple_Watch_Ultra_2.jpg/960px-Apple_Watch_Ultra_2.jpg"
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
            new Product
            {
                Name = "Tai nghe không dây Apple AirPods Pro (Gen 2)",
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
            }
        };

        context.Products.AddRange(products);
        await context.SaveChangesAsync();

        // 5. Seed sample Order
        var sampleOrder = new Order
        {
            CustomerId = customer1.Id,
            OrderDate = DateTime.UtcNow.AddDays(-2),
            ShippingAddress = customer1.Address,
            Status = "Processing",
            TotalAmount = products[4].Price + products[5].Price,
            OrderDetails = new List<OrderDetail>
            {
                new OrderDetail
                {
                    ProductId = products[4].Id,
                    Quantity = 1,
                    UnitPrice = products[4].Price
                },
                new OrderDetail
                {
                    ProductId = products[5].Id,
                    Quantity = 1,
                    UnitPrice = products[5].Price
                }
            }
        };

        context.Orders.Add(sampleOrder);
        await context.SaveChangesAsync();
    }
}
