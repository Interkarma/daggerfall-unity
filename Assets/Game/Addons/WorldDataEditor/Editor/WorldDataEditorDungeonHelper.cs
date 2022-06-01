// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    

using System.IO;
using UnityEngine;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallConnect;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect.Arena2;
using UnityEditor;

namespace DaggerfallWorkshop.Game.Utility.WorldDataEditor
{
    public static class WorldDataEditorDungeonHelper
    {

        public static bool LoadDungeonFile(string path, out DFBlock rdbData)
        {
            rdbData = new DFBlock();

            if (string.IsNullOrEmpty(path)) {
                Debug.LogError("Error: Trying to open file, but path is null or empty");
                return false;
            }

            if (!File.Exists(path)) {
                Debug.LogError("Error: Trying to open file at invalid path: " + path);
                return false;
            }

            StreamReader reader = new StreamReader(path, false);
            rdbData = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), reader.ReadToEnd());

            reader.Close();
            return true;
        }

        public static void SaveDungeonFile(DFBlock rdbData, string path)
        {
            if (string.IsNullOrEmpty(path)) {
                return;
            }
            StreamWriter writer = new StreamWriter(path, false);
            string saveData = SaveLoadManager.Serialize(rdbData.GetType(), rdbData);
            writer.Write(saveData);
            writer.Close();
        }
        
        public static GameObject Add3dObject(DFBlock.RdbObject obj, ref DFBlock.RdbModelReference[] modelReferenceList)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;

            // Get model reference index and id
            int modelReference = obj.Resources.ModelResource.ModelIndex;
            uint modelId = modelReferenceList[modelReference].ModelIdNum;

            // Get matrix
            Vector3 modelRotation = new Vector3(-obj.Resources.ModelResource.XRotation / BlocksFile.RotationDivisor, -obj.Resources.ModelResource.YRotation / BlocksFile.RotationDivisor, -obj.Resources.ModelResource.ZRotation / BlocksFile.RotationDivisor);
            Vector3 modelPosition = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;
            Matrix4x4 modelMatrix = Matrix4x4.TRS(modelPosition, Quaternion.Euler(modelRotation), Vector3.one);

            // Get model data
            ModelData modelData;
            dfUnity.MeshReader.GetModelData(modelId, out modelData);

            // Get GameObject
            GameObject modelGO = MeshReplacement.ImportCustomGameobject(modelId, null, modelMatrix, false);
            if (modelGO == null)
            {
                if (modelData.DFMesh.TotalVertices != 0)
                {
                    modelGO = GameObjectHelper.CreateDaggerfallMeshGameObject(modelId, null);
                    modelGO.transform.position = modelMatrix.GetColumn(3);
                    modelGO.transform.rotation = modelMatrix.rotation;
                    modelGO.transform.localScale = modelMatrix.lossyScale;
                }
                else
                    Debug.LogError("Custom model not found for modelId " + modelId);
            }
            return modelGO;
        }

        public static GameObject Preview3dObject(string modelId)
        {
            Vector3 position = SceneView.lastActiveSceneView.camera.transform.position + (SceneView.lastActiveSceneView.camera.transform.forward * 6);

            return Preview3dObject(modelId, position);
        }

        public static GameObject Preview3dObject(string modelId, Vector3 position)
        {
            // Get matrix
            Matrix4x4 matrix = Matrix4x4.identity;
            matrix *= Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);

            // Get model data
            uint modelIdNum = uint.Parse(modelId);
            ModelData modelData;
            DaggerfallUnity.Instance.MeshReader.GetModelData(modelIdNum, out modelData);

            if (modelData.DFMesh.TotalVertices != 0)
            {
                GameObject modelGO = GameObjectHelper.CreateDaggerfallMeshGameObject(modelIdNum, null);
                modelGO.transform.position = matrix.GetColumn(3);
                modelGO.name = "Model Preview: " + modelId;
                return modelGO;
            }

            Debug.LogWarning("Vanilla model not found for preview, modelId " + modelId);
            return null;
        }

        public static GameObject AddFlatObject(DFBlock.RdbObject obj)
        {
            int archive = obj.Resources.FlatResource.TextureArchive;
            int record = obj.Resources.FlatResource.TextureRecord;

            // Add GameObject to scene
            Vector3 targetPosition = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;
            GameObject go = MeshReplacement.ImportCustomFlatGameobject(archive, record, targetPosition, null, true);
            if (!go)
            {
                // Setup standard billboard and assign RDB data
                go = GameObjectHelper.CreateDaggerfallBillboardGameObject(archive, record, null);
                go.transform.position = targetPosition;
                Billboard dfBillboard = go.GetComponent<Billboard>();
                dfBillboard.SetRDBResourceData(obj.Resources.FlatResource);
            }
            return go;
        }

        public static GameObject AddLightObject(Transform parent, DFBlock.RdbObject obj)
        {
            // Spawn light gameobject
            float range = obj.Resources.LightResource.Radius * MeshReader.GlobalScale;
            Vector3 position = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;
            GameObject go = GameObjectHelper.InstantiatePrefab(DaggerfallUnity.Instance.Option_InteriorLightPrefab.gameObject, string.Empty, parent, position);
            Light light = go.GetComponent<Light>();
            if (light != null)
                light.range = range * 3;
            return go;
        }

    }
}