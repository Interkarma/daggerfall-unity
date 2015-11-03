//ReflectionsMod for Daggerfall Tools For Unity
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: Michael Rauter (a.k.a. Nystul)
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)
//Version: 0.32

using UnityEngine;
using UnityEngine.Rendering;
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

        DaggerfallUnity dfUnity;

        UpdateReflectionTextures reflectionTexturesScript = null;

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

        void Start()
        {
            dfUnity = DaggerfallUnity.Instance;

            reflectionTexturesScript = GameObject.Find("ReflectionsMod").GetComponent<UpdateReflectionTextures>();

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
            if (reflectionTexturesScript.isOutdoorEnvironment())
            {
                GameObject goReflectionPlaneGroundLevel = GameObject.Find("ReflectionPlaneBottom");
                GameObject goReflectionPlaneSeaLevel = GameObject.Find("ReflectionPlaneSeaLevel");

                GameObject go = GameObject.Find("StreamingTarget");
                foreach (Transform child in go.transform)
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
                                terrain.materialTemplate.SetFloat("_GroundLevelHeight", goReflectionPlaneGroundLevel.transform.position.y);
                                terrain.materialTemplate.SetFloat("_SeaLevelHeight", goReflectionPlaneSeaLevel.transform.position.y);
                            }
                        }
                    }
                }
            }

            if (reflectionTexturesScript.isIndoorEnvironment())
            {
                GameObject goReflectionPlaneGroundLevel = GameObject.Find("ReflectionPlaneBottom");
                        
                GameObject goReflectionPlaneLowerLevel = GameObject.Find("ReflectionPlaneSeaLevel");

                Renderer[] renderers = null;
                if (GameObject.Find("Interior"))
                {
                    renderers = GameObject.Find("Interior").GetComponentsInChildren<Renderer>();
                }
                else if (GameObject.Find("Dungeon"))
                {
                    renderers = GameObject.Find("Dungeon").GetComponentsInChildren<Renderer>();
                }

                if (renderers != null)
                {
                    foreach (Renderer r in renderers)
                    {
                        foreach (Material m in r.sharedMaterials)
                        {
                            if (m.shader.name == "Daggerfall/FloorMaterialWithReflections")
                            {
                                m.SetFloat("_GroundLevelHeight", goReflectionPlaneGroundLevel.transform.position.y);
                                m.SetFloat("_LowerLevelHeight", goReflectionPlaneLowerLevel.transform.position.y);
                            }
                        }
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
            // for some reason indoor reflections do not work on first transition to interior if this line is uncommented - TODO: further investigate
            // (seems like indoor floor textures in cache are somehow "initialized" differently if the callback is invoked on startup although player is outdoors but indoor textures are also injected here)
            // even more mysterious is that trying to debug freezes unity if one sets a break point on the InjectMaterialPropertiesIndoor() line and the if statement is active, if it is commented out no freeze occurs
            //if (reflectionTexturesScript.isIndoorEnvironment())
           //{
               InjectMaterialPropertiesIndoor();
           //}

           if (reflectionTexturesScript.isOutdoorEnvironment())
           {
               InjectMaterialPropertiesOutdoor();
           }
        }

        void InjectMaterialPropertiesIndoor()
        {
            // mages guild 4 floors debuging worldpos: 704,337
            IniParser.Model.IniData textureInjectionData = iniData;
            if (iniData != null)
            {
                foreach (IniParser.Model.SectionData section in iniData.Sections)
                {
                    int textureArchive = int.Parse(textureInjectionData[section.SectionName]["textureArchive"]);
                    int textureRecord = int.Parse(textureInjectionData[section.SectionName]["textureRecord"]);
                    int textureFrame = int.Parse(textureInjectionData[section.SectionName]["textureFrame"]);

                    Texture2D albedoTexture = null;
                    if (textureInjectionData[section.SectionName].ContainsKey("filenameAlbedoMap"))
                    {
                        string fileAlbedoMap = textureInjectionData[section.SectionName]["filenameAlbedoMap"];
                        albedoTexture = Resources.Load(fileAlbedoMap) as Texture2D;
                    }

                    Texture2D normalTexture = null;
                    if (textureInjectionData[section.SectionName].ContainsKey("filenameNormalMap"))
                    {
                        string fileNormalMap = textureInjectionData[section.SectionName]["filenameNormalMap"];
                        normalTexture = Resources.Load(fileNormalMap) as Texture2D;
                    }

                    bool useMetallicGlossMap = bool.Parse(textureInjectionData[section.SectionName]["useMetallicGlossMap"]);

                    if (useMetallicGlossMap)
                    {
                        string fileNameMetallicGlossMap = textureInjectionData[section.SectionName]["filenameMetallicGlossMap"];
                        Texture2D metallicGlossMapTexture = Resources.Load(fileNameMetallicGlossMap) as Texture2D;
                        updateMaterial(textureArchive, textureRecord, textureFrame, albedoTexture, normalTexture, metallicGlossMapTexture);
                    }
                    else
                    {
                        float reflectivity = float.Parse(textureInjectionData[section.SectionName]["reflectivity"]);
                        float smoothness = float.Parse(textureInjectionData[section.SectionName]["smoothness"]);
                        updateMaterial(textureArchive, textureRecord, textureFrame, albedoTexture, normalTexture, reflectivity, smoothness);
                    }
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