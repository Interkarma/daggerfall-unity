// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Exports texture atlases for terrains.
    /// Provides editor-only methods to export atlases and runtime methods to load them.
    /// Prebuilt atlas is faster to load and uses less memory than a fully runtime equivalent.
    /// This concept may be expanded to support more atlas types in future.
    /// </summary>
    public static class TerrainAtlasBuilder
    {
        // Indices of terrain textures
        const int desertTextureArchive = 2;
        const int desertTextureArchiveWinter = 3;
        const int desertTextureArchiveRain = 4;
        const int mountainTextureArchive = 102;
        const int mountainTextureArchiveWinter = 103;
        const int mountainTextureArchiveRain = 104;
        const int temperateTextureArchive = 302;
        const int temperateTextureArchiveWinter = 303;
        const int temperateTextureArchiveRain = 304;
        const int swampTextureArchive = 402;
        const int swampTextureArchiveWinter = 403;
        const int swampTextureArchiveRain = 404;

        // Filenames to save/load prebuilt textures from Resources
        static string desertTerrainAtlasFilename { get { return "DesertTerrainAtlas"; } }
        static string desertTerrainAtlasWinterFilename { get { return "DesertTerrainAtlasWinter"; } }
        static string desertTerrainAtlasRainFilename { get { return "DesertTerrainAtlasRain"; } }
        static string mountainTerrainAtlasFilename { get { return "MountainTerrainAtlas"; } }
        static string mountainTerrainAtlasWinterFilename { get { return "MountainTerrainAtlasWinter"; } }
        static string mountainTerrainAtlasRainFilename { get { return "MountainTerrainAtlasRain"; } }
        static string temperateTerrainAtlasFilename { get { return "TemperateTerrainAtlas"; } }
        static string temperateTerrainAtlasWinterFilename { get { return "TemperateTerrainAtlasWinter"; } }
        static string temperateTerrainAtlasRainFilename { get { return "TemperateTerrainAtlasRain"; } }
        static string swampTerrainAtlasFilename { get { return "SwampTerrainAtlas"; } }
        static string swampTerrainAtlasWinterFilename { get { return "SwampTerrainAtlasWinter"; } }
        static string swampTerrainAtlasRainFilename { get { return "SwampTerrainAtlasRain"; } }

        /// <summary>
        /// Attempts to load a prebuilt atlas texture.
        /// </summary>
        /// <param name="archive">Terrain archive index </param>
        /// <param name="resourcesPath">Parent Resources folder.</param>
        /// <param name="subPath">Subfolder inside resources.</param>
        /// <returns>Texture2D or null if not found.</returns>
        public static Texture2D LoadTerrainAtlasTextureResource(int archive, string subFolder)
        {
            return Resources.Load<Texture2D>(GetAssetPath(subFolder, GetTerrainAtlasFilename(archive)));
        }

        #region Editor Methods

#if UNITY_EDITOR && !UNITY_WEBPLAYER
        /// <summary>
        /// Exports terrain atlas textures to Resources folder.
        /// </summary>
        public static bool ExportTerrainAtlasTextureResources(TextureReader textureReader, string resourcesPath, string subFolder)
        {
            if (textureReader == null)
            {
                DaggerfallUnity.LogMessage("TerrainAtlasBuilder: textureReader cannot be null.", true);
                return false;
            }

            // Start timing
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            long startTime = stopwatch.ElapsedMilliseconds;
            int totalSteps = 11;
            int stepCount = 0;

            try
            {
                stepCount = UpdateTextureExportStatus(stepCount, totalSteps);
                CreateTerrainAtlasResource(textureReader, resourcesPath, subFolder, desertTextureArchive, desertTerrainAtlasFilename);
                stepCount = UpdateTextureExportStatus(stepCount, totalSteps);
                CreateTerrainAtlasResource(textureReader, resourcesPath, subFolder, desertTextureArchive + 1, desertTerrainAtlasWinterFilename);
                stepCount = UpdateTextureExportStatus(stepCount, totalSteps);
                CreateTerrainAtlasResource(textureReader, resourcesPath, subFolder, desertTextureArchive + 2, desertTerrainAtlasRainFilename);

                stepCount = UpdateTextureExportStatus(stepCount, totalSteps);
                CreateTerrainAtlasResource(textureReader, resourcesPath, subFolder, mountainTextureArchive, mountainTerrainAtlasFilename);
                stepCount = UpdateTextureExportStatus(stepCount, totalSteps);
                CreateTerrainAtlasResource(textureReader, resourcesPath, subFolder, mountainTextureArchive + 1, mountainTerrainAtlasWinterFilename);
                stepCount = UpdateTextureExportStatus(stepCount, totalSteps);
                CreateTerrainAtlasResource(textureReader, resourcesPath, subFolder, mountainTextureArchive + 2, mountainTerrainAtlasRainFilename);

                stepCount = UpdateTextureExportStatus(stepCount, totalSteps);
                CreateTerrainAtlasResource(textureReader, resourcesPath, subFolder, temperateTextureArchive, temperateTerrainAtlasFilename);
                stepCount = UpdateTextureExportStatus(stepCount, totalSteps);
                CreateTerrainAtlasResource(textureReader, resourcesPath, subFolder, temperateTextureArchive + 1, temperateTerrainAtlasWinterFilename);
                stepCount = UpdateTextureExportStatus(stepCount, totalSteps);
                CreateTerrainAtlasResource(textureReader, resourcesPath, subFolder, temperateTextureArchive + 2, temperateTerrainAtlasRainFilename);

                stepCount = UpdateTextureExportStatus(stepCount, totalSteps);
                CreateTerrainAtlasResource(textureReader, resourcesPath, subFolder, swampTextureArchive, swampTerrainAtlasFilename);
                stepCount = UpdateTextureExportStatus(stepCount, totalSteps);
                CreateTerrainAtlasResource(textureReader, resourcesPath, subFolder, swampTextureArchive + 1, swampTerrainAtlasWinterFilename);
                stepCount = UpdateTextureExportStatus(stepCount, totalSteps);
                CreateTerrainAtlasResource(textureReader, resourcesPath, subFolder, swampTextureArchive + 2, swampTerrainAtlasRainFilename);
            }
            catch (Exception ex)
            {
                DaggerfallUnity.LogMessage(string.Format("Export failed with exception '{0}'", ex.Message), true);
            }

            // Show timer and clear status
            long totalTime = stopwatch.ElapsedMilliseconds - startTime;
            DaggerfallUnity.LogMessage(string.Format("Time to create texture assets: {0}ms", totalTime), true);
            EditorUtility.ClearProgressBar();

            return true;
        }

        // Create atlas texture from terrain tiles
        private static void CreateTerrainAtlasResource(TextureReader textureReader, string resourcesPath, string subFolder, int archive, string filename)
        {
            // Load atlas texture and cache properties
            Texture2D texture = textureReader.GetTerrainTilesetTexture(archive, false);
            int maxTextureSize = (texture.width > texture.height) ? texture.width : texture.height;
            int anisoLevel = texture.anisoLevel;
            TextureWrapMode wrapMode = texture.wrapMode;
            float mipMapBias = texture.mipMapBias;
            bool mipmapEnabled = (texture.mipmapCount > 0) ? true : false;

            // Compose paths based on user-specified Resources folder
            string assetFolderPath = Path.Combine(Application.dataPath, resourcesPath);
            assetFolderPath = Path.Combine(assetFolderPath, subFolder);
            string filePath = Path.Combine(assetFolderPath, filename + ".png");

            // Save texture file to user-specified Resources folder
            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes(filePath, pngData);

            // Loading back asset to modify importer properties
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Texture2D assetTexture = Resources.Load<Texture2D>(GetAssetPath(subFolder, filename));
            string assetTexturePath = AssetDatabase.GetAssetPath(assetTexture);

            // Modify asset importer properties
            TextureImporter importer = AssetImporter.GetAtPath(assetTexturePath) as TextureImporter;
            if (importer == null)
            {
                DaggerfallUnity.LogMessage("MaterialReader: Failed to get TextureImporter.", true);
                return;
            }
            importer.maxTextureSize = maxTextureSize;
            importer.anisoLevel = anisoLevel;
            importer.mipMapBias = mipMapBias;
            importer.mipmapEnabled = mipmapEnabled;
            importer.wrapMode = wrapMode;
            importer.isReadable = false;
            importer.textureFormat = TextureImporterFormat.AutomaticCompressed;

            // Reimport asset with new importer settings
            AssetDatabase.ImportAsset(assetTexturePath, ImportAssetOptions.ForceUpdate);

            // Finish up
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static int UpdateTextureExportStatus(int count, int total)
        {
            string status = string.Format("Completed {0}/{1}", count, total);
            EditorUtility.DisplayProgressBar("Creating Texture Assets", status, (float)count / (float)total);
            return count + 1;
        }
#endif

        #endregion

        #region Private Methods

        private static string GetAssetPath(string subFolder, string filename)
        {
            // Compose relative asset path for Unity
            string assetPath = Path.Combine(subFolder, filename);
            assetPath = assetPath.Replace('\\', '/');

            return assetPath;
        }

        private static string GetTerrainAtlasFilename(int archive)
        {
            // Get filename based on archive
            switch (archive)
            {
                // Desert
                case desertTextureArchive:
                    return desertTerrainAtlasFilename;
                case desertTextureArchiveWinter:
                    return desertTerrainAtlasWinterFilename;
                case desertTextureArchiveRain:
                    return desertTerrainAtlasRainFilename;
                // Mountain
                case mountainTextureArchive:
                    return mountainTerrainAtlasFilename;
                case mountainTextureArchiveWinter:
                    return mountainTerrainAtlasWinterFilename;
                case mountainTextureArchiveRain:
                    return mountainTerrainAtlasRainFilename;
                // Temperate
                case temperateTextureArchive:
                    return temperateTerrainAtlasFilename;
                case temperateTextureArchiveWinter:
                    return temperateTerrainAtlasWinterFilename;
                case temperateTextureArchiveRain:
                    return temperateTerrainAtlasRainFilename;
                // Swamp
                case swampTextureArchive:
                    return swampTerrainAtlasFilename;
                case swampTextureArchiveWinter:
                    return swampTerrainAtlasWinterFilename;
                case swampTextureArchiveRain:
                    return swampTerrainAtlasRainFilename;
                // Unknown
                default:
                    return string.Empty;
            }
        }

        #endregion
    }
}