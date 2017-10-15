// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// A temporary replacement motor for player levitation.
    /// This is just so player can navigate Mantellan Crux and other places where levitation useful.
    /// Will be removed after PlayerMotor refactor and magic system able to perform job properly.
    /// </summary>
    public class FakeLevitate : MonoBehaviour
    {
        bool playerLevitating = false;
        PlayerMotor playerMotor;
        Camera playerCamera;
        float moveSpeed = 4.0f;

        public bool IsLevitating
        {
            get { return playerLevitating; }
            set { SetLevitating(value); }
        }

        private void Start()
        {
            playerMotor = GetComponent<PlayerMotor>();
            playerCamera = GameManager.Instance.MainCamera;
        }

        private void Update()
        {
            if (!playerMotor || !playerCamera || !playerLevitating)
                return;

            // Forward/backwards
            if (InputManager.Instance.HasAction(InputManager.Actions.MoveForwards))
                Move(playerCamera.transform.forward);
            else if (InputManager.Instance.HasAction(InputManager.Actions.MoveBackwards))
                Move(-playerCamera.transform.forward);

            // Right/left
            if (InputManager.Instance.HasAction(InputManager.Actions.MoveRight))
                Move(playerCamera.transform.right);
            else if (InputManager.Instance.HasAction(InputManager.Actions.MoveLeft))
                Move(-playerCamera.transform.right);

            // Up/down
            if (InputManager.Instance.HasAction(InputManager.Actions.Jump) || InputManager.Instance.HasAction(InputManager.Actions.FloatUp))
                Move(Vector3.up);
            else if(InputManager.Instance.HasAction(InputManager.Actions.Crouch) || InputManager.Instance.HasAction(InputManager.Actions.FloatDown))
                Move(-Vector3.up);
        }

        void Move(Vector3 direction)
        {
            playerMotor.controller.Move(direction * moveSpeed * Time.deltaTime);
        }

        void SetLevitating(bool levitating)
        {
            // Must have PlayerMotor reference
            if (!playerMotor)
                return;

            // Start levitating
            if (!playerLevitating && levitating)
            {
                playerMotor.CancelMovement = true;
                playerLevitating = true;
                return;
            }

            // Stop levitating
            if (playerLevitating && !levitating)
            {
                playerLevitating = false;
                return;
            }
        }
    }
}