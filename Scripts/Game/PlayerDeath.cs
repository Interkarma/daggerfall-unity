// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
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

        public float FadeDuration = 2f;
        public float TimeBeforeReset = 3;

        StartGameBehaviour startGameBehaviour;
        DaggerfallEntityBehaviour entityBehaviour;
        PlayerEntity playerEntity;
        CharacterController playerController;
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
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            entityBehaviour.OnSetEntity += EntityBehaviour_OnSetEntity;
            mainCamera = GameManager.Instance.MainCamera;
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
            DaggerfallUI.Instance.FadeHUDToBlack(FadeDuration);

            if (OnPlayerDeath != null)
                OnPlayerDeath(this, null);
        }

        #endregion
    }
}