using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ECommerceMVCProject.Models;

public class Order
{
    public int OrderId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int ShippingAddressId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    
    [ValidateNever]
    public AppUser User { get; set; } = null!;
    [ValidateNever]
    public Address ShippingAddress { get; set; } = null!;
    [ValidateNever]
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public enum OrderStatus
{
    Pending = 0,
    Processing = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4
}
