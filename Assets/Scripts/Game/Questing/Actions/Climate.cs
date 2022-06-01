// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: kaboissonneault (kaboissonneault@gmail.com)
// Contributors:    
// 
// Notes:
//

using System;
using System.Text.RegularExpressions;
using FullSerializer;

using DaggerfallConnect;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// DFU extension action. Triggers when the player is at a location with the right climate
    /// </summary>
    public class Climate : ActionTemplate
    {
        MapsFile.Climates climate;
        DFLocation.ClimateBaseType climateBase;

        public override string Pattern
        {
            get
            {
                return @"climate (?<climate>desert|desert2|mountain|mountainwoods|rainforest|ocean|swamp|subtropical|woodlands|hauntedwoodlands)"
                  + @"|climate (?<base>base) (?<climatebase>desert|mountain|temperate|swamp)";
            }
        }

        public Climate(Quest parentQuest)
                    : base(parentQuest)
        {
            IsTriggerCondition = true;
            IsAlwaysOnTriggerCondition = true;
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            Climate action = new Climate(parentQuest);
            if (!string.IsNullOrEmpty(match.Groups["base"].Value))
            {
                if (!Enum.TryParse(match.Groups["climatebase"].Value, true /*ignore case*/, out action.climateBase))
                {
                    throw new Exception("Climate: Syntax is 'climate base desert|mountain|temperate|swamp'");
                }
            }
            else
            {
                action.climateBase = DFLocation.ClimateBaseType.None;
                if (!Enum.TryParse(match.Groups["climate"].Value, true /*ignore case*/, out action.climate))
                {
                    throw new Exception("Climate: Syntax is 'climate desert|desert2|mountain|mountainwoods|rainforest|ocean|swamp|subtropical|woodlands|hauntedwoodlands'");
                }
            }

            return action;
        }

        public override bool CheckTrigger(Task caller)
        {
            if (climateBase != DFLocation.ClimateBaseType.None)
            {
                return GameManager.Instance.PlayerGPS.ClimateSettings.ClimateType == climateBase;
            }
            else
            {
                return GameManager.Instance.PlayerGPS.ClimateSettings.WorldClimate == (int)climate;
            }
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public MapsFile.Climates climate;
            public DFLocation.ClimateBaseType climateBase;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.climate = climate;
            data.climateBase = climateBase;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            climate = data.climate;
            climateBase = data.climateBase;
        }
        #endregion

    }
}
