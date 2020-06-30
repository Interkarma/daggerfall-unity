// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Allofich
// 
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Enemy motor and AI combat decision-making logic.
    /// </summary>
    [RequireComponent(typeof(EnemySenses))]
    [RequireComponent(typeof(EnemyAttack))]
    [RequireComponent(typeof(EnemyBlood))]
    [RequireComponent(typeof(EnemySounds))]
    [RequireComponent(typeof(CharacterController))]
    public class EnemyMotor : MonoBehaviour
    {

        #region Member Variables

        public float OpenDoorDistance = 2f;         // Maximum distance to open door
        const float attackSpeedDivisor = 2f;        // How much to slow down during attack animations
        float stopDistance = 1.7f;                  // Used to prevent orbiting
        bool flies;                                 // The enemy can fly
        bool swims;                                 // The enemy can swim
        bool pausePursuit;                          // pause to wait for the player to come closer to ground
        float moveInForAttackTimer;                 // Time until next pursue/retreat decision
        bool moveInForAttack;                       // False = retreat. True = pursue.
        float retreatDistanceMultiplier;            // How far to back off while retreating
        float changeStateTimer;                     // Time until next change in behavior. Padding to prevent instant reflexes.
        bool doStrafe;
        float strafeTimer;
        bool pursuing;                              // Is pursuing
        bool retreating;                            // Is retreating
        bool backingUp;                             // Is backing up
        bool fallDetected;                          // Detected a fall in front of us, so don't move there
        bool obstacleDetected;
        bool foundUpwardSlope;
        bool foundDoor;
        Vector3 lastPosition;                       // Used to track whether we have moved or not
        Vector3 lastDirection;                      // Used to track whether we have rotated or not
        bool rotating;                              // Used to track whether we have rotated or not
        float avoidObstaclesTimer;
        bool checkingClockwise;
        float checkingClockwiseTimer;
        bool didClockwiseCheck;
        float lastTimeWasStuck;
        bool hasBowAttack;
        float realHeight;
        float centerChange;
        bool resetHeight;
        float heightChangeTimer;
        bool strafeLeft;
        float strafeAngle;
        int searchMult;
        int ignoreMaskForShooting;
        bool canAct;
        bool falls;
        bool flyerFalls;
        float lastGroundedY;                        // Used for fall damage
        float originalHeight;

        EnemySenses senses;
        Vector3 destination;
        Vector3 detourDestination;
        CharacterController controller;
        DaggerfallMobileUnit mobile;
        DaggerfallEntityBehaviour entityBehaviour;
        EnemyBlood entityBlood;
        EntityEffectManager entityEffectManager;
        EntityEffectBundle selectedSpell;
        EnemyAttack attack;
        EnemyEntity entity;
        #endregion

        #region Auto Properties

        public bool IsLevitating { get; set; }      // Is this enemy levitating
        public bool IsHostile { get; set; }         // Is this enemy hostile to the player
        public float KnockbackSpeed { get; set; }   // While non-zero, this enemy will be knocked back at this speed
        public Vector3 KnockbackDirection { get; set; } // Direction to travel while being knocked back
        public bool Bashing { get; private set; }   // Is this enemy bashing a door
        public int GiveUpTimer { get; set; }        // Timer for enemy giving up pursuit of target
        #endregion

        #region Unity Methods

        void Start()
        {
            senses = GetComponent<EnemySenses>();
            controller = GetComponent<CharacterController>();
            mobile = GetComponentInChildren<DaggerfallMobileUnit>();
            IsHostile = mobile.Summary.Enemy.Reactions == MobileReactions.Hostile;
            flies = CanFly();
            swims = mobile.Summary.Enemy.Behaviour == MobileBehaviour.Aquatic;
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            entityBlood = GetComponent<EnemyBlood>();
            entityEffectManager = GetComponent<EntityEffectManager>();
            entity = entityBehaviour.Entity as EnemyEntity;
            attack = GetComponent<EnemyAttack>();

            // Only need to check for ability to shoot bow once.
            hasBowAttack = mobile.Summary.Enemy.HasRangedAttack1 && mobile.Summary.Enemy.ID > 129 && mobile.Summary.Enemy.ID != 132;

            // Add things AI should ignore when checking for a clear path to shoot.
            ignoreMaskForShooting = ~(1 << LayerMask.NameToLayer("SpellMissiles") | 1 << LayerMask.NameToLayer("Ignore Raycast"));

            lastGroundedY = transform.position.y;

            // Get original height, before any height adjustments
            originalHeight = controller.height;
        }

        void FixedUpdate()
        {
            if (GameManager.Instance.DisableAI)
                return;

            flies = CanFly();
            canAct = true;
            flyerFalls = false;
            falls = false;

            HandleParalysis();
            KnockbackMovement();
            ApplyGravity();
            HandleNoAction();
            HandleBashing();
            UpdateTimers();
            if (canAct)
                TakeAction();
            ApplyFallDamage();
            UpdateToIdleOrMoveAnim();
            OpenDoors();
            HeightAdjust();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Immediately become hostile towards attacker and know attacker's location.
        /// </summary>
        /// <param name="attacker">Attacker to become hostile towards</param>
        public void MakeEnemyHostileToAttacker(DaggerfallEntityBehaviour attacker)
        {
            if (!senses)
                senses = GetComponent<EnemySenses>();
            if (!entityBehaviour)
                entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();

            // Assign target if don't already have target, or original target isn't seen or adjacent
            if (attacker && senses && (senses.Target == null || !senses.TargetInSight || senses.DistanceToTarget > 2f))
            {
                senses.Target = attacker;
                senses.SecondaryTarget = senses.Target;
                senses.OldLastKnownTargetPos = attacker.transform.position;
                senses.LastKnownTargetPos = attacker.transform.position;
                senses.PredictedTargetPos = attacker.transform.position;
                GiveUpTimer = 200;
            }

            if (attacker == GameManager.Instance.PlayerEntityBehaviour)
            {
                IsHostile = true;
                // Reset former ally's team
                if (entityBehaviour.Entity.Team == MobileTeams.PlayerAlly)
                {
                    int id = (entityBehaviour.Entity as EnemyEntity).MobileEnemy.ID;
                    entityBehaviour.Entity.Team = EnemyBasics.Enemies[id].Team;
                }
            }
        }

        /// <summary>
        /// Attempts to find the ground position below enemy, even if player is flying/falling
        /// </summary>
        /// <param name="distance">Distance to fire ray.</param>
        /// <returns>Hit point on surface below enemy, or enemy position if hit not found in distance.</returns>
        public Vector3 FindGroundPosition(float distance = 16)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out hit, distance))
                return hit.point;

            return transform.position;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Handle paralysis halting movement and animation.
        /// </summary>
        void HandleParalysis()
        {
            // Cancel movement and animations if paralyzed, but still allow gravity to take effect
            // This will have the (intentional for now) side-effect of making paralyzed flying enemies fall out of the air
            // Paralyzed swimming enemies will just freeze in place
            // Freezing anims also prevents the attack from triggering until paralysis cleared
            if (entityBehaviour.Entity.IsParalyzed)
            {
                mobile.FreezeAnims = true;
                canAct = false;
                flyerFalls = true;
            }
            mobile.FreezeAnims = false;
        }

        /// <summary>
        /// Handles movement if the enemy has been knocked back.
        /// </summary>
        void KnockbackMovement()
        {
            // If hit, get knocked back
            if (KnockbackSpeed > 0)
            {
                // Limit KnockbackSpeed. This can be higher than what is actually used for the speed of motion,
                // making it last longer and do more damage if the enemy collides with something (TODO).
                if (KnockbackSpeed > (40 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)))
                    KnockbackSpeed = (40 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10));

                if (KnockbackSpeed > (5 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)) &&
                    mobile.Summary.EnemyState != MobileStates.PrimaryAttack)
                {
                    mobile.ChangeEnemyState(MobileStates.Hurt);
                }

                // Actual speed of motion is limited
                Vector3 motion;
                if (KnockbackSpeed <= (25 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)))
                    motion = KnockbackDirection * KnockbackSpeed;
                else
                    motion = KnockbackDirection * (25 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10));

                // Move in direction of knockback
                if (swims)
                    WaterMove(motion);
                else if (flies || IsLevitating)
                    controller.Move(motion * Time.deltaTime);
                else
                    controller.SimpleMove(motion);

                // Remove remaining knockback and restore animation
                if (GameManager.ClassicUpdate)
                {
                    KnockbackSpeed -= (5 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10));
                    if (KnockbackSpeed <= (5 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10))
                        && mobile.Summary.EnemyState != MobileStates.PrimaryAttack)
                    {
                        mobile.ChangeEnemyState(MobileStates.Move);
                    }
                }

                // If a decent hit got in, reconsider whether to continue current tactic
                if (KnockbackSpeed > (10 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)))
                {
                    EvaluateMoveInForAttack();
                }

                canAct = false;
                flyerFalls = true;
            }
        }

        /// <summary>
        /// Apply gravity to ground-based enemies and paralyzed flyers.
        /// </summary>
        void ApplyGravity()
        {
            // Apply gravity
            if (!flies && !swims && !IsLevitating && !controller.isGrounded)
            {
                controller.SimpleMove(Vector3.zero);
                falls = true;

                // Only cancel movement if actually falling. Sometimes mobiles can get stuck where they are !isGrounded but SimpleMove(Vector3.zero) doesn't help.
                // Allowing them to continue and attempt a Move() frees them, but we don't want to allow that if we can avoid it so they aren't moving
                // while falling, which can also accelerate the fall due to anti-bounce downward movement in Move().
                if (lastPosition != transform.position)
                    canAct = false;
            }

            if (flyerFalls && flies && !IsLevitating)
            {
                controller.SimpleMove(Vector3.zero);
                falls = true;
            }
        }

        /// <summary>
        /// Do nothing if no target or after giving up finding the target or if target position hasn't been acquired yet.
        /// </summary>
        void HandleNoAction()
        {
            if (senses.Target == null || GiveUpTimer <= 0 || senses.PredictedTargetPos == EnemySenses.ResetPlayerPos)
            {
                SetChangeStateTimer();
                searchMult = 0;

                canAct = false;
            }
        }

        /// <summary>
        /// Handle bashing doors.
        /// </summary>
        void HandleBashing()
        {
            if (Bashing)
            {
                int speed = entity.Stats.LiveSpeed;
                if (GameManager.ClassicUpdate && DFRandom.rand() % speed >= (speed >> 3) + 6 && attack.MeleeTimer == 0)
                {
                    mobile.ChangeEnemyState(MobileStates.PrimaryAttack);
                    attack.ResetMeleeTimer();
                }

                canAct = false;
            }
        }

        /// <summary>
        /// Updates timers used in this class.
        /// </summary>
        void UpdateTimers()
        {
            if (moveInForAttackTimer > 0)
                moveInForAttackTimer -= Time.deltaTime;

            if (avoidObstaclesTimer > 0)
                avoidObstaclesTimer -= Time.deltaTime;

            // Set avoidObstaclesTimer to 0 if got close enough to detourDestination. Only bother checking if possible to move.
            if (avoidObstaclesTimer > 0 && canAct)
            {
                Vector3 detourDestination2D = detourDestination;
                detourDestination2D.y = transform.position.y;
                if ((detourDestination2D - transform.position).magnitude <= 0.3f)
                {
                    avoidObstaclesTimer = 0;
                }
            }

            if (checkingClockwiseTimer > 0)
                checkingClockwiseTimer -= Time.deltaTime;

            if (changeStateTimer > 0)
                changeStateTimer -= Time.deltaTime;

            if (strafeTimer > 0)
                strafeTimer -= Time.deltaTime;

            // As long as the target is detected,
            // giveUpTimer is reset to full
            if (senses.DetectedTarget)
                GiveUpTimer = 200;

            // GiveUpTimer value is from classic, so decrease at the speed of classic's update loop
            if (GameManager.ClassicUpdate && !senses.DetectedTarget && GiveUpTimer > 0)
                GiveUpTimer--;
        }

        /// <summary>
        /// Make decision about what action to take.
        /// </summary>
        void TakeAction()
        {
            // Monster speed of movement follows the same formula as for when the player walks
            float moveSpeed = (entity.Stats.LiveSpeed + PlayerSpeedChanger.dfWalkBase) * MeshReader.GlobalScale;

            // Get isPlayingOneShot for use below
            bool isPlayingOneShot = mobile.IsPlayingOneShot();

            // Reduced speed if playing a one-shot animation with enhanced AI
            if (isPlayingOneShot && DaggerfallUnity.Settings.EnhancedCombatAI)
                moveSpeed /= attackSpeedDivisor;

            // Classic AI moves only as close as melee range. It uses a different range for the player and for other AI.
            if (!DaggerfallUnity.Settings.EnhancedCombatAI)
            {
                if (senses.Target == GameManager.Instance.PlayerEntityBehaviour)
                    stopDistance = attack.MeleeDistance;
                else
                    stopDistance = attack.ClassicMeleeDistanceVsAI;
            }

            // Get location to move towards.
            GetDestination();

            // Get direction & distance to destination.
            Vector3 direction = (destination - transform.position).normalized;

            float distance;
            // If enemy sees the target, use the distance value from EnemySenses, as this is also used for the melee attack decision and we need to be consistent with that.
            if (avoidObstaclesTimer <= 0 && senses.TargetInSight)
                distance = senses.DistanceToTarget;
            else
                distance = (destination - transform.position).magnitude;

            // Do not change action if currently playing oneshot wants to stop actions
            if (isPlayingOneShot && mobile.OneShotPauseActionsWhilePlaying())
                return;

            // Ranged attacks
            if (DoRangedAttack(direction, moveSpeed, distance, isPlayingOneShot))
                return;

            // Touch spells
            if (DoTouchSpell())
                return;

            // Update advance/retreat decision
            if (moveInForAttackTimer <= 0 && avoidObstaclesTimer <= 0)
                EvaluateMoveInForAttack();

            // If detouring, always attempt to move
            if (avoidObstaclesTimer > 0)
            {
                AttemptMove(direction, moveSpeed);
            }
            // Otherwise, if not still executing a retreat, approach target until close enough to be on-guard.
            // If decided to move in for attack, continue until within melee range. Classic always moves in for attack.
            else if ((!retreating && distance >= (stopDistance * 2.75)) || (distance > stopDistance && moveInForAttack))
            {
                // If state change timer is done, or we are continuing an already started pursuit, we can move immediately
                if (changeStateTimer <= 0 || pursuing)
                    AttemptMove(direction, moveSpeed);
                // Otherwise, look at target until timer finishes
                else if (!senses.TargetIsWithinYawAngle(22.5f, destination))
                    TurnToTarget(direction);
            }
            else if (DaggerfallUnity.Settings.EnhancedCombatAI && strafeTimer <= 0)
            {
                StrafeDecision();
            }
            else if (doStrafe && strafeTimer > 0 && (distance >= stopDistance * .8f))
            {
                AttemptMove(direction, moveSpeed / 4, false, true, distance);
            }
            // Back away from combat target if right next to it, or if decided to retreat and enemy is too close.
            // Classic AI never backs away.
            else if (DaggerfallUnity.Settings.EnhancedCombatAI && senses.TargetInSight && (distance < stopDistance * .8f ||
                !moveInForAttack && distance < stopDistance * retreatDistanceMultiplier && (changeStateTimer <= 0 || retreating)))
            {
                // If state change timer is done, or we are already executing a retreat, we can move immediately
                if (changeStateTimer <= 0 || retreating)
                    AttemptMove(direction, moveSpeed / 2, true);
            }
            // Not moving, just look at target
            else if (!senses.TargetIsWithinYawAngle(22.5f, destination))
            {
                TurnToTarget(direction);
            }
            else // Not moving, and no need to turn
            {
                SetChangeStateTimer();
                pursuing = false;
                retreating = false;
            }
        }

        /// <summary>
        /// Get the destination to move towards.
        /// </summary>
        void GetDestination()
        {
            CharacterController targetController = senses.Target.GetComponent<CharacterController>();
            // If detouring around an obstacle or fall, use the detour position
            if (avoidObstaclesTimer > 0)
            {
                destination = detourDestination;
            }
            // Otherwise, try to get to the combat target if there is a clear path to it
            else if (ClearPathToPosition(senses.PredictedTargetPos, (destination - transform.position).magnitude) || (senses.TargetInSight && (hasBowAttack || entity.CurrentMagicka > 0)))
            {
                destination = senses.PredictedTargetPos;
                // Flying enemies and slaughterfish aim for target face
                if (flies || IsLevitating || (swims && mobile.Summary.Enemy.ID == (int)MonsterCareers.Slaughterfish))
                    destination.y += targetController.height * 0.5f;

                searchMult = 0;
            }
            // Otherwise, search for target based on its last known position and direction
            else
            {
                Vector3 searchPosition = senses.LastKnownTargetPos + (senses.LastPositionDiff.normalized * searchMult);
                if (searchMult <= 10 && (searchPosition - transform.position).magnitude <= stopDistance)
                    searchMult++;

                destination = searchPosition;
            }

            if (avoidObstaclesTimer <= 0 && !flies && !IsLevitating && !swims && senses.Target)
            {
                // Ground enemies target at their own height
                // Otherwise, short enemies' vector can aim up towards the target, which could interfere with distance-to-target calculations.
                float deltaHeight = (targetController.height - originalHeight) / 2;
                destination.y -= deltaHeight;
            }
        }

        /// <summary>
        /// Handles ranged attacks with bows and spells.
        /// </summary>
        bool DoRangedAttack(Vector3 direction, float moveSpeed, float distance, bool isPlayingOneShot)
        {
            bool inRange = senses.DistanceToTarget > EnemyAttack.minRangedDistance && senses.DistanceToTarget < EnemyAttack.maxRangedDistance;
            if (inRange && senses.TargetInSight && senses.DetectedTarget && (CanShootBow() || CanCastRangedSpell()))
            {
                if (DaggerfallUnity.Settings.EnhancedCombatAI && senses.TargetIsWithinYawAngle(22.5f, destination) && strafeTimer <= 0)
                {
                    StrafeDecision();
                }

                if (doStrafe && strafeTimer > 0)
                {
                    AttemptMove(direction, moveSpeed / 4, false, true, distance);
                }

                if (GameManager.ClassicUpdate && senses.TargetIsWithinYawAngle(22.5f, destination))
                {
                    if (!isPlayingOneShot)
                    {
                        if (hasBowAttack)
                        {
                            // Random chance to shoot bow
                            if (Random.value < 1/32f)
                            {
                                if (mobile.Summary.Enemy.HasRangedAttack1 && !mobile.Summary.Enemy.HasRangedAttack2)
                                    mobile.ChangeEnemyState(MobileStates.RangedAttack1);
                                else if (mobile.Summary.Enemy.HasRangedAttack2)
                                    mobile.ChangeEnemyState(MobileStates.RangedAttack2);
                            }
                        }
                        // Random chance to shoot spell
                        else if (Random.value < 1/40f && entityEffectManager.SetReadySpell(selectedSpell))
                        {
                            mobile.ChangeEnemyState(MobileStates.Spell);
                        }
                    }
                }
                else
                    TurnToTarget(direction);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles touch-range spells.
        /// </summary>
        bool DoTouchSpell()
        {
            if (senses.TargetInSight && senses.DetectedTarget && attack.MeleeTimer == 0
                && senses.DistanceToTarget <= attack.MeleeDistance + senses.TargetRateOfApproach
                && CanCastTouchSpell() && entityEffectManager.SetReadySpell(selectedSpell))
            {
                if (mobile.Summary.EnemyState != MobileStates.Spell)
                    mobile.ChangeEnemyState(MobileStates.Spell);

                attack.ResetMeleeTimer();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Decide whether to strafe, and get direction to strafe to.
        /// </summary>
        void StrafeDecision()
        {
            doStrafe = Random.Range(0, 4) == 0;
            strafeTimer = Random.Range(1f, 2f);
            if (doStrafe)
            {
                if (Random.Range(0, 2) == 0)
                    strafeLeft = true;
                else
                    strafeLeft = false;

                Vector3 north = destination;
                north.z++; // Adding 1 to z so this Vector3 will be north of the destination Vector3.

                // Get angle between vector from destination to the north of it, and vector from destination to this enemy's position
                strafeAngle = Vector3.SignedAngle(destination - north, destination - transform.position, Vector3.up);
                if (strafeAngle < 0)
                    strafeAngle = 360 + strafeAngle;

                // Convert to radians
                strafeAngle *= Mathf.PI / 180;
            }
        }

        /// <summary>
        /// Returns whether there is a clear path to move the given distance from the current location towards the given location. True if clear
        /// or if combat target is the first obstacle hit.
        /// </summary>
        bool ClearPathToPosition(Vector3 location, float dist = 30)
        {
            Vector3 sphereCastDir = (location - transform.position).normalized;
            Vector3 sphereCastDir2d = sphereCastDir;
            sphereCastDir2d.y = 0;
            ObstacleCheck(sphereCastDir2d);
            FallCheck(sphereCastDir2d);

            if (obstacleDetected || fallDetected)
                return false;

            RaycastHit hit;
            if (Physics.SphereCast(transform.position, controller.radius / 2, sphereCastDir, out hit, dist, ignoreMaskForShooting))
            {
                DaggerfallEntityBehaviour hitTarget = hit.transform.GetComponent<DaggerfallEntityBehaviour>();
                if (hitTarget == senses.Target)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns true if can shoot projectile at target.
        /// </summary>
        bool HasClearPathToShootProjectile(float speed, float radius)
        {
            // Check that there is a clear path to shoot projectile
            Vector3 sphereCastDir = senses.PredictNextTargetPos(speed);
            if (sphereCastDir == EnemySenses.ResetPlayerPos)
                return false;

            float sphereCastDist = (sphereCastDir - transform.position).magnitude;
            sphereCastDir = (sphereCastDir - transform.position).normalized;

            RaycastHit hit;
            if (Physics.SphereCast(transform.position, radius, sphereCastDir, out hit, sphereCastDist, ignoreMaskForShooting))
            {
                DaggerfallEntityBehaviour hitTarget = hit.transform.GetComponent<DaggerfallEntityBehaviour>();

                // Clear path to target
                if (hitTarget == senses.Target)
                    return true;

                // Something in the way
                return false;
            }

            // Clear path to predicted target position
            return true;
        }

        /// <summary>
        /// Returns true if can shoot bow at target.
        /// </summary>
        bool CanShootBow()
        {
            if (!hasBowAttack)
                return false;

            // Check that there is a clear path to shoot an arrow
            // All arrows are currently 35 speed.
            return HasClearPathToShootProjectile(35f, 0.15f);
        }

        /// <summary>
        /// Selects a ranged spell from this enemy's list and returns true if it can be cast.
        /// </summary>
        bool CanCastRangedSpell()
        {
            if (entity.CurrentMagicka <= 0)
                return false;

            EffectBundleSettings[] spells = entity.GetSpells();
            List<EffectBundleSettings> rangeSpells = new List<EffectBundleSettings>();
            int count = 0;
            foreach (EffectBundleSettings spell in spells)
            {
                if (spell.TargetType == TargetTypes.SingleTargetAtRange
                    || spell.TargetType == TargetTypes.AreaAtRange)
                {
                    rangeSpells.Add(spell);
                    count++;
                }
            }

            if (count == 0)
                return false;

            EffectBundleSettings selectedSpellSettings = rangeSpells[Random.Range(0, count)];
            selectedSpell = new EntityEffectBundle(selectedSpellSettings, entityBehaviour);

            if (EffectsAlreadyOnTarget(selectedSpell))
                return false;

            // Check that there is a clear path to shoot a spell
            // All range spells are currently 25 speed and 0.45f radius
            return HasClearPathToShootProjectile(25f, 0.45f);
        }

        /// <summary>
        /// Selects a touch spell from this enemy's list and returns true if it can be cast.
        /// </summary>
        bool CanCastTouchSpell()
        {
            if (entity.CurrentMagicka <= 0)
                return false;

            EffectBundleSettings[] spells = entity.GetSpells();
            List<EffectBundleSettings> rangeSpells = new List<EffectBundleSettings>();
            int count = 0;
            foreach (EffectBundleSettings spell in spells)
            {
                // Classic AI considers ByTouch and CasterOnly here
                if (!DaggerfallUnity.Settings.EnhancedCombatAI)
                {
                    if (spell.TargetType == TargetTypes.ByTouch
                        || spell.TargetType == TargetTypes.CasterOnly)
                    {
                        rangeSpells.Add(spell);
                        count++;
                    }
                }
                else // Enhanced AI considers ByTouch and AreaAroundCaster. TODO: CasterOnly logic
                {
                    if (spell.TargetType == TargetTypes.ByTouch
                        || spell.TargetType == TargetTypes.AreaAroundCaster)
                    {
                        rangeSpells.Add(spell);
                        count++;
                    }
                }
            }

            if (count == 0)
                return false;

            EffectBundleSettings selectedSpellSettings = rangeSpells[Random.Range(0, count)];
            selectedSpell = new EntityEffectBundle(selectedSpellSettings, entityBehaviour);

            if (EffectsAlreadyOnTarget(selectedSpell))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if enemy can fly based on behaviour.
        /// This can change in the case of a transformed Seducer.
        /// </summary>
        /// <returns>True if enemy can fly.</returns>
        bool CanFly()
        {
            return mobile.Summary.Enemy.Behaviour == MobileBehaviour.Flying || mobile.Summary.Enemy.Behaviour == MobileBehaviour.Spectral;
        }

        /// <summary>
        /// Checks whether the target already is affected by all of the effects of the given spell.
        /// </summary>
        bool EffectsAlreadyOnTarget(EntityEffectBundle spell)
        {
            if (senses.Target)
            {
                EntityEffectManager targetEffectManager = senses.Target.GetComponent<EntityEffectManager>();
                LiveEffectBundle[] bundles = targetEffectManager.EffectBundles;

                for (int i = 0; i < spell.Settings.Effects.Length; i++)
                {
                    bool foundEffect = false;
                    // Get effect template
                    IEntityEffect effectTemplate = GameManager.Instance.EntityEffectBroker.GetEffectTemplate(spell.Settings.Effects[i].Key);
                    for (int j = 0; j < bundles.Length && !foundEffect; j++)
                    {
                        for (int k = 0; k < bundles[j].liveEffects.Count && !foundEffect; k++)
                        {
                            if (bundles[j].liveEffects[k].GetType() == effectTemplate.GetType())
                                foundEffect = true;
                        }
                    }

                    if (!foundEffect)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Try to move in given direction.
        /// </summary>
        void AttemptMove(Vector3 direction, float moveSpeed, bool backAway = false, bool strafe = false, float strafeDist = 0)
        {
            // Set whether pursuing or retreating, for bypassing changeStateTimer delay when continuing these actions
            if (!backAway && !strafe)
            {
                pursuing = true;
                retreating = false;
            }
            else
            {
                retreating = true;
                pursuing = false;
            }

            if (!senses.TargetIsWithinYawAngle(5.625f, destination))
            {
                TurnToTarget(direction);
                // Classic always turns in place. Enhanced only does so if enemy is not in sight,
                // for more natural-looking movement while pursuing.
                if (!DaggerfallUnity.Settings.EnhancedCombatAI || !senses.TargetInSight)
                    return;
            }

            if (backAway)
                direction *= -1;

            if (strafe)
            {
                Vector3 strafeDest = new Vector3(destination.x + (Mathf.Sin(strafeAngle) * strafeDist), transform.position.y, destination.z + (Mathf.Cos(strafeAngle) * strafeDist));
                direction = (strafeDest - transform.position).normalized;

                if ((strafeDest - transform.position).magnitude <= 0.2f)
                {
                    if (strafeLeft)
                        strafeAngle++;
                    else
                        strafeAngle--;
                }
            }

            // Move downward some to eliminate bouncing down inclines
            if (!flies && !swims && !IsLevitating && controller.isGrounded)
                direction.y = -2f;

            // Stop fliers from moving too near the floor during combat
            if (flies && avoidObstaclesTimer <= 0 && direction.y < 0 && FindGroundPosition((originalHeight / 2) + 1f) != transform.position)
                direction.y = 0.1f;

            Vector3 motion = direction * moveSpeed;

            // If using enhanced combat, avoid moving directly below targets
            if (!backAway && DaggerfallUnity.Settings.EnhancedCombatAI && avoidObstaclesTimer <= 0)
            {
                bool withinPitch = senses.TargetIsWithinPitchAngle(45.0f);
                if (!pausePursuit && !withinPitch)
                {
                    if (flies || IsLevitating || swims)
                    {
                        if (!senses.TargetIsAbove())
                            motion = -transform.up * moveSpeed / 2;
                        else
                            motion = transform.up * moveSpeed;
                    }
                    // Causes a random delay after being out of pitch range
                    else if (senses.TargetIsAbove() && changeStateTimer <= 0)
                    {
                        SetChangeStateTimer();
                        pausePursuit = true;
                    }
                }
                else if (withinPitch)
                {
                    pausePursuit = false;
                    backingUp = false;
                }

                if (pausePursuit)
                {
                    if (senses.TargetIsAbove() && !senses.TargetIsWithinPitchAngle(55.0f) && (changeStateTimer <= 0 || backingUp))
                    {
                        // Back away from target
                        motion = -transform.forward * moveSpeed * 0.75f;
                        backingUp = true;
                    }
                    else
                    {
                        // Stop moving
                        backingUp = false;
                        return;
                    }
                }
            }

            SetChangeStateTimer();

            // Check if there is something to collide with directly in movement direction, such as upward sloping ground.
            Vector3 direction2d = direction;
            if (!flies && !swims && !IsLevitating)
                direction2d.y = 0;
            ObstacleCheck(direction2d);
            FallCheck(direction2d);

            if (fallDetected || obstacleDetected)
            {
                if (!strafe && !backAway)
                    FindDetour(direction2d);
            }
            else
            // Clear to move
            {
                if (swims)
                    WaterMove(motion);
                else
                    controller.Move(motion * Time.deltaTime);
            }
        }

        /// <summary>
        /// Try to find a way around an obstacle or fall.
        /// </summary>
        void FindDetour(Vector3 direction2d)
        {
            float angle;
            Vector3 testMove = Vector3.zero;
            bool foundUpDown = false;

            // Try up/down first
            if (flies || swims || IsLevitating)
            {
                float multiplier = 0.3f;
                if (Random.Range(0, 2) == 0)
                    multiplier = -0.3f;

                Vector3 upOrDown = new Vector3(0, 1, 0);
                upOrDown.y *= multiplier;

                testMove = (direction2d + upOrDown).normalized;

                ObstacleCheck(testMove);
                if (obstacleDetected)
                {
                    upOrDown.y *= -1;
                    testMove = (direction2d + upOrDown).normalized;
                    ObstacleCheck(testMove);
                }
                if (!obstacleDetected)
                    foundUpDown = true;
            }

            // Reset clockwise check if we've been clear of obstacles/falls for a while
            if (!foundUpDown && Time.time - lastTimeWasStuck > 2f)
            {
                checkingClockwiseTimer = 0;
                didClockwiseCheck = false;
            }

            if (!foundUpDown && checkingClockwiseTimer <= 0)
            {
                if (!didClockwiseCheck)
                {
                    // Check 45 degrees in both ways first
                    // Pick first direction to check randomly
                    if (Random.Range(0, 2) == 0)
                        angle = 45;
                    else
                        angle = -45;

                    testMove = Quaternion.AngleAxis(angle, Vector3.up) * direction2d;
                    ObstacleCheck(testMove);
                    FallCheck(testMove);

                    if (!obstacleDetected && !fallDetected)
                    {
                        // First direction was clear, use that way
                        if (angle == 45)
                        {
                            checkingClockwise = true;
                        }
                        else
                            checkingClockwise = false;
                    }
                    else
                    {
                        // Tested 45 degrees in the clockwise/counter-clockwise direction we chose,
                        // but hit something, so try other one.
                        angle *= -1;
                        testMove = Quaternion.AngleAxis(angle, Vector3.up) * direction2d;
                        ObstacleCheck(testMove);
                        FallCheck(testMove);

                        if (!obstacleDetected && !fallDetected)
                        {
                            if (angle == 45)
                            {
                                checkingClockwise = true;
                            }
                            else
                                checkingClockwise = false;
                        }
                        else
                        {
                            // Both 45 degrees checks failed, pick clockwise/counterclockwise based on angle to target
                            Vector3 toTarget = destination - transform.position;
                            Vector3 directionToTarget = toTarget.normalized;
                            angle = Vector3.SignedAngle(directionToTarget, direction2d, Vector3.up);

                            if (angle > 0)
                            {
                                checkingClockwise = true;
                            }
                            else
                                checkingClockwise = false;
                        }
                    }
                    checkingClockwiseTimer = 5;
                    didClockwiseCheck = true;
                }
                else
                {
                    didClockwiseCheck = false;
                    checkingClockwise = !checkingClockwise;
                    checkingClockwiseTimer = 5;
                }
            }

            angle = 0;
            int count = 0;

            if (!foundUpDown)
            {
                do
                {
                    if (checkingClockwise)
                        angle += 45;
                    else
                        angle -= 45;

                    testMove = Quaternion.AngleAxis(angle, Vector3.up) * direction2d;
                    ObstacleCheck(testMove);
                    FallCheck(testMove);

                    // Break out of loop if can't find anywhere to go
                    count++;
                    if (count > 7)
                    {
                        break;
                    }
                }
                while (obstacleDetected || fallDetected);
            }

            detourDestination = transform.position + testMove * 2;

            if (avoidObstaclesTimer <= 0)
                avoidObstaclesTimer = 0.75f;
            lastTimeWasStuck = Time.time;
        }

        void ObstacleCheck(Vector3 direction)
        {
            obstacleDetected = false;
            const float checkDistance = 0.8f;
            foundUpwardSlope = false;
            foundDoor = false;

            RaycastHit hit;
            // Climbable/not climbable step for the player seems to be at around a height of 0.65f. The player is 1.8f tall.
            // Using the same ratio to height as these values, set the capsule for the enemy. 
            Vector3 p1 = transform.position + (Vector3.up * -originalHeight * 0.1388F);
            Vector3 p2 = p1 + (Vector3.up * Mathf.Min(originalHeight, 1.75f) / 2);

            if (Physics.CapsuleCast(p1, p2, controller.radius / 2, direction, out hit, checkDistance))
            {
                // Debug.DrawRay(transform.position, direction, Color.red, 2.0f);
                obstacleDetected = true;
                DaggerfallEntityBehaviour entityBehaviour2 = hit.transform.GetComponent<DaggerfallEntityBehaviour>();
                DaggerfallActionDoor door = hit.transform.GetComponent<DaggerfallActionDoor>();
                DaggerfallLoot loot = hit.transform.GetComponent<DaggerfallLoot>();

                if (entityBehaviour2)
                {
                    if (entityBehaviour2 == senses.Target)
                        obstacleDetected = false;
                }
                else if (door)
                {
                    obstacleDetected = false;
                    foundDoor = true;
                    if (senses.TargetIsWithinYawAngle(22.5f, door.transform.position))
                    {
                        senses.LastKnownDoor = door;
                        senses.DistanceToDoor = Vector3.Distance(transform.position, door.transform.position);
                    }
                }
                else if (loot)
                {
                    obstacleDetected = false;
                }
                else if (!swims && !flies && !IsLevitating)
                {
                    // If an obstacle was hit, check for a climbable upward slope
                    Vector3 checkUp = transform.position + direction;
                    checkUp.y++;

                    direction = (checkUp - transform.position).normalized;
                    p1 = transform.position + (Vector3.up * -originalHeight * 0.25f);
                    p2 = p1 + (Vector3.up * originalHeight * 0.75f);

                    if (!Physics.CapsuleCast(p1, p2, controller.radius / 2, direction, checkDistance))
                    {
                        obstacleDetected = false;
                        foundUpwardSlope = true;
                    }
                }
            }
            else
            {
                // Debug.DrawRay(transform.position, direction, Color.green, 2.0f);
            }
        }

        void FallCheck(Vector3 direction)
        {
            if (flies || IsLevitating || swims || obstacleDetected || foundUpwardSlope || foundDoor)
            {
                fallDetected = false;
                return;
            }

            int checkDistance = 1;
            Vector3 rayOrigin = transform.position;

            direction *= checkDistance;
            Ray ray = new Ray(rayOrigin + direction, Vector3.down);
            RaycastHit hit;

            fallDetected = !Physics.Raycast(ray, out hit, (originalHeight * 0.5f) + 1.5f);
        }

        /// <summary>
        /// Decide whether or not to pursue enemy, based on perceived combat odds.
        /// </summary>
        void EvaluateMoveInForAttack()
        {
            // Classic always attacks
            if (!DaggerfallUnity.Settings.EnhancedCombatAI)
            {
                moveInForAttack = true;
                return;
            }

            // No retreat from unseen opponent
            if (!senses.TargetInSight)
            {
                moveInForAttack = true;
                return;
            }

            // No retreat if enemy is paralyzed
            if (senses.Target != null)
            {
                EntityEffectManager targetEffectManager = senses.Target.GetComponent<EntityEffectManager>();
                if (targetEffectManager.FindIncumbentEffect<MagicAndEffects.MagicEffects.Paralyze>() != null)
                {
                    moveInForAttack = true;
                    return;
                }

                // No retreat if enemy's back is turned
                if (senses.TargetHasBackTurned())
                {
                    moveInForAttack = true;
                    return;
                }

                // No retreat if enemy is player with bow or weapon not out
                if (senses.Target == GameManager.Instance.PlayerEntityBehaviour
                    && GameManager.Instance.WeaponManager.ScreenWeapon
                    && (GameManager.Instance.WeaponManager.ScreenWeapon.WeaponType == WeaponTypes.Bow
                    || !GameManager.Instance.WeaponManager.ScreenWeapon.ShowWeapon))
                {
                    moveInForAttack = true;
                    return;
                }
            }
            else
            {
                return;
            }

            const float retreatDistanceBaseMult = 2.25f;

            // Level difference affects likelihood of backing away.
            moveInForAttackTimer = Random.Range(1, 3);
            int levelMod = (entity.Level - senses.Target.Entity.Level) / 2;
            if (levelMod > 4)
                levelMod = 4;
            if (levelMod < -4)
                levelMod = -4;

            int roll = Random.Range(0 + levelMod, 10 + levelMod);

            moveInForAttack = roll > 4;

            // Chose to retreat
            if (!moveInForAttack)
            {
                retreatDistanceMultiplier = (float)(retreatDistanceBaseMult + (retreatDistanceBaseMult * (0.25 * (2 - roll))));

                if (!DaggerfallUnity.Settings.EnhancedCombatAI)
                    return;

                if (Random.Range(0, 2) == 0)
                    strafeLeft = true;
                else
                    strafeLeft = false;

                Vector3 north = destination;
                north.z++; // Adding 1 to z so this Vector3 will be north of the destination Vector3.

                // Get angle between vector from destination to the north of it, and vector from destination to this enemy's position
                strafeAngle = Vector3.SignedAngle(destination - north, destination - transform.position, Vector3.up);
                if (strafeAngle < 0)
                    strafeAngle = 360 + strafeAngle;

                // Convert to radians
                strafeAngle *= Mathf.PI / 180;
            }
        }

        /// <summary>
        /// Set timer for padding between state changes, for non-perfect reflexes.
        /// </summary>
        void SetChangeStateTimer()
        {
            // No timer without enhanced AI
            if (!DaggerfallUnity.Settings.EnhancedCombatAI)
                return;

            if (changeStateTimer <= 0)
                changeStateTimer = Random.Range(0.2f, .8f);
        }

        /// <summary>
        /// Movement for water enemies.
        /// </summary>
        void WaterMove(Vector3 motion)
        {
            // Don't allow aquatic enemies to go above the water level of a dungeon block
            if (GameManager.Instance.PlayerEnterExit.blockWaterLevel != 10000
                    && controller.transform.position.y
                    < GameManager.Instance.PlayerEnterExit.blockWaterLevel * -1 * MeshReader.GlobalScale)
            {
                if (motion.y > 0 && controller.transform.position.y + (100 * MeshReader.GlobalScale)
                        >= GameManager.Instance.PlayerEnterExit.blockWaterLevel * -1 * MeshReader.GlobalScale)
                {
                    motion.y = 0;
                }
                controller.Move(motion * Time.deltaTime);
            }
        }

        /// <summary>
        /// Rotate toward target.
        /// </summary>
        void TurnToTarget(Vector3 targetDirection)
        {
            const float turnSpeed = 20f;
            //Classic speed is 11.25f, too slow for Daggerfall Unity's agile player movement

            if (GameManager.ClassicUpdate)
            {
                transform.forward = Vector3.RotateTowards(transform.forward, targetDirection, turnSpeed * Mathf.Deg2Rad, 0.0f);
            }
        }

        /// <summary>
        /// Set to either idle or move animation depending on whether the enemy has moved or rotated.
        /// </summary>
        void UpdateToIdleOrMoveAnim()
        {
            if (!mobile.IsPlayingOneShot())
            {
                // Rotation is done at classic update rate, so check at classic update rate
                if (GameManager.ClassicUpdate)
                {
                    Vector3 currentDirection = transform.forward;
                    currentDirection.y = 0;
                    rotating = lastDirection != currentDirection;
                    lastDirection = currentDirection;
                }
                // Movement is done at regular update rate, so check position at regular update rate
                if (!rotating && lastPosition == transform.position)
                    mobile.ChangeEnemyState(MobileStates.Idle);
                else
                    mobile.ChangeEnemyState(MobileStates.Move);
            }

            lastPosition = transform.position;
        }

        void ApplyFallDamage()
        {
            // Assuming the same formula is used for the player and enemies
            const float fallingDamageThreshold = 5.0f;
            const float HPPerMetre = 5f;

            if (controller.isGrounded)
            {
                // did enemy just land?
                if (falls)
                {
                    float fallDistance = lastGroundedY - transform.position.y;
                    if (fallDistance > fallingDamageThreshold)
                    {
                        int damage = (int)(HPPerMetre * (fallDistance - fallingDamageThreshold));

                        EnemyEntity enemyEntity = entityBehaviour.Entity as EnemyEntity;
                        enemyEntity.DecreaseHealth(damage);

                        if (entityBlood)
                        {
                            // Like in classic, falling enemies bleed at the center. It must hurt the center of mass ;)
                            entityBlood.ShowBloodSplash(0, transform.position);
                        }

                        DaggerfallUI.Instance.DaggerfallAudioSource.PlayClipAtPoint((int)SoundClips.FallDamage, FindGroundPosition());
                    }
                }

                lastGroundedY = transform.position.y;
            }
        }

        /// <summary>
        /// Open doors that are in the way.
        /// </summary>
        void OpenDoors()
        {
            // Try to open doors blocking way
            if (mobile.Summary.Enemy.CanOpenDoors)
            {
                if (senses.LastKnownDoor != null
                    && senses.DistanceToDoor < OpenDoorDistance && !senses.LastKnownDoor.IsOpen
                    && !senses.LastKnownDoor.IsLocked)
                {
                    senses.LastKnownDoor.ToggleDoor();
                    return;
                }

                // If door didn't open, and we are trying to get to the target, bash
                Bashing = DaggerfallUnity.Settings.EnhancedCombatAI && !senses.TargetInSight && moveInForAttack
                    && senses.LastKnownDoor != null && senses.DistanceToDoor <= attack.MeleeDistance && senses.LastKnownDoor.IsLocked;
            }
        }

        /// <summary>
        /// Limits maximum controller height.
        /// Tall sprites require this hack to get through doors.
        /// </summary>
        void HeightAdjust()
        {
            // If enemy bumps into something, temporarily reduce their height to 1.65, which should be short enough to fit through most if not all doorways.
            // Unfortunately, while the enemy is shortened, projectiles will not collide with the top of the enemy for the difference in height.
            if (!resetHeight && controller && ((controller.collisionFlags & CollisionFlags.CollidedSides) != 0) && originalHeight > 1.65f)
            {
                // Adjust the center of the controller so that sprite doesn't sink into the ground
                centerChange = (1.65f - controller.height) / 2;
                Vector3 newCenter = controller.center;
                newCenter.y += centerChange;
                controller.center = newCenter;
                // Adjust the height
                controller.height = 1.65f;
                resetHeight = true;
                heightChangeTimer = 0.5f;
            }
            else if (resetHeight && heightChangeTimer <= 0)
            {
                // Restore the original center
                Vector3 newCenter = controller.center;
                newCenter.y -= centerChange;
                controller.center = newCenter;
                // Restore the original height
                controller.height = originalHeight;
                resetHeight = false;
            }

            if (resetHeight && heightChangeTimer > 0)
            {
                heightChangeTimer -= Time.deltaTime;
            }
        }
        #endregion
    }
}
