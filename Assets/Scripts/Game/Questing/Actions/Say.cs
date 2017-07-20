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
    /// <summary>
    /// Displays a prompt which user can click to dismiss.
    /// </summary>
    public class Say : ActionTemplate
    {
        int id;

        public override string Pattern
        {
            get { return @"say (?<id>\d+)"; }
        }

        public Say(Quest parentQuest)
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

            // Factory new say
            Say say = new Say(parentQuest);
            say.id = Parser.ParseInt(match.Groups["id"].Value);

            return say;
        }

        public override void Update(Task caller)
        {
            ParentQuest.ShowMessagePopup(id);
            SetComplete();
        }

        #region Seralization

        [fsObject("v1")]
        public struct SaveData_v1
        {
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            SaveData_v1 data = (SaveData_v1)dataIn;
            if (dataIn == null)
                return;
        }

        #endregion
    }
}