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

using System.Collections;
using System.IO;
using UnityEngine;
using DaggerfallWorkshop.Game.Utility.ModSupport;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Handles import and injection of custom videos with the purpose of providing modding support.
    /// MovieTextures are imported from mod bundles with load order or streamed directly from disk.
    /// </summary>
    public static class VideoReplacement
    {
        static readonly string moviePath = Path.Combine(Application.streamingAssetsPath, "Movies");

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
        /// <param name="movieTexture">Movietexture with imported video and sound data.</param>
        /// <returns>True if movie is found.</returns>
        public static bool TryImportMovie(string name, out MovieTexture movieTexture)
        {
            if (DaggerfallUnity.Settings.MeshAndTextureReplacement)
            {
                // Seek from loose files
                string path = Path.Combine(moviePath, name + ".ogv");
                if (File.Exists(path))
                {
                    WWW www = new WWW("file://" + path);
                    movieTexture = www.GetMovieTexture();
                    DaggerfallUnity.Instance.StartCoroutine(LoadMovieTexture(www, movieTexture));
                    return true;
                }

                // Seek from mods
                if (ModManager.Instance != null)
                    return ModManager.Instance.TryGetAsset(name, false, out movieTexture);
            }

            movieTexture = null;
            return false;
        }

        /// <summary>
        /// Load movietexture from WWW in background.
        /// </summary>
        private static IEnumerator LoadMovieTexture(WWW www, MovieTexture movieTexture)
        {
            yield return www;

            if (!movieTexture.isReadyToPlay)
                Debug.LogErrorFormat("Failed to load movie: {0}", www.error);

            www.Dispose();
        }
    }
}
