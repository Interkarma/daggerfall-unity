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

        public float SightRadius = 4096 * MeshReader.GlobalScale;       // Range of enemy sight
        public float HearingRadius = 25f;                               // Range of enemy hearing
        public float FieldOfView = 180f;                                // Enemy field of view

        const float predictionInterval = 0.0625f;

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
        DaggerfallEntityBehaviour target;
        DaggerfallEntityBehaviour lastTarget;
        EnemySenses targetSenses;
        float lastDistanceToTarget;
        float targetRateOfApproach;
        Vector3 lastKnownTargetPos;
        Vector3 oldLastKnownTargetPos;
        Vector3 predictedTargetPos;
        Vector3 predictedTargetPosWithoutLead;
        Vector3 lastPositionDiff;
        bool awareOfTargetForLastPrediction;
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
        float targetPosPredictTimer = 0f;
        bool targetPosPredict = false;

        float classicTargetUpdateTimer = 0f;
        const float systemTimerUpdatesDivisor = .0549254f;  // Divisor for updates per second by the system timer at memory location 0x46C.

        const float classicSpawnDespawnExterior = 4096 * MeshReader.GlobalScale;
        float classicSpawnXZDist = 0f;
        float classicSpawnYDistUpper = 0f;
        float classicSpawnYDistLower = 0f;
        float classicDespawnXZDist = 0f;
        float classicDespawnYDist = 0f;

        public DaggerfallEntityBehaviour Target
        {
            get { return target; }
            set { target = value; }
        }

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

        public Vector3 OldLastKnownTargetPos
        {
            get { return oldLastKnownTargetPos; }
            set { oldLastKnownTargetPos = value; }
        }

        public Vector3 PredictedTargetPos
        {
            get { return predictedTargetPos; }
            set { predictedTargetPos = value; }
        }

        public DaggerfallActionDoor LastKnownDoor
        {
            get { return actionDoor; }
            set { actionDoor = value; }
        }

        public float DistanceToDoor
        {
            get { return distanceToActionDoor; }
            set { distanceToActionDoor = value; }
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
            oldLastKnownTargetPos = ResetPlayerPos;
            predictedTargetPos = ResetPlayerPos;

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

            // 180 degrees is classic's value. 190 degrees is actual human FOV according to online sources.
            if (DaggerfallUnity.Settings.EnhancedCombatAI)
                FieldOfView = 190;
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

            targetPosPredictTimer += Time.deltaTime;
            if (targetPosPredictTimer >= predictionInterval)
            {
                targetPosPredictTimer = 0f;
                targetPosPredict = true;
            }
            else
                targetPosPredict = false;

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

                if (target != null && target.Entity.CurrentHealth <= 0)
                {
                    target = null;
                }

                // NoTarget mode
                if ((GameManager.Instance.PlayerEntity.NoTargetMode || !motor.IsHostile) && target == Player)
                    target = null;

                // Reset these values if no target
                if (target == null)
                {
                    lastKnownTargetPos = ResetPlayerPos;
                    predictedTargetPos = ResetPlayerPos;
                    directionToTarget = ResetPlayerPos;
                    lastDistanceToTarget = 0;
                    targetRateOfApproach = 0;
                    distanceToTarget = 0;
                    targetSenses = null;

                    // If had a valid target before, resume pursuing it. Looks better to first finish any attack animation.
                    if (lastTarget != null && lastTarget.Entity.CurrentHealth > 0 && !mobile.IsPlayingOneShot())
                        target = lastTarget;
                }

                if ((motor.IsHostile && target == null) || classicTargetUpdateTimer > 10) // Timing is 200 in classic, about 10 seconds.
                {
                    classicTargetUpdateTimer = 0f;

                    // Is enemy in area around player or can see player?
                    if (wouldBeSpawnedInClassic || playerInSight)
                    {
                        target = GetTarget();
                        if (target != null && target != Player)
                            targetSenses = target.GetComponent<EnemySenses>();
                        else
                            targetSenses = null;
                    }

                    // Make targeted character also target this character if it doesn't have a target yet.
                    if (target != null && targetSenses && targetSenses.Target == null)
                    {
                        targetSenses.Target = entityBehaviour;
                    }
                }

                // Compare change in target position to give AI some ability to read opponent's movements

                if (target != null && target == lastTarget)
                {
                    if (DaggerfallUnity.Settings.EnhancedCombatAI)
                        targetRateOfApproach = (lastDistanceToTarget - distanceToTarget);
                }
                else
                {
                    lastDistanceToTarget = 0;
                    targetRateOfApproach = 0;
                }

                if (target != null)
                {
                    lastDistanceToTarget = distanceToTarget;
                    lastTarget = target;
                }
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
                if (target != null)
                    toTarget = target.transform.position - transform.position;

                if (toTarget != ResetPlayerPos)
                {
                    distanceToTarget = toTarget.magnitude;
                    directionToTarget = toTarget.normalized;
                }

                if (target == null)
                {
                    targetInSight = false;
                    detectedTarget = false;
                    return;
                }

                targetInSight = CanSeeTarget(target);

                // Classic stealth mechanics would be interfered with by hearing, so only enable
                // hearing if the enemy has detected the target. If target is visible we can omit hearing.
                if (detectedTarget && !targetInSight)
                    targetInEarshot = CanHearTarget(target);
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
                    lastKnownTargetPos = target.transform.position;
                    lastHadLOSTimer = 200f;
                }
                else if (!blockedByIllusionEffect && StealthCheck())
                {
                    detectedTarget = true;

                    // Only get the target's location from the stealth check if we haven't had
                    // actual LOS for a while. This gives better pursuit behavior since enemies
                    // will go to the last spot they saw the player instead of walking into walls.
                    if (lastHadLOSTimer <= 0)
                        lastKnownTargetPos = target.transform.position;
                }
                else
                    detectedTarget = false;

                if (oldLastKnownTargetPos == ResetPlayerPos)
                    oldLastKnownTargetPos = lastKnownTargetPos;

                if (predictedTargetPos == ResetPlayerPos || !DaggerfallUnity.Settings.EnhancedCombatAI)
                    predictedTargetPos = lastKnownTargetPos;

                // Predict target's next position
                if (targetPosPredict && DaggerfallUnity.Settings.EnhancedCombatAI && predictedTargetPos != ResetPlayerPos && lastKnownTargetPos != ResetPlayerPos)
                {
                    float moveSpeed = (enemyEntity.Stats.LiveSpeed + PlayerSpeedChanger.dfWalkBase) * MeshReader.GlobalScale;
                    predictedTargetPos = PredictNextTargetPos(moveSpeed);
                }

                if (detectedTarget && !hasEncounteredPlayer && target == Player)
                {
                    hasEncounteredPlayer = true;

                    // Check appropriate language skill to see if player can pacify enemy
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
                                player.TallySkill(languageSkill, 3);    // BCHG: increased skill uses from 1 in classic on success to make raising language skills easier
                            }
                            else if (languageSkill != DFCareer.Skills.Etiquette && languageSkill != DFCareer.Skills.Streetwise)
                                player.TallySkill(languageSkill, 1);
                        }
                    }
                }
            }
        }

        #region Public Methods

        public Vector3 PredictNextTargetPos(float interceptSpeed)
        {
            // Be sure to only take difference of movement if we've seen the target for two consecutive prediction updates
            if (targetInSight || targetInEarshot)
            {
                if (awareOfTargetForLastPrediction)
                    lastPositionDiff = lastKnownTargetPos - oldLastKnownTargetPos;

                // Store current last known target position for next prediction update
                oldLastKnownTargetPos = lastKnownTargetPos;

                awareOfTargetForLastPrediction = true;
            }
            else
                awareOfTargetForLastPrediction = false;

            Vector3 assumedCurrentPosition;

            // If aware of target, use last known position as assumed current position
            if (targetInSight || targetInEarshot)
            {
                assumedCurrentPosition = lastKnownTargetPos;
            }
            // Stop predicting if distance is too far
            else if ((predictedTargetPos - transform.position).magnitude > SightRadius + mobile.Summary.Enemy.SightModifier)
            {
                assumedCurrentPosition = predictedTargetPosWithoutLead;
                lastPositionDiff = Vector3.zero;
            }
            // If not aware of target and predicted position may still be good, use predicted position
            else
            {
                assumedCurrentPosition = predictedTargetPosWithoutLead;
            }

            // Get seconds to the predicted position so we lead target to intercept. Important when using this function to aim ranged attacks.
            float secondsToPredictedPos = (assumedCurrentPosition - transform.position).magnitude / interceptSpeed;
            float divisor = predictionInterval;

            // Account for mid-interval call by DaggerfallMissile
            if (targetPosPredictTimer != 0)
            {
                divisor = targetPosPredictTimer;
                targetPosPredictTimer = 0;
            }

            Vector3 prediction = assumedCurrentPosition + (lastPositionDiff / divisor * secondsToPredictedPos);

            // Store prediction minus lead for next prediction update
            predictedTargetPosWithoutLead = assumedCurrentPosition + lastPositionDiff;

            // Don't predict target will move right past us (prevents AI from turning around
            // when target is approaching)
            float a = assumedCurrentPosition.z * transform.position.x - assumedCurrentPosition.x * transform.position.z;
            float b = assumedCurrentPosition.z * prediction.x - assumedCurrentPosition.x * prediction.z;
            float c = prediction.z * transform.position.x - prediction.x * transform.position.z;
            float d = prediction.z * assumedCurrentPosition.x - prediction.x * assumedCurrentPosition.z;

            if (a * b >= 0 && c * d >= 0)
                prediction = assumedCurrentPosition;

            // Don't predict target will move through obstacles (prevent predicting movement through walls)
            RaycastHit hit;
            Ray ray = new Ray(assumedCurrentPosition, (prediction - assumedCurrentPosition).normalized);
            if (Physics.Raycast(ray, out hit, (prediction - assumedCurrentPosition).magnitude))
                prediction = assumedCurrentPosition;

            return prediction;
        }

        public bool StealthCheck()
        {
            if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeonCastle && !motor.IsHostile)
                return false;

            if (!wouldBeSpawnedInClassic)
                return false;

            if (distanceToTarget > 1024 * MeshReader.GlobalScale)
                return false;

            uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            if (gameMinutes == timeOfLastStealthCheck)
                return detectedTarget;

            if (target == Player)
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

            int stealthRoll = 2 * ((int)(distanceToTarget / MeshReader.GlobalScale) * target.Entity.Skills.GetLiveSkillValue(DFCareer.Skills.Stealth) >> 10);

            return Random.Range(1, 101) > stealthRoll;
        }

        public bool BlockedByIllusionEffect()
        {
            // In classic if the target is another AI character true is always returned.

            // Some enemy types can see through these effects.
            if (mobile.Summary.Enemy.SeesThroughInvisibility)
                return false;

            // If not one of the above enemy types, and target has invisibility,
            // detection is always blocked.
            if (target.Entity.IsInvisible)
                return true;

            // If target doesn't have any illusion effect, detection is not blocked.
            if (!target.Entity.IsBlending && !target.Entity.IsAShade)
                return false;

            // Target has either chameleon or shade. Try to see through it.
            int chance;
            if (target.Entity.IsBlending)
                chance = 8;
            else // is a shade
                chance = 4;

            return Random.Range(1, 101) > chance;
        }

        public bool TargetIsWithinYawAngle(float targetAngle, Vector3 targetPos)
        {
            Vector3 toTarget = targetPos - transform.position;
            Vector3 directionToLastKnownTarget2D = toTarget.normalized;
            directionToLastKnownTarget2D.y = 0;

            Vector3 enemyDirection2D = transform.forward;
            enemyDirection2D.y = 0;

            float angle = Vector3.Angle(directionToLastKnownTarget2D, enemyDirection2D);
            return angle < targetAngle;
        }

        public bool TargetHasBackTurned()
        {
            Vector3 toTarget = predictedTargetPos - transform.position;
            Vector3 directionToLastKnownTarget2D = toTarget.normalized;
            directionToLastKnownTarget2D.y = 0;

            Vector3 targetDirection2D;

            if (target == Player)
            {
                Camera mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
                targetDirection2D = -new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z);
            }
            else
                targetDirection2D = -new Vector3(target.transform.forward.x, 0, target.transform.forward.z);

            float angle = Vector3.Angle(directionToLastKnownTarget2D, targetDirection2D);

            return angle > 157.5f;
        }

        public bool TargetIsWithinPitchAngle(float targetAngle)
        {
            Vector3 toTarget = predictedTargetPos - transform.position;
            Vector3 directionToLastKnownTarget2D = toTarget.normalized;
            Plane verticalTransformToLastKnownPos = new Plane(predictedTargetPos, transform.position, transform.position + Vector3.up);
            // first project enemy direction to horizontal plane.
            Vector3 enemyDirection2D = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
            // next project enemy direction to vertical plane intersecting with last known position
            enemyDirection2D = Vector3.ProjectOnPlane(enemyDirection2D, verticalTransformToLastKnownPos.normal);

            float angle = Vector3.Angle(directionToLastKnownTarget2D, enemyDirection2D);

            return angle < targetAngle;
        }

        public bool TargetIsAbove()
        {
            return predictedTargetPos.y > transform.position.y;
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
                    // NoTarget mode
                    if ((GameManager.Instance.PlayerEntity.NoTargetMode || !motor.IsHostile) && targetBehaviour == Player)
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

                    // For now, quest AI only targets player
                    if (questBehaviour && targetBehaviour != Player)
                        continue;

                    EnemySenses targetSenses = null;
                    if (targetBehaviour.EntityType == EntityTypes.EnemyMonster || targetBehaviour.EntityType == EntityTypes.EnemyClass)
                        targetSenses = targetBehaviour.GetComponent<EnemySenses>();

                    // For now, quest AI can't be targeted
                    if (targetSenses && targetSenses.QuestBehaviour)
                        continue;

                    Vector3 toTarget = targetBehaviour.transform.position - transform.position;
                    directionToTarget = toTarget.normalized;
                    distanceToTarget = toTarget.magnitude;

                    bool see = CanSeeTarget(targetBehaviour);

                    // Is potential target neither visible nor in area around player? If so, reject as target.
                    if (targetSenses && !targetSenses.WouldBeSpawnedInClassic && !see)
                        continue;

                    float priority = 0;

                    // Add 5 priority if this potential target isn't already targeting someone
                    if (targetSenses && targetSenses.Target == null)
                        priority += 5;

                    if (see)
                        priority += 10;

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
                        directionToTargetHolder = directionToTarget;
                        distanceToTargetHolder = distanceToTarget;
                    }
                }
            }

            // Restore direction and distance values
            directionToTarget = directionToTargetHolder;
            distanceToTarget = distanceToTargetHolder;

            targetInSight = sawSelectedTarget;
            return highestPriorityTarget;
        }

        bool CanSeeTarget(DaggerfallEntityBehaviour target)
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

                    // Set origin of ray to approximate eye position
                    CharacterController controller = entityBehaviour.transform.GetComponent<CharacterController>();
                    Vector3 eyePos = transform.position + controller.center;
                    eyePos.y += controller.height / 3;

                    // Set destination to the target's approximate eye position
                    controller = target.transform.GetComponent<CharacterController>();
                    Vector3 targetEyePos = target.transform.position + controller.center;
                    targetEyePos.y += controller.height / 3;

                    // Check if can see.
                    Vector3 eyeToTarget = targetEyePos - eyePos;
                    Vector3 eyeDirectionToTarget = eyeToTarget.normalized;
                    Ray ray = new Ray(eyePos, eyeDirectionToTarget);

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
                            actionDoor = door;
                            distanceToActionDoor = Vector3.Distance(transform.position, actionDoor.transform.position);
                        }
                    }
                }
            }

            return seen;
        }

        bool CanHearTarget(DaggerfallEntityBehaviour target)
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
