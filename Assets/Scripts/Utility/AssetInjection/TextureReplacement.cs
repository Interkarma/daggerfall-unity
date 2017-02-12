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
 * 1. Dungeon enemies
 * 2. Exterior billboards
 * 3. PaperDoll CharacterLayer textures works only if resolution is the same as vanilla 
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
            return CustomTextureExist(archive.ToString() + "_" + record.ToString() + "-" + frame.ToString());
        }

        static public bool CustomTextureExist(string name)
        {
            if (DaggerfallUnity.Settings.MeshAndTextureReplacement //check .ini setting
                && File.Exists(Path.Combine(texturesPath, name + ".png")))
                return true;

            return false;
        }

        /// import custom image as texture2D
        /// <returns>Texture2D</returns>
        static public Texture2D LoadCustomTexture(int archive, int record, int frame)
        {
            return LoadCustomTexture(archive.ToString() + "_" + record.ToString() + "-" + frame.ToString());
        }

        static public Texture2D LoadCustomTexture(string name)
        {
            Texture2D tex = new Texture2D(2, 2); //create empty texture, size will be the actual size of .png file

            //load image as Texture2D
            tex.LoadImage(File.ReadAllBytes(Path.Combine(texturesPath, name + ".png")));

            return tex; //assign image to the actual texture
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

            if (DaggerfallUnity.Settings.MeshAndTextureReplacement //check .ini setting
                && File.Exists(Path.Combine(imgPath, filename + ".png")))
                return true;

            return false;
        }

        /// load custom image as texture2D
        /// <returns>Texture2D.</returns>
        static public Texture2D LoadCustomImage(string filename)
        {
            Texture2D tex = new Texture2D(2, 2); //create empty texture, size will be the actual size of .png file

            //load image as Texture2D
            tex.LoadImage(File.ReadAllBytes(Path.Combine(imgPath, filename + ".png")));

            return tex; //assign image to the actual texture
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

            if (DaggerfallUnity.Settings.MeshAndTextureReplacement //check .ini setting
                && File.Exists(Path.Combine(cifPath, filename + "_" + record.ToString() + "-" + frame.ToString() + ".png")))
                return true;

            return false;
        }

        /// load custom image as texture2D
        /// <returns>Texture2D.</returns>
        static public Texture2D LoadCustomCif(string filename, int record, int frame)
        {
            Texture2D tex = new Texture2D(2, 2); //create empty texture, size will be the actual size of .png file

            //load image as Texture2D
            tex.LoadImage(File.ReadAllBytes(Path.Combine(cifPath, filename + "_" + record.ToString() + "-" + frame.ToString() + ".png")));

            return tex; //assign image to the actual texture
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
            return CustomNormalExist(archive.ToString() + "_" + record.ToString() + "-" + frame.ToString());
        }

        static public bool CustomNormalExist(string name)
        {
            if (DaggerfallUnity.Settings.MeshAndTextureReplacement //check .ini setting
                && File.Exists(Path.Combine(texturesPath, name + "_Normal.png")))
                return true;

            return false;
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
            if (DaggerfallUnity.Settings.MeshAndTextureReplacement //check .ini setting
                && File.Exists(Path.Combine(texturesPath, archive.ToString() + "_" + record.ToString() + "-" + frame.ToString() + "_Emission.png")))
                return true;

            return false;
        }

        /// import custom image as texture2D
        /// <returns>Texture2D</returns>
        static public Texture2D LoadCustomEmission(int archive, int record, int frame)
        {
            Texture2D tex = new Texture2D(2, 2); //create empty texture, size will be the actual size of .png file

            //load image as Texture2D
            tex.LoadImage(File.ReadAllBytes(Path.Combine(texturesPath, archive.ToString() + "_" + record.ToString() + "-" + frame.ToString() + "_Emission.png")));

            return tex; //assign image to the actual texture
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
        static public void LoadCustomBillboardTexture(ref GameObject go, int archive, int record)
        {
            Texture2D albedoTexture, emissionMap;
            MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();

            // Check if billboard is emissive
            bool isEmissive = false;
            if (meshRenderer.materials[0].GetTexture("_EmissionMap") != null)
                isEmissive = true;

            // Import texture(s)
            LoadCustomBillboardFrameTexture(isEmissive, out albedoTexture, out emissionMap, archive, record);

            // Main texture
            meshRenderer.materials[0].SetTexture("_MainTex", albedoTexture);

            // Emission maps for lights
            if (isEmissive)
                meshRenderer.materials[0].SetTexture("_EmissionMap", emissionMap);

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
            Vector2[] uv = new Vector2[4];
            uv[0] = new Vector2(0, 1);
            uv[1] = new Vector2(1, 1);
            uv[2] = new Vector2(0, 0);
            uv[3] = new Vector2(1, 0);
            go.GetComponent<MeshFilter>().mesh.uv = uv;

            // Check if billboard is animated
            int NumberOfFrames = NumberOfAvailableFrames(archive, record);
            if (NumberOfFrames > 1)
            {
                // Import textures for each frame 
                go.GetComponent<DaggerfallBillboard>().SetCustomMaterial(archive, record, NumberOfFrames, isEmissive);
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
            button.BackgroundTexture = TextureReplacement.LoadCustomTexture(colorName);

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

        /// <summary>
        /// Check all frames available on disk.
        /// </summary>
        /// <param name="archive">Archive index.</param>
        /// <param name="record">Record index.</param>
        /// <returns>Number of textures present on disk for this record</returns>
        static public int NumberOfAvailableFrames(int archive, int record)
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
 