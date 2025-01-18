using System.ComponentModel.DataAnnotations.Schema;
using YawShop.Attributes;
using YawShop.Interfaces;
using YawShop.Utilities;

namespace YawShop.Services.EventService.Models;

public class EventModel : IPublishable
{
    [NoApiUpdate]
    [NotPublic]
    public int Id { get; private set; } = 0;

    [NoApiUpdate]
    public string Code { get; private set; } = Guid.NewGuid().ToString();

    [NoApiUpdate]
    public required string ProductCode { get; set; }

    [NotPublic]
    public required int? RegistrationsQuantityTotal { get; set; }

    [NotPublic]
    [NoApiUpdate]
    public int RegistrationsQuantityUsed { get; set; }

    [NotMapped]
    [NoApiUpdate]
    public int? RegistrationsLeft
    {
        get
        {
            if (RegistrationsQuantityTotal != null)
            {
                return RegistrationsQuantityTotal - RegistrationsQuantityUsed;
            }
            else
            {
                return null;
            }
        }
    }

    [NoApiUpdate]
    public required DateTime EventStart { get; set; }

    [NoApiUpdate]
    public required DateTime EventEnd { get; set; }

    public int HoursBeforeEventUnavailable { get; set; }

    private EventStatus _status = EventStatus.Available;

    public EventStatus Status
    {
        get
        {
            if (EventStart.AddHours(-HoursBeforeEventUnavailable) < DateTime.Now && _status == EventStatus.Available)
            {
                _status = EventStatus.Expired;
                return _status;
            }

            if (RegistrationsLeft != null && RegistrationsLeft <= 0)
            {
                _status = EventStatus.Full;
                return _status;
            }

            return _status;
        }
        set
        {
            _status = value;
        }
    }

    [NotPublic]
    [NoApiUpdate]
    public int? ClientId { get; set; }

    /// <summary>
    /// Returns object from this object that excludes all properties with attribute tag "notPublic".
    /// </summary>
    /// <returns>object</returns>
    public object Public()
    {
        return AttributeParser.FilterPropertiesByAttribute(typeof(NotPublicAttribute), this);
    }

}


public enum EventStatus
{
    Available,
    Full,
    Cancelled,
    Finished,
    Expired,
}

