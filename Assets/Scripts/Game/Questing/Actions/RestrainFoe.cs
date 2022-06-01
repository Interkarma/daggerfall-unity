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

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// Makes a foe non-hostile until player strikes them again.
    /// Quest authors should also use a popup message to inform player that foe is no longer aggressive.
    /// </summary>
    public class RestrainFoe : ActionTemplate
    {
        Symbol foeSymbol;

        public override string Pattern
        {
            get { return @"restrain foe (?<aFoe>[a-zA-Z0-9_.-]+)"; }
        }

        public RestrainFoe(Quest parentQuest)
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
            RestrainFoe action = new RestrainFoe(parentQuest);
            action.foeSymbol = new Symbol(match.Groups["aFoe"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            // Get related Foe resource
            Foe foe = ParentQuest.GetFoe(foeSymbol);
            if (foe == null)
                return;

            // Raise the restrained flag
            foe.SetRestrained();

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol foeSymbol;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.foeSymbol = foeSymbol;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            foeSymbol = data.foeSymbol;
        }

        #endregion
    }
}