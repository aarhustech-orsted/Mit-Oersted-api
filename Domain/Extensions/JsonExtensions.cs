using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Linq
{
    public static class JsonExtensions
    {
        public static JToken GetValueOrThrowException(this JObject parent, string propertyName)
        {
            if (parent.TryGetValue(propertyName, out JToken value))
            {
                return value;
            }

            throw new KeyNotFoundException($"Property '{ propertyName }' was not found in JSON object.");
        }

        public static T GetEnumValueOrThrowException<T>(this JObject parent, string propertyName) where T : struct
        {
            JToken value = parent.GetValueOrThrowException(propertyName);
            T enumValue = (T)Enum.Parse(typeof(T), value.ToString(), true);
            return enumValue;
        }

        public static Dictionary<string, string> GetDictionaryOrThrowException(this JObject parent, string propertyName)
        {
            JToken value = parent.GetValueOrThrowException(propertyName);
            return value.ToObject<Dictionary<string, string>>();
        }

        public static Dictionary<string, string> GetDictionary(this JObject parent, string propertyName, bool isRequired = true)
        {
            JToken value;
            if (isRequired)
            {
                value = parent.GetValueOrThrowException(propertyName);
            }
            else
            {
                parent.TryGetValue(propertyName, out value);
            }

            if (value != null)
            {
                return value.ToObject<Dictionary<string, string>>();
            }
            return new Dictionary<string, string>();
        }

        public static double? GetNullableDouble(this JObject parent, string propertyName)
        {
            if (parent.TryGetValue(propertyName, out JToken value))
            {
                return (double?)value;
            }
            return null;
        }

        public static bool GetBooleanValueOrThrowException(this JObject parent, string propertyName)
        {
            JToken value = parent.GetValueOrThrowException(propertyName);
            return (bool)value;
        }

        public static bool? GetNullableBoolean(this JObject parent, string propertyName)
        {
            if (parent.TryGetValue(propertyName, out JToken value))
            {
                return (bool?)value;
            }
            return null;
        }

        public static int GetIntValueOrThrowException(this JObject parent, string propertyName)
        {
            JToken value = parent.GetValueOrThrowException(propertyName);
            return (int)value;
        }

        public static double GetDoubleValueOrThrowException(this JObject parent, string propertyName)
        {
            JToken value = parent.GetValueOrThrowException(propertyName);
            return (double)value;
        }

        public static int? GetNullableInt(this JObject parent, string propertyName)
        {
            if (parent.TryGetValue(propertyName, out JToken value))
            {
                return (int?)value;
            }
            return null;
        }

        public static double[] GetDoubleArray(this JObject parent, string propertyName, bool isRequired = true)
        {
            JToken value;
            if (isRequired)
            {
                value = parent.GetValueOrThrowException(propertyName);
            }
            else
            {
                parent.TryGetValue(propertyName, out value);
            }

            if (value != null && value is JArray)
            {
                return ((JArray)value).ToObject<double[]>();
            }

            return null;
        }

        public static T[] GetArray<T>(this JObject parent, string propertyName, bool isRequired = true)
        {
            JToken value;
            if (isRequired)
            {
                value = parent.GetValueOrThrowException(propertyName);
            }
            else
            {
                parent.TryGetValue(propertyName, out value);
            }

            if (value != null && value is JArray array)
            {
                return array.ToObject<T[]>();
            }

            return Array.Empty<T>();
        }

    }
}