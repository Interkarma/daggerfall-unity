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
    /// Drops an NPC or Foe portrait from HUD that player is currently escorting.
    /// </summary>
    public class DropFace : ActionTemplate
    {
        Symbol personSymbol;
        Symbol foeSymbol;

        public override string Pattern
        {
            get {
                return
                    @"drop (?<anNPC>[a-zA-Z0-9_.-]+) face|" +
                    @"drop foe (?<aFoe>[a-zA-Z0-9_.-]+) face";
            }
        }

        public DropFace(Quest parentQuest)
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
            DropFace action = new DropFace(parentQuest);
            action.personSymbol = new Symbol(match.Groups["anNPC"].Value);
            action.foeSymbol = new Symbol(match.Groups["aFoe"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            base.Update(caller);

            // Drop related Person or Foe resource
            if (personSymbol != null && !string.IsNullOrEmpty(personSymbol.Name))
            {
                Person person = ParentQuest.GetPerson(personSymbol);
                if (person != null)
                    DaggerfallUI.Instance.DaggerfallHUD.EscortingFaces.DropFace(person);
            }
            else if (foeSymbol != null && !string.IsNullOrEmpty(foeSymbol.Name))
            {
                Foe foe = ParentQuest.GetFoe(foeSymbol);
                if (foe != null)
                    DaggerfallUI.Instance.DaggerfallHUD.EscortingFaces.DropFace(foe);
            }

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol personSymbol;
            public Symbol foeSymbol;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.personSymbol = personSymbol;
            data.foeSymbol = foeSymbol;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            personSymbol = data.personSymbol;
            foeSymbol = data.foeSymbol;
        }

        #endregion
    }
}