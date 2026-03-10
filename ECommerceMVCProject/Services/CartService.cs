using ECommerceMVCProject.ViewModels;
using System.Text.Json;

namespace ECommerceMVCProject.Services;

public interface ICartService
{
    CartVM GetCart();
    void AddToCart(int productId, string productName, decimal price, int stockQuantity);
    void UpdateQuantity(int productId, int quantity);
    void RemoveFromCart(int productId);
    void ClearCart();
}

public class SessionCartService : ICartService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CartSessionKey = "ShoppingCart";

    public SessionCartService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ISession Session => _httpContextAccessor.HttpContext!.Session;

    public CartVM GetCart()
    {
        var cartJson = Session.GetString(CartSessionKey);
        return string.IsNullOrEmpty(cartJson) 
            ? new CartVM() 
            : JsonSerializer.Deserialize<CartVM>(cartJson) ?? new CartVM();
    }

    private void SaveCart(CartVM cart)
    {
        var cartJson = JsonSerializer.Serialize(cart);
        Session.SetString(CartSessionKey, cartJson);
    }

    public void AddToCart(int productId, string productName, decimal price, int stockQuantity)
    {
        var cart = GetCart();
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            cart.Items.Add(new CartItemVM
            {
                ProductId = productId,
                ProductName = productName,
                UnitPrice = price,
                Quantity = 1,
                StockQuantity = stockQuantity
            });
        }

        SaveCart(cart);
    }

    public void UpdateQuantity(int productId, int quantity)
    {
        var cart = GetCart();
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (item != null)
        {
            if (quantity <= 0)
            {
                cart.Items.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }
        }

        SaveCart(cart);
    }

    public void RemoveFromCart(int productId)
    {
        var cart = GetCart();
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (item != null)
        {
            cart.Items.Remove(item);
        }

        SaveCart(cart);
    }

    public void ClearCart()
    {
        Session.Remove(CartSessionKey);
    }
}
