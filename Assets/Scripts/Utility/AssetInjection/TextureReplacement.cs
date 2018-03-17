// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:
// 
// Notes:
//

/*
 * TODO:
 * - PaperDoll CharacterLayer textures works only if resolution is the same as vanilla 
 *        (http://forums.dfworkshop.net/viewtopic.php?f=22&p=3547&sid=6a99dbcffad1a15b08dd5e157274b772#p3547)
 * - Import textures/materials from mods
 */

//#define DEBUG_TEXTURE_FORMAT

using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Utility.ModSupport;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Textures maps.
    /// </summary>
    public enum TextureMap
    {
        Albedo,
        Normal,
        Emission,
        MetallicGloss,
        Heightmap,
        Occlusion
    }

    /// <summary>
    /// Handles import and injection of custom textures and images
    /// with the purpose of providing modding support.
    /// </summary>
    static public class TextureReplacement
    {
        #region Fields & Structs

        const string extension = ".png";

        // Paths
        static readonly string texturesPath = Path.Combine(Application.streamingAssetsPath, "Textures");
        static readonly string imgPath = Path.Combine(texturesPath, "Img");
        static readonly string cifRciPath = Path.Combine(texturesPath, "CifRci");

        /// <summary>
        /// Common tags for textures maps.
        /// </summary>
        public struct MapTags
        {
            public const string Normal = "_Normal";
            public const string Emission = "_Emission";
            public const string MetallicGloss = "_MetallicGloss";
            public const string Heightmap = "_Heightmap"; //unused
            public const string Occlusion = "_Occlusion"; //unused
        }

        /// <summary>
        /// Material components and settings for custom billboards.
        /// </summary>
        public struct CustomBillboard
        {
            public bool isCustomAnimated;                // Is custom and animated?
            public List<Texture2D> MainTexture;          // List of custom albedo maps
            public List<Texture2D> EmissionMap;          // List of custom emission maps
            public bool isEmissive;                      // True if billboard is emissive
            public int NumberOfFrames;                   // Number of frames
        }

        // Enemy will use custom textures if [archive, enemyDefaultRecord, enemyDefaultFrame] is found.
        const int enemyDefaultRecord = 0;          
        const int enemyDefaultFrame = 0;

        /// <summary>
        /// Custom textures for enemies.
        /// </summary>
        public struct CustomEnemyMaterial
        {
            public bool isCustom;                        // True if enemy uses custom textures
            public List<List<Texture2D>> MainTexture;    // Textures
        }

        #endregion

        #region Properties

        static TextureFormat TextureFormat
        {
            get
            {
                return DaggerfallUnity.Instance.MaterialReader.CompressModdedTextures ?
                    TextureFormat.DXT5 :
                    TextureFormat.ARGB32;
            }
        }

        /// <summary>
        /// Path to custom textures on disk.
        /// </summary>
        static public string TexturesPath
        {
            get { return texturesPath; }
        }

        /// <summary>
        /// Path to custom images on disk.
        /// </summary>
        static public string ImagesPath
        {
            get { return imgPath; }
        }

        /// <summary>
        /// Path to custom Cif and Rci files on disk.
        /// </summary>
        static public string CifRciPath
        {
            get { return cifRciPath; }
        }

        #endregion

        #region Textures Import

        /// <summary>
        /// Search for image files on disk to use as textures on models or billboards
        /// (archive_record-frame.png, for example '86_3-0.png').
        /// </summary>
        /// <param name="archive">Archive index from TEXTURE.XXX</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for animated billboards.</param>
        /// <returns>True if texture exists.</returns>
        static public bool CustomTextureExist(int archive, int record, int frame = 0)
        {
            return TextureFileExist(texturesPath, GetName(archive, record, frame));
        }

        /// <summary>
        /// Search for image files on disk to use as textures on models or billboards
        /// (name.png).
        /// </summary>
        /// <param name="name">Name of texture without extension.</param>
        /// <returns>True if texture exists.</returns>
        static public bool CustomTextureExist(string name)
        {
            return TextureFileExist(texturesPath, name);
        }

        /// <summary>
        /// Import image from disk as texture2D
        /// (archive_record-frame.png, for example '86_3-0.png').
        /// </summary>
        /// <param name="archive">Archive index from TEXTURE.XXX</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for animated billboards</param>
        /// <returns>Texture.</returns>
        static public Texture2D LoadCustomTexture(int archive, int record, int frame)
        {
            return ImportTextureFile(texturesPath, GetName(archive, record, frame), true);
        }

        /// <summary>
        /// Import image from disk as texture2D
        /// (name.png).
        /// </summary>
        /// <param name="name">Name of texture without extension.</param>
        /// <returns>Texture.</returns>
        static public Texture2D LoadCustomTexture(string name)
        {
            return ImportTextureFile(texturesPath, name, true);
        }

        /// <summary>
        /// Seek animated texture from mods with all frames.
        /// </summary>
        /// <param name="archive">Texture archive.</param>
        /// <param name="record">Record index.</param>
        /// <param name="texFrames">Imported texture frames.</param>
        /// <returns>True if texture imported.</returns>
        public static bool TryImportTexture(int archive, int record, out Texture2D[] texFrames)
        {
            return TryImportTexture(texturesPath, frame => GetName(archive, record, frame), out texFrames);
        }

        /// <summary>
        /// Seek texture from mods.
        /// </summary>
        /// <param name="archive">Texture archive.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Animation frame index</param>
        /// <param name="tex">Imported texture.</param>
        /// <returns>True if texture imported.</returns>
        public static bool TryImportTexture(int archive, int record, int frame, out Texture2D tex)
        {
            return TryImportTexture(texturesPath, GetName(archive, record, frame), out tex);
        }

        /// <summary>
        /// Seek texture from mods with a specific dye.
        /// </summary>
        /// <param name="archive">Texture archive.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Animation frame index</param>
        /// <param name="dye">Dye colour for armour, weapons, and clothing.</param>
        /// <param name="tex">Imported texture.</param>
        /// <returns>True if texture imported.</returns>
        public static bool TryImportTexture(int archive, int record, int frame, DyeColors dye, out Texture2D tex)
        {
            return TryImportTexture(texturesPath, GetName(archive, record, frame, dye), out tex);
        }

        /// <summary>
        /// Seek texture from mods.
        /// </summary>
        /// <param name="name">Texture name.</param>
        /// <param name="tex">Imported texture.</param>
        /// <returns>True if texture imported.</returns>
        public static bool TryImportTexture(string name, out Texture2D tex)
        {
            return TryImportTexture(texturesPath, name, out tex);
        }

        /// <summary>
        /// Seek image from mods.
        /// </summary>
        /// <param name="name">Image name.</param>
        /// <param name="tex">Imported image as texture.</param>
        /// <returns>True if image imported.</returns>
        public static bool TryImportImage(string name, out Texture2D tex)
        {
            return TryImportTexture(imgPath, name, out tex);
        }

        /// <summary>
        /// Seek CifRci from mods.
        /// </summary>
        /// <param name="name">Image name.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Animation frame index</param>
        /// <param name="tex">Imported image as texture.</param>
        /// <returns>True if CifRci imported.</returns>
        public static bool TryImportCifRci(string name, int record, int frame, out Texture2D tex)
        {
            return TryImportTexture(cifRciPath, GetNameCifRci(name, record, frame), out tex);
        }

        /// <summary>
        /// Seek CifRci with a specific metaltype from mods.
        /// </summary>
        /// <param name="name">Image name.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Animation frame index</param>
        /// <param name="metalType">Metal type.</param>
        /// <param name="tex">Imported image as texture.</param>
        /// <returns>True if CifRci imported.</returns>
        public static bool TryImportCifRci(string name, int record, int frame, MetalTypes metalType, out Texture2D tex)
        {
            return TryImportTexture(cifRciPath, GetNameCifRci(name, record, frame, metalType), out tex);
        }

        /// <summary>
        /// Search for image file on disk to use as normal map
        /// (archive_record-frame_Normal.png, for example '112_3-0_Normal.png').
        /// </summary>
        /// <param name="archive">Archive index from TEXTURE.XXX</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index.</param>
        /// <returns>True if normal map exists.</returns>
        static public bool CustomNormalExist(int archive, int record, int frame)
        {
            return TextureFileExist(texturesPath, GetName(archive, record, frame) + MapTags.Normal);
        }

        /// <summary>
        /// Search for image file on disk to use as normal map
        /// (name_Normal.png).
        /// </summary>
        /// <param name="name">Name of texture.</param>
        /// <returns>True if normal map exists.</returns>
        static public bool CustomNormalExist(string name)
        {
            return TextureFileExist(texturesPath, name + MapTags.Normal);
        }

        /// <summary>
        /// Import image file from disk to use as normal map.
        /// (archive_record-frame_Normal.png, for example '112_3-0_Normal.png').
        /// </summary>
        /// <param name="archive">Archive index from TEXTURE.XXX</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index.</param> 
        /// <returns>Normal map.</returns>
        static public Texture2D LoadCustomNormal(int archive, int record, int frame)
        {
            return ImportNormalMap(texturesPath, GetName(archive, record, frame) + MapTags.Normal);
        }

        /// <summary>
        /// Import image file from disk to use as normal map
        /// (name_Normal.png).
        /// </summary>
        /// <param name="name">Name of texture.</param>
        /// <returns>Normal map.</returns>
        static public Texture2D LoadCustomNormal(string name)
        {
            return ImportNormalMap(texturesPath, name + MapTags.Normal);
        }

        /// <summary>
        /// Search for image file on disk to use as emission map
        /// (archive_record-frame_Emission.png, for example '112_3-0_Emission.png).
        /// </summary>
        /// <param name="archive">Archive index from TEXTURE.XXX</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for animated billboards.</param>
        /// <returns>True if emission map exists.</returns>
        static public bool CustomEmissionExist(int archive, int record, int frame)
        {
            return TextureFileExist(texturesPath, GetName(archive, record, frame) + MapTags.Emission);
        }

        /// <summary>
        /// Search for image file on disk to use as emission map
        /// (name_Emission.png)
        /// </summary>
        /// <param name="name">Name of texture.</param>
        /// <returns>True if emission map exists.</returns>
        static public bool CustomEmissionExist(string name)
        {
            return TextureFileExist(texturesPath, name + MapTags.Emission);
        }

        /// <summary>
        /// Import image file from disk to use as emission map
        /// (archive_record-frame_Emission.png, for example '112_3-0_Emission.png').
        /// </summary>
        /// <param name="archive">Archive index from TEXTURE.XXX</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for animated billboards</param>
        /// <returns>Emission map.</returns>
        static public Texture2D LoadCustomEmission(int archive, int record, int frame)
        {
            return ImportTextureFile(texturesPath, GetName(archive, record, frame) + MapTags.Emission, true);
        }

        /// <summary>
        /// Import image file from disk to use as emission map
        /// (name_Emission.png)
        /// </summary>
        /// <param name="name">Name of texture.</param>
        /// <returns>Emission map.</returns>
        static public Texture2D LoadCustomEmission(string name)
        {
            return ImportTextureFile(texturesPath, name + MapTags.Emission, true);
        }

        /// <summary>
        /// Search for image file on disk to use as metallic map
        /// (archive_record-frame_MetallicGloss.png).
        /// </summary>
        /// <param name="archive">Archive index from TEXTURE.XXX</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index.</param> 
        /// <returns>True if MetallicGloss map exist.</returns>
        static public bool CustomMetallicGlossExist(int archive, int record, int frame)
        {
            return TextureFileExist(texturesPath, GetName(archive, record, frame) + MapTags.MetallicGloss);
        }

        /// <summary>
        /// Search for image file on disk to use as metallic map
        /// (name_MetallicGloss.png).
        /// </summary>
        /// <param name="name">Name of texture.</param> 
        /// <returns>True if MetallicGloss map exist.</returns>
        static public bool CustomMetallicGlossExist(string name)
        {
            return TextureFileExist(texturesPath, name + MapTags.MetallicGloss);
        }

        /// <summary>
        /// Import image file from disk to use as metallic map.
        /// (archive_record-frame_MetallicGloss.png).
        /// </summary>
        /// <param name="archive">Archive index from TEXTURE.XXX</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index.</param> 
        /// <returns>MetallicGloss map.</returns>
        static public Texture2D LoadCustomMetallicGloss(int archive, int record, int frame)
        {
            return ImportTextureFile(texturesPath, GetName(archive, record, frame) + MapTags.MetallicGloss, true);
        }

        /// <summary>
        /// Import image file from disk to use as MetallicGloss map
        /// (name_MetallicGloss.png).
        /// </summary>
        /// <param name="name">Name of texture.</param>
        /// <returns>MetallicGloss map.</returns>
        static public Texture2D LoadCustomMetallicGloss(string name)
        {
            return ImportTextureFile(texturesPath, name + MapTags.MetallicGloss, true);
        }

        // General import methods for mods or other external code.

        /// <summary>
        /// Import png file from disk as Texture2D.
        /// </summary>
        /// <param name="path">Path where image file is located.</param>
        /// <param name="name">Name of image file without extension.</param>
        /// <param name="texture">Texture.</param>
        /// <param name="mapTag">Texture map.</param>
        static public bool ImportTextureFromDisk (string path, string name, out Texture2D texture, TextureMap map = TextureMap.Albedo)
        {
            if (File.Exists(Path.Combine(path, name + ".png")))
            {
                switch (map)
                {
                    case TextureMap.Normal:
                        texture = ImportNormalMap(path, name);
                        break;

                    default:
                        texture = ImportTextureFile(path, name, true);
                        break;
                }

                return true;
            }

            texture = null;
            return false;
        }

        #endregion

        #region Texture Injection

        /// <summary>
        /// Import additional custom components of material.
        /// </summary>
        /// <param name="archive">Archive index</param>
        /// <param name="record">Record index</param>
        /// <param name="frame">Texture frame</param>
        /// <param name="material">Material.</param>
        static public void CustomizeMaterial(int archive, int record, int frame, Material material)
        {
            // MetallicGloss map
            if (CustomMetallicGlossExist(archive, record, frame))
            {
                material.EnableKeyword("_METALLICGLOSSMAP");
                material.SetTexture("_MetallicGlossMap", LoadCustomMetallicGloss(archive, record, frame));
            }

            // Properties
            string path = Path.Combine(texturesPath, GetName(archive, record, frame));
            if (XMLManager.XmlFileExists(path))
            {
                var xml = new XMLManager(path);

                // Metallic parameter
                float metallic;
                if (xml.TryGetFloat("metallic", out metallic))
                    material.SetFloat("_Metallic", metallic);

                // Smoothness parameter
                float smoothness;
                if (xml.TryGetFloat("smoothness", out smoothness))
                    material.SetFloat("_Glossiness", smoothness);
            }
        }

        /// <summary>
        /// Set custom material on billboard gameobject.
        /// </summary>
        /// <paran name="go">Billboard gameobject.</param>
        /// <param name="archive">Archive index.</param>
        /// <param name="record">Record index.</param>
        static public void SetBillboardCustomMaterial(GameObject go, ref DaggerfallBillboard.BillboardSummary summary)
        {
            // Variables
            int numberOfFrames;
            int archive = summary.Archive;
            int record = summary.Record;
            //string name = GetName(archive, record);
            var meshRenderer = go.GetComponent<MeshRenderer>();           
            Texture2D albedoTexture, emissionMap;

            // Check if billboard is emissive
            bool isEmissive = meshRenderer.material.GetTexture("_EmissionMap");

            // UVs
            Vector2 uv = Vector2.zero;

            // Get properties from Xml
            string path = Path.Combine(texturesPath, GetName(archive, record));
            if (XMLManager.XmlFileExists(path))
            {
                var xml = new XMLManager(path);

                // Set billboard scale
                Transform transform = go.GetComponent<Transform>();
                transform.localScale = xml.GetVector3("scaleX", "scaleY", transform.localScale);
                summary.Size.x *= transform.localScale.x;
                summary.Size.y *= transform.localScale.y;

                // Get UV
                uv = xml.GetVector2("uvX", "uvY", uv);
            }

            // Update UV
            SetUv(go.GetComponent<MeshFilter>(), uv.x, uv.y);

            // Get material from cache or import from disk
            MaterialReader materialReader = DaggerfallUnity.Instance.MaterialReader;
            CachedMaterial cachedMaterialOut;
            if (materialReader.GetCachedMaterialCustomBillboard(archive, record, 0, out cachedMaterialOut))
            {
                // Get and set material
                meshRenderer.material = cachedMaterialOut.material;

                // Get other properties
                numberOfFrames = cachedMaterialOut.singleFrameCount;
                albedoTexture = cachedMaterialOut.albedoMap;
                emissionMap = cachedMaterialOut.emissionMap;
            }
            else
            {
                // Get textures from disk
                LoadCustomBillboardFrameTexture(isEmissive, out albedoTexture, out emissionMap, archive, record);

                // Main texture
                meshRenderer.material.SetTexture("_MainTex", albedoTexture);

                // Emission maps for lights
                if (isEmissive)
                    meshRenderer.material.SetTexture("_EmissionMap", emissionMap);

                // Get number of frames on disk
                numberOfFrames = NumberOfAvailableFrames(archive, record);

                // Save material in cache
                CachedMaterial newcm = new CachedMaterial()
                {
                    albedoMap = albedoTexture,
                    emissionMap = emissionMap,
                    material = meshRenderer.material,
                    singleFrameCount = numberOfFrames
                };
                materialReader.SetCachedMaterialCustomBillboard(archive, record, 0, newcm);
            }

            // Import textures for each frame if billboard is animated
            summary.CustomBillboard = new CustomBillboard();
            summary.CustomBillboard.isCustomAnimated = numberOfFrames > 1;
            if (summary.CustomBillboard.isCustomAnimated)
            {
                List<Texture2D> albedoTextures = new List<Texture2D>();
                List<Texture2D> emissionmaps = new List<Texture2D>();

                // Frame zero
                albedoTextures.Add(albedoTexture);
                if (isEmissive)
                    emissionmaps.Add(emissionMap);

                // Other frames
                for (int frame = 1; frame < numberOfFrames; frame++)
                {
                    if (materialReader.GetCachedMaterialCustomBillboard(archive, record, frame, out cachedMaterialOut))
                    {
                        // Get textures from cache
                        albedoTexture = cachedMaterialOut.albedoMap;
                        emissionMap = cachedMaterialOut.emissionMap;
                    }
                    else
                    {
                        // Get textures from disk
                        LoadCustomBillboardFrameTexture(isEmissive, out albedoTexture, out emissionMap, archive, record, frame);

                        // Save textures in cache
                        CachedMaterial newcm = new CachedMaterial()
                        {
                            albedoMap = albedoTexture,
                            emissionMap = emissionMap,
                        };
                        materialReader.SetCachedMaterialCustomBillboard(archive, record, frame, newcm);
                    }

                    albedoTextures.Add(albedoTexture);
                    if (isEmissive)
                        emissionmaps.Add(emissionMap);
                }

                // Set textures and properties
                summary.CustomBillboard.MainTexture = albedoTextures;
                summary.CustomBillboard.EmissionMap = emissionmaps;
                summary.CustomBillboard.NumberOfFrames = numberOfFrames;
                summary.CustomBillboard.isEmissive = isEmissive;
            }
        }

        public static bool EnemyHasCustomMaterial(int archive)
        {
            // This is the first texture set on enemy. Enemies use all custom textures or all vanilla.
            // If (archive, defaultRecord, defaultFrame) is present on disk all other textures are
            // considered required, otherwise vanilla textures are used.
            return TextureFileExist(texturesPath, GetName(archive, enemyDefaultRecord, enemyDefaultFrame));
        }

        /// <summary>
        /// Import and set custom material on Enemy unit.
        /// </summary>
        /// <param name="go">Enemy Mobile Unit.</param>
        /// <param name="archive">Archive which contains all textures except the dead enemy.</param>
        /// <param name="textures">All textures for this enemy (except dead texture).</param>
        static public void SetupCustomEnemyMaterial(GameObject go, int archive, out List<List<Texture2D>> textures)
        {
            var meshRenderer = go.GetComponent<MeshRenderer>();
            MaterialReader materialReader = DaggerfallUnity.Instance.MaterialReader;
            int record = enemyDefaultRecord, frame = enemyDefaultFrame;

            // Update UV map
            SetUv(go.GetComponent<MeshFilter>());

            // Get default texture from cache or import from disk
            meshRenderer.sharedMaterial = MaterialReader.CreateStandardMaterial(MaterialReader.CustomBlendMode.Cutout);
            CachedMaterial cachedMaterialOut;
            if (materialReader.GetCachedMaterialCustomBillboard(archive, record, frame, out cachedMaterialOut))
                meshRenderer.material.mainTexture = cachedMaterialOut.albedoMap;
            else
            {
                // Get texture for default frame
                Texture2D albedoTexture = LoadCustomTexture(archive, record, frame);
                albedoTexture.filterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode;

                // Set texture for default frame
                meshRenderer.material.mainTexture = albedoTexture;

                // Save in cache
                CachedMaterial newcm = new CachedMaterial()
                {
                    albedoMap = albedoTexture,
                    material = meshRenderer.material
                };
                materialReader.SetCachedMaterialCustomBillboard(archive, record, frame, newcm);
            }

            // Import textures for all records and frames
            record = 0;
            textures = new List<List<Texture2D>>();
            while (CustomTextureExist(archive, record))
            {
                frame = 0;
                List<Texture2D> frameTextures = new List<Texture2D>();
                while (CustomTextureExist(archive, record, frame))
                {
                    if (materialReader.GetCachedMaterialCustomBillboard(archive, record, frame, out cachedMaterialOut))
                        frameTextures.Add(cachedMaterialOut.albedoMap);
                    else
                    {
                        // Get texture
                        Texture2D tex = LoadCustomTexture(archive, record, frame);
                        tex.filterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode;
                        frameTextures.Add(tex);

                        // Save in cache
                        CachedMaterial newcm = new CachedMaterial() { albedoMap = tex };
                        materialReader.SetCachedMaterialCustomBillboard(archive, record, frame, newcm);
                    }

                    frame++;
                }
                textures.Add(frameTextures);
                record++;
            }
        }

        public static void SetEnemyScale(int archive, int record, ref Vector2 size)
        {
            string path = Path.Combine(texturesPath, GetName(archive, record));
            if (!XMLManager.XmlFileExists(path))
                return;

            var xml = new XMLManager(path);
            Vector2 scale = xml.GetVector2("scaleX", "scaleY", Vector2.zero);
            size.x *= scale.x;
            size.y *= scale.y;
        }

        /// <summary>
        /// Import custom texture and label settings for buttons
        /// </summary>
        /// <param name="button">Button</param>
        /// <param name="colorName">Name of texture</param>
        static public void SetCustomButton(ref Button button, string colorName)
        {
            // Load texture
            button.BackgroundTexture = LoadCustomTexture(colorName);
            button.BackgroundTexture.filterMode = (FilterMode)DaggerfallUnity.Settings.GUIFilterMode;

            // Load settings from Xml
            string path = Path.Combine(texturesPath, colorName);
            if (XMLManager.XmlFileExists(path))
            {
                var xml = new XMLManager(path);

                string value;
                if (xml.TryGetString("customtext", out value))
                {
                    if (value == "true") // Set custom color for text
                        button.Label.TextColor = xml.GetColor(button.Label.TextColor);
                    else if (value == "notext") // Disable text. This is useful if text is drawn on texture
                        button.Label.Text = string.Empty;
                }
            }            
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Convert (archive, record, frame) to string name.
        /// </summary>
        /// <param name="archive">Archive index from TEXTURE.XXX</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for animations.</param>
        static public string GetName (int archive, int record, int frame = 0)
        {
            return archive.ToString("D3") + "_" + record.ToString() + "-" + frame.ToString();
        }

        /// <summary>
        /// Convert (archive, record, frame) to string name.
        /// </summary>
        /// <param name="archive">Archive index from TEXTURE.XXX</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for animations.</param>
        /// <param name="dye">Color Dye</param>
        public static string GetName(int archive, int record, int frame, DyeColors dye)
        {
            if (dye == DyeColors.Unchanged)
                return GetName(archive, record, frame);
            else
                return GetName(archive, record, frame) + "_" + dye;
        }

        /// <summary>
        /// Get archive and record from "archive_record-0" string.
        /// </summary>
        /// <param name="name">"archive_record-frame string."</param>
        /// <param name="archive">Archive index.</param>
        /// <param name="record">Record index.</param>
        /// <returns>True if texture is a Daggerfall texture.</returns>
        static public bool IsDaggerfallTexture(string name, out int archive, out int record)
        {
            if ((name[3] == '_') && name.EndsWith("-0", StringComparison.CurrentCulture))
            {
                if (Int32.TryParse(name.Substring(0, 3), out archive))
                {
                    if ((name[5] == '-' && Int32.TryParse(name.Substring(4, 1), out record)) ||
                            (name[6] == '-' && Int32.TryParse(name.Substring(4, 2), out record)))
                        return true;
                }
            }

            archive = record = -1;
            return false;
        }

        /// <summary>
        /// Convert (filename, record, frame) to string name.
        /// </summary>
        /// <param name="filename">Name of CIF/RCI file.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for animations.</param>
        static public string GetNameCifRci (string filename, int record, int frame = 0)
        {
            return filename + "_" + record.ToString() + "-" + frame.ToString();
        }

        /// <summary>
        /// Convert (filename, record, frame) to string name.
        /// </summary>
        /// <param name="filename">Name of CIF/RCI file.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for animations.</param>
        /// <param name="metalType">Metal type of weapon.</param>
        static public string GetNameCifRci(string filename, int record, int frame, MetalTypes metalType)
        {
            if (metalType == MetalTypes.None)
                return GetNameCifRci(filename, record, frame);
            else
                return GetNameCifRci(filename, record, frame) + "_" + metalType;
        }

        /// <summary>
        /// Get a safe size for a control based on resolution of img.
        /// </summary>
        public static Vector2 GetSize(Texture2D texture, string textureName, bool allowXml = false)
        {
            if (!DaggerfallUnity.Settings.MeshAndTextureReplacement)
                return new Vector2(texture.width, texture.height);

            if (allowXml)
            {
                // Get size from xml
                string path = Path.Combine(imgPath, textureName);
                if (XMLManager.XmlFileExists(path))
                {
                    var xml = new XMLManager(path);
                    Vector2 size;
                    if (xml.TryGetVector2("width", "height", out size))
                        return size;
                }
            }

            // Get size from Daggerfall image
            ImageData imageData = ImageReader.GetImageData(textureName, createTexture: false);
            return new Vector2(imageData.width, imageData.height);
        }

        /// <summary>
        /// Get a safe size for a control based on resolution of cif or rci.
        /// </summary>
        public static Vector2 GetSize(Texture2D texture, string textureName, int record, int frame = 0)
        {
            if (!DaggerfallUnity.Settings.MeshAndTextureReplacement)
                return new Vector2(texture.width, texture.height);

            // Get size from Daggerfall image
            ImageData imageData = ImageReader.GetImageData(textureName, createTexture: false);
            return new Vector2(imageData.width, imageData.height);
        }

        /// <summary>
        /// Read size associated with a texture from xml.
        /// </summary>
        public static bool TryGetSize(string textureName, out Vector2 size)
        {
            if (DaggerfallUnity.Settings.MeshAndTextureReplacement)
            {
                string path = Path.Combine(texturesPath, textureName);
                if (XMLManager.XmlFileExists(path))
                {
                    var xml = new XMLManager(path);
                    if (xml.TryGetVector2("width", "height", out size))
                        return true;
                }
            }

            size = new Vector2();
            return false;
        }

        public static int FileNameToArchive(string filename)
        {
            return int.Parse(filename.Substring("TEXTURE.".Length));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// True if image file exists and settings allow import.
        /// </summary>
        /// <param name="path">Path to file on disk.</param>
        /// <param name="fileName">Name of file without extension.</param>
        private static bool TextureFileExist (string path, string name)
        {
            return DaggerfallUnity.Settings.MeshAndTextureReplacement
                && File.Exists(Path.Combine(path, name + extension));
        }

        /// <summary>
        /// Import image file as texture2D.
        /// </summary>
        /// <param name="path">Path to file on disk.</param>
        /// <param name="fileName">Name of file without extension.</param>
        /// <param name="mipMaps">Enable MipMaps?</param>
        private static Texture2D ImportTextureFile (string path, string fileName, bool mipMaps = false)
        {
            // Create empty texture, size will be the actual size of .png file
            Texture2D tex = new Texture2D(4, 4, TextureFormat, mipMaps);

            // Load image as Texture2D
            tex.LoadImage(File.ReadAllBytes(Path.Combine(path, fileName + extension)));

#if DEBUG_TEXTURE_FORMAT    
            Debug.LogFormat("{0}: {1} - mipmaps requested: {2}, mipmaps count : {3}", fileName, tex.format, mipMaps, tex.mipmapCount);
#endif

            return tex;
        }

        /// <summary>
        /// Import image file as Normal Map texture2D.
        /// </summary>
        /// <param name="path">Path to file on disk.</param>
        /// <param name="fileName">Name of file without extension.</param>
        private static Texture2D ImportNormalMap (string path, string fileName)
        {
            // Get texture
            Texture2D tex = ImportTextureFile(path, fileName, true);

            // RGBA to DXTnm
            Color32[] colours = tex.GetPixels32();
            for (int i = 0; i < colours.Length; i++)
            {
                colours[i].a = colours[i].r;
                colours[i].r = colours[i].b = colours[i].g;
            }
            tex.SetPixels32(colours);
            tex.Apply();

            return tex;
        }

        /// <summary>
        /// Seek texture from modding locations.
        /// </summary>
        /// <param name="path">Path on disk (loose files only).</param>
        /// <param name="name">Name of texture.</param>
        /// <param name="tex">Imported texture.</param>
        /// <returns>True if texture imported.</returns>
        private static bool TryImportTexture(string path, string name, out Texture2D tex)
        {
            if (DaggerfallUnity.Settings.MeshAndTextureReplacement)
            {
                // Seek from loose files
                if (File.Exists(Path.Combine(path, name + extension)))
                {
                    tex = ImportTextureFile(path, name);
                    return true;
                }

                // Seek from mods
                if (ModManager.Instance != null)
                    return ModManager.Instance.TryGetAsset(name, false, out tex);
            }

            tex = null;
            return false;
        }

        /// <summary>
        /// Seek animated texture from modding locations with all frames.
        /// </summary>
        /// <param name="path">Path on disk (loose files only).</param>
        /// <param name="getName">Get name of frame.</param>
        /// <param name="texFrames">Imported texture frames.</param>
        /// <returns>True if texture imported.</returns>
        private static bool TryImportTexture(string path, Func<int, string> getName, out Texture2D[] texFrames)
        {
            int frame = 0;
            Texture2D tex;
            if (TryImportTexture(path, getName(frame), out tex))
            {
                var textures = new List<Texture2D>();
                do textures.Add(tex);
                while (TryImportTexture(path, getName(++frame), out tex));
                texFrames = textures.ToArray();
                return true;
            }

            texFrames = null;
            return false;
        }

        /// <summary>
        /// Set UV Map for a planar mesh.
        /// </summary>
        /// <param name="x">Offset on X axis.</param>
        /// <param name="y">Offset on Y axis.</param>
        private static void SetUv(MeshFilter meshFilter, float x = 0, float y = 0)
        {
            Vector2[] uv = new Vector2[4];
            uv[0] = new Vector2(x, 1 - y);
            uv[1] = new Vector2(1 - x, 1 - y);
            uv[2] = new Vector2(x, y);
            uv[3] = new Vector2(1 - x, y);
            meshFilter.mesh.uv = uv;
        }

        /// <summary>
        /// Get number of frames available on disk for a record.
        /// </summary>
        /// <param name="archive">Archive index.</param>
        /// <param name="record">Record index.</param>
        private static int NumberOfAvailableFrames(int archive, int record)
        {
            int frames = 0;
            while (CustomTextureExist(archive, record, frames))
                frames++;
            return frames;
        }

        /// <summary>
        /// Import textures from disk for billboard gameobject for specified frame. 
        /// </summary>
        /// <paran name="isEmissive">True for lights.</param>
        /// <paran name="albedoTexture">Main texture for this frame.</param>
        /// <paran name="emissionMap">Eventual Emission map for this frame.</param>
        /// <param name="archive">Archive index.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for animated billboards.</param>
        static private void LoadCustomBillboardFrameTexture(
            bool isEmissive, 
            out Texture2D albedoTexture, 
            out Texture2D emissionMap, 
            int archive, 
            int record, 
            int frame = 0)
        {
            // Main texture
            albedoTexture = LoadCustomTexture(archive, record, frame);
            albedoTexture.filterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode;

            // Emission map
            if (isEmissive)
            {
                // Import emission map if available on disk
                if (CustomEmissionExist(archive, record, frame))
                    emissionMap = LoadCustomEmission(archive, record, frame);
                // If texture is emissive but no emission map is provided, emits from the whole surface
                else
                    emissionMap = albedoTexture;

                emissionMap.filterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode;
            }
            else
                emissionMap = null;
        }

        #endregion
    }
}
 