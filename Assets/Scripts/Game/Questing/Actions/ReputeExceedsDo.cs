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

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Executes a task when reputation with NPC exceeds value.
    /// This is not a trigger condition and will not trigger parent task.
    /// Owning task must be made active before this action starts checking condition each tick.
    /// </summary>
    public class ReputeExceedsDo : ActionTemplate
    {
        Symbol npcSymbol;
        Symbol taskSymbol;
        int minReputation;

        public override string Pattern
        {
            get { return @"repute with (?<npcSymbol>[a-zA-Z0-9_.-]+) exceeds (?<minReputation>\d+) do (?<taskSymbol>[a-zA-Z0-9_.]+)"; }
        }

        public ReputeExceedsDo(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            ReputeExceedsDo action = new ReputeExceedsDo(parentQuest);
            action.npcSymbol = new Symbol(match.Groups["npcSymbol"].Value);
            action.taskSymbol = new Symbol(match.Groups["taskSymbol"].Value);
            action.minReputation = Parser.ParseInt(match.Groups["minReputation"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            // Get related Person resource
            Person person = ParentQuest.GetPerson(npcSymbol);
            if (person == null)
                return;

            // Check faction with this NPC
            // Note: Believe this should be >= check despite name of action - to confirm
            if (GameManager.Instance.PlayerEntity.FactionData.GetReputation(person.FactionData.id) < minReputation)
                return;

            // Start target task
            ParentQuest.StartTask(taskSymbol);

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol npcSymbol;
            public Symbol taskSymbol;
            public int minReputation;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.npcSymbol = npcSymbol;
            data.taskSymbol = taskSymbol;
            data.minReputation = minReputation;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            npcSymbol = data.npcSymbol;
            taskSymbol = data.taskSymbol;
            minReputation = data.minReputation;
        }

        #endregion
    }
}