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
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Items;

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

        public float RayDistance = 0;           // Distance of ray check, tune this to your scale and preference
        public float ActivateDistance = 2.3f;   // Distance within which something must be for player to activate it. Tune as needed.

        // Maximum distance from which different object types can be activated, in classic distance units
        public float DefaultActivationDistance = 128;
        public float DoorActivationDistance = 128;
        public float TreasureActivationDistance = 128;
        public float PickpocketDistance = 128;
        public float CorpseActivationDistance = 150;
        //public float TouchSpellActivationDistance = 160;
        public float StaticNPCActivationDistance = 256;
        public float MobileNPCActivationDistance = 256;

        // Opening and closing hours by building type
        byte[] openHours = { 7, 8, 9, 8, 0, 9, 10, 10, 9, 6, 9, 11, 9, 9, 0, 0, 10, 0 };
        byte[] closeHours = { 22, 16, 19, 15, 25, 21, 19, 20, 18, 23, 23, 23, 20, 20, 25, 25, 16, 0 };

        public PlayerActivateModes CurrentMode
        {
            get { return currentMode; }
        }

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
                // TODO: Clean all this up

                // Ray origin is slightly below camera height to ensure it originates inside player's own collider
                // This prevents ray from intersecting with player's own collider and blocking looting or low triggers
                Ray ray = new Ray(transform.position + Vector3.up * 0.7f, mainCamera.transform.forward);
                RaycastHit hit;
                RayDistance = 75f; // Approximates classic at full view distance (default setting). Classic seems to do raycasts for as far as it can render objects.
                bool hitSomething = Physics.Raycast(ray, out hit, RayDistance);
                if (hitSomething)
                {
                    bool hitBuilding = false;
                    bool buildingUnlocked = false;
                    DFLocation.BuildingTypes buildingType = DFLocation.BuildingTypes.AllValid;
                    StaticBuilding building = new StaticBuilding();

                    #region Hit Checks

                    // Trigger quest resource behaviour click on anything but NPCs
                    QuestResourceBehaviour questResourceBehaviour;
                    if (QuestResourceBehaviourCheck(hit, out questResourceBehaviour))
                    {
                        if (!(questResourceBehaviour.TargetResource is Person))
                        {
                            if (hit.distance > (DefaultActivationDistance * MeshReader.GlobalScale))
                            {
                                DaggerfallUI.SetMidScreenText(HardStrings.youAreTooFarAway);
                                return;
                            }

                            // Only trigger click when not in info mode
                            if (currentMode != PlayerActivateModes.Info)
                            {
                                TriggerQuestResourceBehaviourClick(questResourceBehaviour);
                            }
                        }
                    }

                    // Check for a static building hit
                    Transform buildingOwner;
                    DaggerfallStaticBuildings buildings = GetBuildings(hit.transform, out buildingOwner);
                    if (buildings)
                    {
                        if (buildings.HasHit(hit.point, out building))
                        {
                            hitBuilding = true;

                            // Get building directory for location
                            BuildingDirectory buildingDirectory = GameManager.Instance.StreamingWorld.GetCurrentBuildingDirectory();
                            if (!buildingDirectory)
                                return;

                            // Get detailed building data from directory
                            BuildingSummary buildingSummary;
                            if (!buildingDirectory.GetBuildingSummary(building.buildingKey, out buildingSummary))
                                return;

                            // Check if door is unlocked
                            buildingUnlocked = BuildingIsUnlocked(buildingSummary);

                            // Store building type
                            buildingType = buildingSummary.BuildingType;

                            if (currentMode == PlayerActivateModes.Info)
                            {
                                // Discover building
                                GameManager.Instance.PlayerGPS.DiscoverBuilding(building.buildingKey);

                                // Get discovered building
                                PlayerGPS.DiscoveredBuilding db;
                                if (GameManager.Instance.PlayerGPS.GetDiscoveredBuilding(building.buildingKey, out db))
                                {
                                    // TODO: Check against quest system for an overriding quest-assigned display name for this building
                                    DaggerfallUI.AddHUDText(db.displayName);

                                    if (!buildingUnlocked && buildingType < DFLocation.BuildingTypes.Temple
                                        && buildingType != DFLocation.BuildingTypes.HouseForSale)
                                    {
                                        string storeClosedMessage = HardStrings.storeClosed;
                                        storeClosedMessage = storeClosedMessage.Replace("%d1", openHours[(int)buildingType].ToString());
                                        storeClosedMessage = storeClosedMessage.Replace("%d2", closeHours[(int)buildingType].ToString());
                                        DaggerfallUI.Instance.PopupMessage(storeClosedMessage);
                                    }
                                }
                            }
                        }
                    }

                    // Check for a static door hit
                    Transform doorOwner;
                    DaggerfallStaticDoors doors = GetDoors(hit.transform, out doorOwner);
                    if (doors && playerEnterExit)
                    {
                        StaticDoor door;
                        if (doors.HasHit(hit.point, out door))
                        {
                            // Check if close enough to activate
                            if (hit.distance > (DoorActivationDistance * MeshReader.GlobalScale))
                            {
                                DaggerfallUI.SetMidScreenText(HardStrings.youAreTooFarAway);
                                return;
                            }

                            if (door.doorType == DoorTypes.Building && !playerEnterExit.IsPlayerInside)
                            {
                                // Discover building
                                GameManager.Instance.PlayerGPS.DiscoverBuilding(building.buildingKey);

                                // TODO: Implement lockpicking and door bashing for exterior doors
                                // For now, any locked building door can be entered by using steal mode
                                if (!buildingUnlocked)
                                {
                                    if (currentMode != PlayerActivateModes.Steal)
                                    {
                                        string Locked = "Locked.";
                                        DaggerfallUI.Instance.PopupMessage(Locked);
                                        return;
                                    }
                                    else // Breaking into building
                                    {
                                        PlayerEntity player = GameManager.Instance.PlayerEntity;
                                        //player.TallyCrimeGuildRequirements(true, 1);
                                    }
                                }

                                // If entering a shop let player know the quality level
                                // If entering an open home, show greeting
                                if (hitBuilding)
                                {
                                    const int houseGreetingsTextId = 256;

                                    DaggerfallMessageBox mb;

                                    if (buildingUnlocked && buildingType >= DFLocation.BuildingTypes.House1
                                        && buildingType <= DFLocation.BuildingTypes.House4)
                                    {
                                        string greetingText = DaggerfallUnity.Instance.TextProvider.GetRandomText(houseGreetingsTextId);
                                        mb = DaggerfallUI.MessageBox(greetingText);
                                    }
                                    else
                                        mb = PresentShopQuality(building);

                                    if (mb != null)
                                    {
                                        // Defer transition to interior to after user closes messagebox
                                        deferredInteriorDoorOwner = doorOwner;
                                        deferredInteriorDoor = door;
                                        mb.OnClose += Popup_OnClose;
                                        return;
                                    }
                                }
                                    
                                // Hit door while outside, transition inside
                                TransitionInterior(doorOwner, door, true);
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
                                // Hit dungeon exit while inside, ask if access wagon or transition outside
                                if (GameManager.Instance.PlayerEntity.Items.Contains(ItemGroups.Transportation, (int) Transportation.Small_cart))
                                {
                                    DaggerfallMessageBox messageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallMessageBox.CommonMessageBoxButtons.YesNo, 38, DaggerfallUI.UIManager.TopWindow);
                                    messageBox.OnButtonClick += DungeonWagonAccess_OnButtonClick;
                                    DaggerfallUI.UIManager.PushWindow(messageBox);
                                    return;
                                }
                                else
                                {
                                    playerEnterExit.TransitionDungeonExterior(true);
                                }
                            }
                        }
                    }

                    // Check for an action door hit
                    DaggerfallActionDoor actionDoor;
                    if (ActionDoorCheck(hit, out actionDoor))
                    {
                        // Check if close enough to activate
                        if (hit.distance > (DoorActivationDistance * MeshReader.GlobalScale))
                        {
                            DaggerfallUI.SetMidScreenText(HardStrings.youAreTooFarAway);
                            return;
                        }

                        if (currentMode == PlayerActivateModes.Steal && actionDoor.IsLocked && !actionDoor.IsOpen)
                        {
                            actionDoor.AttemptLockpicking();
                        }
                        else
                            actionDoor.ToggleDoor(true);
                    }

                    // Check for action record hit
                    DaggerfallAction action;
                    if (ActionCheck(hit, out action))
                    {
                        if (hit.distance <= (DefaultActivationDistance * MeshReader.GlobalScale))
                        {
                            action.Receive(this.gameObject, DaggerfallAction.TriggerTypes.Direct);
                        }
                    }

                    // Check for lootable object hit
                    DaggerfallLoot loot;
                    if (LootCheck(hit, out loot))
                    {
                        if (hit.distance > TreasureActivationDistance * MeshReader.GlobalScale)
                        {
                            DaggerfallUI.SetMidScreenText(HardStrings.youAreTooFarAway);
                            return;
                        }
                        // Handle shop shelves. (mode not relevant)
                        if (loot.ContainerType == LootContainerTypes.Shelves)
                        {
                            // Open trade window with activated loot container as remote target.
                            UserInterfaceManager uiManager = DaggerfallUI.Instance.UserInterfaceManager;
                            DaggerfallTradeWindow tradeWindow = new DaggerfallTradeWindow(uiManager, DaggerfallTradeWindow.WindowModes.Buy);
                            tradeWindow.MerchantItems = loot.Items;
                            uiManager.PushWindow(tradeWindow);
                        }
                        else
                        {
                            switch (currentMode)
                            {
                                case PlayerActivateModes.Info:
                                    if (loot.ContainerType == LootContainerTypes.CorpseMarker && !string.IsNullOrEmpty(loot.entityName))
                                    {
                                        string message = string.Empty;
                                        if (loot.isEnemyClass)
                                            message = HardStrings.youSeeADeadPerson;
                                        else
                                        {
                                            message = HardStrings.youSeeADead;
                                            message = message.Replace("%s", loot.entityName);
                                        }
                                        DaggerfallUI.Instance.PopupMessage(message);
                                    }
                                    break;
                                case PlayerActivateModes.Grab:
                                case PlayerActivateModes.Talk:
                                case PlayerActivateModes.Steal:
                                    // Check if close enough to activate
                                    if (loot.ContainerType == LootContainerTypes.CorpseMarker &&
                                        hit.distance > CorpseActivationDistance * MeshReader.GlobalScale)
                                    {
                                        DaggerfallUI.SetMidScreenText(HardStrings.youAreTooFarAway);
                                        break;
                                    }
                                    // For bodies, check has treasure
                                    else if (loot.ContainerType == LootContainerTypes.CorpseMarker && loot.Items.Count == 0)
                                    {
                                        DaggerfallUI.AddHUDText(HardStrings.theBodyHasNoTreasure);
                                        break;
                                    }
                                    // Open inventory window with activated loot container as remote target.
                                    DaggerfallUI.Instance.InventoryWindow.LootTarget = loot;
                                    DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenInventoryWindow);
                                    break;
                            }
                        }
                    }

                    // Check for static NPC hit
                    StaticNPC npc;
                    if (NPCCheck(hit, out npc))
                    {
                        switch (currentMode)
                        {
                            case PlayerActivateModes.Info:
                                PresentNPCInfo(npc);
                                break;
                            case PlayerActivateModes.Grab:
                            case PlayerActivateModes.Talk:
                            case PlayerActivateModes.Steal:
                                if (hit.distance > (StaticNPCActivationDistance * MeshReader.GlobalScale))
                                {
                                    DaggerfallUI.SetMidScreenText(HardStrings.youAreTooFarAway);
                                    break;
                                }
                                StaticNPCClick(npc);
                                break;
                        }
                    }

                    // Check for mobile NPC hit
                    MobilePersonNPC mobileNpc = null;
                    if (MobilePersonMotorCheck(hit, out mobileNpc))
                    {
                        switch (currentMode)
                        {
                            case PlayerActivateModes.Info:
                            case PlayerActivateModes.Grab:
                            case PlayerActivateModes.Talk:
                                if (hit.distance > (MobileNPCActivationDistance * MeshReader.GlobalScale))
                                {
                                    DaggerfallUI.SetMidScreenText(HardStrings.youAreTooFarAway);
                                    break;
                                }
                                GameManager.Instance.TalkManager.TalkToMobileNPC(mobileNpc);
                                break;
                            case PlayerActivateModes.Steal:
                                if (!mobileNpc.PickpocketByPlayerAttempted)
                                {
                                    if (hit.distance > (PickpocketDistance * MeshReader.GlobalScale))
                                    {
                                        DaggerfallUI.SetMidScreenText(HardStrings.youAreTooFarAway);
                                        break;
                                    }
                                    mobileNpc.PickpocketByPlayerAttempted = true;
                                    Pickpocket();
                                }
                                break;
                        }
                    }

                    // Check for mobile enemy hit
                    DaggerfallEntityBehaviour mobileEnemyBehaviour;
                    if (MobileEnemyCheck(hit, out mobileEnemyBehaviour))
                    {
                        EnemyEntity enemyEntity = mobileEnemyBehaviour.Entity as EnemyEntity;
                        switch (currentMode)
                        {
                            case PlayerActivateModes.Info:
                            case PlayerActivateModes.Grab:
                            case PlayerActivateModes.Talk:
                                if (enemyEntity != null)
                                {
                                    MobileEnemy mobileEnemy = enemyEntity.MobileEnemy;
                                    bool startsWithVowel = "aeiouAEIOU".Contains(mobileEnemy.Name[0].ToString());
                                    string message;
                                    if (startsWithVowel)
                                        message = HardStrings.youSeeAn;
                                    else
                                        message = HardStrings.youSeeA;
                                    message = message.Replace("%s", mobileEnemy.Name);
                                    DaggerfallUI.Instance.PopupMessage(message);
                                }
                                break;
                            case PlayerActivateModes.Steal:
                                // Classic allows pickpocketing of NPC mobiles and enemy mobiles.
                                // In early versions the only enemy mobiles that can be pickpocketed are classes,
                                // but patch 1.07.212 allows pickpocketing of creatures.
                                // For now, the only enemy mobiles being allowed by DF Unity are classes.
                                if (mobileEnemyBehaviour && (mobileEnemyBehaviour.EntityType != EntityTypes.EnemyClass))
                                    break;
                                // Classic doesn't set any flag when pickpocketing enemy mobiles, so infinite attempts are possible
                                if (enemyEntity != null && !enemyEntity.PickpocketByPlayerAttempted)
                                {
                                    if (hit.distance > (PickpocketDistance * MeshReader.GlobalScale))
                                    {
                                        DaggerfallUI.SetMidScreenText(HardStrings.youAreTooFarAway);
                                        break;
                                    }
                                    enemyEntity.PickpocketByPlayerAttempted = true;
                                    Pickpocket(mobileEnemyBehaviour);
                                }
                                break;
                        }
                    }

                    // Check for functional interior furniture: Ladders, Bookshelves.
                    DaggerfallLadder ladder = hit.transform.GetComponent<DaggerfallLadder>();
                    DaggerfallBookshelf bookshelf = hit.transform.GetComponent<DaggerfallBookshelf>();

                    if (ladder || bookshelf)
                    {
                        if (hit.distance > (DefaultActivationDistance * MeshReader.GlobalScale))
                        {
                            DaggerfallUI.SetMidScreenText(HardStrings.youAreTooFarAway);
                            return;
                        }
                        if (ladder)
                        {   // Ladders:
                            ladder.ClimbLadder();
                        }
                        else if (bookshelf)
                        {   // Bookshelves:
                            bookshelf.ReadBook();
                        }
                    }
                    // Debug for identifying interior furniture model ids.
                    Debug.Log(hit.transform);

                    #endregion
                }
            }
        }

        // Custom transition to store building data before entering building
        private void TransitionInterior(Transform doorOwner, StaticDoor door, bool doFade = false)
        {
            // Get building directory for location
            BuildingDirectory buildingDirectory = GameManager.Instance.StreamingWorld.GetCurrentBuildingDirectory();
            if (!buildingDirectory)
            {
                Debug.LogError("PlayerActivate.TransitionInterior() could not retrieve BuildingDirectory.");
                return;
            }

            // Get building discovery data - this is added when player clicks door at exterior
            PlayerGPS.DiscoveredBuilding db;
            if (!GameManager.Instance.PlayerGPS.GetDiscoveredBuilding(door.buildingKey, out db))
            {
                Debug.LogErrorFormat("PlayerActivate.TransitionInterior() could not retrieve DiscoveredBuilding for key {0}.", door.buildingKey);
                return;
            }

            // Perform transition
            playerEnterExit.BuildingDiscoveryData = db;
            playerEnterExit.TransitionInterior(doorOwner, door, doFade);
        }

        // Message box closed, move to interior
        private void Popup_OnClose()
        {
            TransitionInterior(deferredInteriorDoorOwner, deferredInteriorDoor, true);
        }

        // Access wagon or dungeon exit
        private void DungeonWagonAccess_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.No)
            {
                playerEnterExit.TransitionDungeonExterior(true);
            }
            else
            {
                DaggerfallUI.Instance.InventoryWindow.AllowDungeonWagonAccess();
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenInventoryWindow);
            }
            sender.CloseWindow();
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

        // Check if raycast hit a mobile NPC
        private bool MobilePersonMotorCheck(RaycastHit hitInfo, out MobilePersonNPC mobileNPC)
        {
            mobileNPC = hitInfo.transform.GetComponent<MobilePersonNPC>();
            if (mobileNPC != null)
                return true;
            else
                return false;
        }

        // Check if raycast hit a mobile enemy
        private bool MobileEnemyCheck(RaycastHit hitInfo, out DaggerfallEntityBehaviour mobileEnemy)
        {
            mobileEnemy = hitInfo.transform.GetComponent<DaggerfallEntityBehaviour>();
            if (mobileEnemy != null)
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

        // Check if non-house building is unlocked and enterable
        private bool BuildingIsUnlocked(BuildingSummary buildingSummary)
        {
            bool unlocked = false;

            DFLocation.BuildingTypes type = buildingSummary.BuildingType;

            // TODO: Guild structures can become unlocked 24hr depending on player rank

            // Handle House1 through House4
            // TODO: Figure out the rest of house door calculations.
            if (type >= DFLocation.BuildingTypes.House1 && type <= DFLocation.BuildingTypes.House4
                && DaggerfallUnity.Instance.WorldTime.Now.IsDay)
            {
                unlocked = true;
            }
            // Handle other structures (stores, temples, taverns, palaces)
            else if (type <= DFLocation.BuildingTypes.Palace)
            {
                unlocked = (openHours[(int)type] <= DaggerfallUnity.Instance.WorldTime.Now.Hour
                    && closeHours[(int)type] > DaggerfallUnity.Instance.WorldTime.Now.Hour);
            }
            return unlocked;
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

        // Output NPC info to HUD
        private void PresentNPCInfo(StaticNPC npc)
        {
            DaggerfallUI.AddHUDText(HardStrings.youSee.Replace("%s", npc.DisplayName));

            // Add debug info
            if (DaggerfallUI.Instance.DaggerfallHUD.QuestDebugger.State != HUDQuestDebugger.DisplayState.Nothing)
            {
                // Get faction info of this NPC
                FactionFile.FactionData factionData;
                if (GameManager.Instance.PlayerEntity.FactionData.GetFactionData(npc.Data.factionID, out factionData))
                {
                    string debugInfo = string.Format("Debugger: Your reputation with this NPC is {0}.", factionData.rep);
                    DaggerfallUI.AddHUDText(debugInfo);
                }
            }
        }

        // Player has clicked a GameObject with a QuestResourceBehaviour attached
        bool TriggerQuestResourceBehaviourClick(QuestResourceBehaviour questResourceBehaviour)
        {
            // Handle typical quest resource click
            if (questResourceBehaviour)
                return questResourceBehaviour.DoClick();
            else
                return false;
        }

        // Player has clicked on a static NPC
        void StaticNPCClick(StaticNPC npc)
        {
            // Do nothing if no NPC passed or fade in progress
            // Quest machine does not tick while fading (to prevent things happening while screen is black)
            // But this can result in player clicking a quest NPC before quest state ticks after load and breaking quest
            if (!npc || DaggerfallUI.Instance.FadeInProgress)
                return;

            // Store the NPC just clicked in quest engine
            QuestMachine.Instance.LastNPCClicked = npc;

            // Check if this NPC is a quest giver and show temp guild quest popup
            // This will be changed later when temp guild system replaced with real thing
            if (QuestorCheck(npc))
                return;

            // Handle quest NPC click and exit if linked to a Person resource
            QuestResourceBehaviour questResourceBehaviour = npc.gameObject.GetComponent<QuestResourceBehaviour>();
            if (questResourceBehaviour)
            {
                if (TriggerQuestResourceBehaviourClick(questResourceBehaviour))
                    return;
            }

            // Do nothing further if a quest is actively listening on this individual NPC
            // This NPC not reserved as a Person resource but has a WhenNpcIsAvailable action listening on it
            // This effectively shuts down several named NPCs during main quest, but not trivial to otherwise determine appropriate access
            // TODO: Try to find a good solution for releasing listeners when the owning action is disabled
            if (QuestMachine.Instance.HasFactionListener(npc.Data.factionID))
                return;

            // Get faction data.
            FactionFile.FactionData factionData;
            if (playerEnterExit.IsPlayerInsideBuilding &&
                GameManager.Instance.PlayerEntity.FactionData.GetFactionData(npc.Data.factionID, out factionData))
            {
                UserInterfaceManager uiManager = DaggerfallUI.Instance.UserInterfaceManager;
                Debug.LogFormat("faction id: {0}, social group: {1}, guild: {2}", 
                    npc.Data.factionID, (FactionFile.SocialGroups)factionData.sgroup, (FactionFile.GuildGroups)factionData.ggroup);

                // Check if the NPC offers a guild service.
                if (Enum.IsDefined(typeof(GuildServices), npc.Data.factionID))
                {
                    FactionFile.GuildGroups guild = (FactionFile.GuildGroups)factionData.ggroup;
                    GuildServices service = (GuildServices)npc.Data.factionID;
                    Debug.Log("NPC offers guild service: " + service.ToString());
                    uiManager.PushWindow(new DaggerfallGuildServicePopupWindow(uiManager, npc, guild, service));
                }
                // Check if this NPC is a merchant.
                else if ((FactionFile.SocialGroups)factionData.sgroup == FactionFile.SocialGroups.Merchants)
                {
                    // Shop?
                    if (RMBLayout.IsShop(playerEnterExit.BuildingDiscoveryData.buildingType))
                    {
                        if (RMBLayout.IsRepairShop(playerEnterExit.BuildingDiscoveryData.buildingType))
                            uiManager.PushWindow(new DaggerfallMerchantRepairPopupWindow(uiManager, npc));
                        else
                            uiManager.PushWindow(new DaggerfallMerchantServicePopupWindow(uiManager, npc, DaggerfallMerchantServicePopupWindow.Services.Sell));
                    }
                    // Bank?
                    else if (playerEnterExit.BuildingDiscoveryData.buildingType == DFLocation.BuildingTypes.Bank)
                        uiManager.PushWindow(new DaggerfallMerchantServicePopupWindow(uiManager, npc, DaggerfallMerchantServicePopupWindow.Services.Banking));
                    // Tavern?
                    else if (playerEnterExit.BuildingDiscoveryData.buildingType == DFLocation.BuildingTypes.Tavern)
                        // for now only talk to all npc in taverns - TODO: add tavern option in here
                        GameManager.Instance.TalkManager.TalkToStaticNPC(npc);
                }
                // TODO - more checks for npc social types?
                else // if no special handling had to be done for npc with social group of type merchant: talk to the static npc
                {
                    GameManager.Instance.TalkManager.TalkToStaticNPC(npc);
                }
            }
            else // if no special handling had to be done (all remaining npcs of the remaining social groups not handled explicitely above): default is talk to the static npc
            {
                // with one exception: guards
                if (npc.Data.billboardArchiveIndex == 183 && npc.Data.billboardRecordIndex == 3) // detect if clicked guard (comment Nystul: didn't find a better mechanism than billboard texture check)
                    return; // if guard was clicked don't open talk window

                // otherwise open talk window
                GameManager.Instance.TalkManager.TalkToStaticNPC(npc);
            }
        }

        // Check if NPC is a Questor
        bool QuestorCheck(StaticNPC npc)
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
                return true;
            }
            return false;
        }

        // Player has clicked on a pickpocket target in steal mode
        void Pickpocket(DaggerfallEntityBehaviour target = null)
        {
            const int foundNothingValuableTextId = 8999;

            PlayerEntity player = GameManager.Instance.PlayerEntity;
            EnemyEntity enemyEntity = null;
            if (target != null)
                enemyEntity = target.Entity as EnemyEntity;
            player.TallySkill(DFCareer.Skills.Pickpocket, 1);

            int chance = Formulas.FormulaHelper.CalculatePickpocketingChance(player, enemyEntity);

            if (UnityEngine.Random.Range(0, 101) <= chance)
            {
                if (UnityEngine.Random.Range(0, 101) >= 33)
                {
                    int pinchedGoldPieces = UnityEngine.Random.Range(0, 6) + 1;
                    player.GoldPieces += pinchedGoldPieces;
                    string gotGold;
                    if (pinchedGoldPieces == 1)
                    {
                        // Classic doesn't have this string, it only has the plural one
                        gotGold = HardStrings.youPinchedGoldPiece;
                    }
                    else
                    {
                        gotGold = HardStrings.youPinchedGoldPieces;
                        gotGold = gotGold.Replace("%d", pinchedGoldPieces.ToString());
                    }
                    DaggerfallUI.MessageBox(gotGold);
                    //player.TallyCrimeGuildRequirements(true, 1);
                }
                else
                {
                    string noGoldFound = DaggerfallUnity.Instance.TextProvider.GetRandomText(foundNothingValuableTextId);
                    DaggerfallUI.MessageBox(noGoldFound, true);
                }
            }
            else
            {
                string notSuccessfulMessage = HardStrings.youAreNotSuccessful;
                DaggerfallUI.Instance.PopupMessage(notSuccessfulMessage);

                // Make enemies in an area aggressive if player failed to pickpocket a non-hostile one.
                EnemyMotor enemyMotor = null;
                if (target != null)
                    enemyMotor = target.GetComponent<EnemyMotor>();
                if (enemyMotor)
                {
                    if (!enemyMotor.IsHostile)
                    {
                        GameManager.Instance.MakeEnemiesHostile();
                    }
                    enemyMotor.MakeEnemyHostileToPlayer(gameObject);
                }
            }
        }       
    }
}