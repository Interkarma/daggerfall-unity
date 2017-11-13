// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
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
using DaggerfallWorkshop.Game.Items;

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
        public const int NumberBodyParts = 7;

        protected Genders gender;
        protected DFCareer career = new DFCareer();
        protected string name;
        protected int level;
        protected DaggerfallStats stats = new DaggerfallStats();
        protected DaggerfallSkills skills = new DaggerfallSkills();
        protected ItemCollection items = new ItemCollection();
        protected ItemEquipTable equipTable = new ItemEquipTable();
        protected int maxHealth;
        protected int currentHealth;
        protected int currentFatigue;
        protected int currentMagicka;
        protected int currentBreath;
        protected WeaponMaterialTypes minMetalToHit;
        protected sbyte[] armorValues = new sbyte[NumberBodyParts];

        bool quiesce = false;

        #endregion

        #region Class Properties

        /// <summary>
        /// Set true to suppress events during state restore.
        /// </summary>
        public bool Quiesce
        {
            get { return quiesce; }
            set { quiesce = value; }
        }

        #endregion

        #region Entity Properties

        public Genders Gender { get { return gender; } set { gender = value; } }
        public DFCareer Career { get { return career; } set { career = value; } } 
        public string Name { get { return name; } set { name = value; } }
        public int Level { get { return level; } set { level = value; } }
        public DaggerfallStats Stats { get { return stats; } set { stats.Copy(value); } }
        public DaggerfallSkills Skills { get { return skills; } set { skills.Copy(value); } }
        public ItemCollection Items { get { return items; } set { items.ReplaceAll(value); } }
        public ItemEquipTable ItemEquipTable { get { return equipTable; } }
        public int MaxHealth { get { return maxHealth; } set { maxHealth = value; } }
        public int CurrentHealth { get { return currentHealth; } set { SetHealth(value); } }
        public int MaxFatigue { get { return (stats.LiveStrength + stats.LiveEndurance) * 64; } }
        public int CurrentFatigue { get { return currentFatigue; } set { SetFatigue(value); } }
        public int MaxMagicka { get { return FormulaHelper.SpellPoints(stats.LiveIntelligence, career.SpellPointMultiplierValue); } }
        public int CurrentMagicka { get { return currentMagicka; } set { SetMagicka(value); } }
        public int MaxBreath { get { return stats.LiveEndurance / 2; } }
        public int CurrentBreath { get { return currentBreath; } set { SetBreath(value); } }
        public WeaponMaterialTypes MinMetalToHit { get { return minMetalToHit; } set { minMetalToHit = value; } }
        public sbyte[] ArmorValues { get { return armorValues; } set { armorValues = value; } }
        public int DamageModifier { get { return FormulaHelper.DamageModifier(stats.LiveStrength); } }
        public int MaxEncumbrance { get { return FormulaHelper.MaxEncumbrance(stats.LiveStrength); } }
        public int MagicResist { get { return FormulaHelper.MagicResist(stats.LiveWillpower); } }
        public int ToHitModifier { get { return FormulaHelper.ToHitModifier(stats.LiveAgility); } }
        public int HitPointsModifier { get { return FormulaHelper.HitPointsModifier(stats.LiveEndurance); } }
        public int HealingRateModifier { get { return FormulaHelper.HealingRateModifier(stats.LiveEndurance); } }

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

        public virtual int SetHealth(int amount)
        {
            currentHealth = Mathf.Clamp(amount, 0, MaxHealth);
            if (currentHealth <= 0)
                RaiseOnDeathEvent();

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

        public virtual int SetFatigue(int amount)
        {
            currentFatigue = Mathf.Clamp(amount, 0, MaxFatigue);
            if (currentFatigue <= 0 && currentHealth > 0)
                RaiseOnExhaustedEvent();

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

        public virtual int SetMagicka(int amount)
        {
            currentMagicka = Mathf.Clamp(amount, 0, MaxMagicka);
            if (currentMagicka <= 0)
                RaiseOnMagickaDepletedEvent();

            return currentMagicka;
        }

        public virtual int SetBreath(int amount)
        {
            currentBreath = Mathf.Clamp(amount, 0, MaxBreath);

            return currentBreath;
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

        /// <summary>
        /// Tally skill usage.
        /// </summary>
        public virtual void TallySkill(DFCareer.Skills skill, short amount)
        {
        }

        /// <summary>
        /// Update armor values after equipping or unequipping a piece of armor.
        /// </summary>
        public void UpdateEquippedArmorValues(DaggerfallUnityItem armor, bool equipping)
        {
            if (armor.ItemGroup == ItemGroups.Armor ||
                (armor.ItemGroup == ItemGroups.MensClothing && armor.GroupIndex >= 6 && armor.GroupIndex <= 8) ||
                (armor.ItemGroup == ItemGroups.WomensClothing && armor.GroupIndex >= 4 && armor.GroupIndex <= 6)
               )
            {
                if (!armor.IsShield)
                {
                    // Get slot used by this armor
                    EquipSlots slot = ItemEquipTable.GetEquipSlot(armor);

                    int index = (int)DaggerfallUnityItem.GetBodyPartForEquipSlot(slot);

                    if (equipping)
                    {
                        armorValues[index] -= (sbyte)(armor.GetMaterialArmorValue() * 5);
                    }
                    else
                    {
                        armorValues[index] += (sbyte)(armor.GetMaterialArmorValue() * 5);
                    }
                }
                else
                {
                    // Shield armor values in classic are unaffected by their material type.
                    int[] values = { 0, 0, 0, 0, 0, 0, 0 }; // shield's effect on the 7 armor values
                    int armorBonus = armor.GetShieldArmorValue();
                    BodyParts[] protectedBodyParts = armor.GetShieldProtectedBodyParts();

                    foreach (var BodyParts in protectedBodyParts)
                    {
                        values[(int)BodyParts] = armorBonus;
                    }

                    for (int i = 0; i < armorValues.Length; i++)
                    {
                        if (equipping)
                        {
                            armorValues[i] -= (sbyte)(values[i] * 5);
                        }
                        else
                        {
                            armorValues[i] += (sbyte)(values[i] * 5);
                        }
                    }
                }
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Called by DaggerfallEntityBehaviour each frame.
        /// </summary>
        /// <param name="sender">DaggerfallEntityBehaviour making call.</param>
        public virtual void Update(DaggerfallEntityBehaviour sender)
        {
        }

        #endregion

        #region Temporary Events

        // These tie in with temporary effects and will be moved later

        public delegate void OnDeathHandler(DaggerfallEntity entity);
        public event OnDeathHandler OnDeath;
        protected void RaiseOnDeathEvent()
        {
            if (OnDeath != null && !quiesce)
                OnDeath(this);
        }

        public delegate void OnExhaustedHandler(DaggerfallEntity entity);
        public event OnExhaustedHandler OnExhausted;
        void RaiseOnExhaustedEvent()
        {
            if (OnExhausted != null && !quiesce)
                OnExhausted(this);
        }

        public delegate void OnMagickaDepletedHandler(DaggerfallEntity entity);
        public event OnMagickaDepletedHandler OnMagickaDepleted;
        void RaiseOnMagickaDepletedEvent()
        {
            if (OnMagickaDepleted != null && !quiesce)
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