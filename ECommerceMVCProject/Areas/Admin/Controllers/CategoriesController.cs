using ECommerceMVCProject.Models;
using ECommerceMVCProject.Repositories;
using ECommerceMVCProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMVCProject.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoriesController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoriesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        return View(categories);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _unitOfWork.Categories.GetAllAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryEditVM vm)
    {
        if (ModelState.IsValid)
        {
            var category = new Category
            {
                Name = vm.Name,
                ParentCategoryId = vm.ParentCategoryId
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
            
            TempData["Success"] = "Category created successfully";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = await _unitOfWork.Categories.GetAllAsync();
        return View(vm);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        var vm = new CategoryEditVM
        {
            CategoryId = category.CategoryId,
            Name = category.Name,
            ParentCategoryId = category.ParentCategoryId
        };

        ViewBag.Categories = (await _unitOfWork.Categories.GetAllAsync())
            .Where(c => c.CategoryId != id).ToList();
        
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryEditVM vm)
    {
        if (id != vm.CategoryId)
            return NotFound();

        if (ModelState.IsValid)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            category.Name = vm.Name;
            category.ParentCategoryId = vm.ParentCategoryId;

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();

            TempData["Success"] = "Category updated successfully";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = (await _unitOfWork.Categories.GetAllAsync())
            .Where(c => c.CategoryId != id).ToList();
        
        return View(vm);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        _unitOfWork.Categories.Remove(category);
        await _unitOfWork.SaveChangesAsync();

        TempData["Success"] = "Category deleted successfully";
        return RedirectToAction(nameof(Index));
    }
}
