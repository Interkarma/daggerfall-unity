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
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Helper behaviour to pass information between GameObjects and Quest system.
    /// Used to trigger resource events in quest systems like ClickedNpc, InjuredFoe, KilledFoe, etc.
    /// </summary>
    public class QuestResourceBehaviour : MonoBehaviour
    {
        #region Fields

        ulong questUID;
        Symbol targetSymbol;
        Quest targetQuest;

        [NonSerialized] QuestResource targetResource = null;
        [NonSerialized] DaggerfallEntityBehaviour enemyEntityBehaviour = null;

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
            get { return targetQuest; }
        }

        /// <summary>
        /// Get target QuestResource object. Can return null.
        /// </summary>
        public QuestResource TargetResource
        {
            get { return targetResource; }
        }

        #endregion

        #region Unity

        private void Start()
        {
            // Cache target resource
            // This will fail if targetQuest and targetSymbol are not set before Start()
            if (!CacheTarget())
                return;

            // Cache local EnemyEntity behaviour if resource is a Foe
            if (targetResource != null && targetResource is Foe)
            {
                enemyEntityBehaviour = gameObject.GetComponent<DaggerfallEntityBehaviour>();
                enemyEntityBehaviour.Entity.OnDeath += Enemy_OnDeath;
            }
        }

        private void Update()
        {
            // Handle enemy checks
            if (enemyEntityBehaviour)
            {
                Foe foe = (Foe)targetResource;
                if (enemyEntityBehaviour.Entity.CurrentHealth < enemyEntityBehaviour.Entity.MaxHealth && !foe.InjuredTrigger)
                {
                    foe.SetInjured();
                }
            }
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
            if (targetResource != null)
                targetResource.SetPlayerClicked();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Cache target quest and resource objects.
        /// If true then TargetQuest and TargetResource objects are cached and available.
        /// </summary>
        bool CacheTarget()
        {
            // Check already cached
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
        }

        /// <summary>
        /// Unsubscribe from events raised by the target resource.
        /// </summary>
        void UnsubscribeEvents()
        {
        }

        #endregion

        #region Event Handlers

        private void Enemy_OnDeath(DaggerfallEntity entity)
        {
            if (targetResource != null)
            {
                Foe foe = (Foe)targetResource;
                foe.IncrementKills();
            }
        }

        #endregion
    }
}