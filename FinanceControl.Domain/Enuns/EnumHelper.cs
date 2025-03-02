using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinanceControl.Domain.Enuns;

public static class EnumHelper
{
    public static string GetEnumDescription<T>(this T enumValue) where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
            return null;

        var description = enumValue.ToString();
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        if (fieldInfo != null)
        {
            var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attrs != null && attrs.Length > 0)
            {
                description = ((DescriptionAttribute)attrs[0]).Description;
            }
        }

        return description;
    }

    public static List<string> GetEnumDescriptionAtributte<TEnum>(this TEnum[] enumValue) where TEnum : struct, IConvertible
    {
        if (!enumValue.Any() || enumValue.Length <= 0)
            return null;

        var result = new List<string>();
        foreach (var item in enumValue)
        {
            var description = item.ToString();
            var fieldInfo = item.GetType().GetField(item.ToString());

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                    description = ((DescriptionAttribute)attrs[0]).Description;
            }

            result.Add(description);
        }

        return result;
    }
}