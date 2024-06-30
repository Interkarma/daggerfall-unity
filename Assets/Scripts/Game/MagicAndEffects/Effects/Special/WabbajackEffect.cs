// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Numidium
// Contributors:    Gavin Clayton (interkarma@dfworkshop.net)
// 
// Notes:
//
using UnityEngine;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Questing;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Used by the Wabbajack artifact staff to change enemies into random monsters.
    /// </summary>
    public class WabbajackEffect : BaseEntityEffect
    {
        public static readonly string EffectKey = ArtifactsSubTypes.Wabbajack.ToString();
        public static readonly MobileTypes[] careerIDs = {
                                                           MobileTypes.Rat,
                                                           MobileTypes.Imp,
                                                           MobileTypes.Spriggan,
                                                           MobileTypes.GiantBat,
                                                           MobileTypes.GrizzlyBear,
                                                           MobileTypes.Spider,
                                                           MobileTypes.Nymph,
                                                           MobileTypes.Harpy,
                                                           MobileTypes.SkeletalWarrior,
                                                           MobileTypes.Giant,
                                                           MobileTypes.Zombie,
                                                           MobileTypes.GiantScorpion,
                                                           MobileTypes.IronAtronach,
                                                           MobileTypes.FleshAtronach,
                                                           MobileTypes.IceAtronach,
                                                           MobileTypes.FireAtronach,
                                                           MobileTypes.Lich
                                                          };

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ShowSpellIcon = false;
            properties.EnchantmentPayloadFlags = EnchantmentPayloadFlags.Strikes;
            bypassSavingThrows = true;
        }

        #region Payloads

        public override PayloadCallbackResults? EnchantmentPayloadCallback(EnchantmentPayloadFlags context, EnchantmentParam? param = null, DaggerfallEntityBehaviour sourceEntity = null, DaggerfallEntityBehaviour targetEntity = null, DaggerfallUnityItem sourceItem = null, int sourceDamage = 0)
        {
            base.EnchantmentPayloadCallback(context, param, sourceEntity, targetEntity, sourceItem, sourceDamage);

            // Validate
            if (context != EnchantmentPayloadFlags.Strikes || targetEntity == null)
                return null;

            // Change target enemy
            if (targetEntity.Entity is EnemyEntity)
            {
                // Get enemy entity - cannot have Wabbajack active already
                EnemyEntity enemy = (EnemyEntity)targetEntity.Entity;
                if (enemy == null || enemy.WabbajackActive)
                    return null;

                // Do not disable enemy if in use by the quest system
                QuestResourceBehaviour questResourceBehaviour = targetEntity.GetComponent<QuestResourceBehaviour>();
                if (questResourceBehaviour && !questResourceBehaviour.IsFoeDead)
                    return null;

                // Get new enemy career and transform
                MobileTypes enemyType;
                do {
                    enemyType = careerIDs[Random.Range(0, careerIDs.Length)];
                } while ((int)enemyType == enemy.CareerIndex);
                Transform parentTransform = targetEntity.gameObject.transform.parent;

                string[] enemyNames = TextManager.Instance.GetLocalizedTextList("enemyNames");
                if (enemyNames == null)
                    throw new System.Exception("enemyNames array text not found");

                // Switch entity
                targetEntity.gameObject.SetActive(false);
                GameObject gameObject = GameObjectHelper.CreateEnemy(enemyNames[(int)enemyType], enemyType, targetEntity.transform.localPosition, MobileGender.Unspecified, parentTransform);
                gameObject.transform.localRotation = targetEntity.gameObject.transform.localRotation;
                DaggerfallEntityBehaviour newEnemyBehaviour = gameObject.GetComponent<DaggerfallEntityBehaviour>();
                EnemyEntity newEnemy = (EnemyEntity)newEnemyBehaviour.Entity;
                newEnemy.WabbajackActive = true;
                newEnemy.CurrentHealth -= enemy.MaxHealth - enemy.CurrentHealth; // carry over damage to new monster
            }

            return null;
        }

        #endregion
    }
}
