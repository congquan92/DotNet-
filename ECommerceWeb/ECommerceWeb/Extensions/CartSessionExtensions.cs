namespace ECommerceWeb.Extensions;

using System.Text.Json;
using Microsoft.AspNetCore.Http;

public static class CartSessionExtensions
{
    private const string CartKey = "Cart";

    public static Dictionary<int, int> GetCart(this ISession session)
    {
        var json = session.GetString(CartKey);

        if (string.IsNullOrEmpty(json))
            return new Dictionary<int, int>();

        return JsonSerializer.Deserialize<
            Dictionary<int, int>>(json) ?? new();
    }

    public static void SetCart(this ISession session, Dictionary<int, int> cart)
    {
        string json = JsonSerializer.Serialize(cart);
        session.SetString(CartKey, json);
    }
}