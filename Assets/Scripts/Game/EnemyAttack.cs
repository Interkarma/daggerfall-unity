// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Allofich
// 
// Notes:
//

using UnityEngine;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Temporary enemy attack.
    /// </summary>
    [RequireComponent(typeof(EnemySenses))]
    public class EnemyAttack : MonoBehaviour
    {
        public const float minRangedDistance = 240 * MeshReader.GlobalScale; // 6m
        public const float maxRangedDistance = 2048 * MeshReader.GlobalScale; // 51.2m
        public float MeleeDistance = 2.25f;                // Maximum distance for melee attack
        public float ClassicMeleeDistanceVsAI = 1.5f;      // Maximum distance for melee attack vs other AI in classic AI mode
        public float MeleeTimer = 0;                       // Must be 0 for a melee attack or touch spell to be done
        public DaggerfallMissile ArrowMissilePrefab;

        EnemyMotor motor;
        EnemySenses senses;
        EnemySounds sounds;
        DaggerfallMobileUnit mobile;
        DaggerfallEntityBehaviour entityBehaviour;
        int damage = 0;

        void Start()
        {
            motor = GetComponent<EnemyMotor>();
            senses = GetComponent<EnemySenses>();
            sounds = GetComponent<EnemySounds>();
            mobile = GetComponentInChildren<DaggerfallMobileUnit>();
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
        }

        void FixedUpdate()
        {
            // Unable to attack if AI disabled or paralyzed
            if (GameManager.Instance.DisableAI || entityBehaviour.Entity.IsParalyzed)
                return;

            // Unable to attack when playing certain oneshot anims
            if (mobile && mobile.IsPlayingOneShot() && mobile.OneShotPauseActionsWhilePlaying())
                return;

            // Countdown to next melee attack
            MeleeTimer -= Time.deltaTime;

            if (MeleeTimer < 0)
                MeleeTimer = 0;

            EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
            int speed = entity.Stats.LiveSpeed;

            // Note: Speed comparison here is reversed from classic. Classic's way makes fewer attack
            // attempts at higher speeds, so it seems backwards.
            if (GameManager.ClassicUpdate && (DFRandom.rand() % speed >= (speed >> 3) + 6 && MeleeTimer == 0))
            {
                if (!MeleeAnimation())
                    return;

                ResetMeleeTimer();
            }
        }

        void Update()
        {
            // Unable to attack if paralyzed
            if (entityBehaviour.Entity.IsParalyzed)
                return;

            // If a melee attack has reached the damage frame we can run a melee attempt
            if (mobile.DoMeleeDamage)
            {
                MeleeDamage();
                mobile.DoMeleeDamage = false;
            }
            // If a bow attack has reached the shoot frame we can shoot an arrow
            else if (mobile.ShootArrow)
            {
                ShootBow();
                mobile.ShootArrow = false;

                DaggerfallAudioSource dfAudioSource = GetComponent<DaggerfallAudioSource>();
                if (dfAudioSource)
                    dfAudioSource.PlayOneShot((int)SoundClips.ArrowShoot, 1, 1.0f);
            }
        }

        public void ResetMeleeTimer()
        {
            MeleeTimer = Random.Range(1500, 3000 + 1);
            MeleeTimer -= 50 * (GameManager.Instance.PlayerEntity.Level - 10);

            // Note: In classic, what happens here is
            // meleeTimer += 450 * (enemydata[130] - 2);
            // Looks like this was meant to reference the game reflexes setting,
            // which is stored in playerentitydata[130].
            // Instead enemydata[130] seems to instead always be 0, the equivalent of
            // "very high" reflexes, regardless of what the game reflexes are.
            // Here, we use the reflexes data as was intended.
            MeleeTimer += 450 * ((int)GameManager.Instance.PlayerEntity.Reflexes - 2);

            if (MeleeTimer < 0)
                MeleeTimer = 0;

            MeleeTimer /= 980; // Approximates classic frame update
        }

        public void BowDamage(Vector3 direction)
        {
            if (senses.Target == null)
                return;

            EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
            if (senses.Target == GameManager.Instance.PlayerEntityBehaviour)
                damage = ApplyDamageToPlayer(entity.ItemEquipTable.GetItem(Items.EquipSlots.RightHand));
            else
                damage = ApplyDamageToNonPlayer(entity.ItemEquipTable.GetItem(Items.EquipSlots.RightHand), direction, true);

            Items.DaggerfallUnityItem arrow = Items.ItemBuilder.CreateItem(Items.ItemGroups.Weapons, (int)Items.Weapons.Arrow);
            senses.Target.Entity.Items.AddItem(arrow);
        }

        #region Private Methods

        private bool MeleeAnimation()
        {
            // Are we in range and facing target? Then start attack.
            if (senses.TargetInSight && senses.TargetIsWithinYawAngle(22.5f, senses.LastKnownTargetPos))
            {
                float distance = MeleeDistance;
                // Classic uses separate melee distance for targeting player and for targeting other AI
                if (!DaggerfallUnity.Settings.EnhancedCombatAI && senses.Target != GameManager.Instance.PlayerEntityBehaviour)
                    distance = ClassicMeleeDistanceVsAI;

                // Take the rate of target approach into account when deciding if to attack
                if (senses.DistanceToTarget > distance + senses.TargetRateOfApproach)
                    return false;

                // Set melee animation state
                mobile.ChangeEnemyState(MobileStates.PrimaryAttack);

                // Play melee sound
                if (sounds)
                {
                    sounds.PlayAttackSound();
                }

                return true;
            }

            return false;
        }

        private void MeleeDamage()
        {
            if (entityBehaviour)
            {
                EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
                EnemyEntity targetEntity = null;

                if (senses.Target != null && senses.Target != GameManager.Instance.PlayerEntityBehaviour)
                    targetEntity = senses.Target.Entity as EnemyEntity;

                // Switch to hand-to-hand if enemy is immune to weapon
                Items.DaggerfallUnityItem weapon = entity.ItemEquipTable.GetItem(Items.EquipSlots.RightHand);
                if (weapon != null && targetEntity != null && targetEntity.MobileEnemy.MinMetalToHit > (Items.WeaponMaterialTypes)weapon.NativeMaterialValue)
                    weapon = null;

                damage = 0;

                // Melee hit detection, matched to classic
                if (senses.Target != null && senses.TargetInSight && (senses.DistanceToTarget <= 0.25f
                    || (senses.DistanceToTarget <= MeleeDistance && senses.TargetIsWithinYawAngle(35.156f, senses.Target.transform.position))))
                {
                    if (senses.Target == GameManager.Instance.PlayerEntityBehaviour)
                        damage = ApplyDamageToPlayer(weapon);
                    else
                        damage = ApplyDamageToNonPlayer(weapon, transform.forward);
                }
                // Handle bashing door
                else if (motor.Bashing && senses.LastKnownDoor != null && senses.DistanceToDoor <= MeleeDistance && !senses.LastKnownDoor.IsOpen)
                {
                    senses.LastKnownDoor.AttemptBash(false);
                }
                else
                {
                    sounds.PlayMissSound(weapon);
                }

                if (DaggerfallUnity.Settings.CombatVoices && entity.EntityType == EntityTypes.EnemyClass && Dice100.SuccessRoll(20))
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

        private void ShootBow()
        {
            if (entityBehaviour)
            {
                DaggerfallMissile missile = Instantiate(ArrowMissilePrefab);
                if (missile)
                {
                    missile.Caster = entityBehaviour;
                    missile.TargetType = TargetTypes.SingleTargetAtRange;
                    missile.ElementType = ElementTypes.None;
                    missile.IsArrow = true;
                }
            }
        }

        private int ApplyDamageToPlayer(Items.DaggerfallUnityItem weapon)
        {
            const int doYouSurrenderToGuardsTextID = 15;

            EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;

            // Calculate damage
            damage = FormulaHelper.CalculateAttackDamage(entity, playerEntity, false, 0, weapon);

            // Break any "normal power" concealment effects on enemy
            if (entity.IsMagicallyConcealedNormalPower && damage > 0)
                EntityEffectManager.BreakNormalPowerConcealmentEffects(entityBehaviour);

            // Tally player's dodging skill
            playerEntity.TallySkill(DFCareer.Skills.Dodging, 1);

            // Handle Strikes payload from enemy to player target - this could change damage amount
            if (damage > 0 && weapon != null && weapon.IsEnchanted)
            {
                EntityEffectManager effectManager = GetComponent<EntityEffectManager>();
                if (effectManager)
                    damage = effectManager.DoItemEnchantmentPayloads(EnchantmentPayloadFlags.Strikes, weapon, entity.Items, playerEntity.EntityBehaviour, damage);
            }

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
            else
                sounds.PlayMissSound(weapon);

            return damage;
        }

        private int ApplyDamageToNonPlayer(Items.DaggerfallUnityItem weapon, Vector3 direction, bool bowAttack = false)
        {
            if (senses.Target == null)
                return 0;
            // TODO: Merge with hit code in WeaponManager to eliminate duplicate code
            EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
            EnemyEntity targetEntity = senses.Target.Entity as EnemyEntity;
            EnemySounds targetSounds = senses.Target.GetComponent<EnemySounds>();
            EnemyMotor targetMotor = senses.Target.transform.GetComponent<EnemyMotor>();

            // Calculate damage
            damage = FormulaHelper.CalculateAttackDamage(entity, targetEntity, false, 0, weapon);

            // Break any "normal power" concealment effects on enemy
            if (entity.IsMagicallyConcealedNormalPower && damage > 0)
                EntityEffectManager.BreakNormalPowerConcealmentEffects(entityBehaviour);

            // Play hit sound and trigger blood splash at hit point
            if (damage > 0)
            {
                targetSounds.PlayHitSound(weapon);

                EnemyBlood blood = senses.Target.transform.GetComponent<EnemyBlood>();
                CharacterController targetController = senses.Target.transform.GetComponent<CharacterController>();
                Vector3 bloodPos = senses.Target.transform.position + targetController.center;
                bloodPos.y += targetController.height / 8;

                if (blood)
                {
                    blood.ShowBloodSplash(targetEntity.MobileEnemy.BloodIndex, bloodPos);
                }

                // Knock back enemy based on damage and enemy weight
                if (targetMotor && (targetMotor.KnockbackSpeed <= (5 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10))
                        && (entityBehaviour.EntityType == EntityTypes.EnemyClass || targetEntity.MobileEnemy.Weight > 0)))
                {
                    float enemyWeight = targetEntity.GetWeightInClassicUnits();
                    float tenTimesDamage = damage * 10;
                    float twoTimesDamage = damage * 2;

                    float knockBackAmount = ((tenTimesDamage - enemyWeight) * 256) / (enemyWeight + tenTimesDamage) * twoTimesDamage;
                    float KnockbackSpeed = (tenTimesDamage / enemyWeight) * (twoTimesDamage - (knockBackAmount / 256));
                    KnockbackSpeed /= (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10);

                    if (KnockbackSpeed < (15 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10)))
                        KnockbackSpeed = (15 / (PlayerSpeedChanger.classicToUnitySpeedUnitRatio / 10));
                    targetMotor.KnockbackSpeed = KnockbackSpeed;
                    targetMotor.KnockbackDirection = direction;
                }

                if (DaggerfallUnity.Settings.CombatVoices && senses.Target.EntityType == EntityTypes.EnemyClass && Dice100.SuccessRoll(40))
                {
                    DaggerfallMobileUnit targetMobileUnit = senses.Target.GetComponentInChildren<DaggerfallMobileUnit>();
                    Genders gender;
                    if (targetMobileUnit.Summary.Enemy.Gender == MobileGender.Male || targetEntity.MobileEnemy.ID == (int)MobileTypes.Knight_CityWatch)
                        gender = Genders.Male;
                    else
                        gender = Genders.Female;

                    targetSounds.PlayCombatVoice(gender, false, damage >= targetEntity.MaxHealth / 4);
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

            // Handle Strikes payload from enemy to non-player target - this could change damage amount
            if (weapon != null && weapon.IsEnchanted)
            {
                EntityEffectManager effectManager = GetComponent<EntityEffectManager>();
                if (effectManager)
                    damage = effectManager.DoItemEnchantmentPayloads(EnchantmentPayloadFlags.Strikes, weapon, entity.Items, targetEntity.EntityBehaviour, damage);
            }

            targetEntity.DecreaseHealth(damage);

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
