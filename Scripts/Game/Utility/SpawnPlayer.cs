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
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Player;

namespace DaggerfallWorkshop.Game.Utility
{
    /// <summary>
    /// Spawns player into world based on method.
    /// This will be used mainly during early stages of development.
    /// </summary>
    public class SpawnPlayer : MonoBehaviour
    {
        DaggerfallUnity dfUnity;
        StreamingWorld streamingWorld;
        bool spawnPending = true;

        public SpawnTypes spawnType = SpawnTypes.FromINI;

        public bool SpawnPending
        {
            get { return spawnPending; }
            set { spawnPending = value; }
        }

        public enum SpawnTypes
        {
            FromINI,
        }

        void Update()
        {
            if (!IsReady())
                return;

            if (spawnPending)
                DoSpawn();
        }

        void DoSpawn()
        {
            switch (spawnType)
            {
                case SpawnTypes.FromINI:
                    SpawnFromINI();
                    break;
            }

            spawnPending = false;
        }

        void SpawnFromINI()
        {
            DFLocation location;
            if (!GameObjectHelper.FindMultiNameLocation(DaggerfallUnity.Settings.StartingLocation, out location))
                return;

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

        bool IsReady()
        {
            if (dfUnity == null)
                dfUnity = DaggerfallUnity.Instance;

            if (streamingWorld == null)
                streamingWorld = GameObject.FindObjectOfType<StreamingWorld>();

            if (!dfUnity.IsReady || !dfUnity.IsPathValidated)
                return false;

            return true;
        }
    }
}