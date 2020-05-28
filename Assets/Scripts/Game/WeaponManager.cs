// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Numidium, Hazelnut
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
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Support for mouse attack gestures, weapon state firing, and damage transfer.
    /// Should only be attached to player game object.
    /// </summary>
    public class WeaponManager : MonoBehaviour
    {
        const float defaultBowReach = 50f;
        public const float defaultWeaponReach = 2.25f;

        // Equip delay times for weapons
        public static ushort[] EquipDelayTimes = { 500, 700, 1200, 900, 900, 1800, 1600, 1700, 1700, 3000, 3400, 2000, 2200, 2000, 2200, 2000, 4000, 5000 };

        // Max time-length of a trail of mouse positions for attack gestures
        private const float MaxGestureSeconds = 1.0f;

        // Max time bow can be held drawn and switch divisor
        private const int MaxBowHeldDrawnSeconds = 10;
        private const float BowSwitchDivisor = 1.7f;

        private const float resetJoystickSwingRadius = 0.4f;

        public FPSWeapon ScreenWeapon;              // Weapon displayed in FPS view
        public bool Sheathed;                       // Weapon is sheathed
        public float SphereCastRadius = 0.25f;      // Radius of SphereCast used to target attacks
        int playerLayerMask = 0;
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
        bool joystickSwungOnce = false;
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

        public DaggerfallUnityItem LastBowUsed { get { return lastBowUsed; } }
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
            playerLayerMask = ~(1 << LayerMask.NameToLayer("Player"));
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
                    cooldownTime = Time.time + FormulaHelper.GetBowCooldownTime(playerEntity);

                isAttacking = false;
                isDamageFinished = false;
                isBowSoundFinished = false;
            }

            // Do nothing while weapon cooldown. Used for bow.
            if (Time.time < cooldownTime)
            {
                return;
            }

            // Do nothing if player paralyzed or is climbing
            if (GameManager.Instance.PlayerEntity.IsParalyzed || GameManager.Instance.ClimbingMotor.IsClimbing)
            {
                ShowWeapons(false);
                return;
            }

            bool doToggleSheath = false;

            // Hide weapons and do nothing if spell is ready or cast animation in progress
            if (GameManager.Instance.PlayerEffectManager)
            {
                if (GameManager.Instance.PlayerEffectManager.HasReadySpell || GameManager.Instance.PlayerSpellCasting.IsPlayingAnim)
                {
                    if (!isAttacking && InputManager.Instance.ActionStarted(InputManager.Actions.ReadyWeapon))
                    {
                        GameManager.Instance.PlayerEffectManager.AbortReadySpell();

                        //if currently unsheathed, then sheath it, so we can give the effect of unsheathing it again
                        if (!Sheathed)
                            ToggleSheath();

                        doToggleSheath = true;
                    }
                    else
                    {
                        ShowWeapons(false);
                        return;
                    }
                }
            }

            // Toggle weapon sheath
            if (doToggleSheath || (!isAttacking && InputManager.Instance.ActionStarted(InputManager.Actions.ReadyWeapon)))
                ToggleSheath();

            // Toggle weapon hand
            if (!isAttacking && InputManager.Instance.ActionComplete(InputManager.Actions.SwitchHand))
                ToggleHand();

            // Do nothing if weapon isn't done equipping
            if ((usingRightHand && EquipCountdownRightHand != 0)
                || (!usingRightHand && EquipCountdownLeftHand != 0))
            {
                ShowWeapons(false);
                return;
            }

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
                        attackDirection = DaggerfallUnity.Settings.BowDrawback ? MouseDirections.Up : MouseDirections.Down; // Force attack without tracking a swing for Bow
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
            if (isAttacking && bowEquipped && DaggerfallUnity.Settings.BowDrawback && ScreenWeapon.GetCurrentFrame() == 3)
            {
                if (InputManager.Instance.HasAction(InputManager.Actions.ActivateCenterObject) || ScreenWeapon.GetAnimTime() > MaxBowHeldDrawnSeconds)
                {   // Un-draw the bow without releasing an arrow.
                    ScreenWeapon.ChangeWeaponState(WeaponStates.Idle);
                }
                else if (!InputManager.Instance.HasAction(InputManager.Actions.SwingWeapon))
                {   // Release arrow. Debug.Log("Release arrow!");
                    attackDirection = MouseDirections.Down;
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

            if (!isBowSoundFinished && ScreenWeapon.WeaponType == WeaponTypes.Bow && ScreenWeapon.GetCurrentFrame() == 4)
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
                // Racial override can suppress optional attack voice
                RacialOverrideEffect racialOverride = GameManager.Instance.PlayerEffectManager.GetRacialOverrideEffect();
                bool suppressCombatVoices = racialOverride != null && racialOverride.SuppressOptionalCombatVoices;

                // Chance to play attack voice
                if (DaggerfallUnity.Settings.CombatVoices && !suppressCombatVoices && ScreenWeapon.WeaponType != WeaponTypes.Bow && Dice100.SuccessRoll(20))
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

                        lastBowUsed = usingRightHand ? currentRightHandWeapon : currentLeftHandWeapon;;
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

        // Returns true if hit the environment
        public bool WeaponEnvDamage(DaggerfallUnityItem strikingWeapon, RaycastHit hit)
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
                actionDoor.AttemptBash(true);
                return true;
            }

            // Check if player hit a static exterior door
            if (GameManager.Instance.PlayerActivate.AttemptExteriorDoorBash(hit))
            {
                return true;
            }

            // Make hitting walls do a thud or clinging sound (not in classic)
            if (GameObjectHelper.IsStaticGeometry(hit.transform.gameObject))
            {
                DaggerfallUI.Instance.PlayOneShot(strikingWeapon == null ? SoundClips.Hit2 : SoundClips.Parry6);
                return true;
            }

            return false;
        }

        // Returns true if hit an enemy entity
        public bool WeaponDamage(DaggerfallUnityItem strikingWeapon, bool arrowHit, Transform hitTransform, Vector3 impactPosition, Vector3 direction)
        {
            DaggerfallEntityBehaviour entityBehaviour = hitTransform.GetComponent<DaggerfallEntityBehaviour>();
            DaggerfallMobileUnit entityMobileUnit = hitTransform.GetComponentInChildren<DaggerfallMobileUnit>();
            EnemyMotor enemyMotor = hitTransform.GetComponent<EnemyMotor>();
            EnemySounds enemySounds = hitTransform.GetComponent<EnemySounds>();

            // Check if hit a mobile NPC
            MobilePersonNPC mobileNpc = hitTransform.GetComponent<MobilePersonNPC>();
            if (mobileNpc)
            {
                if (!mobileNpc.IsGuard)
                {
                    EnemyBlood blood = hitTransform.GetComponent<EnemyBlood>();
                    if (blood)
                    {
                        blood.ShowBloodSplash(0, impactPosition);
                    }
                    mobileNpc.Motor.gameObject.SetActive(false);
                    playerEntity.TallyCrimeGuildRequirements(false, 5);
                    playerEntity.CrimeCommitted = PlayerEntity.Crimes.Murder;
                    playerEntity.SpawnCityGuards(true);

                    // Allow custom race handling of weapon hit against mobile NPCs, e.g. vampire feeding or lycanthrope killing
                    if (entityBehaviour)
                    {
                        entityBehaviour.Entity.SetHealth(0);
                        RacialOverrideEffect racialOverride = GameManager.Instance.PlayerEffectManager.GetRacialOverrideEffect();
                        if (racialOverride != null)
                            racialOverride.OnWeaponHitEntity(GameManager.Instance.PlayerEntity, entityBehaviour.Entity);
                    }
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
                    int animTime = (int)(ScreenWeapon.GetAnimTime() * 1000);    // Get animation time, converted to ms.
                    int damage = FormulaHelper.CalculateAttackDamage(playerEntity, enemyEntity, entityMobileUnit.Summary.AnimStateRecord, animTime, strikingWeapon);

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

                        EnemyBlood blood = hitTransform.GetComponent<EnemyBlood>();
                        if (blood)
                        {
                            blood.ShowBloodSplash(enemyEntity.MobileEnemy.BloodIndex, impactPosition);
                        }

                        // Knock back enemy based on damage and enemy weight
                        if (enemyMotor)
                        {
                            if (enemyMotor.KnockbackSpeed <= (5 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)) &&
                                entityBehaviour.EntityType == EntityTypes.EnemyClass ||
                                enemyEntity.MobileEnemy.Weight > 0)
                            {
                                float enemyWeight = enemyEntity.GetWeightInClassicUnits();
                                float tenTimesDamage = damage * 10;
                                float twoTimesDamage = damage * 2;

                                float knockBackAmount = ((tenTimesDamage - enemyWeight) * 256) / (enemyWeight + tenTimesDamage) * twoTimesDamage;
                                float KnockbackSpeed = (tenTimesDamage / enemyWeight) * (twoTimesDamage - (knockBackAmount / 256));
                                KnockbackSpeed /= (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10);

                                if (KnockbackSpeed < (15 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)))
                                    KnockbackSpeed = (15 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10));
                                enemyMotor.KnockbackSpeed = KnockbackSpeed;
                                enemyMotor.KnockbackDirection = direction;
                            }
                        }

                        if (DaggerfallUnity.Settings.CombatVoices && entityBehaviour.EntityType == EntityTypes.EnemyClass && Dice100.SuccessRoll(40))
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

                    // Handle weapon striking enchantments - this could change damage amount
                    if (strikingWeapon != null && strikingWeapon.IsEnchanted)
                    {
                        EntityEffectManager effectManager = GetComponent<EntityEffectManager>();
                        if (effectManager)
                            damage = effectManager.DoItemEnchantmentPayloads(EnchantmentPayloadFlags.Strikes, strikingWeapon, GameManager.Instance.PlayerEntity.Items, enemyEntity.EntityBehaviour, damage);
                        strikingWeapon.RaiseOnWeaponStrikeEvent(entityBehaviour, damage);
                    }

                    // Remove health
                    enemyEntity.DecreaseHealth(damage);

                    // Handle attack from player
                    enemyEntity.EntityBehaviour.HandleAttackFromSource(GameManager.Instance.PlayerEntityBehaviour);

                    // Allow custom race handling of weapon hit against enemies, e.g. vampire feeding or lycanthrope killing
                    RacialOverrideEffect racialOverride = GameManager.Instance.PlayerEffectManager.GetRacialOverrideEffect();
                    if (racialOverride != null)
                        racialOverride.OnWeaponHitEntity(GameManager.Instance.PlayerEntity, entityBehaviour.Entity);

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

            if (DaggerfallUnity.Settings.BowLeftHandWithSwitching)
            {
                int switchDelay = 0;
                if (currentRightHandWeapon != null)
                    switchDelay += EquipDelayTimes[currentRightHandWeapon.GroupIndex] - 500;
                if (currentLeftHandWeapon != null)
                    switchDelay += EquipDelayTimes[currentLeftHandWeapon.GroupIndex] - 500;
                if (switchDelay > 0)
                {
                    if (UsingRightHand)
                        EquipCountdownRightHand += switchDelay / BowSwitchDivisor;
                    else
                        EquipCountdownLeftHand += switchDelay / BowSwitchDivisor;
                }
            }

            ApplyWeapon();
        }

        void ApplyWeapon()
        {
            if (!ScreenWeapon)
                return;

            RacialOverrideEffect racialOverride = GameManager.Instance.PlayerEffectManager.GetRacialOverrideEffect();
            if (racialOverride != null && racialOverride.SetFPSWeapon(ScreenWeapon))
            {
                return;
            }
            else if (usingRightHand)
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
            target.WeaponHands = ItemEquipTable.GetItemHands(weapon);
            target.DrawWeaponSound = weapon.GetEquipSound();
            target.SwingWeaponSound = weapon.GetSwingSound();
        }

        #endregion

        #region Private Methods

        MouseDirections TrackMouseAttack()
        {
            // Track action for idle plus all eight mouse directions
            var sum = _gesture.Add(InputManager.Instance.MouseX, InputManager.Instance.MouseY) * weaponSensitivity;

            if (InputManager.Instance.UsingController)
            {
                float x = InputManager.Instance.MouseX;
                float y = InputManager.Instance.MouseY;

                bool inResetJoystickSwingRadius = (x >= -resetJoystickSwingRadius && x <= resetJoystickSwingRadius && y >= -resetJoystickSwingRadius && y <= resetJoystickSwingRadius);

                if (joystickSwungOnce || inResetJoystickSwingRadius)
                {
                    if (inResetJoystickSwingRadius)
                        joystickSwungOnce = false;

                    return MouseDirections.None;
                }
            }
            else if (_gesture.TravelDist/_longestDim < AttackThreshold)
            {
                return MouseDirections.None;
            }

            joystickSwungOnce = true;

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
            if (ScreenWeapon)
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
            RaycastHit hit;
            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
            if (Physics.SphereCast(ray, SphereCastRadius, out hit, weapon.Reach, playerLayerMask))
            {
                DaggerfallUnityItem strikingWeapon = usingRightHand ? currentRightHandWeapon : currentLeftHandWeapon;
                if(!WeaponEnvDamage(strikingWeapon, hit)
                   // Fall back to simple ray for narrow cages https://forums.dfworkshop.net/viewtopic.php?f=5&t=2195#p39524
                   || Physics.Raycast(ray, out hit, weapon.Reach, playerLayerMask))
                {
                    hitEnemy = WeaponDamage(strikingWeapon, false, hit.transform, hit.point, mainCamera.transform.forward);
                }
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
