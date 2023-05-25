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
    /// Converts a quest item into a permanent item.
    /// </summary>
    public class MakePermanent : ActionTemplate
    {
        Symbol target;

        public override string Pattern
        {
            get { return @"make (?<target>[a-zA-Z0-9_.-]+) permanent"; }
        }

        public MakePermanent(Quest parentQuest)
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
            MakePermanent action = new MakePermanent(parentQuest);
            action.target = new Symbol(match.Groups["target"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            // Attempt to get Item resource
            Item item = ParentQuest.GetItem(target);
            if (item == null)
            {
                SetComplete();
                throw new Exception(string.Format("Could not find Item resource symbol {0}", target));
            }

            // Convert to permanent
            item.MakePermanent();

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol target;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.target = target;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            target = data.target;
        }

        #endregion
    }
}