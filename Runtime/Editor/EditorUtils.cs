using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Z3.Utils.Editor
{
    public static class EditorUtils
    {
        /// <param name="relativePath"> Example: Assets/MyFolder/MyAsset.asset </param>
        public static T LoadOrCreateAsset<T>(string relativePath) where T : ScriptableObject
        {
#if UNITY_EDITOR
            T obj = AssetDatabase.LoadAssetAtPath<T>(relativePath);

            if (obj == null)
            {
                string fullpath = Path.GetFullPath(relativePath);
                string directory = Path.GetDirectoryName(fullpath);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                obj = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(obj, relativePath);
            }

            return obj;
#else
            throw EditorOperationExpection();
#endif
        }

        public static ScriptableObject CreateAssetInProject(Type type)
        {
#if UNITY_EDITOR
            string path = EditorUtility.SaveFilePanelInProject($"Create Asset of type {type}", $"{type.Name}.asset", "asset", string.Empty);

            if (string.IsNullOrEmpty(path))
                return null;

            ScriptableObject data = ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(data, path);
            AssetDatabase.SaveAssets();
            return data;
#else
            throw EditorOperationExpection();
#endif
        }

        public static InvalidOperationException EditorOperationExpection() => new InvalidOperationException("This operation is only valid in the editor");

        public static List<T> GetAllAssets<T>() where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            Type type = typeof(T);
            List<T> assets = new List<T>();
            string[] assetPaths = AssetDatabase.FindAssets($"t:{type.Name}");

            foreach (string assetPath in assetPaths)
            {
                string fullPath = AssetDatabase.GUIDToAssetPath(assetPath);
                T asset = AssetDatabase.LoadAssetAtPath<T>(fullPath);
                assets.Add(asset);
            }

            return assets;
#else
            throw EditorOperationExpection();
#endif
        }
    }
}