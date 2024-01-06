using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ForkCommon.ExtensionMethods
{
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
        public static string FriendlyName(this Enum GenericEnum)
        {
            Type genericEnumType = GenericEnum.GetType();
            MemberInfo[] memberInfo = genericEnumType.GetMember(GenericEnum.ToString());
            if ((memberInfo.Length > 0))
            {
                var attribs = memberInfo[0]
                    .GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if (attribs.Any())
                {
                    return ((System.ComponentModel.DescriptionAttribute)attribs.ElementAt(0)).Description;
                }
            }

            return GenericEnum.ToString();
        }
    }
}