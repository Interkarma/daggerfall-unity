// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    public class DaggerfallDungeon : MonoBehaviour
    {
        bool isSet = false;
        DaggerfallUnity dfUnity;

        [SerializeField]
        private DungeonSummary summary;

        // Dungeon texture swaps
        public DungeonTextureUse DungeonTextureUse = DungeonTextureUse.UseLocation_PartiallyImplemented;
        public int[] DungeonTextureTable = new int[] { 119, 120, 122, 123, 124, 168 };

        GameObject startMarker = null;

        public DungeonSummary Summary
        {
            get { return summary; }
        }

        public GameObject StartMarker
        {
            get { return startMarker; }
        }

        [Serializable]
        public struct DungeonSummary
        {
            public int ID;
            public string RegionName;
            public string LocationName;
            public DFLocation LocationData;
            public DFRegion.LocationTypes LocationType;
            public DFRegion.DungeonTypes DungeonType;
        }

        public void SetDungeon(DFLocation location)
        {
            if (!ReadyCheck())
                return;

            // Validate
            if (this.isSet)
                throw new Exception("This location has already been set.");
            if (!location.Loaded)
                throw new Exception("DFLocation not loaded.");
            if (!location.HasDungeon)
                throw new Exception("DFLocation does not contain a dungeon.");

            // Set summary
            summary = new DungeonSummary();
            summary.ID = location.MapTableData.MapId;
            summary.RegionName = location.RegionName;
            summary.LocationName = location.Name;
            summary.LocationData = location;
            summary.LocationType = location.MapTableData.Type;
            summary.DungeonType = location.MapTableData.DungeonType;

            // Set texture table from location
            if (DungeonTextureUse == DaggerfallWorkshop.DungeonTextureUse.UseLocation_PartiallyImplemented)
                UseLocationDungeonTextureTable();

            // Perform layout
            startMarker = null;
            if (location.Name == "Orsinium")
                LayoutOrsinium(ref location);
            else
                LayoutDungeon(ref location);

            // Seal location
            isSet = true;
        }

        public void ResetDungeonTextureTable()
        {
            DungeonTextureTable[0] = 119;
            DungeonTextureTable[1] = 120;
            DungeonTextureTable[2] = 122;
            DungeonTextureTable[3] = 123;
            DungeonTextureTable[4] = 124;
            DungeonTextureTable[5] = 168;
            ApplyDungeonTextureTable();
        }

        public void RandomiseDungeonTextureTable()
        {
            DungeonTextureTable = StaticTextureTables.RandomTextureTable(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
            ApplyDungeonTextureTable();
        }

        public void UseLocationDungeonTextureTable()
        {
            // Hard-coding location texture tables as missing information to generate at runtime
            // This will be replaced with true implementation when possible
            switch (Summary.ID)
            {
                case 187853213:         // Daggerfall/Privateer's Hold
                    DungeonTextureTable = StaticTextureTables.PrivateersHold;
                    break;
                case 630439035:         // Wayrest/Wayrest
                    DungeonTextureTable = StaticTextureTables.Wayrest;
                    break;
                case 1291010263:        // Daggerfall/Daggerfall
                    DungeonTextureTable = StaticTextureTables.Daggerfall;
                    break;
                case 6634853:           // Sentinel/Sentinel
                    DungeonTextureTable = StaticTextureTables.Sentinel;
                    break;
                case 19021260:          // Orsinium Area/Orsinium
                    DungeonTextureTable = StaticTextureTables.Orsinium;
                    break;
                case 728811286:         // Wrothgarian Mountains/Shedungent
                    DungeonTextureTable = StaticTextureTables.Shedungent;
                    break;
                case 701948302:         // Dragontail Mountains/Scourg Barrow
                    DungeonTextureTable = StaticTextureTables.ScourgBarrow;
                    break;
                case 83032363:          // Wayrest/Woodborne Hall
                    DungeonTextureTable = StaticTextureTables.WoodborneHall;
                    break;
                case 1001:              // High Rock sea coast/Mantellan Crux
                    DungeonTextureTable = StaticTextureTables.MantellanCrux;
                    break;
                case 207828842:         // Menevia/Lysandus' Tomb
                    DungeonTextureTable = StaticTextureTables.LysandusTomb;
                    break;
                default:                // Everywhere else - random table seeded from ID
                    DungeonTextureTable = StaticTextureTables.RandomTextureTable(Summary.ID);
                    break;
            }

            ApplyDungeonTextureTable();
        }

        public void ApplyDungeonTextureTable()
        {
            // Do nothing if not ready
            if (!ReadyCheck())
                return;

            // Process all DaggerfallMesh child components
            DaggerfallMesh[] meshArray = GetComponentsInChildren<DaggerfallMesh>();
            foreach (var dm in meshArray)
            {
                dm.SetDungeonTextures(DungeonTextureTable);
            }
        }

        public int GetPlayerBlockIndex(Vector3 playerPos)
        {
            if (!summary.LocationData.Loaded)
                return -1;

            // Check if player is inside any block of dungeon
            // RDB blocks are laid out in 2D and have no vertical extents
            // We can just check using rects, which is very fast
            Rect rect = new Rect();
            DFLocation.DungeonBlock block;
            Vector2 pos = new Vector2(playerPos.x, playerPos.z);
            for (int i = 0; i < summary.LocationData.Dungeon.Blocks.Length; i++)
            {
                block = summary.LocationData.Dungeon.Blocks[i];
                rect.xMin = transform.position.x + block.X * RDBLayout.RDBSide;
                rect.xMax = rect.xMin + RDBLayout.RDBSide;
                rect.yMin = transform.position.y + block.Z * RDBLayout.RDBSide;
                rect.yMax = rect.yMin + RDBLayout.RDBSide;

                if (rect.Contains(pos))
                    return i;
            }

            return -1;
        }

        public bool GetBlockData(int index, out DFLocation.DungeonBlock blockDataOut)
        {
            if (!summary.LocationData.Loaded)
            {
                blockDataOut = new DFLocation.DungeonBlock();
                return false;
            }

            blockDataOut = summary.LocationData.Dungeon.Blocks[index];

            return true;
        }

        #region Private Methods

        private void LayoutDungeon(ref DFLocation location)
        {
            //// Start timing
            //Stopwatch stopwatch = Stopwatch.StartNew();
            //long startTime = stopwatch.ElapsedMilliseconds;

            // Create dungeon layout
            foreach (var block in location.Dungeon.Blocks)
            {
                GameObject go = RDBLayout.CreateGameObject(block.BlockName, block.IsStartingBlock, DungeonTextureTable, Summary.DungeonType, Summary.ID);
                go.transform.parent = this.transform;
                go.transform.position = new Vector3(block.X * RDBLayout.RDBSide, 0, block.Z * RDBLayout.RDBSide);

                DaggerfallRDBBlock daggerfallBlock = go.GetComponent<DaggerfallRDBBlock>();
                if (block.IsStartingBlock)
                    FindStartMarker(daggerfallBlock);
            }

            //// Show timer
            //long totalTime = stopwatch.ElapsedMilliseconds - startTime;
            //DaggerfallUnity.LogMessage(string.Format("Time to layout dungeon: {0}ms", totalTime), true);
        }

        // Orsinium defines two blocks at [-1,-1]
        private void LayoutOrsinium(ref DFLocation location)
        {
            // Create dungeon layout and handle misplaced block
            foreach (var block in location.Dungeon.Blocks)
            {
                if (block.X == -1 && block.Z == -1 && block.BlockName == "N0000065.RDB")
                    continue;

                GameObject go = RDBLayout.CreateGameObject(block.BlockName, block.IsStartingBlock, DungeonTextureTable, Summary.DungeonType, Summary.ID);
                go.transform.parent = this.transform;
                go.transform.position = new Vector3(block.X * RDBLayout.RDBSide, 0, block.Z * RDBLayout.RDBSide);

                DaggerfallRDBBlock daggerfallBlock = go.GetComponent<DaggerfallRDBBlock>();
                if (block.IsStartingBlock)
                    FindStartMarker(daggerfallBlock);
            }
        }

        // Finds start marker, should only be called for starting block
        private void FindStartMarker(DaggerfallRDBBlock dfBlock)
        {
            if (!dfBlock)
                throw new Exception("DaggerfallDungeon: dfBlock cannot be null.");
            if (dfBlock.StartMarkers.Length == 0)
            {
                DaggerfallUnity.LogMessage("DaggerfallDungeon: No start markers found in block.", true);
                return;
            }

            // There should only be one start marker per start block
            // This message will let us know if more than one is found
            if (dfBlock.StartMarkers.Length > 1)
            {
                DaggerfallUnity.LogMessage("DaggerfallDungeon: Multiple start markers found. Using first marker.", true);
            }

            startMarker = dfBlock.StartMarkers[0];
        }

        private bool ReadyCheck()
        {
            // Ensure we have a DaggerfallUnity reference
            if (dfUnity == null)
            {
                dfUnity = DaggerfallUnity.Instance;
            }

            // Do nothing if DaggerfallUnity not ready
            if (!dfUnity.IsReady)
            {
                DaggerfallUnity.LogMessage("DaggerfallDungeon: DaggerfallUnity component is not ready. Have you set your Arena2 path?");
                return false;
            }

            return true;
        }

        #endregion
    }
}