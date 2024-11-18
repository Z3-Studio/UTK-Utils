using System;
using System.Globalization;

namespace Z3.Utils.ExtensionMethods
{
    public static class SystemExtensions
    {
        private const string DefaultTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

        public static Guid? ToNullableGuid(this string input)
        {
            return ToNullableGuid(input, Guid.Empty);
        }

        public static Guid? ToNullableGuid(this string input, Guid? defaultValue)
        {
            if (Guid.TryParse(input, out Guid guid))
                return guid;

            return defaultValue;
        }

        public static Guid ToGuid(this string input)
        {
            return Guid.Parse(input);
        }

        public static DateTime? ToNullableDateTime(this string input, string format = DefaultTimeFormat, DateTime? defaultValue = null)
        {
            if (DateTime.TryParse(input, out DateTime result))
                return result;

            return defaultValue;
        }

        public static DateTime ToDateTime(this string input, string format = DefaultTimeFormat)
        {
            return input.ToNullableDateTime(format, new DateTime()).Value;
        }

        public static DateTime? ToExactDateTime(this string input, string format = DefaultTimeFormat)
        {
            if (DateTime.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                return result;

            return null;
        }

        public static TEnum ToEnum<TEnum>(this string input) where TEnum : struct, Enum
        {
            return Enum.Parse<TEnum>(input);
        }
    }
}