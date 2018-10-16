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
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.MagicAndEffects;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Temporary enemy attack.
    /// </summary>
    [RequireComponent(typeof(EnemySenses))]
    public class EnemyAttack : MonoBehaviour
    {
        public float MeleeAttackSpeed = 1.25f;      // Number of seconds between melee attacks
        public float MeleeDistance = 3.2f;          // Maximum distance for melee attack
        public float MeleeTimer = 0;                // Must be 0 for a melee attack or touch spell to be done

        //EnemyMotor motor;
        EnemySenses senses;
        EnemySounds sounds;
        DaggerfallMobileUnit mobile;
        DaggerfallEntityBehaviour entityBehaviour;
        int damage = 0;
        float classicUpdateTimer = 0f;
        bool classicUpdate = false;

        void Start()
        {
            //motor = GetComponent<EnemyMotor>();
            senses = GetComponent<EnemySenses>();
            sounds = GetComponent<EnemySounds>();
            mobile = GetComponentInChildren<DaggerfallMobileUnit>();
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
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
        }

        void Update()
        {
            // If a melee attack has reached the damage frame we can run a melee attempt
            if (mobile.DoMeleeDamage)
            {
                MeleeDamage();
                mobile.DoMeleeDamage = false;
            }
            // If a bow attack has reached the shoot frame we can shoot an arrow
            else if (mobile.ShootArrow)
            {
                BowDamage(); // TODO: Shoot 3D projectile instead of doing an instant hit
                mobile.ShootArrow = false;

                DaggerfallAudioSource dfAudioSource = GetComponent<DaggerfallAudioSource>();
                if (dfAudioSource)
                    dfAudioSource.PlayOneShot((int)SoundClips.ArrowShoot, 1, 1.0f);
            }

            // Countdown to next melee attack
            MeleeTimer -= Time.deltaTime;

            if (MeleeTimer < 0)
                MeleeTimer = 0;

            EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
            int speed = entity.Stats.LiveSpeed;

            // Note: Speed comparison here is reversed from classic. Classic's way makes fewer attack
            // attempts at higher speeds, so it seems backwards.
            if (classicUpdate && (DFRandom.rand() % speed >= (speed >> 3) + 6 && MeleeTimer == 0))
            {
                MeleeAnimation();

                MeleeTimer = Random.Range(1500, 3001);
                MeleeTimer -= 50 * (GameManager.Instance.PlayerEntity.Level - 10);

                // Note: In classic, what happens here is
                // meleeTimer += 450 * (enemydata[130] - 2);
                // Apparently this was meant to reference the game reflexes setting,
                // which is stored in playerentitydata[130].
                // Instead enemydata[130] seems to instead always be 0, the equivalent of
                // "very high" reflexes, regardless of what the game reflexes are.
                // Here, we use the reflexes data as was intended.
                MeleeTimer += 450 * ((int)GameManager.Instance.PlayerEntity.Reflexes - 2);

                if (MeleeTimer > 100000 || MeleeTimer < 0)
                    MeleeTimer = 1500;

                MeleeTimer /= 980; // Approximates classic frame update
            }
        }

        #region Private Methods

        private void MeleeAnimation()
        {
            // Are we in range and facing target? Then start attack.
            if (senses.TargetInSight)
            {
                // Take the rate of target approach into account when deciding if to attack
                if (senses.DistanceToTarget >= MeleeDistance + senses.TargetRateOfApproach)
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
                EnemyEntity targetEntity = null;

                if (entityBehaviour.Target != null && entityBehaviour.Target != GameManager.Instance.PlayerEntityBehaviour)
                    targetEntity = entityBehaviour.Target.Entity as EnemyEntity;

                // Switch to hand-to-hand if enemy is immune to weapon
                Items.DaggerfallUnityItem weapon = entity.ItemEquipTable.GetItem(Items.EquipSlots.RightHand);
                if (weapon != null)
                {
                    if (targetEntity != null && targetEntity.MobileEnemy.MinMetalToHit > (Items.WeaponMaterialTypes)weapon.NativeMaterialValue)
                        weapon = null;
                }

                damage = 0;

                // Are we still in range and facing target? Then apply melee damage.
                if (entityBehaviour.Target != null && senses.DistanceToTarget < MeleeDistance && senses.TargetInSight)
                {
                    if (entityBehaviour.Target == GameManager.Instance.PlayerEntityBehaviour)
                        damage = ApplyDamageToPlayer(weapon);
                    else
                        damage = ApplyDamageToNonPlayer(weapon);
                }
                else
                {
                    sounds.PlayMissSound(weapon);
                }

                if (DaggerfallUnity.Settings.CombatVoices && entity.EntityType == EntityTypes.EnemyClass && Random.Range(1, 101) <= 20)
                {
                    Genders gender;
                    if (mobile.Summary.Enemy.Gender == MobileGender.Male || entity.MobileEnemy.ID == (int)MobileTypes.Knight_CityWatch)
                        gender = Genders.Male;
                    else
                        gender = Genders.Female;

                    sounds.PlayCombatVoice(gender, true);
                }
            }
        }

        private void BowDamage()
        {
            if (entityBehaviour)
            {
                // Can we see target? Then apply damage.
                if (senses.TargetInSight)
                {
                    EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
                    if (entityBehaviour.Target == GameManager.Instance.PlayerEntityBehaviour)
                        damage = ApplyDamageToPlayer(entity.ItemEquipTable.GetItem(Items.EquipSlots.RightHand));
                    else
                        damage = ApplyDamageToNonPlayer(entity.ItemEquipTable.GetItem(Items.EquipSlots.RightHand), true);

                    // Play arrow sound and add arrow to target inventory
                    if (entityBehaviour.Target == GameManager.Instance.PlayerEntityBehaviour)
                        GameManager.Instance.PlayerObject.SendMessage("PlayArrowSound");
                    else
                    {
                        EnemySounds targetSounds = entityBehaviour.Target.GetComponent<EnemySounds>();
                        targetSounds.PlayArrowSound();
                    }
                    Items.DaggerfallUnityItem arrow = Items.ItemBuilder.CreateItem(Items.ItemGroups.Weapons, (int)Items.Weapons.Arrow);
                    entityBehaviour.Target.Entity.Items.AddItem(arrow);
                }
            }
        }

        private int ApplyDamageToPlayer(Items.DaggerfallUnityItem weapon)
        {
            const int doYouSurrenderToGuardsTextID = 15;

            EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;

            // Calculate damage
            damage = FormulaHelper.CalculateAttackDamage(entity, playerEntity, weapon, -1);

            // Break any "normal power" concealment effects on enemy
            if (entity.IsMagicallyConcealedNormalPower && damage > 0)
                EntityEffectManager.BreakNormalPowerConcealmentEffects(entityBehaviour);

            // Tally player's dodging skill
            playerEntity.TallySkill(DFCareer.Skills.Dodging, 1);

            if (damage > 0)
            {
                if (entity.MobileEnemy.ID == (int)MobileTypes.Knight_CityWatch)
                {
                    // If hit by a guard, lower reputation and show the surrender dialogue
                    if (!playerEntity.HaveShownSurrenderToGuardsDialogue && playerEntity.CrimeCommitted != PlayerEntity.Crimes.None)
                    {
                        playerEntity.LowerRepForCrime();

                        DaggerfallMessageBox messageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager);
                        messageBox.SetTextTokens(DaggerfallUnity.Instance.TextProvider.GetRSCTokens(doYouSurrenderToGuardsTextID));
                        messageBox.ParentPanel.BackgroundColor = Color.clear;
                        messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
                        messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
                        messageBox.OnButtonClick += SurrenderToGuardsDialogue_OnButtonClick;
                        messageBox.Show();

                        playerEntity.HaveShownSurrenderToGuardsDialogue = true;
                    }
                    // Surrender dialogue has been shown and player refused to surrender
                    // Guard damages player if player can survive hit, or if hit is fatal but guard rejects player's forced surrender
                    else if (playerEntity.CurrentHealth > damage || !playerEntity.SurrenderToCityGuards(false))
                        SendDamageToPlayer();
                }
                else
                    SendDamageToPlayer();
            }

            return damage;
        }

        private int ApplyDamageToNonPlayer(Items.DaggerfallUnityItem weapon, bool bowAttack = false)
        {
            // TODO: Merge with hit code in WeaponManager to eliminate duplicate code
            EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
            EnemyEntity targetEntity = entityBehaviour.Target.Entity as EnemyEntity;
            EnemySounds targetSounds = entityBehaviour.Target.GetComponent<EnemySounds>();
            EnemyMotor targetMotor = entityBehaviour.Target.transform.GetComponent<EnemyMotor>();

            // Calculate damage
            damage = FormulaHelper.CalculateAttackDamage(entity, targetEntity, weapon, -1);

            // Break any "normal power" concealment effects on enemy
            if (entity.IsMagicallyConcealedNormalPower && damage > 0)
                EntityEffectManager.BreakNormalPowerConcealmentEffects(entityBehaviour);

            // Play hit sound and trigger blood splash at hit point
            if (damage > 0)
            {
                targetSounds.PlayHitSound(weapon);

                EnemyBlood blood = entityBehaviour.Target.transform.GetComponent<EnemyBlood>();

                Vector3 bloodPos = entityBehaviour.Target.transform.position;
                CharacterController targetController = entityBehaviour.Target.transform.GetComponent<CharacterController>();
                bloodPos.y += targetController.height / 8;

                if (blood)
                {
                    blood.ShowBloodSplash(targetEntity.MobileEnemy.BloodIndex, bloodPos);
                }

                // Knock back enemy based on damage and enemy weight
                if (targetMotor)
                {
                    if (targetMotor.KnockBackSpeed <= (5 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)) &&
                        entityBehaviour.EntityType == EntityTypes.EnemyClass ||
                        targetEntity.MobileEnemy.Weight > 0)
                    {
                        float enemyWeight = targetEntity.GetWeightInClassicUnits();
                        float tenTimesDamage = damage * 10;
                        float twoTimesDamage = damage * 2;

                        float knockBackAmount = ((tenTimesDamage - enemyWeight) * 256) / (enemyWeight + tenTimesDamage) * twoTimesDamage;
                        float knockBackSpeed = (tenTimesDamage / enemyWeight) * (twoTimesDamage - (knockBackAmount / 256));
                        knockBackSpeed /= (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10);

                        if (knockBackSpeed < (15 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)))
                            knockBackSpeed = (15 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10));
                        targetMotor.KnockBackSpeed = knockBackSpeed;
                        targetMotor.KnockBackDirection = transform.forward;
                    }
                }

                if (DaggerfallUnity.Settings.CombatVoices && entityBehaviour.Target.EntityType == EntityTypes.EnemyClass && Random.Range(1, 101) <= 40)
                {
                    DaggerfallMobileUnit targetMobileUnit = entityBehaviour.Target.GetComponentInChildren<DaggerfallMobileUnit>();
                    Genders gender;
                    if (targetMobileUnit.Summary.Enemy.Gender == MobileGender.Male || targetEntity.MobileEnemy.ID == (int)MobileTypes.Knight_CityWatch)
                        gender = Genders.Male;
                    else
                        gender = Genders.Female;

                    bool heavyDamage = damage >= targetEntity.MaxHealth / 4;
                    targetSounds.PlayCombatVoice(gender, false, heavyDamage);
                }
            }
            else
            {
                WeaponTypes weaponType = WeaponTypes.Melee;
                if (weapon != null)
                    weaponType = DaggerfallUnity.Instance.ItemHelper.ConvertItemToAPIWeaponType(weapon);

                if ((!bowAttack && !targetEntity.MobileEnemy.ParrySounds) || weaponType == WeaponTypes.Melee)
                    sounds.PlayMissSound(weapon);
                else if (targetEntity.MobileEnemy.ParrySounds)
                    targetSounds.PlayParrySound();
            }

            targetEntity.DecreaseHealth(damage);

            // Assign "cast when strikes" to target entity
            if (weapon != null && weapon.IsEnchanted)
            {
                EntityEffectManager enemyEffectManager = targetEntity.EntityBehaviour.GetComponent<EntityEffectManager>();
                if (enemyEffectManager)
                    enemyEffectManager.StrikeWithItem(weapon, entityBehaviour);
            }

            if (targetMotor)
            {
                targetMotor.MakeEnemyHostileToAttacker(entityBehaviour);
            }

            return damage;
        }

        private void SurrenderToGuardsDialogue_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            sender.CloseWindow();
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
                GameManager.Instance.PlayerEntity.SurrenderToCityGuards(true);
            else
                SendDamageToPlayer();
        }

        private void SendDamageToPlayer()
        {
            GameManager.Instance.PlayerObject.SendMessage("RemoveHealth", damage);

            EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
            Items.DaggerfallUnityItem weapon = entity.ItemEquipTable.GetItem(Items.EquipSlots.RightHand);
            if (weapon == null)
                weapon = entity.ItemEquipTable.GetItem(Items.EquipSlots.LeftHand);
            if (weapon != null)
                GameManager.Instance.PlayerObject.SendMessage("PlayWeaponHitSound");
            else
                GameManager.Instance.PlayerObject.SendMessage("PlayWeaponlessHitSound");
        }

        #endregion
    }
}