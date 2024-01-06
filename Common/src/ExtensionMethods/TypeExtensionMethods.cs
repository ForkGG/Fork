using System;
using System.Linq;
using System.Reflection;

namespace ForkCommon.ExtensionMethods;

public static class TypeExtensionMethods
{
    // Friendly names for types if [Description] is applied
    public static string FriendlyName(this Type type)
    {
        if (type.CustomAttributes.Any())
        {
            var attribs = type.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (attribs.Any())
            {
                return ((System.ComponentModel.DescriptionAttribute)attribs.ElementAt(0)).Description;
            }
        }

        return type.Name;
    }
}