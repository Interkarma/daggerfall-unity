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
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.MagicAndEffects;

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
        public const int FatigueMultiplier = 64;

        protected DaggerfallEntityBehaviour entityBehaviour;

        protected Genders gender;
        protected DFCareer career = new DFCareer();
        protected string name;
        protected int level;
        protected DaggerfallStats stats = new DaggerfallStats();
        protected DaggerfallSkills skills = new DaggerfallSkills();
        protected DaggerfallResistances resistances = new DaggerfallResistances();
        protected ItemCollection items = new ItemCollection();
        protected ItemEquipTable equipTable;
        protected WorldContext worldContext = WorldContext.Nothing;
        protected int maxHealth;
        protected int currentHealth;
        protected int currentFatigue;
        protected int currentMagicka;
        protected int maxMagicka;
        protected int currentBreath;
        protected WeaponMaterialTypes minMetalToHit;
        protected sbyte[] armorValues = new sbyte[NumberBodyParts];

        bool quiesce = false;
        bool isParalyzed = false;

        bool[] resistanceFlags = new bool[5];     // Indices map to DFCareer.Elements 0-4
        int[] resistanceChances = new int[5];

        // Entity spellbook
        protected List<EffectBundleSettings> spellbook = new List<EffectBundleSettings>();

        #endregion

        #region Properties

        // Entity magic effect flags
        // Note: These properties are intentionally not serialized. They should only be set by live effects.
        public bool IsImmuneToParalysis { get; set; }
        public bool IsImmuneToDisease { get; set; }
        public bool IsSilenced { get; set; }
        public bool IsWaterWalking { get; set; }
        public bool IsWaterBreathing { get; set; }
        public MagicalConcealmentFlags MagicalConcealmentFlags { get; set; }
        public bool IsEnhancedClimbing { get; set; }
        public bool IsEnhancedJumping { get; set; }
        public bool IsSlowFalling { get; set; }

        /// <summary>
        /// Gets the DaggerfallEntityBehaviour related to this DaggerfallEntity.
        /// </summary>
        public DaggerfallEntityBehaviour EntityBehaviour
        {
            get { return entityBehaviour; }
        }

        /// <summary>
        /// Set true to suppress events during state restore.
        /// </summary>
        public bool Quiesce
        {
            get { return quiesce; }
            set { quiesce = value; }
        }

        /// <summary>
        /// Gets or sets paralyzation flag.
        /// Always returns false when isImmuneToParalysis is true.
        /// Each entity type will need to act on paralyzation in their own unique way.
        /// Note: This value is intentionally not serialized. It should only be set by live effects.
        /// </summary>
        public bool IsParalyzed
        {
            get { return (!IsImmuneToParalysis && isParalyzed); }
            set { isParalyzed = value; }
        }

        /// <summary>
        /// Gets or sets resisting fire flag.
        /// Note: This value is intentionally not serialized. It should only be set by live effects.
        /// </summary>
        public bool IsResistingFire
        {
            get { return resistanceFlags[(int)DFCareer.Elements.Fire]; }
            set { resistanceFlags[(int)DFCareer.Elements.Fire] = value; }
        }

        /// <summary>
        /// Gets or sets resisting frost flag.
        /// Note: This value is intentionally not serialized. It should only be set by live effects.
        /// </summary>
        public bool IsResistingFrost
        {
            get { return resistanceFlags[(int)DFCareer.Elements.Frost]; }
            set { resistanceFlags[(int)DFCareer.Elements.Frost] = value; }
        }

        /// <summary>
        /// Gets or sets resisting disease or poison flag.
        /// Note: This value is intentionally not serialized. It should only be set by live effects.
        /// </summary>
        public bool IsResistingDiseaseOrPoison
        {
            get { return resistanceFlags[(int)DFCareer.Elements.DiseaseOrPoison]; }
            set { resistanceFlags[(int)DFCareer.Elements.DiseaseOrPoison] = value; }
        }

        /// <summary>
        /// Gets or sets resisting shock flag.
        /// Note: This value is intentionally not serialized. It should only be set by live effects.
        /// </summary>
        public bool IsResistingShock
        {
            get { return resistanceFlags[(int)DFCareer.Elements.Shock]; }
            set { resistanceFlags[(int)DFCareer.Elements.Shock] = value; }
        }

        /// <summary>
        /// Gets or sets resisting magic flag.
        /// Note: This value is intentionally not serialized. It should only be set by live effects.
        /// </summary>
        public bool IsResistingMagic
        {
            get { return resistanceFlags[(int)DFCareer.Elements.Magic]; }
            set { resistanceFlags[(int)DFCareer.Elements.Magic] = value; }
        }

        /// <summary>
        /// True if entity is blending (normal or true).
        /// </summary>
        public bool IsBlending
        {
            get { return (HasConcealment(MagicalConcealmentFlags.BlendingNormal) || HasConcealment(MagicalConcealmentFlags.BlendingTrue)); }
        }

        /// <summary>
        /// True if entity is a shadow (normal or true).
        /// </summary>
        public bool IsAShade
        {
            get { return (HasConcealment(MagicalConcealmentFlags.ShadeNormal) || HasConcealment(MagicalConcealmentFlags.ShadeTrue)); }
        }

        /// <summary>
        /// True if entity is invisible (normal or true).
        /// </summary>
        public bool IsInvisible
        {
            get { return (HasConcealment(MagicalConcealmentFlags.InvisibleNormal) || HasConcealment(MagicalConcealmentFlags.InvisibleTrue)); }
        }

        /// <summary>
        /// True if entity is magically concealed by invisibility/chameleon/shadow (normal or true).
        /// </summary>
        public bool IsMagicallyConcealed
        {
            get { return MagicalConcealmentFlags != MagicalConcealmentFlags.None; }
        }

        /// <summary>
        /// True if entity is magically concealed by invisibility/chameleon/shadow (normal only).
        /// </summary>
        public bool IsMagicallyConcealedNormalPower
        {
            get
            {
                return (HasConcealment(MagicalConcealmentFlags.InvisibleNormal) ||
                        HasConcealment(MagicalConcealmentFlags.BlendingNormal) ||
                        HasConcealment(MagicalConcealmentFlags.ShadeNormal));
            }
        }

        /// <summary>
        /// True if entity is magically concealed by invisibility/chameleon/shadow (true only).
        /// </summary>
        public bool IsMagicallyConcealedTruePower
        {
            get
            {
                return (HasConcealment(MagicalConcealmentFlags.InvisibleTrue) ||
                        HasConcealment(MagicalConcealmentFlags.BlendingTrue) ||
                        HasConcealment(MagicalConcealmentFlags.ShadeTrue));
            }
        }

        /// <summary>
        /// Gets or sets world context of this entity for floating origin support.
        /// Not required by all systems but this is a nice central place for mobiles.
        /// </summary>
        public WorldContext WorldContext
        {
            get { return worldContext; }
            set { worldContext = value; }
        }

        #endregion

        #region Entity Properties

        public Genders Gender { get { return gender; } set { gender = value; } }
        public DFCareer Career { get { return career; } set { career = value; } }
        public string Name { get { return name; } set { name = value; } }
        public int Level { get { return level; } set { level = value; } }
        public DaggerfallStats Stats { get { return stats; } set { stats.Copy(value); } }
        public DaggerfallSkills Skills { get { return skills; } set { skills.Copy(value); } }
        public DaggerfallResistances Resistances { get { return resistances; } set { resistances.Copy(value); } }
        public ItemCollection Items { get { return items; } set { items.ReplaceAll(value); } }
        public ItemEquipTable ItemEquipTable { get { return equipTable; } }
        public int MaxHealth { get { return maxHealth; } set { maxHealth = value; } }
        public int CurrentHealth { get { return GetCurrentHealth(); } set { SetHealth(value); } }
        public float CurrentHealthPercent { get { return GetCurrentHealth() / (float)maxHealth; } }
        public int MaxFatigue { get { return (stats.LiveStrength + stats.LiveEndurance) * 64; } }
        public int CurrentFatigue { get { return GetCurrentFatigue(); } set { SetFatigue(value); } }
        public int MaxMagicka { get { return (this == GameManager.Instance.PlayerEntity ? FormulaHelper.SpellPoints(stats.LiveIntelligence, career.SpellPointMultiplierValue) : maxMagicka); } set { maxMagicka = value; } }
        public int CurrentMagicka { get { return GetCurrentMagicka(); } set { SetMagicka(value); } }
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

        #region Constructors

        public DaggerfallEntity(DaggerfallEntityBehaviour entityBehaviour)
        {
            this.entityBehaviour = entityBehaviour;
            equipTable = new ItemEquipTable(this);

            // Allow for resetting specific player state on new game or when game starts loading
            SaveLoadManager.OnStartLoad += SaveLoadManager_OnStartLoad;
            StartGameBehaviour.OnNewGame += StartGameBehaviour_OnNewGame;
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Sets entity defaults during scene startup.
        /// Defaults should be overwritten by normal processes such as
        /// character creation, monster instantiation, and save game deserialization.
        /// </summary>
        public abstract void SetEntityDefaults();

        #endregion

        #region Status Changes

        public int IncreaseHealth(int amount)
        {
            return SetHealth(currentHealth + amount);
        }

        public int DecreaseHealth(int amount)
        {
            return SetHealth(currentHealth - amount);
        }

        public virtual int SetHealth(int amount, bool restoreMode = false)
        {
            currentHealth = (restoreMode) ? amount : Mathf.Clamp(amount, 0, MaxHealth);
            if (currentHealth <= 0)
                RaiseOnDeathEvent();

            return currentHealth;
        }

        public int IncreaseFatigue(int amount, bool assignMultiplier = false)
        {
            // Optionally assign fatigue multiplier
            // This seems to be case for spell effects that heal fatigue
            if (assignMultiplier)
                amount *= FatigueMultiplier;

            return SetFatigue(currentFatigue + amount);
        }

        public int DecreaseFatigue(int amount, bool assignMultiplier = false)
        {
            // Optionally assign fatigue multiplier
            // This seems to be case for spell effects that damage fatigue
            if (assignMultiplier)
                amount *= FatigueMultiplier;

            return SetFatigue(currentFatigue - amount);
        }

        public virtual int SetFatigue(int amount, bool restoreMode = false)
        {
            currentFatigue = (restoreMode) ? amount : Mathf.Clamp(amount, 0, MaxFatigue);
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

        public virtual int SetMagicka(int amount, bool restoreMode = false)
        {
            currentMagicka = (restoreMode) ? amount : Mathf.Clamp(amount, 0, MaxMagicka);
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

        int GetCurrentHealth()
        {
            return currentHealth;
        }

        int GetCurrentFatigue()
        {
            return currentFatigue;
        }

        int GetCurrentMagicka()
        {
            return currentMagicka;
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

                    foreach (var BodyPart in protectedBodyParts)
                    {
                        values[(int)BodyPart] = armorBonus;
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

        /// <summary>
        /// Helper to check if specific magical concealment flags are active on player.
        /// </summary>
        /// <param name="flags">Comparison flags.</param>
        /// <returns>True if matching.</returns>
        public bool HasConcealment(MagicalConcealmentFlags flags)
        {
            return ((MagicalConcealmentFlags & flags) == flags);
        }

        /// <summary>
        /// Raise or lower a specific resistance flag.
        /// </summary>
        /// <param name="elementType">Element type.</param>
        /// <param name="value">Desired resistance flag.</param>
        public void SetResistanceFlag(DFCareer.Elements elementType, bool value)
        {
            resistanceFlags[(int)elementType] = value;
        }

        /// <summary>
        /// Check if entity has a specific resistance flag raised.
        /// </summary>
        /// <param name="elementType">Element type.</param>
        public bool HasResistanceFlag(DFCareer.Elements elementType)
        {
            return resistanceFlags[(int)elementType];
        }

        /// <summary>
        /// Gets current total resistance chance for an element.
        /// This is only used when corresponding elemental resistance flag is raised by effect.
        /// </summary>
        /// <param name="elementType">Element type to check total resistance value of.</param>
        /// <returns>Resistance chance for that element.</returns>
        public int GetResistanceChance(DFCareer.Elements elementType)
        {
            return resistanceChances[(int)elementType];
        }

        /// <summary>
        /// Raise resistance chance total for an element.
        /// This is only used when corresponding element resistance flag is raised by effect.
        /// Resistance chance is reset each frame so multiple effects can contribute to total resistance chance.
        /// </summary>
        /// <param name="elementType">Element type to raise resistance of.</param>
        /// <param name="value">Amount to raise resist chance for element.</param>
        public void RaiseResistanceChance(DFCareer.Elements elementType, int value)
        {
            resistanceChances[(int)elementType] += value;
        }

        #endregion

        #region Temp Spellbook Helpers

        // NOTES:
        //  Likely to add a custom spell collection class later for spellbook

        public int SpellbookCount()
        {
            return spellbook.Count;
        }

        public bool GetSpell(int index, out EffectBundleSettings spell)
        {
            if (index < 0 || index > spellbook.Count - 1)
            {
                spell = new EffectBundleSettings();
                return false;
            }
            else
            {
                spell = spellbook[index];
                return true;
            }
        }

        public EffectBundleSettings[] GetSpells()
        {
            return spellbook.ToArray();
        }

        public void SortSpellsAlpha()
        {
            List<EffectBundleSettings> sortedSpellbook = spellbook.OrderBy(x => x.Name).ToList();
            if (sortedSpellbook.Count == spellbook.Count)
                spellbook = sortedSpellbook;
        }

        public void AddSpell(EffectBundleSettings spell)
        {
            // Just add spell to end of list for now
            // When implemented, the real collection class will allow for custom sorting
            spellbook.Add(spell);
        }

        public void DeleteSpell(int index)
        {
            if (index < 0 || index > spellbook.Count - 1)
                return;

            spellbook.RemoveAt(index);
        }

        public EffectBundleSettings[] SerializeSpellbook()
        {
            return spellbook.ToArray();
        }

        public void DeserializeSpellbook(EffectBundleSettings[] otherSpellbook)
        {
            spellbook = new List<EffectBundleSettings>();

            if (otherSpellbook == null || otherSpellbook.Length == 0)
                return;

            spellbook.AddRange(otherSpellbook);
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

        /// <summary>
        /// Constant effects are cleared each frame by peered entity effect manager and must be actively set by effects maintaining them.
        /// </summary>
        public virtual void ClearConstantEffects()
        {
            IsParalyzed = false;
            IsImmuneToParalysis = false;
            IsImmuneToDisease = false;
            IsSilenced = false;
            IsWaterWalking = false;
            IsWaterBreathing = false;
            MagicalConcealmentFlags = MagicalConcealmentFlags.None;
            IsEnhancedClimbing = false;
            IsEnhancedJumping = false;
            IsSlowFalling = false;
            IsResistingFire = false;
            IsResistingFrost = false;
            IsResistingDiseaseOrPoison = false;
            IsResistingShock = false;
            IsResistingMagic = false;
            resistanceChances[0] = 0;
            resistanceChances[1] = 0;
            resistanceChances[2] = 0;
            resistanceChances[3] = 0;
            resistanceChances[4] = 0;
        }

        /// <summary>
        /// Called when starting a new game or when a game starts to load.
        /// Used to clear out any state that should not persist to a new game session.
        /// </summary>
        protected virtual void ResetEntityState()
        {
            ClearConstantEffects();
            SetEntityDefaults();
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

        public static SoundClips GetRaceGenderAttackSound(Races race, Genders gender)
        {
            if (gender == Genders.Male)
            {
                switch (race)
                {
                    case Races.Breton:
                        return SoundClips.BretonMalePain1;
                    case Races.Redguard:
                        return SoundClips.RedguardMalePain1;
                    case Races.Nord:
                        return SoundClips.NordMalePain1;
                    case Races.DarkElf:
                        return SoundClips.DarkElfMalePain1;
                    case Races.HighElf:
                        return SoundClips.HighElfMalePain1;
                    case Races.WoodElf:
                        return SoundClips.WoodElfMalePain1;
                    case Races.Khajiit:
                        return SoundClips.KhajiitMalePain1;
                    case Races.Argonian:
                        return SoundClips.ArgonianMalePain1;
                    default:
                        return SoundClips.None;
                }
            }
            else
            {
                switch (race)
                {
                    case Races.Breton:
                    case Races.Redguard:
                    case Races.Nord:
                        int random = UnityEngine.Random.Range(0, 3);
                        if (random == 0)
                            return SoundClips.BretonFemalePain1;
                        else if (random == 1)
                            return SoundClips.BretonFemalePain2;
                        else
                            return SoundClips.DarkElfFemalePain2;
                    case Races.DarkElf:
                    case Races.HighElf:
                    case Races.WoodElf:
                        random = UnityEngine.Random.Range(0, 2);
                        if (random == 0)
                            return SoundClips.HighElfFemalePain1;
                        else
                            return SoundClips.HighElfFemalePain2;
                    case Races.Khajiit:
                        random = UnityEngine.Random.Range(0, 2);
                        if (random == 0)
                            return SoundClips.KhajiitFemalePain1;
                        else
                            return SoundClips.KhajiitFemalePain2;
                    case Races.Argonian:
                        random = UnityEngine.Random.Range(0, 2);
                        if (random == 0)
                            return SoundClips.ArgonianFemalePain1;
                        else
                            return SoundClips.ArgonianFemalePain2;
                    default:
                        return SoundClips.None;
                }
            }
        }

        public static SoundClips GetRaceGenderPainSound(Races race, Genders gender, bool heavyDamage)
        {
            if (gender == Genders.Male)
            {
                switch (race)
                {
                    case Races.Breton:
                        return SoundClips.BretonMalePain2;
                    case Races.Redguard:
                        return SoundClips.RedguardMalePain2;
                    case Races.Nord:
                        return SoundClips.NordMalePain2;
                    case Races.DarkElf:
                        return SoundClips.DarkElfMalePain2;
                    case Races.HighElf:
                        return SoundClips.HighElfMalePain2;
                    case Races.WoodElf:
                        return SoundClips.WoodElfMalePain2;
                    case Races.Khajiit:
                        return SoundClips.KhajiitMalePain2;
                    case Races.Argonian:
                        return SoundClips.ArgonianMalePain2;
                    default:
                        return SoundClips.None;
                }
            }
            else
            {
                switch (race)
                {
                    case Races.Breton:
                    case Races.Redguard:
                    case Races.Nord:
                    case Races.DarkElf:
                    case Races.Argonian:
                    case Races.HighElf:
                        if (heavyDamage)
                            return SoundClips.NordFemalePain2;
                        else
                        {
                            int random = UnityEngine.Random.Range(0, 2);
                            if (random == 0)
                                return SoundClips.RedguardFemalePain1;
                            else
                                return SoundClips.DarkElfFemalePain1;
                        }
                    case Races.WoodElf:
                    case Races.Khajiit:
                        if (heavyDamage)
                            return SoundClips.WoodElfFemalePain2;
                        else
                        {
                            return SoundClips.WoodElfFemalePain1;
                        }
                    default:
                        return SoundClips.None;
                }
            }
        }

        #endregion

        #region Event Handlers

        private void StartGameBehaviour_OnNewGame()
        {
            ResetEntityState();
        }

        private void SaveLoadManager_OnStartLoad(SaveData_v1 saveData)
        {
            ResetEntityState();
        }

        #endregion
    }
}
