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
using System.Collections;
using System.Text.RegularExpressions;
using FullSerializer;

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
        /// Returns true if action called SetComplete().
        /// </summary>
        bool IsComplete { get; }

        /// <summary>
        /// Returns true if this action considers itself a trigger condition.
        ///  * Trigger will be checked even on inactive tasks
        ///  * When trigger evaluates true the task will become active
        /// </summary>
        bool IsTriggerCondition { get; }

        /// <summary>
        /// Returns true if this trigger is always on and should be checked each tick.
        /// </summary>
        bool IsAlwaysOnTriggerCondition { get; }

        /// <summary>
        /// Gets or sets source code for debugger.
        /// </summary>
        string DebugSource { get; set; }

        /// <summary>
        /// Helper to test if source is a match for Pattern.
        /// </summary>
        Match Test(string source);

        /// <summary>
        /// Called by parent task any time it is set/rearmed.
        /// This enables the action to reset state if needed.
        /// </summary>
        void InitialiseOnSet();

        /// <summary>
        /// Factory new instance of this action from source line.
        /// Overrides should always call base to set debug source line.
        /// </summary>
        IQuestAction CreateNew(string source, Quest parentQuest);

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

        /// <summary>
        /// Check trigger condition status.
        /// Allows task to become active when condition returns true.
        /// </summary>
        bool CheckTrigger(Task caller);

        /// <summary>
        /// Sets action as complete so as not to be called again by task.
        /// Used for one-and-done actions.
        /// </summary>
        void SetComplete();

        /// <summary>
        /// Clears action complete flag.
        /// Implementor should override this is if special handling needed on rearm.
        /// </summary>
        void RearmAction();

        /// <summary>
        /// Called to dispose action when quest ends.
        /// </summary>
        void Dispose();
    }

    /// <summary>
    /// Base class template for all quest actions and conditions used by tasks.
    /// Handles some of the boilerplate of IQuestAction.
    /// This class can be used to test and factory new action interfaces from itself.
    /// Actions belong to a task and perform a specific function when task is active and conditions are met.
    /// An action will persist until task terminates.
    /// Think of actions as objects with a lifetime rather than a simple unit of one-shot execution.
    /// For example, the "vengeance" sound played in nighttime Daggerfall is an action that persists until Lysandus is put to rest.
    /// Currently still unclear on when actions get reset (e.g. when does "play sound 10 times" counter get reset?).
    /// </summary>
    public abstract class ActionTemplate : QuestResource, IQuestAction
    {
        bool isComplete = false;
        bool isTriggerCondition = false;
        bool isAlwaysOnTriggerCondition = false;
        string debugSource;
        protected bool allowRearm = true;

        public bool IsComplete { get { return isComplete; } set { isComplete = value; } }
        public bool IsTriggerCondition { get { return isTriggerCondition; } set { isTriggerCondition = value; } }
        public bool IsAlwaysOnTriggerCondition {  get { return isAlwaysOnTriggerCondition; } set { isAlwaysOnTriggerCondition = value; } }
        public string DebugSource { get { return debugSource; } set { debugSource = value; } }

        public abstract string Pattern { get; }

        public ActionTemplate(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public virtual Match Test(string source)
        {
            return Regex.Match(source, Pattern);
        }

        public virtual void InitialiseOnSet()
        {
        }

        public virtual IQuestAction CreateNew(string source, Quest parentQuest)
        {
            return this;
        }

        public virtual void Update(Task caller)
        {
        }

        public virtual bool CheckTrigger(Task caller)
        {
            return false;
        }

        public virtual void SetComplete()
        {
            isComplete = true;
        }

        public virtual void RearmAction()
        {
            if (isComplete && allowRearm)
                isComplete = false;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        #region Serialization

        [fsObject("v1")]
        public struct ActionSaveData_v1
        {
            public Type type;
            public bool isComplete;
            public bool isTriggerCondition;
            public bool isAlwaysOnTriggerCondition;
            public string debugSource;
            public object actionSpecific;
        }

        /// <summary>
        /// Get full action save data including action specific data.
        /// </summary>
        public ActionSaveData_v1 GetActionSaveData()
        {
            ActionSaveData_v1 actionData = new ActionSaveData_v1();
            actionData.type = GetType();
            actionData.isComplete = isComplete;
            actionData.isTriggerCondition = isTriggerCondition;
            actionData.isAlwaysOnTriggerCondition = isAlwaysOnTriggerCondition;
            actionData.debugSource = debugSource;
            actionData.actionSpecific = GetSaveData();

            return actionData;
        }

        /// <summary>
        /// Restore full action save data including action specific data.
        /// </summary>
        public void RestoreActionSaveData(ActionSaveData_v1 data)
        {
            isComplete = data.isComplete;
            isTriggerCondition = data.isTriggerCondition;
            isAlwaysOnTriggerCondition = data.isAlwaysOnTriggerCondition;
            debugSource = data.debugSource;
            RestoreSaveData(data.actionSpecific);
        }

        #endregion
    }
}