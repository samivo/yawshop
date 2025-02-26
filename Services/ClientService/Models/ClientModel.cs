using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using YawShop.Attributes;

namespace YawShop.Services.ClientService.Models;

public class ClientModel
{
    [NoApiUpdate]
    [JsonIgnore]
    [NotPublic]
    public int Id { get; private set; } = 0;

    [NoApiUpdate]
    [NotPublic]
    public string Code { get; private set; } = Guid.NewGuid().ToString();

    [MinLength(2)]
    [MaxLength(60)]
    public required string FirstName { get; set; }

    [MinLength(2)]
    [MaxLength(60)]
    public required string LastName { get; set; }

    [EmailAddress]
    [MaxLength(50)]
    public required string Email { get; set; }

    [NoApiUpdate]
    public List<AdditionalClientFields>? AdditionalInfo { get; set; }

    [NoApiUpdate]
    [NotPublic]
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    [NotPublic]
    public string? InternalComment { get; set; }

}

public class AdditionalClientFields
{
    [NoApiUpdate]
    [NotPublic]
    [JsonIgnore]
    public int Id { get; private set; }

    [NoApiUpdate]
    [NotPublic]
    [JsonIgnore]
    public int ClientModelId { get; set; }

    [NoApiUpdate]
    public required string FieldName { get; set; }

    [NoApiUpdate]
    public string? FieldValue { get; set; }

    [NoApiUpdate]
    public required CustomerFieldType FieldType { get; set; }

}

public enum CustomerFieldType
{
    Text = 0,
    Integer = 1,
    Decimal = 2,
    DateTime = 3,
    Boolean = 4,
    Agreement = 5
}
