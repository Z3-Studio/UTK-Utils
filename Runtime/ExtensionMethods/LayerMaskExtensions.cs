using UnityEngine;

namespace Z3.Utils.ExtensionMethods
{
    public static class LayerMaskExtensions
    {
        public static bool CompareLayer(this Collider2D collider, LayerMask layerMask)
        {
            return collider.gameObject.layer.CompareLayer(layerMask.value);
        }

        public static bool CompareLayer(this LayerMask layerMask, int standardLayer)
        {
            return standardLayer.CompareLayer(layerMask.value);
        }

        public static bool CompareLayer(this GameObject layerObject, LayerMask layerMask)
        {
            return layerObject.layer.CompareLayer(layerMask.value);
        }

        public static bool CompareLayer(this int standardLayer, LayerMask layerMask)
        {
            return standardLayer.CompareLayer(layerMask.value);
        }

        public static bool CompareLayer(this int standardLayer, int layerMask)
        {
            return (1 << standardLayer & layerMask) != 0;
        }
    }
}