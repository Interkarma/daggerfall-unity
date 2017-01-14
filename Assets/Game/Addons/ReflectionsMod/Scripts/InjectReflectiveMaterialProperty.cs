//ReflectionsMod for Daggerfall Tools For Unity
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: Michael Rauter (a.k.a. Nystul)
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)

using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Utility;
using IniParser;

namespace ReflectionsMod
{
    public class InjectReflectiveMaterialProperty : MonoBehaviour
    {
        public string iniPathConfigInjectionTextures = ""; // if not specified will use fallback ini-file
        const string iniPathFallbackConfigInjectionTextures = "configInjectionTextures.ini";

        private bool useDeferredReflections = true;

        // Streaming World Component
        public StreamingWorld streamingWorld;

        private Texture texReflectionGround = null;
        private Texture texReflectionLowerLevel = null;
        private bool playerInside = false;
        private enum InsideSpecification { Building, DungeonOrCastle, Unknown };
        InsideSpecification whereInside = InsideSpecification.Unknown;
        private GameObject gameObjectInterior = null;
        private GameObject gameObjectDungeon = null;

        private GameObject gameObjectReflectionPlaneGroundLevel = null;
        private GameObject gameObjectReflectionPlaneSeaLevel = null;
        private GameObject gameObjectReflectionPlaneLowerLevel = null;

        private float extraTranslationY = 0.0f;

        private GameObject gameObjectStreamingTarget = null;

        private DaggerfallUnity dfUnity;

        private struct TextureRecord
        {
            public int archive;
            public int record;
            public int frame;
            public bool useMetallicGlossMap;
            public Texture2D metallicGlossMap;
            public float reflectivity;
            public float smoothness;
            public Texture2D albedoMap;
            public Texture2D normalMap;
        }

        IniParser.FileIniDataParser iniParser = new FileIniDataParser();
        IniParser.Model.IniData iniData;

        IniParser.Model.IniData getIniParserConfigInjectionTextures()
        {
            IniParser.Model.IniData parsedIniData = null;

            // Attempt to load configInjectionTextures.ini
            string userIniPathConfigInjectionTextures = Path.Combine(Application.dataPath, iniPathConfigInjectionTextures);
            if (File.Exists(userIniPathConfigInjectionTextures))
            {
                parsedIniData = iniParser.ReadFile(userIniPathConfigInjectionTextures);
            }

            // Load fallback configInjectionTextures .ini
            TextAsset asset = Resources.Load<TextAsset>(iniPathFallbackConfigInjectionTextures);
            if (asset != null)
            {
                MemoryStream stream = new MemoryStream(asset.bytes);
                StreamReader reader = new StreamReader(stream);
                parsedIniData = iniParser.ReadData(reader);
                reader.Close();
            }
            return parsedIniData;
        }

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
            StreamingWorld.OnInitWorld += InjectMaterialProperties;
            StreamingWorld.OnTeleportToCoordinates += InjectMaterialProperties;
            FloatingOrigin.OnPositionUpdate += InjectMaterialProperties;
            DaggerfallTerrain.OnInstantiateTerrain += InjectMaterialProperties;
        }

        void OnDestroy()
        {
            StreamingWorld.OnInitWorld -= InjectMaterialProperties;
            StreamingWorld.OnTeleportToCoordinates -= InjectMaterialProperties;
            FloatingOrigin.OnPositionUpdate -= InjectMaterialProperties;
            DaggerfallTerrain.OnInstantiateTerrain -= InjectMaterialProperties;
        }

        void Start()
        {
            dfUnity = DaggerfallUnity.Instance;

            useDeferredReflections = (GameManager.Instance.MainCamera.renderingPath == RenderingPath.DeferredShading);

            if (!streamingWorld)
                streamingWorld = GameObject.Find("StreamingWorld").GetComponent<StreamingWorld>();
            if (!streamingWorld)
            {
                DaggerfallUnity.LogMessage("InjectReflectiveMaterialProperty: Missing StreamingWorld reference.", true);
                if (Application.isEditor)
                    Debug.Break();
                else
                    Application.Quit();
            }

            if (GameObject.Find("IncreasedTerrainDistanceMod") != null)
            {
                if (DaggerfallUnity.Settings.Nystul_IncreasedTerrainDistance)
                {
                    extraTranslationY = GameObject.Find("IncreasedTerrainDistanceMod").GetComponent<ProjectIncreasedTerrainDistance.IncreasedTerrainDistance>().ExtraTranslationY;
                }
            }

            gameObjectReflectionPlaneGroundLevel = GameObject.Find("ReflectionPlaneBottom");
            gameObjectReflectionPlaneSeaLevel = GameObject.Find("ReflectionPlaneSeaLevel");
            gameObjectReflectionPlaneLowerLevel = gameObjectReflectionPlaneSeaLevel;

            // get inactive gameobject StreamingTarget (just GameObject.Find() would fail to find inactive gameobjects)
            GameObject[] gameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject currentGameObject in gameObjects)
            {
                string objectPathInHierarchy = GetGameObjectPath(currentGameObject);
                if (objectPathInHierarchy == "/Exterior/StreamingTarget")
                {
                    gameObjectStreamingTarget = currentGameObject;
                }
            }

            iniData = getIniParserConfigInjectionTextures();
        }

        void Update()
        {
            // I am not super happy with doing this in the update function, but found no other way to make starting in dungeon correctly injecting material properties
            if ((texReflectionGround) && (texReflectionLowerLevel)) // do not change playerInside state before the reflection textures are initialized
            {
                // mechanism implemented according to Interkarma's suggestions
                // transition: inside -> dungeon/castle/building
                if (GameManager.Instance.PlayerEnterExit.IsPlayerInside && !playerInside)
                {
                    playerInside = true; // player now inside

                    // do other stuff when player first inside                    
                    if (GameManager.Instance.IsPlayerInsideBuilding)
                    {
                        gameObjectInterior = GameObject.Find("Interior");
                        whereInside = InsideSpecification.Building;
                    }
                    else if ((GameManager.Instance.IsPlayerInsideDungeon) || (GameManager.Instance.IsPlayerInsideCastle))
                    {
                        gameObjectDungeon = GameObject.Find("Dungeon");
                        whereInside = InsideSpecification.DungeonOrCastle;
                    }

                    InjectMaterialPropertiesIndoor();
                }
                // transition: dungeon/castle/building -> outside
                else if (!GameManager.Instance.PlayerEnterExit.IsPlayerInside && playerInside)
                {
                    playerInside = false; // player no longer inside

                    // do other stuff when player first not inside
                    gameObjectInterior = null;
                    gameObjectDungeon = null;
                    InjectMaterialPropertiesOutdoor();
                    whereInside = InsideSpecification.Unknown;
                }

                // transition: dungeon/castle -> building
                if ((GameManager.Instance.IsPlayerInsideBuilding) && (whereInside == InsideSpecification.DungeonOrCastle))
                {
                    gameObjectInterior = GameObject.Find("Interior");
                    gameObjectDungeon = null;
                    InjectMaterialPropertiesIndoor();
                    //injectIndoor = true;
                    whereInside = InsideSpecification.Building;
                }
                // transition: building -> dungeon/castle
                else if (((GameManager.Instance.IsPlayerInsideDungeon) || (GameManager.Instance.IsPlayerInsideCastle)) && (whereInside == InsideSpecification.Building))
                {
                    gameObjectDungeon = GameObject.Find("Dungeon");
                    gameObjectInterior = null;
                    InjectMaterialPropertiesIndoor();
                    whereInside = InsideSpecification.DungeonOrCastle;
                }
            }
            else
            {
                texReflectionGround = gameObjectReflectionPlaneGroundLevel.GetComponent<MirrorReflection>().m_ReflectionTexture;
                texReflectionLowerLevel = gameObjectReflectionPlaneSeaLevel.GetComponent<MirrorReflection>().m_ReflectionTexture;
            }
        }

        public void OnWillRenderObject()
        {
            if (!GameManager.Instance.IsPlayerInside)
            {
                if (!gameObjectStreamingTarget)
                    return;

                foreach (Transform child in gameObjectStreamingTarget.transform)
                {
                    DaggerfallTerrain dfTerrain = child.GetComponent<DaggerfallTerrain>();
                    if (!dfTerrain)
                        continue;

                    Terrain terrain = child.GetComponent<Terrain>();
                    if (terrain)
                    {
                        if (terrain.materialTemplate)
                        {
                            if (terrain.materialTemplate.shader.name == "Daggerfall/ReflectionsMod/TilemapWithReflections")
                            {
                                terrain.materialTemplate.SetFloat("_GroundLevelHeight", gameObjectReflectionPlaneGroundLevel.transform.position.y - extraTranslationY);
                                terrain.materialTemplate.SetFloat("_SeaLevelHeight", gameObjectReflectionPlaneSeaLevel.transform.position.y - extraTranslationY);
                            }
                        }
                    }
                }
            }
            else if (GameManager.Instance.IsPlayerInside)
            {
                Renderer[] renderers = null;
                // renderers must be aquired here and not in Update() because it seems that this function's execution can happen in parallel to Update() - so a concurrent conflict can occur (and does)
                if (gameObjectInterior != null)
                {
                    renderers = gameObjectInterior.GetComponentsInChildren<Renderer>();
                }
                else if (gameObjectDungeon != null)
                {
                    renderers = gameObjectDungeon.GetComponentsInChildren<Renderer>();
                }

                //Debug.Log(String.Format("renderers: {0}", renderers.Length));

                if (renderers != null)
                {
                    foreach (Renderer r in renderers)
                    {
                        Material[] mats = r.sharedMaterials;
                        foreach (Material m in mats)
                        {
                            //if (m.shader.name == "Daggerfall/ReflectionsMod/FloorMaterialWithReflections")
                            {
                                m.SetFloat("_GroundLevelHeight", gameObjectReflectionPlaneGroundLevel.transform.position.y);
                                m.SetFloat("_LowerLevelHeight", gameObjectReflectionPlaneLowerLevel.transform.position.y);
                            }
                        }
                        r.sharedMaterials = mats;
                    }
                }
            }
        }

        void InjectMaterialPropertiesIndoor()
        {
            List<TextureRecord> listInjectedTextures = new List<TextureRecord>();

            // mages guild 4 floors debuging worldpos: 704,337
            IniParser.Model.IniData textureInjectionData = iniData;
            if (iniData != null)
            {
                foreach (IniParser.Model.SectionData section in iniData.Sections)
                {
                    int textureArchive = int.Parse(textureInjectionData[section.SectionName]["textureArchive"]);
                    int textureRecord = int.Parse(textureInjectionData[section.SectionName]["textureRecord"]);
                    int textureFrame = int.Parse(textureInjectionData[section.SectionName]["textureFrame"]);

                    TextureRecord texRecord = new TextureRecord();
                    texRecord.archive = textureArchive;
                    texRecord.record = textureRecord;
                    texRecord.frame = textureFrame;
                    texRecord.albedoMap = null;
                    texRecord.normalMap = null;
                    texRecord.useMetallicGlossMap = false;
                    texRecord.metallicGlossMap = null;
                    texRecord.reflectivity = 0.0f;
                    texRecord.smoothness = 0.0f;

                    Texture2D albedoTexture = null;
                    if (textureInjectionData[section.SectionName].ContainsKey("filenameAlbedoMap"))
                    {
                        string fileAlbedoMap = textureInjectionData[section.SectionName]["filenameAlbedoMap"];
                        albedoTexture = Resources.Load(fileAlbedoMap) as Texture2D;
                        texRecord.albedoMap = albedoTexture;
                    }

                    Texture2D normalTexture = null;
                    if (textureInjectionData[section.SectionName].ContainsKey("filenameNormalMap"))
                    {
                        string fileNormalMap = textureInjectionData[section.SectionName]["filenameNormalMap"];
                        normalTexture = Resources.Load(fileNormalMap) as Texture2D;
                        texRecord.normalMap = normalTexture;
                    }

                    bool useMetallicGlossMap = bool.Parse(textureInjectionData[section.SectionName]["useMetallicGlossMap"]);

                    texRecord.useMetallicGlossMap = useMetallicGlossMap;

                    if (useMetallicGlossMap)
                    {
                        string fileNameMetallicGlossMap = textureInjectionData[section.SectionName]["filenameMetallicGlossMap"];
                        Texture2D metallicGlossMapTexture = Resources.Load(fileNameMetallicGlossMap) as Texture2D;
                        updateMaterial(textureArchive, textureRecord, textureFrame, albedoTexture, normalTexture, metallicGlossMapTexture);
                        texRecord.metallicGlossMap = metallicGlossMapTexture;
                    }
                    else
                    {
                        float reflectivity = float.Parse(textureInjectionData[section.SectionName]["reflectivity"]);
                        float smoothness = float.Parse(textureInjectionData[section.SectionName]["smoothness"]);
                        updateMaterial(textureArchive, textureRecord, textureFrame, albedoTexture, normalTexture, reflectivity, smoothness);
                        texRecord.reflectivity = reflectivity;
                        texRecord.smoothness = smoothness;
                    }

                    listInjectedTextures.Add(texRecord);
                }
            }

            // force update to textures loaded in current interior/dungeon models            
            Renderer[] renderers = null;
            if (GameManager.Instance.IsPlayerInsideBuilding)
            {
                renderers = gameObjectInterior.GetComponentsInChildren<Renderer>();
            }
            else if (GameManager.Instance.IsPlayerInsideDungeon || GameManager.Instance.IsPlayerInsideCastle)
            {
                renderers = gameObjectDungeon.GetComponentsInChildren<Renderer>();
            }
            if (renderers != null)
            {
                //Debug.Log(String.Format("renderers: {0}", renderers.Length));
                foreach (Renderer r in renderers)
                {
                    Material[] mats = r.sharedMaterials;
                    for (int i = 0; i < mats.Length; i++)
                    {
                        Material m = mats[i];
                        try
                        {
                            // name is in format TEXTURE.xxx [Index=y] - if atlas texture - different format then exception will be thrown - so everything is in try-catch block 
                            string[] parts = m.name.Split('.');
                            parts = parts[1].Split(' ');
                            int archive = Convert.ToInt32(parts[0]);
                            string tmp = parts[1].Replace("[Index=", "").Replace("]", "");
                            //Debug.Log(String.Format("archive: {0}, record: {1}", parts[0], tmp));
                            int record = Convert.ToInt32(tmp);
                            int frame = 0;

                            if (listInjectedTextures.Exists(x => (x.archive == archive) && (x.record == record) && (x.frame == frame)))
                            {
                                TextureRecord? texRecord = listInjectedTextures.Find(x => (x.archive == archive) && (x.record == record) && (x.frame == frame));
                                if (texRecord.HasValue)
                                {
                                    CachedMaterial cmat;
                                    if (dfUnity.MaterialReader.GetCachedMaterial(archive, record, frame, out cmat))
                                    {
                                        if (!texRecord.Value.useMetallicGlossMap)
                                        {
                                            if (useDeferredReflections)
                                            {
                                                updateMaterial(archive, record, frame, texRecord.Value.albedoMap, texRecord.Value.normalMap, texRecord.Value.reflectivity, texRecord.Value.smoothness);
                                            }
                                            else
                                            {
                                                Material newMat = new Material(Shader.Find("Daggerfall/ReflectionsMod/FloorMaterialWithReflections"));
                                                newMat.CopyPropertiesFromMaterial(cmat.material);
                                                newMat.name = cmat.material.name;
                                                if (texReflectionGround)
                                                {
                                                    newMat.SetTexture("_ReflectionGroundTex", texReflectionGround);
                                                }
                                                if (texReflectionLowerLevel)
                                                {
                                                    newMat.SetTexture("_ReflectionLowerLevelTex", texReflectionLowerLevel);
                                                }
                                                newMat.SetFloat("_Metallic", texRecord.Value.reflectivity);
                                                newMat.SetFloat("_Smoothness", texRecord.Value.smoothness);

                                                if (texRecord.Value.albedoMap != null)
                                                {
                                                    newMat.SetTexture("_MainTex", texRecord.Value.albedoMap);
                                                }

                                                if (texRecord.Value.normalMap != null)
                                                {
                                                    newMat.SetTexture("_BumpMap", texRecord.Value.normalMap);
                                                }

                                                m = newMat;
                                            }                                            
                                        }
                                        else
                                        {
                                            if (useDeferredReflections)
                                            {
                                                updateMaterial(archive, record, frame, texRecord.Value.albedoMap, texRecord.Value.normalMap, texRecord.Value.metallicGlossMap);
                                            }
                                            else
                                            {
                                                Material newMat = new Material(Shader.Find("Daggerfall/ReflectionsMod/FloorMaterialWithReflections"));
                                                newMat.CopyPropertiesFromMaterial(cmat.material);
                                                newMat.name = cmat.material.name;
                                                if (texReflectionGround)
                                                {
                                                    newMat.SetTexture("_ReflectionGroundTex", texReflectionGround);
                                                }
                                                if (texReflectionLowerLevel)
                                                {
                                                    newMat.SetTexture("_ReflectionLowerLevelTex", texReflectionLowerLevel);
                                                }
                                                newMat.EnableKeyword("USE_METALLICGLOSSMAP");
                                                newMat.SetTexture("_MetallicGlossMap", texRecord.Value.metallicGlossMap);

                                                if (texRecord.Value.albedoMap != null)
                                                {
                                                    newMat.SetTexture("_MainTex", texRecord.Value.albedoMap);
                                                }

                                                if (texRecord.Value.normalMap != null)
                                                {
                                                    newMat.SetTexture("_BumpMap", texRecord.Value.normalMap);
                                                }

                                                m = newMat;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                        }
                        mats[i] = m;
                    }
                    r.sharedMaterials = mats;
                }
            }
        }

        void InjectMaterialPropertiesOutdoor()
        {
            GameObject go = GameObject.Find("StreamingTarget");
            if (!go)
            {
                return;
            }
            foreach (Transform child in go.transform)
            {
                DaggerfallTerrain dfTerrain = child.GetComponent<DaggerfallTerrain>();
                if (!dfTerrain)
                    continue;

                PlayerGPS playerGPS = GameObject.Find("PlayerAdvanced").GetComponent<PlayerGPS>();
                if (!playerGPS)
                    continue;

                //if ((dfTerrain.MapPixelX != playerGPS.CurrentMapPixel.X) || (dfTerrain.MapPixelY != playerGPS.CurrentMapPixel.Y))
                //    continue;


                Terrain terrain = child.GetComponent<Terrain>();
                if (terrain)
                {
                    if ((terrain.materialTemplate)) //&&(terrain.materialTemplate.shader.name != "Daggerfall/TilemapWithReflections")) // uncommenting this makes initial location (after startup, not fast travelling) not receive correct shader - don't know why - so workaround is to force injecting materialshader even for unset material (not sure why it works, but it does)
                    {
                        Texture tileSetTexture = terrain.materialTemplate.GetTexture("_TileAtlasTex");

                        //Texture2D tileAtlas = dfUnity.MaterialReader.TextureReader.GetTerrainTilesetTexture(402).albedoMap;
                        //System.IO.File.WriteAllBytes("./Assets/Daggerfall/ReflectionsMod/Resources/tileatlas_402.png", tileAtlas.EncodeToPNG());

                        Texture tileMapTexture = terrain.materialTemplate.GetTexture("_TilemapTex");
                        int tileMapDim = terrain.materialTemplate.GetInt("_TilemapDim");

                        Material newMat = new Material(Shader.Find("Daggerfall/ReflectionsMod/TilemapWithReflections"));

                        newMat.SetTexture("_TileAtlasTex", tileSetTexture);
                        newMat.SetTexture("_TilemapTex", tileMapTexture);
                        newMat.SetInt("_TilemapDim", tileMapDim);

                        newMat.SetTexture("_ReflectionGroundTex", texReflectionGround);

                        newMat.SetFloat("_GroundLevelHeight", gameObjectReflectionPlaneLowerLevel.transform.position.y);

                        newMat.SetTexture("_ReflectionSeaTex", texReflectionLowerLevel);

                        newMat.SetFloat("_SeaLevelHeight", gameObjectReflectionPlaneSeaLevel.transform.position.y);

                        WeatherManager weatherManager = GameObject.Find("WeatherManager").GetComponent<WeatherManager>();
                        if (!weatherManager.IsRaining)
                        {
                            Texture2D tileAtlasReflectiveTexture = Resources.Load("tileatlas_reflective") as Texture2D;
                            newMat.SetTexture("_TileAtlasReflectiveTex", tileAtlasReflectiveTexture);
                        }
                        else
                        {
                            Texture2D tileAtlasReflectiveTexture = Resources.Load("tileatlas_reflective_raining") as Texture2D;
                            newMat.SetTexture("_TileAtlasReflectiveTex", tileAtlasReflectiveTexture);
                        }

                        terrain.materialTemplate = newMat;
                    }
                }
            }
        }

        void updateMaterial(int archive, int record, int frame, Texture2D albedoMap, Texture2D normalMap, float reflectivity, float smoothness)
        {
            CachedMaterial cmat;
            if (dfUnity.MaterialReader.GetCachedMaterial(archive, record, frame, out cmat))
            {
                Material newMat;
                if (useDeferredReflections)
                {
                    newMat = cmat.material;
                    //newMat = new Material(Shader.Find("Standard (Specular setup)"));
                    //newMat.CopyPropertiesFromMaterial(cmat.material);
                }
                else
                {
                    newMat = new Material(Shader.Find("Daggerfall/ReflectionsMod/FloorMaterialWithReflections"));
                    newMat.CopyPropertiesFromMaterial(cmat.material);
                    newMat.name = cmat.material.name;
                }

                if (texReflectionGround)
                {
                    newMat.SetTexture("_ReflectionGroundTex", texReflectionGround);
                }
                if (texReflectionLowerLevel)
                {
                    newMat.SetTexture("_ReflectionLowerLevelTex", texReflectionLowerLevel);
                }
                newMat.SetFloat("_Metallic", reflectivity);
                //newMat.SetColor("_SpecColor", new Color(reflectivity, reflectivity, reflectivity));
                newMat.SetFloat("_Glossiness", smoothness);

                if (albedoMap != null)
                {
                    newMat.SetTexture("_MainTex", albedoMap);
                }

                if (normalMap != null)
                {
                    newMat.SetTexture("_BumpMap", normalMap);
                }

                cmat.material = newMat;
                dfUnity.MaterialReader.SetCachedMaterial(archive, record, frame, cmat);
            }
        }

        void updateMaterial(int archive, int record, int frame, Texture2D albedoMap, Texture2D normalMap, Texture2D metallicGlossMap)
        {
            CachedMaterial cmat;
            if (dfUnity.MaterialReader.GetCachedMaterial(archive, record, frame, out cmat))
            {
                Material newMat;
                if (useDeferredReflections)
                {
                    newMat = cmat.material;
                    //newMat = new Material(Shader.Find("Standard (Specular setup)"));
                    //newMat.CopyPropertiesFromMaterial(cmat.material);
                }
                else
                {
                    newMat = new Material(Shader.Find("Daggerfall/ReflectionsMod/FloorMaterialWithReflections"));
                    newMat.CopyPropertiesFromMaterial(cmat.material);
                    newMat.name = cmat.material.name;
                }

                if (texReflectionGround)
                {
                    newMat.SetTexture("_ReflectionGroundTex", texReflectionGround);
                }
                if (texReflectionLowerLevel)
                {
                    newMat.SetTexture("_ReflectionLowerLevelTex", texReflectionLowerLevel);
                }
                
                newMat.EnableKeyword("_METALLICGLOSSMAP");
                //newMat.EnableKeyword("_SPECGLOSSMAP");
                newMat.DisableKeyword("_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A");
                newMat.SetTexture("_MetallicGlossMap", metallicGlossMap);

                if (albedoMap != null)
                {
                    newMat.SetTexture("_MainTex", albedoMap);
                }

                if (normalMap != null)
                {
                    newMat.SetTexture("_BumpMap", normalMap);
                }

                cmat.material = newMat;
                dfUnity.MaterialReader.SetCachedMaterial(archive, record, frame, cmat);
            }
        }

        //overloaded variant
        void InjectMaterialProperties(DaggerfallTerrain sender)
        {
            InjectMaterialProperties(-1, -1);
        }

        //overloaded variant
        void InjectMaterialProperties(DFPosition worldPos)
        {
            InjectMaterialProperties(worldPos.X, worldPos.Y);
        }

        //overloaded variant
        void InjectMaterialProperties(Vector3 offset)
        {
            InjectMaterialProperties();
        }

        //overloaded variant
        void InjectMaterialProperties()
        {
            InjectMaterialProperties(-1, -1);
        }

        void InjectMaterialProperties(int worldPosX, int worldPosY)
        {
            if (!GameManager.Instance.IsPlayerInside)
            {
                InjectMaterialPropertiesOutdoor();
            }
        }
    }
}