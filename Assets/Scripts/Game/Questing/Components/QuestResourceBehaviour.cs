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
        int individualFactionID;
        Quest targetQuest;
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
        /// Gets individual FactionID when this is a unique named individual.
        /// </summary>
        public int IndividualFactionID
        {
            get { return individualFactionID; }
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
        /// Mutually exclusive with AssignIndividualNPC().
        /// </summary>
        public void AssignResource(QuestResource questResource)
        {
            if (questResource != null)
            {
                questUID = questResource.ParentQuest.UID;
                targetSymbol = questResource.Symbol;
                individualFactionID = 0;
            }
        }

        /// <summary>
        /// Assign this behaviour an individual NPC.
        /// Mutually exclusive with AssignResource().
        /// </summary>
        /// <param name="factionID"></param>
        public void AssignIndividualNPC(int factionID)
        {
            questUID = 0;
            targetSymbol = null;
            individualFactionID = factionID;
        }

        /// <summary>
        /// Check target quest and resource can be resolved.
        /// If true then TargetQuest and TargetResource objects are cached and available.
        /// </summary>
        public bool CheckTarget()
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
        /// Called by PlayerActivate when clicking on this GameObject.
        /// </summary>
        public void DoClick()
        {
            // Special individual NPCs can exist in world even if not actively part of a quest
            // This is different to random NPCs which are created as part of a single quest only
            // Check all active quests on this individual NPC
            if (individualFactionID != 0)
            {
                DoIndividualClick();
                return;
            }

            // Set click on resource
            if (CheckTarget())
                targetResource.SetPlayerClicked();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks all active quests for references to this special person and triggers player click across all quests.
        /// These special people may be involved in multiple quests, which is different to singleton NPCs created
        /// specifically for a single quest.
        /// </summary>
        void DoIndividualClick()
        {
            // Check active quests to see if anyone has reserved this NPC
            ulong[] questIDs = QuestMachine.Instance.GetAllActiveQuests();
            foreach (ulong questID in questIDs)
            {
                // Get quest object
                Quest quest = QuestMachine.Instance.GetActiveQuest(questID);
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
                    if (person.IsIndividualNPC && person.FactionData.id == individualFactionID)
                        person.SetPlayerClicked();
                }
            }
        }

        #endregion
    }
}