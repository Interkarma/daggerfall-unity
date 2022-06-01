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

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// NPC will no longer respond to mouse clicks once muted.
    /// </summary>
    public class MuteNpc : ActionTemplate
    {
        Symbol npcSymbol;

        public override string Pattern
        {
            get
            {
                return @"mute npc (?<anNPC>[a-zA-Z0-9_.-]+)";
            }
        }

        public MuteNpc(Quest parentQuest)
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
            MuteNpc action = new MuteNpc(parentQuest);
            action.npcSymbol = new Symbol(match.Groups["anNPC"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            // Get related Person resource
            Person person = ParentQuest.GetPerson(npcSymbol);
            if (person == null)
            {
                SetComplete();
                return;
            }

            // Perform action changes
            person.IsMuted = true;

            SetComplete();
        }

        public override void RearmAction()
        {
            base.RearmAction();

            // Get related Person resource
            Person person = ParentQuest.GetPerson(npcSymbol);
            if (person == null)
            {
                SetComplete();
                return;
            }

            // Perform action changes
            person.IsMuted = false;
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