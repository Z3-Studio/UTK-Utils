using UnityEngine;
using Z3.Utils.ExtensionMethods;

namespace Z3.Utils
{
    public class VectorUtils
    {
        public static Vector2Int Clamp(Vector2Int original, int xMax, int yMax)
        {
            return Clamp(original, Vector2Int.up * xMax, Vector2Int.up * yMax);
        }

        public static Vector2Int Clamp(Vector2Int original, Vector2Int xRange, Vector2Int yRange)
        {
            int x = Mathf.Clamp(original.x, xRange.x, xRange.y);
            int y = Mathf.Clamp(original.y, yRange.x, yRange.y);
            return new Vector2Int(x, y);
        }

        public static Vector2Int Invert(Vector2Int original, bool inverse = true)
        {
            return inverse ? new Vector2Int(original.y, original.x) : original;
        }

        public static Vector2 SmoothStep(Vector2 start, Vector2 end, float t)
        {
            return SmoothStep(start.ToVector3(), end.ToVector3(), t);
        }

        public static Vector3 SmoothStep(Vector3 start, Vector3 end, float t)
        {
            float x = Mathf.SmoothStep(start.x, end.x, t);
            float y = Mathf.SmoothStep(start.y, end.y, t);
            float z = Mathf.SmoothStep(start.z, end.z, t);
            return new Vector3(x, y, z);
        }
    }
}