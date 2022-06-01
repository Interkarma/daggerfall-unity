// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Uncanny Valley
// Contributors:    Hazelnut

using System.IO;
using UnityEngine;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Game.Utility.WorldDataEditor
{
    public static class WorldDataEditorBuildingHelper
    {
        public const byte InteriorHousePart = 13;
        public const byte ExteriorBuilding = 4;
        public const byte InteractiveObject = 3;

        public static bool LoadBuildingFile(string path, out BuildingReplacementData buildingReplacement)
        {
            buildingReplacement = new BuildingReplacementData();

            if (string.IsNullOrEmpty(path)) {
                Debug.LogError("Error: Trying to open file, but path is null or empty");
                return false;
            }

            if (!File.Exists(path)) {
                Debug.LogError("Error: Trying to open file at invalid path: " + path);
                return false;
            }

            StreamReader reader = new StreamReader(path, false);
            buildingReplacement = (BuildingReplacementData)SaveLoadManager.Deserialize(typeof(BuildingReplacementData), reader.ReadToEnd());

            reader.Close();

            return true;
        }

        public static void SaveBuildingFile(BuildingReplacementData interiorData, string path)
        {
            if (string.IsNullOrEmpty(path)) {
                return;
            }

            StreamWriter writer = new StreamWriter(path, false);
            string saveData = SaveLoadManager.Serialize(interiorData.GetType(), interiorData);
            writer.Write(saveData);
            writer.Close();
        }

        public static GameObject Add3dObject(DFBlock.RmbBlock3dObjectRecord rmbBlock)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;

            // Get model data
            ModelData modelData;
            dfUnity.MeshReader.GetModelData(rmbBlock.ModelIdNum, out modelData);

            // Get model position by type (3 seems to indicate props/clutter)
            // Also stop these from being combined as some may carry a loot container
            Vector3 modelPosition;
            if (rmbBlock.ObjectType == (int)InteractiveObject) {
                // Props axis needs to be transformed to lowest Y point
                Vector3 bottom = modelData.Vertices[0];
                for (int i = 0; i < modelData.Vertices.Length; i++) {
                    if (modelData.Vertices[i].y < bottom.y)
                        bottom = modelData.Vertices[i];
                }
                modelPosition = new Vector3(rmbBlock.XPos, rmbBlock.YPos, rmbBlock.ZPos) * MeshReader.GlobalScale;
                modelPosition += new Vector3(0, -bottom.y, 0);
            }
            else {
                modelPosition = new Vector3(rmbBlock.XPos, -rmbBlock.YPos, rmbBlock.ZPos) * MeshReader.GlobalScale;
            }

            // Fix 3D models with 0,0,0 scale
            if (rmbBlock.XScale == 0)
                rmbBlock.XScale = 1;
            if (rmbBlock.YScale == 0)
                rmbBlock.YScale = 1;
            if (rmbBlock.ZScale == 0)
                rmbBlock.ZScale = 1;

            // Get model transform
            Vector3 modelRotation = new Vector3(-rmbBlock.XRotation / BlocksFile.RotationDivisor, -rmbBlock.YRotation / BlocksFile.RotationDivisor, -rmbBlock.ZRotation / BlocksFile.RotationDivisor);                    
            Vector3 modelScale = new Vector3(rmbBlock.XScale, rmbBlock.YScale, rmbBlock.ZScale);
            Matrix4x4 modelMatrix = Matrix4x4.TRS(modelPosition, Quaternion.Euler(modelRotation), modelScale);

            // Inject custom GameObject if available
            GameObject modelGO = MeshReplacement.ImportCustomGameobject(rmbBlock.ModelIdNum, null, modelMatrix);
            if (modelGO == null)
            {
                if (modelData.DFMesh.TotalVertices != 0)
                {
                    modelGO = DaggerfallWorkshop.Utility.GameObjectHelper.CreateDaggerfallMeshGameObject(rmbBlock.ModelIdNum, null);
                    modelGO.transform.position = modelMatrix.GetColumn(3);
                    modelGO.transform.rotation = modelMatrix.rotation;
                    modelGO.transform.localScale = modelMatrix.lossyScale;
                }
                else
                    Debug.LogError("Custom model not found for modelId " + rmbBlock.ModelIdNum);
            }
            return modelGO;
        }

        public static GameObject AddDoorObject(DFBlock.RmbBlockDoorRecord rmbBlock)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;

            // Get model data
            ModelData modelData;
            dfUnity.MeshReader.GetModelData(9000, out modelData);

            Vector3 modelRotation = new Vector3(0, -rmbBlock.YRotation / BlocksFile.RotationDivisor, 0);
            Vector3 modelPosition = new Vector3(rmbBlock.XPos, -rmbBlock.YPos, rmbBlock.ZPos) * MeshReader.GlobalScale;

            //Matrix4x4 modelMatrix = Matrix4x4.TRS(modelPosition, Quaternion.Euler(modelRotation), Vector3.one);

            GameObject go = DaggerfallWorkshop.Utility.GameObjectHelper.CreateDaggerfallMeshGameObject(9000, null);
            go.transform.rotation = Quaternion.Euler(modelRotation);
            go.transform.position = modelPosition;
            return go;
        }

        public static GameObject AddFlatObject(DFBlock.RmbBlockFlatObjectRecord rmbBlock, Quaternion subRecordRotation)
        {
            Vector3 billboardPosition = new Vector3(rmbBlock.XPos, -rmbBlock.YPos, rmbBlock.ZPos) * MeshReader.GlobalScale;
            billboardPosition = subRecordRotation * billboardPosition;

            // Spawn billboard gameobject
            GameObject go = DaggerfallWorkshop.Utility.GameObjectHelper.CreateDaggerfallBillboardGameObject(rmbBlock.TextureArchive, rmbBlock.TextureRecord, null);

            // Set position
            Billboard dfBillboard = go.GetComponent<Billboard>();
            go.transform.position = billboardPosition;
            go.transform.position += new Vector3(0, dfBillboard.Summary.Size.y / 2, 0);

            return go;
        }

        public static GameObject AddPersonObject(DFBlock.RmbBlockPeopleRecord rmbBlock)
        {
            Vector3 billboardPosition = new Vector3(rmbBlock.XPos, -rmbBlock.YPos, rmbBlock.ZPos) * MeshReader.GlobalScale;

            // Spawn billboard gameobject
            GameObject go = DaggerfallWorkshop.Utility.GameObjectHelper.CreateDaggerfallBillboardGameObject(rmbBlock.TextureArchive, rmbBlock.TextureRecord, null);

            // Set position
            Billboard dfBillboard = go.GetComponent<Billboard>();
            go.transform.position = billboardPosition;
            go.transform.position += new Vector3(0, dfBillboard.Summary.Size.y / 2, 0);

            // Add RMB data to billboard
            dfBillboard.SetRMBPeopleData(rmbBlock);

            return go;
        }

        public static Texture2D CreateColorTexture(int width, int height, Color color)
        {
            Texture2D colorTexture = new Texture2D(width, height);

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    colorTexture.SetPixel(x, y, color);

            colorTexture.Apply();
            return colorTexture;
        }

        public static void AddLight(Transform parent, DFBlock.RmbBlockFlatObjectRecord obj)
        {
            if (DaggerfallUnity.Instance.Option_InteriorLightPrefab == null)
                return;

            // Create gameobject
            GameObject go = DaggerfallWorkshop.Utility.GameObjectHelper.InstantiatePrefab(DaggerfallUnity.Instance.Option_InteriorLightPrefab.gameObject, string.Empty, parent, Vector3.zero);

            // Set local position to billboard origin, otherwise light transform is at base of billboard
            go.transform.localPosition = Vector3.zero;

            go.hideFlags = HideFlags.HideInHierarchy;

            // Adjust position of light for standing lights as their source comes more from top than middle
            Vector2 size = DaggerfallUnity.Instance.MeshReader.GetScaledBillboardSize(210, obj.TextureRecord) * MeshReader.GlobalScale;
            switch (obj.TextureRecord)
            {
                case 0:         // Bowl with fire
                    go.transform.localPosition += new Vector3(0, -0.1f, 0);
                    break;
                case 1:         // Campfire
                                // todo
                    break;
                case 2:         // Skull candle
                    go.transform.localPosition += new Vector3(0, 0.1f, 0);
                    break;
                case 3:         // Candle
                    go.transform.localPosition += new Vector3(0, 0.1f, 0);
                    break;
                case 4:         // Candle in bowl
                                // todo
                    break;
                case 5:         // Candleholder with 3 candles
                    go.transform.localPosition += new Vector3(0, 0.15f, 0);
                    break;
                case 6:         // Skull torch
                    go.transform.localPosition += new Vector3(0, 0.6f, 0);
                    break;
                case 7:         // Wooden chandelier with extinguished candles
                                // todo
                    break;
                case 8:         // Turkis lamp
                                // do nothing
                    break;
                case 9:        // Metallic chandelier with burning candles
                    go.transform.localPosition += new Vector3(0, 0.4f, 0);
                    break;
                case 10:         // Metallic chandelier with extinguished candles
                                 // todo
                    break;
                case 11:        // Candle in lamp
                    go.transform.localPosition += new Vector3(0, -0.4f, 0);
                    break;
                case 12:         // Extinguished lamp
                                 // todo
                    break;
                case 13:        // Round lamp (e.g. main lamp in mages guild)
                    go.transform.localPosition += new Vector3(0, -0.35f, 0);
                    break;
                case 14:        // Standing lantern
                    go.transform.localPosition += new Vector3(0, size.y / 2, 0);
                    break;
                case 15:        // Standing lantern round
                    go.transform.localPosition += new Vector3(0, size.y / 2, 0);
                    break;
                case 16:         // Mounted Torch with thin holder
                                 // todo
                    break;
                case 17:        // Mounted torch 1
                    go.transform.localPosition += new Vector3(0, 0.2f, 0);
                    break;
                case 18:         // Mounted Torch 2
                                 // todo
                    break;
                case 19:         // Pillar with firebowl
                                 // todo
                    break;
                case 20:        // Brazier torch
                    go.transform.localPosition += new Vector3(0, 0.6f, 0);
                    break;
                case 21:        // Standing candle
                    go.transform.localPosition += new Vector3(0, size.y / 2.4f, 0);
                    break;
                case 22:         // Round lantern with medium chain
                    go.transform.localPosition += new Vector3(0, -0.5f, 0);
                    break;
                case 23:         // Wooden chandelier with burning candles
                                 // todo
                    break;
                case 24:        // Lantern with long chain
                    go.transform.localPosition += new Vector3(0, -1.85f, 0);
                    break;
                case 25:        // Lantern with medium chain
                    go.transform.localPosition += new Vector3(0, -1.0f, 0);
                    break;
                case 26:        // Lantern with short chain
                                // todo
                    break;
                case 27:        // Lantern with no chain
                    go.transform.localPosition += new Vector3(0, -0.02f, 0);
                    break;
                case 28:        // Street Lantern 1
                                // todo
                    break;
                case 29:        // Street Lantern 2
                                // todo
                    break;
            }

            // adjust properties of light sources (e.g. Shrink light radius of candles)
            Light light = go.GetComponent<Light>();
            switch (obj.TextureRecord)
            {
                case 0:         // Bowl with fire
                    light.range = 20.0f;
                    light.intensity = 1.1f;
                    light.color = new Color(0.95f, 0.91f, 0.63f);
                    break;
                case 1:         // Campfire
                                // todo
                    break;
                case 2:         // Skull candle
                    light.range /= 3f;
                    light.intensity = 0.6f;
                    light.color = new Color(1.0f, 0.99f, 0.82f);
                    break;
                case 3:         // Candle
                    light.range /= 3f;
                    break;
                case 4:         // Candle with base
                    light.range /= 3f;
                    break;
                case 5:         // Candleholder with 3 candles
                    light.range = 7.5f;
                    light.intensity = 0.33f;
                    light.color = new Color(1.0f, 0.89f, 0.61f);
                    break;
                case 6:         // Skull torch
                    light.range = 15.0f;
                    light.intensity = 0.75f;
                    light.color = new Color(1.0f, 0.93f, 0.62f);
                    break;
                case 7:         // Wooden chandelier with extinguished candles
                                // todo
                    break;
                case 8:         // Turkis lamp
                    light.color = new Color(0.68f, 1.0f, 0.94f);
                    break;
                case 9:        // metallic chandelier with burning candles
                    light.range = 15.0f;
                    light.intensity = 0.65f;
                    light.color = new Color(1.0f, 0.92f, 0.6f);
                    break;
                case 10:         // Metallic chandelier with extinguished candles
                                 // todo
                    break;
                case 11:        // Candle in lamp
                    light.range = 5.0f;
                    light.intensity = 0.5f;
                    break;
                case 12:         // Extinguished lamp
                                 // todo
                    break;
                case 13:        // Round lamp (e.g. main lamp in mages guild)
                    light.range *= 1.2f;
                    light.intensity = 1.1f;
                    light.color = new Color(0.93f, 0.84f, 0.49f);
                    break;
                case 14:        // Standing lantern
                                // todo
                    break;
                case 15:        // Standing lantern round
                                // todo
                    break;
                case 16:         // Mounted Torch with thin holder
                                 // todo
                    break;
                case 17:        // Mounted torch 1
                    light.intensity = 0.8f;
                    light.color = new Color(1.0f, 0.97f, 0.87f);
                    break;
                case 18:         // Mounted Torch 2
                                 // todo
                    break;
                case 19:         // Pillar with firebowl
                                 // todo
                    break;
                case 20:        // Brazier torch
                    light.range = 12.0f;
                    light.intensity = 0.75f;
                    light.color = new Color(1.0f, 0.92f, 0.72f);
                    break;
                case 21:        // Standing candle
                    light.range /= 3f;
                    light.intensity = 0.5f;
                    light.color = new Color(1.0f, 0.95f, 0.67f);
                    break;
                case 22:         // Round lantern with medium chain
                    light.intensity = 1.5f;
                    light.color = new Color(1.0f, 0.95f, 0.78f);
                    break;
                case 23:         // Wooden chandelier with burning candles
                                 // todo
                    break;
                case 24:        // Lantern with long chain
                    light.intensity = 1.4f;
                    light.color = new Color(1.0f, 0.98f, 0.64f);
                    break;
                case 25:        // Lantern with medium chain
                    light.intensity = 1.4f;
                    light.color = new Color(1.0f, 0.98f, 0.64f);
                    break;
                case 26:        // Lantern with short chain
                    light.intensity = 1.4f;
                    light.color = new Color(1.0f, 0.98f, 0.64f);
                    break;
                case 27:        // Lantern with no chain
                    light.intensity = 1.4f;
                    light.color = new Color(1.0f, 0.98f, 0.64f);
                    break;
                case 28:        // Street Lantern 1
                                // todo
                    break;
                case 29:        // Street Lantern 2
                                // todo
                    break;
            }

            // TODO: Could also adjust light colour and intensity, or change prefab entirely above for any obj.TextureRecord
        }

    }

}