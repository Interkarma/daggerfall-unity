// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    DFIronman, Hazelnut
// 
// Notes:
// 

using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game.Questing;
using FullSerializer;
using System;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Changes whether or not a quest foe is attackable by other NPCs.
    /// </summary>
    public class ChangeFoeInfighting : ActionTemplate
    {
        Symbol npcSymbol;
        bool isAttackableByAI;

        public override string Pattern
        {
            get { return @"change foe (?<anNPC>[a-zA-Z0-9_.-]+) infighting (?<isAttackableByAI>[a-zA-Z]+)"; }
        }

        public ChangeFoeInfighting(Quest parentQuest)
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
            ChangeFoeInfighting action = new ChangeFoeInfighting(parentQuest);
            action.npcSymbol = new Symbol(match.Groups["anNPC"].Value);
            action.isAttackableByAI = Convert.ToBoolean(match.Groups["isAttackableByAI"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            base.Update(caller);

            // Get related Foe resource
            Foe foe = ParentQuest.GetFoe(npcSymbol);
            if (foe == null)
                 return;

            foreach (DaggerfallEnemy enemy in UnityEngine.Object.FindObjectsOfType<DaggerfallEnemy>())
            {
                if (enemy.QuestSpawn)
                {
                    QuestResourceBehaviour qrb = enemy.GetComponent<QuestResourceBehaviour>();
                    if (qrb && qrb.TargetSymbol == foe.Symbol)
                    {
                        qrb.IsAttackableByAI = isAttackableByAI;

                        SetComplete();
                    }
                }
            }

            return;
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol npcSymbol;
            public bool isAttackableByAI;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.npcSymbol = npcSymbol;
            data.isAttackableByAI = isAttackableByAI;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            npcSymbol = data.npcSymbol;
            isAttackableByAI = data.isAttackableByAI;
        }

        #endregion
    }
}