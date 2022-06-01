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
    /// NPC will be be soft destroyed (permanently removed from world but othwerwise available for macro resolution).
    /// This is different to classic that will return BLANK once NPC is destroyed (probably beause resource is hard deleted).
    /// If there are any emulation issue with soft destruction then will change to hard destruction instead.
    /// </summary>
    public class DestroyNpc : ActionTemplate
    {
        Symbol npcSymbol;

        public override string Pattern
        {
            get
            {
                return @"destroy npc (?<anNPC>[a-zA-Z0-9_.-]+)|destroy (?<anNPC>[a-zA-Z0-9_.-]+)";
            }
        }

        public DestroyNpc(Quest parentQuest)
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
            DestroyNpc action = new DestroyNpc(parentQuest);
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
            person.DestroyNPC();

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