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

        EnemyMotor motor;
        EnemySenses senses;
        EnemySounds sounds;
        DaggerfallMobileUnit mobile;
        DaggerfallEntityBehaviour entityBehaviour;
        float meleeTimer = 0;
        int damage = 0;
        float classicUpdateTimer = 0f;
        bool classicUpdate = false;

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
            meleeTimer -= Time.deltaTime;

            if (meleeTimer < 0)
                meleeTimer = 0;

            EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
            int speed = entity.Stats.LiveSpeed;

            // Note: Speed comparison here is reversed from classic. Classic's way makes fewer attack
            // attempts at higher speeds, so it seems backwards.
            if (classicUpdate && (DFRandom.rand() % speed >= (speed >> 3) + 6 && meleeTimer == 0))
            {
                MeleeAnimation();

                meleeTimer = Random.Range(1500, 3001);
                meleeTimer -= 50 * (GameManager.Instance.PlayerEntity.Level - 10);

                // Note: In classic, what happens here is
                // meleeTimer += 450 * (enemydata[130] - 2);
                // Apparently this was meant to reference the game reflexes setting,
                // which is stored in playerentitydata[130].
                // Instead enemydata[130] seems to instead always be 0, the equivalent of
                // "very high" reflexes, regardless of what the game reflexes are.
                // Here, we use the reflexes data as was intended.
                meleeTimer += 450 * ((int)GameManager.Instance.PlayerEntity.Reflexes - 2);

                if (meleeTimer > 100000 || meleeTimer < 0)
                    meleeTimer = 1500;

                meleeTimer /= 980; // Approximates classic frame update
            }
        }

        #region Private Methods

        private void MeleeAnimation()
        {
            // Are we in range and facing player? Then start attack.
            if (senses.PlayerInSight)
            {
                // Take the speed of movement during the attack animation into account when deciding if to attack
                EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
                float attackSpeed = ((entity.Stats.LiveSpeed + PlayerSpeedChanger.dfWalkBase) / PlayerSpeedChanger.classicToUnitySpeedUnitRatio) / EnemyMotor.AttackSpeedDivisor;

                if (senses.DistanceToPlayer >= MeleeDistance + attackSpeed)
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

                damage = 0;

                // Are we still in range and facing player? Then apply melee damage.
                if (senses.DistanceToPlayer < MeleeDistance && senses.PlayerInSight)
                {
                    damage = ApplyDamageToPlayer();
                }

                Items.DaggerfallUnityItem weapon = entity.ItemEquipTable.GetItem(Items.EquipSlots.RightHand);
                if (weapon == null)
                    weapon = entity.ItemEquipTable.GetItem(Items.EquipSlots.LeftHand);

                if (damage <= 0)
                    sounds.PlayMissSound(weapon);
            }
        }

        private void BowDamage()
        {
            if (entityBehaviour)
            {
                // Can we see player? Then apply damage.
                if (senses.PlayerInSight)
                {
                    damage = ApplyDamageToPlayer();

                    // Play arrow sound and add arrow to player inventory
                    GameManager.Instance.PlayerObject.SendMessage("PlayArrowSound");

                    Items.DaggerfallUnityItem arrow = Items.ItemBuilder.CreateItem(Items.ItemGroups.Weapons, (int)Items.Weapons.Arrow);
                    GameManager.Instance.PlayerEntity.Items.AddItem(arrow);
                }
            }
        }

        private int ApplyDamageToPlayer()
        {
            const int doYouSurrenderToGuardsTextID = 15;

            EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;

            // Calculate damage
            damage = FormulaHelper.CalculateAttackDamage(entity, playerEntity, (int)(Items.EquipSlots.RightHand), -1);

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