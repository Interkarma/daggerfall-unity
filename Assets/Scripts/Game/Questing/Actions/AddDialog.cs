// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Michael Rauter (Nystul)
// 
// Notes:
//

using System.Text.RegularExpressions;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// add dialog command used in quests to make talk options available.
    /// </summary>
    public class AddDialog : ActionTemplate
    {
        Symbol placeSymbol;
        Symbol npcSymbol;
        Symbol itemSymbol;

        public override string Pattern
        {
            get { return @"add dialog for location (?<aPlace>\w+) person (?<anNPC>\w+) item (?<anItem>\w+)|" +
                         @"add dialog for person (?<anNPC>\w+) item (?<anItem>\w+)|" +
                         @"add dialog for location (?<aPlace>\w+) person (?<anNPC>\w+)|" +
                         @"add dialog for location (?<aPlace>\w+) item (?<anItem>\w+)|" +
                         @"add dialog for location (?<aPlace>\w+)|" +
                         @"add dialog for person (?<anNPC>\w+)|" +
                         @"add dialog for item (?<anItem>\w+)"; }
        }

        public AddDialog(Quest parentQuest)
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
            AddDialog action = new AddDialog(parentQuest);
            if (!string.IsNullOrEmpty(match.Groups["aPlace"].Value))
                action.placeSymbol = new Symbol(match.Groups["aPlace"].Value);
            if (!string.IsNullOrEmpty(match.Groups["anNPC"].Value))
                action.npcSymbol = new Symbol(match.Groups["anNPC"].Value);
            if (!string.IsNullOrEmpty(match.Groups["anItem"].Value))
                action.itemSymbol = new Symbol(match.Groups["anItem"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            // Get related Location resource
            Place place = ParentQuest.GetPlace(placeSymbol);
            // Get related Person resource
            Person person = ParentQuest.GetPerson(npcSymbol);
            // Get related Item resource
            Item item = ParentQuest.GetItem(itemSymbol);

            // add dialog for resources
            if (place != null)
            {
                GameManager.Instance.TalkManager.AddDialogForQuestInfoResource(ParentQuest.UID, place.Symbol.Name, TalkManager.QuestInfoResourceType.Location, false);
            }
            if (person != null)
            {
                GameManager.Instance.TalkManager.AddDialogForQuestInfoResource(ParentQuest.UID, person.Symbol.Name, TalkManager.QuestInfoResourceType.Person, false);
            }
            if (item != null)
            {
                GameManager.Instance.TalkManager.AddDialogForQuestInfoResource(ParentQuest.UID, item.Symbol.Name, TalkManager.QuestInfoResourceType.Thing, false);
            }

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol placeSymbol;
            public Symbol npcSymbol;
            public Symbol itemSymbol;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.placeSymbol = placeSymbol;
            data.npcSymbol = npcSymbol;
            data.itemSymbol = itemSymbol;
            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            placeSymbol = data.placeSymbol;
            npcSymbol = data.npcSymbol;
            itemSymbol = data.itemSymbol;
        }

        #endregion
    }
}