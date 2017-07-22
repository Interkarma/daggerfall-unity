// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
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

        public bool IsSet
        {
            get { return GetTriggerValue(); }
            set { SetTriggerValue(value); }
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
                triggered = true;
                symbol = new Symbol(DaggerfallUnity.NextUID.ToString());
            }

            // Read task lines
            ReadTaskLines(lines, i);
        }

        public void Update()
        {
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
                if (action.IsTriggerCondition && !triggered || action.IsAlwaysOnTriggerCondition)
                {
                    if (action.CheckTrigger(this))
                        triggered = true;
                    else
                        triggered = false;
                }

                // Tick other actions only when active
                if (triggered && !action.IsTriggerCondition)
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
                // Unset this task when target symbol is also unset
                Task targetTask = ParentQuest.GetTask(targetSymbol);
                if (targetTask != null)
                {
                    if (!targetTask.IsSet)
                        Unset();
                    // Should these tasks be able to rearm?
                    // Would need strong evidence before allowing this
                }
            }

            // Store trigger state this update
            prevTriggered = triggered;
        }

        /// <summary>
        /// Another way to set/trigger task.
        /// </summary>
        public void Set()
        {
            IsSet = true;
        }

        /// <summary>
        /// Another way to unset/rearm task.
        /// </summary>
        public void Unset()
        {
            IsSet = false;
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
                    triggered = true;
                    symbol = new Symbol(DaggerfallUnity.NextUID.ToString());
                }
                else if (!string.IsNullOrEmpty(match.Groups["variable"].Value))
                {
                    // Variable
                    type = TaskType.Variable;
                    targetSymbol = null;
                    triggered = false;
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

        public bool GetTriggerValue()
        {
            if (globalVarLink != -1)
                return GameManager.Instance.PlayerEntity.GlobalVars.GetGlobalVar(globalVarLink);
            else
                return triggered;
        }

        public void SetTriggerValue(bool value)
        {
            if (globalVarLink != -1)
                GameManager.Instance.PlayerEntity.GlobalVars.SetGlobalVar(globalVarLink, value);
            else
                triggered = value;

            // If clearing a task then need to rearm actions
            if (value == false)
            {
                foreach (IQuestAction action in actions)
                {
                    action.RearmAction();
                }
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
            public string globalVarName;
            public int globalVarLink;
            public bool hasTriggerConditions;
            public ActionSaveData_v1[] actions;
        }

        [fsObject("v1")]
        public struct ActionSaveData_v1
        {
            public Type type;
            public bool complete;
            public bool triggerCondition;
            public bool alwaysOnTriggerCondition;
            public string debugSource;
            public object actionSpecific;
        }

        public TaskSaveData_v1 GetSaveData()
        {
            TaskSaveData_v1 data = new TaskSaveData_v1();
            data.symbol = symbol;
            data.targetSymbol = targetSymbol;
            data.triggered = triggered;
            data.prevTriggered = prevTriggered;
            data.type = type;
            data.globalVarName = globalVarName;
            data.globalVarLink = globalVarLink;
            data.hasTriggerConditions = hasTriggerConditions;

            List<ActionSaveData_v1> actionSaveDataList = new List<ActionSaveData_v1>();
            foreach(ActionTemplate action in actions)
            {
                ActionSaveData_v1 actionData = new ActionSaveData_v1();
                actionData.type = action.GetType();
                actionData.complete = action.IsComplete;
                actionData.triggerCondition = action.IsTriggerCondition;
                actionData.alwaysOnTriggerCondition = action.IsAlwaysOnTriggerCondition;
                actionData.debugSource = action.DebugSource;
                actionData.actionSpecific = action.GetSaveData();
                actionSaveDataList.Add(actionData);
            }
            data.actions = actionSaveDataList.ToArray();

            return data;
        }

        public void RestoreSaveData(TaskSaveData_v1 dataIn)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}