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
using System;
using System.Collections;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Handles player death.
    /// </summary>
    public class PlayerDeath : MonoBehaviour
    {
        #region Fields

        public static System.EventHandler OnPlayerDeath;

        const float fallSpeed = 2.5f;

        public const SoundClips classicPlayerDeathSound = SoundClips.WoodElfMalePain1;
        public float FadeDuration = 2f;
        public float TimeBeforeReset = 3;

        StartGameBehaviour startGameBehaviour;
        DaggerfallEntityBehaviour entityBehaviour;
        PlayerEntity playerEntity;
        CharacterController playerController;
        PlayerMotor playerMotor;
        Camera mainCamera;
        bool deathInProgress;
        float startCameraHeight;
        float targetCameraHeight;
        float currentCameraHeight;
        float timeOfDeath;

        #endregion

        #region properties

        public bool DeathInProgress { get { return deathInProgress;} }

        #endregion

        #region Unity

        void Awake()
        {
            playerController = GetComponent<CharacterController>();
            playerMotor = GetComponent<PlayerMotor>();
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            entityBehaviour.OnSetEntity += EntityBehaviour_OnSetEntity;
            mainCamera = GameManager.Instance.MainCamera;

            startCameraHeight = mainCamera.transform.localPosition.y;
        }

        void Start()
        {
            startGameBehaviour = GameObject.FindObjectOfType<StartGameBehaviour>();
            if (!startGameBehaviour)
                throw new Exception("Could not find StartGameBehaviour in scene.");
        }

        void Update()
        {
            if (deathInProgress && mainCamera)
            {
                playerMotor.CancelMovement = true;

                if (currentCameraHeight > targetCameraHeight)
                {
                    currentCameraHeight -= fallSpeed * Time.deltaTime;
                    Vector3 pos = mainCamera.transform.localPosition;
                    pos.y = currentCameraHeight;
                    mainCamera.transform.localPosition = pos;
                }

                if (Time.fixedTime - timeOfDeath > TimeBeforeReset)
                {
                    // Start new game from death cinematic
                    startGameBehaviour.StartMethod = StartGameBehaviour.StartMethods.TitleMenuFromDeath;
                    deathInProgress = false;
                    ResetCamera();
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resets camera after a death event.
        /// </summary>
        public void ResetCamera()
        {
            if (mainCamera)
            {
                Vector3 pos = mainCamera.transform.localPosition;
                pos.y = startCameraHeight;
                mainCamera.transform.localPosition = pos;
            }
        }

        /// <summary>
        /// Clears death animation state and fade.
        /// </summary>
        public void ClearDeathAnimation()
        {
            if (deathInProgress)
            {
                DaggerfallUI.Instance.FadeBehaviour.ClearFade();
                deathInProgress = false;
                InputManager.Instance.IsPaused = false;
                ResetCamera();
            }
        }

        #endregion

        #region Private Methods

        private void EntityBehaviour_OnSetEntity(DaggerfallEntity oldEntity, DaggerfallEntity newEntity)
        {
            if (oldEntity != null)
            {
                oldEntity.OnDeath -= PlayerEntity_OnDeath;
            }

            if (newEntity != null)
            {
                playerEntity = newEntity as PlayerEntity;
                playerEntity.OnDeath += PlayerEntity_OnDeath;
            }
        }

        private void PlayerEntity_OnDeath(DaggerfallEntity entity)
        {
            if (deathInProgress || !mainCamera)
                return;

            // Start the death process and pause player input
            deathInProgress = true;
            timeOfDeath = Time.fixedTime;
            InputManager.Instance.IsPaused = true;

            // Start camera falling and fading to black
            startCameraHeight = mainCamera.transform.localPosition.y;
            targetCameraHeight = playerController.height - (playerController.height * 1.25f);
            currentCameraHeight = startCameraHeight;
            DaggerfallUI.Instance.FadeBehaviour.FadeHUDToBlack(FadeDuration);

            SoundClips sound = classicPlayerDeathSound;
            if (DaggerfallUnity.Settings.CombatVoices)
            {
                // There are 3 pain-like sounds for each race/gender. The third one, used here, sounds like
                // it may have been meant for when the player dies.
                sound = GetRaceGenderPain3Sound(playerEntity.Race, playerEntity.Gender);
            }

            if (DaggerfallUI.Instance.DaggerfallAudioSource)
                DaggerfallUI.Instance.DaggerfallAudioSource.PlayOneShot(sound, 0);

            if (OnPlayerDeath != null)
                OnPlayerDeath(this, null);
        }

        SoundClips GetRaceGenderPain3Sound(Races race, Genders gender)
        {
            switch (race)
            {
                case Races.Breton:
                    return (playerEntity.Gender == Genders.Male) ? SoundClips.BretonMalePain3 : SoundClips.BretonFemalePain3;
                case Races.Redguard:
                    return (playerEntity.Gender == Genders.Male) ? SoundClips.RedguardMalePain3 : SoundClips.RedguardFemalePain3;
                case Races.Nord:
                    return (playerEntity.Gender == Genders.Male) ? SoundClips.NordMalePain3 : SoundClips.NordFemalePain3;
                case Races.DarkElf:
                    return (playerEntity.Gender == Genders.Male) ? SoundClips.DarkElfMalePain3 : SoundClips.DarkElfFemalePain3;
                case Races.HighElf:
                    return (playerEntity.Gender == Genders.Male) ? SoundClips.HighElfMalePain3 : SoundClips.HighElfFemalePain3;
                case Races.WoodElf:
                    return (playerEntity.Gender == Genders.Male) ? SoundClips.WoodElfMalePain3 : SoundClips.WoodElfFemalePain3;
                case Races.Khajiit:
                    return (playerEntity.Gender == Genders.Male) ? SoundClips.KhajiitMalePain3 : SoundClips.KhajiitFemalePain3;
                case Races.Argonian:
                    return (playerEntity.Gender == Genders.Male) ? SoundClips.ArgonianMalePain3 : SoundClips.ArgonianFemalePain3;
                default:
                    return SoundClips.None;
            }
        }

        #endregion
    }
}
