namespace YawShop.Services.PaytrailService.Models;

public class PaytrailRequestModel
{
    public string Stamp { get; set; } = Guid.NewGuid().ToString();
    public required string Reference { get; set; }
    public int Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public string Language { get; set; } = "FI";
    public List<PaytrailItemModel> Items { get; set; } = new List<PaytrailItemModel>();
    public required PaytrailCustomerModel Customer { get; set; }
    public required CallBackUrl RedirectUrls { get; set; }
    public CallBackUrl CallbackUrls { get; set; } = new CallBackUrl();
}

public class PaytrailItemModel
{
    public required int UnitPrice { get; set; }
    public required int Units { get; set; }
    public required decimal VatPercentage { get; set; }
    public required string ProductCode { get; set; }
    public required string Description { get; set; }
    public string Stamp { get; set; } = Guid.NewGuid().ToString();
}

public class PaytrailCustomerModel
{
    public required string Email { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string VatId { get; set; } = "";
    public string CompanyName { get; set; } = "";
}

public class CallBackUrl
{
    public string Success { get; set; } = "";
    public string Cancel { get; set; } = "";
}