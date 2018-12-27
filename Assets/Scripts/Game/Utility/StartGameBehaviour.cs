// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Lypyl (lypyldf@gmail.com)
// 
// Notes:
//

using UnityEngine;
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
        public static System.EventHandler OnStartMenu;
        public static System.EventHandler OnStartGame;

        // Private fields
        CharacterDocument characterDocument;
        int classicSaveIndex = -1;
        GameObject player;
        PlayerEnterExit playerEnterExit;
        StartMethods lastStartMethod;

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

        void ApplyStartSettings()
        {
            // Resolution
            Screen.SetResolution(
                DaggerfallUnity.Settings.ResolutionWidth,
                DaggerfallUnity.Settings.ResolutionHeight,
                DaggerfallUnity.Settings.Fullscreen);

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
                    mouseLook.invertMouseY = DaggerfallUnity.Settings.InvertMouseVertical;

                // Set mouse look smoothing
                if (mouseLook)
                    mouseLook.enableSmoothing = DaggerfallUnity.Settings.MouseLookSmoothing;

                // Set mouse look sensitivity
                if (mouseLook)
                    mouseLook.sensitivityScale = DaggerfallUnity.Settings.MouseLookSensitivity;

                // Set rendering path
                if (DaggerfallUnity.Settings.UseLegacyDeferred)
                    camera.renderingPath = RenderingPath.DeferredLighting;
            }

            // Shadow Resoltion Mode
            switch (DaggerfallUnity.Settings.ShadowResolutionMode)
            {
                case 0:
                    QualitySettings.shadowResolution = ShadowResolution.Low;
                    break;
                case 1:
                default:
                    QualitySettings.shadowResolution = ShadowResolution.Medium;
                    break;
                case 2:
                    QualitySettings.shadowResolution = ShadowResolution.High;
                    break;
                case 3:
                    QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
                    break;
            }        

            // VSync settings
            if (DaggerfallUnity.Settings.VSync)
                QualitySettings.vSyncCount = 1;
            else
                QualitySettings.vSyncCount = 0;

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
            DaggerfallUnity.ResetUID();
            QuestMachine.Instance.ClearState();
            RaiseOnNewGameEvent();
            DaggerfallUI.Instance.PopToHUD();
            ResetWeaponManager();
            SaveLoadManager.ClearSceneCache(true);
            GameManager.Instance.GuildManager.ClearMembershipData();

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
                        streamingWorld.SetAutoReposition(StreamingWorld.RepositionMethods.Origin, Vector3.zero);
                        streamingWorld.suppressWorld = false;
                    }
                }
            }

            // Assign starting gear to player entity
            DaggerfallUnity.Instance.ItemHelper.AssignStartingGear(playerEntity, characterDocument.classIndex, characterDocument.isCustom);

            // Assign starting spells to player entity
            SetStartingSpells(playerEntity);

            // Apply biography effects to player entity
            BiogFile.ApplyEffects(characterDocument.biographyEffects, playerEntity);

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

            // Offer main quest during pre-alpha
            QuestMachine.Instance.InstantiateQuest("__MQSTAGE00");

            // Launch startup optional quest
            if (!string.IsNullOrEmpty(LaunchQuest))
            {
                QuestMachine.Instance.InstantiateQuest(LaunchQuest);
                LaunchQuest = string.Empty;
            }
            // Launch any InitAtGameStart quests
            GameManager.Instance.QuestListsManager.InitAtGameStartQuests();

            if (OnStartGame != null)
                OnStartGame(this, null);
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
            DaggerfallUnity.ResetUID();
            QuestMachine.Instance.ClearState();
            RaiseOnNewGameEvent();
            ResetWeaponManager();

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

            // Get prototypical character document data
            CharacterRecord characterRecord = (CharacterRecord)records[0];
            characterDocument = characterRecord.ToCharacterDocument();

            // Assign data to player entity
            PlayerEntity playerEntity = FindPlayerEntity();
            playerEntity.AssignCharacter(characterDocument, characterRecord.ParsedData.level, characterRecord.ParsedData.maxHealth, false);
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

            // Assign items to player entity
            playerEntity.AssignItems(saveTree);

            // Assign guild memberships
            playerEntity.AssignGuildMemberships(saveTree);

            // Assign diseases and poisons to player entity
            playerEntity.AssignDiseasesAndPoisons(saveTree);

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

            // TODO: Import classic spellbook
            playerEntity.DeserializeSpellbook(null);

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

            // Restore vampirism if classic character was a vampire
            if (characterDocument.classicTransformedRace == Races.Vampire)
            {
                // Restore effect
                Debug.Log("Restoring vampirism to classic character.");
                EntityEffectBundle bundle = GameManager.Instance.PlayerEffectManager.CreateVampirismCurse();
                GameManager.Instance.PlayerEffectManager.AssignBundle(bundle);

                // Assign correct clan from classic save
                VampirismEffect vampireEffect = (VampirismEffect)GameManager.Instance.PlayerEffectManager.FindIncumbentEffect<VampirismEffect>();
                if (vampireEffect != null)
                {
                    Debug.LogFormat("Setting vampire clan to {0}", (VampireClans)characterDocument.vampireClan);
                    vampireEffect.VampireClan = (VampireClans)characterDocument.vampireClan;
                }
            }

            // TODO: Restore lycanthropy if classic character was a werewolf/wereboar

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

        void ResetWeaponManager()
        {
            // Weapon hand and equip state not serialized currently
            // Interim measure is to reset weapon manager state on new game
            GameManager.Instance.WeaponManager.Reset();
        }

        void SetStartingSpells(PlayerEntity playerEntity)
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
                QuestMachine.Instance.InstantiateQuest(LaunchQuest);
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
