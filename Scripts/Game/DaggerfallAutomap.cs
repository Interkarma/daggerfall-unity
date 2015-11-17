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

        GameObject gameObjectPlayerAdvanced = null; // used to hold reference to instance of GameObject "PlayerAdvanced"

        float slicingBiasPositionY; // bias from player y-position of geometry slice plane (set via Property SlicingBiasPositionY triggered by DaggerfallAutomapWindow script)

        bool isOpenAutomap = false; // flag that indicates if automap window is open (set via Property IsOpenAutomap triggered by DaggerfallAutomapWindow script)

        GameObject gameobjectPlayerMarkerArrow = null; // GameObject which will hold player marker arrow

        GameObject gameobjectRayPlayerPos = null; // GameObject which will hold player marker ray (red ray)
        GameObject gameobjectRayEntrancePos = null; // GameObject which will hold (dungeon) entrance marker ray (green ray)
        GameObject gameobjectRayRotationPivotAxis = null; // GameObject which will hold rotation pivot axis ray (blue ray)

        readonly Vector3 rayPlayerPosOffset = new Vector3(-0.1f, 0.0f, +0.1f); // small offset to prevent ray for player position to be exactly in the same position as the rotation pivot axis
        readonly Vector3 rayEntrancePosOffset = new Vector3(0.1f, 0.0f, +0.1f); // small offset to prevent ray for dungeon entrance to be exactly in the same position as the rotation pivot axis

        DaggerfallWorkshop.Game.UserInterfaceWindows.DaggerfallAutomapWindow instanceDaggerfallAutomapWindow = null; // will hold reference to DaggerfallAutomapWindow class

        #endregion

        #region Properties

        // DaggerfallAutomapWindow script will use this to propagate its slicingBiasPositionY (y-offset from the player y position)
        public float SlicingBiasPositionY
        {
            get { return (slicingBiasPositionY); }
            set { slicingBiasPositionY = value; }
        }

        // DaggerfallAutomapWindow script will use this to propagate if the automap window is open or not
        public bool IsOpenAutomap
        {
            set { isOpenAutomap = value; }
        }

        #endregion

        #region Public Methods

        // DaggerfallAutomapWindow script will use this function to register itself with this script
        public void registerDaggerfallAutomapWindow(DaggerfallWorkshop.Game.UserInterfaceWindows.DaggerfallAutomapWindow instanceDaggerfallAutomapWindow)
        {
            this.instanceDaggerfallAutomapWindow = instanceDaggerfallAutomapWindow;
        }

        // DaggerfallAutomapWindow script will use this to signal this script to update when automap window was pushed - TODO: check if this can done with an event (if events work with gui windows)
        public void updateAutomapStateOnWindowPush()
        {
            gameobjectPlayerMarkerArrow.transform.position = gameObjectPlayerAdvanced.transform.position;
            gameobjectPlayerMarkerArrow.transform.rotation = gameObjectPlayerAdvanced.transform.rotation;

            gameobjectRayPlayerPos.transform.position = gameObjectPlayerAdvanced.transform.position + rayPlayerPosOffset;

            updateSlicingPositionY();
        }

        // DaggerfallAutomapWindow script will use this to signal this script to update when anything changed that requires DaggerfallAutomap to update - TODO: check if this can done with an event (if events work with gui windows)
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

            StartCoroutine(CheckForNewlyDiscoveredMeshes());
        }

        void Update()
        {
            if (isOpenAutomap) // only do stuff if automap is indeed open
            {
                updateSlicingPositionY();

                if (instanceDaggerfallAutomapWindow != null)
                {
                    Vector3 biasFromInitialPosition;
                    switch (instanceDaggerfallAutomapWindow.CurrentAutomapViewMode)
                    {
                        case DaggerfallWorkshop.Game.UserInterfaceWindows.DaggerfallAutomapWindow.AutomapViewMode.View2D:
                        default:
                            biasFromInitialPosition = instanceDaggerfallAutomapWindow.BiasFromInitialPositionViewFromTop;
                            break;
                        case DaggerfallWorkshop.Game.UserInterfaceWindows.DaggerfallAutomapWindow.AutomapViewMode.View3D:
                            biasFromInitialPosition = instanceDaggerfallAutomapWindow.BiasFromInitialPositionView3D;
                            break;
                    }
                    gameobjectRayRotationPivotAxis.transform.position = gameObjectPlayerAdvanced.transform.position + biasFromInitialPosition;
                }
            }
        }

        #endregion

        #region Private Methods

        IEnumerator CheckForNewlyDiscoveredMeshes()
        {
            while (true)
            {
                if (gameobjectGeometry != null)
                {
                    gameobjectPlayerMarkerArrow.gameObject.SetActive(false);
                    gameobjectRayPlayerPos.gameObject.SetActive(false);
                    gameobjectRayEntrancePos.gameObject.SetActive(false);
                    gameobjectRayRotationPivotAxis.gameObject.SetActive(false);

                    if ((GameManager.Instance.IsPlayerInsideDungeon) || (GameManager.Instance.IsPlayerInsidePalace))
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(gameObjectPlayerAdvanced.transform.position + Camera.main.transform.localPosition, Vector3.down, out hit, 1000.0f, 1 << layerAutomap))
                        {
                            MeshCollider meshCollider = hit.collider as MeshCollider;
                            if (meshCollider != null)
                            {
                                meshCollider.gameObject.GetComponent<MeshRenderer>().enabled = true;
                            }
                        }
                    }

                    gameobjectPlayerMarkerArrow.gameObject.SetActive(true);
                    gameobjectRayPlayerPos.gameObject.SetActive(true);
                    gameobjectRayEntrancePos.gameObject.SetActive(true);
                    gameobjectRayRotationPivotAxis.gameObject.SetActive(true);
                }

                yield return new WaitForSeconds(1.0f / scanRateGeometryDiscoveryInHertz);
            }
        }

        private static void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }

        private void updateMaterialsFromRenderer(MeshRenderer meshRenderer)
        {
            Vector3 playerAdvancedPos = gameObjectPlayerAdvanced.transform.position;
            meshRenderer.enabled = false;
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
                newMaterials[i] = newMaterial;
            }
            meshRenderer.materials = newMaterials;
            meshRenderer.enabled = true;
        }

        private void injectMeshAndMaterialProperties()
        {
            if (GameManager.Instance.IsPlayerInsideBuilding)
            {
                foreach (Transform elem in gameobjectGeometry.transform)
                {
                    //Debug.Log(String.Format("name: {0}", elem.name));
                    foreach (Transform innerElem in elem.gameObject.transform)
                    {
                        foreach (Transform inner2Elem in innerElem.gameObject.transform)
                        {
                            MeshRenderer meshRenderer = inner2Elem.gameObject.GetComponent<MeshRenderer>();
                            if (meshRenderer == null)
                                break;

                            updateMaterialsFromRenderer(meshRenderer);
                        }
                    }
                }
            }
            else if ((GameManager.Instance.IsPlayerInsideDungeon)||(GameManager.Instance.IsPlayerInsidePalace))
            {
                foreach (Transform elem in gameobjectGeometry.transform)
                {
                    //Debug.Log(String.Format("name: {0}", elem.name));
                    foreach (Transform innerElem in elem.gameObject.transform)
                    {
                        foreach (Transform inner2Elem in innerElem.gameObject.transform)
                        {
                            foreach (Transform inner3Elem in inner2Elem.gameObject.transform)
                            {
                                MeshRenderer meshRenderer = inner3Elem.gameObject.GetComponent<MeshRenderer>();
                                if (meshRenderer == null)
                                    break;

                                updateMaterialsFromRenderer(meshRenderer);

                                meshRenderer.enabled = false;
                            }
                        }
                    }
                }
            }
        }

        private void doInitialSetupForGeometryCreation()
        {
            if (!gameobjectPlayerMarkerArrow)
            {
                gameobjectPlayerMarkerArrow = GameObjectHelper.CreateDaggerfallMeshGameObject(99900, gameobjectAutomap.transform, false, null, true);
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
                gameobjectRayPlayerPos.transform.SetParent(gameobjectAutomap.transform);
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
                gameobjectRayRotationPivotAxis.transform.SetParent(gameobjectAutomap.transform);
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
                gameobjectRayEntrancePos.transform.SetParent(gameobjectAutomap.transform);
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

        private void updateSlicingPositionY()
        {
            float slicingPositionY = gameObjectPlayerAdvanced.transform.position.y + Camera.main.transform.localPosition.y + slicingBiasPositionY;
            Shader.SetGlobalFloat("_SclicingPositionY", slicingPositionY);
        }

        private void createIndoorGeometryForAutomap(PlayerEnterExit.TransitionEventArgs args)
        {
            if (gameobjectGeometry != null)
            {
                UnityEngine.Object.DestroyImmediate(gameobjectGeometry);
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
                    StaticDoor door = args.StaticDoor;
                    GameObject gameobjectInterior = new GameObject(string.Format("DaggerfallInterior [Block={0}, Record={1}]", door.blockIndex, door.recordIndex));
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
        }

        private void createDungeonGeometryForAutomap()
        {
            if (gameobjectGeometry != null)
            {
                UnityEngine.Object.DestroyImmediate(gameobjectGeometry);
            }

            gameobjectGeometry = new GameObject("GeometryAutomap (Dungeon)");

            doInitialSetupForGeometryCreation();

            DaggerfallUnity.Instance.Option_CombineRDB = false;

            foreach (Transform elem in GameManager.Instance.DungeonParent.transform)
            {
                if (elem.name.Contains("DaggerfallDungeon"))
                {
                    DFLocation location = GameManager.Instance.PlayerGPS.CurrentLocation;

                    GameObject gameobjectDungeon = new GameObject(string.Format("DaggerfallDungeon [Region={0}, Name={1}]", location.RegionName, location.Name));

                    // Create dungeon layout
                    foreach (DFLocation.DungeonBlock block in location.Dungeon.Blocks)
                    {
                        if (location.Name == "Orsinium")
                        {
                            if (block.X == -1 && block.Z == -1 && block.BlockName == "N0000065.RDB")
                                continue;
                        }

                        GameObject go = RDBLayout.CreateBaseGameObject(block.BlockName, null, null, true, null, false);

                        go.transform.parent = this.transform;
                        go.transform.position = new Vector3(block.X * RDBLayout.RDBSide, 0, block.Z * RDBLayout.RDBSide);

                        go.transform.SetParent(gameobjectDungeon.transform);
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