// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
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
using DaggerfallConnect;
using FullSerializer;

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

        public override string Pattern
        {
            // Notes:
            // Docs use form "pc at aPlace do aTask"
            // But observed quests actually seem to use "pc at aPlace set aTask"
            // Probably a change between writing of docs and Template v1.11.
            // Supporting both variants as quest authors are working from docs
            // Docs also missing "pc at aPlace set aTask saying nnnn"
            get { return @"pc at (?<aPlace>\w+) set (?<aTask>[a-zA-Z0-9_.]+) saying (?<id>\d+)|" +
                         @"pc at (?<aPlace>\w+) set (?<aTask>[a-zA-Z0-9_.]+)|" +
                         @"pc at (?<aPlace>\w+) do (?<aTask>[a-zA-Z0-9_.]+) saying (?<id>\d+)|" +
                         @"pc at (?<aPlace>\w+) do (?<aTask>[a-zA-Z0-9_.]+)"; }
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
            pcat.placeSymbol = new Symbol(match.Groups["aPlace"].Value);
            pcat.taskSymbol = new Symbol(match.Groups["aTask"].Value);
            pcat.textId = Parser.ParseInt(match.Groups["id"].Value);

            return pcat;
        }

        /// <summary>
        /// Continuously checks where player is and sets target true/false based on site properties.
        /// </summary>
        public override void Update(Task caller)
        {
            bool result = false;

            // Get place resource
            Place place = ParentQuest.GetPlace(placeSymbol);
            if (place == null)
                return;

            // Check if player at this place
            result = place.IsPlayerHere();

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
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.placeSymbol = placeSymbol;
            data.taskSymbol = taskSymbol;
            data.textId = textId;
            data.textShown = textShown;

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
        }

        #endregion
    }
}