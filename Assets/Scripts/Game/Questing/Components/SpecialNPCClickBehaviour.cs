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
    /// Handles clicks for special named NPCs like King Gothryd and Queen Aubk-i.
    /// These NPCs exist in world whether they are part of a quest or not.
    /// They can even be involved in multiple quests at the same time.
    /// When in their home location, these NPCs are placed by scene layout builders
    /// rather than quest system, and no SiteLink reservation will exist for them.
    /// This component bridges the gap and sends click events on this NPC back to quest system.
    /// 
    /// Notes:
    ///  * This behaviour is always added to special NPCs by scene layout.
    ///  * Currently no two-way communication exists from Person resource back to special NPC.
    ///  * This should not be an issue as these NPCs are not very dynamic.
    ///  * Not required to serialize state at this time.
    /// </summary>
    public class SpecialNPCClickHandler : MonoBehaviour
    {
        public int IndividualFactionID { get; set; }

        /// <summary>
        /// Checks all active quests for references to this special person and triggers player click across all quests.
        /// These special people may be involved in multiple quests which is different to singleton NPCs.
        /// </summary>
        public void DoClick()
        {
            if (IndividualFactionID == 0)
                return;

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
    }
}