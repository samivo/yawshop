namespace YawShop.Services.PaytrailService.Models;

public class ResponseModel
{
    //For debugging purposes
    public string? RequestId { get; set; }
    public string? TransactionId { get; set; }
    public string? Href { get; set; }
    public string? Terms { get; set; }
    public List<PaymentMethodGroupDataModel>? Groups { get; set; }
    public string? Reference { get; set; }
    public List<ProviderModel>? Providers { get; set; }

    public object? CustomProviders { get; set; }

}

public class PaymentMethodGroupDataModel
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Icon { get; set; }
    public string? Svg { get; set; }
}

public class ProviderModel
{
    public string? Url { get; set; }
    public string? Icon { get; set; }
    public string? Svg { get; set; }
    public string? Group { get; set; }
    public string? Name { get; set; }
    public string? Id { get; set; }
    public List<FormFieldModel>? Parameters { get; set; }

}

public class FormFieldModel
{
    public string? Name { get; set; }
    public string? Value { get; set; }
}