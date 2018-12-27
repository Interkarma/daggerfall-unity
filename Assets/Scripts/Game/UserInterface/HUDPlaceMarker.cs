// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallConnect;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// A debugging class to hack a local quest site marker onto HUD.
    /// Not intended for gameplay - only used for bootstrapping site location in quest system.
    /// </summary>
    public class HUDPlaceMarker : Panel
    {
        List<SiteTarget> siteTargets = new List<SiteTarget>();
        int lastSiteLinkCount = 0;
        Vector3 worldCompensation = Vector3.zero;

        struct SiteTarget
        {
            public Vector3 doorPosition;
            public TextLabel markerLabel;
            public string targetName;
        }

        public HUDPlaceMarker()
            : base()
        {
            // Events used to update site list
            QuestMachine.OnQuestStarted += QuestMachine_OnQuestStarted;
            QuestMachine.OnQuestEnded += QuestMachine_OnQuestEnded;
            PlayerGPS.OnEnterLocationRect += PlayerGPS_OnEnterLocationRect;
            PlayerGPS.OnExitLocationRect += PlayerGPS_OnExitLocationRect;
            PlayerGPS.OnMapPixelChanged += PlayerGPS_OnMapPixelChanged;
            StreamingWorld.OnFloatingOriginChange += StreamingWorld_OnFloatingOriginChange;
        }

        public override void Update()
        {
            base.Update();

            // Refresh site targets when SiteLink count changes
            if (lastSiteLinkCount != QuestMachine.Instance.SiteLinkCount)
            {
                RefreshSiteTargets();
            }

            // Disable markers if inside or no targets, or quest debugger hidden
            bool enableMarkers = true;
            if (GameManager.Instance.PlayerEnterExit.IsPlayerInside || siteTargets.Count == 0 ||
                DaggerfallUI.Instance.DaggerfallHUD.QuestDebugger.State == HUDQuestDebugger.DisplayState.Nothing)
            {
                enableMarkers = false;
            }

            // Set marker label position and text
            Rect rect = Rectangle;
            Camera mainCamera = GameManager.Instance.MainCamera;
            for (int i = 0; i < siteTargets.Count; i++)
            {
                siteTargets[i].markerLabel.Enabled = enableMarkers;

                Vector3 screenPos = mainCamera.WorldToScreenPoint(siteTargets[i].doorPosition);
                if (screenPos.z < 0)
                    siteTargets[i].markerLabel.Enabled = false;

                Vector2 panelPos = ScreenToLocal(new Vector2(screenPos.x, rect.height - screenPos.y));
                siteTargets[i].markerLabel.Position = panelPos;
                siteTargets[i].markerLabel.Text = string.Format("{0} {1}", siteTargets[i].targetName, screenPos.z.ToString("[0]"));
            }
        }

        public void ClearSiteTargets()
        {
            for (int i = 0; i < siteTargets.Count; i++)
            {
                Components.Remove(siteTargets[i].markerLabel);
                siteTargets[i].markerLabel.Dispose();
            }
            siteTargets.Clear();
        }

        #region Private Methods

        /// <summary>
        /// Rebuild door marker positions for active quest sites in current player location only.
        /// Quite inefficient but this class is only intended for early quest testing.
        /// This method should only be run when player enters/exits a location rect or when a new quest begins.
        /// </summary>
        void RefreshSiteTargets()
        {
            // Clear existing sites
            ClearSiteTargets();

            // Get player's location component in scene
            DaggerfallLocation dfLocation = GameManager.Instance.StreamingWorld.CurrentPlayerLocationObject;
            if (!dfLocation)
                return;

            // Get all active quest sites
            SiteDetails[] allSites = QuestMachine.Instance.GetAllActiveQuestSites();
            if (allSites == null || allSites.Length == 0)
                return;

            // Get all door collections in this location
            DaggerfallStaticDoors[] allDoors = dfLocation.gameObject.GetComponentsInChildren<DaggerfallStaticDoors>();
            if (allDoors == null || allDoors.Length == 0)
                return;

            // Enumerate sites and look for entrance doors
            int foundTotal = 0;
            foreach (SiteDetails site in allSites)
            {
                // Only interested in buildings within player's current location
                if (site.siteType != SiteTypes.Building || site.mapId != dfLocation.Summary.MapID)
                    continue;

                // Get site layout coords
                int siteLayoutX, siteLayoutY, siteRecordIndex;
                BuildingDirectory.ReverseBuildingKey(site.buildingKey, out siteLayoutX, out siteLayoutY, out siteRecordIndex);

                // Build a list of all doors matching site building index
                foreach (DaggerfallStaticDoors dfStaticDoors in allDoors)
                {
                    foreach (StaticDoor door in dfStaticDoors.Doors)
                    {
                        // Get building layout coords from door
                        int doorLayoutX, doorLayoutY, doorRecordIndex;
                        BuildingDirectory.ReverseBuildingKey(door.buildingKey, out doorLayoutX, out doorLayoutY, out doorRecordIndex);

                        // Reject door collections belonging to a different block than site
                        if (doorLayoutX != siteLayoutX || doorLayoutY != siteLayoutY)
                            break;

                        // Match building keys
                        if (door.buildingKey == site.buildingKey)
                        {
                            foundTotal++;
                            Vector3 position = door.buildingMatrix.MultiplyPoint3x4(door.centre) + dfStaticDoors.transform.position + worldCompensation;

                            SiteTarget target = new SiteTarget();
                            target.doorPosition = position;
                            target.markerLabel = new TextLabel();
                            target.targetName = site.buildingName;
                            Components.Add(target.markerLabel);
                            siteTargets.Add(target);
                        }
                    }
                }
            }

            // Output how many doors were found in thise location and get out if zero
            //Debug.LogFormat("Found {0} doors matching an active quest site in location {1}/{2}.", foundTotal, dfLocation.Summary.RegionName, dfLocation.Summary.LocationName);
            if (foundTotal == 0)
            {
                ClearSiteTargets();
                return;
            }

            // Update SiteLink count
            lastSiteLinkCount = QuestMachine.Instance.SiteLinkCount;

            worldCompensation = Vector3.zero;
        }

        #endregion

        #region Event Handlers

        private void PlayerGPS_OnEnterLocationRect(DaggerfallConnect.DFLocation location)
        {
            // Refresh when entering a location rect
            RefreshSiteTargets();
        }

        private void QuestMachine_OnQuestStarted(Quest quest)
        {
            // Refresh when starting a new quest
            RefreshSiteTargets();
        }

        private void StreamingWorld_OnFloatingOriginChange()
        {
            // Refresh when world compensation changes or markers may appear floating in air
            worldCompensation = GameManager.Instance.StreamingWorld.WorldCompensation;
            RefreshSiteTargets();
        }

        private void QuestMachine_OnQuestEnded(Quest quest)
        {
            // Refresh when a quest ends
            ClearSiteTargets();
        }

        private void PlayerGPS_OnExitLocationRect()
        {
            // Clear when exiting location rect
            ClearSiteTargets();
        }

        private void PlayerGPS_OnMapPixelChanged(DaggerfallConnect.Utility.DFPosition mapPixel)
        {
            // Clear when changing map pixel
            ClearSiteTargets();
        }

        #endregion
    }
}