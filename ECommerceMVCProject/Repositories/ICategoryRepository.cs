using ECommerceMVCProject.Models;

namespace ECommerceMVCProject.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>> GetAllWithSubCategoriesAsync();
    Task<IEnumerable<Category>> GetRootCategoriesAsync();
}
