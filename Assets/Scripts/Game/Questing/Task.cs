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

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Tasks are executed by other tasks, clock timeouts, etc. Somewhat like a subroutine.
    /// Tasks can contain conditions at start that if not met appear to prevent further execution of task.
    /// Task name can also be used as a symbol to query if task has been completed or not.
    /// Provided conditions are met, commands under a task usually run once then end.
    /// Repeating tasks execute (i.e. each command stays alive) until target task/variable completed.
    /// Variables are a special task with no conditions or actions, just set/unset.
    /// </summary>
    public class Task
    {
        #region Fields

        string name;        // Unique name of task, can be used like a boolean if to check if task has completed
        string target;      // Name of target task/variable to check, used by repeating tasks only
        TaskType type;      // Type of task

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
        public Task()
        {
        }

        /// <summary>
        /// Create a task from QBN source code.
        /// </summary>
        /// <param name="lines">Source lines of task block.</param>
        public Task(string[] lines)
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
                type = TaskType.Headless;
                target = string.Empty;
                name = DaggerfallUnity.NextUID.ToString();
            }

            // TODO: Read task conditions and commands
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
                    type = TaskType.Repeating;
                    target = match.Groups["symbol"].Value;
                    name = DaggerfallUnity.NextUID.ToString();
                }
                else if (!string.IsNullOrEmpty(match.Groups["variable"].Value))
                {
                    // Variable
                    type = TaskType.Variable;
                    target = string.Empty;
                    name = match.Groups["symbol"].Value;
                }

                return true;
            }

            return false;
        }

        #endregion
    }
}