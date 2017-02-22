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
using System.Collections;
using System.Text.RegularExpressions;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Interface to a quest command.
    /// </summary>
    public interface IQuestCommand
    {
        /// <summary>
        /// The Regex.Match string pattern expected from source input.
        /// Must be provided by command implementor.
        /// </summary>
        string Pattern { get; }

        /// <summary>
        /// Create task and perform any setup required.
        /// Task must be registered to a Task to become active once created.
        /// </summary>
        bool Create(string source);

        /// <summary>
        /// Get command state data to serialize.
        /// </summary>
        object GetSaveData();

        /// <summary>
        /// Restore command state from serialized data.
        /// </summary>
        void RestoreSaveData(object dataIn);

        /// <summary>
        /// Update command activity.
        /// Called once per frame by owning task.
        /// </summary>
        void Update(Task owner);
    }

    /// <summary>
    /// Base class for all quest commands.
    /// Commands belong to a task and perform a specific function when task is active and conditions are met.
    /// A command will persist until task terminates, although some commands have individual limits (e.g. play sound 10 times).
    /// Think of commands as objects with a lifetime rather than a simple unit of one-shot execution.
    /// </summary>
    public abstract class BaseCommand : IQuestCommand
    {
        public abstract string Pattern { get; }
        public abstract bool Create(string source);
        public abstract object GetSaveData();
        public abstract void RestoreSaveData(object dataIn);

        /// <summary>
        /// Update command activity.
        /// </summary>
        /// <param name="owner">Task owner of this command.B</param>
        public virtual void Update(Task owner)
        {
        }
    }
}