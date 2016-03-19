// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Save;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

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
        public int OverrideSaveIndex = -1;
        public string PostStartMessage = string.Empty;
        public bool EnableVideos = true;
        public bool NoWorld = false;
        public bool GodMod = false;

        //events used to update state in state manager
        public static System.EventHandler OnStartMenu;
        public static System.EventHandler OnStartGame;

        // Private fields
        CharacterDocument characterDocument;
        int classicSaveIndex = -1;
        GameObject player;
        PlayerEnterExit playerEnterExit;
        PlayerHealth playerHealth;

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

        #endregion

        #region Enums

        public enum StartMethods
        {
            DoNothing,                              // No startup action
            Void,                                   // Start to the Void
            TitleMenu,                              // Open title menu
            TitleMenuFromDeath,                     // Open title menu after death
            NewCharacter,                           // Spawn character to start location in INI
            LoadDaggerfallUnityQuickSave,           // Loads the current quicksave slot (if present)
            //LoadDaggerfallUnitySave,              // TODO: This will replace quicksave option
            LoadClassicSave,                        // Loads a classic save using start save index
        }

        #endregion

        #region Unity

        void Awake()
        {
            // Get player objects
            player = FindPlayer();
            playerEnterExit = FindPlayerEnterExit(player);
            playerHealth = FindPlayerHealth(player);
        }

        void Start()
        {
            ApplyStartSettings();
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
                case StartMethods.LoadDaggerfallUnityQuickSave:
                    StartFromQuickSave();
                    break;
                case StartMethods.LoadClassicSave:
                    if (OverrideSaveIndex != -1) classicSaveIndex = OverrideSaveIndex;
                    StartFromClassicSave();
                    break;
                default:
                    break;
            }
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

            // VSync settings
            if (DaggerfallUnity.Settings.VSync)
                QualitySettings.vSyncCount = 1;
            else
                QualitySettings.vSyncCount = 0;

            // Filter settings
            DaggerfallUnity.Instance.MaterialReader.MainFilterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode;

            // HUD settings
            DaggerfallHUD hud = DaggerfallUI.Instance.DaggerfallHUD;
            if (hud != null)
                hud.ShowCrosshair = DaggerfallUnity.Settings.Crosshair;

            // Weapon hand settings
            WeaponManager weaponManager = GameManager.Instance.WeaponManager;
            weaponManager.RightHandWeapon.LeftHand = DaggerfallUnity.Settings.LeftHandWeapons;

            // Weapon swing settings
            weaponManager.HorizontalThreshold = DaggerfallUnity.Settings.WeaponSwingThreshold;
            weaponManager.VerticalThreshold = DaggerfallUnity.Settings.WeaponSwingThreshold;
            weaponManager.TriggerCount = DaggerfallUnity.Settings.WeaponSwingTriggerCount;

            // GodMode setting
            playerHealth.GodMode = GodMod;

            // Enable/disable videos
            DaggerfallUI.Instance.enableVideos = EnableVideos;
        }

        #endregion

        #region Startup Methods

        void StartVoid()
        {
            DaggerfallUI.Instance.PopToHUD();
            playerEnterExit.DisableAllParents();
            NoWorld = true;
        }

        void StartTitleMenu()
        {
            DaggerfallUI.Instance.PopToHUD();
            playerEnterExit.DisableAllParents();

            if (string.IsNullOrEmpty(PostStartMessage))
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiInitGame);
            else
                DaggerfallUI.PostMessage(PostStartMessage);

            if (OnStartMenu != null)
                OnStartMenu(this, null);
        }

        void StartTitleMenuFromDeath()
        {
            // Reset player death camera
            if (GameManager.Instance.PlayerDeath)
                GameManager.Instance.PlayerDeath.ResetCamera();

            DaggerfallUI.Instance.PopToHUD();
            playerEnterExit.DisableAllParents();

            if (string.IsNullOrEmpty(PostStartMessage))
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiInitGameFromDeath);
            else
                DaggerfallUI.PostMessage(PostStartMessage);

            if (OnStartMenu != null)
                OnStartMenu(this, null);
        }

        void StartFromQuickSave()
        {
            DaggerfallUI.Instance.PopToHUD();
            playerEnterExit.DisableAllParents();
            if (SaveLoadManager.Instance.HasQuickSave())
                SaveLoadManager.Instance.QuickLoad();

            DaggerfallUI.PostMessage(PostStartMessage);
        }

        // Start new character to location specified in INI
        void StartNewCharacter()
        {
            DaggerfallUI.Instance.PopToHUD();

            // Assign character sheet
            PlayerEntity playerEntity = FindPlayerEntity();
            playerEntity.AssignCharacter(characterDocument);

            // Set game time
            DaggerfallUnity.Instance.WorldTime.Now.SetClassicGameStartTime();

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

            // Start game
            GameManager.Instance.PauseGame(false);
            DaggerfallUI.Instance.FadeHUDFromBlack();
            DaggerfallUI.PostMessage(PostStartMessage);

            if (OnStartGame != null)
                OnStartGame(this, null);
        }

        #endregion

        #region Classic Save Startup

        void StartFromClassicSave()
        {
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

            if (NoWorld)
            {
                playerEnterExit.DisableAllParents();
            }
            else
            {
                // Set player to world position
                playerEnterExit.EnableExteriorParent();
                StreamingWorld streamingWorld = FindStreamingWorld();
                int worldX = saveTree.Header.CharacterPosition.Position.WorldX;
                int worldZ = saveTree.Header.CharacterPosition.Position.WorldZ;
                streamingWorld.TeleportToWorldCoordinates(worldX, worldZ);
                streamingWorld.suppressWorld = false;
            }

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
            playerEntity.AssignCharacter(characterDocument, characterRecord.ParsedData.level, characterRecord.ParsedData.startingHealth);

            // Assign items to player entity
            playerEntity.AssignItems(saveTree);

            // Start game
            DaggerfallUI.Instance.PopToHUD();
            GameManager.Instance.PauseGame(false);
            DaggerfallUI.Instance.FadeHUDFromBlack();
            DaggerfallUI.PostMessage(PostStartMessage);

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

        PlayerHealth FindPlayerHealth(GameObject player)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (!playerHealth)
                throw new Exception("Could not find PlayerHealth.");

            return playerHealth;
        }

        PlayerEntity FindPlayerEntity()
        {
            GameObject player = FindPlayer();
            PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;

            return playerEntity;
        }

        #endregion
    }
}
