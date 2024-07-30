using UnityEngine;

namespace Z3.Utils
{
    public class ColorUtils
    {
        public static string GetStringFromColor(Color color, bool useAlpha = false)
        {
            string hex = string.Empty;
            hex += FloatNormalizedToHex(color.r);
            hex += FloatNormalizedToHex(color.g);
            hex += FloatNormalizedToHex(color.b);

            if (useAlpha)
            {
                hex += FloatNormalizedToHex(color.a);
            }

            return hex;
        }

        public static string FloatNormalizedToHex(float value)
        {
            return DecToHex(Mathf.RoundToInt(value * 255f));
        }

        public static string DecToHex(int value)
        {
            return value.ToString("X2");
        }

        public static int HexToDec(string hex)
        {
            return System.Convert.ToInt32(hex, 16);
        }

        public static float HexNormalizedToFloat(string hex)
        {
            return HexToDec(hex) / 255f;
        }

        public static Color GetColorFromString(string hex)
        {
            float r = HexNormalizedToFloat(hex.Substring(0, 2));
            float g = HexNormalizedToFloat(hex.Substring(2, 2));
            float b = HexNormalizedToFloat(hex.Substring(4, 2));
            float a = 1f;

            if (hex.Length >= 8)
            {
                a = HexNormalizedToFloat(hex.Substring(6, 2));
            }

            return new Color(r, g, b, a);
        }
    }
}