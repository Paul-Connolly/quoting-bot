using System;
using System.ComponentModel;
using System.Reflection;

namespace QuotingBot.Common.Enums
{
    public static class EnumExtension
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            object[] attribs = field.GetCustomAttributes(typeof(DescriptionAttribute), true);
            return attribs.Length > 0 ? ((DescriptionAttribute)attribs[0]).Description : string.Empty;
        }
    }
}
