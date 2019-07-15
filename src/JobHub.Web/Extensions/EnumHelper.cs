using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JobHub.Web.Extensions
{
    public static class EnumHelper
    {
        public static IList<SelectListItem> ToSelectListItems(Type type, Enum selectedItem = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var items = new List<SelectListItem>();
            var enumItems = Enum.GetValues(type);
            foreach (Enum e in enumItems)
            {
                var item = new SelectListItem()
                {
                    Text = e.GetDisplayName(),
                    Value = Convert.ToInt32(e).ToString()
                };

                if (item.Text == selectedItem.GetDisplayName())
                {
                    item.Selected = true;
                }
                items.Add(item);
            }

            return items;
        }

        public static IDictionary<Enum, string> ToDictionary(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var dics = new Dictionary<Enum, string>();
            var enumValues = Enum.GetValues(type);

            foreach (Enum value in enumValues)
            {
                dics.Add(value, GetDisplayName(value));
            }

            return dics;
        }

        public static string GetDisplayName(this Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var displayName = value.ToString();
            var fieldInfo = value.GetType().GetField(displayName);
            var attributes = (DisplayAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);

            if (attributes.Length > 0)
            {
                displayName = attributes[0].Description;
            }

            return displayName;
        }
    }
}
