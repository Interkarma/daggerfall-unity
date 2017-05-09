// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
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
 * 1. StreamingWorld billboards (Use CreateTextureSettings() and GetTexture2DAtlas() with a 4096x4096 atlas)
 * 2. PaperDoll CharacterLayer textures works only if resolution is the same as vanilla 
 *        (http://forums.dfworkshop.net/viewtopic.php?f=22&p=3547&sid=6a99dbcffad1a15b08dd5e157274b772#p3547)
 * 3. Terrain textures (Texture arrays)
 */

using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;

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

        // Paths
        static private string texturesPath = Path.Combine(Application.streamingAssetsPath, "Textures");
        static private string imgPath = Path.Combine(texturesPath, "Img");
        static private string cifPath = Path.Combine(texturesPath, "CifRci");

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
            public List<Texture2D> MainTexture;          // List of custom albedo maps
            public List<Texture2D> EmissionMap;          // List of custom emission maps
            public bool isEmissive;                      // True if billboard is emissive
            public int NumberOfFrames;                   // number of frame textures avilable on disk
        }

        // Enemy will use custom textures if [archive, enemyDefaultRecord, enemyDefaultFrame] is found.
        public const int enemyDefaultRecord = 0;          
        public const int enemyDefaultFrame = 0;

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

        /// <summary>
        /// Path to custom textures on disk.
        /// </summary>
        static public string TexturesPath
        {
            get { return texturesPath; }
            internal set { texturesPath = value; }
        }

        /// <summary>
        /// Path to custom images on disk.
        /// </summary>
        static public string ImagesPath
        {
            get { return imgPath; }
            internal set { imgPath = value; }
        }

        /// <summary>
        /// Path to custom Cif and Rci files on disk.
        /// </summary>
        static public string CifRciPath
        {
            get { return cifPath; }
            internal set { cifPath = value; }
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
            return ImportTextureFile(texturesPath, GetName(archive, record, frame));
        }

        /// <summary>
        /// Import image from disk as texture2D
        /// (name.png).
        /// </summary>
        /// <param name="name">Name of texture without extension.</param>
        /// <returns>Texture.</returns>
        static public Texture2D LoadCustomTexture(string name)
        {
            return ImportTextureFile(texturesPath, name);
        }

        /// <summary>
        /// Search for image file on disk to replace .IMGs.
        /// (imagefile.png, for example 'REST02I0.IMG.png').
        /// </summary>
        /// <param name="filename">Name of image.</param>
        /// <returns>True if texture exists.</returns>
        static public bool CustomImageExist(string filename)
        {
            return TextureFileExist(imgPath, filename);
        }

        /// <summary>
        /// Import image from disk as texture2D
        /// (imagefile.png, for example 'REST02I0.IMG.png').
        /// </summary>
        /// <param name="filename">Name of image.</param>
        /// <returns>Image.</returns>
        static public Texture2D LoadCustomImage(string filename)
        {
            return ImportTextureFile(imgPath, filename);
        }

        /// <summary>
        /// Search for image file on disk to replace .CIFs and .RCIs.
        /// (filename_record-frame.png, for example 'INVE16I0.CIF_1-0.png').
        /// </summary>
        /// <param name="filename">Name of image.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for weapon animations (WEAPONXX.CIF).</param> 
        /// <returns>True if image exists.</returns>
        static public bool CustomCifExist(string filename, int record, int frame = 0)
        {
            return TextureFileExist(cifPath, GetNameCifRci(filename, record, frame));
        }

        /// <summary>
        /// Import image as Texture2D to replace .CIFs and .RCIs.
        /// (filename_record-frame.png, for example 'INVE16I0.CIF_1-0.png').
        /// </summary>
        /// <param name="filename">Name of image.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for weapon animations (WEAPONXX.CIF) </param> 
        /// <returns>Image.</returns>
        static public Texture2D LoadCustomCif(string filename, int record, int frame)
        {
            return ImportTextureFile(cifPath, GetNameCifRci(filename, record, frame));
        }

        /// <summary>
        /// Search for image on disk to replace .CIFs and .RCIs. for a specific metalType
        /// (filename_record-frame_metalType.png, for example 'WEAPON04.CIF_0-0_Iron.Png').
        /// </summary>
        /// <param name="filename">Name of image.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for weapon animations (WEAPONXX.CIF) </param> 
        /// <returns>True if generic or specific image exists.</returns>
        static public bool CustomCifExist(string filename, int record, int frame, MetalTypes metalType)
        {
            return TextureFileExist(cifPath, GetNameCifRci(filename, record, frame, metalType));
        }


        /// <summary>
        /// Import image from disk to replace .CIFs and .RCIs. for a specific metalType
        /// (filename_record-frame_metalType.png', for example 'WEAPON04.CIF_0-0_Iron.Png').
        /// </summary>
        /// <param name="filename">Name of image.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for weapon animations (WEAPONXX.CIF) </param> 
        /// <returns>Image for this metalType or generic image if metalType is None.</returns>
        static public Texture2D LoadCustomCif(string filename, int record, int frame, MetalTypes metalType)
        {
            return ImportTextureFile(cifPath, GetNameCifRci(filename, record, frame, metalType));
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
            return ImportTextureFile(texturesPath, GetName(archive, record, frame) + MapTags.Emission);
        }

        /// <summary>
        /// Import image file from disk to use as emission map
        /// (name_Emission.png)
        /// </summary>
        /// <param name="name">Name of texture.</param>
        /// <returns>Emission map.</returns>
        static public Texture2D LoadCustomEmission(string name)
        {
            return ImportTextureFile(texturesPath, name + MapTags.Emission);
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
            return ImportTextureFile(texturesPath, GetName(archive, record, frame) + MapTags.MetallicGloss);
        }

        /// <summary>
        /// Import image file from disk to use as MetallicGloss map
        /// (name_MetallicGloss.png).
        /// </summary>
        /// <param name="name">Name of texture.</param>
        /// <returns>MetallicGloss map.</returns>
        static public Texture2D LoadCustomMetallicGloss(string name)
        {
            return ImportTextureFile(texturesPath, name + MapTags.MetallicGloss);
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
                        texture = ImportTextureFile(path, name);
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
        /// Import texture(s) used on models.
        /// </summary>
        /// <param name="archive">Archive index</param>
        /// <param name="record">Record index</param>
        /// <param name="frame">Texture frame</param>
        /// <param name="results">Texture Results</param>
        /// <param name="GenerateNormals">Will create normal map</param>
        static public void LoadCustomTextureResults(int archive, int record, int frame, ref GetTextureResults results, ref bool GenerateNormals)
        {
            // Main texture
            if (CustomTextureExist(archive, record, frame))
            {
                results.albedoMap = LoadCustomTexture(archive, record, frame);
            }

            // Normal map
            if (CustomNormalExist(archive, record, frame))
            {
                results.normalMap = LoadCustomNormal(archive, record, frame);
                GenerateNormals = true;
            }

            // Emission map
            // windowed walls use a custom emission map or stick with vanilla
            // non-window use the main texture as emission, unless a custom map is provided
            if (results.isEmissive)
            {
                if (CustomEmissionExist(archive, record, frame))
                {
                    // Import emission texture
                    results.emissionMap = LoadCustomEmission(archive, record, frame);
                }
                else if (!results.isWindow && CustomTextureExist(archive, record, frame)) 
                {
                    // Reuse albedo map for basic colour emission
                    results.emissionMap = results.albedoMap;
                }
            }
            else if (CustomEmissionExist(archive, record, frame))
            {
                // Force emission map
                results.emissionMap = LoadCustomEmission(archive, record, frame);
                results.isEmissive = true;
                results.isWindow = false;
            }
        }

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
            if (XMLManager.XmlFileExist(archive, record, frame))
            {
                string fileName = GetName(archive, record, frame);
                float value;

                // Metallic parameter
                if (XMLManager.TryGetFloat(fileName, "metallic", out value, texturesPath))
                    material.SetFloat("_Metallic", value);

                // Smoothness parameter
                if (XMLManager.TryGetFloat(fileName, "smoothness", out value, texturesPath))
                    material.SetFloat("_Glossiness", value);
            }
        }

        /// <summary>
        /// Set custom material on billboard gameobject.
        /// </summary>
        /// <paran name="go">Billboard gameobject.</param>
        /// <param name="archive">Archive index.</param>
        /// <param name="record">Record index.</param>
        static public void SetBillboardCustomMaterial(GameObject go, int archive, int record)
        {
            int numberOfFrames;
            string name = GetName(archive, record);
            var meshRenderer = go.GetComponent<MeshRenderer>();
            var daggerfallBillboard = go.GetComponent<DaggerfallBillboard>();
            Texture2D albedoTexture, emissionMap;

            // Check if billboard is emissive
            bool isEmissive = false;
            if (meshRenderer.material.GetTexture("_EmissionMap") != null)
                isEmissive = true;

            // UVs
            Vector2 uv = Vector2.zero;

            // Get properties from Xml
            if (XMLManager.XmlFileExist(archive, record))
            {
                // Customize billboard size (scale)
                Transform transform = go.GetComponent<Transform>();
                transform.localScale = XMLManager.GetScale(name, texturesPath, transform.localScale);
                daggerfallBillboard.SetCustomSize(archive, record, transform.localScale.y);

                // Get UV
                uv = XMLManager.GetUv(name, texturesPath, uv.x, uv.y);
            }

            // Update UV
            UpdateUV(go.GetComponent<MeshFilter>(), uv.x, uv.y);

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
            if (numberOfFrames > 1)
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
                CustomBillboard customBillboard = new CustomBillboard()
                {
                    MainTexture = albedoTextures,
                    EmissionMap = emissionmaps,
                    NumberOfFrames = numberOfFrames,
                    isEmissive = isEmissive
                };
                daggerfallBillboard.SetCustomMaterial(customBillboard);
            }
        }

        /// <summary>
        /// Import and set custom material on Enemy unit.
        /// </summary>
        /// <param name="go">Enemy Mobile Unit.</param>
        /// <param name="archive">Archive which contains all textures except the dead enemy.</param>
        /// <param name="textures">All textures for this enemy (except dead texture).</param>
        static public void SetupCustomEnemyMaterial(GameObject go, int archive, out List<List<Texture2D>> textures)
        {
            // This is the first texture set on enemy. Enemies use all custom textures or all vanilla.
            // If (archive, defaultRecord, defaultFrame) is present on disk all other textures are
            // considered required, otherwise vanilla textures are used.
            int record = enemyDefaultRecord, frame = enemyDefaultFrame;

            // Update UV map
            UpdateUV(go.GetComponent<MeshFilter>());

            // Get default texture from cache or import from disk
            var meshRenderer = go.GetComponent<MeshRenderer>();
            MaterialReader materialReader = DaggerfallUnity.Instance.MaterialReader;
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
            if (XMLManager.XmlFileExist(colorName, texturesPath))
            {
                // Set custom color
                if (XMLManager.GetString(colorName, "customtext", texturesPath) == "true")
                    button.Label.TextColor = XMLManager.GetColor(colorName, texturesPath);
                // Disable text. This is useful if text is drawn on texture
                else if (XMLManager.GetString(colorName, "customtext", texturesPath) == "notext")
                    button.Label.Text = "";
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
        /// Get size of vanilla texture, even if using a custom one.
        /// </summary>
        /// <param name="texture">Texture.</param>
        /// <param name="textureName">Archive of texture.</param>
        /// <param name="record">Record of texture.</param>
        /// <param name="frame">Frame of texture.</param>
        /// <returns>Vector2 with width and height</returns>
        static public Vector2 GetSizeFromTexture(Texture2D texture, int archive, int record, int frame = 0)
        {
            if (CustomTextureExist(archive, record, frame))
            {
                ImageData imageData = ImageReader.GetImageData("TEXTURE." + archive, createTexture: false);
                return new Vector2(imageData.width, imageData.height);
            }
            else
                return new Vector2(texture.width, texture.height);
        }

        /// <summary>
        /// Get size of vanilla texture, even if using a custom one.
        /// </summary>
        /// <param name="texture">Texture.</param>
        /// <param name="textureName">Name of texture file.</param>
        /// <param name="record">Record of texture (CifRci only).</param>
        /// <param name="frame">Frame of texture (CifRci only).</param>
        /// <returns>Vector2 with width and height</returns>
        static public Vector2 GetSizeFromTexture(Texture2D texture, string textureName, int record = 0, int frame = 0)
        {
            if (CustomImageExist(textureName) || CustomCifExist(textureName, record, frame))
            {
                ImageData imageData = ImageReader.GetImageData(textureName, createTexture: false);
                return new Vector2(imageData.width, imageData.height);
            }
            else
                return new Vector2(texture.width, texture.height);
        }

        /// <summary>
        /// Get custom size or default size of texture.
        /// </summary>
        static public Vector2 GetSizeFromXml(Texture2D texture, string textureName, float scaleWidth = 1, float scaleHeight = 1)
        {
            if (CustomImageExist(textureName))
                return XMLManager.GetSize(textureName, imgPath, scaleWidth, scaleHeight);
            else
                return new Vector2(texture.width * scaleWidth, texture.height * scaleHeight);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Check if image file exist on disk.
        /// </summary>
        /// <param name="path">Location of image file.</param>
        /// <param name="name">Name of image file.</param>
        /// <returns></returns>
        static private bool TextureFileExist (string path, string name)
        {
            if (DaggerfallUnity.Settings.MeshAndTextureReplacement //check .ini setting
                && File.Exists(Path.Combine(path, name + ".png")))
                return true;

            return false;
        }

        /// <summary>
        /// Import image file with .png extension from disk,
        /// to be used as a texture.
        /// </summary>
        /// <param name="path">Location of image file.</param>
        /// <param name="name">Name of image file.</param>
        /// <returns></returns>
        static private Texture2D ImportTextureFile (string path, string name)
        {
            // Create empty texture, size will be the actual size of .png file
            Texture2D tex = new Texture2D(2, 2);

            // Load image as Texture2D
            tex.LoadImage(File.ReadAllBytes(Path.Combine(path, name + ".png")));

            // Return imported texture
            if (tex != null)
                return tex;

            Debug.LogError("Can't import custom texture " + name + ".png from " + path);
            return null;
        }

        /// <summary>
        /// Import image file with .png extension from disk,
        /// to be used as a normal map.
        /// </summary>
        /// <param name="path">Location of image file.</param>
        /// <param name="name">Name of image file.</param>
        /// <returns>Normal map as Texture2D</returns>
        static private Texture2D ImportNormalMap (string path, string name)
        {
            //create empty texture, size will be the actual size of .png file
            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, true);

            // Load image as Texture2D
            tex.LoadImage(File.ReadAllBytes(Path.Combine(path, name + ".png")));

            if (tex == null)
            {
                Debug.LogError("Can't import custom texture " + name + ".png from " + path);
                return null;
            }

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
        /// Update UV map
        /// </summary>
        /// <param name="meshFilter">MeshFilter of GameObject</param>
        static private void UpdateUV (MeshFilter meshFilter, float x = 0, float y = 0)
        {
            Vector2[] uv = new Vector2[4];
            uv[0] = new Vector2(x, 1 - y);
            uv[1] = new Vector2(1 - x, 1 - y);
            uv[2] = new Vector2(x, y);
            uv[3] = new Vector2(1 - x, y);
            meshFilter.mesh.uv = uv;
        }

        /// <summary>
        /// Check all frames available on disk.
        /// </summary>
        /// <param name="archive">Archive index.</param>
        /// <param name="record">Record index.</param>
        /// <returns>Number of textures present on disk for this record</returns>
        static private int NumberOfAvailableFrames(int archive, int record)
        {
            int frames = 0;
            while (CustomTextureExist(archive, record, frames))
            {
                frames++;
            }
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
 