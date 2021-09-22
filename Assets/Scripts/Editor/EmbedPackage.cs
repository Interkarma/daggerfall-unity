using UnityEditor.PackageManager;
using UnityEngine;

namespace UnityEditor.Extensions
{
#if UNITY_2017_3_OR_NEWER

    /// <summary>
    /// Editor extension for embedding packages as a local copy in the project.
    /// This can be useful in case you want to modify the package's source code.
    /// </summary>
    public static class EmbedPackage
    {
        [MenuItem("Assets/Embed Package", false, 1000000)]
        private static void EmbedPackageMenuItem()
        {
            var packageName = System.IO.Path.GetFileName(GetSelectionPath());

            Debug.Log($"Embedding package '{packageName}' into the project.");

            Client.Embed(packageName);

            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Embed Package", true)]
        private static bool EmbedPackageValidation()
        {
            var path = GetSelectionPath();
            var folder = System.IO.Path.GetDirectoryName(path);

            // We only deal with direct folders under Packages/
            return folder == "Packages";
        }

        private static string GetSelectionPath()
        {
            if (Selection.assetGUIDs.Length == 0)
                return "";

            string clickedAssetGuid = Selection.assetGUIDs[0];
            string clickedPath = AssetDatabase.GUIDToAssetPath(clickedAssetGuid);
            return clickedPath;
        }
    }

#endif
}