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
    /// Makes all foes hostile, or clears (removes) them all.
    /// </summary>
    public class Enemies : ActionTemplate
    {
        bool clear = false;

        public override string Pattern
        {
            get { return @"enemies (?<action>makehostile|clear)"; }
        }

        public Enemies(Quest parentQuest)
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
            Enemies action = new Enemies(parentQuest);
            if (match.Groups["action"].Value == "clear")
                action.clear = true;

            return action;
        }

        public override void Update(Task caller)
        {
            if (clear)
                GameManager.Instance.ClearEnemies();
            else
                GameManager.Instance.MakeEnemiesHostile();
            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public bool clear;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.clear = clear;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            clear = data.clear;
        }

        #endregion
    }
}