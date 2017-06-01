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
using System;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// Condition which checks if player character at a specific place.
    /// </summary>
    public class PcAt : ActionTemplate
    {
        Symbol placeSymbol;
        Symbol taskSymbol;
        int textId;

        public override string Pattern
        {
            // Notes:
            // Docs use form "pc at aPlace do aTask"
            // But observed quests actually seem to use "pc at aPlace set aTask"
            // Probably a change between writing of docs and Template v1.11.
            // Docs also missing ""pc at aPlace set aTask saying nnnn"
            get { return @"pc at (?<aPlace>\w+) set (?<aTask>[a-zA-Z0-9_.]+) saying (?<id>\d+)|pc at (?<aPlace>\w+) set (?<aTask>[a-zA-Z0-9_.]+)"; }
        }

        public PcAt(Quest parentQuest)
            : base(parentQuest)
        {
            IsTriggerCondition = true;
        }

        public override IQuestAction Create(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new pcat
            PcAt pcat = new PcAt(parentQuest);
            pcat.placeSymbol = new Symbol(match.Groups["aPlace"].Value);
            pcat.taskSymbol = new Symbol(match.Groups["aTask"].Value);
            pcat.textId = Parser.ParseInt(match.Groups["id"].Value);

            return pcat;
        }

        // TODO:
        // Need to check condition at least once for pre-started tasks (e.g. startup task).
        // For example, _BRISIEN calls 'pc at PiratesHold set _exitstarter_' at startup to set _exitstarter_ flag if player starts in Privateer's Hold.
        // This currently won't run because quest machine considers task running already and doesn't evaluate trigger condition.
        // Will redesign conditional checks in near future.
        public override bool CheckCondition(Task caller)
        {
            bool result = false;

            // Get place resource
            Place place = ParentQuest.GetPlace(placeSymbol);
            if (place == null)
                return false;

            // Check building site
            if (place.SiteDetails.isBuilding)
                result = CheckInsideBuilding(place);

            // TODO: Check other place types as they are developed

            // Handle positive check
            // TODO: Which should happen first, the "saying" popup or task execution?
            // With the current execution flow, any task "say" actions will happen before "pc at saying".
            // Popups also trigger before scene transition fade.
            // These fades are nice, but not of classic anyway - might just need to remove.
            if (result)
            {
                // Trigger target task
                ParentQuest.SetTask(taskSymbol);

                // "saying" popup
                if (textId != 0)
                    ShowPopup(textId);
            }

            return result;
        }

        public override void Update(Task caller)
        {
        }

        #region Private Methods

        /// <summary>
        /// Check if player inside a specific target site building.
        /// </summary>
        bool CheckInsideBuilding(Place place)
        {
            // Get component handling player world status and transitions
            PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;
            if (!playerEnterExit)
                return false;

            // Check if player inside the building matching this site
            if (playerEnterExit.IsPlayerInside && playerEnterExit.IsPlayerInsideBuilding)
            {
                // Must have at least one exterior door for building check
                StaticDoor[] exteriorDoors = playerEnterExit.ExteriorDoors;
                if (exteriorDoors == null || exteriorDoors.Length < 1)
                {
                    throw new Exception("CheckInsideBuilding() could not get at least 1 exterior door from playerEnterExit.ExteriorDoors.");
                }

                // Check if building IDs match both site and any exterior door of this building
                if (exteriorDoors[0].buildingKey == place.SiteDetails.buildingKey)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}