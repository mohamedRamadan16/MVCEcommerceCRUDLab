using ECommerceMVCProject.Models;

namespace ECommerceMVCProject.ViewModels;

public class CheckoutVM
{
    public CartVM Cart { get; set; } = new CartVM();
    public List<Address> Addresses { get; set; } = new List<Address>();
    public int SelectedAddressId { get; set; }
}
