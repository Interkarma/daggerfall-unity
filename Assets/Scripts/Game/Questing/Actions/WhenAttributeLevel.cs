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

using System.Text.RegularExpressions;
using FullSerializer;
using DaggerfallConnect;
using System;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Triggers when character reaches minimum value for specified attribute.
    /// </summary>
    public class WhenAttributeLevel : ActionTemplate
    {
        DFCareer.Stats attribute;
        int minAttributeValue;

        public override string Pattern
        {
            get { return @"when attribute (?<attributeName>\w+) is at least (?<minAttributeValue>\d+)"; }
        }

        public WhenAttributeLevel(Quest parentQuest)
            : base(parentQuest)
        {
            IsTriggerCondition = true;
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            WhenAttributeLevel action = new WhenAttributeLevel(parentQuest);
            string attributeName = match.Groups["attributeName"].Value;
            if (!Enum.IsDefined(typeof(DFCareer.Stats), attributeName))
            {
                SetComplete();   
                throw new Exception(string.Format("WhenAttributeLevel: Attribute name {0} is not a known Daggerfall attribute", attributeName));
            }
            action.attribute = (DFCareer.Stats)Enum.Parse(typeof(DFCareer.Stats), attributeName);
            action.minAttributeValue = Parser.ParseInt(match.Groups["minAttributeValue"].Value);

            return action;
        }

        public override bool CheckTrigger(Task caller)
        {
            return GameManager.Instance.PlayerEntity.Stats.GetLiveStatValue(attribute) >= minAttributeValue;
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public DFCareer.Stats attribute;
            public int minAttributeValue;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.attribute = attribute;
            data.minAttributeValue = minAttributeValue;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            attribute = data.attribute;
            minAttributeValue = data.minAttributeValue;
        }

        #endregion
    }
}