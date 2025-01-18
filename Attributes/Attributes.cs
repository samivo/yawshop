using System.ComponentModel.DataAnnotations;

namespace YawShop.Attributes;

/// <summary>
/// API update wont change values for this property
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class NoApiUpdateAttribute : Attribute
{
    //Marker attribute
}

/// <summary>
/// This property is dropped from unauthorized api endpoints. Use attribute parser.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class NotPublicAttribute : Attribute
{
    //Marker attribute
}

/// <summary>
/// Property that can be anonymized
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class AnonymizableAttribute : Attribute
{
    //Marker attribute
}
