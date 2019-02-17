// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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
        public float OpenDoorDistance = 2f;         // Maximum distance to open door

        const float AttackSpeedDivisor = 3f;        // How much to slow down during attack animations
        float stopDistance = 1.7f;                  // Used to prevent orbiting
        int giveUpTimer;                            // Timer before enemy gives up
        bool isHostile;                             // Is enemy hostile to player
        bool flies;                                 // The enemy can fly
        bool swims;                                 // The enemy can swim
        bool pausePursuit;                          // pause to wait for the player to come closer to ground
        bool isLevitating;                          // Allow non-flying enemy to levitate
        float knockBackSpeed;                       // While non-zero, this enemy will be knocked backwards at this speed
        Vector3 knockBackDirection;                 // Direction to travel while being knocked back
        float moveInForAttackTimer;                 // Time until next pursue/retreat decision
        bool moveInForAttack;                       // False = retreat. True = pursue.
        float retreatDistanceMultiplier;            // How far to back off while retreating
        float changeStateTimer;                     // Time until next change in behavior. Padding to prevent instant reflexes.
        bool pursuing;                              // Is pursuing
        bool retreating;                            // Is retreating
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
        bool bashing;
        bool hasBowAttack;
        bool hasSpells;

        EnemySenses senses;
        Vector3 destination;
        Vector3 detourDestination;
        CharacterController controller;
        DaggerfallMobileUnit mobile;
        DaggerfallEntityBehaviour entityBehaviour;
        EntityEffectManager entityEffectManager;
        EntityEffectBundle selectedSpell;
        EnemyAttack attack;
        EnemyEntity entity;

        public bool IsLevitating
        {
            get { return isLevitating; }
            set { isLevitating = value; }
        }

        public bool IsHostile
        {
            get { return isHostile; }
            set { isHostile = value; }
        }

        public float KnockBackSpeed
        {
            get { return knockBackSpeed; }
            set { knockBackSpeed = value; }
        }

        public Vector3 KnockBackDirection
        {
            get { return knockBackDirection; }
            set { knockBackDirection = value; }
        }

        public bool Bashing
        {
            get { return bashing; }
        }

        void Start()
        {
            senses = GetComponent<EnemySenses>();
            controller = GetComponent<CharacterController>();
            mobile = GetComponentInChildren<DaggerfallMobileUnit>();
            isHostile = mobile.Summary.Enemy.Reactions == MobileReactions.Hostile;
            flies = mobile.Summary.Enemy.Behaviour == MobileBehaviour.Flying ||
                    mobile.Summary.Enemy.Behaviour == MobileBehaviour.Spectral;
            swims = mobile.Summary.Enemy.Behaviour == MobileBehaviour.Aquatic;
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            entityEffectManager = GetComponent<EntityEffectManager>();
            entity = entityBehaviour.Entity as EnemyEntity;
            attack = GetComponent<EnemyAttack>();

            // Classic AI moves only as close as melee range
            if (!DaggerfallUnity.Settings.EnhancedCombatAI)
                stopDistance = attack.MeleeDistance;

            // Only need to check for ability to shoot bow once, and if no spells, only need to check for spells once.
            hasBowAttack = mobile.Summary.Enemy.HasRangedAttack1 && mobile.Summary.Enemy.ID > 129 && mobile.Summary.Enemy.ID != 132;
            hasSpells = entity.GetSpells().Length > 0;
        }

        void FixedUpdate()
        {
            Move();
            OpenDoors();
        }

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
                senses.LastTarget = senses.Target;
                senses.OldLastKnownTargetPos = attacker.transform.position;
                senses.LastKnownTargetPos = attacker.transform.position;
                senses.PredictedTargetPos = attacker.transform.position;
                giveUpTimer = 200;
            }

            if (attacker == GameManager.Instance.PlayerEntityBehaviour)
                isHostile = true;

            bashing = false;
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
        /// Make decision about what movement action to take.
        /// </summary>
        void Move()
        {
            // Cancel movement and animations if paralyzed, but still allow gravity to take effect
            // This will have the (intentional for now) side-effect of making paralyzed flying enemies fall out of the air
            // Paralyzed swimming enemies will just freeze in place
            // Freezing anims also prevents the attack from triggering until paralysis cleared
            if (entityBehaviour.Entity.IsParalyzed)
            {
                mobile.FreezeAnims = true;

                if ((swims || flies) && !isLevitating)
                    controller.Move(Vector3.zero);
                else
                    controller.SimpleMove(Vector3.zero);

                return;
            }
            mobile.FreezeAnims = false;

            // If hit, get knocked back
            if (knockBackSpeed > 0)
            {
                // Limit knockBackSpeed. This can be higher than what is actually used for the speed of motion,
                // making it last longer and do more damage if the enemy collides with something (TODO).
                if (knockBackSpeed > (40 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)))
                    knockBackSpeed = (40 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10));

                if (knockBackSpeed > (5 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)) &&
                    mobile.Summary.EnemyState != MobileStates.PrimaryAttack)
                {
                    mobile.ChangeEnemyState(MobileStates.Hurt);
                }

                // Actual speed of motion is limited
                Vector3 motion;
                if (knockBackSpeed <= (25 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)))
                    motion = knockBackDirection * knockBackSpeed;
                else
                    motion = knockBackDirection * (25 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10));

                // Move in direction of knockback
                if (swims)
                    WaterMove(motion);
                else if (flies || isLevitating)
                    controller.Move(motion * Time.deltaTime);
                else
                    controller.SimpleMove(motion);

                // Remove remaining knockback and restore animation
                if (GameManager.ClassicUpdate)
                {
                    knockBackSpeed -= (5 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10));
                    if (knockBackSpeed <= (5 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10))
                        && mobile.Summary.EnemyState != MobileStates.PrimaryAttack)
                    {
                        mobile.ChangeEnemyState(MobileStates.Move);
                    }
                }

                // If a decent hit got in, reconsider whether to continue current tactic
                if (knockBackSpeed > (10 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)))
                {
                    EvaluateMoveInForAttack();
                }

                return;
            }

            // Apply gravity
            if (!flies && !swims && !isLevitating && !controller.isGrounded)
            {
                controller.SimpleMove(Vector3.zero);

                // Only return if actually falling. Sometimes mobiles can get stuck where they are !isGrounded but SimpleMove(Vector3.zero) doesn't help.
                // Allowing them to continue and attempt a Move() in the code below frees them, but we don't want to allow that if we can avoid it so they aren't moving
                // while falling, which can also accelerate the fall due to anti-bounce downward movement in Move().
                if (lastPosition != transform.position)
                    return;
            }

            // Monster speed of movement follows the same formula as for when the player walks
            float moveSpeed = (entity.Stats.LiveSpeed + PlayerSpeedChanger.dfWalkBase) * MeshReader.GlobalScale;

            // Get isPlayingOneShot for use below
            bool isPlayingOneShot = mobile.IsPlayingOneShot();

            // Reduced speed if playing a one-shot animation with enhanced AI
            if (isPlayingOneShot && DaggerfallUnity.Settings.EnhancedCombatAI)
                moveSpeed /= AttackSpeedDivisor;

            // As long as the target is detected,
            // giveUpTimer is reset to full
            if (senses.DetectedTarget)
                giveUpTimer = 200;

            // GiveUpTimer value is from classic, so decrease at the speed of classic's update loop
            if (GameManager.ClassicUpdate && !senses.DetectedTarget && giveUpTimer > 0)
                giveUpTimer--;

            // Change to idle animation if haven't moved or rotated
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

                lastPosition = transform.position;
            }

            // Do nothing if no target or after giving up finding the target or if target position hasn't been acquired yet
            if (senses.Target == null || giveUpTimer == 0 || senses.PredictedTargetPos == EnemySenses.ResetPlayerPos)
            {
                SetChangeStateTimer();
                bashing = false;

                return;
            }

            if (bashing)
            {
                if (senses.TargetInSight || senses.LastKnownDoor == null || !senses.LastKnownDoor.IsLocked)
                    bashing = false;
                else
                {
                    int speed = entity.Stats.LiveSpeed;
                    if (GameManager.ClassicUpdate && DFRandom.rand() % speed >= (speed >> 3) + 6 && attack.MeleeTimer == 0)
                    {
                        mobile.ChangeEnemyState(MobileStates.PrimaryAttack);
                        attack.ResetMeleeTimer();
                    }

                    return;
                }
            }

            bool clearPathToShootAtPredictedPos = false;

            // Get location to move towards. Either the combat target's position or, if trying to avoid an obstacle or fall,
            // a location to try to detour around the obstacle/fall.
            if (avoidObstaclesTimer == 0 && ClearPathToPosition(senses.PredictedTargetPos))
            {
                destination = senses.PredictedTargetPos;
                // Flying enemies and slaughterfish aim for target face
                if (flies || isLevitating || (swims && mobile.Summary.Enemy.ID == (int)MonsterCareers.Slaughterfish))
                    destination.y += 0.9f;
                else
                {
                    // Ground enemies target at their own height
                    // This avoids short enemies from stepping on each other as they approach the target
                    // Otherwise, their target vector aims up towards the target
                    var targetController = senses.Target.GetComponent<CharacterController>();
                    var deltaHeight = (targetController.height - controller.height) / 2;
                    destination.y -= deltaHeight;
                }

                clearPathToShootAtPredictedPos = true;
            }
            else if (DaggerfallUnity.Settings.EnhancedCombatAI && avoidObstaclesTimer == 0 && ClearPathToPosition(senses.LastKnownTargetPos))
            {
                destination = senses.LastKnownTargetPos + (senses.LastPositionDiff * 2);
            }
            // If detouring, use the detour position
            else if (avoidObstaclesTimer > 0)
            {
                destination = detourDestination;
            }
            // Otherwise, go straight
            else
            {
                destination = transform.position + transform.forward * (stopDistance + .3f); // Move position needs to be a little further away (adding .3f) than stop distance so that AI will move, not stop
            }

            // Get direction & distance.
            var direction = (destination - transform.position).normalized;
            float distance = (destination - transform.position).magnitude;

            // Ranged attacks
            if ((hasBowAttack || hasSpells) && clearPathToShootAtPredictedPos && senses.TargetInSight && senses.DetectedTarget && 360 * MeshReader.GlobalScale < senses.DistanceToTarget && senses.DistanceToTarget < 2048 * MeshReader.GlobalScale)
            {
                bool rangedMagicAvailable = false;
                if (!hasBowAttack && hasSpells)
                    rangedMagicAvailable = CanCastRangedSpell();

                if (hasBowAttack || rangedMagicAvailable)
                {
                    if (GameManager.ClassicUpdate && senses.TargetIsWithinYawAngle(22.5f, senses.LastKnownTargetPos))
                    {
                        if (!isPlayingOneShot)
                        {
                            if (hasBowAttack)
                            {
                                // Random chance to shoot bow
                                if (DFRandom.rand() < 1000)
                                {
                                    if (mobile.Summary.Enemy.HasRangedAttack1 && !mobile.Summary.Enemy.HasRangedAttack2)
                                        mobile.ChangeEnemyState(MobileStates.RangedAttack1);
                                    else if (mobile.Summary.Enemy.HasRangedAttack2)
                                        mobile.ChangeEnemyState(MobileStates.RangedAttack2);
                                }
                            }
                            // Random chance to shoot spell
                            else if (DFRandom.rand() % 40 == 0
                                 && entityEffectManager.SetReadySpell(selectedSpell))
                            {
                                mobile.ChangeEnemyState(MobileStates.Spell);
                            }
                        }
                    }
                    else
                        TurnToTarget(direction);

                    return;
                }
            }

            // Touch spells
            if (senses.TargetInSight && senses.DetectedTarget && attack.MeleeTimer == 0 && senses.DistanceToTarget <= attack.MeleeDistance
                + senses.TargetRateOfApproach && CanCastTouchSpell() && entityEffectManager.SetReadySpell(selectedSpell))
            {
                if (mobile.Summary.EnemyState != MobileStates.Spell)
                    mobile.ChangeEnemyState(MobileStates.Spell);

                attack.ResetMeleeTimer();
                return;
            }

            // Update advance/retreat decision
            if (moveInForAttackTimer <= 0 && avoidObstaclesTimer == 0)
                EvaluateMoveInForAttack();

            // Update timers
            if (moveInForAttackTimer > 0)
                moveInForAttackTimer -= Time.deltaTime;

            if (avoidObstaclesTimer > 0 && senses.TargetIsWithinYawAngle(5.625f, destination))
                avoidObstaclesTimer -= Time.deltaTime;
            if (avoidObstaclesTimer < 0)
                avoidObstaclesTimer = 0;

            if (checkingClockwiseTimer > 0)
                checkingClockwiseTimer -= Time.deltaTime;
            if (checkingClockwiseTimer < 0)
            {
                checkingClockwiseTimer = 0;
            }

            if (changeStateTimer > 0)
                changeStateTimer -= Time.deltaTime;

            // If detouring, attempt to move
            if (avoidObstaclesTimer > 0)
            {
                AttemptMove(direction, moveSpeed);
            }
            // Otherwise, if not still executing a retreat, approach target until close enough to be on-guard.
            // If decided to move in for attack, continue until within melee range. Classic always moves in for attack.
            else if ((!retreating && distance >= (stopDistance * 2.75))
                    || (distance > stopDistance && moveInForAttack))
            {
                // If state change timer is done, or we are continuing an already started combatMove, we can move immediately
                if (changeStateTimer <= 0 || pursuing)
                    AttemptMove(direction, moveSpeed);
                // Otherwise, look at target until timer finishes
                else if (!senses.TargetIsWithinYawAngle(22.5f, destination))
                    TurnToTarget(direction);
            }
            // Back away from combat target if right next to it, or if decided to retreat and enemy is too close.
            // Classic AI never backs awwy.
            else if (DaggerfallUnity.Settings.EnhancedCombatAI && senses.TargetInSight && (distance < stopDistance * .8f ||
                (!moveInForAttack && distance < (stopDistance * retreatDistanceMultiplier))))
            {
                // If state change timer is done, or we are already executing a retreat, we can move immediately
                if (changeStateTimer <= 0 || retreating)
                    AttemptMove(direction, moveSpeed / 2, true);
                // Otherwise, look at target until timer finishes
                else if (!senses.TargetIsWithinYawAngle(22.5f, destination))
                    TurnToTarget(direction);
            }
            // Not moving, just look at target
            else if (!senses.TargetIsWithinYawAngle(22.5f, destination))
                TurnToTarget(direction);
            else // Not moving, and no need to turn
            {
                SetChangeStateTimer();
                pursuing = false;
                retreating = false;
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
            RayCheckForObstacle(sphereCastDir2d);
            RayCheckForFall(sphereCastDir2d);

            if (obstacleDetected || fallDetected)
                return false;

            RaycastHit hit;
            if (Physics.SphereCast(transform.position, controller.radius / 2, sphereCastDir, out hit, dist))
            {
                DaggerfallEntityBehaviour hitTarget = hit.transform.GetComponent<DaggerfallEntityBehaviour>();
                if (hitTarget == senses.Target)
                    return true;

                return false;
            }

            return true;
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

            int totalGoldCostUnused;
            int readySpellCastingCost;

            Formulas.FormulaHelper.CalculateTotalEffectCosts(selectedSpell.Settings.Effects, selectedSpell.Settings.TargetType, out totalGoldCostUnused, out readySpellCastingCost);
            if (entity.CurrentMagicka < readySpellCastingCost)
                return false;

            if (EffectsAlreadyOnTarget(selectedSpell))
                return false;

            // Check that there is a clear path to shoot a spell
            float spellMovementSpeed = 25; // All range spells are currently 25 speed
            Vector3 sphereCastDir = senses.PredictNextTargetPos(spellMovementSpeed);
            if (sphereCastDir == EnemySenses.ResetPlayerPos)
                return false;

            float sphereCastDist = (sphereCastDir - transform.position).magnitude;
            sphereCastDir = (sphereCastDir - transform.position).normalized;

            RaycastHit hit;
            if (Physics.SphereCast(transform.position, 0.45f, sphereCastDir, out hit, sphereCastDist))
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

            int totalGoldCostUnused;
            int readySpellCastingCost;

            Formulas.FormulaHelper.CalculateTotalEffectCosts(selectedSpell.Settings.Effects, selectedSpell.Settings.TargetType, out totalGoldCostUnused, out readySpellCastingCost);
            if (entity.CurrentMagicka < readySpellCastingCost)
                return false;

            if (EffectsAlreadyOnTarget(selectedSpell))
                return false;

            return true;
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
        /// Try to move in given direction
        /// </summary>
        void AttemptMove(Vector3 direction, float moveSpeed, bool backAway = false)
        {
            // Set whether pursuing or retreating, for bypassing changeStateTimer delay when continuing these actions
            if (!backAway)
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
                TurnToTarget(direction.normalized);
                // Classic always turns in place. Enhanced only does so if enemy is not in sight,
                // for more natural-looking movement while pursuing.
                if (!DaggerfallUnity.Settings.EnhancedCombatAI || !senses.TargetInSight)
                    return;
            }

            if (backAway)
                direction *= -1;

            // Move downward some to eliminate bouncing down inclines
            if (!flies && !swims && !isLevitating && controller.isGrounded)
                direction.y = -2f;

            Vector3 motion = direction * moveSpeed;

            // If using enhanced combat, avoid moving directly below targets
            if (!backAway && DaggerfallUnity.Settings.EnhancedCombatAI && avoidObstaclesTimer == 0)
            {
                bool withinPitch = senses.TargetIsWithinPitchAngle(45.0f);
                if (!pausePursuit && !withinPitch)
                {
                    if (flies || isLevitating || swims)
                    {
                        if (!senses.TargetIsAbove())
                            motion = -transform.up * moveSpeed;
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
                else if (pausePursuit && withinPitch)
                    pausePursuit = false;

                if (pausePursuit)
                {
                    if (senses.TargetIsAbove() && !senses.TargetIsWithinPitchAngle(55.0f) && changeStateTimer <= 0)
                    {
                        // Back away from target
                        motion = -transform.forward * moveSpeed * 0.75f;
                    }
                    else
                    {
                        // Stop moving
                        return;
                    }
                }
            }

            SetChangeStateTimer();

            // Check if there is something to collide with directly in movement direction, such as upward sloping ground.
            Vector3 motion2d = motion.normalized;
            motion2d.y = 0;
            RayCheckForObstacle(motion2d);
            RayCheckForFall(motion2d);

            if (fallDetected || obstacleDetected)
            {
                FindDetour(motion2d);
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

        void FindDetour(Vector3 motion)
        {
            Vector3 motion2d = motion;
            motion2d.y = 0;
            float angle;
            Vector3 testMove;

            // Reset clockwise check if we've been clear of obstacles/falls for a while
            if (Time.time - lastTimeWasStuck > 2f)
            {
                checkingClockwiseTimer = 0;
                didClockwiseCheck = false;
            }

            if (checkingClockwiseTimer == 0)
            {
                if (!didClockwiseCheck)
                {
                    // Check 45 degrees in both ways first
                    // Pick first direction to check randomly
                    if (Random.Range(0, 1) == 0)
                        angle = 45;
                    else
                        angle = -45;

                    testMove = Quaternion.AngleAxis(angle, Vector3.up) * motion2d;
                    RayCheckForObstacle(testMove);
                    RayCheckForFall(testMove);

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
                        testMove = Quaternion.AngleAxis(angle, Vector3.up) * motion2d;
                        RayCheckForObstacle(testMove);
                        RayCheckForFall(testMove);

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
                            angle = Vector3.SignedAngle(directionToTarget, motion2d, Vector3.up);

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
            testMove = Vector3.zero;

            do
            {
                if (checkingClockwise)
                    angle += 10;
                else
                    angle -= 10;

                testMove = Quaternion.AngleAxis(angle, Vector3.up) * motion2d;
                RayCheckForObstacle(testMove);
                RayCheckForFall(testMove);

                // Break out of loop if can't find anywhere to go
                count++;
                if (count > 36)
                {
                    break;
                }
            }
            while (obstacleDetected || fallDetected);

            detourDestination = transform.position + testMove.normalized * 2;
            detourDestination.y = transform.position.y;

            if (avoidObstaclesTimer == 0)
                avoidObstaclesTimer = 0.25f;
            lastTimeWasStuck = Time.time;
        }

        void RayCheckForObstacle(Vector3 direction)
        {
            obstacleDetected = false;
            RaycastHit hit;
            int checkDistance = 1;
            Vector3 rayOrigin = transform.position + controller.center;
            rayOrigin.y -= controller.height / 3;
            foundUpwardSlope = false;
            foundDoor = false;

            Ray ray = new Ray(rayOrigin, direction);
            if (Physics.Raycast(ray, out hit, checkDistance))
            {
                obstacleDetected = true;
                float firstDistance = hit.distance;

                rayOrigin.y += 0.5f;
                ray = new Ray(rayOrigin, direction);
                RaycastHit hit2;
                bool secondRayHit = Physics.Raycast(ray, out hit2, checkDistance);

                if (!secondRayHit || firstDistance < hit2.distance)
                {
                    obstacleDetected = false;
                    foundUpwardSlope = true;
                }

                DaggerfallEntityBehaviour entityBehaviour2 = hit.transform.GetComponent<DaggerfallEntityBehaviour>();
                if (entityBehaviour2 == senses.Target)
                    obstacleDetected = false;

                DaggerfallActionDoor door = hit.transform.GetComponent<DaggerfallActionDoor>();
                if (door)
                {
                    obstacleDetected = false;
                    foundDoor = true;
                    if (senses.TargetIsWithinYawAngle(22.5f, door.transform.position))
                    {
                        senses.LastKnownDoor = door;
                        senses.DistanceToDoor = Vector3.Distance(transform.position, door.transform.position);
                    }
                }

                DaggerfallLoot loot = hit.transform.GetComponent<DaggerfallLoot>();
                if (loot)
                    obstacleDetected = false;
            }
        }

        void RayCheckForFall(Vector3 direction)
        {
            if (flies || isLevitating || swims || obstacleDetected || foundUpwardSlope || foundDoor)
            {
                fallDetected = false;
                return;
            }

            int checkDistance = 1;
            Vector3 rayOrigin = transform.position;

            direction *= checkDistance;
            Ray ray = new Ray(rayOrigin + direction, Vector3.down);
            RaycastHit hit;

            fallDetected = !Physics.Raycast(ray, out hit, 5);
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
                retreatDistanceMultiplier = (float)(retreatDistanceBaseMult + (retreatDistanceBaseMult * (0.25 * (2 - roll))));
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
        /// Open doors that are in the way.
        /// </summary>
        void OpenDoors()
        {
            // Try to open doors blocking way
            if (mobile.Summary.Enemy.CanOpenDoors && senses.LastKnownDoor != null
                && senses.DistanceToDoor < OpenDoorDistance && !senses.LastKnownDoor.IsOpen
                && !senses.LastKnownDoor.IsLocked)
            {
                senses.LastKnownDoor.ToggleDoor();
            }

            // If door didn't open, and we are trying to get to the target, bash
            if (DaggerfallUnity.Settings.EnhancedCombatAI && moveInForAttack && senses.LastKnownDoor != null
                && senses.DistanceToDoor < attack.MeleeDistance && senses.LastKnownDoor.IsLocked)
                bashing = true;
        }

        #endregion
    }
}
