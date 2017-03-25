// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:
// 
// Notes:           Uses AssetBundle instead of www.movie because of this:
//                  (https://issuetracker.unity3d.com/issues/calling-www-dot-movie-fails-to-load-and-prints-error-loadmovedata-got-null)
//

using System.IO;
using UnityEngine;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Handles import and injection of custom videos
    /// with the purpose of providing modding support.
    /// </summary>
    static public class VideoReplacement
    {
        static public string videosPath = Path.Combine(Application.streamingAssetsPath, "Videos");
        const string extension = ".dfvideo";
        const string fileExtension = ".ogg";

        /// <summary>
        /// Check if video file exist on disk.
        /// </summary>
        /// <param name="name">Name of sound file.</param>
        static public bool CustomVideoExist(string name)
        {
            name = name.Replace(".VID", extension);

            if (DaggerfallUnity.Settings.MeshAndTextureReplacement
                && File.Exists(Path.Combine(videosPath, name)))
                return true;

            return false;
        }

        /// <summary>
        /// Get Video.
        /// </summary>
        /// <param name="name">Name of sound file.</param>
        /// <returns>New WWW object</returns>
        static public MovieTexture GetVideo(string name)
        {
            // Get AssetBundle
            name = name.Replace(".VID", extension);
            string path = Path.Combine(videosPath, name);
            var loadedAssetBundle = AssetBundle.LoadFromFile(path);

            // Get MovieTexture
            name = name.Replace(extension, fileExtension);
            if ((loadedAssetBundle != null) && (loadedAssetBundle.Contains(name)))
                return loadedAssetBundle.LoadAsset<MovieTexture>(name);

            Debug.LogError(string.Format("File {0} from {1} is corrupted", name, videosPath));
            return null;
        }
    }
}
