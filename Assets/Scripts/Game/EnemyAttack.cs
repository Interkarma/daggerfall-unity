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
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Temporary enemy attack.
    /// </summary>
    [RequireComponent(typeof(EnemySenses))]
    public class EnemyAttack : MonoBehaviour
    {
        public float MeleeAttackSpeed = 1.25f;      // Number of seconds between melee attacks
        public float MeleeDistance = 2f;          // Maximum distance for melee attack

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
            if (mobile.IsPlayingOneShot() && (mobile.LastFrameAnimated < mobile.Summary.Enemy.HitFrame))
            {
                // Are we attacking?
                if (mobile.IsAttacking())
                    isAttacking = true;

                return;
            }

            // If an attack was in progress it is now complete and we can apply damage
            if (isAttacking && mobile.LastFrameAnimated == mobile.Summary.Enemy.HitFrame)
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
                // Randomize
                meleeTimer += Random.Range(-.50f, .50f);
            }
        }

        #region Private Methods

        private void MeleeAnimation()
        {
            // Are we in range and facing player? Then start attack.
            if (senses.PlayerInSight)
            {
                // Take the speed of movement during the attack animation and hit frame into account when calculating attack range
                EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
                float attackSpeed = ((entity.Stats.LiveSpeed + PlayerMotor.dfWalkBase) / PlayerMotor.classicToUnitySpeedUnitRatio) / EnemyMotor.AttackSpeedDivisor;
                float timeUntilHit = mobile.Summary.Enemy.HitFrame / DaggerfallWorkshop.Utility.EnemyBasics.PrimaryAttackAnimSpeed;

                if (senses.DistanceToPlayer >= (MeleeDistance + (attackSpeed * timeUntilHit)))
                    return;

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
            if (entityBehaviour)
            {
                EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
                MobileEnemy enemy = entity.MobileEnemy;

                int damage = 0;

                // Are we still in range and facing player? Then apply melee damage.
                if (senses.DistanceToPlayer < MeleeDistance && senses.PlayerInSight)
                {
                    // Calculate damage
                    damage = Game.Formulas.FormulaHelper.CalculateWeaponDamage(entity, GameManager.Instance.PlayerEntity, null);
                    if (damage > 0)
                    {
                        GameManager.Instance.PlayerObject.SendMessage("RemoveHealth", damage);
                    }

                    // Tally player's dodging skill
                    GameManager.Instance.PlayerEntity.TallySkill(DFCareer.Skills.Dodging, 1);
                }

                if (sounds)
                {
                    Items.DaggerfallUnityItem weapon = entity.ItemEquipTable.GetItem(Items.EquipSlots.RightHand);
                    if (weapon == null)
                        weapon = entity.ItemEquipTable.GetItem(Items.EquipSlots.LeftHand);
                    if (damage > 0)
                    {
                        // TODO: Play hit and parry sounds on other AI characters once attacks against other AI are possible
                        DaggerfallAudioSource dfAudioSource = GetComponent<DaggerfallAudioSource>();
                        if (dfAudioSource)
                        {
                            if (weapon == null)
                                dfAudioSource.PlayOneShot((int)SoundClips.Hit1 + UnityEngine.Random.Range(2, 4), 0, 1.1f);
                            else
                                dfAudioSource.PlayOneShot((int)SoundClips.Hit1 + UnityEngine.Random.Range(0, 5), 0, 1.1f);
                        }
                    }
                    else
                        sounds.PlayMissSound(weapon);
                }
            }
        }

        #endregion
    }
}