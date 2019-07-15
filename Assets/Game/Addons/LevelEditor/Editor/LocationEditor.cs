using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DaggerfallWorkshop.Utility.AssetInjection;
using System.IO;

/// <summary>
/// Location editor
/// </summary>
namespace DaggerfallWorkshop.Loc {

    public class LocationEditor : EditorWindow {
        protected GUIStyle lightGrayBG = new GUIStyle();
        protected GUIStyle bigText = new GUIStyle();

        protected void CreataGUIStyles() {
            lightGrayBG.normal.background = LocationEditorHelper.CreateColorTexture(1, 1, new Color(0.9f, 0.9f, 0.9f, 1.0f));
            bigText.fontSize = 24;
        }

        private enum EditMode { EditMode, AddItem };
        private GameObject parent;
        private string searchField = "", selectedObjectID = "";
        private List<string> searchListNames = new List<string>(), searchListID = new List<string>();
        private EditMode editMode;
        private int objectPicker, chooseFileMode, listMode, billboardSubList, interiorSubPartSetList;
        private Vector2 scrollPosition2 = Vector2.zero;
        private string[] chooseFileModeString = { "Choose from List", "Add manually" };
        private string[] listModeString = { "3D Model", "Billboard", "NPC", "Door" ,"Int. Parts"};
        private string[] billboardSubListString = { "Interior", "Nature", "Lights", "Treasure", "Markers" };
        private string[] interiorPartsSetsString= { "BlueGrey", "brownGreyPillars"};
        private bool isExteriorMode = false;
        private BuildingReplacementData levelData;
        private string currentWorkFile;

        [MenuItem("Daggerfall Tools/Location Editor")]
        static void Init() {
            LocationEditor window = (LocationEditor)GetWindow(typeof(LocationEditor));
            window.titleContent = new GUIContent("Location Editor");
        }

        private void Awake() {
            CreataGUIStyles();
            UpdateSearchList();
        }

        void OnGUI() {

            GUIFileMenu();

            if (parent != null) {
                if (editMode == EditMode.EditMode) {
                    EditInteriorWindow();
                }

                else if (editMode == EditMode.AddItem) {
                    AddItemWindow();
                }
            }
        }

        private void GUIFileMenu() {
            GUI.BeginGroup(new Rect(8, 8, Screen.width - 16, 32), lightGrayBG);
            {
                if (GUI.Button(new Rect(8, 4, 96, 24), "Open File")) {

                    bool newFile = false;

                    if (parent != null) {
                        newFile = EditorUtility.DisplayDialog("Open New File ?", "Are you sure you wish to open a NEW file, all current unsaved changes will be lost!", "Ok", "Cancel");
                    }

                    if (newFile || parent == null) {
                        OpenFile();
                    }
                }

                else if (parent != null && GUI.Button(new Rect(128, 4, 96, 24), "Save File")) {
                    UpdateLevelData();
                    string path = EditorUtility.SaveFilePanel("Save as", LocationEditorHelper.locationFolder, currentWorkFile, "json");
                    LocationEditorHelper.SaveInterior(levelData, path);
                }
            }
            GUI.EndGroup();
        }

        private void LoadObjects() {

            foreach (var blockRecord in levelData.RmbSubRecord.Interior.Block3dObjectRecords) {
                GameObject go = LocationEditorHelper.Add3dObject(blockRecord);
                go.transform.parent = parent.transform;
                go.AddComponent<LevelEditorObject>().CreateData(blockRecord, false);
            }

            foreach (var blockRecord in levelData.RmbSubRecord.Interior.BlockFlatObjectRecords) {
                GameObject go = LocationEditorHelper.AddFlatObject(blockRecord);
                go.transform.parent = parent.transform;
                go.AddComponent<LevelEditorObject>().CreateData(blockRecord, false);
            }

            foreach (var blockRecord in levelData.RmbSubRecord.Interior.BlockPeopleRecords) {
                GameObject go = LocationEditorHelper.AddPersonObject(blockRecord);
                go.transform.parent = parent.transform;
                go.AddComponent<LevelEditorObject>().CreateData(blockRecord, false);
            }

            foreach (var blockRecord in levelData.RmbSubRecord.Interior.BlockDoorRecords) {
                GameObject go = LocationEditorHelper.AddDoorObject(blockRecord);
                go.transform.parent = parent.transform;
                go.AddComponent<LevelEditorObject>().CreateData(blockRecord, false);
            }

            foreach (var blockRecord in levelData.RmbSubRecord.Exterior.Block3dObjectRecords) {
                GameObject go = LocationEditorHelper.Add3dObject(blockRecord);
                go.transform.parent = parent.transform;
                go.AddComponent<LevelEditorObject>().CreateData(blockRecord, true);
            }

            foreach (var blockRecord in levelData.RmbSubRecord.Exterior.BlockFlatObjectRecords) {
                GameObject go = LocationEditorHelper.AddFlatObject(blockRecord);
                go.transform.parent = parent.transform;
                go.AddComponent<LevelEditorObject>().CreateData(blockRecord, true);
            }

            foreach (var blockRecord in levelData.RmbSubRecord.Exterior.BlockPeopleRecords) {
                GameObject go = LocationEditorHelper.AddPersonObject(blockRecord);
                go.transform.parent = parent.transform;
                go.AddComponent<LevelEditorObject>().CreateData(blockRecord, true);
            }

            foreach (var blockRecord in levelData.RmbSubRecord.Exterior.BlockDoorRecords) {
                GameObject go = LocationEditorHelper.AddDoorObject(blockRecord);
                go.transform.parent = parent.transform;
                go.AddComponent<LevelEditorObject>().CreateData(blockRecord, true);
            }

            UpdateObjectsVisibility();
        }

        private void UpdateObjectsVisibility() {

            foreach (Transform child in parent.GetComponentInChildren<Transform>()) {

                if (child.GetComponent<LevelEditorObject>() == null)
                    return;

                child.GetComponent<LevelEditorObject>().UpdateVisibility(isExteriorMode);
            }
        }

        private void EditInteriorWindow() {

            Repaint(); //Keep this repainted

            GUI.BeginGroup(new Rect(8, 48, Screen.width - 16, 32), lightGrayBG);
            {
                if (GUI.Button(new Rect(8, 4, 96, 24), "Add Object")) {
                    editMode = EditMode.AddItem;
                }

                else if (parent != null && GUI.Button(new Rect(128, 4, 196, 24), "Toggle Interior/Exterior")) {
                    isExteriorMode = !isExteriorMode;
                    UpdateObjectsVisibility();
                }
            }
            GUI.EndGroup();

            if (Selection.activeGameObject == null)
                return;

            if (Selection.Contains(parent)) {

                GUI.Label(new Rect(16, 96, 96, 28), "XPos: ");
                levelData.RmbSubRecord.XPos = EditorGUI.IntField(new Rect(128, 96, 128, 16), levelData.RmbSubRecord.XPos);

                GUI.Label(new Rect(16, 128, 96, 28), "YPos: ");
                levelData.RmbSubRecord.ZPos = EditorGUI.IntField(new Rect(128, 128, 128, 16), levelData.RmbSubRecord.ZPos);

                GUI.Label(new Rect(16, 150, 96, 28), "YRotation: ");
                levelData.RmbSubRecord.YRotation = EditorGUI.IntField(new Rect(128, 150, 128, 16), levelData.RmbSubRecord.YRotation);

                GUI.Label(new Rect(16, 182, 96, 28), "Building Type: ");
                levelData.BuildingType = (int)(DaggerfallConnect.DFLocation.BuildingTypes)EditorGUI.EnumPopup(new Rect(128, 182, 128, 16), (DaggerfallConnect.DFLocation.BuildingTypes)levelData.BuildingType);
 
                GUI.Label(new Rect(16, 214, 96, 28), "Faction ID: ");
                levelData.FactionId = (ushort)EditorGUI.IntField(new Rect(128, 214, 128, 16), levelData.FactionId);

                GUI.Label(new Rect(16, 236, 96, 28), "Quality: ");
                levelData.Quality = (byte)EditorGUI.IntSlider(new Rect(128, 236, 128, 16), levelData.Quality, 1, 20);
            }

            else {

                if (Selection.activeGameObject.GetComponent<LevelEditorObject>()) {

                    LevelEditorObject data = Selection.activeGameObject.GetComponent<LevelEditorObject>();

                    GUI.Label(new Rect(16, 96, 96, 16), "Transform ");
                    GUI.BeginGroup(new Rect(8, 112, Screen.width - 16, 136), lightGrayBG);
                    {
                        data.transform.localPosition = EditorGUI.Vector3Field(new Rect(32, 8, 312, 32), "Position", data.transform.localPosition);
                        data.transform.eulerAngles = EditorGUI.Vector3Field(new Rect(32, 48, 312, 32), "Rotation", data.transform.eulerAngles);
                    }
                    GUI.EndGroup();

                    //3D object
                    if (data.type == 0) {
                        GUI.Label(new Rect(16, 258, 96, 28), "Object Type: ");
                        data.objectType = (byte)(LocationEditorHelper.ObjectType)EditorGUI.EnumPopup(new Rect(128, 258, 128, 16),(LocationEditorHelper.ObjectType)data.objectType);

                    }
                    //Flat object
                    else if (data.type == 1) {
                        GUI.Label(new Rect(16, 258, 96, 28), "Faction ID: ");
                        data.factionID = (short)EditorGUI.IntField(new Rect(128, 258, 128, 16), data.factionID);

                        GUI.Label(new Rect(16, 280, 96, 28), "Flags: ");
                        data.flags = (byte)EditorGUI.IntField(new Rect(128, 280, 128, 16), data.flags);

                        if(Input.GetKeyDown(KeyCode.Keypad2)) {
                            data.transform.position = new Vector3();
                            Debug.Log("Tst");
                        }
                    }
                    //Person object
                    else if (data.type == 2) {
                        GUI.Label(new Rect(16, 258, 96, 28), "Faction ID: ");
                        data.factionID = (short)EditorGUI.IntField(new Rect(128, 258, 128, 16), data.factionID);

                        GUI.Label(new Rect(16, 280, 96, 28), "Flags: ");
                        data.flags = (byte)EditorGUI.IntField(new Rect(128, 280, 128, 16), data.flags);
                    }
                    //Door object
                    else if (data.type == 3) {
                        GUI.Label(new Rect(16, 258, 96, 28), "Open Rotation: ");
                        data.openRotation = (byte)EditorGUI.IntField(new Rect(128, 258, 96, 16), data.openRotation);
                    }
                }
                else {
                    GUI.TextArea(new Rect(24, 128, 196, 32), "No level object selected", bigText);
                }
            }
        }

        private void AddItemWindow() {

            GUI.BeginGroup(new Rect(8, 48, Screen.width - 16, 32), lightGrayBG);
            chooseFileMode = GUI.SelectionGrid(new Rect(8, 4, 312, 24), chooseFileMode, chooseFileModeString, 2);
            GUI.EndGroup();

            GUI.BeginGroup(new Rect(8, 84, Screen.width - 16, 32), lightGrayBG);
            listMode = GUI.SelectionGrid(new Rect(8, 4, (listModeString.Length*80)+8, 24), listMode, listModeString, listModeString.Length);
            GUI.EndGroup();

            if (chooseFileMode == 0) {

                if (listMode == 1) {

                    GUI.BeginGroup(new Rect(8, 124, Screen.width - 16, 32), lightGrayBG);
                    billboardSubList = GUI.SelectionGrid(new Rect(8, 4, (billboardSubListString.Length * 80) + 8, 24), billboardSubList, billboardSubListString, billboardSubListString.Length);
                    GUI.EndGroup();
                }

                else if (listMode == 4) {
                    GUI.BeginGroup(new Rect(8, 124, Screen.width - 16, 32), lightGrayBG);
                    interiorSubPartSetList = GUI.SelectionGrid(new Rect(8, 4, (interiorPartsSetsString.Length * 80) + 8, 24), interiorSubPartSetList, interiorPartsSetsString, interiorPartsSetsString.Length);
                    GUI.EndGroup();

                }

                if (listMode != 3) {

                    GUI.Label(new Rect(new Rect(16, 160, 64, 16)), "Search: ");
                    searchField = EditorGUI.TextField(new Rect(70, 160, 156, 16), searchField);

                    if (GUI.changed)
                        UpdateSearchList();

                    scrollPosition2 = GUI.BeginScrollView(new Rect(4, 184, 256, 368), scrollPosition2, new Rect(0, 0, 236, 20 + (searchListNames.Count * 24)));
                    objectPicker = GUI.SelectionGrid(new Rect(10, 10, 216, searchListNames.Count * 24), objectPicker, searchListNames.ToArray(), 1);
                    GUI.EndScrollView();
                }
            }

            else if (chooseFileMode == 1) {

                GUI.Label(new Rect(new Rect(16, 160, 96, 16)), "Object ID : ");
                selectedObjectID = EditorGUI.TextField(new Rect(128, 160, 156, 16), selectedObjectID);
            }

            if (GUI.Button(new Rect(16, 582, 96, 20), "OK")) {

                if (chooseFileMode == 0)
                    selectedObjectID = searchListID[objectPicker];

                GameObject go = null;

                if (listMode == 0 || listMode == 4) {
                    DaggerfallConnect.DFBlock.RmbBlock3dObjectRecord blockRecord = new DaggerfallConnect.DFBlock.RmbBlock3dObjectRecord();
                    blockRecord.ModelId = selectedObjectID;
                    blockRecord.ModelIdNum = uint.Parse(selectedObjectID);

                    if (listMode == 4)
                        blockRecord.ObjectType = (byte)LocationEditorHelper.ObjectType.InteriorHousePart;

                    go = LocationEditorHelper.Add3dObject(blockRecord);
                    go.AddComponent<LevelEditorObject>().CreateData(blockRecord, isExteriorMode);
                }

                else if (listMode == 1) {
                    DaggerfallConnect.DFBlock.RmbBlockFlatObjectRecord blockRecord = new DaggerfallConnect.DFBlock.RmbBlockFlatObjectRecord();
                    blockRecord.TextureArchive = int.Parse(selectedObjectID.Split('.')[0]);
                    blockRecord.TextureRecord = int.Parse(selectedObjectID.Split('.')[1]);
                    go = LocationEditorHelper.AddFlatObject(blockRecord);
                    go.AddComponent<LevelEditorObject>().CreateData(blockRecord, isExteriorMode);
                }

                else if (listMode == 2) {
                    DaggerfallConnect.DFBlock.RmbBlockPeopleRecord blockRecord = new DaggerfallConnect.DFBlock.RmbBlockPeopleRecord();
                    blockRecord.TextureArchive = int.Parse(selectedObjectID.Split('.')[0]);
                    blockRecord.TextureRecord = int.Parse(selectedObjectID.Split('.')[1]);
                    go = LocationEditorHelper.AddPersonObject(blockRecord);
                    go.AddComponent<LevelEditorObject>().CreateData(blockRecord, isExteriorMode);
                }

                else if (listMode == 3) {
                    DaggerfallConnect.DFBlock.RmbBlockDoorRecord blockRecord = new DaggerfallConnect.DFBlock.RmbBlockDoorRecord();
                    blockRecord.OpenRotation = 95; //Seems to be the default rotation used in the game
                    go = LocationEditorHelper.AddDoorObject(blockRecord);
                    go.AddComponent<LevelEditorObject>().CreateData(blockRecord, isExteriorMode);
                }

                if (go != null) {
                    go.transform.parent = parent.transform;

                    Ray newRay = new Ray(SceneView.lastActiveSceneView.camera.transform.position, SceneView.lastActiveSceneView.camera.transform.forward);
                    RaycastHit hit = new RaycastHit();
                    if (Physics.Raycast(newRay, out hit, 200)) {
                        go.transform.position = hit.point;
                    }
                }
                else {

                }

                editMode = EditMode.EditMode;
            }

            if (GUI.Button(new Rect(128, 582, 96, 20), "Cancel")) {
                editMode = EditMode.EditMode;
            }
        }

        private void UpdateSearchList() {

            searchListNames.Clear();
            searchListID.Clear();
            Dictionary<string, string> currentList;

            if (listMode == 0)
                currentList = LocationEditorHelper.models;
            else if (listMode == 1 && billboardSubList == 0)
                currentList = LocationEditorHelper.billboards_interior;
            else if (listMode == 1 && billboardSubList == 1)
                currentList = LocationEditorHelper.billboards_nature;
            else if (listMode == 1 && billboardSubList == 2)
                currentList = LocationEditorHelper.billboards_lights;
            else if (listMode == 1 && billboardSubList == 3)
                currentList = LocationEditorHelper.billboards_treasure;
            else if (listMode == 1 && billboardSubList == 4)
                currentList = LocationEditorHelper.billboards_markers;
            else if (listMode == 4 && interiorSubPartSetList == 0)
                currentList = LocationEditorHelper.blueGrayHouseParts;
            else if (listMode == 4 && interiorSubPartSetList == 1)
                currentList = LocationEditorHelper.brownGreyPillarsHouseParts;
            else
                currentList = LocationEditorHelper.NPCs;

            foreach (KeyValuePair<string, string> pair in currentList) {
                if (pair.Value.ToLower().Contains(searchField.ToLower())) {
                    searchListNames.Add(pair.Value);
                    searchListID.Add(pair.Key);
                }
            }
        }

        private void OnDestroy() {
            if (parent != null)
                DestroyImmediate(parent.gameObject);
        }

        private void CreateNewFile() {
            if (parent != null)
                DestroyImmediate(parent);

            levelData = new BuildingReplacementData();
            parent = new GameObject("Location Prefab");
        }

        private void OpenFile() {

            string path = EditorUtility.OpenFilePanel("Open", LocationEditorHelper.locationFolder, "json");

            if (LocationEditorHelper.LoadInterior(path, out levelData)) {

                if (parent != null) {
                    DestroyImmediate(parent);
                }

                currentWorkFile = Path.GetFileName(path);

                parent = new GameObject("Location : " + currentWorkFile);
                LoadObjects();
                editMode = EditMode.EditMode;
            }
            else {
                path = "";
            }

            //We clear the Undo
            Undo.ClearAll();
        }

        private void UpdateLevelData() {

            ArrayUtility.Clear(ref levelData.RmbSubRecord.Exterior.Block3dObjectRecords);
            ArrayUtility.Clear(ref levelData.RmbSubRecord.Interior.Block3dObjectRecords);
            ArrayUtility.Clear(ref levelData.RmbSubRecord.Exterior.BlockFlatObjectRecords);
            ArrayUtility.Clear(ref levelData.RmbSubRecord.Interior.BlockFlatObjectRecords);
            ArrayUtility.Clear(ref levelData.RmbSubRecord.Exterior.BlockPeopleRecords);
            ArrayUtility.Clear(ref levelData.RmbSubRecord.Interior.BlockPeopleRecords);
            ArrayUtility.Clear(ref levelData.RmbSubRecord.Exterior.BlockDoorRecords);
            ArrayUtility.Clear(ref levelData.RmbSubRecord.Interior.BlockDoorRecords);

            Vector3 modelPosition;
            LevelEditorObject data;

            foreach (Transform child in parent.GetComponentInChildren<Transform>()) {

                if (child.GetComponent<LevelEditorObject>() == null)
                    return;

                data = child.GetComponent<LevelEditorObject>();

                //3D models
                if (data.type == 0) {
                    DaggerfallConnect.DFBlock.RmbBlock3dObjectRecord record = new DaggerfallConnect.DFBlock.RmbBlock3dObjectRecord();
                    record.ModelId = data.id;
                    record.ModelIdNum = uint.Parse(data.id);
                    record.ObjectType = data.objectType;

                    if (data.objectType == 3) {

                        Vector3[] vertices = child.GetComponent<MeshFilter>().sharedMesh.vertices;

                        // Props axis needs to be transformed to lowest Y point
                        Vector3 bottom = vertices[0];
                        for (int j = 0; j < vertices.Length; j++) {
                            if (vertices[j].y < bottom.y)
                                bottom = vertices[j];
                        }
                        modelPosition = new Vector3(child.localPosition.x, (child.localPosition.y + (bottom.y)), child.localPosition.z) / MeshReader.GlobalScale;
                    }
                    else {
                        modelPosition = new Vector3(child.localPosition.x, -child.localPosition.y, child.localPosition.z) / MeshReader.GlobalScale;
                    }

                    record.XPos = Mathf.RoundToInt(modelPosition.x);
                    record.YPos = Mathf.RoundToInt(modelPosition.y);
                    record.ZPos = Mathf.RoundToInt(modelPosition.z);
                    record.XRotation = (short)(-child.eulerAngles.x * DaggerfallConnect.Arena2.BlocksFile.RotationDivisor);
                    record.YRotation = (short)(-child.eulerAngles.y * DaggerfallConnect.Arena2.BlocksFile.RotationDivisor);
                    record.ZRotation = (short)(-child.eulerAngles.z * DaggerfallConnect.Arena2.BlocksFile.RotationDivisor);

                    if (data.isExterior)
                        ArrayUtility.Add(ref levelData.RmbSubRecord.Exterior.Block3dObjectRecords, record);
                    else
                        ArrayUtility.Add(ref levelData.RmbSubRecord.Interior.Block3dObjectRecords, record);
                }

                else if (data.type == 1) {

                    DaggerfallConnect.DFBlock.RmbBlockFlatObjectRecord record = new DaggerfallConnect.DFBlock.RmbBlockFlatObjectRecord();
                    record.TextureArchive = int.Parse(data.id.Split('.')[0]);
                    record.TextureRecord = int.Parse(data.id.Split('.')[1]);
                    record.FactionID = data.factionID;
                    record.Flags = data.flags;

                    modelPosition = child.transform.localPosition / MeshReader.GlobalScale;
                    record.XPos = Mathf.RoundToInt(modelPosition.x);
                    record.YPos = Mathf.RoundToInt(-((child.localPosition.y - (child.GetComponent<DaggerfallBillboard>().Summary.Size.y / 2)) / MeshReader.GlobalScale));
                    record.ZPos = Mathf.RoundToInt(modelPosition.z);

                    if (data.isExterior)
                        ArrayUtility.Add(ref levelData.RmbSubRecord.Exterior.BlockFlatObjectRecords, record);
                    else
                        ArrayUtility.Add(ref levelData.RmbSubRecord.Interior.BlockFlatObjectRecords, record);
                }

                else if (data.type == 2) {

                    DaggerfallConnect.DFBlock.RmbBlockPeopleRecord record = new DaggerfallConnect.DFBlock.RmbBlockPeopleRecord();
                    record.TextureArchive = int.Parse(data.id.Split('.')[0]);
                    record.TextureRecord = int.Parse(data.id.Split('.')[1]);
                    record.FactionID = data.factionID;
                    record.Flags = data.flags;

                    modelPosition = child.transform.localPosition / MeshReader.GlobalScale;
                    record.XPos = Mathf.RoundToInt(modelPosition.x);
                    record.YPos = Mathf.RoundToInt(-((child.localPosition.y - (child.GetComponent<DaggerfallBillboard>().Summary.Size.y / 2)) / MeshReader.GlobalScale));
                    record.ZPos = Mathf.RoundToInt(modelPosition.z);

                    if (data.isExterior)
                        ArrayUtility.Add(ref levelData.RmbSubRecord.Exterior.BlockPeopleRecords, record);
                    else
                        ArrayUtility.Add(ref levelData.RmbSubRecord.Interior.BlockPeopleRecords, record);
                }

                else if (data.type == 3) {

                    DaggerfallConnect.DFBlock.RmbBlockDoorRecord record = new DaggerfallConnect.DFBlock.RmbBlockDoorRecord();
                    record.OpenRotation = data.openRotation;
                    modelPosition = child.transform.localPosition / MeshReader.GlobalScale;
                    record.XPos = Mathf.RoundToInt(modelPosition.x);
                    record.YPos = -Mathf.RoundToInt(modelPosition.y);
                    record.ZPos = Mathf.RoundToInt(modelPosition.z);
                    record.YRotation = (short)(-child.eulerAngles.y * DaggerfallConnect.Arena2.BlocksFile.RotationDivisor);

                    if (data.isExterior)
                        ArrayUtility.Add(ref levelData.RmbSubRecord.Exterior.BlockDoorRecords, record);
                    else
                        ArrayUtility.Add(ref levelData.RmbSubRecord.Interior.BlockDoorRecords, record);
                }
            }
        }
    }
}