using ECommerceMVCProject.Models;
using ECommerceMVCProject.Repositories;
using ECommerceMVCProject.Services;
using ECommerceMVCProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMVCProject.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICartService _cartService;
    private readonly UserManager<AppUser> _userManager;

    public OrdersController(IUnitOfWork unitOfWork, ICartService cartService, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _cartService = cartService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var orders = await _unitOfWork.Orders.GetOrdersByUserIdAsync(userId);
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(id);
        
        if (order == null)
            return NotFound();

        var userId = _userManager.GetUserId(User);
        if (order.UserId != userId && !User.IsInRole("Admin"))
            return Forbid();

        var vm = new OrderDetailsVM { Order = order };
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var cart = _cartService.GetCart();
        
        if (!cart.Items.Any())
            return RedirectToAction("Index", "Cart");

        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var addresses = await _unitOfWork.Addresses.GetAddressesByUserIdAsync(userId);
        var defaultAddress = addresses.FirstOrDefault(a => a.IsDefault);

        var vm = new CheckoutVM
        {
            Cart = cart,
            Addresses = addresses.ToList(),
            SelectedAddressId = defaultAddress?.AddressId ?? 0
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(int selectedAddressId)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var cart = _cartService.GetCart();
        if (!cart.Items.Any())
            return RedirectToAction("Index", "Cart");

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            foreach (var item in cart.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product == null || product.StockQuantity < item.Quantity)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    TempData["Error"] = $"Insufficient stock for {item.ProductName}";
                    return RedirectToAction(nameof(Checkout));
                }
            }

            var order = new Order
            {
                UserId = userId,
                ShippingAddressId = selectedAddressId,
                OrderNumber = $"ORD-{DateTime.Now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}",
                Status = OrderStatus.Pending,
                OrderDate = DateTime.Now,
                TotalAmount = cart.TotalAmount
            };

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            foreach (var item in cart.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity,
                        LineTotal = item.LineTotal
                    };

                    await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.OrderId == order.OrderId);
                    order.OrderItems.Add(orderItem);

                    product.StockQuantity -= item.Quantity;
                    _unitOfWork.Products.Update(product);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            _cartService.ClearCart();

            TempData["Success"] = "Order placed successfully!";
            return RedirectToAction(nameof(Details), new { id = order.OrderId });
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            TempData["Error"] = "Failed to process order. Please try again.";
            return RedirectToAction(nameof(Checkout));
        }
    }
}
