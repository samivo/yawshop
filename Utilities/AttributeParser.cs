using System.Reflection;

namespace YawShop.Utilities;

public class AttributeParser
{
    /// <summary>
    /// Copies properties to new object, excluding properties that are tagged with given attribute.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="sourceObject"></param>
    /// <returns>Object that exclueds properties that include given attribute</returns>
    public static object FilterPropertiesByAttribute(Type attributeType, object sourceObject)
    {
        // TODO: attributes inside an array is not checked!

        var dictionary = new Dictionary<string, object?>();

        foreach (var prop in sourceObject.GetType().GetProperties())
        {
            if (!prop.IsDefined(attributeType))
            {
                dictionary.Add(SetCamelCase(prop.Name), prop.GetValue(sourceObject));
            }

        }

        return dictionary;
    }

    private static string SetCamelCase(string targetString)
    {

        if (string.IsNullOrEmpty(targetString))
        {
            throw new ArgumentNullException(nameof(targetString), "Cannot convert null or empty string to camel case.");
        }

        return string.Concat(targetString[0].ToString().ToLowerInvariant(), targetString.Substring(1));

    }
}