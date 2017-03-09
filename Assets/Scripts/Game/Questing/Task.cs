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
    public class Task : QuestResource
    {
        #region Fields

        string name;            // Unique name of task, can be used like a boolean if to check if task has completed
        string target;          // Name of target task/variable to check, used by repeating tasks only
        bool triggered;         // Has task been triggered?
        TaskType type;          // Type of task

        List<IQuestAction> actions = new List<IQuestAction>();

        #endregion

        #region Properties

        public string Name
        {
            get { return name; }
        }

        public TaskType Type
        {
            get { return type; }
        }

        public bool Triggered
        {
            set { triggered = value; }
            get { return triggered; }
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
            : base(parentQuest)
        {
        }

        /// <summary>
        /// Create a task from QBN source code.
        /// </summary>
        /// <param name="parentQuest">Parent quest.</param>
        /// <param name="lines">Source lines of task block.</param>
        public Task(Quest parentQuest, string[] lines)
            : base(parentQuest)
        {
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
                name = DaggerfallUnity.NextUID.ToString();
            }

            // Read task lines
            ReadTaskLines(lines, i);
        }

        public void Update()
        {
            // Update actions inside this task if triggered
            if (triggered)
            {
                foreach(IQuestAction action in actions)
                {
                    action.Update(this);
                }
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
                    name = match.Groups["symbol"].Value;
                }
                else if (!string.IsNullOrEmpty(match.Groups["repeating"].Value))
                {
                    // Repeating task
                    // These appear to be triggered automatically but need to confirm
                    type = TaskType.Repeating;
                    target = match.Groups["symbol"].Value;
                    triggered = false;
                    name = DaggerfallUnity.NextUID.ToString();
                }
                else if (!string.IsNullOrEmpty(match.Groups["variable"].Value))
                {
                    // Variable
                    type = TaskType.Variable;
                    target = string.Empty;
                    triggered = false;
                    name = match.Groups["symbol"].Value;
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
                if (IsCondition(lines[i]))
                {
                    // TODO: Create condition and add to task
                    Debug.LogFormat("Conditions not implemented. Ignoring: '{0}'", lines[i]);
                }
                else
                {
                    // Try to find registered action template using this line
                    IQuestAction actionTemplate = QuestMachine.Instance.GetActionTemplate(lines[i]);
                    if (actionTemplate != null)
                    {
                        // Create a new action from template (don't link template itself)
                        IQuestAction action = actionTemplate.Create(lines[i]);
                        actions.Add(action);
                    }
                    else
                    {
                        Debug.LogFormat("Action not found. Ignoring '{0}'", lines[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Quickly identify condition lines.
        /// </summary>
        /// <param name="line">Line to evaluate.</param>
        /// <returns>True if this is a condition.</returns>
        bool IsCondition(string line)
        {
            string matchStr = @"cast|clicked|daily|dropped|faction|from|have|injured|killed|level|pc at|repute|toting|(?<anItem>[a-zA-Z0-9_.]+) used do|(?<anItem>[a-zA-Z0-9_.]+) used saying|when";
            Match match = Regex.Match(line, matchStr);
            if (match.Success)
                return true;
            else
                return false;
        }

        #endregion
    }
}