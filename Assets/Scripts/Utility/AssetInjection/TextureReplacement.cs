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
 */

//#define DEBUG_TEXTURE_FORMAT

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Utility.ModSupport;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    #region Enums and Structs

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

    public enum TextureImport
    {
        None,
        LooseFiles,
        AllLocations
    }

    public struct BillboardImportedTextures
    {
        public bool HasImportedTextures;            // Contains imported textures ?
        public int FrameCount;                      // Number of frames
        public bool IsEmissive;                     // Is billboard emissive ?
        public List<Texture2D> Albedo;              // Textures for all frames.
        public List<Texture2D> Emission;            // EmissionMaps for all frames.
    }

    public struct MobileBillboardImportedTextures
    {
        public bool HasImportedTextures;            // Contains imported textures ?  
        public List<List<Texture2D>> Textures;      // Textures for all records and frames.
    }

    #endregion

    /// <summary>
    /// Handles import and injection of custom textures and images with the purpose of providing modding support.
    /// Import materials from mods and textures from mods and loose files.
    /// </summary>
    public static class TextureReplacement
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

        static FilterMode MainFilterMode
        {
            get { return DaggerfallUnity.Instance.MaterialReader.MainFilterMode; }
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
        /// Seek material from mods.
        /// </summary>
        /// <param name="archive">Texture archive.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Animation frame index.</param>
        /// <param name="material">Imported material.</param>
        /// <returns>True if material imported.</returns>
        public static bool TryImportMaterial(int archive, int record, int frame, out Material material)
        {
            return TryImportMaterial(GetName(archive, record, frame), out material);
        }

        /// <summary>
        /// Seek animated texture from modding locations with all frames.
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
        /// Seek texture from modding locations.
        /// </summary>
        /// <param name="archive">Texture archive.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Animation frame index.</param>
        /// <param name="tex">Imported texture.</param>
        /// <returns>True if texture imported.</returns>
        public static bool TryImportTexture(int archive, int record, int frame, out Texture2D tex)
        {
            return TryImportTexture(texturesPath, GetName(archive, record, frame), false, out tex);
        }

        /// <summary>
        /// Seek texture from modding locations.
        /// </summary>
        /// <param name="archive">Texture archive.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Animation frame index.</param>
        /// <param name="textureMap">Texture type.</param>
        /// <param name="tex">Imported texture.</param>
        /// <returns>True if texture imported.</returns>
        public static bool TryImportTexture(int archive, int record, int frame, TextureMap textureMap, out Texture2D tex)
        {
            return TryImportTexture(texturesPath, GetName(archive, record, frame, textureMap), false, out tex);
        }

        /// <summary>
        /// Seek texture from modding locations.
        /// </summary>
        /// <param name="archive">Texture archive.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Animation frame index.</param>
        /// <param name="textureMap">Texture type.</param>
        /// <param name="textureImport">Texture import options.</param>
        /// <param name="tex">Imported texture.</param>
        /// <returns>True if texture imported.</returns>
        public static bool TryImportTexture(int archive, int record, int frame, TextureMap textureMap, TextureImport textureImport, out Texture2D tex)
        {
            tex = null;
            return (textureImport == TextureImport.AllLocations && TryImportTexture(texturesPath, GetName(archive, record, frame, textureMap), false, out tex))
                || (textureImport == TextureImport.LooseFiles && TryImportTextureFromLooseFiles(archive, record, frame, textureMap, out tex));
        }

        /// <summary>
        /// Seek texture from modding locations with a specific dye.
        /// </summary>
        /// <param name="archive">Texture archive.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Animation frame index</param>
        /// <param name="dye">Dye colour for armour, weapons, and clothing.</param>
        /// <param name="tex">Imported texture.</param>
        /// <returns>True if texture imported.</returns>
        public static bool TryImportTexture(int archive, int record, int frame, DyeColors dye, out Texture2D tex)
        {
            return TryImportTexture(texturesPath, GetName(archive, record, frame, dye), false, out tex);
        }

        /// <summary>
        /// Seek texture from modding locations.
        /// </summary>
        /// <param name="name">Texture name.</param>
        /// <param name="tex">Imported texture.</param>
        /// <returns>True if texture imported.</returns>
        public static bool TryImportTexture(string name, out Texture2D tex)
        {
            return TryImportTexture(texturesPath, name, false, out tex);
        }

        /// <summary>
        /// Seek image from modding locations.
        /// </summary>
        /// <param name="name">Image name.</param>
        /// <param name="readOnly">Release copy on system memory after uploading to gpu.</param>
        /// <param name="tex">Imported image as texture.</param>
        /// <returns>True if image imported.</returns>
        public static bool TryImportImage(string name, bool readOnly, out Texture2D tex)
        {
            return TryImportTexture(imgPath, name, readOnly, out tex);
        }

        /// <summary>
        /// Seek CifRci from modding locations.
        /// </summary>
        /// <param name="name">Image name.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Animation frame index</param>
        /// <param name="readOnly">Release copy on system memory after uploading to gpu.</param>
        /// <param name="tex">Imported image as texture.</param>
        /// <returns>True if CifRci imported.</returns>
        public static bool TryImportCifRci(string name, int record, int frame, bool readOnly, out Texture2D tex)
        {
            return TryImportTexture(cifRciPath, GetNameCifRci(name, record, frame), readOnly, out tex);
        }

        /// <summary>
        /// Seek CifRci with a specific metaltype from modding locations.
        /// </summary>
        /// <param name="name">Image name.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Animation frame index</param>
        /// <param name="metalType">Metal type.</param>
        /// <param name="readOnly">Release copy on system memory after uploading to gpu.</param>
        /// <param name="tex">Imported image as texture.</param>
        /// <returns>True if CifRci imported.</returns>
        public static bool TryImportCifRci(string name, int record, int frame, MetalTypes metalType, bool readOnly, out Texture2D tex)
        {
            return TryImportTexture(cifRciPath, GetNameCifRci(name, record, frame, metalType), false, out tex);
        }

        /// <summary>
        /// Seek texture from loose files.
        /// </summary>
        /// <param name="archive">Texture archive.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Animation index.</param>
        /// <param name="textureMap">Texture type.</param>
        /// <param name="tex">Imported texture.</param>
        /// <returns>True if texture imported.</returns>
        public static bool TryImportTextureFromLooseFiles(int archive, int record, int frame, TextureMap textureMap, out Texture2D tex)
        { 
            if (DaggerfallUnity.Settings.AssetInjection)
            {
                string path = Path.Combine(texturesPath, GetName(archive, record, frame, textureMap));
                return TryImportTextureFromDisk(path, true, textureMap == TextureMap.Normal, out tex);
            }

            tex = null;
            return false;           
        }

        /// <summary>
        /// Seek texture from loose files using a relative path from <see cref="TexturesPath"/>.
        /// </summary>
        /// <param name="relPath">Relative path to file from <see cref="TexturesPath"/>.</param>
        /// <param name="mipMaps">Enable mipmaps?</param>
        /// <param name="encodeAsNormalMap">Convert from RGB to DTXnm.</param>
        /// <param name="tex">Imported texture.</param>
        /// <returns>True if texture exists and has been imported.</returns>
        public static bool TryImportTextureFromLooseFiles(string relPath, bool mipMaps, bool encodeAsNormalMap, out Texture2D tex)
        {
            return TryImportTextureFromDisk(Path.Combine(texturesPath, relPath), mipMaps, encodeAsNormalMap, out tex);
        }

        /// <summary>
        /// Seek texture from disk using a full path.
        /// </summary>
        /// <param name="directory">Full path to texture file.</param>
        /// <param name="fileName">Name of texture file.</param>
        /// <param name="mipMaps">Enable mipmaps?</param>
        /// <param name="encodeAsNormalMap">Convert from RGB to DTXnm.</param>
        /// <param name="tex">Imported texture.</param>
        /// <param name="readOnly">Release copy on system memory after uploading to gpu.</param>
        /// <returns>True if texture exists and has been imported.</returns>
        public static bool TryImportTextureFromDisk(string path, bool mipMaps, bool encodeAsNormalMap, out Texture2D tex, bool readOnly = true)
        {
            if (!path.EndsWith(extension))
                path += extension;

            if (File.Exists(path))
            {
                tex = ImportTextureFromDisk(path, mipMaps, encodeAsNormalMap, readOnly);
                return true;
            }

            tex = null;
            return false;
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
            Texture2D metallicGloss;
            if (TryImportTextureFromLooseFiles(archive, record, frame, TextureMap.MetallicGloss, out metallicGloss))
            {
                metallicGloss.filterMode = MainFilterMode;
                material.EnableKeyword(KeyWords.MetallicGlossMap);
                material.SetTexture(Uniforms.MetallicGlossMap, metallicGloss);
            }

            // Properties
            string path = Path.Combine(texturesPath, GetName(archive, record, frame));
            if (XMLManager.XmlFileExists(path))
            {
                var xml = new XMLManager(path);

                // Metallic parameter
                float metallic;
                if (xml.TryGetFloat("metallic", out metallic))
                    material.SetFloat(Uniforms.Metallic, metallic);

                // Smoothness parameter
                float smoothness;
                if (xml.TryGetFloat("smoothness", out smoothness))
                    material.SetFloat(Uniforms.Glossiness, smoothness);
            }
        }

        /// <summary>
        /// Import textures and emission maps for all frames of this billboard. Also set other material properties from xml.
        /// </summary>
        public static void SetBillboardImportedTextures(GameObject go, ref DaggerfallBillboard.BillboardSummary summary)
        {
            if (!DaggerfallUnity.Settings.AssetInjection)
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
                meshRenderer.material.SetTexture(Uniforms.MainTex, albedo);
                if (isEmissive)
                    meshRenderer.material.SetTexture(Uniforms.EmissionMap, emission);

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
        /// Import textures for all records and frames of a mobile billboard.
        /// </summary>
        public static void SetMobileBillboardImportedTextures(int archive, MeshFilter meshFilter, ref MobileBillboardImportedTextures importedTextures)
        {
            if (!DaggerfallUnity.Settings.AssetInjection)
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

        /// <summary>
        /// Read scale from xml and apply to given vector.
        /// </summary>
        public static void SetBillboardScale(int archive, int record, ref Vector2 size)
        {
            if (!DaggerfallUnity.Settings.AssetInjection)
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
        static public bool TryCustomizeButton(ref Button button, string colorName)
        {
            Texture2D tex;
            if (!TryImportTexture(colorName, out tex))
                return false;

            // Load texture
            button.BackgroundTexture = tex;
            button.BackgroundTexture.filterMode = (FilterMode)DaggerfallUnity.Settings.GUIFilterMode;

            // Load settings from Xml
            XMLManager xml;
            if (XMLManager.TryReadXml(texturesPath, colorName, out xml))
            {
                string value;
                if (xml.TryGetString("customtext", out value))
                {
                    if (value == "true") // Set custom color for text
                        button.Label.TextColor = xml.GetColor(button.Label.TextColor);
                    else if (value == "notext") // Disable text. This is useful if text is drawn on texture
                        button.Label.Text = string.Empty;
                }
            }

            return true;
        }

        #endregion

        #region Public Helpers

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
        /// Makes texture results for given material.
        /// </summary>
        /// <param name="material">Unity material.</param>
        /// <param name="archive">Texture archive.</param>
        /// <param name="record">Record index.</param>
        /// <returns>Results for the given material.</returns>
        public static GetTextureResults MakeResults(Material material, int archive, int record)
        {
            string path = Path.Combine(DaggerfallUnity.Instance.MaterialReader.TextureReader.Arena2Path, TextureFile.IndexToFileName(archive));
            TextureFile textureFile = new TextureFile(path, FileUsage.UseMemory, true);
            return new GetTextureResults()
            {
                albedoMap = GetTextureOrDefault(material, Uniforms.MainTex),
                normalMap = GetTextureOrDefault(material, Uniforms.BumpMap),
                emissionMap = GetTextureOrDefault(material, Uniforms.EmissionMap),
                singleRect = new Rect(0, 0, 1, 1),
                isWindow = ClimateSwaps.IsExteriorWindow(archive, record),
                isEmissive = material.HasProperty(Uniforms.EmissionMap),
                textureFile = textureFile
            };
        }

        /// <summary>
        /// Assign current filtermode to all standard shader textures of the given material.
        /// </summary>
        public static void AssignFiltermode(Material material)
        {
            FilterMode filterMode = MainFilterMode;
            foreach (var property in Uniforms.Textures.Where(x => material.HasProperty(x)))
            {
                Texture tex = material.GetTexture(property);
                if (tex) tex.filterMode = filterMode;
            }
        }

        /// <summary>
        /// Seek a texture on disk inside <see cref="TexturesPath"/> without importing it.
        /// </summary>
        /// <param name="archive">Texture archive.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index.</param>
        /// <param name="textureMap">Texture type.</param>
        /// <returns>True if texture is found.</returns>
        public static bool TextureExistsAmongLooseFiles(int archive, int record, int frame = 0, TextureMap textureMap = TextureMap.Albedo)
        {
            return DaggerfallUnity.Settings.AssetInjection
                && File.Exists(Path.Combine(texturesPath, GetName(archive, record, frame, textureMap) + extension));
        }

        /// <summary>
        /// Get a safe size for a control based on resolution of img.
        /// </summary>
        public static Vector2 GetSize(Texture2D texture, string textureName, bool allowXml = false)
        {
            if (!DaggerfallUnity.Settings.AssetInjection)
                return new Vector2(texture.width, texture.height);

            if (allowXml)
            {
                // Get size from xml
                XMLManager xml;
                if (XMLManager.TryReadXml(imgPath, textureName, out xml))
                {
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
            if (!DaggerfallUnity.Settings.AssetInjection)
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
            if (DaggerfallUnity.Settings.AssetInjection)
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
        /// Seek material from mods.
        /// </summary>
        /// <param name="name">Name of material.</param>
        /// <param name="material">Imported material.</param>
        /// <returns>True if material imported.</returns>
        private static bool TryImportMaterial(string name, out Material material)
        {
            if (DaggerfallUnity.Settings.AssetInjection)
            {
                // Seek from mods
                if (ModManager.Instance != null)
                    return ModManager.Instance.TryGetAsset(name, false, out material);
            }

            material = null;
            return false;
        }

        /// <summary>
        /// Seek texture from modding locations.
        /// </summary>
        /// <param name="path">Path on disk (loose files only).</param>
        /// <param name="name">Name of texture.</param>
        /// <param name="readOnly">Release copy on system memory after uploading to gpu.</param>
        /// <param name="tex">Imported texture.</param>
        /// <remarks>
        /// The <paramref name="readOnly"/> flag is only respected by loose files. It is up to mod authors
        /// to ensure that textures from asset bundles have `Read/Write Enabled` flag set when required.
        /// </remarks>
        /// <returns>True if texture imported.</returns>
        private static bool TryImportTexture(string path, string name, bool readOnly, out Texture2D tex)
        {
            if (DaggerfallUnity.Settings.AssetInjection)
            {
                // Seek from loose files
                if (TryImportTextureFromDisk(Path.Combine(path, name), false, false, out tex, readOnly))
                    return true;

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
            if (TryImportTexture(path, getName(frame), false, out tex))
            {
                var textures = new List<Texture2D>();
                do textures.Add(tex);
                while (TryImportTexture(path, getName(++frame), false, out tex));
                texFrames = textures.ToArray();
                return true;
            }

            texFrames = null;
            return false;
        }

        /// <summary>
        /// Import data from a file on disk as a texture.
        /// </summary>
        /// <param name="path">Location of texture file.</param>
        /// <param name="fileName">Name of texture file.</param>
        /// <param name="mipMaps">Enable mipmaps?</param>
        /// <param name="encodeAsNormalMap">Convert from RGB to DTXnm.</param>
        /// <param name="readOnly">Release copy on system memory after uploading to gpu.</param>
        /// <returns>Imported texture2D.</returns>
        private static Texture2D ImportTextureFromDisk(string path, bool mipMaps = false, bool encodeAsNormalMap = false, bool readOnly = true)
        {
            // Load texture file
            Texture2D tex = new Texture2D(4, 4, TextureFormat, mipMaps);
            if (!tex.LoadImage(File.ReadAllBytes(path), readOnly && !encodeAsNormalMap))
                Debug.LogErrorFormat("Failed to import texture data at {0}", path);

            if (encodeAsNormalMap)
            {
                // RGBA to DXTnm
                Color32[] colours = tex.GetPixels32();
                for (int i = 0; i < colours.Length; i++)
                {
                    colours[i].a = colours[i].r;
                    colours[i].r = colours[i].b = colours[i].g;
                }
                tex.SetPixels32(colours);
                tex.Apply(true, readOnly);
            }

#if DEBUG_TEXTURE_FORMAT
            Debug.LogFormat("{0}: {1} - mipmaps requested: {2}, mipmaps count : {3}", fileName, tex.format, mipMaps, tex.mipmapCount);
#endif

            return tex;
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
        /// Seek albedo and, if requested, emission map from cache, loose files and mods.
        /// </summary>
        /// <param name="isEmissive">Request emission map. If missing, albedo is used as a fully emissive surface.</param>
        /// <returns>True if textures found and loaded.</returns>
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

            // Try to load from cache
            if (materialReader.GetCachedMaterialCustomBillboard(archive, record, frame, out cachedMaterial))
            {
                albedo = cachedMaterial.albedoMap;
                emission = cachedMaterial.emissionMap;
                return true;
            }

            // Try to import and save in cache
            if (TryImportTexture(archive, record, frame, out albedo))
            {
                var filterMode = MainFilterMode;

                albedo.filterMode = filterMode;
                cachedMaterial.albedoMap = albedo;

                if (isEmissive)
                {
                    if (!TryImportTexture(archive, record, frame, TextureMap.Emission, out emission))
                        emission = albedo;
                    emission.filterMode = filterMode;
                    cachedMaterial.emissionMap = emission;
                }

                materialReader.SetCachedMaterialCustomBillboard(archive, record, frame, cachedMaterial);
                return true;
            }

            return false;
        }

        private static Texture2D GetTextureOrDefault(Material material, int propertyID)
        {
            return material.HasProperty(propertyID) ? (Texture2D)material.GetTexture(propertyID) : null;
        }

        #endregion
    }
}
 