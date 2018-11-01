// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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
    /// Condition that fires when player casts a specific spell.
    /// Classic only accepts standard versions of spell, not custom spells created by player.
    /// Daggerfall Unity makes no distinction between standard or custom spells and will instead match by effect.
    /// </summary>
    public class CastSpellDo : ActionTemplate
    {
        int spellID;
        Symbol taskSymbol;

        public override string Pattern
        {
            get
            {
                return @"cast (?<aSpell>[a-zA-Z0-9_.-]+) spell do (?<aTask>[a-zA-Z0-9_.-]+)";
            }
        }

        public CastSpellDo(Quest parentQuest)
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
            CastSpellDo action = new CastSpellDo(parentQuest);
            string sourceSpellName = match.Groups["aSpell"].Value;
            action.taskSymbol = new Symbol(match.Groups["aTask"].Value);

            // TODO: Attempt to get spellID from table using source name

            return action;
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public int spellID;
            public Symbol taskSymbol;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.spellID = spellID;
            data.taskSymbol = taskSymbol;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            spellID = data.spellID;
            taskSymbol = data.taskSymbol;
        }

        #endregion
    }
}