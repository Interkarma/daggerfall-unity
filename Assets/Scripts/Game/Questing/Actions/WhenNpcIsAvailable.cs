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
    /// Triggers when player clicks on an individual NPC that is not currently assigned to another quest.
    /// This is only used in a small number of canonical quests which refer to special individuals.
    /// Examples are King Gothryd, Queen Aubk-i, Prince Lhotun, etc.
    /// </summary>
    public class WhenNpcIsAvailable : ActionTemplate
    {
        string npcName;
        int npcFactionID;

        StaticNPC clickMemory = null;

        public override string Pattern
        {
            get { return @"when (?<individualNPCName>[a-zA-Z0-9_.-]+) is available"; }
        }

        public WhenNpcIsAvailable(Quest parentQuest)
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
            WhenNpcIsAvailable action = new WhenNpcIsAvailable(parentQuest);

            // Confirm this is an individual NPC
            string individualNPCName = match.Groups["individualNPCName"].Value;
            int factionID = Person.GetIndividualFactionID(individualNPCName);
            if (factionID != -1)
            {
                FactionFile.FactionData factionData = Person.GetFactionData(factionID);
                if (factionData.type != (int)FactionFile.FactionTypes.Individual)
                {
                    SetComplete();
                    throw new Exception(string.Format("WhenNpcIsAvailable: NPC {0} with FactionID {1} is not an individual NPC", individualNPCName, factionID));
                }
            }

            action.npcName = individualNPCName;
            action.npcFactionID = factionID;

            return action;
        }

        public override bool CheckTrigger(Task caller)
        {
            // Update faction listener for this individual
            // This allows other agencies to determine if a quest is actively listening on this NPC
            QuestMachine.Instance.AddFactionListener(npcFactionID, this);

            // Check if player has clicked on anyone
            StaticNPC lastClicked = QuestMachine.Instance.LastNPCClicked;
            if (lastClicked == null)
                return false;

            // Exit if player is spam clicking same NPC, they will need to click someone else and come back
            if (lastClicked == clickMemory)
                return false;
            else
                clickMemory = null;

            // Check if player clicked on our NPC
            if (lastClicked.Data.factionID != npcFactionID)
                return false;

            // Not clear when named NPC should be considered available
            // For now they are considered available if no Person record exists for their individual FactionID
            Person[] foundActive = QuestMachine.Instance.ActiveFactionPersons(npcFactionID);
            if (foundActive == null || foundActive.Length == 0)
            {
                // Remember we just clicked this NPC and exit
                clickMemory = lastClicked;
                return true;
            }

            return false;
        }

        public override void Dispose()
        {
            base.Dispose();

            QuestMachine.Instance.RemoveFactionListener(npcFactionID);
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public string npcName;
            public int npcFactionID;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.npcName = npcName;
            data.npcFactionID = npcFactionID;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            npcName = data.npcName;
            npcFactionID = data.npcFactionID;
        }

        #endregion
    }
}