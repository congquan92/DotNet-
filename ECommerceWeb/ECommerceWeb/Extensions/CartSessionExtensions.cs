namespace ECommerceWeb.Extensions;

using System.Text.Json;
using Microsoft.AspNetCore.Http;
using ECommerceWeb.Models;

public static class CartSessionExtensions
{
    private const string CartKey = "CartSession";

    public static List<CartItem> GetCart(this ISession session)
    {
        var json = session.GetString(CartKey);

        if (string.IsNullOrEmpty(json))
            return new List<CartItem>();

        try
        {
            return JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
        }
        catch
        {
            return new List<CartItem>();
        }
    }

    public static void SetCart(this ISession session, List<CartItem> cart)
    {
        string json = JsonSerializer.Serialize(cart);
        session.SetString(CartKey, json);
    }
}
