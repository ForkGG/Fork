using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ForkCommon.ExtensionMethods;

public static class EnumExtensions
{
    // Get a list of all values of enum T
    public static IEnumerable<T> GetValues<T>() where T : Enum
    {
        if (!typeof(T).IsEnum)
        {
            return new List<T>();
        }

        return Enum.GetValues(typeof(T)).Cast<T>();
    }

    // Friendly names for enums if [Description] is applied
    public static string FriendlyName(this Enum genericEnum)
    {
        Type genericEnumType = genericEnum.GetType();
        MemberInfo[] memberInfo = genericEnumType.GetMember(genericEnum.ToString());
        if (memberInfo.Length > 0)
        {
            object[] attribs = memberInfo[0]
                .GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attribs.Any())
            {
                return ((DescriptionAttribute)attribs.ElementAt(0)).Description;
            }
        }

        return genericEnum.ToString();
    }
}