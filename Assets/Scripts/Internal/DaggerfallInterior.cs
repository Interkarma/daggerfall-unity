// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Nystul, Hazelnut
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Banking;

namespace DaggerfallWorkshop
{
    public class DaggerfallInterior : MonoBehaviour
    {
        const int doorModelId = 9800;
        const int ladderModelId = 41409;
        const int propModelType = 3;

        private const int posMask = 0x3FF;  // 10 bits

        const uint houseContainerObjectGroup = 418;
        const uint containerObjectGroupOffset = 41000;
        static List<uint> shopShelvesObjectGroupIndices = new List<uint> { 5, 6, 11, 12, 13, 14, 15, 16, 17, 18, 19, 26, 28, 29, 31, 35, 36, 37, 40, 42, 44, 46, 47, 48, 49, 808 };
        static List<uint> houseContainerObjectGroupIndices = new List<uint> { 3, 4, 7, 8, 27, 32, 33, 34, 35, 37, 38, 50, 51 };

        // Building data for map layout, indicates no activation components needed.
        static PlayerGPS.DiscoveredBuilding mapBD = new PlayerGPS.DiscoveredBuilding {
            buildingType = DFLocation.BuildingTypes.AllValid
        };

        DaggerfallUnity dfUnity;
        DFBlock blockData;
        DFBlock.RmbSubRecord recordData;
        ModelCombiner combiner = new ModelCombiner();
        ClimateBases climateBase = ClimateBases.Temperate;
        ClimateSeason climateSeason = ClimateSeason.Summer;
        List<InteriorEditorMarker> markers = new List<InteriorEditorMarker>();
        List<Vector3> spawnPoints = new List<Vector3>();
        StaticDoor entryDoor;
        Transform doorOwner;

        public struct InteriorEditorMarker
        {
            public InteriorMarkerTypes type;
            public GameObject gameObject;
        }

        public enum InteriorMarkerTypes
        {
            Rest = 4,
            Enter = 8,
            Treasure = 19,
            LadderBottom = 21,
            LadderTop = 22,
        }

        /// <summary>
        /// Gets the scene name for the interior behind the given door.
        /// </summary>
        public static string GetSceneName(DFLocation location, StaticDoor door)
        {
            return GetSceneName(location.MapTableData.MapId, door.buildingKey);
        }
        public static string GetSceneName(int mapID, int buildingKey)
        {
            return string.Format("DaggerfallInterior [MapID={0}, BuildingKey={1}]", mapID, buildingKey);
        }

        /// <summary>
        /// Gets transform owning door array.
        /// </summary>
        public Transform DoorOwner
        {
            get { return doorOwner; }
        }

        public DFLocation.BuildingData BuildingData
        {
            get { return blockData.RmbBlock.FldHeader.BuildingDataList[entryDoor.recordIndex]; }
        }

        /// <summary>
        /// Gets door array from owner.
        /// </summary>
        public DaggerfallStaticDoors ExteriorDoors
        {
            get { return (doorOwner) ? doorOwner.GetComponent<DaggerfallStaticDoors>() : null; }
        }

        /// <summary>
        /// Gets the door player clicked on to enter building.
        /// </summary>
        public StaticDoor EntryDoor
        {
            get { return entryDoor; }
        }

        /// <summary>
        /// Gets array of markers in this building interior.
        /// </summary>
        public InteriorEditorMarker[] Markers
        {
            get { return markers.ToArray(); }
        }

        /// <summary>
        /// Gets array of spawn points in this building interior.
        /// </summary>
        public Vector3[] SpawnPoints
        {
            get { return spawnPoints.ToArray(); }
        }

        void Start()
        {
            dfUnity = DaggerfallUnity.Instance;
        }

        /// <summary>
        /// Layout interior based on data in exterior door and optional location for climate settings.
        /// </summary>
        /// <param name="doorOwner">Parent transform owning door array.</param>
        /// <param name="door">Exterior door player clicked on.</param>
        /// <returns>True if successful.</returns>
        public bool DoLayout(Transform doorOwner, StaticDoor door, ClimateBases climateBase, PlayerGPS.DiscoveredBuilding buildingData)
        {
            if (dfUnity == null)
                dfUnity = DaggerfallUnity.Instance;

            // Use specified climate
            this.climateBase = climateBase;

            // Save exterior information
            this.entryDoor = door;
            this.doorOwner = doorOwner;

            // Get block data
            blockData = dfUnity.ContentReader.BlockFileReader.GetBlock(door.blockIndex);
            if (blockData.Type != DFBlock.BlockTypes.Rmb)
                throw new Exception(string.Format("Could not load RMB block index {0}", door.blockIndex), null);

            // Get record data
            recordData = blockData.RmbBlock.SubRecords[door.recordIndex];
            if (recordData.Interior.Header.Num3dObjectRecords == 0)
                throw new Exception(string.Format("No interior 3D models found for record index {0}", door.recordIndex), null);

            // Layout interior data
            AddModels(buildingData);
            AddFlats(buildingData);
            AddPeople(buildingData);
            AddActionDoors();
            AddSpawnPoints();

            return true;
        }

        /// <summary>
        /// Layout interior for automap based on data in exterior door and optional location for climate settings.
        /// </summary>
        /// <param name="doorOwner">Parent transform owning door array.</param>
        /// <param name="door">Exterior door player clicked on.</param>
        /// <returns>True if successful.</returns>
        public bool DoLayoutAutomap(Transform doorOwner, StaticDoor door, ClimateBases climateBase)
        {
            if (dfUnity == null)
                dfUnity = DaggerfallUnity.Instance;

            // Use specified climate
            this.climateBase = climateBase;

            // Save exterior information
            this.entryDoor = door;
            this.doorOwner = doorOwner;

            // Get block data
            blockData = dfUnity.ContentReader.BlockFileReader.GetBlock(door.blockIndex);
            if (blockData.Type != DFBlock.BlockTypes.Rmb)
                throw new Exception(string.Format("Could not load RMB block index {0}", door.blockIndex), null);

            // Get record data
            recordData = blockData.RmbBlock.SubRecords[door.recordIndex];
            if (recordData.Interior.Header.Num3dObjectRecords == 0)
                throw new Exception(string.Format("No interior 3D models found for record index {0}", door.recordIndex), null);

            // Layout interior data
            AddModels(mapBD);

            return true;
        }

        /// <summary>
        /// Finds closest entrance marker to door position.
        /// </summary>
        /// <param name="playerPos">Player position in world space.</param>
        /// <param name="closestMarkerOut">Closest enter marker to door position.</param>
        /// <returns>True if successful.</returns>
        public bool FindClosestEnterMarker(Vector3 playerPos, out Vector3 closestMarkerOut)
        {
            bool foundOne = false;
            float minDistance = float.MaxValue;
            closestMarkerOut = Vector3.zero;
            for (int i = 0; i < markers.Count; i++)
            {
                // Must be an enter marker 199.8
                // Sometimes marker 199.4 is used where the 199.8 enter marker should be
                // Being a little forgiving and also accepting 199.4 as enter marker
                if (markers[i].type != InteriorMarkerTypes.Enter && markers[i].type != InteriorMarkerTypes.Rest)
                    continue;

                // Refine to closest enter marker
                float distance = Vector3.Distance(playerPos, markers[i].gameObject.transform.position);
                if (distance < minDistance || !foundOne)
                {
                    closestMarkerOut = markers[i].gameObject.transform.position;
                    minDistance = distance;
                    foundOne = true;
                }
            }

            if (!foundOne)
            {
                closestMarkerOut = Vector3.zero;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Find a specific marker. Will stop searching at first item found, or at a random-ish marker if random=true.
        /// </summary>
        /// <returns>True if at least one marker found.</returns>
        public bool FindMarker(out Vector3 markerOut, InteriorMarkerTypes type, bool random = false)
        {
            markerOut = Vector3.zero;

            for (int i = 0; i < markers.Count; i++)
            {
                if (markers[i].type == type)
                {
                    markerOut = markers[i].gameObject.transform.position;
                    if (!random || UnityEngine.Random.Range(0, 2) == 0)
                        return true;
                }
            }

            return !(markerOut == Vector3.zero);
        }

        #region Private Methods

        /// <summary>
        /// Add interior models.
        /// </summary>
        private void AddModels(PlayerGPS.DiscoveredBuilding buildingData)
        {
            List<StaticDoor> doors = new List<StaticDoor>();
            GameObject node = new GameObject("Models");
            GameObject doorsNode = new GameObject("Doors");
            node.transform.parent = this.transform;
            doorsNode.transform.parent = this.transform;

            // Iterate through models in this subrecord
            combiner.NewCombiner();
            foreach (DFBlock.RmbBlock3dObjectRecord obj in recordData.Interior.Block3dObjectRecords)
            {
                bool stopCombine = false;

                // Get model data
                ModelData modelData;
                dfUnity.MeshReader.GetModelData(obj.ModelIdNum, out modelData);

                // Get model position by type (3 seems to indicate props/clutter)
                // Also stop these from being combined as some may carry a loot container
                Vector3 modelPosition;
                if (obj.ObjectType == propModelType)
                {
                    // Props axis needs to be transformed to lowest Y point
                    Vector3 bottom = modelData.Vertices[0];
                    for (int i = 0; i < modelData.Vertices.Length; i++)
                    {
                        if (modelData.Vertices[i].y < bottom.y)
                            bottom = modelData.Vertices[i];
                    }
                    modelPosition = new Vector3(obj.XPos, obj.YPos, obj.ZPos) * MeshReader.GlobalScale;
                    modelPosition += new Vector3(0, -bottom.y, 0);
                    stopCombine = true;
                }
                else
                {
                    modelPosition = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;
                }

                // Stop special object from being combined
                if (obj.ModelIdNum == ladderModelId)
                    stopCombine = true;

                // Get model transform
                Vector3 modelRotation = new Vector3(0, -obj.YRotation / BlocksFile.RotationDivisor, 0);
                Matrix4x4 modelMatrix = Matrix4x4.TRS(modelPosition, Quaternion.Euler(modelRotation), Vector3.one);

                // Does this model have doors?
                if (modelData.Doors != null)
                    doors.AddRange(GameObjectHelper.GetStaticDoors(ref modelData, entryDoor.blockIndex, entryDoor.recordIndex, modelMatrix));

                // Inject custom GameObject if available
                GameObject go = MeshReplacement.ImportCustomGameobject(obj.ModelIdNum, node.transform, modelMatrix);

                // Otherwise use Daggerfall mesh - combine or add
                if (!go)
                {
                    if (dfUnity.Option_CombineRMB && !stopCombine)
                    {
                        combiner.Add(ref modelData, modelMatrix);
                    }
                    else
                    {
                        // Add individual GameObject
                        go = GameObjectHelper.CreateDaggerfallMeshGameObject(obj.ModelIdNum, node.transform, dfUnity.Option_SetStaticFlags);
                        go.transform.position = modelMatrix.GetColumn(3);
                        go.transform.rotation = GameObjectHelper.QuaternionFromMatrix(modelMatrix);

                        // Update climate
                        DaggerfallMesh dfMesh = go.GetComponent<DaggerfallMesh>();
                        dfMesh.SetClimate(climateBase, climateSeason, WindowStyle.Disabled);
                    }
                }

                // Make ladder collider convex
                if (obj.ModelIdNum == ladderModelId)
                {
                    go.GetComponent<MeshCollider>().convex = true;
                    go.AddComponent<DaggerfallLadder>();
                }

                // Optionally add action objects to specific furniture items (e.g. loot containers), except when laying out map (buildingType=AllValid)
                if (obj.ObjectType == propModelType && buildingData.buildingType != DFLocation.BuildingTypes.AllValid)
                    AddFurnitureAction(obj, go, buildingData);
            }

            // Add combined GameObject
            if (dfUnity.Option_CombineRMB)
            {
                if (combiner.VertexCount > 0)
                {
                    combiner.Apply();
                    GameObject go = GameObjectHelper.CreateCombinedMeshGameObject(combiner, "CombinedModels", node.transform, dfUnity.Option_SetStaticFlags);

                    // Update climate
                    DaggerfallMesh dfMesh = go.GetComponent<DaggerfallMesh>();
                    dfMesh.SetClimate(climateBase, climateSeason, WindowStyle.Disabled);
                }
            }

            // Add static doors component
            DaggerfallStaticDoors c = this.gameObject.AddComponent<DaggerfallStaticDoors>();
            c.Doors = doors.ToArray();
        }

        private void AddFurnitureAction(DFBlock.RmbBlock3dObjectRecord obj, GameObject go, PlayerGPS.DiscoveredBuilding buildingData)
        {
            // Create unique LoadID for save system, using 9 lsb and the sign bit from each coord pos int
            ulong loadID = ((ulong) buildingData.buildingKey) << 30 |
                            (uint)(obj.XPos << 1 & posMask) << 20 |
                            (uint)(obj.YPos << 1 & posMask) << 10 |
                            (uint)(obj.ZPos << 1 & posMask);

            DFLocation.BuildingTypes buildingType = buildingData.buildingType;

            // Handle shelves:
            if (shopShelvesObjectGroupIndices.Contains(obj.ModelIdNum - containerObjectGroupOffset))
            {
                if (RMBLayout.IsShop(buildingType))
                {
                    // Shop shelves, so add a DaggerfallLoot component
                    DaggerfallLoot loot = go.AddComponent<DaggerfallLoot>();
                    if (loot)
                    {
                        // Set as shelves, assign load id and create serialization object
                        loot.ContainerType = LootContainerTypes.ShopShelves;
                        loot.ContainerImage = InventoryContainerImages.Shelves;
                        loot.LoadID = loadID;
                        if (SaveLoadManager.Instance != null)
                            go.AddComponent<SerializableLootContainer>();
                    }
                }
                else if (buildingType == DFLocation.BuildingTypes.Library ||
                         buildingType == DFLocation.BuildingTypes.GuildHall ||
                         buildingType == DFLocation.BuildingTypes.Temple)
                {
                    // Bookshelves, add DaggerfallBookshelf component
                    go.AddComponent<DaggerfallBookshelf>();
                }
                else if (DaggerfallBankManager.IsHouseOwned(buildingData.buildingKey))
                {   // Player owned house, everything is a house container
                    MakeHouseContainer(obj, go, loadID);
                }
            }
            // Handle generic furniture as (private) house containers:
            // (e.g. shelves, boxes, wardrobes, drawers etc)
            else if (obj.ModelIdNum / 100 == houseContainerObjectGroup ||
                     houseContainerObjectGroupIndices.Contains(obj.ModelIdNum - containerObjectGroupOffset))
            {
                MakeHouseContainer(obj, go, loadID);
            }
        }

        private static void MakeHouseContainer(DFBlock.RmbBlock3dObjectRecord obj, GameObject go, ulong loadID)
        {
            DaggerfallLoot loot = go.AddComponent<DaggerfallLoot>();
            if (loot)
            {
                // Set as house container (private furniture) and assign load id
                loot.ContainerType = LootContainerTypes.HouseContainers;
                loot.ContainerImage = InventoryContainerImages.Shelves;
                loot.LoadID = loadID;
                loot.TextureRecord = (int)obj.ModelIdNum % 100;
                if (SaveLoadManager.Instance != null)
                    go.AddComponent<SerializableLootContainer>();
            }
        }

        /// <summary>
        /// Add interior flats.
        /// </summary>
        private void AddFlats(PlayerGPS.DiscoveredBuilding buildingData)
        {
            GameObject node = new GameObject("Interior Flats");
            node.transform.parent = this.transform;

            // Add block flats
            markers.Clear();
            foreach (DFBlock.RmbBlockFlatObjectRecord obj in recordData.Interior.BlockFlatObjectRecords)
            {
                // Calculate position
                Vector3 billboardPosition = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;

                // Import custom 3d gameobject instead of flat
                if (MeshReplacement.ImportCustomFlatGameobject(obj.TextureArchive, obj.TextureRecord, billboardPosition, node.transform) != null)
                    continue;

                // Spawn billboard gameobject
                GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(obj.TextureArchive, obj.TextureRecord, node.transform);

                // Set position
                DaggerfallBillboard dfBillboard = go.GetComponent<DaggerfallBillboard>();
                go.transform.position = billboardPosition;
                go.transform.position += new Vector3(0, dfBillboard.Summary.Size.y / 2, 0);

                // Add editor markers to list
                if (obj.TextureArchive == TextureReader.EditorFlatsTextureArchive)
                {
                    InteriorEditorMarker marker = new InteriorEditorMarker();
                    marker.type = (InteriorMarkerTypes)obj.TextureRecord;
                    marker.gameObject = go;
                    markers.Add(marker);

                    // Add loot containers for treasure markers (always use pile of clothes icon)
                    if (marker.type == InteriorMarkerTypes.Treasure)
                    {
                        // Create unique LoadID for save system, using 9 lsb and the sign bit from each coord pos int
                        ulong loadID = ((ulong) buildingData.buildingKey) << 30 |
                                        (uint)(obj.XPos << 1 & posMask) << 20 |
                                        (uint)(obj.YPos << 1 & posMask) << 10 |
                                        (uint)(obj.ZPos << 1 & posMask);

                        DaggerfallLoot loot = GameObjectHelper.CreateLootContainer(
                            LootContainerTypes.RandomTreasure,
                            InventoryContainerImages.Chest,
                            billboardPosition,
                            node.transform,
                            DaggerfallLootDataTables.clothingArchive,
                            0, loadID);

                        if (!LootTables.GenerateLoot(loot, (int) GameManager.Instance.PlayerGPS.CurrentLocationType))
                            DaggerfallUnity.LogMessage(string.Format("DaggerfallInterior: Location type {0} is out of range or unknown.", GameManager.Instance.PlayerGPS.CurrentLocationType), true);
                    }
                }

                // Add point lights
                if (obj.TextureArchive == TextureReader.LightsTextureArchive)
                {
                    AddLight(obj, go.transform);
                }
            }
        }

        /// <summary>
        /// This data appears to be spawn/waypoint data for placing interior enemies.
        /// </summary>
        private void AddSpawnPoints()
        {
            foreach(DFBlock.RmbBlockSection3Record obj in recordData.Interior.BlockSection3Records)
            {
                Vector3 spawnPosition = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;
                spawnPoints.Add(spawnPosition);
            }
        }

        /// <summary>
        /// Adds interior point light.
        /// </summary>
        private static void AddLight(DFBlock.RmbBlockFlatObjectRecord obj, Transform parent = null)
        {
            if (DaggerfallUnity.Instance.Option_InteriorLightPrefab == null)
                return;

            // Create gameobject
            GameObject go = GameObjectHelper.InstantiatePrefab(DaggerfallUnity.Instance.Option_InteriorLightPrefab.gameObject, string.Empty, parent, Vector3.zero);

            // Set local position to billboard origin, otherwise light transform is at base of billboard
            go.transform.localPosition = Vector3.zero;

            // Adjust position of light for standing lights as their source comes more from top than middle
            Vector2 size = DaggerfallUnity.Instance.MeshReader.GetScaledBillboardSize(210, obj.TextureRecord) * MeshReader.GlobalScale;
            switch (obj.TextureRecord)
            {
                case 0:         // Bowl with fire
                    go.transform.localPosition += new Vector3(0, -0.1f, 0);
                    break;
                case 1:         // Campfire
                    // todo
                    break;
                case 2:         // Skull candle
                    go.transform.localPosition += new Vector3(0, 0.1f, 0);
                    break;
                case 3:         // Candle
                    go.transform.localPosition += new Vector3(0, 0.1f, 0);
                    break;
                case 4:         // Candle in bowl
                    // todo
                    break;
                case 5:         // Candleholder with 3 candles
                    go.transform.localPosition += new Vector3(0, 0.15f, 0);
                    break;
                case 6:         // Skull torch
                    go.transform.localPosition += new Vector3(0, 0.6f, 0);
                    break;
                case 7:         // Wooden chandelier with extinguished candles
                    // todo
                    break;
                case 8:         // Turkis lamp
                    // do nothing
                    break;
                case 9:        // Metallic chandelier with burning candles
                    go.transform.localPosition += new Vector3(0, 0.4f, 0);
                    break;
                case 10:         // Metallic chandelier with extinguished candles
                    // todo
                    break;
                case 11:        // Candle in lamp
                    go.transform.localPosition += new Vector3(0, -0.4f, 0);
                    break;
                case 12:         // Extinguished lamp
                    // todo
                    break;
                case 13:        // Round lamp (e.g. main lamp in mages guild)
                    go.transform.localPosition += new Vector3(0, -0.35f, 0);
                    break;
                case 14:        // Standing lantern
                    go.transform.localPosition += new Vector3(0, size.y / 2, 0);
                    break;
                case 15:        // Standing lantern round
                    go.transform.localPosition += new Vector3(0, size.y / 2, 0);
                    break;
                case 16:         // Mounted Torch with thin holder
                    // todo
                    break;
                case 17:        // Mounted torch 1
                    go.transform.localPosition += new Vector3(0, 0.2f, 0);
                    break;
                case 18:         // Mounted Torch 2
                    // todo
                    break;
                case 19:         // Pillar with firebowl
                    // todo
                    break;
                case 20:        // Brazier torch
                    go.transform.localPosition += new Vector3(0, 0.6f, 0);
                    break;
                case 21:        // Standing candle
                    go.transform.localPosition += new Vector3(0, size.y / 2.4f, 0);
                    break;
                case 22:         // Round lantern with medium chain
                    go.transform.localPosition += new Vector3(0, -0.5f, 0);
                    break;
                case 23:         // Wooden chandelier with burning candles
                    // todo
                    break;
                case 24:        // Lantern with long chain
                    go.transform.localPosition += new Vector3(0, -1.85f, 0);
                    break;
                case 25:        // Lantern with medium chain
                    go.transform.localPosition += new Vector3(0, -1.0f, 0);
                    break;
                case 26:        // Lantern with short chain
                    // todo
                    break;
                case 27:        // Lantern with no chain
                    go.transform.localPosition += new Vector3(0, -0.02f, 0);
                    break;
                case 28:        // Street Lantern 1
                    // todo
                    break;
                case 29:        // Street Lantern 2
                    // todo
                    break;
            }

            // adjust properties of light sources (e.g. Shrink light radius of candles)
            Light light = go.GetComponent<Light>();
            switch (obj.TextureRecord)
            {
                case 0:         // Bowl with fire
                    light.range = 20.0f;
                    light.intensity = 1.1f;
                    light.color = new Color(0.95f, 0.91f, 0.63f);
                    break;
                case 1:         // Campfire
                    // todo
                    break;
                case 2:         // Skull candle
                    light.range /= 3f;
                    light.intensity = 0.6f;
                    light.color = new Color(1.0f, 0.99f, 0.82f);
                    break;
                case 3:         // Candle
                    light.range /= 3f;
                    break;
                case 4:         // Candle with base
                    light.range /= 3f;
                    break;
                case 5:         // Candleholder with 3 candles
                    light.range = 7.5f;
                    light.intensity = 0.33f;
                    light.color = new Color(1.0f, 0.89f, 0.61f);
                    break;
                case 6:         // Skull torch
                    light.range = 15.0f;
                    light.intensity = 0.75f;
                    light.color = new Color(1.0f, 0.93f, 0.62f);
                    break;
                case 7:         // Wooden chandelier with extinguished candles
                    // todo
                    break;
                case 8:         // Turkis lamp
                    light.color = new Color(0.68f, 1.0f, 0.94f);
                    break;
                case 9:        // metallic chandelier with burning candles
                    light.range = 15.0f;
                    light.intensity = 0.65f;
                    light.color = new Color(1.0f, 0.92f, 0.6f);
                    break;
                case 10:         // Metallic chandelier with extinguished candles
                    // todo
                    break;
                case 11:        // Candle in lamp
                    light.range = 5.0f;
                    light.intensity = 0.5f;
                    break;
                case 12:         // Extinguished lamp
                    // todo
                    break;
                case 13:        // Round lamp (e.g. main lamp in mages guild)
                    light.range *= 1.2f;
                    light.intensity = 1.1f;
                    light.color = new Color(0.93f, 0.84f, 0.49f);
                    break;
                case 14:        // Standing lantern
                    // todo
                    break;
                case 15:        // Standing lantern round
                    // todo
                    break;
                case 16:         // Mounted Torch with thin holder
                    // todo
                    break;
                case 17:        // Mounted torch 1
                    light.intensity = 0.8f;
                    light.color = new Color(1.0f, 0.97f, 0.87f);
                    break;
                case 18:         // Mounted Torch 2
                    // todo
                    break;
                case 19:         // Pillar with firebowl
                    // todo
                    break;
                case 20:        // Brazier torch
                    light.range = 12.0f;
                    light.intensity = 0.75f;
                    light.color = new Color(1.0f, 0.92f, 0.72f);
                    break;
                case 21:        // Standing candle
                    light.range /= 3f;
                    light.intensity = 0.5f;
                    light.color = new Color(1.0f, 0.95f, 0.67f);
                    break;
                case 22:         // Round lantern with medium chain
                    light.intensity = 1.5f;
                    light.color = new Color(1.0f, 0.95f, 0.78f);
                    break;
                case 23:         // Wooden chandelier with burning candles
                    // todo
                    break;
                case 24:        // Lantern with long chain
                    light.intensity = 1.4f;
                    light.color = new Color(1.0f, 0.98f, 0.64f);
                    break;
                case 25:        // Lantern with medium chain
                    light.intensity = 1.4f;
                    light.color = new Color(1.0f, 0.98f, 0.64f);
                    break;
                case 26:        // Lantern with short chain
                    light.intensity = 1.4f;
                    light.color = new Color(1.0f, 0.98f, 0.64f);
                    break;
                case 27:        // Lantern with no chain
                    light.intensity = 1.4f;
                    light.color = new Color(1.0f, 0.98f, 0.64f);
                    break;
                case 28:        // Street Lantern 1
                    // todo
                    break;
                case 29:        // Street Lantern 2
                    // todo
                    break;
            }

            // TODO: Could also adjust light colour and intensity, or change prefab entirely above for any obj.TextureRecord
        }

        /// <summary>
        /// Add interior people flats.
        /// </summary>
        private void AddPeople(PlayerGPS.DiscoveredBuilding buildingData)
        {
            GameObject node = new GameObject("People Flats");
            node.transform.parent = this.transform;

            // Add block flats
            foreach (DFBlock.RmbBlockPeopleRecord obj in recordData.Interior.BlockPeopleRecords)
            {
                // Calculate position
                Vector3 billboardPosition = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;

                // Import 3D character instead of billboard
                if (MeshReplacement.ImportCustomFlatGameobject(obj.TextureArchive, obj.TextureRecord, billboardPosition, node.transform) != null)
                    continue;

                // Spawn billboard gameobject
                GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(obj.TextureArchive, obj.TextureRecord, node.transform);

                // Set position
                DaggerfallBillboard dfBillboard = go.GetComponent<DaggerfallBillboard>();
                go.transform.position = billboardPosition;
                go.transform.position += new Vector3(0, dfBillboard.Summary.Size.y / 2, 0);

                // Add RMB data to billboard
                dfBillboard.SetRMBPeopleData(obj);

                // Add StaticNPC behaviour
                StaticNPC npc = go.AddComponent<StaticNPC>();
                npc.SetLayoutData(obj, entryDoor.buildingKey);

                // Disable people if shop or building is closed
                DFLocation.BuildingTypes buildingType = buildingData.buildingType;
                if ((RMBLayout.IsShop(buildingType) && !GameManager.Instance.PlayerEnterExit.IsPlayerInsideOpenShop) ||
                    (buildingType <= DFLocation.BuildingTypes.Palace && !RMBLayout.IsShop(buildingType) && !PlayerActivate.IsBuildingOpen(buildingType)))
                {
                    go.SetActive(false);
                }
                // Disable people if player owns this house
                else if (DaggerfallBankManager.IsHouseOwned(buildingData.buildingKey))
                    go.SetActive(false);
            }
        }

        /// <summary>
        /// Add action doors to parent transform.
        /// </summary>
        private void AddActionDoors()
        {
            GameObject actionDoorsNode = new GameObject("Action Doors");
            actionDoorsNode.transform.parent = this.transform;

            foreach (DFBlock.RmbBlockDoorRecord obj in recordData.Interior.BlockDoorRecords)
            {
                // Create unique LoadID for save sytem
                ulong loadID = (ulong)(blockData.Position + obj.Position);

                // Get model transform
                Vector3 modelRotation = new Vector3(0, -obj.YRotation / BlocksFile.RotationDivisor, 0);
                Vector3 modelPosition = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;

                // Instantiate door prefab and add model
                GameObject go = GameObjectHelper.InstantiatePrefab(dfUnity.Option_InteriorDoorPrefab.gameObject, string.Empty, actionDoorsNode.transform, Vector3.zero);
                GameObjectHelper.CreateDaggerfallMeshGameObject(doorModelId, actionDoorsNode.transform, false, go, true);

                // Resize box collider to new mesh bounds
                BoxCollider boxCollider = go.GetComponent<BoxCollider>();
                MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
                if (boxCollider != null && meshRenderer != null)
                {
                    boxCollider.center = meshRenderer.bounds.center;
                    boxCollider.size = meshRenderer.bounds.size;
                }

                // Apply transforms
                go.transform.rotation = Quaternion.Euler(modelRotation);
                go.transform.position = modelPosition;

                // Get action door script
                DaggerfallActionDoor actionDoor = go.GetComponent<DaggerfallActionDoor>();

                // Assign loadID
                if (actionDoor)
                    actionDoor.LoadID = loadID;

                if (SaveLoadManager.Instance != null)
                    go.AddComponent<SerializableActionDoor>();
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Helper to get a random spawn point from interior list (if present).
        /// </summary>
        /// <param name="localPositionOut">Local position of spawn point.</param>
        /// <returns>True if spawn point found.</returns>
        public bool GetRandomSpawnPoint(out Vector3 localPositionOut)
        {
            // Handle no spawn points
            if (spawnPoints.Count == 0)
            {
                // Inform caller to use a fallback
                localPositionOut = Vector3.zero;
                return false;
            }

            // Return a random spawn point
            localPositionOut = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];
            return true;
        }

        #endregion
    }
}