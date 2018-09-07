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

        float stopDistance = 1.7f;                  // Used to prevent orbiting
        float giveUpTimer;                          // Timer before enemy gives up
        bool isHostile;                             // Is enemy hostile to player
        bool flies;                                 // The enemy can fly
        bool swims;                                 // The enemy can swim
        int enemyLayerMask;                         // Layer mask for Enemies to optimize collision checks

        bool isLevitating;                          // Allow non-flying enemy to levitate

        bool isAttackFollowsPlayerSet;              // For setting if the enemy will follow the player or not during an attack
        bool attackFollowsPlayer;                   // For setting if the enemy will follow the player or not during an attack

        float classicUpdateTimer = 0f;
        bool classicUpdate = false;
        float knockBackSpeed;                       // While non-zero, this enemy will be knocked backwards at this speed
        Vector3 knockBackDirection;                 // Direction to travel while being knocked back

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
            isAttackFollowsPlayerSet = false;
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
        /// Immediately become hostile towards player and know player's location.
        /// </summary>
        public void MakeEnemyHostileToPlayer(GameObject player)
        {
            if (player && senses)
            {
                senses.LastKnownPlayerPos = player.transform.position;
                giveUpTimer = 200;
            }
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

                return;
            }

            // Monster speed of movement follows the same formula as for when the player walks
            EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
            float moveSpeed = ((entity.Stats.LiveSpeed + PlayerSpeedChanger.dfWalkBase) / PlayerSpeedChanger.classicToUnitySpeedUnitRatio);

            // Reduced speed if playing a one-shot animation
            if (mobile.IsPlayingOneShot())
                moveSpeed /= AttackSpeedDivisor;

            // As long as the player is directly seen/heard,
            // giveUpTimer is reset to full
            if (senses.DetectedPlayer)
                giveUpTimer = 200;

            // Remain idle when not hostile or giving up finding the player
            if (!isHostile || giveUpTimer == 0)
            {
                mobile.ChangeEnemyState(MobileStates.Idle);
                return;
            }

            // GiveUpTimer value is from classic, so decrease at the speed of classic's update loop
            if (!senses.DetectedPlayer
                && giveUpTimer > 0 && classicUpdate)
                giveUpTimer--;

            // Enemy will keep moving towards last known player position
            targetPos = senses.LastKnownPlayerPos;

            // Flying enemies and slaughterfish aim for player face
            if (flies || isLevitating || (swims && mobile.Summary.Enemy.ID == (int)MonsterCareers.Slaughterfish))
                targetPos.y += 0.9f;
            else
            {
                // Ground enemies target at their own height
                // This avoids short enemies from stepping on each other as they approach the player
                // Otherwise, their target vector aims up towards the player
                var playerController = GameManager.Instance.PlayerController;
                var deltaHeight = (playerController.height - controller.height) / 2;
                targetPos.y -= deltaHeight;
            }

            // Get direction & distance.
            var direction = targetPos - transform.position;
            float distance = direction.magnitude;

            // If attacking, randomly follow player with attack.
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

            if (attackFollowsPlayer)
                transform.forward = direction.normalized;

            // Bow attack for enemies that have the appropriate animation
            if (senses.PlayerInSight && 360 * MeshReader.GlobalScale < distance && distance < 2048 * MeshReader.GlobalScale)
            {
                if (senses.TargetIsWithinYawAngle(22.5f))
                {
                    if (mobile.Summary.Enemy.HasRangedAttack1 && mobile.Summary.Enemy.ID > 129 && mobile.Summary.Enemy.ID != 132)
                    {
                        // Random chance to shoot bow
                        if (classicUpdate && DFRandom.rand() < 1000)
                        {
                            if (mobile.Summary.Enemy.HasRangedAttack1 && !mobile.Summary.Enemy.HasRangedAttack2
                                && mobile.Summary.EnemyState != MobileStates.RangedAttack1)
                                mobile.ChangeEnemyState(MobileStates.RangedAttack1);
                            else if (mobile.Summary.Enemy.HasRangedAttack2 && mobile.Summary.EnemyState != MobileStates.RangedAttack2)
                                mobile.ChangeEnemyState(MobileStates.RangedAttack2);
                        }
                        // Otherwise hold ground
                        else if (!mobile.IsPlayingOneShot())
                            mobile.ChangeEnemyState(MobileStates.Idle);
                    }
                    //else if (spellPoints > 0 && canCastRangeSpells && DFRandom.rand() % 40 == 0) TODO: Ranged spell shooting
                    //          CastRangedSpell();
                    //          Spell Cast Animation;
                    else
                        // If no ranged attack, move towards target
                        PursueTarget(direction, moveSpeed);
                }
                else
                {
                    if (!mobile.IsPlayingOneShot())
                        mobile.ChangeEnemyState(MobileStates.Move);
                    TurnToTarget(direction.normalized);
                    return;
                }
            }
            // Move towards target
            else if (distance > stopDistance)
                PursueTarget(direction, moveSpeed);
            else if (!senses.TargetIsWithinYawAngle(22.5f))
                TurnToTarget(direction.normalized);
            //else
            //{
            // TODO: Touch spells.
            //if (hasSpellPoints && attackCoolDownFinished && CanCastTouchSpells)
            //{
            //    Cast Touch Spell
            //    Spell Cast Animation
            //}
            //}
            else if (!senses.DetectedPlayer && mobile.Summary.EnemyState == MobileStates.Move)
                mobile.ChangeEnemyState(MobileStates.Idle);
        }

        private void PursueTarget(Vector3 direction, float moveSpeed)
        {
            if (!mobile.IsPlayingOneShot())
                mobile.ChangeEnemyState(MobileStates.Move);

            if (!senses.TargetIsWithinYawAngle(5.625f))
            {
                TurnToTarget(direction.normalized);
                return;
            }

            var motion = transform.forward * moveSpeed;

            // Prevent rat stacks (enemies don't stand on shorter enemies)
            AvoidEnemies(ref motion);

            if (swims)
            {
                WaterMove(motion);
            }
            else if (flies || isLevitating)
                controller.Move(motion * Time.deltaTime);
            else
                controller.SimpleMove(motion);
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

            if (classicUpdate)
                transform.forward = Vector3.RotateTowards(transform.forward, targetDirection, turnSpeed * Mathf.Deg2Rad, 0.0f);
        }

        private void OpenDoors()
        {
            // Can we open doors?
            if (mobile.Summary.Enemy.CanOpenDoors)
            {
                // Is there a door blocking path to player?
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