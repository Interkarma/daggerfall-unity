// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2021 Daggerfall Workshop
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
    /// dialog link command used in quests.
    /// </summary>
    public class DialogLink : ActionTemplate
    {        
        Symbol placeSymbol;
        Symbol npcSymbol;
        Symbol itemSymbol;

        public override string Pattern
        {
            get { return @"dialog link for location (?<aSite>\w+) person (?<anNPC>\w+) item (?<anItem>\w+)|" +
                         @"dialog link for location (?<aSite>\w+) person (?<anNPC>\w+)|" +
                         @"dialog link for location (?<aSite>\w+) item (?<anItem>\w+)|" +
                         @"dialog link for location (?<aSite>\w+)|" +
                         @"dialog link for person (?<anNPC>\w+) item (?<anItem>\w+)|" +
                         @"dialog link for person (?<anNPC>\w+)|" +
                         @"dialog link for item (?<anItem>\w+)"; }
        }

        public DialogLink(Quest parentQuest)
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
            DialogLink action = new DialogLink(parentQuest);
            if (!string.IsNullOrEmpty(match.Groups["aSite"].Value))
                action.placeSymbol = new Symbol(match.Groups["aSite"].Value);
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

            // first create dialog link for just the separated resources (which will hide them)
            if (place != null)
            {
                GameManager.Instance.TalkManager.DialogLinkForQuestInfoResource(ParentQuest.UID, place, TalkManager.QuestInfoResourceType.Location);
            }
            if (person != null)
            {
                GameManager.Instance.TalkManager.DialogLinkForQuestInfoResource(ParentQuest.UID, person, TalkManager.QuestInfoResourceType.Person);
            }
            if (item != null)
            {
                GameManager.Instance.TalkManager.DialogLinkForQuestInfoResource(ParentQuest.UID, item, TalkManager.QuestInfoResourceType.Thing);
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