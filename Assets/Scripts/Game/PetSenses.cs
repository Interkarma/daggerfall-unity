// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Allofich
// 
// Notes:
//

using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Questing;
using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    public class PetSenses : MonoBehaviour
    {
        public static readonly Vector3 ResetPlayerPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

        public float SightRadius = 4096 * MeshReader.GlobalScale; // Range of pet sight
        public float HearingRadius = 25f; // Range of pet hearing
        public float FieldOfView = 180f; // Pet field of view
        public int maxdDistanceToPlayer = 2;
        public float movingSpeed = 20f;
        const float predictionInterval = 0.0625f;

        PetMobileUnit mobile;
        PetBehaviour petBehaviour;
        bool targetInSight;
        bool playerInSight;

        Vector3 directionToTarget;
        float distanceToPlayer;
        float distanceToTarget;
        DaggerfallEntityBehaviour player;
        DaggerfallEntityBehaviour target;

        bool wouldBeSpawnedInClassic = false;


        float targetPosPredictTimer = 0f;
        bool targetPosPredict = false;

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

        public bool TargetInSight
        {
            get { return targetInSight; }
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

        private void Awake()
        {
            petBehaviour = GetComponent<PetBehaviour>();
        }

        void Start()
        {
            mobile = GetComponentInChildren<DaggerfallPetMobileUnit>();
            player = GameManager.Instance.PlayerEntityBehaviour;

            short[] classicSpawnXZDistArray = { 1024, 384, 640, 768, 768, 768, 768 };
            short[] classicSpawnYDistUpperArray = { 128, 128, 128, 384, 768, 128, 256 };
            short[] classicSpawnYDistLowerArray = { 0, 0, 0, 0, -128, -768, 0 };
            short[] classicDespawnXZDistArray = { 1024, 1024, 1024, 1024, 768, 768, 768 };
            short[] classicDespawnYDistArray = { 384, 384, 384, 384, 768, 768, 768 };

            byte index = mobile.ClassicSpawnDistanceType;

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
            if (GameManager.Instance.DisableAI)
                return;

            targetPosPredictTimer += Time.deltaTime;
            if (targetPosPredictTimer >= predictionInterval)
            {
                targetPosPredictTimer = 0f;
                targetPosPredict = true;
            }
            else
                targetPosPredict = false;

            // Update whether pet would be spawned or not in classic.
            // Only check if within the maximum possible distance (Just under 1094 classic units)
            if (GameManager.ClassicUpdate)
            {
                if (distanceToPlayer < 1094 * MeshReader.GlobalScale)
                {
                    float upperXZ;
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

                    float YDiffToPlayer = transform.position.y - player.transform.position.y;
                    float YDiffToPlayerAbs = Mathf.Abs(YDiffToPlayer);
                    float distanceToPlayerXZ =
                        Mathf.Sqrt(distanceToPlayer * distanceToPlayer - YDiffToPlayerAbs * YDiffToPlayerAbs);

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
                else
                    wouldBeSpawnedInClassic = false;
            }

            if (GameManager.ClassicUpdate)
            {
                if (target != null && target.Entity.CurrentHealth <= 0)
                {
                    target = null;
                }

                // Non-hostile mode
                if (GameManager.Instance.PlayerEntity.NoTargetMode)
                {
                    if (target == player)
                        target = null;

                }

                // Reset these values if no target
                if (target == null)
                {
                    directionToTarget = ResetPlayerPos;
                    distanceToTarget = 0;

                }
            }

            if (player != null)
            {
                // Get distance to player
                Vector3 toPlayer = player.transform.position - transform.position;
                distanceToPlayer = toPlayer.magnitude;
                petBehaviour.HpTransform.transform.LookAt(player.transform);

                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance > 10)
                {
                    transform.position = player.transform.position;
                }
                else if (distance >= maxdDistanceToPlayer)
                {
                    mobile.ChangePetState(MobileStates.Move);

                    transform.LookAt(player.transform);
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position,
                        movingSpeed * Time.deltaTime);
                }
            }
        }
    }
}