using System;
using System.Collections.Generic;

namespace Nightingale.Core.Auth
{
    public static class AuthExtensions
    {
        public static string GetProp(this IAuth authItem, string key)
        {
            if (authItem?.AuthProperties == null)
            {
                return "";
            }

            if (authItem.AuthProperties.TryGetValue(key, out string value))
            {
                return value ?? "";
            }

            return "";
        }


        public static TEnum GetEnumProp<TEnum>(this IAuth item, string key) where TEnum : struct
        {
            var value = item.GetProp(key);

            if (Enum.TryParse<TEnum>(value, out var result))
            {
                return result;
            }

            return default;
        }

        public static void SetProp(this IAuth authItem, string key, string value)
        {
            if (authItem == null)
            {
                return;
            }

            if (authItem.AuthProperties == null)
            {
                authItem.AuthProperties = new Dictionary<string, string>();
            }

            if (string.IsNullOrEmpty(value))
            {
                // delete the prop
                authItem.AuthProperties.Remove(key);
                return;
            }

            if (authItem.AuthProperties.ContainsKey(key))
            {
                authItem.AuthProperties[key] = value;
            }
            else
            {
                authItem.AuthProperties.Add(key, value);
            }
        }
    }
}
