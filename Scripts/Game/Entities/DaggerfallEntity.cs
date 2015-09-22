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
    /// Common entity type parent for any living, moving entity in the world.
    /// </summary>
    public class DaggerfallEntity
    {
        #region Fields

        Genders gender;
        RaceTemplate race;
        DFCareer career;
        string name;
        int level;
        int faceIndex;
        DaggerfallStats stats;
        DaggerfallSkills skills;
        PlayerReflexes reflexes;
        int maxHealth;
        int currentHealth;
        int currentMagicka;
        int currentFatigue;

        #endregion

        #region Properties

        public Genders Gender { get { return gender; } }
        public RaceTemplate Race { get { return race; } }
        public DFCareer Career { get { return career; } }
        public string Name { get { return name; } }
        public int Level { get { return level; } }
        public int FaceIndex { get { return faceIndex; } }
        public PlayerReflexes Reflexes { get { return reflexes; } }
        public DaggerfallStats Stats { get { return stats; } }
        public DaggerfallSkills Skills { get { return skills; } }
        public int MaxHealth { get { return maxHealth; } }
        public int CurrentHealth { get { return CurrentHealth; } }
        public int MaxFatigue { get { return stats.Strength + stats.Endurance; } }
        public int CurrentFatigue { get { return currentFatigue; } }
        public int MaxMagicka { get { return FormulaHelper.SpellPoints(stats.Intelligence, career.SpellPointMultiplierValue); } }
        public int CurrentMagicka { get { return currentMagicka; } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Assigns entity from a character sheet.
        /// </summary>
        public void AssignCharacter(CharacterSheet character, int level = 1, int maxHealth = 0)
        {
            this.level = level;
            this.gender = character.gender;
            this.race = character.race;
            this.career = character.career;
            this.name = character.name;
            this.faceIndex = character.faceIndex;
            this.stats = character.workingStats;
            this.skills = character.workingSkills;
            this.reflexes = character.reflexes;

            if (maxHealth <= 0)
                this.maxHealth = FormulaHelper.RollMaxHealth(level, stats.Endurance, career.HitPointsPerLevelOrMonsterLevel);
            else
                this.maxHealth = maxHealth;
        }

        #endregion

        #region Temporary Effects

        // Following are temporary effects for initial releases
        // Will be migrated to a full effect system later

        public int IncreaseHealth(int amount)
        {
            currentHealth += amount;
            if (currentHealth > MaxHealth)
                currentHealth = maxHealth;

            return currentHealth;
        }

        public int DecreaseHealth(int amount)
        {
            currentHealth -= amount;
            if (currentHealth < 0)
            {
                currentHealth = 0;
                RaiseOnDeathEvent();
            }

            return currentHealth;
        }

        public int IncreaseFatigue(int amount)
        {
            currentFatigue += amount;
            if (currentFatigue > MaxFatigue)
                currentFatigue = MaxFatigue;

            return currentFatigue;
        }

        public int DecreaseFatigue(int amount)
        {
            currentFatigue -= amount;
            if (currentFatigue < 0)
            {
                currentFatigue = 0;
                RaiseOnExhaustedEvent();
            }

            return currentFatigue;
        }

        public int IncreaseMagicka(int amount)
        {
            currentMagicka += amount;
            if (currentMagicka > MaxMagicka)
                currentMagicka = MaxMagicka;

            return currentMagicka;
        }

        public int DecreaseMagicka(int amount)
        {
            currentMagicka -= amount;
            if (currentMagicka < 0)
            {
                currentMagicka = 0;
                RaiseOnMagickaDrainedEvent();
            }

            return currentMagicka;
        }

        #endregion

        #region Temporary Events

        // These tie in with temporary effects and will be moved later

        public delegate void OnDeathHandler(DaggerfallEntity entity);
        public event OnDeathHandler OnDeath;
        void RaiseOnDeathEvent()
        {
            if (OnDeath != null)
                OnDeath(this);
        }

        public delegate void OnExhaustedHandler(DaggerfallEntity entity);
        public event OnExhaustedHandler OnExhausted;
        void RaiseOnExhaustedEvent()
        {
            if (OnExhausted != null)
                OnExhausted(this);
        }

        public delegate void OnMagickaDrainedHandler(DaggerfallEntity entity);
        public event OnMagickaDrainedHandler OnMagickaDrained;
        void RaiseOnMagickaDrainedEvent()
        {
            if (OnMagickaDrained != null)
                OnMagickaDrained(this);
        }

        #endregion
    }
}