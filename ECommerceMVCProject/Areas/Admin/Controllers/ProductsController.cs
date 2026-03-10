using ECommerceMVCProject.Models;
using ECommerceMVCProject.Repositories;
using ECommerceMVCProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMVCProject.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _unitOfWork.Products.GetProductsWithCategoryAsync();
        return View(products);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _unitOfWork.Categories.GetAllAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductEditVM vm)
    {
        if (ModelState.IsValid)
        {
            var product = new Product
            {
                Name = vm.Name,
                SKU = vm.SKU,
                Price = vm.Price,
                StockQuantity = vm.StockQuantity,
                CategoryId = vm.CategoryId,
                IsActive = vm.IsActive,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            TempData["Success"] = "Product created successfully";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = await _unitOfWork.Categories.GetAllAsync();
        return View(vm);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        var vm = new ProductEditVM
        {
            ProductId = product.ProductId,
            Name = product.Name,
            SKU = product.SKU,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            CategoryId = product.CategoryId,
            IsActive = product.IsActive
        };

        ViewBag.Categories = await _unitOfWork.Categories.GetAllAsync();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductEditVM vm)
    {
        if (id != vm.ProductId)
            return NotFound();

        if (ModelState.IsValid)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            product.Name = vm.Name;
            product.SKU = vm.SKU;
            product.Price = vm.Price;
            product.StockQuantity = vm.StockQuantity;
            product.CategoryId = vm.CategoryId;
            product.IsActive = vm.IsActive;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            TempData["Success"] = "Product updated successfully";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = await _unitOfWork.Categories.GetAllAsync();
        return View(vm);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _unitOfWork.Products.GetProductWithCategoryAsync(id);
        if (product == null)
            return NotFound();

        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        _unitOfWork.Products.Remove(product);
        await _unitOfWork.SaveChangesAsync();

        TempData["Success"] = "Product deleted successfully";
        return RedirectToAction(nameof(Index));
    }
}
