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
using System.Collections;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Temporary enemy attack.
    /// </summary>
    [RequireComponent(typeof(EnemySenses))]
    public class EnemyAttack : MonoBehaviour
    {
        public float MeleeAttackSpeed = 1.0f;       // Number of seconds between melee attacks
        public float MeleeDistance = 2.0f;          // Maximum distance for melee attack

        EnemyMotor motor;
        EnemySenses senses;
        EnemySounds sounds;
        DaggerfallMobileUnit mobile;
        DaggerfallEntityBehaviour entityBehaviour;
        float meleeTimer = 0;
        bool isAttacking;

        void Start()
        {
            motor = GetComponent<EnemyMotor>();
            senses = GetComponent<EnemySenses>();
            sounds = GetComponent<EnemySounds>();
            mobile = GetComponentInChildren<DaggerfallMobileUnit>();
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
        }

        void Update()
        {
            // Handle state in progress
            if (mobile.IsPlayingOneShot())
            {
                // Are we attacking?
                if (mobile.IsAttacking())
                    isAttacking = true;

                return;
            }

            // If an attack was in progress it is now complete and we can apply damage
            if (isAttacking)
            {
                MeleeDamage();
                isAttacking = false;
            }

            // Countdown to next attack
            meleeTimer -= Time.deltaTime;
            if (meleeTimer < 0)
            {
                MeleeAnimation();
                meleeTimer = MeleeAttackSpeed;
            }
        }

        #region Private Methods

        private void MeleeAnimation()
        {
            // Are we in range and facing player? Then start attack.
            if (senses.DistanceToPlayer < MeleeDistance && senses.PlayerInSight)
            {
                // Don't attack if not hostile
                if (!motor.IsHostile)
                    return;

                // Set melee animation state
                mobile.ChangeEnemyState(MobileStates.PrimaryAttack);

                // Play melee sound
                if (sounds)
                {
                    sounds.PlayAttackSound();
                }
            }
        }

        private void MeleeDamage()
        {
            int minDamage = 0, maxDamage = 0;
            int minDamage2 = 0, maxDamage2 = 0;
            int minDamage3 = 0, maxDamage3 = 0;
            if (entityBehaviour)
            {
                EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
                MobileEnemy enemy = entity.MobileEnemy;
                minDamage = enemy.MinDamage;
                maxDamage = enemy.MaxDamage;
                minDamage2 = enemy.MinDamage2;
                maxDamage2 = enemy.MinDamage2;
                minDamage3 = enemy.MinDamage3;
                maxDamage3 = enemy.MaxDamage3;
            }

            // Are we still in range and facing player? Then apply melee damage.
            if (senses.DistanceToPlayer < MeleeDistance && senses.PlayerInSight)
            {
                int damage = Random.Range(minDamage, maxDamage + 1);
                damage += Random.Range(minDamage2, maxDamage2 + 1);
                damage += Random.Range(minDamage3, maxDamage3 + 1);
                senses.Player.SendMessage("RemoveHealth", damage);
            }
        }

        #endregion
    }
}