// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
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
    public class ChangeReputeWith : ActionTemplate
    {
        Symbol target;
        int amount;

        public override string Pattern
        {
            get { return @"change repute with (?<target>[a-zA-Z0-9_.-]+) by (?<amount>\d+)"; }
        }

        public ChangeReputeWith(Quest parentQuest)
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
            ChangeReputeWith action = new ChangeReputeWith(parentQuest);
            action.target = new Symbol(match.Groups["target"].Value);
            action.amount = Parser.ParseInt(match.Groups["amount"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            // TODO: Perform action changes
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol target;
            public int amount;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.target = target;
            data.amount = amount;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            SaveData_v1 data = (SaveData_v1)dataIn;
            if (dataIn == null)
                return;

            target = data.target;
            amount = data.amount;
        }

        #endregion
    }
}