using UnityEngine;
using Z3.Utils.ExtensionMethods;

namespace Z3.Utils
{
    public static class UnityUtils
    {
        public static void AddComponentIfNeeded<TComponent>(ref TComponent component, GameObject gameObject) where TComponent : Component
        {
            if (!component || component.gameObject != gameObject)
            {
                component = gameObject.GetOrAddComponent<TComponent>();
            }
        }
    }
}