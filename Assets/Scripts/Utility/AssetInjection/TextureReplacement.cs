// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
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
 * - Support for Sky (SKYxx.DAT)
 */

//#define DEBUG_TEXTURE_FORMAT

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Items;
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
        Height,
        Emission,
        MetallicGloss,
        Mask
    }

    public enum TextureImport
    {
        None,
        LooseFiles,
        AllLocations
    }

    /// <summary>
    /// Textures for all frames of a billboard texture record.
    /// </summary>
    public struct BillboardImportedTextures
    {
        public bool HasImportedTextures;            // Contains imported textures ?
        public int FrameCount;                      // Number of frames
        public bool IsEmissive;                     // Is billboard emissive ?
        public List<Texture2D> Albedo;              // Textures for all frames.
        public List<Texture2D> Emission;            // EmissionMaps for all frames.
    }

    /// <summary>
    /// Imported textures for an archive used by a wandering npc or a foe.
    /// </summary>
    public struct MobileBillboardImportedTextures
    {
        public bool HasImportedTextures;            // Contains imported textures ?
        public bool IsEmissive;                     // Is billboard emissive ?
        public Texture2D[][] Albedo;                // Textures for all records and frames.
        public Texture2D[][] EmissionMaps;          // Emission maps for all records and frames.
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

        static readonly Type customBlendModeType = typeof(MaterialReader.CustomBlendMode);
        static readonly int ihdrIdentifier = ToInt(Encoding.UTF8.GetBytes("IHDR"), 0);

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
        /// Seek animated texture from modding locations with all frames. Gives CPU-accessible textures - use other overload if only GPU textures are needed
        /// </summary>
        /// <param name="archive">Texture archive.</param>
        /// <param name="record">Record index.</param>
        /// <param name="texFrames">Imported texture frames.</param>
        /// <returns>True if texture imported.</returns>
        public static bool TryImportTexture(int archive, int record, out Texture2D[] texFrames)
        {
            return TryImportTexture(texturesPath, frame => GetName(archive, record, frame), false, out texFrames);
        }

        /// <summary>
        /// Seek animated texture from modding locations with all frames
        /// </summary>
        /// <param name="archive">Texture archive.</param>
        /// <param name="record">Record index.</param>
        /// <param name="readOnly">Release copy on system memory after uploading to gpu.</param>
        /// <param name="texFrames">Imported texture frames.</param>
        /// <returns>True if texture imported.</returns>
        public static bool TryImportTexture(int archive, int record, bool readOnly, out Texture2D[] texFrames)
        {
            return TryImportTexture(texturesPath, frame => GetName(archive, record, frame), readOnly, out texFrames);
        }

        /// <summary>
        /// Seek texture from modding locations. Gives CPU-accessible textures - use other overload if only GPU textures are needed
        /// </summary>
        /// <param name="archive">Texture archive.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Animation frame index.</param>
        /// <param name="tex">Imported texture.</param>
        /// <returns>True if texture imported.</returns>
        public static bool TryImportTexture(int archive, int record, int frame, out Texture2D tex)
        {
            return TryImportTexture(texturesPath, GetName(archive, record, frame), false, null, out tex);
        }

        /// <summary>
        /// Seek texture from modding locations.
        /// </summary>
        /// <param name="archive">Texture archive.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Animation frame index.</param>
        /// <param name="textureMap">Texture type.</param>
        /// <param name="readOnly">Release copy on system memory after uploading to gpu.</param>
        /// <param name="tex">Imported texture.</param>
        /// <returns>True if texture imported.</returns>
        public static bool TryImportTexture(int archive, int record, int frame, TextureMap textureMap, bool readOnly, out Texture2D tex)
        {
            return TryImportTexture(texturesPath, GetName(archive, record, frame, textureMap), readOnly, textureMap, out tex);
        }

        /// <summary>
        /// Seek texture from modding locations.
        /// </summary>
        /// <param name="archive">Texture archive.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Animation frame index.</param>
        /// <param name="textureMap">Texture type.</param>
        /// <param name="textureImport">Texture import options.</param>
        /// <param name="readOnly">Release copy on system memory after uploading to gpu.</param>
        /// <param name="tex">Imported texture.</param>
        /// <returns>True if texture imported.</returns>
        public static bool TryImportTexture(int archive, int record, int frame, TextureMap textureMap, TextureImport textureImport, bool readOnly, out Texture2D tex)
        {
            tex = null;
            return (textureImport == TextureImport.AllLocations && TryImportTexture(texturesPath, GetName(archive, record, frame, textureMap), readOnly, textureMap, out tex))
                || (textureImport == TextureImport.LooseFiles && TryImportTextureFromLooseFiles(archive, record, frame, textureMap, readOnly, out tex));
        }

        /// <summary>
        /// Seek texture from modding locations with a specific dye. Gives CPU-accessible textures - use other overload if only GPU textures are needed
        /// </summary>
        /// <param name="archive">Texture archive.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Animation frame index</param>
        /// <param name="dye">Dye colour for armour, weapons, and clothing.</param>
        /// <param name="textureMap">Texture type.</param>
        /// <param name="tex">Imported texture.</param>
        /// <returns>True if texture imported.</returns>
        public static bool TryImportTexture(int archive, int record, int frame, DyeColors dye, TextureMap textureMap, out Texture2D tex)
        {
            return TryImportTexture(texturesPath, GetName(archive, record, frame, textureMap, dye), false, null, out tex);
        }

        /// <summary>
        /// Seek texture from modding locations with a specific dye. Gives CPU-accessible textures - use other overload if only GPU textures are needed
        /// </summary>
        /// <param name="archive">Texture archive.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Animation frame index</param>
        /// <param name="dye">Dye colour for armour, weapons, and clothing.</param>
        /// <param name="textureMap">Texture type.</param>
        /// <param name="readOnly">Release copy on system memory after uploading to gpu.</param>
        /// <param name="tex">Imported texture.</param>
        /// <returns>True if texture imported.</returns>
        public static bool TryImportTexture(int archive, int record, int frame, DyeColors dye, TextureMap textureMap, bool readOnly, out Texture2D tex)
        {
            return TryImportTexture(texturesPath, GetName(archive, record, frame, textureMap, dye), readOnly, null, out tex);
        }

        /// <summary>
        /// Seek texture from modding locations.
        /// </summary>
        /// <param name="name">Texture name.</param>
        /// <param name="readOnly">Release copy on system memory after uploading to gpu.</param>
        /// <param name="tex">Imported texture.</param>
        /// <returns>True if texture imported.</returns>
        public static bool TryImportTexture(string name, bool readOnly, out Texture2D tex)
        {
            return TryImportTexture(texturesPath, name, readOnly, null, out tex);
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
            return TryImportTexture(imgPath, name, readOnly, null, out tex);
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
            return TryImportTexture(cifRciPath, GetNameCifRci(name, record, frame), readOnly, null, out tex);
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
            return TryImportTexture(cifRciPath, GetNameCifRci(name, record, frame, metalType), readOnly, null, out tex);
        }

        /// <summary>
        /// Seeks a texture array asset or an archive of individual textures to merge with <see cref="Graphics.CopyTexture(Texture, Texture)"/>.
        /// NOTE: It is possible to make a texture array on the cpu with <see cref="Texture2DArray.SetPixels32(Color32[], int)"/> but current
        /// implementation doesn't use this feature. It is up to the caller to potentially do it as a fallback if this method return false.
        /// </summary>
        /// <param name="archive">The requested texture archive.</param>
        /// <param name="depth">The expected number of layer.</param>
        /// <param name="textureMap">The texture type.</param>
        /// <param name="fallbackColor">If provided is used silenty for missing layers; texture format must be RGBA32 or ARGB32.</param>
        /// <param name="textureArray">Imported or created texture array or null.</param>
        /// <returns>True if the texture array has been imported or created.</returns>
        internal static bool TryImportTextureArray(int archive, int depth, TextureMap textureMap, Color32? fallbackColor, out Texture2DArray textureArray)
        {
            if (!DaggerfallUnity.Settings.AssetInjection)
            {
                textureArray = null;
                return false;
            }

            if (ModManager.Instance && !TextureExistsAmongLooseFiles(archive, 0, 0, textureMap))
            {
                string[] names = { GetNameTexArray(archive, textureMap), GetName(archive, 0, 0, textureMap) };

                // Seek texture array or individual textures with load order.
                // If the first match is a texture array, is returned successfully.
                // If the first match is the first texture in the archive, an array is created at runtime.
                Texture texture;
                if (ModManager.Instance.TryGetAsset(names, null, out texture) && texture.dimension == TextureDimension.Tex2DArray)
                {
                    if ((textureArray = texture as Texture2DArray).depth == depth)
                        return true;

                    Debug.LogErrorFormat("{0}: expected depth {0} but got {1}.", textureArray.name, depth, textureArray.depth);
                }
            }

            // Seek individual textures from mods and loose files
            return TryMakeTextureArrayCopyTexture(archive, depth, textureMap, fallbackColor, out textureArray);
        }

        /// <summary>
        /// Seek texture from loose files.
        /// </summary>
        /// <param name="archive">Texture archive.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Animation index.</param>
        /// <param name="textureMap">Texture type.</param>
        /// <param name="readOnly">Release copy on system memory after uploading to gpu.</param>
        /// <param name="tex">Imported texture.</param>
        /// <returns>True if texture imported.</returns>
        public static bool TryImportTextureFromLooseFiles(int archive, int record, int frame, TextureMap textureMap, bool readOnly, out Texture2D tex)
        { 
            if (DaggerfallUnity.Settings.AssetInjection)
            {
                string path = Path.Combine(texturesPath, GetName(archive, record, frame, textureMap));
                return TryImportTextureFromDisk(path, true, IsLinearTextureMap(textureMap), readOnly, out tex);
            }

            tex = null;
            return false;           
        }

        /// <summary>
        /// Seek texture from loose files using a relative path from <see cref="TexturesPath"/>.
        /// </summary>
        /// <param name="relPath">Relative path to file from <see cref="TexturesPath"/>.</param>
        /// <param name="mipMaps">Enable mipmaps?</param>
        /// <param name="isLinear">This is a linear texture such as a normal map.</param>
        /// <param name="tex">Imported texture.</param>
        /// <returns>True if texture exists and has been imported.</returns>
        [Obsolete("Use overload that accepts readOnly flag.")]
        public static bool TryImportTextureFromLooseFiles(string relPath, bool mipMaps, bool isLinear, out Texture2D tex)
        {
            return TryImportTextureFromLooseFiles(Path.Combine(texturesPath, relPath), mipMaps, isLinear, false, out tex);
        }

        /// <summary>
        /// Seeks a texture from loose files using a full path or a relative path from <see cref="TexturesPath"/>.
        /// </summary>
        /// <param name="path">Path to texture file, full or relative to <see cref="TexturesPath"/>.</param>
        /// <param name="mipMaps">Enable mipmaps?</param>
        /// <param name="isLinear">This is a linear texture such as a normal map.</param>
        /// <param name="readOnly">Release copy on system memory after uploading to gpu.</param>
        /// <param name="tex">Imported texture.</param>
        /// <returns>True if texture exists and has been imported.</returns>
        public static bool TryImportTextureFromLooseFiles(string path, bool mipMaps, bool isLinear, bool readOnly, out Texture2D tex)
        {
            return TryImportTextureFromDisk(Path.IsPathRooted(path) ? path : Path.Combine(texturesPath, path), mipMaps, isLinear, readOnly, out tex);
        }

        /// <summary>
        /// Seek texture from disk using a full path.
        /// </summary>
        /// <param name="directory">Full path to texture file.</param>
        /// <param name="fileName">Name of texture file.</param>
        /// <param name="mipMaps">Enable mipmaps?</param>
        /// <param name="isLinear">This is a linear texture such as a normal map.</param>
        /// <param name="tex">Imported texture.</param>
        /// <param name="readOnly">Release copy on system memory after uploading to gpu.</param>
        /// <returns>True if texture exists and has been imported.</returns>
        [Obsolete("Use TryImportTextureFromLooseFiles()")]
        public static bool TryImportTextureFromDisk(string path, bool mipMaps, bool isLinear, out Texture2D tex, bool readOnly = true)
        {
            return TryImportTextureFromLooseFiles(path, mipMaps, isLinear, readOnly, out tex);
        }

        #endregion

        #region Textures Injection

        /// <summary>
        /// Determine if texture map is of a linear type.
        /// </summary>
        /// <param name="textureMap">Texture map type.</param>
        /// <returns>True if this texture should be loaded as linear.</returns>
        public static bool IsLinearTextureMap(TextureMap textureMap)
        {
            return textureMap == TextureMap.Normal || textureMap == TextureMap.Height || textureMap == TextureMap.MetallicGloss;
        }

        /// <summary>
        /// Import additional custom components of material.
        /// </summary>
        /// <param name="archive">Archive index</param>
        /// <param name="record">Record index</param>
        /// <param name="frame">Texture frame</param>
        /// <param name="material">Material.</param>
        public static void CustomizeMaterial(int archive, int record, int frame, Material material)
        {
            // MetallicGloss map
            Texture2D metallicGloss;
            if (TryImportTextureFromLooseFiles(archive, record, frame, TextureMap.MetallicGloss, true, out metallicGloss))
            {
                metallicGloss.filterMode = MainFilterMode;
                material.EnableKeyword(KeyWords.MetallicGlossMap);
                material.SetTexture(Uniforms.MetallicGlossMap, metallicGloss);
                material.SetFloat(Uniforms.Smoothness, 0.35f);
            }

            // Height Map
            Texture2D height;
            if (TryImportTextureFromLooseFiles(archive, record, frame, TextureMap.Height, true, out height))
            {
                height.filterMode = MainFilterMode;
                material.EnableKeyword(KeyWords.HeightMap);
                material.SetTexture(Uniforms.HeightMap, height);
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
        /// Gets a custom material for a static billboard with textures and configuration imported from mods.
        /// </summary>
        /// <param name="go">The billboard object.</param>
        /// <param name="archive">Archive index.</param>
        /// <param name="record">Record index.</param>
        /// <param name="summary">Summary data of the billboard object.</param>
        /// <param name="scale">Custom local scale for the billboard.</param>
        /// <remarks>
        /// Seek the texture for the first frame of the given record. If found, it imports all other frames.
        /// Always creates an emission map for textures marked as emissive by TextureReader, import emission maps for others only if available.
        /// </remarks>
        /// <returns>A material or null.</returns>
        public static Material GetStaticBillboardMaterial(GameObject go, int archive, int record, ref BillboardSummary summary, out Vector2 scale)
        {
            scale = Vector2.one;

            if (!DaggerfallUnity.Settings.AssetInjection)
                return null;

            //MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
            int frame = 0;

            Texture2D albedo, emission;
            if (summary.ImportedTextures.HasImportedTextures = LoadFromCacheOrImport(archive, record, frame, true, true, out albedo, out emission))
            {
                bool isEmissive = emission || DaggerfallUnity.Instance.MaterialReader.TextureReader.IsEmissive(archive, record);

                // Read xml configuration
                Vector2 uv = Vector2.zero;
                string renderMode = null;
                XMLManager xml;
                if (XMLManager.TryReadXml(texturesPath, GetName(archive, record), out xml))
                {
                    xml.TryGetString("renderMode", out renderMode);
                    isEmissive |= xml.GetBool("emission");

                    // Set billboard scale
                    Transform transform = go.GetComponent<Transform>();
                    scale = transform.localScale = xml.GetVector3("scaleX", "scaleY", transform.localScale);

                    // Get UV
                    uv = xml.GetVector2("uvX", "uvY", uv);
                }

                // Make material
                Material material = MakeBillboardMaterial(renderMode);
                summary.Rect = new Rect(uv.x, uv.y, 1 - 2 * uv.x, 1 - 2 * uv.y);

                // Set textures on material; emission is always overriden, with actual texture or null.
                material.SetTexture(Uniforms.MainTex, albedo);
                material.SetTexture(Uniforms.EmissionMap, isEmissive ? emission ?? albedo : null);
                ToggleEmission(material, isEmissive);

                // Import animation frames
                var albedoTextures = new List<Texture2D>();
                var emissionTextures = isEmissive ? new List<Texture2D>() : null;
                do
                {
                    albedoTextures.Add(albedo);
                    if (isEmissive)
                        emissionTextures.Add(emission ?? albedo);
                }
                while (LoadFromCacheOrImport(archive, record, ++frame, isEmissive, true, out albedo, out emission));

                // Save results
                summary.ImportedTextures.FrameCount = frame;
                summary.ImportedTextures.IsEmissive = isEmissive;
                summary.ImportedTextures.Albedo = albedoTextures;
                summary.ImportedTextures.Emission = emissionTextures;

                return material;
            }

            return null;
        }

        /// <summary>
        /// Gets a custom material for a mobile billboard with textures and configuration imported from mods.
        /// </summary>
        /// <param name="archive">Archive index.</param>
        /// <param name="meshFilter">The MeshFilter of the billboard object.</param>
        /// <param name="importedTextures">All the imported textures for the archive.</param>
        /// <remarks>
        /// Seek the texture for the first frame of the first record. If found, it imports the entire archive.
        /// If this texture has an emission map the material is considered emissive and all emission maps are imported.
        /// </remarks>
        /// <returns>A material or null.</returns>
        public static Material GetMobileBillboardMaterial(int archive, MeshFilter meshFilter, ref MobileBillboardImportedTextures importedTextures)
        {
            if (!DaggerfallUnity.Settings.AssetInjection)
                return null;

            Texture2D tex, emission;
            if (importedTextures.HasImportedTextures = LoadFromCacheOrImport(archive, 0, 0, allowEmissionMap: true, readOnly: true, out tex, out emission))
            {
                string renderMode = null;

                // Read xml configuration
                XMLManager xml;
                if (XMLManager.TryReadXml(ImagesPath, string.Format("{0:000}", archive), out xml))
                {
                    xml.TryGetString("renderMode", out renderMode);
                    importedTextures.IsEmissive = xml.GetBool("emission");
                }

                // Make material
                Material material = MakeBillboardMaterial(renderMode);

                // Enable emission
                ToggleEmission(material, importedTextures.IsEmissive |= emission != null);

                // If the archive has a Arena2 texture file, use it to get record and frame count
                string fileName = TextureFile.IndexToFileName(archive);
                var textureFile = new TextureFile();
                if (textureFile.Load(Path.Combine(DaggerfallUnity.Instance.Arena2Path, fileName), FileUsage.UseMemory, readOnly: true))
                {
                    // Import all textures in this archive
                    importedTextures.Albedo = new Texture2D[textureFile.RecordCount][];
                    importedTextures.EmissionMaps = importedTextures.IsEmissive ? new Texture2D[textureFile.RecordCount][] : null;
                    for (int record = 0; record < textureFile.RecordCount; record++)
                    {
                        int frames = textureFile.GetFrameCount(record);
                        var frameTextures = new Texture2D[frames];
                        var frameEmissionMaps = importedTextures.IsEmissive ? new Texture2D[frames] : null;

                        for (int frame = 0; frame < frames; frame++)
                        {
                            if (record != 0 || frame != 0)
                                LoadFromCacheOrImport(archive, record, frame, importedTextures.IsEmissive, true, out tex, out emission);

                            frameTextures[frame] = tex ?? ImageReader.GetTexture(fileName, record, frame, true);
                            if (frameEmissionMaps != null)
                                frameEmissionMaps[frame] = emission ?? frameTextures[frame];
                        }

                        importedTextures.Albedo[record] = frameTextures;
                        if (importedTextures.EmissionMaps != null)
                            importedTextures.EmissionMaps[record] = frameEmissionMaps;
                    }
                }
                // Otherwise, check what files are available in the injected assets
                else
                {
                    List<Texture2D[]> allAlbedo = new List<Texture2D[]>();
                    List<Texture2D[]> allEmission = importedTextures.IsEmissive ? new List<Texture2D[]>() : null;

                    int record = 0;
                    while (TryImportTexture(archive, record, readOnly: true, out Texture2D[] currentAlbedo))
                    {
                        allAlbedo.Add(currentAlbedo);

                        if (importedTextures.IsEmissive)
                        {
                            if (TryImportTexture(texturesPath, frame => GetName(archive, record, frame, TextureMap.Emission), readOnly: true, out Texture2D[] currentEmissive))
                            {
                                allEmission.Add(currentEmissive);
                            }
                            else
                            {
                                allEmission.Add(currentAlbedo);
                            }
                        }

                        ++record;
                    }

                    importedTextures.Albedo = allAlbedo.ToArray();
                    importedTextures.EmissionMaps = importedTextures.IsEmissive ? allEmission.ToArray() : null;
                }

                // Update UV map
                SetUv(meshFilter);

                return material;
            }

            return null;
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
                Vector2 scale = xml.GetVector2("scaleX", "scaleY", Vector2.one);
                size.x *= scale.x;
                size.y *= scale.y;
            }
        }

        /// <summary>
        /// Import custom texture and label settings for buttons.
        /// This feature has been deprecated in favor of <see cref="DaggerfallWorkshop.Game.UserInterfaceWindows.UIWindowFactory"/>.
        /// </summary>
        /// <param name="button">Button</param>
        /// <param name="colorName">Name of texture</param>
        [Obsolete("This feature has been deprecated in favor of UIWindowFactory.")]
        public static bool TryCustomizeButton(ref Button button, string colorName)
        {
            Texture2D tex;
            if (!TryImportTexture(colorName, true, out tex))
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

            LogLegacyUICustomizationMessage(colorName);
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
        /// <param name="textureMap">Texture type.</param>
        /// <param name="dye">Color Dye.</param>
        /// <returns>The name for the texture with requested options.</returns>
        public static string GetName(int archive, int record, int frame = 0, TextureMap textureMap = TextureMap.Albedo, DyeColors dye = DyeColors.Unchanged)
        {
            string name = string.Format("{0:000}_{1}-{2}", archive, record, frame);

            if (dye != DyeColors.Unchanged)
                name = string.Format("{0}_{1}", name, dye);

            if (textureMap != TextureMap.Albedo)
                name = string.Format("{0}_{1}", name, textureMap);

            return name;
        }

        /// <summary>
        /// Gets name for a texture array.
        /// </summary>
        /// <param name="archive">Archive index from TEXTURE.XXX</param>
        /// <param name="textureMap">Texture type.</param>
        /// <returns>The name for the texture array with requested options.</returns>
        public static string GetNameTexArray(int archive, TextureMap textureMap = TextureMap.Albedo)
        {
            string name = string.Format("{0:000}-TexArray", archive);

            if (textureMap != TextureMap.Albedo)
                name = string.Format("{0}_{1}", name, textureMap);

            return name;
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

        /// <summary>
        /// Read configuration for a paperdoll item with custom rect.
        /// </summary>
        /// <param name="item">Target item or null.</param>
        /// <param name="imageData">Source image data.</param>
        /// <param name="rect">Rect for the item on paperdoll.</param>
        internal static void OverridePaperdollItemRect(DaggerfallUnityItem item, ImageData imageData, float paperdollScale, ref Rect rect)
        {
            DyeColors dyeColor = item != null ? item.dyeColor : DyeColors.Unchanged;

            string directory;
            string name;
            XMLManager xml;
            if (MakeName(imageData, dyeColor, out directory, out name) && XMLManager.TryReadXml(directory, name, out xml))
                rect = xml.GetRect("rect", rect, paperdollScale);
        }

        /// <summary>
        /// Parses the ID from the name of a texture archive from classic Daggerfall.
        /// </summary>
        /// <param name="filename">A name with format <c>"TEXTURE.XXX"</c>.</param>
        /// <returns>The number parsed from <c>"XXX"</c>.</returns>
        /// <seealso cref="TextureFile.IndexToFileName(int)"/>
        public static int FileNameToArchive(string filename)
        {
            return int.Parse(filename.Substring("TEXTURE.".Length));
        }

        #endregion

        #region  Internal Methods

        internal static void LogLegacyUICustomizationMessage(string textureName)
        {
            Debug.LogWarningFormat("Imported texture {0} for legacy support of UI customization. This feature has been deprecated in favor of UIWindowFactory.", textureName);
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
                    return ModManager.Instance.TryGetAsset(name, null, out material);
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
        /// <param name="textureMap">The texture type. Can pass null if type not known.</param>
        /// <param name="tex">Imported texture.</param>
        /// <remarks>
        /// The <paramref name="readOnly"/> flag is only respected by loose files. It is up to mod authors
        /// to ensure that textures from asset bundles have `Read/Write Enabled` flag set when required.
        /// </remarks>
        /// <returns>True if texture imported.</returns>
        private static bool TryImportTexture(string path, string name, bool readOnly, TextureMap? textureMap, out Texture2D tex)
        {
            bool isLinear = textureMap != null && IsLinearTextureMap(textureMap.Value);
            if (DaggerfallUnity.Settings.AssetInjection)
            {
                // Seek from loose files
                if (TryImportTextureFromDisk(Path.Combine(path, name), false, isLinear, readOnly, out tex))
                    return true;

                // Seek from mods
                if (ModManager.Instance && ModManager.Instance.TryGetAsset(name, null, out tex))
                {
                    if (!readOnly && !tex.isReadable)
                        Debug.LogWarning($"Texture {name} is not readable.");

                    return true;
                }
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
        private static bool TryImportTexture(string path, Func<int, string> getName, bool readOnly, out Texture2D[] texFrames)
        {
            int frame = 0;
            Texture2D tex;
            if (TryImportTexture(path, getName(frame), readOnly, null, out tex))
            {
                var textures = new List<Texture2D>();
                do textures.Add(tex);
                while (TryImportTexture(path, getName(++frame), readOnly, null, out tex));
                texFrames = textures.ToArray();
                return true;
            }

            texFrames = null;
            return false;
        }

        /// <summary>
        /// Import texture data from disk with a full path to file.
        /// </summary>
        /// <param name="directory">Full path to texture file.</param>
        /// <param name="fileName">Name of texture file.</param>
        /// <param name="mipMaps">Enable mipmaps?</param>
        /// <param name="isLinear">This is a linear texture such as a normal map.</param>
        /// <param name="readOnly">Release copy on system memory after uploading to gpu.</param>
        /// <param name="tex">Imported texture.</param>
        /// <returns>True if texture exists and has been imported.</returns>
        private static bool TryImportTextureFromDisk(string path, bool mipMaps, bool isLinear, bool readOnly, out Texture2D tex)
        {
            const int retroThreshold = 256; // Imported textures with a width or height below this threshold will never be compressed to preserve retro appearance

            if (!path.EndsWith(extension))
                path += extension;

            if (File.Exists(path))
            {
                byte[] bytes = File.ReadAllBytes(path);
                Vector2Int resolution = FindPngResolution(bytes) ?? throw new Exception($"Failed to find PNG resolution for {path}");
                TextureFormat textureFormat = resolution.x < retroThreshold || resolution.y < retroThreshold ? TextureFormat.ARGB32 : TextureFormat;

                tex = new Texture2D(4, 4, textureFormat, mipMaps, isLinear);
                if (!tex.LoadImage(bytes, readOnly))
                    Debug.LogError($"Failed to import texture data at {path}");

#if DEBUG_TEXTURE_FORMAT
                Debug.LogFormat("{0}: format: {1}, mipmaps: {2}, mipmaps count: {3}", Path.GetFileName(path), tex.format, mipMaps, tex.mipmapCount);
#endif

                tex.filterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode;
                return true;
            }

            tex = null;
            return false;
        }

        /// <summary>
        /// Makes a texture array from textures loaded from mods, using <see cref="Graphics.CopyTexture"/> which is very efficient.
        /// The result texture array respects format and settings of individual textures and is not available on the cpu side.
        /// </summary>
        /// <remarks>
        /// <see cref="Graphics.CopyTexture"/> provides a very fast copy operation between textures, but is not supported on some platforms.
        /// The result is also not available for further edits from the cpu.
        /// The first record must be available and defines size and format that all records must match. A fallback color can be provided for other layers.
        /// </remarks>
        /// <param name="archive">The requested texture archive.</param>
        /// <param name="depth">The expected number of layer.</param>
        /// <param name="textureMap">The texture type.</param>
        /// <param name="fallbackColor">If provided is used silenty for missing layers; texture format must be RGBA32 or ARGB32.</param>
        /// <param name="textureArray">The created texture array or null.</param>
        /// <returns>True if the texture array has been created.</returns>
        private static bool TryMakeTextureArrayCopyTexture(int archive, int depth, TextureMap textureMap, Color32? fallbackColor, out Texture2DArray textureArray)
        {
            textureArray = null;

            if ((SystemInfo.copyTextureSupport & CopyTextureSupport.DifferentTypes) == CopyTextureSupport.None)
                return false;

            bool mipMaps = false;
            Texture2D fallback = null;

            for (int record = 0; record < depth; record++)
            {
                Texture2D tex;
                if (!TryImportTexture(archive, record, 0, textureMap, true, out tex))
                {
                    if (!textureArray)
                        return false;

                    if (!fallbackColor.HasValue)
                    {
                        Debug.LogErrorFormat("Failed to inject record {0} for texture archive {1} ({2}) because texture data is not available.", record, archive, textureMap);
                        continue;
                    }

                    if (!fallback)
                    {
                        fallback = new Texture2D(textureArray.width, textureArray.height, textureArray.format, mipMaps, IsLinearTextureMap(textureMap));
                        fallback.filterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode;
                        Color32[] colors = new Color32[fallback.width * fallback.height];
                        for (int i = 0; i < colors.Length; i++)
                            colors[i] = fallbackColor.Value;
                        fallback.SetPixels32(colors);
                        fallback.Apply(mipMaps, true);
                    }

                    tex = fallback;
                }

                if (!textureArray)
                {
                    if (fallbackColor.HasValue && tex.format != TextureFormat.RGBA32 && tex.format != TextureFormat.ARGB32)
                        return false;

                    textureArray = new Texture2DArray(tex.width, tex.height, depth, tex.format, mipMaps = tex.mipmapCount > 1, IsLinearTextureMap(textureMap));
                    textureArray.filterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode;
                }

                if (tex.width == textureArray.width && tex.height == textureArray.height && tex.format == textureArray.format)
                    Graphics.CopyTexture(tex, 0, textureArray, record);
                else
                    Debug.LogErrorFormat("Failed to inject record {0} for texture archive {1} ({2}) due to size or format mismatch.", record, archive, textureMap);
            }

            if (fallback)
                Texture2D.Destroy(fallback);

            if (textureArray)
            {
                textureArray.wrapMode = TextureWrapMode.Clamp;
                textureArray.anisoLevel = 8;
                return true;
            }

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
        /// Seek albedo and, if requested, emission map from cache, loose files and mods.
        /// </summary>
        /// <param name="archive">Archive index.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Texture frame.</param>
        /// <param name="allowEmissionMap">Should the emission map be seeked?</param>
        /// <param name="readOnly">Release copy on system memory after uploading to gpu.</param>
        /// <param name="albedo">Imported albedo or null.</param>
        /// <param name="emission">Imported emission map or null.</param>
        /// <returns>True if textures found and loaded.</returns>
        private static bool LoadFromCacheOrImport(
            int archive,
            int record,
            int frame,
            bool allowEmissionMap,
            bool readOnly,
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
            if (TryImportTexture(archive, record, frame, TextureMap.Albedo, readOnly, out albedo))
            {
                var filterMode = MainFilterMode;

                albedo.filterMode = filterMode;
                cachedMaterial.albedoMap = albedo;

                if (allowEmissionMap && TryImportTexture(archive, record, frame, TextureMap.Emission, readOnly, out emission))
                {
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

        private static void ToggleEmission(Material material, bool isEmissive)
        {
            bool isEnabled = material.IsKeywordEnabled(KeyWords.Emission);

            if (isEmissive)
            {
                if (!isEnabled)
                {
                    material.EnableKeyword(KeyWords.Emission);
                    material.SetColor(Uniforms.EmissionColor, Color.white);
                }
            }
            else
            {
                if (isEnabled)
                    material.DisableKeyword(KeyWords.Emission);
            }
        }

        private static Material MakeBillboardMaterial(string renderMode = null)
        {
            // Parse blendMode from string or use Cutout if no custom blendMode specified
            MaterialReader.CustomBlendMode blendMode =
                renderMode != null && Enum.IsDefined(customBlendModeType, renderMode) ?
                (MaterialReader.CustomBlendMode)Enum.Parse(customBlendModeType, renderMode) :
                MaterialReader.CustomBlendMode.Cutout;

            // Use Daggerfall/Billboard material for standard cutout billboards or create a Standard material if using any other custom blendMode
            if (blendMode == MaterialReader.CustomBlendMode.Cutout)
                return MaterialReader.CreateBillboardMaterial();
            else
                return MaterialReader.CreateStandardMaterial(blendMode);
        }

        private static bool MakeName(ImageData imageData, DyeColors dyeColor, out string directory, out string name)
        {
            switch (imageData.type)
            {
                case ImageTypes.TEXTURE:
                    directory = texturesPath;
                    int archive = FileNameToArchive(imageData.filename);
                    name = GetName(archive, imageData.record, imageData.frame, TextureMap.Albedo, dyeColor);
                    return true;

                case ImageTypes.IMG:
                    directory = imgPath;
                    name = imageData.filename;
                    return true;

                case ImageTypes.CIF:
                case ImageTypes.RCI:
                    directory = cifRciPath;
                    name = GetNameCifRci(imageData.filename, imageData.record, imageData.frame);
                    return true;

                default:
                    directory = null;
                    name = null;
                    return false;
            }
        }

        /// <summary>
        /// Finds the resolution of a png image from its byte array content.
        /// </summary>
        /// <param name="bytes">Content of png file.</param>
        /// <returns>Vector where x is the width and y is the height.</returns>
        private static Vector2Int? FindPngResolution(byte[] bytes)
        {
            // Specifications: http://www.libpng.org/pub/png/spec/iso/index-object.html#11IHDR
            // The IHDR chunk shall be the first chunk in the PNG datastream. It contains:
            // Width	4 bytes
            // Height	4 bytes

            const int widthOffset = 1 * 4;
            const int heightOffset = 2 * 4;

            for (int i = 0; i <= bytes.Length - 12; i += 4)
            {
                if (ToInt(bytes, i) == ihdrIdentifier)
                    return new Vector2Int(ToInt(bytes, i + widthOffset), ToInt(bytes, i + heightOffset));
            }

            return null;
        }

        /// <summary>
        /// Converts four bytes to int (big endian).
        /// </summary>
        /// <param name="bytes">Array of bytes.</param>
        /// <param name="startIndex">Start index in the array.</param>
        /// <returns>Int value.</returns>
        private static int ToInt(byte[] bytes, int startIndex)
        {
            return (bytes[startIndex] << 24) | (bytes[startIndex + 1] << 16) | (bytes[startIndex + 2] << 8) | bytes[startIndex + 3];
        }

        #endregion
    }
}
 