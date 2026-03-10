using ECommerceMVCProject.Data;
using ECommerceMVCProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceMVCProject.Repositories;

public class AddressRepository : Repository<Address>, IAddressRepository
{
    public AddressRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Address>> GetAddressesByUserIdAsync(string userId)
    {
        return await _dbSet.Where(a => a.UserId == userId).ToListAsync();
    }

    public async Task<Address?> GetDefaultAddressAsync(string userId)
    {
        return await _dbSet.FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault);
    }
}
