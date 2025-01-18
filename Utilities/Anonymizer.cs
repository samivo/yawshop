using System.Reflection;

namespace YawShop.Utilities;

public class Anonymizer
{
    /// <summary>
    /// Anonymizes properties of object with given attribute.
    /// </summary>
    /// <param name="attributeType">Attributte that marks anonymizable properties</param>
    /// <param name="targetObject">Anonymization target object</param>
    /// <returns></returns>
    public static void AnonymizeObjectProperties(Type attributeType, object targetObject)
    {
        foreach (var props in targetObject.GetType().GetProperties())
        {
            if (props.IsDefined(attributeType))
            {
                props.SetValue(targetObject, default);
            }
        }
    }
}

