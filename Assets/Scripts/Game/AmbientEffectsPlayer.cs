// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Allofich
// 
// Notes:
//

using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Plays different ambient effects, both audible and visual, at random intervals.
    /// Certain effects such as lightning are timed to each other.
    /// NOTE: Lightning sky effects are deprecated for now.
    /// </summary>
    [RequireComponent(typeof(DaggerfallAudioSource))]
    public class AmbientEffectsPlayer : MonoBehaviour
    {
        public int MinWaitTime = 4;             // Min wait time in seconds before next sound
        public int MaxWaitTime = 35;            // Max wait time in seconds before next sound
        public int CemeteryMinWaitTime = 1;     // Min wait time in seconds before next sound
        public int CemeteryMaxWaitTime = 80;   // Max wait time in seconds before next sound
        public AmbientSoundPresets Presets;     // Ambient sound preset
        public bool IsMuted = false;
        public bool doNotPlayInCastle = true;   // Do not play ambient effects in castle blocks
        public bool PlayLightningEffect;        // Play a lightning effect where appropriate
        //public DaggerfallSky SkyForEffects;     // Sky to receive effects
        public Light LightForEffects;           // Light to receive effects

        System.Random random;
        DaggerfallAudioSource dfAudioSource;
        AudioSource loopAudioSource;
        float loopVolumeScale;
        AudioSource ambientAudioSource;
        //private Coroutine relativePositionCoroutine = null;

        SoundClips[] ambientSounds;
        SoundClips[] cemeteryAmbientSounds = new SoundClips[]
            {
                SoundClips.AmbientDistantHowl,
                SoundClips.AmbientCreepyBirdCall,
                SoundClips.AmbientCreepyBirdCall
            };
        AudioClip rainLoop;
        AudioClip cricketsLoop;

        float waitTime;
        float waitCounter;

        bool IsCemeteryNearby;
        float cemeteryWaitTime;
        float cemeteryWaitCounter;

        float waterWaitCounter;

        AmbientSoundPresets lastPresets;
        Entity.DaggerfallEntityBehaviour playerBehaviour;
        PlayerEnterExit playerEnterExit;

        public enum AmbientSoundPresets
        {
            None,                   // No ambience
            Dungeon,                // Dungeon ambience
            Rain,                   // Just raining
            Storm,                  // Storm ambience
            SunnyDay,               // Sunny day birds
            ClearNight,             // Clear night crickets
        }

        void Start()
        {
            random = new System.Random(System.DateTime.Now.Millisecond);
            dfAudioSource = GetComponent<DaggerfallAudioSource>();
            loopAudioSource = GetNewAudioSource();
            ambientAudioSource = GetNewAudioSource();

            ApplyPresets();
            StartWaiting();
            playerBehaviour = GameManager.Instance.PlayerEntityBehaviour;
            playerEnterExit = GameManager.Instance.PlayerEnterExit;

            PlayerGPS.OnEnterLocationRect += PlayerGPS_OnEnterLocationRect;
            PlayerGPS.OnExitLocationRect += PlayerGPS_OnExitLocationRect;

            DaggerfallVidPlayerWindow.OnVideoStart += AmbientEffectsPlayer_OnVideoStart;
            DaggerfallVidPlayerWindow.OnVideoEnd += AmbientEffectsPlayer_OnVideoEnd;
        }

        void OnDisable()
        {
            rainLoop = null;
            cricketsLoop = null;
        }

        void OnEnable()
        {
            rainLoop = null;
            cricketsLoop = null;
        }

        void Update()
        {
            if (IsMuted)
                return;

            // Change sound presets
            if (Presets != lastPresets)
            {
                // Clear settings
                lastPresets = Presets;
                rainLoop = null;
                cricketsLoop = null;

                // Stop playing any loops
                if (loopAudioSource.isPlaying)
                {
                    loopAudioSource.Stop();
                    loopAudioSource.clip = null;
                    loopAudioSource.loop = false;
                }

                ApplyPresets();
                StartWaiting();
            }

            // Start rain loop if not running
            if ((Presets == AmbientSoundPresets.Rain || Presets == AmbientSoundPresets.Storm) && rainLoop == null)
            {
                rainLoop = PlayLoop(SoundClips.AmbientRaining, 1f);
            }

            // Start crickets loop if not running
            if ((Presets == AmbientSoundPresets.ClearNight) && cricketsLoop == null)
            {
                cricketsLoop = PlayLoop(SoundClips.AmbientCrickets, 1f);
            }

            // Tick counters
            waitCounter += Time.deltaTime;
            waterWaitCounter += Time.deltaTime;
            if (waitCounter > waitTime)
            {
                PlayEffects();
                StartWaiting();
            }

            if (IsCemeteryNearby && !playerEnterExit.IsPlayerInside)
            {
                cemeteryWaitCounter += Time.deltaTime;
                if (cemeteryWaitCounter > cemeteryWaitTime)
                {
                    PlayCemeteryEffects();
                    StartCemeteryWaiting();
                }
            }

            // Play water sound effects. Timing based on classic.
            if (waterWaitCounter > GameManager.classicUpdateInterval)
            {
                if (playerEnterExit && playerEnterExit.blockWaterLevel != 10000)
                {
                    // Chance to play gentle water sound at water surface
                    if (DFRandom.rand() < 50)
                    {
                        Vector3 waterSoundPosition = playerBehaviour.transform.position;
                        waterSoundPosition.y = playerEnterExit.blockWaterLevel * -1 * MeshReader.GlobalScale;
                        waterSoundPosition.x += Random.Range(-3f, 3f);
                        waterSoundPosition.z += Random.Range(-3f, 3f);
                        SpatializedPlayOneShot(SoundClips.WaterGentle, waterSoundPosition, 1f);
                    }

                    // Chance to play water bubbles sound if player is underwater
                    if (playerEnterExit.IsPlayerSubmerged && DFRandom.rand() < 100)
                    {
                        AmbientPlayOneShot(SoundClips.AmbientWaterBubbles, 1f);
                    }
                }
                waterWaitCounter = 0;
            }

            // Update sound volume
            if (loopAudioSource.isPlaying)
                loopAudioSource.volume = loopVolumeScale * DaggerfallUnity.Settings.SoundVolume;
        }

        #region Private Methods

        private AudioSource GetNewAudioSource()
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.hideFlags = HideFlags.HideInInspector;
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.dopplerLevel = 0f;
            audioSource.spatialBlend = 0f;
            audioSource.volume = DaggerfallUnity.Settings.SoundVolume;
            return audioSource;
        }

        private AudioClip PlayLoop(SoundClips clip, float volumeScale)
        {
            AudioClip loopClip = dfAudioSource.GetAudioClip((int)clip);
            loopVolumeScale = volumeScale;
            loopAudioSource.loop = true;
            loopAudioSource.spatialBlend = 0;
            loopAudioSource.PlayWhenReady(loopClip, volumeScale);
            return loopClip;
        }

        private void AmbientPlayOneShot(SoundClips clip, float volumeScale)
        {
            if (!ambientAudioSource.isPlaying)
            {
                AudioClip audioClip = dfAudioSource.GetAudioClip((int)clip);
                ambientAudioSource.spatialBlend = 0;
                ambientAudioSource.PlayOneShotWhenReady(audioClip, volumeScale);
            }
        }


        private void SpatializedPlayOneShot(SoundClips clip, Vector3 position, float volumeScale, float minDistance = 8f)
        {
            if (!ambientAudioSource.isPlaying)
            {
                AudioClip audioClip = dfAudioSource.GetAudioClip((int)clip);
                ambientAudioSource.transform.position = position;
                ambientAudioSource.spatialBlend = 1f;
                ambientAudioSource.minDistance = minDistance;
                ambientAudioSource.maxDistance = minDistance * 8;
                ambientAudioSource.PlayOneShotWhenReady(audioClip, volumeScale);
            }
        }

        private float sqr(float x) 
        {
            return x * x;
        }

        private void PlaySomewhereAround(SoundClips clip, float volumeScale)
        {
            Vector3 randomPos = playerBehaviour.transform.position +
                Random.onUnitSphere * Mathf.Sqrt(Random.Range(sqr(10f), sqr(20f)));
            SpatializedPlayOneShot(clip, randomPos, volumeScale, 13f);
        }

        private void PlaySomewhereOnHorizon(SoundClips clip, float volumeScale)
        {
            // Somewhere around, 20° above horizon
            Vector3 randomPos = playerBehaviour.transform.position + 
                Quaternion.AngleAxis(Mathf.Sqrt(Random.Range(0f, sqr(10000f))), Vector3.up) * new Vector3(0.94f, 0.34f, 0f);
            SpatializedPlayOneShot(clip, randomPos, volumeScale, 3000f);
        }

        private void PlayEffects()
        {
            // Do nothing if audio not setup
            if (ambientAudioSource == null || ambientSounds == null)
                return;

            // Get next sound index
            int index = random.Next(0, ambientSounds.Length);

            // Play effect
            if (Presets == AmbientSoundPresets.Storm)
            {
                if (PlayLightningEffect)
                {
                    // Play lightning effects together with appropriate sounds
                    StartCoroutine(PlayLightningEffects(index));
                }
                else
                {
                    // Play ambient sound as a one-shot 3D sound
                    SoundClips clip = ambientSounds[index];
                    PlaySomewhereOnHorizon(clip, 1f);

                    // AmbientPlayOneShot(clip, 5f);
                    RaiseOnPlayEffectEvent(clip);
                }
            }
            else
            {
                // Do not play ambient effect in castle blocks
                if (doNotPlayInCastle)
                {
                    if (playerEnterExit == null)
                        playerEnterExit = GameManager.Instance.PlayerEnterExit;
                    if (playerEnterExit && playerEnterExit.IsPlayerInsideDungeonCastle)
                    {
                        return;
                    }
                }

                // Play ambient sound as a one-shot 3D sound
                SoundClips clip = ambientSounds[index];
                PlaySomewhereAround(clip, 1f);
                RaiseOnPlayEffectEvent(clip);
            }
        }

        private void PlayCemeteryEffects()
        {
            // Do nothing if audio not setup
            if (ambientAudioSource == null)
                return;

            // Get next sound index
            int index = random.Next(0, cemeteryAmbientSounds.Length);

            // Play ambient sound as a one-shot 3D sound
            SoundClips clip = cemeteryAmbientSounds[index];
            PlaySomewhereAround(clip, 1f);
        }

        private IEnumerator PlayLightningEffects(int index)
        {
            //Debug.Log(string.Format("Playing index {0}", index));

            int minFlashes;
            int maxFlashes;
            float soundDelay = 0f;
            const float randomSkip = 0.6f;

            // Store starting values
            //float startSkyScale = 1f;
            float startLightIntensity = 1f;
            //if (SkyForEffects) startSkyScale = SkyForEffects.SkyColorScale;
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
                AmbientPlayOneShot(clip, 1f);
                RaiseOnPlayEffectEvent(clip);
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
                    //if (SkyForEffects) SkyForEffects.SkyColorScale = 2f;
                    if (LightForEffects) LightForEffects.intensity = 2f;
                    yield return new WaitForEndOfFrame();
                }

                // Flash off
                //if (SkyForEffects) SkyForEffects.SkyColorScale = startSkyScale;
                if (LightForEffects) LightForEffects.intensity = startLightIntensity;
                yield return new WaitForEndOfFrame();
            }

            // Reset values just to be sure
            //if (SkyForEffects) SkyForEffects.SkyColorScale = startSkyScale;
            if (LightForEffects) LightForEffects.intensity = startLightIntensity;

            // Delay for sound effect
            if (soundDelay > 0)
                yield return new WaitForSeconds(soundDelay);

            // Play sound effect
            PlaySomewhereOnHorizon(clip, 1f);

            // Raise event
            RaiseOnPlayEffectEvent(clip);
        }

        private void StartWaiting()
        {
            // Reset countdown to next sound
            waitTime = random.Next(MinWaitTime, MaxWaitTime);
            waitCounter = 0;
        }

        private void StartCemeteryWaiting()
        {
            // Reset countdown to next sound
            cemeteryWaitTime = random.Next(CemeteryMinWaitTime, CemeteryMaxWaitTime);
            cemeteryWaitCounter = 0;
        }

        private void ApplyPresets()
        {
            switch (Presets)
            {
                case AmbientSoundPresets.Dungeon:
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
                        SoundClips.AmbientWindBlow1a,
                        SoundClips.AmbientWindBlow1b,
                        SoundClips.AmbientMonsterRoar,
                        SoundClips.AmbientGoldPieces,
                        SoundClips.AmbientBirdCall,
                        SoundClips.AmbientDoorClose,
                    };
                    AmbientInteriorPresets();
                    break;

                case AmbientSoundPresets.Storm:
                    // Set storm one-shots
                    ambientSounds = new SoundClips[] {
                        SoundClips.StormLightningShort,
                        SoundClips.StormLightningThunder,
                        SoundClips.StormThunderRoll,
                    };
                    AmbientExteriorPresets();
                    break;

                case AmbientSoundPresets.SunnyDay:
                    ambientSounds = new SoundClips[]
                    {
                        SoundClips.BirdCall1,
                        SoundClips.BirdCall2,
                    };
                    AmbientExteriorPresets();
                    break;

                case AmbientSoundPresets.ClearNight:
                case AmbientSoundPresets.Rain:
                case AmbientSoundPresets.None:
                    ambientSounds = null;
                    break;
            }

            lastPresets = Presets;
        }

        private void AmbientInteriorPresets()
        {
            // Wider spread angle because of environment reverberation
            ambientAudioSource.spread = 45f;
        }

        private void AmbientExteriorPresets()
        {
            // Narroer spread angle because of lesser environment reverberation
            ambientAudioSource.spread = 15f;
        }

        #endregion

        #region Event Arguments

        /// <summary>
        /// Arguments for AmbientEffectsPlayer events.
        /// </summary>
        public class AmbientEffectsEventArgs : System.EventArgs
        {
            /// <summary>The clip just played.</summary>
            SoundClips Clip { get; set; }

            /// <summary>Constructor.</summary>
            public AmbientEffectsEventArgs()
            {
                this.Clip = SoundClips.None;
            }

            /// <summary>Constructor helper.</summary>
            public AmbientEffectsEventArgs(SoundClips clip)
            {
                this.Clip = clip;
            }
        }

        #endregion

        #region Event Handlers

        // OnPlayEffect
        public delegate void OnPlayEffectEventHandler(AmbientEffectsEventArgs args);
        public static event OnPlayEffectEventHandler OnPlayEffect;
        protected virtual void RaiseOnPlayEffectEvent(SoundClips clip)
        {
            AmbientEffectsEventArgs args = new AmbientEffectsEventArgs(clip);
            if (OnPlayEffect != null)
                OnPlayEffect(args);
        }

        void PlayerGPS_OnEnterLocationRect(DaggerfallConnect.DFLocation location)
        {
            IsCemeteryNearby = false;
            bool isPlayerInside = playerEnterExit.IsPlayerInside;

            if (!isPlayerInside)
            {
                IsCemeteryNearby = (location.MapTableData.LocationType == DaggerfallConnect.DFRegion.LocationTypes.Graveyard);
                if (IsCemeteryNearby)
                    StartCemeteryWaiting();
            }
        }

        void PlayerGPS_OnExitLocationRect()
        {
            IsCemeteryNearby = false;
        }

        private void AmbientEffectsPlayer_OnVideoStart()
        {
            rainLoop = null;
            cricketsLoop = null;

            // Stop playing any loops
            if (loopAudioSource.isPlaying)
            {
                loopAudioSource.Stop();
                loopAudioSource.clip = null;
                loopAudioSource.loop = false;
            }
            IsMuted = true;
        }

        private void AmbientEffectsPlayer_OnVideoEnd()
        {
            IsMuted = false;
        }

        #endregion
    }
}
