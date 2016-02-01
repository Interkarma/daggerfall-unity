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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Player;

namespace DaggerfallWorkshop.Game.Entity
{
    /// <summary>
    /// Base entity type for all living GameObjects in the world.
    /// The Effect system (spells, diseases, etc.) operates on entities.
    /// </summary>
    [Serializable]
    public abstract class DaggerfallEntity
    {
        #region Fields

        protected Genders gender;
        protected DFCareer career = new DFCareer();
        protected string name;
        protected int level;
        protected DaggerfallStats stats;
        protected DaggerfallSkills skills;
        protected EntityItems items = new EntityItems();
        protected int maxHealth;
        protected int currentHealth;
        protected int currentFatigue;
        protected int currentMagicka;

        #endregion

        #region Properties

        public Genders Gender { get { return gender; } set { gender = value; } }
        public DFCareer Career { get { return career; } set { career = value; } } 
        public string Name { get { return name; } set { name = value; } }
        public int Level { get { return level; } set { level = value; } }
        public DaggerfallStats Stats { get { return stats; } set { stats.Copy(value); } }
        public DaggerfallSkills Skills { get { return skills; } set { skills.Copy(value); } }
        public EntityItems Items { get { return items; } set { items.ReplaceAll(value); } }
        public int MaxHealth { get { return maxHealth; } set { maxHealth = value; } }
        public int CurrentHealth { get { return currentHealth; } set { SetHealth(value); } }
        public int MaxFatigue { get { return stats.Strength + stats.Endurance; } }
        public int CurrentFatigue { get { return currentFatigue; } set { SetFatigue(value); } }
        public int MaxMagicka { get { return FormulaHelper.SpellPoints(stats.Intelligence, career.SpellPointMultiplierValue); } }
        public int CurrentMagicka { get { return currentMagicka; } set { SetMagicka(value); } }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Sets entity defaults during scene startup.
        /// Defaults should be overwritten by normal processes such as
        /// character creation, monster instantiation, and save game deserialization.
        /// </summary>
        public abstract void SetEntityDefaults();

        #endregion

        #region Temporary Effects

        // Following are temporary effects for initial releases
        // Will be migrated to a full effect system later

        public int IncreaseHealth(int amount)
        {
            return SetHealth(currentHealth + amount);
        }

        public int DecreaseHealth(int amount)
        {
            return SetHealth(currentHealth - amount);
        }

        public int SetHealth(int amount)
        {
            if (amount <= 0)
            {
                amount = 0;
                RaiseOnDeathEvent();
            }
            if (amount > MaxHealth) amount = MaxHealth;
            currentHealth = amount;

            return currentHealth;
        }

        public int IncreaseFatigue(int amount)
        {
            return SetFatigue(currentFatigue + amount);
        }

        public int DecreaseFatigue(int amount)
        {
            return SetFatigue(currentFatigue - amount);
        }

        public int SetFatigue(int amount)
        {
            if (amount <= 0)
            {
                amount = 0;
                RaiseOnExhaustedEvent();
            }
            if (amount > MaxFatigue) amount = MaxFatigue;
            currentFatigue = amount;

            return currentFatigue;
        }

        public int IncreaseMagicka(int amount)
        {
            return SetMagicka(currentMagicka + amount);
        }

        public int DecreaseMagicka(int amount)
        {
            return SetMagicka(currentMagicka - amount);
        }

        public int SetMagicka(int amount)
        {
            if (amount <= 0)
            {
                amount = 0;
                RaiseOnMagickaDepletedEvent();
            }
            if (amount > MaxMagicka) amount = MaxMagicka;
            currentMagicka = amount;

            return currentMagicka;
        }

        public void FillVitalSigns()
        {
            currentHealth = MaxHealth;
            currentFatigue = MaxFatigue;
            currentMagicka = MaxMagicka;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets list of primary skills.
        /// </summary>
        public List<DFCareer.Skills> GetPrimarySkills()
        {
            List<DFCareer.Skills> primarySkills = new List<DFCareer.Skills>();
            primarySkills.Add(career.PrimarySkill1);
            primarySkills.Add(career.PrimarySkill2);
            primarySkills.Add(career.PrimarySkill3);

            return primarySkills;
        }

        /// <summary>
        /// Gets list of major skills.
        /// </summary>
        public List<DFCareer.Skills> GetMajorSkills()
        {
            List<DFCareer.Skills> majorSkills = new List<DFCareer.Skills>();
            majorSkills.Add(career.MajorSkill1);
            majorSkills.Add(career.MajorSkill2);
            majorSkills.Add(career.MajorSkill3);

            return majorSkills;
        }

        /// <summary>
        /// Gets list of minor skills.
        /// </summary>
        public List<DFCareer.Skills> GetMinorSkills()
        {
            List<DFCareer.Skills> minorSkills = new List<DFCareer.Skills>();
            minorSkills.Add(career.MinorSkill1);
            minorSkills.Add(career.MinorSkill2);
            minorSkills.Add(career.MinorSkill3);
            minorSkills.Add(career.MinorSkill4);
            minorSkills.Add(career.MinorSkill5);
            minorSkills.Add(career.MinorSkill6);

            return minorSkills;
        }

        /// <summary>
        /// Gets list of miscellaneous skills.
        /// </summary>
        public List<DFCareer.Skills> GetMiscSkills()
        {
            List<DFCareer.Skills> primarySkills = GetPrimarySkills();
            List<DFCareer.Skills> majorSkills = GetMajorSkills();
            List<DFCareer.Skills> minorSkills = GetMinorSkills();

            List<DFCareer.Skills> miscSkills = new List<DFCareer.Skills>();
            for (int i = 0; i < DaggerfallSkills.Count; i++)
            {
                if (!primarySkills.Contains((DFCareer.Skills)i) &&
                    !majorSkills.Contains((DFCareer.Skills)i) &&
                    !minorSkills.Contains((DFCareer.Skills)i))
                {
                    miscSkills.Add((DFCareer.Skills)i);
                }
            }

            return miscSkills;
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

        public delegate void OnMagickaDepletedHandler(DaggerfallEntity entity);
        public event OnMagickaDepletedHandler OnMagickaDepleted;
        void RaiseOnMagickaDepletedEvent()
        {
            if (OnMagickaDepleted != null)
                OnMagickaDepleted(this);
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Gets class career template.
        /// Currently read from CLASS??.CFG. Would like to migrate this to a custom JSON format later.
        /// </summary>
        public static DFCareer GetClassCareerTemplate(ClassCareers career)
        {
            string filename = string.Format("CLASS{0:00}.CFG", (int)career);
            ClassFile file = new ClassFile();
            if (!file.Load(Path.Combine(DaggerfallUnity.Instance.Arena2Path, filename)))
                return null;

            return file.Career;
        }

        /// <summary>
        /// Gets monster career template.
        /// Currently read from MONSTER.BSA. Would like to migrate this to a custom JSON format later.
        /// </summary>
        /// <param name="career"></param>
        /// <returns></returns>
        public static DFCareer GetMonsterCareerTemplate(MonsterCareers career)
        {
            MonsterFile monsterFile = new MonsterFile();
            if (!monsterFile.Load(Path.Combine(DaggerfallUnity.Instance.Arena2Path, MonsterFile.Filename), FileUsage.UseMemory, true))
                throw new Exception("Could not load " + MonsterFile.Filename);

            return monsterFile.GetMonsterClass((int)career);
        }

        #endregion
    }
}