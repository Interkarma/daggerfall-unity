// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using UnityEngine;
using System.Text.RegularExpressions;
using FullSerializer;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Partial implementation.
    /// Teleport player to a dungeon for dungeon traps, or as part of main quest.
    /// Does not exactly emulate classic for "transfer pc inside" variant. This is only used in Sx016.
    /// </summary>
    public class TeleportPc : ActionTemplate
    {
        Symbol targetPlace;
        int targetMarker = -1;

        bool resumePending = false;
        Vector3 resumePosition = Vector3.zero;

        public override string Pattern
        {
            get { return @"teleport pc to (?<aPlace>[a-zA-Z0-9_.-]+)|" +
                         @"transfer pc inside (?<aPlace>[a-zA-Z0-9_.-]+) marker (?<marker>\d+)"; }
        }

        public TeleportPc(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            TeleportPc action = new TeleportPc(parentQuest);
            action.targetPlace = new Symbol(match.Groups["aPlace"].Value);
            if (match.Groups["marker"].Success)
                action.targetMarker = Parser.ParseInt(match.Groups["marker"].Value);

            return action;
        }

        public override void RearmAction()
        {
            base.RearmAction();

            // If the action is disabled then rearmed, then teleport called before resumePending is finished, then player can desync from world
            // Lower the resumePending flag as this will be a new instance of teleport
            resumePending = false;
        }

        public override void Update(Task caller)
        {
            base.Update(caller);

            // Do nothing while player respawning
            if (GameManager.Instance.PlayerEnterExit.IsRespawning)
                return;

            // Handle resume on next tick of action after respawn process complete
            if (resumePending)
            {
                GameObject player = GameManager.Instance.PlayerObject;
                player.transform.position = resumePosition;
                resumePending = false;
                SetComplete();
                return;
            }

            // Create SiteLink if not already present
            if (!QuestMachine.HasSiteLink(ParentQuest, targetPlace))
                QuestMachine.CreateSiteLink(ParentQuest, targetPlace);

            // Attempt to get Place resource
            Place place = ParentQuest.GetPlace(targetPlace);
            if (place == null)
                return;

            // Get selected spawn QuestMarker for this Place
            bool usingMarker = false;
            QuestMarker marker = new QuestMarker();
            if (targetMarker >= 0 && targetMarker < place.SiteDetails.questSpawnMarkers.Length)
            {
                marker = place.SiteDetails.questSpawnMarkers[targetMarker];
                usingMarker = true;
            }

            // Attempt to get location data - using GetLocation(regionName, locationName) as it can support all locations
            DFLocation location;
            if (!DaggerfallUnity.Instance.ContentReader.GetLocation(place.SiteDetails.regionName, place.SiteDetails.locationName, out location))
                return;

            // Spawn inside dungeon at this world position
            DFPosition mapPixel = MapsFile.LongitudeLatitudeToMapPixel((int)location.MapTableData.Longitude, location.MapTableData.Latitude);
            DFPosition worldPos = MapsFile.MapPixelToWorldCoord(mapPixel.X, mapPixel.Y);
            GameManager.Instance.PlayerEnterExit.RespawnPlayer(
                worldPos.X,
                worldPos.Y,
                true,
                true);

            // Determine start position
            if (usingMarker)
            {
                // Use specified quest marker
                Vector3 dungeonBlockPosition = new Vector3(marker.dungeonX * RDBLayout.RDBSide, 0, marker.dungeonZ * RDBLayout.RDBSide);
                resumePosition = dungeonBlockPosition + marker.flatPosition;
            }
            else
            {
                // Use first quest marker
                marker = place.SiteDetails.questSpawnMarkers[0];
                Vector3 dungeonBlockPosition = new Vector3(marker.dungeonX * RDBLayout.RDBSide, 0, marker.dungeonZ * RDBLayout.RDBSide);
                resumePosition = dungeonBlockPosition + marker.flatPosition;
            }

            resumePending = true;
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol targetPlace;
            public int targetMarker;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.targetPlace = targetPlace;
            data.targetMarker = targetMarker;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            targetPlace = data.targetPlace;
            targetMarker = data.targetMarker;
        }

        #endregion
    }
}