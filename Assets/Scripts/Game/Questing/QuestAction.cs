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
    /// Interface to a quest action. 
    /// </summary>
    public interface IQuestAction
    {
        /// <summary>
        /// The Regex.Match string pattern expected from source input.
        /// Must be provided by action implementor.
        /// </summary>
        string Pattern { get; }

        /// <summary>
        /// Helper to test if source is a match for Pattern.
        /// </summary>
        Match Test(string source);

        /// <summary>
        /// Factory new instance of this action from source line.
        /// </summary>
        IQuestAction Create(string source, Quest parentQuest);

        /// <summary>
        /// Get action state data to serialize.
        /// </summary>
        object GetSaveData();

        /// <summary>
        /// Restore action state from serialized data.
        /// </summary>
        void RestoreSaveData(object dataIn);

        /// <summary>
        /// Update action activity.
        /// Called once per frame by owning task.
        /// </summary>
        void Update(Task caller);
    }

    /// <summary>
    /// Base class template for all quest actions.
    /// This class can be used to test and factory new action interfaces from itself.
    /// Actions belong to a task and perform a specific function when task is active and conditions are met.
    /// An action will persist until task terminates.
    /// Think of actions as objects with a lifetime rather than a simple unit of one-shot execution.
    /// For example, the "vengeance" sound played in nighttime Daggerfall is an action that persists until Lysandus is put to rest.
    /// Currently still unclear on when actions get reset (e.g. when does "play sound 10 times" counter get reset?).
    /// </summary>
    public abstract class ActionTemplate : QuestResource, IQuestAction
    {
        public abstract string Pattern { get; }
        public abstract IQuestAction Create(string source, Quest parentQuest);
        public abstract object GetSaveData();
        public abstract void RestoreSaveData(object dataIn);

        public ActionTemplate(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public virtual Match Test(string source)
        {
            return Regex.Match(source, Pattern);
        }

        public virtual void Update(Task caller)
        {
        }
    }
}