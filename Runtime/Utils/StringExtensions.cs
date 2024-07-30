using System;
using UnityEngine;

namespace Z3.Utils.ExtensionMethods
{
    public enum TextAlign
    {
        Left,
        Center,
        Right
    }

    public static class StringExtensions
    {

        public static string GetTypeNiceString(this object obj)
        {
            return obj.GetType().GetTypeNiceString();
        }

        public static string GetTypeNiceString(this Type obj)
        {
            return StringUtils.GetTypeNiceString(obj);
        }

        /// <summary> Similar than UnityEditor.ObjectNames.NicifyVariableName </summary>
        public static string GetNiceString(this string value)
        {
            return StringFormater.GetNiceString(value);
        }

        public static string ToNiceString(this object value)
        {
            return StringFormater.GetNiceString(value.ToString());
        }

        public static T ConvertToEnum<T>(this string value) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        public static string AddRichTextAlign(this string text, TextAlign alignType)
        {
            string align = alignType.ToString().ToLower();
            return $"<align=\"{align}\">{text}</align>";
        }

        public static string AddRichTextColor(this string text, Color color)
        {
            string hex = ColorUtils.GetStringFromColor(color);
            return $"<color=#{hex}>{text}</color>";
        }

        public static string ToStringBold(this object text) => $"<b>{text}</b>";

        public static string ToBold(this string text) => $"<b>{text}</b>";

        public static string ToStringItalic(this object text) => $"<i>{text}</i>";

        public static string ToItalic(this string text) => $"<i>{text}</i>";

        public static string ToStringUnderline(this object text) => $"<u>{text}</u>";

        public static string ToUnderline(this string text) => $"<u>{text}</u>";

        public static bool SearchMatch(this string thisString, string otherString)
        {
            return thisString.ToLower().Contains(otherString.ToLower());
        }

        /// <summary>
        /// Ignore everything before the first '_'
        /// </summary>
        /// <returns>Before: Name_SubName -> After: SubName</returns>
        public static string StringReduction(this string value, char matchCharacter = '_')
        {
            string shortName = string.Empty;
            bool findedUnderscore = false;

            foreach (char c in value)
            {
                if (findedUnderscore)
                {
                    shortName += c;
                }
                else if (c == matchCharacter)
                {
                    findedUnderscore = true;
                }
            }

            if (findedUnderscore)
            {
                return shortName;
            }

            return value;
        }

        /// <summary>
        /// Ignore everything before the first '_'
        /// </summary>
        /// <returns>Before: Name_SubName -> After: SubName</returns>
        public static string StringSimplified(this string value, char matchCharacter = '_')
        {
            string shortName = string.Empty;

            foreach (char c in value)
            {
                if (c == matchCharacter)
                    break;

                shortName += c;
            }

            return shortName;
        }

        public static string LimitMaxCharacters(this string value, int max)
        {
            if (value.Length <= max)
                return value;

            int start = value.Length - max;
            string result = "...";
            for (int i = start; i < value.Length; i++)
            {
                result += value[i];
            }

            return result;
        }

        #region Float
        public static float ToFloat(this string input, float defaultValue = default)
        {
            return ToNullableFloat(input, defaultValue).Value;
        }

        public static float? ToNullableFloat(this string input, float? defaultValue = null)
        {
            if (float.TryParse(input, out float output))
                return output;

            return defaultValue;
        }

        public static bool TryParseToFloat(this string input, out float value)
        {
            float? nullableValue = ToNullableFloatCultureInvariant(input);

            value = default;
            if (nullableValue.HasValue)
            {
                value = nullableValue.Value;
            }

            return nullableValue.HasValue;
        }

        public static float? ToNullableFloatCultureInvariant(this string input, float? defaultValue = null)
        {
            if (string.IsNullOrWhiteSpace(input))
                return defaultValue;

            if (float.TryParse(input.Replace(".", ","), out float output))
                return output;

            return defaultValue;
        }

        public static float ToFloatCultureInvariant(this string input, float defaultValue = default)
        {
            return ToNullableFloatCultureInvariant(input, defaultValue).Value;
        }
        #endregion

        #region Int
        public static int ToInt(this string input, int defaultValue = default)
        {
            return ToNullableInt(input, defaultValue).Value;
        }

        public static int? ToNullableInt(this string input, int? defaultValue = null)
        {
            if (int.TryParse(input, out int output))
                return output;

            return defaultValue;
        }
        #endregion

        #region Bool
        public static bool ToBool(this string input, bool defaultValue = default)
        {
            return ToNullableBool(input, defaultValue).Value;
        }

        public static bool? ToNullableBool(this string input, bool? defaultValue = default)
        {
            if (bool.TryParse(input, out bool output))
                return output;

            return defaultValue;
        }
        #endregion
    }
}