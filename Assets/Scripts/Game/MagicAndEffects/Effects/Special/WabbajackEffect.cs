// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Numidium
// Contributors:    
// 
// Notes:
//
using UnityEngine;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Questing;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Used by the Wabbajack artifact staff to change enemies into random monsters.
    /// </summary>
    public class WabbajackEffect : BaseEntityEffect
    {
        public static readonly string EffectKey = "WabbajackEffect";
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

        public override void MagicRound()
        {
            base.MagicRound();
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;
            if (entityBehaviour.Entity is EnemyEntity)
            {
                EnemyEntity enemy = (EnemyEntity)entityBehaviour.Entity;
                if (enemy.WabbajackActive)
                    return;

                MobileTypes enemyType = careerIDs[Random.Range(0, careerIDs.Length)];
                if ((int)enemyType == enemy.CareerIndex)
                {
                    enemyType = (MobileTypes)(((int)enemyType + 1) % careerIDs.Length);
                }
                Transform parentTransform = entityBehaviour.gameObject.transform.parent;
                // Do not disable enemy if in use by the quest system
                QuestResourceBehaviour questResourceBehaviour = entityBehaviour.GetComponent<QuestResourceBehaviour>();
                if (questResourceBehaviour)
                {
                    if (!questResourceBehaviour.IsFoeDead)
                        return;
                }
                entityBehaviour.gameObject.SetActive(false);
                GameObject gameObject = GameObjectHelper.CreateEnemy(HardStrings.enemyNames[(int)enemyType], enemyType, entityBehaviour.transform.localPosition, parentTransform);
                DaggerfallEntityBehaviour newEnemyBehaviour = gameObject.GetComponent<DaggerfallEntityBehaviour>();
                EnemyEntity newEnemy = (EnemyEntity)newEnemyBehaviour.Entity;
                newEnemy.WabbajackActive = true;
                newEnemy.CurrentHealth -= enemy.MaxHealth - enemy.CurrentHealth; // carry over damage to new monster
            }
        }

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            bypassSavingThrows = true;
        }
    }
}
