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
    /// Changes reputation with an NPC by specified amount.
    /// </summary>
    public class ChangeReputeWith : ActionTemplate
    {
        Symbol target;
        int amount;

        public override string Pattern
        {
            get { return @"change repute with (?<target>[a-zA-Z0-9_.-]+) by (?<sign>[+-])(?<amount>\d+)"; }
        }

        public ChangeReputeWith(Quest parentQuest)
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

            // Get signed value
            int value;
            string sign = match.Groups["sign"].Value;
            if (sign == "+")
                value = Parser.ParseInt(match.Groups["amount"].Value);
            else if (sign == "-")
                value = -Parser.ParseInt(match.Groups["amount"].Value);
            else
                throw new System.Exception("Invalid sign encountered by ChangeReputeWith action");

            // Factory new action
            ChangeReputeWith action = new ChangeReputeWith(parentQuest);
            action.target = new Symbol(match.Groups["target"].Value);
            action.amount = value;

            return action;
        }

        public override void Update(Task caller)
        {
            // Get related Person resource
            Person person = ParentQuest.GetPerson(target);
            if (person == null)
            {
                // Stop if Person does not exist
                SetComplete();
                return;
            }

            // Change reputation with target
            GameManager.Instance.PlayerEntity.FactionData.ChangeReputation(person.FactionData.id, amount, true);

            SetComplete();
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
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            target = data.target;
            amount = data.amount;
        }

        #endregion
    }
}