using UnityEngine;
using Z3.Utils.Editor;

namespace Z3.Utils
{
    public class Z3Path
    {
        public const string ScriptableObjects = "Z3/";

        public const string MenuPath = "Z3/";
        public const string UiBuilderMenuPath = MenuPath + "UI Builder/";

        // Used in methods
        public const string Resources = "Assets/Plugins/Z3";

        public const string PackageCompanyName = "com.z3";

        public static T LoadAssetPath<T>(string subModule = "UIBuilder") where T : ScriptableObject
        {
            string path = $"{Resources}/{subModule}/{typeof(T).Name}.asset";
            return EditorUtils.LoadOrCreateAsset<T>(path);
        }
    }
}