using BlazorPage.Data.Entities;

namespace BlazorPage.Services;

public class CartItem
{
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; } = 1;
    public decimal SubTotal => Product.Price * Quantity;
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
        var existing = _items.FirstOrDefault(i => i.Product.Id == product.Id);
        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            _items.Add(new CartItem { Product = product, Quantity = quantity });
        }
        NotifyStateChanged();
    }

    public void UpdateQuantity(int productId, int quantity)
    {
        var existing = _items.FirstOrDefault(i => i.Product.Id == productId);
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
        var existing = _items.FirstOrDefault(i => i.Product.Id == productId);
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
