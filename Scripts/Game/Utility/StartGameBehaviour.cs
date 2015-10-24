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

        // Constants
        string fallbackLocatioName = "Daggerfall/Privateer's Hold";

        // Editor properties
        public StartMethods StartMethod = StartMethods.Nothing;
        public bool EnableVideos = true;
        public bool GodMod = false;

        // Private fields
        CharacterSheet characterSheet;
        int classicSaveIndex = -1;
        GameObject player;
        PlayerEnterExit playerEnterExit;
        PlayerHealth playerHealth;

        #endregion

        #region Properties

        public CharacterSheet CharacterSheet
        {
            get { return characterSheet; }
            set { characterSheet = value; }
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
            Nothing,                                // No startup action
            TitleMenu,                              // Open title menu
            TitleMenuFromDeath,                     // Open title menu after death
            NewCharacter,                           // Spawn character to start location in INI
            LoadDaggerfallUnityQuickSave,           // Loads the current quicksave slot (if present)
            //LoadDaggerfallUnitySave,              // TODO: This will replace quicksave option
            LoadClassicSave,                        // Loads a classic save
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
            if (StartMethod != StartMethods.Nothing)
            {
                DaggerfallUI.Instance.PopToHUD();
                GameManager.Instance.PauseGame(true);
                InvokeStartMethod();
                StartMethod = StartMethods.Nothing;
            }
        }

        #endregion

        #region Private Methods

        void InvokeStartMethod()
        {
            switch (StartMethod)
            {
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
            weaponManager.RightHandWeapon.LeftHand = DaggerfallUnity.Settings.ShowWeaponLeftHand;

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

        void StartTitleMenu()
        {
            playerEnterExit.DisableAllParents();
            DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiInitGame);
        }

        void StartTitleMenuFromDeath()
        {
            // Reset player death camera
            if (GameManager.Instance.PlayerDeath)
                GameManager.Instance.PlayerDeath.ResetCamera();

            playerEnterExit.DisableAllParents();
            DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiInitGameFromDeath);
        }

        void StartFromQuickSave()
        {
            playerEnterExit.DisableAllParents();
            if (SaveLoadManager.Instance.HasQuickSave())
                SaveLoadManager.Instance.QuickLoad();
        }

        // Start new character to location specified in INI
        void StartNewCharacter()
        {
            // Assign character sheet
            PlayerEntity playerEntity = FindPlayerEntity();
            playerEntity.AssignCharacter(characterSheet);

            // Get start parameters
            string startingLocationName = DaggerfallUnity.Settings.StartingLocation;
            bool startInDungeon = DaggerfallUnity.Settings.StartInDungeon;

            // Find start location
            DFLocation location;
            if (!GameObjectHelper.FindMultiNameLocation(startingLocationName, out location))
            {
                // Could not find INI location specified, fallback to Privateer's Hold
                startInDungeon = true;
                DaggerfallUnity.LogMessage(string.Format("Could not find {0}, fallback to {1}", startingLocationName, fallbackLocatioName), true);
                if (!GameObjectHelper.FindMultiNameLocation(fallbackLocatioName, out location))
                    throw new Exception("Could not find INI location or fallback location. There could be a problem with Arena2 folder.");
            }

            // Start at specified location
            if (startInDungeon)
            {
                playerEnterExit.EnableDungeonParent();
                playerEnterExit.StartDungeonInterior(location);
            }
            else
            {
                playerEnterExit.EnableExteriorParent();
                StreamingWorld streamingWorld = FindStreamingWorld();
                DFPosition mapPixel = MapsFile.LongitudeLatitudeToMapPixel((int)location.MapTableData.Longitude, (int)location.MapTableData.Latitude);
                streamingWorld.MapPixelX = mapPixel.X;
                streamingWorld.MapPixelY = mapPixel.Y;
                streamingWorld.suppressWorld = false;
            }

            // Start game
            GameManager.Instance.PauseGame(false);
            DaggerfallUI.Instance.FadeHUDFromBlack();
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
            if (!saveGames.OpenSave(classicSaveIndex))
                throw new Exception(string.Format("Could not open save index {0}", classicSaveIndex));

            // Get required save data
            SaveTree saveTree = saveGames.SaveTree;
            SaveVars saveVars = saveGames.SaveVars;

            // Set player to world position
            playerEnterExit.EnableExteriorParent();
            StreamingWorld streamingWorld = FindStreamingWorld();
            int worldX = saveTree.Header.CharacterPosition.Position.WorldX;
            int worldZ = saveTree.Header.CharacterPosition.Position.WorldZ;
            streamingWorld.TeleportToWorldCoordinates(worldX, worldZ);
            streamingWorld.suppressWorld = false;

            // Set game time
            DaggerfallUnity.Instance.WorldTime.Now.FromClassicDaggerfallTime(saveVars.GameTime);

            // Get character record
            List<SaveTreeBaseRecord> records = saveTree.FindRecords(RecordTypes.Character);
            if (records.Count != 1)
                throw new Exception("SaveTree CharacterRecord not found.");

            // Get prototypical character sheet data
            CharacterRecord characterRecord = (CharacterRecord)records[0];
            characterSheet = characterRecord.ToCharacterSheet();

            // Assign data to player entity
            PlayerEntity playerEntity = FindPlayerEntity();
            playerEntity.AssignCharacter(characterSheet, characterRecord.ParsedData.level, characterRecord.ParsedData.startingHealth);

            // Start game
            GameManager.Instance.PauseGame(false);
            DaggerfallUI.Instance.FadeHUDFromBlack();
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
