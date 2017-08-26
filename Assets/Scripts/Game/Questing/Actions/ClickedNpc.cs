// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Handles player clicking on NPC.
    /// </summary>
    public class ClickedNpc : ActionTemplate
    {
        Symbol npcSymbol;

        public override string Pattern
        {
            get { return @"clicked npc (?<anNPC>[a-zA-Z0-9_.-]+)"; }
        }

        public ClickedNpc(Quest parentQuest)
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
            ClickedNpc action = new ClickedNpc(parentQuest);
            action.npcSymbol = new Symbol(match.Groups["anNPC"].Value);

            return action;
        }

        public override bool CheckTrigger(Task caller)
        {
            // Always return true once owning Task is triggered
            // Another action will need to rearm/unset this task if another click is required
            // This seems to fit how classic works based on current observation
            if (caller.IsTriggered)
                return true;

            // Get related Person resource
            Person person = ParentQuest.GetPerson(npcSymbol);
            if (person == null)
                return false;

            // Check player clicked flag
            if (person.HasPlayerClicked)
            {
                //person.RearmPlayerClick();
                return true;
            }

            return false;
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol npcSymbol;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.npcSymbol = npcSymbol;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            SaveData_v1 data = (SaveData_v1)dataIn;
            if (dataIn == null)
                return;

            npcSymbol = data.npcSymbol;
        }

        #endregion
    }
}