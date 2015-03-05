// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;

namespace DaggerfallWorkshop.Demo
{
    /// <summary>
    /// Plays different ambient effects, both audible and visual, at random intervals.
    /// Certain effects such as lightning are timed to each other.
    /// </summary>
    [RequireComponent(typeof(DaggerfallAudioSource))]
    public class AmbientEffectsPlayer : MonoBehaviour
    {
        public int MinWaitTime = 4;             // Min wait time in seconds before next sound
        public int MaxWaitTime = 35;            // Max wait time in seconds before next sound
        public AmbientSoundPresets Presets;     // Ambient sound preset
        public bool PlayLightningEffect;        // Play a lightning effect where appropriate
        public DaggerfallSky SkyForEffects;     // Sky to receive effects
        public Light LightForEffects;           // Light to receive effects

        System.Random random;
        DaggerfallAudioSource dfAudioSource;
        SoundClips[] ambientSounds;
        AudioClip rainLoop;
        float waitTime;
        float waitCounter;
        AmbientSoundPresets lastPresets;

        public enum AmbientSoundPresets
        {
            None,                   // No ambience
            Dungeon,                // Dungeon ambience
            Rain,                   // Just raining
            Storm,                  // Storm ambience
        }

        void Start()
        {
            random = new System.Random(System.DateTime.Now.Millisecond);
            dfAudioSource = GetComponent<DaggerfallAudioSource>();
            dfAudioSource.Preset = AudioPresets.OnDemand;
            ApplyPresets();
            StartWaiting();
        }

        void OnDisable()
        {
            rainLoop = null;
        }

        void OnEnable()
        {
            rainLoop = null;
        }

        void Update()
        {
            // Change sound presets
            if (Presets != lastPresets)
            {
                lastPresets = Presets;
                rainLoop = null;
                ApplyPresets();
                StartWaiting();
            }

            // Start rain loop if not running
            if ((Presets == AmbientSoundPresets.Rain || Presets == AmbientSoundPresets.Storm) && rainLoop == null)
            {
                rainLoop = dfAudioSource.GetAudioClip((int)SoundClips.AmbientRaining);
                dfAudioSource.AudioSource.clip = rainLoop;
                dfAudioSource.AudioSource.loop = true;
                dfAudioSource.AudioSource.spatialBlend = 0;
                dfAudioSource.AudioSource.Play();
            }

            // Tick counter
            waitCounter += Time.deltaTime;
            if (waitCounter > waitTime)
            {
                PlayEffects();
                StartWaiting();
            }
        }

        #region Private Methods

        private void PlayEffects()
        {
            // Do nothing if audio not setup
            if (dfAudioSource == null || ambientSounds == null)
                return;

            // Get next sound index
            int index = random.Next(0, ambientSounds.Length);

            // Play effect
            if (Presets == AmbientSoundPresets.Storm && PlayLightningEffect)
            {
                // Play lightning effects together with appropriate sounds
                StartCoroutine(PlayLightningEffects(index));
            }
            else
            {
                // Play ambient sound as a one-shot 2D sound
                dfAudioSource.PlayOneShot((int)ambientSounds[index], 0);
            }
        }

        private IEnumerator PlayLightningEffects(int index)
        {
            //Debug.Log(string.Format("Playing index {0}", index));

            int minFlashes = 5;
            int maxFlashes = 10;
            float soundDelay = 0f;
            float randomSkip = 0.6f;

            // Store starting values
            float startSkyScale = 1f;
            float startLightIntensity = 1f;
            if (SkyForEffects) startSkyScale = SkyForEffects.SkyColorScale;
            if (LightForEffects) startLightIntensity = LightForEffects.intensity;

            SoundClips clip = ambientSounds[index];
            if (clip == SoundClips.StormLightningShort)
            {
                // Short close lightning flash
                minFlashes = 4;
                maxFlashes = 8;
            }
            else if (clip == SoundClips.StormLightningThunder)
            {
                // Short close lightning flash followed by thunder
                minFlashes = 5;
                maxFlashes = 10;
            }
            else if (clip == SoundClips.StormThunderRoll)
            {
                // Distant lightning strike with followed by a long delay then rolling thunder
                minFlashes = 20;
                maxFlashes = 30;
                soundDelay = 1.7f;
            }
            else
            {
                // Unknown clip, just play as one-shot and exit
                dfAudioSource.PlayOneShot((int)clip, 0);
                yield break;
            }

            // Play lightning flashes
            int numFlashes = random.Next(minFlashes, maxFlashes);
            for (int i = 0; i < numFlashes; i++)
            {
                // Randomly skip frames to introduce delay between flashes
                if (Random.value < randomSkip)
                {
                    // Flash on
                    if (SkyForEffects) SkyForEffects.SkyColorScale = 2f;
                    if (LightForEffects) LightForEffects.intensity = 2f;
                    yield return new WaitForEndOfFrame();
                }

                // Flash off
                if (SkyForEffects) SkyForEffects.SkyColorScale = startSkyScale;
                if (LightForEffects) LightForEffects.intensity = startLightIntensity;
                yield return new WaitForEndOfFrame();
            }

            // Reset values just to be sure
            if (SkyForEffects) SkyForEffects.SkyColorScale = startSkyScale;
            if (LightForEffects) LightForEffects.intensity = startLightIntensity;

            // Delay for sound effect
            if (soundDelay > 0)
                yield return new WaitForSeconds(1f / soundDelay);

            // Play sound effect
            dfAudioSource.PlayOneShot((int)clip, 0);

            yield break;
        }

        private void StartWaiting()
        {
            // Reset countdown to next sound
            waitTime = random.Next(MinWaitTime, MaxWaitTime);
            waitCounter = 0;
        }

        private void ApplyPresets()
        {
            if (Presets == AmbientSoundPresets.Dungeon)
            {
                // Set dungeon one-shots
                ambientSounds = new SoundClips[] {
                    SoundClips.AmbientDripShort,
                    SoundClips.AmbientDripLong,
                    SoundClips.AmbientWindMoan,
                    SoundClips.AmbientWindMoanDeep,
                    SoundClips.AmbientDoorOpen,
                    SoundClips.AmbientGrind,
                    SoundClips.AmbientStrumming,
                    SoundClips.AmbientWindBlow1,
                    SoundClips.AmbientWindBlow2,
                    SoundClips.AmbientMetalJangleLow,
                    SoundClips.AmbientBirdCall,
                    SoundClips.AmbientSqueaks,
                    SoundClips.AmbientClank,
                    SoundClips.AmbientDistantMoan,
                };
            }
            else if (Presets == AmbientSoundPresets.Storm)
            {
                // Set storm one-shots
                ambientSounds = new SoundClips[] {
                    SoundClips.StormLightningShort,
                    SoundClips.StormLightningThunder,
                    SoundClips.StormThunderRoll,
                };
            }
            else
            {
                ambientSounds = null;
            }

            lastPresets = Presets;
            dfAudioSource.SetSound(0, AudioPresets.OnDemand, 0);
        }

        #endregion
    }
}