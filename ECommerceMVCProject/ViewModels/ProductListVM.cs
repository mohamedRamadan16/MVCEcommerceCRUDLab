using ECommerceMVCProject.Models;

namespace ECommerceMVCProject.ViewModels;

public class ProductListVM
{
    public IEnumerable<Product> Products { get; set; } = new List<Product>();
    public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    public int? SelectedCategoryId { get; set; }
    public string? SearchTerm { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
}
