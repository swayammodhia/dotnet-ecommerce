using ElectronyatShop.Models;

namespace ElectronyatShop.ViewModels;

public class CartViewModel
{
    public int Id { get; set; }

    public const int ShippingCost = 10;

    public decimal SubTotalPrice { get; set; } = 0;

    public decimal DiscountAmount { get; set; } = 0; // In percent (e.g., 10 for 10%)

    public string? CouponCode { get; set; }

    public string? UserId { get; set; }

    public List<CartItem>? CartItems { get; set; }

    public decimal DiscountedTotal => SubTotalPrice * (1 - DiscountAmount / 100);

    public decimal TotalPrice => DiscountedTotal + ShippingCost;
}
