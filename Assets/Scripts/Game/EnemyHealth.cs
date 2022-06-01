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
using System.Collections;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Example enemy health.
    /// Note: Deprecated. For standalone DFTFU scenes only.
    /// </summary>
    [RequireComponent(typeof(EnemyBlood))]
    public class EnemyHealth : MonoBehaviour
    {
        public float Health = 50f;

        EnemyMotor motor;
        MobileUnit mobile;
        EnemyBlood blood;

        void Start()
        {
            motor = GetComponent<EnemyMotor>();
            mobile = GetComponentInChildren<MobileUnit>();
            blood = GetComponent<EnemyBlood>();
        }

        /// <summary>
        /// Enemy has been damaged.
        /// </summary>
        public void RemoveHealth(GameObject sendingPlayer, float amount, Vector3 hitPosition)
        {
            Health -= amount;
            if (Health < 0)
                SendMessage("Die");

            // Aggro this enemy
            // To enhance, use a script that "shouts" to other enemies in range and make them hostile to player also
            motor.MakeEnemyHostileToAttacker(GameManager.Instance.PlayerEntityBehaviour);

            if (mobile != null)
            {
                blood.ShowBloodSplash(mobile.Enemy.BloodIndex, hitPosition);
            }

            //Debug.Log(string.Format("Enemy health is {0}", Health));
        }
    }
}