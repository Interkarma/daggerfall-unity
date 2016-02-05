// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Player;

namespace DaggerfallWorkshop.Game.Entity
{
    /// <summary>
    /// Implements DaggerfallEntity with properties specific to enemies.
    /// Currently enemy setup is bridging between old "demo" components and newer "game" systems.
    /// TODO: Migrate completely to "game" methods and simplify enemy instantiation and setup.
    /// </summary>
    public class EnemyEntity : DaggerfallEntity
    {
        #region Fields

        int careerIndex = -1;
        EntityTypes entityType = EntityTypes.None;
        MobileEnemy mobileEnemy;

        #endregion

        #region Properties

        public EntityTypes EntityType
        {
            get { return entityType; }
        }

        public int CareerIndex
        {
            get { return careerIndex; }
        }

        public MobileEnemy MobileEnemy
        {
            get { return mobileEnemy; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Assigns default entity settings.
        /// </summary>
        public override void SetEntityDefaults()
        {
        }

        /// <summary>
        /// Sets enemy career and prepares entity settings.
        /// </summary>
        public void SetEnemyCareer(MobileEnemy mobileEnemy, EntityTypes entityType)
        {
            if (entityType == EntityTypes.EnemyMonster)
            {
                careerIndex = (int)mobileEnemy.ID;
                career = GetMonsterCareerTemplate((MonsterCareers)careerIndex);
                stats.SetFromCareer(career);

                // Enemy monster has predefined level and health
                level = career.HitPointsPerLevelOrMonsterLevel;
                maxHealth = UnityEngine.Random.Range(mobileEnemy.MinHealth, mobileEnemy.MaxHealth + 1);
            }
            else if (entityType == EntityTypes.EnemyClass)
            {
                careerIndex = (int)mobileEnemy.ID - 128;
                career = GetClassCareerTemplate((ClassCareers)careerIndex);
                stats.SetFromCareer(career);

                // Enemy class is levelled to player and uses same health rules
                level = GameManager.Instance.PlayerEntity.Level;
                maxHealth = FormulaHelper.RollMaxHealth(level, stats.Endurance, career.HitPointsPerLevelOrMonsterLevel);

                // Enemy class damage is temporarily set by a fudged level multiplier
                // This will change once full entity setup and items are available
                const float damageMultiplier = 4f;
                mobileEnemy.MinDamage = (int)(level * damageMultiplier);
                mobileEnemy.MaxDamage = (int)((level + 2) * damageMultiplier);
            }
            else
            {
                career = new DFCareer();
                careerIndex = -1;
                return;
            }

            this.mobileEnemy = mobileEnemy;
            this.entityType = entityType;
            name = career.Name;
            
            FillVitalSigns();
        }

        #endregion
    }
}