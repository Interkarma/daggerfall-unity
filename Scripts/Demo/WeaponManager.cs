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
    /// Support for mouse attack gestures, weapon state firing, and damage transfer.
    /// Should only be attached to player game object.
    /// </summary>
    public class WeaponManager : MonoBehaviour
    {
        public FPSWeapon LeftHandWeapon;            // Weapon in left hand
        public FPSWeapon RightHandWeapon;           // Weapon in right hand
        public bool Sheathed;                       // Weapon (or weapons) are sheathed
        public float SphereCastRadius = 0.35f;      // Radius of SphereCast used to target attacks
        public float HorizontalThreshold = 0.8f;    // Horizontal mouse delta threshold for action to register
        public float VerticalThreshold = 0.8f;      // Vertical mouse delta threshold for action to register
        public int TriggerCount = 3;                // Minimum number of times action must register before triggering attack
        public float ChanceToBeParried = 0.1f;      // Example: Chance for player hit to be parried

        MouseDirections lastAction;                 // Last registered action
        int actionCount = 0;                        // Number of times in a row action has been registered
        bool alternateAttack;                       // Flag to flip weapons on alternating attacks

        GameObject player;
        GameObject mainCamera;
        bool isAttacking;
        int lastAttackHand = 0;                     // 0-left-hand, 1=right-hand, -1=no weapon

        /// <summary>
        /// Mouse directions for attack trigger.
        /// </summary>
        public enum MouseDirections
        {
            None,
            UpLeft,
            Up,
            UpRight,
            Left,
            Right,
            DownLeft,
            Down,
            DownRight,
        }

        void Start()
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            player = transform.gameObject;
        }

        void Update()
        {
            // Toggle weapon sheath
            if (Input.GetKeyDown(KeyCode.Z))
                ToggleSheath();

            // Do nothing if weapons sheathed
            if (Sheathed)
            {
                ShowWeapons(false);
                return;
            }

            // Only track mouse if user holding down rmb and not attacking
            if (!Input.GetButton("Fire2") && !isAttacking)
            {
                lastAction = MouseDirections.None;
                actionCount = 0;
                isAttacking = false;
                ShowWeapons(true);
                return;
            }

            // Handle attack in progress
            if (IsLeftHandAttacking() || IsRightHandAttacking())
            {
                isAttacking = true;
                return;
            }

            // If an attack was in progress it is now complete.
            // Attempt to transfer damage based on last attack hand.
            if (isAttacking)
            {
                // Complete attack
                isAttacking = false;
                
                // Transfer melee damage
                if (lastAttackHand == 0)
                    MeleeDamage(LeftHandWeapon);
                else if (lastAttackHand == 1)
                    MeleeDamage(RightHandWeapon);
            }

            // Restore weapon visibility
            ShowWeapons(true);

            // Track mouse attack and exit if no action registered
            TrackMouseAttack();
            if (lastAction == MouseDirections.None || actionCount < TriggerCount)
                return;

            // Time for attacks
            ExecuteAttacks();
        }

        #region Private Methods

        private void TrackMouseAttack()
        {
            // Track action for idle plus all eight mouse directions
            var ms = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            if (IsPassive(ms.x, HorizontalThreshold) && IsPassive(ms.y, VerticalThreshold))
                TrackAction(MouseDirections.None);
            else if (IsNegative(ms.x, HorizontalThreshold) && IsPositive(ms.y, VerticalThreshold))
                TrackAction(MouseDirections.UpLeft);
            else if (IsPassive(ms.x, HorizontalThreshold) && IsPositive(ms.y, VerticalThreshold))
                TrackAction(MouseDirections.Up);
            else if (IsPositive(ms.x, HorizontalThreshold) && IsPositive(ms.y, VerticalThreshold))
                TrackAction(MouseDirections.UpRight);
            else if (IsNegative(ms.x, HorizontalThreshold) && IsPassive(ms.y, VerticalThreshold))
                TrackAction(MouseDirections.Left);
            else if (IsPositive(ms.x, HorizontalThreshold) && IsPassive(ms.y, VerticalThreshold))
                TrackAction(MouseDirections.Right);
            else if (IsNegative(ms.x, HorizontalThreshold) && IsNegative(ms.y, VerticalThreshold))
                TrackAction(MouseDirections.DownLeft);
            else if (IsPassive(ms.x, HorizontalThreshold) && IsNegative(ms.y, VerticalThreshold))
                TrackAction(MouseDirections.Down);
            else if (IsPositive(ms.x, HorizontalThreshold) && IsNegative(ms.y, VerticalThreshold))
                TrackAction(MouseDirections.DownRight);
        }

        private void ExecuteAttacks()
        {
            // Perform dual-wield attacks
            if (LeftHandWeapon && RightHandWeapon)
            {
                // Hand-specific attacks
                if (lastAction == MouseDirections.Right || lastAction == MouseDirections.DownRight)
                {
                    RightHandWeapon.ShowWeapon = false;
                    LeftHandWeapon.OnAttackDirection(lastAction);
                    lastAttackHand = 0;
                    return;
                }
                else if (lastAction == MouseDirections.Left || lastAction == MouseDirections.DownLeft)
                {
                    LeftHandWeapon.ShowWeapon = false;
                    RightHandWeapon.OnAttackDirection(lastAction);
                    lastAttackHand = 1;
                    return;
                }
                else
                {
                    // Alternate attack hand
                    alternateAttack = !alternateAttack;
                }

                // Fire alternating attack
                if (alternateAttack)
                {
                    LeftHandWeapon.OnAttackDirection(lastAction);
                    lastAttackHand = 0;
                }
                else
                {
                    RightHandWeapon.OnAttackDirection(lastAction);
                    lastAttackHand = 1;
                }
            }
            else if (LeftHandWeapon && !RightHandWeapon)
            {
                // Just fire left hand
                LeftHandWeapon.OnAttackDirection(lastAction);
                lastAttackHand = 0;
            }
            else if (!LeftHandWeapon && RightHandWeapon)
            {
                // Just fire right hand
                RightHandWeapon.OnAttackDirection(lastAction);
                lastAttackHand = 1;
            }
            else
            {
                // No weapons set, no attacks possible
                lastAttackHand = -1;
                return;
            }
        }

        private bool IsPassive(float value, float threshold)
        {
            if (value > -threshold && value < threshold)
                return true;
            else
                return false;
        }

        private bool IsNegative(float value, float threshold)
        {
            if (value < -threshold)
                return true;
            else
                return false;
        }

        private bool IsPositive(float value, float threshold)
        {
            if (value > threshold)
                return true;
            else
                return false;
        }

        private void TrackAction(MouseDirections action)
        {
            if (action == lastAction)
            {
                actionCount++;
                return;
            }
            else
            {
                lastAction = action;
                actionCount = 0;
            }
        }

        private bool IsLeftHandAttacking()
        {
            if (!LeftHandWeapon)
                return false;
            else
                return LeftHandWeapon.IsAttacking();
        }

        private bool IsRightHandAttacking()
        {
            if (!RightHandWeapon)
                return false;
            else
                return RightHandWeapon.IsAttacking();
        }

        private void ShowWeapons(bool show)
        {
            if (LeftHandWeapon) LeftHandWeapon.ShowWeapon = show;
            if (RightHandWeapon) RightHandWeapon.ShowWeapon = show;
        }

        private void MeleeDamage(FPSWeapon weapon)
        {
            if (!mainCamera || !weapon)
                return;

            // Fire ray along player facing using weapon range
            RaycastHit hit;
            Ray ray = new Ray(mainCamera.transform.position + mainCamera.transform.forward * 0.1f, mainCamera.transform.forward);
            if (Physics.SphereCast(ray, SphereCastRadius, out hit, weapon.Range - SphereCastRadius))
            {
                // Check if hit has an DaggerfallActionDoor component
                DaggerfallActionDoor actionDoor = hit.transform.gameObject.GetComponent<DaggerfallActionDoor>();
                if (actionDoor)
                {
                    actionDoor.AttemptBash();
                    return;
                }

                // Check if hit has an EnemyHealth - this should be done with Enemy tag in a real project
                EnemyHealth enemyHealth = hit.transform.gameObject.GetComponent<EnemyHealth>();
                if (enemyHealth)
                {
                    // Example: Play sound based on fake parry mechanics
                    if (Random.value < ChanceToBeParried)
                    {
                        // Parried
                        weapon.PlayParrySound();
                        return;
                    }
                    else
                    {
                        // Connected
                        weapon.PlayHitSound();
                        enemyHealth.RemoveHealth(player, Random.Range(weapon.MinDamage, weapon.MaxDamage), hit.point);
                    }
                }
            }
        }

        private void ToggleSheath()
        {
            Sheathed = !Sheathed;
            if (!Sheathed)
            {
                if (LeftHandWeapon) LeftHandWeapon.PlayActivateSound();
                if (RightHandWeapon) RightHandWeapon.PlayActivateSound();
            }
        }

        #endregion
    }
}