using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Z3.Utils.ExtensionMethods
{
    /// <summary>
    /// General Extensions
    /// </summary>
    public static class UnityExtensions
    {
        public static T CloneT<T>(this T node) where T : Object
        {
            return Object.Instantiate(node);
        }

        public static bool ObjectNullCheck(this object target)
        {
            return target == null || (target is Object obj && !obj);
        }

        public static void DoActionNextFrame(this MonoBehaviour monoBehaviour, Action action) // Prevents Event system bugs
        {
            monoBehaviour.StartCoroutine(CallNextFrame(action));
        }

        /// <summary> Prevents Event system bugs </summary>
        public static void SelectWithDelay(this Selectable selectable)
        {
            selectable.StartCoroutine(CallNextFrame(selectable.Select));
        }

        /// <summary> Prevents Event system bugs </summary>
        public static void SelectWithDelay(this MonoBehaviour monoBehaviour, GameObject gameObject)
        {
            monoBehaviour.StartCoroutine(CallNextFrame(() => EventSystem.current.SetSelectedGameObject(gameObject)));
        }

        private static IEnumerator CallNextFrame(Action action)
        {
            yield return new WaitForEndOfFrame();
            action();
        }

        public static int ToIntLayer(this LayerMask layer) => (int)Mathf.Log(layer.value, 2f);

        public static bool IsPointInsideBounds(this Vector3 point, Vector3 center, Vector3 size)
        {
            Bounds bounds = new Bounds(center, size);
            return bounds.Contains(point);
        }

        public static bool TryGetComponentFromRigidbody<T>(this Collision collider, out T component)
        {
            component = default;
            return collider.rigidbody && collider.rigidbody.TryGetComponent(out component);
        }

        public static bool TryGetComponentFromRigidbody<T>(this Collider collider, out T component)
        {
            component = default;
            return collider.attachedRigidbody && collider.attachedRigidbody.TryGetComponent(out component);
        }

        public static void RebuildLayout(this MonoBehaviour monoBehaviour)
        {
            monoBehaviour.RebuildLayout(monoBehaviour.transform as RectTransform);
        }

        public static void RebuildLayout(this MonoBehaviour monoBehaviour, RectTransform rectTransform)
        {
            monoBehaviour.StartCoroutine(RebuildLayout(rectTransform));
        }

        private static IEnumerator RebuildLayout(RectTransform rectTransform)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            yield return new WaitForEndOfFrame();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
        /*
        public static void RebuildLayoutAfterFixedUpdate(this RectTransform rectTransform, Action callback = null)
        {
            UniTask.Create(() => RebuildLayoutAsync(rectTransform, callback));
        }

        private static async UniTask RebuildLayoutAsync(RectTransform rectTransform, Action callback)
        {
            await UniTask.WaitForFixedUpdate();
            await UniTask.WaitForFixedUpdate();

            if (rectTransform != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }

            callback?.Invoke();
        }*/
    }
}