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

using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Handles import and injection of custom AudioClips
    /// with the purpose of providing modding support.
    /// </summary>
    static public class SoundReplacement
    {
        // Fields
        static private string soundPath = Path.Combine(Application.streamingAssetsPath, "Sound");
        const string extension = ".wav";

        // Methods

        /// <summary>
        /// Check if sound file exist on disk.
        /// </summary>
        /// <param name="soundIndex">Index of clip. Will be casted to SoundClips enum for better readability.</param>
        /// <returns></returns>
        static public bool CustomSoundExist(int soundIndex)
        {
            if (DaggerfallUnity.Settings.MeshAndTextureReplacement
                && File.Exists(Path.Combine(soundPath, (SoundClips)soundIndex + extension)))
                return true;

            return false;
        }

        /// <summary>
        /// Load custom sound file files from disk.
        /// </summary>
        /// <param name="soundIndex">Index of clip. Will be casted to SoundClips enum for better readability.</param>
        /// <returns></returns>
        static public WWW LoadCustomSound(int soundIndex)
        {
            string path = "file://" + soundPath;
            WWW soundFile = new WWW(Path.Combine(path, (SoundClips)soundIndex + extension));
            return soundFile;
        }
    }
}
