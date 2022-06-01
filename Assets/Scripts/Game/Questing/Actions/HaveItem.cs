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

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Starts a task when player has a particular item resource in their inventory.
    /// This task continues to run and will start task when item present
    /// </summary>
    public class HaveItem : ActionTemplate
    {
        Symbol targetItem;
        Symbol targetTask;

        public override string Pattern
        {
            get { return @"have (?<targetItem>[a-zA-Z0-9_.-]+) set (?<targetTask>[a-zA-Z0-9_.-]+)"; }
        }

        public HaveItem(Quest parentQuest)
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
            HaveItem action = new HaveItem(parentQuest);
            action.targetItem = new Symbol(match.Groups["targetItem"].Value);
            action.targetTask = new Symbol(match.Groups["targetTask"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            // Attempt to get Item resource
            Item item = ParentQuest.GetItem(targetItem);
            if (item == null)
            {
                SetComplete();
                throw new Exception(string.Format("Could not find Item resource symbol {0}", targetItem));
            }

            // Start target task based on player carrying item
            if (GameManager.Instance.PlayerEntity.Items.Contains(item))
                ParentQuest.StartTask(targetTask);
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol targetItem;
            public Symbol targetTask;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.targetItem = targetItem;
            data.targetTask = targetTask;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            targetItem = data.targetItem;
            targetTask = data.targetTask;
        }

        #endregion
    }
}