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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Helper component to pass information between GameObjects and Quest system.
    /// Used to trigger resource events in quest systems like ClickedNpc.
    /// </summary>
    public class QuestResourceBehaviour : MonoBehaviour
    {
        #region Fields

        ulong questUID;
        Symbol targetSymbol;
        Quest targetQuest;

        [NonSerialized]
        QuestResource targetResource;

        #endregion

        #region Properties

        /// <summary>
        /// Gets assigned Quest UID.
        /// </summary>
        public ulong QuestUID
        {
            get { return questUID; }
        }

        /// <summary>
        /// Gets assigned target Symbol.
        /// </summary>
        public Symbol TargetSymbol
        {
            get { return targetSymbol; }
        }

        /// <summary>
        /// Gets target Quest object. Can return null.
        /// </summary>
        public Quest TargetQuest
        {
            get { return (CheckTarget()) ? targetQuest : null; }
        }

        /// <summary>
        /// Get target QuestResource object. Can return null.
        /// </summary>
        public QuestResource TargetResource
        {
            get { return (CheckTarget()) ? targetResource : null; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Assign this behaviour a QuestResource object.
        /// </summary>
        public void AssignResource(QuestResource questResource)
        {
            UnsubscribeEvents();
            if (questResource != null)
            {
                questUID = questResource.ParentQuest.UID;
                targetSymbol = questResource.Symbol;
            }
            SubscribeEvents();
        }

        /// <summary>
        /// Called by PlayerActivate when clicking on this GameObject.
        /// </summary>
        public void DoClick()
        {
            // Set click on resource
            if (CheckTarget())
                targetResource.SetPlayerClicked();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Check target quest and resource can be resolved.
        /// If true then TargetQuest and TargetResource objects are cached and available.
        /// </summary>
        bool CheckTarget()
        {
            if (targetQuest != null && targetResource != null)
                return true;

            // Must have a questUID and targetSymbol
            if (questUID == 0 || targetSymbol == null)
                return false;

            // Get the quest this resource belongs to
            targetQuest = QuestMachine.Instance.GetActiveQuest(questUID);
            if (targetQuest == null)
                return false;

            // Get the resource from quest
            targetResource = targetQuest.GetResource(targetSymbol);
            if (targetResource == null)
                return false;

            return true;
        }

        /// <summary>
        /// Subscribe to events raised by the target resource.
        /// </summary>
        void SubscribeEvents()
        {
            if (!CheckTarget())
                return;
        }

        /// <summary>
        /// Unsubscribe from events raised by the target resource.
        /// </summary>
        void UnsubscribeEvents()
        {
            if (!CheckTarget())
                return;
        }

        #endregion
    }
}