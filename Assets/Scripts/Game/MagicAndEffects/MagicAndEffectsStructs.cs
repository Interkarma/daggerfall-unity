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

namespace DaggerfallWorkshop.Game.MagicAndEffects
{
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
        public EffectEntry[] Effects;
        public LegacyEffectEntry[] LegacyEffects;
    }
}