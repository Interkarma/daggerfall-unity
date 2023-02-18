// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using DaggerfallConnect;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using DaggerfallWorkshop.Game.Addons.RmbBlockEditor.BuildingPresets;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    /// <summary>
    /// MapBlock editor
    /// </summary>
    public class RmbBlockEditorWindow : EditorWindow
    {
        private const string WorldDataFolder = "/StreamingAssets/WorldData/";
        private GameObject root;
        private GameObject light;
        private GroundEditor groundEditor;
        private SceneryEditor sceneryEditor;
        private AutomapEditor automapEditor;
        private DFBlock loadedBlock;
        private bool fileLoaded;

        private AddBuilding addBuilding;
        private Add3d add3dElement;
        private AddFlat addFlat;

        private SettingsEditor settingsEditor;

        [MenuItem("Daggerfall Tools/RMB Block Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<RmbBlockEditorWindow>();
            window.titleContent = new GUIContent("RMB Block Editor");
        }

        private void OnEnable()
        {
            var original =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/RmbBlockEditorWindowTemplate.uxml");
            var treeAsset = original.CloneTree();
            rootVisualElement.Add(treeAsset);
            var stylesheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/RmbBlockEditorWindowStyles.uss");
            rootVisualElement.styleSheets.Add(stylesheet);

            fileLoaded = false;

            addBuilding = new AddBuilding();
            add3dElement = new Add3d();
            addFlat = new AddFlat();
            settingsEditor = new SettingsEditor();

            RenderMenus();
        }

        private void OnDestroy()
        {
            rootVisualElement.Clear();
        }

        private void OnRemoveRoot()
        {
            // Clear the editor
            var contentContainer = rootVisualElement.Query<VisualElement>("content").First();
            contentContainer.Clear();

            // Clear Sub-editors
            addBuilding.Destroy();
            add3dElement.Destroy();
            addFlat.Destroy();

            // Disable menus
            fileLoaded = false;
        }

        private void OnModify3d(uint modelId)
        {
            var rmbBlockObject = root.GetComponent<RmbBlockObject>();
            var contentContainer = rootVisualElement.Query<VisualElement>("content").First();
            contentContainer.Clear();
            var modify3dEditor = new ModifyMisc3dEditor(modelId, OnApply3dModification);
            contentContainer.Add(modify3dEditor.Render(rmbBlockObject.Climate, rmbBlockObject.Season,
                rmbBlockObject.WindowStyle));
        }

        private void OnApply3dModification(uint modelId)
        {
            var rmbBlockObject = root.GetComponent<RmbBlockObject>();
            var selected = Selection.activeGameObject;
            var currentMisc3d = selected.GetComponent<Misc3d>();
            var record = currentMisc3d.GetRecord();
            record.ModelIdNum = modelId;
            record.ModelId = modelId.ToString();

            try
            {
                var newGo = RmbBlockHelper.Add3dObject(record, rmbBlockObject.Climate, rmbBlockObject.Season,
                    rmbBlockObject.WindowStyle);
                newGo.transform.parent = selected.transform.parent;
                newGo.AddComponent<Misc3d>().CreateObject(record);

                DestroyImmediate(selected);
                Selection.SetActiveObjectWithContext(newGo, null);
            }
            catch (Exception error)
            {
                Debug.LogError(error);
            }
        }

        private void OnModifyFlat(string flatId)
        {
            var contentContainer = rootVisualElement.Query<VisualElement>("content").First();
            contentContainer.Clear();
            var modifyFlatEditor = new ModifyMiscFlatEditor(flatId, OnApplyFlatModification);
            contentContainer.Add(modifyFlatEditor.Render());
        }

        private void OnApplyFlatModification(string flatId)
        {
            var selected = Selection.activeGameObject;
            var currentMiscFlat = selected.GetComponent<MiscFlat>();
            var record = currentMiscFlat.GetRecord();

            var dot = Char.Parse(".");
            var splitId = flatId.Split(dot);

            record.TextureArchive = int.Parse(splitId[0]);
            record.TextureRecord = int.Parse(splitId[1]);

            try
            {
                var newGo = RmbBlockHelper.AddFlatObject(flatId);
                newGo.transform.parent = selected.transform.parent;
                newGo.AddComponent<MiscFlat>().CreateObject(record);

                DestroyImmediate(selected);
                Selection.SetActiveObjectWithContext(newGo, null);
            }
            catch (Exception error)
            {
                Debug.LogError(error);
            }
        }

        private void OnModifyBuilding()
        {
            var rmbBlockObject = root.GetComponent<RmbBlockObject>();
            var contentContainer = rootVisualElement.Query<VisualElement>("content").First();
            contentContainer.Clear();
            var modifyBuildingEditor = new ModifyBuildingEditor(OnApplyBuildingModification);
            contentContainer.Add(modifyBuildingEditor.Render(rmbBlockObject.Climate, rmbBlockObject.Season,
                rmbBlockObject.WindowStyle));
        }

        private void OnApplyBuildingModification(string buildingId)
        {
            var selected = Selection.activeGameObject;
            var currentBuilding = selected.GetComponent<Building>();
            var oldPosition = new Vector3(currentBuilding.XPos, currentBuilding.ModelsYPos, currentBuilding.ZPos);
            var oldRotation = new Vector3(0, currentBuilding.YRotation, 0);

            try
            {
                var buildingHelper = new BuildingPreset();
                var newGo = buildingHelper.AddBuildingObject(buildingId, oldPosition, oldRotation);
                newGo.transform.parent = selected.transform.parent;

                DestroyImmediate(selected);
                Selection.SetActiveObjectWithContext(newGo, null);
            }
            catch (Exception error)
            {
                Debug.LogError(error);
            }
        }

        private void OnSelectionChange()
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady || root == null)
                return;

            var contentContainer = rootVisualElement.Query<VisualElement>("content").First();
            var mapBlockEditor = new BlockEditor(root);
            var mapBlockControls = mapBlockEditor.Render();
            contentContainer.Clear();

            var rmbBlockObject = root.GetComponent<RmbBlockObject>();

            // If nothing is selected, render controls for the whole MapBlock
            if (Selection.activeGameObject == null)
            {
                contentContainer.Add(mapBlockControls);
                return;
            }

            // If the selection is NOT the Automap, turn it off, so that the ground is visible instead
            var automapSelected = Selection.activeGameObject.GetComponent<Automap>();
            if (!automapSelected)
            {
                rmbBlockObject.Automap.gameObject.SetActive(false);
            }

            // Render controls for Automap
            if (automapSelected)
            {
                rmbBlockObject.Automap.gameObject.SetActive(true);
                automapEditor = new AutomapEditor(automapSelected.gameObject);
                contentContainer.Add(automapEditor.Render());
                return;
            }

            // Render controls for ground
            var groundObject = Selection.activeGameObject.GetComponent<Ground>();
            if (groundObject)
            {
                groundEditor = new GroundEditor(groundObject.gameObject);
                contentContainer.Add(groundEditor.Render());
                return;
            }

            // Render controls for scenery
            var sceneryObject = Selection.activeGameObject.GetComponent<Scenery>();
            if (sceneryObject)
            {
                sceneryEditor = new SceneryEditor(sceneryObject.gameObject);
                contentContainer.Add(sceneryEditor.Render());
                return;
            }

            // Render controls for buildings
            var buildingObject = Selection.activeGameObject.GetComponent<Building>();
            if (buildingObject)
            {
                contentContainer.Add(BuildingEditor.Render(buildingObject.gameObject, OnModifyBuilding));
                return;
            }

            // Render controls for Misc3d Objects
            var misc3dObjects = Selection.activeGameObject.GetComponent<Misc3d>();
            if (misc3dObjects)
            {
                contentContainer.Add(Misc3dEditor.Render(misc3dObjects.gameObject, OnModify3d));
                return;
            }

            // Render controls for Misc Flats
            var miscFlatObject = Selection.activeGameObject.GetComponent<MiscFlat>();
            if (miscFlatObject)
            {
                contentContainer.Add(MiscFlatEditor.Render(miscFlatObject.gameObject, OnModifyFlat));
                return;
            }

            // Render controls for the whole MapBlock
            contentContainer.Add(mapBlockControls);
        }

        private void RenderMenus()
        {
            var fileMenu = rootVisualElement.Query<ToolbarMenu>("rmb-file-menu").First();
            fileMenu.menu.AppendAction("Open", action => LoadFile());
            fileMenu.menu.AppendAction("Save", action => SaveFile(), (action) => SetMenuStatus(action));
            fileMenu.menu.AppendAction("Export Selected Objects", action => ExportSelection(),
                (action) => SetExportMenuStatus(action));
            fileMenu.menu.AppendAction("Import Objects", action => ImportObjectGroupFile(),
                (action) => SetMenuStatus(action));
            fileMenu.menu.AppendAction("Settings", action =>
            {
                var contentContainer = rootVisualElement.Query<VisualElement>("content").First();
                contentContainer.Clear();
                contentContainer.Add(settingsEditor.Render(root));
            }, (action) => SetMenuStatus(action));

            var editMenu = rootVisualElement.Query<ToolbarMenu>("rmb-edit-menu").First();
            editMenu.menu.AppendAction("Block Properties",
                action => { Selection.SetActiveObjectWithContext(root, null); }, (action) => SetMenuStatus(action));
            editMenu.menu.AppendAction("Ground", action =>
            {
                var ground = root.GetComponentInChildren<Ground>();
                Selection.SetActiveObjectWithContext(ground, null);
            }, (action) => SetMenuStatus(action));

            editMenu.menu.AppendAction("Ground Scenery", action =>
            {
                var groundScenery = root.GetComponentInChildren<Scenery>();
                Selection.SetActiveObjectWithContext(groundScenery, null);
            }, (action) => SetMenuStatus(action));

            editMenu.menu.AppendAction("Automap", action =>
            {
                var rmbBlockObject = root.GetComponent<RmbBlockObject>();
                rmbBlockObject.Automap.gameObject.SetActive(true);
                Selection.SetActiveObjectWithContext(rmbBlockObject.Automap, null);
            }, (action) => SetMenuStatus(action));

            var addMenu = rootVisualElement.Query<ToolbarMenu>("rmb-add-menu").First();
            addMenu.menu.AppendAction("Buildings", action =>
            {
                var rmbBlockObject = root.GetComponent<RmbBlockObject>();
                var contentContainer = rootVisualElement.Query<VisualElement>("content").First();
                contentContainer.Clear();
                contentContainer.Add(addBuilding.Render(rmbBlockObject.Climate, rmbBlockObject.Season,
                    rmbBlockObject.WindowStyle));
            }, (action) => SetMenuStatus(action));
            addMenu.menu.AppendAction("Models", action =>
            {
                var rmbBlockObject = root.GetComponent<RmbBlockObject>();
                var contentContainer = rootVisualElement.Query<VisualElement>("content").First();
                contentContainer.Clear();
                contentContainer.Add(add3dElement.Render(rmbBlockObject.Climate, rmbBlockObject.Season,
                    rmbBlockObject.WindowStyle));
            }, (action) => SetMenuStatus(action));
            addMenu.menu.AppendAction("Flats", action =>
            {
                var contentContainer = rootVisualElement.Query<VisualElement>("content").First();
                contentContainer.Clear();
                contentContainer.Add(addFlat.Render());
            }, (action) => SetMenuStatus(action));
        }

        private void LoadFile()
        {
            var path = EditorUtility.OpenFilePanel("Open RMB Block File", WorldDataFolder, "json");

            if (String.IsNullOrEmpty(path))
            {
                return;
            }

            if (root != null)
            {
                var confirmed = EditorUtility.DisplayDialog("Open Map Block File?",
                    "Are you sure you wish to open a NEW file, all current unsaved changes will be lost!", "Ok",
                    "Cancel");
                if (!confirmed) return;
            }

            try
            {
                loadedBlock = RmbBlockHelper.LoadRmbBlockFile(path);
                if (root != null)
                    DestroyImmediate(root);

                if (light != null)
                    DestroyImmediate(light);

                // Add a  light to the scene
                light = new GameObject("Light");
                var lightComponent = light.AddComponent<Light>();
                lightComponent.type = LightType.Directional;
                lightComponent.shadows = LightShadows.Soft;
                lightComponent.transform.rotation = Quaternion.Euler(new Vector3(50, -30, 0));

                // Create and select the root GameObject
                root = new GameObject("RMB Block : " + Path.GetFileName(path));
                var rmbBlock = root.AddComponent<RmbBlockObject>();
                rmbBlock.CreateObject(loadedBlock, OnRemoveRoot);
                OnSelectionChange();

                // Enable the menus
                fileLoaded = true;
            }
            catch (ArgumentException e)
            {
                Debug.LogError(e.Message);
            }
        }

        private void ImportObjectGroupFile()
        {
            var path = EditorUtility.OpenFilePanel("Open Object Group", WorldDataFolder, "json");

            try
            {
                var import = RmbBlockHelper.LoadObjectGroupFile(path);
                var rmbBlockObject = root.GetComponent<RmbBlockObject>();
                rmbBlockObject.AddObjects(import);
            }
            catch (ArgumentException e)
            {
                Debug.LogError(e.Message);
            }
        }

        private void SaveFile()
        {
            var rmbBlockObject = root.GetComponent<RmbBlockObject>();
            var path = EditorUtility.SaveFilePanel("Save as", WorldDataFolder, rmbBlockObject.Name, "json");
            loadedBlock.RmbBlock = rmbBlockObject.RmbBlock;
            loadedBlock.Name = rmbBlockObject.Name;
            loadedBlock.Position = rmbBlockObject.Position;
            loadedBlock.Index = rmbBlockObject.Index;
            loadedBlock.Type = rmbBlockObject.Type;

            // Save Automap
            var automap = rmbBlockObject.Automap;
            loadedBlock.RmbBlock.FldHeader.AutoMapData = automap.automapData;

            // Save Ground
            var ground = root.GetComponentsInChildren<Ground>().First();
            var scenery = root.GetComponentsInChildren<Scenery>().First();
            loadedBlock.RmbBlock.FldHeader.GroundData.GroundTiles = ground.groundTiles;
            loadedBlock.RmbBlock.FldHeader.GroundData.GroundScenery = scenery.getGroundScenery();

            // Save Buildings
            var buildings = root.GetComponentsInChildren<Building>();
            loadedBlock.RmbBlock.FldHeader.BuildingDataList = new DFLocation.BuildingData[buildings.Length];
            loadedBlock.RmbBlock.SubRecords = new DFBlock.RmbSubRecord[buildings.Length];
            for (var i = 0; i < buildings.Length; i++)
            {
                var building = buildings[i];
                var buildingData = building.GetBuildingData();
                var subRecord = building.GetSubRecord();
                loadedBlock.RmbBlock.FldHeader.BuildingDataList[i] = buildingData;
                loadedBlock.RmbBlock.SubRecords[i] = subRecord;
            }

            // Save Misc 3d Objects
            var misc3ds = root.GetComponentsInChildren<Misc3d>();
            loadedBlock.RmbBlock.Misc3dObjectRecords = new DFBlock.RmbBlock3dObjectRecord[misc3ds.Length];
            for (var i = 0; i < misc3ds.Length; i++)
            {
                var misc3dObject = misc3ds[i];
                var record = misc3dObject.GetRecord();
                loadedBlock.RmbBlock.Misc3dObjectRecords[i] = record;
            }

            // Save Misc Flat Objects
            var miscFlats = root.GetComponentsInChildren<MiscFlat>();
            loadedBlock.RmbBlock.MiscFlatObjectRecords = new DFBlock.RmbBlockFlatObjectRecord[miscFlats.Length];
            for (var i = 0; i < miscFlats.Length; i++)
            {
                var miscFlat = miscFlats[i];
                var record = miscFlat.GetRecord();
                loadedBlock.RmbBlock.MiscFlatObjectRecords[i] = record;
            }

            RmbBlockHelper.SaveBlockFile(loadedBlock, path);
        }

        private void ExportSelection()
        {
            var path = EditorUtility.SaveFilePanel("Save as", WorldDataFolder, "ObjectGroup", "json");
            var exportBlock = new DFBlock.RmbBlockDesc();
            var selectedObjects = Selection.gameObjects;
            if (selectedObjects.Length < 1) return;
            var buildingsList = new List<Building>();
            var misc3dList = new List<Misc3d>();
            var flatList = new List<MiscFlat>();

            foreach (var selectedObject in selectedObjects)
            {
                var building = selectedObject.GetComponent<Building>();
                var misc3d = selectedObject.GetComponent<Misc3d>();
                var miscFlat = selectedObject.GetComponent<MiscFlat>();
                if (building != null)
                {
                    buildingsList.Add(building);
                }
                else if (misc3d != null)
                {
                    misc3dList.Add(misc3d);
                }
                else if (miscFlat != null)
                {
                    flatList.Add(miscFlat);
                }
            }

            // Save Buildings
            exportBlock.FldHeader.BuildingDataList = new DFLocation.BuildingData[buildingsList.Count];
            exportBlock.SubRecords = new DFBlock.RmbSubRecord[buildingsList.Count];
            for (var i = 0; i < buildingsList.Count; i++)
            {
                var building = buildingsList[i];
                var buildingData = building.GetBuildingData();
                var subRecord = building.GetSubRecord();
                exportBlock.FldHeader.BuildingDataList[i] = buildingData;
                exportBlock.SubRecords[i] = subRecord;
            }

            // Save Misc 3d Objects
            exportBlock.Misc3dObjectRecords = new DFBlock.RmbBlock3dObjectRecord[misc3dList.Count];
            for (var i = 0; i < misc3dList.Count; i++)
            {
                var misc3dObject = misc3dList[i];
                var record = misc3dObject.GetRecord();
                exportBlock.Misc3dObjectRecords[i] = record;
            }

            // Save Misc Flat Objects
            exportBlock.MiscFlatObjectRecords = new DFBlock.RmbBlockFlatObjectRecord[flatList.Count];
            for (var i = 0; i < flatList.Count; i++)
            {
                var miscFlat = flatList[i];
                var record = miscFlat.GetRecord();
                exportBlock.MiscFlatObjectRecords[i] = record;
            }

            RmbBlockHelper.SaveObjectGroupFile(exportBlock, path);
        }

        private DropdownMenuAction.Status SetMenuStatus(DropdownMenuAction action)
        {
            return fileLoaded ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled;
        }

        private DropdownMenuAction.Status SetExportMenuStatus(DropdownMenuAction action)
        {
            var selectedObjects = Selection.gameObjects;
            return selectedObjects.Length >= 1 ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled;
        }
    }
}