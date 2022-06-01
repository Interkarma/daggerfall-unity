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

using System.Collections;
using System.IO;
using UnityEngine;
using DaggerfallWorkshop.Game.Utility.ModSupport;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Handles import and injection of custom sounds and songs with the purpose of providing modding support.
    /// Sound files are imported from mod bundles with load order or loaded directly from disk.
    /// </summary>
    public static class SoundReplacement
    {
        #region Fields & Properties

        static readonly string soundPath = Path.Combine(Application.streamingAssetsPath, "Sound");

        /// <summary>
        /// Path to custom sounds and songs on disk.
        /// </summary>
        public static string SoundPath
        {
            get { return soundPath; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Seek sound from mods.
        /// </summary>
        /// <param name="sound">Sound clip to seek.</param>
        /// <param name="audioClip">Audioclip with imported sound data.</param>
        /// <returns>True if sound is found.</returns>
        public static bool TryImportSound(SoundClips sound, out AudioClip audioClip)
        {
            return TryImportAudioClip(sound.ToString(), ".wav", false, out audioClip);
        }

        /// <summary>
        /// Seek song from mods.
        /// </summary>
        /// <param name="song">Song to seek.</param>
        /// <param name="audioClip">Audioclip with imported sound data.</param>
        /// <returns>True if song is found.</returns>
        public static bool TryImportSong(SongFiles song, out AudioClip audioClip)
        {
            return TryImportAudioClip(song.ToString(), ".ogg", true, out audioClip);
        }

        /// <summary>
        /// Seek midi song from mods.
        /// </summary>
        /// <param name="filename">Name of song to seek including .mid extension.</param>
        /// <param name="songBytes">Midi data as a byte array.</param>
        /// <returns>True if song is found.</returns>
        public static bool TryImportMidiSong(string filename, out byte[] songBytes)
        {
            return TryGetAudioBytes("song_" + filename, out songBytes);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Import sound data from modding locations as an audio clip.
        /// </summary>
        private static bool TryImportAudioClip(string name, string extension, bool streaming, out AudioClip audioClip)
        {
            if (DaggerfallUnity.Settings.AssetInjection)
            {
                // Seek from loose files
                string path = Path.Combine(soundPath, name + extension);
                if (File.Exists(path))
                {
                    WWW www = new WWW("file://" + path); // TODO: Replace with UnityWebRequest
                    if (streaming) {
                        audioClip = www.GetAudioClip(true, true);
                    }
                    else
                    {
                        audioClip = www.GetAudioClip();
                        DaggerfallUnity.Instance.StartCoroutine(LoadAudioData(www, audioClip));
                    }
                    return true;
                }

                // Seek from mods
                if (ModManager.Instance != null && ModManager.Instance.TryGetAsset(name, false, out audioClip))
                {
                    if (audioClip.preloadAudioData || audioClip.LoadAudioData())
                        return true;

                    Debug.LogErrorFormat("Failed to load audiodata for audioclip {0}", name);
                }
            }

            audioClip = null;
            return false;
        }

        /// <summary>
        /// Import midi data from modding locations as a byte array.
        /// </summary>
        private static bool TryGetAudioBytes(string name, out byte[] songBytes)
        {
            if (DaggerfallUnity.Settings.AssetInjection)
            {
                // Seek from loose files
                string path = Path.Combine(soundPath, name);
                if (File.Exists(path))
                {
                    songBytes = File.ReadAllBytes(path);
                    return true;
                }

                // Seek from mods
                if (ModManager.Instance != null)
                {
                    TextAsset textAsset;
                    if (ModManager.Instance.TryGetAsset(name, false, out textAsset))
                    {
                        songBytes = textAsset.bytes;
                        return true;
                    }
                }
            }

            songBytes = null;
            return false;
        }

        /// <summary>
        /// Load audio data from WWW in background.
        /// </summary>
        private static IEnumerator LoadAudioData(WWW www, AudioClip clip) // TODO: Replace with UnityWebRequest
        {
            yield return www;

            if (clip.loadState == AudioDataLoadState.Failed)
                Debug.LogErrorFormat("Failed to load audioclip: {0}", www.error);

            www.Dispose();
        }

        #endregion
    }
}
