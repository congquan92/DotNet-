using ECommerceWeb.Models;

namespace ECommerceWeb.Data;

public static class ProductCatalog
{
    public static IReadOnlyList<Product> All { get; } =
        new List<Product>
        {
            new(101, "iPhone 16 Pro Max", 30990000, "/images/products/iphone-16-pro-max.png",
                "Màn hình rộng, thiết kế titanium và camera Pro cho những khoảnh khắc đáng nhớ.",
                "Apple", "256 GB", "Titanium tự nhiên", "6,9 inch Super Retina XDR · ProMotion 120 Hz",
                "Apple A18 Pro", "48 MP Fusion + 48 MP góc siêu rộng + 12 MP telephoto 5x",
                "Xem video đến 33 giờ (theo Apple)"),

            new(102, "iPhone 16 Pro", 25990000, "/images/products/iphone-16-pro.png",
                "Hiệu năng A18 Pro và hệ thống camera Pro trong một thiết kế gọn gàng.",
                "Apple", "256 GB", "Titanium sa mạc", "6,3 inch Super Retina XDR · ProMotion 120 Hz",
                "Apple A18 Pro", "48 MP Fusion + 48 MP góc siêu rộng + 12 MP telephoto 5x",
                "Xem video đến 27 giờ (theo Apple)"),

            new(103, "iPhone 16", 19990000, "/images/products/iphone-16.png",
                "Sắc màu nổi bật, chip A18 và camera 48 MP để ghi lại từng ngày theo cách của bạn.",
                "Apple", "128 GB", "Xanh lưu ly", "6,1 inch Super Retina XDR",
                "Apple A18", "48 MP Fusion + 12 MP góc siêu rộng",
                "Xem video đến 22 giờ (theo Apple)"),

            new(104, "Samsung Galaxy S25 Ultra", 29990000, "/images/products/galaxy-s25-ultra.png",
                "Camera 200 MP, màn hình lớn và bút S Pen cho công việc lẫn sáng tạo.",
                "Samsung", "256 GB", "Titanium xanh bạc", "6,9 inch Dynamic AMOLED 2X · QHD+ · 120 Hz",
                "Snapdragon 8 Elite for Galaxy", "200 MP chính + 50 MP góc siêu rộng + 50 MP telephoto 5x + 10 MP telephoto 3x",
                "5.000 mAh (dung lượng điển hình)"),

            new(105, "Samsung Galaxy S25+", 22990000, "/images/products/galaxy-s25-plus.png",
                "Màn hình QHD+ rộng rãi và pin 4.900 mAh, sẵn sàng cho nhịp sống mỗi ngày.",
                "Samsung", "256 GB", "Xanh navy", "6,7 inch Dynamic AMOLED 2X · QHD+ · 120 Hz",
                "Snapdragon 8 Elite for Galaxy", "50 MP chính + 12 MP góc siêu rộng + 10 MP telephoto 3x",
                "4.900 mAh (dung lượng điển hình)"),

            new(106, "Samsung Galaxy S25", 18990000, "/images/products/galaxy-s25.png",
                "Nhỏ gọn, màn hình 120 Hz mượt mà và hiệu năng Snapdragon 8 Elite for Galaxy.",
                "Samsung", "256 GB", "Xanh băng", "6,2 inch Dynamic AMOLED 2X · FHD+ · 120 Hz",
                "Snapdragon 8 Elite for Galaxy", "50 MP chính + 12 MP góc siêu rộng + 10 MP telephoto 3x",
                "4.000 mAh (dung lượng điển hình)")
        };

    public static Product? Find(int id)
    {
        return All.FirstOrDefault(p => p.Id == id);
    }
}
