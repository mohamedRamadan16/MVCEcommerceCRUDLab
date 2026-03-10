using ECommerceMVCProject.Data;
using ECommerceMVCProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceMVCProject.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Category>> GetAllWithSubCategoriesAsync()
    {
        return await _dbSet.Include(c => c.SubCategories).ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetRootCategoriesAsync()
    {
        return await _dbSet.Where(c => c.ParentCategoryId == null).ToListAsync();
    }
}
