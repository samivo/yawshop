using YawShop.Services.ClientService.Models;

namespace YawShop.Services.CheckoutService.Models;

public class ShoppingCartModel
{

    public required ClientModel Client { get; set; }

    /// <summary>
    /// List of products and quanties in shopping cart
    /// </summary>
    public required List<ProductInCart> ProductDetails { get; set; }

    /// <summary>
    /// Discount code. Currently supports only one per purchase.
    /// </summary>
    public string? DiscountCode { get; set; }

    /// <summary>
    /// Giftcard code. Currently supports only one per purchase and discounts full price.
    /// Maybe value based giftcards later?
    /// </summary>
    public string? GiftcardCode { get; set; }

}

public class ProductInCart
{
    public required string ProductCode { get; set; }
    public List<string>? EventCodes { get; set; }
    public required int Quantity { get; set; }

}
