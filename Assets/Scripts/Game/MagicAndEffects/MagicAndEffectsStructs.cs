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
    /// Settings for an entity effect bunde.
    /// Some effects in bundle may not consume all settings depending on scripted behaviour of effect iself.
    /// </summary>
    public struct EffectBundleSettings
    {
        public int DurationBase;
        public int DurationBonus;
        public int DurationBonusPerLevel;

        public int ChanceBase;
        public int ChanceBonus;
        public int ChanceBonusPerLevel;
        public int ChanceCostResult;

        public int MagnitudeBaseMin;
        public int MagnitudeBaseMax;
        public int MagnitudeBonusMin;
        public int MagnitudeBonusMax;
        public int MagnitudeBonusPerLevel;
    }
}