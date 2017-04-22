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
//                  Use WWWAudioExtensions.GetMovieTexture in Unity 5.6
//

/*
 * TODO:
 * - Import videos from disk (see Notes).
 */

using UnityEngine;
using DaggerfallWorkshop.Game.Utility.ModSupport;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Handles import and injection of custom videos
    /// with the purpose of providing modding support.
    /// </summary>
    static public class VideoReplacement
    {
        /// <summary>
        /// Import custom video from mods.
        /// </summary>
        /// <param name="name">Name of video.</param>
        static public bool ImportCustomVideo(string name, out MovieTexture video)
        {
            if (DaggerfallUnity.Settings.MeshAndTextureReplacement)
            {
                Mod[] mods = ModManager.Instance.GetAllMods(true);
                for (int i = mods.Length; i-- > 0;)
                {
                    if (mods[i].AssetBundle.Contains(name))
                    {
                        video = mods[i].GetAsset<MovieTexture>(name, true);
                        if (video != null)
                            return true;

                        Debug.LogError("Failed to import " + name + " from " + mods[i].Title + " as MovieTexture.");
                    }
                }
            }

            video = null;
            return false;
        }
    }
}
