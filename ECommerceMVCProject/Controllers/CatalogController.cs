using ECommerceMVCProject.Repositories;
using ECommerceMVCProject.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMVCProject.Controllers;

public class CatalogController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private const int PageSize = 12;

    public CatalogController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index(int? categoryId, string? q, int page = 1)
    {
        var (products, totalCount) = await _unitOfWork.Products
            .GetPagedProductsAsync(categoryId, q, page, PageSize);
        
        var categories = await _unitOfWork.Categories.GetAllAsync();

        var vm = new ProductListVM
        {
            Products = products,
            Categories = categories,
            SelectedCategoryId = categoryId,
            SearchTerm = q,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize),
            PageSize = PageSize
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _unitOfWork.Products.GetProductWithCategoryAsync(id);
        
        if (product == null)
            return NotFound();

        var vm = new ProductDetailsVM
        {
            Product = product
        };

        return View(vm);
    }
}
