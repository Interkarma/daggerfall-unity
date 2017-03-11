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
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Contains live state of quests in play.
    /// Quests are instantiated from text source and executed inside quest machine.
    /// Each quest is assigned a unique id (UID) as its possible for same question to be instantiated multiple times,
    /// such as a basic fetch quest to two different dungeons. The name of quest cannot not be used for unique identification.
    /// Child resources generally will not care about quest UID, but this is used by quest machine.
    /// </summary>
    public class Quest
    {
        #region Fields

        // Quest object collections
        Dictionary<int, Message> messages = new Dictionary<int, Message>();
        Dictionary<string, Clock> clocks = new Dictionary<string, Clock>();
        Dictionary<string, Task> tasks = new Dictionary<string, Task>();
        Dictionary<string, Place> places = new Dictionary<string, Place>();

        ulong uid;
        bool questComplete = false;
        Dictionary<int, LogEntry> activeLogMessages = new Dictionary<int, LogEntry>();

        DaggerfallDateTime questStartTime;

        #endregion

        #region Structures

        /// <summary>
        /// Stores active log messages as ID only.
        /// This allows text to be linked/unlinked into log without duplication of text.
        /// Actual quest UI can decide how to present these messages to user.
        /// </summary>
        public struct LogEntry
        {
            public int stepID;
            public int messageID;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets quest UID assigned at create time.
        /// </summary>
        public ulong UID
        {
            get { return uid; }
        }

        /// <summary>
        /// True when quest has completed and will be deleted from quest machine.
        /// </summary>
        public bool QuestComplete
        {
            get { return questComplete; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Quest()
        {
            uid = DaggerfallUnity.NextUID;
            questStartTime = new DaggerfallDateTime(DaggerfallUnity.Instance.WorldTime.Now);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Update quest.
        /// </summary>
        public void Update()
        {
            // Do nothing if complete
            // Now waiting to be removed from quest machine
            if (questComplete)
                return;

            // Update tasks
            foreach(Task task in tasks.Values)
            {
                task.Update();
            }
        }

        public void EndQuest()
        {
            questComplete = true;
        }

        public void SetTask(string name)
        {
            Task task = GetTask(name);
            if (task != null)
                task.Set();
        }

        public void UnsetTask(string name)
        {
            Task task = GetTask(name);
            if (task != null)
                task.Unset();
        }

        #endregion

        #region Log Message Methods

        /// <summary>
        /// Adds quest log message for quest at step position.
        /// Quests can only have 0-9 steps and cannot log the same step more than once at a time.
        /// </summary>
        /// <param name="stepID">StepID to key this message.</param>
        /// <param name="messageID">MessageID to display for this step.</param>
        public void AddLogStep(int stepID, int messageID)
        {
            // Cannot log step more than once at a time
            if (activeLogMessages.ContainsKey(stepID))
            {
                throw new System.Exception("Attempting to log stepID + " + stepID + "more than once.");
            }

            // Add the step to active log messages
            LogEntry entry = new LogEntry();
            entry.stepID = stepID;
            entry.messageID = messageID;
            activeLogMessages.Add(stepID, entry);

            // Test messages using popup
            // To be removed
            //TestLogMessages();
        }

        /// <summary>
        /// Removes quest log message for step position.
        /// </summary>
        /// <param name="stepID">StepID to remove from quest log.</param>
        public void RemoveLogStep(int stepID)
        {
            // Remove the step
            if (activeLogMessages.ContainsKey(stepID))
            {
                activeLogMessages.Remove(stepID);
            }
        }

        /// <summary>
        /// Gets the active log messages associated with this quest.
        /// Usually only one log step is active at a time.
        /// This allows log messages to be displayed however desired (as list, verbose, sorted, etc.).
        /// </summary>
        /// <returns>LogEntry array.</returns>
        public LogEntry[] GetLogMessages()
        {
            // Create an array of active log messages
            LogEntry[] logs = new LogEntry[activeLogMessages.Count];
            foreach (LogEntry log in activeLogMessages.Values)
            {
                logs[0] = log;
            }

            return logs;
        }

        /// <summary>
        /// Quick test of grabbing log message before UI is ready.
        /// To be removed.
        /// </summary>
        public void TestLogMessages()
        {
            LogEntry[] logs = GetLogMessages();
            for (int i = 0; i < logs.Length; i++)
            {
                Message message = GetMessage(logs[i].messageID);
                if (message != null)
                {
                    // Get message tokens
                    DaggerfallConnect.Arena2.TextFile.Token[] tokens = message.GetTextTokens();

                    UserInterfaceWindows.DaggerfallMessageBox messageBox = new UserInterfaceWindows.DaggerfallMessageBox(DaggerfallUI.UIManager);
                    messageBox.SetTextTokens(tokens);
                    messageBox.ClickAnywhereToClose = true;
                    messageBox.AllowCancel = true;
                    messageBox.ParentPanel.BackgroundColor = Color.clear;
                    messageBox.Show();
                }
            }
        }

        #endregion

        #region Resource Query Methods

        public Message GetMessage(int messageID)
        {
            if (messages.ContainsKey(messageID))
                return messages[messageID];
            else
                return null;
        }

        public Task GetTask(string name)
        {
            if (tasks.ContainsKey(name))
                return tasks[name];
            else
                return null;
        }

        #endregion

        #region Resource Allocation Methods

        public void AddMessage(int messageID, Message message)
        {
            messages.Add(messageID, message);
        }

        public void AddClock(string symbol, Clock clock)
        {
            clocks.Add(symbol, clock);
        }

        public void AddPlace(string symbol, Place place)
        {
            places.Add(symbol, place);
        }

        public void AddTask(string symbol, Task task)
        {
            tasks.Add(symbol, task);
        }

        #endregion

        #region Private Methods
        #endregion
    }
}