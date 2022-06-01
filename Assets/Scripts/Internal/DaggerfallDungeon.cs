// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

//#define SHOW_LAYOUT_TIMES

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game;

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

        // Random monsters
        public int RandomMonsterVariance = 4;

        GameObject startMarker = null;
        GameObject enterMarker = null;
        List<Vector3> debuggerMarkerPositions = null;

        /// <summary>
        /// Gets the scene name for the dungeon at the given location.
        /// </summary>
        public static string GetSceneName(DFLocation location)
        {
            return string.Format("DaggerfallDungeon [Region={0}, Name={1}]", location.RegionName, location.Name);
        }


        public DungeonSummary Summary
        {
            get { return summary; }
        }

        public GameObject StartMarker
        {
            get { return startMarker; }
        }

        public GameObject EnterMarker
        {
            get { return enterMarker; }
        }

        public DaggerfallStaticDoors[] StaticDoorCollections
        {
            get { return EnumerateStaticDoorCollections(); }
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

        public void SetDungeon(DFLocation location, bool importEnemies = true)
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
            summary.LocationType = location.MapTableData.LocationType;
            summary.DungeonType = location.MapTableData.DungeonType;

            // Set texture table from location
            if (DungeonTextureUse == DaggerfallWorkshop.DungeonTextureUse.UseLocation_PartiallyImplemented)
                UseLocationDungeonTextureTable();

            // Perform layout
            startMarker = null;
            LayoutDungeon(location, importEnemies);

            // Seal location
            isSet = true;
            RaiseOnSetDungeonEvent();
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
            DungeonTextureTable = DungeonTextureTables.RandomTextureTableAlternate(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
            ApplyDungeonTextureTable();
        }

        /// <summary>
        /// Helper to check if dungeon is a main story dungeon.
        /// </summary>
        /// <param name="id">ID of dungeon.</param>
        /// <returns>True if dungeon is a main story dungeon.</returns>
        public static bool IsMainStoryDungeon(int id)
        {
            bool mainStoryDungeon = false;
            switch (id)
            {
                case 187853213:         // Daggerfall/Privateer's Hold
                case 630439035:         // Wayrest/Wayrest
                case 1291010263:        // Daggerfall/Daggerfall
                case 6634853:           // Sentinel/Sentinel
                case 19021260:          // Orsinium Area/Orsinium
                case 728811286:         // Wrothgarian Mountains/Shedungent
                case 701948302:         // Dragontail Mountains/Scourg Barrow
                case 83032363:          // Wayrest/Woodborne Hall
                case 1001:              // High Rock sea coast/Mantellan Crux
                case 207828842:         // Menevia/Lysandus' Tomb
                case 9570447:           // Daggerfall/Castle Necromoghan
                case 2352284:           // Betony/Tristore Laboratory
                case 336619236:         // Ykalon/Castle Llugwych
                case 43196334:          // Isle of Balfiera/Direnni Tower
                    mainStoryDungeon = true;
                    break;
                default:
                    break;
            }

            return mainStoryDungeon;
        }

        public void UseLocationDungeonTextureTable()
        {
            // Generates dungeon texture table from random seed
            // RandomDungeonTextures are read from settings.ini. Values are
            // 0 : Classic textures (swamp and woodland texture sets unused)
            // 1 : Textures by climate + classic textures for main story dungeons
            // 2 : Textures by climate for all dungeons
            // 3 : Randomized + classic textures for main story dungeons (method used in earlier DF Unity builds)
            // 4 : Randomized for all dungeons
            bool mainStoryDungeon = IsMainStoryDungeon(Summary.ID);
            int randomDungeonTextures = DaggerfallUnity.Settings.RandomDungeonTextures;
            // If not overriding with other textures (modes 2 and 4), use classic algorithm for main story dungeons
            if (mainStoryDungeon && randomDungeonTextures != 2 && randomDungeonTextures != 4)
                DungeonTextureTable = DungeonTextureTables.RandomTextureTableClassic(Summary.LocationData.Dungeon.RecordElement.Header.LocationId);
            else // Otherwise, use a random texture according to the mode set in settings.ini
            {
                if (randomDungeonTextures < 3)
                    DungeonTextureTable = DungeonTextureTables.RandomTextureTableClassic(Summary.LocationData.Dungeon.RecordElement.Header.LocationId, DaggerfallUnity.Settings.RandomDungeonTextures);
                else
                    DungeonTextureTable = DungeonTextureTables.RandomTextureTableAlternate(Summary.ID);
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

        /// <summary>
        /// Gets special dungeon name (e.g. Castle Daggerfall).
        /// Fallback to current location name if not in a special named dungeon.
        /// </summary>
        public string GetSpecialDungeonName()
        {
            string dungeonName = string.Empty;
            if (summary.RegionName == "Daggerfall" && summary.LocationName == "Daggerfall")
                dungeonName = DaggerfallUnity.Instance.TextProvider.GetText(475);
            else if (summary.RegionName == "Wayrest" && summary.LocationName == "Wayrest")
                dungeonName = DaggerfallUnity.Instance.TextProvider.GetText(476);
            else if (summary.RegionName == "Sentinel" && summary.LocationName == "Sentinel")
                dungeonName = DaggerfallUnity.Instance.TextProvider.GetText(477);
            else
                dungeonName = summary.LocationName;

            return dungeonName.TrimEnd('.');
        }

        /// <summary>
        /// Gets all debugger marker positions for this dungeon.
        /// Not related to gameplay systems like quests. Only used for teleporting player around marker positions using quest debugger.
        /// </summary>
        /// <returns>Array of all debugger marker positions. Can return null or empty.</returns>
        public Vector3[] GetAllDebuggerMarkerPositions()
        {
            EnumerateDebuggerMarkers();
            if (debuggerMarkerPositions != null && debuggerMarkerPositions.Count > 0)
                return debuggerMarkerPositions.ToArray();
            else
                return null;
        }

        #region Private Methods

        private void LayoutDungeon(in DFLocation location, bool importEnemies = true)
        {
#if SHOW_LAYOUT_TIMES
            // Start timing
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            long startTime = stopwatch.ElapsedMilliseconds;
#endif

            // Get player level - use level 1 if game not running (e.g. importing in editor mode)
            float playerLevel = 1;
            if (Application.isPlaying)
                playerLevel = GameManager.Instance.PlayerEntity.Level;

            // Calculate monster power - this is a clamped 0-1 value based on player's level from 1-20
            float monsterPower = Mathf.Clamp01(playerLevel / 20f);

            // Create dungeon layout
            for (int i = 0; i < summary.LocationData.Dungeon.Blocks.Length; i++)
            {
                DFLocation.DungeonBlock block = summary.LocationData.Dungeon.Blocks[i];
                GameObject go = GameObjectHelper.CreateRDBBlockGameObject(
                    block.BlockName,
                    DungeonTextureTable,
                    block.IsStartingBlock,
                    Summary.DungeonType,
                    monsterPower,
                    RandomMonsterVariance,
                    (int)DateTime.Now.Ticks/*Summary.ID*/,      // TODO: Add more options for seed
                    dfUnity.Option_DungeonBlockPrefab,
                    importEnemies);
                go.transform.parent = this.transform;
                go.transform.position = new Vector3(block.X * RDBLayout.RDBSide, 0, block.Z * RDBLayout.RDBSide);

                DaggerfallRDBBlock daggerfallBlock = go.GetComponent<DaggerfallRDBBlock>();
                if (block.IsStartingBlock)
                    FindMarkers(daggerfallBlock, ref block, true); // Assign start marker and enter marker
                else
                    FindMarkers(daggerfallBlock, ref block, false); // Only find water level and palaceblock info from start marker

                summary.LocationData.Dungeon.Blocks[i].WaterLevel = block.WaterLevel;
                summary.LocationData.Dungeon.Blocks[i].CastleBlock = block.CastleBlock;

                // Add water blocks
                RDBLayout.AddWater(go, go.transform.position, block.WaterLevel);
            }

            RemoveOverlappingDoors();

#if SHOW_LAYOUT_TIMES
            // Show timer
            long totalTime = stopwatch.ElapsedMilliseconds - startTime;
            DaggerfallUnity.LogMessage(string.Format("Time to layout dungeon: {0}ms", totalTime), true);
#endif
        }

        // Remove duplicate/overlapping action doors in dungeons
        // Action doors that are intentionally placed flush next to each other (e.g. entrance in Daggerfall Castle) have ~2.25 units between origins
        // Using a tolerance of <1.4 and culling a door found within this limit - any doors this close together are definitely overlapping
        // Note some doors appear sunken in doorframe - not trying to select "best" door based on placement height at this time
        // This process adds approx. 7ms layout time per 100 doors processed - hardly noticeable above layout times of ~1250ms for large dungeons (e.g. Scourg, 206 doors)
        // TODO:
        //  * Try to identify the "best" aligned door
        private void RemoveOverlappingDoors()
        {
            const float tolerance = 1.4f;

            List<Vector3> doorPosRegistry = new List<Vector3>();
            DaggerfallStaticDoors[] staticDoorCollections = EnumerateStaticDoorCollections();
            DaggerfallActionDoor[] actionDoors = GetComponentsInChildren<DaggerfallActionDoor>();

            // Add static exit door to registry - should be just the one
            foreach (DaggerfallStaticDoors collection in staticDoorCollections)
            {
                if (collection.Doors != null && collection.Doors.Length > 0)
                {
                    foreach (StaticDoor staticDoor in collection.Doors)
                    {
                        if (staticDoor.doorType == DoorTypes.DungeonExit)
                        {
                            // Get static door centre in world space and add to registry
                            Vector3 centre = transform.rotation * staticDoor.buildingMatrix.MultiplyPoint3x4(staticDoor.centre) + transform.position;
                            doorPosRegistry.Add(centre);
                            break;
                        }
                    }
                }
            }

            // Check all action doors against registry and reject if close enough to overlap another door
            foreach (DaggerfallActionDoor actionDoor in actionDoors)
            {
                bool duplicateFound = false;
                foreach (Vector3 pos in doorPosRegistry)
                {
                    if (Vector3.Distance(actionDoor.transform.position, pos) < tolerance)
                    {
                        actionDoor.gameObject.SetActive(false);
                        duplicateFound = true;
                        Debug.Log(">Disabled overlapping action door");
                        break;
                    }
                }
                if (!duplicateFound)
                {
                    doorPosRegistry.Add(actionDoor.transform.position);
                }
            }
        }

        // Finds start and enter markers, should be called with true for starting block, otherwise false to just get water level and castle block data
        private void FindMarkers(DaggerfallRDBBlock dfBlock, ref DFLocation.DungeonBlock block, bool assign)
        {
            if (!dfBlock)
                throw new Exception("DaggerfallDungeon: dfBlock cannot be null.");

            if (dfBlock.StartMarkers != null && dfBlock.StartMarkers.Length > 0)
            {
                // There should only be one start marker per start block
                // This message will let us know if more than one is found
                if (dfBlock.StartMarkers.Length > 1)
                    DaggerfallUnity.LogMessage("DaggerfallDungeon: Multiple 'Start' markers found. Using first marker.", true);

                if (assign)
                    startMarker = dfBlock.StartMarkers[0];

                Billboard dfBillboard = dfBlock.StartMarkers[0].GetComponent<Billboard>();
                block.WaterLevel = dfBillboard.Summary.WaterLevel;
                block.CastleBlock = dfBillboard.Summary.CastleBlock;
            }
            else // No water
                block.WaterLevel = 10000;

            if (dfBlock.EnterMarkers != null && dfBlock.EnterMarkers.Length > 0)
            {

                // There should only be one enter marker per start block
                // This message will let us know if more than one is found
                if (dfBlock.EnterMarkers.Length > 1)
                    DaggerfallUnity.LogMessage("DaggerfallDungeon: Multiple 'Enter' markers found. Using first marker.", true);

                if (assign)
                    enterMarker = dfBlock.EnterMarkers[0];
            }
        }

        // Enumerates all static doors in child blocks
        DaggerfallStaticDoors[] EnumerateStaticDoorCollections()
        {
            return GetComponentsInChildren<DaggerfallStaticDoors>();
        }

        // Enumerates marker positions in dungeon for player to teleport around using quest debugger
        // Not used for any gameplay system and does not collect any other information about marker other than position
        // Currently collects all Enter, Start, Quest, Item markers
        void EnumerateDebuggerMarkers()
        {
            const int editorFlatArchive = 199;
            const int enterMarkerIndex = 8;
            const int startMarkerIndex = 10;
            const int spawnMarkerFlatIndex = 11;
            const int itemMarkerFlatIndex = 18;

            // Only enumerate debugger marker positions once for this dungeon object
            if (debuggerMarkerPositions != null)
                return;
            else
                debuggerMarkerPositions = new List<Vector3>();

            // Step through dungeon layout to find all blocks with markers
            foreach (var dungeonBlock in summary.LocationData.Dungeon.Blocks)
            {
                // Get block data
                DFBlock blockData = DaggerfallUnity.Instance.ContentReader.BlockFileReader.GetBlock(dungeonBlock.BlockName);

                // Iterate all groups
                foreach (DFBlock.RdbObjectRoot group in blockData.RdbBlock.ObjectRootList)
                {
                    // Skip empty object groups
                    if (null == group.RdbObjects)
                        continue;

                    // Look for flats in this group
                    foreach (DFBlock.RdbObject obj in group.RdbObjects)
                    {
                        // Get marker ID
                        ulong markerID = (ulong)(blockData.Position + obj.Position);

                        // Look for editor flats and collect marker positions adjusted for dungeon block position
                        Vector3 position = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;
                        if (obj.Type == DFBlock.RdbResourceTypes.Flat)
                        {
                            if (obj.Resources.FlatResource.TextureArchive == editorFlatArchive)
                            {
                                Vector3 dungeonBlockPosition = new Vector3(dungeonBlock.X * RDBLayout.RDBSide, 0, dungeonBlock.Z * RDBLayout.RDBSide);
                                switch (obj.Resources.FlatResource.TextureRecord)
                                {
                                    case enterMarkerIndex:
                                    case startMarkerIndex:
                                    case itemMarkerFlatIndex:
                                    case spawnMarkerFlatIndex:
                                        debuggerMarkerPositions.Add(dungeonBlockPosition + position);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
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

        /// <summary>
        /// An event raised after a dungeon has been set and its layout has been performed.
        /// </summary>
        public static event Action<DaggerfallDungeon> OnSetDungeon;
        private void RaiseOnSetDungeonEvent()
        {
            if (OnSetDungeon != null)
                OnSetDungeon(this);
        }
    }
}