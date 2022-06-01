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
using DaggerfallWorkshop.Game.MagicAndEffects;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Executes target task when player readies a spell containing a single specific effect.
    /// Matches effect by key, so should support custom effects from mods.
    /// </summary>
    public class CastEffectDo : ActionTemplate
    {
        string effectKey;
        Symbol taskSymbol;

        EntityEffectBundle lastReadySpell;

        public override string Pattern
        {
            get
            {
                return @"cast (?<effectKey>[a-zA-Z0-9_.-]+) effect do (?<aTask>[a-zA-Z0-9_.-]+)";
            }
        }

        public CastEffectDo(Quest parentQuest)
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
            CastEffectDo action = new CastEffectDo(parentQuest);
            action.effectKey = match.Groups["effectKey"].Value;
            action.taskSymbol = new Symbol(match.Groups["aTask"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            // Validate
            if (string.IsNullOrEmpty(effectKey) || taskSymbol == null || lastReadySpell == null)
            {
                lastReadySpell = null;
                return;
            }

            // Compare readied spell for target effect
            for (int i = 0; i < lastReadySpell.Settings.Effects.Length; i++)
            {
                if (lastReadySpell.Settings.Effects[i].Key == effectKey)
                {
                    ParentQuest.StartTask(taskSymbol);
                    SetComplete();
                    break;
                }
            }
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
            public string effectKey;
            public Symbol taskSymbol;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.effectKey = effectKey;
            data.taskSymbol = taskSymbol;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            effectKey = data.effectKey;
            taskSymbol = data.taskSymbol;
        }

        #endregion
    }
}