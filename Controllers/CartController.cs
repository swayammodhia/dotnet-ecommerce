using ElectronyatShop.Data;
using ElectronyatShop.Models;
using ElectronyatShop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectronyatShop.Controllers;

[Authorize("CustomerRole")]
public class CartController(UserManager<ApplicationUser> userManager, ElectronyatShopDbContext context)
    : Controller
{
    #region Controller Constructor and Attributes

    private string? UserId { get; set; }

    private Cart? Cart { get; set; }

    #endregion

    #region Controller Actions

    [HttpGet]
public async Task<IActionResult> Index()
{
    SetCart();

    CartViewModel cartViewModel = new()
    {
        Id = Cart?.Id ?? throw new NullReferenceException("Cart is not initialized"),
        SubTotalPrice = 0,
        CartItems = Cart.CartItems?.ToList(),

        // Read coupon code from TempData
        CouponCode = TempData["CouponApplied"] as string,
    };

    // Parse discount percentage stored as string in TempData (e.g., "30")
    if (TempData["CouponDiscount"] != null && decimal.TryParse(TempData["CouponDiscount"].ToString(), out var discount))
    {
        cartViewModel.DiscountAmount = discount;
    }
    else
    {
        cartViewModel.DiscountAmount = 0;
    }

    foreach (var item in Cart?.CartItems ?? new List<CartItem>())
    {
        var product = await context.Products.FindAsync(item.ProductId);
        if (product is not null)
            cartViewModel.SubTotalPrice += product.ActualPrice * item.Quantity;
    }

    // Persist TempData to keep coupon info for next request if needed
    TempData.Keep("CouponApplied");
    TempData.Keep("CouponDiscount");

    return View("Index", cartViewModel);
}


    [HttpPost]
    public async Task<IActionResult> AddToCart([FromForm] CartItemViewModel cartItem)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(actionName: "Index", controllerName: "Product");

        SetCart();
        var item = Cart?.CartItems?.FirstOrDefault(item => item.ProductId == cartItem.ProductId) ??
                   new CartItem { CartId = Cart?.Id, ProductId = cartItem.ProductId, Quantity = 0 };

        item.Quantity += cartItem.Quantity;
        var product = await context.Products.FindAsync(item.ProductId);
        if (product is not null)
        {
            product.AvailableQuantity -= item.Quantity;
            context.Products.Update(product);
        }
        context.CartItems.Update(item);
        await context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult ApplyCoupon([FromForm] string couponCode)
    {
        // For now just accept "SUMMER30" as a valid code
        if (!string.IsNullOrEmpty(couponCode) && couponCode.Equals("SUMMER30", StringComparison.OrdinalIgnoreCase))
        {
            TempData["CouponApplied"] = "SUMMER30";
            TempData["CouponDiscount"] = "30"; // 30% discount (as decimal)
            //TempData.Remove("CouponError");
        }
        else
        {
            TempData["CouponError"] = "Invalid coupon code!";
            TempData["CouponDiscount"] = "0";
            //TempData.Remove("CouponApplied");
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromCart([FromForm] int id)
    {
        if (!ModelState.IsValid) return RedirectToAction("Index");

        SetCart();
        var item = await context.CartItems.FindAsync(id);
        if (item is null) return RedirectToAction("Index");

        var product = await context.Products.FindAsync(item.ProductId);
        if (product is not null)
        {
            product.AvailableQuantity += item.Quantity;
            context.Products.Update(product);
        }
        Cart?.CartItems?.Remove(item);
        context.CartItems.Remove(item);
        await context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    #endregion

    #region Controller Logic

    private void SetCart()
    {
        UserId = userManager.GetUserId(User);
        Cart = new Cart();
        if (UserId is not null)
            Cart = context.Carts.Include(userCart => userCart.CartItems).First(c => c.UserId == UserId);
    }

    #endregion
}
