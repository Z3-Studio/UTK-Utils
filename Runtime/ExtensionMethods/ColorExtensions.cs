using UnityEngine;

namespace Z3.Utils.ExtensionMethods
{
    public static class ColorExtensions
    {
        public static Color SetAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        public static Color[] TintColor(this Color[] pixels, Color newColor)
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new Color(newColor.r, newColor.g, newColor.b, pixels[i].a);
            }
            return pixels;
        }
    }
}