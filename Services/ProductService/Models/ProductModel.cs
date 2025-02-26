using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YawShop.Attributes;
using YawShop.Services.ClientService.Models;

namespace YawShop.Services.ProductService.Models;

public class ProductModel
{
    [NoApiUpdate]
    [NotPublic]
    public int Id { get; private set; } = 0;

    [NoApiUpdate]
    public required string Code { get;  set; } 

    [Required]
    public required string Name { get; set; }

    [NotPublic]
    public bool IsActive { get; set; }

    [NotPublic]
    public bool IsVisibleToPublic { get; set; }

    public int? MaxQuantityPerPurchase { get; set; }

    [NotPublic]
    public int? QuantityTotal { get; set; }

    [NoApiUpdate]
    [NotPublic]
    public int QuantityUsed { get; set; }

    [NotMapped]
    [NoApiUpdate]
    public int? QuantityLeft
    {
        get
        {
            if (QuantityTotal != null)
            {
                return QuantityTotal - QuantityUsed;
            }
            else
            {
                return null;
            }
        }
    }

    [Required]
    [Range(0, double.MaxValue)]
    public required int PriceInMinorUnitsIncludingVat { get; set; }

    [Required]
    [Range(0, 100.0)]
    public required decimal VatPercentage { get; set; }

    public string? ShortDescription { get; set; }

    public string? DescriptionOrInnerHtml { get; set; }

    public string? AvatarImage { get; set; }

    [NotPublic]
    public string? InternalComment { get; set; }

    [Required]
    [NotPublic]
    public required DateTime AvailableFrom { get; set; }

    [NotPublic]
    public DateTime? AvailableTo { get; set; }

    [Required]
    [NoApiUpdate]
    public required ProductType ProductType { get; set; }

    public List<ProductSpesificClientFields>? CustomerFields { get; set; }

    public int? ProductGroupId { get; set; }

    public string GiftcardTargetProductCode { get; set; } = "";

    public int GiftcardPeriodInDays { get; set; }

    [NotPublic]
    [NoApiUpdate]
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    [NotPublic]
    [NoApiUpdate]
    public DateTime ModifiedAt { get; private set; } = DateTime.UtcNow;

    [NotPublic]
    [NoApiUpdate]
    public string? Modifier { get; private set; }

    public void UpdateAccess(string modifier)
    {
        Modifier = modifier;
        ModifiedAt = DateTime.UtcNow;
    }
}

public class ProductSpesificClientFields
{
    [NoApiUpdate]
    [NotPublic]
    public int Id { get; private set; }

    [NoApiUpdate]
    [NotPublic]
    public int ProductModelId { get; private set; }
    
    [Required]
    public required string FieldName { get; set; }

    public bool IsRequired { get; set; } = true;

    public string? Href { get; set; }

    [Required]
    public required CustomerFieldType FieldType { get; set; }
}

public enum ProductType
{
    Virtual,
    Giftcard,
    Event
}