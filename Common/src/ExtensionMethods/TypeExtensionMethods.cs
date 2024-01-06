using System;
using System.ComponentModel;
using System.Linq;

namespace ForkCommon.ExtensionMethods;

public static class TypeExtensionMethods
{
    // Friendly names for types if [Description] is applied
    public static string FriendlyName(this Type type)
    {
        if (type.CustomAttributes.Any())
        {
            object[] attribs = type.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attribs.Any())
            {
                return ((DescriptionAttribute)attribs.ElementAt(0)).Description;
            }
        }

        return type.Name;
    }
}