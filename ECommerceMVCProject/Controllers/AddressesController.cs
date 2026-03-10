using ECommerceMVCProject.Models;
using ECommerceMVCProject.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMVCProject.Controllers;

[Authorize]
public class AddressesController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public AddressesController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var addresses = await _unitOfWork.Addresses.GetAddressesByUserIdAsync(userId);
        return View(addresses);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Address address)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        if (ModelState.IsValid)
        {
            address.UserId = userId;
            
            if (address.IsDefault)
            {
                var existingAddresses = await _unitOfWork.Addresses.GetAddressesByUserIdAsync(userId);
                foreach (var addr in existingAddresses)
                {
                    addr.IsDefault = false;
                    _unitOfWork.Addresses.Update(addr);
                }
            }

            await _unitOfWork.Addresses.AddAsync(address);
            await _unitOfWork.SaveChangesAsync();

            TempData["Success"] = "Address added successfully";
            return RedirectToAction(nameof(Index));
        }

        return View(address);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var address = await _unitOfWork.Addresses.GetByIdAsync(id);
        if (address == null)
            return NotFound();

        var userId = _userManager.GetUserId(User);
        if (address.UserId != userId)
            return Forbid();

        return View(address);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Address address)
    {
        if (id != address.AddressId)
            return NotFound();

        var userId = _userManager.GetUserId(User);
        if (userId == null || address.UserId != userId)
            return Unauthorized();

        if (ModelState.IsValid)
        {
            if (address.IsDefault)
            {
                var existingAddresses = await _unitOfWork.Addresses.GetAddressesByUserIdAsync(userId);
                foreach (var addr in existingAddresses.Where(a => a.AddressId != id))
                {
                    addr.IsDefault = false;
                    _unitOfWork.Addresses.Update(addr);
                }
            }

            _unitOfWork.Addresses.Update(address);
            await _unitOfWork.SaveChangesAsync();

            TempData["Success"] = "Address updated successfully";
            return RedirectToAction(nameof(Index));
        }

        return View(address);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var address = await _unitOfWork.Addresses.GetByIdAsync(id);
        if (address == null)
            return NotFound();

        var userId = _userManager.GetUserId(User);
        if (address.UserId != userId)
            return Forbid();

        return View(address);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var address = await _unitOfWork.Addresses.GetByIdAsync(id);
        if (address == null)
            return NotFound();

        var userId = _userManager.GetUserId(User);
        if (address.UserId != userId)
            return Unauthorized();

        _unitOfWork.Addresses.Remove(address);
        await _unitOfWork.SaveChangesAsync();

        TempData["Success"] = "Address deleted successfully";
        return RedirectToAction(nameof(Index));
    }
}
