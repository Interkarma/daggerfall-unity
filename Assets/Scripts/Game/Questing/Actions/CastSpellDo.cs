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
using DaggerfallConnect.Save;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.MagicAndEffects;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Executes target task when player readies a spell containing specific effects.
    /// Classic only accepts standard versions of spell, not custom spells created by player.
    /// Daggerfall Unity makes no distinction between standard or custom spells and will instead match by effects.
    /// </summary>
    public class CastSpellDo : ActionTemplate
    {
        int spellID = -1;
        SpellRecord.EffectRecordData[] classicEffects;
        Symbol taskSymbol;

        EntityEffectBundle lastReadySpell;

        public override string Pattern
        {
            get
            {
                return @"cast (?<aSpell>[a-zA-Z0-9'_.-]+) spell do (?<aTask>[a-zA-Z0-9_.-]+)";
            }
        }

        public CastSpellDo(Quest parentQuest)
            : base(parentQuest)
        {
            GameManager.Instance.PlayerEffectManager.OnNewReadySpell += PlayerEffectManager_OnNewReadySpell;
            GameManager.Instance.PlayerEffectManager.OnCastReadySpell += PlayerEffectManager_OnCastReadySpell;
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

            // Cache classic effects to match
            Table spellsTable = QuestMachine.Instance.SpellsTable;
            if (spellsTable.HasValue(sourceSpellName))
            {
                action.spellID = int.Parse(spellsTable.GetValue("id", sourceSpellName));
                SpellRecord.SpellRecordData spellRecord;
                if (GameManager.Instance.EntityEffectBroker.GetClassicSpellRecord(action.spellID, out spellRecord))
                {
                    action.classicEffects = spellRecord.effects;
                }
                else
                {
                    QuestMachine.LogFormat("CastSpellDo could not find spell matching spellID '{0}' from spell '{1}'", spellID, sourceSpellName);
                    SetComplete();
                }
            }
            else
            {
                QuestMachine.LogFormat("CastSpellDo could not resolve spell '{0}' in Quests-Spells data table", sourceSpellName);
                SetComplete();
            }

            return action;
        }

        public override void Update(Task caller)
        {
            // Validate
            if (spellID == -1 || classicEffects == null || classicEffects.Length == 0 || taskSymbol == null || lastReadySpell == null)
            {
                lastReadySpell = null;
                return;
            }

            // Compare readied effect properties to spell record
            int foundEffects = 0;
            for (int i = 0; i < classicEffects.Length; i++)
            {
                // Effect slot must be populated
                if (classicEffects[i].type == -1)
                    continue;

                // Bundle must have contain native effects matching this classic effect
                foundEffects++;
                if (!lastReadySpell.HasMatchForClassicEffect(classicEffects[i]))
                {
                    lastReadySpell = null;
                    return;
                }
            }

            // Do nothing if no effects found in spell
            if (foundEffects == 0)
            {
                SetComplete();
                return;
            }

            // Only reached here if action is running and matching spell is cast
            ParentQuest.StartTask(taskSymbol);
            SetComplete();
        }

        public override void SetComplete()
        {
            base.SetComplete();
            GameManager.Instance.PlayerEffectManager.OnNewReadySpell -= PlayerEffectManager_OnNewReadySpell;
            GameManager.Instance.PlayerEffectManager.OnCastReadySpell -= PlayerEffectManager_OnCastReadySpell;
        }

        #region Event Handlers

        private void PlayerEffectManager_OnNewReadySpell(EntityEffectBundle spell)
        {
            // Store last ready spell to evaluate on next tick
            lastReadySpell = spell;
        }

        private void PlayerEffectManager_OnCastReadySpell(EntityEffectBundle spell)
        {
            // Clear last ready spell so player can't queue it up before entering location
            lastReadySpell = null;
        }

        #endregion

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public int spellID;
            public SpellRecord.EffectRecordData[] classicEffects;
            public Symbol taskSymbol;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.spellID = spellID;
            data.classicEffects = classicEffects;
            data.taskSymbol = taskSymbol;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            spellID = data.spellID;
            classicEffects = data.classicEffects;
            taskSymbol = data.taskSymbol;
        }

        #endregion
    }
}