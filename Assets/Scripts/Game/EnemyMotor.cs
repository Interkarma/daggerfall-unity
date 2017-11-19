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
        public float OpenDoorDistance = 2f;         // Maximum distance to open door
        public float GiveUpTime = 4f;               // Time in seconds enemy will give up if target is unreachable
        public const float AttackSpeedDivisor = 3f;       // How much to slow down during attack animations

        EnemySenses senses;
        Vector3 targetPos;
        CharacterController controller;
        DaggerfallMobileUnit mobile;
        DaggerfallEntityBehaviour entityBehaviour;

        float stopDistance = 1.7f;                  // Used to prevent orbiting
        Vector3 lastTargetPos;                      // Target from previous update
        float giveUpTimer;                          // Timer before enemy gives up
        bool isHostile;                             // Is enemy hostile to player
        bool flies;                                 // The enemy can fly
        int enemyLayerMask;                         // Layer mask for Enemies to optimize collision checks

        bool isAttackFollowsPlayerSet;              // For setting if the enemy will follow the player or not during an attack
        bool attackFollowsPlayer;                   // For setting if the enemy will follow the player or not during an attack

        float classicUpdateTimer = 0f;      // Just used for knockback processing currently
        float knockBackSpeed;                         // While non-zero, this enemy will be knocked backwards at this speed
        Vector3 knockBackDirection;                 // Direction to travel while being knocked back

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
            enemyLayerMask = LayerMask.GetMask("Enemies");
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            isAttackFollowsPlayerSet = false;
        }

        void Update()
        {
        }

        void FixedUpdate()
        {
            Move();
            OpenDoors();
        }

        /// <summary>
        /// Immediately become hostile towards player.
        /// </summary>
        public void MakeEnemyHostileToPlayer(GameObject player)
        {
            if (player)
            {
                senses.LastKnownPlayerPos = player.transform.position;
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
//            plannedMotion = Quaternion.Euler(0, 90, 0) * plannedMotion;
        }

        private void Move()
        {
            // If hit, get knocked back
            if (knockBackSpeed > 0)
            {
                // Limit knockBackSpeed. This can be higher than what is actually used for the speed of motion,
                // making it last longer and do more damage if the enemy collides with something.
                if (knockBackSpeed > (40 / (PlayerMotor.classicToUnitySpeedUnitRatio / 10)))
                    knockBackSpeed = (40 / (PlayerMotor.classicToUnitySpeedUnitRatio / 10));

                if (knockBackSpeed > (5 / (PlayerMotor.classicToUnitySpeedUnitRatio / 10)) &&
                    mobile.Summary.EnemyState != MobileStates.PrimaryAttack)
                    mobile.ChangeEnemyState(MobileStates.Hurt);

                // Actual speed of motion is limited
                Vector3 motion;
                if (knockBackSpeed <= (25 / (PlayerMotor.classicToUnitySpeedUnitRatio / 10)))
                    motion = knockBackDirection * knockBackSpeed;
                else
                    motion = knockBackDirection * (25 / (PlayerMotor.classicToUnitySpeedUnitRatio / 10));

                if (flies)
                    controller.Move(motion * Time.deltaTime);
                else
                    controller.SimpleMove(motion);

                classicUpdateTimer += Time.deltaTime;
                if (classicUpdateTimer >= PlayerEntity.ClassicUpdateInterval)
                {
                    classicUpdateTimer = 0;
                    knockBackSpeed -= (5 / (PlayerMotor.classicToUnitySpeedUnitRatio / 10));
                    if (knockBackSpeed <= (5 / (PlayerMotor.classicToUnitySpeedUnitRatio / 10)) &&
                        mobile.Summary.EnemyState != MobileStates.PrimaryAttack)
                        mobile.ChangeEnemyState(MobileStates.Move);
                }

                return;
            }

            // Monster speed of movement follows the same formula as for when the player walks
            EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
            float moveSpeed = ((entity.Stats.LiveSpeed + PlayerMotor.dfWalkBase) / PlayerMotor.classicToUnitySpeedUnitRatio);

            // Reduced speed if playing a one-shot animation
            if (mobile.IsPlayingOneShot())
                moveSpeed /= AttackSpeedDivisor;

            // Remain idle when player not acquired or not hostile
            if (senses.LastKnownPlayerPos == EnemySenses.ResetPlayerPos || !isHostile)
            {
                mobile.ChangeEnemyState(MobileStates.Idle);
                return;
            }

            // Enemy will keep moving towards last known player position
            targetPos = senses.LastKnownPlayerPos;
            if (targetPos == lastTargetPos)
            {
                // Increment countdown to giving up when target is uncreachable and player lost
                giveUpTimer += Time.deltaTime;
                if (giveUpTimer > GiveUpTime &&
                    !senses.PlayerInSight && !senses.PlayerInEarshot)
                {
                    // Target is unreachable or player lost for too long, time to give up
                    senses.LastKnownPlayerPos = EnemySenses.ResetPlayerPos;
                    return;
                }
            }
            else
            {
                // Still chasing, update last target and reset give up timer
                lastTargetPos = targetPos;
                giveUpTimer = 0;
            }

            // Flying enemies aim for player face
            if (flies)
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

            // Get direction & distance and face target.
            // If attacking, randomly do not do so so player has a chance to see
            // attack animations other than those directly facing the player
            var direction = targetPos - transform.position;
            float distance = direction.magnitude;

            if (mobile.IsPlayingOneShot() && !isAttackFollowsPlayerSet)
            {
                attackFollowsPlayer = (Random.Range(0f, 1f) > 0.5f);
                isAttackFollowsPlayerSet = true;
            }
            else if (!mobile.IsPlayingOneShot())
                isAttackFollowsPlayerSet = false;

            if (!mobile.IsPlayingOneShot() || !attackFollowsPlayer)
                transform.forward = direction.normalized;

            // Move towards target
            if (distance > stopDistance)
            {
                if (!mobile.IsPlayingOneShot())
                    mobile.ChangeEnemyState(MobileStates.Move);
                var motion = transform.forward * moveSpeed;

                // Prevent rat stacks (enemies don't stand on shorter enemies)
                AvoidEnemies(ref motion);

                if (flies)
                    controller.Move(motion * Time.deltaTime);
                else
                    controller.SimpleMove(motion);
            }
            else
            {
                // We have reached target, is player nearby?
                if (!senses.PlayerInSight && !senses.PlayerInEarshot)
                    senses.LastKnownPlayerPos = EnemySenses.ResetPlayerPos;
            }
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