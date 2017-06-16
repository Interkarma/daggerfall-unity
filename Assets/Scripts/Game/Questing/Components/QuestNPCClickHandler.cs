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
    /// Accepts clicks on quest-related NPCs and passes to quest system.
    /// This usually triggers a new Task in the quest itself.
    /// </summary>
    public class QuestNPCClickHandler : MonoBehaviour
    {
        public int IndividualFactionID { get; set; }            // Individual NPC - only use this for special individuals
        public ulong QuestUID { get; set; }                     // Quest owning this NPC
        public Symbol QuestPersonSymbol { get; set; }           // NPC symbol in quest system

        /// <summary>
        /// Called by PlayerActivate when clicking on this component.
        /// </summary>
        public void DoClick()
        {
            // Special individual NPCs can exist in world even if not actively part of a quest
            // This is different to random NPCs which are created as part of a single quest only
            if (IndividualFactionID != 0)
            {
                DoIndividualClick();
                return;
            }

            // Get the quest this Person belongs to
            Quest quest = QuestMachine.Instance.GetActiveQuest(QuestUID);
            if (quest == null)
                return;

            // Get the Person resource from quest and set click
            Person person = quest.GetPerson(QuestPersonSymbol);
            if (person == null)
                return;

            // Set this Person as clicked so quest can pick up event
            person.SetPlayerClicked();
        }

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
                    if (person.IsIndividualNPC && person.FactionData.id == IndividualFactionID)
                        person.SetPlayerClicked();
                }
            }
        }

        #endregion
    }
}