// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:
// 
// Notes: LoadCustomTexture can import textures for billboards, but the actual sprites replacement is not implemented yet
// TO DO: add support for bump maps
//

using System.IO;
using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    static public class DFTextureReplacement
    {
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
        static public bool CustomTextureExist(int archive, int record, int frame)
        {
            if (DaggerfallUnity.Settings.MeshAndTextureReplacement //check .ini setting
                && File.Exists(Application.persistentDataPath + "/textures/" + archive.ToString() + "_" + record.ToString() + "-" + frame.ToString() + ".png"))
                return true;

            return false;
        }

        /// import custom image as texture2D
        /// <returns>Texture2D</returns>
        static public Texture2D LoadCustomTexture(int archive, int record, int frame)
        {
            Texture2D tex = new Texture2D(2, 2); //create empty texture, size will be the actual size of .png file

            //load image as Texture2D
            tex.LoadImage(File.ReadAllBytes(Application.persistentDataPath + "/textures/" + archive.ToString() + "_" + record.ToString() + "-" + frame.ToString() + ".png"));

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
                && File.Exists(Application.persistentDataPath + "/textures/img/" + filename + ".png"))
                return true;

            return false;
        }

        /// load custom image as texture2D
        /// <returns>Texture2D.</returns>
        static public Texture2D LoadCustomImage(string filename)
        {
            Texture2D tex = new Texture2D(2, 2); //create empty texture, size will be the actual size of .png file

            //load image as Texture2D
            tex.LoadImage(File.ReadAllBytes(Application.persistentDataPath + "/textures/img/" + filename + ".png"));

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
        static public bool CustomCifExist(string filename, int record, int frame)
        {

            if (DaggerfallUnity.Settings.MeshAndTextureReplacement //check .ini setting
                && File.Exists(Application.persistentDataPath + "/textures/cif/" + filename + "_" + record.ToString() + "-" + frame.ToString() + ".png"))
                return true;

            return false;
        }

        /// load custom image as texture2D
        /// <returns>Texture2D.</returns>
        static public Texture2D LoadCustomCif(string filename, int record, int frame)
        {
            Texture2D tex = new Texture2D(2, 2); //create empty texture, size will be the actual size of .png file

            //load image as Texture2D
            tex.LoadImage(File.ReadAllBytes(Application.persistentDataPath + "/textures/cif/" + filename + "_" + record.ToString() + "-" + frame.ToString() + ".png"));

            return tex; //assign image to the actual texture
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
                && File.Exists(Application.persistentDataPath + "/textures/" + archive.ToString() + "_" + record.ToString() + "-" + frame.ToString() + "_Emission.png"))
                return true;

            return false;
        }

        /// import custom image as texture2D
        /// <returns>Texture2D</returns>
        static public Texture2D LoadCustomEmission(int archive, int record, int frame)
        {
            Texture2D tex = new Texture2D(2, 2); //create empty texture, size will be the actual size of .png file

            //load image as Texture2D
            tex.LoadImage(File.ReadAllBytes(Application.persistentDataPath + "/textures/" + archive.ToString() + "_" + record.ToString() + "-" + frame.ToString() + "_Emission.png"));

            return tex; //assign image to the actual texture
        }
    }
}