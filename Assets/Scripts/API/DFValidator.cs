// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

#region Using Statements
using System.IO;
using DaggerfallConnect.Arena2;
#endregion

namespace DaggerfallConnect.Utility
{
    /// <summary>
    /// Static methods to validate ARENA2 folder.
    /// Does not verify contents, just that critical files exist in minimum quantities.
    /// This allows test to be fast enough to be run at startup.
    /// </summary>
    public class DFValidator
    {
        #region Fields

        const string textureSearchPattern = "TEXTURE.???";
        const string vidSearchPattern = "*.VID";
        const string vidAlternateTestFile = "ANIM0011.VID";

        const int minTextureCount = 472;
        const int minVidCount = 17;

        #endregion

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

            /// <summary>True if texture count is correct.</summary>
            public bool TexturesValid;

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

            /// <summary>True if all .VID files present.</summary>
            public bool VideosValid;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Validates an ARENA2 folder.
        ///  This currently just checks the right major files exist in the right quantities.
        ///  Does not verify contents so test is quite speedy and can be performed at startup.
        ///  Will also look for main .BSA files in Unity Resources folder.
        /// </summary>
        /// <param name="path">Full path of ARENA2 folder to validate.</param>
        /// <param name="results">Output results.</param>
        /// <param name="requireVideos">Videos must be present to pass final validation.</param>
        public static void ValidateArena2Folder(string path, out ValidationResults results, bool requireVideos = false)
        {
            results = new ValidationResults();
            results.PathTested = path;

            // Check folder exists
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                return;
            else
                results.FolderValid = true;

            // Get files
            string[] textures = Directory.GetFiles(path, textureSearchPattern);
            string[] models = Directory.GetFiles(path, Arch3dFile.Filename);
            string[] blocks = Directory.GetFiles(path, BlocksFile.Filename);
            string[] maps = Directory.GetFiles(path, MapsFile.Filename);
            string[] sounds = Directory.GetFiles(path, SndFile.Filename);
            string[] woods = Directory.GetFiles(path, WoodsFile.Filename);
            string[] videos = Directory.GetFiles(path, vidSearchPattern);

            // Validate texture count
            if (textures.Length >= minTextureCount)
                results.TexturesValid = true;

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

            // Validate videos count
            if (videos.Length >= minVidCount)
                results.VideosValid = true;

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

            // Supports alternate *.VID from Resources if available
            // Just tests for smallest video for performance reasons
            // Assumes build creator has added all other videos
            if (!results.VideosValid)
            {
                UnityEngine.TextAsset videoAsset = UnityEngine.Resources.Load(vidAlternateTestFile) as UnityEngine.TextAsset;
                if (videoAsset != null)
                    results.VideosValid = true;
            }

            // If everything else is valid then set AppearsValid flag
            if (results.FolderValid &&
                results.TexturesValid &&
                results.ModelsValid &&
                results.BlocksValid &&
                results.MapsValid &&
                results.SoundsValid &&
                results.WoodsValid)
            {
                results.AppearsValid = true;
            }

            // Check videos
            if (requireVideos && !results.VideosValid)
                results.AppearsValid = false;
        }

        #endregion

    }
}
