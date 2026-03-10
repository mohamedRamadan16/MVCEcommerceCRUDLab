using ECommerceMVCProject.Models;

namespace ECommerceMVCProject.Repositories;

public interface IAddressRepository : IRepository<Address>
{
    Task<IEnumerable<Address>> GetAddressesByUserIdAsync(string userId);
    Task<Address?> GetDefaultAddressAsync(string userId);
}
