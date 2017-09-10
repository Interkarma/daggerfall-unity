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
using System;
using System.Collections.Generic;
using System.Linq;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Formulas;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Support for mouse attack gestures, weapon state firing, and damage transfer.
    /// Should only be attached to player game object.
    /// </summary>
    public class WeaponManager : MonoBehaviour
    {
        // Max time-length of a trail of mouse positions for attack gestures
        private const float MaxGestureSeconds = 1.0f;

        public FPSWeapon LeftHandWeapon;            // Weapon in left hand
        public FPSWeapon RightHandWeapon;           // Weapon in right hand
        public bool Sheathed;                       // Weapon (or weapons) are sheathed
        public float SphereCastRadius = 0.3f;       // Radius of SphereCast used to target attacks
        [Range(0, 1)]
        public float AttackThreshold = 0.05f;        // Minimum mouse gesture travel distance for an attack. % of screen
        public float ChanceToBeParried = 0.1f;      // Example: Chance for player hit to be parried

        bool alternateAttack;                       // Flag to flip weapons on alternating attacks

        float weaponSensitivity = 1.0f;             // Sensitivity of weapon swings to mouse movements
        private Gesture _gesture;
        private int _longestDim;                     // Longest screen dimension, used to compare gestures for attack

        PlayerEntity playerEntity;
        GameObject player;
        GameObject mainCamera;
        bool isStartingAttack = false;
        bool isDamageFinished = false;
        Hand lastAttackHand = Hand.None;
        float cooldownTime = 0.0f;                  // Wait for weapon cooldown
        int swingWeaponFatigueLoss = 11;            // According to DF Chronicles and verified in classic

        bool usingRightHand = true;
        bool holdingShield = false;
        DaggerfallUnityItem currentRightHandWeapon = null;
        DaggerfallUnityItem currentLeftHandWeapon = null;

        #region Properties

        public bool UsingRightHand { get { return usingRightHand; } set { usingRightHand = value; } }

        #endregion

        /// <summary>
        /// Tracks mouse gestures. Auto trims the list of mouse x/ys based on time.
        /// </summary>
        private class Gesture
        {
            // The cursor is auto-centered every frame so the x/y becomes delta x/y
            private readonly List<TimestampedMotion> _points;
            // The result of the sum of all points in the gesture trail
            private Vector2 _sum;
            // The total travel distance of the gesture trail
            // This isn't equal to the magnitude of the sum because the trail may bend
            public float TravelDist { get; private set; }

            public Gesture()
            {
                _points = new List<TimestampedMotion>();
                _sum = new Vector2();
                TravelDist = 0f;
            }

            // Trims old gesture points & keeps the sum and travel variables up to date
            private void TrimOld()
            {
                var old = 0;
                foreach (var point in _points)
                {
                    if (Time.time - point.Time <= MaxGestureSeconds)
                        continue;
                    old++;
                    _sum -= point.Delta;
                    TravelDist -= point.Delta.magnitude;
                }
                _points.RemoveRange(0, old);
            }

            /// <summary>
            /// Adds the given delta mouse x/ys top the gesture trail
            /// </summary>
            /// <param name="dx">Mouse delta x</param>
            /// <param name="dy">Mouse delta y</param>
            /// <returns>The summed vector of the gesture (not the trail itself)</returns>
            public Vector2 Add(float dx, float dy)
            {
                TrimOld();

                _points.Add(new TimestampedMotion
                {
                    Time = Time.time,
                    Delta = new Vector2 {x = dx, y = dy}
                });
                _sum += _points.Last().Delta;
                TravelDist += _points.Last().Delta.magnitude;

                return new Vector2 {x = _sum.x, y = _sum.y};
            }

            /// <summary>
            /// Clears the gesture
            /// </summary>
            public void Clear()
            {
                _points.Clear();
                _sum *= 0;
                TravelDist = 0f;
            }
        }

        /// <summary>
        /// A timestamped motion point
        /// </summary>
        private struct TimestampedMotion
        {
            public float Time;
            public Vector2 Delta;

            public override string ToString()
            {
                return string.Format("t={0}s, dx={1}, dy={2}", Time, Delta.x, Delta.y);
            }
        }

        private enum Hand
        {
            None,
            Left,
            Right
        }

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
            DownRight
        }

        void Start()
        {
            weaponSensitivity = DaggerfallUnity.Settings.WeaponSensitivity;
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            player = transform.gameObject;
            _gesture = new Gesture();
            _longestDim = Math.Max(Screen.width, Screen.height);
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

            bool isBowAttacking = (RightHandWeapon && !LeftHandWeapon && (RightHandWeapon.WeaponType == WeaponTypes.Bow))
    || (!RightHandWeapon && LeftHandWeapon && (LeftHandWeapon.WeaponType == WeaponTypes.Bow));

            // If the last attack has tried to apply damage and the swing animation has finished, reset isDamageFinished
            if (isDamageFinished && !IsLeftHandAttacking() && !IsRightHandAttacking())
                isDamageFinished = false;

            // Reset tracking if user not holding down 'SwingWeapon' button and no attack in progress
            if (!InputManager.Instance.HasAction(InputManager.Actions.SwingWeapon) && !isStartingAttack)
            {
                lastAttackHand = Hand.None;
                _gesture.Clear();
                ShowWeapons(true);
                return;
            }

            // Handle attack in progress. For melee weapons set isAttacking but don't go further until the animation passes the middle frame.
            // This recreates timing similar to classic Daggerfall.
            // Bow is plays through its full animation before proceeding.
            if (!isBowAttacking && ((IsLeftHandAttacking() && !LeftHandWeapon.IsPastMiddleFrame())
                || (IsRightHandAttacking() && !RightHandWeapon.IsPastMiddleFrame())))
            {
                isStartingAttack = true;
                return;
            }
            else if (isBowAttacking && (IsLeftHandAttacking() || IsRightHandAttacking()))
            {
                isStartingAttack = true;
                return;
            }

            // The attack has passed the middle animation frame.
            // Attempt to transfer damage based on last attack hand.
            if (isStartingAttack)
            {
                // First part of attack is over (first half of melee swing, or the full animation for the bow)
                isStartingAttack = false;

                // Get attack hand weapon
                FPSWeapon weapon;
                switch (lastAttackHand)
                {
                    case Hand.Left:
                        weapon = LeftHandWeapon;
                        break;
                    case Hand.Right:
                        weapon = RightHandWeapon;
                        break;
                    default:
                        return;
                }

                // Transfer damage.
                bool hitEnemy = false;
                WeaponDamage(weapon, out hitEnemy);

                // Fatigue loss
                playerEntity.DecreaseFatigue(swingWeaponFatigueLoss);

                // Play swing sound if attack didn't hit an enemy.
                if (!hitEnemy)
                    weapon.PlaySwingSound();
                else
                {
                    // Tally skills
                    if (weapon.WeaponType == WeaponTypes.Melee || weapon.WeaponType == WeaponTypes.Werecreature)
                        playerEntity.TallySkill((short)Skills.HandToHand, 1);
                    else if (usingRightHand && (currentRightHandWeapon != null))
                        playerEntity.TallySkill(currentRightHandWeapon.GetWeaponSkillID(), 1);
                    else if (currentLeftHandWeapon != null)
                        playerEntity.TallySkill(currentLeftHandWeapon.GetWeaponSkillID(), 1);

                    playerEntity.TallySkill((short)Skills.CriticalStrike, 1);
                }

                // Damage transfer is done. The attack now plays through the remainder of its animation frames.
                isDamageFinished = true;

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

            var attackDirection = MouseDirections.None;
            if (isBowAttacking)
            {
                // Ensure attack button was released before starting the next attack
                if (lastAttackHand == Hand.None) 
                    attackDirection = MouseDirections.Down; // Force attack without tracking a swing for Bow
            }
            else
                attackDirection = TrackMouseAttack(); // Track swing direction for other weapons

            // Exit if no attack action registered
            if (attackDirection != MouseDirections.None)
                ExecuteAttacks(attackDirection);
        }

        public void SheathWeapons()
        {
            Sheathed = true;
            ShowWeapons(false);
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

        MouseDirections TrackMouseAttack()
        {
            // Track action for idle plus all eight mouse directions
            var sum = _gesture.Add(InputManager.Instance.MouseX, InputManager.Instance.MouseY) * weaponSensitivity;

            // Short mouse gestures are ignored
            if (_gesture.TravelDist/_longestDim < AttackThreshold)
                return MouseDirections.None;

            // Treat mouse movement as a vector from the origin
            // The angle of the vector will be used to determine the angle of attack/swing
            var angle = Mathf.Atan2(sum.y, sum.x) * Mathf.Rad2Deg;
            // Put angle into 0 - 360 deg range
            if (angle < 0f) angle += 360f;
            // The swing gestures are divided into radial segments
            // Up-down and left-right attacks are in a 30 deg cone about the x/y axes
            // Up-right and up-left aren't valid so the up range is expanded to fill the range
            // The remaining 60 deg quadrants trigger the diagonal attacks
            var radialSection = Mathf.CeilToInt(angle / 15f);
            MouseDirections direction;
            switch (radialSection)
            {
                case 0: // 0 - 15 deg
                case 1:
                case 24: // 345 - 365 deg
                    direction = MouseDirections.Right;
                    break;
                case 2: // 15 - 75 deg
                case 3:
                case 4:
                case 5:
                case 6: // 75 - 105 deg
                case 7:
                case 8: // 105 - 165 deg
                case 9:
                case 10:
                case 11:
                    direction = MouseDirections.Up;
                    break;
                case 12: // 165 - 195 deg
                case 13:
                    direction = MouseDirections.Left;
                    break;
                case 14: // 195 - 255 deg
                case 15:
                case 16:
                case 17:
                    direction = MouseDirections.DownLeft;
                    break;
                case 18: // 255 - 285 deg
                case 19:
                    direction = MouseDirections.Down;
                    break;
                case 20: // 285 - 345 deg
                case 21:
                case 22:
                case 23:
                    direction = MouseDirections.DownRight;
                    break;
                default: // Won't happen
                    direction = MouseDirections.None;
                    break;
            }
            _gesture.Clear();
            return direction;
        }

        void ExecuteAttacks(MouseDirections direction)
        {
            // Perform dual-wield attacks
            if (LeftHandWeapon && RightHandWeapon)
            {
                // Hand-specific attacks
                if (direction == MouseDirections.Right || direction == MouseDirections.DownRight)
                {
                    RightHandWeapon.ShowWeapon = false;
                    LeftHandWeapon.OnAttackDirection(direction);
                    lastAttackHand = Hand.Left;
                    return;
                }
                else if (direction == MouseDirections.Left || direction == MouseDirections.DownLeft)
                {
                    LeftHandWeapon.ShowWeapon = false;
                    RightHandWeapon.OnAttackDirection(direction);
                    lastAttackHand = Hand.Right;
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
                    LeftHandWeapon.OnAttackDirection(direction);
                    lastAttackHand = Hand.Left;
                }
                else
                {
                    RightHandWeapon.OnAttackDirection(direction);
                    lastAttackHand = Hand.Right;
                }
            }
            else if (LeftHandWeapon && !RightHandWeapon)
            {
                // Just fire left hand
                LeftHandWeapon.OnAttackDirection(direction);
                lastAttackHand = Hand.Left;
            }
            else if (!LeftHandWeapon && RightHandWeapon)
            {
                // Just fire right hand
                RightHandWeapon.OnAttackDirection(direction);
                lastAttackHand = Hand.Right;
            }
            else
            {
                // No weapons set, no attacks possible
                lastAttackHand = Hand.None;
            }
        }

        private bool IsLeftHandAttacking()
        {
            return LeftHandWeapon && LeftHandWeapon.IsAttacking();
        }

        private bool IsRightHandAttacking()
        {
            return RightHandWeapon && RightHandWeapon.IsAttacking();
        }

        private void ShowWeapons(bool show)
        {
            if (LeftHandWeapon) LeftHandWeapon.ShowWeapon = show;
            if (RightHandWeapon) RightHandWeapon.ShowWeapon = show;
        }

        private void WeaponDamage(FPSWeapon weapon, out bool hitEnemy)
        {
            hitEnemy = false;

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

                // Check if hit an entity and remove health
                DaggerfallEntityBehaviour entityBehaviour = hit.transform.GetComponent<DaggerfallEntityBehaviour>();
                if (entityBehaviour)
                {
                    if (entityBehaviour.EntityType == EntityTypes.EnemyMonster || entityBehaviour.EntityType == EntityTypes.EnemyClass)
                    {
                        EnemyEntity enemyEntity = entityBehaviour.Entity as EnemyEntity;

                        // Calculate damage
                        int damage = FormulaHelper.CalculateWeaponDamage(playerEntity, enemyEntity, weapon);

                        // Play hit sound and trigger blood splash at hit point
                        if (damage > 0)
                        {
                            weapon.PlayHitSound();
                            EnemyBlood blood = hit.transform.GetComponent<EnemyBlood>();
                            if (blood)
                            {
                                blood.ShowBloodSplash(enemyEntity.MobileEnemy.BloodIndex, hit.point);
                            }
                        }
                        else
                        {
                            if (!enemyEntity.MobileEnemy.ParrySounds || weapon.WeaponType == WeaponTypes.Melee)
                                weapon.PlaySwingSound();
                            else
                                weapon.PlayParrySound();
                        }

                        // Remove health
                        enemyEntity.DecreaseHealth(damage);
                        hitEnemy = true;
                    }
                }

                // Check if hit a mobile NPC
                MobilePersonNPC mobileNpc = hit.transform.GetComponent<MobilePersonNPC>();
                if (mobileNpc)
                {
                    // TODO: Create blood splash.
                    weapon.PlayHitSound();
                    mobileNpc.Motor.gameObject.SetActive(false);
                    //GameManager.Instance.PlayerEntity.TallyCrimeGuildRequirements(false, 5);
                }
            }
        }

        private void ToggleSheath()
        {
            Sheathed = !Sheathed;
            if (!Sheathed)
            {
                // Play left-hand weapon equip sound
                if (LeftHandWeapon &&
                    LeftHandWeapon.WeaponType != WeaponTypes.Melee &&
                    LeftHandWeapon.WeaponType != WeaponTypes.None)
                {
                    LeftHandWeapon.PlayActivateSound();
                }

                // Play right-hand weapon equip sound
                if (RightHandWeapon &&
                    RightHandWeapon.WeaponType != WeaponTypes.Melee &&
                    RightHandWeapon.WeaponType != WeaponTypes.None)
                {
                    RightHandWeapon.PlayActivateSound();
                }
            }
        }

        #endregion
    }
}