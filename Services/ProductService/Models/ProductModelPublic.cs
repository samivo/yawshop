using System.ComponentModel.DataAnnotations;
using YawShop.Services.ClientService.Models;

namespace YawShop.Services.ProductService.Models;

public class ProductModelPublic
{
    [Required]
    public required string Code { get; set; }

    [Required]
    public required string Name { get; set; }

    [Required]
    public required int? MaxQuantityPerPurchase { get; set; }

    [Required]
    public required int? QuantityLeft { get; set; }

    [Required]
    public required int PriceInMinorUnitsIncludingVat { get; set; }

    [Required]
    public required decimal VatPercentage { get; set; }

    [Required]
    public required string? ShortDescription { get; set; }

    [Required]
    public required string? DescriptionOrInnerHtml { get; set; }

    [Required]
    public required string? AvatarImage { get; set; }

    [Required]
    public required ProductType ProductType { get; set; }

    [Required]
    public required List<ProductSpesificClientFieldsPublic>? CustomerFields { get; set; }

    [Required]

    public required int? ProductGroupId { get; set; }
    [Required]
    public required string GiftcardTargetProductCode { get; set; } = "";

    [Required]
    public required int GiftcardPeriodInDays { get; set; }

}

public class ProductSpesificClientFieldsPublic
{
    [Required]
    public required string FieldName { get; set; }

    [Required]
    public required bool IsRequired { get; set; } = true;

    [Required]
    public required string? Href { get; set; }
    
    [Required]
    public required CustomerFieldType FieldType { get; set; }
}