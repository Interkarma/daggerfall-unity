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

namespace DaggerfallWorkshop.Game.Utility
{
    /// <summary>
    /// This component is intended to survive from the start screen into main game.
    /// It will carry settings for new characters or detail an existing game to load.
    /// Uses DontDestroyOnLoad() to keep information live during transition.
    /// </summary>
    public class StartGameBehaviour : MonoBehaviour
    {
        bool doNotDeploy = true;
        StartMethods startMethod = StartMethods.Nothing;
        StartMethods lastMethod = StartMethods.Nothing;
        CharacterSheet characterSheet;
        int classicSaveIndex = -1;

        public StartMethods StartMethod
        {
            get { return startMethod; }
            set { startMethod = value; }
        }

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

        public enum StartMethods
        {
            Nothing,
            FromINI,
            DefaultCharacter,
            NewCharacter,
            LoadClassicSave,
            LoadDaggerfallUnitySave,
        }

        void Awake()
        {
            DontDestroyOnLoad(transform.gameObject);
        }

        void OnLevelWasLoaded()
        {
            // Lower doNotDeploy flag after level transition to main scene
            if (Application.loadedLevel == 1)
                doNotDeploy = false;
        }

        void Update()
        {
            if (startMethod != lastMethod && !doNotDeploy)
            {
                lastMethod = startMethod;
                InvokeStartMethod();
            }
        }

        void InvokeStartMethod()
        {
            switch(startMethod)
            {
                case StartMethods.FromINI:
                    StartFromINI();
                    break;
                case StartMethods.NewCharacter:
                    StartNewCharacter();
                    break;
                case StartMethods.LoadClassicSave:
                    StartFromClassicSave();
                    break;
                default:
                    break;
            }
        }

        #region INI Startup

        void StartFromINI()
        {
            DFLocation location;
            if (!GameObjectHelper.FindMultiNameLocation(DaggerfallUnity.Settings.StartingLocation, out location))
                return;

            StreamingWorld streamingWorld = FindStreamingWorld();
            if (DaggerfallUnity.Settings.StartInDungeon)
            {
                PlayerEnterExit playerEnterExit = GetComponent<PlayerEnterExit>();
                playerEnterExit.StartDungeonInterior(location);
                streamingWorld.suppressWorld = false;
            }
            else
            {
                DFPosition mapPixel = MapsFile.LongitudeLatitudeToMapPixel((int)location.MapTableData.Longitude, (int)location.MapTableData.Latitude);
                streamingWorld.MapPixelX = mapPixel.X;
                streamingWorld.MapPixelY = mapPixel.Y;
                streamingWorld.suppressWorld = false;
            }
        }

        #endregion

        #region New Game Startup

        void StartNewCharacter()
        {
            PlayerEntity playerEntity = FindPlayerEntity();
            playerEntity.AssignCharacter(characterSheet);
        }

        #endregion

        #region Classic Save Startup

        void StartFromClassicSave()
        {
            // Save index must be in range
            if (classicSaveIndex < 0 || classicSaveIndex >= 6)
                throw new IndexOutOfRangeException("classicSaveIndex out of range.");

            // Open saves in parent path of Arena2 folder
            string path = Path.GetDirectoryName(DaggerfallUnity.Instance.Arena2Path);
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

        PlayerEntity FindPlayerEntity()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (!player)
                throw new Exception("Could not find Player.");

            PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;

            return playerEntity;
        }

        #endregion
    }
}
