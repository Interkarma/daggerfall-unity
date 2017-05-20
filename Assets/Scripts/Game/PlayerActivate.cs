// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
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
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Utility;

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

            // Fire ray into scene
            if (InputManager.Instance.ActionStarted(InputManager.Actions.ActivateCenterObject))
            {
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
                        // Check for a static building hit
                        Transform buildingOwner;
                        DaggerfallStaticBuildings buildings = GetBuildings(hits[i].transform, out buildingOwner);
                        if (buildings)
                        {
                            if (buildings.HasHit(hits[i].point, out building))
                            {
                                hitBuilding = true;

                                //// TEMP: Print building name or type to HUD as a test
                                //// This will be moved to a proper info click soon
                                //if (building.buildingData.BuildingType >= DFLocation.BuildingTypes.House1 &&
                                //    building.buildingData.BuildingType <= DFLocation.BuildingTypes.House6)
                                //{
                                //    DaggerfallUI.AddHUDText("Residence");
                                //}
                                //else
                                //{
                                //    ContentReader.MapSummary mapSummary;
                                //    DFPosition mapPixel = GameManager.Instance.PlayerGPS.CurrentMapPixel;
                                //    if (DaggerfallUnity.Instance.ContentReader.HasLocation(mapPixel.X, mapPixel.Y, out mapSummary))
                                //    {
                                //        DFLocation playerLocation = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(mapSummary.RegionIndex, mapSummary.MapIndex);

                                //        string buildingName = BuildingNames.GetName(
                                //            building.buildingData.NameSeed,
                                //            building.buildingData.BuildingType,
                                //            building.buildingData.FactionId,
                                //            playerLocation.Name,
                                //            playerLocation.RegionName);

                                //        DaggerfallUI.AddHUDText(buildingName);
                                //    }
                                //}
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
                    }
                }
            }
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
    }
}