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
 * - Import materials from mods
 * - Import terrain textures from mods
 */

//#define DEBUG_TEXTURE_FORMAT

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Utility.ModSupport;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Supported textures maps.
    /// </summary>
    public enum TextureMap
    {
        Albedo,
        Normal,
        Emission,
        MetallicGloss
    }

    public struct BillboardImportedTextures
    {
        public bool HasImportedTextures;            // Contains imported textures ?
        public int FrameCount;                      // Number of frames
        public bool IsEmissive;                     // Is billboard emissive ?
        public List<Texture2D> Albedo;              // Textures for all frames.
        public List<Texture2D> Emission;            // EmissionMaps for all frames.
    }

    public struct EnemyImportedTextures
    {
        public bool HasImportedTextures;            // Contains imported textures ?  
        public List<List<Texture2D>> Textures;      // Textures for all records and frames.
    }

    /// <summary>
    /// Handles import and injection of custom textures and images
    /// with the purpose of providing modding support.
    /// </summary>
    static public class TextureReplacement
    {
        #region Fields

        const string extension = ".png";

        // Paths
        static readonly string texturesPath = Path.Combine(Application.streamingAssetsPath, "Textures");
        static readonly string imgPath = Path.Combine(texturesPath, "Img");
        static readonly string cifRciPath = Path.Combine(texturesPath, "CifRci");

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
            return TextureFileExist(texturesPath, GetName(archive, record, frame, TextureMap.Normal));
        }

        /// <summary>
        /// Search for image file on disk to use as normal map
        /// (name_Normal.png).
        /// </summary>
        /// <param name="name">Name of texture.</param>
        /// <returns>True if normal map exists.</returns>
        static public bool CustomNormalExist(string name)
        {
            return TextureFileExist(texturesPath, name + "_" + TextureMap.Normal);
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
            return ImportNormalMap(texturesPath, GetName(archive, record, frame, TextureMap.Normal));
        }

        /// <summary>
        /// Import image file from disk to use as normal map
        /// (name_Normal.png).
        /// </summary>
        /// <param name="name">Name of texture.</param>
        /// <returns>Normal map.</returns>
        static public Texture2D LoadCustomNormal(string name)
        {
            return ImportNormalMap(texturesPath, name + "_" + TextureMap.Normal);
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
            return TextureFileExist(texturesPath, GetName(archive, record, frame, TextureMap.Emission));
        }

        /// <summary>
        /// Search for image file on disk to use as emission map
        /// (name_Emission.png)
        /// </summary>
        /// <param name="name">Name of texture.</param>
        /// <returns>True if emission map exists.</returns>
        static public bool CustomEmissionExist(string name)
        {
            return TextureFileExist(texturesPath, name + "_" + TextureMap.Emission);
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
            return ImportTextureFile(texturesPath, GetName(archive, record, frame, TextureMap.Emission), true);
        }

        /// <summary>
        /// Import image file from disk to use as emission map
        /// (name_Emission.png)
        /// </summary>
        /// <param name="name">Name of texture.</param>
        /// <returns>Emission map.</returns>
        static public Texture2D LoadCustomEmission(string name)
        {
            return ImportTextureFile(texturesPath, name + "_" + TextureMap.Emission, true);
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
            return TextureFileExist(texturesPath, GetName(archive, record, frame, TextureMap.MetallicGloss));
        }

        /// <summary>
        /// Search for image file on disk to use as metallic map
        /// (name_MetallicGloss.png).
        /// </summary>
        /// <param name="name">Name of texture.</param> 
        /// <returns>True if MetallicGloss map exist.</returns>
        static public bool CustomMetallicGlossExist(string name)
        {
            return TextureFileExist(texturesPath, name + "_" + TextureMap.MetallicGloss);
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
            return ImportTextureFile(texturesPath, GetName(archive, record, frame, TextureMap.MetallicGloss), true);
        }

        /// <summary>
        /// Import image file from disk to use as MetallicGloss map
        /// (name_MetallicGloss.png).
        /// </summary>
        /// <param name="name">Name of texture.</param>
        /// <returns>MetallicGloss map.</returns>
        static public Texture2D LoadCustomMetallicGloss(string name)
        {
            return ImportTextureFile(texturesPath, name + "_" + TextureMap.Emission, true);
        }

        /// <summary>
        /// Import a png file from loose files following arguments requirements.
        /// This is a helper method for mods and should not be used in core.
        /// </summary>
        /// <param name="relPath">Relative path to png file inside Textures folder.</param>
        /// <param name="textureMap">Require modifications for specific texture maps</param>
        /// <param name="mipMaps">Enable mipmaps?</param>
        /// <param name="tex">Imported texture.</param>
        /// <returns>True if texture imported.</returns>
        public static bool TryImportTextureFromLooseFiles(string relPath, TextureMap textureMap, bool mipMaps, out Texture2D tex)
        {
            return TryImportTextureFromDisk(texturesPath, relPath, textureMap, mipMaps, out tex);  
        }

        /// <summary>
        /// Import a png file from given location following arguments requirements.
        /// This is a helper method for mods and should not be used in core.
        /// </summary>
        /// <param name="directory">Folder on disk.</param>
        /// <param name="fileName">Name of png file without extension.</param>
        /// <param name="textureMap">Require modifications for specific texture maps</param>
        /// <param name="mipMaps">Enable mipmaps?</param>
        /// <param name="tex">Imported texture.</param>
        /// <returns>True if texture imported.</returns>
        public static bool TryImportTextureFromDisk(string directory, string fileName, TextureMap textureMap, bool mipMaps, out Texture2D tex)
        {
            if (File.Exists(Path.Combine(directory, fileName + extension)))
            {
                if (textureMap == TextureMap.Normal)
                    tex = ImportNormalMap(directory, fileName);
                else
                    tex = ImportTextureFile(directory, fileName, mipMaps);
                return true;
            }

            tex = null;
            return false;
        }

        /// <summary>
        /// Import png file from disk as Texture2D.
        /// </summary>
        /// <param name="path">Path where image file is located.</param>
        /// <param name="name">Name of image file without extension.</param>
        /// <param name="texture">Texture.</param>
        /// <param name="mapTag">Texture map.</param>
        [Obsolete("Use 'TryImportTextureFromDisk' with more options")]
        static public bool ImportTextureFromDisk(string path, string name, out Texture2D texture, TextureMap map = TextureMap.Albedo)
        {
            return TryImportTextureFromDisk(path, name, map, true, out texture);
        }

        #endregion

        #region Textures Injection

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
        /// Import textures and emission maps for all frames of this billboard. Also set other material properties from xml.
        /// </summary>
        public static void SetBillboardImportedTextures(GameObject go, ref DaggerfallBillboard.BillboardSummary summary)
        {
            if (!DaggerfallUnity.Settings.MeshAndTextureReplacement)
                return;

            MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();

            int archive = summary.Archive;
            int record = summary.Record;
            int frame = 0;
            bool isEmissive = DaggerfallUnity.Instance.MaterialReader.TextureReader.IsEmissive(archive, record);

            // Check first frame
            Texture2D albedo, emission;
            bool hasImportedTextures = LoadFromCacheOrImport(archive, record, frame, isEmissive, out albedo, out emission);

            if (summary.ImportedTextures.HasImportedTextures = hasImportedTextures)
            {
                // Set texture on material
                meshRenderer.material.SetTexture("_MainTex", albedo);
                if (isEmissive)
                    meshRenderer.material.SetTexture("_EmissionMap", emission);

                // Import animation frames
                var albedoTextures = new List<Texture2D>();
                var emissionTextures = new List<Texture2D>();
                do
                {
                    albedoTextures.Add(albedo);
                    if (isEmissive)
                        emissionTextures.Add(emission);
                }
                while (LoadFromCacheOrImport(archive, record, ++frame, isEmissive, out albedo, out emission));

                // Set scale and uv
                Vector2 uv = Vector2.zero;
                XMLManager xml;
                if (XMLManager.TryReadXml(texturesPath, GetName(archive, record), out xml))
                {
                    // Set billboard scale
                    Transform transform = go.GetComponent<Transform>();
                    transform.localScale = xml.GetVector3("scaleX", "scaleY", transform.localScale);
                    summary.Size.x *= transform.localScale.x;
                    summary.Size.y *= transform.localScale.y;

                    // Get UV
                    uv = xml.GetVector2("uvX", "uvY", uv);
                }
                SetUv(go.GetComponent<MeshFilter>(), uv.x, uv.y);

                // Save results
                summary.ImportedTextures.FrameCount = frame;
                summary.ImportedTextures.IsEmissive = isEmissive;
                summary.ImportedTextures.Albedo = albedoTextures;
                summary.ImportedTextures.Emission = emissionTextures;
            }        
        }

        /// <summary>
        /// Import textures for all records and frames of this enemy.
        /// </summary>
        public static void SetEnemyImportedTextures(int archive, MeshFilter meshFilter, ref EnemyImportedTextures importedTextures)
        {
            if (!DaggerfallUnity.Settings.MeshAndTextureReplacement)
                return;

            // Check first texture.
            Texture2D tex;
            bool hasImportedTextures = LoadFromCacheOrImport(archive, 0, 0, out tex);

            if (importedTextures.HasImportedTextures = hasImportedTextures)
            {
                string fileName = TextureFile.IndexToFileName(archive);
                var textureFile = new TextureFile(Path.Combine(DaggerfallUnity.Instance.Arena2Path, fileName), FileUsage.UseMemory, true);

                // Import all textures in this archive
                var textures = new List<List<Texture2D>>();
                for (int record = 0; record < textureFile.RecordCount; record++)
                {
                    int frames = textureFile.GetFrameCount(record);
                    var frameTextures = new List<Texture2D>();
                    for (int frame = 0; frame < frames; frame++)
                    {
                        if ((record != 0 || frame != 0) && !LoadFromCacheOrImport(archive, record, frame, out tex))
                        {
                            Debug.LogErrorFormat("Imported archive {0} does not contain texture for record {1}, frame {2}!", archive, record, frame);
                            tex = ImageReader.GetTexture(fileName, record, frame, true);
                        }
                        frameTextures.Add(tex);
                    }
                    textures.Add(frameTextures);
                }

                // Update UV map
                SetUv(meshFilter);

                // Save results
                importedTextures.Textures = textures;
            }
        }

        public static void SetEnemyScale(int archive, int record, ref Vector2 size)
        {
            if (!DaggerfallUnity.Settings.MeshAndTextureReplacement)
                return;

            XMLManager xml;
            if (XMLManager.TryReadXml(texturesPath, GetName(archive, record), out xml))
            {
                Vector2 scale = xml.GetVector2("scaleX", "scaleY", Vector2.zero);
                size.x *= scale.x;
                size.y *= scale.y;
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
        /// Get name for a texture.
        /// </summary>
        /// <param name="archive">Archive index from TEXTURE.XXX</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for animations.</param>
        public static string GetName(int archive, int record, int frame = 0)
        {
            return string.Format("{0:000}_{1}-{2}", archive, record, frame);
        }

        /// <summary>
        /// Get name for a texture with a dye.
        /// </summary>
        /// <param name="archive">Archive index from TEXTURE.XXX</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for animations.</param>
        /// <param name="dye">Color Dye</param>
        public static string GetName(int archive, int record, int frame, DyeColors dye)
        {
            if (dye == DyeColors.Unchanged)
                return GetName(archive, record, frame);

            return string.Format("{0}_{1}", GetName(archive, record, frame), dye);
        }

        /// <summary>
        /// Get name for a specific texture map.
        /// </summary>
        /// <param name="archive">Archive index from TEXTURE.XXX</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for animations.</param>
        /// <param name="textureMap">Shader texture type.</param>
        /// <returns></returns>
        public static string GetName(int archive, int record, int frame, TextureMap textureMap)
        {
            if (textureMap == TextureMap.Albedo)
                return GetName(archive, record, frame);

            return string.Format("{0}_{1}", GetName(archive, record, frame), textureMap);
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
        /// Get name for a CifRci image.
        /// </summary>
        /// <param name="filename">Name of CIF/RCI file.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for animations.</param>
        static public string GetNameCifRci (string filename, int record, int frame = 0)
        {
            return string.Format("{0}_{1}-{2}", filename, record, frame);
        }

        /// <summary>
        /// Get name for a CifRci image with a metal type.
        /// </summary>
        /// <param name="filename">Name of CIF/RCI file.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for animations.</param>
        /// <param name="metalType">Metal type of weapon.</param>
        static public string GetNameCifRci(string filename, int record, int frame, MetalTypes metalType)
        {
            if (metalType == MetalTypes.None)
                return GetNameCifRci(filename, record, frame);

            return string.Format("{0}_{1}", GetNameCifRci(filename, record, frame), metalType);
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
        /// True if image file is found inside loose files and settings allow import.
        /// </summary>
        /// <param name="path">Path to file on disk.</param>
        /// <param name="fileName">Name of file without extension.</param>
        private static bool TextureFileExist (string path, string name)
        {
            return DaggerfallUnity.Settings.MeshAndTextureReplacement
                && File.Exists(Path.Combine(path, name + extension));
        }

        /// <summary>
        /// Import image file as texture2D from loose files.
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
        /// Import image file as Normal Map texture2D from loose files.
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
        /// Seek albedo from cache, loose files and mods.
        /// </summary>
        private static bool LoadFromCacheOrImport(int archive, int record, int frame, out Texture2D albedo)
        {
            Texture2D emission;
            return LoadFromCacheOrImport(archive, record, frame, false, out albedo, out emission);
        }

        /// <summary>
        /// Seek albedo and, if isEmissive is set, emission from cache, loose files and mods.
        /// </summary>
        /// <remarks>
        /// Import textures from modding locations and cache them. If isEmissive is true, emissionMap is always set
        /// with imported texture or, if missing, with albedo for a full-emissive surface.
        /// </remarks>
        private static bool LoadFromCacheOrImport(
            int archive,
            int record,
            int frame,
            bool isEmissive,
            out Texture2D albedo,
            out Texture2D emission)
        {
            MaterialReader materialReader = DaggerfallUnity.Instance.MaterialReader;

            CachedMaterial cachedMaterial;
            albedo = null;
            emission = null;

            if (materialReader.GetCachedMaterialCustomBillboard(archive, record, frame, out cachedMaterial))
            {
                albedo = cachedMaterial.albedoMap;
                emission = cachedMaterial.emissionMap;
                return true;
            }
            else if (TryImportTexture(texturesPath, GetName(archive, record, frame), out albedo))
            {
                var filterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode;

                albedo.filterMode = filterMode;
                cachedMaterial.albedoMap = albedo;

                if (isEmissive)
                {
                    if (!TryImportTexture(texturesPath, GetName(archive, record, frame++, TextureMap.Emission), out emission))
                        emission = albedo;
                    emission.filterMode = filterMode;
                    cachedMaterial.emissionMap = emission;
                }

                materialReader.SetCachedMaterialCustomBillboard(archive, record, frame, cachedMaterial);
                return true;
            }

            return false;
        }

        #endregion
    }
}
 