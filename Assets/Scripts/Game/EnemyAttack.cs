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
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Temporary enemy attack.
    /// </summary>
    [RequireComponent(typeof(EnemySenses))]
    public class EnemyAttack : MonoBehaviour
    {
        public float MeleeAttackSpeed = 1.25f;      // Number of seconds between melee attacks
        public float MeleeDistance = 2.5f;          // Maximum distance for melee attack

        EnemyMotor motor;
        EnemySenses senses;
        EnemySounds sounds;
        DaggerfallMobileUnit mobile;
        DaggerfallEntityBehaviour entityBehaviour;
        float meleeTimer = 0;
        bool isMeleeAttackingPreHitFrame;
        bool isShootingPreHitFrame;

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
            // Handle state in progress before hit frame
            if (mobile.IsPlayingOneShot())
            {
                if (mobile.LastFrameAnimated < mobile.Summary.Enemy.HitFrame
                    && mobile.Summary.EnemyState == MobileStates.PrimaryAttack)
                {
                    // Are we melee attacking?
                    if (mobile.IsAttacking())
                        isMeleeAttackingPreHitFrame = true;

                    return;
                }
                else if (mobile.LastFrameAnimated < 2 // TODO: Animate bow correctly
                    && (mobile.Summary.EnemyState == MobileStates.RangedAttack1
                    || mobile.Summary.EnemyState == MobileStates.RangedAttack2))
                {
                    // Are we shooting bow?
                    if (mobile.IsAttacking())
                        isShootingPreHitFrame = true;

                    return;
                }
            }

            // If a melee attack has reached the hit frame we can apply damage
            if (isMeleeAttackingPreHitFrame && mobile.LastFrameAnimated == mobile.Summary.Enemy.HitFrame)
            {
                MeleeDamage();
                isMeleeAttackingPreHitFrame = false;
            }
            // Same for shooting bow
            else if (isShootingPreHitFrame && mobile.LastFrameAnimated == 2) // TODO: Animate bow correctly
            {
                BowDamage();
                isShootingPreHitFrame = false;

                DaggerfallAudioSource dfAudioSource = GetComponent<DaggerfallAudioSource>();
                if (dfAudioSource)
                    dfAudioSource.PlayOneShot((int)SoundClips.ArrowShoot, 1, 1.0f);
            }

            // Countdown to next melee attack
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

                int damage = 0;

                // Are we still in range and facing player? Then apply melee damage.
                if (senses.DistanceToPlayer < MeleeDistance && senses.PlayerInSight)
                {
                    damage = ApplyDamageToPlayer();
                }

                Items.DaggerfallUnityItem weapon = entity.ItemEquipTable.GetItem(Items.EquipSlots.RightHand);
                if (weapon == null)
                    weapon = entity.ItemEquipTable.GetItem(Items.EquipSlots.LeftHand);

                if (damage > 0)
                {
                    // TODO: Play hit and parry sounds on other AI characters once attacks against other AI are possible
                    if (weapon != null)
                        GameManager.Instance.PlayerObject.SendMessage("PlayWeaponHitSound");
                    else
                        GameManager.Instance.PlayerObject.SendMessage("PlayWeaponlessHitSound");
                }
                else if (sounds)
                {
                    sounds.PlayMissSound(weapon);
                }
            }
        }

        private void BowDamage()
        {
            if (entityBehaviour)
            {
                // Can we see player? Then apply damage.
                if (senses.PlayerInSight)
                {
                    int damage = ApplyDamageToPlayer();

                    // Play arrow sound and add arrow to player inventory
                    GameManager.Instance.PlayerObject.SendMessage("PlayArrowSound");

                    if (damage > 0)
                        GameManager.Instance.PlayerObject.SendMessage("PlayWeaponHitSound");

                    Items.DaggerfallUnityItem arrow = Items.ItemBuilder.CreateItem(Items.ItemGroups.Weapons, (int)Items.Weapons.Arrow);
                    GameManager.Instance.PlayerEntity.Items.AddItem(arrow);
                }
            }
        }

        private int ApplyDamageToPlayer()
        {
            int damage = 0;
            EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;

            // Calculate damage
            damage = FormulaHelper.CalculateAttackDamage(entity, GameManager.Instance.PlayerEntity, (int)(Items.EquipSlots.RightHand), -1);

            // Tally player's dodging skill
            GameManager.Instance.PlayerEntity.TallySkill(DFCareer.Skills.Dodging, 1);

            if (damage > 0)
            {
                GameManager.Instance.PlayerObject.SendMessage("RemoveHealth", damage);

                Diseases disease = FormulaHelper.CalculateChanceOfDisease(entity);
                if (disease != Diseases.None)
                    GameManager.Instance.PlayerEntity.Disease = new DaggerfallDisease(disease);
            }

            return damage;
        }

        #endregion
    }
}