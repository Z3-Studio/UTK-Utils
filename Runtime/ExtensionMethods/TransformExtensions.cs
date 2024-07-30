using UnityEngine;

namespace Z3.Utils.ExtensionMethods
{
    public static class TransformExtensions
    {
        public static void LookAtY(this Transform transform, Vector3 target)
        {
            Vector3 worldPosition = new Vector3(target.x, transform.position.y, target.z);
            transform.LookAt(worldPosition);
        }

        public static void LookAtX(this Transform transform, Vector3 target)
        {
            Vector3 worldPosition = new Vector3(transform.position.x, target.y, target.z);
            transform.LookAt(worldPosition);
        }

        public static Vector2 GetRealSize(this RectTransform rectTransform)
        {
            float width = Mathf.Abs(rectTransform.GetRealWidth());
            float height = Mathf.Abs(rectTransform.GetRealHeight());
            return new Vector2(width, height);
        }

        public static float GetRealWidth(this RectTransform rectTransform)
        {
            Vector3[] corners = rectTransform.GetWorldCorners();
            Vector3 leftBottom = corners[0];
            Vector3 rightTop = corners[2];
            return rightTop.x - leftBottom.x;
        }

        public static float GetRealHeight(this RectTransform rectTransform)
        {
            Vector3[] corners = rectTransform.GetWorldCorners();
            Vector3 leftBottom = corners[0];
            Vector3 rightTop = corners[2];
            return rightTop.y - leftBottom.y;
        }

        public static void SetWorldSize(this RectTransform rectTransform, Vector2 size)
        {
            Vector2 realSize = rectTransform.GetRealSize();
            Vector2 scaleMultiplier = size / realSize;
            Vector2 canvasSize = rectTransform.rect.size * scaleMultiplier;
            rectTransform.SetCanvasSize(canvasSize);
        }

        public static void SetCanvasSize(this RectTransform rectTransform, Vector2 size)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        }

        public static void ScaleAround(this Transform target, Vector3 pivot, Vector3 newScale)
        {
            Vector3 targetPosition = target.position;
            Vector3 difference = targetPosition - pivot;

            float relativeScale = newScale.x / target.localScale.x;
            Vector3 finalPosition = pivot + difference * relativeScale;

            target.localScale = newScale;
            target.position = finalPosition;
        }

        public static Vector3[] GetWorldCorners(this RectTransform rect)
        {
            Vector3[] corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            return corners;
        }
    }
}