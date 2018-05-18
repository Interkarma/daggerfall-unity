// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
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
    /// Triggers when player has reached specified skill level or higher.
    /// </summary>
    public class SkillLevel : ActionTemplate
    {
        DFCareer.Skills skill;
        int minSkillValue;

        public override string Pattern
        {
            get { return @"skill (?<skillName>\w+) (?<minSkillValue>\d+)"; }
        }

        public SkillLevel(Quest parentQuest)
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
            SkillLevel action = new SkillLevel(parentQuest);
            string skillName = match.Groups["skillName"].Value;
            if (!Enum.IsDefined(typeof(DFCareer.Skills), skillName, true))
            {
                SetComplete();   
                throw new Exception(string.Format("SkillLevel: Skill name {0} is not a known Daggerfall skill", skillName));
            }
            action.skill = Enum.Parse(typeof(DFCareer.Skills), skillName, true);
            action.minSkillValue = Parser.ParseInt(match.Groups["minSkillValue"].Value);

            return action;
        }

        public override bool CheckTrigger(Task caller)
        {
            return GameManager.Instance.PlayerEntity.Skills.GetLiveSkillValue(skill) >= minSkillValue;
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public int minSkillValue;
            public int skill;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.minSkillValue = minSkillValue;
            data.skill = (int) skill;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            SaveData_v1 data = (SaveData_v1)dataIn;
            if (dataIn == null)
                return;

            minSkillValue = data.minSkillValue;
            skill = (DFCareer.Skills) data.skill;
        }

        #endregion
    }
}