
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using YawShop.Attributes;
using YawShop.Services.ClientService.Models;

namespace YawShop.Services.CheckoutService.Models;

public class CheckoutModel
{

    [JsonIgnore]
    [NoApiUpdate]
    [NotPublic]
    public int Id { get; private set; } = 0;

    /// <summary>
    /// Identifies checkout because transaction id could be empty or null.
    /// </summary>
    [NoApiUpdate]
    [NotPublic]
    public string Reference { get; private set; } = Guid.NewGuid().ToString();

    [NoApiUpdate]
    public int TotalAmount { get; set; }

    [NotPublic]
    [NoApiUpdate]
    public int ClientId { get; set; }

    [NotMapped]
    public required ClientModel Client { get; set; }

    /// <summary>
    /// List of product information.
    /// Coupons and discounts is included in this list as product(s) with negative unit prices, so make sure
    /// the payment service provider accepts negative inline item values!
    /// </summary>
    [NoApiUpdate]
    public required List<CheckoutItem> Products { get; set; }

    /// <summary>
    /// If transaction id is null and payment status is paid, payment gateway was not invoked.
    /// This is because total amount was zero. Giftcard or discount was used.
    /// </summary>
    [NoApiUpdate]
    [NotPublic]
    public string? TransactionId { get; set; }

    [NoApiUpdate]
    public PaymentStatus PaymentStatus { get; set; }

    /// <summary>
    /// Card, mobile, online bank, paypal etc..
    /// </summary>
    [NoApiUpdate]
    public string? PaymentMethod { get; set; }

    [NotPublic]
    public string? InternalComment { get; set; }

    [NoApiUpdate]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [NoApiUpdate]
    [NotPublic]
    public DateTime UpdatetAt { get; set; } = DateTime.Now;

    [NotPublic]
    [NoApiUpdate]
    public string? ModifierName { get; set; }

    [NotPublic]
    [NoApiUpdate]
    public string? Hash { get; set; }

}

public class CheckoutItem
{
    [NoApiUpdate]
    [JsonIgnore]
    [NotPublic]
    public int Id { get; private set; }

    [NoApiUpdate]
    [JsonIgnore]
    [NotPublic]
    public int CheckoutModelId { get; set; }
    public required int UnitPrice { get; set; }
    public required int Units { get; set; }
    public required decimal VatPercentage { get; set; }
    public required string ProductCode { get; set; }
    public required string ProductName { get; set; }
    public string? EventCode { get; set; }
}

public enum PaymentStatus
{
    Initialized,
    New,
    Ok,
    Fail,
    Cancelled,
    Pending,
    Delayed
}

public class CallbackResult
{
    public required string CheckoutReference { get; set; }

    public string? TransactionId { get; set; }

    public string? PaymentProvider { get; set; }

    public required PaymentStatus PaymentStatus { get; set; }
}