// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    LypyL, Hazelnut
// 
// Notes:
//

#define SHOW_LAYOUT_TIMES
//#define SHOW_LAYOUT_TIMES_NATURE

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Serialization;
using Unity.Jobs;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Manages terrains for streaming world at runtime.
    /// At runtime only coordinates from LocalPlayerGPS will be used. Be sure to apply desired preview to PlayerGPS.
    /// Terrain tiles are spread outwards from a centre tile up to TerrainDistance around player.
    /// Terrains will be marked inactive once they pass beyond TerrainDistance.
    /// Inactive terrains greater than TerrainDistance+1 from player will be recycled.
    /// Locations and other loose objects greater than TerrainDistance+1 from player will be destroyed.
    /// </summary>
    public class StreamingWorld : MonoBehaviour
    {
        #region Fields

        const HideFlags defaultHideFlags = HideFlags.None;

        const int maxTerrainArray = 256;        // Maximum terrains in memory at any time

        // Local player GPS for tracking player virtual position
        public PlayerGPS LocalPlayerGPS;

        // Number of terrains tiles to load around central terrain tile
        // Each terrain is equivalent to one full city area of 8x8 RMB blocks
        // 1 : ( 2 * 1 + 1 ) * ( 2 * 1 + 1 ) = 9 tiles
        // 2 : ( 2 * 2 + 1 ) * ( 2 * 2 + 1 ) = 25 tiles
        // 3 : ( 2 * 3 + 1 ) * ( 2 * 3 + 1 ) = 49 tiles
        // 4 : ( 2 * 4 + 1 ) * ( 2 * 4 + 1 ) = 81 tiles
        [Range(1, 4)]
        public int TerrainDistance = 3;

        // This controls central map pixel for streaming world
        // Synced to PlayerGPS at runtime
        [Range(TerrainHelper.minMapPixelX, TerrainHelper.maxMapPixelX)]
        public int MapPixelX = TerrainHelper.defaultMapPixelX;
        [Range(TerrainHelper.minMapPixelY, TerrainHelper.maxMapPixelY)]
        public int MapPixelY = TerrainHelper.defaultMapPixelY;

        // Increasing scale will amplify height of small details
        // This scale is replicated to every managed terrain
        // Not recommended to use greater than default value
        [Range(TerrainHelper.minTerrainScale, TerrainHelper.maxTerrainScale)]
        public float TerrainScale = TerrainHelper.defaultTerrainScale;

        [HideInInspector]
        public string EditorFindLocationString = "Daggerfall/Privateer's Hold";

        //public bool AddLocationBeacon = false;
        public GameObject streamingTarget = null;
        public bool suppressWorld = false;
        public bool ShowDebugString = false;

        // List of terrain objects
        // Terrains all have the same format and will be endlessly recycled
        TerrainDesc[] terrainArray = new TerrainDesc[maxTerrainArray];
        Dictionary<int, int> terrainIndexDict = new Dictionary<int, int>();

        // List of loose objects, such as locations or loot containers
        // These objects are not recycled and will created/destroyed as needed
        List<LooseObjectDesc> looseObjectsList = new List<LooseObjectDesc>();

        // Compensation for floating origin
        Vector3 worldCompensation = Vector3.zero;

        Vector3 playerStartPos;
        Vector3 lastPlayerPos;

        DaggerfallUnity dfUnity;
        DFPosition mapOrigin;
        double worldX, worldZ;
        bool isReady = false;

        Vector3 autoRepositionOffset = Vector3.zero;
        RepositionMethods autoRepositionMethod = RepositionMethods.RandomStartMarker;

        bool init;
        bool terrainUpdateRunning;
        bool updateLocatations;

        DaggerfallLocation currentPlayerLocationObject;
        int playerTilemapIndex = -1;
        DFPosition prevMapPixel;

        private int? travelStartX = null;
        private int? travelStartZ = null;

        #endregion

        #region Properties

        /// <summary>
        /// True if component is ready.
        /// </summary>
        public bool IsReady { get { return ReadyCheck(); } }

        /// <summary>
        /// True if world is currently initialising.
        /// </summary>
        public bool IsInit { get { return init; } }

        /// <summary>
        /// Gets the scene name for the current map pixel.
        /// </summary>
        public string SceneName { get { return GetSceneName(MapPixelX, MapPixelY); } }

        /// <summary>Gets the scene name for the given map pixel.</summary>
        public static string GetSceneName(int MapPixelX, int MapPixelY)
        {
            return string.Format("DaggerfallWorld [mapX={0}, mapY={1}]", MapPixelX, MapPixelY);
        }

        /// <summary>
        /// Gets Transform of central terrain player is standing on.
        /// </summary>
        public Transform PlayerTerrainTransform { get { return GetPlayerTerrainTransform(); } }

        /// <summary>
        /// Offset position of world for floating origin logic.
        /// </summary>
        public Vector3 WorldCompensation { get { return worldCompensation; } }

        /// <summary>
        /// This value is the amount of scene player movement equivalent to 1 native world map unit.
        /// </summary>
        public static float SceneMapRatio { get { return 1f / MeshReader.GlobalScale; } }

        // Streaming world objects are ultimately parented to this object
        // If left null, world objects will be parented to this transform
        // This allows for decoupling of GPS logic and streaming functions
        public Transform StreamingTarget
        {
            get { return (streamingTarget != null) ? streamingTarget.transform : this.transform; }
        }

        /// <summary>
        /// Gets current DaggerfallLocation (if any) for player's current map pixel.
        /// Will return null for any map pixel in wilderness or if world still being built on init.
        /// </summary>
        public DaggerfallLocation CurrentPlayerLocationObject
        {
            get { return currentPlayerLocationObject; }
        }

        /// <summary>
        /// Gets index of tile under player at their current world position.
        /// There are many different tiles a player can stand on, including corner tiles for roads, etc.
        /// The main tile types in nature are:
        /// -1 = Nothing/Error
        ///  0 = Water
        ///  1 = Dirt (snow in winter set)
        ///  2 = Grass (snow in winter set)
        ///  3 = Stone (snow in winter set)
        /// </summary>
        public int PlayerTileMapIndex
        {
            get { return playerTilemapIndex; }
        }

        public bool IsRepositioningPlayer { get; private set; }

        #endregion

        #region Structs/Enums

        public struct TerrainDesc
        {
            public bool active;
            public bool updateData;
            public bool updateNature;
            public bool updateLocation;
            public bool hasLocation;
            public GameObject terrainObject;
            public GameObject billboardBatchObject;
            public int mapPixelX;
            public int mapPixelY;
        }

        struct LooseObjectDesc
        {
            public GameObject gameObject;
            public int mapPixelX;
            public int mapPixelY;
            public bool statefulObj;
        }

        /// <summary>
        /// Methods for auto-reposition logic.
        /// </summary>
        public enum RepositionMethods
        {
            None,
            Origin,
            Offset,
            DungeonEntrance,
            RandomStartMarker,
            DirectionFromStartMarker,
        }

        #endregion

        #region Unity

        private void Awake()
        {
            SaveLoadManager.OnStartLoad += SaveLoadManager_OnStartLoad;
            StartGameBehaviour.OnNewGame += StartGameBehaviour_OnNewGame;
        }

        void Update()
        {
            // Cannot proceed until ready and player is set
            if (!ReadyCheck())
                return;

            // Handle moving to new map pixel or first-time init
            DFPosition curMapPixel = LocalPlayerGPS.CurrentMapPixel;
            if (curMapPixel.X != MapPixelX ||
                curMapPixel.Y != MapPixelY ||
                init)
            {
                Debug.Log(string.Format("Entering new map pixel X={0}, Y={1}", curMapPixel.X, curMapPixel.Y));
                prevMapPixel = new DFPosition(MapPixelX, MapPixelY);
                MapPixelX = curMapPixel.X;
                MapPixelY = curMapPixel.Y;
                UpdateWorld();
                InitPlayerTerrain();
                StartCoroutine(UpdateTerrains());
                updateLocatations = true;
                init = false;
            }

            // Exit if terrain data still updating
            // Raise location update flag any time terrain updates
            if (terrainUpdateRunning)
            {
                updateLocatations = true;
                return;
            }

            // Update locations when terrain data finished
            // This flag is raised during terrain update
            if (updateLocatations)
            {
                UpdateLocations();
                updateLocatations = false;
            }

            // Reposition player
            if (autoRepositionMethod != RepositionMethods.None)
            {
                switch (autoRepositionMethod)
                {
                    case RepositionMethods.DirectionFromStartMarker:
                    case RepositionMethods.RandomStartMarker:
                        PositionPlayerToLocation();
                        break;
                    case RepositionMethods.Offset:
                        RepositionPlayer(MapPixelX, MapPixelY, autoRepositionOffset);
                        break;
                    case RepositionMethods.DungeonEntrance:
                        PositionPlayerToDungeonExit();
                        break;
                    default:
                    case RepositionMethods.Origin:
                        RepositionPlayer(MapPixelX, MapPixelY, Vector3.zero);
                        break;
                }
                autoRepositionMethod = RepositionMethods.None;
            }

            // Do not update world position if player is inside dungeon
            // This can cause player to become desynced from world as dungeon can
            // actually extend beyond the current map pixel area
            if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon)
                return;

            // Get distance player has moved in world map units and apply to world position
            Vector3 playerPos = LocalPlayerGPS.transform.position;
            worldX += (playerPos.x - lastPlayerPos.x) * SceneMapRatio;
            worldZ += (playerPos.z - lastPlayerPos.z) * SceneMapRatio;

            // Sync PlayerGPS to new world position
            LocalPlayerGPS.WorldX = (int)worldX;
            LocalPlayerGPS.WorldZ = (int)worldZ;

            // Update last player position
            lastPlayerPos = playerPos;

            // Update index of terrain tile beneath player in world
            UpdatePlayerTerrainTileIndex();
        }

        void UpdatePlayerTerrainTileIndex()
        {
            playerTilemapIndex = -1;

            // Player must be above a known terrain object
            DaggerfallTerrain playerTerrain = GetPlayerTerrain();
            if (!playerTerrain)
                return;

            // The terrain must have a valid tilemap array
            if (playerTerrain.TileMap == null || playerTerrain.TileMap.Length == 0)
                return;

            // Get player relative position from terrain origin
            Vector3 relativePos = lastPlayerPos - playerTerrain.transform.position;

            // Convert X, Z position into 0-1 domain
            float dim = MapsFile.WorldMapTerrainDim * MeshReader.GlobalScale;
            float u = relativePos.x / dim;
            float v = relativePos.z / dim;

            // Get clamped offset into tilemap array
            int x = Mathf.Clamp((int)(MapsFile.WorldMapTileDim * u), 0, MapsFile.WorldMapTileDim - 1);
            int y = Mathf.Clamp((int)(MapsFile.WorldMapTileDim * v), 0, MapsFile.WorldMapTileDim - 1);

            // Update index - divide by 4 to find actual tile base as each tile has 4x variants (flip, rotate, etc.)
            playerTilemapIndex = playerTerrain.TileMap[y * MapsFile.WorldMapTileDim + x].r / 4;

            //Debug.LogFormat("X={0}, Z={1}, Index={2}", x, y, playerTilemapIndex);
        }

        void OnGUI()
        {
            if (Event.current.type.Equals(EventType.Repaint) && ShowDebugString)
            {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.black;
                string text = GetDebugString();
                GUI.Label(new Rect(10, 30, 800, 24), text, style);
                GUI.Label(new Rect(8, 28, 800, 24), text);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Teleport to specific map pixel with an optional automatic reposition.
        /// </summary>
        public void TeleportToCoordinates(int mapPixelX, int mapPixelY, RepositionMethods autoReposition = RepositionMethods.Origin)
        {
            TeleportToMapPixel(mapPixelX, mapPixelY, Vector3.zero, autoReposition);
        }

        /// <summary>
        /// Teleport to specific map pixel and apply a specific reposition.
        /// </summary>
        public void TeleportToCoordinates(int mapPixelX, int mapPixelY, Vector3 repositionOffset)
        {
            TeleportToMapPixel(mapPixelX, mapPixelY, repositionOffset, RepositionMethods.None);
        }

        /// <summary>
        /// Teleport to specific world coordinates.
        /// </summary>
        public void TeleportToWorldCoordinates(int worldPosX, int worldPosZ)
        {
            // Convert world coordinates to map pixel and find origin world position of tile
            DFPosition mapPixel = MapsFile.WorldCoordToMapPixel(worldPosX, worldPosZ);
            DFPosition originWorldPos = MapsFile.MapPixelToWorldCoord(mapPixel.X, mapPixel.Y);

            // Find reposition offset based on difference between tile origin and desired absolute position
            Vector3 offset = new Vector3(
                (worldPosX - originWorldPos.X) / SceneMapRatio,
                0,
                (worldPosZ - originWorldPos.Y) / SceneMapRatio);

            // Teleport to coordinates with reposition
            TeleportToMapPixel(mapPixel.X, mapPixel.Y, offset, RepositionMethods.Offset);
        }

        // Offset world compensation for floating origin world
        /// <summary>
        /// Offset world compensation for floating origin.
        /// </summary>
        /// <param name="change">Amount to offset world compensation.</param>
        /// <param name="offsetLastPlayerPos">Optionally update last known player position so GPS does not change.</param>
        public void OffsetWorldCompensation(Vector3 offset, bool offsetLastPlayerPos = true)
        {
            worldCompensation += offset;
            if (offsetLastPlayerPos)
                lastPlayerPos += offset;
            RaiseOnFloatingOriginChangeEvent();
        }

        /// <summary>
        /// Returns terrain GameObject from mapPixel, or null if
        /// no terrain objects are mapped to that pixel
        /// </summary>
        public GameObject GetTerrainFromPixel(DFPosition mapPixel)
        {
            return GetTerrainFromPixel(mapPixel.X, mapPixel.Y);
        }

        /// <summary>
        /// Returns terrain GameObject from mapPixel, or null if
        /// no terrain objects are mapped to that pixel
        /// </summary>
        public GameObject GetTerrainFromPixel(int mapPixelX, int mapPixelY)//##Lypyl
        {
            //Get Key for terrain lookup
            int key = TerrainHelper.MakeTerrainKey(mapPixelX, mapPixelY);

            if (terrainIndexDict.ContainsKey(key))
                return terrainArray[terrainIndexDict[key]].terrainObject;
            else
                return null;
        }

        public void SetAutoReposition(RepositionMethods method, Vector3 offset)
        {
            autoRepositionOffset = offset;
            autoRepositionMethod = method;
        }

        /// <summary>
        /// Track an exterior object in streaming world.
        /// Tracked object will be automatically destroyed when outside of range.
        /// </summary>
        /// <param name="gameObject">Game object to track.</param>
        /// <param name="statefulObj">True to mark as a stateful game object which is managed by SerializedStateManager.</param>
        /// <param name="mapPixelX">Map pixel X to store gameobject. -1 for player location.</param>
        /// <param name="mapPixelY">Map pixel Y to store gameobject. -1 for player location.</param>
        /// <param name="setParent">True to set parent as StreamingTarget.</param>
        public void TrackLooseObject(GameObject gameObject, bool statefulObj = false, int mapPixelX = -1, int mapPixelY = -1, bool setParent = false)
        {
            // Create loose object description
            LooseObjectDesc desc = new LooseObjectDesc();
            desc.gameObject = gameObject;
            if (mapPixelX == -1)
                desc.mapPixelX = MapPixelX;
            if (mapPixelY == -1)
                desc.mapPixelY = MapPixelY;
            desc.statefulObj = statefulObj;
            looseObjectsList.Add(desc);

            // Change object parent
            if (setParent)
                gameObject.transform.parent = StreamingTarget.transform;
        }

        public void ClearStatefulLooseObjects()
        {
            looseObjectsList.RemoveAll(x => {
                if (x.statefulObj)
                    Destroy(x.gameObject);
                return x.statefulObj;
            });
        }

        /// <summary>
        /// Gets the building directory for current player position.
        /// </summary>
        /// <returns>BuildingDirectory or null if player not inside a location map pixel.</returns>
        public BuildingDirectory GetCurrentBuildingDirectory()
        {
            if (!currentPlayerLocationObject)
                return null;

            // buildingDirectory MapID must match PlayerGPS.CurrentLocation MapID or building data might be out of sync with location until world finishes loading
            BuildingDirectory buildingDirectory = currentPlayerLocationObject.GetComponent<BuildingDirectory>();
            if (buildingDirectory && buildingDirectory.MapID != GameManager.Instance.PlayerGPS.CurrentLocation.MapTableData.MapId)
            {
                Debug.LogWarningFormat(
                    "GetCurrentBuildingDirectory() MapID={0} does not match PlayerGPS.CurrentLocation MapID={1}. StreamingWorld might still be loading locations.",
                    currentPlayerLocationObject.Summary.MapID,
                    GameManager.Instance.PlayerGPS.CurrentLocation.MapTableData.MapId);
                return null;
            }

            return buildingDirectory;
        }

        /// <summary>
        /// Gets the city navigation component for current player position.
        /// </summary>
        /// <returns>CityNavigation or null if player not inside a location map pixel.</returns>
        public CityNavigation GetCurrentCityNavigation()
        {
            if (!currentPlayerLocationObject)
                return null;

            return currentPlayerLocationObject.GetComponent<CityNavigation>();
        }

        /// <summary>
        /// Gets terrain transform at mapPixelX, mapPixelY.
        /// </summary>
        /// <returns>DaggerfallTerrain Transform if found, or null if not currently in world.</returns>
        public Transform GetTerrainTransform(int mapPixelX, int mapPixelY)
        {
            int key = TerrainHelper.MakeTerrainKey(mapPixelX, mapPixelY);
            if (!terrainIndexDict.ContainsKey(key))
                return null;

            return terrainArray[terrainIndexDict[key]].terrainObject.transform;
        }

        /// <summary>
        /// Gets the DaggerfallLocation component for current player position.
        /// </summary>
        /// <returns>DaggerfallLocation component if player inside location map pixel, otherwise null.</returns>
        private DaggerfallLocation GetPlayerLocationObject()
        {
            // Look for location at current map pixel coords inside loose object list
            for (int i = 0; i < looseObjectsList.Count; i++)
            {
                LooseObjectDesc desc = looseObjectsList[i];
                if (!desc.statefulObj && desc.gameObject && desc.mapPixelX == MapPixelX && desc.mapPixelY == MapPixelY)
                {
                    DaggerfallLocation location = desc.gameObject.GetComponent<DaggerfallLocation>();
                    if (location)
                        return location;
                }
            }

            return null;
        }

        #endregion

        #region World Setup Methods

        // Init world at startup or when player teleports
        private void InitWorld()
        {
            // Cannot init world without a player, as world positions around player
            // Also do nothing if world is on hold at start
            if (LocalPlayerGPS == null || suppressWorld)
                return;

            // Player must be at origin on init for proper world sync
            // Starting position will be assigned when terrain ready based on respositionMethod
            LocalPlayerGPS.transform.position = Vector3.zero;

            // Raise OnPreInitWorld event
            RaiseOnPreInitWorldEvent();

            // Init streaming world
            ClearStreamingWorld();
            worldCompensation = Vector3.zero;
            mapOrigin = LocalPlayerGPS.CurrentMapPixel;
            MapPixelX = mapOrigin.X;
            MapPixelY = mapOrigin.Y;
            playerStartPos = new Vector3(LocalPlayerGPS.transform.position.x, 0, LocalPlayerGPS.transform.position.z);
            lastPlayerPos = playerStartPos;

            // Set player world position to match PlayerGPS
            // StreamingWorld will then sync player to world
            worldX = LocalPlayerGPS.WorldX;
            worldZ = LocalPlayerGPS.WorldZ;

            init = true;
            RaiseOnInitWorldEvent();
        }

        // Place terrain tiles when player changes map pixels
        private void UpdateWorld()
        {
            int dim = TerrainDistance * 2 + 1;
            int xs = MapPixelX - TerrainDistance;
            int ys = MapPixelY - TerrainDistance;
            for (int y = ys; y < ys + dim; y++)
            {
                for (int x = xs; x < xs + dim; x++)
                {
                    PlaceTerrain(x, y);
                }
            }
        }

        // Fully init central terrain so player can be dropped into world as soon as possible
        private void InitPlayerTerrain()
        {
            if (!init)
                return;

#if SHOW_LAYOUT_TIMES
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
#endif

            CollectLooseObjects();
            int playerTerrainIndex = terrainIndexDict[TerrainHelper.MakeTerrainKey(MapPixelX, MapPixelY)];

            UpdateTerrainData(terrainArray[playerTerrainIndex]);
            terrainArray[playerTerrainIndex].updateData = false;

            UpdateTerrainNature(terrainArray[playerTerrainIndex]);
            terrainArray[playerTerrainIndex].updateNature = false;

#if SHOW_LAYOUT_TIMES
            stopwatch.Stop();
            DaggerfallUnity.LogMessage(string.Format("Time to init player terrain: {0}ms", stopwatch.ElapsedMilliseconds), true);
#endif

            StartCoroutine(UpdateLocation(playerTerrainIndex, false));
            terrainArray[playerTerrainIndex].updateLocation = false;
        }

        private IEnumerator UpdateTerrains()
        {
            RaiseOnUpdateTerrainsStartEvent();
            terrainUpdateRunning = true;

#if SHOW_LAYOUT_TIMES
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
#endif

            // Update terrain heights and nature billboards
            CollectTerrains();
            for (int i = 0; i < terrainArray.Length; i++)
            {
                if (terrainArray[i].active)
                {
                    if (terrainArray[i].updateData)
                    {
                        if (init)
                            UpdateTerrainData(terrainArray[i]);
                        else
                            yield return StartCoroutine(UpdateTerrainDataCoroutine(terrainArray[i]));
                        terrainArray[i].updateData = false;
                    }
                    if (terrainArray[i].updateNature)
                    {
                        UpdateTerrainNature(terrainArray[i]);
                        terrainArray[i].updateNature = false;
                        if (!init)
                            yield return new WaitForEndOfFrame();
                    }
                }
            }

            // If this is an init we can use the load time to unload unused assets
            // Keeps memory usage much lower over time
            if (init)
            {
                DaggerfallUnity.Instance.PruneCache();
                DaggerfallGC.ThrottledUnloadUnusedAssets();
            }

            // Set terrain neighbours
            UpdateNeighbours();

#if SHOW_LAYOUT_TIMES
            stopwatch.Stop();
            DaggerfallUnity.LogMessage(string.Format("Time to update terrains: {0}ms", stopwatch.ElapsedMilliseconds), true);
#endif

            terrainUpdateRunning = false;
            RaiseOnUpdateTerrainsEndEvent();
        }

        private void UpdateLocations()
        {
            CollectLooseObjects();

            for (int i = 0; i < terrainArray.Length; i++)
            {
                if (terrainArray[i].active && terrainArray[i].hasLocation)
                    StartCoroutine(UpdateLocation(i, true));
            }

            // Update current player location object once location update complete
            currentPlayerLocationObject = GetPlayerLocationObject();

            // Raise event
            RaiseOnAvailableLocationGameObjectEvent();
        }

        private IEnumerator UpdateLocation(int index, bool allowYield)
        {
            if (terrainArray[index].active && terrainArray[index].hasLocation && terrainArray[index].updateLocation)
            {
#if SHOW_LAYOUT_TIMES
                System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
#endif
                // Disable update flag
                terrainArray[index].updateLocation = false;

                // Create location object
                DFLocation location;
                GameObject locationObject = CreateLocationGameObject(index, out location);
                if (locationObject)
                {
                    // Add building directory to location game object
                    BuildingDirectory buildingDirectory = locationObject.AddComponent<BuildingDirectory>();
                    buildingDirectory.SetLocation(location);

                    // Add location to loose object list
                    LooseObjectDesc looseObject = new LooseObjectDesc();
                    looseObject.gameObject = locationObject;
                    looseObject.mapPixelX = terrainArray[index].mapPixelX;
                    looseObject.mapPixelY = terrainArray[index].mapPixelY;
                    looseObject.statefulObj = false;
                    looseObjectsList.Add(looseObject);

                    // Create billboard batch game objects for this location
                    // Streaming world always batches for performance, regardless of options
                    int natureArchive = ClimateSwaps.GetNatureArchive(LocalPlayerGPS.ClimateSettings.NatureSet, dfUnity.WorldTime.Now.SeasonValue);
                    //TextureAtlasBuilder miscBillboardAtlas = dfUnity.MaterialReader.MiscBillboardAtlas;
                    DaggerfallBillboardBatch natureBillboardBatch = GameObjectHelper.CreateBillboardBatchGameObject(natureArchive, locationObject.transform);
                    DaggerfallBillboardBatch lightsBillboardBatch = GameObjectHelper.CreateBillboardBatchGameObject(TextureReader.LightsTextureArchive, locationObject.transform);
                    DaggerfallBillboardBatch animalsBillboardBatch = GameObjectHelper.CreateBillboardBatchGameObject(TextureReader.AnimalsTextureArchive, locationObject.transform);
                    //DaggerfallBillboardBatch miscBillboardBatch = GameObjectHelper.CreateBillboardBatchGameObject(miscBillboardAtlas.AtlasMaterial, locationObject.transform);

                    // Set hide flags
                    natureBillboardBatch.hideFlags = defaultHideFlags;
                    lightsBillboardBatch.hideFlags = defaultHideFlags;
                    animalsBillboardBatch.hideFlags = defaultHideFlags;
                    //miscBillboardBatch.hideFlags = defaultHideFlags;

                    // Position RMB blocks inside terrain area
                    int width = location.Exterior.ExteriorData.Width;
                    int height = location.Exterior.ExteriorData.Height;
                    DFPosition tilePos = TerrainHelper.GetLocationTerrainTileOrigin(location);
                    Vector3 origin = new Vector3(tilePos.X * RMBLayout.RMBTileSide, 2.0f * MeshReader.GlobalScale, tilePos.Y * RMBLayout.RMBTileSide);

                    // Get location component
                    DaggerfallLocation dfLocation = locationObject.GetComponent<DaggerfallLocation>();

                    // Create CityNavigation component
                    CityNavigation cityNavigation = dfLocation.gameObject.AddComponent<CityNavigation>();
                    cityNavigation.FormatNavigation(location.RegionName, location.Name, width, height);

                    // Create PopulationManager component for locations with wandering NPCs
                    if (location.MapTableData.LocationType == DFRegion.LocationTypes.TownCity ||
                        location.MapTableData.LocationType == DFRegion.LocationTypes.TownHamlet ||
                        location.MapTableData.LocationType == DFRegion.LocationTypes.TownVillage ||
                        location.MapTableData.LocationType == DFRegion.LocationTypes.HomeFarms ||
                        location.MapTableData.LocationType == DFRegion.LocationTypes.HomeWealthy ||
                        location.MapTableData.LocationType == DFRegion.LocationTypes.ReligionTemple ||
                        location.MapTableData.LocationType == DFRegion.LocationTypes.Tavern)
                    {
                        dfLocation.gameObject.AddComponent<PopulationManager>();
                    }

                    // Perform layout and yield after each block is placed
                    ContentReader contentReader = DaggerfallUnity.Instance.ContentReader;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            // Set block origin for billboard batches
                            // This causes next additions to be offset by this position
                            Vector3 blockOrigin = origin + new Vector3((x * RMBLayout.RMBSide), 0, (y * RMBLayout.RMBSide));
                            natureBillboardBatch.BlockOrigin = blockOrigin;
                            lightsBillboardBatch.BlockOrigin = blockOrigin;
                            animalsBillboardBatch.BlockOrigin = blockOrigin;
                            //miscBillboardBatch.BlockOrigin = blockOrigin;

                            // Get block name and data
                            DFBlock blockData;
                            string blockName = contentReader.BlockFileReader.CheckName(contentReader.MapFileReader.GetRmbBlockName(ref location, x, y));
                            RMBLayout.GetBlockData(blockName, out blockData);

                            // Add block
                            GameObject go = GameObjectHelper.CreateRMBBlockGameObject(
                                blockData,
                                x,
                                y,
                                false,
                                dfUnity.Option_CityBlockPrefab,
                                natureBillboardBatch,
                                lightsBillboardBatch,
                                animalsBillboardBatch,
                                null,
                                null,
                                dfLocation.Summary.Nature,
                                dfUnity.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter ? ClimateSeason.Winter : ClimateSeason.Summer);

                            // Set game object properties
                            go.hideFlags = defaultHideFlags;
                            go.transform.parent = locationObject.transform;
                            go.transform.localPosition = blockOrigin;
                            if (allowYield)
                                dfLocation.ApplyClimateSettings();

                            // Set navigation info for this block
                            cityNavigation.SetRMBData(ref blockData, x, y);

                            // Optionally yield after placing block
                            if (allowYield)
                                yield return new WaitForEndOfFrame();
                        }
                    }
                    // Only apply climate settings after entire location updated if not spanning multiple frames.
                    if (!allowYield)
                        dfLocation.ApplyClimateSettings();

                    // Apply billboard batches
                    natureBillboardBatch.Apply();
                    lightsBillboardBatch.Apply();
                    animalsBillboardBatch.Apply();
                    //miscBillboardBatch.Apply();

                    RaiseOnUpdateLocationGameObjectEvent(locationObject, allowYield);

                    //// TEST: Store a RAW image of navgrid
                    //string filename = string.Format("{0} [{1}x{2}].raw", location.Name, cityNavigation.CityWidth * 64, cityNavigation.CityHeight * 64);
                    //cityNavigation.SaveTestRawImage(Path.Combine(@"d:\test\navgrids\", filename));
                }

#if SHOW_LAYOUT_TIMES
                stopwatch.Stop();
                DaggerfallUnity.LogMessage(string.Format("Time to update location {1}: {0}ms", stopwatch.ElapsedMilliseconds, index), true);
#endif
            }
        }


        // Place a single terrain and mark it for update
        private void PlaceTerrain(int mapPixelX, int mapPixelY)
        {
            // Do nothing if out of range
            if (mapPixelX < MapsFile.MinMapPixelX || mapPixelX >= MapsFile.MaxMapPixelX ||
                mapPixelY < MapsFile.MinMapPixelY || mapPixelY >= MapsFile.MaxMapPixelY)
            {
                return;
            }

            // Get terrain key
            int key = TerrainHelper.MakeTerrainKey(mapPixelX, mapPixelY);

            // If terrain is available
            if (terrainIndexDict.ContainsKey(key))
            {
                // Terrain exists, check if active
                int index = terrainIndexDict[key];
                if (terrainArray[index].active)
                {
                    // Terrain already active in scene, nothing to do
                }
                else
                {
                    // Terrain inactive but available, re-activate terrain
                    terrainArray[index].active = true;
                    terrainArray[index].terrainObject.SetActive(true);
                    terrainArray[index].billboardBatchObject.SetActive(true);
                    // also set flag for updating locations if this terrain contains a location - so building data gets created again
                    // this fixes the missing buildings bug - see this forum-thread: https://forums.dfworkshop.net/viewtopic.php?f=4&t=3391&start=10)
                    terrainArray[index].updateLocation = terrainArray[index].hasLocation;
                }
                // If any nature model replacements are used then do extra nature updates for any terrains moving into or out of distance 1 or less.
                if (dfUnity.TerrainNature.NatureMeshUsed)
                {
                    int prevDist = GetTerrainDist(prevMapPixel, terrainArray[index].mapPixelX, terrainArray[index].mapPixelY);
                    int currDist = GetTerrainDist(LocalPlayerGPS.CurrentMapPixel, terrainArray[index].mapPixelX, terrainArray[index].mapPixelY);
                    if ((prevDist == 1 && currDist > 1) || (currDist == 1 && prevDist > 1))
                        terrainArray[index].updateNature = true;
                }
                return;
            }

            // Need to place a new terrain, find next available terrain
            // This will either find a fresh terrain or recycle an old one
            int nextTerrain = FindNextAvailableTerrain();
            if (nextTerrain == -1)
                return;

            // Setup new terrain
            terrainArray[nextTerrain].active = true;
            terrainArray[nextTerrain].updateData = true;
            terrainArray[nextTerrain].updateNature = true;
            terrainArray[nextTerrain].mapPixelX = mapPixelX;
            terrainArray[nextTerrain].mapPixelY = mapPixelY;
            if (!terrainArray[nextTerrain].terrainObject)
            {
                // Create game objects for new terrain
                // This is only done once then recycled
                CreateTerrainGameObjects(
                    mapPixelX,
                    mapPixelY,
                    out terrainArray[nextTerrain].terrainObject,
                    out terrainArray[nextTerrain].billboardBatchObject);
            }

            // Apply local transform
            float scale = MapsFile.WorldMapTerrainDim * MeshReader.GlobalScale;
            int xdif = mapPixelX - mapOrigin.X;
            int ydif = mapPixelY - mapOrigin.Y;
            Vector3 localPosition = new Vector3(xdif * scale, 0, -ydif * scale) + WorldCompensation;
            terrainArray[nextTerrain].terrainObject.transform.localPosition = localPosition;

            // Add new terrain index to dictionary
            terrainIndexDict.Add(key, nextTerrain);

            // Check if terrain has a location, if so it will be added on next update
            ContentReader.MapSummary mapSummary;
            terrainArray[nextTerrain].hasLocation = dfUnity.ContentReader.HasLocation(mapPixelX, mapPixelY, out mapSummary);
            terrainArray[nextTerrain].updateLocation = terrainArray[nextTerrain].hasLocation;
        }

        // Finds next available terrain in array
        private int FindNextAvailableTerrain()
        {
            // Evaluate terrain array
            int found = -1;
            for (int i = 0; i < terrainArray.Length; i++)
            {
                // A null terrain has never been instantiated and is free
                if (terrainArray[i].terrainObject == null)
                {
                    found = i;
                    break;
                }

                // Inactive terrain object can be evaluated for recycling based
                // on distance from current map pixel
                if (!terrainArray[i].active)
                {
                    // If terrain out of range then recycle
                    if (!IsInRange(terrainArray[i].mapPixelX, terrainArray[i].mapPixelY))
                    {
                        found = i;
                        break;
                    }
                }
            }

            // Was a terrain found?
            if (found != -1)
            {
                // If we are recycling an inactive terrain, remove it from dictionary first
                int key = TerrainHelper.MakeTerrainKey(terrainArray[found].mapPixelX, terrainArray[found].mapPixelY);
                if (terrainIndexDict.ContainsKey(key))
                {
                    terrainIndexDict.Remove(key);
                }
                return found;
            }
            else
            {
                // Unable to find an available terrain
                // This should never happen unless TerrainDistance too high or maxTerrainArray too low
                DaggerfallUnity.LogMessage("StreamingWorld: Unable to find free terrain. Check maxTerrainArray is sized appropriately and you are collecting terrains. This can also happen when player movement speed too high.", true);
                if (Application.isEditor)
                    Debug.Break();
                else
                    Application.Quit();

                return -1;
            }
        }

        #endregion

        #region World Cleanup Methods

        // Remove managed child objects
        private void ClearStreamingWorld()
        {
            // Collect everything
            CollectTerrains(true);
            CollectLooseObjects(true);

            // Clear lists
            terrainIndexDict.Clear();

            // Raise event
            RaiseOnClearStreamingWorldEvent();
        }

        // Mark inactive any terrains outside of range
        private void CollectTerrains(bool collectAll = false)
        {
            for (int i = 0; i < terrainArray.Length; i++)
            {
                // Ignore uninitialised and inactive terrains
                if (terrainArray[i].terrainObject == null ||
                    !terrainArray[i].active)
                {
                    continue;
                }

                // If terrain out of range then mark inactive for recycling
                if (!IsInRange(terrainArray[i].mapPixelX, terrainArray[i].mapPixelY) || collectAll)
                {
                    // Mark terrain inactive
                    terrainArray[i].terrainObject.name = "Pooled";
                    terrainArray[i].active = false;
                    terrainArray[i].terrainObject.SetActive(false);
                    terrainArray[i].billboardBatchObject.SetActive(false);

                    // If collecting all then ensure terrain is out of range
                    // This fixes a bug where continuously loading same location overflows terrain buffer
                    if (collectAll)
                    {
                        terrainArray[i].mapPixelX = int.MinValue;
                        terrainArray[i].mapPixelY = int.MinValue;
                        terrainArray[i].updateLocation = terrainArray[i].hasLocation;
                    }
                }
            }
        }

        // Destroy any loose objects outside of range
        private void CollectLooseObjects(bool collectAll = false)
        {
            // Walk list backward to RemoveAt doesn't shift unprocessed items
            for (int i = looseObjectsList.Count; i-- > 0;)
            {
                if (!IsInRange(looseObjectsList[i].mapPixelX, looseObjectsList[i].mapPixelY) || collectAll)
                {
                    if (looseObjectsList[i].gameObject != null)
                    {
                        looseObjectsList[i].gameObject.SetActive(false);
                        StartCoroutine(DestroyGameObjectIterative(looseObjectsList[i].gameObject));
                    }
                    looseObjectsList.RemoveAt(i);
                }
            }
        }

        // Iteratively destroys game object as unity seems bad at doing this when just destroying parent
        private IEnumerator DestroyGameObjectIterative(GameObject gameObject)
        {
            // Destroy all children iteratively
            foreach (Transform t in gameObject.transform)
            {
                DestroyGameObjectIterative(t.gameObject);
                yield return new WaitForEndOfFrame();
            }

            // Now destroy this object
            GameObject.Destroy(gameObject.transform.gameObject);
        }

        #endregion

        #region World Utility Methods

        // Teleports to map pixel with an optional reset or autoreposition
        void TeleportToMapPixel(int mapPixelX, int mapPixelY, Vector3 repositionOffset, RepositionMethods autoReposition)
        {
            if (autoReposition == RepositionMethods.DirectionFromStartMarker)
            {
                // Save travel origin
                travelStartX = LocalPlayerGPS.WorldX;
                travelStartZ = LocalPlayerGPS.WorldZ;
            }
            DFPosition worldPos = MapsFile.MapPixelToWorldCoord(mapPixelX, mapPixelY);
            LocalPlayerGPS.WorldX = worldPos.X;
            LocalPlayerGPS.WorldZ = worldPos.Y;
            LocalPlayerGPS.UpdateWorldInfo();
            autoRepositionOffset = repositionOffset;
            autoRepositionMethod = autoReposition;
            suppressWorld = false;
            InitWorld();
            // Clear falling damage so player doesn't take damage if they were in the air before a transition
            GameManager.Instance.AcrobatMotor.ClearFallingDamage();
            RaiseOnTeleportToCoordinatesEvent(worldPos);
        }

        // Sets terrain neighbours
        // Should only be done after terrain is placed and collected
        private void UpdateNeighbours()
        {
            for (int i = 0; i < terrainArray.Length; i++)
            {
                // Check object exists
                if (!terrainArray[i].terrainObject)
                    continue;

                // Get DaggerfallTerrain
                DaggerfallTerrain dfTerrain = terrainArray[i].terrainObject.GetComponent<DaggerfallTerrain>();
                if (!dfTerrain)
                    continue;

                // Set or clear neighbours
                if (terrainArray[i].active)
                {
                    dfTerrain.LeftNeighbour = GetTerrain(dfTerrain.MapPixelX - 1, dfTerrain.MapPixelY);
                    dfTerrain.RightNeighbour = GetTerrain(dfTerrain.MapPixelX + 1, dfTerrain.MapPixelY);
                    dfTerrain.TopNeighbour = GetTerrain(dfTerrain.MapPixelX, dfTerrain.MapPixelY - 1);
                    dfTerrain.BottomNeighbour = GetTerrain(dfTerrain.MapPixelX, dfTerrain.MapPixelY + 1);
                }
                else
                {
                    dfTerrain.LeftNeighbour = null;
                    dfTerrain.RightNeighbour = null;
                    dfTerrain.TopNeighbour = null;
                    dfTerrain.BottomNeighbour = null;
                }

                // Update Unity Terrain
                dfTerrain.UpdateNeighbours();
            }
        }

        // Anything greater than TerrainDistance+1 is out of range
        private bool IsInRange(int mapPixelX, int mapPixelY)
        {
            if (Math.Abs(mapPixelX - this.MapPixelX) > TerrainDistance ||
                Math.Abs(mapPixelY - this.MapPixelY) > TerrainDistance)
            {
                return false;
            }

            return true;
        }

        // Create new terrain game objects
        public void CreateTerrainGameObjects(int mapPixelX, int mapPixelY, out GameObject terrainObject, out GameObject billboardBatchObject)
        {
            // Create new terrain object parented to streaming world
            terrainObject = GameObjectHelper.CreateDaggerfallTerrainGameObject(StreamingTarget);
            terrainObject.name = TerrainHelper.GetTerrainName(mapPixelX, mapPixelY);
            terrainObject.hideFlags = defaultHideFlags;

            // Create new billboard batch object parented to terrain
            billboardBatchObject = new GameObject();
            billboardBatchObject.name = string.Format("DaggerfallBillboardBatch [{0},{1}]", mapPixelX, mapPixelY);
            billboardBatchObject.hideFlags = defaultHideFlags;
            billboardBatchObject.transform.parent = terrainObject.transform;
            billboardBatchObject.transform.localPosition = Vector3.zero;
            billboardBatchObject.AddComponent<DaggerfallBillboardBatch>();
        }

        // Create new location game object
        private GameObject CreateLocationGameObject(int terrain, out DFLocation locationOut)
        {
            locationOut = new DFLocation();

            // Terrain must have a location
            DaggerfallTerrain dfTerrain = terrainArray[terrain].terrainObject.GetComponent<DaggerfallTerrain>();
            if (!dfTerrain.MapData.hasLocation)
                return null;

            // Get location data
            locationOut = dfUnity.ContentReader.MapFileReader.GetLocation(dfTerrain.MapData.mapRegionIndex, dfTerrain.MapData.mapLocationIndex);
            if (!locationOut.Loaded)
                return null;

            // Sample height of terrain at origin tile position, this is more accurate than scaled average - thanks Nystul!
            // TODO: Daggerfall does not always position locations at exact centre as assumed.
            // Working around this for now and will update this code later
            Terrain terrainInstance = dfTerrain.gameObject.GetComponent<Terrain>();
            float scale = terrainInstance.terrainData.heightmapScale.x;
            float xSamplePos = dfUnity.TerrainSampler.HeightmapDimension * 0.55f;
            float ySamplePos = dfUnity.TerrainSampler.HeightmapDimension * 0.55f;
            Vector3 pos = new Vector3(xSamplePos * scale, 0, ySamplePos * scale);
            float height = terrainInstance.SampleHeight(pos + terrainArray[terrain].terrainObject.transform.position);

            // Spawn parent game object for new location
            //float height = dfTerrain.MapData.averageHeight * TerrainScale;
            GameObject locationObject = new GameObject(string.Format("DaggerfallLocation [Region={0}, Name={1}]", locationOut.RegionName, locationOut.Name));
            locationObject.transform.parent = StreamingTarget;
            locationObject.hideFlags = defaultHideFlags;
            locationObject.transform.position = terrainArray[terrain].terrainObject.transform.position + new Vector3(0, height, 0);
            DaggerfallLocation dfLocation = locationObject.AddComponent<DaggerfallLocation>() as DaggerfallLocation;
            dfLocation.SetLocation(locationOut, false);

            // Raise event
            RaiseOnCreateLocationGameObjectEvent(dfLocation);

            return locationObject;
        }

        // Update terrain data.
        public void UpdateTerrainData(TerrainDesc terrainDesc)
        {
            // Instantiate Daggerfall terrain
            DaggerfallTerrain dfTerrain = terrainDesc.terrainObject.GetComponent<DaggerfallTerrain>();
            if (dfTerrain)
            {
                dfTerrain.TerrainScale = TerrainScale;
                dfTerrain.MapPixelX = terrainDesc.mapPixelX;
                dfTerrain.MapPixelY = terrainDesc.mapPixelY;
                dfTerrain.InstantiateTerrain();
            }

            // Update data for terrain
            JobHandle updateTerrainDataJobHandle = dfTerrain.BeginMapPixelDataUpdate(dfUnity.TerrainTexturing);

            CompleteUpdateTerrainDataJobs(terrainDesc, dfTerrain, updateTerrainDataJobHandle);
        }

        private void CompleteUpdateTerrainDataJobs(TerrainDesc terrainDesc, DaggerfallTerrain dfTerrain, JobHandle updateTerrainDataJobHandle)
        {
            // Ensure jobs have completed.
            updateTerrainDataJobHandle.Complete();
            dfTerrain.CompleteMapPixelDataUpdate(dfUnity.TerrainTexturing);

            // Promote data to live terrain
            dfTerrain.UpdateClimateMaterial(init);
            dfTerrain.PromoteTerrainData();

            // Only set active again once complete
            terrainDesc.terrainObject.SetActive(true);
            terrainDesc.terrainObject.name = TerrainHelper.GetTerrainName(dfTerrain.MapPixelX, dfTerrain.MapPixelY);
        }

        // Update terrain data using coroutine to decouple from main thread & FPS.
        private IEnumerator UpdateTerrainDataCoroutine(TerrainDesc terrainDesc)
        {
            // Instantiate Daggerfall terrain
            DaggerfallTerrain dfTerrain = terrainDesc.terrainObject.GetComponent<DaggerfallTerrain>();
            if (dfTerrain)
            {
                dfTerrain.TerrainScale = TerrainScale;
                dfTerrain.MapPixelX = terrainDesc.mapPixelX;
                dfTerrain.MapPixelY = terrainDesc.mapPixelY;
                dfTerrain.InstantiateTerrain();
            }

            JobHandle updateTerrainDataJobHandle = dfTerrain.BeginMapPixelDataUpdate(dfUnity.TerrainTexturing);
            yield return new WaitUntil(() => updateTerrainDataJobHandle.IsCompleted);

            CompleteUpdateTerrainDataJobs(terrainDesc, dfTerrain, updateTerrainDataJobHandle);
        }

        // Update terrain nature
        public void UpdateTerrainNature(TerrainDesc terrainDesc)
        {
#if SHOW_LAYOUT_TIMES_NATURE
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
#endif
            // Setup billboards
            DaggerfallTerrain dfTerrain = terrainDesc.terrainObject.GetComponent<DaggerfallTerrain>();
            DaggerfallBillboardBatch dfBillboardBatch = terrainDesc.billboardBatchObject.GetComponent<DaggerfallBillboardBatch>();
            if (dfTerrain && dfBillboardBatch)
            {
                // Get current climate and nature archive and terrain distance
                int natureArchive = ClimateSwaps.GetNatureArchive(LocalPlayerGPS.ClimateSettings.NatureSet, dfUnity.WorldTime.Now.SeasonValue);
                dfBillboardBatch.SetMaterial(natureArchive);
                int terrainDist = GetTerrainDist(LocalPlayerGPS.CurrentMapPixel, dfTerrain.MapPixelX, dfTerrain.MapPixelY);
                dfUnity.TerrainNature.LayoutNature(dfTerrain, dfBillboardBatch, TerrainScale, terrainDist);
            }

            // Only set active again once complete
            terrainDesc.billboardBatchObject.SetActive(true);
#if SHOW_LAYOUT_TIMES_NATURE
            stopwatch.Stop();
            DaggerfallUnity.LogMessage(string.Format("Time to update terrain natures for ({1},{2}): {0}ms", stopwatch.ElapsedMilliseconds, terrainDesc.mapPixelX, terrainDesc.mapPixelY), true);
#endif
        }

        // Gets terrain at map pixel coordinates, or null if not found
        private Terrain GetTerrain(int mapPixelX, int mapPixelY)
        {
            int key = TerrainHelper.MakeTerrainKey(mapPixelX, mapPixelY);
            if (terrainIndexDict.ContainsKey(key))
            {
                return terrainArray[terrainIndexDict[key]].terrainObject.GetComponent<Terrain>();
            }

            return null;
        }

        #endregion

        #region Player Utility Methods

        // Calculate the distance of terrain from given map position
        private int GetTerrainDist(DFPosition mapPosition, int terrainMapPixelX, int terrainMapPixelY)
        {
            DFPosition curMapPixel = LocalPlayerGPS.CurrentMapPixel;
            int dx = Mathf.Abs(terrainMapPixelX - mapPosition.X);
            int dy = Mathf.Abs(terrainMapPixelY - mapPosition.Y);
            return Mathf.Max(dx, dy);
        }

        private DaggerfallTerrain GetPlayerTerrain()
        {
            Transform terrainTransform = GetPlayerTerrainTransform();
            if (terrainTransform)
                return terrainTransform.GetComponent<DaggerfallTerrain>();

            return null;
        }

        // Gets transform of the terrain player is standing on
        private Transform GetPlayerTerrainTransform()
        {
            int key = TerrainHelper.MakeTerrainKey(MapPixelX, MapPixelY);
            if (!terrainIndexDict.ContainsKey(key))
                return null;

            return terrainArray[terrainIndexDict[key]].terrainObject.transform;
        }

        // Repositions player in world after a teleport.
        // If position.y is less than terrain height then player will be raised to sit on terrain.
        // If grounded is true, position.y will be unconditionally set to terrain height.
        // Terrain data must already be loaded and LocalGPS must be attached to your player game object.
        private void RepositionPlayer(int mapPixelX, int mapPixelY, Vector3 position, bool grounded = false)
        {
            // Get terrain key
            int key = TerrainHelper.MakeTerrainKey(mapPixelX, mapPixelY);
            if (!terrainIndexDict.ContainsKey(key))
                return;

            // Get terrain
            Terrain terrain = terrainArray[terrainIndexDict[key]].terrainObject.GetComponent<Terrain>();

            // Sample height at this position
            CharacterController controller = LocalPlayerGPS.gameObject.GetComponent<CharacterController>();
            if (controller)
            {
                // Get target position at terrain height + player standing height
                // This is our minimum height before player falls through world
                IsRepositioningPlayer = true;
                Vector3 targetPosition = new Vector3(position.x, 0, position.z);
                float height = terrain.SampleHeight(targetPosition + terrain.transform.position) + worldCompensation.y;
                targetPosition.y = height + controller.height / 2f + 0.15f;

                // If desired position is higher then minimum position then we can safely use that
                if (!grounded && position.y > targetPosition.y)
                    targetPosition.y = position.y;

                // Move player object to new position
                LocalPlayerGPS.transform.position = targetPosition;

                // Clear falling damage so player doesn't take damage after reposition
                GameManager.Instance.AcrobatMotor.ClearFallingDamage();

                // Perform another pass at collecting loose objects as sometimes they don't clean up properly after fast travel
                // This is more common the more loose objects are present in scene, such as with a greater TerrainDistance
                CollectLooseObjects();

                ResyncWorldCoordinates();
                IsRepositioningPlayer = false;
            }
            else
            {
                throw new Exception("StreamingWorld: Could not find CharacterController peered with LocalPlayerGPS.");
            }
        }

        // Position player outside first dungeon door of this location
        private void PositionPlayerToDungeonExit(DaggerfallLocation location = null)
        {
            // If location is null, try current player location and bail if not found
            if (!location)
            {
                location = GetPlayerLocationObject();
                if (!location)
                    return;
            }

            DaggerfallStaticDoors[] doors = location.StaticDoorCollections;

            // If no doors found then just position to origin
            if (doors == null || doors.Length == 0)
            {
                RepositionPlayer(MapPixelX, MapPixelY, Vector3.zero);
                return;
            }

            // Find vertically lowest dungeon door, this should be the ground-level door
            int foundIndex = -1;
            float lowestHeight = float.MaxValue;
            DaggerfallStaticDoors foundCollection = null;
            Vector3 foundDoorNormal = Vector3.zero;
            foreach (var collection in doors)
            {
                for (int i = 0; i < collection.Doors.Length; i++)
                {
                    if (collection.Doors[i].doorType == DoorTypes.DungeonEntrance)
                    {
                        Vector3 pos = collection.GetDoorPosition(i);
                        if (pos.y < lowestHeight)
                        {
                            lowestHeight = pos.y;
                            foundCollection = collection;
                            foundDoorNormal = collection.GetDoorNormal(i);
                            foundIndex = i;
                        }
                    }
                }
            }

            // Position player outside door position
            if (foundCollection)
            {
                Vector3 startPosition = foundCollection.GetDoorPosition(foundIndex);
                startPosition += foundDoorNormal * (GameManager.Instance.PlayerController.radius + 0.1f);
                RepositionPlayer(MapPixelX, MapPixelY, startPosition);
            }
            else
            {
                RepositionPlayer(MapPixelX, MapPixelY, Vector3.zero);
            }

            // Set player facing away from door
            PlayerMouseLook playerMouseLook = GameManager.Instance.PlayerMouseLook;
            if (playerMouseLook)
            {
                playerMouseLook.SetHorizontalFacing(foundDoorNormal);
            }
        }

        private void PositionPlayerToLocation()
        {
            // Find current location
            DaggerfallLocation currentLocation = GetPlayerLocationObject();
            if (!currentLocation)
            {
                // No location found, fail back to terrain origin
                RepositionPlayer(MapPixelX, MapPixelY, Vector3.zero);
                return;
            }

            // Get location dimensions for positioning
            int width = currentLocation.Summary.BlockWidth;
            int height = currentLocation.Summary.BlockHeight;
            DFPosition tilePos = TerrainHelper.GetLocationTerrainTileOrigin(currentLocation.Summary.LegacyLocation);
            Vector3 origin = new Vector3(tilePos.X * RMBLayout.RMBTileSide, 2.0f * MeshReader.GlobalScale, tilePos.Y * RMBLayout.RMBTileSide);

            // Position player to random side of location
            PositionPlayerToLocation(
                MapPixelX,
                MapPixelY,
                currentLocation,
                origin,
                width,
                height,
                (currentLocation.Summary.LocationType == DFRegion.LocationTypes.TownCity ||
                currentLocation.Summary.LocationType == DFRegion.LocationTypes.HomeYourShips),
                currentLocation.Summary.LocationType != DFRegion.LocationTypes.HomeYourShips);
        }

        // Sets player to ground level near a location
        // Will spawn at a random edge facing location
        // Can use start markers if present
        private void PositionPlayerToLocation(
            int mapPixelX,
            int mapPixelY,
            DaggerfallLocation dfLocation,
            Vector3 origin,
            int mapWidth,
            int mapHeight,
            bool useNearestStartMarker = false,
            bool grounded = true)
        {
            UnityEngine.Random.InitState(DateTime.Now.Millisecond);

            int side;
            if (travelStartX == null || travelStartZ == null)
            {
                // Randomly pick one side of location to spawn
                side = UnityEngine.Random.Range(0, 4);
            }
            else
            {
                // use travel origin as a facing hint
                int worldDeltaX = LocalPlayerGPS.WorldX - (int)travelStartX;
                int worldDeltaZ = LocalPlayerGPS.WorldZ - (int)travelStartZ;

                if (worldDeltaX == 0 && worldDeltaZ == 0)
                    side = UnityEngine.Random.Range(0, 4);
                else
                {
                    int px = Math.Abs(worldDeltaX);
                    int pz = Math.Abs(worldDeltaZ);
                    // if travel start is distant enough, chances of hitting square sides are
                    // approximatively px/(px+pz) and pz/(px+pz)
                    int random = UnityEngine.Random.Range(0, px + pz);
                    if (px > pz)
                    {
                        // direction is mainly E-W, do we hit square front side?
                        if (random < px)
                            side = worldDeltaX > 0 ? 3 : 2;
                        else
                            side = worldDeltaZ > 0 ? 1 : 0;
                    }
                    else
                    {
                        // direction is mainly N-S, do we hit square front side?
                        if (random < pz)
                            side = worldDeltaZ > 0 ? 1 : 0;
                        else
                            side = worldDeltaX > 0 ? 3 : 2;
                    }
                }
                // Debug.Log(String.Format("{0},{1} => {2},{3}: facing {4}", (int)travelStartX, (int)travelStartZ, LocalPlayerGPS.WorldX, LocalPlayerGPS.WorldZ, side));
                travelStartX = null;
                travelStartZ = null;
            }

            // Get half width and height
            float halfWidth = (float)mapWidth * 0.5f * RMBLayout.RMBSide;
            float halfHeight = (float)mapHeight * 0.5f * RMBLayout.RMBSide;
            Vector3 centre = origin + new Vector3(halfWidth, 0, halfHeight);

            // Sometimes buildings are right up to edge of block
            // Extra distance places player a little bit outside location area
            float extraDistance = RMBLayout.RMBSide * 0.1f;

            // Start player in position
            Vector3 newPlayerPosition = centre;
            PlayerMouseLook mouseLook = LocalPlayerGPS.GetComponentInChildren<PlayerMouseLook>();
            if (mouseLook)
            {
                switch (side)
                {
                    case 0:         // North
                        newPlayerPosition += new Vector3(0, 0, (halfHeight + extraDistance));
                        mouseLook.SetFacing(180, 0);
                        //LocalPlayerGPS.gameObject.SendMessage("SetFacing", Vector3.back, SendMessageOptions.DontRequireReceiver);
                        //Debug.Log("Spawned player north.");
                        break;
                    case 1:         // South
                        newPlayerPosition += new Vector3(0, 0, -(halfHeight + extraDistance));
                        mouseLook.SetFacing(0, 0);
                        //LocalPlayerGPS.gameObject.SendMessage("SetFacing", Vector3.forward, SendMessageOptions.DontRequireReceiver);
                        //Debug.Log("Spawned player south.");
                        break;
                    case 2:         // East
                        newPlayerPosition += new Vector3((halfWidth + extraDistance), 0, 0);
                        mouseLook.SetFacing(270, 0);
                        //LocalPlayerGPS.gameObject.SendMessage("SetFacing", Vector3.left, SendMessageOptions.DontRequireReceiver);
                        //Debug.Log("Spawned player east.");
                        break;
                    case 3:         // West
                        newPlayerPosition += new Vector3(-(halfWidth + extraDistance), 0, 0);
                        mouseLook.SetFacing(90, 0);
                        //LocalPlayerGPS.gameObject.SendMessage("SetFacing", Vector3.right, SendMessageOptions.DontRequireReceiver);
                        //Debug.Log("Spawned player west.");
                        break;
                }
            }

            // Adjust to nearest start marker if requested
            if (useNearestStartMarker)
            {
                float smallestDistance = float.MaxValue;
                int closestMarker = -1;
                GameObject[] startMarkers = dfLocation.StartMarkers;
                for (int i = 0; i < startMarkers.Length; i++)
                {
                    float distance = Vector3.Distance(newPlayerPosition, startMarkers[i].transform.position);
                    if (distance < smallestDistance)
                    {
                        smallestDistance = distance;
                        closestMarker = i;
                    }
                }
                if (closestMarker != -1)
                {
                    //PositionPlayerToTerrain(mapPixelX, mapPixelY, startMarkers[closestMarker].transform.position);
                    RepositionPlayer(mapPixelX, mapPixelY, startMarkers[closestMarker].transform.position, grounded);
                    return;
                }
            }

            // Just position to outside location
            //PositionPlayerToTerrain(mapPixelX, mapPixelY, newPlayerPosition);
            RepositionPlayer(mapPixelX, mapPixelY, newPlayerPosition, grounded);
        }

        // Align player to ground
        private bool FixStanding(Transform playerTransform, float playerHeight, float extraHeight = 0, float extraDistance = 0)
        {
            RaycastHit hit;
            Ray ray = new Ray(playerTransform.position + (Vector3.up * extraHeight), Vector3.down);
            if (Physics.Raycast(ray, out hit, (playerHeight * 2) + extraHeight + extraDistance))
            {
                // Position player at hit position plus just over half controller height up
                playerTransform.position = hit.point + Vector3.up * (playerHeight * 0.6f);
                return true;
            }

            return false;
        }

        // Resync world coordinates after changing player transform position
        void ResyncWorldCoordinates()
        {
            Vector3 playerPos = LocalPlayerGPS.transform.position;
            DFPosition mapPixelOrigin = MapsFile.MapPixelToWorldCoord(MapPixelX, MapPixelY);
            worldX = mapPixelOrigin.X + (playerPos.x * SceneMapRatio);
            worldZ = mapPixelOrigin.Y + (playerPos.z * SceneMapRatio);
            lastPlayerPos = playerPos;
        }

        // Destroy untracked objects parented to streaming target
        // This will remove loose enemies, missiles, etc. on load or new game
        // These dynamically spawned objects are fully untracked in wilderness
        void CleanupUntrackedObjects()
        {
            // Destroy loose enemies
            EnemyMotor[] enemies = StreamingTarget.GetComponentsInChildren<EnemyMotor>();
            foreach(EnemyMotor enemy in enemies)
                GameObject.Destroy(enemy.gameObject);

            // Destroy loose missiles
            DaggerfallMissile[] missiles = StreamingTarget.GetComponentsInChildren<DaggerfallMissile>();
            foreach (DaggerfallMissile missile in missiles)
                GameObject.Destroy(missile.gameObject);
        }

        private void StartGameBehaviour_OnNewGame()
        {
            CleanupUntrackedObjects();
        }

        private void SaveLoadManager_OnStartLoad(SaveData_v1 saveData)
        {
            CleanupUntrackedObjects();
        }

        #endregion

        #region Startup/Shutdown Methods

        private bool ReadyCheck()
        {
            if (suppressWorld)
                return false;

            if (isReady)
                return true;

            if (dfUnity == null)
            {
                dfUnity = DaggerfallUnity.Instance;
            }

            if (LocalPlayerGPS == null)
                return false;
            else
                InitWorld();

            // Do nothing if DaggerfallUnity not ready
            if (!dfUnity.IsReady)
            {
                DaggerfallUnity.LogMessage("StreamingWorld: DaggerfallUnity component is not ready. Have you set your Arena2 path?");
                return false;
            }

            // Perform initial runtime setup
            if (Application.isPlaying)
            {
                // Fix coastal climate data
                TerrainHelper.DilateCoastalClimate(dfUnity.ContentReader, 2);

                // Smooth steep location on steep gradients
                // TODO: What is this supposed to be doing? It doesn't seem to change any data that's used anywhere..
                TerrainHelper.SmoothLocationNeighbourhood(dfUnity.ContentReader);
            }

            // Raise ready flag
            isReady = true;
            RaiseOnReadyEvent();

            return true;
        }

        private string GetDebugString()
        {
            string final = string.Format("[{0},{1}] [{2},{3}] You are in the {4} region with a {5} climate.",
                MapPixelX,
                MapPixelY,
                LocalPlayerGPS.WorldX,
                LocalPlayerGPS.WorldZ,
                LocalPlayerGPS.CurrentRegionName,
                LocalPlayerGPS.ClimateSettings.ClimateType.ToString());
            if (LocalPlayerGPS.CurrentLocation.Loaded)
            {
                final += string.Format(" {0} is nearby.", LocalPlayerGPS.CurrentLocation.Name);
            }

            return final;
        }

        #endregion

        #region Editor Support

#if UNITY_EDITOR

        public void __EditorFindLocation()
        {
            DFLocation location;
            if (!GameObjectHelper.FindMultiNameLocation(EditorFindLocationString, out location))
            {
                DaggerfallUnity.LogMessage(string.Format("Could not find location [Region={0}, Name={1}]", location.RegionName, location.Name), true);
                return;
            }

            int longitude = (int)location.MapTableData.Longitude;
            int latitude = (int)location.MapTableData.Latitude;
            DFPosition pos = MapsFile.LongitudeLatitudeToMapPixel(longitude, latitude);

            MapPixelX = pos.X;
            MapPixelY = pos.Y;
        }

        public void __EditorGetFromPlayerGPS()
        {
            if (LocalPlayerGPS != null)
            {
                DFPosition pos = MapsFile.WorldCoordToMapPixel(LocalPlayerGPS.WorldX, LocalPlayerGPS.WorldZ);
                MapPixelX = pos.X;
                MapPixelY = pos.Y;
            }
        }

        public void __EditorApplyToPlayerGPS()
        {
            if (LocalPlayerGPS != null)
            {
                DFPosition pos = MapsFile.MapPixelToWorldCoord(MapPixelX, MapPixelY);
                LocalPlayerGPS.WorldX = pos.X;
                LocalPlayerGPS.WorldZ = pos.Y;
            }
        }

#endif

        #endregion

        #region Event Handlers

        // OnReady
        public delegate void OnReadyEventHandler();
        public static event OnReadyEventHandler OnReady;
        protected virtual void RaiseOnReadyEvent()
        {
            if (OnReady != null)
                OnReady();
        }

        // OnTeleportToCoordinates
        public delegate void OnTeleportToCoordinatesEventHandler(DFPosition worldPos);
        public static event OnTeleportToCoordinatesEventHandler OnTeleportToCoordinates;
        protected virtual void RaiseOnTeleportToCoordinatesEvent(DFPosition worldPos)
        {
            if (OnTeleportToCoordinates != null)
                OnTeleportToCoordinates(worldPos);
        }

        // OnPreInitWorld
        public delegate void OnPreInitWorldEventHandler();
        public static event OnPreInitWorldEventHandler OnPreInitWorld;
        protected virtual void RaiseOnPreInitWorldEvent()
        {
            if (OnPreInitWorld != null)
                OnPreInitWorld();
        }

        // OnInitWorld
        public delegate void OnInitWorldEventHandler();
        public static event OnInitWorldEventHandler OnInitWorld;
        protected virtual void RaiseOnInitWorldEvent()
        {
            if (OnInitWorld != null)
                OnInitWorld();
        }

        // OnUpdateTerrainsStart
        public delegate void OnUpdateTerrainsStartEventHandler();
        public static event OnUpdateTerrainsStartEventHandler OnUpdateTerrainsStart;
        protected virtual void RaiseOnUpdateTerrainsStartEvent()
        {
            if (OnUpdateTerrainsStart != null)
                OnUpdateTerrainsStart();
        }

        // OnUpdateTerrainsEnd
        public delegate void OnUpdateTerrainsEndEventHandler();
        public static event OnUpdateTerrainsEndEventHandler OnUpdateTerrainsEnd;
        protected virtual void RaiseOnUpdateTerrainsEndEvent()
        {
            if (OnUpdateTerrainsEnd != null)
                OnUpdateTerrainsEnd();
        }

        // OnClearStreamingWorld
        public delegate void OnClearStreamingWorldEventHandler();
        public static event OnClearStreamingWorldEventHandler OnClearStreamingWorld;
        protected virtual void RaiseOnClearStreamingWorldEvent()
        {
            if (OnClearStreamingWorld != null)
                OnClearStreamingWorld();
        }

        // OnCreateLocationGameObject
        public delegate void OnCreateLocationGameObjectEventHandler(DaggerfallLocation dfLocation);
        public static event OnCreateLocationGameObjectEventHandler OnCreateLocationGameObject;
        protected virtual void RaiseOnCreateLocationGameObjectEvent(DaggerfallLocation dfLocation)
        {
            if (OnCreateLocationGameObject != null)
                OnCreateLocationGameObject(dfLocation);
        }

        // OnUpdateLocationGameObject
        public delegate void OnUpdateLocationGameObjectEventHandler(GameObject locationObject, bool allowYeld);
        public static event OnUpdateLocationGameObjectEventHandler OnUpdateLocationGameObject;
        protected virtual void RaiseOnUpdateLocationGameObjectEvent(GameObject locationObject, bool allowYeld)
        {
            if (OnUpdateLocationGameObject != null)
                OnUpdateLocationGameObject(locationObject, allowYeld);
        }

        // OnAvailableLocationGameObject
        public delegate void OnAvailableLocationGameObjectHandler();
        public static event OnAvailableLocationGameObjectHandler OnAvailableLocationGameObject;
        protected virtual void RaiseOnAvailableLocationGameObjectEvent()
        {
            if (OnAvailableLocationGameObject != null)
                OnAvailableLocationGameObject();
        }

        // OnFloatingOriginChange
        public delegate void OnFloatingOriginChangeEventHandler();
        public static event OnFloatingOriginChangeEventHandler OnFloatingOriginChange;
        protected virtual void RaiseOnFloatingOriginChangeEvent()
        {
            if (OnFloatingOriginChange != null)
                OnFloatingOriginChange();
        }

        #endregion
    }
}