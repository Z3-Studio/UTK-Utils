using UnityEngine;

namespace Z3.Utils.ExtensionMethods
{
    public static class IntExtensions
    {
        public static int Remap(this int value, float minIn, float maxIn, float minOut, float maxOut)
        {
            float result = (value - minIn) / (maxIn - minIn) * (maxOut - minOut) + minOut;
            return Mathf.RoundToInt(result);
        }

        public static Vector2 ToVector(this int value) => new Vector2(value, value);

        public static Vector2Int ToVectorInt(this int value) => new Vector2Int(value, value);

        public static int Navigate(this int value, int length, bool goRight)
        {
            if (goRight)
            {
                value++;
                if (value == length)
                    value = 0;
            }
            else
            {
                value--;
                if (value < 0)
                    value = length - 1;
            }
            return value;
        }
    }
}