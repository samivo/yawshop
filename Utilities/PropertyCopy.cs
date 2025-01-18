using System.Diagnostics;

namespace YawShop.Utilities;

public static class PropertyCopy
{

    /// <summary>
    /// Copies all properties from object to object and ignores properties where given attribute.
    /// </summary>
    /// <param name="copyFrom"></param>
    /// <param name="copyTo"></param>
    /// <param name="attribute"></param>
    public static void CopyWithoutAttribute(object copyFrom, object copyTo, Type ignoreAttribute)
    {
        foreach (var prop in copyTo.GetType().GetProperties())
        {
            if (!Attribute.IsDefined(prop, ignoreAttribute))
            {
                var newValue = prop.GetValue(copyFrom);
                prop.SetValue(copyTo, newValue);
            }
        }
    }

    public static void CopyWithAttribute(object copyFrom, object copyTo, Type attribute)
    {
        foreach (var prop in copyTo.GetType().GetProperties())
        {
            if (Attribute.IsDefined(prop, attribute))
            {
                var newValue = prop.GetValue(copyFrom);
                prop.SetValue(copyTo, newValue);
            }
        }
    }
}