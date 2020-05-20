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

        public bool hitobject = false;
        float changeitemrange;

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
        public float SphereCastRadius = 0.05f;      // Radius of SphereCast used to target attacks
        int playerLayerMask = 0;
        [Range(0, 1)]
        public float AttackThreshold = 0.05f;       // Minimum mouse gesture travel distance for an attack. % of screen
        public float ChanceToBeParried = 0.1f;      // Example: Chance for player hit to be parried
        public DaggerfallMissile ArrowMissilePrefab;
        public DaggerfallUnityItem strikingWeapon;

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
        int swingWeaponFatigueLoss = 11;            // According to DF Chronicles and verified in classic. Is dynamically set below based on on screen weapon.
        float itemRange = 0;

        bool usingRightHand = true;
        bool holdingShield = false;
        DaggerfallUnityItem currentRightHandWeapon = null;
        DaggerfallUnityItem currentLeftHandWeapon = null;
        DaggerfallUnityItem lastBowUsed = null;

        public float EquipCountdownRightHand;
        public float EquipCountdownLeftHand;
        public Vector3 attackPosition;

        //COMBAT OVERHAUL\\
        //Sets Vars and Objects for combat overhaul rayarc
        //and FPSWeapon script animation system.
        private PlayerSpeedChanger playerspeed;
        FPSConsoleCommands fpsconsole;
        float startpos;
        float endpos;
        int CurrentFrame = 0;
        float timer = 0f;
        float animetime = 0f;
        Vector3 attackcast;

        public bool IsAttacking
        {
            get { return isAttacking; }
            set { isAttacking = value; }
        }

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
            playerspeed = GetComponent<PlayerSpeedChanger>();
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
                hitobject = false;
                isDamageFinished = false;
                isBowSoundFinished = false;
            }

            // Do nothing while weapon cooldown. Used for bow.
            if (Time.time < cooldownTime)
            {
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

            // Do nothing if player paralyzed or is climbing
            if (GameManager.Instance.PlayerEntity.IsParalyzed || GameManager.Instance.ClimbingMotor.IsClimbing)
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
                //COMBAT OVERHAUL\\
                //Start of movement based attack system. Checks players movement input and picks corresponding mouse attack direction.
                //if no attack is selected, it randomly selects from the mouse attacks. attacks are limited to the down,left,right,up directions
                //in the FPSWeapon scrip; there is no smooth animations yet for down-right and down-left. Preserves original bow triggers.

                if (bowEquipped)
                {
                    // Ensure attack button was released before starting the next attack
                    if (lastAttackHand == Hand.None)
                        attackDirection = DaggerfallUnity.Settings.BowDrawback ? MouseDirections.Up : MouseDirections.Down; // Force attack without tracking a swing for Bow
                }
                else if (isClickAttack && InputManager.Instance.HasAction(InputManager.Actions.MoveLeft))
                {
                    attackDirection = MouseDirections.Left;
                    isClickAttack = false;
                }
                else if (isClickAttack && InputManager.Instance.HasAction(InputManager.Actions.MoveRight))
                {
                    attackDirection = MouseDirections.Right;
                    isClickAttack = false;
                }
                else if (isClickAttack && InputManager.Instance.HasAction(InputManager.Actions.MoveForwards))
                {
                    attackDirection = MouseDirections.Up;
                    isClickAttack = false;
                }
                else if (isClickAttack && InputManager.Instance.HasAction(InputManager.Actions.MoveBackwards))
                {
                    attackDirection = MouseDirections.Down;
                    isClickAttack = false;
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
                //gets current attacking weapon, checks if its fists, and then assigns weapon stats for the attack.
                strikingWeapon = usingRightHand ? currentRightHandWeapon : currentLeftHandWeapon;

                //*COMBAT OVERHAUL ADDITION*//
                //checks screen weapon, and if it isn't fists/melee, assigns custom, dynamic range, fatigue, and attack speed cost.
                //else default to classic engine range and fatigue cost for fists, and any other bug issues non-default numbers can cause.
                if (ScreenWeapon.WeaponType != WeaponTypes.Melee)
                {
                    float itemWeight = ItemHelper.getItemWeight(strikingWeapon);
                    float itemRange = ItemHelper.getItemRange(strikingWeapon);

                    //pulls weapons range from template using itemhelper script as passthrough
                    ScreenWeapon.Reach = ItemHelper.getItemRange(strikingWeapon);
                    ScreenWeapon.AttackSpeed = ItemHelper.getItemSpeed(strikingWeapon);
                    swingWeaponFatigueLoss = (((int)itemWeight + (int)itemRange) * 2);
                }
                else
                {
                    //sets default classic DF values for range and fatigue cost.
                    ScreenWeapon.Reach = 2.25f;
                    swingWeaponFatigueLoss = 11;
                    ScreenWeapon.AttackSpeed = 0;
                }

                //prints out weapon range and stamina fatigue on swing.
                //DaggerfallUI.Instance.PopupMessage("Weapon Range: " + ScreenWeapon.Reach.ToString() + "f");
                //DaggerfallUI.Instance.PopupMessage("Weapon Fatigue: " + swingWeaponFatigueLoss.ToString());
                //DaggerfallUI.Instance.PopupMessage("Weapon Speed: " + (ScreenWeapon.AttackSpeed * 3).ToString());

                //*COMBAT OVERHAUL ADDITION*//
                //resets values for hit arc code below.
                hitobject = false;
                itemRange = 0;
                itemRange = ItemHelper.getItemRange(strikingWeapon);
                timer = 0;
                animetime = ScreenWeapon.animTickTime;

                //executes screen animation code.
                ExecuteAttacks(attackDirection);

                isAttacking = true;
            }

            // Stop here if no attack is happening
            if (!isAttacking)
            {
                return;
            }
            else
            {
                float startframe;
                float lerptimer;
                PlayerEntity player = GameManager.Instance.PlayerEntity;

                if (!mainCamera || !ScreenWeapon)
                    return;

                //*COMBAT OVERHAUL ADDITION*//
                //start of raycast arc code. This runs the hit detection arc for weapons and communicates with
                //FPSWeapon script to ensure animations and hit detections line up with one another.
                //wait timer. gets the difference between the original timer start and time passed since then and assigns it to timer.
                //used to ensure raycast is only done once a frame.
                timer += Time.deltaTime;

                //insures calculation only happens every frame.
                if (timer > 1f)
                {
                    timer = 1f;
                }

                //checks of console value has been set and defaults it if not to 5th of a second.
                if ((lerptimer = FPSConsoleCommands.ChangeRaycastLerp.changeRaycastLerp) == 0)
                    lerptimer = .2f;

                //calculates the float timer for the below lerp. it divides the animation time by a 5th of a second to get
                //how much time has passed in the current frame (which there are 5 of) and move  below lerp.
                float perc = timer / (animetime / lerptimer);

                //grabs current attack frame from fpsweapon script.
                //This is used to set the frames raycasts will shoot out through.
                CurrentFrame = ScreenWeapon.GetCurrentFrame();

                //sets starting frame for raycasting. ensures all weapons align hit rays properly.
                if (ScreenWeapon.WeaponType != WeaponTypes.Flail)
                    startframe = 0;
                else
                    startframe = 25;

                //starts raycasts loop to send out raycasts based on attack direction. Aligns with the attack direction
                //then starts to offset the raycast one increment at a time to follow animation. When hits object, marks
                // hitobject as true and ends raycasts. Also, tells fpsweapon script to start collision animation.
                //calculates only once every frame.
                if ((CurrentFrame > 0 && CurrentFrame <= 5) && ScreenWeapon.WeaponType != WeaponTypes.Bow && !hitobject)
                {
                    //if then loop to select the specific raycast offset transformation based on the choosen attack animation.
                    if (ScreenWeapon.WeaponState == WeaponStates.StrikeUp)
                    {
                        if (ScreenWeapon.WeaponType == WeaponTypes.Melee)
                        {
                            if ((startpos = FPSConsoleCommands.ChangeHorPos.SchangeHorPos) == 0)
                                startpos = 25;

                            if ((endpos = FPSConsoleCommands.ChangeHorPos.EchangeHorPos) == 0)
                                endpos = 0;

                            //sets up starting and ending quaternion angles for the vector3 offset/raycast.
                            Quaternion startq = Quaternion.AngleAxis(startpos, transform.right);
                            Quaternion endq = Quaternion.AngleAxis(endpos, transform.right);
                            //computes rotation for each raycast using a lerp. The time percentage is modified above using the animation time.
                            Quaternion slerpq = Quaternion.Slerp(startq, endq, perc);
                            //computes rotation for each raycast. First angle sets permanent left/right offset. Second runs ray down cast for each tick.
                            attackcast = Quaternion.AngleAxis(5, transform.up) * (slerpq * (mainCamera.transform.forward * (itemRange - SphereCastRadius)));
                            FireRayArc();

                        }
                        else if (ScreenWeapon.GetCurrentFrame() > 2)
                        {
                            //sets start range at negative the itemrange to deal with raycasting starting on the third frame in.
                            changeitemrange = Mathf.Lerp(0, itemRange, perc);

                            //computes rotation for each raycast.
                            attackcast = (mainCamera.transform.forward * (changeitemrange - SphereCastRadius));
                            FireRayArc();
                        }
                    }
                    else if (ScreenWeapon.WeaponState == WeaponStates.StrikeDown)
                    {
                        if (ScreenWeapon.WeaponType == WeaponTypes.Melee && ScreenWeapon.GetCurrentFrame() > 2)
                        {
                            if ((startpos = FPSConsoleCommands.ChangeHorPos.SchangeHorPos) == 0)
                                startpos = 25;

                            if ((endpos = FPSConsoleCommands.ChangeHorPos.EchangeHorPos) == 0)
                                endpos = 0;

                            //sets up starting and ending quaternion angles for the vector3 offset/raycast.
                            Quaternion startq = Quaternion.AngleAxis(startpos, transform.right);
                            Quaternion endq = Quaternion.AngleAxis(endpos, transform.right);
                            //computes rotation for each raycast using a lerp. The time percentage is modified above using the animation time.
                            Quaternion slerpq = Quaternion.Slerp(startq, endq, perc);
                            //sets start range at negative the itemrange to deal with raycasting starting on the third frame in.
                            changeitemrange = Mathf.Lerp(itemRange * -1, itemRange, perc);

                            //computes rotation for each raycast.
                            attackcast = slerpq * (mainCamera.transform.forward * (changeitemrange - SphereCastRadius));
                            FireRayArc();
                        }
                        else if (ScreenWeapon.WeaponType != WeaponTypes.Melee)
                        {
                            if ((startpos = FPSConsoleCommands.ChangeHorPos.SchangeHorPos) == 0)
                                startpos = -35;

                            if ((endpos = FPSConsoleCommands.ChangeHorPos.EchangeHorPos) == 0)
                                endpos = 30;

                            //sets up starting and ending quaternion angles for the vector3 offset/raycast.
                            Quaternion startq = Quaternion.AngleAxis(startpos, transform.right);
                            Quaternion endq = Quaternion.AngleAxis(endpos, transform.right);
                            //computes rotation for each raycast using a lerp. The time percentage is modified above using the animation time.
                            Quaternion slerpq = Quaternion.Slerp(startq, endq, perc);
                            //computes rotation for each raycast. First angle sets permanent left/right offset. Second runs ray down cast for each tick.
                            attackcast = Quaternion.AngleAxis(startframe, transform.up) * (slerpq * (mainCamera.transform.forward * (itemRange - SphereCastRadius)));
                            FireRayArc();
                        }
                    }
                    else if (ScreenWeapon.WeaponState == WeaponStates.StrikeRight)
                    {
                        if (ScreenWeapon.WeaponType == WeaponTypes.Melee && ScreenWeapon.GetCurrentFrame() > 1)
                        {
                            if ((startpos = FPSConsoleCommands.ChangeHorPos.SchangeHorPos) == 0)
                                startpos = 15;

                            if ((endpos = FPSConsoleCommands.ChangeHorPos.EchangeHorPos) == 0)
                                endpos = -5;

                            //sets up starting and ending quaternion angles for the vector3 offset/raycast.
                            Quaternion startq = Quaternion.AngleAxis(startpos, transform.up);
                            Quaternion endq = Quaternion.AngleAxis(endpos, transform.up);
                            //computes rotation for each raycast using a lerp. The time percentage is modified above using the animation time.
                            Quaternion slerpq = Quaternion.Slerp(startq, endq, perc);

                            //sets start range at negative the itemrange to deal with raycasting starting on the third frame in.
                            changeitemrange = Mathf.Lerp(itemRange * -1, itemRange, perc);

                            //computes rotation for each raycast.
                            attackcast = Quaternion.AngleAxis(10, transform.right) * (slerpq * (mainCamera.transform.forward * (changeitemrange - SphereCastRadius)));
                            FireRayArc();
                        }
                        else if (ScreenWeapon.WeaponType != WeaponTypes.Melee)
                        {
                            if ((startpos = FPSConsoleCommands.ChangeHorPos.SchangeHorPos) == 0)
                                startpos = -35;

                            if ((endpos = FPSConsoleCommands.ChangeHorPos.EchangeHorPos) == 0)
                                endpos = 45;

                            //sets up starting and ending quaternion angles for the vector3 offset/raycast.
                            Quaternion startq = Quaternion.AngleAxis(startpos, transform.up);
                            Quaternion endq = Quaternion.AngleAxis(endpos, transform.up);
                            //computes rotation for each raycast using a lerp. The time percentage is modified above using the animation time.
                            Quaternion slerpq = Quaternion.Slerp(startq, endq, perc);
                            attackcast = slerpq * (mainCamera.transform.forward * (itemRange - SphereCastRadius));
                            FireRayArc();
                        }
                    }
                    else if (ScreenWeapon.WeaponState == WeaponStates.StrikeLeft)
                    {
                        if (ScreenWeapon.WeaponType == WeaponTypes.Melee)
                        {
                            //sets start range at negative the itemrange to deal with raycasting starting on the third frame in.
                            changeitemrange = Mathf.Lerp(itemRange * -1, itemRange, perc);

                            //computes rotation for each raycast.
                            attackcast = Quaternion.AngleAxis(10, transform.right) * (Quaternion.AngleAxis(15, transform.up) * (mainCamera.transform.forward * (changeitemrange - SphereCastRadius)));
                            FireRayArc();
                        }
                        else
                        {
                            if ((startpos = FPSConsoleCommands.ChangeHorPos.SchangeHorPos) == 0)
                                startpos = 45;

                            if ((endpos = FPSConsoleCommands.ChangeHorPos.EchangeHorPos) == 0)
                                endpos = -45;

                            //sets up starting and ending quaternion angles for the vector3 offset/raycast.
                            Quaternion startq = Quaternion.AngleAxis(startpos, transform.up);
                            Quaternion endq = Quaternion.AngleAxis(endpos, transform.up);
                            //computes rotation for each raycast using a lerp. The time percentage is modified above using the animation time.
                            Quaternion slerpq = Quaternion.Slerp(startq, endq, perc);
                            attackcast = slerpq * (mainCamera.transform.forward * (itemRange - SphereCastRadius));
                            FireRayArc();
                        }
                    }
                }
            }

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
                //bool hitEnemy = false;

                // Non-bow weapons
                if (ScreenWeapon.WeaponType != WeaponTypes.Bow)
                {
                    //MeleeDamage(ScreenWeapon, out hitEnemy); // Bow weapons
                }
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
                if (!hitobject && ScreenWeapon.WeaponType != WeaponTypes.Bow)
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

        

        void FireRayArc()
        {
            if (changeitemrange == 0)
                changeitemrange = itemRange;

            //assigns the above triggered attackcast to the debug ray for easy debugging in unity.
            Debug.DrawRay(mainCamera.transform.position + -mainCamera.transform.forward * 0.1f, attackcast, Color.red, 5);
            //creates engine raycast, assigns current player camera position as starting vector and attackcast vector as the direction.
            RaycastHit hit;
            Ray ray = new Ray(mainCamera.transform.position + -mainCamera.transform.forward * 0.1f, attackcast);
            //attaches the ray to a physics spherecast, shoots it out, and triggers true when any object is hit.           
            if (Physics.SphereCast(ray, SphereCastRadius, out hit, changeitemrange))
            {
                //assigns hit object to true so fpsweapon will start collision animation loop and raycast loop will stop shooting raycasts.
                hitobject = true;
                changeitemrange = 0;
                //sets frame back to the current frame, as recoil system moves to next frame before starting recoil causing an frame skip.
                ScreenWeapon.SetCurrentFrame(CurrentFrame);
                //checks what entity is hit, if it is a valid enemy, it checks for successful hit and assigns out damage. Returns true when hits enemy.
                WeaponDamage(strikingWeapon, false, hit.transform, hit.point, mainCamera.transform.forward);
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
