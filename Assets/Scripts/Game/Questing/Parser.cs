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

        // Constants
        const string idCol = "id";
        const string nameCol = "name";
        const string globalVarsFilename = "Quests-GlobalVars";
        const string staticMessagesFilename = "Quests-StaticMessages";

        // Data tables
        Table globalVars;
        Table messageTypes;

        // Quest object collections
        Dictionary<int, Message> messages = new Dictionary<int, Message>();
        Dictionary<string, Clock> clocks = new Dictionary<string, Clock>();

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Parser()
        {
            globalVars = new Table(QuestMachine.Instance.GetTableSourceText(globalVarsFilename));
            messageTypes = new Table(QuestMachine.Instance.GetTableSourceText(staticMessagesFilename));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Attempts to parse a text source file.
        /// </summary>
        /// <param name="source">Array of text lines from quest source.</param>
        public void Parse(string[] source)
        {
            const StringComparison comparison = StringComparison.InvariantCultureIgnoreCase;

            string questName = string.Empty;
            bool inQRC = false;
            bool inQBN = false;
            List<string> qrcLines = new List<string>();
            List<string> qbnLines = new List<string>();

            // Clear existing quest data
            messages = new Dictionary<int, Message>();

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

                // Skip comment lines
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
            ParseQRC(qrcLines);
            ParseQBN(qbnLines);

            // End timer
            long totalTime = stopwatch.ElapsedMilliseconds - startTime;
            Debug.Log(string.Format("Time to parse quest {0} was {1}ms.", questName, totalTime));
        }

        #endregion

        #region Private Methods

        void ParseQRC(List<string> lines)
        {
            const string parseIdError = "Could not parse text '{0}' to an int. Expected message ID value.";

            for (int i = 0; i < lines.Count; i++)
            {
                // Skip empty lines while scanning for start of message block
                if (string.IsNullOrEmpty(lines[i].Trim()))
                    continue;

                // Check for start of message block
                // Only present in QRC section
                // Begins with field Message: (or fixed message type)
                string[] parts = SplitField(lines[i]);
                if (messageTypes.HasValue(parts[0]))
                {
                    // Read ID of message
                    int messageID = 0;
                    if (parts[1].StartsWith("[") && parts[1].EndsWith("]"))
                    {
                        // Fixed message types use ID from table
                        messageID = messageTypes.GetInt(idCol, parts[0]);
                        if (messageID == -1)
                            throw new Exception(string.Format(parseIdError, messageTypes.GetInt(idCol, parts[0])));
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
                    Message message = new Message(messageID, messageLines.ToArray());

                    // Add message to collection
                    messages.Add(messageID, message);
                }
            }
        }

        void ParseQBN(List<string> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                // Skip empty lines while scanning for next QBN item
                if (string.IsNullOrEmpty(lines[i].Trim()))
                    continue;

                // Simple way to identify certain lines
                // This is just to get started on some basics for now
                if (lines[i].StartsWith("clock", StringComparison.InvariantCultureIgnoreCase))
                {
                    Clock clock = new Clock(lines[i]);
                    //clocks.Add()
                }
            }
        }

        #endregion

        #region Helpers

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

        #endregion
    }
}