// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
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
using System.Collections.Generic;

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

        Vector2 ms;                                 // Mouse swing based on input
        float weaponSensitivity = 1.0f;             // Sensitivity of weapon swings to mouse movements
        bool showDebugStrings = false;              // Draw debug data

        PlayerEntity playerEntity;
        GameObject player;
        GameObject mainCamera;
        bool isAttacking;
        int lastAttackHand = 0;                     // 0-left-hand, 1=right-hand, -1=no weapon
        float cooldownTime = 0.0f;                  // Wait for weapon cooldown

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
            weaponSensitivity = DaggerfallUnity.Settings.WeaponSensitivity;
            showDebugStrings = DaggerfallUnity.Settings.DebugWeaponSwings;
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

            // Do nothing while weapon cooldown
            if (Time.time < cooldownTime)
                return;

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

                // Get attack hand weapon
                FPSWeapon weapon = null; 
                if (lastAttackHand == 0)
                    weapon = LeftHandWeapon;
                else if (lastAttackHand == 1)
                    weapon = RightHandWeapon;

                // Transfer melee damage
                MeleeDamage(weapon, lastAction);

                // Weapon cooldown
                if (weapon.Cooldown > 0.0f)
                {
                    cooldownTime = Time.time + weapon.Cooldown;
                    ShowWeapons(false);
                }

                return;
            }

            // Restore weapon visibility
            ShowWeapons(true);

            // Track mouse swing attacks and exit if no action registered
            TrackMouseAttack();
            if (lastAction != MouseDirections.None && actionCount < TriggerCount)
                return;

            // Time for attacks
            ExecuteAttacks();
        }

        void OnGUI()
        {
            if (Event.current.type.Equals(EventType.Repaint) && showDebugStrings && !Sheathed)
            {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.black;
                string text = GetDebugString();
                GUI.Label(new Rect(4, 4, 800, 24), text, style);
                GUI.Label(new Rect(2, 2, 800, 24), text);
            }
        }

        string GetDebugString()
        {
            return string.Format("WeaponX: {0:0.00} | WeaponY: {1:0.00} | TriggerCount: {2}", ms.x, ms.y, actionCount);
        }

        public void SheathWeapons()
        {
            Sheathed = true;
            ShowWeapons(false);
        }

        public int CalculateWeaponMinDamage(FPSWeapon weapon)
        {
            int damage_low;

            // Hand-to-hand damage formula from Daggerfall Chronicles and testing
            if (weapon.WeaponType == WeaponTypes.Melee)
            {
                int skill = playerEntity.Skills.HandToHand;

                damage_low = (skill / 10) + 1;
            }
            else
            {
                damage_low = Mathf.Max(0, weapon.DamageRange[0] + getMaterialModifier(weapon.MetalType));
            }

            return damage_low;
        }

        public int CalculateWeaponMaxDamage(FPSWeapon weapon)
        {
            int damage_high;

            // Hand-to-hand damage formula from Daggerfall Chronicles and testing
            // Daggerfall Chronicles table lists hand-to-hand skills of 80 and above (45 through 79 are omitted)
            // as if they cause 2 to be added to damage_high instead of 1, but the hand-to-hand damage display
            // in the in-game character sheet contradicts this.
            if (weapon.WeaponType == WeaponTypes.Melee)
            {
                int skill = playerEntity.Skills.HandToHand;

                damage_high = (skill / 5) + 1;
            }
            else
            {
                damage_high = weapon.DamageRange[1] + getMaterialModifier(weapon.MetalType);
            }

            return damage_high;
        }

    public void Reset()
        {
            usingRightHand = true;
            holdingShield = false;
            currentRightHandWeapon = null;
            currentLeftHandWeapon = null;
            SheathWeapons();
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
                    //string message = HardStrings.equippingWeapon;
                    //message = message.Replace("%s", rightHandItem.ItemTemplate.name);
                    //DaggerfallUI.Instance.PopupMessage(message);
                }
                //DaggerfallUI.Instance.PopupMessage(HardStrings.rightHandEquipped);
                currentRightHandWeapon = rightHandItem;
            }

            // Left-hand item changed
            if (!DaggerfallUnityItem.CompareItems(currentLeftHandWeapon, leftHandItem))
            {
                if (leftHandItem != null)
                {
                    //string message = HardStrings.equippingWeapon;
                    //message = message.Replace("%s", leftHandItem.ItemTemplate.name);
                    //DaggerfallUI.Instance.PopupMessage(message);
                }
                //DaggerfallUI.Instance.PopupMessage(HardStrings.leftHandEquipped);
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
            target.SwingWeaponSound = SoundClips.SwingHighPitch;

            // TODO: Adjust FPSWeapon attack speed scale for swing pitch variance
        }

        void SetWeapon(FPSWeapon target, DaggerfallUnityItem weapon)
        {
            // Must be a weapon
            if (weapon.ItemGroup != ItemGroups.Weapons)
                return;

            // Setup target
            target.DamageRange = getBaseWeaponDamageRange(weapon.TemplateIndex);
            target.WeaponType = DaggerfallUnity.Instance.ItemHelper.ConvertItemToAPIWeaponType(weapon);
            target.MetalType = DaggerfallUnity.Instance.ItemHelper.ConvertItemMaterialToAPIMetalType(weapon);
            target.DrawWeaponSound = weapon.GetEquipSound();
            target.SwingWeaponSound = weapon.GetSwingSound();

            // Adjust attributes by weapon type
            if (target.WeaponType == WeaponTypes.Bow)
            {
                target.Reach = 50f;
                target.Cooldown = 1.0f;
            }
            else
            {
                target.Reach = 2.5f;
                target.Cooldown = 0.0f;
            }

            // TODO: Adjust FPSWeapon attack speed scale for swing pitch variance
        }

        #endregion

        #region Private Methods

        private void TrackMouseAttack()
        {
            // Track action for idle plus all eight mouse directions
            ms = new Vector2(InputManager.Instance.MouseX, InputManager.Instance.MouseY) * weaponSensitivity;
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

        private int CalculateWeaponDamage(FPSWeapon weapon, MouseDirections swingDirection)
        {          
            int damage_low = CalculateWeaponMinDamage(weapon);
            int damage_high = CalculateWeaponMaxDamage(weapon);
            int damage = Random.Range(damage_low, damage_high + 1);

            // Apply the strength modifier. Testing in original Daggerfall shows hand-to-hand ignores it.
            if (weapon.WeaponType != WeaponTypes.Melee)
            {
                int strengthModifier = (playerEntity.Stats.Strength / 10) - 5;
               
                damage += strengthModifier;

                // Bows do not factor in swing direction
                if (weapon.WeaponType != WeaponTypes.Bow)
                {
                    damage += getDirectionModifier(swingDirection);
                }
            }

            // Weapons can do 0 damage. Causes no blood or hit sound in original Daggerfall.
            return Mathf.Max(0, damage);
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

        private void MeleeDamage(FPSWeapon weapon, MouseDirections swingDirection)
        {
            if (!mainCamera || !weapon)
                return;

            // Fire ray along player facing using weapon range
            RaycastHit hit;
            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
            if (Physics.SphereCast(ray, SphereCastRadius, out hit, weapon.Reach - SphereCastRadius))
            {
                // Check if hit has an DaggerfallAction component
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

                // Calculate damage
                int damage = CalculateWeaponDamage(weapon, lastAction);

                //// Check if hit has an EnemyHealth
                //// This is part of the old Demo code and will eventually be removed
                //// For now enemies should either use EnemyHealth (deprecated) or EnemyEntity (current) to track enemy health
                //// Never use both components on same enemy
                //EnemyHealth enemyHealth = hit.transform.gameObject.GetComponent<EnemyHealth>();
                //if (enemyHealth)
                //{
                //    // Example: Play sound based on fake parry mechanics
                //    if (Random.value < ChanceToBeParried)
                //    {
                //        // Parried
                //        weapon.PlayParrySound();
                //        return;
                //    }
                //    else
                //    {
                //        // Connected
                //        weapon.PlayHitSound();
                //        enemyHealth.RemoveHealth(player, damage, hit.point);
                //    }
                //}

                // Check if hit an entity and remove health
                DaggerfallEntityBehaviour entityBehaviour = hit.transform.GetComponent<DaggerfallEntityBehaviour>();
                if (entityBehaviour)
                {
                    if (entityBehaviour.EntityType == EntityTypes.EnemyMonster || entityBehaviour.EntityType == EntityTypes.EnemyClass)
                    {
                        EnemyEntity enemyEntity = entityBehaviour.Entity as EnemyEntity;

                        // Play hit sound and trigger blood splash at hit point
                        weapon.PlayHitSound();
                        EnemyBlood blood = hit.transform.GetComponent<EnemyBlood>();
                        if (blood)
                        {
                            blood.ShowBloodSplash(enemyEntity.MobileEnemy.BloodIndex, hit.point);
                        }

                        // Remove health
                        enemyEntity.DecreaseHealth(damage);
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

        // Couldn't find the actual modifier values used in Daggerfall, so defaulted to Arena's modifiers
        private int getDirectionModifier (MouseDirections direction)
        {
            switch (direction) 
            {
                // Vertical Swing
                case MouseDirections.Down:
                    return 4;
                // Diagonal Swing
                case MouseDirections.DownLeft:
                case MouseDirections.DownRight:
                    return 2;
                // Forward Thrust
                case MouseDirections.Up:
                case MouseDirections.UpLeft:
                case MouseDirections.UpRight:
                    return -4;
                // Horizontal Swing
                default:
                    return 0;
            }
        }

        private int getMaterialModifier (MetalTypes type)
        {
            switch (type)
            {
                case MetalTypes.Iron:
                    return -2;
                case MetalTypes.Elven:
                    return 2;
                case MetalTypes.Dwarven:
                    return 4;
                case MetalTypes.Mithril:
                case MetalTypes.Adamantium:
                    return 6;
                case MetalTypes.Ebony:
                    return 8;
                case MetalTypes.Orcish:
                    return 10;
                case MetalTypes.Daedric:
                    return 12;
                // Covers Iron and Steel
                default:
                    return 0;
            }
        }

        private List<int> getBaseWeaponDamageRange (int itemIndex)
        {
            switch (itemIndex)
            {
                case (int)Weapons.Dagger:
                    return new List<int> { 1, 6 };
                case (int)Weapons.Shortsword:
                case (int)Weapons.Tanto:
                case (int)Weapons.Staff:
                    return new List<int> { 1, 8 };
                case (int)Weapons.Wakazashi:
                    return new List<int> { 1, 10 };
                case (int)Weapons.Broadsword:
                case (int)Weapons.Mace:
                    return new List<int> { 1, 12 };
                case (int)Weapons.Battle_Axe:
                    return new List<int> { 2, 12 };
                case (int)Weapons.Flail:
                    return new List<int> { 2, 14 };
                case (int)Weapons.Longsword:
                case (int)Weapons.War_Axe:
                    return new List<int> { 2, 16 };
                case (int)Weapons.Claymore:
                    return new List<int> { 2, 18 };
                case (int)Weapons.Saber:
                    return new List<int> { 3, 12 };
                case (int)Weapons.Katana:
                    return new List<int> { 3, 16 };
                case (int)Weapons.Warhammer:
                    return new List<int> { 3, 18 };
                case (int)Weapons.Dai_Katana:
                    return new List<int> { 3, 21 };
                case (int)Weapons.Short_Bow:
                    return new List<int> { 4, 16 };
                case (int)Weapons.Long_Bow:
                    return new List<int> { 4, 18 };
                default:
                    return new List<int> { 0, 0 };
            }
        }

        #endregion
    }
}