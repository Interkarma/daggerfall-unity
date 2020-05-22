// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using DaggerfallWorkshop.Game.Items;
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
        const float levitateMoveSpeed = 4.0f;

        bool playerLevitating = false;
        bool playerSwimming = false;
        PlayerMotor playerMotor;
        PlayerSpeedChanger speedChanger;
        PlayerGroundMotor groundMotor;
        ClimbingMotor climbingMotor;
        Camera playerCamera;
        float moveSpeed = levitateMoveSpeed;
        Vector3 moveDirection = Vector3.zero;

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
            groundMotor = GetComponent<PlayerGroundMotor>();
            speedChanger = GetComponent<PlayerSpeedChanger>();
            climbingMotor = GetComponent<ClimbingMotor>();
            playerCamera = GameManager.Instance.MainCamera;
        }

        private void Update()
        {
            if (!playerMotor || !playerCamera || (!playerLevitating && !playerSwimming))
                return;

            // Cancel levitate movement if player is paralyzed
            if (GameManager.Instance.PlayerEntity.IsParalyzed)
                return;

            // Forward/backwards
            if (InputManager.Instance.HasAction(InputManager.Actions.MoveForwards))
                AddMovement(playerCamera.transform.forward);
            else if (InputManager.Instance.HasAction(InputManager.Actions.MoveBackwards))
                AddMovement(-playerCamera.transform.forward);

            // Right/left
            if (InputManager.Instance.HasAction(InputManager.Actions.MoveRight))
                AddMovement(playerCamera.transform.right);
            else if (InputManager.Instance.HasAction(InputManager.Actions.MoveLeft))
                AddMovement(-playerCamera.transform.right);

            // Up/down
            Vector3 upDownVector = new Vector3 (0, 0, 0);

            bool overEncumbered = (GameManager.Instance.PlayerEntity.CarriedWeightInGoldPieceUnits * 4 > 250 * DaggerfallUnityItem.goldPiecesPerKg) && !playerLevitating && !GameManager.Instance.PlayerEntity.GodMode;
            if (playerSwimming && overEncumbered && !climbingMotor.IsClimbing && !GameManager.Instance.PlayerEntity.IsWaterWalking)
                upDownVector += Vector3.down;
            else if (InputManager.Instance.HasAction(InputManager.Actions.Jump) || InputManager.Instance.HasAction(InputManager.Actions.FloatUp))
                upDownVector += Vector3.up;
            else if (InputManager.Instance.HasAction(InputManager.Actions.Crouch) || InputManager.Instance.HasAction(InputManager.Actions.FloatDown))
                upDownVector += Vector3.down;

            AddMovement(upDownVector, true);

            // Execute movement
            if (moveDirection == Vector3.zero)
            {  
                // Hack to make sure that the player can get pushed by moving objects if he's not moving
                const float pos = 0.0001f;
                groundMotor.MoveWithMovingPlatform(Vector3.up * pos);
                groundMotor.MoveWithMovingPlatform(Vector3.down * pos);
                groundMotor.MoveWithMovingPlatform(Vector3.left * pos);
                groundMotor.MoveWithMovingPlatform(Vector3.right * pos);
                groundMotor.MoveWithMovingPlatform(Vector3.forward * pos);
                groundMotor.MoveWithMovingPlatform(Vector3.back * pos);
            }

            groundMotor.MoveWithMovingPlatform(moveDirection);
            moveDirection = Vector3.zero;
        }

        void AddMovement(Vector3 direction, bool upOrDown = false)
        {
            // No up or down movement while swimming without using a float up/float down key, levitating while swimming, or sinking from encumbrance
            if (!upOrDown && playerSwimming && !playerLevitating)
                direction.y = 0;

            if (playerSwimming && GameManager.Instance.PlayerEntity.IsWaterWalking)
            {
                // Swimming with water walking on makes player move at normal speed in water
                moveSpeed = GameManager.Instance.PlayerMotor.Speed;
                moveDirection += direction * moveSpeed;
                return;
            }
            else if (playerSwimming && !playerLevitating)
            {
                // Do not allow player to swim up out of water, as he would immediately be pulled back in, making jerky movement and playing the splash sound repeatedly
                if ((direction.y > 0) && (playerMotor.controller.transform.position.y + (50 * MeshReader.GlobalScale) - 0.93f) >=
                (GameManager.Instance.PlayerEnterExit.blockWaterLevel * -1 * MeshReader.GlobalScale) &&
                !playerLevitating)
                    direction.y = 0;

                // There's a fixed speed for up/down movement in classic (use moveSpeed = 2 here to replicate)
                float baseSpeed = speedChanger.GetBaseSpeed();
                moveSpeed = speedChanger.GetSwimSpeed(baseSpeed);
            }

            moveDirection += direction * moveSpeed;

            // Reset to levitate speed in case it has been changed by swimming
            moveSpeed = levitateMoveSpeed;
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