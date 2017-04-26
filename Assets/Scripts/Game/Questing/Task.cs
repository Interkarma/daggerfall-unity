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

        Symbol symbol;          // Unique symbol of task, can be used like a boolean if to check if task has completed
        string target;          // Name of target task/variable to check, used by repeating tasks only
        bool triggered;         // Has task been triggered?
        TaskType type;          // Type of task

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

        public string Target
        {
            get { return target; }
        }

        public TaskType Type
        {
            get { return type; }
        }

        public bool IsSet
        {
            set { triggered = value; }
            get { return triggered; }
        }

        public bool HasTriggerConditions
        {
            get { return hasTriggerConditions; }
        }

        #endregion

        #region Enumerations

        public enum TaskType
        {
            Headless,
            Standard,
            Repeating,
            Variable,
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

        #endregion

        #region Public Methods

        public void SetTask(string[] lines)
        {
            int i = 0;
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
                target = string.Empty;
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
                // Skip completed
                if (action.IsComplete)
                    continue;

                // Check trigger conditions on inactive tasks
                if (!triggered && action.IsTriggerCondition)
                {
                    if (action.CheckCondition(this))
                        triggered = true;
                }

                // Tick actions when active
                if (triggered && !action.IsTriggerCondition)
                {
                    action.Update(this);
                }
            }
        }

        /// <summary>
        /// Another way to set/trigger task.
        /// </summary>
        public void Set()
        {
            triggered = true;
        }

        /// <summary>
        /// Another way to unset/rearm task.
        /// </summary>
        public void Unset()
        {
            triggered = false;
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

        #region Private Method

        /// <summary>
        /// Reads task header based on supported task types.
        /// </summary>
        /// <param name="line">Header source line.</param>
        /// <returns>True if task header match found.</returns>
        bool ReadTaskHeader(string line)
        {
            // Try to match task types
            Match match = Regex.Match(line, @"(?<symbol>[a-zA-Z0-9_.]+) (?<task>task):|until (?<symbol>[a-zA-Z0-9_.]+) (?<repeating>performed:)|(?<variable>variable) (?<symbol>[a-zA-Z0-9_.]+)");
            if (match.Success)
            {
                if (!string.IsNullOrEmpty(match.Groups["task"].Value))
                {
                    // Standard task
                    type = TaskType.Standard;
                    target = string.Empty;
                    symbol = new Symbol(match.Groups["symbol"].Value);
                }
                else if (!string.IsNullOrEmpty(match.Groups["repeating"].Value))
                {
                    // Repeating task
                    // These appear to be triggered automatically but need to confirm
                    type = TaskType.Repeating;
                    target = match.Groups["symbol"].Value;
                    triggered = false;
                    symbol = new Symbol(DaggerfallUnity.NextUID.ToString());
                }
                else if (!string.IsNullOrEmpty(match.Groups["variable"].Value))
                {
                    // Variable
                    type = TaskType.Variable;
                    target = string.Empty;
                    triggered = false;
                    symbol = new Symbol(match.Groups["symbol"].Value);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Reads task lines to find conditions and actions.
        /// Must be 
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
                    IQuestAction action = actionTemplate.Create(lines[i], ParentQuest);
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

        #endregion
    }
}