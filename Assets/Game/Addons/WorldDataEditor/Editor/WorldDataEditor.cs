// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Uncanny Valley
// Contributors:    Hazelnut

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.Utility.WorldDataEditor
{
    /// <summary>
    /// WorldData editor
    /// </summary>
    public class WorldDataEditor : EditorWindow
    {
        public const string WorldDataFolder = "/StreamingAssets/WorldData/";
        private GUIStyle lightBG = new GUIStyle();
        private GUIStyle lightMedBG = new GUIStyle();
        private GUIStyle bigText = new GUIStyle();

        private void CreataGUIStyles()
        {
            if (EditorGUIUtility.isProSkin)
            {
                lightBG.normal.background = WorldDataEditorBuildingHelper.CreateColorTexture(1, 1, new Color(0.35f, 0.35f, 0.35f, 1.0f));
                lightMedBG.normal.background = WorldDataEditorBuildingHelper.CreateColorTexture(1, 1, new Color(0.25f, 0.25f, 0.25f, 1.0f));
            }
            else
            {
                lightBG.normal.background = WorldDataEditorBuildingHelper.CreateColorTexture(1, 1, new Color(0.98f, 0.98f, 0.98f, 1.0f));
                lightMedBG.normal.background = WorldDataEditorBuildingHelper.CreateColorTexture(1, 1, new Color(0.9f, 0.9f, 0.9f, 1.0f));
            }
            bigText.fontSize = 24;
        }

        private enum EditMode { EditMode, AddItem };
        private enum DataMode { Building, Dungeon };
        private string searchField = "", selectedObjectID = "";
        private List<string> searchListNames = new List<string>(), searchListID = new List<string>();
        private EditMode editMode;
        private int objectPicker, chooseFileMode, listMode, billboardSubList, modelSubList, partsSubList;
        private Vector2 scrollPosition2 = Vector2.zero;
        private string[] chooseFileModeString = { "Choose from List", "Add manually" };
        private string[] listModesBuilding = { "3D Model", "Billboard", "NPC", "Door", "Int. Parts" };
        private string[] listModesBuildingExt = { "3D Model", "Billboard", "NPC" };
        private string[] listModesDungeon = { "3D Model", "Flat", "NPC", "Light", "Dung. Parts" };
        private string[] modelSubListString = { "Furniture", "Clutter", "Structure", "Graveyard", "Dungeon" };
        private string[] dungPartsSubListString = { "Rooms", "Corridors", "Misc", "Caves", "Doors/Exits" };
        private string[] billboardSubListString = { "Interior", "Exterior", "Lights", "Dungeon", "Markers" };
        private bool isExteriorMode = true;
        private DataMode dataMode;
        private string currentWorkFile;

        private GameObject root, interior, exterior, groundPlane, previewGO;
        string previewModelId;

        private BuildingReplacementData buildingData;

        private DFBlock dungeonData;
        private const int RDB_ObjLists = 10;    // Number of RDB object lists to support
        private GameObject[] rdbObjectLists = new GameObject[RDB_ObjLists];
        string[] ObjListLabels = { "All", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        int[] ObjListValues = { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        int ObjListSelect = -1;
        enum RdbModelTypes { MOD, DOR, LID, WHE, COL };
        RdbModelTypes RdbModelType = RdbModelTypes.MOD;
        ushort newRadius = 100;
        HashSet<int> positionIds = new HashSet<int>();

        [MenuItem("Daggerfall Tools/WorldData Editor")]
        static void Init()
        {
            WorldDataEditor window = (WorldDataEditor)GetWindow(typeof(WorldDataEditor));
            window.titleContent = new GUIContent("WorldData Editor");
        }

        private void Awake()
        {
            CreataGUIStyles();
            UpdateSearchList();
        }

        #region GUI

        string[] GetListModes()
        {
            if (dataMode == DataMode.Building && isExteriorMode == false)
                return listModesBuilding;
            else if (dataMode == DataMode.Dungeon)
                return listModesDungeon;
            else
                return listModesBuildingExt;
        }

        void OnGUI()
        {
            GUIFileMenu();
            if (root != null)
            {
                if (editMode == EditMode.EditMode)
                {
                    EditorWindow();
                }
                else if (editMode == EditMode.AddItem)
                {
                    AddItemWindow();
                }
            }
        }

        private void GUIFileMenu()
        {
            GUI.BeginGroup(new Rect(8, 8, Screen.width - 16, 48), lightBG);
            {
                if (GUI.Button(new Rect(8, 8, 96, 32), "Open Building"))
                {
                    bool newFile = false;

                    if (root != null)
                    {
                        newFile = EditorUtility.DisplayDialog("Open New Building File?", "Are you sure you wish to open a NEW file, all current unsaved changes will be lost!", "Ok", "Cancel");
                    }

                    if (newFile || root == null)
                    {
                        OpenBuildingFile();
                    }
                }
                else if (GUI.Button(new Rect(128, 8, 96, 32), "Open RDB"))
                {
                    bool newFile = false;

                    if (root != null)
                    {
                        newFile = EditorUtility.DisplayDialog("Open New Dungeon Block File?", "Are you sure you wish to open a NEW file, all current unsaved changes will be lost!", "Ok", "Cancel");
                    }

                    if (newFile || root == null)
                    {
                        OpenFileRDB();
                    }
                }
                else if (root != null && GUI.Button(new Rect(248, 8, 96, 32), "Save File"))
                {
                    string path = EditorUtility.SaveFilePanel("Save as", WorldDataFolder, currentWorkFile, "json");

                    if (dataMode == DataMode.Building)
                    {
                        UpdateBuildingWorldData();
                        WorldDataEditorBuildingHelper.SaveBuildingFile(buildingData, path);
                    }
                    else if (dataMode == DataMode.Dungeon)
                    {
                        UpdateDungeonWorldData();
                        WorldDataEditorDungeonHelper.SaveDungeonFile(dungeonData, path);
                    }
                }
            }
            GUI.EndGroup();
        }

        private void EditorWindow()
        {
            Repaint(); //Keep this repainted

            int elementIndex = 0;

            GUI.BeginGroup(new Rect(8, 64, Screen.width - 16, 48), lightBG);
            {
                if (dataMode == DataMode.Building)
                {
                    if (GUI.Button(new Rect(8, 8, 96, 32), "Add Object"))
                    {
                        editMode = EditMode.AddItem;
                    }
                    else if (root != null)
                    {
                        if (GUI.Button(new Rect(128, 8, 172, 32), "Switch Interior/Exterior"))
                        {
                            isExteriorMode = !isExteriorMode;
                            UpdateBuildingVisibility();
                        }
                        else if (GUI.Button(new Rect(316, 8, 172, 32), "Toggle Ground Plane"))
                        {
                            if (groundPlane == null)
                            {
                                // Add exterior ground plane
                                AddGroundPlane(5f);
                            }
                            groundPlane.SetActive(!groundPlane.activeSelf);
                        }
                    }
                }
                else if (dataMode == DataMode.Dungeon)
                {
                    if (root != null)
                    {
                        int objListSelected = EditorGUI.IntPopup(new Rect(8, 8, 196, 32), "Obj List Display Selection:", ObjListSelect, ObjListLabels, ObjListValues);
                        if (objListSelected != ObjListSelect) //GUI.Button(new Rect(348, 8, 96, 32), "Display"))
                        {
                            ObjListSelect = objListSelected;
                            UpdateObjListVisibility();
                        }
                    }
                    if (ObjListSelect > -1 && GUI.Button(new Rect(220, 8, 96, 32), "Add Object"))
                    {
                        editMode = EditMode.AddItem;
                    }

                }
            }
            GUI.EndGroup();

            if (Selection.activeGameObject == null)
                return;

            if (Selection.Contains(root) && dataMode == DataMode.Building)
            {
                GUI.Label(new Rect(16, 128, 96, 28), "XPos: ");
                buildingData.RmbSubRecord.XPos = EditorGUI.IntField(new Rect(128, 128, 128, 16), buildingData.RmbSubRecord.XPos);

                GUI.Label(new Rect(16, 150, 96, 28), "YPos: ");
                buildingData.RmbSubRecord.ZPos = EditorGUI.IntField(new Rect(128, 150, 128, 16), buildingData.RmbSubRecord.ZPos);

                GUI.Label(new Rect(16, 182, 96, 28), "YRotation: ");
                buildingData.RmbSubRecord.YRotation = EditorGUI.IntField(new Rect(128, 182, 128, 16), buildingData.RmbSubRecord.YRotation);

                GUI.Label(new Rect(16, 214, 96, 28), "Building Type: ");
                buildingData.BuildingType = (int)(DFLocation.BuildingTypes)EditorGUI.EnumPopup(new Rect(128, 214, 128, 16), (DFLocation.BuildingTypes)buildingData.BuildingType);

                GUI.Label(new Rect(16, 236, 96, 28), "Faction ID: ");
                buildingData.FactionId = (ushort)EditorGUI.IntField(new Rect(128, 236, 128, 16), buildingData.FactionId);

                GUI.Label(new Rect(16, 258, 96, 28), "Quality: ");
                buildingData.Quality = (byte)EditorGUI.IntSlider(new Rect(128, 258, 128, 16), buildingData.Quality, 1, 20);
            }

            else
            {
                if (Selection.activeGameObject.GetComponent<WorldDataEditorObject>())
                {
                    WorldDataEditorObject data = Selection.activeGameObject.GetComponent<WorldDataEditorObject>();

                    if (dataMode == DataMode.Dungeon)
                    {
                        GUI.Label(new Rect(330, 70, 60, 16), "Selected : ");
                        GUI.Label(new Rect(390, 62, 128, 16), data.type + ", ID = " + data.rdbObj.Position);
                        GUI.Label(new Rect(390, 78, 128, 16), "Object List = " + data.rdbObjListIdx);
                        GUI.Label(new Rect(390, 94, 128, 16), "Object Index = " + data.rdbObj.Index);
                    }

                    if (data.type == (int)WorldDataEditorObject.DataType.Object3D)
                    {
                        GUIElementPosition(ref data, ref elementIndex);
                        GUIElementShift(ref data, ref elementIndex);//Shift by size
                        GUIElementRotation(ref data, ref elementIndex);
                        if (dataMode == DataMode.Building)
                            GUIElementScale(ref data, ref elementIndex);

                        if (dataMode == DataMode.Dungeon)
                        {
                            GUIElementModelTriggerLockSound(ref data, ref elementIndex);
                            GUIElementModelActionRecord(ref data, ref elementIndex);
                            GUIElementNextObject(ref data, ref elementIndex);
                            GUIElementPrevObject(ref data, ref elementIndex);
                            GUIElementGroundButton(ref elementIndex);//move to ground
                        }
                        else if (dataMode == DataMode.Building)
                        {
                            if (data.objectType == WorldDataEditorBuildingHelper.InteriorHousePart)
                            {
                                GUIElementSwitchSet(ref data, ref elementIndex);
                            }
                            else if (data.id == WorldDataEditorObjectData.ladder.ToString())
                            {
                                GUIElementObjectType(ref data, ref elementIndex, "Is Climbable");
                            }
                            else if (WorldDataEditorObjectData.shopShelvesObjectGroupIndices.Contains(uint.Parse(data.id) - WorldDataEditorObjectData.containerObjectGroupOffset))
                            {
                                GUIElementObjectType(ref data, ref elementIndex, "Is Container");
                            }
                            else if (uint.Parse(data.id) / 100 == WorldDataEditorObjectData.houseContainerObjectGroup ||
                                    WorldDataEditorObjectData.houseContainerObjectGroupIndices.Contains(uint.Parse(data.id) - WorldDataEditorObjectData.containerObjectGroupOffset))
                            {
                                GUIElementObjectType(ref data, ref elementIndex, "Is Container");
                            }
                            else if (WorldDataEditorObjectData.houseFireplaceObjectGroupIndices.Contains(uint.Parse(data.id)))
                            {
                                GUIElementObjectType(ref data, ref elementIndex, "Animate");
                            }

                            ModelGroupSwitchers(ref elementIndex, data.id);
                            GUIElementGroundButton(ref elementIndex);//move to ground
                        }
                    }                  
                    else if (data.type == WorldDataEditorObject.DataType.Flat) //Flat object
                    {
                        GUIElementPosition(ref data, ref elementIndex);
                        GUIElementShift(ref data, ref elementIndex);//Shift by size
                        GUIElementFaction(ref data, ref elementIndex);

                        if (dataMode == DataMode.Dungeon)
                        {
                            GUIElementFlatActionRecord(ref data, ref elementIndex);
                            GUIElementFlatNextObject(ref data, ref elementIndex);
                        }
                        else
                        {
                            GUIElementFlag(ref data, ref elementIndex);
                        }

                        FlatGroupSwitchers(ref elementIndex, data.id);
                        GUIElementGroundButton(ref elementIndex);//move to ground
                    }                  
                    else if (data.type == WorldDataEditorObject.DataType.Person) //Person object
                    {
                        GUIElementPosition(ref data, ref elementIndex);
                        GUIElementShift(ref data, ref elementIndex);//Shift by size
                        GUIElementFaction(ref data, ref elementIndex);
                        GUIElementFlag(ref data, ref elementIndex);

                        FlatGroupSwitchers(ref elementIndex, data.id);
                        GUIElementGroundButton(ref elementIndex);//move to ground
                    }
                    else if (data.type == WorldDataEditorObject.DataType.Door) //Door object
                    {
                        GUIElementPosition(ref data, ref elementIndex);
                        GUIElementShift(ref data, ref elementIndex);//Shift by size
                        GUIElementRotation(ref data, ref elementIndex);
                        GUIElementGroundButton(ref elementIndex);//move to ground
                    }
                    else if (data.type == WorldDataEditorObject.DataType.Light) //Light object
                    {
                        GUIElementPosition(ref data, ref elementIndex);
                        GUIElementLightRadius(ref data, ref elementIndex);
                        GUIElementGroundButton(ref elementIndex);//move to ground
                    }
                }
                else
                {
                    GUI.TextArea(new Rect(24, 128, 196, 32), "No level object selected", bigText);
                }
            }
        }


        private void FlatGroupSwitchers(ref int elementIndex, string flatId)
        {
            //Debug.LogFormat("flat group switch: {0}", flatId);
            WorldDataEditorObject data = Selection.activeGameObject.GetComponent<WorldDataEditorObject>();
            foreach (string flatGroupName in WorldDataEditorObjectData.flatGroups.Keys)
            {
                string[] flatGroup = WorldDataEditorObjectData.flatGroups[flatGroupName];
                if (ArrayUtility.Contains(flatGroup, flatId))
                {
                    GUIElementFlatSwitchType(ref data, ref elementIndex, flatGroup);
                }
            }
        }

        private void ModelGroupSwitchers(ref int elementIndex, string modelId)
        {
            WorldDataEditorObject data = Selection.activeGameObject.GetComponent<WorldDataEditorObject>();
            foreach (string modelGroupName in WorldDataEditorObjectData.modelGroups.Keys)
            {
                string[] modelGroup = WorldDataEditorObjectData.modelGroups[modelGroupName];
                if (ArrayUtility.Contains(modelGroup, modelId))
                {
                    GUIElement3DSwitchType(ref data, ref elementIndex, modelGroup, modelId);
                }
            }
        }

        private void GUIElementSwitchSet(ref WorldDataEditorObject data, ref int elementIndex)
        {
            GUI.BeginGroup(new Rect(8, 120 + (56 * elementIndex), Screen.width - 16, 48), lightMedBG);
            {
                if (GUI.Button(new Rect(8, 8, 96, 32), "Switch set"))
                {
                    foreach (GameObject obj in Selection.objects)
                    {
                        if (obj.GetComponent<WorldDataEditorObject>() && obj.GetComponent<WorldDataEditorObject>().objectType == WorldDataEditorBuildingHelper.InteriorHousePart)
                        {
                            DFBlock.RmbBlock3dObjectRecord blockRecord = new DFBlock.RmbBlock3dObjectRecord();
                            WorldDataEditorObject interiorPiece = obj.GetComponent<WorldDataEditorObject>();
                            int currentSet = int.Parse(interiorPiece.id[interiorPiece.id.Length - 3].ToString());
                            currentSet++;

                            if (currentSet > 8)
                            {
                                currentSet = 0;
                            }

                            interiorPiece.id = interiorPiece.id.Remove(interiorPiece.id.Length - 3, 1);
                            interiorPiece.id = interiorPiece.id.Insert(interiorPiece.id.Length - 2, currentSet.ToString());
                            blockRecord.ModelIdNum = uint.Parse(interiorPiece.id);
                            blockRecord.ModelId = interiorPiece.id;
                            blockRecord.ObjectType = interiorPiece.objectType;
                            GameObject tempGo = WorldDataEditorBuildingHelper.Add3dObject(blockRecord);

                            interiorPiece.gameObject.GetComponent<MeshRenderer>().sharedMaterials = tempGo.GetComponent<MeshRenderer>().sharedMaterials;
                            DestroyImmediate(tempGo);
                        }
                    }
                }
            }
            elementIndex++;
            GUI.EndGroup();
        }

        private void GUIElementPosition(ref WorldDataEditorObject data, ref int elementIndex)
        {

            GUI.BeginGroup(new Rect(8, 120 + (56 * elementIndex), Screen.width - 16, 48), lightMedBG);
            {
                data.transform.localPosition = EditorGUI.Vector3Field(new Rect(32, 4, 312, 32), "Position", data.transform.localPosition); ;
            }
            elementIndex++;
            GUI.EndGroup();
        }

        private void GUIElementRotation(ref WorldDataEditorObject data, ref int elementIndex)
        {
            GUI.BeginGroup(new Rect(8, 120 + (56 * elementIndex), Screen.width - 16, 48), lightMedBG);
            {
                data.transform.eulerAngles = EditorGUI.Vector3Field(new Rect(32, 4, 312, 32), "Rotation", data.transform.eulerAngles);
            }
            elementIndex++;
            GUI.EndGroup();
        }

        private void GUIElementScale(ref WorldDataEditorObject data, ref int elementIndex)
        {
            GUI.BeginGroup(new Rect(8, 120 + (56 * elementIndex), Screen.width - 16, 48), lightMedBG);
            {
                data.transform.localScale = EditorGUI.Vector3Field(new Rect(32, 4, 312, 32), "Scale", data.transform.localScale);
            }
            elementIndex++;
            GUI.EndGroup();
        }

        //Shows a series of buttons that shift an object in a given direction based on its bounding box size in that direction.
        private void GUIElementShift(ref WorldDataEditorObject data, ref int elementIndex)
        {
            GUI.BeginGroup(new Rect(8, 120 + (56 * elementIndex), Screen.width - 16, 48), lightMedBG);
            {
                int buttonCount = 0;
                if (GUI.Button(new Rect(8 + buttonCount * 40, 8, 32, 32), "-X"))
                {
                    foreach (GameObject go in Selection.gameObjects)
                    {
                        Vector3 shiftSize = GetObjectSize(go);
                        
                        go.transform.localPosition += new Vector3(-shiftSize.x, 0, 0);
                    }
                }
                buttonCount++;
                
                if (GUI.Button(new Rect(8 + buttonCount * 40, 8, 32, 32), "+X"))
                {
                    foreach (GameObject go in Selection.gameObjects)
                    {
                        Vector3 shiftSize = GetObjectSize(go);
                        
                        go.transform.localPosition += new Vector3(shiftSize.x, 0, 0);
                    }
                }
                buttonCount++;

                if (GUI.Button(new Rect(8 + buttonCount * 40, 8, 32, 32), "-Y"))
                {
                    foreach (GameObject go in Selection.gameObjects)
                    {
                        Vector3 shiftSize = GetObjectSize(go);
                        
                        go.transform.localPosition += new Vector3(0, -shiftSize.y, 0);
                    }
                }
                buttonCount++;
                
                if (GUI.Button(new Rect(8 + buttonCount * 40, 8, 32, 32), "+Y"))
                {
                    foreach (GameObject go in Selection.gameObjects)
                    {
                        Vector3 shiftSize = GetObjectSize(go);
                        
                        go.transform.localPosition += new Vector3(0, shiftSize.y, 0);
                    }
                }
                buttonCount++;

                if (GUI.Button(new Rect(8 + buttonCount * 40, 8, 32, 32), "-Z"))
                {
                    foreach (GameObject go in Selection.gameObjects)
                    {
                        Vector3 shiftSize = GetObjectSize(go);
                        
                        go.transform.localPosition += new Vector3(0, 0, -shiftSize.z);
                    }
                }
                buttonCount++;
                
                if (GUI.Button(new Rect(8 + buttonCount * 40, 8, 32, 32), "+Z"))
                {
                    foreach (GameObject go in Selection.gameObjects)
                    {
                        Vector3 shiftSize = GetObjectSize(go);
                        
                        go.transform.localPosition += new Vector3(0, 0, shiftSize.z);
                    }
                }
                buttonCount++;

                GUI.Label(new Rect(8 + buttonCount * 40, 8, 128, 32), "Shift object by its size");

            }
            elementIndex++;
            GUI.EndGroup();
        }

        private void GUIElementObjectType(ref WorldDataEditorObject data, ref int elementIndex, string interactionName)
        {
            GUI.BeginGroup(new Rect(8, 120 + (56 * elementIndex), Screen.width - 16, 48), lightMedBG);
            {
                bool isInteractable = (data.objectType == WorldDataEditorBuildingHelper.InteractiveObject);
                GUI.Label(new Rect(8, 16, 96, 16), interactionName + " : ");
                isInteractable = GUI.Toggle(new Rect(96, 16, 32, 32), isInteractable, "");
                data.objectType = isInteractable ? WorldDataEditorBuildingHelper.InteractiveObject : (byte)0;
            }
            elementIndex++;
            GUI.EndGroup();
        }

        private void GUIElementFaction(ref WorldDataEditorObject data, ref int elementIndex)
        {
            GUI.BeginGroup(new Rect(8, 120 + (56 * elementIndex), Screen.width - 16, 48), lightMedBG);
            {
                if (dataMode == DataMode.Building)
                {
                    GUI.Label(new Rect(8, 16, 96, 16), "Faction ID : ");
                    data.factionID = (short)EditorGUI.IntField(new Rect(96, 16, 96, 16), data.factionID);
                }
                else
                {
                    GUI.Label(new Rect(8, 16, 128, 16), "Faction or Mobile ID :");
                    data.factionID = (short)EditorGUI.IntField(new Rect(136, 16, 96, 16), data.factionID);
                    GUI.Label(new Rect(260, 16, 96, 16), "Distance Index : ");
                    data.rdbObj.Resources.FlatResource.SoundIndex = (byte)EditorGUI.IntField(new Rect(356, 16, 96, 16), data.rdbObj.Resources.FlatResource.SoundIndex);
                }
            }
            elementIndex++;
            GUI.EndGroup();
        }

        private void GUIElementFlag(ref WorldDataEditorObject data, ref int elementIndex)
        {
            GUI.BeginGroup(new Rect(8, 120 + (56 * elementIndex), Screen.width - 16, 48), lightMedBG);
            {
                GUI.Label(new Rect(8, 16, 96, 16), "Flags : ");
                data.flags = (byte)EditorGUI.IntField(new Rect(96, 16, 96, 16), data.flags);
            }
            elementIndex++;
            GUI.EndGroup();
        }

        private void GUIElementDoorRotation(ref WorldDataEditorObject data, ref int elementIndex)
        {
            GUI.BeginGroup(new Rect(8, 120 + (56 * elementIndex), Screen.width - 16, 48), lightMedBG);
            {
                GUI.Label(new Rect(8, 16, 96, 16), "Open Rotation : ");
                data.openRotation = (byte)EditorGUI.IntField(new Rect(96, 16, 96, 16), data.openRotation);
            }
            elementIndex++;
            GUI.EndGroup();
        }

        private void GUIElementLightRadius(ref WorldDataEditorObject data, ref int elementIndex)
        {
            GUI.BeginGroup(new Rect(8, 120 + (56 * elementIndex), Screen.width - 16, 48), lightMedBG);
            {
                GUI.Label(new Rect(8, 16, 96, 16), "Light Radius : ");
                data.radius = (byte)EditorGUI.IntField(new Rect(96, 16, 96, 16), data.radius);
            }
            elementIndex++;
            GUI.EndGroup();
        }

        private void GUIElementModelTriggerLockSound(ref WorldDataEditorObject data, ref int elementIndex)
        {
            GUI.BeginGroup(new Rect(8, 120 + (56 * elementIndex), Screen.width - 16, 48), lightMedBG);
            {
                GUI.Label(new Rect(8, 16, 172, 16), "TriggerFlag & StartingLock : ");
                data.rdbObj.Resources.ModelResource.TriggerFlag_StartingLock =
                    (uint)EditorGUI.IntField(new Rect(172, 16, 96, 16), (int)data.rdbObj.Resources.ModelResource.TriggerFlag_StartingLock);

                GUI.Label(new Rect(280, 16, 96, 16), "Sound Index : ");
                data.rdbObj.Resources.ModelResource.SoundIndex = (byte)EditorGUI.IntField(new Rect(376, 16, 96, 16), data.rdbObj.Resources.ModelResource.SoundIndex);
            }
            elementIndex++;
            GUI.EndGroup();
        }

        private void GUIElementModelActionRecord(ref WorldDataEditorObject data, ref int elementIndex)
        {
            GUI.BeginGroup(new Rect(8, 120 + (56 * elementIndex), Screen.width - 16, 48), lightMedBG);
            {
                ref DFBlock.RdbActionResource record = ref data.rdbObj.Resources.ModelResource.ActionResource;
                GUI.Label(new Rect(8, 16, 48, 16), "Axis : ");
                record.Axis = (byte)EditorGUI.IntField(new Rect(48, 16, 48, 16), record.Axis);

                GUI.Label(new Rect(104, 16, 64, 16), "Duration : ");
                record.Duration = (ushort)EditorGUI.IntField(new Rect(168, 16, 48, 16), record.Duration);

                GUI.Label(new Rect(224, 16, 72, 16), "Magnitude : ");
                record.Magnitude = (ushort)EditorGUI.IntField(new Rect(296, 16, 48, 16), record.Magnitude);

                GUI.Label(new Rect(354, 16, 48, 16), "Action : ");
                record.Flags = (int)(DFBlock.RdbActionFlags)EditorGUI.EnumPopup(new Rect(400, 16, 140, 16), (DFBlock.RdbActionFlags)record.Flags);
            }
            elementIndex++;
            GUI.EndGroup();
        }

        private void GUIElementFlatActionRecord(ref WorldDataEditorObject data, ref int elementIndex)
        {
            GUI.BeginGroup(new Rect(8, 120 + (56 * elementIndex), Screen.width - 16, 48), lightMedBG);
            {
                ref DFBlock.RdbFlatResource record = ref data.rdbObj.Resources.FlatResource;
                GUI.Label(new Rect(8, 16, 72, 16), "Magnitude : ");
                record.Magnitude = (byte)EditorGUI.IntField(new Rect(80, 16, 48, 16), record.Magnitude);

                GUI.Label(new Rect(136, 16, 48, 16), "Flags : ");
                record.Flags = (ushort)EditorGUI.IntField(new Rect(186, 16, 48, 16), record.Flags);

                GUI.Label(new Rect(242, 16, 48, 16), "Action : ");
                record.Action = (byte)(DFBlock.RdbActionFlags)EditorGUI.EnumPopup(new Rect(298, 16, 140, 16), (DFBlock.RdbActionFlags)record.Action);
                GUI.Label(new Rect(242, 32, 260, 16), "(set to 'DoorText' for Passive Mob markers)");
            }
            elementIndex++;
            GUI.EndGroup();
        }

        private void GUIElementFlatNextObject(ref WorldDataEditorObject data, ref int elementIndex)
        {
            GUI.BeginGroup(new Rect(8, 120 + (56 * elementIndex), Screen.width - 16, 48), lightMedBG);
            {
                GUI.Label(new Rect(8, 16, 96, 16), "Next Object Id : ");
                data.rdbObj.Resources.FlatResource.NextObjectOffset =
                    EditorGUI.IntField(new Rect(96, 16, 96, 16), data.rdbObj.Resources.FlatResource.NextObjectOffset);

                using (new EditorGUI.DisabledScope(data.rdbObj.Resources.FlatResource.NextObjectOffset == 0))
                {
                    if (GUI.Button(new Rect(200, 12, 64, 24), "Select"))
                    {
                        SelectDungeonObject(data.rdbObj.Resources.FlatResource.NextObjectOffset);
                    }
                }
            }
            elementIndex++;
            GUI.EndGroup();
        }

        private void GUIElementNextObject(ref WorldDataEditorObject data, ref int elementIndex)
        {
            GUI.BeginGroup(new Rect(8, 120 + (56 * elementIndex), Screen.width - 16, 48), lightMedBG);
            {
                GUI.Label(new Rect(8, 16, 96, 16), "Next Object Id : ");
                data.rdbObj.Resources.ModelResource.ActionResource.NextObjectOffset =
                    EditorGUI.IntField(new Rect(96, 16, 96, 16), data.rdbObj.Resources.ModelResource.ActionResource.NextObjectOffset);

                using (new EditorGUI.DisabledScope(data.rdbObj.Resources.ModelResource.ActionResource.NextObjectOffset == 0))
                {
                    if (GUI.Button(new Rect(200, 12, 64, 24), "Select"))
                    {
                        SelectDungeonObject(data.rdbObj.Resources.ModelResource.ActionResource.NextObjectOffset);
                    }
                }
            }
            elementIndex++;
            GUI.EndGroup();
        }

        private void GUIElementPrevObject(ref WorldDataEditorObject data, ref int elementIndex)
        {
            GUI.BeginGroup(new Rect(8, 120 + (56 * elementIndex), Screen.width - 16, 48), lightMedBG);
            {
                GUI.Label(new Rect(8, 16, 96, 16), "Prev Object Id : ");
                data.rdbObj.Resources.ModelResource.ActionResource.PreviousObjectOffset =
                    EditorGUI.IntField(new Rect(96, 16, 96, 16), data.rdbObj.Resources.ModelResource.ActionResource.PreviousObjectOffset);

                using (new EditorGUI.DisabledScope(data.rdbObj.Resources.ModelResource.ActionResource.PreviousObjectOffset == 0))
                {
                    if (GUI.Button(new Rect(200, 12, 64, 24), "Select"))
                    {
                        SelectDungeonObject(data.rdbObj.Resources.ModelResource.ActionResource.PreviousObjectOffset);
                    }
                }
            }
            elementIndex++;
            GUI.EndGroup();
        }

        private void GUIElement3DSwitchType(ref WorldDataEditorObject data, ref int elementIndex, string[] tempArray, string modelId)
        {
            int pos = ArrayUtility.IndexOf(tempArray, modelId);

            GUI.BeginGroup(new Rect(8, 120 + (56 * elementIndex), Screen.width - 16, 32), lightMedBG);
            {
                if (GUI.Button(new Rect(16, 8, 32, 16), "<"))
                {
                    pos--;

                    if (pos < 0)
                        pos = tempArray.Length - 1;
                }

                GUI.Label(new Rect(50, 8, 48, 16), tempArray[pos]);

                if (GUI.Button(new Rect(96, 8, 32, 16), ">"))
                {
                    pos++;

                    if (pos >= tempArray.Length)
                        pos = 0;
                }

                if (pos != ArrayUtility.IndexOf(tempArray, modelId))
                {
                    if (previewGO)
                    {
                        Vector3 prevPos = previewGO.transform.position;
                        DestroyImmediate(previewGO);
                        previewModelId = tempArray[pos];
                        previewGO = WorldDataEditorDungeonHelper.Preview3dObject(previewModelId, prevPos);
                    }
                    else if (dataMode == DataMode.Building)
                    {
                        data.id = tempArray[pos];
                        DFBlock.RmbBlock3dObjectRecord newBlockRecord = new DFBlock.RmbBlock3dObjectRecord();
                        newBlockRecord.ModelId = data.id;
                        newBlockRecord.ModelIdNum = uint.Parse(data.id);
                        GameObject tempGo = Create3DObject(newBlockRecord, isExteriorMode);
                        data.gameObject.GetComponent<MeshRenderer>().sharedMaterials = tempGo.GetComponent<MeshRenderer>().sharedMaterials;
                        data.gameObject.GetComponent<MeshFilter>().sharedMesh = tempGo.GetComponent<MeshFilter>().sharedMesh;
                        data.gameObject.GetComponent<MeshCollider>().sharedMesh = tempGo.GetComponent<MeshFilter>().sharedMesh;
                        data.gameObject.name = tempGo.gameObject.name;
                        DestroyImmediate(tempGo);
                    }
                }
            }
            elementIndex++;
            GUI.EndGroup();
        }

        //Adds a button that moves the object to the ground
        private void GUIElementGroundButton(ref int elementIndex)
        {
            GUI.BeginGroup(new Rect(8, 120 + (56 * elementIndex), Screen.width - 16, 48), lightMedBG);
            if (GUI.Button(new Rect(8, 8, 100, 32), "Snap to Floor"))
            {
                foreach (GameObject go in Selection.gameObjects)
                {
                    Vector3 goSize = GetObjectSize(go, true);

                    //The AlignBillboardToGround fires a raycast. We need to turn off the collider if it's there so we don't hit ourselves.
                    Collider coll = go.GetComponent<Collider>();

                    if (coll)
                    {
                        coll.enabled = false;
                    }

                    //There's already a helper function that moves things to the ground.
                    DaggerfallWorkshop.Utility.GameObjectHelper.AlignBillboardToGround(go, new Vector2(0, goSize.y), 4);
                    
                    if (coll)
                    {
                        coll.enabled = true;
                    }
                }
            }
            elementIndex++;
            GUI.EndGroup();
        }

        private void GUIElementFlatSwitchType(ref WorldDataEditorObject data, ref int elementIndex, string[] tempArray)
        {
            int pos = ArrayUtility.IndexOf(tempArray, data.id.ToString());

            GUI.BeginGroup(new Rect(8, 120 + (56 * elementIndex), Screen.width - 16, 32), lightMedBG);
            {
                if (GUI.Button(new Rect(16, 8, 32, 16), "<"))
                {
                    pos--;

                    if (pos < 0)
                        pos = tempArray.Length - 1;
                }

                GUI.Label(new Rect(50, 8, 48, 16), tempArray[pos]);

                if (GUI.Button(new Rect(96, 8, 32, 16), ">"))
                {
                    pos++;

                    if (pos >= tempArray.Length)
                        pos = 0;
                }

                if (pos != ArrayUtility.IndexOf(tempArray, data.id.ToString()))
                {
                    data.id = tempArray[pos];
                    int archive = int.Parse(data.id.Split('.')[0]);
                    int record = int.Parse(data.id.Split('.')[1]);
                    
                    Billboard billboard = Selection.activeGameObject.GetComponent<Billboard>();

                    data.gameObject.name = DaggerfallWorkshop.Utility.GameObjectHelper.GetGoFlatName(archive, record);
                    
                    //Dungeon mode needs us to update the rdbObj and building mode ignores it.
                    data.rdbObj.Resources.FlatResource.TextureArchive = archive;
                    data.rdbObj.Resources.FlatResource.TextureRecord = record;
                    
                    DestroyImmediate(Selection.activeGameObject.GetComponent<Collider>());
                    billboard.SetMaterial(archive, record);
                }

            }
            elementIndex++;
            GUI.EndGroup();
        }

        private void AddItemWindow()
        {
            GUI.BeginGroup(new Rect(8, 64, Screen.width - 16, 32 ), lightMedBG);
            chooseFileMode = GUI.SelectionGrid(new Rect(8, 4, 312, 24), chooseFileMode, chooseFileModeString, 2);
            GUI.EndGroup();

            if (chooseFileMode == 0)    // Search / Select
            {
                GUI.BeginGroup(new Rect(8, 104, Screen.width - 16, 32), lightMedBG);
                string[] listModes = GetListModes();
                listMode = GUI.SelectionGrid(new Rect(8, 4, (listModes.Length * 80) + 8, 24), listMode, listModes, listModes.Length);
                GUI.EndGroup();

                if (listMode == 0)      // Model list
                {
                    GUI.BeginGroup(new Rect(8, 144, Screen.width - 16, 32), lightMedBG);
                    modelSubList = GUI.SelectionGrid(new Rect(8, 4, (modelSubListString.Length * 80) + 8, 24), modelSubList, modelSubListString, modelSubListString.Length);
                    GUI.EndGroup();
                }
                else if (listMode == 1) // Billboard list
                {
                    GUI.BeginGroup(new Rect(8, 144, Screen.width - 16, 32), lightMedBG);
                    billboardSubList = GUI.SelectionGrid(new Rect(8, 4, (billboardSubListString.Length * 80) + 8, 24), billboardSubList, billboardSubListString, billboardSubListString.Length);
                    GUI.EndGroup();
                }
                else if (listMode == 3 && dataMode == DataMode.Dungeon) // Lights
                {
                    GUI.Label(new Rect(new Rect(16, 190, 64, 16)), "Radius: ");
                    newRadius = (ushort)EditorGUI.IntField(new Rect(70, 190, 156, 16), newRadius);
                }
                else if (listMode == 4 && dataMode == DataMode.Dungeon) // Parts
                {
                    GUI.BeginGroup(new Rect(8, 144, Screen.width - 16, 32), lightMedBG);
                    partsSubList = GUI.SelectionGrid(new Rect(8, 4, (billboardSubListString.Length * 80) + 8, 24), partsSubList, dungPartsSubListString, dungPartsSubListString.Length);
                    GUI.EndGroup();
                }

                // Search (disabled for interior doors & dungeon lights)
                if (listMode != 3)
                {
                    GUI.Label(new Rect(new Rect(16, 190, 64, 16)), "Search: ");
                    searchField = EditorGUI.TextField(new Rect(70, 190, 156, 16), searchField);

                    if (GUI.changed)
                        UpdateSearchList();

                    scrollPosition2 = GUI.BeginScrollView(new Rect(4, 210, 312, 536), scrollPosition2, new Rect(0, 0, 256, 20 + (searchListNames.Count * 24)));
                    objectPicker = GUI.SelectionGrid(new Rect(10, 10, 256, searchListNames.Count * 24), objectPicker, searchListNames.ToArray(), 1);
                    GUI.EndScrollView();
                }
            }

            else if (chooseFileMode == 1)   // Manually enter id
            {
                GUI.Label(new Rect(new Rect(16, 190, 64, 16)), "Object ID : ");
                selectedObjectID = EditorGUI.TextField(new Rect(80, 190, 156, 16), selectedObjectID);
            }

            if (dataMode == DataMode.Dungeon && (listMode == 0 || listMode == 4 || chooseFileMode == 1))
            {
                EditorGUIUtility.labelWidth = 80;
                RdbModelType = (RdbModelTypes)EditorGUI.EnumPopup(new Rect(288, 190, 136, 16), "Model Type : ", RdbModelType);
            }

            if (GUI.Button(new Rect(16, 760, 96, 20), "OK"))
            {
                if (previewGO)
                {
                    selectedObjectID = previewModelId;
                    DestroyImmediate(previewGO);
                }
                else if (chooseFileMode == 0)
                    selectedObjectID = searchListID[objectPicker];

                GameObject go = null;
                if (dataMode == DataMode.Building)
                    go = AddNewBuildingObject();
                else if (dataMode == DataMode.Dungeon)
                    go = AddNewDungeonObject();

                if (go != null)
                {
                    // Try to position where the camera is looking
                    Ray newRay = new Ray(SceneView.lastActiveSceneView.camera.transform.position, SceneView.lastActiveSceneView.camera.transform.forward);
                    RaycastHit hit = new RaycastHit();
                    if (Physics.Raycast(newRay, out hit, 200))
                        go.transform.position = hit.point;
                    else
                        go.transform.position = SceneView.lastActiveSceneView.camera.transform.position + (SceneView.lastActiveSceneView.camera.transform.forward * 10);

                    // Ensure added object is selected
                    Selection.objects = new UnityEngine.Object[] { go };
                }

                editMode = EditMode.EditMode;
            }

            if (GUI.Button(new Rect(128, 760, 96, 20), "Cancel"))
            {
                RemovePreviewModel();
                editMode = EditMode.EditMode;
            }

            if (chooseFileMode == 0 && (listMode == 0 || listMode == 4))
            {
                if (GUI.Button(new Rect(240, 760, 96, 20), "Preview"))
                {
                    previewModelId = searchListID[objectPicker];
                    if (previewGO)
                    {
                        Vector3 prevPos = previewGO.transform.position;
                        DestroyImmediate(previewGO);
                        previewGO = WorldDataEditorDungeonHelper.Preview3dObject(previewModelId, prevPos);
                    }
                    else
                        previewGO = WorldDataEditorDungeonHelper.Preview3dObject(previewModelId);
                }
            }
            if (previewGO)
            {
                Selection.objects = new UnityEngine.Object[] { previewGO };
                int elementIndex = 12;
                ModelGroupSwitchers(ref elementIndex, previewModelId);
            }
        }

        private void RemovePreviewModel()
        {
            if (previewGO)
                DestroyImmediate(previewGO);
        }

        private GameObject AddNewBuildingObject()
        {
            GameObject go = null;
            if (listMode == 0 || listMode == 4)
            {
                DFBlock.RmbBlock3dObjectRecord blockRecord = new DFBlock.RmbBlock3dObjectRecord();
                blockRecord.XScale = 1;
                blockRecord.YScale = 1;
                blockRecord.ZScale = 1;
                blockRecord.ModelIdNum = uint.Parse(selectedObjectID);

                if (isExteriorMode)
                    blockRecord.ObjectType = WorldDataEditorBuildingHelper.ExteriorBuilding;
                else if (listMode == 4)
                    blockRecord.ObjectType = WorldDataEditorBuildingHelper.InteriorHousePart;

                blockRecord.ModelId = blockRecord.ModelIdNum.ToString();
                go = WorldDataEditorBuildingHelper.Add3dObject(blockRecord);

                //Rotate Carpets
                if (blockRecord.ModelId == "74800")
                    go.transform.rotation = Quaternion.Euler(270, 0, 0);

                go.AddComponent<WorldDataEditorObject>().CreateData(blockRecord, isExteriorMode);
            }
            else if (listMode == 1)
            {
                DFBlock.RmbBlockFlatObjectRecord blockRecord = new DFBlock.RmbBlockFlatObjectRecord();
                blockRecord.TextureArchive = int.Parse(selectedObjectID.Split('.')[0]);
                blockRecord.TextureRecord = int.Parse(selectedObjectID.Split('.')[1]);
                go = WorldDataEditorBuildingHelper.AddFlatObject(blockRecord, Quaternion.identity);
                go.AddComponent<WorldDataEditorObject>().CreateData(blockRecord, isExteriorMode);

                // Add point lights
                if (blockRecord.TextureArchive == DaggerfallWorkshop.Utility.TextureReader.LightsTextureArchive)
                {
                    WorldDataEditorBuildingHelper.AddLight(go.transform, blockRecord);
                }
            }
            else if (listMode == 2)
            {
                DFBlock.RmbBlockPeopleRecord blockRecord = new DFBlock.RmbBlockPeopleRecord();
                blockRecord.TextureArchive = int.Parse(selectedObjectID.Split('.')[0]);
                blockRecord.TextureRecord = int.Parse(selectedObjectID.Split('.')[1]);
                go = WorldDataEditorBuildingHelper.AddPersonObject(blockRecord);
                go.AddComponent<WorldDataEditorObject>().CreateData(blockRecord, isExteriorMode);
            }
            else if (listMode == 3)
            {
                DFBlock.RmbBlockDoorRecord blockRecord = new DFBlock.RmbBlockDoorRecord();
                blockRecord.OpenRotation = 90; // Seems to be the default rotation used in DF data, but unused by DFU
                go = WorldDataEditorBuildingHelper.AddDoorObject(blockRecord);
                go.AddComponent<WorldDataEditorObject>().CreateData(blockRecord, isExteriorMode);
            }

            if (go != null)
                go.transform.parent = isExteriorMode ? exterior.transform : interior.transform;

            return go;
        }

        private GameObject AddNewDungeonObject()
        {
            GameObject go = null;
            if (listMode == 0 || listMode == 4)      // Add Model
            {
                uint modelIdNum = uint.Parse(selectedObjectID);
                Debug.Log("Adding model id= " + modelIdNum);
                ushort modelIndex = 0;
                for (; modelIndex < dungeonData.RdbBlock.ModelReferenceList.Length; modelIndex++)
                {
                    if (dungeonData.RdbBlock.ModelReferenceList[modelIndex].ModelIdNum == modelIdNum &&
                        dungeonData.RdbBlock.ModelReferenceList[modelIndex].Description == RdbModelType.ToString())
                        break;
                }
                if (modelIndex == dungeonData.RdbBlock.ModelReferenceList.Length)
                {
                    // Add a new model reference to list
                    DFBlock.RdbModelReference newModelRef = new DFBlock.RdbModelReference
                    {
                        ModelId = selectedObjectID,
                        ModelIdNum = modelIdNum,
                        Description = RdbModelType.ToString()
                    };
                    ArrayUtility.Add(ref dungeonData.RdbBlock.ModelReferenceList, newModelRef);
                }
                ref DFBlock.RdbModelReference modelRef = ref dungeonData.RdbBlock.ModelReferenceList[modelIndex];
                RdbModelType = RdbModelTypes.MOD;   // Reset model type to normal

                DFBlock.RdbObject obj = new DFBlock.RdbObject();
                obj.Resources.ModelResource.ModelIndex = modelIndex;
                obj.Type = DFBlock.RdbResourceTypes.Model;
                go = WorldDataEditorDungeonHelper.Add3dObject(obj, ref dungeonData.RdbBlock.ModelReferenceList);
                if (go != null)
                {
                    int rol = GetRdbObjectListAndSetObjIndex(go, ref obj);
                    obj.Position = GetNewPositionId();
                    go.AddComponent<WorldDataEditorObject>().CreateData(obj, rol, WorldDataEditorObject.DataType.Object3D);
                }
            }
            else if (listMode == 1 || listMode == 2) // Add Flat
            {
                DFBlock.RdbObject obj = new DFBlock.RdbObject();
                obj.Type = DFBlock.RdbResourceTypes.Flat;
                obj.Resources.FlatResource.TextureArchive = int.Parse(selectedObjectID.Split('.')[0]);
                obj.Resources.FlatResource.TextureRecord = int.Parse(selectedObjectID.Split('.')[1]);
                obj.Resources.FlatResource.IsCustomData = true;

                go = WorldDataEditorDungeonHelper.AddFlatObject(obj);
                if (go != null)
                {
                    int rol = GetRdbObjectListAndSetObjIndex(go, ref obj);
                    obj.Position = GetNewPositionId();
                    go.AddComponent<WorldDataEditorObject>().CreateData(obj, rol, WorldDataEditorObject.DataType.Flat);
                }
            }
            else if (listMode == 3) // Add Light
            {
                DFBlock.RdbObject obj = new DFBlock.RdbObject();
                obj.Type = DFBlock.RdbResourceTypes.Light;
                obj.Resources.LightResource.Radius = newRadius;

                int rol = 0;
                Transform selObjList = null;
                foreach (Transform objList in root.transform)
                {
                    if (ObjListSelect == rol)
                    {
                        selObjList = objList;
                        Transform[] objects = objList.GetComponentsInChildren<Transform>(true);
                        obj.Index = objects.Length - 1;
                        break;
                    }
                    rol++;
                }
                go = WorldDataEditorDungeonHelper.AddLightObject(selObjList, obj);
                if (go != null)
                {
                    obj.Position = GetNewPositionId();
                    go.AddComponent<WorldDataEditorObject>().CreateData(obj, rol, WorldDataEditorObject.DataType.Light);
                }
            }

            return go;
        }

        private int GetRdbObjectListAndSetObjIndex(GameObject go, ref DFBlock.RdbObject obj)
        {
            int rol = 0;
            foreach (Transform objList in root.transform)
            {
                if (ObjListSelect == rol)
                {
                    go.transform.parent = objList;
                    Transform[] objects = objList.GetComponentsInChildren<Transform>(true);
                    obj.Index = objects.Length - 1;
                    return rol;
                }
                rol++;
            }
            return 0;
        }

        //Gets the size of an object, used mostly so we can put things on the ground or move them based on size.
        //getOffsetSize can be set to true to adjust the size if the origin is not the center of the mesh
        private Vector3 GetObjectSize(GameObject go, bool getOffsetSize = false)
        {
            Vector3 size = new Vector3();

            Renderer objRend = go.GetComponent<Renderer>();
            if (objRend)
            {
                size = objRend.bounds.size;

                if (getOffsetSize)
                {
                    //We need to find the difference between the object's origin and the mesh's center and subtract that from the size
                    //this way it will align properly if the origin is offset.
                    size.y -= (objRend.bounds.center.y - go.transform.position.y) * 2;
                }
            }

            Billboard objBill = go.GetComponent<Billboard>();
            if (objBill)
            {
                size.x = objBill.Summary.Size.x;
                size.y = objBill.Summary.Size.y;
                size.z = objBill.Summary.Size.x; //billboards are 2d so their x and z data are basically the same
            }
            return size;
        }

        #endregion

        #region 3D scene

        private GameObject CreateFlatObject(DFBlock.RmbBlockFlatObjectRecord blockRecord, bool isExterior)
        {
            Quaternion subRecordRotation = isExterior ? Quaternion.AngleAxis(buildingData.RmbSubRecord.YRotation / BlocksFile.RotationDivisor, Vector3.up) : Quaternion.identity;

            GameObject go = WorldDataEditorBuildingHelper.AddFlatObject(blockRecord, subRecordRotation);
            go.transform.parent = isExterior ? exterior.transform : interior.transform;
            go.AddComponent<WorldDataEditorObject>().CreateData(blockRecord, isExterior);

            // Add point lights
            if (blockRecord.TextureArchive == DaggerfallWorkshop.Utility.TextureReader.LightsTextureArchive)
            {
                WorldDataEditorBuildingHelper.AddLight(go.transform, blockRecord);
            }
            return go;
        }

        private GameObject Create3DObject(DFBlock.RmbBlock3dObjectRecord blockRecord, bool isExterior)
        {
            GameObject go = WorldDataEditorBuildingHelper.Add3dObject(blockRecord);
            if (go != null)
            {
                go.transform.parent = isExterior ? exterior.transform : interior.transform;
                go.AddComponent<WorldDataEditorObject>().CreateData(blockRecord, isExterior);
            }
            return go;
        }

        private void UpdateSearchList()
        {
            searchListNames.Clear();
            searchListID.Clear();
            Dictionary<string, string> currentList;

            if (listMode == 0 && modelSubList == 0)
                currentList = WorldDataEditorObjectData.models_furniture;
            else if (listMode == 0 && modelSubList == 1)
                currentList = WorldDataEditorObjectData.models_clutter;
            else if (listMode == 0 && modelSubList == 2)
                currentList = WorldDataEditorObjectData.models_structure;
            else if (listMode == 0 && modelSubList == 3)
                currentList = WorldDataEditorObjectData.models_graveyard;
            else if (listMode == 0 && modelSubList == 4)
                currentList = WorldDataEditorObjectData.models_dungeon;
            else if (listMode == 1 && billboardSubList == 0)
                currentList = WorldDataEditorObjectData.billboards_interior;
            else if (listMode == 1 && billboardSubList == 1)
                currentList = WorldDataEditorObjectData.billboards_nature;
            else if (listMode == 1 && billboardSubList == 2)
                currentList = WorldDataEditorObjectData.billboards_lights;
            else if (listMode == 1 && billboardSubList == 3)
                currentList = WorldDataEditorObjectData.billboards_treasure;
            else if (listMode == 1 && billboardSubList == 4)
                currentList = WorldDataEditorObjectData.billboards_markers;
            else if (listMode == 4 && dataMode == DataMode.Dungeon) {
                if (partsSubList == 0)
                    currentList = WorldDataEditorObjectData.dungeonParts_rooms;
                else if (partsSubList == 1)
                    currentList = WorldDataEditorObjectData.dungeonParts_corridors;
                else if (partsSubList == 2)
                    currentList = WorldDataEditorObjectData.dungeonParts_misc;
                else if (partsSubList == 3)
                    currentList = WorldDataEditorObjectData.dungeonParts_caves;
                else // if (partsSubList == 4)
                    currentList = WorldDataEditorObjectData.dungeonParts_doors;
            } else if (listMode == 4 && dataMode == DataMode.Building)
                    currentList = WorldDataEditorObjectData.houseParts;
            else
                currentList = WorldDataEditorObjectData.NPCs;


            foreach (KeyValuePair<string, string> pair in currentList)
            {
                if (pair.Value.ToLower().Contains(searchField.ToLower()))
                {
                    searchListNames.Add(pair.Value);
                    searchListID.Add(pair.Key);
                }
            }
        }

        private void UpdateBuildingVisibility()
        {
            interior.SetActive(false);
            exterior.SetActive(false);
            if (isExteriorMode)
                exterior.SetActive(true);
            else
                interior.SetActive(true);
        }

        private void UpdateObjListVisibility()
        {
            if (ObjListSelect == -1)
            {
                foreach (Transform child in root.transform)
                    child.gameObject.SetActive(true);
            }
            else
            {
                int c = 0;
                foreach (Transform child in root.transform)
                    child.gameObject.SetActive(ObjListSelect == c++);
            }
        }

        private void AddGroundPlane(float size)
        {
            groundPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            groundPlane.name = "GroundPlane";
            groundPlane.transform.parent = root.transform;
            groundPlane.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_Color", Color.gray);
            groundPlane.transform.localScale = new Vector3(size, 1f, size);
            groundPlane.SetActive(false);
        }

        #endregion

        #region WorldData files

        int GetNewPositionId()
        {
            int newId;
            while (positionIds.Contains(newId = UnityEngine.Random.Range(100, 100000)))
                Debug.Log("Collision for new id " + newId);
            return newId;
        }

        private void OnDestroy()
        {
            RemovePreviewModel();
            if (root != null)
                DestroyImmediate(root.gameObject);
        }

        private void OpenBuildingFile()
        {
            string path = EditorUtility.OpenFilePanel("Open Building Override File", WorldDataFolder, "json");

            if (WorldDataEditorBuildingHelper.LoadBuildingFile(path, out buildingData))
            {
                if (root != null)
                    DestroyImmediate(root);

                positionIds.Clear();
                dataMode = DataMode.Building;
                currentWorkFile = Path.GetFileName(path);

                root = new GameObject("Building : " + currentWorkFile);

                LoadObjectsBuilding();
                editMode = EditMode.EditMode;
            }
            else
            {
                path = "";
            }

            //We clear the Undo
            Undo.ClearAll();
        }

        private void OpenFileRDB()
        {
            string path = EditorUtility.OpenFilePanel("Open RDB Override File", WorldDataFolder, "json");

            if (WorldDataEditorDungeonHelper.LoadDungeonFile(path, out dungeonData))
            {
                if (root != null)
                    DestroyImmediate(root);

                positionIds.Clear();
                dataMode = DataMode.Dungeon;
                currentWorkFile = Path.GetFileName(path);
                ObjListSelect = -1;

                root = new GameObject("Dungeon Block : " + currentWorkFile);

                LoadObjectsDungeon();
                editMode = EditMode.EditMode;
            }
            else
            {
                path = "";
            }

            //We clear the Undo
            Undo.ClearAll();
        }

        private void LoadObjectsBuilding()
        {
            interior = new GameObject("Interior");
            interior.transform.parent = root.transform;
            exterior = new GameObject("Exterior");
            exterior.transform.parent = root.transform;

            foreach (var blockRecord in buildingData.RmbSubRecord.Interior.Block3dObjectRecords)
            {
                Create3DObject(blockRecord, false);
            }
            foreach (var blockRecord in buildingData.RmbSubRecord.Interior.BlockFlatObjectRecords)
            {
                CreateFlatObject(blockRecord, false);
            }
            foreach (var blockRecord in buildingData.RmbSubRecord.Interior.BlockPeopleRecords)
            {
                GameObject go = WorldDataEditorBuildingHelper.AddPersonObject(blockRecord);
                go.transform.parent = interior.transform;
                go.AddComponent<WorldDataEditorObject>().CreateData(blockRecord, false);
            }
            foreach (var blockRecord in buildingData.RmbSubRecord.Interior.BlockDoorRecords)
            {
                GameObject go = WorldDataEditorBuildingHelper.AddDoorObject(blockRecord);
                go.transform.parent = interior.transform;
                go.AddComponent<WorldDataEditorObject>().CreateData(blockRecord, false);
            }
            foreach (var blockRecord in buildingData.RmbSubRecord.Exterior.Block3dObjectRecords)
            {
                Create3DObject(blockRecord, true);
            }
            foreach (var blockRecord in buildingData.RmbSubRecord.Exterior.BlockFlatObjectRecords)
            {
                CreateFlatObject(blockRecord, true);
            }
            foreach (var blockRecord in buildingData.RmbSubRecord.Exterior.BlockPeopleRecords)
            {
                GameObject go = WorldDataEditorBuildingHelper.AddPersonObject(blockRecord);
                go.transform.parent = exterior.transform;
                go.AddComponent<WorldDataEditorObject>().CreateData(blockRecord, true);
            }
            foreach (var blockRecord in buildingData.RmbSubRecord.Exterior.BlockDoorRecords)
            {
                GameObject go = WorldDataEditorBuildingHelper.AddDoorObject(blockRecord);
                go.transform.parent = exterior.transform;
                go.AddComponent<WorldDataEditorObject>().CreateData(blockRecord, true);
            }
            isExteriorMode = false;
            exterior.SetActive(false);
            EditorGUIUtility.PingObject(interior);
        }


        private void LoadObjectsDungeon()
        {
            GameObject rdbObjList = null;
            for (int rol=0; rol < RDB_ObjLists; rol++)
            {
                rdbObjList = new GameObject("RdbObjectList-" + rol);
                rdbObjList.transform.parent = root.transform;
                rdbObjectLists[rol] = rdbObjList;

                if (rol < dungeonData.RdbBlock.ObjectRootList.Length && dungeonData.RdbBlock.ObjectRootList[rol].RdbObjects != null)
                {
                    foreach (DFBlock.RdbObject obj in dungeonData.RdbBlock.ObjectRootList[rol].RdbObjects)
                    {
                        positionIds.Add(obj.Position);
                        if (obj.Type == DFBlock.RdbResourceTypes.Model)
                        {
                            GameObject go = WorldDataEditorDungeonHelper.Add3dObject(obj, ref dungeonData.RdbBlock.ModelReferenceList);
                            if (go == null)
                                continue;
                            go.transform.parent = rdbObjList.transform;
                            go.AddComponent<WorldDataEditorObject>().CreateData(obj, rol, WorldDataEditorObject.DataType.Object3D);
                        }
                        else if (obj.Type == DFBlock.RdbResourceTypes.Flat)
                        {
                            GameObject go = WorldDataEditorDungeonHelper.AddFlatObject(obj);
                            go.transform.parent = rdbObjList.transform;
                            go.AddComponent<WorldDataEditorObject>().CreateData(obj, rol, WorldDataEditorObject.DataType.Flat);
                        }
                        else if (obj.Type == DFBlock.RdbResourceTypes.Light)
                        {
                            GameObject go = WorldDataEditorDungeonHelper.AddLightObject(rdbObjList.transform, obj);
                            go.AddComponent<WorldDataEditorObject>().CreateData(obj, rol, WorldDataEditorObject.DataType.Light);
                        }
                    }
                }
            }
            if (rdbObjList != null)
                EditorGUIUtility.PingObject(rdbObjList);

            if (dungeonData.RdbBlock.ObjectRootList.Length > RDB_ObjLists)
                for (int rol = RDB_ObjLists; rol < dungeonData.RdbBlock.ObjectRootList.Length; rol++)
                    if (dungeonData.RdbBlock.ObjectRootList[rol].RdbObjects != null && dungeonData.RdbBlock.ObjectRootList[rol].RdbObjects.Length > 0)
                        Debug.LogWarningFormat("More than {0} object lists in this RDB, increase RDB_ObjLists to allow for {1} since it has object data.", RDB_ObjLists, rol);
        }

        private void UpdateDungeonWorldData()
        {
            dungeonData.RdbBlock.ObjectRootList = new DFBlock.RdbObjectRoot[RDB_ObjLists];

            Vector3 objectPosition;
            WorldDataEditorObject data;
            int rol = 0;
            foreach (Transform objList in root.transform)
            {
                Transform[] objects = objList.GetComponentsInChildren<Transform>(true);
                if (objects.Length <= 1)
                {
                    dungeonData.RdbBlock.ObjectRootList[rol++].RdbObjects = null;
                    continue;
                }

                // Create the new object list array
                dungeonData.RdbBlock.ObjectRootList[rol].RdbObjects = new DFBlock.RdbObject[objects.Length-1];

                for (int i=1; i < objects.Length; i++)
                {
                    Transform child = objects[i];
                    int index = i - 1;
                    data = child.GetComponent<WorldDataEditorObject>();
                    if (data == null)
                    {
                        Debug.LogErrorFormat("Object data is missing for list {0} index {1}", rol, index);
                        return;
                    }

                    // Set index and position
                    ref DFBlock.RdbObject record = ref data.rdbObj;
                    record.Index = index;
                    objectPosition = child.transform.localPosition / MeshReader.GlobalScale;
                    record.XPos = Mathf.RoundToInt(objectPosition.x);
                    record.YPos = -Mathf.RoundToInt(objectPosition.y);
                    record.ZPos = Mathf.RoundToInt(objectPosition.z);

                    // Update rotation (models) and type specific data
                    if (data.type == WorldDataEditorObject.DataType.Object3D)
                    {
                        record.Type = DFBlock.RdbResourceTypes.Model;
                        record.Resources.ModelResource.XRotation = (int)(-child.eulerAngles.x * BlocksFile.RotationDivisor);
                        record.Resources.ModelResource.YRotation = (int)(-child.eulerAngles.y * BlocksFile.RotationDivisor);
                        record.Resources.ModelResource.ZRotation = (int)(-child.eulerAngles.z * BlocksFile.RotationDivisor);
                    }
                    else if (data.type == WorldDataEditorObject.DataType.Flat)
                    {
                        record.Type = DFBlock.RdbResourceTypes.Flat;
                        record.Resources.FlatResource.FactionOrMobileId = (ushort)data.factionID;
                        int mask = (record.Resources.FlatResource.TextureArchive == 199 && record.Resources.FlatResource.TextureRecord == 10) ? 0xFF : 0x7;
                        record.Resources.FlatResource.SoundIndex = (byte)(record.Resources.FlatResource.SoundIndex & mask);  // Ensure only values 0-7 allowed for distance arrays in EnemySenses.Start()
                    }
                    else if (data.type == WorldDataEditorObject.DataType.Light)
                    {
                        record.Type = DFBlock.RdbResourceTypes.Light;
                        record.Resources.LightResource.Radius = data.radius;
                    }
                    dungeonData.RdbBlock.ObjectRootList[rol].RdbObjects[index] = record;

                    //Debug.LogFormat("{0} {1} {2} {3}", rol, index, dungeonData.RdbBlock.ObjectRootList[rol].RdbObjects[index].Type, dungeonData.RdbBlock.ObjectRootList[rol].RdbObjects[index].Position);
                }
                rol++;
            }
        }

        private void SelectDungeonObject(int objectId)
        {
            WorldDataEditorObject data;
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                data = child.GetComponent<WorldDataEditorObject>();
                if (data != null)
                {
                    if (data.rdbObj.Position == objectId)
                    {
                        Selection.objects = new UnityEngine.Object[] { child };
                        return;
                    }
                }
            }
        }

        private void UpdateBuildingWorldData()
        {
            ArrayUtility.Clear(ref buildingData.RmbSubRecord.Exterior.Block3dObjectRecords);
            ArrayUtility.Clear(ref buildingData.RmbSubRecord.Interior.Block3dObjectRecords);
            ArrayUtility.Clear(ref buildingData.RmbSubRecord.Exterior.BlockFlatObjectRecords);
            ArrayUtility.Clear(ref buildingData.RmbSubRecord.Interior.BlockFlatObjectRecords);
            ArrayUtility.Clear(ref buildingData.RmbSubRecord.Exterior.BlockPeopleRecords);
            ArrayUtility.Clear(ref buildingData.RmbSubRecord.Interior.BlockPeopleRecords);
            ArrayUtility.Clear(ref buildingData.RmbSubRecord.Exterior.BlockDoorRecords);
            ArrayUtility.Clear(ref buildingData.RmbSubRecord.Interior.BlockDoorRecords);

            Vector3 modelPosition;
            WorldDataEditorObject data;

            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                //Debug.Log(child.name);
                if (child.GetComponent<WorldDataEditorObject>() == null)
                    continue;

                data = child.GetComponent<WorldDataEditorObject>();

                //3D models
                if (data.type == (int)WorldDataEditorObject.DataType.Object3D)
                {
                    DFBlock.RmbBlock3dObjectRecord record = new DFBlock.RmbBlock3dObjectRecord();
                    record.ModelId = data.id;
                    record.ModelIdNum = uint.Parse(data.id);
                    record.ObjectType = data.objectType;

                    if (data.objectType == 3)
                    {
                        Vector3[] vertices = child.GetComponent<MeshFilter>().sharedMesh.vertices;

                        // Props axis needs to be transformed to lowest Y point
                        Vector3 bottom = vertices[0];
                        for (int j = 0; j < vertices.Length; j++)
                        {
                            if (vertices[j].y < bottom.y)
                                bottom = vertices[j];
                        }
                        modelPosition = new Vector3(child.localPosition.x, (child.localPosition.y + (bottom.y)), child.localPosition.z) / MeshReader.GlobalScale;
                    }
                    else
                    {
                        modelPosition = new Vector3(child.localPosition.x, -child.localPosition.y, child.localPosition.z) / MeshReader.GlobalScale;
                    }

                    record.XPos = Mathf.RoundToInt(modelPosition.x);
                    record.YPos = Mathf.RoundToInt(modelPosition.y);
                    record.ZPos = Mathf.RoundToInt(modelPosition.z);
                    record.XRotation = (short)(-child.eulerAngles.x * BlocksFile.RotationDivisor);
                    record.YRotation = (short)(-child.eulerAngles.y * BlocksFile.RotationDivisor);
                    record.ZRotation = (short)(-child.eulerAngles.z * BlocksFile.RotationDivisor);
                    if (child.localScale.x < 0.999f || child.localScale.x > 1.001f)
                        record.XScale = child.localScale.x;
                    if (child.localScale.y < 0.999f || child.localScale.y > 1.001f)
                        record.YScale = child.localScale.y;
                    if (child.localScale.z < 0.999f || child.localScale.z > 1.001f)
                        record.ZScale = child.localScale.z;


                    if (data.isExterior)
                        ArrayUtility.Add(ref buildingData.RmbSubRecord.Exterior.Block3dObjectRecords, record);
                    else
                        ArrayUtility.Add(ref buildingData.RmbSubRecord.Interior.Block3dObjectRecords, record);
                }

                else if (data.type == WorldDataEditorObject.DataType.Flat || (data.type == WorldDataEditorObject.DataType.Person && data.isExterior))
                {
                    DFBlock.RmbBlockFlatObjectRecord record = new DFBlock.RmbBlockFlatObjectRecord();
                    record.TextureArchive = int.Parse(data.id.Split('.')[0]);
                    record.TextureRecord = int.Parse(data.id.Split('.')[1]);
                    record.FactionID = data.factionID;
                    record.Flags = data.flags;

                    modelPosition = child.transform.localPosition / MeshReader.GlobalScale;
                    if (data.isExterior)
                    {
                        // Pre-apply sub record rotation to any exterior flat coordinates.
                        Quaternion subRecordRotation = Quaternion.AngleAxis(-buildingData.RmbSubRecord.YRotation / BlocksFile.RotationDivisor, Vector3.up);
                        modelPosition = subRecordRotation * modelPosition;
                    }

                    record.XPos = Mathf.RoundToInt(modelPosition.x);
                    record.YPos = Mathf.RoundToInt(-((child.localPosition.y - (child.GetComponent<Billboard>().Summary.Size.y / 2)) / MeshReader.GlobalScale));
                    record.ZPos = Mathf.RoundToInt(modelPosition.z);

                    if (data.isExterior)
                        ArrayUtility.Add(ref buildingData.RmbSubRecord.Exterior.BlockFlatObjectRecords, record);
                    else
                        ArrayUtility.Add(ref buildingData.RmbSubRecord.Interior.BlockFlatObjectRecords, record);
                }

                else if (data.type == WorldDataEditorObject.DataType.Person)
                {
                    DFBlock.RmbBlockPeopleRecord record = new DFBlock.RmbBlockPeopleRecord();
                    record.TextureArchive = int.Parse(data.id.Split('.')[0]);
                    record.TextureRecord = int.Parse(data.id.Split('.')[1]);
                    record.FactionID = data.factionID;
                    record.Flags = data.flags;

                    modelPosition = child.transform.localPosition / MeshReader.GlobalScale;
                    record.XPos = Mathf.RoundToInt(modelPosition.x);
                    record.YPos = Mathf.RoundToInt(-((child.localPosition.y - (child.GetComponent<Billboard>().Summary.Size.y / 2)) / MeshReader.GlobalScale));
                    record.ZPos = Mathf.RoundToInt(modelPosition.z);

                    ArrayUtility.Add(ref buildingData.RmbSubRecord.Interior.BlockPeopleRecords, record);
                }

                else if (data.type == WorldDataEditorObject.DataType.Door)
                {
                    DFBlock.RmbBlockDoorRecord record = new DFBlock.RmbBlockDoorRecord();
                    record.OpenRotation = data.openRotation;
                    modelPosition = child.transform.localPosition / MeshReader.GlobalScale;
                    record.XPos = Mathf.RoundToInt(modelPosition.x);
                    record.YPos = -Mathf.RoundToInt(modelPosition.y);
                    record.ZPos = Mathf.RoundToInt(modelPosition.z);
                    record.YRotation = (short)(-child.eulerAngles.y * BlocksFile.RotationDivisor);

                    if (data.isExterior)
                        ArrayUtility.Add(ref buildingData.RmbSubRecord.Exterior.BlockDoorRecords, record);
                    else
                        ArrayUtility.Add(ref buildingData.RmbSubRecord.Interior.BlockDoorRecords, record);
                }
            }

            //Loop through all doors to give them a unique position ID
            for (int i = 0, doorID = 0; i < buildingData.RmbSubRecord.Interior.BlockDoorRecords.Length; i++)
            {
                buildingData.RmbSubRecord.Interior.BlockDoorRecords[i].Position = doorID;
                doorID++;
            }

            //Loop through all NPCS to give them a unique position ID
            int NPC_ID = buildingData.RmbSubRecord.XPos + buildingData.RmbSubRecord.ZPos + buildingData.RmbSubRecord.YRotation + buildingData.Quality;
            for (int i = 0; i < buildingData.RmbSubRecord.Interior.BlockPeopleRecords.Length; i++)
            {
                buildingData.RmbSubRecord.Interior.BlockPeopleRecords[i].Position = NPC_ID;
                NPC_ID++;
            }
            for (int i = 0; i < buildingData.RmbSubRecord.Exterior.BlockFlatObjectRecords.Length; i++)
            {
                buildingData.RmbSubRecord.Exterior.BlockFlatObjectRecords[i].Position = NPC_ID;
                NPC_ID++;
            }

            for (int i = 0; i < buildingData.RmbSubRecord.Exterior.BlockSection3Records.Length; i++)
            {
                buildingData.RmbSubRecord.Exterior.Block3dObjectRecords[i].ObjectType = 4;
            }
        }

        #endregion
    }
}
