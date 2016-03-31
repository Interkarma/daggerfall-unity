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
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Game
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
        public float SphereCastRadius = 0.4f;       // Radius of SphereCast used to target attacks
        public float HorizontalThreshold = 0.8f;    // Horizontal mouse delta threshold for action to register
        public float VerticalThreshold = 0.8f;      // Vertical mouse delta threshold for action to register
        public int TriggerCount = 3;                // Minimum number of times action must register before triggering attack
        public float ChanceToBeParried = 0.1f;      // Example: Chance for player hit to be parried

        MouseDirections lastAction;                 // Last registered action
        int actionCount = 0;                        // Number of times in a row action has been registered
        bool alternateAttack;                       // Flag to flip weapons on alternating attacks

        PlayerEntity playerEntity;
        GameObject player;
        GameObject mainCamera;
        bool isAttacking;
        int lastAttackHand = 0;                     // 0-left-hand, 1=right-hand, -1=no weapon

        bool usingRightHand = true;
        bool holdingShield = false;
        DaggerfallUnityItem currentRightHandWeapon = null;
        DaggerfallUnityItem currentLeftHandWeapon = null;

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
            SetMelee(RightHandWeapon);
        }

        void Update()
        {
            // Automatically update weapons from inventory when PlayerEntity available
            if (playerEntity != null)
                UpdateHands();
            else
                playerEntity = GameManager.Instance.PlayerEntity;

            // Toggle weapon sheath
            if (InputManager.Instance.ActionStarted(InputManager.Actions.ReadyWeapon))
                ToggleSheath();

            // Toggle weapon hand
            if (InputManager.Instance.ActionComplete(InputManager.Actions.SwitchHand))
                ToggleHand();

            // Do nothing if weapons sheathed
            if (Sheathed)
            {
                ShowWeapons(false);
                return;
            }

            // Reset tracking if user not holding down 'SwingWeapon' button and no attack in progress
            //if (!Input.GetButton("Fire2") && !isAttacking)
            if (!InputManager.Instance.HasAction(InputManager.Actions.SwingWeapon) && !isAttacking)
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

        public void SheathWeapons()
        {
            Sheathed = true;
            ShowWeapons(false);
        }

        #region Weapon Setup Methods

        void UpdateHands()
        {
            // Get current items
            DaggerfallUnityItem rightHandItem = playerEntity.ItemEquipTable.GetItem(EquipSlots.RightHand);
            DaggerfallUnityItem leftHandItem = playerEntity.ItemEquipTable.GetItem(EquipSlots.LeftHand);

            // Handle shields
            holdingShield = false;
            if (leftHandItem != null && leftHandItem.IsShield)
            {
                usingRightHand = true;
                holdingShield = true;
                leftHandItem = null;
            }

            // Right-hand item changed
            if (!DaggerfallUnityItem.CompareItems(currentRightHandWeapon, rightHandItem))
            {
                if (rightHandItem != null)
                {
                    string message = HardStrings.equippingWeapon;
                    message = message.Replace("%s", rightHandItem.ItemTemplate.name);
                    DaggerfallUI.Instance.PopupMessage(message);
                }
                DaggerfallUI.Instance.PopupMessage(HardStrings.rightHandEquipped);
                currentRightHandWeapon = rightHandItem;
            }

            // Left-hand item changed
            if (!DaggerfallUnityItem.CompareItems(currentLeftHandWeapon, leftHandItem))
            {
                if (leftHandItem != null)
                {
                    string message = HardStrings.equippingWeapon;
                    message = message.Replace("%s", leftHandItem.ItemTemplate.name);
                    DaggerfallUI.Instance.PopupMessage(message);
                }
                DaggerfallUI.Instance.PopupMessage(HardStrings.leftHandEquipped);
                currentLeftHandWeapon = leftHandItem;
            }

            // Apply weapon settings
            ApplyWeapon();
        }

        void ToggleHand()
        {
            if (usingRightHand && holdingShield)
                return;

            usingRightHand = !usingRightHand;
            if (usingRightHand)
                DaggerfallUI.Instance.PopupMessage(HardStrings.usingRightHand);
            else
                DaggerfallUI.Instance.PopupMessage(HardStrings.usingLeftHand);

            ApplyWeapon();
        }

        void ApplyWeapon()
        {
            if (usingRightHand)
            {
                if (currentRightHandWeapon == null)
                    SetMelee(RightHandWeapon);
                else
                    SetWeapon(RightHandWeapon, currentRightHandWeapon);
            }
            else
            {
                if (currentLeftHandWeapon == null)
                    SetMelee(RightHandWeapon);
                else
                    SetWeapon(RightHandWeapon, currentLeftHandWeapon);
            }
        }

        void SetMelee(FPSWeapon target)
        {
            target.WeaponType = WeaponTypes.Melee;
            target.MetalType = MetalTypes.None;
            target.DrawWeaponSound = SoundClips.None;
            target.SwingWeaponSound = SoundClips.PlayerSwing;

            // TODO: Adjust FPSWeapon attack speed scale for swing pitch variance
        }

        void SetWeapon(FPSWeapon target, DaggerfallUnityItem weapon)
        {
            // Must be a weapon
            if (weapon.ItemGroup != ItemGroups.Weapons)
                return;

            // Setup target
            target.WeaponType = DaggerfallUnity.Instance.ItemHelper.ConvertItemToAPIWeaponType(weapon);
            target.MetalType = DaggerfallUnity.Instance.ItemHelper.ConvertItemMaterialToAPIMetalType(weapon);
            target.DrawWeaponSound = SoundClips.DrawWeapon;
            target.SwingWeaponSound = SoundClips.PlayerSwing;

            // TODO: Adjust FPSWeapon attack speed scale for swing pitch variance
        }

        #endregion

        #region Private Methods

        private void TrackMouseAttack()
        {
            // Track action for idle plus all eight mouse directions
            //var ms = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            var ms = new Vector2(InputManager.Instance.MouseX, InputManager.Instance.MouseY);
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
            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
            if (Physics.SphereCast(ray, SphereCastRadius, out hit, weapon.Reach - SphereCastRadius))
            {
                //check if hit has an DaggerfallAction component
                DaggerfallAction action = hit.transform.gameObject.GetComponent<DaggerfallAction>();
                if (action)
                {
                    action.Receive(player, DaggerfallAction.TriggerTypes.Attack);
                }

                // Check if hit has an DaggerfallActionDoor component
                DaggerfallActionDoor actionDoor = hit.transform.gameObject.GetComponent<DaggerfallActionDoor>();
                if (actionDoor)
                {
                    actionDoor.AttemptBash();
                    return;
                }

                // TODO: Use correct damage based on weapon and swing type
                // Just using fudge values during development
                int damage = Random.Range(1, 25);

                // Check if hit has an EnemyHealth
                // This is part of the old Demo code and will eventually be removed
                // For now enemies should either use EnemyHealth (deprecated) or EnemyEntity (current) to track enemy health
                // Never use both components on same enemy
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
                        enemyHealth.RemoveHealth(player, damage, hit.point);
                    }
                }

                // Check if hit an entity and remove health
                DaggerfallEntityBehaviour entityBehaviour = hit.transform.GetComponent<DaggerfallEntityBehaviour>();
                if (entityBehaviour)
                {
                    if (entityBehaviour.EntityType == EntityTypes.EnemyMonster || entityBehaviour.EntityType == EntityTypes.EnemyClass)
                    {
                        EnemyEntity enemyEntity = entityBehaviour.Entity as EnemyEntity;

                        // Trigger blood splash at hit point
                        EnemyBlood blood = hit.transform.GetComponent<EnemyBlood>();
                        if (blood)
                        {
                            blood.ShowBloodSplash(enemyEntity.MobileEnemy.BloodIndex, hit.point);
                        }

                        // Remove health and handle death
                        enemyEntity.DecreaseHealth(damage);
                        if (enemyEntity.CurrentHealth <= 0)
                        {
                            // Using SendMessage for now, will replace later
                            hit.transform.SendMessage("Die");
                        }
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