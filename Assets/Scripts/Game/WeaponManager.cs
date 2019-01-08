// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors: Numidium   
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Support for mouse attack gestures, weapon state firing, and damage transfer.
    /// Should only be attached to player game object.
    /// </summary>
    public class WeaponManager : MonoBehaviour
    {
        const float defaultBowReach = 50f;
        const float defaultWeaponReach = 2.25f;

        // Max time-length of a trail of mouse positions for attack gestures
        private const float MaxGestureSeconds = 1.0f;

        public FPSWeapon ScreenWeapon;              // Weapon displayed in FPS view
        public bool Sheathed;                       // Weapon is sheathed
        public float SphereCastRadius = 0.3f;       // Radius of SphereCast used to target attacks
        [Range(0, 1)]
        public float AttackThreshold = 0.05f;       // Minimum mouse gesture travel distance for an attack. % of screen
        public float ChanceToBeParried = 0.1f;      // Example: Chance for player hit to be parried
        public DaggerfallMissile ArrowMissilePrefab;

        float weaponSensitivity = 1.0f;             // Sensitivity of weapon swings to mouse movements
        private Gesture _gesture;
        private int _longestDim;                    // Longest screen dimension, used to compare gestures for attack

        PlayerEntity playerEntity;
        GameObject player;
        GameObject mainCamera;
        bool isClickAttack = false;
        bool isAttacking = false;
        bool isDamageFinished = false;
        bool isBowSoundFinished = false;
        Hand lastAttackHand = Hand.None;
        float cooldownTime = 0.0f;                  // Wait for weapon cooldown
        int swingWeaponFatigueLoss = 11;            // According to DF Chronicles and verified in classic

        bool usingRightHand = true;
        bool holdingShield = false;
        DaggerfallUnityItem currentRightHandWeapon = null;
        DaggerfallUnityItem currentLeftHandWeapon = null;
        DaggerfallUnityItem lastBowUsed = null;

        public float EquipCountdownRightHand;
        public float EquipCountdownLeftHand;

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
            SetMelee(ScreenWeapon);
        }

        void Update()
        {
            // Automatically update weapons from inventory when PlayerEntity available
            if (playerEntity != null)
                UpdateHands();
            else
                playerEntity = GameManager.Instance.PlayerEntity;

            // Reset variables if there isn't an attack ongoing
            if (!IsWeaponAttacking())
            {
                // If an attack with a bow just finished, set cooldown
                if (ScreenWeapon.WeaponType == WeaponTypes.Bow && isAttacking)
                {
                    float cooldown = 10 * (100 - playerEntity.Stats.LiveSpeed) + 800;
                    cooldownTime = Time.time + (cooldown / 980); // Approximates classic frame update
                }

                isAttacking = false;
                isDamageFinished = false;
                isBowSoundFinished = false;
            }

            // Do nothing while weapon cooldown. Used for bow.
            if (Time.time < cooldownTime)
            {
                return;
            }

            // Do nothing if weapon isn't done equipping
            if ((usingRightHand && EquipCountdownRightHand != 0)
                || (!usingRightHand && EquipCountdownLeftHand != 0))
            {
                ShowWeapons(false);
                return;
            }

            // Hide weapons and do nothing if spell is ready or cast animation in progress
            if (GameManager.Instance.PlayerEffectManager)
            {
                if (GameManager.Instance.PlayerEffectManager.HasReadySpell || GameManager.Instance.PlayerSpellCasting.IsPlayingAnim)
                {
                    ShowWeapons(false);
                    return;
                }
            }

            // Do nothing if player paralyzed
            if (GameManager.Instance.PlayerEntity.IsParalyzed)
            {
                ShowWeapons(false);
                return;
            }

            // Toggle weapon sheath
            if (!isAttacking && InputManager.Instance.ActionStarted(InputManager.Actions.ReadyWeapon))
                ToggleSheath();

            // Toggle weapon hand
            if (!isAttacking && InputManager.Instance.ActionComplete(InputManager.Actions.SwitchHand))
                ToggleHand();

            // Do nothing if weapons sheathed
            if (Sheathed)
            {
                ShowWeapons(false);
                return;
            }
            else
                ShowWeapons(true);

            // Get if bow is equipped
            bool bowEquipped = (ScreenWeapon && ScreenWeapon.WeaponType == WeaponTypes.Bow);

            // Handle beginning a new attack
            if (!isAttacking)
            {
                if (!DaggerfallUnity.Settings.ClickToAttack || bowEquipped)
                {
                    // Reset tracking if user not holding down 'SwingWeapon' button and no attack in progress
                    if (!InputManager.Instance.HasAction(InputManager.Actions.SwingWeapon))
                    {
                        lastAttackHand = Hand.None;
                        _gesture.Clear();
                        return;
                    }
                }
                else
                {
                    // Player must click to attack
                    if (InputManager.Instance.ActionStarted(InputManager.Actions.SwingWeapon))
                    {
                        isClickAttack = true;
                    }
                    else
                    {
                        _gesture.Clear();
                        return;
                    }
                }
            }

            var attackDirection = MouseDirections.None;
            if (!isAttacking)
            {
                if (bowEquipped)
                {
                    // Ensure attack button was released before starting the next attack
                    if (lastAttackHand == Hand.None)
                        attackDirection = MouseDirections.Down; // Force attack without tracking a swing for Bow
                }
                else if (isClickAttack)
                {
                    attackDirection = (MouseDirections)UnityEngine.Random.Range((int)MouseDirections.Left, (int)MouseDirections.DownRight + 1);
                    isClickAttack = false;
                }
                else
                {
                    attackDirection = TrackMouseAttack(); // Track swing direction for other weapons
                }
            }

            // Start attack if one has been initiated
            if (attackDirection != MouseDirections.None)
            {
                ExecuteAttacks(attackDirection);
                isAttacking = true;
            }

            // Stop here if no attack is happening
            if (!isAttacking)
                return;

            if (!isBowSoundFinished && ScreenWeapon.WeaponType == WeaponTypes.Bow && ScreenWeapon.GetCurrentFrame() == 3)
            {
                ScreenWeapon.PlaySwingSound();
                isBowSoundFinished = true;

                // Remove arrow
                ItemCollection playerItems = playerEntity.Items;
                DaggerfallUnityItem arrow = playerItems.GetItem(ItemGroups.Weapons, (int)Weapons.Arrow);
                playerItems.RemoveOne(arrow);
            }
            else if (!isDamageFinished && ScreenWeapon.GetCurrentFrame() == ScreenWeapon.GetHitFrame())
            {
                // Chance to play attack voice
                if (DaggerfallUnity.Settings.CombatVoices && ScreenWeapon.WeaponType != WeaponTypes.Bow && UnityEngine.Random.Range(1, 101) <= 20)
                    ScreenWeapon.PlayAttackVoice();

                // Transfer damage.
                bool hitEnemy = false;

                // Non-bow weapons
                if (ScreenWeapon.WeaponType != WeaponTypes.Bow)
                    MeleeDamage(ScreenWeapon, out hitEnemy);
                // Bow weapons
                else
                {
                    DaggerfallMissile missile = Instantiate(ArrowMissilePrefab);
                    if (missile)
                    {
                        missile.Caster = GameManager.Instance.PlayerEntityBehaviour;
                        missile.TargetType = TargetTypes.SingleTargetAtRange;
                        missile.ElementType = ElementTypes.None;
                        missile.IsArrow = true;

                        lastBowUsed = currentRightHandWeapon;
                    }
                }

                // Fatigue loss
                playerEntity.DecreaseFatigue(swingWeaponFatigueLoss);

                // Play swing sound if attack didn't hit an enemy.
                if (!hitEnemy && ScreenWeapon.WeaponType != WeaponTypes.Bow)
                    ScreenWeapon.PlaySwingSound();
                else
                {
                    // Tally skills
                    if (ScreenWeapon.WeaponType == WeaponTypes.Melee || ScreenWeapon.WeaponType == WeaponTypes.Werecreature)
                        playerEntity.TallySkill(DFCareer.Skills.HandToHand, 1);
                    else if (usingRightHand && (currentRightHandWeapon != null))
                        playerEntity.TallySkill(currentRightHandWeapon.GetWeaponSkillID(), 1);
                    else if (currentLeftHandWeapon != null)
                        playerEntity.TallySkill(currentLeftHandWeapon.GetWeaponSkillID(), 1);

                    playerEntity.TallySkill(DFCareer.Skills.CriticalStrike, 1);
                }
                isDamageFinished = true;
            }
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

        // Returns true if hit an enemy entity
        public bool WeaponDamage(RaycastHit hit, Vector3 direction, bool arrowHit = false)
        {
            DaggerfallUnityItem strikingWeapon = null;

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
                actionDoor.AttemptBash(true);
                return false;
            }

            // Check if player hit a static exterior door
            if (GameManager.Instance.PlayerActivate.AttemptExteriorDoorBash(hit))
            {
                return false;
            }

            // Set up for use below
            DaggerfallEntityBehaviour entityBehaviour = hit.transform.GetComponent<DaggerfallEntityBehaviour>();
            DaggerfallMobileUnit entityMobileUnit = hit.transform.GetComponentInChildren<DaggerfallMobileUnit>();
            EnemyMotor enemyMotor = hit.transform.GetComponent<EnemyMotor>();
            EnemySounds enemySounds = hit.transform.GetComponent<EnemySounds>();

            // Check if hit a mobile NPC
            MobilePersonNPC mobileNpc = hit.transform.GetComponent<MobilePersonNPC>();
            if (mobileNpc)
            {
                if (!mobileNpc.Billboard.IsUsingGuardTexture)
                {
                    EnemyBlood blood = hit.transform.GetComponent<EnemyBlood>();
                    if (blood)
                    {
                        blood.ShowBloodSplash(0, hit.point);
                    }
                    mobileNpc.Motor.gameObject.SetActive(false);
                    playerEntity.TallyCrimeGuildRequirements(false, 5);
                    playerEntity.CrimeCommitted = PlayerEntity.Crimes.Murder;
                    playerEntity.SpawnCityGuards(true);
                }
                else
                {
                    playerEntity.CrimeCommitted = PlayerEntity.Crimes.Assault;
                    GameObject guard = playerEntity.SpawnCityGuard(mobileNpc.transform.position, mobileNpc.transform.forward);
                    entityBehaviour = guard.GetComponent<DaggerfallEntityBehaviour>();
                    entityMobileUnit = guard.GetComponentInChildren<DaggerfallMobileUnit>();
                    enemyMotor = guard.GetComponent<EnemyMotor>();
                    enemySounds = guard.GetComponent<EnemySounds>();
                }
                mobileNpc.Motor.gameObject.SetActive(false);
            }

            // Check if hit an entity and remove health
            if (entityBehaviour)
            {
                if (entityBehaviour.EntityType == EntityTypes.EnemyMonster || entityBehaviour.EntityType == EntityTypes.EnemyClass)
                {
                    EnemyEntity enemyEntity = entityBehaviour.Entity as EnemyEntity;

                    // Calculate damage
                    int damage;
                    if (!arrowHit)
                    {
                        if (usingRightHand)
                        {
                            strikingWeapon = currentRightHandWeapon;
                        }
                        else
                        {
                            strikingWeapon = currentLeftHandWeapon;
                        }
                    }
                    else
                    {
                        strikingWeapon = lastBowUsed;
                    }

                    damage = FormulaHelper.CalculateAttackDamage(playerEntity, enemyEntity, strikingWeapon, entityMobileUnit.Summary.AnimStateRecord);

                    // Break any "normal power" concealment effects on player
                    if (playerEntity.IsMagicallyConcealedNormalPower && damage > 0)
                        EntityEffectManager.BreakNormalPowerConcealmentEffects(GameManager.Instance.PlayerEntityBehaviour);

                    // Play arrow sound and add arrow to target's inventory
                    if (arrowHit)
                    {
                        DaggerfallUnityItem arrow = ItemBuilder.CreateItem(ItemGroups.Weapons, (int)Weapons.Arrow);
                        enemyEntity.Items.AddItem(arrow);
                    }

                    // Play hit sound and trigger blood splash at hit point
                    if (damage > 0)
                    {
                        if (usingRightHand)
                            enemySounds.PlayHitSound(currentRightHandWeapon);
                        else
                            enemySounds.PlayHitSound(currentLeftHandWeapon);

                        EnemyBlood blood = hit.transform.GetComponent<EnemyBlood>();
                        if (blood)
                        {
                            blood.ShowBloodSplash(enemyEntity.MobileEnemy.BloodIndex, hit.point);
                        }

                        // Knock back enemy based on damage and enemy weight
                        if (enemyMotor)
                        {
                            if (enemyMotor.KnockBackSpeed <= (5 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)) &&
                                entityBehaviour.EntityType == EntityTypes.EnemyClass ||
                                enemyEntity.MobileEnemy.Weight > 0)
                            {
                                float enemyWeight = enemyEntity.GetWeightInClassicUnits();
                                float tenTimesDamage = damage * 10;
                                float twoTimesDamage = damage * 2;

                                float knockBackAmount = ((tenTimesDamage - enemyWeight) * 256) / (enemyWeight + tenTimesDamage) * twoTimesDamage;
                                float knockBackSpeed = (tenTimesDamage / enemyWeight) * (twoTimesDamage - (knockBackAmount / 256));
                                knockBackSpeed /= (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10);

                                if (knockBackSpeed < (15 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)))
                                    knockBackSpeed = (15 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10));
                                enemyMotor.KnockBackSpeed = knockBackSpeed;
                                enemyMotor.KnockBackDirection = direction;
                            }
                        }

                        if (DaggerfallUnity.Settings.CombatVoices && entityBehaviour.EntityType == EntityTypes.EnemyClass && UnityEngine.Random.Range(1, 101) <= 40)
                        {
                            Genders gender;
                            if (entityMobileUnit.Summary.Enemy.Gender == MobileGender.Male || enemyEntity.MobileEnemy.ID == (int)MobileTypes.Knight_CityWatch)
                                gender = Genders.Male;
                            else
                                gender = Genders.Female;

                            bool heavyDamage = damage >= enemyEntity.MaxHealth / 4;
                            enemySounds.PlayCombatVoice(gender, false, heavyDamage);
                        }
                    }
                    else
                    {
                        if ((!arrowHit && !enemyEntity.MobileEnemy.ParrySounds) || strikingWeapon == null)
                            ScreenWeapon.PlaySwingSound();
                        else if (enemyEntity.MobileEnemy.ParrySounds)
                            enemySounds.PlayParrySound();
                    }

                    // Remove health
                    enemyEntity.DecreaseHealth(damage);

                    // Assign "cast when strikes" to target entity
                    if (strikingWeapon != null && strikingWeapon.IsEnchanted)
                    {
                        EntityEffectManager enemyEffectManager = enemyEntity.EntityBehaviour.GetComponent<EntityEffectManager>();
                        if (enemyEffectManager)
                            enemyEffectManager.StrikeWithItem(strikingWeapon, GameManager.Instance.PlayerEntityBehaviour);
                    }

                    // Make foe attack their aggressor
                    // Currently this is just player, but should be expanded later
                    // for a wider variety of behaviours
                    if (enemyMotor)
                    {
                        // Make enemies in an area aggressive if player attacked a non-hostile one.
                        if (!enemyMotor.IsHostile)
                        {
                            GameManager.Instance.MakeEnemiesHostile();
                        }
                        enemyMotor.MakeEnemyHostileToAttacker(GameManager.Instance.PlayerEntityBehaviour);
                    }

                    if (enemyEntity.MobileEnemy.ID == (int)MobileTypes.Knight_CityWatch && enemyEntity.CurrentHealth <= 0)
                    {
                        playerEntity.TallyCrimeGuildRequirements(false, 1);
                        playerEntity.CrimeCommitted = PlayerEntity.Crimes.Murder;
                    }

                    // Allow custom race handling of weapon hit, e.g. vampire feeding
                    RacialOverrideEffect racialOverride = GameManager.Instance.PlayerEffectManager.GetRacialOverrideEffect();
                    if (racialOverride != null)
                        racialOverride.OnWeaponHitEnemy(GameManager.Instance.PlayerEntity, enemyEntity);

                    return true;
                }
            }

            return false;
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
                currentRightHandWeapon = rightHandItem;

            // Left-hand item changed
            if (!DaggerfallUnityItem.CompareItems(currentLeftHandWeapon, leftHandItem))
                currentLeftHandWeapon = leftHandItem;

            if (EquipCountdownRightHand > 0)
            {
                EquipCountdownRightHand -= Time.deltaTime * 980; // Approximating classic update time based off measuring video
                if (EquipCountdownRightHand <= 0)
                {
                    EquipCountdownRightHand = 0;
                    string message = HardStrings.rightHandEquipped;
                    DaggerfallUI.Instance.PopupMessage(message);
                }
            }
            if (EquipCountdownLeftHand > 0)
            {
                EquipCountdownLeftHand -= Time.deltaTime * 980; // Approximating classic update time based off measuring video
                if (EquipCountdownLeftHand <= 0)
                {
                    EquipCountdownLeftHand = 0;
                    string message = HardStrings.leftHandEquipped;
                    DaggerfallUI.Instance.PopupMessage(message);
                }
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
            if (!ScreenWeapon)
                return;

            if (usingRightHand)
            {
                if (currentRightHandWeapon == null)
                    SetMelee(ScreenWeapon);
                else
                    SetWeapon(ScreenWeapon, currentRightHandWeapon);
            }
            else
            {
                if (currentLeftHandWeapon == null)
                    SetMelee(ScreenWeapon);
                else
                    SetWeapon(ScreenWeapon, currentLeftHandWeapon);
            }

            ScreenWeapon.Reach = defaultWeaponReach;
        }

        void SetMelee(FPSWeapon target)
        {
            target.WeaponType = WeaponTypes.Melee;
            target.MetalType = MetalTypes.None;
            target.DrawWeaponSound = SoundClips.None;
            target.SwingWeaponSound = SoundClips.SwingHighPitch;
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
            if(ScreenWeapon)
            {
                // Fire screen weapon animation
                ScreenWeapon.OnAttackDirection(direction);
                lastAttackHand = Hand.Right;
            }
            else
            {
                // No weapon set, no attacks possible
                lastAttackHand = Hand.None;
            }
        }

        private bool IsWeaponAttacking()
        {
            return ScreenWeapon && ScreenWeapon.IsAttacking();
        }

        private void ShowWeapons(bool show)
        {
            if (ScreenWeapon)
                ScreenWeapon.ShowWeapon = show;
        }

        private void MeleeDamage(FPSWeapon weapon, out bool hitEnemy)
        {
            hitEnemy = false;

            if (!mainCamera || !weapon)
                return;

            // Fire ray along player facing using weapon range
            // Origin point of ray is set back slightly to fix issue where strikes against enemy capsules touching player capsule do not connect
            RaycastHit hit;
            Ray ray = new Ray(mainCamera.transform.position + -mainCamera.transform.forward * 0.1f, mainCamera.transform.forward);
            if (Physics.SphereCast(ray, SphereCastRadius, out hit, weapon.Reach - SphereCastRadius))
            {
                hitEnemy = WeaponDamage(hit, mainCamera.transform.forward);
            }
        }

        private void ToggleSheath()
        {
            Sheathed = !Sheathed;
            if (!Sheathed)
            {
                // Play right-hand weapon equip sound
                if (ScreenWeapon &&
                    ScreenWeapon.WeaponType != WeaponTypes.Melee &&
                    ScreenWeapon.WeaponType != WeaponTypes.None)
                {
                    ScreenWeapon.PlayActivateSound();
                }
            }
        }

        #endregion
    }
}