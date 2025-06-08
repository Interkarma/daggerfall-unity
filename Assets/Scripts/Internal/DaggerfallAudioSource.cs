// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Load sound effects from Daggerfall into a normal AudioSource.
    /// This component loads clips procedurally in editor and at runtime.
    /// Can preview clips by index, ID, or enum directly from editor window.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class DaggerfallAudioSource : MonoBehaviour
    {
        const int minIndex = 0;
        const int maxIndex = 458;

        public AudioPresets Preset = AudioPresets.OnDemand;
        public int SoundIndex = -1;

#if UNITY_EDITOR
        [HideInInspector]
        public int PreviewIndex = 0;
        [HideInInspector]
        public int PreviewID = 3;
        [HideInInspector]
        public SoundClips PreviewClip = SoundClips.SpookyHigh;
#endif

        GameObject player;
        DaggerfallUnity dfUnity;
        AudioSource audioSource;
        AudioClip audioClip;

        // Will enable/disable AudioSource based on player proximity.
        // This works around having too many point audio sources in scene by
        // turning off audio sources the player cannot possibly hear.
        // Can only be flagged using LoopOnDistance preset.
        private bool playerCheck = false;

        // Sets looping sound to only play randomly and on a slower update matched to classic.
        // Only used for animal sounds.
        private bool playRandomly = false;

        public bool IsReady
        {
            get { return ReadyCheck(); }
        }

        /// <summary>
        /// Gets peer AudioSource component.
        /// </summary>
        public AudioSource AudioSource
        {
            get { return (audioSource) ? audioSource : GetComponent<AudioSource>(); }
        }

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            player = GameObject.FindGameObjectWithTag("Player");
        }

        void FixedUpdate()
        {
            // Clip needs to be loaded before it can be played.
            // Only try to apply a valid index range.
            if (audioClip == null && SoundIndex >= minIndex && SoundIndex < maxIndex)
                Apply();

            // Handle player checks
            if (playerCheck && audioSource && player)
            {
                bool playerInRange = (Vector3.Distance(transform.position, player.transform.position) <= audioSource.maxDistance);

                audioSource.enabled = playerInRange;
                // Allows volume to be adjusted without reloading game.
                audioSource.volume = DaggerfallUnity.Settings.SoundVolume;

                // Handle random play
                if (audioSource.enabled && playRandomly && Game.GameManager.ClassicUpdate && DFRandom.rand() <= 100)
                    audioSource.Play();
            }
        }

#if UNITY_EDITOR
        public void EditorPreviewByIndex()
        {
            if (!ReadyCheck())
                return;

            if (PreviewIndex < minIndex || PreviewIndex >= maxIndex)
                return;

            if (ReadyCheck())
            {
                PreviewID = (int)dfUnity.SoundReader.GetSoundID(PreviewIndex);
                PreviewClip = (SoundClips)PreviewIndex;
                AudioClip clip = dfUnity.SoundReader.GetAudioClip(PreviewIndex);
                audioSource.PlayOneShotWhenReady(clip, 1.0f);
            }
        }

        public void EditorPreviewByID()
        {
            if (!ReadyCheck())
                return;

            PreviewIndex = dfUnity.SoundReader.GetSoundIndex((uint)PreviewID);
            if (PreviewIndex >= 0)
            {
                PreviewClip = (SoundClips)PreviewIndex;
                EditorPreviewByIndex();
            }
            else
            {
                PreviewClip = SoundClips.None;
            }
        }

        public void EditorPreviewBySoundClip()
        {
            if (!ReadyCheck())
                return;

            PreviewIndex = (int)PreviewClip;
            if (PreviewIndex >= 0)
            {
                PreviewID = (int)dfUnity.SoundReader.GetSoundID(PreviewIndex);
                EditorPreviewByIndex();
            }
        }
#endif

        /// <summary>
        /// Quick set from index.
        /// </summary>
        public void SetSound(int soundIndex, AudioPresets preset = AudioPresets.OnDemand, float spatialBlend = 1)
        {
            SoundIndex = soundIndex;
            Preset = preset;
            Apply(spatialBlend);
        }

        /// <summary>
        /// Quick set from clip name.
        /// </summary>
        public void SetSound(SoundClips soundClip, AudioPresets preset = AudioPresets.OnDemand, float spatialBlend = 1)
        {
            SoundIndex = (int)soundClip;
            Preset = preset;
            Apply(spatialBlend);
        }

        /// <summary>
        /// Quick set from ID.
        /// </summary>
        public void SetSound(uint soundID, AudioPresets preset = AudioPresets.OnDemand, float spatialBlend = 1)
        {
            if (ReadyCheck())
            {
                int soundIndex = dfUnity.SoundReader.GetSoundIndex(soundID);
                SoundIndex = soundIndex;
                Preset = preset;
                Apply(spatialBlend);
            }
        }

        /// <summary>
        /// Plays sound index once without changing clip on AudioSource.
        /// </summary>
        public void PlayOneShot(int soundIndex, float spatialBlend = 1, float volumeScale = 1f)
        {
            if (enabled && ReadyCheck())
            {
                AudioClip clip = dfUnity.SoundReader.GetAudioClip(soundIndex);
                if (clip)
                {
                    audioSource.spatialBlend = spatialBlend;
                    audioSource.PlayOneShotWhenReady(clip, volumeScale);
                }
            }
        }

        /// <summary>
        /// Plays sound clip once without changing clip on AudioSource.
        /// </summary>
        public void PlayOneShot(SoundClips soundClip, float spatialBlend = 1, float volumeScale = 1f)
        {
            PlayOneShot((int)soundClip, spatialBlend, volumeScale);
        }

        /// <summary>
        /// Plays sound clip once at a specified position without changing clip on AudioSource.
        /// </summary>
        public void PlayClipAtPoint(int soundIndex, Vector3 position, float volumeScale = 1f)
        {
            if (enabled && ReadyCheck())
            {
                AudioClip clip = dfUnity.SoundReader.GetAudioClip(soundIndex);
                if (clip)
                {
                    AudioSourceExtensionMethods.PlayClipAtPointWhenReady(clip, position, volumeScale);
                }
            }
        }

        /// <summary>
        /// Plays sound clip once at a specified position without changing clip on AudioSource.
        /// </summary>
        public void PlayClipAtPoint(SoundClips soundClip, Vector3 position, float volumeScale = 1f)
        {
            PlayClipAtPoint((int)soundClip, position, volumeScale);
        }

        /// <summary>
        /// Plays sound ID once without changing clip on AudioSource.
        /// </summary>
        public void PlayOneShot(uint soundID, float spatialBlend = 1, float volumeScale = 1f)
        {
            int soundIndex = dfUnity.SoundReader.GetSoundIndex(soundID);
            PlayOneShot(soundIndex, spatialBlend, volumeScale);
        }

        /// <summary>
        /// Check if AudioSource is playing a clip.
        /// </summary>
        public bool IsPlaying()
        {
            return (audioSource) ? audioSource.isPlaying : false;
        }

        /// <summary>
        /// Get AudioClip from sound index.
        /// </summary>
        public AudioClip GetAudioClip(int soundIndex)
        {
            if (ReadyCheck())
                return dfUnity.SoundReader.GetAudioClip(soundIndex);
            else
                return null;
        }

        #region Private Methods

        /// <summary>
        /// Apply current sound index and behaviour to AudioSource.
        /// </summary>
        private void Apply(float spatialBlend = 1)
        {
            // Do nothing if not ready
            if (!ReadyCheck() || !audioSource.enabled)
                return;

            // Do nothing if out of range
            if (SoundIndex < minIndex || SoundIndex >= maxIndex)
                return;

            // Get new clip
            audioClip = dfUnity.SoundReader.GetAudioClip(SoundIndex);
            if (audioClip == null)
            {
                DaggerfallUnity.LogMessage("Failed to load Daggerfall audio clip.");
                return;
            }

            // Set spatial blend
            audioSource.spatialBlend = spatialBlend;

            // Apply preset
            switch (Preset)
            {
                case AudioPresets.OnDemand:
                    audioSource.playOnAwake = false;
                    audioSource.loop = false;
                    playerCheck = false;
                    playRandomly = false;
                    break;
                case AudioPresets.LoopOnAwake:
                    audioSource.playOnAwake = true;
                    audioSource.loop = true;
                    playerCheck = false;
                    playRandomly = false;
                    break;
                case AudioPresets.LoopOnDemand:
                    audioSource.playOnAwake = false;
                    audioSource.loop = true;
                    playerCheck = false;
                    playRandomly = false;
                    break;
                case AudioPresets.LoopIfPlayerNear:
                    audioSource.playOnAwake = true;
                    audioSource.loop = true;
                    playerCheck = true;
                    playRandomly = false;
                    break;
                case AudioPresets.PlayRandomlyIfPlayerNear:
                    audioSource.playOnAwake = false;
                    audioSource.loop = false;
                    playerCheck = true;
                    playRandomly = true;
                    break;
                default:
                    break;
            }

            // Assign clip to Unity AudioSource only if app playing
            if (Application.isPlaying)
                audioSource.clip = audioClip;

            // Manually start sound if playOnAwake true.
            // This is necessary as sound is procedurally created after awake.
            if (audioSource.playOnAwake)
            {
                audioSource.volume = DaggerfallUnity.Settings.SoundVolume;
                audioSource.Play();
            }
        }

        private bool ReadyCheck()
        {
            // Ensure we have a DaggerfallUnity reference
            if (dfUnity == null)
                dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return false;

            // Get audio source
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                DaggerfallUnity.LogMessage("DaggerfallAudioSource: Could not find AudioSource component.");
                return false;
            }

            return true;
        }

        #endregion
    }

}