using System.Runtime.Serialization;

namespace YawShop.Interfaces;

public interface IPublishable
{
    /// <summary>
    /// Returns object from this object that excludes all properties with attribute tag "notPublic".
    /// Sets property names to camelCase
    /// </summary>
    /// <returns>object</returns>
    public object Public();
}