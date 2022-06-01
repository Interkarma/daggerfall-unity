// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    
// 

using System.Collections.Generic;
using System.Text.RegularExpressions;
using DaggerfallConnect.Arena2;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// Adds a text entry to the player journal as a note.
    /// </summary>
    public class JournalNote : ActionTemplate
    {
        int id;

        public override string Pattern
        {
            get { return @"journal note (?<id>\d+)"; }
        }

        public JournalNote(Quest parentQuest)
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

            // Factory new note
            JournalNote note = new JournalNote(parentQuest);
            note.id = Parser.ParseInt(match.Groups["id"].Value);
            return note;
        }

        public override void Update(Task caller)
        {
            Message message = ParentQuest.GetMessage(id);

            GameManager.Instance.PlayerEntity.Notebook.AddNote(new List<TextFile.Token>(message.GetTextTokens()));

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