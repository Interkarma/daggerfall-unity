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
        bool playerInSight;
        bool playerInEarshot;
        Vector3 directionToPlayer;
        float distanceToPlayer;
        Vector3 lastKnownPlayerPos;
        DaggerfallActionDoor actionDoor;
        float distanceToActionDoor;
        bool hasEncounteredPlayer = false;
        bool wouldBeSpawnedInClassic = false;
        bool detectedPlayer = false;
        uint timeOfLastStealthCheck = 0;
        bool blockedByIllusionEffect = false;
        float lastHadLOSTimer = 0f;

        float classicUpdateTimer = 0f;
        bool classicUpdate = false;

        float classicSpawnDespawnExterior = 4096 * MeshReader.GlobalScale;
        float classicSpawnXZDist = 0f;
        float classicSpawnYDistUpper = 0f;
        float classicSpawnYDistLower = 0f;
        float classicDespawnXZDist = 0f;
        float classicDespawnYDist = 0f;

        GameObject Player
        {
            get { return GameManager.Instance.PlayerObject; }
        }

        public bool PlayerInSight
        {
            get { return playerInSight; }
        }

        public bool DetectedPlayer
        {
            get { return detectedPlayer; }
            set { detectedPlayer = value; }
        }

        public bool PlayerInEarshot
        {
            get { return playerInEarshot; }
        }

        public Vector3 DirectionToPlayer
        {
            get { return directionToPlayer; }
        }

        public float DistanceToPlayer
        {
            get { return distanceToPlayer; }
        }

        public Vector3 LastKnownPlayerPos
        {
            get { return lastKnownPlayerPos; }
            set { lastKnownPlayerPos = value; }
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

        void Start()
        {
            mobile = GetComponentInChildren<DaggerfallMobileUnit>();
            lastKnownPlayerPos = ResetPlayerPos;

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
                    if (lowerY == 0 && YDiffToPlayerAbs > upperY)
                        wouldBeSpawnedInClassic = false;
                    else if (YDiffToPlayer < lowerY || YDiffToPlayer > upperY)
                        wouldBeSpawnedInClassic = false;
                }
            }

            if (Player != null)
            {
                Vector3 toPlayer = Player.transform.position - transform.position;
                directionToPlayer = toPlayer.normalized;
                distanceToPlayer = toPlayer.magnitude;

                playerInSight = CanSeePlayer();

                // Classic stealth mechanics would be interfered with by hearing, so only enable
                // hearing if the enemy has detected the player. If player has been seen we can omit hearing.
                if (detectedPlayer && !playerInSight)
                    playerInEarshot = CanHearPlayer();
                else
                    playerInEarshot = false;

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

                if (!blockedByIllusionEffect && (playerInSight || playerInEarshot))
                {
                    detectedPlayer = true;
                    lastKnownPlayerPos = Player.transform.position;
                    lastHadLOSTimer = 200f;
                }
                else if (!blockedByIllusionEffect && StealthCheck())
                {
                    detectedPlayer = true;

                    // Only get the player's location from the stealth check if we haven't had
                    // actual LOS for a while. This gives better pursuit behavior since enemies
                    // will go to the last spot they saw the player instead of walking into walls.
                    if (lastHadLOSTimer <= 0)
                        lastKnownPlayerPos = Player.transform.position;
                }
                else
                    detectedPlayer = false;

                if (detectedPlayer && !hasEncounteredPlayer)
                {
                    hasEncounteredPlayer = true;

                    // Check appropriate language skill to see if player can pacify enemy
                    DaggerfallEntityBehaviour entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
                    EnemyMotor motor = GetComponent<EnemyMotor>();
                    if (entityBehaviour && motor &&
                        (entityBehaviour.EntityType == EntityTypes.EnemyMonster || entityBehaviour.EntityType == EntityTypes.EnemyClass))
                    {
                        EnemyEntity enemyEntity = entityBehaviour.Entity as EnemyEntity;
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

            if (distanceToPlayer > 1024 * MeshReader.GlobalScale)
                return false;

            uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            if (gameMinutes == timeOfLastStealthCheck)
                return detectedPlayer;

            PlayerMotor playerMotor = GameManager.Instance.PlayerMotor;
            if (playerMotor.IsMovingLessThanHalfSpeed)
            {
                if ((gameMinutes & 1) == 1)
                    return detectedPlayer;
            }
            else if (hasEncounteredPlayer)
                return true;

            PlayerEntity player = GameManager.Instance.PlayerEntity;
            if (player.TimeOfLastStealthCheck != gameMinutes)
            {
                player.TallySkill(DFCareer.Skills.Stealth, 1);
                player.TimeOfLastStealthCheck = gameMinutes;
            }
            timeOfLastStealthCheck = gameMinutes;

            int stealthRoll = 2 * ((int)(distanceToPlayer / MeshReader.GlobalScale) * player.Skills.GetLiveSkillValue(DaggerfallConnect.DFCareer.Skills.Stealth) >> 10);

            return Random.Range(1, 101) > stealthRoll;
        }

        public bool BlockedByIllusionEffect()
        {
            // Note: These effects only block detection for the player.
            // In classic if the target is another AI character true is always returned.

            // Some enemy types can see through these effects.
            if (mobile.Summary.Enemy.SeesThroughInvisibility)
                return false;

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

        public bool TargetIsWithinYawAngle(float targetAngle)
        {
            Vector3 toTarget = lastKnownPlayerPos - transform.position;
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

        private bool CanSeePlayer()
        {
            bool seen = false;
            actionDoor = null;

            if (distanceToPlayer < SightRadius + mobile.Summary.Enemy.SightModifier)
            {
                // Check if player in field of view
                float angle = Vector3.Angle(directionToPlayer, transform.forward);
                if (angle < FieldOfView * 0.5f)
                {
                    // Check if line of sight to player
                    RaycastHit hit;
                    Ray ray = new Ray(transform.position, directionToPlayer);
                    if (Physics.Raycast(ray, out hit, SightRadius))
                    {
                        // Check if hit was player
                        if (hit.transform.gameObject == Player)
                        {
                            seen = true;
                        }

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

        private bool CanHearPlayer()
        {
            bool heard = false;
            float hearingScale = 1f;

            // If something is between enemy and player then return false (was reduce hearingScale by half), to minimize
            // enemies walking against walls.
            // Hearing is not impeded by doors or other non-static objects
            RaycastHit hit;
            Ray ray = new Ray(transform.position, directionToPlayer);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject != Player && hit.transform.gameObject.isStatic)
                    return false;
            }

            // TODO: Modify this by how much noise the player is making
            if (distanceToPlayer < (HearingRadius * hearingScale) + mobile.Summary.Enemy.HearingModifier)
            {
                heard = true;
            }

            return heard;
        }

        #endregion
    }
}
