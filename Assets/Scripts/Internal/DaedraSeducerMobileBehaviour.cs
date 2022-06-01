// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Special mobile unit behaviours for Daedra Seducer.
    /// </summary>
    public class DaedraSeducerMobileBehaviour : MonoBehaviour
    {
        const float secondsToTransform = 8.0f;      // 0.0 will disable transform completely

        MobileUnit enemyMobile;
        DaggerfallEntityBehaviour enemyEntityBehaviour;
        EnemyEntity enemyEntity;
        EnemySenses enemySenses;

        float transformCountdown = secondsToTransform;
        bool transformStarted = false;

        private void Start()
        {
            // Get references
            enemyMobile = GetComponent<MobileUnit>();
            enemyEntityBehaviour = GetComponentInParent<DaggerfallEntityBehaviour>();
            if (enemyEntityBehaviour && enemyEntityBehaviour.EntityType == EntityTypes.EnemyMonster)
            {
                enemyEntity = (EnemyEntity)enemyEntityBehaviour.Entity;
                enemySenses = enemyEntityBehaviour.GetComponent<EnemySenses>();
            }
        }

        private void Update()
        {
            // Validate references
            if (!enemySenses || !enemyMobile || enemyEntity == null)
                return;

            // Exit if special transformation already completed
            // Raise suppress infighting flag in case player has loaded game after transform
            if (enemyMobile.SpecialTransformationCompleted)
            {
                enemyEntity.SuppressInfighting = true;
                return;
            }

            // Keep trying to raise transform state if wants to start and currently in another state
            // This prevents some other state (e.g. hurt) breaking switch to transformation
            if (transformStarted &&
                enemyMobile.EnemyState != MobileStates.SeducerTransform1 &&
                enemyMobile.EnemyState != MobileStates.SeducerTransform2)
            {
                StartTransformation();
                return;
            }

            // Only transform when targeting player and hurt or after timer elapsed
            // This improves chance player is close enough to witness transformation
            // A transformed Seducer is excluded from infighting due to sprite limitations (has player facing sprites only)
            if (enemySenses.Target == GameManager.Instance.PlayerEntityBehaviour && transformCountdown > 0)
            {
                // Check if  if hurt
                bool isHurt = enemyEntity.CurrentHealth < enemyEntity.MaxHealth;

                // Progress countdown
                transformCountdown -= Time.deltaTime;

                // Transform when hurt or countdown ended while player is targeted
                // Countdown allows winged form to reach player even when humanoid form cannot (e.g. stuck on pillar in Direnni Tower)
                if (isHurt || transformCountdown <= 0)
                    StartTransformation();
            }
        }

        void StartTransformation()
        {
            transformCountdown = 0;
            enemyMobile.ChangeEnemyState(MobileStates.SeducerTransform1);
            transformStarted = true;
        }
    }
}