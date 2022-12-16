// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Lypyl (lypyldf@gmail.com)
// 
// Notes:
//

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Save;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;

namespace DaggerfallWorkshop.Game.Utility
{
    /// <summary>
    /// Game startup and shutdown helper.
    /// </summary>
    public class StartGameBehaviour : MonoBehaviour
    {
        #region Fields

        // Editor properties
        public StartMethods StartMethod = StartMethods.DoNothing;
        public int SaveIndex = -1;
        public string PostStartMessage = string.Empty;
        public string LaunchQuest = string.Empty;
        public bool EnableVideos = true;
        public bool ShowEditorFlats = false;
        public bool NoWorld = false;
        public bool GodMode = false;

        //events used to update state in state manager
        public static EventHandler OnStartMenu;
        public static EventHandler OnStartGame;

        // Private fields
        CharacterDocument characterDocument;
        int classicSaveIndex = -1;
        GameObject player;
        PlayerEnterExit playerEnterExit;
        StartMethods lastStartMethod;
        PostProcessLayer postProcessLayer;
        PostProcessVolume postProcessVolume;

        #endregion

        #region Properties

        public CharacterDocument CharacterDocument
        {
            get { return characterDocument; }
            set { characterDocument = value; }
        }

        public int ClassicSaveIndex
        {
            get { return classicSaveIndex; }
            set { classicSaveIndex = value; }
        }

        public StartMethods LastStartMethod
        {
            get { return lastStartMethod; }
        }

        public delegate void PlayerStartingEquipment(PlayerEntity playerEntity, CharacterDocument characterDocument);
        public PlayerStartingEquipment AssignStartingEquipment { get; set; }

        public delegate void PlayerStartingSpells(PlayerEntity playerEntity, CharacterDocument characterDocument);
        public PlayerStartingSpells AssignStartingSpells { get; set; }

        #endregion

        #region Enums

        public enum StartMethods
        {
            DoNothing,                              // No startup action
            Void,                                   // Start to the Void
            TitleMenu,                              // Open title menu
            TitleMenuFromDeath,                     // Open title menu after death
            NewCharacter,                           // Spawn character to start location in INI
            LoadDaggerfallUnitySave,                // Make this work with new save/load system
            LoadClassicSave,                        // Loads a classic save using start save index
        }

        #endregion

        #region Unity

        void Awake()
        {
            // Get player objects
            player = FindPlayer();
            playerEnterExit = FindPlayerEnterExit(player);

            // Assign default player equipment & spells allocation methods
            AssignStartingEquipment = DaggerfallUnity.Instance.ItemHelper.AssignStartingGear;
            AssignStartingSpells = SetStartingSpells;
        }

        void Start()
        {
            ApplyStartSettings();
            SaveLoadManager.OnLoad += SaveLoadManager_OnLoad;
        }

        void Update()
        {
            // Restart game using method provided
            if (StartMethod != StartMethods.DoNothing)
            {
                GameManager.Instance.PauseGame(true);
                InvokeStartMethod();
                StartMethod = StartMethods.DoNothing;
            }
        }

        #endregion

        #region Private Methods

        void InvokeStartMethod()
        {
            // Disable parent GameObjects - the appropriate parent GameObject will be re-enabled by following startup process
            // This mainly just prevents all SongPlayers from starting at once
            GameManager.Instance.PlayerEnterExit.DisableAllParents();

            switch (StartMethod)
            {
                case StartMethods.Void:
                    StartVoid();
                    break;
                case StartMethods.TitleMenu:
                    StartTitleMenu();
                    break;
                case StartMethods.TitleMenuFromDeath:
                    StartTitleMenuFromDeath();
                    break;
                case StartMethods.NewCharacter:
                    StartNewCharacter();
                    break;
                case StartMethods.LoadDaggerfallUnitySave:
                    LoadDaggerfallUnitySave();
                    break;
                case StartMethods.LoadClassicSave:
                    if (SaveIndex != -1) classicSaveIndex = SaveIndex;
                    StartFromClassicSave();
                    break;
                default:
                    break;
            }

            // Reset save index
            SaveIndex = -1;
        }

        #endregion

        #region Common Startup

        public void ApplyStartSettings()
        {
            // Resolution
            if (DaggerfallUnity.Settings.ExclusiveFullscreen && DaggerfallUnity.Settings.Fullscreen)
            {
                Screen.SetResolution(
                    DaggerfallUnity.Settings.ResolutionWidth,
                    DaggerfallUnity.Settings.ResolutionHeight,
                    FullScreenMode.ExclusiveFullScreen);
            }
            else
            {
                Screen.SetResolution(
                    DaggerfallUnity.Settings.ResolutionWidth,
                    DaggerfallUnity.Settings.ResolutionHeight,
                    DaggerfallUnity.Settings.Fullscreen);
            }

            // Camera settings
            GameObject cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            if (cameraObject)
            {
                // Set camera FOV
                Camera camera = cameraObject.GetComponent<Camera>();
                if (camera)
                    camera.fieldOfView = DaggerfallUnity.Settings.FieldOfView;

                // Set mouse look
                PlayerMouseLook mouseLook = cameraObject.GetComponent<PlayerMouseLook>();
                if (mouseLook)
                {
                    mouseLook.invertMouseY = DaggerfallUnity.Settings.InvertMouseVertical;
                    // Set mouse look smoothing
                    mouseLook.Smoothing = DaggerfallUnity.Settings.MouseLookSmoothingFactor;
                    // Set mouse look sensitivity
                    mouseLook.sensitivityScale = DaggerfallUnity.Settings.MouseLookSensitivity;

                    mouseLook.joystickSensitivityScale = DaggerfallUnity.Settings.JoystickLookSensitivity;
                }
            }

            DeployCoreGameEffectSettings(CoreGameEffectSettingsGroups.Everything);

            InputManager.Instance.JoystickCursorSensitivity = DaggerfallUnity.Settings.JoystickCursorSensitivity;

            InputManager.Instance.JoystickMovementThreshold = DaggerfallUnity.Settings.JoystickMovementThreshold;

            InputManager.Instance.JoystickDeadzone = DaggerfallUnity.Settings.JoystickDeadzone;

            InputManager.Instance.EnableController = DaggerfallUnity.Settings.EnableController;

            Application.runInBackground = DaggerfallUnity.Settings.RunInBackground;

            // Set shadow resolution
            GameManager.UpdateShadowResolution();

            // VSync settings
            if (DaggerfallUnity.Settings.VSync)
                QualitySettings.vSyncCount = 1;
            else
                QualitySettings.vSyncCount = 0;

            // Target frame rate settings
            // Does nothing if VSync enabled
            // Default is 0 but anything below 30 is ignored and treated as disabled
            if (DaggerfallUnity.Settings.TargetFrameRate >= 30 && !DaggerfallUnity.Settings.VSync)
            {
                Application.targetFrameRate = DaggerfallUnity.Settings.TargetFrameRate;
            }

            // Filter settings
            DaggerfallUnity.Instance.MaterialReader.MainFilterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode;
            DaggerfallUnity.Instance.MaterialReader.CompressModdedTextures = DaggerfallUnity.Settings.CompressModdedTextures;

            // HUD settings
            DaggerfallHUD hud = DaggerfallUI.Instance.DaggerfallHUD;
            if (hud != null)                                              //null at startup
                hud.ShowCrosshair = DaggerfallUnity.Settings.Crosshair; 

            // Weapon swing settings
            WeaponManager weaponManager = GameManager.Instance.WeaponManager;
            weaponManager.AttackThreshold = DaggerfallUnity.Settings.WeaponAttackThreshold;
            TransportManager transportManager = GameManager.Instance.TransportManager;

            // Weapon hand settings
            // Only supporting left-hand rendering for now
            // More handedness options may be added later
            if (DaggerfallUnity.Settings.Handedness == 1)
                weaponManager.ScreenWeapon.FlipHorizontal = true;

            // GodMode setting
            PlayerEntity playerEntity = FindPlayerEntity();
            playerEntity.GodMode = GodMode;

            PlayerSpeedChanger speedChanger = FindPlayerSpeedChanger();
            speedChanger.ToggleSneak = DaggerfallUnity.Settings.ToggleSneak;

            // Enable/disable videos
            DaggerfallUI.Instance.enableVideos = EnableVideos;

            // Streaming world terrain distance
            GameManager.Instance.StreamingWorld.TerrainDistance = DaggerfallUnity.Settings.TerrainDistance;
        }

        #endregion

        #region Startup Methods

        void StartVoid()
        {
            RaiseOnNewGameEvent();
            DaggerfallUI.Instance.PopToHUD();
            QuestMachine.Instance.ClearState();
            playerEnterExit.DisableAllParents();
            ResetWeaponManager();
            NoWorld = true;
            lastStartMethod = StartMethods.Void;

            if (string.IsNullOrEmpty(PostStartMessage))
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiInitGame);
            else
                DaggerfallUI.PostMessage(PostStartMessage);
        }

        void StartTitleMenu()
        {
            RaiseOnNewGameEvent();
            DaggerfallUI.Instance.PopToHUD();
            QuestMachine.Instance.ClearState();
            playerEnterExit.DisableAllParents();
            ResetWeaponManager();

            if (string.IsNullOrEmpty(PostStartMessage))
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiInitGame);
            else
                DaggerfallUI.PostMessage(PostStartMessage);

            lastStartMethod = StartMethods.TitleMenu;

            if (OnStartMenu != null)
                OnStartMenu(this, null);
        }

        void StartTitleMenuFromDeath()
        {
            RaiseOnNewGameEvent();

            // Reset player death camera
            if (GameManager.Instance.PlayerDeath)
                GameManager.Instance.PlayerDeath.ResetCamera();

            // Set behaviour back to title menu
            StartMethod = StartMethods.TitleMenu;

            DaggerfallUI.Instance.PopToHUD();
            QuestMachine.Instance.ClearState();
            playerEnterExit.DisableAllParents();
            ResetWeaponManager();

            if (string.IsNullOrEmpty(PostStartMessage))
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiInitGameFromDeath);
            else
                DaggerfallUI.PostMessage(PostStartMessage);

            lastStartMethod = StartMethods.TitleMenuFromDeath;

            if (OnStartMenu != null)
                OnStartMenu(this, null);
        }

        // Start new character to location specified in INI
        void StartNewCharacter()
        {
            NewCharacterCleanup();

            // Must have a character document
            if (characterDocument == null)
                characterDocument = new CharacterDocument();

            // Assign character sheet
            PlayerEntity playerEntity = FindPlayerEntity();
            playerEntity.AssignCharacter(characterDocument);

            // Set game time
            DaggerfallUnity.Instance.WorldTime.Now.SetClassicGameStartTime();

            // Set time tracked in playerEntity
            playerEntity.LastGameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();

            // Get start parameters
            DFPosition mapPixel = new DFPosition(DaggerfallUnity.Settings.StartCellX, DaggerfallUnity.Settings.StartCellY);
            bool startInDungeon = DaggerfallUnity.Settings.StartInDungeon;

            // Read location if any
            DFLocation location = new DFLocation();
            ContentReader.MapSummary mapSummary;
            bool hasLocation = DaggerfallUnity.Instance.ContentReader.HasLocation(mapPixel.X, mapPixel.Y, out mapSummary);
            if (hasLocation)
            {
                if (!DaggerfallUnity.Instance.ContentReader.GetLocation(mapSummary.RegionIndex, mapSummary.MapIndex, out location))
                    hasLocation = false;
            }

            if (NoWorld)
            {
                playerEnterExit.DisableAllParents();
            }
            else
            {
                // Start at specified location
                StreamingWorld streamingWorld = FindStreamingWorld();
                if (hasLocation && startInDungeon && location.HasDungeon)
                {
                    if (streamingWorld)
                    {
                        streamingWorld.TeleportToCoordinates(mapPixel.X, mapPixel.Y);
                        streamingWorld.suppressWorld = true;
                    }
                    playerEnterExit.EnableDungeonParent();
                    playerEnterExit.StartDungeonInterior(location);
                }
                else
                {
                    playerEnterExit.EnableExteriorParent();
                    if (streamingWorld)
                    {
                        streamingWorld.TeleportToCoordinates(mapPixel.X, mapPixel.Y);
                        streamingWorld.SetAutoReposition(StreamingWorld.RepositionMethods.Origin, Vector3.zero);
                        streamingWorld.suppressWorld = false;
                    }
                }
            }

            // Apply biography effects to player entity
            BiogFile.ApplyEffects(characterDocument.biographyEffects, playerEntity);

            // Assign starting gear to player entity
            AssignStartingEquipment(playerEntity, characterDocument);
            
            // Assign starting spells to player entity
            AssignStartingSpells(playerEntity, characterDocument);

            // Assign starting level up skill sum
            playerEntity.SetCurrentLevelUpSkillSum();
            playerEntity.StartingLevelUpSkillSum = playerEntity.CurrentLevelUpSkillSum;

            // Setup bank accounts and houses
            Banking.DaggerfallBankManager.SetupAccounts();
            Banking.DaggerfallBankManager.SetupHouses();

            // Initialize region data
            playerEntity.InitializeRegionData();

            // Randomize weathers
            GameManager.Instance.WeatherManager.SetClimateWeathers();

            // Start game
            GameManager.Instance.PauseGame(false);
            DaggerfallUI.Instance.FadeBehaviour.FadeHUDFromBlack();
            DaggerfallUI.PostMessage(PostStartMessage);

            lastStartMethod = StartMethods.NewCharacter;

            // Start main quest
            QuestMachine.Instance.StartQuest("_TUTOR__");
            QuestMachine.Instance.StartQuest("_BRISIEN");

            // Launch startup optional quest
            if (!string.IsNullOrEmpty(LaunchQuest))
            {
                QuestMachine.Instance.StartQuest(LaunchQuest);
                LaunchQuest = string.Empty;
            }
            // Launch any InitAtGameStart quests
            GameManager.Instance.QuestListsManager.InitAtGameStartQuests();

            if (OnStartGame != null)
                OnStartGame(this, null);
        }

        // Performs cleanup before starting a new character or loading a classic save
        void NewCharacterCleanup()
        {
            DaggerfallUnity.ResetUID();
            QuestMachine.Instance.ClearState();
            DaggerfallUI.Instance.PopToHUD();
            SaveLoadManager.ClearSceneCache(true);
            GameManager.Instance.GuildManager.ClearMembershipData();
            GameManager.Instance.PlayerGPS.ClearDiscoveryData();
            Banking.DaggerfallBankManager.ResetShip();
            RaiseOnNewGameEvent();
            ResetWeaponManager();
        }

        #endregion

        #region Daggerfall Unity Save Startup

        void LoadDaggerfallUnitySave()
        {
            if (SaveIndex == -1)
                return;

            SaveLoadManager.Instance.EnumerateSaves();
            SaveLoadManager.Instance.Load(SaveIndex);
        }

        #endregion

        #region Classic Save Startup

        void StartFromClassicSave()
        {
            NewCharacterCleanup();

            // Save index must be in range
            if (classicSaveIndex < 0 || classicSaveIndex >= 6)
                throw new IndexOutOfRangeException("classicSaveIndex out of range.");

            // Open saves in parent path of Arena2 folder
            string path = SaveLoadManager.Instance.DaggerfallSavePath;
            SaveGames saveGames = new SaveGames(path);
            if (!saveGames.IsPathOpen)
                throw new Exception(string.Format("Could not open Daggerfall saves path {0}", path));

            // Open save index
            if (!saveGames.TryOpenSave(classicSaveIndex))
            {
                string error = string.Format("Could not open classic save index {0}.", classicSaveIndex);
                DaggerfallUI.MessageBox(error);
                DaggerfallUnity.LogMessage(string.Format(error), true);
                return;
            }

            // Get required save data
            SaveTree saveTree = saveGames.SaveTree;
            SaveVars saveVars = saveGames.SaveVars;

            SaveTreeBaseRecord positionRecord = saveTree.FindRecord(RecordTypes.CharacterPositionRecord);

            if (NoWorld)
            {
                playerEnterExit.DisableAllParents();
            }
            else
            {
                // Set player to world position
                playerEnterExit.EnableExteriorParent();
                StreamingWorld streamingWorld = FindStreamingWorld();
                int worldX = positionRecord.RecordRoot.Position.WorldX;
                int worldZ = positionRecord.RecordRoot.Position.WorldZ;
                streamingWorld.TeleportToWorldCoordinates(worldX, worldZ);
                streamingWorld.suppressWorld = false;
            }

            GameObject cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            PlayerMouseLook mouseLook = cameraObject.GetComponent<PlayerMouseLook>();
            if (mouseLook)
            {
                // Classic save value ranges from -256 (looking up) to 256 (looking down).
                // The maximum up and down range of view in classic is similar to 45 degrees up and down in DF Unity.
                float pitch = positionRecord.RecordRoot.Pitch;
                if (pitch != 0)
                    pitch = (pitch * 45 / 256);
                mouseLook.Pitch = pitch;

                float yaw = positionRecord.RecordRoot.Yaw;
                // In classic saves 2048 units of yaw is 360 degrees.
                if (yaw != 0)
                    yaw = (yaw * 360 / 2048);
                mouseLook.Yaw = yaw;
            }

            // Set whether the player's weapon is drawn
            WeaponManager weaponManager = GameManager.Instance.WeaponManager;
            weaponManager.Sheathed = (!saveVars.WeaponDrawn);

            // Set game time
            DaggerfallUnity.Instance.WorldTime.Now.FromClassicDaggerfallTime(saveVars.GameTime);

            // Get character record
            List<SaveTreeBaseRecord> records = saveTree.FindRecords(RecordTypes.Character);
            if (records.Count != 1)
                throw new Exception("SaveTree CharacterRecord not found.");

            // Assign diseases and poisons to player entity
            LycanthropyTypes lycanthropyType;
            PlayerEntity playerEntity = FindPlayerEntity();
            playerEntity.AssignDiseasesAndPoisons(saveTree, out lycanthropyType);

            // Get prototypical character document data
            CharacterRecord characterRecord = (CharacterRecord)records[0];
            characterDocument = characterRecord.ToCharacterDocument(lycanthropyType);

            // Assign data to player entity
            playerEntity.AssignCharacter(characterDocument, characterRecord.ParsedData.level, characterRecord.ParsedData.baseHealth, false);
            playerEntity.SetCurrentLevelUpSkillSum();

            // Assign biography modifiers
            playerEntity.BiographyResistDiseaseMod = saveVars.BiographyResistDiseaseMod;
            playerEntity.BiographyResistMagicMod = saveVars.BiographyResistMagicMod;
            playerEntity.BiographyAvoidHitMod = saveVars.BiographyAvoidHitMod;
            playerEntity.BiographyResistPoisonMod = saveVars.BiographyResistPoisonMod;
            playerEntity.BiographyFatigueMod = saveVars.BiographyFatigueMod;

            // Assign faction data
            playerEntity.FactionData.ImportClassicReputation(saveVars);

            // Assign global variables
            playerEntity.GlobalVars.ImportClassicGlobalVars(saveVars);

            // Set time of last check for raising skills
            playerEntity.TimeOfLastSkillIncreaseCheck = saveVars.LastSkillCheckTime;

            // Assign classic items and spells to player entity
            playerEntity.AssignItemsAndSpells(saveTree);

            // Assign guild memberships
            playerEntity.AssignGuildMemberships(saveTree, characterDocument.classicTransformedRace == Races.Vampire);

            // Assign gold pieces
            playerEntity.GoldPieces = (int)characterRecord.ParsedData.physicalGold;

            // Assign weapon hand being used
            weaponManager.UsingRightHand = !saveVars.UsingLeftHandWeapon;

            // GodMode setting
            playerEntity.GodMode = saveVars.GodMode;

            // Setup bank accounts
            var bankRecords = saveTree.FindRecord(RecordTypes.BankAccount);
            Banking.DaggerfallBankManager.ReadNativeBankData(bankRecords);

            // Ship ownership
            Banking.DaggerfallBankManager.AssignShipToPlayer(saveVars.PlayerOwnedShip);

            // Get regional data.
            playerEntity.RegionData = saveVars.RegionData;

            // Set time tracked by playerEntity for game minute-based updates
            playerEntity.LastGameMinutes = saveVars.GameTime;

            // Get breath remaining if player was submerged (0 if they were not in the water)
            playerEntity.CurrentBreath = saveVars.BreathRemaining;

            // Get last type of crime committed
            playerEntity.CrimeCommitted = (PlayerEntity.Crimes)saveVars.CrimeCommitted;

            // Get weather
            byte[] climateWeathers = saveVars.ClimateWeathers;

            // Enums for thunder and snow are reversed in classic and Unity, so they are converted here.
            for (int i = 0; i < climateWeathers.Length; i++)
            {
                // TODO: 0x80 flag can be set for snow or rain, to add fog to these weathers. This isn't in DF Unity yet, so
                // temporarily removing the flag.
                climateWeathers[i] &= 0x7f;
                if (climateWeathers[i] == 5)
                    climateWeathers[i] = 6;
                else if (climateWeathers[i] == 6)
                    climateWeathers[i] = 5;
            }
            GameManager.Instance.WeatherManager.PlayerWeather.ClimateWeathers = climateWeathers;

            // Load character biography text
            playerEntity.BackStory = saveGames.BioFile.Lines;

            // Validate spellbook item
            DaggerfallUnity.Instance.ItemHelper.ValidateSpellbookItem(playerEntity);

            // Restore old class specials
            RestoreOldClassSpecials(saveTree, characterDocument.classicTransformedRace, lycanthropyType);

            // Restore vampirism if classic character was a vampire
            if (characterDocument.classicTransformedRace == Races.Vampire)
            {
                // Restore effect
                Debug.Log("Restoring vampirism to classic character.");
                EntityEffectBundle bundle = GameManager.Instance.PlayerEffectManager.CreateVampirismCurse();
                GameManager.Instance.PlayerEffectManager.AssignBundle(bundle, AssignBundleFlags.BypassSavingThrows);

                // Assign correct clan from classic save
                VampirismEffect vampireEffect = (VampirismEffect)GameManager.Instance.PlayerEffectManager.FindIncumbentEffect<VampirismEffect>();
                if (vampireEffect != null)
                {
                    Debug.LogFormat("Setting vampire clan to {0}", (VampireClans)characterDocument.vampireClan);
                    vampireEffect.VampireClan = (VampireClans)characterDocument.vampireClan;
                }
            }

            // Restore lycanthropy if classic character was a werewolf/wereboar
            if (lycanthropyType != LycanthropyTypes.None)
            {
                // TODO: Restore lycanthropy effect
                switch (lycanthropyType)
                {
                    case LycanthropyTypes.Werewolf:
                        Debug.Log("Restoring lycanthropy (werewolf) to classic character.");
                        break;
                    case LycanthropyTypes.Wereboar:
                        Debug.Log("Restoring lycanthropy (wereboar) to classic character.");
                        break;
                }
                EntityEffectBundle bundle = GameManager.Instance.PlayerEffectManager.CreateLycanthropyCurse();
                GameManager.Instance.PlayerEffectManager.AssignBundle(bundle, AssignBundleFlags.BypassSavingThrows);
                LycanthropyEffect lycanthropyEffect = (LycanthropyEffect)GameManager.Instance.PlayerEffectManager.FindIncumbentEffect<LycanthropyEffect>();
                if (lycanthropyEffect != null)
                {
                    lycanthropyEffect.InfectionType = lycanthropyType;
                    GameManager.Instance.PlayerEntity.AssignPlayerLycanthropySpell();
                }
            }

            // Start game
            DaggerfallUI.Instance.PopToHUD();
            GameManager.Instance.PauseGame(false);
            DaggerfallUI.Instance.FadeBehaviour.FadeHUDFromBlack();
            DaggerfallUI.PostMessage(PostStartMessage);

            lastStartMethod = StartMethods.LoadClassicSave;
            SaveIndex = -1;

            if (OnStartGame != null)
                OnStartGame(this, null);
        }

        void RestoreOldClassSpecials(SaveTree saveTree, Races classicTransformedRace, LycanthropyTypes lycanthropyType)
        {
            try
            {
                // Get old class record
                SaveTreeBaseRecord oldClassRecord = saveTree.FindRecord(RecordTypes.OldClass);
                if (oldClassRecord == null)
                    return;

                // Read old class data
                System.IO.MemoryStream stream = new System.IO.MemoryStream(oldClassRecord.RecordData);
                System.IO.BinaryReader reader = new System.IO.BinaryReader(stream);
                ClassFile classFile = new ClassFile();
                classFile.Load(reader);
                reader.Close();

                // Restore any specials set by transformed race
                if (classicTransformedRace == Races.Vampire)
                {
                    // Restore pre-vampire specials
                    characterDocument.career.DamageFromSunlight = classFile.Career.DamageFromSunlight;
                    characterDocument.career.DamageFromHolyPlaces = classFile.Career.DamageFromHolyPlaces;
                    characterDocument.career.Paralysis = classFile.Career.Paralysis;
                    characterDocument.career.Disease = classFile.Career.Disease;
                }
                else if (lycanthropyType != LycanthropyTypes.None)
                {
                    // Restore pre-lycanthropy specials
                    characterDocument.career.Disease = classFile.Career.Disease;
                }
            }
            catch(Exception ex)
            {
                Debug.LogErrorFormat("Could not restore old class specials for vamp/were import. Error: '{0}'", ex.Message);
            }
        }

        #endregion

        #region Utility

        StreamingWorld FindStreamingWorld()
        {
            StreamingWorld streamingWorld = GameObject.FindObjectOfType<StreamingWorld>();
            if (!streamingWorld)
                throw new Exception("Could not find StreamingWorld.");

            return streamingWorld;
        }

        GameObject FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (!player)
                throw new Exception("Could not find Player.");

            return player;
        }

        PlayerEnterExit FindPlayerEnterExit(GameObject player)
        {
            PlayerEnterExit playerEnterExit = player.GetComponent<PlayerEnterExit>();
            if (!playerEnterExit)
                throw new Exception("Could not find PlayerEnterExit.");

            return playerEnterExit;
        }

        PlayerEntity FindPlayerEntity()
        {
            GameObject player = FindPlayer();
            PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;

            return playerEntity;
        }

        PlayerSpeedChanger FindPlayerSpeedChanger()
        {
            GameObject player = FindPlayer();
            PlayerSpeedChanger speedChanger = player.GetComponent<PlayerSpeedChanger>();

            return speedChanger;
        }

        void ResetWeaponManager()
        {
            // Weapon hand and equip state not serialized currently
            // Interim measure is to reset weapon manager state on new game
            GameManager.Instance.WeaponManager.Reset();
        }

        /// <summary>
        /// Assigns starting spells to the spellbook item for a new character.
        /// </summary>
        void SetStartingSpells(PlayerEntity playerEntity, CharacterDocument characterDocument)
        {
            if (characterDocument.classIndex > 6 && !characterDocument.isCustom) // Class does not have starting spells
                return;

            // Get starting set based on class
            int spellSetIndex = -1;
            if (characterDocument.isCustom)
            {
                DFCareer dfc = characterDocument.career;

                // Custom class uses Spellsword starting spells if it has at least 1 primary or major magic skill
                if (Enum.IsDefined(typeof(DFCareer.MagicSkills), (int)dfc.PrimarySkill1) ||
                    Enum.IsDefined(typeof(DFCareer.MagicSkills), (int)dfc.PrimarySkill2) ||
                    Enum.IsDefined(typeof(DFCareer.MagicSkills), (int)dfc.PrimarySkill3) ||
                    Enum.IsDefined(typeof(DFCareer.MagicSkills), (int)dfc.MajorSkill1) ||
                    Enum.IsDefined(typeof(DFCareer.MagicSkills), (int)dfc.MajorSkill2) ||
                    Enum.IsDefined(typeof(DFCareer.MagicSkills), (int)dfc.MajorSkill3))
                {
                    spellSetIndex = 1;
                }
            }
            else
            {
                spellSetIndex = characterDocument.classIndex;
            }

            if (spellSetIndex == -1)
                return;

            // Get the set's spell indices
            TextAsset spells = Resources.Load<TextAsset>("StartingSpells") as TextAsset;
            List<CareerStartingSpells> startingSpells = SaveLoadManager.Deserialize(typeof(List<CareerStartingSpells>), spells.text) as List<CareerStartingSpells>;
            List<StartingSpell> spellsToAdd = new List<StartingSpell>();
            for (int i = 0; i < startingSpells[spellSetIndex].SpellsList.Length; i++)
            {
                spellsToAdd.Add(startingSpells[spellSetIndex].SpellsList[i]);
            }

            // Add spells to player from standard list
            foreach (StartingSpell spell in spellsToAdd)
            {
                SpellRecord.SpellRecordData spellData;
                GameManager.Instance.EntityEffectBroker.GetClassicSpellRecord(spell.SpellID, out spellData);
                if (spellData.index == -1)
                {
                    Debug.LogError("Failed to locate starting spell in standard spells list.");
                    continue;
                }

                EffectBundleSettings bundle;
                if (!GameManager.Instance.EntityEffectBroker.ClassicSpellRecordDataToEffectBundleSettings(spellData, BundleTypes.Spell, out bundle))
                {
                    Debug.LogError("Failed to create effect bundle for starting spell.");
                    continue;
                }
                playerEntity.AddSpell(bundle);
            }
        }

        public void DeployCoreGameEffectSettings(CoreGameEffectSettingsGroups groups)
        {
            // Get layer
            if (!postProcessLayer)
            {
                postProcessLayer = GameManager.Instance.MainCamera.GetComponent<PostProcessLayer>();
                if (!postProcessLayer)
                {
                    Debug.LogError("Could not locate PostProcessLayer on MainCamera");
                    return;
                }
            }

            // Get volume
            if (!postProcessVolume)
            {
                postProcessVolume = GameManager.Instance.PostProcessVolume;
                if (!postProcessVolume)
                {
                    Debug.LogError("Could not locate PostProcessVolume in scene");
                    return;
                }
            }

            // Antialiasing
            if (groups.HasFlag(CoreGameEffectSettingsGroups.Antialiasing))
            {
                switch ((AntiAliasingMethods)DaggerfallUnity.Settings.AntialiasingMethod)
                {
                    case AntiAliasingMethods.None:
                        postProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.None;
                        break;
                    case AntiAliasingMethods.FXAA:
                        postProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
                        postProcessLayer.fastApproximateAntialiasing.fastMode = DaggerfallUnity.Settings.AntialiasingFXAAFastMode;
                        break;
                    case AntiAliasingMethods.SMAA:
                        postProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
                        postProcessLayer.subpixelMorphologicalAntialiasing.quality = (SubpixelMorphologicalAntialiasing.Quality)DaggerfallUnity.Settings.AntialiasingSMAAQuality;
                        break;
                    case AntiAliasingMethods.TAA:
                        postProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
                        postProcessLayer.temporalAntialiasing.sharpness = DaggerfallUnity.Settings.AntialiasingTAASharpness;
                        break;
                }
            }

            // Ambient Occlusion
            if (groups.HasFlag(CoreGameEffectSettingsGroups.AmbientOcclusion))
            {
                AmbientOcclusion ambientOcclusionSettings;
                if (postProcessVolume.profile.TryGetSettings<AmbientOcclusion>(out ambientOcclusionSettings))
                {
                    ambientOcclusionSettings.enabled.value = DaggerfallUnity.Settings.AmbientOcclusionEnable;
                    ambientOcclusionSettings.mode.overrideState = true;
                    ambientOcclusionSettings.mode.value = (AmbientOcclusionMode)DaggerfallUnity.Settings.AmbientOcclusionMethod;
                    ambientOcclusionSettings.intensity.overrideState = true;
                    ambientOcclusionSettings.intensity.value = DaggerfallUnity.Settings.AmbientOcclusionIntensity;
                    ambientOcclusionSettings.radius.overrideState = true;
                    ambientOcclusionSettings.radius.value = DaggerfallUnity.Settings.AmbientOcclusionRadius;
                    ambientOcclusionSettings.quality.overrideState = true;
                    ambientOcclusionSettings.quality.value = (AmbientOcclusionQuality)DaggerfallUnity.Settings.AmbientOcclusionQuality;
                    ambientOcclusionSettings.thicknessModifier.overrideState = true;
                    ambientOcclusionSettings.thicknessModifier.value = DaggerfallUnity.Settings.AmbientOcclusionThickness;
                }
            }

            // Bloom
            if (groups.HasFlag(CoreGameEffectSettingsGroups.Bloom))
            {
                Bloom bloomSettings;
                if (postProcessVolume.profile.TryGetSettings<Bloom>(out bloomSettings))
                {
                    bloomSettings.enabled.value = DaggerfallUnity.Settings.BloomEnable;
                    bloomSettings.intensity.overrideState = true;
                    bloomSettings.intensity.value = DaggerfallUnity.Settings.BloomIntensity;
                    bloomSettings.threshold.overrideState = true;
                    bloomSettings.threshold.value = DaggerfallUnity.Settings.BloomThreshold;
                    bloomSettings.diffusion.overrideState = true;
                    bloomSettings.diffusion.value = DaggerfallUnity.Settings.BloomDiffusion;
                    bloomSettings.fastMode.overrideState = true;
                    bloomSettings.fastMode.value = DaggerfallUnity.Settings.BloomFastMode;
                }
            }

            // Motion Blur
            if (groups.HasFlag(CoreGameEffectSettingsGroups.MotionBlur))
            {
                MotionBlur motionBlurSettings;
                if (postProcessVolume.profile.TryGetSettings<MotionBlur>(out motionBlurSettings))
                {
                    motionBlurSettings.enabled.value = DaggerfallUnity.Settings.MotionBlurEnable;
                    motionBlurSettings.shutterAngle.overrideState = true;
                    motionBlurSettings.shutterAngle.value = DaggerfallUnity.Settings.MotionBlurShutterAngle;
                    motionBlurSettings.sampleCount.overrideState = true;
                    motionBlurSettings.sampleCount.value = DaggerfallUnity.Settings.MotionBlurSampleCount;
                }
            }

            // Vignette
            if (groups.HasFlag(CoreGameEffectSettingsGroups.Vignette))
            {
                Vignette vignetteSettings;
                if (postProcessVolume.profile.TryGetSettings<Vignette>(out vignetteSettings))
                {
                    vignetteSettings.enabled.value = DaggerfallUnity.Settings.VignetteEnable;
                    vignetteSettings.intensity.overrideState = true;
                    vignetteSettings.intensity.value = DaggerfallUnity.Settings.VignetteIntensity;
                    vignetteSettings.smoothness.overrideState = true;
                    vignetteSettings.smoothness.value = DaggerfallUnity.Settings.VignetteSmoothness;
                    vignetteSettings.roundness.overrideState = true;
                    vignetteSettings.roundness.value = DaggerfallUnity.Settings.VignetteRoundness;
                    vignetteSettings.rounded.overrideState = true;
                    vignetteSettings.rounded.value = DaggerfallUnity.Settings.VignetteRounded;
                }    
            }

            // Depth of Field
            if (groups.HasFlag(CoreGameEffectSettingsGroups.DepthOfField))
            {
                DepthOfField depthOfFieldSettings;
                if (postProcessVolume.profile.TryGetSettings<DepthOfField>(out depthOfFieldSettings))
                {
                    depthOfFieldSettings.enabled.value = DaggerfallUnity.Settings.DepthOfFieldEnable;
                    depthOfFieldSettings.focusDistance.overrideState = true;
                    depthOfFieldSettings.focusDistance.value = DaggerfallUnity.Settings.DepthOfFieldFocusDistance;
                    depthOfFieldSettings.aperture.overrideState = true;
                    depthOfFieldSettings.aperture.value = DaggerfallUnity.Settings.DepthOfFieldAperture;
                    depthOfFieldSettings.focalLength.overrideState = true;
                    depthOfFieldSettings.focalLength.value = DaggerfallUnity.Settings.DepthOfFieldFocalLength;
                    depthOfFieldSettings.kernelSize.overrideState = true;
                    depthOfFieldSettings.kernelSize.value = (KernelSize)DaggerfallUnity.Settings.DepthOfFieldMaxBlurSize;
                }
            }

            // Dither
            if (groups.HasFlag(CoreGameEffectSettingsGroups.Dither))
            {
                const string ditherKeyword = "_PPV2_DITHER_ON";
                if (DaggerfallUnity.Settings.DitherEnable)
                    Shader.EnableKeyword(ditherKeyword);
                else
                    Shader.DisableKeyword(ditherKeyword);
            }

            // ColorBoost
            if (groups.HasFlag(CoreGameEffectSettingsGroups.ColorBoost))
            {
                ColorBoost colorBoostSettings;
                if (postProcessVolume.profile.TryGetSettings<ColorBoost>(out colorBoostSettings))
                {
                    colorBoostSettings.enabled.value = DaggerfallUnity.Settings.ColorBoostEnable;
                    colorBoostSettings.radius.overrideState = true;
                    colorBoostSettings.radius.value = DaggerfallUnity.Settings.ColorBoostRadius;
                    colorBoostSettings.globalIntensity.overrideState = true;
                    colorBoostSettings.globalIntensity.value = DaggerfallUnity.Settings.ColorBoostIntensity;
                    colorBoostSettings.dungeonScale.overrideState = true;
                    colorBoostSettings.dungeonScale.value = DaggerfallUnity.Settings.ColorBoostDungeonScale;
                    colorBoostSettings.interiorScale.overrideState = true;
                    colorBoostSettings.interiorScale.value = DaggerfallUnity.Settings.ColorBoostInteriorScale;
                    colorBoostSettings.exteriorScale.overrideState = true;
                    colorBoostSettings.exteriorScale.value = DaggerfallUnity.Settings.ColorBoostExteriorScale;
                    colorBoostSettings.dungeonFalloff.overrideState = true;
                    colorBoostSettings.dungeonFalloff.value = DaggerfallUnity.Settings.ColorBoostDungeonFalloff;
                }
            }

            // Retro Mode
            if (groups.HasFlag(CoreGameEffectSettingsGroups.RetroMode))
            {
                GameManager.Instance.RetroRenderer.UpdateSettings();
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Launch starting quest on first load.
        /// This is only used as a debug helper to launch a quest after
        /// automatically loading a DFUnity save game.
        /// </summary>
        private void SaveLoadManager_OnLoad(SaveData_v1 saveData)
        {
            if (!string.IsNullOrEmpty(LaunchQuest))
            {
                QuestMachine.Instance.StartQuest(LaunchQuest);
                LaunchQuest = string.Empty;
            }
        }

        #endregion

        #region Events

        // OnNewGame
        public delegate void OnNewGameEventHandler();
        public static event OnNewGameEventHandler OnNewGame;
        protected virtual void RaiseOnNewGameEvent()
        {
            if (OnNewGame != null)
                OnNewGame();
        }

        #endregion
    }
}
