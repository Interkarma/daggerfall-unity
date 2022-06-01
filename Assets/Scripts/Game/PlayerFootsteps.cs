// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Player footstep sounds.
    /// Designed to be attached to Player object.
    /// </summary>
    [RequireComponent(typeof(DaggerfallAudioSource))]
    public class PlayerFootsteps : MonoBehaviour
    {
        public float WalkStepInterval = 2.5f; // Matched to classic. Was 1.6f;
        public float RunStepInterval = 2.5f; // Matched to classic. Was 1.8f;
        public float GroundDistance = 1.8f;
        public float FootstepVolumeScale = 0.7f;

        public SoundClips FootstepSoundDungeon1 = SoundClips.PlayerFootstepStone1;
        public SoundClips FootstepSoundDungeon2 = SoundClips.PlayerFootstepStone2;
        public SoundClips FootstepSoundOutside1 = SoundClips.PlayerFootstepOutside1;
        public SoundClips FootstepSoundOutside2 = SoundClips.PlayerFootstepOutside2;
        public SoundClips FootstepSoundSnow1 = SoundClips.PlayerFootstepSnow1;
        public SoundClips FootstepSoundSnow2 = SoundClips.PlayerFootstepSnow2;
        public SoundClips FootstepSoundBuilding1 = SoundClips.PlayerFootstepWood1;
        public SoundClips FootstepSoundBuilding2 = SoundClips.PlayerFootstepWood2;
        public SoundClips FootstepSoundShallow = SoundClips.SplashSmallLow;
        public SoundClips FootstepSoundSubmerged = SoundClips.SplashSmall;

        public SoundClips FallHardSound = SoundClips.FallHard;
        public SoundClips FallDamageSound = SoundClips.FallDamage;
        public SoundClips SplashLargeSound = SoundClips.SplashLarge;

        DaggerfallUnity dfUnity;
        PlayerEnterExit playerEnterExit;
        DaggerfallAudioSource dfAudioSource;
        PlayerMotor playerMotor;
        TransportManager transportManager;
        AudioSource customAudioSource;
        AudioClip clip1;
        AudioClip clip2;
        Vector3 lastPosition;
        bool lostGrounding;
        float distance;
        bool alternateStep = false;
        bool ignoreLostGrounding = true;

        SoundClips currentFootstepSound1 = SoundClips.None;
        SoundClips currentFootstepSound2 = SoundClips.None;

        DaggerfallDateTime.Seasons currentSeason = DaggerfallDateTime.Seasons.Summer;
        int currentClimateIndex;
        bool isInside = false;
        bool isInOutsideWater = false;
        bool isInOutsidePath = false;
        bool isOnStaticGeometry = false;

        void Start()
        {
            // Store references
            dfUnity = DaggerfallUnity.Instance;
            dfAudioSource = GetComponent<DaggerfallAudioSource>();
            playerMotor = GetComponent<PlayerMotor>();
            playerEnterExit = GetComponent<PlayerEnterExit>();
            transportManager = GetComponent<TransportManager>();

            // CustomAudioSource was here for adjusting pitch. It should be removable now, but doing so makes the swimming sound loud, so leaving it for now.
            customAudioSource = gameObject.AddComponent<AudioSource>();
            customAudioSource.hideFlags = HideFlags.HideInInspector;
            customAudioSource.playOnAwake = false;
            customAudioSource.loop = false;
            customAudioSource.dopplerLevel = 0f;
            customAudioSource.spatialBlend = 0f;

            // Set start position
            lastPosition = GetHorizontalPosition();

            // Set starting footsteps
            currentFootstepSound1 = FootstepSoundDungeon1;
            currentFootstepSound2 = FootstepSoundDungeon2;
        }

        void FixedUpdate()
        {

            //this condition helps prevent making a nuisance footstep noise when the player first
            //loads a save, or into an interior or exterior location
            if (GameManager.Instance.SaveLoadManager.LoadInProgress || GameManager.Instance.StreamingWorld.IsRepositioningPlayer)
            {
                ignoreLostGrounding = true;
                return;
            }

            DaggerfallDateTime.Seasons playerSeason = dfUnity.WorldTime.Now.SeasonValue;
            int playerClimateIndex = GameManager.Instance.PlayerGPS.CurrentClimateIndex;

            // Get player inside flag
            // Can only do this when PlayerEnterExit is available, otherwise default to true
            bool playerInside = (playerEnterExit == null) ? true : playerEnterExit.IsPlayerInside;
            bool playerInBuilding = (playerEnterExit == null) ? false : playerEnterExit.IsPlayerInsideBuilding;

            // Play splash footsteps whether player is walking on or swimming in exterior water
            bool playerOnExteriorWater = (GameManager.Instance.PlayerMotor.OnExteriorWater == PlayerMotor.OnExteriorWaterMethod.Swimming || GameManager.Instance.PlayerMotor.OnExteriorWater == PlayerMotor.OnExteriorWaterMethod.WaterWalking);

            bool playerOnExteriorPath = GameManager.Instance.PlayerMotor.OnExteriorPath;
            bool playerOnStaticGeometry = GameManager.Instance.PlayerMotor.OnExteriorStaticGeometry;

            // Change footstep sounds between winter/summer variants, when player enters/exits an interior space, or changes between path, water, or other outdoor ground
            if (playerSeason != currentSeason || playerClimateIndex != currentClimateIndex || isInside != playerInside || playerOnExteriorWater != isInOutsideWater || playerOnExteriorPath != isInOutsidePath || playerOnStaticGeometry != isOnStaticGeometry)
            {
                currentSeason = playerSeason;
                currentClimateIndex = playerClimateIndex;
                isInside = playerInside;
                isInOutsideWater = playerOnExteriorWater;
                isInOutsidePath = playerOnExteriorPath;
                isOnStaticGeometry = playerOnStaticGeometry;

                if (!isInside && !playerOnStaticGeometry)
                {
                    if (currentSeason == DaggerfallDateTime.Seasons.Winter && !WeatherManager.IsSnowFreeClimate(currentClimateIndex))
                    {
                        currentFootstepSound1 = FootstepSoundSnow1;
                        currentFootstepSound2 = FootstepSoundSnow2;
                    }
                    else
                    {
                        currentFootstepSound1 = FootstepSoundOutside1;
                        currentFootstepSound2 = FootstepSoundOutside2;
                    }
                }
                else if (playerInBuilding)
                {
                    currentFootstepSound1 = FootstepSoundBuilding1;
                    currentFootstepSound2 = FootstepSoundBuilding2;
                }
                else // in dungeon
                {
                    currentFootstepSound1 = FootstepSoundDungeon1;
                    currentFootstepSound2 = FootstepSoundDungeon2;
                }

                clip1 = null;
                clip2 = null;
            }

            // walking on water tile
            if (playerOnExteriorWater)
            {
                currentFootstepSound1 = FootstepSoundSubmerged;
                currentFootstepSound2 = FootstepSoundSubmerged;
                clip1 = null;
                clip2 = null;
            }

            // walking on path tile
            if (playerOnExteriorPath)
            {
                currentFootstepSound1 = FootstepSoundDungeon1;
                currentFootstepSound2 = FootstepSoundDungeon2;
                clip1 = null;
                clip2 = null;
            }

            // Use water sounds if in dungeon water
            if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon && playerEnterExit.blockWaterLevel != 10000)
            {
                // In water, deep depth
                if ((currentFootstepSound1 != FootstepSoundSubmerged) && playerEnterExit.IsPlayerSwimming)
                {
                    currentFootstepSound1 = FootstepSoundSubmerged;
                    currentFootstepSound2 = FootstepSoundSubmerged;
                    clip1 = null;
                    clip2 = null;
                }
                // In water, shallow depth
                else if ((currentFootstepSound1 != FootstepSoundShallow) && !playerEnterExit.IsPlayerSwimming && (playerMotor.transform.position.y - 0.57f) < (playerEnterExit.blockWaterLevel * -1 * MeshReader.GlobalScale))
                {
                    currentFootstepSound1 = FootstepSoundShallow;
                    currentFootstepSound2 = FootstepSoundShallow;
                    clip1 = null;
                    clip2 = null;
                }
            }

            // Not in water, reset footsteps to normal
            if ((!playerOnExteriorWater) 
                && (currentFootstepSound1 == FootstepSoundSubmerged || currentFootstepSound1 == FootstepSoundShallow)
                && (playerEnterExit.blockWaterLevel == 10000 || (playerMotor.transform.position.y - 0.95f) >= (playerEnterExit.blockWaterLevel * -1 * MeshReader.GlobalScale)))
            {
                currentFootstepSound1 = FootstepSoundDungeon1;
                currentFootstepSound2 = FootstepSoundDungeon2;

                clip1 = null;
                clip2 = null;
            }

            // Reload clips if needed
            if (clip1 == null)
            {
                clip1 = dfAudioSource.GetAudioClip((int)currentFootstepSound1);
            }
            if (clip2 == null)
            {
                clip2 = dfAudioSource.GetAudioClip((int)currentFootstepSound2);
            }

            // Check whether player is on foot and abort playing footsteps if not.
            if (playerMotor.IsLevitating || !transportManager.IsOnFoot && GameManager.Instance.PlayerMotor.OnExteriorWater == PlayerMotor.OnExteriorWaterMethod.None)
            {
                distance = 0f;
                return;
            }

            // Check if player is grounded
            // Note: In classic, submerged "footstep" sound is only played when walking on the floor while in the water, but it sounds like a swimming sound
            // and when outside is played while swimming at the water's surface, so it seems better to play it all the time while submerged in water.
            if (!playerMotor.IsSwimming)
            {
                if (!IsGrounded())
                {
                    // Player has lost grounding
                    distance = 0f;
                    lostGrounding = true;
                    return;
                }
                else
                {
                    // Player is grounded but we might need to reset after losing grounding
                    if (lostGrounding)
                    {
                        distance = 0f;
                        lastPosition = GetHorizontalPosition();
                        lostGrounding = false;

                        if (ignoreLostGrounding)
                            ignoreLostGrounding = false;
                        else if (customAudioSource && clip1 && clip2)
                        {
                            if (!alternateStep)
                                customAudioSource.PlayOneShot(clip1, FootstepVolumeScale * DaggerfallUnity.Settings.SoundVolume);
                            else
                                customAudioSource.PlayOneShot(clip2, FootstepVolumeScale * DaggerfallUnity.Settings.SoundVolume);

                            alternateStep = (!alternateStep);
                        }
                        return;
                    }
                }
            }

            if (playerMotor.IsStandingStill)
                return;

            // Get distance player travelled horizontally
            Vector3 position = GetHorizontalPosition();
            distance += Vector3.Distance(position, lastPosition);
            lastPosition = position;

            // Get threshold
            float threshold = WalkStepInterval;
            if (playerMotor)
                threshold = (playerMotor.IsRunning) ? RunStepInterval : WalkStepInterval;

            // Play sound if over distance threshold
            if (distance > threshold && customAudioSource && clip1 && clip2)
            {
                float volumeScale = FootstepVolumeScale;
                if (playerMotor.IsMovingLessThanHalfSpeed)
                    volumeScale *= 0.5f;

                if (!alternateStep)
                    customAudioSource.PlayOneShot(clip1, volumeScale * DaggerfallUnity.Settings.SoundVolume);
                else
                    customAudioSource.PlayOneShot(clip2, volumeScale * DaggerfallUnity.Settings.SoundVolume);

                alternateStep = (!alternateStep);
                distance = 0f;
            }
        }

        #region Private Methods

        private Vector3 GetHorizontalPosition()
        {
            return new Vector3(transform.position.x, 0, transform.position.z);
        }

        private bool IsGrounded()
        {
            return playerMotor.IsGrounded;
        }

        // Capture this message so we can play fall damage sound
        private void ApplyPlayerFallDamage(float fallDistance)
        {
            // Play falling damage one-shot through normal audio source
            if (dfAudioSource)
                dfAudioSource.PlayOneShot((int)FallDamageSound, 0, FootstepVolumeScale);
        }

        // Capture this message so we can play hard fall sound
        private void HardFallAlert(float fallDistance)
        {
            // Play hard fall one-shot through normal audio source
            if (dfAudioSource)
                dfAudioSource.PlayOneShot((int)FallHardSound, 0, FootstepVolumeScale);
        }

        // Capture this message so we can play large splash sound
        public void PlayLargeSplash()
        {
            if (dfAudioSource)
                dfAudioSource.PlayOneShot((int)SplashLargeSound, 0, FootstepVolumeScale);
        }

        // Capture this message so we can play enemies' weapon hit sounds on player
        public void PlayWeaponHitSound()
        {
            if (dfAudioSource)
            {
                dfAudioSource.PlayOneShot((int)SoundClips.Hit1 + Random.Range(0, 5), 0, 1f);
            }
        }

        // Capture this message so we can play enemies' weaponless hit sounds on player
        public void PlayWeaponlessHitSound()
        {
            if (dfAudioSource)
            {
                dfAudioSource.PlayOneShot((int)SoundClips.Hit1 + Random.Range(2, 4), 0, 1f);
            }
        }

        // Capture this message so we can play pain voice
        public void RemoveHealth(int amount)
        {
            // Racial override can suppress optional attack voice
            RacialOverrideEffect racialOverride = GameManager.Instance.PlayerEffectManager.GetRacialOverrideEffect();
            bool suppressCombatVoices = racialOverride != null && racialOverride.SuppressOptionalCombatVoices;

            if (dfAudioSource && DaggerfallUnity.Settings.CombatVoices && !suppressCombatVoices && Dice100.SuccessRoll(40))
            {
                Entity.PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
                bool heavyDamage = amount >= playerEntity.MaxHealth / 4;
                SoundClips sound = Entity.DaggerfallEntity.GetRaceGenderPainSound(playerEntity.Race, playerEntity.Gender, heavyDamage);
                float pitch = dfAudioSource.AudioSource.pitch;
                dfAudioSource.AudioSource.pitch = pitch + Random.Range(0, 0.3f);
                dfAudioSource.PlayOneShot((int)sound, 0, 1f);
                dfAudioSource.AudioSource.pitch = pitch;
            }
        }

        // Capture this message so we can play enemies' arrow sounds on player
        public void PlayArrowSound()
        {
            if (dfAudioSource)
            {
                dfAudioSource.PlayOneShot((int)SoundClips.ArrowHit, 0, 1f);
            }
        }

    #endregion
}
}
