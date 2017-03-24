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
 * 1. StreamingWorld billboards
 * 2. PaperDoll CharacterLayer textures works only if resolution is the same as vanilla 
 *        (http://forums.dfworkshop.net/viewtopic.php?f=22&p=3547&sid=6a99dbcffad1a15b08dd5e157274b772#p3547)
 * 3. Terrain textures
 */

using System.IO;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Handles import and injection of custom textures and images
    /// with the purpose of providing modding support.
    /// </summary>
    static public class TextureReplacement
    {
        #region Fields & Structs

        // Paths
        static public string texturesPath = Path.Combine(Application.streamingAssetsPath, "Textures");
        static public string imgPath = Path.Combine(texturesPath, "img");
        static public string cifPath = Path.Combine(texturesPath, "cif");

        // Map tags
        const string NormalTag =  "_Normal";
        const string EmissionTag = "_Emission";
        const string MetallicGlossTag = "_MetallicGloss";

        // Structs

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

        /// <summary>
        /// Custom textures for enemies.
        /// </summary>
        public struct CustomEnemyMaterial
        {
            public bool isCustom;                        // True if enemy uses custom textures
            public List<List<Texture2D>> MainTexture;    // Textures
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
            return TextureFileExist(texturesPath, archive.ToString() + "_" + record.ToString() + "-" + frame.ToString());
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
            return ImportTextureFile(texturesPath, archive.ToString() + "_" + record.ToString() + "-" + frame.ToString());
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
            return TextureFileExist(cifPath, filename + "_" + record.ToString() + "-" + frame.ToString());
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
            return ImportTextureFile(cifPath, filename + "_" + record.ToString() + "-" + frame.ToString());
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
            if (metalType == MetalTypes.None)
                return CustomCifExist(filename, record, frame);

            return TextureFileExist(cifPath, filename + "_" + record.ToString() + "-" + frame.ToString() + "_" + metalType);
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
            if (metalType == MetalTypes.None)
                return LoadCustomCif(filename, record, frame);

            return ImportTextureFile(cifPath, filename + "_" + record.ToString() + "-" + frame.ToString() + "_" + metalType);
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
            return TextureFileExist(texturesPath, archive.ToString() + "_" + record.ToString() + "-" + frame.ToString() + NormalTag);
        }

        /// <summary>
        /// Search for image file on disk to use as normal map
        /// (name_Normal.png).
        /// </summary>
        /// <param name="name">Name of texture.</param>
        /// <returns>True if normal map exists.</returns>
        static public bool CustomNormalExist(string name)
        {
            return TextureFileExist(texturesPath, name + NormalTag);
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
            return LoadCustomNormal(archive.ToString() + "_" + record.ToString() + "-" + frame.ToString());
        }

        /// <summary>
        /// Import image file from disk to use as normal map
        /// (name_Normal.png).
        /// </summary>
        /// <param name="name">Name of texture.</param>
        /// <returns>Normal map.</returns>
        static public Texture2D LoadCustomNormal(string name)
        {
            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, true); //create empty texture, size will be the actual size of .png file
            tex.LoadImage(File.ReadAllBytes(Path.Combine(texturesPath, name + NormalTag)));

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
        /// Search for image file on disk to use as emission map
        /// (archive_record-frame_Emission.png, for example '112_3-0_Emission.png).
        /// </summary>
        /// <param name="archive">Archive index from TEXTURE.XXX</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for animated billboards.</param>
        /// <returns>True if emission map exists.</returns>
        static public bool CustomEmissionExist(int archive, int record, int frame)
        {
            return TextureFileExist(texturesPath, archive.ToString() + "_" + record.ToString() + "-" + frame.ToString() + EmissionTag);
        }

        /// <summary>
        /// Search for image file on disk to use as emission map
        /// (name_Emission.png)
        /// </summary>
        /// <param name="name">Name of texture.</param>
        /// <returns>True if emission map exists.</returns>
        static public bool CustomEmissionExist(string name)
        {
            return TextureFileExist(texturesPath, name + EmissionTag);
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
            return ImportTextureFile(texturesPath, archive.ToString() + "_" + record.ToString() + "-" + frame.ToString() + EmissionTag);
        }

        /// <summary>
        /// Import image file from disk to use as emission map
        /// (name_Emission.png)
        /// </summary>
        /// <param name="name">Name of texture.</param>
        /// <returns>Emission map.</returns>
        static public Texture2D LoadCustomEmission(string name)
        {
            return ImportTextureFile(texturesPath, name + EmissionTag);
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
            return TextureFileExist(texturesPath, archive.ToString() + "_" + record.ToString() + "-" + frame.ToString() + MetallicGlossTag);
        }

        /// <summary>
        /// Search for image file on disk to use as metallic map
        /// (name_MetallicGloss.png).
        /// </summary>
        /// <param name="name">Name of texture.</param> 
        /// <returns>True if MetallicGloss map exist.</returns>
        static public bool CustomMetallicGlossExist(string name)
        {
            return TextureFileExist(texturesPath, name + MetallicGlossTag);
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
            return ImportTextureFile(texturesPath, archive.ToString() + "_" + record.ToString() + "-" + frame.ToString() + MetallicGlossTag);
        }

        /// <summary>
        /// Import image file from disk to use as MetallicGloss map
        /// (name_MetallicGloss.png).
        /// </summary>
        /// <param name="name">Name of texture.</param>
        /// <returns>MetallicGloss map.</returns>
        static public Texture2D LoadCustomMetallicGloss(string name)
        {
            return ImportTextureFile(texturesPath, name + MetallicGlossTag);
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
            results.albedoMap = LoadCustomTexture(archive, record, frame);

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
                if (CustomEmissionExist(archive, record, frame)) //import emission texture
                    results.emissionMap = LoadCustomEmission(archive, record, frame);
                else if (!results.isWindow) //reuse albedo map for basic colour emission
                    results.emissionMap = results.albedoMap;
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
        /// Replace texture(s) on billboard gameobject.
        /// This is implemented only for interior and dungeon billboards for now
        /// </summary>
        /// <paran name="go">Billboard gameobject.</param>
        /// <param name="archive">Archive index.</param>
        /// <param name="record">Record index.</param>
        static public void LoadCustomBillboardTexture(GameObject go, int archive, int record)
        {
            // Get MeshRenderer
            MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();

            // UV default values
            var uv = Vector2.zero;

            // Customize billboard size (scale)
            if (XMLManager.XmlFileExist(archive, record))
            {
                // Get name of file
                string name = GetName(archive, record);

                // Set scale
                Transform transform = go.GetComponent<Transform>();
                transform.localScale = XMLManager.GetScale(name, texturesPath, transform.localScale);
                go.GetComponent<DaggerfallBillboard>().SetCustomSize(archive, record, transform.localScale.y);

                // Get UV
                uv = XMLManager.GetUv(name, texturesPath, uv.x, uv.y);
            }

            // Update UV map
            MeshFilter meshFilter = go.GetComponent<MeshFilter>();
            UpdateUV(meshFilter, uv.x, uv.y);

            // Check if billboard is emissive
            bool isEmissive = false;
            if (meshRenderer.materials[0].GetTexture("_EmissionMap") != null)
                isEmissive = true;

            // Import texture(s)
            Texture2D albedoTexture, emissionMap;
            LoadCustomBillboardFrameTexture(isEmissive, out albedoTexture, out emissionMap, archive, record);

            // Main texture
            meshRenderer.materials[0].SetTexture("_MainTex", albedoTexture);

            // Emission maps for lights
            if (isEmissive)
                meshRenderer.materials[0].SetTexture("_EmissionMap", emissionMap);

            // Check if billboard is animated
            int NumberOfFrames = NumberOfAvailableFrames(archive, record);
            if (NumberOfFrames > 1)
            {
                // Import textures for each frame 
                go.GetComponent<DaggerfallBillboard>().SetCustomMaterial(archive, record, NumberOfFrames, isEmissive);
            }
        }

        static public void SetupCustomEnemyMaterial(ref MeshRenderer meshRenderer, ref MeshFilter meshFilter, int archive)
        {
            // Set Main Texture
            Texture2D albedoTexture = LoadCustomTexture(archive, 0, 0);
            albedoTexture.filterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode;
            meshRenderer.material.mainTexture = albedoTexture;

            // Update UV map
            UpdateUV(meshFilter);
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
        /// <returns></returns>
        static public string GetName (int archive, int record, int frame = 0)
        {
            return archive.ToString() + "_" + record.ToString() + "-" + frame.ToString();
        }

        /// <summary>
        /// Import texture(s) for billboard gameobject for specified frame. 
        /// </summary>
        /// <paran name="isEmissive">True for lights.</param>
        /// <paran name="albedoTexture">Main texture for this frame.</param>
        /// <paran name="emissionMap">Eventual Emission map for this frame.</param>
        /// <param name="archive">Archive index.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for animated billboards.</param>
        static public void LoadCustomBillboardFrameTexture(bool isEmissive, out Texture2D albedoTexture, out Texture2D emissionMap, int archive, int record, int frame = 0)
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

        #endregion
    }
}
 