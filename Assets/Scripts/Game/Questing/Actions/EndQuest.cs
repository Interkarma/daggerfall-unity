// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    public class EndQuest : ActionTemplate
    {
        public override string Pattern
        {
            get { return "end quest"; }
        }

        public EndQuest(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            base.CreateNew(source, parentQuest);

            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            EndQuest action = new EndQuest(parentQuest);

            return action;
        }

        public override void Update(Task caller)
        {
            // Flag quest over so quest machine can remove it
            //Debug.LogFormat("Ending quest {0}", ParentQuest.UID);
            ParentQuest.QuestBreak = true;
            ParentQuest.EndQuest();
            SetComplete();
        }

        #region Serialization


        public override object GetSaveData()
        {
            return null;
        }

        public override void RestoreSaveData(object dataIn)
        {
        }

        #endregion
    }
}