// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    
// 

using System.Text.RegularExpressions;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// Take an amount from player, and start a task depending on if they could pay.
    /// </summary>
    public class PayMoney : ActionTemplate
    {
        Symbol paidTaskSymbol;
        Symbol notTaskSymbol;
        int amount;

        public override string Pattern
        {
            get { return @"pay (?<amount>\d+) money do (?<paidTaskName>[a-zA-Z0-9_.]+) otherwise do (?<notTaskName>[a-zA-Z0-9_.]+)"; }
        }

        public PayMoney(Quest parentQuest)
            : base(parentQuest)
        {
            allowRearm = true;
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            PayMoney action = new PayMoney(parentQuest);
            action.amount = Parser.ParseInt(match.Groups["amount"].Value);
            action.paidTaskSymbol = new Symbol(match.Groups["paidTaskName"].Value);
            action.notTaskSymbol = new Symbol(match.Groups["notTaskName"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            base.Update(caller);

            // When an amount is specified
            if (amount > 0)
            {
                // Does player have enough gold?
                if (GameManager.Instance.PlayerEntity.GetGoldAmount() >= amount)
                {
                    // Then deduct gold
                    GameManager.Instance.PlayerEntity.DeductGoldAmount(amount);
                    ParentQuest.StartTask(paidTaskSymbol);
                }
                else
                {
                    // Otherwise trigger secondary task and exit
                    ParentQuest.StartTask(notTaskSymbol);
                }
            }

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol paidTaskSymbol;
            public Symbol notTaskSymbol;
            public int amount;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.paidTaskSymbol = paidTaskSymbol;
            data.notTaskSymbol = notTaskSymbol;
            data.amount = amount;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            paidTaskSymbol = data.paidTaskSymbol;
            notTaskSymbol = data.notTaskSymbol;
            amount = data.amount;
        }

        #endregion
    }
}