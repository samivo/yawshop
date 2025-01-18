using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using YawShop.Attributes;
using YawShop.Interfaces;
using YawShop.Services.ClientService.Models;
using YawShop.Utilities;

namespace YawShop.Services.ProductService.Models;

public class ProductModel : IPublishable
{
    [NoApiUpdate]
    [NotPublic]
    public int Id { get; private set; } = 0;

    [NoApiUpdate]
    public string Code { get; private set; } = Guid.NewGuid().ToString();

    public required string Name { get; set; }

    [NotPublic]
    public required bool IsActive { get; set; }

    [NotPublic]
    public required bool IsVisibleToPublic { get; set; }

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

    public required int PriceInMinorUnitsIncludingVat { get; set; }

    [Range(0, 100.0)]
    public required decimal VatPercentage { get; set; }

    public required string ShortDescription { get; set; }

    public required string DescriptionOrInnerHtml { get; set; }

    public string? AvatarImage { get; set; }

    [NotPublic]
    public string? InternalComment { get; set; }

    [NotPublic]
    public DateTime AvailableFrom { get; set; } = DateTime.Now;

    [NotPublic]
    public DateTime? AvailableTo { get; set; }

    [NoApiUpdate]
    public ProductType ProductType { get; set; }

    public List<ProductSpesificClientFields>? CustomerFields { get; set; }

    /// <summary>
    /// Not implemented yet. Example if you have same type of product with different options(eg. different size t-shirts), use group id to group them up.
    /// Gourp id then could be used in the product page to list same type of products in drop down menu.
    /// Maybe better options but just reminder.
    /// </summary>
    public int? ProductGroupId { get; set; }

    [NoApiUpdate]
    public string GiftcardTargetProductCode { get; set; } = "";

    public int GiftcardPeriodInDays { get; set; }

    [NotPublic]
    [NoApiUpdate]
    public DateTime CreatedAt { get; private set; } = DateTime.Now;

    [NotPublic]
    [NoApiUpdate]
    public DateTime ModifiedAt { get; private set; } = DateTime.Now;

    [NotPublic]
    [NoApiUpdate]
    public string? Modifier { get; private set; }

    public void UpdateAccess(string modifier)
    {
        Modifier = modifier;
        ModifiedAt = DateTime.Now;
    }

    /// <summary>
    /// Returns object that excludes all properties with attribute tag "notPublic".
    /// </summary>
    /// <returns>object</returns>
    public object Public()
    {
        return AttributeParser.FilterPropertiesByAttribute(typeof(NotPublicAttribute), this);
    }

}

public class ProductSpesificClientFields
{
    [NoApiUpdate]
    [NotPublic]
    public int Id { get; private set; }

    [NoApiUpdate]
    [NotPublic]
    public int ProductModelId { get; set; }
    public required string FieldName { get; set; }
    public required string FieldNamePublic { get; set; }
    public bool IsRequired { get; set; }
    public string? Href { get; set; }
    public required CustomerFieldType FieldType { get; set; }
}

public enum ProductType
{
    Virtual,
    Giftcard,
    Event
}