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

        bool isFoeDead = false;

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

        /// <summary>
        /// Flag stating if this Foe is dead and quest system is done with any jobs it needs to perform.
        /// </summary>
        public bool IsDead
        {
            get { return isFoeDead; }
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
                enemyEntityBehaviour = gameObject.GetComponent<DaggerfallEntityBehaviour>();
        }

        private void Update()
        {
            // Handle enemy checks
            if (enemyEntityBehaviour)
            {
                // Handle injured check
                // This has to happen before death or script actions attached to injured event will not trigger
                Foe foe = (Foe)targetResource;
                if (enemyEntityBehaviour.Entity.CurrentHealth < enemyEntityBehaviour.Entity.MaxHealth && !foe.InjuredTrigger)
                {
                    foe.SetInjured();
                    return;
                }

                // Handle death check
                if (enemyEntityBehaviour.Entity.CurrentHealth <= 0 && !isFoeDead)
                {
                    foe.IncrementKills();
                    isFoeDead = true;
                }
            }
        }

        private void OnDestroy()
        {
            RaiseOnGameObjectDestroyEvent();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Assign this behaviour a QuestResource object.
        /// </summary>
        public void AssignResource(QuestResource questResource)
        {
            if (questResource != null)
            {
                questUID = questResource.ParentQuest.UID;
                targetSymbol = questResource.Symbol;
            }
        }

        /// <summary>
        /// Called by PlayerActivate when clicking on this GameObject.
        /// </summary>
        public void DoClick()
        {
            // Set click on resource
            if (targetResource != null)
            {
                // Set the click on resource
                targetResource.SetPlayerClicked();

                // Give item to player and hide resource
                if (targetResource is Item)
                    TransferWorldItemToPlayer();
            }
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

        void TransferWorldItemToPlayer()
        {
            // Give item to player
            Item item = (Item)targetResource;
            GameManager.Instance.PlayerEntity.Items.AddItem(item.DaggerfallUnityItem);

            // Hide item so player cannot pickup again
            // This will cause it not to display in world again despite being placed by SiteLink
            item.IsHidden = true;
        }

        #endregion

        #region Events

        public delegate void OnGameObjectDestroyHandler(QuestResourceBehaviour questResourceBehaviour);
        public event OnGameObjectDestroyHandler OnGameObjectDestroy;
        protected void RaiseOnGameObjectDestroyEvent()
        {
            if (OnGameObjectDestroy != null)
                OnGameObjectDestroy(this);
        }

        #endregion
    }
}