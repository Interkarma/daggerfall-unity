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
using System.Collections;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Questing;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Example enemy senses.
    /// </summary>
    public class EnemySenses : MonoBehaviour
    {
        public static readonly Vector3 ResetPlayerPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

        public float SightRadius = 4096 * MeshReader.GlobalScale;         // Range of enemy sight
        public float HearingRadius = 25f;       // Range of enemy hearing
        public float FieldOfView = 180f;        // Enemy field of view

        DaggerfallMobileUnit mobile;
        DaggerfallEntityBehaviour entityBehaviour;
        QuestResourceBehaviour questBehaviour;
        EnemyMotor motor;
        EnemyEntity enemyEntity;
        bool targetInSight;
        bool playerInSight;
        bool targetInEarshot;
        Vector3 directionToTarget;
        float distanceToPlayer;
        float distanceToTarget;
        float lastDistanceToTarget;
        float targetRateOfApproach;
        Vector3 lastKnownTargetPos;
        DaggerfallActionDoor actionDoor;
        float distanceToActionDoor;
        bool hasEncounteredPlayer = false;
        bool wouldBeSpawnedInClassic = false;
        bool detectedTarget = false;
        uint timeOfLastStealthCheck = 0;
        bool blockedByIllusionEffect = false;
        float lastHadLOSTimer = 0f;

        float classicUpdateTimer = 0f;
        bool classicUpdate = false;
        float classicTargetUpdateTimer = 0f;
        float systemTimerUpdatesDivisor = .0549254f;  // Divisor for updates per second by the system timer at memory location 0x46C.

        float classicSpawnDespawnExterior = 4096 * MeshReader.GlobalScale;
        float classicSpawnXZDist = 0f;
        float classicSpawnYDistUpper = 0f;
        float classicSpawnYDistLower = 0f;
        float classicDespawnXZDist = 0f;
        float classicDespawnYDist = 0f;

        DaggerfallEntityBehaviour Player
        {
            get { return GameManager.Instance.PlayerEntityBehaviour; }
        }

        public bool TargetInSight
        {
            get { return targetInSight; }
        }

        public bool DetectedTarget
        {
            get { return detectedTarget; }
            set { detectedTarget = value; }
        }

        public bool TargetInEarshot
        {
            get { return targetInEarshot; }
        }

        public Vector3 DirectionToTarget
        {
            get { return directionToTarget; }
        }

        public float DistanceToPlayer
        {
            get { return distanceToPlayer; }
        }

        public float DistanceToTarget
        {
            get { return distanceToTarget; }
        }

        public Vector3 LastKnownTargetPos
        {
            get { return lastKnownTargetPos; }
            set { lastKnownTargetPos = value; }
        }

        public DaggerfallActionDoor LastKnownDoor
        {
            get { return actionDoor; }
        }

        public float DistanceToDoor
        {
            get { return distanceToActionDoor; }
        }

        public bool HasEncounteredPlayer
        {
            get { return hasEncounteredPlayer; }
            set { hasEncounteredPlayer = value; }
        }

        public bool WouldBeSpawnedInClassic
        {
            get { return wouldBeSpawnedInClassic; }
            set { wouldBeSpawnedInClassic = value; }
        }

        public QuestResourceBehaviour QuestBehaviour
        {
            get { return questBehaviour; }
            set { questBehaviour = value; }
        }

        public float TargetRateOfApproach
        {
            get { return targetRateOfApproach; }
            set { targetRateOfApproach = value; }
        }

        void Start()
        {
            mobile = GetComponentInChildren<DaggerfallMobileUnit>();
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            enemyEntity = entityBehaviour.Entity as EnemyEntity;
            motor = GetComponent<EnemyMotor>();
            questBehaviour = GetComponent<QuestResourceBehaviour>();
            lastKnownTargetPos = ResetPlayerPos;

            short[] classicSpawnXZDistArray = { 1024, 384, 640, 768, 768, 768, 768 };
            short[] classicSpawnYDistUpperArray = { 128, 128, 128, 384, 768, 128, 256 };
            short[] classicSpawnYDistLowerArray = { 0, 0, 0, 0, -128, -768, 0 };
            short[] classicDespawnXZDistArray = { 1024, 1024, 1024, 1024, 768, 768, 768 };
            short[] classicDespawnYDistArray = { 384, 384, 384, 384, 768, 768, 768 };

            byte index = mobile.Summary.ClassicSpawnDistanceType;

            classicSpawnXZDist = classicSpawnXZDistArray[index] * MeshReader.GlobalScale;
            classicSpawnYDistUpper = classicSpawnYDistUpperArray[index] * MeshReader.GlobalScale;
            classicSpawnYDistLower = classicSpawnYDistLowerArray[index] * MeshReader.GlobalScale;
            classicDespawnXZDist = classicDespawnXZDistArray[index] * MeshReader.GlobalScale;
            classicDespawnYDist = classicDespawnYDistArray[index] * MeshReader.GlobalScale;
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

            // Reset whether enemy would be spawned or not in classic.
            if (classicUpdate)
                wouldBeSpawnedInClassic = false;

            // Update whether enemy would be spawned or not in classic.
            // Only check if within the maximum possible distance (Just under 1094 classic units)
            if (classicUpdate && distanceToPlayer < 1094 * MeshReader.GlobalScale)
            {
                float upperXZ = 0;
                float upperY = 0;
                float lowerY = 0;
                bool playerInside = GameManager.Instance.PlayerGPS.GetComponent<PlayerEnterExit>().IsPlayerInside;

                if (!playerInside)
                {
                    upperXZ = classicSpawnDespawnExterior;
                }
                else
                {
                    if (!wouldBeSpawnedInClassic)
                    {
                        upperXZ = classicSpawnXZDist;
                        upperY = classicSpawnYDistUpper;
                        lowerY = classicSpawnYDistLower;
                    }
                    else
                    {
                        upperXZ = classicDespawnXZDist;
                        upperY = classicDespawnYDist;
                    }
                }

                float YDiffToPlayer = transform.position.y - Player.transform.position.y;
                float YDiffToPlayerAbs = Mathf.Abs(YDiffToPlayer);
                float distanceToPlayerXZ = Mathf.Sqrt(distanceToPlayer * distanceToPlayer - YDiffToPlayerAbs * YDiffToPlayerAbs);

                wouldBeSpawnedInClassic = true;

                if (distanceToPlayerXZ > upperXZ)
                    wouldBeSpawnedInClassic = false;

                if (playerInside)
                {
                    if (lowerY == 0)
                    {
                        if (YDiffToPlayerAbs > upperY)
                            wouldBeSpawnedInClassic = false;
                    }
                    else if (YDiffToPlayer < lowerY || YDiffToPlayer > upperY)
                        wouldBeSpawnedInClassic = false;
                }
            }

            if (classicUpdate)
            {
                classicTargetUpdateTimer += Time.deltaTime / systemTimerUpdatesDivisor;

                if (entityBehaviour.Target != null && entityBehaviour.Target.Entity.CurrentHealth <= 0)
                    entityBehaviour.Target = null;

                // NoTarget mode
                if ((GameManager.Instance.PlayerEntity.NoTargetMode || !motor.IsHostile) && entityBehaviour.Target == Player)
                    entityBehaviour.Target = null;

                if (entityBehaviour.Target == null)
                    lastKnownTargetPos = ResetPlayerPos;

                if ((motor.IsHostile && entityBehaviour.Target == null) || classicTargetUpdateTimer > 10) // Timing is 200 in classic, about 10 seconds.
                {
                    classicTargetUpdateTimer = 0f;

                    // Is enemy in area around player or can see player?
                    if (wouldBeSpawnedInClassic || playerInSight)
                    {
                        entityBehaviour.Target = GetTarget();
                    }

                    // Make targeted character also target this character if it doesn't have a target yet.
                    if (entityBehaviour.Target != null && entityBehaviour.Target.Target == null)
                    {
                        entityBehaviour.Target.Target = entityBehaviour;
                    }
                }

                // Compare change in target position to give AI some ability to read opponent's movements
                if (lastDistanceToTarget != 0)
                {
                    targetRateOfApproach = (lastDistanceToTarget - distanceToTarget);
                }

                lastDistanceToTarget = distanceToTarget;
            }

            if (Player != null)
            {
                // Get distance to player
                Vector3 toPlayer = Player.transform.position - transform.position;
                distanceToPlayer = toPlayer.magnitude;

                // If out of classic spawn range, still check for direct LOS to player so that enemies who see player will
                // try to attack.
                if (!wouldBeSpawnedInClassic)
                {
                    distanceToTarget = distanceToPlayer;
                    directionToTarget = toPlayer.normalized;
                    playerInSight = CanSeeTarget(Player);
                }

                Vector3 toTarget = ResetPlayerPos;
                if (entityBehaviour.Target != null)
                    toTarget = entityBehaviour.Target.transform.position - transform.position;

                if (toTarget != ResetPlayerPos)
                {
                    distanceToTarget = toTarget.magnitude;
                    directionToTarget = toTarget.normalized;
                }

                if (entityBehaviour.Target == null)
                {
                    targetInSight = false;
                    detectedTarget = false;
                    return;
                }

                targetInSight = CanSeeTarget(entityBehaviour.Target);

                // Classic stealth mechanics would be interfered with by hearing, so only enable
                // hearing if the enemy has detected the target. If target is visible we can omit hearing.
                if (detectedTarget && !targetInSight)
                    targetInEarshot = CanHearTarget(entityBehaviour.Target);
                else
                    targetInEarshot = false;

                // Note: In classic an enemy can continue to track the player as long as their
                // giveUpTimer is > 0. Since the timer is reset to 200 on every detection this
                // would make chameleon and shade essentially useless, since the enemy is sure
                // to detect the player during one of the many AI updates. Here, the enemy has to
                // successfully see through the illusion spell each classic update to continue
                // to know where the player is.
                if (classicUpdate)
                {
                    blockedByIllusionEffect = BlockedByIllusionEffect();
                    if (lastHadLOSTimer > 0)
                        lastHadLOSTimer--;
                }

                if (!blockedByIllusionEffect && (targetInSight || targetInEarshot))
                {
                    detectedTarget = true;
                    lastKnownTargetPos = entityBehaviour.Target.transform.position;
                    lastHadLOSTimer = 200f;
                }
                else if (!blockedByIllusionEffect && StealthCheck())
                {
                    detectedTarget = true;

                    // Only get the target's location from the stealth check if we haven't had
                    // actual LOS for a while. This gives better pursuit behavior since enemies
                    // will go to the last spot they saw the player instead of walking into walls.
                    if (lastHadLOSTimer <= 0)
                        lastKnownTargetPos = entityBehaviour.Target.transform.position;
                }
                else
                    detectedTarget = false;

                if (detectedTarget && !hasEncounteredPlayer && entityBehaviour.Target == Player)
                {
                    hasEncounteredPlayer = true;

                    // Check appropriate language skill to see if player can pacify enemy
                    DaggerfallEntityBehaviour entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
                    if (entityBehaviour && motor &&
                        (entityBehaviour.EntityType == EntityTypes.EnemyMonster || entityBehaviour.EntityType == EntityTypes.EnemyClass))
                    {
                        DFCareer.Skills languageSkill = enemyEntity.GetLanguageSkill();
                        if (languageSkill != DFCareer.Skills.None)
                        {
                            PlayerEntity player = GameManager.Instance.PlayerEntity;
                            if (FormulaHelper.CalculateEnemyPacification(player, languageSkill))
                            {
                                motor.IsHostile = false;
                                DaggerfallUI.AddHUDText(HardStrings.languagePacified.Replace("%e", enemyEntity.Name).Replace("%s", languageSkill.ToString()), 5);
                                player.TallySkill(languageSkill, 3);    // BCHG: increased skill uses from (assumed) 1 in classic on success to make raising language skills easier
                            }
                            else if (languageSkill != DFCareer.Skills.Etiquette && languageSkill != DFCareer.Skills.Streetwise)
                                player.TallySkill(languageSkill, 1);
                        }
                    }
                }
            }
        }

        #region Public Methods

        public bool StealthCheck()
        {
            if (!wouldBeSpawnedInClassic)
                return false;

            if (distanceToTarget > 1024 * MeshReader.GlobalScale)
                    return false;

            uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            if (gameMinutes == timeOfLastStealthCheck)
                return detectedTarget;

            if (entityBehaviour.Target == Player)
            {
                PlayerMotor playerMotor = GameManager.Instance.PlayerMotor;
                if (playerMotor.IsMovingLessThanHalfSpeed)
                {
                    if ((gameMinutes & 1) == 1)
                        return detectedTarget;
                }
                else if (hasEncounteredPlayer)
                    return true;

                PlayerEntity player = GameManager.Instance.PlayerEntity;
                if (player.TimeOfLastStealthCheck != gameMinutes)
                {
                    player.TallySkill(DFCareer.Skills.Stealth, 1);
                    player.TimeOfLastStealthCheck = gameMinutes;
                }
            }

            timeOfLastStealthCheck = gameMinutes;

            int stealthRoll = 2 * ((int)(distanceToTarget / MeshReader.GlobalScale) * entityBehaviour.Target.Entity.Skills.GetLiveSkillValue(DFCareer.Skills.Stealth) >> 10);

            return Random.Range(1, 101) > stealthRoll;
        }

        public bool BlockedByIllusionEffect()
        {
            // In classic if the target is another AI character true is always returned.

            // Some enemy types can see through these effects.
            if (mobile.Summary.Enemy.SeesThroughInvisibility)
                return false;

            if (entityBehaviour.Target == Player)
            {
                // If not one of the above enemy types, and player has invisibility,
                // detection is always blocked.
                PlayerEntity player = GameManager.Instance.PlayerEntity;
                if (player.IsInvisible)
                    return true;

                // If player doesn't have any illusion effect, detection is not blocked.
                if (!player.IsBlending && !player.IsAShade)
                    return false;

                // Player has either chameleon or shade. Try to see through it.
                int chance;
                if (player.IsBlending)
                    chance = 8;
                else // is a shade
                    chance = 4;

                return Random.Range(1, 101) > chance;
            }
            else // TODO: Apply these effects for concealed AI characers.
            {
                return false;
            }
        }

        public bool TargetIsWithinYawAngle(float targetAngle)
        {
            Vector3 toTarget = lastKnownTargetPos - transform.position;
            Vector3 directionToLastKnownTarget2D = toTarget.normalized;
            directionToLastKnownTarget2D.y = 0;

            Vector3 enemyDirection2D = transform.forward;
            enemyDirection2D.y = 0;

            float angle = Vector3.Angle(directionToLastKnownTarget2D, enemyDirection2D);
            if (angle < targetAngle)
                return true;
            else
                return false;
        }

        #endregion

        #region Private Methods

        DaggerfallEntityBehaviour GetTarget()
        {
            DaggerfallEntityBehaviour highestPriorityTarget = null;
            float highestPriority = -1;
            bool sawSelectedTarget = false;
            Vector3 directionToTargetHolder = directionToTarget;
            float distanceToTargetHolder = distanceToTarget;

            DaggerfallEntityBehaviour[] entityBehaviours = FindObjectsOfType<DaggerfallEntityBehaviour>();
            for (int i = 0; i < entityBehaviours.Length; i++)
            {
                DaggerfallEntityBehaviour targetBehaviour = entityBehaviours[i];
                EnemyEntity targetEntity = null;
                if (targetBehaviour != Player)
                    targetEntity = targetBehaviour.Entity as EnemyEntity;

                // Can't target self
                if (targetBehaviour == entityBehaviour)
                    continue;

                // Evaluate potential targets
                if (targetBehaviour.EntityType == EntityTypes.EnemyMonster || targetBehaviour.EntityType == EntityTypes.EnemyClass
                    || targetBehaviour.EntityType == EntityTypes.Player)
                {
                    Vector3 toTarget = targetBehaviour.transform.position - transform.position;
                    directionToTarget = toTarget.normalized;
                    distanceToTarget = toTarget.magnitude;

                    EnemySenses targetSenses = null;
                    if (targetBehaviour.EntityType == EntityTypes.EnemyMonster || targetBehaviour.EntityType == EntityTypes.EnemyClass)
                        targetSenses = targetBehaviour.GetComponent<EnemySenses>();

                    bool see = CanSeeTarget(targetBehaviour);

                    if (targetSenses)
                    {
                        // Is potential target neither visible nor in area around player? If so, reject as target.
                        if (!targetSenses.WouldBeSpawnedInClassic && !see)
                            continue;
                    }

                    // For now, quest AI only targets player
                    if (questBehaviour && targetBehaviour != Player)
                        continue;

                    // For now, quest AI can't be targeted
                    if (targetSenses && targetSenses.QuestBehaviour)
                        continue;

                    // Can't target ally
                    if (DaggerfallUnity.Settings.EnemyInfighting)
                    {
                        if (targetEntity != null && targetEntity.MobileEnemy.Team == enemyEntity.MobileEnemy.Team)
                            continue;
                    }
                    else // TODO: Support player-summoned allies, even without infighting option
                    {
                        if (targetBehaviour != Player)
                            continue;
                    }

                    // NoTarget mode
                    if ((GameManager.Instance.PlayerEntity.NoTargetMode || !motor.IsHostile) && targetBehaviour == Player)
                        continue;

                    float priority = 0;

                    // Add 5 priority if this potential target isn't already targeting someone
                    if (targetBehaviour.Target == null)
                        priority += 5;

                    if (see)
                        priority += 20;

                    // Add distance priority
                    float distancePriority = 30 - distanceToTarget;
                    if (distancePriority < 0)
                        distancePriority = 0;

                    priority += distancePriority;
                    if (priority > highestPriority)
                    {
                        highestPriority = priority;
                        highestPriorityTarget = targetBehaviour;
                        sawSelectedTarget = see;
                    }
                }
            }

            // Restore direction and distance values
            directionToTarget = directionToTargetHolder;
            distanceToTarget = distanceToTargetHolder;

            targetInSight = sawSelectedTarget;
            return highestPriorityTarget;
        }


        private bool CanSeeTarget(DaggerfallEntityBehaviour target)
        {
            bool seen = false;
            actionDoor = null;

            if (distanceToTarget < SightRadius + mobile.Summary.Enemy.SightModifier)
            {
                // Check if target in field of view
                float angle = Vector3.Angle(directionToTarget, transform.forward);
                if (angle < FieldOfView * 0.5f)
                {
                    // Check if line of sight to target
                    RaycastHit hit;
                    Ray ray = new Ray(transform.position, directionToTarget);
                    if (Physics.Raycast(ray, out hit, SightRadius))
                    {
                        // Check if hit was target
                        DaggerfallEntityBehaviour entity = hit.transform.gameObject.GetComponent<DaggerfallEntityBehaviour>();
                        if (entity == target)
                            seen = true;

                        // Check if hit was an action door
                        DaggerfallActionDoor door = hit.transform.gameObject.GetComponent<DaggerfallActionDoor>();
                        if (door != null)
                        {
                            if (!door.IsLocked && !door.IsMagicallyHeld)
                            {
                                actionDoor = door;
                                distanceToActionDoor = Vector3.Distance(transform.position, actionDoor.transform.position);
                            }
                        }
                    }
                }
            }

            return seen;
        }

        private bool CanHearTarget(DaggerfallEntityBehaviour target)
        {
            bool heard = false;
            float hearingScale = 1f;

            // If something is between enemy and target then return false (was reduce hearingScale by half), to minimize
            // enemies walking against walls.
            // Hearing is not impeded by doors or other non-static objects
            RaycastHit hit;
            Ray ray = new Ray(transform.position, directionToTarget);
            if (Physics.Raycast(ray, out hit))
            {
                DaggerfallEntityBehaviour entity = hit.transform.gameObject.GetComponent<DaggerfallEntityBehaviour>();
                if (entity != target && hit.transform.gameObject.isStatic)
                    return false;
            }

            // TODO: Modify this by how much noise the target is making
            if (distanceToTarget < (HearingRadius * hearingScale) + mobile.Summary.Enemy.HearingModifier)
            {
                heard = true;
            }

            return heard;
        }

        #endregion
    }
}
