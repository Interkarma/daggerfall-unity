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
 * 1. Exterior billboards
 * 2. PaperDoll CharacterLayer textures works only if resolution is the same as vanilla 
 *        (http://forums.dfworkshop.net/viewtopic.php?f=22&p=3547&sid=6a99dbcffad1a15b08dd5e157274b772#p3547)
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

        static public string texturesPath = Path.Combine(Application.streamingAssetsPath, "Textures");
        static public string imgPath = Path.Combine(texturesPath, "img");
        static public string cifPath = Path.Combine(texturesPath, "cif");

        public struct CustomBillboard
        {
            public List<Texture2D> MainTexture;          // List of custom albedo maps
            public List<Texture2D> EmissionMap;          // List of custom emission maps
            public bool isEmissive;                      // True if billboard is emissive
            public int NumberOfFrames;                   // number of frame textures avilable on disk
        }

        public struct CustomEnemyMaterial
        {
            public bool isCustom;                        // True if enemy uses custom textures
            public List<List<Texture2D>> MainTexture;    // Textures
        }

        #endregion

        #region Textures import

        /// <summary>
        /// Load custom image files from disk to use as textures on models or billboards
        /// .png files are located in persistentData/textures
        /// and are named 'archive_record-frame.png' 
        /// for example '86_3-0.png'
        /// </summary>
        /// <param name="archive">Archive index from TEXTURE.XXX</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for animated billboards</param>

        /// check if file exist on disk. 
        /// <returns>Bool</returns>
        static public bool CustomTextureExist(int archive, int record, int frame = 0)
        {
            return TextureFileExist(texturesPath, archive.ToString() + "_" + record.ToString() + "-" + frame.ToString());
        }

        static public bool CustomTextureExist(string name)
        {
            return TextureFileExist(texturesPath, name);
        }

        /// import custom image as texture2D
        /// <returns>Texture2D</returns>
        static public Texture2D LoadCustomTexture(int archive, int record, int frame)
        {
            return ImportTextureFile(texturesPath, archive.ToString() + "_" + record.ToString() + "-" + frame.ToString());
        }

        static public Texture2D LoadCustomTexture(string name)
        {
            return ImportTextureFile(texturesPath, name);
        }

        /// <summary>
        /// Load custom image files from disk to replace .IMGs. Useful for customizing the UI
        /// .png files are located in persistentdata/textures/img
        /// and are named 'imagefile.png' 
        /// for example 'REST02I0.IMG.png'
        /// </summary>
        /// <param name="filename">Name of standalone file as it appears in arena2 folder.</param>

        /// check if file exist on disk. 
        /// <returns>Bool</returns>
        static public bool CustomImageExist(string filename)
        {
            return TextureFileExist(imgPath, filename);
        }

        /// load custom image as texture2D
        /// <returns>Texture2D.</returns>
        static public Texture2D LoadCustomImage(string filename)
        {
            return ImportTextureFile(imgPath, filename);
        }

        /// <summary>
        /// Load custom image files from disk to replace .CIFs and .RCIs
        /// .png files are located in persistentdata/textures/cif
        /// and are named 'CifFile_record-frame.png' 
        /// for example 'INVE16I0.CIF_1-0.png'
        /// </summary>
        /// <param name="filename">Name of standalone file as it appears in arena2 folder.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for weapon animations (WEAPONXX.CIF) </param>

        /// check if file exist on disk. 
        /// <returns>Bool</returns>
        static public bool CustomCifExist(string filename, int record, int frame = 0)
        {
            return TextureFileExist(cifPath, filename + "_" + record.ToString() + "-" + frame.ToString());
        }

        /// load custom image as texture2D
        /// <returns>Texture2D.</returns>
        static public Texture2D LoadCustomCif(string filename, int record, int frame)
        {
            return ImportTextureFile(cifPath, filename + "_" + record.ToString() + "-" + frame.ToString());
        }

        /// Check if file exist on disk for a specific metal type. 
        /// Ex: WEAPON04.CIF_0-0.Png for Iron --> WEAPON04.CIF-1_0-0.Png
        /// <returns>Bool</returns>
        static public bool CustomCifExist(string filename, int record, int frame, MetalTypes metalType)
        {
            if (metalType == MetalTypes.None)
                return CustomCifExist(filename, record, frame);

            return CustomCifExist(filename + "-" + (int)metalType, record, frame);
        }

        /// Load custom image as texture2D for a specific metal type.
        /// <returns>Texture2D.</returns>
        static public Texture2D LoadCustomCif(string filename, int record, int frame, MetalTypes metalType)
        {
            if (metalType == MetalTypes.None)
                return LoadCustomCif(filename, record, frame);

            return LoadCustomCif(filename + "-" + (int)metalType, record, frame);
        }

        /// <summary>
        /// Load custom image files from disk to use as normal maps
        /// .png files are located in persistentData/textures
        /// and are named 'archive_record-frame_Normal.png' 
        /// for example '112_3-0_Normal.png'
        /// </summary>
        /// <param name="archive">Archive index from TEXTURE.XXX</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for animated billboards</param>

        /// check if file exist on disk. 
        /// <returns>Bool</returns>
        static public bool CustomNormalExist(int archive, int record, int frame)
        {
            return TextureFileExist(texturesPath, archive.ToString() + "_" + record.ToString() + "-" + frame.ToString() + "_Normal");
        }

        static public bool CustomNormalExist(string name)
        {
            return TextureFileExist(texturesPath, name + "_Normal");
        }

        /// import custom image as texture2D
        /// <returns>Texture2D</returns>
        static public Texture2D LoadCustomNormal(int archive, int record, int frame)
        {
            return LoadCustomNormal(archive.ToString() + "_" + record.ToString() + "-" + frame.ToString());
        }

        static public Texture2D LoadCustomNormal(string name)
        {
            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, true); //create empty texture, size will be the actual size of .png file
            tex.LoadImage(File.ReadAllBytes(Path.Combine(texturesPath, name + "_Normal.png")));

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
        /// Load custom image files from disk to use as emission maps
        /// This is useful for walls, where only the windows emits light
        /// .png files are located in persistentData/textures
        /// and are named 'archive_record-frame_Emission.png' 
        /// for example '112_3-0_Emission.png'
        /// </summary>
        /// <param name="archive">Archive index from TEXTURE.XXX</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index. It's different than zero only for animated billboards</param>

        /// check if file exist on disk. 
        /// <returns>Bool</returns>
        static public bool CustomEmissionExist(int archive, int record, int frame)
        {
            return TextureFileExist(texturesPath, archive.ToString() + "_" + record.ToString() + "-" + frame.ToString() + "_Emission");
        }

        /// import custom image as texture2D
        /// <returns>Texture2D</returns>
        static public Texture2D LoadCustomEmission(int archive, int record, int frame)
        {
            return ImportTextureFile(texturesPath, archive.ToString() + "_" + record.ToString() + "-" + frame.ToString() + "_Emission");
        }

        #endregion

        #region Texture injection

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

            // Customize billboard size (scale)
            string XmlFile = archive + "_" + record + "-0";
            if (File.Exists(Path.Combine(texturesPath, XmlFile + ".xml")))
            {
                // Get current scale
                Transform transform = go.GetComponent<Transform>();
                Vector3 scale = transform.localScale;

                // Get new scale
                scale.x = XMLManager.GetColorValue(XmlFile, "scaleX");
                scale.y = XMLManager.GetColorValue(XmlFile, "scaleY");

                // Set new scale
                transform.localScale = scale;
                go.GetComponent<DaggerfallBillboard>().SetCustomSize(archive, record, scale.y);
            }

            // Update UV map
            MeshFilter meshFilter = go.GetComponent<MeshFilter>();
            UpdateUV(ref meshFilter);

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
            UpdateUV(ref meshFilter);
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

            // Load settings from Xml (if present)
            // Set custom color
            if (XMLManager.GetString(colorName, "customtext", false) == "true")
                button.Label.TextColor = new Color(XMLManager.GetColorValue(colorName, "r"), 
                    XMLManager.GetColorValue(colorName, "g"),  XMLManager.GetColorValue(colorName, "b"), 
                    XMLManager.GetColorValue(colorName, "a"));
            // Disable text. This is useful if text is drawn on texture
            else if (XMLManager.GetString(colorName, "customtext", false) == "notext")
                button.Label.Text = "";
        }

        #endregion

        #region Utilities

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
        static private void UpdateUV(ref MeshFilter meshFilter)
        {
            Vector2[] uv = new Vector2[4];
            uv[0] = new Vector2(0, 1);
            uv[1] = new Vector2(1, 1);
            uv[2] = new Vector2(0, 0);
            uv[3] = new Vector2(1, 0);
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
 