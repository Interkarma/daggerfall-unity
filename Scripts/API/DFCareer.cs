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

#region Using Statements
using System;
using System.Text;
#endregion

namespace DaggerfallConnect
{
    /// <summary>
    /// Common career template shared by player, monsters, and enemy classes.
    /// </summary>
    [Serializable]
    public class DFCareer
    {
        #region Fields

        /// <summary>
        /// Raw data as read from CFG file.
        /// </summary>
        public CFGData RawData;

        // Career name
        public string Name;

        // Advancement multiplier
        public float AdvancementMultiplier;

        // Hit points per level or monster level
        public int HitPointsPerLevelOrMonsterLevel;

        // Attributes
        public int Strength;
        public int Intelligence;
        public int Willpower;
        public int Agility;
        public int Endurance;
        public int Personality;
        public int Speed;
        public int Luck;

        // Primary skills
        public Skills PrimarySkill1;
        public Skills PrimarySkill2;
        public Skills PrimarySkill3;

        // Major skills
        public Skills MajorSkill1;
        public Skills MajorSkill2;
        public Skills MajorSkill3;

        // Minor skills
        public Skills MinorSkill1;
        public Skills MinorSkill2;
        public Skills MinorSkill3;
        public Skills MinorSkill4;
        public Skills MinorSkill5;
        public Skills MinorSkill6;

        // Tolerances
        public Tolerance Paralysis;
        public Tolerance Magic;
        public Tolerance Poison;
        public Tolerance Fire;
        public Tolerance Frost;
        public Tolerance Shock;
        public Tolerance Disease;

        // Skill levels with weapon groups
        public Proficiency ShortBlades;
        public Proficiency LongBlades;
        public Proficiency HandToHand;
        public Proficiency Axes;
        public Proficiency BluntWeapons;
        public Proficiency MissileWeapons;

        // Attack modifier for enemy groups
        public AttackModifier UndeadAttackModifier;
        public AttackModifier DaedraAttackModifier;
        public AttackModifier HumanoidAttackModifier;
        public AttackModifier AnimalsAttackModifier;

        // Dark and light powered magery
        public DarknessMageryFlags DarknessPoweredMagery;
        public LightMageryFlags LightPoweredMagery;

        // Forbidden materials
        public MaterialFlags ForbiddenMaterials;

        // Forbidden shields
        public ShieldFlags ForbiddenShields;

        // Forbidden armor
        public ArmorFlags ForbiddenArmors;

        // Forbidden proficiencies
        public ProficiencyFlags ForbiddenProficiencies;

        // Expert proficiencies
        public ProficiencyFlags ExpertProficiencies;

        // Spell point multiplier
        public SpellPointMultipliers SpellPointMultiplier;
        public float SpellPointMultiplierValue;

        // Spell absorption
        public SpellAbsorptionFlags SpellAbsorption;

        // Spell point regeneration
        public bool NoRegenSpellPoints;

        // Talents
        public bool AcuteHearing;
        public bool Athleticism;
        public bool AdrenalineRush;

        // Regeneration and rapid healing
        public RegenerationFlags Regeneration;
        public RapidHealingFlags RapidHealing;

        // Damage
        public bool DamageFromSunlight;
        public bool DamageFromHolyPlaces;

        #endregion

        #region Structures & Enumerations

        /// <summary>
        /// A 74-byte data structure found in CLASS*.CFG and ENEMY*.CFG.
        /// </summary>
        [Serializable]
        public struct CFGData
        {
            // bytes [0-3]
            // Flags controlling how class tolerates various magic effects.
            public Byte ResistanceFlags;
            public Byte ImmunityFlags;
            public Byte LowToleranceFlags;
            public Byte CriticalWeaknessFlags;

            // byte [4-5]
            // Bitfield controlling special ability flags and spellpoints
            public UInt16 AbilityFlagsAndSpellPointsBitfield;

            // bytes [6]
            // Rapid healing flags
            public Byte RapidHealing;

            // bytes [7]
            // Regeneration flags
            public Byte Regeneration;

            // bytes [8]
            // Unknown value
            // All classes are 0 except Ranger which has a value of 8
            // All monsters are 0
            public Byte Unknown1;

            // bytes [9]
            public Byte SpellAbsorptionFlags;

            // bytes [10]
            // Attack modifier flags
            public Byte AttackModifierFlags;

            // bytes [11-12]
            // Forbidden materials flags
            public UInt16 ForbiddenMaterialsFlags;

            // bytes [13-15]
            // Bitfield controlling weapon proficiencies, forbidden armor, forbidden shields
            // 24-bits of data read into a UInt32
            public UInt32 WeaponArmorShieldsBitfield;

            // bytes [16-18]
            // Primary skills
            public Byte PrimarySkill1;
            public Byte PrimarySkill2;
            public Byte PrimarySkill3;

            // bytes [19-21]
            // Major skills
            public Byte MajorSkill1;
            public Byte MajorSkill2;
            public Byte MajorSkill3;

            // bytes [22-27]
            // Minor skills
            public Byte MinorSkill1;
            public Byte MinorSkill2;
            public Byte MinorSkill3;
            public Byte MinorSkill4;
            public Byte MinorSkill5;
            public Byte MinorSkill6;

            // bytes [28-43]
            // Class name.
            public String Name;

            // bytes [44-51]
            // Unknown values
            // All classes and monsters are 0
            public Byte[] Unknown2;

            // bytes [52-53]
            // Hit points per level for classes, monster level for monsters
            public UInt16 HitPointsPerLevelOrMonsterLevel;

            // bytes [54-57]
            // Advancement multiplier fixed point format
            public UInt32 AdvancementMultiplier;

            // bytes [58-73]
            // Base attributes like STR, END, etc.
            public UInt16[] Attributes;
        }

        /// <summary>
        /// Tolerance to effects.
        /// </summary>
        public enum Tolerance
        {
            Normal,
            Immune,
            Resistant,
            LowTolerance,
            CriticalWeakness,
        }

        /// <summary>
        /// Proficiency levels for various weapon groups.
        /// </summary>
        public enum Proficiency
        {
            Normal,
            Forbidden,
            Expert,
        }

        public enum AttackModifier
        {
            Normal,
            Bonus,
            Phobia
        }

        /// <summary>
        /// Material flags for forbidden materials.
        /// </summary>
        [Flags]
        public enum MaterialFlags
        {
            Iron = 1,
            Steel = 2,
            Silver = 4,
            Elven = 8,
            Dwarven = 16,
            Mithril = 32,
            Adamantium = 64,
            Ebony = 128,
            Orcish = 256,
            Daedric = 512,
        }

        /// <summary>
        /// Shield flags for forbidden shields.
        /// </summary>
        [Flags]
        public enum ShieldFlags
        {
            Buckler = 1,
            RoundShield = 2,
            KiteShield = 4,
            TowerShield = 8,
        }

        /// <summary>
        /// Armor flags for forbidden armor.
        /// </summary>
        [Flags]
        public enum ArmorFlags
        {
            Leather = 1,
            Chain = 2,
            Plate = 4,
        }

        /// <summary>
        /// Proficiency flags for forbidden weapons and weapon expertise.
        /// </summary>
        [Flags]
        public enum ProficiencyFlags
        {
            ShortBlades = 1,
            LongBlades = 2,
            HandToHand = 4,
            Axes = 8,
            BluntWeapons = 16,
            MissileWeapons = 32,
        }

        /// <summary>
        /// Major enemy groups.
        /// </summary>
        public enum EnemyGroups
        {
            Undead,
            Daedra,
            Humanoid,
            Animals,
        }

        /// <summary>
        /// Flags for darkness-powered magery.
        /// </summary>
        [Flags]
        public enum DarknessMageryFlags
        {
            Normal = 0,
            UnableToCastInLight = 1,
            ReducedPowerInLight = 2,
        }

        /// <summary>
        /// Flags for light-powered magery.
        /// </summary>
        [Flags]
        public enum LightMageryFlags
        {
            Normal = 0,
            UnableToCastInDarkness = 1,
            ReducedPowerInDarkness = 2,
        }

        /// <summary>
        /// Flags for spell absorption.
        /// </summary>
        [Flags]
        public enum SpellAbsorptionFlags
        {
            None = 0,
            InLight = 1,
            InDarkness = 2,
            Always = 4,
        }

        /// <summary>
        /// Flags for regeneration.
        /// </summary>
        [Flags]
        public enum RegenerationFlags
        {
            None = 0,
            InLight = 1,
            InDarkness = 2,
            InWater = 4,
            Always = 8,
        }

        /// <summary>
        /// Flags for rapid healing.
        /// </summary>
        [Flags]
        public enum RapidHealingFlags
        {
            None = 0,
            InLight = 1,
            InDarkness = 2,
            Always = 4,
        }

        /// <summary>
        /// Flags for effect types.
        /// </summary>
        [Flags]
        public enum EffectFlags
        {
            None = 0,
            Paralysis = 1,
            Magic = 2,
            Poison = 4,
            Fire = 8,
            Frost = 16,
            Shock = 32,
            Disease = 64,
        }

        /// <summary>
        /// Flags for entity special abilities.
        /// These appear to be consistent in ENEMY*.CFG and CLASS*.CFG files.
        /// </summary>
        [Flags]
        public enum SpecialAbilityFlags
        {
            None = 0,
            AcuteHearing = 1,
            Athleticism = 2,
            AdrenalineRush = 4,
            NoRegenSpellPoints = 8,
            SunDamage = 16,
            HolyDamage = 32,
        }

        /// <summary>
        /// Primary stats.
        /// </summary>
        public enum Stats
        {
            Strength = 0,
            Intelligence = 1,
            Willpower = 2,
            Agility = 3,
            Endurance = 4,
            Personality = 5,
            Speed = 6,
            Luck = 7,
        }

        /// <summary>
        /// Skills.
        /// The indices below match those in BIOG*.TXT files and CLASS*.CFG files.
        /// Likely to be the same indices using internally by the game.
        /// TEXT.RSC description records start at 1360, but are in a different order to below.
        /// </summary>
        public enum Skills
        {
            Medical = 0,
            Etiquette = 1,
            Streetwise = 2,
            Jumping = 3,
            Orcish = 4,
            Harpy = 5,
            Giantish = 6,
            Dragonish = 7,
            Nymph = 8,
            Daedric = 9,
            Spriggan = 10,
            Centaurian = 11,
            Impish = 12,
            Lockpicking = 13,
            Mercantile = 14,
            Pickpocket = 15,
            Stealth = 16,
            Swimming = 17,
            Climbing = 18,
            Backstabbing = 19,
            Dodging = 20,
            Running = 21,
            Destruction = 22,
            Restoration = 23,
            Illusion = 24,
            Alteration = 25,
            Thaumaturgy = 26,
            Mysticism = 27,
            ShortBlade = 28,
            LongBlade = 29,
            HandToHand = 30,
            Axe = 31,
            BluntWeapon = 32,
            Archery = 33,
            CriticalStrike = 34,
        }

        /// <summary>
        /// Spell point multipliers
        /// </summary>
        public enum SpellPointMultipliers
        {
            Times_3_00 = 0,     // 3.00 * INT
            Times_2_00 = 4,     // 2.00 * INT
            Times_1_75 = 8,     // 1.75 * INT
            Times_1_50 = 12,    // 1.50 * INT
            Times_1_00 = 16,    // 1.00 * INT
            Times_0_50 = 20,    // 0.50 * INT
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DFCareer()
        {
        }

        /// <summary>
        /// CFGData constructor.
        /// </summary>
        /// <param name="cfg">CFGData to load into class.</param>
        public DFCareer(CFGData cfg)
        {
            this.RawData = cfg;
            StructureData();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reads RawData into structured data.
        /// </summary>
        public void StructureData()
        {
            this.Name = RawData.Name;

            this.Strength = RawData.Attributes[0];
            this.Intelligence = RawData.Attributes[1];
            this.Willpower = RawData.Attributes[2];
            this.Agility = RawData.Attributes[3];
            this.Endurance = RawData.Attributes[4];
            this.Personality = RawData.Attributes[5];
            this.Speed = RawData.Attributes[6];
            this.Luck = RawData.Attributes[7];

            this.PrimarySkill1 = (Skills)RawData.PrimarySkill1;
            this.PrimarySkill2 = (Skills)RawData.PrimarySkill2;
            this.PrimarySkill3 = (Skills)RawData.PrimarySkill3;
            this.MajorSkill1 = (Skills)RawData.MajorSkill1;
            this.MajorSkill2 = (Skills)RawData.MajorSkill2;
            this.MajorSkill3 = (Skills)RawData.MajorSkill3;
            this.MinorSkill1 = (Skills)RawData.MinorSkill1;
            this.MinorSkill2 = (Skills)RawData.MinorSkill2;
            this.MinorSkill3 = (Skills)RawData.MinorSkill3;
            this.MinorSkill4 = (Skills)RawData.MinorSkill4;
            this.MinorSkill5 = (Skills)RawData.MinorSkill5;
            this.MinorSkill6 = (Skills)RawData.MinorSkill6;

            this.HitPointsPerLevelOrMonsterLevel = RawData.HitPointsPerLevelOrMonsterLevel;

            float value = (RawData.AdvancementMultiplier >> 16) + ((RawData.AdvancementMultiplier & 0xffff)) / 65536f;
            this.AdvancementMultiplier = float.Parse(string.Format("{0:0.00}", value));

            this.Paralysis = GetTolerance(EffectFlags.Paralysis);
            this.Magic = GetTolerance(EffectFlags.Magic);
            this.Poison = GetTolerance(EffectFlags.Poison);
            this.Fire = GetTolerance(EffectFlags.Fire);
            this.Frost = GetTolerance(EffectFlags.Frost);
            this.Shock = GetTolerance(EffectFlags.Shock);
            this.Disease = GetTolerance(EffectFlags.Disease);

            this.ForbiddenMaterials = (MaterialFlags)RawData.ForbiddenMaterialsFlags;
            this.ForbiddenShields = (ShieldFlags)((RawData.WeaponArmorShieldsBitfield >> 9) & 0x0f);
            this.ForbiddenArmors = (ArmorFlags)((RawData.WeaponArmorShieldsBitfield >> 6) & 0x07);
            this.ForbiddenProficiencies = (ProficiencyFlags)(RawData.WeaponArmorShieldsBitfield & 0x3f);
            this.ExpertProficiencies = (ProficiencyFlags)((RawData.WeaponArmorShieldsBitfield >> 16) & 0x3f);

            this.ShortBlades = GetProficiency(ProficiencyFlags.ShortBlades);
            this.LongBlades = GetProficiency(ProficiencyFlags.LongBlades);
            this.HandToHand = GetProficiency(ProficiencyFlags.HandToHand);
            this.Axes = GetProficiency(ProficiencyFlags.Axes);
            this.BluntWeapons = GetProficiency(ProficiencyFlags.BluntWeapons);
            this.MissileWeapons = GetProficiency(ProficiencyFlags.MissileWeapons);

            this.SpellPointMultiplier = GetSpellPointMultiplier();
            this.SpellPointMultiplierValue = GetSpellPointMultiplierValue(this.SpellPointMultiplier);

            this.DarknessPoweredMagery = (DarknessMageryFlags)((RawData.AbilityFlagsAndSpellPointsBitfield & 0x300) >> 8);
            this.LightPoweredMagery = (LightMageryFlags)((RawData.AbilityFlagsAndSpellPointsBitfield & 0x00C0) >> 6);

            this.SpellAbsorption = (SpellAbsorptionFlags)RawData.SpellAbsorptionFlags;

            this.NoRegenSpellPoints = HasSpecialAbility(SpecialAbilityFlags.NoRegenSpellPoints);

            this.AcuteHearing = HasSpecialAbility(SpecialAbilityFlags.AcuteHearing);
            this.Athleticism = HasSpecialAbility(SpecialAbilityFlags.Athleticism);
            this.AdrenalineRush = HasSpecialAbility(SpecialAbilityFlags.AdrenalineRush);

            this.Regeneration = (RegenerationFlags)RawData.Regeneration;
            this.RapidHealing = (RapidHealingFlags)RawData.RapidHealing;

            this.DamageFromSunlight = HasSpecialAbility(SpecialAbilityFlags.SunDamage);
            this.DamageFromHolyPlaces = HasSpecialAbility(SpecialAbilityFlags.HolyDamage);

            this.UndeadAttackModifier = GetAttackModifier(EnemyGroups.Undead);
            this.DaedraAttackModifier = GetAttackModifier(EnemyGroups.Daedra);
            this.HumanoidAttackModifier = GetAttackModifier(EnemyGroups.Humanoid);
            this.AnimalsAttackModifier = GetAttackModifier(EnemyGroups.Animals);
        }

        /// <summary>
        /// Determines if material type is forbidden.
        /// </summary>
        /// <param name="flags">MaterialFlags to test.</param>
        /// <returns>True if material forbidden.</returns>
        public bool IsMaterialForbidden(MaterialFlags flags)
        {
            return ((ForbiddenMaterials & flags) == flags) ? true : false;
        }

        /// <summary>
        /// Determines if a shield type is forbidden.
        /// </summary>
        /// <param name="flags">ShieldFlags to test.</param>
        /// <returns>True if shield is forbidden.</returns>
        public bool IsShieldForbidden(ShieldFlags flags)
        {
            return ((ForbiddenShields & flags) == flags) ? true : false;
        }

        /// <summary>
        /// Determines if an armor type is forbidden.
        /// </summary>
        /// <param name="flags">ArmorFlags to test.</param>
        /// <returns>True if armor is forbidden.</returns>
        public bool IsArmorForbidden(ArmorFlags flags)
        {
            return ((ForbiddenArmors & flags) == flags) ? true : false;
        }

        /// <summary>
        /// Determines if a proficiency type is forbidden.
        /// </summary>
        /// <param name="flags">ProficiencyFlags to test.</param>
        /// <returns>True if proficiency is forbidden.</returns>
        public bool IsProficiencyForbidden(ProficiencyFlags flags)
        {
            return ((ForbiddenProficiencies & flags) == flags) ? true : false;
        }

        /// <summary>
        /// Determines if a proficiency type is expert.
        /// </summary>
        /// <param name="flags">ProficiencyFlags to test.</param>
        /// <returns>True if proficiency is forbidden.</returns>
        public bool IsProficiencyExpert(ProficiencyFlags flags)
        {
            return ((ExpertProficiencies & flags) == flags) ? true : false;
        }

        /// <summary>
        /// Determines if class is resistant to effects.
        /// </summary>
        /// <param name="flags">EffectFlags to test.</param>
        /// <returns>True if resistant.</returns>
        bool HasResistance(EffectFlags flags)
        {
            return ((RawData.ResistanceFlags & (byte)flags) == (byte)flags) ? true : false;
        }

        /// <summary>
        /// Determines if class is immune to effects.
        /// </summary>
        /// <param name="flags">EffectFlags to test.</param>
        /// <returns>True if immune.</returns>
        bool HasImmunity(EffectFlags flags)
        {
            return ((RawData.ImmunityFlags & (byte)flags) == (byte)flags) ? true : false;
        }

        /// <summary>
        /// Determines if class has low tolerance to effects.
        /// </summary>
        /// <param name="flags">EffectFlags to test.</param>
        /// <returns>True if low tolerance.</returns>
        bool HasLowTolerance(EffectFlags flags)
        {
            return ((RawData.LowToleranceFlags & (byte)flags) == (byte)flags) ? true : false;
        }

        /// <summary>
        /// Determines if class has critical weakness to effects.
        /// </summary>
        /// <param name="flags">EffectFlags to test.</param>
        /// <returns>True if critical weakness.</returns>
        bool HasCriticalWeakness(EffectFlags flags)
        {
            return ((RawData.CriticalWeaknessFlags & (byte)flags) == (byte)flags) ? true : false;
        }

        /// <summary>
        /// Determines if class has special ability.
        /// </summary>
        /// <param name="flags">SpecialAbilityFlags to test.</param>
        /// <returns>True if has special ability.</returns>
        bool HasSpecialAbility(SpecialAbilityFlags flags)
        {
            return ((RawData.AbilityFlagsAndSpellPointsBitfield & (byte)flags) == (byte)flags) ? true : false;
        }

        #endregion

        #region Private Methods

        Tolerance GetTolerance(EffectFlags flags)
        {
            Tolerance result;

            if (HasResistance(flags))
                result = Tolerance.Resistant;
            else if (HasImmunity(flags))
                result = Tolerance.Immune;
            else if (HasLowTolerance(flags))
                result = Tolerance.LowTolerance;
            else if (HasCriticalWeakness(flags))
                result = Tolerance.CriticalWeakness;
            else
                result = Tolerance.Normal;

            return result;
        }

        Proficiency GetProficiency(ProficiencyFlags flags)
        {
            Proficiency result;

            if (IsProficiencyForbidden(flags))
                result = Proficiency.Forbidden;
            else if (IsProficiencyExpert(flags))
                result = Proficiency.Expert;
            else
                result = Proficiency.Normal;

            return result;
        }

        SpellPointMultipliers GetSpellPointMultiplier()
        {
            SpellPointMultipliers result;

            result = (SpellPointMultipliers)((RawData.AbilityFlagsAndSpellPointsBitfield & 0x1C00) >> 8);

            return result;
        }

        float GetSpellPointMultiplierValue(SpellPointMultipliers multiplier)
        {
            float result;

            switch (multiplier)
            {
                case SpellPointMultipliers.Times_0_50:
                    result = 0.5f;
                    break;
                case SpellPointMultipliers.Times_1_00:
                    result = 1.0f;
                    break;
                case SpellPointMultipliers.Times_1_50:
                    result = 1.5f;
                    break;
                case SpellPointMultipliers.Times_1_75:
                    result = 1.75f;
                    break;
                case SpellPointMultipliers.Times_2_00:
                    result = 2.0f;
                    break;
                default:
                case SpellPointMultipliers.Times_3_00:
                    result = 3.0f;
                    break;
            }

            return result;
        }

        AttackModifier GetAttackModifier(EnemyGroups group)
        {
            AttackModifier result = AttackModifier.Normal;

            switch (group)
            {
                case EnemyGroups.Undead:
                    if (HasFlags(RawData.AttackModifierFlags, 0x01))
                        result = AttackModifier.Bonus;
                    else if (HasFlags(RawData.AttackModifierFlags, 0x10))
                        result = AttackModifier.Phobia;
                    break;
                case EnemyGroups.Daedra:
                    if (HasFlags(RawData.AttackModifierFlags, 0x02))
                        result = AttackModifier.Bonus;
                    else if (HasFlags(RawData.AttackModifierFlags, 0x20))
                        result = AttackModifier.Phobia;
                    break;
                case EnemyGroups.Humanoid:
                    if (HasFlags(RawData.AttackModifierFlags, 0x04))
                        result = AttackModifier.Bonus;
                    else if (HasFlags(RawData.AttackModifierFlags, 0x40))
                        result = AttackModifier.Phobia;
                    break;
                case EnemyGroups.Animals:
                    if (HasFlags(RawData.AttackModifierFlags, 0x08))
                        result = AttackModifier.Bonus;
                    else if (HasFlags(RawData.AttackModifierFlags, 0x80))
                        result = AttackModifier.Phobia;
                    break;
            }

            return result;
        }

        bool HasFlags(int value, int flags)
        {
            return ((value & flags) == flags) ? true : false;
        }

        #endregion
    }
}