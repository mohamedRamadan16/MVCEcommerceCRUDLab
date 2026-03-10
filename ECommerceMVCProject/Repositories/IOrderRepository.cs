using ECommerceMVCProject.Models;

namespace ECommerceMVCProject.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
    Task<Order?> GetOrderWithDetailsAsync(int orderId);
    Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync();
}
