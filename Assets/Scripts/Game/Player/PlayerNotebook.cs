// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2021 Daggerfall Workshop
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
using FullSerializer;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using UnityEngine;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Questing;

namespace DaggerfallWorkshop.Game.Player
{
    /// <summary>
    /// Stores player notes and finished quests.
    /// </summary>
    public class PlayerNotebook
    {
        public const int MaxLineLenth = 70;
        private const string PrefixDateHeader = "D:";
        private const string PrefixQuestion = "Q:";
        private const string PrefixAnswer = "A:";
        const int MaxMessageCount = 50;

        readonly static TextFile.Token NothingToken = new TextFile.Token() {
            formatting = TextFile.Formatting.Nothing,
        };

        List<TextFile.Token[]> notes = new List<TextFile.Token[]>();

        List<TextFile.Token[]> finishedQuests = new List<TextFile.Token[]>();

        List<TextFile.Token[]> messages = new List<TextFile.Token[]>();
        int nextMessageIndex = 0;

        #region Notes

        public List<TextFile.Token[]> GetNotes()
        {
            return new List<TextFile.Token[]>(notes);
        }

        public TextFile.Token[] GetNote(int index)
        {
            if (index < notes.Count)
                return notes[index];
            else
                return null;
        }

        public void RemoveNote(int index)
        {
            notes.RemoveAt(index);
        }

        public void MoveNote(int srcIdx, int destIdx)
        {
            Debug.LogFormat("Moving note {0} to {1} ", srcIdx, destIdx);
            var item = notes[srcIdx];
            notes.RemoveAt(srcIdx);
            if (destIdx > srcIdx)
                destIdx--;
            notes.Insert(destIdx, item);
        }

        public void AddNote(string str, int index = -1)
        {
            if (!string.IsNullOrEmpty(str))
            {
                List<TextFile.Token> note = CreateNote();
                WrapLinesIntoNote(note, str, TextFile.Formatting.Text);
                if (index == -1)
                    notes.Add(note.ToArray());
                else
                    notes.Insert(index, note.ToArray());
            }
        }

        public void AddNote(List<TextFile.Token> texts)
        {
            if (texts != null && texts.Count > 0)
            {
                List<TextFile.Token> note = CreateNote();
                foreach (TextFile.Token token in texts)
                {
                    if (string.IsNullOrEmpty(token.text))
                        note.Add(TextFile.NewLineToken);
                    else
                        WrapLinesIntoNote(note, token.text, token.formatting);

                    if ((note.Count - 2) >= (DaggerfallQuestJournalWindow.maxLinesSmall * 2))
                    {
                        notes.Add(note.ToArray());
                        note = CreateNote();
                    }
                }
                notes.Add(note.ToArray());
            }
        }

        private static List<TextFile.Token> CreateNote()
        {
            List<TextFile.Token> note = new List<TextFile.Token>();
            note.Add(new TextFile.Token() {
                text = string.Format(TextManager.Instance.GetLocalizedText("noteHeader"),
                    DaggerfallUnity.Instance.WorldTime.Now.DateTimeString(), MacroHelper.CityName(null)),
                formatting = TextFile.Formatting.TextHighlight,
            });
            note.Add(NothingToken);
            return note;
        }

        private static void WrapLinesIntoNote(List<TextFile.Token> note, string str, TextFile.Formatting format)
        {
            while (str.Length > MaxLineLenth)
            {
                int pos = str.LastIndexOf(' ', MaxLineLenth);
                note.Add(new TextFile.Token() {
                    text = ' ' + str.Substring(0, pos),
                    formatting = format,
                });
                note.Add(NothingToken);
                str = str.Substring(pos + 1);
            }
            note.Add(new TextFile.Token() {
                text = ' ' + str,
                formatting = format,
            });
            note.Add(NothingToken);
        }

        public List<TextFile.Token[]> GetMessages()
        {
            List<TextFile.Token[]> result = new List<TextFile.Token[]>(messages.Count);
            for (int current = nextMessageIndex; current < messages.Count; current++)
            {
                result.Add(messages[current]);
            }
            for (int current = 0; current < nextMessageIndex; current++)
            {
                result.Add(messages[current]);
            }
            return result;
        }

        public void AddMessage(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                List<TextFile.Token> message = CreateMessage(str);
                if (messages.Count < MaxMessageCount)
                    messages.Add(message.ToArray());
                else
                {
                    messages[nextMessageIndex] = message.ToArray();
                    nextMessageIndex = (nextMessageIndex + 1) % MaxMessageCount;
                }
            }
        }

        private static List<TextFile.Token> CreateMessage(string text)
        {
            List<TextFile.Token> message = new List<TextFile.Token>
            {
                TextFile.CreateFormatToken(TextFile.Formatting.JustifyCenter),
                TextFile.CreateTextToken(text),
            };
            return message;
        }

        #endregion

        #region Finished Quests

        public List<TextFile.Token[]> GetFinishedQuests()
        {
            return new List<TextFile.Token[]>(finishedQuests);
        }

        public TextFile.Token[] GetFinishedQuest(int index)
        {
            if (index < finishedQuests.Count)
                return finishedQuests[index];
            else
                return null;
        }

        public void RemoveFinishedQuest(int index)
        {
            finishedQuests.RemoveAt(index);
        }

        public void MoveFinishedQuest(int srcIdx, int destIdx)
        {
            Debug.LogFormat("Moving FinishedQuest {0} to {1} ", srcIdx, destIdx);
            var item = finishedQuests[srcIdx];
            finishedQuests.RemoveAt(srcIdx);
            if (destIdx > srcIdx)
                destIdx--;
            finishedQuests.Insert(destIdx, item);
        }

        public void AddFinishedQuest(TextFile.Token[] message)
        {
            if (message != null && message.Length > 0)
                finishedQuests.Add(message);
        }

        public void AddFinishedQuest(List<Message> messages)
        {
            if (messages != null && messages.Count > 0)
            {
                Quest quest = messages[0].ParentQuest;
                string questName = string.IsNullOrEmpty(quest.DisplayName) ? TextManager.Instance.GetLocalizedText("quest") : quest.DisplayName;
                List<TextFile.Token> entry = CreateFinishedQuest(questName, quest.QuestSuccess);
                foreach (Message msg in messages)
                {
                    foreach (TextFile.Token token in msg.GetTextTokens())
                        entry.Add(token);
                    entry.Add(TextFile.NewLineToken);

                    if ((entry.Count - 2) >= (DaggerfallQuestJournalWindow.maxLinesQuests * 2))
                    {
                        finishedQuests.Add(entry.ToArray());
                        AddFinishedQuest(msg.GetTextTokens());
                        entry.Clear();
                    }
                }
                finishedQuests.Add(entry.ToArray());
            }
        }

        private static List<TextFile.Token> CreateFinishedQuest(string questName, bool success)
        {
            string status = TextManager.Instance.GetLocalizedText(success ? "completedQuest" : "endedQuest");
            List<TextFile.Token> entry = new List<TextFile.Token>();
            entry.Add(new TextFile.Token() {
                text = string.Format(TextManager.Instance.GetLocalizedText("finishQuestHeader"),
                    questName, status, DaggerfallUnity.Instance.WorldTime.Now.MidDateTimeString()),
                formatting = TextFile.Formatting.TextHighlight,
            });
            entry.Add(NothingToken);
            return entry;
        }

        #endregion

        #region Save, Load & Clear

        public void Clear()
        {
            notes.Clear();
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
                bool lBreak = false;
                foreach (TextFile.Token token in entry)
                {
                    if (token.formatting == TextFile.Formatting.Text)
                        lines.Add(token.text);
                    else if (token.formatting == TextFile.Formatting.TextHighlight)
                        lines.Add(PrefixDateHeader + token.text);
                    else if (token.formatting == TextFile.Formatting.TextQuestion)
                        lines.Add(PrefixQuestion + token.text);
                    else if (token.formatting == TextFile.Formatting.TextAnswer)
                        lines.Add(PrefixAnswer + token.text);
                    else if (lBreak)
                        lines.Add("");
                    else
                    {   // One newline, if find another its a line break.
                        lBreak = true;
                        continue;
                    }
                    lBreak = false;
                }
                entries.Add(lines);
            }
            return entries;
        }

        public void RestoreNotebookData(NotebookData_v1 notebookData)
        {
            notes = new List<TextFile.Token[]>();
            ConvertData(notebookData.notebookEntries, notes, true);

            finishedQuests = new List<TextFile.Token[]>();
            ConvertData(notebookData.finishedQuestEntries, finishedQuests, false);
        }

        private void ConvertData(List<List<string>> listData, List<TextFile.Token[]> list, bool notes)
        {
            foreach (List<string> entry in listData)
            {
                List<TextFile.Token> lines = new List<TextFile.Token>(entry.Count * 2);
                foreach (string line in entry)
                {
                    if (line.StartsWith(PrefixDateHeader))
                    {
                        lines.Add(new TextFile.Token() {
                            text = line.Substring(PrefixDateHeader.Length),
                            formatting = TextFile.Formatting.TextHighlight
                        });
                    }
                    else if (line.StartsWith(PrefixQuestion))
                    {
                        lines.Add(new TextFile.Token() {
                            text = line.Substring(PrefixQuestion.Length),
                            formatting = TextFile.Formatting.TextQuestion
                        });
                    }
                    else if (line.StartsWith(PrefixAnswer))
                    {
                        lines.Add(new TextFile.Token() {
                            text = line.Substring(PrefixAnswer.Length),
                            formatting = TextFile.Formatting.TextAnswer
                        });
                    }
                    else if (!string.IsNullOrEmpty(line))
                    {
                        lines.Add(new TextFile.Token() {
                            text = line,
                            formatting = TextFile.Formatting.Text
                        });
                    }
                    lines.Add(!string.IsNullOrEmpty(line) ? NothingToken : TextFile.NewLineToken);
                }
                list.Add(lines.ToArray());
            }
        }

        [fsObject("v1")]
        public class NotebookData_v1
        {
            public List<List<string>> notebookEntries;
            public List<List<string>> finishedQuestEntries;
        }

        #endregion
    }
}
