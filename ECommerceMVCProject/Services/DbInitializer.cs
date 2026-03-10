using ECommerceMVCProject.Data;
using ECommerceMVCProject.Models;
using Microsoft.AspNetCore.Identity;

namespace ECommerceMVCProject.Services;

public static class DbInitializer
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        if (!await roleManager.RoleExistsAsync("Customer"))
        {
            await roleManager.CreateAsync(new IdentityRole("Customer"));
        }

        var adminEmail = "admin@ecommerce.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        
        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Admin User",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(adminUser, "Admin@123");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        if (!context.Categories.Any())
        {
            var electronics = new Category { Name = "Electronics" };
            var clothing = new Category { Name = "Clothing" };
            var books = new Category { Name = "Books" };

            context.Categories.AddRange(electronics, clothing, books);
            await context.SaveChangesAsync();

            var phones = new Category { Name = "Phones", ParentCategoryId = electronics.CategoryId };
            var laptops = new Category { Name = "Laptops", ParentCategoryId = electronics.CategoryId };
            
            context.Categories.AddRange(phones, laptops);
            await context.SaveChangesAsync();
        }

        if (!context.Products.Any())
        {
            var category = context.Categories.First();

            var products = new[]
            {
                new Product { Name = "Sample Product 1", SKU = "PROD001", Price = 99.99m, StockQuantity = 50, IsActive = true, CategoryId = category.CategoryId, CreatedAt = DateTime.Now },
                new Product { Name = "Sample Product 2", SKU = "PROD002", Price = 149.99m, StockQuantity = 30, IsActive = true, CategoryId = category.CategoryId, CreatedAt = DateTime.Now },
                new Product { Name = "Sample Product 3", SKU = "PROD003", Price = 79.99m, StockQuantity = 100, IsActive = true, CategoryId = category.CategoryId, CreatedAt = DateTime.Now }
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
    }
}
