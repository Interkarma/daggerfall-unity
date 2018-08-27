// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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
    /// A temporary replacement motor for player levitation and swimming.
    /// This is just so player can navigate Mantellan Crux and other places where levitation useful, and to allow for work on swimming mechanics.
    /// Will be removed after PlayerMotor refactor and magic system able to perform job properly.
    /// </summary>
    public class LevitateMotor : MonoBehaviour
    {
        bool playerLevitating = false;
        bool playerSwimming = false;
        PlayerMotor playerMotor;
        PlayerSpeedChanger speedChanger;
        Camera playerCamera;
        float moveSpeed = 4.0f;
        private ClimbingMotor climbingMotor;

        public bool IsLevitating
        {
            get { return playerLevitating; }
            set { SetLevitating(value); }
        }

        public bool IsSwimming
        {
            get { return playerSwimming; }
            set { SetSwimming(value); }
        }

        private void Start()
        {
            playerMotor = GetComponent<PlayerMotor>();
            speedChanger = GetComponent<PlayerSpeedChanger>();
            climbingMotor = GetComponent<ClimbingMotor>();
            playerCamera = GameManager.Instance.MainCamera;
        }

        private void Update()
        {
            if (!playerMotor || !playerCamera || (!playerLevitating && !playerSwimming) || climbingMotor.IsClimbing)
                return;

            // Cancel levitate movement if player is paralyzed
            if (GameManager.Instance.PlayerEntity.IsParalyzed)
                return;
            Vector3 wasdDirection = Vector3.zero;
            // Forward/backwards
            if (InputManager.Instance.HasAction(InputManager.Actions.MoveForwards))
                wasdDirection += (playerCamera.transform.forward);
            else if (InputManager.Instance.HasAction(InputManager.Actions.MoveBackwards))
                wasdDirection -= (playerCamera.transform.forward);

            // Right/left
            if (InputManager.Instance.HasAction(InputManager.Actions.MoveRight))
                wasdDirection += (playerCamera.transform.right);
            else if (InputManager.Instance.HasAction(InputManager.Actions.MoveLeft))
                wasdDirection -= (playerCamera.transform.right);

            // Up/down
            Vector3 upDownDirection = Vector3.zero;
            if (InputManager.Instance.HasAction(InputManager.Actions.Jump) || InputManager.Instance.HasAction(InputManager.Actions.FloatUp))
                upDownDirection += Vector3.up;
            if (InputManager.Instance.HasAction(InputManager.Actions.Crouch) || InputManager.Instance.HasAction(InputManager.Actions.FloatDown) ||
                GameManager.Instance.PlayerEnterExit.IsPlayerSwimming && (GameManager.Instance.PlayerEntity.CarriedWeight * 4) > 250)
                upDownDirection += Vector3.down;

            Move(wasdDirection, upDownDirection);
        }

        void Move(Vector3 wasdDirection, Vector3 crouchJumpDirection)
        {
            if (playerSwimming)
            {
                // Do not allow player to swim up out of water, as he would immediately be pulled back in, making jerky movement and playing the splash sound repeatedly
                if ((wasdDirection.y > 0) && (playerMotor.controller.transform.position.y + (50 * MeshReader.GlobalScale) - 0.93f) >=
                (GameManager.Instance.PlayerEnterExit.blockWaterLevel * -1 * MeshReader.GlobalScale) &&
                !playerLevitating)
                    wasdDirection.y = 0;

                Entity.PlayerEntity player = GameManager.Instance.PlayerEntity;
                float baseSpeed = speedChanger.GetBaseSpeed();
                moveSpeed = speedChanger.GetSwimSpeed(baseSpeed);
            }

            Vector3 wasdFormula = wasdDirection * moveSpeed * Time.deltaTime;
            // There's a fixed speed for up/down movement
            Vector3 crouchJumpFormula = crouchJumpDirection * 80f / PlayerSpeedChanger.classicToUnitySpeedUnitRatio * Time.deltaTime;
            Vector3 FinalVector = wasdFormula;

            // use whatever y value is greater for y magnitude
            if (crouchJumpDirection.y > 0)
                FinalVector.y = Mathf.Max(FinalVector.y, crouchJumpFormula.y);
            else if (crouchJumpDirection.y < 0)
                FinalVector.y = Mathf.Min(FinalVector.y, crouchJumpFormula.y);

            playerMotor.controller.Move(FinalVector);
            playerMotor.CollisionFlags = playerMotor.controller.collisionFlags;

            // Reset to levitate speed in case it has been changed by swimming
            moveSpeed = 4.0f;
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
                playerMotor.CancelMovement = true;
                playerLevitating = false;
                return;
            }
        }

        void SetSwimming(bool swimming)
        {
            // Must have PlayerMotor reference
            if (!playerMotor)
                return;

            // Start swimming
            if (!playerSwimming && swimming)
            {
                playerMotor.CancelMovement = true;
                playerSwimming = true;
                return;
            }

            // Stop swimming
            if (playerSwimming && !swimming)
            {
                playerMotor.CancelMovement = true;
                playerSwimming = false;
                return;
            }
        }
    }
}