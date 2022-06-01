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
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Tasks are executed by other tasks, clock timeouts, etc. Somewhat like a subroutine.
    /// Tasks can contain conditions at start that if not met appear to prevent further execution of task.
    /// Task name can also be used as a symbol to query if task has been triggered or not.
    /// Provided conditions are met, commands under a task usually run once then end.
    /// Repeating tasks execute (i.e. each command stays alive) until target task/variable completed.
    /// Variables are a special task with no conditions or actions, just set/unset.
    /// </summary>
    public class Task
    {
        #region Fields

        Symbol symbol;              // Unique symbol of task, can be used like a boolean if to check if task has completed
        Symbol targetSymbol;        // Symbol of target task/variable to check, used by repeating tasks only
        bool triggered;             // Is the task currently triggered/true/set?
        bool prevTriggered;         // Was the task triggered/true/set on last tick?
        TaskType type;              // Type of task
        bool dropped;               // Tasks is permanently dropped

        string globalVarName;       // Name of global variable from source
        int globalVarLink = -1;     // Link to a global variable

        Quest parentQuest = null;
        bool hasTriggerConditions = false;
        List<IQuestAction> actions = new List<IQuestAction>();

        #endregion

        #region Properties

        public Quest ParentQuest
        {
            get { return parentQuest; }
        }

        public Symbol Symbol
        {
            get { return symbol; }
            set { symbol = value; }
        }

        public Symbol TargetSymbol
        {
            get { return targetSymbol; }
        }

        public TaskType Type
        {
            get { return type; }
        }

        public bool IsTriggered
        {
            get { return GetTriggerValue(); }
            set { SetTriggerValue(value); }
        }

        public bool IsDropped
        {
            get { return dropped; }
        }

        public bool HasTriggerConditions
        {
            get { return hasTriggerConditions; }
        }

        public string GlobalVarName
        {
            get { return globalVarName; }
        }

        public int GlobalVarLink
        {
            get { return globalVarLink; }
        }

        public IEnumerable<IQuestAction> Actions
        {
            get { return actions; }
        }

        #endregion

        #region Enumerations

        public enum TaskType
        {
            Headless,           // Startup task - starts automatically when quest begins
            Standard,           // Normal task - must be started by a set or trigger
            PersistUntil,       // Automatic startup - when check is true, task and all members stop immediately
            Variable,           // Boolean variable only - must be set/unset
            GlobalVarLink,      // Boolean link to global variable - must be set/unset
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="parentQuest">Parent quest.</param>
        public Task(Quest parentQuest)
        {
            this.parentQuest = parentQuest;
        }

        /// <summary>
        /// Create a task from QBN source code.
        /// </summary>
        /// <param name="parentQuest">Parent quest.</param>
        /// <param name="lines">Source lines of task block.</param>
        public Task(Quest parentQuest, string[] lines)
        {
            this.parentQuest = parentQuest;
            SetTask(lines);
        }

        /// <summary>
        /// Global variable constructor.
        /// </summary>
        /// <param name="parentQuest">Parent quest.</param>
        /// <param name="lines">Source lines of task block.</param>
        /// <param name="globalVar">Global variable key.</param>
        public Task(Quest parentQuest, string[] lines, int globalVar)
        {
            this.parentQuest = parentQuest;
            SetTask(lines, globalVar);
        }

        #endregion

        #region Public Methods

        public void SetTask(string[] lines, int globalVar = -1)
        {
            int i = 0;

            // Handle global variable link task
            if (globalVar != -1)
            {
                // Read globalvar header for symbol
                ReadGlobalVarTaskHeader(lines[i++]);

                // Create custom task type
                type = TaskType.GlobalVarLink;
                targetSymbol = null;
                globalVarLink = globalVar;
                ReadTaskLines(lines, i);
                return;
            }

            // Handle standard, variable, and headless tasks
            if (ReadTaskHeader(lines[i]))
            {
                // Increment to next line after header
                i++;
            }
            else
            {
                // No match on task header treat as a headless task (e.g. startup task)
                // Startup task begins as triggered
                type = TaskType.Headless;
                targetSymbol = null;
                IsTriggered = true;
                symbol = new Symbol(DaggerfallUnity.NextUID.ToString());
            }

            // Read task lines
            ReadTaskLines(lines, i);
        }

        public void Update()
        {
            // "Always on" triggers can both start and stop a task
            // An example is S0000977 _S.03_ task which uses a single "when" trigger to start/stop spawns and play vengeance sound in Daggerfall city
            // But this becomes an issue if task has multiple "always on" triggers, as trigger A might start the task then trigger B will stop it again
            // One example of this behaviour is W0C00Y00 _pcgetsgold_ task where either "when" trigger can start reward task but subsequent triggers should not stop it again
            // This implementation considers the first "always on" trigger as the "primary" - able to both start & stop parent task
            // Subsequent "always on" triggers within the same task are "secondary" - only able to start a parent task but not stop it again
            bool ranPrimaryAlwaysOnTrigger = false;

            // Iterate conditions and actions for this task
            foreach (IQuestAction action in actions)
            {
                // Completed actions are never executed again
                // The action itself should decide if/when to be complete
                // At a higher level, turning off the task will also disable actions
                // The exception being triggers which always run even when task disabled
                if (action.IsComplete)
                    continue;

                // Always check trigger conditions
                // These can turn task on when any trigger evaluates true
                // They are no longer checked once task is triggered (unless set to always be on)
                // But can fire again if owning task is unset/rearmed later
                if (action.IsTriggerCondition && !IsTriggered || action.IsAlwaysOnTriggerCondition)
                {
                    // Handle primary/secondary "always on" triggers
                    if (action.IsAlwaysOnTriggerCondition && !ranPrimaryAlwaysOnTrigger)
                    {
                        // Primary "always on" trigger can start/stop parent task
                        // Flag is raised at first "always on" trigger so that subsequent triggers know the primary has run
                        IsTriggered = action.CheckTrigger(this);
                        ranPrimaryAlwaysOnTrigger = true;
                    }
                    else if (action.IsAlwaysOnTriggerCondition && ranPrimaryAlwaysOnTrigger)
                    {
                        // Secondary "always on" triggers can only start parent task, they cannot stop it again
                        if (action.CheckTrigger(this))
                            IsTriggered = true;
                    }
                    else
                    {
                        // All other triggers
                        IsTriggered = action.CheckTrigger(this);
                    }
                }

                // Tick other actions only when active
                if (IsTriggered && !action.IsTriggerCondition)
                {
                    // Initialise action if task was previously untriggered
                    if (!prevTriggered)
                    {
                        action.InitialiseOnSet();
                    }

                    // Update action and handle quest break
                    action.Update(this);
                    if (ParentQuest.QuestBreak)
                        return;
                }
            }

            // If this is a PersistUntil task we need to check target condition for unset state.
            // Experimentation seems to indicate these monitoring tasks get at least one tick.
            // For example, starting player outside of PH in classic using Z.CFG cheat will
            // cause main quest to start as normal before _exitstarter_ flag has a chance to
            // terminate "until _exitstarter_ performed" task.
            // Performing termination check AFTER executing task at least once to ensure behaviour matches classic.
            if (type == TaskType.PersistUntil)
            {
                Task targetTask = ParentQuest.GetTask(targetSymbol);
                if (targetTask.IsTriggered)
                {
                    // Target now set, terminate persistent task
                    Clear();
                }
                else
                {
                    // Rearm actions so they can repeat next run
                    // This behaviour obvserved in S0000011 in "until _S.12_ performed" to continuously clear Barenziah click
                    RearmActions();
                }
                    
            }

            // Store trigger state this update
            prevTriggered = IsTriggered;
        }

        /// <summary>
        /// Another way to start/trigger task.
        /// </summary>
        public void Start()
        {
            IsTriggered = true;
        }

        /// <summary>
        /// Another way to clear/rearm task.
        /// </summary>
        public void Clear()
        {
            IsTriggered = false;
        }

        /// <summary>
        /// Permanently clear trigger state and drop task.
        /// This is a one-way setting.
        /// </summary>
        public void Drop()
        {
            dropped = true;
            IsTriggered = false;
        }

        /// <summary>
        /// Called by parent quest when it ends so that all actions of this task can dispose themselves.
        /// </summary>
        public void DisposeActions()
        {
            foreach (IQuestAction action in actions)
            {
                action.Dispose();
            }
        }

        /// <summary>
        /// Add action to task
        /// </summary>
        /// <param name="questAction">Action</param>
        public void AddQuestAction(IQuestAction questAction)
        {
            if (questAction != null && this.actions.Contains(questAction) == false)
                actions.Add(questAction);
        }

        /// <summary>
        /// Adds actions from existing task
        /// </summary>
        /// <param name="other"></param>
        public void CopyQuestActions(Task other)
        {
            foreach (var action in other.actions)
            {
                this.actions.Add(action);
            }
        }

        #endregion

        #region Private Methods

        bool ReadGlobalVarTaskHeader(string line)
        {
            Match match = Regex.Match(line, @"(?<globalVarName>[a-zA-Z0-9_.]+) (?<symbol>[a-zA-Z0-9_.]+)");
            if (match.Success)
            {
                globalVarName = match.Groups["globalVarName"].Value;
                symbol = new Symbol(match.Groups["symbol"].Value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reads task header based on supported task types.
        /// </summary>
        /// <param name="line">Header source line.</param>
        /// <returns>True if task header match found.</returns>
        bool ReadTaskHeader(string line)
        {
            // Try to match task types
            Match match = Regex.Match(line, @"(?<symbol>[a-zA-Z0-9_.]+) (?<task>task):|until (?<symbol>[a-zA-Z0-9_.]+) (?<persist>performed:)|(?<variable>variable) (?<symbol>[a-zA-Z0-9_.]+)");
            if (match.Success)
            {
                if (!string.IsNullOrEmpty(match.Groups["task"].Value))
                {
                    // Standard task
                    type = TaskType.Standard;
                    targetSymbol = null;
                    symbol = new Symbol(match.Groups["symbol"].Value);
                }
                else if (!string.IsNullOrEmpty(match.Groups["persist"].Value))
                {
                    // PersistUntil task
                    // Starts automatically and terminates when target symbol is true
                    // Daggerfall seems to use these most commonly to init state or begin timed spawns of enemy mobiles
                    // It rarely makes sense for these actions to be repeated "over and over" as per Template docs
                    type = TaskType.PersistUntil;
                    targetSymbol = new Symbol(match.Groups["symbol"].Value);
                    IsTriggered = true;
                    symbol = new Symbol(DaggerfallUnity.NextUID.ToString());
                }
                else if (!string.IsNullOrEmpty(match.Groups["variable"].Value))
                {
                    // Variable
                    type = TaskType.Variable;
                    targetSymbol = null;
                    IsTriggered = false;
                    symbol = new Symbol(match.Groups["symbol"].Value);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Reads task lines to find conditions and actions.
        /// </summary>
        /// <param name="lines">Array of lines in this task block.</param>
        /// <param name="startLine">First line after task header (if present).</param>
        void ReadTaskLines(string[] lines, int startLine)
        {
            // Step through lines of quest and log when encountering unsupported lines
            // This will generate a lot of log noise early on until things are further along
            for (int i = startLine; i < lines.Length; i++)
            {
                // Try to find registered action template using this line
                IQuestAction actionTemplate = QuestMachine.Instance.GetActionTemplate(lines[i]);
                if (actionTemplate != null)
                {
                    // Create a new action from template (don't link template itself)
                    IQuestAction action = actionTemplate.CreateNew(lines[i], ParentQuest);
                    if (action != null)
                    {
                        if (QuestMachine.Instance.IsDebugModeEnabled)
                            action.DebugSource = lines[i].Trim();

                        actions.Add(action);
                        if (action.IsTriggerCondition)
                            hasTriggerConditions = true;
                    }
                }
                else
                {
                    Debug.LogFormat("Action not found. Ignoring '{0}'", lines[i]);
                }
            }
        }

        void RearmActions()
        {
            foreach (IQuestAction action in actions)
            {
                action.RearmAction();
            }
        }

        public bool GetTriggerValue()
        {
            if (globalVarLink != -1)
                return triggered = GameManager.Instance.PlayerEntity.GlobalVars.GetGlobalVar(globalVarLink);
            else
                return triggered;
        }

        public void SetTriggerValue(bool value)
        {
            if (globalVarLink != -1)
            {
                GameManager.Instance.PlayerEntity.GlobalVars.SetGlobalVar(globalVarLink, value);
                triggered = value;
            }
            else
            {
                triggered = value;
            }

            // If this task is dropped then trigger state cannot be changed
            if (dropped)
            {
                triggered = false;
                return;
            }

            // If clearing a task then need to rearm actions
            if (value == false)
            {
                RearmActions();
            }
        }

        #endregion

        #region Serialization

        [fsObject("v1")]
        public struct TaskSaveData_v1
        {
            public Symbol symbol;
            public Symbol targetSymbol;
            public bool triggered;
            public bool prevTriggered;
            public TaskType type;
            public bool dropped;
            public string globalVarName;
            public int globalVarLink;
            public bool hasTriggerConditions;
            public ActionTemplate.ActionSaveData_v1[] actions;
        }

        public TaskSaveData_v1 GetSaveData()
        {
            // Save base task data
            TaskSaveData_v1 data = new TaskSaveData_v1();
            data.symbol = symbol;
            data.targetSymbol = targetSymbol;
            data.triggered = IsTriggered;
            data.prevTriggered = prevTriggered;
            data.type = type;
            data.dropped = dropped;
            data.globalVarName = globalVarName;
            data.globalVarLink = globalVarLink;
            data.hasTriggerConditions = hasTriggerConditions;

            // Save actions
            List<ActionTemplate.ActionSaveData_v1> actionSaveDataList = new List<ActionTemplate.ActionSaveData_v1>();
            foreach(ActionTemplate action in actions)
            {
                actionSaveDataList.Add(action.GetActionSaveData());
            }
            data.actions = actionSaveDataList.ToArray();

            return data;
        }

        public void RestoreSaveData(TaskSaveData_v1 data)
        {
            // Restore base task data
            symbol = data.symbol;
            targetSymbol = data.targetSymbol;
            IsTriggered = data.triggered;
            prevTriggered = data.prevTriggered;
            type = data.type;
            dropped = data.dropped;
            globalVarName = data.globalVarName;
            globalVarLink = data.globalVarLink;
            hasTriggerConditions = data.hasTriggerConditions;

            // Restore actions
            actions.Clear();
            foreach (ActionTemplate.ActionSaveData_v1 actionData in data.actions)
            {
                // Construct deserialized QuestAction based on type
                System.Reflection.ConstructorInfo ctor = actionData.type.GetConstructor(new Type[] { typeof(Quest) });
                ActionTemplate action = (ActionTemplate)ctor.Invoke(new object[] { ParentQuest });

                // Restore state
                action.RestoreActionSaveData(actionData);
                actions.Add(action);
            }
        }

        #endregion
    }
}
