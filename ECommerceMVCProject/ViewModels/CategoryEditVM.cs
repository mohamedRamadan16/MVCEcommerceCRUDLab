using System.ComponentModel.DataAnnotations;

namespace ECommerceMVCProject.ViewModels;

public class CategoryEditVM
{
    public int CategoryId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    public int? ParentCategoryId { get; set; }
}
