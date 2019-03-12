// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
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
using FullSerializer;

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
        bool isFoeDead = false;

        [NonSerialized] Quest targetQuest;
        [NonSerialized] QuestResource targetResource = null;
        [NonSerialized] DaggerfallEntityBehaviour enemyEntityBehaviour = null;

        #endregion

        #region Structures

        [fsObject("v1")]
        public struct QuestResourceSaveData_v1
        {
            public ulong questUID;
            public Symbol targetSymbol;
            public bool isFoeDead;
        }

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
        /// Flag stating if this Foe is dead .
        /// </summary>
        public bool IsFoeDead
        {
            get { return isFoeDead; }
        }

        /// <summary>
        /// Gets target Quest object. Can return null.
        /// </summary>
        public Quest TargetQuest
        {
            get { return targetQuest; }
        }

        /// <summary>
        /// Gets target QuestResource object. Can return null.
        /// </summary>
        public QuestResource TargetResource
        {
            get { return targetResource; }
        }

        /// <summary>
        /// Gets DaggerfallEntityBehaviour on enemy.
        /// Will be null if not an enemy quest resource.
        /// </summary>
        DaggerfallEntityBehaviour EnemyEntityBehaviour
        {
            get { return enemyEntityBehaviour; }
        }

        #endregion

        #region Unity

        private void Start()
        {
            // Cache target resource
            // This will fail if targetQuest and targetSymbol are not set before Start()
            if (!CacheTarget())
                return;
        }

        private void Update()
        {
            // Ensure target resource has this behaviour assigned
            // Coupling is otherwise lost when reloading a game
            if (targetResource != null)
            {
                if (!targetResource.QuestResourceBehaviour)
                    targetResource.QuestResourceBehaviour = this;
            }

            // Handle enemy checks
            if (enemyEntityBehaviour)
            {
                // Get foe resource
                Foe foe = (Foe)targetResource;
                if (foe == null)
                    return;

                // If foe is hidden then remove self from game
                if (foe.IsHidden)
                {
                    Destroy(gameObject);
                    return;
                }

                // Handle restrained check
                // This might need some tuning in relation to injured and death checks
                if (foe.IsRestrained)
                {
                    // Make enemy non-hostile
                    EnemyMotor enemyMotor = transform.GetComponent<EnemyMotor>();
                    if (enemyMotor)
                        enemyMotor.IsHostile = false;

                    // Lower flag now this has been handled
                    foe.ClearRestrained();
                }

                // Handle injured check
                // This has to happen before death or script actions attached to injured event will not trigger
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
        /// <returns>True if this resource was found in any active quests.</returns>
        public bool DoClick()
        {
            bool foundInActiveQuest = false;

            // Handle linked resource
            if (targetResource != null)
            {
                // Click resource
                targetResource.SetPlayerClicked();

                // If this an item then transfer item to player and hide resource
                if (targetResource is Item)
                    TransferWorldItemToPlayer();

                foundInActiveQuest = true;
            }

            // Possible for NPC to start a direct follow-up quest and new quest needs a bootstrap click
            // But if resource is still associated with old quest from previous layout then click never sent to new quest
            // So if behaviour is peered with an individual StaticNPC then send click to all quests using this NPC
            // This allows new quest to receive click and NPC will be re-linked on next layout or by "add NPC as questor"
            StaticNPC npc = GetComponent<StaticNPC>();
            if (npc)
            {
                int factionID = npc.Data.factionID;
                if (QuestMachine.Instance.IsIndividualNPC(factionID))
                    foundInActiveQuest = ClickAllIndividualNPCs(factionID);
            }

            return foundInActiveQuest;
        }

        /// <summary>
        /// Gets save data for serialization.
        /// </summary>
        public QuestResourceSaveData_v1 GetSaveData()
        {
            QuestResourceSaveData_v1 data = new QuestResourceSaveData_v1();
            data.questUID = questUID;
            data.targetSymbol = targetSymbol;
            data.isFoeDead = isFoeDead;

            return data;
        }

        /// <summary>
        /// Restores deserialized save data.
        /// Must be called after quest system state restored.
        /// </summary>
        public void RestoreSaveData(QuestResourceSaveData_v1 data)
        {
            questUID = data.questUID;
            targetSymbol = data.targetSymbol;
            isFoeDead = data.isFoeDead;
            CacheTarget();
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
            targetQuest = QuestMachine.Instance.GetQuest(questUID);
            if (targetQuest == null)
                return false;

            // Get the resource from quest
            targetResource = targetQuest.GetResource(targetSymbol);
            if (targetResource == null)
                return false;

            // Cache local EnemyEntity behaviour if resource is a Foe
            if (targetResource != null && targetResource is Foe)
                enemyEntityBehaviour = gameObject.GetComponent<DaggerfallEntityBehaviour>();

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

        bool ClickAllIndividualNPCs(int factionID)
        {
            // Check active quests to see if any are using this NPC
            ulong[] questIDs = QuestMachine.Instance.GetAllActiveQuests();
            bool matched = false;
            foreach (ulong questID in questIDs)
            {
                // Get quest object
                Quest quest = QuestMachine.Instance.GetQuest(questID);
                if (quest == null)
                    continue;

                // Get all the Person resources in this quest
                QuestResource[] personResources = quest.GetAllResources(typeof(Person));
                if (personResources == null || personResources.Length == 0)
                    continue;

                // Check each Person for a match
                foreach (QuestResource resource in personResources)
                {
                    // Set click if individual matches Person factionID
                    Person person = (Person)resource;
                    if (person.IsIndividualNPC && person.FactionData.id == factionID)
                    {
                        person.SetPlayerClicked();
                        matched = true;
                    }
                }
            }

            return matched;
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