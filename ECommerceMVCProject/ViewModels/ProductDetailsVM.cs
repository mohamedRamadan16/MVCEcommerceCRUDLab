using ECommerceMVCProject.Models;

namespace ECommerceMVCProject.ViewModels;

public class ProductDetailsVM
{
    public Product Product { get; set; } = null!;
    public bool InStock => Product.StockQuantity > 0;
}
