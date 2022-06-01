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
using System;
using DaggerfallConnect.Arena2;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Triggers when player reputation with a special named NPC equals or exceeds a minimum value.
    /// This is only used in a small number of canonical quests which refer to special individuals.
    /// Examples are King Gothryd, Queen Aubk-i, Prince Lhotun, etc.
    /// </summary>
    public class WhenReputeWith : ActionTemplate
    {
        string npcName;
        int npcFactionID;
        int minRepValue;

        public override string Pattern
        {
            get { return @"when repute with (?<individualNPCName>[a-zA-Z0-9_.-]+) is at least (?<minRepValue>\d+)"; }
        }

        public WhenReputeWith(Quest parentQuest)
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
            WhenReputeWith action = new WhenReputeWith(parentQuest);

            // Confirm this is an individual NPC
            string individualNPCName = match.Groups["individualNPCName"].Value;
            int factionID = Person.GetIndividualFactionID(individualNPCName);
            if (factionID != -1)
            {
                FactionFile.FactionData factionData = Person.GetFactionData(factionID);
                if (factionData.type != (int)FactionFile.FactionTypes.Individual)
                    throw new Exception(string.Format("WhenReputeWith: NPC {0} with FactionID {1} is not an individual NPC", individualNPCName, factionID));
            }

            action.npcName = individualNPCName;
            action.npcFactionID = factionID;
            action.minRepValue = Parser.ParseInt(match.Groups["minRepValue"].Value);

            return action;
        }

        public override bool CheckTrigger(Task caller)
        {
            FactionFile.FactionData factionData;
            if (!GameManager.Instance.PlayerEntity.FactionData.GetFactionData(npcFactionID, out factionData))
                return false;

            return factionData.rep >= minRepValue;
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public string npcName;
            public int npcFactionID;
            public int minRepValue;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.npcName = npcName;
            data.npcFactionID = npcFactionID;
            data.minRepValue = minRepValue;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            npcName = data.npcName;
            npcFactionID = data.npcFactionID;
            minRepValue = data.minRepValue;
        }

        #endregion
    }
}