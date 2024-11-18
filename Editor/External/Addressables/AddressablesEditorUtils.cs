// asmdef Version Defines, enabled when com.unity.addressables is imported.

#if Z3_ADDRESSABLE_SUPPORT

using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using Z3.Utils.Editor.ExtensionMethods;


namespace Z3.Utils.Editor.Addressables
{
    public static class AddressablesEditorUtils
    {
        private const AddressableAssetSettings.ModificationEvent ModificationEvent = AddressableAssetSettings.ModificationEvent.EntryMoved;

        public static bool IsAddressable(Object asset)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetEntry entry = settings.FindAssetEntry(asset.GetAssetGuid());
            return entry != null;
        }

        public static string GetAddress(Object asset)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetEntry entry = settings.FindAssetEntry(asset.GetAssetGuid());
            return entry.address;
        }

        public static void SetupAsset(Object asset, string address, string groupName, string label)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetGroup roomsGroup = settings.FindGroup(groupName);

            string assetGuid = asset.GetAssetGuid();
            AddressableAssetEntry assetEntry = settings.CreateOrMoveEntry(assetGuid, roomsGroup);
            assetEntry.address = address;
            assetEntry.labels.Add(label);

            settings.SetDirty(ModificationEvent, assetEntry, true);
            AssetDatabase.SaveAssets();
        }

        public static void UpdateAsset(Object asset, string address, string groupName, string label)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetGroup roomsGroup = settings.FindGroup(groupName);

            string assetGuid = asset.GetAssetGuid();
            AddressableAssetEntry assetEntry = settings.FindAssetEntry(assetGuid, roomsGroup);

            if (assetEntry == null)
                SetupAsset(asset, address, groupName, label);

            assetEntry.address = address;
            assetEntry.labels.Clear();
            assetEntry.labels.Add(label);

            settings.SetDirty(ModificationEvent, assetEntry, true);
            AssetDatabase.SaveAssets();
        }
    }
}

#endif