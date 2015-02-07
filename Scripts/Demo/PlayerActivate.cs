// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;

namespace DaggerfallWorkshop.Demo
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
            if (Input.GetButtonDown("Fire1"))
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
                    for (int i = 0; i < hits.Length; i++)
                    {
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
                                    playerEnterExit.TransitionInterior(doorOwner, door);
                                    return;
                                }
                                else if (door.doorType == DoorTypes.Building && playerEnterExit.IsPlayerInside)
                                {
                                    // Hit door while inside, transition outside
                                    playerEnterExit.TransitionExterior();
                                    return;
                                }
                                else if (door.doorType == DoorTypes.DungeonEntrance && !playerEnterExit.IsPlayerInside)
                                {
                                    if (playerGPS)
                                    {
                                        // Hit dungeon door while outside, transition inside
                                        playerEnterExit.TransitionDungeonInterior(doorOwner, door, playerGPS.CurrentLocation);
                                        return;
                                    }
                                }
                                else if (door.doorType == DoorTypes.DungeonExit && playerEnterExit.IsPlayerInside)
                                {
                                    // Hit dungeon exit while inside, transtion outside
                                    playerEnterExit.TransitionDungeonExterior();
                                    return;
                                }
                            }
                        }

                        // Check for an action door hit
                        DaggerfallActionDoor actionDoor;
                        if (ActionDoorCheck(hits[i], out actionDoor))
                        {
                            actionDoor.ToggleDoor();
                            return;
                        }

                        // Check for action record hit
                        DaggerfallAction action;
                        if (ActionCheck(hits[i], out action))
                        {
                            action.Play();
                            return;
                        }
                    }
                }
            }
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

            // Must be root action of chain (no parent)
            if (action.PreviousObject != null)
            {
                action = null;
                return false;
            }

            return true;
        }
    }
}