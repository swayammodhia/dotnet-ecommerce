using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElectronyatShop.Data;
using ElectronyatShop.Models;
using ElectronyatShop.ViewModels;

public class PaymentController : Controller
{
    private readonly ElectronyatShopDbContext _dbContext;

    public PaymentController(ElectronyatShopDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Show payment form
    [HttpGet]
    public async Task<IActionResult> Index(int cartId)
    {
        var cart = await _dbContext.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.Id == cartId);

        if (cart == null) return NotFound();

        // Calculate subtotal
        decimal subTotal = 0;
        foreach (var item in cart.CartItems)
        {
            subTotal += item.Quantity * item.Product.ActualPrice;
        }

        // Apply discount if available
        decimal discountPercent = 0;
        if (TempData["CouponDiscount"] is not null && decimal.TryParse(TempData["CouponDiscount"].ToString(), out var parsedDiscount))
        {
            discountPercent = parsedDiscount * 100; // assuming stored as 0.3
        }

        decimal discountMultiplier = 1 - (discountPercent / 100);
        decimal totalAfterDiscount = (subTotal * discountMultiplier) + CartViewModel.ShippingCost;

        var model = new PaymentViewModel
        {
            CartId = cartId,
            Amount = totalAfterDiscount
        };

        return View(model);
    }

    // Handle payment submission
    [HttpPost]
    public async Task<IActionResult> Index(PaymentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Simulate payment processing (replace with Stripe or real gateway later)
        bool paymentSuccess = true;

        if (paymentSuccess)
        {
            var cart = await _dbContext.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.Id == model.CartId);

            if (cart is null)
                return NotFound();

            // Recalculate total with potential discount
            decimal subTotal = 0;
            foreach (var item in cart.CartItems)
            {
                subTotal += item.Quantity * item.Product.ActualPrice;
            }

            decimal discountPercent = 0;
            if (TempData["CouponDiscount"] is not null && decimal.TryParse(TempData["CouponDiscount"].ToString(), out var parsedDiscount))
            {
                discountPercent = parsedDiscount * 100;
            }

            decimal discountMultiplier = 1 - (discountPercent / 100);
            cart.TotalPrice = (subTotal * discountMultiplier) + CartViewModel.ShippingCost;

            _dbContext.Carts.Update(cart);
            await _dbContext.SaveChangesAsync();

            // Redirect to order creation
            return RedirectToAction("Create", "Order", new { cartId = model.CartId });
        }

        ModelState.AddModelError("", "Payment failed. Please try again.");
        return View(model);
    }
}
