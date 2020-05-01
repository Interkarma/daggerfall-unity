// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Allofich, Numidium, TheLacus
// 
// Notes:
//

using UnityEngine;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Banking;
using DaggerfallWorkshop.Game.Guilds;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Utility.ModSupport;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Defines a <see cref="MonoBehaviour"/> component that can be activated by player interaction.
    /// Activation is detected when a ray cast hits a collider or trigger collider on the GameObject on which the component is instantiated.
    /// </summary>
    public interface IPlayerActivable
    {
        /// <summary>
        /// Fired when the player activate this object. This method can be called more than once if the collider is not disabled by implementation.
        /// </summary>
        /// <param name="hit">The hit that caused the activation.</param>
        void Activate(RaycastHit hit);
    }

    /// <summary>
    /// Example class to handle activation of doors, switches, etc. from Fire1 input.
    /// </summary>
    public class PlayerActivate : MonoBehaviour
    {
        PlayerGPS playerGPS;
        PlayerEnterExit playerEnterExit;        // Example component to enter/exit buildings
        GameObject mainCamera;
        int playerLayerMask = 0;

        Transform deferredInteriorDoorOwner;    // Used to defer interior transition after popup message
        StaticDoor deferredInteriorDoor;

        PlayerActivateModes currentMode = PlayerActivateModes.Grab;
        bool castPending = false;

        float clickDelay = 0;
        float clickDelayStartTime = 0;

        const float RayDistance = 3072 * MeshReader.GlobalScale;    // Classic's farthest view distance (outside, clear weather).
                                                                    // This is needed for using "Info" mode and clicking on buildings, which can
                                                                    // be done in classic for as far as the view distance.

        // Maximum distance from which different object types can be activated, converted from classic units (divided by 40)
        public const float DefaultActivationDistance = 128 * MeshReader.GlobalScale;
        public const float DoorActivationDistance = 128 * MeshReader.GlobalScale;
        public const float TreasureActivationDistance = 128 * MeshReader.GlobalScale;
        public const float PickpocketDistance = 128 * MeshReader.GlobalScale;
        public const float CorpseActivationDistance = 150 * MeshReader.GlobalScale;
        //const float TouchSpellActivationDistance = 160 * MeshReader.GlobalScale;
        public const float StaticNPCActivationDistance = 256 * MeshReader.GlobalScale;
        public const float MobileNPCActivationDistance = 256 * MeshReader.GlobalScale;

        // Opening and closing hours by building type
        static byte[] openHours  = {  7,  8,  9,  8,  0,  9, 10, 10,  9,  6,  9, 11,  9,  9,  0,  0, 10, 0 };
        static byte[] closeHours = { 22, 16, 19, 15, 25, 21, 19, 20, 18, 23, 23, 23, 20, 20, 25, 25, 16, 0 };

        const int PrivatePropertyId = 37;

        public PlayerActivateModes CurrentMode
        {
            get { return currentMode; }
        }

        // Public opening hours; Guilds' HallAccessAnytime can override that
        public static bool IsBuildingOpen(DFLocation.BuildingTypes buildingType)
        {
            return (openHours[(int)buildingType] <= DaggerfallUnity.Instance.WorldTime.Now.Hour &&
                    closeHours[(int)buildingType] > DaggerfallUnity.Instance.WorldTime.Now.Hour);
        }

        #region custom mod activation
        private struct CustomModActivation
        {
            internal readonly CustomActivation Action;

            internal readonly float ActivationDistance;
            internal readonly Mod Provider;

            internal CustomModActivation(CustomActivation action, float activationDistance, Mod provider)
            {
                Action = action;
                ActivationDistance = activationDistance;
                Provider = provider;
            }
        }
        readonly static Dictionary<string, CustomModActivation> customModActivations = new Dictionary<string, CustomModActivation>();
        // Allow mods to register custom flat / model activation methods.
        public delegate void CustomActivation(RaycastHit hit);

        /// <summary>
        /// Registers a custom activation for a model object. Uses the modelID parameter to retrieve the correct object name
        /// </summary>
        /// <param name="provider">The mod that provides this override; used to enforce load order.</param>
        /// <param name="modelID">The model ID of the object that will trigger the custom action upon activation.</param>
        /// <param name="customActivation">A callback that implements the custom action.</param>
        public static void RegisterCustomActivation(Mod provider, uint modelID, CustomActivation customActivation, float activationDistance = DefaultActivationDistance)
        {
            string goModelName = GameObjectHelper.GetGoModelName(modelID);
            HandleRegisterCustomActivation(provider, goModelName, customActivation, activationDistance);
        }

        /// <summary>
        /// Registers a custom activation for a flat object. Uses the textureArchive and textureRecord parameters to retrieve the correct object name
        /// </summary>
        /// <param name="provider">The mod that provides this override; used to enforce load order.</param>
        /// <param name="textureArchive">The texture archive of the flat object that will trigger the custom action upon activation.</param>
        /// <param name="textureRecord">The texture record of the flat object that will trigger the custom action upon activation.</param>
        /// <param name="customActivation">A callback that implements the custom action.</param>
        public static void RegisterCustomActivation(Mod provider, int textureArchive, int textureRecord, CustomActivation customActivation, float activationDistance = DefaultActivationDistance)
        {
            string goFlatName = GameObjectHelper.GetGoFlatName(textureArchive, textureRecord);
            HandleRegisterCustomActivation(provider, goFlatName, customActivation, activationDistance);
        }

        /// <summary>
        /// Registers a custom activation for a flat object
        /// </summary>
        /// <param name="provider">The mod that provides this override; used to enforce load order.</param>
        /// <param name="textureArchive">The texture archive of the flat object that will trigger the custom action upon activation.</param>
        /// <param name="textureRecord">The texture record of the flat object that will trigger the custom action upon activation.</param>
        /// <param name="customActivation">A callback that implements the custom action.</param>
        private static void HandleRegisterCustomActivation(Mod provider, string goFlatModelName, CustomActivation customActivation, float activationDistance)
        {
            DaggerfallUnity.LogMessage("HandleRegisterCustomActivation: " + goFlatModelName, true);
            CustomModActivation existingActivation;
            if (customModActivations.TryGetValue(goFlatModelName, out existingActivation) && existingActivation.Provider.LoadPriority > provider.LoadPriority) {
                Debug.Log("Denied custom activation registration from " + provider.Title + " for " + goFlatModelName + " | " + existingActivation.Provider.Title + " has higher load priority");
            } else {
                customModActivations[goFlatModelName] = new CustomModActivation(customActivation, activationDistance, provider);
            }
        }

        /// <summary>
        /// Checks if a model object has a custom activation assigned
        /// </summary>
        /// <param name="modelID">The model ID of the object to check.</param>
        public static bool HasCustomActivation(uint modelID)
        {
            string goModelName = GameObjectHelper.GetGoModelName(modelID);
            return HasCustomActivation(goModelName);
        }

        /// <summary>
        /// Checks if a model object has a custom activation assigned
        /// </summary>
        /// <param name="textureArchive">The texture archive of the flat object to check.</param>
        /// <param name="textureRecord">The texture record of the flat object to check.</param>
        public static bool HasCustomActivation(int textureArchive, int textureRecord)
        {
            string goFlatName = GameObjectHelper.GetGoFlatName(textureArchive, textureRecord);
            return HasCustomActivation(goFlatName);
        }

        /// <summary>
        /// Checks if an object has a custom activation assigned
        /// </summary>
        /// <param name="goFlatModelName">The name of the flat / model object to check.</param>
        public static bool HasCustomActivation(string goFlatModelName) {
            return customModActivations.ContainsKey(goFlatModelName);
        }
        #endregion

        void Start()
        {
            playerGPS = GetComponent<PlayerGPS>();
            playerEnterExit = GetComponent<PlayerEnterExit>();
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            playerLayerMask = ~(1 << LayerMask.NameToLayer("Player"));
        }

        void Update()
        {
            if (mainCamera == null)
                return;

            // Do nothing further if player has spell ready to cast as activate button is now used to fire spell
            // The exception is a readied touch spell where player can activate doors, etc.
            // Touch spells only fire once a target entity is in range
            if (GameManager.Instance.PlayerEffectManager)
            {
                // Handle pending spell cast
                if (GameManager.Instance.PlayerEffectManager.HasReadySpell)
                {
                    // Exclude touch spells from this check
                    MagicAndEffects.EntityEffectBundle spell = GameManager.Instance.PlayerEffectManager.ReadySpell;
                    if (spell.Settings.TargetType != MagicAndEffects.TargetTypes.ByTouch)
                    {
                        castPending = true;
                        return;
                    }
                }

                // Prevents last spell cast click from falling through to normal click handling this frame
                if (castPending)
                {
                    castPending = false;
                    return;
                }
            }

            // Change activate mode
            if (InputManager.Instance.ActionStarted(InputManager.Actions.StealMode))
                ChangeInteractionMode(PlayerActivateModes.Steal);
            else if (InputManager.Instance.ActionStarted(InputManager.Actions.GrabMode))
                ChangeInteractionMode(PlayerActivateModes.Grab);
            else if (InputManager.Instance.ActionStarted(InputManager.Actions.InfoMode))
                ChangeInteractionMode(PlayerActivateModes.Info);
            else if (InputManager.Instance.ActionStarted(InputManager.Actions.TalkMode))
                ChangeInteractionMode(PlayerActivateModes.Talk);

            // Handle click delay
            if (clickDelay > 0 && Time.realtimeSinceStartup < clickDelayStartTime + clickDelay)
            {
                return;
            }
            else
            {
                clickDelay = 0;
                clickDelayStartTime = 0;
            }

            // Fire ray into scene
            if (InputManager.Instance.ActionComplete(InputManager.Actions.ActivateCenterObject))
            {
                // Fire ray into scene for hit tests (excluding player so their ray does not intersect self)
                Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
                RaycastHit hit;
                bool hitSomething = Physics.Raycast(ray, out hit, RayDistance, playerLayerMask);
                // Debug.DrawRay(ray.origin, ray.direction, Color.yellow, 2f);
                if (hitSomething)
                {
                    bool hitBuilding = false;
                    bool buildingUnlocked = false;
                    int buildingLockValue = 0;
                    DFLocation.BuildingTypes buildingType = DFLocation.BuildingTypes.AllValid;
                    StaticBuilding building = new StaticBuilding();

                    #region Hit Checks
                    // Trigger quest resource behaviour click on anything but NPCs
                    QuestResourceBehaviour questResourceBehaviour;
                    if (QuestResourceBehaviourCheck(hit, out questResourceBehaviour) && !(questResourceBehaviour.TargetResource is Person))
                    {
                        if (hit.distance > DefaultActivationDistance)
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

                    // Check for a static building hit
                    Transform buildingOwner;
                    DaggerfallStaticBuildings buildings = GetBuildings(hit.transform, out buildingOwner);
                    if (buildings && buildings.HasHit(hit.point, out building))
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

                        buildingUnlocked = BuildingIsUnlocked(buildingSummary);
                        buildingLockValue = GetBuildingLockValue(buildingSummary);
                        buildingType = buildingSummary.BuildingType;
                        ActivateBuilding(building, buildingType, buildingUnlocked);
                    }

                    // Check for a static door hit
                    Transform doorOwner;
                    DaggerfallStaticDoors doors = GetDoors(hit.transform, out doorOwner);
                    if (doors && playerEnterExit)
                    {
                        ActivateStaticDoor(doors, hit, hitBuilding, building, buildingType, buildingUnlocked, buildingLockValue, doorOwner);
                    }

                    // Check for an action door hit
                    DaggerfallActionDoor actionDoor;
                    if (ActionDoorCheck(hit, out actionDoor))
                    {
                        ActivateActionDoor(hit, actionDoor);
                    }

                    // Check for action record hit
                    DaggerfallAction action;
                    if (ActionCheck(hit, out action) && hit.distance <= DefaultActivationDistance)
                    {
                        action.Receive(this.gameObject, DaggerfallAction.TriggerTypes.Direct);
                    }

                    // Check for lootable object hit
                    DaggerfallLoot loot;
                    if (LootCheck(hit, out loot))
                    {
                        ActivateLootContainer(hit, loot);
                    }

                    // Check for bulletin board object hit
                    DaggerfallBulletinBoard bulletinBoard;
                    if (BulletinBoardCheck(hit, out bulletinBoard))
                    {
                        Debug.Log("Player clicked bulletin board");
                    }

                    // Check for static NPC hit
                    StaticNPC npc;
                    if (NPCCheck(hit, out npc))
                    {
                        ActivateStaticNPC(hit, npc);
                    }

                    // Check for mobile NPC hit
                    MobilePersonNPC mobileNpc = null;
                    if (MobilePersonMotorCheck(hit, out mobileNpc))
                    {
                        ActivateMobileNPC(hit, mobileNpc);
                    }

                    // Check for mobile enemy hit
                    DaggerfallEntityBehaviour mobileEnemyBehaviour;
                    if (MobileEnemyCheck(hit, out mobileEnemyBehaviour))
                    {
                        ActivateMobileEnemy(hit, mobileEnemyBehaviour);
                    }

                    // Check for functional interior furniture: Ladders, Bookshelves.
                    ActivateLaddersAndShelves(hit);

                    // Invoke any matched custom flat / model activations registered by mods.
                    string flatModelName = hit.transform.gameObject.name;
                    int pos = flatModelName.IndexOf(']');
                    if (pos > 0 && pos < flatModelName.Length - 1)
                        flatModelName = flatModelName.Remove(pos + 1);

                    CustomModActivation customActivation;
                    if (customModActivations.TryGetValue(flatModelName, out customActivation))
                    {
                        if(hit.distance <= customActivation.ActivationDistance) {
                            customActivation.Action(hit);
                        }
                    }

                    // Check for custom activation
                    var playerActivable = hit.transform.GetComponent<IPlayerActivable>();
                    if (playerActivable != null)
                        playerActivable.Activate(hit);

                    // Debug for identifying interior furniture model ids.
                    Debug.Log(string.Format("hit='{0}' static={1}", hit.transform, GameObjectHelper.IsStaticGeometry(hit.transform.gameObject)));
                    #endregion
                }
            }
        }

        #region Activation Logic

        void ActivateBuilding(
            StaticBuilding building,
            DFLocation.BuildingTypes buildingType,
            bool buildingUnlocked)
        {
            if (currentMode == PlayerActivateModes.Info)
            {
                // Discover building
                GameManager.Instance.PlayerGPS.DiscoverBuilding(building.buildingKey);

                // Get discovered building
                PlayerGPS.DiscoveredBuilding db;
                if (GameManager.Instance.PlayerGPS.GetDiscoveredBuilding(building.buildingKey, out db))
                {
                    // Check against quest system for an overriding quest-assigned display name for this building
                    DaggerfallUI.AddHUDText(db.displayName);

                    if (!buildingUnlocked && buildingType < DFLocation.BuildingTypes.Temple
                        && buildingType != DFLocation.BuildingTypes.HouseForSale)
                    {
                        string buildingClosedMessage = (buildingType == DFLocation.BuildingTypes.GuildHall) ? HardStrings.guildClosed : HardStrings.storeClosed;
                        buildingClosedMessage = buildingClosedMessage.Replace("%d1", openHours[(int)buildingType].ToString());
                        buildingClosedMessage = buildingClosedMessage.Replace("%d2", closeHours[(int)buildingType].ToString());
                        DaggerfallUI.Instance.PopupMessage(buildingClosedMessage);
                    }
                }
            }
        }

        void ActivateStaticDoor(
            DaggerfallStaticDoors doors,
            RaycastHit hit,
            bool hitBuilding,
            StaticBuilding building,
            DFLocation.BuildingTypes buildingType,
            bool buildingUnlocked,
            int buildingLockValue,
            Transform doorOwner)
        {
            StaticDoor door;
            if (doors.HasHit(hit.point, out door) || CustomDoor.HasHit(hit, out door))
            {
                // Check if close enough to activate
                if (hit.distance > DoorActivationDistance)
                {
                    DaggerfallUI.SetMidScreenText(HardStrings.youAreTooFarAway);
                    return;
                }

                if (door.doorType == DoorTypes.Building && !playerEnterExit.IsPlayerInside)
                {
                    // Discover building
                    GameManager.Instance.PlayerGPS.DiscoverBuilding(building.buildingKey);

                    // Handle clicking exterior door with Open spell active
                    if (HandleOpenEffectOnExteriorDoor(buildingLockValue))
                        buildingUnlocked = true;

                    // TODO: Implement lockpicking and door bashing for exterior doors
                    // For now, any locked building door can be entered by using steal mode
                    if (!buildingUnlocked)
                    {
                        if (currentMode != PlayerActivateModes.Steal)
                        {
                            DaggerfallUI.Instance.PopupMessage(TextManager.Instance.GetText("GeneralText", "lockedExteriorDoor"));
                            LookAtInteriorLock(buildingLockValue);
                            return;
                        }
                        else // Breaking into building
                        {
                            // Reject if player has already failed this building at current skill level
                            PlayerEntity player = GameManager.Instance.PlayerEntity;
                            int skillValue = player.Skills.GetLiveSkillValue(DFCareer.Skills.Lockpicking);
                            int lastAttempt = GameManager.Instance.PlayerGPS.GetLastLockpickAttempt(building.buildingKey);
                            if (skillValue <= lastAttempt)
                            {
                                LookAtInteriorLock(buildingLockValue);
                                return;
                            }

                            // Attempt to unlock building
                            Random.InitState(Time.frameCount);
                            player.TallySkill(DFCareer.Skills.Lockpicking, 1);
                            int chance = FormulaHelper.CalculateExteriorLockpickingChance(buildingLockValue, skillValue);
                            int roll = Random.Range(1, 101);
                            Debug.LogFormat("Attempting pick against lock strength {0}. Chance={1}, Roll={2}.", buildingLockValue, chance, roll);
                            if (chance > roll)
                            {
                                // Show success and play unlock sound
                                player.TallyCrimeGuildRequirements(true, 1);
                                DaggerfallUI.Instance.PopupMessage(HardStrings.lockpickingSuccess);
                                DaggerfallAudioSource dfAudioSource = GetComponent<DaggerfallAudioSource>();
                                if (dfAudioSource != null)
                                    dfAudioSource.PlayOneShot(SoundClips.ActivateLockUnlock);
                            }
                            else
                            {
                                // Show failure and record attempt skill level in discovery data
                                // Have not been able to create a guard response in classic, even when early morning NPCs are nearby
                                // Assuming for now that exterior lockpicking is discrete enough that no response on failure is required
                                DaggerfallUI.Instance.PopupMessage(HardStrings.lockpickingFailure);
                                GameManager.Instance.PlayerGPS.SetLastLockpickAttempt(building.buildingKey, skillValue);
                                return;
                            }
                        }
                    }

                    // If entering a shop let player know the quality level
                    // If entering an open home, show greeting
                    if (hitBuilding)
                    {
                        const int houseGreetingsTextId = 256;

                        DaggerfallMessageBox mb;

                        PlayerGPS.DiscoveredBuilding buildingData;
                        GameManager.Instance.PlayerGPS.GetDiscoveredBuilding(building.buildingKey, out buildingData);

                        if (buildingUnlocked &&
                            buildingType >= DFLocation.BuildingTypes.House1 &&
                            buildingType <= DFLocation.BuildingTypes.House4 &&
                            buildingData.factionID != (int)FactionFile.FactionIDs.The_Thieves_Guild &&
                            buildingData.factionID != (int)FactionFile.FactionIDs.The_Dark_Brotherhood &&
                            !DaggerfallBankManager.IsHouseOwned(building.buildingKey))
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
                            mb.OnClose += BuildingGreetingPopup_OnClose;
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
                    if (GameManager.Instance.PlayerEntity.Items.Contains(ItemGroups.Transportation, (int)Transportation.Small_cart) && DaggerfallUnity.Settings.DungeonExitWagonPrompt)
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

        int GetBuildingLockValue(int quality)
        {
            // Currently unknown how classic calculates building lock value but suspect related to building quality level
            // No exterior buildings are known to have magically held locks, so 20 quality buildings (e.g. The Odd Blades) must have a lower lock value
            // Using building quality / 2 for now until a more accurate method is known

            return quality / 2;
        }

        int GetBuildingLockValue(BuildingSummary buildingSummary)
        {
            return GetBuildingLockValue(buildingSummary.Quality);
        }

        void ActivateActionDoor(RaycastHit hit, DaggerfallActionDoor actionDoor)
        {
            // Check if close enough to activate
            if (hit.distance > DoorActivationDistance)
            {
                DaggerfallUI.SetMidScreenText(HardStrings.youAreTooFarAway);
                return;
            }

            // Handle Lock/Open spell effects (if active)
            if (HandleLockEffect(actionDoor))
                return;
            if (HandleOpenEffect(actionDoor))
                return;

            if (currentMode == PlayerActivateModes.Steal && actionDoor.IsLocked && !actionDoor.IsOpen)
            {
                actionDoor.AttemptLockpicking();
            }
            else
                actionDoor.ToggleDoor(true);
        }

        void ActivateStaticNPC(RaycastHit hit, StaticNPC npc)
        {
            // Do not activate static NPCs carrying specific non-dialog actions as these usually have some bespoke task to perform
            // Note: currently only ShowText and ShowTextWithInput NPCs are excluded
            // Examples are guard at entrance of Daggerfall Castle and Benefactor and Sheogorath in Mantellan Crux
            DaggerfallAction action = npc.GetComponent<DaggerfallAction>();
            if (action &&
                (action.ActionFlag == DFBlock.RdbActionFlags.ShowTextWithInput ||
                 action.ActionFlag == DFBlock.RdbActionFlags.ShowText))
                return;

            switch (currentMode)
            {
                case PlayerActivateModes.Info:
                    PresentNPCInfo(npc);
                    break;
                case PlayerActivateModes.Grab:
                case PlayerActivateModes.Talk:
                case PlayerActivateModes.Steal:
                    if (hit.distance > StaticNPCActivationDistance)
                    {
                        DaggerfallUI.SetMidScreenText(HardStrings.youAreTooFarAway);
                        break;
                    }
                    StaticNPCClick(npc);
                    break;
            }
        }

        void ActivateMobileNPC(RaycastHit hit, MobilePersonNPC mobileNpc)
        {
            switch (currentMode)
            {
                case PlayerActivateModes.Info:
                case PlayerActivateModes.Grab:
                case PlayerActivateModes.Talk:
                    if (hit.distance > MobileNPCActivationDistance)
                    {
                        DaggerfallUI.SetMidScreenText(HardStrings.youAreTooFarAway);
                        break;
                    }
                    GameManager.Instance.TalkManager.TalkToMobileNPC(mobileNpc);
                    break;
                case PlayerActivateModes.Steal:
                    if (!mobileNpc.PickpocketByPlayerAttempted)
                    {
                        if (hit.distance > PickpocketDistance)
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

        void ActivateMobileEnemy(RaycastHit hit, DaggerfallEntityBehaviour mobileEnemyBehaviour)
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
                        if (hit.distance > PickpocketDistance)
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

        void ActivateLaddersAndShelves(RaycastHit hit)
        {
            DaggerfallLadder ladder = hit.transform.GetComponent<DaggerfallLadder>();
            DaggerfallBookshelf bookshelf = hit.transform.GetComponent<DaggerfallBookshelf>();
            if (ladder || bookshelf)
            {
                if (hit.distance > DefaultActivationDistance)
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
        }

        void ActivateLootContainer(RaycastHit hit, DaggerfallLoot loot)
        {
            // Check if close enough to activate for all types, except for corpses
            if (loot.ContainerType != LootContainerTypes.CorpseMarker &&
                hit.distance > TreasureActivationDistance)
            {
                DaggerfallUI.SetMidScreenText(HardStrings.youAreTooFarAway);
                return;
            }
            Random.InitState(Time.frameCount);
            UserInterfaceManager uiManager = DaggerfallUI.Instance.UserInterfaceManager;
            switch (loot.ContainerType)
            {
                // Handle shop shelves: stock shelf if needed, then open trade window with activated loot container as remote target
                case LootContainerTypes.ShopShelves:
                    // Stock shop shelf on first access
                    if (loot.stockedDate < DaggerfallLoot.CreateStockedDate(DaggerfallUnity.Instance.WorldTime.Now))
                        loot.StockShopShelf(playerEnterExit.BuildingDiscoveryData);
                    // Open Trade Window if shop is open
                    if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideOpenShop)
                    {
                        DaggerfallTradeWindow tradeWindow = (DaggerfallTradeWindow) UIWindowFactory.GetInstanceWithArgs(UIWindowType.Trade, new object[] { uiManager, null, DaggerfallTradeWindow.WindowModes.Buy, null });
                        tradeWindow.MerchantItems = loot.Items;
                        uiManager.PushWindow(tradeWindow);
                        return;
                    }
                    else
                    {   // Open Inventory if shop closed, i.e. stealing
                        DaggerfallUI.Instance.InventoryWindow.SetShopShelfStealing();
                        break;
                    }

                // Handle house furniture containers: ask player if they want to look through private property
                case LootContainerTypes.HouseContainers:
                    // Allow access for player owned interiors. (not distinguishing between ships)
                    if ((playerEnterExit.BuildingType == DFLocation.BuildingTypes.Ship && DaggerfallBankManager.OwnsShip) ||
                        DaggerfallBankManager.IsHouseOwned(playerEnterExit.BuildingDiscoveryData.buildingKey))
                    {
                        loot.stockedDate = 1;   // Ensure it gets serialized
                        break;
                    }
                    // Stock house container on first access
                    if (loot.stockedDate < DaggerfallLoot.CreateStockedDate(DaggerfallUnity.Instance.WorldTime.Now))
                        loot.StockHouseContainer(playerEnterExit.BuildingDiscoveryData);
                    // If no contents, do nothing
                    if (loot.Items.Count == 0)
                        return;
                    loot.houseOwned = true;
                    DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, DaggerfallMessageBox.CommonMessageBoxButtons.YesNo, PrivatePropertyId, uiManager.TopWindow);
                    messageBox.OnButtonClick += PrivateProperty_OnButtonClick;
                    uiManager.PushWindow(messageBox);
                    DaggerfallUI.Instance.InventoryWindow.LootTarget = loot;
                    return;

                // Handle corpses: info mode gives a description
                case LootContainerTypes.CorpseMarker:
                    if (currentMode == PlayerActivateModes.Info)
                    {   // Corpse info mode
                        if (!string.IsNullOrEmpty(loot.entityName))
                            DaggerfallUI.AddHUDText((loot.isEnemyClass) ? HardStrings.youSeeADeadPerson : HardStrings.youSeeADead.Replace("%s", loot.entityName));
                        return;
                    }
                    else
                    {   // Check if close enough to activate and that corpse has items
                        if (hit.distance > CorpseActivationDistance)
                        {
                            DaggerfallUI.SetMidScreenText(HardStrings.youAreTooFarAway);
                            return;
                        }
                        else if (loot.Items.Count == 0)
                        {
                            DaggerfallUI.AddHUDText(HardStrings.theBodyHasNoTreasure);
                            DisableEmptyCorpseContainer(loot.gameObject);
                            return;
                        }
                        else if (loot.Items.Count == 1 && loot.Items.Contains(ItemGroups.Weapons, (int)Weapons.Arrow))
                        {   // If only one item and it's arrows, then auto-pickup.
                            GameManager.Instance.PlayerEntity.Items.TransferAll(loot.Items);
                            DaggerfallUI.AddHUDText(HardStrings.youCollectArrows);
                            return;
                        }
                        break;
                    }
                    // No special handling for all other loot container types: (Nothing, RandomTreasure, DroppedLoot)
            }
            // Open inventory window with activated loot container as remote target (if we fall through to here)
            DaggerfallUI.Instance.InventoryWindow.LootTarget = loot;
            DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenInventoryWindow);
        }

        void DisableEmptyCorpseContainer(GameObject go)
        {
            if (go)
            {
                SphereCollider sphereCollider = go.GetComponent<SphereCollider>();
                if (sphereCollider)
                    sphereCollider.enabled = false;
            }
        }

        #endregion

        #region Public Static Methods

        public static void LookAtInteriorLock(int lockValue)
        {
            if (lockValue < 20)
            {
                PlayerEntity player = Game.GameManager.Instance.PlayerEntity;
                // There seems to be an oversight in classic. It uses two separate lockpicking functions (seems to be one for animated doors in interiors and one for exterior doors)
                // but the difficulty text is always based on the exterior function.
                // DF Unity doesn't have exterior locked doors yet, so the below uses the interior function.
                // TODO: Implement LookAtExteriorLock variant for exterior doors
                int chance = FormulaHelper.CalculateInteriorLockpickingChance(player.Level, lockValue, player.Skills.GetLiveSkillValue(DFCareer.Skills.Lockpicking));

                if (chance >= 30)
                    if (chance >= 35)
                        if (chance >= 95)
                            Game.DaggerfallUI.SetMidScreenText(HardStrings.lockpickChance[9]);
                        else if (chance >= 45)
                            Game.DaggerfallUI.SetMidScreenText(HardStrings.lockpickChance[(chance - 45) / 5]);
                        else
                            Game.DaggerfallUI.SetMidScreenText(HardStrings.lockpickChance3);
                    else
                        Game.DaggerfallUI.SetMidScreenText(HardStrings.lockpickChance2);
                else
                    Game.DaggerfallUI.SetMidScreenText(HardStrings.lockpickChance1);
            }
            else
                Game.DaggerfallUI.SetMidScreenText(HardStrings.magicLock);
        }

        #endregion

        bool HandleLockEffect(DaggerfallActionDoor actionDoor)
        {
            // Check if player has Lock effect running
            Lock lockEffect = (Lock)GameManager.Instance.PlayerEffectManager.FindIncumbentEffect<Lock>();
            if (lockEffect == null)
                return false;

            lockEffect.TriggerLockEffect(actionDoor);
            return true;
        }

        bool HandleOpenEffect(DaggerfallActionDoor actionDoor)
        {
            // Check if player has Open effect running
            Open openEffect = (Open)GameManager.Instance.PlayerEffectManager.FindIncumbentEffect<Open>();
            if (openEffect == null)
                return false;

            openEffect.TriggerOpenEffect(actionDoor);
            return true;
        }

        // NOTE: Open effect currently ALWAYS works on exterior doors, should operate on lock level
        // Lockpick and lock level not fully implemented on exterior doors
        bool HandleOpenEffectOnExteriorDoor(int buildingLockValue)
        {
            // Check if player has Open effect running
            Open openEffect = (Open)GameManager.Instance.PlayerEffectManager.FindIncumbentEffect<Open>();
            if (openEffect == null)
                return false;

            // Cancel effect
            openEffect.CancelEffect();

            // Player level must meet or exceed lock level for success
            if (GameManager.Instance.PlayerEntity.Level < buildingLockValue)
            {
                DaggerfallUI.AddHUDText(TextManager.Instance.GetText("ClassicEffects", "openFailed"), 1.5f);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Set a click delay before new clicks are accepted, usually when exiting UI.
        /// </summary>
        /// <param name="delay">Delay in seconds. Valid range is 0.0 - 1.0</param>
        public void SetClickDelay(float delay = 0.3f)
        {
            clickDelay = Mathf.Clamp01(delay);
            clickDelayStartTime = Time.realtimeSinceStartup;
        }

        public bool AttemptExteriorDoorBash(RaycastHit hit)
        {
            Transform doorOwner;
            DaggerfallStaticDoors doors = GetDoors(hit.transform, out doorOwner);
            StaticDoor door;
            if (doors && doors.HasHit(hit.point, out door))
            {
                // Discover building - this is needed to check lock level and transition to interior
                GameManager.Instance.PlayerGPS.DiscoverBuilding(door.buildingKey);

                // Play bashing sound
                DaggerfallAudioSource dfAudioSource = GetComponent<DaggerfallAudioSource>();
                if (dfAudioSource != null)
                    dfAudioSource.PlayOneShot(SoundClips.PlayerDoorBash);

                // Get lock value from discovered building
                int lockValue = 0;
                PlayerGPS.DiscoveredBuilding discoveredBuilding;
                if (GameManager.Instance.PlayerGPS.GetDiscoveredBuilding(door.buildingKey, out discoveredBuilding))
                    lockValue = GetBuildingLockValue(discoveredBuilding.quality);

                // Roll for chance to open - Lower lock values have a higher chance
                PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
                Random.InitState(Time.frameCount);
                int chance = 25 - lockValue;
                if (Dice100.SuccessRoll(chance))
                {
                    // Success - player has forced their way into building
                    playerEntity.TallyCrimeGuildRequirements(true, 1);
                    TransitionInterior(doorOwner, door, true);
                    return true;
                }
                else
                {
                    // Bashing doors in cities is a crime - 10% chance of summoning guards on each failed bash attempt
                    if (Dice100.SuccessRoll(10))
                    {
                        Debug.Log("Breaking and entering detected - spawning city guards.");
                        playerEntity.CrimeCommitted = PlayerEntity.Crimes.Attempted_Breaking_And_Entering;
                        playerEntity.SpawnCityGuards(true);
                    }
                }
            }
            return false;
        }

        public void PrivateProperty_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            sender.CloseWindow();
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                // Open inventory window with activated private container as remote target (pre-set)
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenInventoryWindow);
            }
            else
                DaggerfallUI.Instance.InventoryWindow.LootTarget = null;
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
            playerEnterExit.IsPlayerInsideOpenShop = RMBLayout.IsShop(db.buildingType) && IsBuildingOpen(db.buildingType);
            playerEnterExit.TransitionInterior(doorOwner, door, doFade, false);
        }

        // Message box closed, move to interior
        private void BuildingGreetingPopup_OnClose()
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
        private DaggerfallStaticBuildings GetBuildings(Transform buildingsTransform, out Transform owner)
        {
            owner = null;
            DaggerfallStaticBuildings buildings = buildingsTransform.GetComponent<DaggerfallStaticBuildings>();
            if (!buildings)
            {
                buildings = buildingsTransform.GetComponentInParent<DaggerfallStaticBuildings>();
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
        private DaggerfallStaticDoors GetDoors(Transform doorsTransform, out Transform owner)
        {
            owner = null;
            DaggerfallStaticDoors doors = doorsTransform.GetComponent<DaggerfallStaticDoors>();
            if (!doors)
            {
                doors = doorsTransform.GetComponentInParent<DaggerfallStaticDoors>();
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

            return door != null;
        }

        // Check if raycast hit an action door
        private bool ActionDoorCheck(RaycastHit hitInfo, out DaggerfallActionDoor door)
        {
            door = hitInfo.transform.GetComponent<DaggerfallActionDoor>();

            return door != null;
        }

        // Check if raycast hit a generic action component
        private bool ActionCheck(RaycastHit hitInfo, out DaggerfallAction action)
        {
            // Look for action
            action = hitInfo.transform.GetComponent<DaggerfallAction>();

            return action != null;
        }

        // Check if raycast hit a lootable object
        private bool LootCheck(RaycastHit hitInfo, out DaggerfallLoot loot)
        {
            loot = hitInfo.transform.GetComponent<DaggerfallLoot>();

            return loot != null;
        }

        // Check if raycast hit a bulletin board
        private bool BulletinBoardCheck(RaycastHit hitInfo, out DaggerfallBulletinBoard bulletinBoard)
        {
            bulletinBoard = hitInfo.transform.GetComponent<DaggerfallBulletinBoard>();

            return bulletinBoard != null;
        }

        // Check if raycast hit a StaticNPC
        private bool NPCCheck(RaycastHit hitInfo, out StaticNPC staticNPC)
        {
            staticNPC = hitInfo.transform.GetComponent<StaticNPC>();

            return staticNPC != null;
        }

        // Check if raycast hit a mobile NPC
        private bool MobilePersonMotorCheck(RaycastHit hitInfo, out MobilePersonNPC mobileNPC)
        {
            mobileNPC = hitInfo.transform.GetComponent<MobilePersonNPC>();

            return mobileNPC != null;
        }

        // Check if raycast hit a mobile enemy
        private bool MobileEnemyCheck(RaycastHit hitInfo, out DaggerfallEntityBehaviour mobileEnemy)
        {
            mobileEnemy = hitInfo.transform.GetComponent<DaggerfallEntityBehaviour>();

            return mobileEnemy != null;
        }

        // Check if raycast hit a QuestResource
        private bool QuestResourceBehaviourCheck(RaycastHit hitInfo, out QuestResourceBehaviour questResourceBehaviour)
        {
            questResourceBehaviour = hitInfo.transform.GetComponent<QuestResourceBehaviour>();

            return questResourceBehaviour != null;
        }

        // Check if building is unlocked and enterable
        private bool BuildingIsUnlocked(BuildingSummary buildingSummary)
        {
            // Player owned house is always unlocked
            if (DaggerfallBankManager.IsHouseOwned(buildingSummary.buildingKey))
                return true;

            // Buildings part of an active quest are always unlocked
            if (IsActiveQuestBuilding(buildingSummary))
                return true;

            bool unlocked = false;

            DFLocation.BuildingTypes type = buildingSummary.BuildingType;
            Debug.LogFormat("type: {0}, factionId: {1}", type, buildingSummary.FactionId);

            // Handle guild halls
            if (type == DFLocation.BuildingTypes.GuildHall)
            {
                IGuild guild = GameManager.Instance.GuildManager.GetGuild(buildingSummary.FactionId);
                unlocked = guild.HallAccessAnytime() || IsBuildingOpen(type);
            }
            // Handle TG/DB houses
            else if (type == DFLocation.BuildingTypes.House2 && buildingSummary.FactionId != 0)
            {
                IGuild guild = GameManager.Instance.GuildManager.GetGuild(buildingSummary.FactionId);
                unlocked = guild.IsMember();
            }
            // Handle House1 through House4
            // TODO: Figure out the rest of house door calculations.
            // TODO: Need to lock doors if quest target for stealing, and unlock for other quests.
            else if (type >= DFLocation.BuildingTypes.House1 && type <= DFLocation.BuildingTypes.House4
                && DaggerfallUnity.Instance.WorldTime.Now.IsDay)
            {
                unlocked = true;
            }
            // Handle stores
            else if (RMBLayout.IsShop(type))
            {
                uint minutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
                int holidayId = Formulas.FormulaHelper.GetHolidayId(minutes, GameManager.Instance.PlayerGPS.CurrentRegionIndex);
                if (holidayId == (int)DFLocation.Holidays.Suns_Rest)
                    unlocked = false;   // Shops are closed on Suns Rest holiday
                else
                    unlocked = IsBuildingOpen(type);
            }
            // Handle other structures (temples, taverns, palaces)
            else if (type <= DFLocation.BuildingTypes.Palace)
            {
                unlocked = IsBuildingOpen(type);
            }
            else if (type == DFLocation.BuildingTypes.Ship && DaggerfallBankManager.OwnsShip)
                unlocked = true;

            return unlocked;
        }

        // Check if building is used in an active quest inside current map
        public bool IsActiveQuestBuilding(BuildingSummary buildingSummary, bool residencesOnly = true)
        {
            SiteDetails[] siteDetails = QuestMachine.Instance.GetAllActiveQuestSites();
            foreach (SiteDetails site in siteDetails)
            {
                if (residencesOnly &&
                    (buildingSummary.BuildingType < DFLocation.BuildingTypes.House1 || buildingSummary.BuildingType > DFLocation.BuildingTypes.House6))
                    continue;
                if (site.buildingKey == buildingSummary.buildingKey &&
                    site.mapId == GameManager.Instance.PlayerGPS.CurrentMapID)
                    return true;
            }

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
            switch (currentMode)
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
            if (!npc || DaggerfallUI.Instance.FadeBehaviour.FadeInProgress)
                return;

            // Store the NPC just clicked in quest engine
            QuestMachine.Instance.LastNPCClicked = npc;

            // Handle quest NPC click and exit if linked to a Person resource
            QuestResourceBehaviour questResourceBehaviour = npc.gameObject.GetComponent<QuestResourceBehaviour>();
            if (questResourceBehaviour && TriggerQuestResourceBehaviourClick(questResourceBehaviour))
            {
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
            TalkManager talkManager = GameManager.Instance.TalkManager;
            if (GameManager.Instance.PlayerEntity.FactionData.GetFactionData(npc.Data.factionID, out factionData))
            {
                UserInterfaceManager uiManager = DaggerfallUI.Instance.UserInterfaceManager;
                FactionFile.FactionData buildingFactionData = new FactionFile.FactionData();
                if (!playerEnterExit.IsPlayerInsideBuilding ||
                    !GameManager.Instance.PlayerEntity.FactionData.GetFactionData(playerEnterExit.BuildingDiscoveryData.factionID, out buildingFactionData))
                    buildingFactionData.ggroup = (int)FactionFile.GuildGroups.None;

                Debug.LogFormat("faction id: {0}, social group: {1}, guild: {2}, building faction: {3}, building guild: {4}", npc.Data.factionID,
                    (FactionFile.SocialGroups)factionData.sgroup, (FactionFile.GuildGroups)factionData.ggroup, buildingFactionData.id, (FactionFile.GuildGroups)buildingFactionData.ggroup);

                // Check if the NPC offers a guild service.
                if (Services.HasGuildService(npc.Data.factionID))
                {
                    FactionFile.GuildGroups guildGroup = (FactionFile.GuildGroups)buildingFactionData.ggroup;
                    if (guildGroup == FactionFile.GuildGroups.None)
                    {   // Use NPC guild group if building has none (e.g. Temple buildings of divine faction)
                        guildGroup = (FactionFile.GuildGroups)factionData.ggroup;
                        // Don't popup guild service menu when holy order NPC isn't in a divine faction building. (bug t=1238)
                        // Don't popup guild service menu when TG spymaster NPC isn't in a faction building. (bug t=2037)
                        if (guildGroup == FactionFile.GuildGroups.HolyOrder && !Temple.IsDivine(buildingFactionData.id) ||
                            factionData.id == (int)GuildNpcServices.TG_Spymaster && buildingFactionData.id == 0)
                        {
                            talkManager.TalkToStaticNPC(npc, false, factionData.id == (int)GuildNpcServices.TG_Spymaster);
                            return;
                        }
                    }
                    // Popup guild service menu.
                    uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.GuildServicePopup, new object[] { uiManager, npc, guildGroup, playerEnterExit.BuildingDiscoveryData.factionID }));
                }
                // Check if this NPC is a merchant.
                else if ((FactionFile.SocialGroups)factionData.sgroup == FactionFile.SocialGroups.Merchants)
                {
                    // Custom merchant service registered?
                    if (Services.HasCustomMerchantService(npc.Data.factionID))
                        uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.MerchantServicePopup, new object[] { uiManager, npc, DaggerfallMerchantServicePopupWindow.Services.Sell }));
                    // Shop?
                    else if (RMBLayout.IsShop(playerEnterExit.BuildingDiscoveryData.buildingType))
                    {
                        if (RMBLayout.IsRepairShop(playerEnterExit.BuildingDiscoveryData.buildingType))
                            uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.MerchantRepairPopup, new object[] { uiManager, npc }));
                        else
                            uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.MerchantServicePopup, new object[] { uiManager, npc, DaggerfallMerchantServicePopupWindow.Services.Sell }));
                    }
                    // Bank?
                    else if (playerEnterExit.BuildingDiscoveryData.buildingType == DFLocation.BuildingTypes.Bank)
                        uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.MerchantServicePopup, new object[] { uiManager, npc, DaggerfallMerchantServicePopupWindow.Services.Banking }));
                    // Tavern?
                    else if (playerEnterExit.BuildingDiscoveryData.buildingType == DFLocation.BuildingTypes.Tavern)
                        uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.Tavern, new object[] { uiManager, npc }));
                    else
                        talkManager.TalkToStaticNPC(npc, false);
                }
                // Check if this NPC is part of a witches coven.
                else if ((FactionFile.FactionTypes)factionData.type == FactionFile.FactionTypes.WitchesCoven)
                {
                    uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.WitchesCovenPopup, new object[] { uiManager, npc }));
                }
                // TODO - more checks for npc social types?
                else // if no special handling had to be done for npc with social group of type merchant: talk to the static npc
                {
                    talkManager.TalkToStaticNPC(npc, false);
                }
            }
            else // if no special handling had to be done (all remaining npcs of the remaining social groups not handled explicitely above): default is talk to the static npc
            {
                talkManager.TalkToStaticNPC(npc, false);
            }
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

            if (Dice100.SuccessRoll(chance))
            {
                if (Dice100.FailedRoll(33))
                {
                    int pinchedGoldPieces = Random.Range(0, 6) + 1;
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
                    player.TallyCrimeGuildRequirements(true, 1);
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

                // Register crime and start spawning guards.
                if (target == null) // target is a townsperson
                {
                    player.CrimeCommitted = PlayerEntity.Crimes.Pickpocketing;
                    player.SpawnCityGuards(true);
                }

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
                    enemyMotor.MakeEnemyHostileToAttacker(GameManager.Instance.PlayerEntityBehaviour);
                }
            }
        }
    }
}
