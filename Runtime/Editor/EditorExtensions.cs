using System;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Z3.Utils.Editor
{
    public static class EditorExtensions
    {
        public static string GetAssetPath(this Object obj)
        {
#if UNITY_EDITOR
            return AssetDatabase.GetAssetPath(obj);
#else
            throw EditorOperationExpection();
#endif
        }

        private static InvalidOperationException EditorOperationExpection() => EditorUtils.EditorOperationExpection();
    }
}