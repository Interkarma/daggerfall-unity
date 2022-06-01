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
    /// Moves NPC to a reserved site.
    /// Fixed NPCs always starts in their home location but quests can move them around as needed.
    /// Random NPCs are instantiated to target location only as they don't otherwise exist in world.
    /// Site must be reserved before moving NPC to that location.
    /// </summary>
    public class PlaceNpc : ActionTemplate
    {
        Symbol npcSymbol;
        Symbol placeSymbol;
        int marker = -1;

        public override string Pattern
        {
            get {
                return @"place npc (?<anNPC>[a-zA-Z0-9_.-]+) at (?<aPlace>\w+) marker (?<marker>\d+)|" +
                       @"place npc (?<anNPC>[a-zA-Z0-9_.-]+) at (?<aPlace>\w+)";
            }
        }

        public PlaceNpc(Quest parentQuest)
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
            PlaceNpc action = new PlaceNpc(parentQuest);
            action.npcSymbol = new Symbol(match.Groups["anNPC"].Value);
            action.placeSymbol = new Symbol(match.Groups["aPlace"].Value);

            // Set custom marker
            Group markerGroup = match.Groups["marker"];
            if (markerGroup.Success)
                action.marker = Parser.ParseInt(markerGroup.Value);

            return action;
        }

        public override void Update(Task caller)
        {
            base.Update(caller);

            // Create SiteLink if not already present
            if (!QuestMachine.HasSiteLink(ParentQuest, placeSymbol))
                QuestMachine.CreateSiteLink(ParentQuest, placeSymbol);

            // Attempt to get Person resource
            Person person = ParentQuest.GetPerson(npcSymbol);
            if (person == null)
            {
                SetComplete();
                throw new Exception(string.Format("Could not find Person resource symbol {0}", npcSymbol));
            }

            // Do nothing if Person is destroyed
            if (person.IsDestroyed)
            {
                SetComplete();
                return;
            }

            // Attempt to get Place resource
            Place place = ParentQuest.GetPlace(placeSymbol);
            if (place == null)
            {
                SetComplete();
                throw new Exception(string.Format("Could not find Place resource symbol {0}", placeSymbol));
            }

            // Is target an individual NPC that is supposed to be at home
            // Daggerfall never seems to use "create npc at" or "place npc" for "athome" NPCs
            // Treating this as an error and logging as such, but don't throw an exception
            // Just log, terminate action, and get out of dodge
            if (person.IsIndividualNPC && person.IsIndividualAtHome)
            {
                Debug.LogErrorFormat("Quest tried to place Person {0} [_{1}_] at Place _{2}_ but they are supposed to be atHome", person.DisplayName, person.Symbol.Name, place.Symbol.Name);
                SetComplete();
                return;
            }

            // Assign Person to Place
            place.AssignQuestResource(person.Symbol, marker);
            person.SetAssignedPlaceSymbol(placeSymbol);

            // Person is also unhidden when placed
            person.IsHidden = false;

            TalkManager.Instance.ForceTopicListsUpdate();

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol npcSymbol;
            public Symbol placeSymbol;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.npcSymbol = npcSymbol;
            data.placeSymbol = placeSymbol;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            npcSymbol = data.npcSymbol;
            placeSymbol = data.placeSymbol;
        }

        #endregion
    }
}