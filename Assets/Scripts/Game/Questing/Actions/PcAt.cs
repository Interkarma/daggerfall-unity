// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
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
using FullSerializer;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// Condition which checks if player character at a specific place.
    /// </summary>
    public class PcAt : ActionTemplate
    {
        Symbol placeSymbol;
        Symbol taskSymbol;
        int textId;
        bool textShown;
        // Place type parameters
        int p1;                     // Parameter 1
        int p2;                     // Parameter 2
        int p3;                     // Parameter 3

        public override string Pattern
        {
            // Notes:
            // Docs use form "pc at aPlace do aTask"
            // But observed quests actually seem to use "pc at aPlace set aTask"
            // Probably a change between writing of docs and Template v1.11.
            // Supporting both variants as quest authors are working from docs
            // Docs also missing "pc at aPlace set aTask saying nnnn"
            // DFU extension: also adding "pc at any <placeType>" so quests can check for any place of a certain type
            get { return @"pc at (?<aPlace>\w+) set (?<aTask>[a-zA-Z0-9_.]+) saying (?<id>\d+)|" +
                         @"pc at (?<aPlace>\w+) set (?<aTask>[a-zA-Z0-9_.]+)|" +
                         @"pc at (?<aPlace>\w+) do (?<aTask>[a-zA-Z0-9_.]+) saying (?<id>\d+)|" +
                         @"pc at (?<aPlace>\w+) do (?<aTask>[a-zA-Z0-9_.]+)|" +
                         @"pc at any (?<placeType>\w+) set (?<aTask>[a-zA-Z0-9_.]+) saying (?<id>\d+)|" +
                         @"pc at any (?<placeType>\w+) set (?<aTask>[a-zA-Z0-9_.]+)|" +
                         @"pc at any (?<placeType>\w+) do (?<aTask>[a-zA-Z0-9_.]+) saying (?<id>\d+)|" +
                         @"pc at any (?<placeType>\w+) do (?<aTask>[a-zA-Z0-9_.]+)";
            }
        }

        public PcAt(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new pcat
            PcAt pcat = new PcAt(parentQuest);

            // If this is a classic "pc at" action, use a declared location
            if (string.IsNullOrEmpty(match.Groups["placeType"].Value))
            {
                pcat.placeSymbol = new Symbol(match.Groups["aPlace"].Value);
            }
            else
            {
                string name = match.Groups["placeType"].Value;
                Table placesTable = QuestMachine.Instance.PlacesTable;
                if (placesTable.HasValue(name))
                {
                    // Store values
                    var p_1 = Place.CustomParseInt(placesTable.GetValue("p1", name));
                    if(p_1 != 0 && p_1 != 1)
                    {
                        throw new Exception("PcAt: This trigger condition can only be used with building types (p1=0) and dungeon types (p1=1) in Quests-Places table.");
                    }
                    pcat.p1 = p_1;
                    pcat.p2 = Place.CustomParseInt(placesTable.GetValue("p2", name));
                    pcat.p3 = Place.CustomParseInt(placesTable.GetValue("p3", name));
                }
                else
                {
                    throw new Exception(string.Format("PcAt: Could not find place type name in data table: '{0}'", name));
                }
            }

            pcat.taskSymbol = new Symbol(match.Groups["aTask"].Value);
            pcat.textId = Parser.ParseInt(match.Groups["id"].Value);

            return pcat;
        }

        /// <summary>
        /// Continuously checks where player is and sets target true/false based on site properties.
        /// </summary>
        public override void Update(Task caller)
        {
            bool result;

            if (placeSymbol != null)
            {
                // Get place resource
                Place place = ParentQuest.GetPlace(placeSymbol);
                if (place == null)
                    return;

                // Check if player at this place
                result = place.IsPlayerHere();
            }
            else
            {
                result = p1 == 1 ? Place.IsPlayerAtDungeonType(p2) : Place.IsPlayerAtBuildingType(p2, p3);
            }

            // Handle positive check
            if (result)
            {
                // "saying" popup
                // Only display this once or player can get a popup loop
                if (textId != 0 && !textShown)
                {
                    ParentQuest.ShowMessagePopup(textId);
                    textShown = true;
                }

                // Start target task
                ParentQuest.StartTask(taskSymbol);
            }
            else
            {
                // Clear target task
                ParentQuest.ClearTask(taskSymbol);
            }
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol placeSymbol;
            public Symbol taskSymbol;
            public int textId;
            public bool textShown;
            // Place type parameters
            public int p1;
            public int p2;
            public int p3;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.placeSymbol = placeSymbol;
            data.taskSymbol = taskSymbol;
            data.textId = textId;
            data.textShown = textShown;
            data.p1 = p1;
            data.p2 = p2;
            data.p3 = p3;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            placeSymbol = data.placeSymbol;
            taskSymbol = data.taskSymbol;
            textId = data.textId;
            textShown = data.textShown;
            p1 = data.p1;
            p2 = data.p2;
            p3 = data.p3;
        }

        #endregion
    }
}