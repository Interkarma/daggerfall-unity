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
using System;
using System.Collections;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Player;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Example class to handle activation of doors, switches, etc. from Fire1 input.
    /// </summary>
    public class PlayerActivate : MonoBehaviour
    {
        PlayerGPS playerGPS;
        PlayerEnterExit playerEnterExit;        // Example component to enter/exit buildings
        GameObject mainCamera;

        Transform deferredInteriorDoorOwner;    // Used to defer interior transition after popup message
        StaticDoor deferredInteriorDoor;

        PlayerActivateModes currentMode = PlayerActivateModes.Grab;

        public float RayDistance = 2.0f;        // Distance of ray check, tune this to your scale and preference

        void Start()
        {
            playerGPS = GetComponent<PlayerGPS>();
            playerEnterExit = GetComponent<PlayerEnterExit>();
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        void Update()
        {
            if (mainCamera == null)
                return;

            // Change activate mode
            if (InputManager.Instance.ActionStarted(InputManager.Actions.StealMode))
                ChangeInteractionMode(PlayerActivateModes.Steal);
            else if (InputManager.Instance.ActionStarted(InputManager.Actions.GrabMode))
                ChangeInteractionMode(PlayerActivateModes.Grab);
            else if (InputManager.Instance.ActionStarted(InputManager.Actions.InfoMode))
                ChangeInteractionMode(PlayerActivateModes.Info);
            else if (InputManager.Instance.ActionStarted(InputManager.Actions.TalkMode))
                ChangeInteractionMode(PlayerActivateModes.Talk);

            // Fire ray into scene
            if (InputManager.Instance.ActionStarted(InputManager.Actions.ActivateCenterObject))
            {
                // TODO: Clean all this up and support mobile enemy info-clicks

                // Using RaycastAll as hits can be blocked by decorations or other models
                // When this happens activation feels unresponsive to player
                // Also processing hit detection in order of priority
                Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
                RaycastHit[] hits;
                hits = Physics.RaycastAll(ray, RayDistance);
                if (hits != null)
                {
                    // Check each hit in range for action, exit on first valid action processed
                    bool hitBuilding = false;
                    StaticBuilding building = new StaticBuilding();
                    for (int i = 0; i < hits.Length; i++)
                    {
                        #region Hit Checks

                        // Check for a static building hit
                        Transform buildingOwner;
                        DaggerfallStaticBuildings buildings = GetBuildings(hits[i].transform, out buildingOwner);
                        if (buildings)
                        {
                            if (buildings.HasHit(hits[i].point, out building))
                            {
                                hitBuilding = true;

                                // Show building info
                                if (currentMode == PlayerActivateModes.Info)
                                    PresentBuildingInfo(building);
                            }
                        }

                        // Check for a static door hit
                        Transform doorOwner;
                        DaggerfallStaticDoors doors = GetDoors(hits[i].transform, out doorOwner);
                        if (doors && playerEnterExit)
                        {
                            StaticDoor door;
                            if (doors.HasHit(hits[i].point, out door))
                            {
                                if (door.doorType == DoorTypes.Building && !playerEnterExit.IsPlayerInside)
                                {
                                    // If entering a shop let player know the quality level
                                    if (hitBuilding)
                                    {
                                        DaggerfallMessageBox mb = PresentShopQuality(building);
                                        if (mb != null)
                                        {
                                            // Defer transition to interior to after user closes messagebox
                                            deferredInteriorDoorOwner = doorOwner;
                                            deferredInteriorDoor = door;
                                            mb.OnClose += ShopQualityPopup_OnClose;
                                            return;
                                        }
                                    }

                                    // Hit door while outside, transition inside
                                    playerEnterExit.TransitionInterior(doorOwner, door, true);
                                    return;
                                }
                                else if (door.doorType == DoorTypes.Building && playerEnterExit.IsPlayerInside)
                                {
                                    // Hit door while inside, transition outside
                                    playerEnterExit.TransitionExterior(true);
                                    return;
                                }
                                else if (door.doorType == DoorTypes.DungeonEntrance && !playerEnterExit.IsPlayerInside)
                                {
                                    if (playerGPS)
                                    {
                                        // Hit dungeon door while outside, transition inside
                                        playerEnterExit.TransitionDungeonInterior(doorOwner, door, playerGPS.CurrentLocation, true);
                                        return;
                                    }
                                }
                                else if (door.doorType == DoorTypes.DungeonExit && playerEnterExit.IsPlayerInside)
                                {
                                    // Hit dungeon exit while inside, transition outside
                                    playerEnterExit.TransitionDungeonExterior(true);
                                    return;
                                }
                            }
                        }

                        // Check for an action door hit
                        DaggerfallActionDoor actionDoor;
                        if (ActionDoorCheck(hits[i], out actionDoor))
                        {
                            if (currentMode == PlayerActivateModes.Steal && actionDoor.IsLocked)
                            {
                                if (actionDoor.IsNoLongerPickable)
                                    return;
                                else
                                    actionDoor.AttemptLockpicking();
                            }
                            else
                                actionDoor.ToggleDoor();
                        }

                        // Check for action record hit
                        DaggerfallAction action;
                        if (ActionCheck(hits[i], out action))
                        {
                            action.Receive(this.gameObject, DaggerfallAction.TriggerTypes.Direct);
                        }

                        // Check for lootable object hit
                        DaggerfallLoot loot;
                        if (LootCheck(hits[i], out loot))
                        {
                            // For bodies, check has treasure first
                            if (loot.ContainerType == LootContainerTypes.CorpseMarker && loot.Items.Count == 0)
                            {
                                DaggerfallUI.AddHUDText(HardStrings.theBodyHasNoTreasure);
                                return;
                            }

                            // Open inventory window with loot as remote target
                            DaggerfallUI.Instance.InventoryWindow.LootTarget = loot;
                            DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenInventoryWindow);
                        }

                        // Check for static NPC hit
                        StaticNPC npc;
                        if (NPCCheck(hits[i], out npc))
                        {
                            switch (currentMode)
                            {
                                case PlayerActivateModes.Info:
                                    PresentNPCInfo(npc);
                                    break;
                                case PlayerActivateModes.Grab:
                                case PlayerActivateModes.Talk:
                                    StaticNPCClick(npc);
                                    break;
                            }
                        }

                        // Trigger general quest resource behaviour click
                        // Note: This will cause a second click on special NPCs, look into a way to unify this handling
                        QuestResourceBehaviour questResourceBehaviour;
                        if (QuestResourceBehaviourCheck(hits[i], out questResourceBehaviour))
                        {
                            TriggerQuestResourceBehaviourClick(questResourceBehaviour);
                        }

                        // Trigger ladder hit
                        DaggerfallLadder ladder = hits[i].transform.GetComponent<DaggerfallLadder>();
                        if (ladder)
                        {
                            ladder.ClimbLadder();
                        }

                        #endregion
                    }
                }
            }
        }

        // Shop quality message box closed, move to interior
        private void ShopQualityPopup_OnClose()
        {
            playerEnterExit.TransitionInterior(deferredInteriorDoorOwner, deferredInteriorDoor, true);
        }

        // Look for building array on object, then on direct parent
        private DaggerfallStaticBuildings GetBuildings(Transform transform, out Transform owner)
        {
            owner = null;
            DaggerfallStaticBuildings buildings = transform.GetComponent<DaggerfallStaticBuildings>();
            if (!buildings)
            {
                buildings = transform.GetComponentInParent<DaggerfallStaticBuildings>();
                if (buildings)
                    owner = buildings.transform;
            }
            else
            {
                owner = buildings.transform;
            }

            return buildings;
        }

        // Look for doors on object, then on direct parent
        private DaggerfallStaticDoors GetDoors(Transform transform, out Transform owner)
        {
            owner = null;
            DaggerfallStaticDoors doors = transform.GetComponent<DaggerfallStaticDoors>();
            if (!doors)
            {
                doors = transform.GetComponentInParent<DaggerfallStaticDoors>();
                if (doors)
                    owner = doors.transform;
            }
            else
            {
                owner = doors.transform;
            }

            return doors;
        }

        // Check if raycast hit a static door
        private bool StaticDoorCheck(RaycastHit hitInfo, out DaggerfallStaticDoors door)
        {
            door = hitInfo.transform.GetComponent<DaggerfallStaticDoors>();
            if (door == null)
                return false;

            return true;
        }

        // Check if raycast hit an action door
        private bool ActionDoorCheck(RaycastHit hitInfo, out DaggerfallActionDoor door)
        {
            door = hitInfo.transform.GetComponent<DaggerfallActionDoor>();
            if (door == null)
                return false;

            return true;
        }

        // Check if raycast hit a generic action component
        private bool ActionCheck(RaycastHit hitInfo, out DaggerfallAction action)
        {
            // Look for action
            action = hitInfo.transform.GetComponent<DaggerfallAction>();
            if (action == null)
                return false;
            else
                return true;
        }

        // Check if raycast hit a lootable object
        private bool LootCheck(RaycastHit hitInfo, out DaggerfallLoot loot)
        {
            loot = hitInfo.transform.GetComponent<DaggerfallLoot>();
            if (loot == null)
                return false;
            else
                return true;
        }

        // Check if raycast hit a StaticNPC
        private bool NPCCheck(RaycastHit hitInfo, out StaticNPC staticNPC)
        {
            staticNPC = hitInfo.transform.GetComponent<StaticNPC>();
            if (staticNPC != null)
                return true;
            else
                return false;
        }

        // Check if raycast hit a QuestResource
        private bool QuestResourceBehaviourCheck(RaycastHit hitInfo, out QuestResourceBehaviour questResourceBehaviour)
        {
            questResourceBehaviour = hitInfo.transform.GetComponent<QuestResourceBehaviour>();
            if (questResourceBehaviour != null)
                return true;
            else
                return false;
        }

        // Display a shop quality level
        private DaggerfallMessageBox PresentShopQuality(StaticBuilding building)
        {
            const int qualityLevel1TextId = 266;    // "Incense and soft music soothe your nerves"
            const int qualityLevel2TextId = 267;    // "The shop is better appointed than many"
            const int qualityLevel3TextId = 268;    // "The shop is laid out in a practical"
            const int qualityLevel4TextId = 269;    // "Sturdy shelves, cobbled together"
            const int qualityLevel5TextId = 270;    // "Rusty relics lie wherever they were last tossed"

            // Get building directory for location
            BuildingDirectory buildingDirectory = GameManager.Instance.StreamingWorld.GetCurrentBuildingDirectory();
            if (!buildingDirectory)
                return null;

            // Get detailed building data from directory
            BuildingSummary buildingSummary;
            if (!buildingDirectory.GetBuildingSummary(building.buildingKey, out buildingSummary))
                return null;

            // Do nothing if not a shop
            if (!RMBLayout.IsShop(buildingSummary.BuildingType))
                return null;

            // Set quality level text ID from quality value 01-20
            // UESP states this is building quality / 4 but Daggerfall uses manual thresholds
            int qualityTextId;
            if (buildingSummary.Quality <= 3)
                qualityTextId = qualityLevel5TextId;        // 01 - 03
            else if (buildingSummary.Quality <= 7)
                qualityTextId = qualityLevel4TextId;        // 04 - 07
            else if (buildingSummary.Quality <= 13)
                qualityTextId = qualityLevel3TextId;        // 08 - 13
            else if (buildingSummary.Quality <= 17)
                qualityTextId = qualityLevel2TextId;        // 14 - 17
            else
                qualityTextId = qualityLevel1TextId;        // 18 - 20

            // Log quality of building entered for debugging
            //Debug.Log("Entered store with quality of " + buildingData.Quality);

            // Output quality text based on settings
            switch (DaggerfallUnity.Settings.ShopQualityPresentation)
            {
                case 0:     // Display popup as per classic
                    return DaggerfallUI.MessageBox(qualityTextId);

                case 1:     // Display HUD text only with variable delay
                    TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(qualityTextId);
                    for (int i = 0; i < tokens.Length; i++)
                    {
                        if (tokens[i].formatting == TextFile.Formatting.Text)
                            DaggerfallUI.AddHUDText(tokens[i].text, DaggerfallUnity.Settings.ShopQualityHUDDelay);
                    }
                    break;

                case 2:     // Display nothing about shop quality
                default:
                    return null;
            }

            return null;
        }

        // Sets new activation mode
        private void ChangeInteractionMode(PlayerActivateModes newMode)
        {
            // Do nothing if new mode matches current mode
            if (newMode == currentMode)
                return;

            // Set the new mode
            currentMode = newMode;

            // Get output text based on mode
            string modeText = string.Empty;
            switch(currentMode)
            {
                case PlayerActivateModes.Steal:
                    modeText = HardStrings.steal;
                    break;
                case PlayerActivateModes.Grab:
                    modeText = HardStrings.grab;
                    break;
                case PlayerActivateModes.Info:
                    modeText = HardStrings.info;
                    break;
                case PlayerActivateModes.Talk:
                    modeText = HardStrings.dialogue;
                    break;
            }

            // Present new mode to player
            DaggerfallUI.SetMidScreenText(HardStrings.interactionIsNowInMode.Replace("%s", modeText));
        }

        // Output building info to HUD
        private void PresentBuildingInfo(StaticBuilding building)
        {
            // Get building directory for location
            BuildingDirectory buildingDirectory = GameManager.Instance.StreamingWorld.GetCurrentBuildingDirectory();
            if (!buildingDirectory)
                return;

            // Get detailed building data from directory
            BuildingSummary buildingSummary;
            if (!buildingDirectory.GetBuildingSummary(building.buildingKey, out buildingSummary))
            {
                int layoutX, layoutY, recordIndex;
                BuildingDirectory.ReverseBuildingKey(building.buildingKey, out layoutX, out layoutY, out recordIndex);
                Debug.LogFormat("Unable to find expected building key {0} in {1}.{2}", building.buildingKey, buildingDirectory.LocationData.RegionName, buildingDirectory.LocationData.Name);
                Debug.LogFormat("LayoutX={0}, LayoutY={1}, RecordIndex={2}", layoutX, layoutY, recordIndex);
                throw new Exception("Error finding building key in directory.");
            }

            // Resolve name by building type
            string buildingName;
            if (RMBLayout.IsResidence(buildingSummary.BuildingType))
            {
                // Residence
                // TODO: Link to quest system active sites
                buildingName = HardStrings.residence;
            }
            else
            {
                // Fixed building name
                buildingName = BuildingNames.GetName(
                    buildingSummary.NameSeed,
                    buildingSummary.BuildingType,
                    buildingSummary.FactionId,
                    buildingDirectory.LocationData.Name,
                    buildingDirectory.LocationData.RegionName);
            }

            // Output building name to HUD
            DaggerfallUI.AddHUDText(buildingName);
        }

        // Output NPC info to HUD
        private void PresentNPCInfo(StaticNPC npc)
        {
            DaggerfallUI.AddHUDText(HardStrings.youSee.Replace("%s", npc.DisplayName));
        }

        // Player has clicked a GameObject with a QuestResourceBehaviour attached
        void TriggerQuestResourceBehaviourClick(QuestResourceBehaviour questResourceBehaviour)
        {
            // Handle typical quest resource click
            if (questResourceBehaviour)
                questResourceBehaviour.DoClick();
        }

        // Player has clicked on a static NPC
        void StaticNPCClick(StaticNPC npc)
        {
            // Store the NPC just clicked in quest engine
            QuestMachine.Instance.LastNPCClicked = npc;

            // Check if this NPC is a quest giver and show temp guild quest popup
            QuestorCheck(npc);

            // Handle special NPC in home location click
            SpecialNPCClickHandler specialNPCClickHandler = npc.gameObject.GetComponent<SpecialNPCClickHandler>();
            if (specialNPCClickHandler)
                specialNPCClickHandler.DoClick();
        }

        // Check if NPC is a Questor
        void QuestorCheck(StaticNPC npc)
        {
            // Check if player clicked on supported guild questor
            DaggerfallGuildPopupWindow.TempGuilds guild;
            if (npc.Data.factionID == PersistentFactionData.fightersGuildQuestorFactionID)
                guild = DaggerfallGuildPopupWindow.TempGuilds.Fighter;
            else if (npc.Data.factionID == PersistentFactionData.magesGuildQuestorFactionID)
                guild = DaggerfallGuildPopupWindow.TempGuilds.Mage;
            else
                guild = DaggerfallGuildPopupWindow.TempGuilds.None;

            // Open guild service window
            if (guild != DaggerfallGuildPopupWindow.TempGuilds.None)
            {
                DaggerfallGuildPopupWindow guildWindow = new DaggerfallGuildPopupWindow(DaggerfallUI.Instance.UserInterfaceManager);
                guildWindow.CurrentGuild = guild;
                guildWindow.CurrentService = DaggerfallGuildPopupWindow.TempGuildServices.Questor;
                guildWindow.QuestorNPC = npc;
                DaggerfallUI.Instance.UserInterfaceManager.PushWindow(guildWindow);
            }
        }
    }
}