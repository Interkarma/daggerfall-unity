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
using DaggerfallWorkshop.Utility;
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
            get { return @"say (?<id>\d+)|say (?<idName>\w+)"; }
        }

        public Say(Quest parentQuest)
            : base(parentQuest)
        {
            allowRearm = false;
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new say
            Say say = new Say(parentQuest);
            say.id = Parser.ParseInt(match.Groups["id"].Value);

            // Resolve static message back to ID
            string idName = match.Groups["idName"].Value;
            if (say.id == 0 && !string.IsNullOrEmpty(idName))
            {
                Table table = QuestMachine.Instance.StaticMessagesTable;
                say.id = Parser.ParseInt(table.GetValue("id", idName));
            }

            return say;
        }

        public override void Update(Task caller)
        {
            ParentQuest.ShowMessagePopup(id, true);
            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public int id;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.id = id;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            id = data.id;
        }

        #endregion
    }
}