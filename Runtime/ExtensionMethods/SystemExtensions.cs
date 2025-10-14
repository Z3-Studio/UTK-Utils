using System;
using System.Globalization;

namespace Z3.Utils.ExtensionMethods
{
    public static class SystemExtensions
    {
        private const string Greek = "el-GR";
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

        /// <summary> Formats displaying day, month and year </summary>
        /// <returns> Ex: 01/01/2000 </returns>
        public static string FormatToDate(this DateTime dateTime) => dateTime.ToString("dd/MM/yyyy");

        /// <summary> Formats displaying hours, minutes and seconds </summary>
        /// <returns> Ex: 00:00:00 </returns>
        public static string FormatToTime(this DateTime dateTime) => dateTime.ToString("HH:mm:ss");

        /// <returns> Ex: 00:00:00 </returns>
        public static string FormatToTime(this float time)
        {
            TimeSpan timeSpawn = TimeSpan.FromSeconds(time);
            return $"{(int)timeSpawn.TotalHours}:{timeSpawn.Minutes:D2}:{timeSpawn.Seconds:D2}";
        }

        /// <returns> 00:00:000 </returns>
        public static string FormatToStopwatch(this float time)
        {
            TimeSpan timeSpawn = TimeSpan.FromSeconds(time);
            return $"{(int)timeSpawn.TotalMinutes:D2}:{timeSpawn.Seconds:D2}:{timeSpawn.Milliseconds:D3}";
        }

        /// <returns> 0h:00m:00s </returns>
        public static string FormatToHours(this float time)
        {
            TimeSpan timeSpawn = TimeSpan.FromSeconds(time);
            return $"{(int)timeSpawn.TotalHours}h:{timeSpawn.Minutes:D2}m:{timeSpawn.Seconds:D2}s";
        }

        /// <summary> Add dot every three digits </summary>
        /// <returns> Ex: 1.000.000 </returns>
        public static string FormatToThousands(this int value)
        {
            CultureInfo elGR = CultureInfo.CreateSpecificCulture(Greek);
            return value.ToString("0,0", elGR);
        }
    }
}
