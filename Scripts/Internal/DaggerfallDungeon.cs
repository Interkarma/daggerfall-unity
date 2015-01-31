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
        public DungeonTextureUse DungeonTextureUse = DungeonTextureUse.Disabled;
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
            summary.LocationType = location.MapTableData.Type;
            summary.DungeonType = location.MapTableData.DungeonType;

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
            // Valid dungeon textures table indices
            int[] valids = new int[]
            {
                019, 020, 022, 023, 024, 068,
                119, 120, 122, 123, 124, 168,
                319, 320, 322, 323, 324, 368,
                419, 420, 422, 423, 424, 468,
            };

            // Repopulate table
            for (int i = 0; i < DungeonTextureTable.Length; i++)
            {
                DungeonTextureTable[i] = valids[UnityEngine.Random.Range(0, valids.Length)];
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
                dm.SetDungeonTextures(dfUnity, DungeonTextureTable);
            }
        }

        #region Private Methods

        private void LayoutDungeon(ref DFLocation location)
        {
            // Start timing
            Stopwatch stopwatch = Stopwatch.StartNew();
            long startTime = stopwatch.ElapsedMilliseconds;

            // Create dungeon layout
            foreach (var block in location.Dungeon.Blocks)
            {
                GameObject go = RDBLayout.CreateGameObject(dfUnity, block.BlockName);
                go.transform.parent = this.transform;
                go.transform.position = new Vector3(block.X * RDBLayout.RDBSide, 0, block.Z * RDBLayout.RDBSide);
                if (block.IsStartingBlock)
                    FindStartMarker(go.GetComponent<DaggerfallBlock>());
            }

            // Show timer
            long totalTime = stopwatch.ElapsedMilliseconds - startTime;
            DaggerfallUnity.LogMessage(string.Format("Time to layout dungeon: {0}ms", totalTime), true);
        }

        // Orsinium defines two blocks at [-1,-1]
        private void LayoutOrsinium(ref DFLocation location)
        {
            // Create dungeon layout and handle misplaced block
            foreach (var block in location.Dungeon.Blocks)
            {
                if (block.X == -1 && block.Z == -1 && block.BlockName == "N0000065.RDB")
                    continue;

                GameObject go = RDBLayout.CreateGameObject(dfUnity, block.BlockName);
                go.transform.parent = this.transform;
                go.transform.position = new Vector3(block.X * RDBLayout.RDBSide, 0, block.Z * RDBLayout.RDBSide);
                if (block.IsStartingBlock)
                    FindStartMarker(go.GetComponent<DaggerfallBlock>());
            }
        }

        // Finds start marker, should only be called for starting block
        private void FindStartMarker(DaggerfallBlock dfBlock)
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