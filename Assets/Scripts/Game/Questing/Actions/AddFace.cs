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
    /// Adds an NPC or Foe portrait to HUD which indicates player is escorting this NPC or Foe.
    /// </summary>
    public class AddFace : ActionTemplate
    {
        Symbol personSymbol;
        Symbol foeSymbol;
        int sayingID;

        public override string Pattern
        {
            get
            {
                return
                    @"add (?<anNPC>[a-zA-Z0-9_.-]+) face saying (?<sayingID>\d+)|add (?<anNPC>[a-zA-Z0-9_.-]+) face|" +
                    @"add foe (?<aFoe>[a-zA-Z0-9_.-]+) face saying (?<sayingID>\d+)|add foe (?<aFoe>[a-zA-Z0-9_.-]+) face";
            }
        }

        public AddFace(Quest parentQuest)
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
            AddFace action = new AddFace(parentQuest);
            action.personSymbol = new Symbol(match.Groups["anNPC"].Value);
            action.foeSymbol = new Symbol(match.Groups["aFoe"].Value);
            action.sayingID = Parser.ParseInt(match.Groups["sayingID"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            base.Update(caller);

            // Popup saying message
            if (sayingID != 0)
                ParentQuest.ShowMessagePopup(sayingID, true);

            // Add related Person or Foe resource
            if (personSymbol != null && !string.IsNullOrEmpty(personSymbol.Name))
            {
                Person person = ParentQuest.GetPerson(personSymbol);
                if (person != null)
                    DaggerfallUI.Instance.DaggerfallHUD.EscortingFaces.AddFace(person);
            }
            else if (foeSymbol != null && !string.IsNullOrEmpty(foeSymbol.Name))
            {
                Foe foe = ParentQuest.GetFoe(foeSymbol);
                if (foe != null)
                    DaggerfallUI.Instance.DaggerfallHUD.EscortingFaces.AddFace(foe);
            }

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol personSymbol;
            public Symbol foeSymbol;
            public int sayingID;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.personSymbol = personSymbol;
            data.foeSymbol = foeSymbol;
            data.sayingID = sayingID;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            personSymbol = data.personSymbol;
            foeSymbol = data.foeSymbol;
            sayingID = data.sayingID;
        }

        #endregion
    }
}