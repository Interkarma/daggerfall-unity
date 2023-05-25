// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using System;
using DaggerfallConnect;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Game.Entity
{
    public partial class DaggerfallStats : IMacroContextProvider
    {
        [NonSerializedAttribute]
        private StatsMacroDataSource dataSource;

        public MacroDataSource GetMacroDataSource()
        {
            if (dataSource == null)
                dataSource = new StatsMacroDataSource(this);
            return dataSource;
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for stats in Daggerfall Unity. Context
        /// is based off the last stat macro accessed.
        /// </summary>
        /// Assuming thread safety unneccessary as DFU wont be creating messages in parallel.
        /// 
        private class StatsMacroDataSource : MacroDataSource
        {
            private readonly int[] statThresholds = { 10, 20, 30, 40, 50, 60, 70, 80, 90, 1000 };
            private readonly string[] statRatingKeys = { "strRatings", "intRatings", "wilRatings", "agiRatings", "endRatings", "perRatings", "spdRatings", "lucRatings", };

            private DFCareer.Stats lastStat = DFCareer.Stats.Strength;
            private int lastStatValue;

            private readonly DaggerfallStats parent;
            public StatsMacroDataSource(DaggerfallStats parent)
            {
                this.parent = parent;
            }

            public override string AttributeRating()
            {
                int i = 0;
                while (lastStatValue >= statThresholds[i])
                    i++;
                return TextManager.Instance.GetLocalizedTextList(statRatingKeys[(int)lastStat])[i];
            }

            public override string Str()
            {
                lastStat = DFCareer.Stats.Strength;
                lastStatValue = parent.LiveStrength;
                return lastStatValue.ToString();
            }

            public override string Int()
            {
                lastStat = DFCareer.Stats.Intelligence;
                lastStatValue = parent.LiveIntelligence;
                return lastStatValue.ToString();
            }

            public override string Wil()
            {
                lastStat = DFCareer.Stats.Willpower;
                lastStatValue = parent.LiveWillpower;
                return lastStatValue.ToString();
            }

            public override string Agi()
            {
                lastStat = DFCareer.Stats.Agility;
                lastStatValue = parent.LiveAgility;
                return lastStatValue.ToString();
            }

            public override string End()
            {
                lastStat = DFCareer.Stats.Endurance;
                lastStatValue = parent.LiveEndurance;
                return lastStatValue.ToString();
            }

            public override string Per()
            {
                lastStat = DFCareer.Stats.Personality;
                lastStatValue = parent.LivePersonality;
                return lastStatValue.ToString();
            }

            public override string Spd()
            {
                lastStat = DFCareer.Stats.Speed;
                lastStatValue = parent.LiveSpeed;
                return lastStatValue.ToString();
            }

            public override string Luck()
            {
                lastStat = DFCareer.Stats.Luck;
                lastStatValue = parent.LiveLuck;
                return lastStatValue.ToString();
            }

        }
    }
}
