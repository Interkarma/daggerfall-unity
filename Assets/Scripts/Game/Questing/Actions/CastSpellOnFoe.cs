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

using System.Text.RegularExpressions;
using FullSerializer;
using DaggerfallWorkshop.Game.MagicAndEffects;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Queues a spell to be cast on a Foe resource.
    /// Can queue multiple spells at once or at different stages of quest.
    /// If the Foe has not been spawned: All spells currently in queue will be cast on Foe the moment they spawn.
    /// If the Foe has been spawned: The next spell(s) added to queue will be cast on spawned Foes on next quest tick.
    /// This allows quest author to cast spells on Foe both before and after spawn or at different stages of quest (e.g. after foe injured).
    /// </summary>
    public class CastSpellOnFoe : ActionTemplate
    {
        int spellID = -1;
        Symbol foeSymbol;

        public override string Pattern
        {
            get
            {
                return @"cast (?<aSpell>[a-zA-Z0-9_.-]+) spell on (?<aFoe>[a-zA-Z0-9_.-]+)";
            }
        }

        public CastSpellOnFoe(Quest parentQuest)
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
            CastSpellOnFoe action = new CastSpellOnFoe(parentQuest);
            string sourceSpellName = match.Groups["aSpell"].Value;
            action.foeSymbol = new Symbol(match.Groups["aFoe"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            // TODO: Add spell to Foe resource queue

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public int spellID;
            public Symbol foeSymbol;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.spellID = spellID;
            data.foeSymbol = foeSymbol;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            spellID = data.spellID;
            foeSymbol = data.foeSymbol;
        }

        #endregion
    }
}