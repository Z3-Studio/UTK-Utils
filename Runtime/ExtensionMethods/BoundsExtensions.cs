using UnityEngine;

namespace Z3.Utils.ExtensionMethods
{
    public static class BoundsExtensions
    {
        // Z Front
        public static Vector3 RightTopFront(this Bounds bounds) => bounds.max;
        public static Vector3 CenterTopFront(this Bounds bounds) => new(bounds.center.x, bounds.max.y, bounds.max.z);
        public static Vector3 LeftTopFront(this Bounds bounds) => new(bounds.min.x, bounds.max.y, bounds.max.z);

        public static Vector3 RightCenterFront(this Bounds bounds) => new(bounds.max.x, bounds.center.y, bounds.max.z);
        public static Vector3 CenterCenterFront(this Bounds bounds) => new(bounds.center.x, bounds.center.y, bounds.max.z);
        public static Vector3 LeftCenterFront(this Bounds bounds) => new(bounds.min.x, bounds.center.y, bounds.max.z);

        public static Vector3 RightBottomFront(this Bounds bounds) => new(bounds.max.x, bounds.min.y, bounds.max.z);
        public static Vector3 CenterBottomFront(this Bounds bounds) => new(bounds.center.x, bounds.min.y, bounds.max.z);
        public static Vector3 LeftBottomFront(this Bounds bounds) => new(bounds.min.x, bounds.min.y, bounds.max.z);


        // Z Center
        public static Vector3 RightTopCenter(this Bounds bounds) => new(bounds.max.x, bounds.max.y, bounds.center.z);
        public static Vector3 CenterTopCenter(this Bounds bounds) => new(bounds.center.x, bounds.max.y, bounds.center.z);
        public static Vector3 LeftTopCenter(this Bounds bounds) => new(bounds.min.x, bounds.max.y, bounds.center.z);

        public static Vector3 RightCenterCenter(this Bounds bounds) => new(bounds.max.x, bounds.center.y, bounds.center.z);
        public static Vector3 CenterCenterCenter(this Bounds bounds) => bounds.center;
        public static Vector3 LeftCenterCenter(this Bounds bounds) => new(bounds.min.x, bounds.center.y, bounds.center.z);

        public static Vector3 RightBottomCenter(this Bounds bounds) => new(bounds.max.x, bounds.min.y, bounds.center.z);
        public static Vector3 CenterBottomCenter(this Bounds bounds) => new(bounds.center.x, bounds.min.y, bounds.center.z);
        public static Vector3 LeftBottomCenter(this Bounds bounds) => new(bounds.min.x, bounds.min.y, bounds.center.z);


        // Z Back
        public static Vector3 RightTopBack(this Bounds bounds) => new(bounds.max.x, bounds.max.y, bounds.min.z);
        public static Vector3 CenterTopBack(this Bounds bounds) => new(bounds.center.x, bounds.max.y, bounds.min.z);
        public static Vector3 LeftTopBack(this Bounds bounds) => new(bounds.min.x, bounds.max.y, bounds.min.z);

        public static Vector3 RightCenterBack(this Bounds bounds) => new(bounds.max.x, bounds.center.y, bounds.min.z);
        public static Vector3 CenterCenterBack(this Bounds bounds) => new(bounds.center.x, bounds.center.y, bounds.min.z);
        public static Vector3 LeftCenterBack(this Bounds bounds) => new(bounds.min.x, bounds.center.y, bounds.min.z);

        public static Vector3 RightBottomBack(this Bounds bounds) => new(bounds.max.x, bounds.min.y, bounds.min.z);
        public static Vector3 CenterBottomBack(this Bounds bounds) => new(bounds.center.x, bounds.min.y, bounds.min.z);
        public static Vector3 LeftBottomBack(this Bounds bounds) => bounds.min;
    }
}