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
        List<Vector3> doorPositions = new List<Vector3>();
        List<TextLabel> markerLabels = new List<TextLabel>();

        public HUDPlaceMarker()
            : base()
        {
            // Events used to update site list
            QuestMachine.OnQuestStarted += QuestMachine_OnQuestStarted;
            PlayerGPS.OnEnterLocationRect += PlayerGPS_OnEnterLocationRect;
            PlayerGPS.OnExitLocationRect += PlayerGPS_OnExitLocationRect;
            PlayerGPS.OnMapPixelChanged += PlayerGPS_OnMapPixelChanged;
        }

        public override void Update()
        {
            base.Update();

            // Do nothing if inside - might be handy to add object markers later
            bool enableMarkers = true;
            if (GameManager.Instance.PlayerEnterExit.IsPlayerInside)
            {
                enableMarkers = false;
            }

            // Do nothing if no positions or labels don't match markers
            if (doorPositions.Count == 0 || doorPositions.Count != markerLabels.Count)
            {
                enableMarkers = false;
            }

            // Set marker label position and text
            Rect rect = Rectangle;
            Camera mainCamera = GameManager.Instance.MainCamera;
            for (int i = 0; i < doorPositions.Count; i++)
            {
                markerLabels[i].Enabled = enableMarkers;

                Vector3 screenPos = mainCamera.WorldToScreenPoint(doorPositions[i]);
                if (screenPos.z < 0)
                    continue;

                Vector2 panelPos = ScreenToLocal(new Vector2(screenPos.x, rect.height - screenPos.y));
                markerLabels[i].Position = panelPos;
                markerLabels[i].Text = screenPos.z.ToString("[0]");
            }
        }

        #region Private Methods

        /// <summary>
        /// Rebuild door marker positions for active quest sites in current player location only.
        /// Quite inefficient but this class is only intended for early quest testing.
        /// This method should only be run when player enters/exits a location rect or when a new quest begins.
        /// </summary>
        void RefreshSites()
        {
            // Clear existing sites
            ClearSites();

            // Get player's location component in scene
            DaggerfallLocation dfLocation = GameManager.Instance.StreamingWorld.GetPlayerLocationObject();
            if (!dfLocation)
            {
                ClearSites();
                return;
            }

            // Get all active quest sites
            SiteDetails[] allSites = QuestMachine.Instance.GetAllActiveQuestSites();
            if (allSites == null || allSites.Length == 0)
            {
                ClearSites();
                return;
            }

            // Get all doors attached to this location
            DaggerfallStaticDoors[] allDoors = dfLocation.gameObject.GetComponentsInChildren<DaggerfallStaticDoors>();
            if (allDoors == null || allDoors.Length == 0)
            {
                ClearSites();
                return;
            }

            // Enumerate sites and look for entrance doors
            int foundTotal = 0;
            foreach (SiteDetails site in allSites)
            {
                // Skip other locations
                if (site.mapId != dfLocation.Summary.MapID)
                    continue;

                // Build a list of all location doors with matching block/record indices
                foreach (DaggerfallStaticDoors dfStaticDoors in allDoors)
                {
                    // Get DaggerfallRMBBlock holding this door collection
                    DaggerfallRMBBlock rmbBlock = dfStaticDoors.GetParentRMBBlock();
                    if (!rmbBlock)
                        continue;

                    // Check we're in the same block as site
                    if (site.layoutX != rmbBlock.LayoutX || site.layoutY != rmbBlock.LayoutY)
                        continue;

                    // Site is in the same location and layout position, check for matching building record
                    foreach (StaticDoor door in dfStaticDoors.Doors)
                    {
                        if (door.recordIndex == site.buildingSummary.RecordIndex)
                        {
                            foundTotal++;
                            Vector3 position = door.buildingMatrix.MultiplyPoint3x4(door.centre) + dfStaticDoors.transform.position;
                            doorPositions.Add(position);
                        }
                    }
                }
            }

            // Output how many doors were found in thise location and get out if zero
            Debug.LogFormat("Found {0} doors matching an active quest site in location {1}/{2}.", foundTotal, dfLocation.Summary.RegionName, dfLocation.Summary.LocationName);
            if (foundTotal == 0)
            {
                ClearSites();
                return;
            }

            // Create a text label for door position
            for (int i = 0; i < doorPositions.Count; i++)
            {
                TextLabel label = new TextLabel();
                markerLabels.Add(label);
                Components.Add(label);
            }
        }

        void ClearSites()
        {
            doorPositions.Clear();
            for (int i = 0; i < markerLabels.Count; i++)
            {
                Components.Remove(markerLabels[i]);
                markerLabels[i].Dispose();
                markerLabels[i] = null;
            }
            markerLabels.Clear();
        }

        #endregion

        #region Event Handlers

        private void PlayerGPS_OnEnterLocationRect(DaggerfallConnect.DFLocation location)
        {
            // Refresh when entering a location rect
            RefreshSites();
        }

        private void QuestMachine_OnQuestStarted(Quest quest)
        {
            // Refresh when starting a new quest
            RefreshSites();
        }

        private void PlayerGPS_OnExitLocationRect()
        {
            // Clear when exiting location rect
            ClearSites();
        }

        private void PlayerGPS_OnMapPixelChanged(DaggerfallConnect.Utility.DFPosition mapPixel)
        {
            // Clear when changing map pixel
            ClearSites();
        }

        #endregion
    }
}