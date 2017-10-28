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

using System.Text.RegularExpressions;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Incomplete. Just stubbing out action for now so quest will compile.
    /// </summary>
    public class MuteNpc : ActionTemplate
    {
        public override string Pattern
        {
            get { return @"mute npc"; }
        }

        public MuteNpc(Quest parentQuest)
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
            MuteNpc action = new MuteNpc(parentQuest);

            return action;
        }

        public override void Update(Task caller)
        {
            // TODO: Perform action changes

            SetComplete();
        }

        #region Serialization

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
            //SaveData_v1 data = (SaveData_v1)dataIn;
            //if (dataIn == null)
            //    return;
        }

        #endregion
    }
}