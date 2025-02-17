using System.ComponentModel.DataAnnotations.Schema;
using YawShop.Attributes;
using YawShop.Interfaces;
using YawShop.Services.ClientService.Models;
using YawShop.Utilities;

namespace YawShop.Services.EventService.Models;

public class EventModel : IPublishable
{
    [NoApiUpdate]
    [NotPublic]
    public int Id { get; private set; } = 0;

    [NoApiUpdate]
    public required string Code { get; set; }

    [NoApiUpdate]
    public required string ProductCode { get; set; }

    
    public required DateTime EventStart { get; set; }

    
    public required DateTime EventEnd { get; set; }

    public int HoursBeforeEventUnavailable { get; set; }

    [NotPublic]
    public bool IsVisible { get; set; } = false;

    [NoApiUpdate]
    [NotMapped]
    public bool IsAvailable { 
        get
        {
            //Check that events start is still in future
            if (EventStart.CompareTo(DateTime.UtcNow.AddHours(HoursBeforeEventUnavailable)) > 0)
            {
                //Check that there is no client registered
                if (ClientCode == null)
                {
                    return true;
                }
                
            }

            return false;
        }
     }

    [NotPublic]
    public string? ClientCode { get; set; }

    [NotMapped]
    [NotPublic]
    public ClientModel? Client { get; set; }

    /// <summary>
    /// Returns object from this object that excludes all properties with attribute tag "notPublic".
    /// </summary>
    /// <returns>object</returns>
    public object Public()
    {
        return AttributeParser.FilterPropertiesByAttribute(typeof(NotPublicAttribute), this);
    }

}

