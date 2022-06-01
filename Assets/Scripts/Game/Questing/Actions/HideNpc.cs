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
    /// Hide NPC from world temporarily.
    /// </summary>
    public class HideNpc : ActionTemplate
    {
        Symbol npcSymbol;

        public override string Pattern
        {
            get { return @"hide npc (?<anNPC>[a-zA-Z0-9_.-]+)|hide (?<anNPC>[a-zA-Z0-9_.-]+)"; }
        }

        public HideNpc(Quest parentQuest)
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
            HideNpc action = new HideNpc(parentQuest);
            action.npcSymbol = new Symbol(match.Groups["anNPC"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            base.Update(caller);

            // Get related Person resource
            Person person = ParentQuest.GetPerson(npcSymbol);
            if (person == null)
                return;

            // Hide this Person
            person.IsHidden = true;

            SetComplete();
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
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            npcSymbol = data.npcSymbol;
        }

        #endregion
    }
}