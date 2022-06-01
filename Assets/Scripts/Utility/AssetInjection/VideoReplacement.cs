// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:
// 
// Notes:
//

using System.IO;
using UnityEngine;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using UnityEngine.Video;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Handles import and injection of custom videos with the purpose of providing modding support.
    /// MovieTextures are imported from mod bundles with load order or streamed directly from disk.
    /// </summary>
    public static class VideoReplacement
    {
        static readonly string moviePath = Path.Combine(Application.streamingAssetsPath, "Movies");

// https://docs.unity3d.com/Manual/VideoSources-FileCompatibility.html
#if !UNITY_STANDALONE_LINUX
        static readonly string videoExtension = ".mp4";
#else
        static readonly string videoExtension = ".webm";
#endif

        /// <summary>
        /// Path to custom movies on disk.
        /// </summary>
        public static string MoviePath
        {
            get { return moviePath; }
        }

        /// <summary>
        /// Seek movie from mods.
        /// </summary>
        /// <param name="name">Name of movie to seek including .VID extension.</param>
        /// <param name="videoPlayerDrawer">Video player with imported video and sound data.</param>
        /// <returns>True if movie is found.</returns>
        public static bool TryImportMovie(string name, out VideoPlayerDrawer videoPlayerDrawer)
        {
            if (DaggerfallUnity.Settings.AssetInjection)
            {
                // Remove VID extension
                int index = name.LastIndexOf(".VID");
                if (index > 0)
                    name = name.Substring(0, index);

                // Seek from loose files
                string path = Path.Combine(moviePath, name + videoExtension);
                if (File.Exists(path))
                {
                    videoPlayerDrawer = new VideoPlayerDrawer(path);
                    return true;
                }

                // Seek from mods
                VideoClip videoClip;
                if (ModManager.Instance != null && ModManager.Instance.TryGetAsset(name, false, out videoClip))
                {
                    videoPlayerDrawer = new VideoPlayerDrawer(videoClip);
                    return true;
                }
            }

            videoPlayerDrawer = null;
            return false;
        }
    }
}
