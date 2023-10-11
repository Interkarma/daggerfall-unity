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

using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect.Arena2;

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

        const StringComparison comparison = StringComparison.InvariantCultureIgnoreCase;

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
        /// <param name="factionId">Faction id of quest giver for guilds.</param>
        /// <param name="partialParse">If true the QRC and QBN sections will not be parsed.</param>
        public Quest Parse(string[] source, int factionId, bool partialParse = false)
        {
            Quest quest = new Quest();
            quest.FactionId = factionId;
            string questName = string.Empty;
            string displayName = string.Empty;
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

                // Handle basic structure
                if (text.StartsWith("quest:", comparison))
                {
                    questName = GetFieldStringValue(text);
                    quest.QuestName = questName;
                }
                else if (text.StartsWith("displayname:", comparison))
                {
                    displayName = GetFieldStringValue(text);
                    quest.DisplayName = displayName;
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

                // Add full lines to QRC or QBN sections
                if (inQRC)
                {
                    qrcLines.Add(line);
                }
                else if (inQBN)
                {
                    // Always skip comment lines in QBN
                    if (text.StartsWith("-", comparison))
                        continue;

                    qbnLines.Add(line);
                }
                else
                {
                    // Always skip comment lines when in neither QBN or QRC
                    if (text.StartsWith("-", comparison))
                        continue;
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

            // Parse QRC and QBN, unless partial parse requested
            if (!partialParse)
            {
                ParseQRC(quest, qrcLines);
                ParseQBN(quest, qbnLines);
            }

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

        /// <summary>
        /// Special cut-down parser to read localized quest source containing only QRC text part.
        /// Only reads lines related to quest display name and messages.
        /// Does not tokenize message text. Messages are returned in a dictionary format suitable to store in string tables.
        /// Keeping this process separate from standard QRC parser to avoid introducing additional complexity or regressions.
        /// </summary>
        /// <param name="lines">List of text lines from localized quest file.</param>
        /// <param name="displayName">Localized display name out.</param>
        /// <param name="messages">Localized message dictionary out.</param>
        /// <returns>True if successful.</returns>
        public bool ParseLocalized(List<string> lines, out string displayName, out Dictionary<int, string> messages)
        {
            const string parseIdError = "Could not parse localized quest message ID '{0}' to an int. Expected message ID value.";

            displayName = string.Empty;
            messages = new Dictionary<int, string>();

            // Must have a valid lines array
            if (lines == null || lines.Count == 0)
            {
                Debug.LogErrorFormat("ParseLocalized() lines input is null or empty.");
                return false;
            }

            bool inQRC = false;
            bool inQBN = false;
            const string idCol = "id";
            Table staticMessagesTable = QuestMachine.Instance.StaticMessagesTable;
            for (int i = 0; i < lines.Count; i++)
            {
                // Trim white space from either end of source line data
                string text = lines[i].Trim();

                // Skip empty lines and comments
                if (string.IsNullOrEmpty(text) || text.StartsWith("-", comparison))
                    continue;

                // Handle expected header values
                if (text.StartsWith("quest:", comparison)) // Accepted but ignored
                    continue;
                else if (text.StartsWith("displayname:", comparison))
                {
                    displayName = GetFieldStringValue(text);
                    continue;
                }
                else if (text.StartsWith("qrc:", comparison))
                    inQRC = true;
                else if (text.StartsWith("qbn:", comparison))
                    inQBN = true;

                // Don't try to read messages until QRC: section starts
                // Everything else that follows in file must be message text.
                if (!inQRC)
                    continue;

                // Ignore everything after QBN: section
                // This really shouldn't be in localized quest sources at all, but can simply ignore rather than breaking execution
                // It might appear if translator just copies and renames core quest sources for some reason
                if (inQBN)
                    continue;

                // Check for start of message block
                // Begins with field Message: (or fixed message type)
                // Ignores any lines that cannot be split
                string[] parts = SplitField(lines[i]);
                if (parts == null || parts.Length == 0)
                    continue;

                // Read message lines
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
                    string messageLines = string.Empty;
                    while (true)
                    {
                        // Check for end of lines
                        // This handles a case where final message block isn't terminated an empty line causing an overflow
                        if (i + 1 >= lines.Count)
                            break;

                        // Read line
                        string textLine = lines[++i].TrimEnd('\r');
                        if (string.IsNullOrEmpty(textLine))
                        {
                            // Sometimes quest author will forget single space in front of an empty message line
                            // Peek ahead to see if next line is really a new message header
                            // Otherwise just treat this as a line break in message (add a single ' ' character)
                            if (!PeekMessageEnd(lines, i))
                            {
                                messageLines += " " + "\n";
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            messageLines += textLine + "\n";
                        }
                    }

                    // Trim trailing newline
                    messageLines = messageLines.TrimEnd('\n');

                    // Store message ID and lines to output dictionary
                    messages.Add(messageID, messageLines);
                }
                else
                {
                    continue;
                }
            }

            return true;
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
                // Skip comment lines while scanning for start of message block
                if (lines[i].StartsWith("-", comparison))
                    continue;

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
                        // Check for end of lines
                        // This handles a case where final message block isn't terminated an empty line causing an overflow
                        if (i + 1 >= lines.Count)
                            break;

                        // Read line
                        string text = lines[++i].TrimEnd('\r');
                        if (string.IsNullOrEmpty(text))
                        {
                            // Sometimes quest author will forget single space in front of an empty message line
                            // Peek ahead to see if next line is really a new message header
                            // Otherwise just treat this as a line break in message (add a single ' ' character)
                            if (!PeekMessageEnd(lines, i))
                            {
                                messageLines.Add(" ");
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            messageLines.Add(text);
                        }
                    }

                    // Instantiate message
                    Message message = new Message(quest, messageID, messageLines.ToArray());

                    // Add message to collection
                    quest.AddMessage(messageID, message);
                }
                else
                {
                    throw new Exception(string.Format("Could not parse message block near '{0}'. Check message header syntax, spelling, and casing are all correct.", lines[i]));
                }
            }
        }

        // Try to decide if message block is actually at an end
        // This will occur if next line matches one of the following:
        // - End of stream
        // - A second empty line
        // - A comment line after an empty line
        // - Start of a new message block tab or QBN: tag
        // Returns true if this appears to be end of message
        bool PeekMessageEnd(List<string> lines, int line)
        {
            if (line + 1 >= lines.Count)
                return true;

            string nextLine = lines[line + 1];
            if (nextLine.Contains(":") || nextLine.StartsWith("-") || string.IsNullOrEmpty(nextLine.Trim()))
                return true;

            return false;
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
                    Person person = new Person(quest, lines[i]);
                    quest.AddResource(person);
                }
                else if (lines[i].StartsWith("foe", StringComparison.InvariantCultureIgnoreCase))
                {
                    // This is an enemy declaration
                    Foe foe = new Foe(quest, lines[i]);
                    quest.AddResource(foe);
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
                    // Check if following line is another task or a line break
                    bool hasBody = false;
                    if (i + 1 != lines.Count)
                        hasBody = !IsNewTaskOrLineBreak(lines[i + 1]);

                    // This is a global variable link task
                    Task task;
                    int globalVar = GetGlobalReference(lines[i]);
                    if (hasBody)
                    {
                        List<string> taskLines = ReadBlock(lines, ref i);
                        task = new Task(quest, taskLines.ToArray(), globalVar);
                    }
                    else
                    {
                        string[] variableLines = new string[1];
                        variableLines[0] = lines[i];
                        task = new Task(quest, variableLines, globalVar);
                    }
                    quest.AddTask(task);
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

        bool IsNewTaskOrLineBreak(string line)
        {
            // Check is this line is a new task or a line break
            if (line.StartsWith("variable", StringComparison.InvariantCultureIgnoreCase) ||
                line.Contains("task:") ||
                line.StartsWith("until", StringComparison.InvariantCultureIgnoreCase) && line.Contains("performed:") ||
                IsGlobalReference(line) ||
                string.IsNullOrEmpty(line.Trim()))
            {
                return true;
            }

            return false;
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
                string text = linesIn[++currentLine].Trim();
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

        int GetGlobalReference(string line)
        {
            string[] parts = SplitLine(line);
            if (parts == null || parts.Length == 0)
                return -1;

            if (QuestMachine.Instance.GlobalVarsTable.HasValue(parts[0]))
            {
                return ParseInt(QuestMachine.Instance.GlobalVarsTable.GetValue("id", parts[0]));
            }

            return -1;
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