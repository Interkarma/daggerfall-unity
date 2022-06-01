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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Output quest information on HUD to view state in real-time and optionally step-through execution.
    /// Uses default keys (set in DialogShortcuts.txt file):
    ///  * Ctrl+Shift+D             Cycle through debugger and global variables display state
    ///  * Ctrl+Shift+RightArrow    Show next quest tasks/vars/timers (only when debugger HUD open)
    ///  * Ctrl+Shift+LeftArrow     Show previous quest tasks/vars/timers (only when debugger HUD open)
    ///  * Ctrl+Shift+UpArrow       Teleport to next dungeon marker (only when debugger HUD open inside dungeon)
    ///  * Ctrl+Shift+DownArrow     Teleport to next dungeon marker (only when debugger HUD open inside dungeon)
    /// NOTE: EnableQuestDebugger must be True in settings.ini
    /// </summary>
    public class HUDQuestDebugger : Panel
    {
        const int maxTaskRows = 20;
        const int maxTimerRows = 3;
        const int maxGlobalRows = 31;
        const int rowHeight = 10;
        const int taskColWidth = 60;
        const int timerColWidth = 100;
        public const string noQuestsRunning = "NO QUESTS RUNNING";
        public const string questRunning = "Running";
        public const string questFinishedSuccess = "Finished (success)";
        public const string questFinishedEnded = "Finished (ended)";
        const int taskLabelPoolCount = 84;
        const int timerLabelPoolCount = 20;
        const int globalLabelPoolCount = 64;

        ulong[] allQuests;
        int currentQuestIndex;
        Quest currentQuest;
        int currentMarkerIndex = -1;

        DisplayState displayState = DisplayState.Nothing;

        TextLabel questNameLabel = new TextLabel();
        TextLabel processLabel = new TextLabel();
        TextLabel tasksHeaderLabel = new TextLabel();
        TextLabel timersHeaderLabel = new TextLabel();
        TextLabel[] taskLabelPool = new TextLabel[taskLabelPoolCount];
        TextLabel[] timerLabelPool = new TextLabel[timerLabelPoolCount];
        TextLabel[] globalsLabelPool = new TextLabel[globalLabelPoolCount];

        public enum DisplayState
        {
            Nothing,
            QuestState,
            QuestStateFull,
        }

        public Quest CurrentQuest
        {
            get { return currentQuest; }
        }

        public DisplayState State
        {
            get { return displayState; }
            set { displayState = value; }
        }

        public HUDQuestDebugger()
            : base()
        {
            QuestMachine.OnQuestStarted += QuestMachine_OnQuestStarted;
            QuestMachine.OnQuestEnded += QuestMachine_OnQuestEnded;
            SaveLoadManager.OnLoad += SaveLoadManager_OnLoad;

            // Quest name label
            questNameLabel.Text = noQuestsRunning;
            questNameLabel.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            questNameLabel.ShadowPosition = Vector2.zero;
            questNameLabel.Position = new Vector2(0, 0);
            Components.Add(questNameLabel);

            // Process label
            processLabel.TextColor = Color.white;
            processLabel.ShadowPosition = Vector2.zero;
            processLabel.Position = new Vector2(0, 10);
            Components.Add(processLabel);

            // Tasks header label
            tasksHeaderLabel.Text = "Tasks";
            tasksHeaderLabel.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            tasksHeaderLabel.ShadowPosition = Vector2.zero;
            tasksHeaderLabel.Position = new Vector2(0, 25);
            Components.Add(tasksHeaderLabel);

            // Timers header label
            timersHeaderLabel.Text = "Timers";
            timersHeaderLabel.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            timersHeaderLabel.ShadowPosition = Vector2.zero;
            timersHeaderLabel.Position = new Vector2(0, 255);
            Components.Add(timersHeaderLabel);

            // Label pool setup
            SetupTaskLabels(new Vector2(0, 35));
            SetupTimerLabels(new Vector2(0, 265));
            SetupGlobalVarLabel(new Vector2(550, 0));

            // Disable global vars by default
            EnableGlobalVars(false);

            // Set starting state
            ClearCurrentQuest();
            RefreshQuestsList();

            // Tick with QuestMachine
            QuestMachine.OnTick += QuestMachine_OnTick;
        }

        public override void Update()
        {
            base.Update();

            // Display nothing and exit if quest debugger not enabled
            if (!DaggerfallUnity.Settings.EnableQuestDebugger)
            {
                displayState = DisplayState.Nothing;
                return;
            }

            // Do not tick while HUD fading or load in progress
            // This is to prevent quest popups or other actions while player/world unavailable
            if (DaggerfallUI.Instance.FadeBehaviour.FadeInProgress || SaveLoadManager.Instance.LoadInProgress)
                return;

            if (QuestMachine.Instance.QuestCount == 0 && currentQuest != null)
            {
                ClearCurrentQuest();
                FullRefresh();
                return;
            }

            if (allQuests == null || allQuests.Length != QuestMachine.Instance.QuestCount)
                FullRefresh();

            // Clamp display state
            if (displayState < DisplayState.Nothing || displayState > DisplayState.QuestStateFull)
                displayState = DisplayState.Nothing;

            // Change quest selection
            HotkeySequence.KeyModifiers keyModifiers = HotkeySequence.GetKeyboardKeyModifiers();
            if (DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.DebuggerPrevQuest).IsDownWith(keyModifiers))
                MovePreviousQuest();
            else if (DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.DebuggerNextQuest).IsDownWith(keyModifiers))
                MoveNextQuest();

            // Change marker selection
            if (DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.DebuggerPrevMarker).IsDownWith(keyModifiers))
                MovePreviousMarker();
            else if (DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.DebuggerNextMarker).IsDownWith(keyModifiers))
                MoveNextMarker();
        }

        private void QuestMachine_OnTick()
        {
            // Update global variables status
            if (displayState == DisplayState.QuestStateFull)
            {
                UpdateGlobalVarsStatus();
            }

            // Must at least one running quests
            if (QuestMachine.Instance.QuestCount == 0)
            {
                if (currentQuest != null)
                {
                    ClearCurrentQuest();
                }
                return;
            }

            // If no quest has been set, get the first active quest
            // We know from previous check there is at least one quest available
            if (currentQuest == null)
            {
                RefreshQuestsList();
                SetCurrentQuest(QuestMachine.Instance.GetQuest(allQuests[0]));
            }

            // Update task and timer status
            UpdateQuestStatus();
        }

        public void NextState()
        {
            // Rotate through states
            displayState = displayState + 1;
            if (displayState > DisplayState.QuestStateFull)
                displayState = DisplayState.Nothing;

            // Show/hide globals
            if (displayState == DisplayState.QuestStateFull)
                EnableGlobalVars(true);
            else
                EnableGlobalVars(false);
        }

        #region Private Methods

        void UpdateGlobalVarsStatus()
        {
            // Set global vars status
            PersistentGlobalVars playerGlovalVars = GameManager.Instance.PlayerEntity.GlobalVars;
            for (int i = 0; i < globalsLabelPool.Length; i++)
            {
                if (playerGlovalVars.GetGlobalVar(i))
                    globalsLabelPool[i].TextColor = Color.green;
                else
                    globalsLabelPool[i].TextColor = Color.gray;
            }
        }

        void UpdateQuestStatus()
        {
            // Set task status
            Quest.TaskState[] states = currentQuest.GetTaskStates();
            for (int i = 0; i < states.Length && i < taskLabelPool.Length; i++)
            {
                if (!states[i].set)
                    taskLabelPool[i].TextColor = Color.gray;
                else
                    taskLabelPool[i].TextColor = Color.green;
            }

            // Set timer status
            QuestResource[] clocks = currentQuest.GetAllResources(typeof(Clock));
            for (int i = 0; i < clocks.Length && i < timerLabelPool.Length; i++)
            {
                Clock clock = (Clock)clocks[i];
                timerLabelPool[i].Text = string.Format("{0} [{1}]", clock.Symbol.Original, clock.GetTimeString(clock.RemainingTimeInSeconds));
                if (clock.Enabled && !clock.Finished)
                    timerLabelPool[i].TextColor = Color.green;
                else if (!clock.Enabled && !clock.Finished)
                    timerLabelPool[i].TextColor = Color.gray;
                else
                    timerLabelPool[i].TextColor = Color.red;
            }

            // Set running status
            // TODO: Use this line for step-through debugging
            if (!currentQuest.QuestComplete)
            {
                processLabel.Text = string.Format("[{0}] - {1}", DaggerfallUnity.Instance.WorldTime.Now.MinTimeString(), questRunning);
            }
            else
            {
                if (currentQuest.QuestSuccess)
                    processLabel.Text = string.Format("[{0}] - {1}", DaggerfallUnity.Instance.WorldTime.Now.MinTimeString(), questFinishedSuccess);
                else
                    processLabel.Text = string.Format("[{0}] - {1}", DaggerfallUnity.Instance.WorldTime.Now.MinTimeString(), questFinishedEnded);
            }
        }

        void SetupTaskLabels(Vector2 startPosition)
        {
            // Create a pool of labels for output
            int row = 0, col = 0;
            for (int i = 0; i < taskLabelPool.Length; i++)
            {
                // Get current position
                Vector2 position = startPosition + new Vector2(col * taskColWidth, row * rowHeight);

                // Create label at current position
                TextLabel label = new TextLabel();
                label.Text = string.Format("label{0}", i);
                label.Position = position;
                label.TextColor = Color.gray;
                label.ShadowPosition = Vector2.zero;
                label.Enabled = false;
                taskLabelPool[i] = label;
                Components.Add(label);

                // Step row and column
                if (++row > maxTaskRows)
                {
                    row = 0;
                    col++;
                }
            }
        }

        void SetupTimerLabels(Vector2 startPosition)
        {
            // Create a pool of labels for output
            int row = 0, col = 0;
            for (int i = 0; i < timerLabelPool.Length; i++)
            {
                // Get current position
                Vector2 position = startPosition + new Vector2(col * timerColWidth, row * rowHeight);

                // Create label at current position
                TextLabel label = new TextLabel();
                label.Text = string.Format("label{0}", i);
                label.Position = position;
                label.TextColor = Color.gray;
                label.ShadowPosition = Vector2.zero;
                label.Enabled = false;
                timerLabelPool[i] = label;
                Components.Add(label);

                // Step row and column
                if (++row > maxTimerRows)
                {
                    row = 0;
                    col++;
                }
            }
        }

        void SetupGlobalVarLabel(Vector2 startPosition)
        {
            // Get quest global variables table
            Table globalVarsTables = QuestMachine.Instance.GlobalVarsTable;

            // Create a pool of labels for output
            int row = 0, col = 0;
            for (int i = 0; i < globalsLabelPool.Length; i++)
            {
                // Get current position
                Vector2 position = startPosition + new Vector2(col * timerColWidth, row * rowHeight);

                // Get global variable name
                string[] rowData = globalVarsTables.GetRow(i);

                // Create label at current position
                TextLabel label = new TextLabel();
                label.Text = string.Format("{0}", rowData[1]);
                label.Position = position;
                label.TextColor = Color.gray;
                label.ShadowPosition = Vector2.zero;
                label.Enabled = true;
                globalsLabelPool[i] = label;
                Components.Add(label);

                // Step row and column
                if (++row > maxGlobalRows)
                {
                    row = 0;
                    col++;
                }
            }
        }

        void ClearCurrentQuest()
        {
            currentQuest = null;

            // Set headers
            questNameLabel.Text = noQuestsRunning;
            tasksHeaderLabel.Enabled = false;
            processLabel.Enabled = false;
            timersHeaderLabel.Enabled = false;

            // Disable task labels
            for (int i = 0; i < taskLabelPool.Length; i++)
            {
                taskLabelPool[i].Enabled = false;
            }

            // Disable timer labels
            for(int i = 0; i < timerLabelPool.Length; i++)
            {
                timerLabelPool[i].Enabled = false;
            }
        }

        void SetCurrentQuest(Quest quest)
        {
            ClearCurrentQuest();

            currentQuest = quest;
            RefreshQuestsList();

            // Set quest index
            questNameLabel.Text = string.Format("[{0} of {1}] ", currentQuestIndex + 1, allQuests.Length);

            // Set quest name
            if (!string.IsNullOrEmpty(quest.DisplayName))
                questNameLabel.Text += string.Format("{0} '{1}' ", quest.QuestName, quest.DisplayName);
            else
                questNameLabel.Text += string.Format("{0} ", quest.QuestName);

            // Set quest UID
            questNameLabel.Text += string.Format("[UID={0}]", quest.UID);

            // Set headers
            tasksHeaderLabel.Enabled = true;
            processLabel.Enabled = true;
            timersHeaderLabel.Enabled = true;

            // Set task labels
            Quest.TaskState[] states = currentQuest.GetTaskStates();
            for (int i = 0; i < states.Length && i < taskLabelPool.Length; i++)
            {
                taskLabelPool[i].Enabled = true;
                if (states[i].type == Task.TaskType.Headless)
                    taskLabelPool[i].Text = "startup";
                else if (states[i].type == Task.TaskType.PersistUntil)
                {
                    Task task = quest.GetTask(states[i].symbol);
                    taskLabelPool[i].Text = string.Format("until_{0}", task.TargetSymbol.Name);
                }
                else
                    taskLabelPool[i].Text = states[i].symbol.Name;
            }

            // Set timer status
            QuestResource[] clocks = currentQuest.GetAllResources(typeof(Clock));
            for (int i = 0; i < clocks.Length && i < timerLabelPool.Length; i++)
            {
                timerLabelPool[i].Enabled = true;
            }
        }

        void RefreshQuestsList()
        {
            currentQuestIndex = -1;
            allQuests = QuestMachine.Instance.GetAllQuests();
            if (allQuests != null && allQuests.Length > 0 && currentQuest != null)
            {
                // Find index of current quest
                for (int i = 0; i < allQuests.Length; i++)
                {
                    Quest quest = QuestMachine.Instance.GetQuest(allQuests[i]);
                    if (quest != null && quest.UID == currentQuest.UID)
                    {
                        currentQuestIndex = i;
                        break;
                    }
                }
            }
        }

        void FullRefresh()
        {
            // Clear existing state
            ClearCurrentQuest();
            RefreshQuestsList();

            // Exit if no quests at all
            if (QuestMachine.Instance.QuestCount == 0)
            {
                currentQuestIndex = -1;
                return;
            }

            // Try to get first active quest
            ulong[] activeQuests = QuestMachine.Instance.GetAllActiveQuests();
            if (activeQuests != null && activeQuests.Length > 0)
            {
                SetCurrentQuest(QuestMachine.Instance.GetQuest(activeQuests[0]));
                return;
            }

            // Otherwise just use latest quest
            ulong[] quests = QuestMachine.Instance.GetAllQuests();
            SetCurrentQuest(QuestMachine.Instance.GetQuest(quests[quests.Length - 1]));
        }

        void MoveNextQuest()
        {
            if (allQuests == null || allQuests.Length == 0)
                return;

            if (++currentQuestIndex >= allQuests.Length)
                currentQuestIndex = 0;

            SetCurrentQuest(QuestMachine.Instance.GetQuest(allQuests[currentQuestIndex]));
        }

        void MovePreviousQuest()
        {
            if (allQuests == null || allQuests.Length == 0)
                return;

            if (--currentQuestIndex < 0)
                currentQuestIndex = allQuests.Length - 1;

            SetCurrentQuest(QuestMachine.Instance.GetQuest(allQuests[currentQuestIndex]));
        }

        void MoveNextMarker()
        {
            // Must be inside a dungeon
            if (!GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon)
                return;

            // Get markers
            Vector3[] markerPositions = GameManager.Instance.PlayerEnterExit.Dungeon.GetAllDebuggerMarkerPositions();
            if (markerPositions == null || markerPositions.Length == 0)
                return;

            // Select next index
            if (++currentMarkerIndex >= markerPositions.Length)
                currentMarkerIndex = 0;

            // Move player object to marker position
            GameManager.Instance.PlayerObject.transform.localPosition = markerPositions[currentMarkerIndex];
            GameManager.Instance.PlayerMotor.FixStanding();

            Debug.LogFormat("Moved to next marker - index {0}", currentMarkerIndex);
        }

        void MovePreviousMarker()
        {
            // Must be inside a dungeon
            if (!GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon)
                return;

            // Get markers
            Vector3[] markerPositions = GameManager.Instance.PlayerEnterExit.Dungeon.GetAllDebuggerMarkerPositions();
            if (markerPositions == null || markerPositions.Length == 0)
                return;

            // Select previous index
            if (--currentMarkerIndex < 0)
                currentMarkerIndex = markerPositions.Length - 1;

            // Move player object to marker position
            GameManager.Instance.PlayerObject.transform.localPosition = markerPositions[currentMarkerIndex];
            GameManager.Instance.PlayerMotor.FixStanding();

            Debug.LogFormat("Moved to previous marker - index {0}", currentMarkerIndex);
        }

        void EnableGlobalVars(bool value)
        {
            for (int i = 0; i < globalsLabelPool.Length; i++)
            {
                globalsLabelPool[i].Enabled = value;
            }
        }

        #endregion

        #region Event Handlers

        private void QuestMachine_OnQuestStarted(Quest quest)
        {
            ClearCurrentQuest();
            SetCurrentQuest(quest);
        }

        private void QuestMachine_OnQuestEnded(Quest quest)
        {
            RefreshQuestsList();
        }

        private void SaveLoadManager_OnLoad(SaveData_v1 saveData)
        {
            FullRefresh();
        }

        #endregion
    }
}