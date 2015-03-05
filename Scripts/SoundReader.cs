// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Imports Daggerfall sounds into Unity as AudioClip objects.
    /// Should only be attached to DaggerfallUnity (for which it is a required component).
    /// </summary>
    [RequireComponent(typeof(DaggerfallUnity))]
    public class SoundReader : MonoBehaviour
    {
        #region Fields

        DaggerfallUnity dfUnity;
        SndFile soundFile;

        Dictionary<int, AudioClip> clipDict = new Dictionary<int, AudioClip>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets true if file reading is ready.
        /// </summary>
        public bool IsReady
        {
            get { return ReadyCheck(); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets AudioClip based on Daggerfall sound index.
        /// </summary>
        /// <param name="soundIndex">Sound index.</param>
        /// <returns>AudioClip or null.</returns>
        public AudioClip GetAudioClip(int soundIndex)
        {
            const float divisor = 1.0f / 128.0f;

            if (!ReadyCheck())
                return null;

            // Look for clip in cache
            AudioClip cachedClip = GetCachedClip(soundIndex);
            if (cachedClip)
                return cachedClip;

            // Get sound data
            DFSound dfSound;
            if (!soundFile.GetSound(soundIndex, out dfSound))
                return null;

            // Create audio clip
            AudioClip clip;
            string name = string.Format("DaggerfallClip [Index={0}, ID={1}]", soundIndex, (int)soundFile.BsaFile.GetRecordId(soundIndex));
            clip = AudioClip.Create(name, dfSound.WaveData.Length, 1, SndFile.SampleRate, false);

            // Create data array
            float[] data = new float[dfSound.WaveData.Length];
            for (int i = 0; i < dfSound.WaveData.Length; i++)
                data[i] = (dfSound.WaveData[i] - 128) * divisor;

            // Set clip data
            clip.SetData(data, 0);

            // Cache the clip
            CacheClip(soundIndex, clip);

            return clip;
        }

        /// <summary>
        /// Gets AudioClip based on Daggerfall SoundID.
        /// </summary>
        /// <param name="soundID">Sound ID.</param>
        /// <returns>AudioClip or null.</returns>
        public AudioClip GetAudioClip(uint soundID)
        {
            if (!ReadyCheck())
                return null;

            return GetAudioClip(soundFile.GetRecordIndex(soundID));
        }

        /// <summary>
        /// Gets AudioClip based on Daggerfall SoundClip enum.
        /// </summary>
        /// <param name="soundClip">SoundClip enum.</param>
        /// <returns>AudioClip or null.</returns>
        public AudioClip GetAudioClip(SoundClips soundClip)
        {
            if (!ReadyCheck())
                return null;

            return GetAudioClip((int)soundClip);
        }

        /// <summary>
        /// Gets sound ID from index.
        /// </summary>
        /// <param name="soundIndex">Sound index.</param>
        /// <returns>Sound ID.</returns>
        public uint GetSoundID(int soundIndex)
        {
            if (!ReadyCheck())
                return 0;

            return soundFile.BsaFile.GetRecordId(soundIndex);
        }

        /// <summary>
        /// Gets sound index from ID.
        /// </summary>
        /// <param name="soundID">Sound ID.</param>
        /// <returns>Sound index.</returns>
        public int GetSoundIndex(uint soundID)
        {
            if (!ReadyCheck())
                return -1;

            return soundFile.GetRecordIndex((uint)soundID);
        }

        #endregion

        #region Private Methods

        private AudioClip GetCachedClip(int key)
        {
            if (clipDict.ContainsKey(key))
                return clipDict[key];
            else
                return null;
        }

        private void CacheClip(int key, AudioClip clip)
        {
            clipDict.Add(key, clip);
        }

        private bool ReadyCheck()
        {
            // Ensure we have a DaggerfallUnity reference
            if (dfUnity == null)
            {
                dfUnity = DaggerfallUnity.Instance;
            }

            // Do nothing if DaggerfallUnity not ready
            if (!dfUnity.IsReady)
            {
                DaggerfallUnity.LogMessage("SoundReader: DaggerfallUnity component is not ready. Have you set your Arena2 path?");
                return false;
            }

            // Ensure sound reader is ready
            if (soundFile == null)
            {
                soundFile = new SndFile(Path.Combine(dfUnity.Arena2Path, SndFile.Filename), FileUsage.UseMemory, true);
            }

            return true;
        }

        #endregion
    }
}