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
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Utility;


namespace DaggerfallWorkshop.Game.Questing
{
    class TrainPc : ActionTemplate
    {
        DFCareer.Skills skill;

        public override string Pattern
        {
            get { return @"train pc (?<skillName>\w+)"; }
        }

        public TrainPc(Quest parentQuest)
            : base(parentQuest)
        {
            allowRearm = false;
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            TrainPc action = new TrainPc(parentQuest);
            string skillName = match.Groups["skillName"].Value;
            if (!Enum.IsDefined(typeof(DFCareer.Skills), skillName))
            {
                SetComplete();
                throw new Exception(string.Format("TrainPc: Skill name {0} is not a known Daggerfall skill", skillName));
            }
            action.skill = (DFCareer.Skills)Enum.Parse(typeof(DFCareer.Skills), skillName);

            return action;
        }

        public override void Update(Task caller)
        {
            base.Update(caller);

            // Quest successful
            ParentQuest.QuestSuccess = true;

            // Show quest complete message
            DaggerfallMessageBox messageBox = ParentQuest.ShowMessagePopup((int)QuestMachine.QuestMessages.QuestComplete);

            // Schedule loot window to open when player dismisses message
            messageBox.OnClose += QuestCompleteMessage_OnClose;

            SetComplete();
        }

        private void QuestCompleteMessage_OnClose()
        {
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;

            // Train the skill, for free
            DaggerfallDateTime now = DaggerfallUnity.Instance.WorldTime.Now;
            playerEntity.TimeOfLastSkillTraining = now.ToClassicDaggerfallTime();
            now.RaiseTime(DaggerfallDateTime.SecondsPerHour * 3);
            playerEntity.DecreaseFatigue(PlayerEntity.DefaultFatigueLoss * 180);
            int skillAdvancementMultiplier = DaggerfallSkills.GetAdvancementMultiplier(skill);
            short tallyAmount = (short)(UnityEngine.Random.Range(10, 20 + 1) * skillAdvancementMultiplier);
            playerEntity.TallySkill(skill, tallyAmount);
        }

        #region Serialization 
        [fsObject("v1")]
        public struct SaveData_v1
        {
            public int skill;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.skill = (int)skill;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            skill = (DFCareer.Skills)data.skill;
        }

        #endregion
    }
}
