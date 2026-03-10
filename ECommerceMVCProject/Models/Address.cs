using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ECommerceMVCProject.Models;

public class Address
{
    public int AddressId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
    public bool IsDefault { get; set; }

    [ValidateNever]
    public AppUser User { get; set; } = null!;
    [ValidateNever]
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
