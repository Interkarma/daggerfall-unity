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

using System;
using System.Text.RegularExpressions;
using FullSerializer;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.MagicAndEffects;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Queues a spell to be cast on a Foe resource.
    /// Can queue multiple spells at once or at different stages of quest.
    /// If the Foe has not been spawned: All spells currently in queue will be cast on Foe the moment they spawn.
    /// If the Foe has been spawned: The next spell(s) added to queue will be cast on spawned Foes on next quest tick.
    /// This allows quest author to cast spells on Foe both before and after spawn or at different stages of quest (e.g. after foe injured).
    /// Notes:
    ///  -As spells have durations recommend casting spells on "place foe" dungeon foes after being injured or spell will likely expire by the time player locates foe.
    ///  -For "create foe" foes spell can be queued at any time as spell is cast when foe is directly spawned near player.
    /// </summary>
    public class CastSpellOnFoe : ActionTemplate
    {
        SpellReference spell;
        Symbol foeSymbol;

        public override string Pattern
        {
            get
            {
                return @"cast (?<aSpell>[a-zA-Z0-9'_.-]+) spell on (?<aFoe>[a-zA-Z0-9_.-]+)|" +
                       @"cast (?<aCustomSpell>[a-zA-Z0-9_.-]+) custom spell on (?<aFoe>[a-zA-Z0-9_.-]+)";
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
            action.spell.CustomKey = match.Groups["aCustomSpell"].Value;
            action.foeSymbol = new Symbol(match.Groups["aFoe"].Value);

            // Get classic spell ID
            if (string.IsNullOrEmpty(action.spell.CustomKey))
            {
                Table spellsTable = QuestMachine.Instance.SpellsTable;
                if (spellsTable.HasValue(sourceSpellName))
                {
                    action.spell.ClassicID = int.Parse(spellsTable.GetValue("id", sourceSpellName));
                }
                else
                {
                    QuestMachine.LogFormat("CastSpellOnFoe could not resolve classic spell '{0}' in Quests-Spells data table", sourceSpellName);
                    SetComplete();
                }
            }

            return action;
        }

        public override void Update(Task caller)
        {
            // Get the Foe resource
            Foe foe = ParentQuest.GetFoe(foeSymbol);
            if (foe == null)
            {
                SetComplete();
                throw new Exception(string.Format("CastSpellOnFoe could not find Foe with symbol name {0}", Symbol.Name));
            }

            // Add spell to Foe resource queue
            foe.QueueSpell(spell);

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public SpellReference spell;
            public Symbol foeSymbol;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.spell = spell;
            data.foeSymbol = foeSymbol;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            spell = data.spell;
            foeSymbol = data.foeSymbol;
        }

        #endregion
    }
}