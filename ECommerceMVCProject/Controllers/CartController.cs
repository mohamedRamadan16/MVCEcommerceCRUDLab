using ECommerceMVCProject.Repositories;
using ECommerceMVCProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMVCProject.Controllers;

public class CartController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICartService _cartService;

    public CartController(IUnitOfWork unitOfWork, ICartService cartService)
    {
        _unitOfWork = unitOfWork;
        _cartService = cartService;
    }

    public IActionResult Index()
    {
        var cart = _cartService.GetCart();
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int productId)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId);
        
        if (product == null || !product.IsActive || product.StockQuantity <= 0)
            return NotFound();

        _cartService.AddToCart(product.ProductId, product.Name, product.Price, product.StockQuantity);
        
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Update(int productId, int quantity)
    {
        _cartService.UpdateQuantity(productId, quantity);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Remove(int productId)
    {
        _cartService.RemoveFromCart(productId);
        return RedirectToAction(nameof(Index));
    }
}
