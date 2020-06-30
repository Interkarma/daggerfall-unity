// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
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
        const float secondsToTransform = 0.0f;      // 0.0 will disable transform for now

        DaggerfallMobileUnit enemyMobile;
        DaggerfallEntityBehaviour enemyEntityBehaviour;
        EnemyEntity enemyEntity;
        EnemySenses enemySenses;

        float transformCountdown = secondsToTransform;
        bool startTransform = false;

        private void Start()
        {
            // Get references
            enemyMobile = GetComponent<DaggerfallMobileUnit>();
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
            if (!enemySenses || enemyEntity == null)
                return;

            // If targeting player always transform after a few seconds
            // This allows winged form to reach player even when humanoid form cannot (e.g. stuck on pillar in Direnni Tower)
            if (enemySenses.Target == GameManager.Instance.PlayerEntityBehaviour && transformCountdown > 0)
            {
                transformCountdown -= Time.deltaTime;
                if (transformCountdown <= 0)
                {
                    transformCountdown = 0;
                    startTransform = true;
                    enemyEntity.SuppressInfighting = true;
                }
            }

            // Start transformation to winged form
            if (startTransform && enemyMobile)
            {
                enemyMobile.ChangeEnemyState(MobileStates.SeducerTransform1);
                startTransform = false;
            }
        }
    }
}