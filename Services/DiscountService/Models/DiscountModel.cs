using System.Text.Json.Serialization;
using YawShop.Attributes;
using YawShop.Utilities;



namespace YawShop.Services.DiscountService.Models
{

    public class DiscountModel
    {
        [JsonIgnore]
        [NoApiUpdate]
        [NotPublic]
        public int Id { get; private set; } = 0;

        [NoApiUpdate]
        public required string Code { get; set; }

        [NotPublic]
        public required string Description { get; set; }

        [NoApiUpdate]
        public required string TargetProductCode { get; set; }

        [NoApiUpdate]
        public required int DiscountAmountInMinorUnits { get; set; }

        [NotPublic]
        public int? QuantityTotal { get; set; }

        [NotPublic]
        public DateTime? ValidFrom { get; set; }

        [NotPublic]
        public DateTime? ValidTo { get; set; }

        [NotPublic]
        [NoApiUpdate]
        public int QuantityUsed { get; set; }

        [NotPublic]
        public required string InternalDescription { get; set; }

        public bool IsValid()
        {
            if (ValidFrom != null && ValidFrom > DateTime.UtcNow)
            {
                return false;
            }

            if (ValidTo != null && ValidTo < DateTime.UtcNow)
            {
                return false;
            }

            if (QuantityUsed >= QuantityTotal)
            {
                return false;
            }

            return true;

        }

        public object Public()
        {
            return AttributeParser.FilterPropertiesByAttribute(typeof(NotPublicAttribute), this);
        }

    }




}
