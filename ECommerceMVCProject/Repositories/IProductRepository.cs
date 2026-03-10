using ECommerceMVCProject.Models;

namespace ECommerceMVCProject.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsWithCategoryAsync();
    Task<Product?> GetProductWithCategoryAsync(int id);
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
    Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedProductsAsync(int? categoryId, string? searchTerm, int page, int pageSize);
}
