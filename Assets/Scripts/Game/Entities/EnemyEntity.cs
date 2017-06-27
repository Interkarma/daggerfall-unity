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

                // Enemy class is levelled to player and uses similar health rules
                level = GameManager.Instance.PlayerEntity.Level;
                maxHealth = FormulaHelper.RollEnemyClassMaxHealth(level, career.HitPointsPerLevelOrMonsterLevel);

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

        public DFCareer.EnemyGroups GetEnemyGroup()
        {
            switch (careerIndex)
            {
                case (int)MonsterCareers.Rat:
                case (int)MonsterCareers.GiantBat:
                case (int)MonsterCareers.GrizzlyBear:
                case (int)MonsterCareers.SabertoothTiger:
                case (int)MonsterCareers.Spider:
                case (int)MonsterCareers.Slaughterfish:
                case (int)MonsterCareers.GiantScorpion:
                case (int)MonsterCareers.Dragonling:
                case (int)MonsterCareers.Horse_Invalid:             // (grouped as undead in classic)
                case (int)MonsterCareers.Dragonling_Alternate:      // (grouped as undead in classic)
                    return DFCareer.EnemyGroups.Animals;
                case (int)MonsterCareers.Imp:
                case (int)MonsterCareers.Spriggan:
                case (int)MonsterCareers.Orc:
                case (int)MonsterCareers.Centaur:
                case (int)MonsterCareers.Werewolf:
                case (int)MonsterCareers.Nymph:
                case (int)MonsterCareers.OrcSergeant:
                case (int)MonsterCareers.Harpy:
                case (int)MonsterCareers.Wereboar:
                case (int)MonsterCareers.Giant:
                case (int)MonsterCareers.OrcShaman:
                case (int)MonsterCareers.Gargoyle:
                case (int)MonsterCareers.OrcWarlord:
                case (int)MonsterCareers.Dreugh:                    // (grouped as undead in classic)
                case (int)MonsterCareers.Lamia:                     // (grouped as undead in classic)
                    return DFCareer.EnemyGroups.Humanoid;
                case (int)MonsterCareers.SkeletalWarrior:
                case (int)MonsterCareers.Zombie:                    // (grouped as animal in classic)
                case (int)MonsterCareers.Ghost:
                case (int)MonsterCareers.Mummy:
                case (int)MonsterCareers.Wraith:
                case (int)MonsterCareers.Vampire:
                case (int)MonsterCareers.VampireAncient:
                case (int)MonsterCareers.Lich:
                case (int)MonsterCareers.AncientLich:
                    return DFCareer.EnemyGroups.Undead;
                case (int)MonsterCareers.FrostDaedra:
                case (int)MonsterCareers.FireDaedra:
                case (int)MonsterCareers.Daedroth:
                case (int)MonsterCareers.DaedraSeducer:
                case (int)MonsterCareers.DaedraLord:
                    return DFCareer.EnemyGroups.Daedra;
                case (int)MonsterCareers.FireAtronach:
                case (int)MonsterCareers.IronAtronach:
                case (int)MonsterCareers.FleshAtronach:
                case (int)MonsterCareers.IceAtronach:
                    return DFCareer.EnemyGroups.None;

                default:
                    return DFCareer.EnemyGroups.None;
            }
        }

        #endregion
    }
}