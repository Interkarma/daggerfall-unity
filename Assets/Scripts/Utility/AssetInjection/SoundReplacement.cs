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

using System.IO;
using UnityEngine;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Handles import and injection of custom sounds and
    /// songs with the purpose of providing modding support.
    /// </summary>
    static public class SoundReplacement
    {
        #region Fields & Structs

        static private string soundPath = Path.Combine(Application.streamingAssetsPath, "Sound");
        const string extension = ".wav";

        public struct CustomSong
        {
            public bool UseCustomSong;                   // Use custom song from disk or Daggerfall song
            public float Timer;                          // Current position in song
            public float Lenght;                         // Lenght of song
            public bool IsReady;                         // True if song is completely imported
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Check if sound file exist on disk.
        /// </summary>
        /// <param name="soundIndex">Index of clip. Will be casted to SoundClips enum for better readability.</param>
        /// <returns></returns>
        static public bool CustomSoundExist(int soundIndex)
        {
            return SoundFileExist((SoundClips)soundIndex + extension);
        }

        /// <summary>
        /// Load custom sound file from disk.
        /// </summary>
        /// <param name="soundIndex">Index of clip. Will be casted to SoundClips enum for better readability.</param>
        /// <returns></returns>
        static public WWW LoadCustomSound(int soundIndex)
        {
            return GetWwwFile((SoundClips)soundIndex + extension);
        }

        /// <summary>
        /// Check if song file exists on disk.
        /// We use the same nomenclature used in SongFiles enum.
        /// </summary>
        /// <param name="filename">Name of song, including .mid extension</param>
        /// <returns></returns>
        static public bool CustomMidiSongExist(string filename)
        {
            return SoundFileExist("song_" + filename);
        }

        /// <summary>
        /// Load custom song file from disk as a byte array.
        /// We use the same nomenclature used in SongFiles enum.
        /// </summary>
        /// <param name="filename">Name of song, including .mid extension</param>
        /// <returns></returns>
        static public byte[] LoadCustomMidiSong(string filename)
        {
            byte[] songFile = File.ReadAllBytes(Path.Combine(soundPath, "song_" + filename));

            if (songFile != null)
                return songFile;

            Debug.LogError("can't load custom song song_" + filename);
            return null;
        }

        /// <summary>
        /// Check if song file exist on disk.
        /// </summary>
        /// <param name="song">Name of song.</param>
        /// <returns></returns>
        static public bool CustomSongExist(SongFiles song)
        {
            return SoundFileExist(song.ToString() + extension);
        }

        /// <summary>
        /// Load custom sound file from disk.
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        static public WWW LoadCustomSong(SongFiles song)
        {
            return GetWwwFile(song.ToString() + extension);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Check if sound file exist on disk.
        /// </summary>
        /// <param name="name">Name of sound file.</param>
        static private bool SoundFileExist(string name)
        {
            if (DaggerfallUnity.Settings.MeshAndTextureReplacement
                && File.Exists(Path.Combine(soundPath, name)))
                return true;

            return false;
        }

        /// <summary>
        /// Get content of sound file.
        /// </summary>
        /// <param name="name">Name of sound file.</param>
        /// <returns>New WWW object</returns>
        static private WWW GetWwwFile (string name)
        {
            WWW www = new WWW(Path.Combine("file://" + soundPath, name));

            if (www!=null)
                return www;

            Debug.LogError(string.Format("File {1} from {2} is corrupted", name, soundPath));
            return null;
        }

        #endregion
    }
}
