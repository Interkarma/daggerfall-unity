// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
#endregion

namespace DaggerfallConnect.Utility
{

    /// <summary>
    /// Static methods to validate ARENA2 folder.
    /// </summary>
    public class DFValidator
    {

        #region Structures

        /// <summary>
        /// Packages validation information.
        /// </summary>
        public struct ValidationResults
        {
            /// <summary>The full path that was tested.</summary>
            public string PathTested;

            /// <summary>True if all tests succeeded.</summary>
            public bool AppearsValid;

            /// <summary>True if folder exists.</summary>
            public bool FolderValid;

            ///// <summary>True if texture count is correct.</summary>
            //public bool TexturesValid;

            /// <summary>True if ARCH3D.BSA exists.</summary>
            public bool ModelsValid;

            /// <summary>True if BLOCKS.BSA exists.</summary>
            public bool BlocksValid;

            /// <summary>True if MAPS.BSA exists.</summary>
            public bool MapsValid;

            /// <summary>True if DAGGER.SND exists.</summary>
            public bool SoundsValid;

            /// <summary>True if WOODS.WLD exists.</summary>
            public bool WoodsValid;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Validates an ARENA2 folder.
        ///  This currently just checks the right major files exist in the right quantities.
        ///  Does not verify contents so test is quite speedy and can be performed at startup.
        ///  Will also look for main .BSA files in Unity Resources folder.
        ///  Does not verify texture/image files.
        /// </summary>
        /// <param name="path">Full path of ARENA2 folder to validate.</param>
        /// <param name="results">Output results.</param>
        public static void ValidateArena2Folder(string path, out ValidationResults results)
        {
            results = new ValidationResults();
            results.PathTested = path;

            // Check folder exists
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                return;
            else
                results.FolderValid = true;

            // Get files
            //string[] textures = Directory.GetFiles(path, "TEXTURE.???");
            string[] models = Directory.GetFiles(path, Arch3dFile.Filename);
            string[] blocks = Directory.GetFiles(path, BlocksFile.Filename);
            string[] maps = Directory.GetFiles(path, MapsFile.Filename);
            string[] sounds = Directory.GetFiles(path, SndFile.Filename);
            string[] woods = Directory.GetFiles(path, WoodsFile.Filename);

            //// Validate texture count
            //if (textures.Length >= 472)
            //    results.TexturesValid = true;

            // Validate models count
            if (models.Length >= 1)
                results.ModelsValid = true;

            // Validate blocks count
            if (blocks.Length >= 1)
                results.BlocksValid = true;

            // Validate maps count
            if (maps.Length >= 1)
                results.MapsValid = true;

            // Validate sounds count
            if (sounds.Length >= 1)
                results.SoundsValid = true;

            // Validate woods count
            if (woods.Length >= 1)
                results.WoodsValid = true;

            // Support alternate ARCH3D.BSA from Resources if available
            if (!results.ModelsValid)
            {
                UnityEngine.TextAsset arch3dAsset = UnityEngine.Resources.Load(Arch3dFile.Filename) as UnityEngine.TextAsset;
                if (arch3dAsset != null)
                    results.ModelsValid = true;
            }

            // Supports alternate BLOCKS.BSA from Resources if available
            if (!results.BlocksValid)
            {
                UnityEngine.TextAsset blocksAsset = UnityEngine.Resources.Load(BlocksFile.Filename) as UnityEngine.TextAsset;
                if (blocksAsset != null)
                    results.BlocksValid = true;
            }

            // Supports alternate MAPS.BSA from Resources if available
            if (!results.MapsValid)
            {
                UnityEngine.TextAsset mapsAsset = UnityEngine.Resources.Load(MapsFile.Filename) as UnityEngine.TextAsset;
                if (mapsAsset != null)
                    results.MapsValid = true;
            }

            // Supports alternate DAGGER.SND from Resources if available
            if (!results.SoundsValid)
            {
                UnityEngine.TextAsset soundAsset = UnityEngine.Resources.Load(SndFile.Filename) as UnityEngine.TextAsset;
                if (soundAsset != null)
                    results.SoundsValid = true;
            }

            // Supports alternate WOODS.WLD from Resources if available
            if (!results.WoodsValid)
            {
                UnityEngine.TextAsset woodsAsset = UnityEngine.Resources.Load(WoodsFile.Filename) as UnityEngine.TextAsset;
                if (woodsAsset != null)
                    results.WoodsValid = true;
            }

            // If everything else is valid then set AppearsValid flag
            if (results.FolderValid &&
                //results.TexturesValid &&
                results.ModelsValid &&
                results.BlocksValid &&
                results.MapsValid &&
                results.SoundsValid &&
                results.WoodsValid)
            {
                results.AppearsValid = true;
            }
        }

        #endregion

    }

}
