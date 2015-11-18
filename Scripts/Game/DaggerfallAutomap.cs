// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Nystul
// 
// Notes:
//

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game
{
    public class DaggerfallAutomap : MonoBehaviour
    {
        #region Fields

        const float scanRateGeometryDiscoveryInHertz = 10.0f; // n times per second the discovery of new geometry/meshes is checked

        GameObject gameobjectAutomap = null; // used to hold reference to instance of GameObject "Automap" (which has script Game/DaggerfallAutomap.cs attached)

        GameObject gameobjectGeometry = null; // used to hold reference to instance of GameObject with level geometry used for automap
        int layerAutomap; // layer used for level geometry of automap

        String oldGeometryName = "";

        GameObject gameObjectPlayerAdvanced = null; // used to hold reference to instance of GameObject "PlayerAdvanced"

        float slicingBiasPositionY; // bias from player y-position of geometry slice plane (set via Property SlicingBiasPositionY triggered by DaggerfallAutomapWindow script)

        bool isOpenAutomap = false; // flag that indicates if automap window is open (set via Property IsOpenAutomap triggered by DaggerfallAutomapWindow script)

        GameObject gameobjectBeacons = null; // collector GameObject to hold beacons
        GameObject gameobjectPlayerMarkerArrow = null; // GameObject which will hold player marker arrow
        GameObject gameobjectRayPlayerPos = null; // GameObject which will hold player marker ray (red ray)
        GameObject gameobjectRayEntrancePos = null; // GameObject which will hold (dungeon) entrance marker ray (green ray)
        GameObject gameobjectRayRotationPivotAxis = null; // GameObject which will hold rotation pivot axis ray (blue ray)

        readonly Vector3 rayPlayerPosOffset = new Vector3(-0.1f, 0.0f, +0.1f); // small offset to prevent ray for player position to be exactly in the same position as the rotation pivot axis
        readonly Vector3 rayEntrancePosOffset = new Vector3(0.1f, 0.0f, +0.1f); // small offset to prevent ray for dungeon entrance to be exactly in the same position as the rotation pivot axis

        DaggerfallWorkshop.Game.UserInterfaceWindows.DaggerfallAutomapWindow instanceDaggerfallAutomapWindow = null; // will hold reference to DaggerfallAutomapWindow class

        #endregion

        #region Properties

        /**
         * DaggerfallAutomapWindow script will use this to propagate its slicingBiasPositionY (y-offset from the player y position)
         */
        public float SlicingBiasPositionY
        {
            get { return (slicingBiasPositionY); }
            set { slicingBiasPositionY = value; }
        }

        /**
         * DaggerfallAutomapWindow script will use this to propagate if the automap window is open or not
         */
        public bool IsOpenAutomap
        {
            set { isOpenAutomap = value; }
        }

        #endregion

        #region Public Methods

        /**
         * DaggerfallAutomapWindow script will use this function to register itself with this script
         * @param instanceDaggerfallAutomapWindow the DaggerfallAutomapWindow to register
         */
        public void registerDaggerfallAutomapWindow(DaggerfallWorkshop.Game.UserInterfaceWindows.DaggerfallAutomapWindow instanceDaggerfallAutomapWindow)
        {
            this.instanceDaggerfallAutomapWindow = instanceDaggerfallAutomapWindow;
        }

        /**
         * DaggerfallAutomapWindow script will use this to signal this script to update when automap window was pushed - TODO: check if this can done with an event (if events work with gui windows)
         */
        public void updateAutomapStateOnWindowPush()
        {
            gameobjectGeometry.SetActive(true); // enable automap level geometry for revealing (so raycasts can hit colliders of automap level geometry)

            gameobjectPlayerMarkerArrow.transform.position = gameObjectPlayerAdvanced.transform.position;
            gameobjectPlayerMarkerArrow.transform.rotation = gameObjectPlayerAdvanced.transform.rotation;

            gameobjectRayPlayerPos.transform.position = gameObjectPlayerAdvanced.transform.position + rayPlayerPosOffset;

            updateSlicingPositionY();
        }

        /**
         * DaggerfallAutomapWindow script will use this to signal this script to update when automap window was popped - TODO: check if this can done with an event (if events work with gui windows)
         */
        public void updateAutomapStateOnWindowPop()
        {
            // about gameobjectGeometry.SetActive(false):
            // this will not be enough if we will eventually allow gui windows to be opened while exploring the world
            // then it will be necessary to either only disable the colliders on the automap level geometry or
            // make player collision ignore colliders of objects in automap layer - I would clearly prefer this option
            gameobjectGeometry.SetActive(false); // disable gameobjectGeometry so player movement won't be affected by geometry colliders of automap level geometry
        }

        /**
         * DaggerfallAutomapWindow script will use this to signal this script to update when anything changed that requires DaggerfallAutomap to update - TODO: check if this can done with an event (if events work with gui windows)
         */
        public void forceUpdate()
        {
            Update();
        }

        #endregion

        #region Unity

        void Awake()
        {
            gameObjectPlayerAdvanced = GameObject.Find("PlayerAdvanced");
            if (!gameObjectPlayerAdvanced)
            {
                DaggerfallUnity.LogMessage("GameObject \"PlayerAdvanced\" not found! in script DaggerfallAutomap (in function Awake())", true);
                if (Application.isEditor)
                    Debug.Break();
                else
                    Application.Quit();
            }

            layerAutomap = LayerMask.NameToLayer("Automap");
            if (layerAutomap == -1)
            {
                DaggerfallUnity.LogMessage("Layer with name \"Automap\" missing! Set it in Unity Editor under \"Edit/Project Settings/Tags and Layers!\"", true);
                if (Application.isEditor)
                    Debug.Break();
                else
                    Application.Quit();
            }
        }

        void OnDestroy()
        {

        }

        void OnEnable()
        {
            PlayerEnterExit.OnTransitionInterior += OnTransitionToInterior;
            PlayerEnterExit.OnTransitionDungeonInterior += OnTransitionToDungeonInterior;
        }

        void OnDisable()
        {
            PlayerEnterExit.OnTransitionInterior -= OnTransitionToInterior;
            PlayerEnterExit.OnTransitionDungeonInterior -= OnTransitionToDungeonInterior;
        }

        void Start()
        {
            gameobjectAutomap = GameObject.Find("Automap");
            if (gameobjectAutomap == null)
            {
                DaggerfallUnity.LogMessage("GameObject \"Automap\" missing! Create a GameObject called \"Automap\" in root of hierarchy and add script Game/DaggerfallAutomap!\"", true);
                if (Application.isEditor)
                    Debug.Break();
                else
                    Application.Quit();
            }

            StartCoroutine(CheckForNewlyDiscoveredMeshes()); // coroutine for periodically update discovery state of automap level geometry
        }

        void Update()
        {
            if (isOpenAutomap) // only do stuff if automap is indeed open
            {
                updateSlicingPositionY();

                if (instanceDaggerfallAutomapWindow != null) // do only if instance of DaggerfallAutomapWindow class registered itself (with function registerDaggerfallAutomapWindow())
                {
                    // update bias from initial position of rotation pivot axis
                    Vector3 biasRotationPivotAxisFromInitialPosition;
                    switch (instanceDaggerfallAutomapWindow.CurrentAutomapViewMode)
                    {
                        case DaggerfallWorkshop.Game.UserInterfaceWindows.DaggerfallAutomapWindow.AutomapViewMode.View2D:
                        default:
                            biasRotationPivotAxisFromInitialPosition = instanceDaggerfallAutomapWindow.BiasRotationPivotAxisFromInitialPositionViewFromTop;
                            break;
                        case DaggerfallWorkshop.Game.UserInterfaceWindows.DaggerfallAutomapWindow.AutomapViewMode.View3D:
                            biasRotationPivotAxisFromInitialPosition = instanceDaggerfallAutomapWindow.BiasRotationPivotAxisFromInitialPositionView3D;
                            break;
                    }
                    gameobjectRayRotationPivotAxis.transform.position = gameObjectPlayerAdvanced.transform.position + biasRotationPivotAxisFromInitialPosition;
                }
            }
        }

        #endregion

        #region Private Methods

        /**
         * does a raycast based hit test with additional protection raycast and marks automap level geometry meshes as discovered and visited in this entering/dungeon run
         */
        private void scanWithRaycastInDirectionAndUpdateMeshesAndMaterials(
            Vector3 rayStartPos,    ///< the start position for the detection raycast
            Vector3 rayDirection,   ///< the ray direction of the detection raycast
            float rayDistance,      ///< the ray distance used for the detection raycast
            Vector3 offsetSecondProtectionRaycast ///< offset for second protection raycast (is used to prevent discovery of geometry through gaps in the level geometry)
            )
        {
            RaycastHit hit1, hit2, hitTrueLevelGeometry1, hitTrueLevelGeometry2;

            // do raycast and protection raycast on main level geometry (use default layer as layer mask)
            bool didHitTrueLevelGeometry1 = Physics.Raycast(rayStartPos, rayDirection, out hitTrueLevelGeometry1, rayDistance, 1 << 0);
            bool didHitTrueLevelGeometry2 = Physics.Raycast(rayStartPos + offsetSecondProtectionRaycast, rayDirection, out hitTrueLevelGeometry2, rayDistance, 1 << 0);

            // main raycast (on Colliders in layer "Automap")
            bool didHit1 = Physics.Raycast(rayStartPos, rayDirection, out hit1, rayDistance, 1 << layerAutomap);
            // 2nd protection raycast (on Colliders in layer "Automap") with offset (protection against hole in daggerfall geometry prevention)
            bool didHit2 = Physics.Raycast(rayStartPos + offsetSecondProtectionRaycast, rayDirection, out hit2, rayDistance, 1 << layerAutomap);
            
            if (
                didHit1 &&
                didHit2 &&
                hit1.collider == hit2.collider &&  // hits must have same collider (TODO: check if there are no problems with small geometry)              
                didHitTrueLevelGeometry1 &&
                didHitTrueLevelGeometry2 &&
                // hits on true geometry must have same distance as hits on automap geometry - otherwise there is a obstacle, e.g. a door
                Math.Abs(hitTrueLevelGeometry1.distance - hit1.distance) < 0.01f &&
                Math.Abs(hitTrueLevelGeometry2.distance - hit2.distance) < 0.01f
                )
            {
                MeshCollider meshCollider = hit1.collider as MeshCollider;
                if (meshCollider != null)
                {
                    meshCollider.gameObject.GetComponent<MeshRenderer>().enabled = true; // mark mesh renderer as discovered (by enabling it)

                    Material[] mats = meshCollider.gameObject.GetComponent<MeshRenderer>().materials;
                    foreach (Material mat in mats)
                    {
                        mat.SetFloat("_VisitedInThisEntering", 1.0f); // mark material as visited in this entrance/dungeon run
                    }
                    meshCollider.gameObject.GetComponent<MeshRenderer>().materials = mats;
                }
            }

        }

        /*
         * basic automap level geometry revealing functionality - this function is periodically invoked
         */
        IEnumerator CheckForNewlyDiscoveredMeshes()
        {
            while (true)
            {
                if ((gameobjectGeometry != null) && ((GameManager.Instance.IsPlayerInsideDungeon) || (GameManager.Instance.IsPlayerInsidePalace)))
                {
                    // enable automap level geometry for revealing (so raycasts can hit colliders of automap level geometry)
                    gameobjectGeometry.SetActive(true);

                    // disable beacons to prevent their colliders
                    gameobjectPlayerMarkerArrow.gameObject.SetActive(false);
                    gameobjectRayPlayerPos.gameObject.SetActive(false);
                    gameobjectRayEntrancePos.gameObject.SetActive(false);
                    gameobjectRayRotationPivotAxis.gameObject.SetActive(false);

                    if ((GameManager.Instance.IsPlayerInsideDungeon) || (GameManager.Instance.IsPlayerInsidePalace))
                    {
                        // reveal geometry right below player - raycast down from player head position
                        Vector3 rayStartPos = gameObjectPlayerAdvanced.transform.position + Camera.main.transform.localPosition;
                        Vector3 rayDirection = Vector3.down;
                        float rayDistance = 3.0f; // 3 meters should be enough (note: flying to high will result in geometry not being revealed by this raycast
                        Vector3 offsetSecondProtectionRaycast = Vector3.left * 0.1f; // will be used for protection raycast with slight offset of 10cm (protection against hole in daggerfall geometry prevention)            
                        scanWithRaycastInDirectionAndUpdateMeshesAndMaterials(rayStartPos, rayDirection, rayDistance, offsetSecondProtectionRaycast);

                        // reveal geometry which player is looking at (and which is near enough)
                        rayDirection = Camera.main.transform.rotation * Vector3.forward;
                        // shift 10cm to the side (computed by normalized cross product of forward vector of view direction and down vector of view direction)
                        offsetSecondProtectionRaycast = Vector3.Normalize(Vector3.Cross(Camera.main.transform.rotation * Vector3.down, rayDirection)) * 0.1f;
                        rayDistance = 25.0f;
                        scanWithRaycastInDirectionAndUpdateMeshesAndMaterials(rayStartPos, rayDirection, rayDistance, offsetSecondProtectionRaycast);
                    }

                    // enable previously disabled beacons
                    gameobjectPlayerMarkerArrow.gameObject.SetActive(true);
                    gameobjectRayPlayerPos.gameObject.SetActive(true);
                    gameobjectRayEntrancePos.gameObject.SetActive(true);
                    gameobjectRayRotationPivotAxis.gameObject.SetActive(true);

                    // disable gameobjectGeometry so player movement won't be affected by geometry colliders of automap level geometry
                    gameobjectGeometry.SetActive(false);
                }
                yield return new WaitForSeconds(1.0f / scanRateGeometryDiscoveryInHertz);
            }
        }

        /*
         * sets layer of a GameObject and all of its childs recursively
         * @param obj the target GameObject
         * @param layer the layer to be set
         */
        private static void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }

        /*
         * updates materials of mesh renderer
         * (this injects the automap shader and sets the state for materials to be rendered dependent
         * on if they where revealed already in a previous dungeon run)
         * @param meshRenderer the MeshRenderer whose materials needs to be updated
         * @param visitedInThisEntering indicates if the materials of meshRenderer should be marked as "visited in this entering/dungeon run" (rendered in color) or not (rendered in grayscale)
         */
        private void updateMaterialsOfMeshRenderer(MeshRenderer meshRenderer, bool visitedInThisEntering = false)
        {
            Vector3 playerAdvancedPos = gameObjectPlayerAdvanced.transform.position;
            //meshRenderer.enabled = false;
            Material[] newMaterials = new Material[meshRenderer.materials.Length];
            for (int i = 0; i < meshRenderer.materials.Length; i++)
            {
                Material material = meshRenderer.materials[i];
                Material newMaterial = newMaterials[i];

                newMaterial = new Material(Shader.Find("Daggerfall/Automap"));
                //newMaterial.CopyPropertiesFromMaterial(material);
                newMaterial.name = "Automap injected for: " + material.name;
                Texture mainTex = material.GetTexture("_MainTex");
                newMaterial.SetTexture("_MainTex", mainTex);
                Texture bumpMapTex = material.GetTexture("_BumpMap");
                newMaterial.SetTexture("_BumpMap", bumpMapTex);
                Texture emissionMapTex = material.GetTexture("_EmissionMap");
                newMaterial.SetTexture("_EmissionMap", emissionMapTex);
                Color emissionColor = material.GetColor("_EmissionColor");
                newMaterial.SetColor("_EmissionColor", emissionColor);
                Vector4 playerPosition = new Vector4(playerAdvancedPos.x, playerAdvancedPos.y + Camera.main.transform.localPosition.y, playerAdvancedPos.z, 0.0f);
                newMaterial.SetVector("_PlayerPosition", playerPosition);
                if (visitedInThisEntering == true)
                    newMaterial.SetFloat("_VisitedInThisEntering", 1.0f);
                else
                    newMaterial.SetFloat("_VisitedInThisEntering", 0.0f);
                newMaterials[i] = newMaterial;
            }
            meshRenderer.materials = newMaterials;
            //meshRenderer.enabled = true;
        }

        /*
         * will inject materials and properties to MeshRenderer in the proper hierarchy level of automap level geometry GameObject
         * note: the proper hierarchy level differs between an "Interior" and a "Dungeon" geometry GameObject
         * @param resetDiscoveryState if true resets the discovery state for geometry that needs to be discovered (when inside dungeons or palaces)
         */
        private void injectMeshAndMaterialProperties(bool resetDiscoveryState = true)
        {
            if (GameManager.Instance.IsPlayerInsideBuilding)
            {
                // find all MeshRenderers in 3rd hierarchy level
                foreach (Transform elem in gameobjectGeometry.transform)
                {
                    foreach (Transform innerElem in elem.gameObject.transform)
                    {
                        foreach (Transform inner2Elem in innerElem.gameObject.transform)
                        {
                            MeshRenderer meshRenderer = inner2Elem.gameObject.GetComponent<MeshRenderer>();
                            if (meshRenderer == null)
                                break;

                            // update materials and set meshes as visited in this run (so "Interior" geometry always is colored
                            // (since we don't disable the mesh, it is also discovered - which is a precondition for being rendered))
                            updateMaterialsOfMeshRenderer(meshRenderer, true);                            
                        }
                    }
                }
            }
            else if ((GameManager.Instance.IsPlayerInsideDungeon)||(GameManager.Instance.IsPlayerInsidePalace))
            {
                // find all MeshRenderers in 4th hierarchy level
                foreach (Transform elem in gameobjectGeometry.transform)
                {
                    foreach (Transform innerElem in elem.gameObject.transform)
                    {
                        foreach (Transform inner2Elem in innerElem.gameObject.transform)
                        {
                            foreach (Transform inner3Elem in inner2Elem.gameObject.transform)
                            {
                                MeshRenderer meshRenderer = inner3Elem.gameObject.GetComponent<MeshRenderer>();
                                if (meshRenderer == null)
                                    break;

                                // update materials (omit 2nd parameter so default behavior is initiated which is:
                                // meshes are marked as not visited in this run (so "Dungeon" geometry that has been discovered in a previous dungeon run is rendered in grayscale)
                                updateMaterialsOfMeshRenderer(meshRenderer);

                                if (resetDiscoveryState) // if forced reset of discovery state
                                {
                                    // mark meshRenderer as undiscovered
                                    meshRenderer.enabled = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void doInitialSetupForGeometryCreation()
        {
            if (!gameobjectBeacons)
            {
                gameobjectBeacons = new GameObject("Beacons");
                gameobjectBeacons.layer = layerAutomap;
                gameobjectBeacons.transform.SetParent(gameobjectAutomap.transform);
            }
            if (!gameobjectPlayerMarkerArrow)
            {
                gameobjectPlayerMarkerArrow = GameObjectHelper.CreateDaggerfallMeshGameObject(99900, gameobjectBeacons.transform, false, null, true);
                gameobjectPlayerMarkerArrow.name = "PlayerMarkerArrow";
                gameobjectPlayerMarkerArrow.layer = layerAutomap;
            }
            gameobjectPlayerMarkerArrow.transform.position = gameObjectPlayerAdvanced.transform.position;
            gameobjectPlayerMarkerArrow.transform.rotation = gameObjectPlayerAdvanced.transform.rotation;

            if (!gameobjectRayPlayerPos)
            {
                gameobjectRayPlayerPos = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                UnityEngine.Object.DestroyImmediate(gameobjectRayPlayerPos.GetComponent<Collider>());
                gameobjectRayPlayerPos.name = "RayPlayerPosition";
                gameobjectRayPlayerPos.transform.SetParent(gameobjectBeacons.transform);
                gameobjectRayPlayerPos.layer = layerAutomap;                
                gameobjectRayPlayerPos.transform.localScale = new Vector3(0.3f, 50.0f, 0.3f);
                Material material = new Material(Shader.Find("Standard"));
                material.color = new Color(1.0f, 0.0f, 0.0f);
                gameobjectRayPlayerPos.GetComponent<MeshRenderer>().material = material;
            }
            gameobjectRayPlayerPos.transform.position = gameObjectPlayerAdvanced.transform.position + rayPlayerPosOffset;

            if (!gameobjectRayRotationPivotAxis)
            {
                gameobjectRayRotationPivotAxis = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                UnityEngine.Object.DestroyImmediate(gameobjectRayRotationPivotAxis.GetComponent<Collider>());
                gameobjectRayRotationPivotAxis.name = "RayRotationPivotAxis";
                gameobjectRayRotationPivotAxis.transform.SetParent(gameobjectBeacons.transform);
                gameobjectRayRotationPivotAxis.layer = layerAutomap;                
                gameobjectRayRotationPivotAxis.transform.localScale = new Vector3(0.3f, 50.0f, 0.3f);
                Material material = new Material(Shader.Find("Standard"));
                material.color = new Color(0.0f, 0.0f, 1.0f);
                gameobjectRayRotationPivotAxis.GetComponent<MeshRenderer>().material = material;
            }
            gameobjectRayRotationPivotAxis.transform.position = gameObjectPlayerAdvanced.transform.position;

            if (!gameobjectRayEntrancePos)
            {
                gameobjectRayEntrancePos = new GameObject("EntracePositionMarker");
                gameobjectRayEntrancePos.transform.SetParent(gameobjectBeacons.transform);
                gameobjectRayEntrancePos.layer = layerAutomap;

                GameObject gameobjectRay = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                UnityEngine.Object.DestroyImmediate(gameobjectRay.GetComponent<Collider>());
                gameobjectRay.name = "RayEntracePositionMarker";
                gameobjectRay.transform.SetParent(gameobjectRayEntrancePos.transform);
                gameobjectRay.layer = layerAutomap;
                gameobjectRay.transform.localScale = new Vector3(0.3f, 50.0f, 0.3f);
                Material material = new Material(Shader.Find("Standard"));
                material.color = new Color(0.0f, 1.0f, 0.0f);
                gameobjectRay.GetComponent<MeshRenderer>().material = material;

                GameObject gameObjectCubeMarker = GameObject.CreatePrimitive(PrimitiveType.Cube);
                UnityEngine.Object.DestroyImmediate(gameObjectCubeMarker.GetComponent<Collider>());
                gameObjectCubeMarker.name = "CubeEntracePositionMarker";
                gameObjectCubeMarker.transform.SetParent(gameobjectRayEntrancePos.transform);
                gameObjectCubeMarker.GetComponent<MeshRenderer>().material = material;
                gameObjectCubeMarker.layer = layerAutomap;
                gameObjectCubeMarker.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }
            gameobjectRayEntrancePos.transform.position = gameObjectPlayerAdvanced.transform.position + rayEntrancePosOffset;
        }

        /**
         * update y-position of place of slicing automap level geometry
         */
        private void updateSlicingPositionY()
        {
            float slicingPositionY = gameObjectPlayerAdvanced.transform.position.y + Camera.main.transform.localPosition.y + slicingBiasPositionY;
            Shader.SetGlobalFloat("_SclicingPositionY", slicingPositionY);
        }

        private void createIndoorGeometryForAutomap(PlayerEnterExit.TransitionEventArgs args)
        {
            StaticDoor door = args.StaticDoor;
            String newGeometryName = string.Format("DaggerfallInterior [Block={0}, Record={1}]", door.blockIndex, door.recordIndex);
            if (gameobjectGeometry != null)
            {
                if (oldGeometryName != newGeometryName)
                {
                    UnityEngine.Object.DestroyImmediate(gameobjectGeometry);
                }
                else
                {
                    injectMeshAndMaterialProperties(false);
                    return;
                }
            }

            gameobjectGeometry = new GameObject("GeometryAutomap (Interior)");

            doInitialSetupForGeometryCreation();

            foreach (Transform elem in GameManager.Instance.InteriorParent.transform)
            {
                if (elem.name.Contains("DaggerfallInterior"))
                {
                    // Get climate
                    ClimateBases climateBase = ClimateBases.Temperate;
                    climateBase = ClimateSwaps.FromAPIClimateBase(GameManager.Instance.PlayerGPS.ClimateSettings.ClimateType);

                    // Layout interior
                    
                    GameObject gameobjectInterior = new GameObject(newGeometryName);
                    DaggerfallInterior interior = gameobjectInterior.AddComponent<DaggerfallInterior>();

                    interior.DoLayoutAutomap(null, door, climateBase);

                    gameobjectInterior.transform.SetParent(gameobjectGeometry.transform);

                    gameobjectGeometry.transform.position = elem.transform.position;
                    gameobjectGeometry.transform.rotation = elem.transform.rotation;
                }
            }

            SetLayerRecursively(gameobjectGeometry, layerAutomap);
            gameobjectGeometry.transform.SetParent(gameobjectAutomap.transform);

            injectMeshAndMaterialProperties();

            oldGeometryName = newGeometryName;
        }

        private void createDungeonGeometryForAutomap()
        {
            DFLocation location = GameManager.Instance.PlayerGPS.CurrentLocation;
            String newGeometryName = string.Format("DaggerfallDungeon [Region={0}, Name={1}]", location.RegionName, location.Name);
            if (gameobjectGeometry != null)
            {
                if (oldGeometryName != newGeometryName)
                {
                    UnityEngine.Object.DestroyImmediate(gameobjectGeometry);
                }
                else
                {
                    injectMeshAndMaterialProperties(false);
                    return;
                }
            }

            gameobjectGeometry = new GameObject("GeometryAutomap (Dungeon)");

            doInitialSetupForGeometryCreation();

            DaggerfallUnity.Instance.Option_CombineRDB = false;

            foreach (Transform elem in GameManager.Instance.DungeonParent.transform)
            {
                if (elem.name.Contains("DaggerfallDungeon"))
                {
                    GameObject gameobjectDungeon = new GameObject(newGeometryName);

                    // Create dungeon layout
                    foreach (DFLocation.DungeonBlock block in location.Dungeon.Blocks)
                    {
                        if (location.Name == "Orsinium")
                        {
                            if (block.X == -1 && block.Z == -1 && block.BlockName == "N0000065.RDB")
                                continue;
                        }

                        DFBlock blockData;
                        int[] textureTable = null;
                        GameObject gameobjectBlock = RDBLayout.CreateBaseGameObject(block.BlockName, null, out blockData, textureTable, true, null, false);
                        //gameobjectBlock.transform.parent = this.transform;
                        gameobjectBlock.transform.position = new Vector3(block.X * RDBLayout.RDBSide, 0, block.Z * RDBLayout.RDBSide);

                        gameobjectBlock.transform.SetParent(gameobjectDungeon.transform);

                        //RDBLayout.AddActionDoors(gameobjectBlock, null, ref blockData, null, false);
                    }

                    gameobjectDungeon.transform.SetParent(gameobjectGeometry.transform);

                    gameobjectGeometry.transform.position = elem.transform.position;
                    gameobjectGeometry.transform.rotation = elem.transform.rotation;

                    break;
                }
            }

            DaggerfallUnity.Instance.Option_CombineRDB = true;

            SetLayerRecursively(gameobjectGeometry, layerAutomap);
            gameobjectGeometry.transform.SetParent(gameobjectAutomap.transform);

            injectMeshAndMaterialProperties();

            oldGeometryName = newGeometryName;
        }

        private void OnTransitionToInterior(PlayerEnterExit.TransitionEventArgs args)
        {
            createIndoorGeometryForAutomap(args);
        }

        private void OnTransitionToDungeonInterior(PlayerEnterExit.TransitionEventArgs args)
        {
            createDungeonGeometryForAutomap();
        }

        #endregion
    }
}