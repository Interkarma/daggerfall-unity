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
    public struct EffectEntry
    {
        public string Key;
        public EffectSettings Settings;
    }

    /// <summary>
    /// Settings for an entity effect bundle.
    /// </summary>
    public struct EffectBundleSettings
    {
        public BundleTypes BundleType;
        public TargetTypes TargetType;
        public ElementTypes ElementType;

        public EffectEntry[] Effects;
    }
}