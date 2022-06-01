// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using DaggerfallConnect.FallExe;

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
        public ItemMakerFlags ItemMakerFlags;                       // Item maker features
        public EnchantmentPayloadFlags EnchantmentPayloadFlags;     // How an enchantment wants to receive execution callbacks to deliver payload
        public bool DisableReflectiveEnumeration;                   // Prevents effect template from being registered automatically with broker
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
    /// Reference a spell icon within an icon pack.
    /// A null, empty, or invalid key value will fallback to a classic icon.
    /// </summary>
    [Serializable]
    public struct SpellIcon
    {
        public string key;                                          // Key of icon pack matching source filename without extension
        public int index;                                           // Index of key within pack
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
        public EnchantmentParam? EnchantmentParam;

        public EffectEntry(string key, EnchantmentParam? enchantmentParam = null)
        {
            Key = key;
            Settings = new EffectSettings();
            EnchantmentParam = enchantmentParam;
        }

        public EffectEntry(string key, EffectSettings settings, EnchantmentParam? enchantmentParam = null)
        {
            Key = key;
            Settings = settings;
            EnchantmentParam = enchantmentParam;
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
        public BundleRuntimeFlags RuntimeFlags;
        public string Name;
        public int IconIndex;
        public SpellIcon Icon;
        public bool MinimumCastingCost;
        public bool NoCastingAnims;
        public string Tag;
        public EffectEntry[] Effects;
        public LegacyEffectEntry[] LegacyEffects;
        public int? StandardSpellIndex;
    }

    /// <summary>
    /// Settings for a single enchantment on item.
    /// </summary>
    [Serializable]
    public struct EnchantmentSettings : IEquatable<EnchantmentSettings>
    {
        public int Version;
        public string EffectKey;
        public EnchantmentTypes ClassicType;
        public short ClassicParam;
        public string CustomParam;
        public string PrimaryDisplayName;
        public string SecondaryDisplayName;
        public int EnchantCost;
        public int ParentEnchantment;

        public bool Equals(EnchantmentSettings other)
        {
            return
                Version == other.Version &&
                EffectKey == other.EffectKey &&
                ClassicType == other.ClassicType &&
                ClassicParam == other.ClassicParam &&
                CustomParam == other.CustomParam &&
                PrimaryDisplayName == other.PrimaryDisplayName &&
                SecondaryDisplayName == other.SecondaryDisplayName &&
                EnchantCost == other.EnchantCost;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is EnchantmentSettings))
                return false;

            return Equals((EnchantmentSettings)obj);
        }

        public static bool operator == (EnchantmentSettings enchantment1, EnchantmentSettings enchantment2)
        {
            if (((object)enchantment1) == null || ((object)enchantment2) == null)
                return Object.Equals(enchantment1, enchantment2);

            return enchantment1.Equals(enchantment2);
        }

        public static bool operator != (EnchantmentSettings enchantment1, EnchantmentSettings enchantment2)
        {
            if (((object)enchantment1) == null || ((object)enchantment2) == null)
                return !Object.Equals(enchantment1, enchantment2);

            return !(enchantment1.Equals(enchantment2));
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + Version.GetHashCode();
            if (!string.IsNullOrEmpty(EffectKey)) hash = hash * 23 + EffectKey.GetHashCode();
            hash = hash * 23 + ClassicType.GetHashCode();
            hash = hash * 23 + ClassicParam.GetHashCode();
            if (!string.IsNullOrEmpty(CustomParam)) hash = hash * 23 + CustomParam.GetHashCode();
            if (!string.IsNullOrEmpty(PrimaryDisplayName)) hash = hash * 23 + PrimaryDisplayName.GetHashCode();
            if (!string.IsNullOrEmpty(SecondaryDisplayName)) hash = hash * 23 + SecondaryDisplayName.GetHashCode();
            hash = hash * 23 + EnchantCost.GetHashCode();

            return hash;
        }
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

    /// <summary>
    /// Defines a custom enchantment for items.
    /// Classic enchantments use a type/param number pair in DaggerfallEnchantment.
    /// Custom enchantments use a key/param string pair in CustomEnchantment.
    /// </summary>
    [Serializable]
    public struct CustomEnchantment
    {
        public string EffectKey;                                    // Define the effect used by this enchantment
        public string CustomParam;                                  // Passed back to effect to locate/invoke enchantment settings
    }

    /// <summary>
    /// References either a classic or custom spell bundle.
    /// Always considered to reference a custom spell bundle when CustomKey is not null or empty.
    /// </summary>
    [Serializable]
    public struct SpellReference
    {
        public int ClassicID;                                       // Spell ID into SPELLS.STD
        public string CustomKey;                                    // Key into custom spell bundle offers
    }

    /// <summary>
    /// Flexible enchantment parameter for either a classic effect or a custom effect.
    /// This is stored with EffectEntry for enchantment bundles and assigned to live effect instance.
    /// Formalising this to a data structure allows for expanding custom enchantment params later.
    /// </summary>
    [Serializable]
    public struct EnchantmentParam
    {
        public short ClassicParam;                                  // Classic echantment param
        public string CustomParam;                                  // Custom enchantment param
    }

    /// <summary>
    /// Optional information returned to framework by enchantment payload callbacks.
    /// </summary>
    [Serializable]
    public struct PayloadCallbackResults
    {
        public int strikesModulateDamage;                           // Amount to plus/minus from damage after Strikes effect payload
        public int durabilityLoss;                                  // Amount of durability lost after callback
        public bool removeItem;                                     // Removes item from collection if true
    }

    /// <summary>
    /// Defines a single forced enchantment effect with param.
    /// </summary>
    public struct ForcedEnchantment
    {
        public string key;
        public EnchantmentParam param;

        public ForcedEnchantment(string key, short classicParam = -1)
        {
            this.key = key;
            param = new EnchantmentParam() { ClassicParam = classicParam, CustomParam = string.Empty };
        }
    }

    /// <summary>
    /// Contains a set of forced effects keyed to a valid mobile type.
    /// </summary>
    public struct ForcedEnchantmentSet
    {
        public MobileTypes soulType;
        public ForcedEnchantment[] forcedEffects;
    }
}
