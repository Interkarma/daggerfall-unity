//ReflectionsMod for Daggerfall Tools For Unity
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: Michael Rauter (a.k.a. Nystul)
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)
//Version: 0.32

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
        // Streaming World Component
        public StreamingWorld streamingWorld;

        private GameObject gameObjectReflectionPlaneGroundLevel = null;
        private GameObject gameObjectReflectionPlaneSeaLevel = null;
        private GameObject gameObjectReflectionPlaneLowerLevel = null;

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

        void Start()
        {
            dfUnity = DaggerfallUnity.Instance;

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

        void Awake()        
        {
            StreamingWorld.OnInitWorld += InjectMaterialProperties;

            StreamingWorld.OnTeleportToCoordinates += InjectMaterialProperties;

            FloatingOrigin.OnPositionUpdate += InjectMaterialProperties;

            PlayerEnterExit.OnTransitionInterior += InjectMaterialPropertiesIndoor;
            PlayerEnterExit.OnTransitionExterior += InjectMaterialPropertiesOutdoor;
            PlayerEnterExit.OnTransitionDungeonInterior += InjectMaterialPropertiesIndoor;
            PlayerEnterExit.OnTransitionDungeonExterior += InjectMaterialPropertiesOutdoor;

            DaggerfallTerrain.OnInstantiateTerrain += InjectMaterialProperties;
        }

        void OnDestroy()
        {
            StreamingWorld.OnInitWorld -= InjectMaterialProperties;

            StreamingWorld.OnTeleportToCoordinates -= InjectMaterialProperties;

            FloatingOrigin.OnPositionUpdate -= InjectMaterialProperties;

            PlayerEnterExit.OnTransitionInterior -= InjectMaterialPropertiesIndoor;
            PlayerEnterExit.OnTransitionExterior -= InjectMaterialPropertiesOutdoor;
            PlayerEnterExit.OnTransitionDungeonInterior -= InjectMaterialPropertiesIndoor;
            PlayerEnterExit.OnTransitionDungeonExterior -= InjectMaterialPropertiesOutdoor;

            DaggerfallTerrain.OnInstantiateTerrain -= InjectMaterialProperties;
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
                            if (terrain.materialTemplate.shader.name == "Daggerfall/TilemapWithReflections")
                            {
                                terrain.materialTemplate.SetFloat("_GroundLevelHeight", gameObjectReflectionPlaneGroundLevel.transform.position.y);
                                terrain.materialTemplate.SetFloat("_SeaLevelHeight", gameObjectReflectionPlaneSeaLevel.transform.position.y);
                            }
                        }
                    }
                }
            }

            if (GameManager.Instance.IsPlayerInside)
            {
                Renderer[] renderers = null;

                // TODO: find a way to eliminate GameObject.Find() here and determine if inside building or dungeon
                GameObject gameObjectInterior = GameObject.Find("Interior"); 
                GameObject gameObjectDungeon = GameObject.Find("Dungeon");
                if (gameObjectInterior != null)
                {
                    renderers = gameObjectInterior.GetComponentsInChildren<Renderer>();
                }
                else if (gameObjectDungeon != null)
                {
                    renderers = gameObjectDungeon.GetComponentsInChildren<Renderer>();
                }

                if (renderers != null)
                {
                    foreach (Renderer r in renderers)
                    {
                        Material[] mats = r.sharedMaterials;
                        foreach (Material m in mats)
                        {
                            if (m.shader.name == "Daggerfall/FloorMaterialWithReflections")
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

        //overloaded variant
        void InjectMaterialProperties(DaggerfallTerrain sender)
        {
            InjectMaterialProperties(-1, -1);
        }

        //overloaded variant
        void InjectMaterialPropertiesIndoor(PlayerEnterExit.TransitionEventArgs args)
        {
            InjectMaterialPropertiesIndoor();
        }
        
        //overloaded variant
        void InjectMaterialPropertiesOutdoor(PlayerEnterExit.TransitionEventArgs args)
        {
            InjectMaterialPropertiesOutdoor();
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
            if (GameManager.Instance.IsPlayerInside)
            {
                InjectMaterialPropertiesIndoor();
            }

            if (!GameManager.Instance.IsPlayerInside)
            {
                InjectMaterialPropertiesOutdoor();
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

            // force update to textures loaded in current interior/dungeon models (TODO: find a better way to replace the long code section starting from here to the end of the function)
            Renderer[] renderers = null;

            // TODO: find a way to eliminate GameObject.Find() here and determine if inside building or dungeon
            GameObject gameObjectInterior = GameObject.Find("Interior");
            GameObject gameObjectDungeon = GameObject.Find("Dungeon");
            if (gameObjectInterior != null)
            {
                renderers = gameObjectInterior.GetComponentsInChildren<Renderer>();
            }
            else if (gameObjectDungeon != null)
            {
                renderers = gameObjectDungeon.GetComponentsInChildren<Renderer>();
            }
            if (renderers != null)
            {
                Debug.Log(String.Format("renderers: {0}", renderers.Length));
                foreach (Renderer r in renderers)
                {
                    Material[] mats = r.sharedMaterials;
                    for (int i=0; i<mats.Length; i++)
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

                            TextureRecord ?texRecord = listInjectedTextures.Find(x => (x.archive == archive) && (x.record == record) && (x.frame == frame));
                            if (texRecord != null)
                            {
                                CachedMaterial cmat;
                                if (dfUnity.MaterialReader.GetCachedMaterial(archive, record, frame, out cmat))
                                {
                                    if (!texRecord.Value.useMetallicGlossMap)
                                    {
                                        Material newMat = new Material(Shader.Find("Daggerfall/FloorMaterialWithReflections"));
                                        newMat.CopyPropertiesFromMaterial(cmat.material);
                                        newMat.name = cmat.material.name;
                                        Texture tex = GameObject.Find("ReflectionPlaneBottom").GetComponent<MirrorReflection>().m_ReflectionTexture;
                                        if (tex)
                                        {
                                            newMat.SetTexture("_ReflectionGroundTex", tex);
                                        }
                                        tex = GameObject.Find("ReflectionPlaneSeaLevel").GetComponent<MirrorReflection>().m_ReflectionTexture;
                                        if (tex)
                                        {
                                            newMat.SetTexture("_ReflectionLowerLevelTex", tex);
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
                                    else
                                    {
                                        Material newMat = new Material(Shader.Find("Daggerfall/FloorMaterialWithReflections"));
                                        newMat.CopyPropertiesFromMaterial(cmat.material);
                                        newMat.name = cmat.material.name;
                                        Texture tex = GameObject.Find("ReflectionPlaneBottom").GetComponent<MirrorReflection>().m_ReflectionTexture;
                                        if (tex)
                                        {
                                            newMat.SetTexture("_ReflectionGroundTex", tex);
                                        }
                                        tex = GameObject.Find("ReflectionPlaneSeaLevel").GetComponent<MirrorReflection>().m_ReflectionTexture;
                                        if (tex)
                                        {
                                            newMat.SetTexture("_ReflectionLowerLevelTex", tex);
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
                        //System.IO.File.WriteAllBytes("./Assets/ReflectionsMod/Resources/tileatlas_402.png", tileAtlas.EncodeToPNG());

                        Texture tileMapTexture = terrain.materialTemplate.GetTexture("_TilemapTex");
                        int tileMapDim = terrain.materialTemplate.GetInt("_TilemapDim");

                        Material newMat = new Material(Shader.Find("Daggerfall/TilemapWithReflections"));

                        newMat.SetTexture("_TileAtlasTex", tileSetTexture);
                        newMat.SetTexture("_TilemapTex", tileMapTexture);
                        newMat.SetInt("_TilemapDim", tileMapDim);

                        GameObject goReflectionPlaneBottom = GameObject.Find("ReflectionPlaneBottom");
                        Texture tex = goReflectionPlaneBottom.GetComponent<MirrorReflection>().m_ReflectionTexture;
                        newMat.SetTexture("_ReflectionGroundTex", tex);

                        //newMat.SetFloat("_GroundLevelHeight", goReflectionPlaneBottom.transform.position.y);

                        GameObject goReflectionPlaneSeaLevel = GameObject.Find("ReflectionPlaneSeaLevel");
                        Texture texSea = goReflectionPlaneSeaLevel.GetComponent<MirrorReflection>().m_ReflectionTexture;
                        newMat.SetTexture("_ReflectionSeaTex", texSea);

                        //newMat.SetFloat("_SeaLevelHeight", goReflectionPlaneSeaLevel.transform.position.y);                            

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
                Material newMat = new Material(Shader.Find("Daggerfall/FloorMaterialWithReflections"));
                newMat.CopyPropertiesFromMaterial(cmat.material);
                newMat.name = cmat.material.name;
                Texture tex = GameObject.Find("ReflectionPlaneBottom").GetComponent<MirrorReflection>().m_ReflectionTexture;
                if (tex)
                {
                    newMat.SetTexture("_ReflectionGroundTex", tex);
                }
                tex = GameObject.Find("ReflectionPlaneSeaLevel").GetComponent<MirrorReflection>().m_ReflectionTexture;
                if (tex)
                {
                    newMat.SetTexture("_ReflectionLowerLevelTex", tex);
                }
                newMat.SetFloat("_Metallic", reflectivity);
                newMat.SetFloat("_Smoothness", smoothness);

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
                Material newMat = new Material(Shader.Find("Daggerfall/FloorMaterialWithReflections"));
                newMat.CopyPropertiesFromMaterial(cmat.material);
                newMat.name = cmat.material.name;
                Texture tex = GameObject.Find("ReflectionPlaneBottom").GetComponent<MirrorReflection>().m_ReflectionTexture;
                if (tex)
                {
                    newMat.SetTexture("_ReflectionGroundTex", tex);
                }
                tex = GameObject.Find("ReflectionPlaneSeaLevel").GetComponent<MirrorReflection>().m_ReflectionTexture;
                if (tex)
                {
                    newMat.SetTexture("_ReflectionLowerLevelTex", tex);
                }
                newMat.EnableKeyword("USE_METALLICGLOSSMAP");
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

    }
}