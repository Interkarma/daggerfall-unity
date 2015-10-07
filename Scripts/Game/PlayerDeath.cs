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
using System.Collections;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Handles player death.
    /// </summary>
    public class PlayerDeath : MonoBehaviour
    {
        const float fallSpeed = 2.5f;

        public float FadeDuration = 2f;
        public float TimeBeforeReset = 3;

        DaggerfallEntityBehaviour entityBehaviour;
        PlayerEntity playerEntity;
        CharacterController playerController;
        Camera mainCamera;
        bool deathInProgress;
        float startCameraHeight;
        float targetCameraHeight;
        float currentCameraHeight;
        float timeOfDeath;

        void Awake()
        {
            playerController = GetComponent<CharacterController>();
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            entityBehaviour.OnSetEntity += EntityBehaviour_OnSetEntity;

            // Find main camera gameobject
            GameObject go = GameObject.FindGameObjectWithTag("MainCamera");
            if (go)
            {
                mainCamera = go.GetComponent<Camera>();
            }
        }

        void Update()
        {
            if (deathInProgress)
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
                    Application.LoadLevel(2);
                }
            }
        }

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
            if (deathInProgress)
                return;

            // Start the death process and pause player input
            deathInProgress = true;
            timeOfDeath = Time.fixedTime;
            InputManager.Instance.IsPaused = true;

            // Start camera falling and fading to black
            startCameraHeight = mainCamera.transform.localPosition.y;
            targetCameraHeight -= playerController.height / 3;
            currentCameraHeight = startCameraHeight;
            DaggerfallUI.Instance.FadeToBlack(FadeDuration);
        }
    }
}