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

using System;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Game.MagicAndEffects
{
    /// <summary>
    /// Defines properties intrinsic to an effect.
    /// </summary>
    [Serializable]
    public struct EffectProperties
    {
        public string Key;                                          // Unique key to identify effect
        public int ClassicKey;                                      // Unique key only for matching classic effect group/subgroup
        public string GroupName;                                    // Group display name (used by crafting stations)
        public string SubGroupName;                                 // SubGroup display name (used by crafting stations)
        public string DisplayName;                                  // Display name (used by crafting stations)
        public TextFile.Token[] SpellMakerDescription;              // Description for spellmaker
        public TextFile.Token[] SpellBookDescription;               // Description for spellbook
        public bool ShowSpellIcon;                                  // True to make spell show icon on player HUD
        public bool SupportDuration;                                // Uses duration
        public bool SupportChance;                                  // Uses chance
        public bool SupportMagnitude;                               // Uses magnitude
        public ChanceFunction ChanceFunction;                       // Determines if chance check is done OnCast (by manager) or Custom (elsewhere)
        public TargetTypes AllowedTargets;                          // Targets allowed by this effect
        public ElementTypes AllowedElements;                        // Elements allowed by this effect
        public MagicCraftingStations AllowedCraftingStations;       // Magic stations that can consume this effect (e.g. spellmaker, itemmaker)
        public DFCareer.MagicSkills MagicSkill;                     // Related magic skill for this effect
        public EffectCosts DurationCosts;                           // Duration cost values
        public EffectCosts ChanceCosts;                             // Chance cost values
        public EffectCosts MagnitudeCosts;                          // Magnitude cost values
    }

    /// <summary>
    /// Defines properties intrinsic to a potion.
    /// Note: Under early development. Subject to change.
    /// </summary>
    [Serializable]
    public struct PotionProperties
    {
        public PotionRecipe[] Recipes;                              // Potion recipe for effect
    }

    /// <summary>
    /// Allows tuning of cost per setting.
    /// </summary>
    [Serializable]
    public struct EffectCosts
    {
        public float OffsetGold;                                    // Increase base gold cost
        public float Factor;                                        // Scaling factor applied to spellpoint cost
        public float CostA;                                         // First magic number related to costs
        public float CostB;                                         // Second magic number related to costs
    }

    /// <summary>
    /// Duration, Chance, Magnitude settings for an effect.
    /// </summary>
    [Serializable]
    public struct EffectSettings
    {
        public int DurationBase;
        public int DurationPlus;
        public int DurationPerLevel;

        public int ChanceBase;
        public int ChancePlus;
        public int ChancePerLevel;

        public int MagnitudeBaseMin;
        public int MagnitudeBaseMax;
        public int MagnitudePlusMin;
        public int MagnitudePlusMax;
        public int MagnitudePerLevel;
    }

    /// <summary>
    /// For storing effect in bundle settings.
    /// </summary>
    [Serializable]
    public struct EffectEntry
    {
        public string Key;
        public EffectSettings Settings;

        public EffectEntry(string key)
        {
            Key = key;
            Settings = new EffectSettings();
        }

        public EffectEntry(string key, EffectSettings settings)
        {
            Key = key;
            Settings = settings;
        }
    }

    /// <summary>
    /// Stores an effect group / subgroup pair as read from classic save.
    /// This is only used when importing character spellbook from classic.
    /// During startup any legacy spells will be migrated to Daggerfall Unity spells
    /// provided all classic group / subgroup pairs can be resolved to a known effect key.
    /// </summary>
    [Serializable]
    public struct LegacyEffectEntry
    {
        public int Group;
        public int SubGroup;
        public EffectSettings Settings;
    }

    /// <summary>
    /// Settings for an entity effect bundle.
    /// </summary>
    [Serializable]
    public struct EffectBundleSettings
    {
        public int Version;
        public BundleTypes BundleType;
        public TargetTypes TargetType;
        public ElementTypes ElementType;
        public string Name;
        public int IconIndex;
        public bool MinimumCastingCost;
        public string Tag;
        public EffectEntry[] Effects;
        public LegacyEffectEntry[] LegacyEffects;
    }

    /// <summary>
    /// Settings for a basic disease effect.
    /// </summary>
    [Serializable]
    public struct DiseaseData
    {
        // Affected stats
        public byte STR;
        public byte INT;
        public byte WIL;
        public byte AGI;
        public byte END;
        public byte PER;
        public byte SPD;
        public byte LUC;
        public byte HEA;
        public byte FAT;
        public byte SPL;
        public byte minDamage;
        public byte maxDamage;
        public byte daysOfSymptomsMin; // 0xFF means never-ending
        public byte daysOfSymptomsMax;

        // Constructor
        public DiseaseData(byte STRp, byte INTp,
            byte WILp, byte AGIp, byte ENDp, byte PERp,
            byte SPDp, byte LUCp, byte HEAp, byte FATp,
            byte SPLp, byte minDamagep, byte maxDamagep,
            byte daysOfSymptomsMinp, byte daysOfSymptomsMaxp)
        {
            STR = STRp;
            INT = INTp;
            WIL = WILp;
            AGI = AGIp;
            END = ENDp;
            PER = PERp;
            SPD = SPDp;
            LUC = LUCp;
            HEA = HEAp;
            FAT = FATp;
            SPL = SPLp;
            minDamage = minDamagep;
            maxDamage = maxDamagep;
            daysOfSymptomsMin = daysOfSymptomsMinp;
            daysOfSymptomsMax = daysOfSymptomsMaxp;
        }
    }
}
