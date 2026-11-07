using BlazorPage.Data.Entities;

namespace BlazorPage.Services;

public class CartItem
{
    private Product? _product;

    public Product Product
    {
        get => _product ??= new Product 
        { 
            Id = ProductId, 
            Name = ProductName, 
            Price = Price, 
            ImageUrl = ImageUrl 
        };
        set
        {
            _product = value;
            if (value != null)
            {
                ProductId = value.Id;
                ProductName = value.Name;
                Price = value.Price;
                ImageUrl = value.ImageUrl;
            }
        }
    }

    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public decimal SubTotal => Price * Quantity;
}

public class CartService
{
    private readonly List<CartItem> _items = new();

    public event Action? OnChange;

    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

    public int TotalItems => _items.Sum(i => i.Quantity);

    public decimal TotalAmount => _items.Sum(i => i.SubTotal);

    public void AddToCart(Product product, int quantity = 1)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == product.Id);
        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            _items.Add(new CartItem 
            { 
                Product = product,
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Quantity = quantity 
            });
        }
        NotifyStateChanged();
    }

    public void UpdateQuantity(int productId, int quantity)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing != null)
        {
            if (quantity <= 0)
            {
                _items.Remove(existing);
            }
            else
            {
                existing.Quantity = quantity;
            }
            NotifyStateChanged();
        }
    }

    public void RemoveFromCart(int productId)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing != null)
        {
            _items.Remove(existing);
            NotifyStateChanged();
        }
    }

    public void ClearCart()
    {
        _items.Clear();
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
