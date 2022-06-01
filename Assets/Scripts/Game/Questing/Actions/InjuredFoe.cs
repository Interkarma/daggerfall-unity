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

using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// Triggers when a Foe has been injured.
    /// Will not fire if Foe dies immediately (e.g. player one-shots enemy).
    /// </summary>
    public class InjuredFoe : ActionTemplate
    {
        Symbol foeSymbol;
        int textID;

        public override string Pattern
        {
            get { return @"injured (?<aFoe>[a-zA-Z0-9_.-]+) saying (?<textID>\d+)|injured (?<aFoe>[a-zA-Z0-9_.-]+)"; }
        }

        public InjuredFoe(Quest parentQuest)
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
            InjuredFoe action = new InjuredFoe(parentQuest);
            action.foeSymbol = new Symbol(match.Groups["aFoe"].Value);
            action.textID = Parser.ParseInt(match.Groups["textID"].Value);

            return action;
        }

        public override bool CheckTrigger(Task caller)
        {
            // Get related Foe resource
            Foe foe = ParentQuest.GetFoe(foeSymbol);
            if (foe == null)
                return false;

            // Check injured flag
            if (foe.InjuredTrigger)
            {
                // Optionally show message
                if (textID != 0)
                    ParentQuest.ShowMessagePopup(textID);

                return true;
            }

            return false;
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol foeSymbol;
            public int textID;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.foeSymbol = foeSymbol;
            data.textID = textID;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            foeSymbol = data.foeSymbol;
            textID = data.textID;
        }

        #endregion
    }
}