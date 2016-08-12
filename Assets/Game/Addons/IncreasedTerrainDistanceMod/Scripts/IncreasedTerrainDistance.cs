//Increased Terrain Distance Mod for Daggerfall Tools For Unity
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: Michael Rauter (a.k.a. Nystul)
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)
//Contributors: Lypyl, Interkarma

// uncomment next line if enhanced sky mod by Lypyl is present
#define ENHANCED_SKY_CODE_AVAILABLE

#define REFLECTIONSMOD_CODE_AVAILABLE

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Utility;

namespace ProjectIncreasedTerrainDistance
{
    /// <summary>
    /// Manages a world terrain object built from the world height map for increased terrain/view distance
    /// one main objective was to make everything work inside this script (and the shader for texturing) without a need for changes in any other files
    /// this means as a consequence that initialization of resources of other scripts that are used inside the script needs to be finished before any other work can be done.
    /// another objective was that render and loading performance should only be impacted as less as possible. I did some investigations and decided to use one big
    /// unity terrain object for the whole world since it gave the best performance compared with splitting up into several terrains. The terrain has to be created only once
    /// and only needs to be translated to match with the StreamingWorld component. Experiments were made in Unity 4.6, don't know if Unity 5.0 would make a difference.
    /// furthermore when using unity terrain one can use unity's level of detail mechanism for geometry (if needed), and other benefits provided by unity’s terrain system.
    /// a consequence is though that low-detailed terrain geometry would be also rendered at the position of the detailed terrain inside the distance from the player defined by TerrainDistance.
    /// the IncreasedTerrainTilemap shader defined in file DaggerfallIncreasedTerrainTilemap.shader discards fragments inside the area containing the detailed terrain from StreamingWorld,
    /// a terrain transition ring is now matching the heights of the detailed terrain with those of the far terrain in a ring of terrain blocks in between them.
    /// </summary>
    public class IncreasedTerrainDistance : MonoBehaviour
    {
        #region Fields

        // Streaming World Component
        public StreamingWorld streamingWorld = null;

        // Local player GPS for tracking player virtual position
        public PlayerGPS playerGPS;       

        // WeatherManager is used for seasonal textures
        public WeatherManager weatherManager;

        public int stackedCameraDepth = 1;
        public int stackedNearCameraDepth = 2;
        public int cameraRenderSkyboxToTextureDepth = -10;
        public float mainCameraFarClipPlane = 1200.0f;
        public FogMode sceneFogMode = FogMode.Exponential;
        public float sceneFogDensity = 0.000025f;

        //public RenderTexture renderTextureSky;

        //[Range(0.0001f, 0.000001f)]
        //public float blendFactor = 0.000015f;

        [Range(0.0f, 300000.0f)]
        public float blendStart = 120000.0f;

        [Range(0.0f, 300000.0f)]
        public float blendEnd = 145000.0f;

        public RenderTexture reflectionSeaTexture = null;

        bool isActiveReflectionsMod = false;
        bool isActiveEnhancedSkyMod = false;


        // is dfUnity ready?
        bool isReady = false;

        // the height values of the world height map used as input for unity terrain function SetHeights()
        float[,] worldHeights = null;

        int worldMapWidth = MapsFile.MaxMapPixelX - MapsFile.MinMapPixelX;
        int worldMapHeight = MapsFile.MaxMapPixelY - MapsFile.MinMapPixelY;

        // used to track changes of playerGPS x- resp. y-position on the world map (-> a change results in an update of the terrain object's translation)
        int MapPixelX = -1;
        int MapPixelY = -1;

        // unity terrain object which will hold the low-detail world map geometry, set to null initially for lazy creation
        GameObject worldTerrainGameObject = null;

        // terrain material used for world texturing (will be changed when daggerFallLocation.currentSeason changes from previous setting)
        Material terrainMaterial = null;

        // map holds info for tiles of terrain: r-channel... climate index, gb... currently unused, a-channel... discard-rendering flag for shader
        Color32[] terrainInfoTileMap = null;
        int terrainInfoTileMapDim;

        // texture for terrainInfoTileMap
        Texture2D textureTerrainInfoTileMap = null;


        ClimateSeason currentSeason = ClimateSeason.Summer;

        Texture2D textureAtlasDesertSummer = null;
        Texture2D textureAtlasWoodlandSummer = null;
        Texture2D textureAtlasMountainSummer = null;
        Texture2D textureAtlasSwampSummer = null;

        Texture2D textureAtlasDesertWinter = null;
        Texture2D textureAtlasWoodlandWinter = null;
        Texture2D textureAtlasMountainWinter = null;
        Texture2D textureAtlasSwampWinter = null;

        Texture2D textureAtlasDesertRain = null;
        Texture2D textureAtlasWoodlandRain = null;
        Texture2D textureAtlasMountainRain = null;
        Texture2D textureAtlasSwampRain = null;


        // List of terrain objects for transition terrain ring
        private struct TransitionRingBorderDesc
        {
            public bool isTopRingBorder;
            public bool isBottomRingBorder;
            public bool isLeftRingBorder;
            public bool isRightRingBorder;
        }

        private struct TransitionTerrainDesc
        {
            public StreamingWorld.TerrainDesc terrainDesc;
            public TransitionRingBorderDesc transitionRingBorderDesc;
            public bool ready; // was creation process complete?
            public bool heightsUpdatePending; // is it necessary to update the heights of terrainObject inside terrainDesc
            public bool keepThisBlock; // can this block be kept and reused
            //public bool positionUpdatePending; // is it necessary to update the position of terrainObject inside terrainDesc
            //public bool updateSeasonalTextures;  // is it necessary to update the textures due to seasonal change
            //public bool updateMaterialProperties;  // is it necessary to update the update material properties
        }
        TransitionTerrainDesc[] terrainTransitionRingArray = null;
        int numberOfTerrainBlocksInTransitionRingArray;
        Dictionary<int, int> terrainTransitionRingIndexDict = new Dictionary<int, int>();

        GameObject gameobjectTerrainTransitionRing = null; // container gameobject for transition ring's terrain blocks
        
        bool terrainTransitionRingUpdateRunning = false;
        bool transitionRingAllBlocksReady = false;
        bool terrainTransitionRingUpdateSeasonalTextures = false;
        bool terrainTransitionRingUpdateMaterialProperties = false;
        bool updateTerrainTransitionRing = false;

        Texture2D tileAtlasReflectiveTexture = null; // used in blending of normal and far terrain to identify pixels in the normal terrain texture that are water

        // stacked near camera (used for near terrain from range 1000-15000) to prevent floating-point rendering precision problems for huge clipping ranges
        Camera stackedNearCamera = null; 

        // stacked camera (used for far terrain) to prevent floating-point rendering precision problems for huge clipping ranges
        Camera stackedCamera = null;

        public Camera getFarTerrainCamera() { return stackedCamera; }
        public Camera getStackedNearCamera() { return stackedNearCamera; }

        #if ENHANCED_SKY_CODE_AVAILABLE
        EnhancedSky.SkyManager skyMan = null;
        bool sampleFogColorFromSky = false;
        #endif

        GameObject goRenderSkyboxToTexture = null;
        Camera cameraRenderSkyboxToTexture = null;

        const int renderTextureSkyWidth = 256;
        const int renderTextureSkyHeight = 256;
        const int renderTextureSkyDepth = 16;
        const RenderTextureFormat renderTextureSkyFormat = RenderTextureFormat.ARGB32;
        RenderTexture renderTextureSky = null;


        // instance of dfUnity
        DaggerfallUnity dfUnity;

        #endregion

        #region Properties

        public bool IsReady { get { return ReadyCheck(); } }

        #endregion

        #region Unity

        static string GetGameObjectPath(GameObject obj)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path;
        }

        void Awake()
        {
            if (!DaggerfallUnity.Settings.Nystul_IncreasedTerrainDistance)
                return;

            dfUnity = DaggerfallUnity.Instance;

            dfUnity.TerrainSampler = new ImprovedTerrainSampler();

            //ImprovedTerrainSampler improvedTerrainSampler = DaggerfallUnity.Instance.TerrainSampler as ImprovedTerrainSampler;
            //if (improvedTerrainSampler == null)
            //{
            //    DaggerfallUnity.LogMessage("IncreasedTerrainDistance: TerrainSampler instance is not of type ImprovedTerrainSampler (use ITerrainSampler terrainSampler = new ImprovedTerrainSampler() in DaggerfallUnity.cs)", true);
            //}

            if (!streamingWorld)
                streamingWorld = GameObject.Find("StreamingWorld").GetComponent<StreamingWorld>();
            if (!streamingWorld)
            {
                DaggerfallUnity.LogMessage("IncreasedTerrainDistance: Missing StreamingWorld reference.", true);
                if (Application.isEditor)
                    Debug.Break();
                else
                    Application.Quit();
            }

            if (!playerGPS)
                playerGPS = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerGPS>();
            if (!playerGPS)
            {
                DaggerfallUnity.LogMessage("IncreasedTerrainDistance: Missing PlayerGPS reference.", true);
                if (Application.isEditor)
                    Debug.Break();
                else
                    Application.Quit();
            }

            if (!weatherManager)
            {
                //weatherManager = GameObject.Find("WeatherManager").GetComponent<WeatherManager>();
                WeatherManager[] weatherManagers = Resources.FindObjectsOfTypeAll<WeatherManager>();
                foreach (WeatherManager currentWeatherManager in weatherManagers)
                {
                    GameObject go = currentWeatherManager.gameObject;
                    string objectPathInHierarchy = GetGameObjectPath(go);
                    if (objectPathInHierarchy == "/Exterior/WeatherManager")
                    {
                        weatherManager = currentWeatherManager;
                        break;
                    }
                }
            }
            if (!weatherManager)
            {
                DaggerfallUnity.LogMessage("IncreasedTerrainDistance: Missing WeatherManager reference.", true);
                if (Application.isEditor)
                    Debug.Break();
                else
                    Application.Quit();
            }

            SetupGameObjects(); // create cameras here in OnAwake() so MirrorReflection script of ReflectionsMod can find cameras in its Start() function
        }

        void OnEnable()
        {
            if (!DaggerfallUnity.Settings.Nystul_IncreasedTerrainDistance)
                return;

            FloatingOrigin.OnPositionUpdate += WorldTerrainUpdatePosition;

            StreamingWorld.OnReady += InitFarTerrain; // important to do actions after TerrainHelper.DilateCoastalClimate() was called in StreamingWorld.ReadyCheck()

            StreamingWorld.OnTeleportToCoordinates += UpdateWorldTerrain;

            PlayerEnterExit.OnTransitionExterior += TransitionToExterior;
            PlayerEnterExit.OnTransitionDungeonExterior += TransitionToExterior;
            #if ENHANCED_SKY_CODE_AVAILABLE
            if (isActiveEnhancedSkyMod)
            {
                EnhancedSky.SkyManager.toggleSkyObjectsEvent += EnhancedSkyToggle;
            }
            #endif
        }

        void OnDisable()
        {
            if (!DaggerfallUnity.Settings.Nystul_IncreasedTerrainDistance)
                return;

            FloatingOrigin.OnPositionUpdate -= WorldTerrainUpdatePosition;

            StreamingWorld.OnReady -= InitFarTerrain;

            StreamingWorld.OnTeleportToCoordinates -= UpdateWorldTerrain;

            PlayerEnterExit.OnTransitionExterior -= TransitionToExterior;
            PlayerEnterExit.OnTransitionDungeonExterior -= TransitionToExterior;

            #if ENHANCED_SKY_CODE_AVAILABLE
            if (isActiveEnhancedSkyMod)
            {
                EnhancedSky.SkyManager.toggleSkyObjectsEvent -= EnhancedSkyToggle;
            }
            #endif
        }

        void EnhancedSkyToggle(bool toggle)
        {
            #if ENHANCED_SKY_CODE_AVAILABLE
            if (toggle)
            {
                sampleFogColorFromSky = true;
            }
            else
            {
                sampleFogColorFromSky = false;
            }
            #endif
        }

        void InitImprovedWorldTerrain()
        {
            // preprocess heights
            ImprovedWorldTerrain.InitImprovedWorldTerrain(DaggerfallUnity.Instance.ContentReader);
        }

        void WorldTerrainUpdatePosition(Vector3 offset)
        {
            if (worldTerrainGameObject != null)
            {
                // do not forget to update shader parameters (needed for correct fragment discarding for terrain tiles of map pixels inside TerrainDistance-1 area (the detailed terrain))
                Terrain terrain = worldTerrainGameObject.GetComponent<Terrain>();
                terrain.materialTemplate.SetInt("_PlayerPosX", this.playerGPS.CurrentMapPixel.X);
                terrain.materialTemplate.SetInt("_PlayerPosY", this.playerGPS.CurrentMapPixel.Y);

                //Debug.Log("update from floating origin event");
                updatePositionWorldTerrain(ref worldTerrainGameObject, offset);
            }
        }

        void InitFarTerrain()
        {
            InitImprovedWorldTerrain();

            generateWorldTerrain();

            GameObject goExterior = null;

            GameObject[] gameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject go in gameObjects)
            {
                string objectPathInHierarchy = GetGameObjectPath(go);
                if (objectPathInHierarchy == "/Exterior")
                {
                    goExterior = go;
                }
            }

            worldTerrainGameObject.transform.SetParent(goExterior.transform);

            // Setup cameras here or stacked camera depth will conflict with main camera on new interior game
            // e.g. start in dungeon, load interior save
            SetUpCameras();

            Terrain terrain = worldTerrainGameObject.GetComponent<Terrain>();

            int worldMapResolution = Math.Max(worldMapWidth, worldMapHeight);

            int[] climateMap = new int[worldMapResolution * worldMapResolution];
            for (int y = 0; y < worldMapHeight; y++)
            {
                for (int x = 0; x < worldMapWidth; x++)
                {
                    // get climate record for this map pixel
                    int worldClimate = dfUnity.ContentReader.MapFileReader.GetClimateIndex(x, y);
                    climateMap[(worldMapHeight - 1 - y) * worldMapResolution + x] = worldClimate;
                }
            }

            terrainInfoTileMapDim = terrain.terrainData.heightmapResolution - 1;

            terrainInfoTileMap = new Color32[terrainInfoTileMapDim * terrainInfoTileMapDim];

            // Assign tile data to tilemap
            Color32 tileColor = new Color32(0, 0, 0, 0);
            for (int y = 0; y < worldMapHeight; y++)
            {
                for (int x = 0; x < worldMapWidth; x++)
                {
                    // Get sample tile data
                    int climateIndex = climateMap[y * worldMapWidth + x];

                    // get location data
                    //byte hasLocation = ImprovedWorldTerrain.MapLocations[y * worldMapWidth + x];                    

                    byte locationMapRangeX = 0;
                    byte locationMapRangeY = 0;
                    if (x < worldMapWidth - 1)
                    {
                        locationMapRangeX = ImprovedWorldTerrain.MapLocationRangeX[(500 - 1 - y) * worldMapWidth + x + 1];
                        locationMapRangeY = ImprovedWorldTerrain.MapLocationRangeY[(500 - 1 - y) * worldMapWidth + x + 1];
                    }

                    byte treeCoverage = ImprovedWorldTerrain.MapTreeCoverage[y * worldMapWidth + x];

                    // Assign to tileMap
                    tileColor.r = Convert.ToByte(climateIndex);
                    tileColor.g = treeCoverage; // hasLocation;
                    tileColor.b = locationMapRangeX;
                    tileColor.a = locationMapRangeY;
                    terrainInfoTileMap[y * terrainInfoTileMapDim + x] = tileColor;
                }
            }

            textureTerrainInfoTileMap = new Texture2D(terrainInfoTileMapDim, terrainInfoTileMapDim, TextureFormat.RGBA32, false);
            textureTerrainInfoTileMap.filterMode = FilterMode.Point;
            textureTerrainInfoTileMap.wrapMode = TextureWrapMode.Clamp;

            // Promote tileMap
            textureTerrainInfoTileMap.SetPixels32(terrainInfoTileMap);
            textureTerrainInfoTileMap.Apply(false);

            terrainMaterial.SetTexture("_MainTex", textureTerrainInfoTileMap);
            terrainMaterial.SetTexture("_FarTerrainTilemapTex", textureTerrainInfoTileMap);

            terrainMaterial.SetInt("_FarTerrainTilemapDim", terrainInfoTileMapDim);

            terrainMaterial.mainTexture = textureTerrainInfoTileMap;

            generateTerrainTransitionRing();
        }

        void Start()
        {
            if (!DaggerfallUnity.Settings.Nystul_IncreasedTerrainDistance)
                return;

            if (GameObject.Find("EnhancedSkyController") != null)
                isActiveEnhancedSkyMod = true;

            if (GameObject.Find("ReflectionsMod") != null)
            {
                if (DaggerfallUnity.Settings.Nystul_RealtimeReflections)
                {
                    isActiveReflectionsMod = true;
                    tileAtlasReflectiveTexture = Resources.Load("tileatlas_reflective") as Texture2D;
                }
            }

            SetUpCameras();

            if (worldTerrainGameObject == null) // lazy creation
            {
                if (!ReadyCheck())
                    return;

                if (!dfUnity.MaterialReader.IsReady)
                    return;

                SetupGameObjects(); // it should be possible to eliminated this line without any impact: please verify!
            }

            // reserve terrain objects for transition ring (2 x long sides (with 2 extra terrains for corner terrains) + 2 x normal sides)
            numberOfTerrainBlocksInTransitionRingArray = 2 * (streamingWorld.TerrainDistance * 2 + 1 + 2) + 2 * (streamingWorld.TerrainDistance * 2 + 1);
            terrainTransitionRingArray = new TransitionTerrainDesc[numberOfTerrainBlocksInTransitionRingArray];
        }

        void OnDestroy()
        {
            if (!DaggerfallUnity.Settings.Nystul_IncreasedTerrainDistance)
                return;

            ImprovedWorldTerrain.Unload();

            worldHeights = null;
            worldTerrainGameObject = null;
            terrainMaterial = null;
            terrainInfoTileMap = null;
            textureTerrainInfoTileMap = null;

            textureAtlasDesertSummer = null;
            textureAtlasWoodlandSummer = null;
            textureAtlasMountainSummer = null;
            textureAtlasSwampSummer = null;

            textureAtlasDesertWinter = null;
            textureAtlasWoodlandWinter = null;
            textureAtlasMountainWinter = null;
            textureAtlasSwampWinter = null;

            textureAtlasDesertRain = null;
            textureAtlasWoodlandRain = null;
            textureAtlasMountainRain = null;
            textureAtlasSwampRain = null;

            Resources.UnloadUnusedAssets();

            System.GC.Collect();
        }

        void TransitionToExterior(PlayerEnterExit.TransitionEventArgs args)
        {
            SetUpCameras();
        }

        void SetupGameObjects()
        {
            int layerExtendedTerrain = LayerMask.NameToLayer("WorldTerrain");
            if (layerExtendedTerrain == -1)
            {
                DaggerfallUnity.LogMessage("Layer with name \"WorldTerrain\" missing! Set it in Unity Editor under \"Edit/Project Settings/Tags and Layers!\"", true);
                if (Application.isEditor)
                    Debug.Break();
                else
                    Application.Quit();
            }

            // Set main camera settings
            Camera.main.farClipPlane = mainCameraFarClipPlane;

            // Set fog mode and density
            // This must be done in script from startup only when mod is enabled
            // The settings in Lighting panel are for core game (no mods) only
            RenderSettings.fogMode = sceneFogMode;
            RenderSettings.fogDensity = sceneFogDensity;

            if (!stackedNearCamera)
            {
                GameObject goStackedNearCamera = new GameObject("stackedNearCamera");
                stackedNearCamera = goStackedNearCamera.AddComponent<Camera>();
                stackedNearCamera.cullingMask = Camera.main.cullingMask;
                stackedNearCamera.nearClipPlane = 980.0f;
                stackedNearCamera.farClipPlane = 15000.0f;
                stackedNearCamera.fieldOfView = Camera.main.fieldOfView;
                stackedNearCamera.renderingPath = Camera.main.renderingPath;
                stackedNearCamera.gameObject.AddComponent<CloneCameraRotationFromMainCamera>();
                stackedNearCamera.gameObject.AddComponent<CloneCameraPositionFromMainCamera>();
                stackedNearCamera.transform.SetParent(this.transform);
            }

            if (!stackedCamera)
            {
                GameObject goStackedCamera = new GameObject("stackedCamera");
                stackedCamera = goStackedCamera.AddComponent<Camera>();
                stackedCamera.cullingMask = (1 << layerExtendedTerrain) +(1 << LayerMask.NameToLayer("Water")); // add water layer so reflections are updated in time (workaround)
                stackedCamera.nearClipPlane = 980.0f;
                stackedCamera.farClipPlane = 300000.0f;
                stackedCamera.fieldOfView = Camera.main.fieldOfView;
                stackedCamera.renderingPath = Camera.main.renderingPath;
                stackedCamera.gameObject.AddComponent<CloneCameraRotationFromMainCamera>();
                stackedCamera.gameObject.AddComponent<CloneCameraPositionFromMainCamera>();
                stackedCamera.transform.SetParent(this.transform);
            }

            if (!renderTextureSky)
            {
                renderTextureSky = new RenderTexture(renderTextureSkyWidth, renderTextureSkyHeight, renderTextureSkyDepth, renderTextureSkyFormat);
            }

            if (!goRenderSkyboxToTexture)
            {
                goRenderSkyboxToTexture = new GameObject("stackedCameraSkyboxRenderToTextureGeneric", typeof(Camera));
                goRenderSkyboxToTexture.transform.SetParent(this.transform);
            }

            if (!cameraRenderSkyboxToTexture)
            {
                cameraRenderSkyboxToTexture = goRenderSkyboxToTexture.GetComponent<Camera>();

                goRenderSkyboxToTexture.AddComponent<CloneCameraRotationFromMainCamera>();
                goRenderSkyboxToTexture.AddComponent<RenderSkyboxWithoutSun>();

                cameraRenderSkyboxToTexture.clearFlags = CameraClearFlags.Skybox;
                cameraRenderSkyboxToTexture.cullingMask = 0; // nothing
                cameraRenderSkyboxToTexture.nearClipPlane = stackedCamera.nearClipPlane;
                cameraRenderSkyboxToTexture.farClipPlane = stackedCamera.farClipPlane;
                cameraRenderSkyboxToTexture.fieldOfView = stackedCamera.fieldOfView;
            }
        }

        void SetUpCameras()
        {
            // Ensure these are setup first or SetUpCameras() will barf
            SetupGameObjects();

            // set up camera stack - AFTER layer "WorldTerrain" has been assigned to worldTerrainGameObject (is done in function generateWorldTerrain())

            Camera.main.clearFlags = CameraClearFlags.Depth;
            stackedNearCamera.clearFlags = CameraClearFlags.Depth;
            stackedCamera.clearFlags = CameraClearFlags.Depth;
            stackedCamera.depth = stackedCameraDepth; // rendered first            
            stackedNearCamera.depth = stackedNearCameraDepth;
            //Camera.main.depth = 3; // renders over stacked camera

            UnityStandardAssets.ImageEffects.GlobalFog globalFogScript = Camera.main.GetComponent<UnityStandardAssets.ImageEffects.GlobalFog>();
            globalFogScript.excludeFarPixels = false; // this is important so that global fog works correctly with camera stack

            cameraRenderSkyboxToTexture.depth = cameraRenderSkyboxToTextureDepth; // make sure to render first
            cameraRenderSkyboxToTexture.renderingPath = stackedCamera.renderingPath;

            #if ENHANCED_SKY_CODE_AVAILABLE
            if (isActiveEnhancedSkyMod)
            {
                //skyMan = GameObject.Find("EnhancedSkyController").GetComponent<EnhancedSky.SkyManager>();
                EnhancedSky.SkyManager[] skyManagers = Resources.FindObjectsOfTypeAll<EnhancedSky.SkyManager>();
                foreach (EnhancedSky.SkyManager currentSkyManager in skyManagers)
                {
                    GameObject go = currentSkyManager.gameObject;
                    string objectPathInHierarchy = GetGameObjectPath(go);
                    if (objectPathInHierarchy == "/EnhancedSkyController")
                    {
                        skyMan = currentSkyManager;
                        break;
                    }
                }

                if ((skyMan) && (skyMan.gameObject.activeInHierarchy))
                {
                    sampleFogColorFromSky = true;
                }
            }
            #endif
    
            cameraRenderSkyboxToTexture.targetTexture = renderTextureSky;
        }

        void setMaterialFogParameters(ref Material terrainMaterial)
        {
            if (terrainMaterial != null)
            {
                if (RenderSettings.fog == true)
                {
                    if (RenderSettings.fogMode == FogMode.Linear)
                    {
                        terrainMaterial.SetInt("_FogMode", 1);
                        terrainMaterial.SetFloat("_FogStartDistance", RenderSettings.fogStartDistance);
                        terrainMaterial.SetFloat("_FogEndDistance", RenderSettings.fogEndDistance);
                    }
                    else if (RenderSettings.fogMode == FogMode.Exponential)
                    {
                        terrainMaterial.SetInt("_FogMode", 2);
                        terrainMaterial.SetFloat("_FogDensity", RenderSettings.fogDensity);
                    }
                    else if (RenderSettings.fogMode == FogMode.ExponentialSquared)
                    {
                        terrainMaterial.SetInt("_FogMode", 3);
                        terrainMaterial.SetFloat("_FogDensity", RenderSettings.fogDensity);
                    }                    
                }
                else
                {
                    terrainMaterial.SetInt("_FogMode", 0);
                }
            }
        }

        void Update()
        {
            if (!DaggerfallUnity.Settings.Nystul_IncreasedTerrainDistance)
                return;

            bool doSeasonalTexturesUpdate = shouldUpdateSeasonalTextures();
            if (worldTerrainGameObject != null)
            {
                // TODO: make sure this block is not executed when in floating origin mode (otherwise position update is done twice)
                // Handle moving to new map pixel or first-time init
                DFPosition curMapPixel = playerGPS.CurrentMapPixel;
                if (curMapPixel.X != MapPixelX ||
                    curMapPixel.Y != MapPixelY)
                {
                    UpdateWorldTerrain(curMapPixel);                
                }

                Terrain terrain = worldTerrainGameObject.GetComponent<Terrain>();
                if (terrain)
                {
                    setMaterialFogParameters(ref this.terrainMaterial);
            
                    #if ENHANCED_SKY_CODE_AVAILABLE
                    if (isActiveEnhancedSkyMod)
                    {
                        if ((sampleFogColorFromSky == true) && (!skyMan.IsOvercast))
                        {
                            terrain.materialTemplate.SetFloat("_FogFromSkyTex", 1);
                        }
                        else
                        {
                            terrain.materialTemplate.SetFloat("_FogFromSkyTex", 0);
                        }
                    }
                    #endif

                    //terrain.materialTemplate.SetFloat("_BlendFactor", blendFactor);
                    terrain.materialTemplate.SetFloat("_BlendStart", blendStart);
                    terrain.materialTemplate.SetFloat("_BlendEnd", blendEnd);
                }

                if (doSeasonalTexturesUpdate)
                {
                    Material mat = terrain.materialTemplate;
                    updateMaterialSeasonalTextures(ref mat, currentSeason); // this is necessary since climate changes may occur after UpdateWorldTerrain() has been invoked, TODO: an event would be ideal to trigger updateSeasonalTextures() instead
                    terrain.materialTemplate = mat;
                }
            }

            // if terrain transition ring was marked to be updated
            if (updateTerrainTransitionRing)
            {
                if (!terrainTransitionRingUpdateRunning) // if at the moment no terrain transition ring update is still in progress
                {
                    // update terrain transition ring in this Update() iteration if no terrain transition ring update is still in progress - otherwise postprone
                    generateTerrainTransitionRing(); // update
                    updateTerrainTransitionRing = false; // mark as updated
                }
            }

            if (gameobjectTerrainTransitionRing != null)
            {
                if (doSeasonalTexturesUpdate)
                {
                    terrainTransitionRingUpdateSeasonalTextures = true;
                }                

                if (!terrainTransitionRingUpdateRunning) // if at the moment no terrain transition ring update is still in progress
                {
                    terrainTransitionRingUpdateMaterialProperties = true;                    
                }
            }

            if (terrainTransitionRingUpdateSeasonalTextures)
            {
                updateSeasonalTexturesTerrainTransitionRing();
            }
            if (terrainTransitionRingUpdateMaterialProperties)
            {
                updateMaterialShaderPropertiesTerrainTransitionRing();
            }
        }

        void UpdateWorldTerrain()
        {
            UpdateWorldTerrain(playerGPS.CurrentMapPixel);
        }

        void UpdateWorldTerrain(DFPosition worldPos)
        {         
            if (worldTerrainGameObject != null) // sometimes it can happen that this point is reached before worldTerrainGameObject was created, in such case we just skip
            {
                // do not forget to update shader parameters (needed for correct fragment discarding for terrain tiles of map pixels inside TerrainDistance-1 area (the detailed terrain))
                Terrain terrain = worldTerrainGameObject.GetComponent<Terrain>();
                terrain.materialTemplate.SetInt("_PlayerPosX", this.playerGPS.CurrentMapPixel.X);
                terrain.materialTemplate.SetInt("_PlayerPosY", this.playerGPS.CurrentMapPixel.Y);

                Vector3 offset = new Vector3(0.0f, 0.0f, 0.0f);
                updatePositionWorldTerrain(ref worldTerrainGameObject, offset);

                bool doSeasonalTexturesUpdate = shouldUpdateSeasonalTextures();

                if (doSeasonalTexturesUpdate)
                {
                    Material mat = terrain.materialTemplate;
                    updateMaterialSeasonalTextures(ref mat, currentSeason); // this is necessary since climate changes may occur after UpdateWorldTerrain() has been invoked, TODO: an event would be ideal to trigger updateSeasonalTextures() instead
                    terrain.materialTemplate = mat;
                }              

                // make terrain transition ring update on next iteration in Update() as soon as no terrain transition ring update is still in progress
                updateTerrainTransitionRing = true;

                if (doSeasonalTexturesUpdate)
                {
                    //updateSeasonalTexturesTerrainTransitionRing();
                    terrainTransitionRingUpdateSeasonalTextures = true;
                }

                Resources.UnloadUnusedAssets();

                //System.GC.Collect();
            }
        }

        #endregion

        #region Private Methods

        private void updateMaterialSeasonalTextures(ref Material terrainMaterial, ClimateSeason currentSeason)
        {

            switch (currentSeason)
            {
                case ClimateSeason.Summer:
                    terrainMaterial.SetTexture("_TileAtlasTexDesert", textureAtlasDesertSummer);
                    terrainMaterial.SetTexture("_TileAtlasTexWoodland", textureAtlasWoodlandSummer);
                    terrainMaterial.SetTexture("_TileAtlasTexMountain", textureAtlasMountainSummer);
                    terrainMaterial.SetTexture("_TileAtlasTexSwamp", textureAtlasSwampSummer);
                    terrainMaterial.SetInt("_TextureSetSeasonCode", 0);
                    break;
                case ClimateSeason.Winter:
                    terrainMaterial.SetTexture("_TileAtlasTexDesert", textureAtlasDesertWinter);
                    terrainMaterial.SetTexture("_TileAtlasTexWoodland", textureAtlasWoodlandWinter);
                    terrainMaterial.SetTexture("_TileAtlasTexMountain", textureAtlasMountainWinter);
                    terrainMaterial.SetTexture("_TileAtlasTexSwamp", textureAtlasSwampWinter);
                    terrainMaterial.SetInt("_TextureSetSeasonCode", 1);
                    break;
                case ClimateSeason.Rain:
                    terrainMaterial.SetTexture("_TileAtlasTexDesert", textureAtlasDesertRain);
                    terrainMaterial.SetTexture("_TileAtlasTexWoodland", textureAtlasWoodlandRain);
                    terrainMaterial.SetTexture("_TileAtlasTexMountain", textureAtlasMountainRain);
                    terrainMaterial.SetTexture("_TileAtlasTexSwamp", textureAtlasSwampRain);
                    terrainMaterial.SetInt("_TextureSetSeasonCode", 2);
                    break;
                default:
                    terrainMaterial.SetTexture("_TileAtlasTexDesert", textureAtlasDesertSummer);
                    terrainMaterial.SetTexture("_TileAtlasTexWoodland", textureAtlasWoodlandSummer);
                    terrainMaterial.SetTexture("_TileAtlasTexMountain", textureAtlasMountainSummer);
                    terrainMaterial.SetTexture("_TileAtlasTexSwamp", textureAtlasSwampSummer);
                    terrainMaterial.SetInt("_TextureSetSeasonCode", 0);
                    break;
            }
        }

        private bool shouldUpdateSeasonalTextures()
        {
            if (!weatherManager)
                return false;

            ClimateSeason newSeason;

            // Get season and weather
            if (dfUnity.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter)
            {
                newSeason = ClimateSeason.Winter;
            }
            else
            {
                newSeason = ClimateSeason.Summer;

                if (weatherManager.IsRaining)
                {
                    newSeason = ClimateSeason.Rain;
                }
                else if (weatherManager.IsSnowing) // should not happen (snow in summer would be weird...)
                {
                    newSeason = ClimateSeason.Winter;
                }
            }

            if (newSeason != currentSeason)
            {
                currentSeason = newSeason;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void updatePositionWorldTerrain(ref GameObject terrainGameObject, Vector3 offset)
        {
            float extraTranslationY = -10.0f;

            // world scale computed as in StreamingWorld.cs and DaggerfallTerrain.cs scripts
            float scale = MapsFile.WorldMapTerrainDim * MeshReader.GlobalScale;
            
            // get displacement in world map pixels
            float xdif = + 1 - playerGPS.CurrentMapPixel.X;
            float zdif = worldMapHeight - 1 - playerGPS.CurrentMapPixel.Y;

            // world map level transform (for whole world map pixels)
            Vector3 worldMapLevelTransform;
            worldMapLevelTransform.x = xdif * scale;
            worldMapLevelTransform.y = extraTranslationY;
            worldMapLevelTransform.z = -zdif * scale;

            // used location [693,225] and [573,27] for debugging the local translation

            // local world level transform (for inter- world map pixels)
            float localTransformX = 0.0f; // old obsolete computation formula was: (float)Math.Floor((playerGPS.transform.position.x) / scale) * scale; // (float)Math.Floor((cameraPos.x) / scale) * scale;
            float localTransformZ = 0.0f; // old obsolete computation formula was: (float)Math.Floor((playerGPS.transform.position.z) / scale) * scale; // (float)Math.Floor((cameraPos.z) / scale) * scale;
            float localTransformY = 0.0f;

            localTransformX += streamingWorld.WorldCompensation.x;
            localTransformZ += streamingWorld.WorldCompensation.z;
            localTransformY += streamingWorld.WorldCompensation.y;

            float remainderX;
            if (offset.x != 0)
            {               
                remainderX = playerGPS.transform.position.x - (float)Math.Floor((playerGPS.transform.position.x) / Math.Abs(offset.x)) * Math.Abs(offset.x);
            }
            else
            {
                remainderX = playerGPS.transform.position.x;
            }

            float remainderZ;
            if (offset.z != 0)
            {
                remainderZ = playerGPS.transform.position.z - (float)Math.Floor((playerGPS.transform.position.z) / Math.Abs(offset.z)) * Math.Abs(offset.z);
            }
            else
            {
                remainderZ = playerGPS.transform.position.z;
            }
               
            //Debug.Log(string.Format("remainderX, remainderZ: {0}, {1}; playerGPS x,z: {2},{3}", remainderX, remainderZ, playerGPS.transform.position.x, playerGPS.transform.position.z));

            localTransformX += (float)Math.Floor((-streamingWorld.WorldCompensation.x + remainderX) / scale) * scale;
            localTransformZ += (float)Math.Floor((-streamingWorld.WorldCompensation.z + remainderZ) / scale) * scale;
            
            // compute composite transform and apply it to terrain object
            Vector3 finalTransform = new Vector3(worldMapLevelTransform.x + localTransformX, worldMapLevelTransform.y + localTransformY, worldMapLevelTransform.z + localTransformZ);
            terrainGameObject.gameObject.transform.localPosition = finalTransform;

            if (worldTerrainGameObject != null) // sometimes it can happen that this point is reached before worldTerrainGameObject was created, in such case we just skip
            {
                // update water height (thanks Lypyl!!!):
                Vector3 vecWaterHeight = new Vector3(0.0f, (DaggerfallUnity.Instance.TerrainSampler.OceanElevation + 1.0f) * streamingWorld.TerrainScale, 0.0f); // water height level on y-axis (+1.0f some coastlines are incorrect otherwise)
                Vector3 vecWaterHeightTransformed = worldTerrainGameObject.transform.TransformPoint(vecWaterHeight); // transform to world coordinates
                terrainMaterial.SetFloat("_WaterHeightTransformed", vecWaterHeightTransformed.y);
            
                if (gameobjectTerrainTransitionRing != null)
                {
                    if (!terrainTransitionRingUpdateRunning) // if at the moment no terrain transition ring update is still in progress
                    {
                        for (int i = 0; i < terrainTransitionRingArray.Length; i++)
                        {
                            Terrain terrain = terrainTransitionRingArray[i].terrainDesc.terrainObject.GetComponent<Terrain>();
                            Material material = terrain.materialTemplate;
                            material.SetFloat("_WaterHeightTransformed", vecWaterHeightTransformed.y);
                            terrain.materialTemplate = material;
                        }
                    }
                }
            }

            MapPixelX = playerGPS.CurrentMapPixel.X;
            MapPixelY = playerGPS.CurrentMapPixel.Y;
        }

        private void generateWorldTerrain()
        {
            // Create Unity Terrain game object
            GameObject terrainGameObject = Terrain.CreateTerrainGameObject(null);
            terrainGameObject.name = string.Format("WorldTerrain");
            
            terrainGameObject.gameObject.transform.localPosition = Vector3.zero;

            // assign terrainGameObject to layer "WorldTerrain" if available (used for rendering with secondary camera to prevent floating-point precision problems with huge clipping ranges)
            int layerExtendedTerrain = LayerMask.NameToLayer("WorldTerrain");
            if (layerExtendedTerrain != -1)
                terrainGameObject.layer = layerExtendedTerrain;

            int worldMapResolution = Math.Max(worldMapWidth, worldMapHeight);

            if (worldHeights == null)
            {
                worldHeights = new float[worldMapResolution, worldMapResolution];
            }
            
            for (int y = 0; y < worldMapHeight; y++)
            {
                for (int x = 0; x < worldMapWidth; x++)
                {
                    // get height data for this map pixel from world map and scale it to approximately match StreamingWorld's terrain heights
                    float sampleHeight = Convert.ToSingle(dfUnity.ContentReader.WoodsFileReader.GetHeightMapValue(x, y));

                    sampleHeight *= (ImprovedWorldTerrain.computeHeightMultiplier(x, y) * ImprovedTerrainSampler.baseHeightScale + ImprovedTerrainSampler.noiseMapScale);
                    
                    // make ocean elevation the lower limit
                    if (sampleHeight < ImprovedTerrainSampler.scaledOceanElevation)
                    {
                        sampleHeight = ImprovedTerrainSampler.scaledOceanElevation;
                    }

                    // normalize with TerrainHelper.maxTerrainHeight
                    worldHeights[worldMapHeight - 1 - y, x] = Mathf.Clamp01(sampleHeight / ImprovedTerrainSampler.maxTerrainHeight);
                }
            }

            // Basemap not used and is just pushed far away
            const float basemapDistance = 1000000f;

            // Ensure TerrainData is created
            Terrain terrain = terrainGameObject.GetComponent<Terrain>();
            if (terrain.terrainData == null)
            {
                // Setup terrain data
                TerrainData terrainData = new TerrainData();
                terrainData.name = "TerrainData";

                // this is not really an assignment! you tell unity terrain what resolution you want for your heightmap and it will allocate resources and take the next power of 2 increased by 1 as heightmapResolution...
                terrainData.heightmapResolution = worldMapResolution;

                float heightmapResolution = terrainData.heightmapResolution;
                // Calculate width and length of terrain in world units
                float terrainSize = ((MapsFile.WorldMapTerrainDim * MeshReader.GlobalScale) * (heightmapResolution - 1.0f));


                terrainData.size = new Vector3(terrainSize, ImprovedTerrainSampler.maxTerrainHeight, terrainSize);

                //terrainData.size = new Vector3(terrainSize, TerrainHelper.maxTerrainHeight * TerrainScale * worldMapResolution, terrainSize);
                terrainData.SetDetailResolution(worldMapResolution, 16);
                terrainData.alphamapResolution = worldMapResolution;
                terrainData.baseMapResolution = worldMapResolution;

                // Apply terrain data
                terrain.terrainData = terrainData;
                terrain.basemapDistance = basemapDistance;
            }

            terrain.heightmapPixelError = 0; // 0 ... prevent unity terrain lod approach, set to higher values to enable it
            //terrain.castShadows = true;

            // Promote heights
            Vector3 size = terrain.terrainData.size;
            terrain.terrainData.size = new Vector3(size.x, ImprovedTerrainSampler.maxTerrainHeight * streamingWorld.TerrainScale, size.z);
            terrain.terrainData.SetHeights(0, 0, worldHeights);


            // update world terrain position - do this before terrainGameObject.transform invocation, so that object2world matrix is updated with correct values
            Vector3 offset = new Vector3(0.0f, 0.0f, 0.0f);
            updatePositionWorldTerrain(ref terrainGameObject, offset);

            textureAtlasDesertSummer = dfUnity.MaterialReader.TextureReader.GetTerrainTilesetTexture(2).albedoMap;
            textureAtlasDesertSummer.filterMode = FilterMode.Point;

            textureAtlasWoodlandSummer = dfUnity.MaterialReader.TextureReader.GetTerrainTilesetTexture(302).albedoMap;
            textureAtlasWoodlandSummer.filterMode = FilterMode.Point;

            textureAtlasMountainSummer = dfUnity.MaterialReader.TextureReader.GetTerrainTilesetTexture(102).albedoMap;
            textureAtlasMountainSummer.filterMode = FilterMode.Point;

            textureAtlasSwampSummer = dfUnity.MaterialReader.TextureReader.GetTerrainTilesetTexture(402).albedoMap;
            textureAtlasSwampSummer.filterMode = FilterMode.Point;

            textureAtlasDesertWinter = dfUnity.MaterialReader.TextureReader.GetTerrainTilesetTexture(3).albedoMap;
            textureAtlasDesertWinter.filterMode = FilterMode.Point;

            textureAtlasWoodlandWinter = dfUnity.MaterialReader.TextureReader.GetTerrainTilesetTexture(303).albedoMap;
            textureAtlasWoodlandWinter.filterMode = FilterMode.Point;

            textureAtlasMountainWinter = dfUnity.MaterialReader.TextureReader.GetTerrainTilesetTexture(103).albedoMap;
            textureAtlasMountainWinter.filterMode = FilterMode.Point;

            textureAtlasSwampWinter = dfUnity.MaterialReader.TextureReader.GetTerrainTilesetTexture(403).albedoMap;
            textureAtlasSwampWinter.filterMode = FilterMode.Point;

            textureAtlasDesertRain = dfUnity.MaterialReader.TextureReader.GetTerrainTilesetTexture(4).albedoMap;
            textureAtlasDesertRain.filterMode = FilterMode.Point;

            textureAtlasWoodlandRain = dfUnity.MaterialReader.TextureReader.GetTerrainTilesetTexture(304).albedoMap;
            textureAtlasWoodlandRain.filterMode = FilterMode.Point;

            textureAtlasMountainRain = dfUnity.MaterialReader.TextureReader.GetTerrainTilesetTexture(104).albedoMap;
            textureAtlasMountainRain.filterMode = FilterMode.Point;

            textureAtlasSwampRain = dfUnity.MaterialReader.TextureReader.GetTerrainTilesetTexture(404).albedoMap;
            textureAtlasSwampRain.filterMode = FilterMode.Point;
          
            terrainMaterial = new Material(Shader.Find("Daggerfall/IncreasedTerrainTilemap"));
            terrainMaterial.name = string.Format("world terrain material");

            // Assign textures and parameters     
            terrainMaterial.SetTexture("_TileAtlasTexDesert", textureAtlasDesertSummer);
            terrainMaterial.SetTexture("_TileAtlasTexWoodland", textureAtlasWoodlandSummer);
            terrainMaterial.SetTexture("_TileAtlasTexMountain", textureAtlasMountainSummer);
            terrainMaterial.SetTexture("_TileAtlasTexSwamp", textureAtlasSwampSummer);
            //terrainMaterial.SetTexture("_FarTerrainTilemapTex", textureTerrainInfoTileMap);

            terrainMaterial.SetInt("_TextureSetSeasonCode", 0);

            updateMaterialSeasonalTextures(ref terrainMaterial, currentSeason); // change seasonal textures if necessary

            terrainMaterial.SetInt("_PlayerPosX", this.playerGPS.CurrentMapPixel.X);
            terrainMaterial.SetInt("_PlayerPosY", this.playerGPS.CurrentMapPixel.Y);

            terrainMaterial.SetInt("_TerrainDistance", streamingWorld.TerrainDistance);

            Vector3 vecWaterHeight = new Vector3(0.0f, (ImprovedTerrainSampler.scaledOceanElevation + 1.0f) * streamingWorld.TerrainScale, 0.0f); // water height level on y-axis (+1.0f some coastlines are incorrect otherwise)
            Vector3 vecWaterHeightTransformed = terrainGameObject.transform.TransformPoint(vecWaterHeight); // transform to world coordinates
            terrainMaterial.SetFloat("_WaterHeightTransformed", vecWaterHeightTransformed.y);

            terrainMaterial.SetTexture("_SkyTex", renderTextureSky);           

            setMaterialFogParameters(ref terrainMaterial);

            //terrainMaterial.SetFloat("_BlendFactor", blendFactor);
            terrainMaterial.SetFloat("_BlendStart", blendStart);
            terrainMaterial.SetFloat("_BlendEnd", blendEnd);

            #if REFLECTIONSMOD_CODE_AVAILABLE
            if (isActiveReflectionsMod)
            {
                reflectionSeaTexture = GameObject.Find("ReflectionsMod").GetComponent<ReflectionsMod.UpdateReflectionTextures>().getSeaReflectionRenderTexture();
                if (reflectionSeaTexture != null)
                {
                    terrainMaterial.EnableKeyword("ENABLE_WATER_REFLECTIONS");
                    terrainMaterial.SetTexture("_SeaReflectionTex", reflectionSeaTexture);
                    terrainMaterial.SetInt("_UseSeaReflectionTex", 1);
                }
                else
                {
                    terrainMaterial.SetInt("_UseSeaReflectionTex", 0);
                }
            }
            #else
            terrainMaterial.SetInt("_UseSeaReflectionTex", 0);
            #endif

            // Promote material
            terrain.materialType = Terrain.MaterialType.Custom;
            terrain.materialTemplate = terrainMaterial;            

            terrainGameObject.SetActive(true);

            worldTerrainGameObject = terrainGameObject;
        }

        private void updateSeasonalTexturesTerrainTransitionRingBlock(int i)
        {
            if (terrainTransitionRingArray[i].terrainDesc.terrainObject)
            {
                DaggerfallTerrain dfTerrain = terrainTransitionRingArray[i].terrainDesc.terrainObject.GetComponent<DaggerfallTerrain>();
                if (dfTerrain != null)
                {
                    dfTerrain.UpdateClimateMaterial();
                }

                Terrain terrain = terrainTransitionRingArray[i].terrainDesc.terrainObject.GetComponent<Terrain>();
                Material mat = terrain.materialTemplate;
                updateMaterialSeasonalTextures(ref mat, currentSeason);
                terrain.materialTemplate = mat;
            }
        }

        private void updateSeasonalTexturesTerrainTransitionRing()
        {
            if (terrainTransitionRingUpdateSeasonalTextures)
            {
                if (!transitionRingAllBlocksReady)
                    return;
                for (int i = 0; i < terrainTransitionRingArray.Length; i++)
                {
                    updateSeasonalTexturesTerrainTransitionRingBlock(i);
                }
                terrainTransitionRingUpdateSeasonalTextures = false;
            }
        }

        private void updateMaterialShaderPropertiesTerrainTransitionRingBlock(int i)
        {
            if (terrainTransitionRingArray[i].terrainDesc.terrainObject)
            {
                Terrain terrain = terrainTransitionRingArray[i].terrainDesc.terrainObject.GetComponent<Terrain>();
                Material oldMaterial = terrain.materialTemplate;
                Material mat = new Material(Shader.Find("Daggerfall/TransitionRingTilemap"));
                mat.CopyPropertiesFromMaterial(oldMaterial);

                Vector3 vecWaterHeight = new Vector3(0.0f, (ImprovedTerrainSampler.scaledOceanElevation + 1.0f) * streamingWorld.TerrainScale, 0.0f); // water height level on y-axis (+1.0f some coastlines are incorrect otherwise)
                Vector3 vecWaterHeightTransformed = worldTerrainGameObject.transform.TransformPoint(vecWaterHeight); // transform to world coordinates
                mat.SetFloat("_WaterHeightTransformed", vecWaterHeightTransformed.y);

                mat.SetTexture("_SkyTex", renderTextureSky);

                mat.SetFloat("_BlendStart", blendStart);
                mat.SetFloat("_BlendEnd", blendEnd);

                //DFPosition posMaxPixel = MapsFile.MapPixelToWorldCoord(playerGPS.CurrentMapPixel.X, playerGPS.CurrentMapPixel.Y);
                //float fractionalPlayerPosInBlockX = (playerGPS.WorldX - posMaxPixel.X) / (MapsFile.WorldMapTerrainDim * MeshReader.GlobalScale) * MeshReader.GlobalScale;
                //float fractionalPlayerPosInBlockY = (playerGPS.WorldZ - posMaxPixel.Y) / (MapsFile.WorldMapTerrainDim * MeshReader.GlobalScale) * MeshReader.GlobalScale;                            
                //Debug.Log(String.Format("relativePosInBlockX: {0}, relativePosInBlockY: {1}", relativePosInBlockX , relativePosInBlockY);
                mat.SetFloat("_WorldOffsetX", 0.0f);
                mat.SetFloat("_WorldOffsetY", 0.0f);

#if ENHANCED_SKY_CODE_AVAILABLE
                if (isActiveEnhancedSkyMod)
                {
                    if ((sampleFogColorFromSky == true) && (!skyMan.IsOvercast))
                    {
                        mat.SetFloat("_FogFromSkyTex", 1);
                    }
                    else
                    {
                        mat.SetFloat("_FogFromSkyTex", 0);
                    }
                }
#endif

                setMaterialFogParameters(ref mat);

                terrain.materialTemplate = mat;
            }
        }

        private void updateMaterialShaderPropertiesTerrainTransitionRing()
        {
            if (terrainTransitionRingUpdateMaterialProperties)
            {
                if (!transitionRingAllBlocksReady)
                    return;
                for (int i = 0; i < terrainTransitionRingArray.Length; i++)
                {
                   updateMaterialShaderPropertiesTerrainTransitionRingBlock(i);
                }
                terrainTransitionRingUpdateMaterialProperties = false;
            }
        }

        // Update terrain data
        private void UpdateTerrainDataTransitionRing(TransitionTerrainDesc transitionTerrainDesc)
        {
            StreamingWorld.TerrainDesc terrainDesc = transitionTerrainDesc.terrainDesc;

            // Instantiate Daggerfall terrain
            DaggerfallTerrain dfTerrain = terrainDesc.terrainObject.GetComponent<DaggerfallTerrain>();
            if (dfTerrain)
            {
                dfTerrain.TerrainScale = streamingWorld.TerrainScale;
                dfTerrain.MapPixelX = terrainDesc.mapPixelX;
                dfTerrain.MapPixelY = terrainDesc.mapPixelY;
                dfTerrain.InstantiateTerrain();
            }

            // Update data for terrain
            //UpdateMapPixelData(ref dfTerrain, terrainDesc.mapPixelX, terrainDesc.mapPixelY, streamingWorld.TerrainTexturing);
            dfTerrain.UpdateMapPixelData(streamingWorld.TerrainTexturing);

            // Update heights of transition terrain ring
            float weightFarTerrainLeft = 0.0f;
            float weightFarTerrainRight = 0.0f;
            float weightFarTerrainTop = 0.0f;
            float weightFarTerrainBottom = 0.0f;
            if (transitionTerrainDesc.transitionRingBorderDesc.isLeftRingBorder) weightFarTerrainLeft = 1.0f;
            if (transitionTerrainDesc.transitionRingBorderDesc.isRightRingBorder) weightFarTerrainRight = 1.0f;
            if (transitionTerrainDesc.transitionRingBorderDesc.isTopRingBorder) weightFarTerrainTop = 1.0f;
            if (transitionTerrainDesc.transitionRingBorderDesc.isBottomRingBorder) weightFarTerrainBottom = 1.0f;
            float heightFarTerrainTopLeft = worldHeights[worldMapHeight - 1 - terrainDesc.mapPixelY, terrainDesc.mapPixelX - 1]; // TODO: map border handling
            float heightFarTerrainTopRight = worldHeights[worldMapHeight - 1 - terrainDesc.mapPixelY, terrainDesc.mapPixelX]; // TODO: map border handling
            float heightFarTerrainBottomLeft = worldHeights[worldMapHeight - 1 - terrainDesc.mapPixelY + 1, terrainDesc.mapPixelX - 1]; // TODO: map border handling
            float heightFarTerrainBottomRight = worldHeights[worldMapHeight - 1 - terrainDesc.mapPixelY + 1, terrainDesc.mapPixelX]; // TODO: map border handling

            //Debug.Log(String.Format("heightFarTerrainTopLeft: {0}, heightFarTerrainTopRight: {1}, heightFarTerrainBottomLeft: {0}, heightFarTerrainBottomRight: {1}", heightFarTerrainTopLeft, heightFarTerrainTopRight, heightFarTerrainBottomLeft, heightFarTerrainBottomRight));

            //Terrain terrain = terrainDesc.terrainObject.GetComponent<Terrain>();
            int heightmapHeight = dfTerrain.MapData.heightmapSamples.GetLength(0);
            int heightmapWidth = dfTerrain.MapData.heightmapSamples.GetLength(1);
            for (int y = 0; y < heightmapHeight; y++)
            {
                float fractionalAmountY = (float)y / ((float)heightmapHeight - 1);
                for (int x = 0; x < heightmapWidth; x++)
                {
                    float fractionalAmountX = (float)x / ((float)heightmapWidth - 1);
                    float weightFarTerrainX = weightFarTerrainLeft * (1.0f - fractionalAmountX) + weightFarTerrainRight * (fractionalAmountX);
                    float weightFarTerrainY = weightFarTerrainTop * (1.0f - fractionalAmountY) + weightFarTerrainBottom * (fractionalAmountY);
                    float weightFarTerrainCombined = Math.Max(weightFarTerrainX, weightFarTerrainY);
                    float heightFarTerrain = heightFarTerrainTopLeft * (1.0f - fractionalAmountX) * (1.0f - fractionalAmountY) +
                                             heightFarTerrainTopRight * (fractionalAmountX) * (1.0f - fractionalAmountY) +
                                             heightFarTerrainBottomLeft * (1.0f - fractionalAmountX) * (fractionalAmountY) +
                                             heightFarTerrainBottomRight * (fractionalAmountX) * (fractionalAmountY);
                    dfTerrain.MapData.heightmapSamples[y, x] = dfTerrain.MapData.heightmapSamples[y, x] * (1.0f - weightFarTerrainCombined) + heightFarTerrain * (weightFarTerrainCombined);
                }
            }

            dfTerrain.UpdateTileMapData();

            // Promote data to live terrain
            dfTerrain.UpdateClimateMaterial();
            dfTerrain.PromoteTerrainData();

            // inject transition ring shader
            Terrain terrain = transitionTerrainDesc.terrainDesc.terrainObject.GetComponent<Terrain>();
            Material oldMaterial = terrain.materialTemplate;
            Material newMaterial = new Material(Shader.Find("Daggerfall/TransitionRingTilemap"));
            newMaterial.CopyPropertiesFromMaterial(oldMaterial);
            newMaterial.mainTexture = oldMaterial.mainTexture;
            newMaterial.mainTextureOffset = oldMaterial.mainTextureOffset;
            newMaterial.mainTextureScale = oldMaterial.mainTextureScale;            

            // Assign textures and parameters
            newMaterial.SetTexture("_FarTerrainTilemapTex", textureTerrainInfoTileMap);
            newMaterial.SetInt("_FarTerrainTilemapDim", terrainInfoTileMapDim);

            newMaterial.SetTexture("_TileAtlasTexDesert", textureAtlasDesertSummer);
            newMaterial.SetTexture("_TileAtlasTexWoodland", textureAtlasWoodlandSummer);
            newMaterial.SetTexture("_TileAtlasTexMountain", textureAtlasMountainSummer);
            newMaterial.SetTexture("_TileAtlasTexSwamp", textureAtlasSwampSummer);

            newMaterial.SetFloat("_blendWeightFarTerrainTop", weightFarTerrainTop);
            newMaterial.SetFloat("_blendWeightFarTerrainBottom", weightFarTerrainBottom);
            newMaterial.SetFloat("_blendWeightFarTerrainLeft", weightFarTerrainLeft);
            newMaterial.SetFloat("_blendWeightFarTerrainRight", weightFarTerrainRight);

            //newMaterial.SetInt("_TextureSetSeasonCode", 0);

            updateMaterialSeasonalTextures(ref newMaterial, currentSeason); // change seasonal textures if necessary

            newMaterial.SetInt("_MapPixelX", dfTerrain.MapPixelX);
            newMaterial.SetInt("_MapPixelY", dfTerrain.MapPixelY);

            newMaterial.SetInt("_PlayerPosX", this.playerGPS.CurrentMapPixel.X);
            newMaterial.SetInt("_PlayerPosY", this.playerGPS.CurrentMapPixel.Y);

            newMaterial.SetInt("_TerrainDistance", streamingWorld.TerrainDistance); // - 1); // -1... allow the outer ring of of detailed terrain to intersect with far terrain (to prevent some holes)

            Vector3 vecWaterHeight = new Vector3(0.0f, (ImprovedTerrainSampler.scaledOceanElevation + 1.0f) * streamingWorld.TerrainScale, 0.0f); // water height level on y-axis (+1.0f some coastlines are incorrect otherwise)
            Vector3 vecWaterHeightTransformed = worldTerrainGameObject.transform.TransformPoint(vecWaterHeight); // transform to world coordinates
            newMaterial.SetFloat("_WaterHeightTransformed", vecWaterHeightTransformed.y);

            newMaterial.SetTexture("_SkyTex", renderTextureSky);

            newMaterial.SetInt("_FogFromSkyTex", 0);
            #if ENHANCED_SKY_CODE_AVAILABLE
                if (isActiveEnhancedSkyMod)
                {
                    if ((sampleFogColorFromSky == true) && (!skyMan.IsOvercast))
                    {
                        newMaterial.SetFloat("_FogFromSkyTex", 1);
                    }
                    else
                    {
                        newMaterial.SetFloat("_FogFromSkyTex", 0);
                    }
                }
            #endif

            setMaterialFogParameters(ref newMaterial);            

            //terrainMaterial.SetFloat("_BlendFactor", blendFactor);
            newMaterial.SetFloat("_BlendStart", blendStart);
            newMaterial.SetFloat("_BlendEnd", blendEnd);

#if REFLECTIONSMOD_CODE_AVAILABLE
            if (isActiveReflectionsMod)
            {
                reflectionSeaTexture = GameObject.Find("ReflectionsMod").GetComponent<ReflectionsMod.UpdateReflectionTextures>().getSeaReflectionRenderTexture();
                if (reflectionSeaTexture != null)
                {
                    newMaterial.EnableKeyword("ENABLE_WATER_REFLECTIONS");
                    newMaterial.SetTexture("_SeaReflectionTex", reflectionSeaTexture);
                    if (tileAtlasReflectiveTexture != null)
                    {
                        newMaterial.SetTexture("_TileAtlasReflectiveTex", tileAtlasReflectiveTexture);
                    }
                    newMaterial.SetInt("_UseSeaReflectionTex", 1);
                }
                else
                {
                    newMaterial.SetInt("_UseSeaReflectionTex", 0);
                }
            }
#else
            terrainMaterial.SetInt("_UseSeaReflectionTex", 0);
#endif

            terrain.materialType = Terrain.MaterialType.Custom;
            terrain.materialTemplate = newMaterial;            
            dfTerrain.TerrainMaterial = terrain.materialTemplate; // important so that we can later call DaggerfallTerrain.UpdateClimateMaterial and it will update the correct reference

            // Only set active again once complete
            terrainDesc.terrainObject.SetActive(true);
            terrainDesc.terrainObject.name = streamingWorld.GetTerrainName(dfTerrain.MapPixelX, dfTerrain.MapPixelY);
        }

        private bool CreateTerrain(int mapPixelX, int mapPixelY, int indexTerrain)
        {
            // Do nothing if out of range
            if (mapPixelX < MapsFile.MinMapPixelX || mapPixelX >= MapsFile.MaxMapPixelX ||
                mapPixelY < MapsFile.MinMapPixelY || mapPixelY >= MapsFile.MaxMapPixelY)
            {
                return false;
            }

            // Get terrain key
            int key = TerrainHelper.MakeTerrainKey(mapPixelX, mapPixelY);

            // Setup new terrain
            terrainTransitionRingArray[indexTerrain].terrainDesc.active = true;
            terrainTransitionRingArray[indexTerrain].terrainDesc.updateData = true;
            terrainTransitionRingArray[indexTerrain].terrainDesc.updateNature = true;
            terrainTransitionRingArray[indexTerrain].terrainDesc.mapPixelX = mapPixelX;
            terrainTransitionRingArray[indexTerrain].terrainDesc.mapPixelY = mapPixelY;
            if (!terrainTransitionRingArray[indexTerrain].terrainDesc.terrainObject)
            {
                // Create game objects for new terrain
                streamingWorld.CreateTerrainGameObjects(
                    mapPixelX,
                    mapPixelY,
                    out terrainTransitionRingArray[indexTerrain].terrainDesc.terrainObject,
                    out terrainTransitionRingArray[indexTerrain].terrainDesc.billboardBatchObject);
            }

            // Add new terrain index to transition ring dictionary
            terrainTransitionRingIndexDict.Add(key, indexTerrain);

            terrainTransitionRingArray[indexTerrain].terrainDesc.terrainObject.transform.SetParent(gameobjectTerrainTransitionRing.transform);

            return true;
        }

        private void PlaceTerrainOfTransitionRing(int terrainIndex)
        {
            // Apply local transform
            int mapPixelX = terrainTransitionRingArray[terrainIndex].terrainDesc.mapPixelX;
            int mapPixelY = terrainTransitionRingArray[terrainIndex].terrainDesc.mapPixelY;
            float scale = MapsFile.WorldMapTerrainDim * MeshReader.GlobalScale;
            int xdif = mapPixelX - streamingWorld.LocalPlayerGPS.CurrentMapPixel.X;
            int ydif = mapPixelY - streamingWorld.LocalPlayerGPS.CurrentMapPixel.Y;
            //Debug.Log(String.Format("world-compensation: x:{0}, y: {1}, z: {2}", streamingWorld.WorldCompensation.x, streamingWorld.WorldCompensation.y, streamingWorld.WorldCompensation.z));
            Vector3 localPosition = new Vector3(xdif * scale, 0, -ydif * scale); // +streamingWorld.WorldCompensation;
            terrainTransitionRingArray[terrainIndex].terrainDesc.terrainObject.transform.localPosition = localPosition;
            
            // if block was not reused - it does not exist - so create unity terrain object - otherwise position update will be enough
            if (!terrainTransitionRingArray[terrainIndex].keepThisBlock)
            {
                UpdateTerrainDataTransitionRing(terrainTransitionRingArray[terrainIndex]);
            }
            else
            {
                // activate unity terrain now since it has been updated
                terrainTransitionRingArray[terrainIndex].terrainDesc.terrainObject.SetActive(true);
                terrainTransitionRingArray[terrainIndex].terrainDesc.billboardBatchObject.SetActive(true);
            }
        }

        private TransitionRingBorderDesc getTransitionRingBorderDesc(int x, int y, int distanceTransitionRingFromCenterX, int distanceTransitionRingFromCenterY)
        {
            TransitionRingBorderDesc transitionRingBorderDesc;
            transitionRingBorderDesc.isLeftRingBorder = false;
            transitionRingBorderDesc.isRightRingBorder = false;
            transitionRingBorderDesc.isTopRingBorder = false;
            transitionRingBorderDesc.isBottomRingBorder = false;
            if (x == -distanceTransitionRingFromCenterX)
            {
                transitionRingBorderDesc.isLeftRingBorder = true;
            }
            if (x == +distanceTransitionRingFromCenterX)
            {
                transitionRingBorderDesc.isRightRingBorder = true;
            }
            if (y == -distanceTransitionRingFromCenterY)
            {
                transitionRingBorderDesc.isBottomRingBorder = true;
            }
            if (y == +distanceTransitionRingFromCenterY)
            {
                transitionRingBorderDesc.isTopRingBorder = true;
            }
            return transitionRingBorderDesc;
        }

        private Terrain GetTerrainTransitionRing(int mapPixelX, int mapPixelY)
        {
            int key = TerrainHelper.MakeTerrainKey(mapPixelX, mapPixelY);
            if (terrainTransitionRingIndexDict.ContainsKey(key))
            {
                return terrainTransitionRingArray[terrainTransitionRingIndexDict[key]].terrainDesc.terrainObject.GetComponent<Terrain>();
            }

            return null;
        }

        private void UpdateNeighboursTransitionRing()
        {
            for (int i = 0; i < terrainTransitionRingArray.Length; i++)
            {
                // Check object exists
                if (!terrainTransitionRingArray[i].terrainDesc.terrainObject)
                    continue;

                // Get DaggerfallTerrain
                DaggerfallTerrain dfTerrain = terrainTransitionRingArray[i].terrainDesc.terrainObject.GetComponent<DaggerfallTerrain>();
                if (!dfTerrain)
                    continue;

                // Set or clear neighbours
                if (terrainTransitionRingArray[i].terrainDesc.active)
                {
                    dfTerrain.LeftNeighbour = GetTerrainTransitionRing(dfTerrain.MapPixelX - 1, dfTerrain.MapPixelY);
                    dfTerrain.RightNeighbour = GetTerrainTransitionRing(dfTerrain.MapPixelX + 1, dfTerrain.MapPixelY);
                    dfTerrain.TopNeighbour = GetTerrainTransitionRing(dfTerrain.MapPixelX, dfTerrain.MapPixelY - 1);
                    dfTerrain.BottomNeighbour = GetTerrainTransitionRing(dfTerrain.MapPixelX, dfTerrain.MapPixelY + 1);
                    //    if (dfTerrain.LeftNeighbour == null)
                    //    {
                    //        dfTerrain.LeftNeighbour = streamingWorld.GetTerrainFromPixel(dfTerrain.MapPixelX - 1, dfTerrain.MapPixelY).GetComponent<Terrain>();
                    //        if (dfTerrain.LeftNeighbour != null) Debug.Log("worked");
                    //    }
                    //    if (dfTerrain.RightNeighbour == null)
                    //    {
                    //        dfTerrain.RightNeighbour = streamingWorld.GetTerrainFromPixel(dfTerrain.MapPixelX + 1, dfTerrain.MapPixelY).GetComponent<Terrain>();
                    //        if (dfTerrain.RightNeighbour != null) Debug.Log("worked");
                    //    }
                    //    if (dfTerrain.TopNeighbour == null)
                    //    {
                    //        dfTerrain.TopNeighbour = streamingWorld.GetTerrainFromPixel(dfTerrain.MapPixelX, dfTerrain.MapPixelY - 1).GetComponent<Terrain>();
                    //        if (dfTerrain.TopNeighbour != null) Debug.Log("worked");
                    //    }
                    //    if (dfTerrain.BottomNeighbour == null)
                    //    {
                    //        dfTerrain.BottomNeighbour = streamingWorld.GetTerrainFromPixel(dfTerrain.MapPixelX, dfTerrain.MapPixelY + 1).GetComponent<Terrain>();
                    //        if (dfTerrain.BottomNeighbour != null) Debug.Log("worked");
                    //    }
                    dfTerrain.UpdateNeighbours();
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

        private IEnumerator UpdateTerrainsTransitionRing()
        {
            for (int i = 0; i < terrainTransitionRingArray.Length; i++)
            {
                if (terrainTransitionRingArray[i].terrainDesc.active)
                {

                    if (terrainTransitionRingArray[i].terrainDesc.updateData)
                    {
                        PlaceTerrainOfTransitionRing(i);
                        //UpdateTerrainData(terrainTransitionRingArray[i]);
                        terrainTransitionRingArray[i].terrainDesc.updateData = false;

                        yield return new WaitForEndOfFrame();
                    }
                    
                    if (terrainTransitionRingArray[i].terrainDesc.updateNature)
                    {
                        streamingWorld.UpdateTerrainNature(terrainTransitionRingArray[i].terrainDesc);
                        terrainTransitionRingArray[i].terrainDesc.updateNature = false;

                        MeshRenderer meshRenderer = terrainTransitionRingArray[i].terrainDesc.billboardBatchObject.GetComponent<MeshRenderer>();
                        Material[] rendererMaterials = meshRenderer.materials;
                        for (int m = 0; m < rendererMaterials.Length; m++)
                        {
                            Material newMaterial = new Material(Shader.Find("Daggerfall/BillboardBatchFaded"));
                            newMaterial.CopyPropertiesFromMaterial(rendererMaterials[m]);
                            newMaterial.SetInt("_TerrainDistance", streamingWorld.TerrainDistance);
                            newMaterial.SetFloat("_TerrainBlockSize", (MapsFile.WorldMapTerrainDim * MeshReader.GlobalScale));
                            rendererMaterials[m] = newMaterial;
                        }
                        meshRenderer.materials = rendererMaterials;

                        //if (!terrainTransitionRingUpdateRunning) //(!transitionRingUpdateFinished)
                        yield return new WaitForEndOfFrame();
                    }

                    terrainTransitionRingArray[i].ready = true;
                }
            }

            UpdateNeighboursTransitionRing();

            terrainMaterial.SetInt("_TerrainDistance", streamingWorld.TerrainDistance); // after of transition ring update - restore far terrain rendering to streamingWorld.TerrainDistance

            terrainTransitionRingUpdateRunning = false;
            transitionRingAllBlocksReady = true;
        }

        private void generateTerrainTransitionRing()
        {
            if (gameobjectTerrainTransitionRing == null)
            {
                gameobjectTerrainTransitionRing = new GameObject("TerrainTransitionRing");
                gameobjectTerrainTransitionRing.transform.SetParent(GameManager.Instance.ExteriorParent.transform);
            }

            //// this is not perfect I know - but since events can get in asynchronous and may trigger an update and thus an invocation to generateTerrainTransitionRing() it is important that no update is currently performed
            //while (transitionRingAllBlocksReady == true)
            //{

            //}
            transitionRingAllBlocksReady = false;
            terrainTransitionRingUpdateRunning = true;

            terrainMaterial.SetInt("_TerrainDistance", streamingWorld.TerrainDistance-2); // for time of transition ring update - change far terrain to be rendered to streamingWorld.TerrainDistance-2

            int distanceTransitionRingFromCenterX = (streamingWorld.TerrainDistance + 1);
            int distanceTransitionRingFromCenterY = (streamingWorld.TerrainDistance + 1);

            // initially mark all blocks as potential blocks to remove - terrain blocks in terrain transition ring that can be reused will be updated next
            for (int i = 0; i < terrainTransitionRingArray.Length; i++)
            {
                terrainTransitionRingArray[i].keepThisBlock = false;
            }

            // mark blocks that will be reused
            for (int y = -distanceTransitionRingFromCenterY; y <= distanceTransitionRingFromCenterY; y++)
            {
                for (int x = -distanceTransitionRingFromCenterX; x <= distanceTransitionRingFromCenterX; x++)
                {
                    if ((Math.Abs(x) == distanceTransitionRingFromCenterX) || (Math.Abs(y) == distanceTransitionRingFromCenterY))
                    {
                        int mapPixelX = playerGPS.CurrentMapPixel.X + x;
                        int mapPixelY = playerGPS.CurrentMapPixel.Y + y;
                        int key = TerrainHelper.MakeTerrainKey(mapPixelX, mapPixelY);
                        if (terrainTransitionRingIndexDict.ContainsKey(key)) // if desired terrain block already exists in transition ring
                        {
                            int indexFound = terrainTransitionRingIndexDict[key]; // get index of existing terrain block of interest
                            TransitionRingBorderDesc borderDesc = getTransitionRingBorderDesc(x, y, distanceTransitionRingFromCenterX, distanceTransitionRingFromCenterY);
                            // if transition ring border description has not changed since last time - the block's heights can be reused - so the block can be reused
                            if ((borderDesc.isLeftRingBorder == terrainTransitionRingArray[indexFound].transitionRingBorderDesc.isLeftRingBorder) &&
                                (borderDesc.isRightRingBorder == terrainTransitionRingArray[indexFound].transitionRingBorderDesc.isRightRingBorder) &&
                                (borderDesc.isTopRingBorder == terrainTransitionRingArray[indexFound].transitionRingBorderDesc.isTopRingBorder) &&
                                (borderDesc.isBottomRingBorder == terrainTransitionRingArray[indexFound].transitionRingBorderDesc.isBottomRingBorder))
                            {
                                terrainTransitionRingArray[indexFound].keepThisBlock = true; // mark block that it will be reused 
                            }
                        }
                    }
                }
            }

            // remove unused terrain blocks from terrainTransitionRingArray (and its key from terrainTransitionRingIndexDict)
            for (int i = 0; i < terrainTransitionRingArray.Length; i++)
            {               
                if (terrainTransitionRingArray[i].terrainDesc.terrainObject)
                {
                    // deactivate unity terrain (until it is recreated or until it is updated)
                    terrainTransitionRingArray[i].terrainDesc.terrainObject.SetActive(false);
                    terrainTransitionRingArray[i].terrainDesc.billboardBatchObject.SetActive(false);
                }

                if (!terrainTransitionRingArray[i].keepThisBlock)
                {
                    // get key for terrain block
                    int key = TerrainHelper.MakeTerrainKey(terrainTransitionRingArray[i].terrainDesc.mapPixelX, terrainTransitionRingArray[i].terrainDesc.mapPixelY);
                    terrainTransitionRingIndexDict.Remove(key); // remove it

                    // now destroy unity terrain object
                    GameObject.Destroy(terrainTransitionRingArray[i].terrainDesc.terrainObject);
                    terrainTransitionRingArray[i].terrainDesc.terrainObject = null;
                    terrainTransitionRingArray[i].terrainDesc.active = false;
                    terrainTransitionRingArray[i].ready = false;                    
                }
                else
                {
                    // mark terrain block for data update (position, ...)
                    terrainTransitionRingArray[i].terrainDesc.updateData = true;
                    terrainTransitionRingArray[i].terrainDesc.updateNature = true;
                }
            }
            
            int terrainIndex = 0;
            for (int y = -distanceTransitionRingFromCenterY; y <= distanceTransitionRingFromCenterY; y++)
            {
                for (int x = -distanceTransitionRingFromCenterX; x <= distanceTransitionRingFromCenterX; x++)
                {
                    if ((Math.Abs(x) == distanceTransitionRingFromCenterX) || (Math.Abs(y) == distanceTransitionRingFromCenterY))
                    {
                        int mapPixelX = playerGPS.CurrentMapPixel.X + x;
                        int mapPixelY = playerGPS.CurrentMapPixel.Y + y;

                        // get key for terrain block
                        int key = TerrainHelper.MakeTerrainKey(mapPixelX, mapPixelY);
                        // if desired terrain block already exists in transition ring
                        if (terrainTransitionRingIndexDict.ContainsKey(key))
                        {
                            continue; // do nothing
                        }

                        // if desired terrain block does not exist in transition ring - go on here

                        // go to next free block in terrainTransitionRingArray
                        while (terrainTransitionRingArray[terrainIndex].keepThisBlock)
                        {
                            terrainIndex++;
                            if (terrainIndex >= terrainTransitionRingArray.Length)
                            {
                                throw new Exception("generateTerrainTransitionRing: Could not find free terrain block. This should not happen!");
                            }
                        }

                        bool successCreate = CreateTerrain(mapPixelX, mapPixelY, terrainIndex);
                        if (successCreate)
                        {
                            terrainTransitionRingArray[terrainIndex].transitionRingBorderDesc = getTransitionRingBorderDesc(x, y, distanceTransitionRingFromCenterX, distanceTransitionRingFromCenterY);
                            terrainTransitionRingArray[terrainIndex].heightsUpdatePending = true;
                            terrainIndex++;
                        }
                    }
                }
            }

            StartCoroutine(UpdateTerrainsTransitionRing());
        }

        #endregion

        #region Startup/Shutdown Methods

        private bool ReadyCheck()
        {
            if (isReady)
                return true;

            if (dfUnity == null)
            {
                dfUnity = DaggerfallUnity.Instance;
            }

            // Do nothing if DaggerfallUnity not ready
            if (!dfUnity.IsReady)
            {
                DaggerfallUnity.LogMessage("ExtendedTerrainDistance: DaggerfallUnity component is not ready. Have you set your Arena2 path?");
                return false;
            }

            // Raise ready flag
            isReady = true;

            return true;
        }

        #endregion
    }
}