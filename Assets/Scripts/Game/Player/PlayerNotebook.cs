// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    
// 
// Notes:
//

using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Player
{
    /// <summary>
    /// Stores player notes and finished quests.
    /// </summary>
    public class PlayerNotebook
    {
        List<TextFile.Token[]> notes = new List<TextFile.Token[]>();

        List<TextFile.Token[]> finishedQuests = new List<TextFile.Token[]>();

        readonly static TextFile.Token NothingToken = new TextFile.Token() {
            formatting = TextFile.Formatting.Nothing,
        };


        public List<TextFile.Token[]> GetNotes()
        {
            return notes;
        }

        public void AddNote(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                List<string> lines = new List<string>();
                while (str.Length > 56)
                {
                    int pos = str.LastIndexOf(' ', 56);
                    lines.Add(' ' + str.Substring(0, pos));
                    str = str.Substring(pos+1);
                }
                lines.Add(' ' + str);

                TextFile.Token[] note = new TextFile.Token[(lines.Count*2)+2];
                note[0] = new TextFile.Token() {
                    text = DaggerfallUnity.Instance.WorldTime.Now.DateString() + ':',
                    formatting = TextFile.Formatting.Text,
                };
                note[1] = NothingToken;
                int i = 2;
                foreach (string line in lines)
                {
                    note[i++] = new TextFile.Token() {
                        text = line,
                        formatting = TextFile.Formatting.Text,
                    };
                    note[i++] = NothingToken;
                }
                notes.Add(note);
            }
        }

        public List<TextFile.Token[]> GetFinishedQuests()
        {
            return finishedQuests;
        }

        public void AddFinishedQuestMessage(TextFile.Token[] message)
        {
            if (message != null && message.Length > 0)
                finishedQuests.Add(message);
        }

        public void Clear()
        {
            finishedQuests.Clear();
        }


        public NotebookData_v1 GetNotebookSaveData()
        {
            NotebookData_v1 data = new NotebookData_v1();

            List<List<string>> entries = ConvertList(notes);
            data.notebookEntries = entries;

            entries = ConvertList(finishedQuests);
            data.finishedQuestEntries = entries;

            return data;
        }

        private List<List<string>> ConvertList(List<TextFile.Token[]> list)
        {
            List<List<string>> entries = new List<List<string>>();
            foreach (TextFile.Token[] entry in list)
            {
                List<string> lines = new List<string>();
                foreach (TextFile.Token token in entry)
                {
                    if (token.formatting == TextFile.Formatting.Text)
                        lines.Add(token.text);
                }
                entries.Add(lines);
            }
            return entries;
        }

        public void RestoreNotebookData(NotebookData_v1 notebookData)
        {
            notes = new List<TextFile.Token[]>();
            ConvertData(notebookData.notebookEntries, notes);

            finishedQuests = new List<TextFile.Token[]>();
            ConvertData(notebookData.finishedQuestEntries, finishedQuests);
        }

        private void ConvertData(List<List<string>> listData, List<TextFile.Token[]> list)
        {
            foreach (List<string> entry in listData)
            {
                int i = 0;
                TextFile.Token[] lines = new TextFile.Token[entry.Count * 2];
                foreach (string line in entry)
                {
                    lines[i++] = new TextFile.Token() {
                        text = line,
                        formatting = TextFile.Formatting.Text
                    };
                    lines[i++] = NothingToken;
                }
                list.Add(lines);
            }
        }

        [fsObject("v1")]
        public class NotebookData_v1
        {
            public List<List<string>> notebookEntries;
            public List<List<string>> finishedQuestEntries;
        }

    }
}