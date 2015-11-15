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

        GameObject gameobjectAutomap = null;

        GameObject gameobjectGeometry = null;
        int layerAutomap; // layer used for geometry of automap

        #endregion

        #region Properties
        #endregion

        #region Unity

        void Awake()
        {
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
        }

        #endregion

        #region Private Methods

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
            meshRenderer.enabled = false;
            Material[] newMaterials = new Material[meshRenderer.materials.Length];
            for (int i = 0; i < meshRenderer.materials.Length; i++)
            {
                Debug.Log("here...");
                Material material = meshRenderer.materials[i];
                Material newMaterial = newMaterials[i];

                newMaterial = new Material(Shader.Find("Daggerfall/Automap"));
                newMaterial.CopyPropertiesFromMaterial(material);
                newMaterial.name = "Automap injected for: " + material.name;
                //Texture mainTex = material.GetTexture("_MainTex");
                //newMaterial.SetTexture("_MainTex", mainTex);
                //Texture bumpMapTex = material.GetTexture("_BumpMap");
                //newMaterial.SetTexture("_BumpMap", bumpMapTex);
                //Texture emissionMapTex = material.GetTexture("_EmissionMap");
                //newMaterial.SetTexture("_EmissionMap", emissionMapTex);
                //Color emissionColor = material.GetColor("_EmissionColor");
                //newMaterial.SetColor("_EmissionColor", emissionColor);
                newMaterials[i] = newMaterial;
            }
            meshRenderer.materials = newMaterials;
            meshRenderer.enabled = true;
        }
        private void injectCustomAutomapShaderForMaterials()
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
                            }
                        }
                    }
                }
            }
        }

        private void createIndoorGeometryForAutomap(PlayerEnterExit.TransitionEventArgs args)
        {
            if (gameobjectGeometry != null)
            {
                UnityEngine.Object.DestroyImmediate(gameobjectGeometry);
            }

            gameobjectGeometry = new GameObject("GeometryAutomap (Interior)");

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

            injectCustomAutomapShaderForMaterials();
        }

        private void createDungeonGeometryForAutomap()
        {
            if (gameobjectGeometry != null)
            {
                UnityEngine.Object.DestroyImmediate(gameobjectGeometry);
            }

            gameobjectGeometry = new GameObject("GeometryAutomap (Dungeon)");

            foreach (Transform elem in GameManager.Instance.DungeonParent.transform)
            {
                if (elem.name.Contains("DaggerfallDungeon"))
                {
                    DFLocation location = GameManager.Instance.PlayerGPS.CurrentLocation;

                    GameObject gameobjectDungeon = new GameObject(string.Format("DaggerfallDungeon [Region={0}, Name={1}]", location.RegionName, location.Name));
                    //DaggerfallDungeon dungeon = gameobjectDungeon.AddComponent<DaggerfallDungeon>();

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

            SetLayerRecursively(gameobjectGeometry, layerAutomap);
            gameobjectGeometry.transform.SetParent(gameobjectAutomap.transform);

            injectCustomAutomapShaderForMaterials();
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