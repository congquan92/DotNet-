namespace ECommerceWeb.Models;

public record Product(
    int Id,
    string Name,
    decimal Price,
    string ImageUrl,
    string Description,
    string Brand,
    string Storage,
    string Color,
    string Display,
    string Chip,
    string Camera,
    string Battery
);
