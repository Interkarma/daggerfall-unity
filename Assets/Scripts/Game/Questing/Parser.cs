// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Reads a quest source file and generates new quest object.
    /// Final step of parsing may be handed off to components as text at construction time.
    /// This allows parser to handle high-level structure while components take care of their own specific needs.
    /// </summary>
    public class Parser
    {
        #region Fields

        const string specialFieldToken = "--+";
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Parser()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Attempts to parse a text source file.
        /// </summary>
        /// <param name="source">Array of text lines from quest source.</param>
        public Quest Parse(string[] source)
        {
            Quest quest = new Quest();
            const StringComparison comparison = StringComparison.InvariantCultureIgnoreCase;

            string questName = string.Empty;
            bool inQRC = false;
            bool inQBN = false;
            List<string> qrcLines = new List<string>();
            List<string> qbnLines = new List<string>();

            // Start diagnostic timer
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            long startTime = stopwatch.ElapsedMilliseconds;

            // First pass gathers basic information and structure
            // Basic structure is:
            //   <Header>
            //   <QRC>
            //   <QBN>
            foreach(string line in source)
            {
                // Trim trailing white space from either end of source line data
                string text = line.Trim();

                // Handle special tag code lines
                // This will parse as comments to Template but have special meaning in Daggerfall Unity
                if (text.StartsWith(specialFieldToken, comparison))
                {
                    ReadSpecialField(quest, line);
                    continue;
                }

                // Skip other comment lines
                if (text.StartsWith("-", comparison))
                    continue;

                // Handle basic structure
                if (text.StartsWith("quest:", comparison))
                {
                    questName = GetFieldStringValue(text);
                }
                else if (text.StartsWith("qrc:", comparison))
                {
                    inQRC = true;
                    inQBN = false;
                    continue;
                }
                else if (text.StartsWith("qbn:", comparison))
                {
                    inQRC = false;
                    inQBN = true;
                    continue;
                }

                // Add full lines to QRC section
                if (inQRC)
                {
                    qrcLines.Add(line);
                }

                // Add full lines to QBN section
                if (inQBN)
                {
                    qbnLines.Add(line);
                }
            }

            // Validate name
            if (string.IsNullOrEmpty(questName))
            {
                throw new Exception("Parse() error: Quest has no name.");
            }

            // Validate QRC
            if (qrcLines.Count == 0)
            {
                throw new Exception("Parse() error: Quest has no QRC section.");
            }

            // Validate QBN
            if (qbnLines.Count == 0)
            {
                throw new Exception("Parse() error: Quest has no QBN section.");
            }

            // Parse QRC and QBN
            ParseQRC(quest, qrcLines);
            ParseQBN(quest, qbnLines);

            // End timer
            long totalTime = stopwatch.ElapsedMilliseconds - startTime;
            if (string.IsNullOrEmpty(quest.DisplayName))
            {
                Debug.Log(string.Format("Time to parse quest {0} was {1}ms.", questName, totalTime));
            }
            else
            {
                Debug.Log(string.Format("Time to parse quest {0} ({1}) was {2}ms.", questName, quest.DisplayName, totalTime));
            }

            return quest;
        }

        #endregion

        #region Private Methods

        void ParseQRC(Quest quest, List<string> lines)
        {
            const string parseIdError = "Could not parse text '{0}' to an int. Expected message ID value.";

            const string idCol = "id";
            Table staticMessagesTable = QuestMachine.Instance.StaticMessagesTable;

            for (int i = 0; i < lines.Count; i++)
            {
                // Skip empty lines while scanning for start of message block
                if (string.IsNullOrEmpty(lines[i].Trim()))
                    continue;

                // Check for start of message block
                // Only present in QRC section
                // Begins with field Message: (or fixed message type)
                string[] parts = SplitField(lines[i]);
                if (staticMessagesTable.HasValue(parts[0]))
                {
                    // Read ID of message
                    int messageID = 0;

                    if (parts[1].StartsWith("[") && parts[1].EndsWith("]"))
                    {
                        // Fixed message types use ID from table
                        messageID = staticMessagesTable.GetInt(idCol, parts[0]);
                        if (messageID == -1)
                            throw new Exception(string.Format(parseIdError, staticMessagesTable.GetInt(idCol, parts[0])));
                    }
                    else
                    {
                        // Other messages use ID from message block header
                        if (!int.TryParse(parts[1], out messageID))
                            throw new Exception(string.Format(parseIdError, parts[1]));
                    }

                    // Keep reading message lines until empty line is found, indicating end of block
                    List<string> messageLines = new List<string>();
                    while (true)
                    {
                        string text = lines[++i].TrimEnd('\r');
                        if (string.IsNullOrEmpty(text))
                            break;
                        else
                            messageLines.Add(text);
                    }

                    // Instantiate message
                    Message message = new Message(quest, messageID, messageLines.ToArray());

                    // Add message to collection
                    quest.AddMessage(messageID, message);
                }
            }
        }

        void ParseQBN(Quest quest, List<string> lines)
        {
            bool foundHeadlessTask = false;
            for (int i = 0; i < lines.Count; i++)
            {
                // Skip empty lines while scanning for next QBN item
                if (string.IsNullOrEmpty(lines[i].Trim()))
                    continue;

                // Simple way to identify certain lines
                // This is just to get started on some basics for now
                if (lines[i].StartsWith("clock", StringComparison.InvariantCultureIgnoreCase))
                {
                    Clock clock = new Clock(quest, lines[i]);
                    quest.AddResource(clock);
                }
                else if (lines[i].StartsWith("item", StringComparison.InvariantCultureIgnoreCase))
                {
                    Item item = new Item(quest, lines[i]);
                    quest.AddResource(item);
                }
                else if (lines[i].StartsWith("person", StringComparison.InvariantCultureIgnoreCase))
                {
                    // This is a person declaration
                }
                else if (lines[i].StartsWith("foe", StringComparison.InvariantCultureIgnoreCase))
                {
                    // This is an enemy declaration
                }
                else if (lines[i].StartsWith("place", StringComparison.InvariantCultureIgnoreCase))
                {
                    // This is a place declaration
                    Place place = new Place(quest, lines[i]);
                    quest.AddResource(place);
                }
                else if (lines[i].StartsWith("variable", StringComparison.InvariantCultureIgnoreCase))
                {
                    // This is a single-line variable declaration task
                    string[] variableLines = new string[1];
                    variableLines[0] = lines[i];
                    Task task = new Task(quest, variableLines);
                    quest.AddTask(task);
                }
                else if (lines[i].Contains("task:") ||
                    (lines[i].StartsWith("until", StringComparison.InvariantCultureIgnoreCase) && lines[i].Contains("performed:")))
                {
                    // This is a standard or repeating task declaration
                    List<string> taskLines = ReadBlock(lines, ref i);
                    Task task = new Task(quest, taskLines.ToArray());
                    quest.AddTask(task);
                }
                else if (IsGlobalReference(lines[i]))
                {
                    // This is a global variable reference
                }
                else if (foundHeadlessTask == false)
                {
                    // The first QBN line found that is not a resource declaration should be our headless entry point
                    // Currently only a single headless task is expected for startup task
                    // May be expanded later to allow multiple headless tasks
                    List<string> taskLines = ReadBlock(lines, ref i);
                    Task task = new Task(quest, taskLines.ToArray());
                    quest.AddTask(task);
                    foundHeadlessTask = true;
                }
                else
                {
                    // Something went wrong
                    throw new Exception(string.Format("Unknown line signature encounted '{0}'.", lines[i]));
                }
            }
        }

        void ReadSpecialField(Quest quest, string line)
        {
            // Read field
            line = line.Replace(specialFieldToken, "");
            string[] field = SplitField(line, 2);

            if (string.Compare(field[0], "DisplayName", true) == 0)
                quest.DisplayName = field[1];
        }

        #endregion

        #region Private Methods

        List<string> ReadBlock(List<string> linesIn, ref int currentLine)
        {
            List<string> linesOut = new List<string>();
            while (true)
            {
                // Add current line to lines out
                linesOut.Add(linesIn[currentLine].Trim('\t'));

                // End block if about to overflow lines
                if (currentLine + 1 >= linesIn.Count)
                    break;

                // Trim and look for pure white-space to end block
                string text = linesIn[++currentLine].TrimEnd('\r');
                if (string.IsNullOrEmpty(text))
                    break;
            }

            return linesOut;
        }

        bool IsGlobalReference(string line)
        {
            string[] parts = SplitLine(line);
            if (parts == null || parts.Length == 0)
                return false;

            if (QuestMachine.Instance.GlobalVarsTable.HasValue(parts[0]))
                return true;

            return false;
        }

        #endregion

        #region Static Helpers

        public static string[] SplitLine(string text, bool trim = true)
        {
            string[] parts = text.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

            if (trim)
            {
                TrimLines(ref parts);
            }

            return parts;
        }

        // Splits a field with format 'FieldName: Value" using : as separator
        // Be default expects two string values as result
        public static string[] SplitField(string text, int expectedCount = 2, bool trim = true)
        {
            const string error = "SplitField() encountered invalid number of results.";

            string[] parts = text.Split(':');
            if (parts.Length != expectedCount && expectedCount != -1)
                throw new Exception(error);

            if (trim)
            {
                TrimLines(ref parts);
            }

            return parts;
        }

        // Gets string value from text field with format 'FieldName: String'
        public static string GetFieldStringValue(string text)
        {
            string[] parts = SplitField(text);
            return parts[1].Trim();
        }

        // Gets int value from text field with format 'FieldName: Int'
        public static int GetFieldIntValue(string text)
        {
            string[] parts = SplitField(text);
            return ParseInt(parts[1].Trim());
        }

        // Just a wrapper for int.Parse with default of 0 for null or empty strings
        public static int ParseInt(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;
            else
                return int.Parse(text);
        }

        // Trims all lines in string array
        public static void TrimLines(ref string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Trim();
            }
        }

        // Gets inner symbol name between e.g. "_symbol_" or "=symbol_" becomes "symbol"
        // Does not care about context just wants the interior name
        // Does not trim inner characters - for example "_one_day_" will become "one_day"
        public static string GetInnerSymbolName(string symbol)
        {
            if (string.IsNullOrEmpty(symbol))
                return string.Empty;

            // Trim symbol wrappers from outside in
            string result;
            result = symbol.Trim('=');          // Outer =
            result = symbol.Trim('#');          // Outer # (custom, gets binding)
            result = symbol.Trim('_');          // Outer _

            return result;
        }

        #endregion
    }
}