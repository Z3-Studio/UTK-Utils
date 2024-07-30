#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Z3.Utils.ExtensionMethods
{
    public static class HandleExtensions
    {
        #region Public
        public static void DrawArc(this Transform transform, float openingAngle, float distance, Color color)
        {
            Vector3 normal = GetArcDirection(transform, openingAngle);
            DrawArc(transform.position, transform.up, normal, openingAngle, distance, color);
        }

        public static void DrawArc(this Transform transform, Vector3 direction, float openingAngle, float distance, Color color)
        {
            Vector3 normal = GetArcDirection(Vector3.forward, direction, openingAngle);
            DrawArc(transform.position, Vector3.forward, normal, openingAngle, distance, color);
        }

        public static void DrawWireArc(this Transform transform, float openingAngle, float distance, Color color)
        {
            Vector3 normal = GetArcDirection(transform, openingAngle);
            DrawWireArc(transform.position, transform.forward, normal, openingAngle, distance, color);
        }
        #endregion

        private static void DrawWireArc(Vector3 position, Vector3 axis, Vector3 normal, float openingAngle, float distance, Color color)
        {
#if UNITY_EDITOR
            Color lastHandlesColor = Handles.color;
            Handles.color = color;
            Handles.DrawWireArc(position, axis, normal, openingAngle, distance);
            Handles.color = lastHandlesColor;
#endif
        }

        private static void DrawArc(Vector3 position, Vector3 axis, Vector3 normal, float openingAngle, float distance, Color color)
        {
#if UNITY_EDITOR
            Color lastHandlesColor = Handles.color;
            Handles.color = color;
            Handles.DrawSolidArc(position, axis, normal, openingAngle, distance);
            Handles.color = lastHandlesColor;
#endif
        }

        private static Vector3 GetArcDirection(Transform transform, float openingAngle)
        {
            return GetArcDirection(transform.up, transform.forward, openingAngle);
        }

        private static Vector3 GetArcDirection(Vector3 axis, Vector3 direction, float openingAngle)
        {
            Quaternion rot = Quaternion.AngleAxis(-openingAngle / 2, axis);
            return rot * direction;
        }
    }
}