
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using YawShop.Attributes;
using YawShop.Interfaces;
using YawShop.Services.ClientService.Models;
using YawShop.Utilities;

namespace YawShop.Services.GiftcardService.Models;

public class GiftcardModel : IPublishable
{
    [JsonIgnore]
    [NoApiUpdate]
    [NotPublic]
    public int Id { get; private set; } = 0;

    [NoApiUpdate]
    public string Code { get; set; } = GiftcardCodeGenerator.CreateCode();

    [NoApiUpdate]
    public required string Name { get; set; }

    public int ValueLeftInMinorUnits { get; set; }

    /// <summary>
    /// If true, Valueleft is used. If false, target product code is used to discount full product price.
    /// </summary>
    [NotPublic]
    [NoApiUpdate]
    public bool IsValueBased { get; set; }

    [NotPublic]
    [NoApiUpdate]
    public required string TargetProductCode { get; set; }

    [NotPublic]
    public string? InternalComment { get; set; }

    [NoApiUpdate]
    public required DateTime PurchaseDate { get; set; }

    public required DateTime ExpireDate { get; set; }

    public DateTime? UsedDate { get; set; }

    [NoApiUpdate]
    [NotPublic]
    public int OwnerClientId { get; set; }

    [NotMapped]
    [NotPublic]
    public ClientModel? OwnerClient { get; set; }

    [NotPublic]
    public int? UserClientId { get; set; }

    [NotMapped]
    [NotPublic]
    public ClientModel? UserClient { get; set; }

    public void SetUsed(int userClientId)
    {

        UserClientId = userClientId;
        UsedDate = DateTime.Now;

    }

    public void SetUnused()
    {
        UserClientId = null;
        UsedDate = null;
    }

    public bool IsValid()
    {
        if (UserClientId != null || UsedDate != null || ExpireDate < DateTime.Now)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    /// <summary>
    /// Returns object from this object that excludes all properties with attribute tag "notPublic".
    /// </summary>
    /// <returns>object</returns>
    public object Public()
    {
        return AttributeParser.FilterPropertiesByAttribute(typeof(NotPublicAttribute), this);
    }


}