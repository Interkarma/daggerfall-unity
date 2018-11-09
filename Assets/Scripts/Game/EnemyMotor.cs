// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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
using DaggerfallWorkshop.Game.MagicAndEffects;
using System.Collections.Generic;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Example enemy motor.
    /// </summary>
    [RequireComponent(typeof(EnemySenses))]
    [RequireComponent(typeof(EnemyAttack))]
    [RequireComponent(typeof(EnemyBlood))]
    [RequireComponent(typeof(EnemySounds))]
    [RequireComponent(typeof(CharacterController))]
    public class EnemyMotor : MonoBehaviour
    {
        public float OpenDoorDistance = 2f;             // Maximum distance to open door
        public const float AttackSpeedDivisor = 3f;     // How much to slow down during attack animations

        EnemySenses senses;
        Vector3 targetPos;
        CharacterController controller;
        DaggerfallMobileUnit mobile;
        DaggerfallEntityBehaviour entityBehaviour;
        EntityEffectManager entityEffectManager;
        EntityEffectBundle selectedSpell;
        EnemyAttack attack;
        EnemyEntity entity;

        float stopDistance = 1.7f;                  // Used to prevent orbiting
        float giveUpTimer;                          // Timer before enemy gives up
        bool isHostile;                             // Is enemy hostile to player
        bool flies;                                 // The enemy can fly
        bool swims;                                 // The enemy can swim
        bool pausePursuit;                          // pause to wait for the player to come closer to ground
        int enemyLayerMask;                         // Layer mask for Enemies to optimize collision checks

        bool isLevitating;                          // Allow non-flying enemy to levitate

        bool isAttackFollowsPlayerSet;              // For setting if the enemy will follow the player or not during an attack
        bool attackFollowsPlayer;                   // For setting if the enemy will follow the player or not during an attack

        float classicUpdateTimer;
        bool classicUpdate;
        float knockBackSpeed;                       // While non-zero, this enemy will be knocked backwards at this speed
        Vector3 knockBackDirection;                 // Direction to travel while being knocked back

        float pursueDecisionTimer;                   // Time until next pursue/retreat decision
        bool pursueDecision;                         // False = retreat. True = pursue.
        float retreatDistanceMultiplier;            // How far to back off while retreating
        float changeStateTimer;                     // Time until next change in behavior. Padding to prevent instant reflexes.
        bool pursuing;                              // Is pursuing
        bool retreating;                            // Is retreating
        bool fallDetected;                          // Detected a fall in front of us, so don't move there

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

        void Start()
        {
            senses = GetComponent<EnemySenses>();
            controller = GetComponent<CharacterController>();
            mobile = GetComponentInChildren<DaggerfallMobileUnit>();
            isHostile = mobile.Summary.Enemy.Reactions == MobileReactions.Hostile;
            flies = mobile.Summary.Enemy.Behaviour == MobileBehaviour.Flying ||
                    mobile.Summary.Enemy.Behaviour == MobileBehaviour.Spectral;
            swims = mobile.Summary.Enemy.Behaviour == MobileBehaviour.Aquatic;
            enemyLayerMask = LayerMask.GetMask("Enemies");
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            entityEffectManager = GetComponent<EntityEffectManager>();
            entity = entityBehaviour.Entity as EnemyEntity;
            attack = GetComponent<EnemyAttack>();
            isAttackFollowsPlayerSet = false;

            // Classic AI moves only as close as melee range
            if (!DaggerfallUnity.Settings.EnhancedCombatAI)
                stopDistance = attack.MeleeDistance;
        }

        void Update()
        {
        }

        void FixedUpdate()
        {
            classicUpdateTimer += Time.deltaTime;
            if (classicUpdateTimer >= PlayerEntity.ClassicUpdateInterval)
            {
                classicUpdateTimer = 0;
                classicUpdate = true;
            }
            else
                classicUpdate = false;

            Move();
            OpenDoors();
        }

        /// <summary>
        /// Immediately become hostile towards attacker and know attacker's location.
        /// </summary>
        public void MakeEnemyHostileToAttacker(DaggerfallEntityBehaviour attacker)
        {
            if (attacker && senses)
            {
                // Assign target if don't already have target, or original target isn't seen or adjacent
                if (entityBehaviour.Target == null || !senses.TargetInSight || senses.DistanceToTarget > 2f)
                    entityBehaviour.Target = attacker;
                senses.LastKnownTargetPos = attacker.transform.position;
                giveUpTimer = 200;
            }
            if (attacker == GameManager.Instance.PlayerEntityBehaviour)
                isHostile = true;
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

        #region Private Methods

        /// <summary>
        /// Prevent Rat Stacks by checking for collisions with other enemies along the planned motion path.
        ///
        /// Since enemies are moving capsule bodies, if one collides with a shorter enemy (like a rat), it will "roll"
        /// over the shorter enemy, causing stacks of enemies.
        ///
        /// This is a very simple path planning approach. One might use NavMeshes and NavMeshAgents to compute this
        /// automatically, but I didn't want to introduce that level of complexity early on during development, when
        /// things can easily change.
        /// </summary>
        /// <param name="plannedMotion">Path to check for collisions. This will be updated if a collision is found.</param>
        void AvoidEnemies(ref Vector3 plannedMotion)
        {
            // Compute the capsule start/end points for the casting operation
            var capsuleStart = transform.position;
            var capsuleEnd = transform.position;
            capsuleStart.y += controller.height / 2 - controller.radius;
            capsuleEnd.y -= controller.height / 2 + controller.radius;

            // We capsule cast because a ray might grace the edge of an enemy and allow it to move across & over
            // We use cast all to detect a collision at the start of the cast as well
            // To optimize, cast only in enemy layer and don't cause triggers to fire
            var hits = Physics.CapsuleCastAll(capsuleStart, capsuleEnd, controller.radius, plannedMotion,
                controller.radius * 2, enemyLayerMask, QueryTriggerInteraction.Ignore);

            // Note: CapsuleCastAll doesn't know about the "source", so "this" enemy will always count as a collision.
            if (hits.Length <= 1)
                return;

            // Simplest approach: Stop moving.
            plannedMotion *= 0;

            if (mobile.Summary.EnemyState == MobileStates.Move && DaggerfallUnity.Settings.EnhancedCombatAI)
                mobile.ChangeEnemyState(MobileStates.Idle);
            {
                SetChangeStateTimer();
            }

            // Slightly better approach: Route around.
            // This isn't perfect. In some cases enemies may still stack. It seems to happen when enemies are very close.
            // Always choose one direction. If this is random, the enemy will wiggle behind the other enemy because it's
            // computed so frequently. We could choose a direction at a lower rate to still give some randomness.
            // plannedMotion = Quaternion.Euler(0, 90, 0) * plannedMotion;
        }

        private void Move()
        {
            // Cancel movement and animations if paralyzed, but still allow gravity to take effect
            // This will have the (intentional for now) side-effect of making paralyzed flying enemies fall out of the air
            // Paralyzed swimming enemies will just freeze in place
            // Freezing anims also prevents the attack from triggering until paralysis cleared
            if (entityBehaviour.Entity.IsParalyzed)
            {
                mobile.FreezeAnims = true;

                if (swims)
                    controller.Move(Vector3.zero);
                else
                    controller.SimpleMove(Vector3.zero);

                return;
            }
            else
            {
                mobile.FreezeAnims = false;
            }

            // Apply gravity to non-moving AI if active (has a combat target)
            if (entityBehaviour.Target != null && !flies && !swims && mobile.Summary.EnemyState != MobileStates.Move &&
                mobile.Summary.EnemyState != MobileStates.Hurt)
            {
                controller.SimpleMove(Vector3.zero);
            }

            // If hit, get knocked back
            if (knockBackSpeed > 0)
            {
                // Limit knockBackSpeed. This can be higher than what is actually used for the speed of motion,
                // making it last longer and do more damage if the enemy collides with something.
                if (knockBackSpeed > (40 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)))
                    knockBackSpeed = (40 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10));

                if (knockBackSpeed > (5 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)) &&
                    mobile.Summary.EnemyState != MobileStates.PrimaryAttack)
                    mobile.ChangeEnemyState(MobileStates.Hurt);

                // Actual speed of motion is limited
                Vector3 motion;
                if (knockBackSpeed <= (25 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)))
                    motion = knockBackDirection * knockBackSpeed;
                else
                    motion = knockBackDirection * (25 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10));

                if (swims)
                {
                    WaterMove(motion);
                }
                else if (flies || isLevitating)
                    controller.Move(motion * Time.deltaTime);
                else
                    controller.SimpleMove(motion);

                if (classicUpdate)
                {
                    knockBackSpeed -= (5 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10));
                    if (knockBackSpeed <= (5 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)) &&
                        mobile.Summary.EnemyState != MobileStates.PrimaryAttack)
                        mobile.ChangeEnemyState(MobileStates.Move);
                }

                // If a decent hit got in, reconsider whether to continue current tactic
                if (knockBackSpeed > (10 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)))
                {
                    EvaluatepursueDecision();
                }

                return;
            }

            // Monster speed of movement follows the same formula as for when the player walks
            float moveSpeed = ((entity.Stats.LiveSpeed + PlayerSpeedChanger.dfWalkBase) / PlayerSpeedChanger.classicToUnitySpeedUnitRatio);

            // Reduced speed if playing a one-shot animation with enhanced AI
            if (mobile.IsPlayingOneShot() && DaggerfallUnity.Settings.EnhancedCombatAI)
                moveSpeed /= AttackSpeedDivisor;

            // As long as the target is detected,
            // giveUpTimer is reset to full
            if (senses.DetectedTarget)
                giveUpTimer = 200;

            // GiveUpTimer value is from classic, so decrease at the speed of classic's update loop
            if (!senses.DetectedTarget
                && giveUpTimer > 0 && classicUpdate)
                giveUpTimer--;

            // Enemy will keep moving towards last known target position
            targetPos = senses.LastKnownTargetPos;

            // Remain idle after finishing any attacks if no target or after giving up finding the target
            if (entityBehaviour.Target == null || giveUpTimer == 0 || targetPos == EnemySenses.ResetPlayerPos)
            {
                if (mobile.Summary.EnemyState == MobileStates.Move)
                    mobile.ChangeEnemyState(MobileStates.Idle);
                SetChangeStateTimer();

                return;
            }

            // Flying enemies and slaughterfish aim for target face
            if (flies || isLevitating || (swims && mobile.Summary.Enemy.ID == (int)MonsterCareers.Slaughterfish))
                targetPos.y += 0.9f;
            else
            {
                // Ground enemies target at their own height
                // This avoids short enemies from stepping on each other as they approach the target
                // Otherwise, their target vector aims up towards the target
                var playerController = GameManager.Instance.PlayerController;
                var deltaHeight = (playerController.height - controller.height) / 2;
                targetPos.y -= deltaHeight;
            }

            // Get direction & distance.
            var direction = targetPos - transform.position;
            float distance = direction.magnitude;

            // If attacking, randomly follow target with attack.
            if (mobile.Summary.EnemyState == MobileStates.PrimaryAttack)
            {
                if (!isAttackFollowsPlayerSet)
                {
                    attackFollowsPlayer = (Random.Range(0f, 1f) > 0.5f);
                    isAttackFollowsPlayerSet = true;
                }
            }
            else
                isAttackFollowsPlayerSet = false;

            // Classic AI attack always follows player
            if (!DaggerfallUnity.Settings.EnhancedCombatAI || attackFollowsPlayer)
                transform.forward = direction.normalized;

            // Ranged attacks
            if (senses.TargetInSight && 360 * MeshReader.GlobalScale < distance && distance < 2048 * MeshReader.GlobalScale)
            {
                bool evaluateBow = (mobile.Summary.Enemy.HasRangedAttack1 && mobile.Summary.Enemy.ID > 129 && mobile.Summary.Enemy.ID != 132);
                bool evaluateRangedMagic = false;
                if (!evaluateBow)
                    evaluateRangedMagic = (CanCastRangedSpell(entity));

                if (evaluateBow || evaluateRangedMagic)
                {
                    if (senses.TargetIsWithinYawAngle(22.5f))
                    {
                        if (!mobile.IsPlayingOneShot())
                        {
                            if (evaluateBow)
                            {
                                // Random chance to shoot bow
                                if (classicUpdate && DFRandom.rand() < 1000)
                                {
                                    if (mobile.Summary.Enemy.HasRangedAttack1 && !mobile.Summary.Enemy.HasRangedAttack2)
                                        mobile.ChangeEnemyState(MobileStates.RangedAttack1);
                                    else if (mobile.Summary.Enemy.HasRangedAttack2)
                                        mobile.ChangeEnemyState(MobileStates.RangedAttack2);
                                }
                                // Otherwise hold ground
                                else
                                {
                                    if (mobile.Summary.EnemyState == MobileStates.Move)
                                        mobile.ChangeEnemyState(MobileStates.Idle);
                                }
                            }
                            // Random chance to shoot spell
                            else if (classicUpdate && DFRandom.rand() % 40 == 0
                                && entityEffectManager.SetReadySpell(selectedSpell))
                            {
                                mobile.ChangeEnemyState(MobileStates.Spell);
                            }
                            // Otherwise hold ground
                            else
                            {
                                if (mobile.Summary.EnemyState == MobileStates.Move)
                                    mobile.ChangeEnemyState(MobileStates.Idle);
                            }
                        }
                    }
                    else
                    {
                        if (!mobile.IsPlayingOneShot())
                            mobile.ChangeEnemyState(MobileStates.Move);
                        TurnToTarget(direction.normalized);
                    }

                    return;
                }
            }

            if (senses.TargetInSight && attack.MeleeTimer == 0 && senses.DistanceToTarget <= attack.MeleeDistance +
                senses.TargetRateOfApproach && CanCastTouchSpell(entity) && entityEffectManager.SetReadySpell(selectedSpell))
            {
                if (mobile.Summary.EnemyState != MobileStates.Spell)
                    mobile.ChangeEnemyState(MobileStates.Spell);

                attack.MeleeTimer = Random.Range(1500, 3001);
                attack.MeleeTimer -= 50 * (GameManager.Instance.PlayerEntity.Level - 10);
                attack.MeleeTimer += 450 * ((int)GameManager.Instance.PlayerEntity.Reflexes - 2);

                if (attack.MeleeTimer < 0)
                    attack.MeleeTimer = 1500;

                attack.MeleeTimer /= 980; // Approximates classic frame update

                return;
            }

            // Update melee decision
            if (pursueDecisionTimer == 0 && senses.TargetInSight)
            {
                EvaluatepursueDecision();
            }
            if (pursueDecisionTimer > 0)
                pursueDecisionTimer -= Time.deltaTime;
            if (pursueDecisionTimer < 0)
                pursueDecisionTimer = 0;

            if (changeStateTimer > 0)
                changeStateTimer -= Time.deltaTime;

            // Approach target until we are close enough to be on-guard, or continue to melee range if attacking
            if ((!retreating && distance >= (stopDistance * 2.75)) ||
                    (distance > stopDistance && pursueDecision))
            {
                // If state change timer is done, or we are already pursuing, we can move
                if (changeStateTimer <= 0 || pursuing)
                    PursueTarget(direction, moveSpeed);
                else // Otherwise, just keep an eye on target until timer finishes
                {
                    if (mobile.Summary.EnemyState == MobileStates.Move)
                        mobile.ChangeEnemyState(MobileStates.Idle);
                    if (!senses.TargetIsWithinYawAngle(22.5f))
                        TurnToTarget(direction.normalized);
                }
            }
            // Back away if right next to target, if retreating, or if cooling down from attack
            // Classic AI never backs away
            else if (DaggerfallUnity.Settings.EnhancedCombatAI && (senses.TargetInSight && (distance < stopDistance * .50 ||
                (!pursueDecision && distance < (stopDistance * retreatDistanceMultiplier)))))
            {
                // If state change timer is done, or we are already retreating, we can move
                if (changeStateTimer <= 0 || retreating)
                    BackAwayFromTarget(direction, moveSpeed / 2);
                else // Otherwise, just keep an eye on target until timer finishes
                {
                    if (mobile.Summary.EnemyState == MobileStates.Move)
                        mobile.ChangeEnemyState(MobileStates.Idle);
                    if (!senses.TargetIsWithinYawAngle(22.5f))
                        TurnToTarget(direction.normalized);
                }
            }
            else if (!senses.TargetIsWithinYawAngle(22.5f))
                TurnToTarget(direction.normalized);
            else
            {
                if (mobile.Summary.EnemyState == MobileStates.Move)
                    mobile.ChangeEnemyState(MobileStates.Idle);
                SetChangeStateTimer();
                pursuing = false;
                retreating = false;
            }
        }

        bool CanCastRangedSpell(DaggerfallEntity entity)
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

            return true;
        }

        bool CanCastTouchSpell(DaggerfallEntity entity)
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

        bool EffectsAlreadyOnTarget(EntityEffectBundle spell)
        {
            if (entityBehaviour.Target)
            {
                EntityEffectManager targetEffectManager = entityBehaviour.Target.GetComponent<EntityEffectManager>();
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

        private void PursueTarget(Vector3 direction, float moveSpeed)
        {
            pursuing = true;
            retreating = false;

            if (!senses.TargetIsWithinYawAngle(5.625f))
            {
                TurnToTarget(direction.normalized);
                return;
            }

            var motion = transform.forward * moveSpeed;

            // If using enhanced combat, avoid moving directly below targets
            if (DaggerfallUnity.Settings.EnhancedCombatAI)
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
                    // causes a random delay after being out of pitch range. for more realistic movements
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
                        motion = -transform.forward * moveSpeed * 0.75f;
                    else
                    {
                        if (mobile.Summary.EnemyState == MobileStates.Move)
                            mobile.ChangeEnemyState(MobileStates.Idle);
                        return;
                    }
                }

                if (!pausePursuit)
                    SetChangeStateTimer();
            }

            // Prevent rat stacks (enemies don't stand on shorter enemies)
            AvoidEnemies(ref motion);

            if (!mobile.IsPlayingOneShot() && motion != Vector3.zero)
                mobile.ChangeEnemyState(MobileStates.Move);

            if (swims)
            {
                WaterMove(motion);
            }
            else if (flies || isLevitating)
                controller.Move(motion * Time.deltaTime);
            else
                MoveIfNoFallDetected(motion);
        }

        private void BackAwayFromTarget(Vector3 direction, float moveSpeed)
        {
            retreating = true;
            pursuing = false;
            SetChangeStateTimer();

            if (!mobile.IsPlayingOneShot())
            {
                mobile.ChangeEnemyState(MobileStates.Move);

                if (!senses.TargetIsWithinYawAngle(5.625f))
                {
                    TurnToTarget(direction.normalized);
                    return;
                }

                var motion = -transform.forward * moveSpeed;

                // Prevent rat stacks (enemies don't stand on shorter enemies)
                AvoidEnemies(ref motion);

                if (swims)
                {
                    WaterMove(motion);
                }
                else if (flies || isLevitating)
                    controller.Move(motion * Time.deltaTime);
                else
                    MoveIfNoFallDetected(motion);
            }
        }

        private void MoveIfNoFallDetected(Vector3 motion)
        {
            // Check that we aren't walking off a ledge
            if (classicUpdate)
            {
                // First check if there is something to collide with directly in movement direction, such as upward sloping ground.
                // If there is, we assume we won't fall.
                RaycastHit hit;
                Vector3 rayOrigin = transform.position;
                Vector3 rayDirection = targetPos - transform.position;
                rayDirection.y = 0;
                Ray ray = new Ray(rayOrigin, rayDirection);
                if (Physics.Raycast(ray, out hit, 1))
                    fallDetected = false;
                // Nothing to collide with. Check for a reasonably short fall.
                else
                {
                    Vector3 motion2d = motion.normalized;
                    motion2d.y = 0;
                    motion2d *= 2;

                    ray = new Ray(transform.position + motion2d, Vector3.down);
                    if (Physics.Raycast(ray, out hit, 5))
                        fallDetected = false;
                    else
                        fallDetected = true;
                }
            }

            if (!fallDetected)
                controller.SimpleMove(motion);
            else if (mobile.Summary.EnemyState == MobileStates.Move && DaggerfallUnity.Settings.EnhancedCombatAI)
            {
                mobile.ChangeEnemyState(MobileStates.Idle);
                SetChangeStateTimer();
            }
        }

        private void EvaluatepursueDecision()
        {
            // No retreat without enhanced AI
            if (!DaggerfallUnity.Settings.EnhancedCombatAI)
            {
                pursueDecision = true;
                return;
            }

            // No retreat if enemy is paralyzed
            if (entityBehaviour.Target != null)
            {
                EntityEffectManager targetEffectManager = entityBehaviour.Target.GetComponent<EntityEffectManager>();
                if (targetEffectManager.FindIncumbentEffect<MagicAndEffects.MagicEffects.Paralyze>() != null)
                {
                    pursueDecision = true;
                    return;
                }

                // No retreat if enemy's back is turned
                if (senses.TargetHasBackTurned())
                {
                    pursueDecision = true;
                    return;
                }

                // No retreat if enemy is player with bow or weapon not out
                if (entityBehaviour.Target == GameManager.Instance.PlayerEntityBehaviour &&
                    GameManager.Instance.WeaponManager.ScreenWeapon &&
                    (GameManager.Instance.WeaponManager.ScreenWeapon.WeaponType == WeaponTypes.Bow ||
                    !GameManager.Instance.WeaponManager.ScreenWeapon.ShowWeapon))
                {
                    pursueDecision = true;
                    return;
                }
            }

            float retreatDistanceBaseMult = 2.25f;

            // Level difference affects likelihood of backing away.
            pursueDecisionTimer = Random.Range(1, 3);
            int levelMod = (entity.Level - entityBehaviour.Target.Entity.Level) / 2;
            if (levelMod > 4)
                levelMod = 4;
            if (levelMod < -4)
                levelMod = -4;

            int roll = Random.Range(0 + levelMod, 10 + levelMod);

            pursueDecision = roll > 4;

            // Chose to retreat
            if (!pursueDecision)
            {
                retreatDistanceMultiplier = (float)(retreatDistanceBaseMult + (retreatDistanceBaseMult * (0.25 * (2 - roll))));
            }
        }

        private void SetChangeStateTimer()
        {
            // No timer without enhanced AI
            if (!DaggerfallUnity.Settings.EnhancedCombatAI)
            {
                return;
            }

            // Set a delay between state changes so AI doesn't seem to instantly react to things
            if (changeStateTimer <= 0)
            {
                changeStateTimer = Random.Range(0.2f, .8f);
            }
        }

        private void WaterMove(Vector3 motion)
		{
            // Don't allow aquatic enemies to go above the water level of a dungeon block
            if (GameManager.Instance.PlayerEnterExit.blockWaterLevel != 10000
					&& controller.transform.position.y <
					GameManager.Instance.PlayerEnterExit.blockWaterLevel * -1 * MeshReader.GlobalScale)
			{
				if (motion.y > 0 && controller.transform.position.y + (100 * MeshReader.GlobalScale) >=
						GameManager.Instance.PlayerEnterExit.blockWaterLevel * -1 * MeshReader.GlobalScale)
				{
					motion.y = 0;
				}
                controller.Move(motion * Time.deltaTime);
			}
		}

        private void TurnToTarget(Vector3 targetDirection)
        {
            const float turnSpeed = 20f;
            //const float turnSpeed = 11.25f;

            if (!mobile.IsPlayingOneShot())
                mobile.ChangeEnemyState(MobileStates.Move);

            if (classicUpdate)
                transform.forward = Vector3.RotateTowards(transform.forward, targetDirection, turnSpeed * Mathf.Deg2Rad, 0.0f);
        }

        private void OpenDoors()
        {
            // Can we open doors?
            if (mobile.Summary.Enemy.CanOpenDoors)
            {
                // Is there a door blocking path to target?
                if (senses.LastKnownDoor != null && senses.DistanceToDoor < OpenDoorDistance)
                {
                    // Is the door closed? Try to open it!
                    if (!senses.LastKnownDoor.IsOpen)
                        senses.LastKnownDoor.ToggleDoor();
                }
            }
        }

        #endregion
    }
}