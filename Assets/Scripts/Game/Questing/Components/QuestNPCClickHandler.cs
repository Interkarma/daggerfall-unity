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
        public ulong QuestUID { get; set; }                     // Quest owning this NPC
        public Symbol QuestPersonSymbol { get; set; }           // NPC symbol in quest system

        /// <summary>
        /// Called by PlayerActivate when clicking on this component.
        /// </summary>
        public void DoClick()
        {
            // Get the quest this Person belongs to
            Quest quest = QuestMachine.Instance.GetActiveQuest(QuestUID);
            if (quest == null)
                throw new Exception("DoClick() QuestUID references a quest that could not be found.");

            // Get the Person resource from quest and set click
            Person person = quest.GetPerson(QuestPersonSymbol);
            person.SetPlayerClicked();
        }
    }
}