using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Nongzhsh.JobHub
{
    public static class DictionaryHelper
    {
        public static IDictionary<string, TValue> ToDictionary<TValue>(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo =>
                {
                    if(propInfo.PropertyType.IsValueType)
                    {
                        var obj = propInfo.GetValue(source);
                        return (TValue)Convert.ChangeType(obj, typeof(TValue), CultureInfo.InvariantCulture);

                    }
                    else
                    {
                        return (TValue)propInfo.GetValue(source);
                    }
                });
        }
    }
}
