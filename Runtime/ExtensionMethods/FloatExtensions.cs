using UnityEngine;

namespace Z3.Utils.ExtensionMethods
{
    public static class FloatExtensions
    {
        /// <returns> Return a value between -180 to 180 </returns>
        public static float NormalizeAngle(this float angle)
        {
            angle %= 360;
            return angle > 180 ? angle - 360 : angle;
        }

        /// <returns> Return a value between 0 to 360 </returns>
        public static float NormalizeAngle360(this float angle)
        {
            angle %= 360;
            return angle < 0 ? angle + 360 : angle;
        }

        public static float Remap(this float value, float minIn, float maxIn, float minOut, float maxOut)
        {
            return (value - minIn) / (maxIn - minIn) * (maxOut - minOut) + minOut;
        }

        public static bool IsEqualTo(this float originValue, float value)
        {
            return Mathf.Abs(originValue - value) < Mathf.Epsilon;
        }

        public static Vector2 ToVector2(this float value) => new Vector2(value, value);

        public static Vector2 ToVector3(this float value) => new Vector3(value, value, value);

        public static string ToDecimalString(this float input, string format = "")
        {
            return ((decimal)input).ToString(format).Replace(",", ".");
        }

        public static string ToDecimalString(this float? input, string format = "")
        {
            return input.HasValue ? input.Value.ToDecimalString(format) : string.Empty;
        }

        public static string ToSafeString(this float? input, string format = "")
        {
            return input.HasValue ? input.Value.ToString(format) : string.Empty;
        }
    }
}