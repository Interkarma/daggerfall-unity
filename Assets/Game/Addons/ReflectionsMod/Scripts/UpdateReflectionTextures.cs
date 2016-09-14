//ReflectionsMod for Daggerfall Tools For Unity
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: Michael Rauter (a.k.a. Nystul)
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)

using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Utility;

namespace ReflectionsMod
{
    public class UpdateReflectionTextures : MonoBehaviour
    {
        public string iniPathConfigInjectionTextures = ""; // handed over to InjectReflectiveMaterialProperty script - if not specified will use fallback ini-file
        //public string iniPathConfigInjectionTextures = "Assets/daggerfall-unity/Game/Addons/ReflectionsMod/Resources/configInjectionTextures.ini"; // use this line to test ini file loading from filepath

        private GameObject reflectionPlaneBottom = null;
        private GameObject reflectionPlaneSeaLevel = null;

        private MirrorReflection mirrorRefl = null; 
        private MirrorReflection mirrorReflSeaLevel = null;

        private DeferredPlanarReflections componentDefferedPlanarReflections = null;

        private bool playerInside = false;

        public RenderTexture getSeaReflectionRenderTexture()
        {
            return mirrorReflSeaLevel.m_ReflectionTexture;
        }

        public RenderTexture getGroundReflectionRenderTexture()
        {
            return mirrorRefl.m_ReflectionTexture;
        }

        public float ReflectionPlaneGroundLevelY
        {
            get
            {
                if (reflectionPlaneBottom)
                    return reflectionPlaneBottom.transform.position.y;
                else
                    return 0.0f;
            }
        }

        public float ReflectionPlaneLowerLevelY
        {
            get
            {
                if (reflectionPlaneSeaLevel)
                    return reflectionPlaneSeaLevel.transform.position.y;
                else
                    return 0.0f;
            }
        }

        bool computeStepDownRaycast(Vector3 raycastStartPoint, Vector3 directionVec, float maxDiffMagnitude, out RaycastHit hit)
        {
            if (Physics.Raycast(raycastStartPoint, directionVec, out hit, 1000.0F))
            {
                Vector3 hitPoint = hit.point;
                hitPoint -= directionVec * 0.1f; // move away a bit from hitpoint in opposite direction of raycast - this is required since daggerfall interiors often have small gaps/holes where a walls meet the floor - so upcoming down-raycast will not miss floor...

                // and now raycast down
                if (Physics.Raycast(hitPoint, Vector3.down, out hit, 1000.0F))
                {
                    Vector3 diffVec = raycastStartPoint - hit.point;
                    if (diffVec.sqrMagnitude <= maxDiffMagnitude)
                    {
                        return (true);
                    }
                }
                else
                {
                    return (false);
                }
            }
            return (false);
        }

        float distanceToLowerLevel(Vector3 startPoint, Vector3 directionVec, out RaycastHit hit)
        {
            const int maxIterations = 20;
            const float offset = 0.01f;
            float maxDiffMagnitude = 3 * offset * offset * 1.1f; // ... once for every axis, *1.1f ... make it a bit bigger than the offset "boundingbox"
            
            // iterative raycast in forward direction
            Vector3 raycastStartPoint = startPoint; // +Camera.main.transform.position; // +new Vector3(0.0f, 0.1f, 0.0f);
            for (int i = 0; i < maxIterations; i++)
            {
                if (computeStepDownRaycast(raycastStartPoint, directionVec, maxDiffMagnitude, out hit))
                { 
                    return (startPoint.y - hit.point.y);
                }

                Vector3 offsetVec = -directionVec * offset;  // move away a bit from hitpoint in opposite direction of raycast - this is required since daggerfall interiors often have small gaps/holes where a walls meet the floor - so upcoming down-raycast will not miss floor...
                offsetVec.y = offset; // move a bit up as well - so that next raycast starts above ground a bit
                raycastStartPoint = hit.point + offsetVec;
            }
            hit = new RaycastHit();
            return(float.MinValue);
        }

        float majorityOf3FloatValues(float value1, float value2, float value3, float allowedDistance, float valueWhenTied)
        {
            if ((Mathf.Abs(value1 - value2) < allowedDistance) || (Mathf.Abs(value1 - value3) < allowedDistance))
                return (value1);
            else if (Mathf.Abs(value2 - value3) < allowedDistance)
            {
                return (value2);
            }
            else
                return valueWhenTied;
        }

        float distanceToLowerLevelStartingFromWall(Vector3 startPoint, Vector3 directionVectorToWall, Vector3 directionVectorRaycast1, Vector3 directionVectorRaycast2)
        {
            const float offset = 0.1f;
            RaycastHit hit, hit1, hit2, hit3;
            float biasAmount = 0.5f; // vector bias of the 2 additional parallel raycasts
            Vector3 biasVec = biasAmount * Vector3.Normalize(Vector3.Cross(directionVectorToWall, Vector3.up)); // get normalized normal vector to directionVectorToWall and up vector
            Physics.Raycast(startPoint, directionVectorToWall, out hit1, 1000.0F);
            // do 2 additional parallel raycast with small bias and do majority vote - workaround to reduce problems with holes in geometry...
            Physics.Raycast(startPoint + biasVec, directionVectorToWall, out hit2, 1000.0F);
            Physics.Raycast(startPoint - biasVec, directionVectorToWall, out hit3, 1000.0F);

            Vector3 wallPoint;
            float distance = majorityOf3FloatValues(hit1.distance, hit2.distance, hit3.distance, 0.5f, float.MinValue); // 0.5f must be near to each other but allows for slanted walls (e.g. mages guild's spiral stair)
            if (Mathf.Abs(hit1.distance - distance) < 0.5f)
            {
                wallPoint = hit1.point;
            }
            else if (Mathf.Abs(hit2.distance - distance) < 0.5f)
            {
                wallPoint = hit2.point;
            }
            else if (Mathf.Abs(hit3.distance - distance) < 0.5f)
            {
                wallPoint = hit3.point;
            }
            else
            {
                return (float.MinValue);
            }


            //Vector3 wallPoint = hit.point;
            //wallPoint.y += 0.05f;
            float distance1 = distanceToLowerLevel(wallPoint - directionVectorToWall * offset, directionVectorRaycast1, out hit);
            float distance2 = distanceToLowerLevel(wallPoint - directionVectorToWall * offset, directionVectorRaycast2, out hit);
            return Mathf.Max(distance1, distance2);
        }

        float getDistanceToLowerLevel(GameObject goPlayerAdvanced)
        {
            float distanceToLowerLevelWhenGoingForward = float.MinValue;
            float distanceToLowerLevelWhenGoingBack = float.MinValue;
            float distanceToLowerLevelWhenGoingLeft = float.MinValue;
            float distanceToLowerLevelWhenGoingRight = float.MinValue;
            float distanceToLowerLevelWhenGoingForwardLeft = float.MinValue;
            float distanceToLowerLevelWhenGoingForwardRight = float.MinValue;
            float distanceToLowerLevelWhenGoingBackLeft = float.MinValue;
            float distanceToLowerLevelWhenGoingBackRight = float.MinValue;

            float distanceToLowerLevelStartingFromLeftWall = float.MinValue;
            float distanceToLowerLevelStartingFromRightWall = float.MinValue;
            float distanceToLowerLevelStartingFromForwardWall = float.MinValue;
            float distanceToLowerLevelStartingFromBackWall = float.MinValue;

            RaycastHit hit;

            Vector3 startPoint = goPlayerAdvanced.transform.position;

            // 2 additional raycasts parallel to the main raycast just next to it are performed - majority vote of result of these 3 raycasts is then used as distance to lower level in the direction of interest
            float biasAmount = 0.5f; // vector bias of the 2 additional parallel raycasts

            distanceToLowerLevelWhenGoingForward = majorityOf3FloatValues(distanceToLowerLevel(startPoint, Vector3.forward, out hit),
                                                                          distanceToLowerLevel(startPoint + new Vector3(-biasAmount, 0.0f, 0.0f), Vector3.forward, out hit),
                                                                          distanceToLowerLevel(startPoint + new Vector3(+biasAmount, 0.0f, 0.0f), Vector3.forward, out hit),
                                                                          0.001f,
                                                                          float.MinValue);
            distanceToLowerLevelWhenGoingBack = majorityOf3FloatValues(distanceToLowerLevel(startPoint, Vector3.back, out hit),
                                                                       distanceToLowerLevel(startPoint + new Vector3(-biasAmount, 0.0f, 0.0f), Vector3.back, out hit),
                                                                       distanceToLowerLevel(startPoint + new Vector3(+biasAmount, 0.0f, 0.0f), Vector3.back, out hit),
                                                                       0.001f,
                                                                       float.MinValue);


            distanceToLowerLevelWhenGoingLeft = majorityOf3FloatValues(distanceToLowerLevel(startPoint, Vector3.left, out hit),
                                                                       distanceToLowerLevel(startPoint + new Vector3(0.0f, 0.0f, -biasAmount), Vector3.left, out hit),
                                                                       distanceToLowerLevel(startPoint + new Vector3(0.0f, 0.0f, +biasAmount), Vector3.left, out hit),
                                                                       0.001f,
                                                                       float.MinValue);

            distanceToLowerLevelWhenGoingRight = majorityOf3FloatValues(distanceToLowerLevel(startPoint, Vector3.right, out hit),
                                                                       distanceToLowerLevel(startPoint + new Vector3(0.0f, 0.0f, -biasAmount), Vector3.right, out hit),
                                                                       distanceToLowerLevel(startPoint + new Vector3(0.0f, 0.0f, +biasAmount), Vector3.right, out hit),
                                                                       0.001f,
                                                                       float.MinValue);


            distanceToLowerLevelWhenGoingForwardLeft = majorityOf3FloatValues(distanceToLowerLevel(startPoint, new Vector3(-1.0f, 0.0f, 1.0f), out hit),
                                                                       distanceToLowerLevel(startPoint + new Vector3(-biasAmount, 0.0f, -biasAmount), new Vector3(-1.0f, 0.0f, 1.0f), out hit),
                                                                       distanceToLowerLevel(startPoint + new Vector3(+biasAmount, 0.0f, +biasAmount), new Vector3(-1.0f, 0.0f, 1.0f), out hit),
                                                                       0.001f,
                                                                       float.MinValue);

            distanceToLowerLevelWhenGoingForwardRight = majorityOf3FloatValues(distanceToLowerLevel(startPoint, new Vector3(1.0f, 0.0f, 1.0f), out hit),
                                                                       distanceToLowerLevel(startPoint + new Vector3(-biasAmount, 0.0f, +biasAmount), new Vector3(1.0f, 0.0f, 1.0f), out hit),
                                                                       distanceToLowerLevel(startPoint + new Vector3(+biasAmount, 0.0f, -biasAmount), new Vector3(1.0f, 0.0f, 1.0f), out hit),
                                                                       0.001f,
                                                                       float.MinValue);

            distanceToLowerLevelWhenGoingBackLeft = majorityOf3FloatValues(distanceToLowerLevel(startPoint, new Vector3(-1.0f, 0.0f, -1.0f), out hit),
                                                                       distanceToLowerLevel(startPoint + new Vector3(-biasAmount, 0.0f, +biasAmount), new Vector3(-1.0f, 0.0f, -1.0f), out hit),
                                                                       distanceToLowerLevel(startPoint + new Vector3(+biasAmount, 0.0f, -biasAmount), new Vector3(-1.0f, 0.0f, -1.0f), out hit),
                                                                       0.001f,
                                                                       float.MinValue);

            distanceToLowerLevelWhenGoingBackRight = majorityOf3FloatValues(distanceToLowerLevel(startPoint, new Vector3(1.0f, 0.0f, -1.0f), out hit),
                                                                       distanceToLowerLevel(startPoint + new Vector3(-biasAmount, 0.0f, -biasAmount), new Vector3(1.0f, 0.0f, -1.0f), out hit),
                                                                       distanceToLowerLevel(startPoint + new Vector3(+biasAmount, 0.0f, +biasAmount), new Vector3(1.0f, 0.0f, -1.0f), out hit),
                                                                       0.001f,
                                                                       float.MinValue);

            // now go to wall and start 2 perpendicular raycast (perpendicular to the wall) to determine distance to lower level (this is important when facing the edge of a wall but you can see to lower level beside the edge, otherwise you miss the lower level plane with the above raycast steps)
            float extraY = Camera.main.transform.localPosition.y; // start from eye position to reduce unintentional "around the corner sampling" - can still happen but less likely
            startPoint.y += extraY;
            distanceToLowerLevelStartingFromLeftWall = -extraY + distanceToLowerLevelStartingFromWall(startPoint, Vector3.left, Vector3.forward, Vector3.back);
            distanceToLowerLevelStartingFromRightWall = -extraY + distanceToLowerLevelStartingFromWall(startPoint, Vector3.right, Vector3.forward, Vector3.back);
            distanceToLowerLevelStartingFromForwardWall = -extraY + distanceToLowerLevelStartingFromWall(startPoint, Vector3.forward, Vector3.left, Vector3.right);
            distanceToLowerLevelStartingFromBackWall = -extraY + distanceToLowerLevelStartingFromWall(startPoint, Vector3.back, Vector3.left, Vector3.right);


            return (Mathf.Max(distanceToLowerLevelWhenGoingForward, distanceToLowerLevelWhenGoingBack, distanceToLowerLevelWhenGoingLeft, distanceToLowerLevelWhenGoingRight, 
                                distanceToLowerLevelWhenGoingForwardLeft, distanceToLowerLevelWhenGoingForwardRight, distanceToLowerLevelWhenGoingBackLeft, distanceToLowerLevelWhenGoingBackRight,
                                distanceToLowerLevelStartingFromLeftWall, distanceToLowerLevelStartingFromRightWall, distanceToLowerLevelStartingFromForwardWall, distanceToLowerLevelStartingFromBackWall));

        }

        Mesh CreateMesh(float width, float height)
        {
            Mesh m = new Mesh();
            m.name = "ScriptedMesh";
            m.vertices = new Vector3[] {
                new Vector3(-width, 0.01f, -height),
                new Vector3(width, 0.01f, -height),
                new Vector3(width, 0.01f, height),
                new Vector3(-width, 0.01f, height),
                new Vector3(-width, -10000.0f, -height), // create 2nd plane at level -10000, so that the stacked near camera (with near clip plane of around 1000) will trigger OnWillRenderObject() callback
                new Vector3(width, -10000.0f, -height), // create 2nd plane at level -10000, so that the stacked near camera (with near clip plane of around 1000) will trigger OnWillRenderObject() callback
                new Vector3(width, -10000.0f, height), // create 2nd plane at level -10000, so that the stacked near camera (with near clip plane of around 1000) will trigger OnWillRenderObject() callback
                new Vector3(-width, -10000.0f, height) // create 2nd plane at level -10000, so that the stacked near camera (with near clip plane of around 1000) will trigger OnWillRenderObject() callback
            };
            m.uv = new Vector2[] {
                new Vector2 (0, 0),
                new Vector2 (0, 1),
                new Vector2(1, 1),
                new Vector2 (1, 0),
                new Vector2 (0, 0), // from 2nd plane
                new Vector2 (0, 1), // from 2nd plane
                new Vector2(1, 1), // from 2nd plane
                new Vector2 (1, 0) // from 2nd plane
            };
            m.triangles = new int[] { 0, 1, 2, 0, 2, 3, /* here starts 2nd plane */ 4, 5, 6, 4, 6, 7 };
            m.RecalculateNormals();

            return m;
        }

        void Awake()
        {
            if (!DaggerfallUnity.Settings.Nystul_RealtimeReflections)
                return;

            reflectionPlaneBottom = new GameObject("ReflectionPlaneBottom");
            reflectionPlaneBottom.layer = LayerMask.NameToLayer("Water");
            MeshFilter meshFilter = (MeshFilter)reflectionPlaneBottom.AddComponent(typeof(MeshFilter));
            meshFilter.mesh = CreateMesh(100000.0f, 100000.0f); // create quad with normal facing into negative y-direction (so it is not visible but it will trigger OnWillRenderObject() in MirrorReflection.cs) - should be big enough to be "visible" even when looking parallel to the x/z-plane
            MeshRenderer renderer = reflectionPlaneBottom.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            
            renderer.material.shader = Shader.Find("Standard");
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.green);
            tex.Apply();
            renderer.material.mainTexture = tex;
            renderer.material.color = Color.green;
            renderer.enabled = true; // if this is set to false OnWillRenderObject() in MirrorReflection.cs will not work (workaround would be to change OnWillRenderObject() to Update()

            mirrorRefl = reflectionPlaneBottom.AddComponent<MirrorReflection>();
            mirrorRefl.m_TextureSize = 512;

            reflectionPlaneBottom.transform.SetParent(this.transform);

            InjectReflectiveMaterialProperty scriptInjectReflectiveMaterialProperty = reflectionPlaneBottom.AddComponent<InjectReflectiveMaterialProperty>(); // the inject script is parented to this plane so that the OnWillRenderObject() method of the inject script will work - this is important since update() function resulted in slightly delayed update which could be seen when ground level height changed
            scriptInjectReflectiveMaterialProperty.iniPathConfigInjectionTextures = this.iniPathConfigInjectionTextures;            

            reflectionPlaneSeaLevel = new GameObject("ReflectionPlaneSeaLevel");
            reflectionPlaneSeaLevel.layer = LayerMask.NameToLayer("Water");
            MeshFilter meshFilterSeaLevel = (MeshFilter)reflectionPlaneSeaLevel.AddComponent(typeof(MeshFilter));
            meshFilterSeaLevel.mesh = CreateMesh(1000000.0f, 1000000.0f); // create quad facing into negative y-direction (so it is not visible but it will trigger OnWillRenderObject() in MirrorReflection.cs) - should be big enough to be "visible" even when looking parallel to the x/z-plane
            MeshRenderer rendererSeaLevel = reflectionPlaneSeaLevel.AddComponent(typeof(MeshRenderer)) as MeshRenderer;


            rendererSeaLevel.material.shader = Shader.Find("Standard");
            Texture2D texSeaLevel = new Texture2D(1, 1);
            texSeaLevel.SetPixel(0, 0, Color.green);
            texSeaLevel.Apply();
            rendererSeaLevel.material.mainTexture = texSeaLevel;
            rendererSeaLevel.material.color = Color.green;
            rendererSeaLevel.enabled = true; // if this is set to false OnWillRenderObject() in MirrorReflection.cs will not work (workaround would be to change OnWillRenderObject() to Update()

            mirrorReflSeaLevel = reflectionPlaneSeaLevel.AddComponent<MirrorReflection>();
            mirrorReflSeaLevel.m_TextureSize = 512;

            reflectionPlaneSeaLevel.transform.SetParent(this.transform);

            LayerMask layerIndexWorldTerrain = LayerMask.NameToLayer("WorldTerrain");
            if (layerIndexWorldTerrain != -1)
            {
                mirrorRefl.m_ReflectLayers.value = (1 << LayerMask.NameToLayer("Default")) + (1 << LayerMask.NameToLayer("WorldTerrain"));
                mirrorReflSeaLevel.m_ReflectLayers = (1 << LayerMask.NameToLayer("Default")) + (1 << LayerMask.NameToLayer("WorldTerrain"));
            }
            else
            {
                mirrorRefl.m_ReflectLayers.value = 1 << LayerMask.NameToLayer("Default");
                mirrorReflSeaLevel.m_ReflectLayers = 1 << LayerMask.NameToLayer("Default");
            }

            componentDefferedPlanarReflections = GameManager.Instance.MainCameraObject.AddComponent<ReflectionsMod.DeferredPlanarReflections>();

            playerInside = GameManager.Instance.IsPlayerInside;

            PlayerEnterExit.OnTransitionInterior += OnTransitionToInterior;
            PlayerEnterExit.OnTransitionExterior += OnTransitionToExterior;
            PlayerEnterExit.OnTransitionDungeonInterior += OnTransitionToInterior;
            PlayerEnterExit.OnTransitionDungeonExterior += OnTransitionToExterior;
        }

        void OnDestroy()
        {
            if (!DaggerfallUnity.Settings.Nystul_RealtimeReflections)
                return;
            PlayerEnterExit.OnTransitionInterior -= OnTransitionToInterior;
            PlayerEnterExit.OnTransitionExterior -= OnTransitionToExterior;
            PlayerEnterExit.OnTransitionDungeonInterior -= OnTransitionToInterior;
            PlayerEnterExit.OnTransitionDungeonExterior -= OnTransitionToExterior;
        }

        void OnTransitionToInterior(PlayerEnterExit.TransitionEventArgs args)
        {
            mirrorRefl.m_ReflectLayers.value = 1 << LayerMask.NameToLayer("Default");
            mirrorReflSeaLevel.m_ReflectLayers = 1 << LayerMask.NameToLayer("Default");

            mirrorRefl.CurrentBackgroundSettings = MirrorReflection.EnvironmentSetting.IndoorSetting;
            mirrorReflSeaLevel.CurrentBackgroundSettings = MirrorReflection.EnvironmentSetting.IndoorSetting;
        }

        void OnTransitionToExterior(PlayerEnterExit.TransitionEventArgs args)
        {
            LayerMask layerIndexWorldTerrain = LayerMask.NameToLayer("WorldTerrain");
            if (layerIndexWorldTerrain != -1)
            {
                mirrorRefl.m_ReflectLayers.value = (1 << LayerMask.NameToLayer("Default")) + (1 << LayerMask.NameToLayer("WorldTerrain"));
                mirrorReflSeaLevel.m_ReflectLayers = (1 << LayerMask.NameToLayer("Default")) + (1 << LayerMask.NameToLayer("WorldTerrain"));
            }
            else
            {
                mirrorRefl.m_ReflectLayers.value = 1 << LayerMask.NameToLayer("Default");
                mirrorReflSeaLevel.m_ReflectLayers = 1 << LayerMask.NameToLayer("Default");
            }

            mirrorRefl.CurrentBackgroundSettings = MirrorReflection.EnvironmentSetting.OutdoorSetting;
            mirrorReflSeaLevel.CurrentBackgroundSettings = MirrorReflection.EnvironmentSetting.OutdoorSetting;
        }

        void Update()
        { 
            if (!DaggerfallUnity.Settings.Nystul_RealtimeReflections)
                return;

            GameObject goPlayerAdvanced = GameObject.Find("PlayerAdvanced");

            PlayerGPS playerGPS = GameObject.Find("PlayerAdvanced").GetComponent<PlayerGPS>();
            if (!playerGPS)
                return;

            if (GameManager.Instance.IsPlayerInside)
            {
                RaycastHit hit;
                float distanceToGround = 0;

                if (Physics.Raycast(goPlayerAdvanced.transform.position, -Vector3.up, out hit, 100.0F))
                {
                    distanceToGround = hit.distance;
                }                   
                reflectionPlaneBottom.transform.position = goPlayerAdvanced.transform.position - new Vector3(0.0f, distanceToGround, 0.0f);

                float distanceLevelBelow = getDistanceToLowerLevel(goPlayerAdvanced);
                //Debug.Log(string.Format("distance to lower level: {0}", distanceLevelBelow));
                reflectionPlaneSeaLevel.transform.position = goPlayerAdvanced.transform.position - new Vector3(0.0f, distanceLevelBelow, 0.0f);                
            }
            else
            //if (!GameManager.Instance.IsPlayerInside)
            {
                Terrain terrainInstancePlayerTerrain = null;

                int referenceLocationX = playerGPS.CurrentMapPixel.X;
                int referenceLocationY = playerGPS.CurrentMapPixel.Y;

                ContentReader.MapSummary mapSummary;
                // if there is no location at current player position...
                if (!DaggerfallUnity.Instance.ContentReader.HasLocation(referenceLocationX, referenceLocationY, out mapSummary))
                {                    
                    // search for largest location in local 8-neighborhood and take this as reference location for location reflection plane
                    int maxLocationArea = -1;
                    for (int y = -1; y <= +1; y++)
                    {
                        for (int x = -1; x <= +1; x++)
                        {
                            if (DaggerfallUnity.Instance.ContentReader.HasLocation(playerGPS.CurrentMapPixel.X + x, playerGPS.CurrentMapPixel.Y + y, out mapSummary))
                            {
                                DFLocation location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(mapSummary.RegionIndex, mapSummary.MapIndex);
                                byte locationRangeX = location.Exterior.ExteriorData.Width;
                                byte locationRangeY = location.Exterior.ExteriorData.Height;
                                int locationArea = locationRangeX * locationRangeY;

                                if (locationArea > maxLocationArea)
                                {
                                    referenceLocationX = playerGPS.CurrentMapPixel.X + x;
                                    referenceLocationY = playerGPS.CurrentMapPixel.Y + y;
                                    maxLocationArea = locationArea;
                                }       
                            }
                        }
                    }
                }

                GameObject go = GameObject.Find("StreamingTarget");
                if (go == null)
                    return;

                foreach (Transform child in go.transform)
                {
                    DaggerfallTerrain dfTerrain = child.GetComponent<DaggerfallTerrain>();
                    if (!dfTerrain)
                        continue;

                    if ((dfTerrain.MapPixelX != referenceLocationX) || (dfTerrain.MapPixelY != referenceLocationY))
                        continue;


                    Terrain terrainInstance = child.GetComponent<Terrain>();
                    terrainInstancePlayerTerrain = terrainInstance;

                    if ((terrainInstance) && (terrainInstance.terrainData))
                    {
                        float scale = terrainInstance.terrainData.heightmapScale.x;
                        float xSamplePos = DaggerfallUnity.Instance.TerrainSampler.HeightmapDimension * 0.55f;
                        float ySamplePos = DaggerfallUnity.Instance.TerrainSampler.HeightmapDimension * 0.55f;
                        Vector3 pos = new Vector3(xSamplePos * scale, 0, ySamplePos * scale);
                        float height = terrainInstance.SampleHeight(pos + terrainInstance.transform.position);

                        float positionY = height + terrainInstance.transform.position.y;
                        reflectionPlaneBottom.transform.position = new Vector3(goPlayerAdvanced.transform.position.x + terrainInstance.transform.position.x, positionY, goPlayerAdvanced.transform.position.z + terrainInstance.transform.position.z);
                    }                   
                }

                if (!terrainInstancePlayerTerrain)
                    return;

                //Debug.Log(string.Format("playerGPS: {0}, plane: {1}", goPlayerAdvanced.transform.position.y, reflectionPlaneBottom.transform.position.y));
                if (playerGPS.transform.position.y < reflectionPlaneBottom.transform.position.y)
                {
                    RaycastHit hit;
                    float distanceToGround = 0;

                    if (Physics.Raycast(goPlayerAdvanced.transform.position, -Vector3.up, out hit, 100.0F))
                    {
                        distanceToGround = hit.distance;
                    }

                    //Debug.Log(string.Format("distance to ground: {0}", distanceToGround));
                    reflectionPlaneBottom.transform.position = goPlayerAdvanced.transform.position - new Vector3(0.0f, distanceToGround, 0.0f);                    
                }

                StreamingWorld streamingWorld = GameObject.Find("StreamingWorld").GetComponent<StreamingWorld>();
                Vector3 vecWaterHeight = new Vector3(0.0f, (DaggerfallUnity.Instance.TerrainSampler.OceanElevation) * streamingWorld.TerrainScale, 0.0f); // water height level on y-axis (+1.0f some coastlines are incorrect otherwise)
                Vector3 vecWaterHeightTransformed = terrainInstancePlayerTerrain.transform.TransformPoint(vecWaterHeight); // transform to world coordinates
                //Debug.Log(string.Format("x,y,z: {0}, {1}, {2}", vecWaterHeight.x, vecWaterHeight.y, vecWaterHeight.z));
                //Debug.Log(string.Format("transformed x,y,z: {0}, {1}, {2}", vecWaterHeightTransformed.x, vecWaterHeightTransformed.y, vecWaterHeightTransformed.z));
                reflectionPlaneSeaLevel.transform.position = new Vector3(goPlayerAdvanced.transform.position.x, vecWaterHeightTransformed.y, goPlayerAdvanced.transform.position.z);
            }

            if (GameManager.Instance.IsPlayerInside && !playerInside)
            {
                playerInside = true; // player now inside

                mirrorRefl.CurrentBackgroundSettings = MirrorReflection.EnvironmentSetting.IndoorSetting;
                mirrorReflSeaLevel.CurrentBackgroundSettings = MirrorReflection.EnvironmentSetting.IndoorSetting;

                componentDefferedPlanarReflections.enabled = true;
            }
            else if (!GameManager.Instance.IsPlayerInside && playerInside)
            {
                playerInside = false; // player now outside

                mirrorRefl.CurrentBackgroundSettings = MirrorReflection.EnvironmentSetting.OutdoorSetting;
                mirrorReflSeaLevel.CurrentBackgroundSettings = MirrorReflection.EnvironmentSetting.OutdoorSetting;
                
                componentDefferedPlanarReflections.enabled = false;
            }
        }
	}
}