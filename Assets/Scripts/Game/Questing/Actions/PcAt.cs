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
using DaggerfallConnect;

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
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            base.CreateNew(source, parentQuest);

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

        /// <summary>
        /// Continuously checks where player is and sets target true/false based on site properties.
        /// </summary>
        public override void Update(Task caller)
        {
            bool result = false;

            // Get place resource
            Place place = ParentQuest.GetPlace(placeSymbol);
            if (place == null)
                return;

            // Check building site
            if (place.SiteDetails.siteType == SiteTypes.Building)
                result = CheckInsideBuilding(place);
            else if (place.SiteDetails.siteType == SiteTypes.Town)
                result = CheckInsideTown(place);
            else if (place.SiteDetails.siteType == SiteTypes.Dungeon)
                result = CheckInsideDungeon(place);

            // Handle positive check
            if (result)
            {
                // "saying" popup
                // TODO: Should this run every time or only once?
                if (textId != 0)
                    ParentQuest.ShowMessagePopup(textId);

                // Enable target task
                ParentQuest.SetTask(taskSymbol);
            }
            else
            {
                // Disable target task
                ParentQuest.UnsetTask(taskSymbol);
            }
        }

        #region Private Methods

        bool CheckInsideDungeon(Place place)
        {
            // Get component handling player world status and transitions
            PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;
            if (!playerEnterExit)
                return false;

            // Player must be inside a dungeon
            if (!playerEnterExit.IsPlayerInsideDungeon)
                return false;

            // Compare mapId of site and current location
            DFLocation location = GameManager.Instance.PlayerGPS.CurrentLocation;
            if (location.Loaded)
            {
                if (location.MapTableData.MapId == place.SiteDetails.mapId)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Check if player at specific town exterior.
        /// This includes the exterior RMB area of dungeons in world.
        /// </summary>
        bool CheckInsideTown(Place place)
        {
            // Get component handling player world status and transitions
            PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;
            if (!playerEnterExit)
                return false;

            // Player must be outside
            if (playerEnterExit.IsPlayerInside)
                return false;

            // Compare mapId of site and current location
            DFLocation location = GameManager.Instance.PlayerGPS.CurrentLocation;
            if (location.Loaded && GameManager.Instance.PlayerGPS.IsPlayerInLocationRect)
            {
                if (location.MapTableData.MapId == place.SiteDetails.mapId)
                    return true;
            }

            return false;
        }

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