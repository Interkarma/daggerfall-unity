// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using System.IO;
using UnityEngine;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using Exception = System.Exception;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
#if UNITY_EDITOR
    public static class RmbBlockHelper
    {
        private const byte InteractiveObject = 3;

        public static DFBlock LoadRmbBlockFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Error: Trying to open file, but path is null or empty");
            }

            if (!File.Exists(path))
            {
                throw new ArgumentException("Error: Trying to open file at invalid path: " + path);
            }

            var blockReplacementJson = File.ReadAllText(path);
            return (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), blockReplacementJson);
        }

        public static DFBlock.RmbBlockDesc LoadObjectGroupFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Error: Trying to open file, but path is null or empty");
            }

            if (!File.Exists(path))
            {
                throw new ArgumentException("Error: Trying to open file at invalid path: " + path);
            }

            var blockReplacementJson = File.ReadAllText(path);
            return (DFBlock.RmbBlockDesc)SaveLoadManager.Deserialize(typeof(DFBlock.RmbBlockDesc),
                blockReplacementJson);
        }

        public static BuildingReplacementData LoadBuildingFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Error: Trying to open file, but path is null or empty");
            }

            if (!File.Exists(path))
            {
                throw new ArgumentException("Error: Trying to open file at invalid path: " + path);
            }

            var buildingJson = File.ReadAllText(path);
            return (BuildingReplacementData)SaveLoadManager.Deserialize(typeof(BuildingReplacementData),
                buildingJson);
        }

        public static void AddGroundPlane(DFBlock.RmbGroundTiles[,] groundTiles, ref DaggerfallGroundPlane dfGround,
            ref MeshFilter meshFilter)
        {
            var climate = PersistedSettings.ClimateBases();
            var season = PersistedSettings.ClimateSeason();

            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            DFBlock blockData = new DFBlock();
            blockData.RmbBlock.FldHeader.GroundData.GroundTiles = groundTiles;

            // Assign climate and mesh
            Color32[] tileMap;
            Mesh mesh = dfUnity.MeshReader.GetSimpleGroundPlaneMesh(
                ref blockData,
                out tileMap,
                dfUnity.MeshReader.AddMeshTangents,
                dfUnity.MeshReader.AddMeshLightmapUVs);
            if (mesh)
            {
                meshFilter.sharedMesh = mesh;
            }

            // Assign tileMap and climate
            dfGround.tileMap = tileMap;
            dfGround.SetClimate(dfUnity, climate, season);
        }

        public static int GetSceneryTextureArchive()
        {
            var climate = PersistedSettings.ClimateBases();
            var season = PersistedSettings.ClimateSeason();
            if (climate == ClimateBases.Desert)
            {
                return 503;
            }

            if (climate == ClimateBases.Swamp)
            {
                return 502;
            }

            if (climate == ClimateBases.Temperate && season != ClimateSeason.Winter)
            {
                return 504;
            }

            if (climate == ClimateBases.Temperate && season == ClimateSeason.Winter)
            {
                return 505;
            }

            if (climate == ClimateBases.Mountain && season != ClimateSeason.Winter)
            {
                return 510;
            }

            if (climate == ClimateBases.Mountain && season == ClimateSeason.Winter)
            {
                return 511;
            }

            return 504;
        }

        public static GameObject AddPersonObject(DFBlock.RmbBlockPeopleRecord rmbBlock)
        {
            var billboardPosition = new Vector3(rmbBlock.XPos, -rmbBlock.YPos, rmbBlock.ZPos) * MeshReader.GlobalScale;

            // Spawn billboard gameobject
            var go = DaggerfallWorkshop.Utility.GameObjectHelper.CreateDaggerfallBillboardGameObject(
                rmbBlock.TextureArchive, rmbBlock.TextureRecord, null);

            // Set position
            var dfBillboard = go.GetComponent<Billboard>();
            go.transform.localPosition = billboardPosition;
            go.transform.localPosition += new Vector3(0, dfBillboard.Summary.Size.y / 2, 0);

            // Add RMB data to billboard
            dfBillboard.SetRMBPeopleData(rmbBlock);

            return go;
        }

        public static GameObject Add3dObject(DFBlock.RmbBlock3dObjectRecord rmbBlock)
        {
            var climate = PersistedSettings.ClimateBases();
            var season = PersistedSettings.ClimateSeason();
            var windowStyle = PersistedSettings.WindowStyle();
            var dfUnity = DaggerfallUnity.Instance;
            // Get model data
            dfUnity.MeshReader.GetModelData(rmbBlock.ModelIdNum, out var modelData);
            // Get model position by type (3 seems to indicate props/clutter)
            // Also stop these from being combined as some may carry a loot container
            Vector3 modelPosition;
            if (rmbBlock.ObjectType == InteractiveObject)
            {
                // Props axis needs to be transformed to lowest Y point
                var bottom = modelData.Vertices[0];
                for (var i = 0; i < modelData.Vertices.Length; i++)
                {
                    if (modelData.Vertices[i].y < bottom.y)
                        bottom = modelData.Vertices[i];
                }

                modelPosition = new Vector3(rmbBlock.XPos, rmbBlock.YPos, rmbBlock.ZPos) * MeshReader.GlobalScale;
                modelPosition += new Vector3(0, -bottom.y, 0);
            }
            else
            {
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
            var modelRotation = new Vector3(-rmbBlock.XRotation / BlocksFile.RotationDivisor,
                -rmbBlock.YRotation / BlocksFile.RotationDivisor, -rmbBlock.ZRotation / BlocksFile.RotationDivisor);
            var modelScale = new Vector3(rmbBlock.XScale, rmbBlock.YScale, rmbBlock.ZScale);
            var modelMatrix = Matrix4x4.TRS(modelPosition, Quaternion.Euler(modelRotation), modelScale);

            // Inject custom GameObject from RmbModManager
            var modelGo = RMBModManager.GetCustomModel(rmbBlock.ModelId);
            if (modelGo != null)
            {
                var mesh = modelGo.GetComponent<DaggerfallMesh>();
                if (mesh != null)
                {
                    mesh.SetClimate(climate, season, windowStyle);
                }

                return modelGo;
            }

            // Inject custom GameObject from ModManager
            modelGo = MeshReplacement.ImportCustomGameobject(rmbBlock.ModelIdNum, null, modelMatrix);
            if (modelGo != null)
            {
                modelGo.GetComponent<DaggerfallMesh>().SetClimate(climate, season, windowStyle);
                return modelGo;
            }

            // Try creating a DaggerfallMesh
            if (modelData.DFMesh.TotalVertices != 0)
            {
                modelGo = GameObjectHelper.CreateDaggerfallMeshGameObject(rmbBlock.ModelIdNum, null);
                modelGo.transform.localPosition = modelMatrix.GetColumn(3);
                modelGo.transform.localRotation = modelMatrix.rotation;
                modelGo.transform.localScale = modelMatrix.lossyScale;
                modelGo.GetComponent<DaggerfallMesh>().SetClimate(climate, season, windowStyle);
                return modelGo;
            }

            // Return a magenta-colored cube if the mesh can't be found
            Material material = new Material(Shader.Find("Standard"));
            material.color = Color.magenta;

            modelGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            modelGo.name = $"Missing Mesh [ID={rmbBlock.ModelIdNum}]";
            modelGo.GetComponent<Renderer>().material = material;
            return modelGo;
        }

        public static GameObject Add3dObject(string modelId)
        {
            Vector3 position = new Vector3(0, 0, 0);

            return Add3dObject(modelId, position);
        }

        public static GameObject Add3dObject(string modelId, Vector3 position)
        {
            var climate = PersistedSettings.ClimateBases();
            var season = PersistedSettings.ClimateSeason();
            var windowStyle = PersistedSettings.WindowStyle();

            // Get matrix
            Matrix4x4 matrix = Matrix4x4.identity;
            matrix *= Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);

            uint modelIdNum = uint.Parse(modelId);

            // Inject custom GameObject from RmbModManager
            var modelGo = RMBModManager.GetCustomModel(modelId);
            if (modelGo != null)
            {
                var mesh = modelGo.GetComponent<DaggerfallMesh>();
                if (mesh != null)
                {
                    mesh.SetClimate(climate, season, windowStyle);
                }

                return modelGo;
            }

            // Inject custom GameObject if available
            modelGo = MeshReplacement.ImportCustomGameobject(modelIdNum, null, matrix);
            if (modelGo != null)
            {
                return modelGo;
            }

            // Get model data
            ModelData modelData;
            DaggerfallUnity.Instance.MeshReader.GetModelData(modelIdNum, out modelData);

            if (modelData.DFMesh.TotalVertices != 0)
            {
                GameObject modelGO = GameObjectHelper.CreateDaggerfallMeshGameObject(modelIdNum, null);
                modelGO.transform.position = matrix.GetColumn(3);
                modelGO.GetComponent<DaggerfallMesh>().SetClimate(climate, season, windowStyle);
                return modelGO;
            }

            throw new Exception("Vanilla model not found for preview, modelId " + modelId);
        }

        public static GameObject AddBuilding3dObjects(DFBlock.RmbBlock3dObjectRecord[] ExteriorBlock3dObjectRecords,
            int YPos)
        {
            var obj3D = new GameObject("3D Objects");
            var mainRecord = ExteriorBlock3dObjectRecords[0];
            var scaledYPos = YPos * MeshReader.GlobalScale;

            var main = Add3dObject(mainRecord);
            var mainLocalPos = main.transform.localPosition;
            main.transform.localPosition = new Vector3(mainLocalPos.x, 0, mainLocalPos.z);
            main.transform.parent = obj3D.transform;
            if (ExteriorBlock3dObjectRecords.Length == 1)
            {
                return obj3D;
            }

            for (var i = 1; i < ExteriorBlock3dObjectRecords.Length; i++)
            {
                var blockRecord = ExteriorBlock3dObjectRecords[i];
                var relativeY = blockRecord.YPos * MeshReader.GlobalScale;
                var go = Add3dObject(blockRecord);
                var localPos = go.transform.localPosition;
                go.transform.localPosition = new Vector3(localPos.x, scaledYPos - relativeY, localPos.z);
                go.transform.parent = obj3D.transform;
            }

            return obj3D;
        }

        public static GameObject AddFlatObject(DFBlock.RmbBlockFlatObjectRecord rmbBlock, Quaternion subRecordRotation)
        {
            var id = $"{rmbBlock.TextureArchive}.{rmbBlock.TextureRecord}";
            var billboardPosition = new Vector3(rmbBlock.XPos, -rmbBlock.YPos, rmbBlock.ZPos) * MeshReader.GlobalScale;
            billboardPosition = subRecordRotation * billboardPosition;

            // try to get a custom flat
            var go = RMBModManager.GetCustomBillboard(id);
            if (go != null)
            {
                return go;
            }

            try
            {
                // Spawn billboard gameobject
                go = GameObjectHelper.CreateDaggerfallBillboardGameObject(
                    rmbBlock.TextureArchive, rmbBlock.TextureRecord, null);

                // Set position
                var dfBillboard = go.GetComponent<Billboard>();
                go.transform.position = billboardPosition;
                go.transform.position += new Vector3(0, dfBillboard.Summary.Size.y / 2, 0);

                return go;
            }
            catch (Exception error)
            {
                // Return a magenta-colored flat if the id can't be found
                Texture2D texture = new Texture2D(1, 1);
                texture.SetPixel(0, 0, Color.magenta);
                texture.Apply();

                go = new GameObject($"Missing Flat [ID={id}]");
                var billboard = go.AddComponent<DaggerfallBillboard>();
                billboard.SetMaterial(texture, new Vector2(1, 2));
                return go;
            }
        }

        public static GameObject AddFlatObject(string flatId)
        {
            var dot = Char.Parse(".");
            var splitId = flatId.Split(dot);
            GameObject go;
            // try to get a custom flat
            go = RMBModManager.GetCustomBillboard(flatId);
            if (go != null)
            {
                return go;
            }

            go = GameObjectHelper.CreateDaggerfallBillboardGameObject(
                int.Parse(splitId[0]), int.Parse(splitId[1]), null);
            return go;
        }

        public static void SaveBlockFile(DFBlock blockData, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var writer = new StreamWriter(path, false);
            var saveData = SaveLoadManager.Serialize(blockData.GetType(), blockData);
            writer.Write(saveData);
            writer.Close();
        }

        public static void SaveObjectGroupFile(DFBlock.RmbBlockDesc data, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var writer = new StreamWriter(path, false);
            var saveData = SaveLoadManager.Serialize(data.GetType(), data);
            writer.Write(saveData);
            writer.Close();
        }

        public static void SaveBuildingFile(BuildingReplacementData buildingData, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            StreamWriter writer = new StreamWriter(path, false);
            string saveData = SaveLoadManager.Serialize(buildingData.GetType(), buildingData);
            writer.Write(saveData);
            writer.Close();
        }

        public static DFBlock.RmbBlockData CloneBlockData(DFBlock.RmbBlockData blockData)
        {
            var cloned = new DFBlock.RmbBlockData();
            cloned = new DFBlock.RmbBlockData();
            cloned.Header = new DFBlock.RmbBlockHeader();
            cloned.Header.Num3dObjectRecords = blockData.Header.Num3dObjectRecords;
            cloned.Header.NumFlatObjectRecords = blockData.Header.NumFlatObjectRecords;
            cloned.Header.NumSection3Records = blockData.Header.NumSection3Records;
            cloned.Header.NumPeopleRecords = blockData.Header.NumPeopleRecords;
            cloned.Header.NumDoorRecords = blockData.Header.NumDoorRecords;
            cloned.Block3dObjectRecords = new DFBlock.RmbBlock3dObjectRecord[blockData.Block3dObjectRecords.Length];
            cloned.BlockFlatObjectRecords =
                new DFBlock.RmbBlockFlatObjectRecord[blockData.BlockFlatObjectRecords.Length];
            cloned.BlockSection3Records = new DFBlock.RmbBlockSection3Record[blockData.BlockSection3Records.Length];
            cloned.BlockPeopleRecords = new DFBlock.RmbBlockPeopleRecord[blockData.BlockPeopleRecords.Length];
            cloned.BlockDoorRecords = new DFBlock.RmbBlockDoorRecord[blockData.BlockDoorRecords.Length];

            for (int i = 0; i < blockData.Block3dObjectRecords.Length; i++)
            {
                var obj = new DFBlock.RmbBlock3dObjectRecord();
                obj.ModelId = blockData.Block3dObjectRecords[i].ModelId;
                obj.ModelIdNum = blockData.Block3dObjectRecords[i].ModelIdNum;
                obj.ObjectId1 = blockData.Block3dObjectRecords[i].ObjectId1;
                obj.ObjectId2 = blockData.Block3dObjectRecords[i].ObjectId2;
                obj.ObjectType = blockData.Block3dObjectRecords[i].ObjectType;
                obj.NullValue1 = blockData.Block3dObjectRecords[i].NullValue1;
                obj.XPos1 = blockData.Block3dObjectRecords[i].XPos1;
                obj.YPos1 = blockData.Block3dObjectRecords[i].YPos1;
                obj.ZPos1 = blockData.Block3dObjectRecords[i].ZPos1;
                obj.XPos = blockData.Block3dObjectRecords[i].XPos;
                obj.YPos = blockData.Block3dObjectRecords[i].YPos;
                obj.ZPos = blockData.Block3dObjectRecords[i].ZPos;
                obj.XScale = blockData.Block3dObjectRecords[i].XScale;
                obj.YScale = blockData.Block3dObjectRecords[i].YScale;
                obj.ZScale = blockData.Block3dObjectRecords[i].ZScale;
                obj.NullValue2 = blockData.Block3dObjectRecords[i].NullValue2;
                obj.XRotation = blockData.Block3dObjectRecords[i].XRotation;
                obj.YRotation = blockData.Block3dObjectRecords[i].YRotation;
                obj.ZRotation = blockData.Block3dObjectRecords[i].ZRotation;
                obj.NullValue3 = blockData.Block3dObjectRecords[i].NullValue3;
                obj.NullValue4 = blockData.Block3dObjectRecords[i].NullValue4;
                obj.Unknown1 = blockData.Block3dObjectRecords[i].Unknown1;
                obj.Unknown2 = blockData.Block3dObjectRecords[i].Unknown2;
                obj.Unknown3 = blockData.Block3dObjectRecords[i].Unknown3;
                obj.Unknown4 = blockData.Block3dObjectRecords[i].Unknown4;
                obj.Unknown5 = blockData.Block3dObjectRecords[i].Unknown5;
                cloned.Block3dObjectRecords[i] = obj;
            }

            for (int i = 0; i < blockData.BlockFlatObjectRecords.Length; i++)
            {
                var obj = new DFBlock.RmbBlockFlatObjectRecord();
                obj.Position = blockData.BlockFlatObjectRecords[i].Position;
                obj.XPos = blockData.BlockFlatObjectRecords[i].XPos;
                obj.YPos = blockData.BlockFlatObjectRecords[i].YPos;
                obj.ZPos = blockData.BlockFlatObjectRecords[i].ZPos;
                obj.TextureBitfield = blockData.BlockFlatObjectRecords[i].TextureBitfield;
                obj.TextureArchive = blockData.BlockFlatObjectRecords[i].TextureArchive;
                obj.TextureRecord = blockData.BlockFlatObjectRecords[i].TextureRecord;
                obj.FactionID = blockData.BlockFlatObjectRecords[i].FactionID;
                obj.Flags = blockData.BlockFlatObjectRecords[i].Flags;

                cloned.BlockFlatObjectRecords[i] = obj;
            }

            for (int i = 0; i < blockData.BlockSection3Records.Length; i++)
            {
                var obj = new DFBlock.RmbBlockSection3Record();
                obj.XPos = blockData.BlockSection3Records[i].XPos;
                obj.YPos = blockData.BlockSection3Records[i].YPos;
                obj.ZPos = blockData.BlockSection3Records[i].ZPos;
                obj.Unknown1 = blockData.BlockSection3Records[i].Unknown1;
                obj.Unknown2 = blockData.BlockSection3Records[i].Unknown2;
                obj.Unknown3 = blockData.BlockSection3Records[i].Unknown3;

                cloned.BlockSection3Records[i] = obj;
            }

            for (int i = 0; i < blockData.BlockPeopleRecords.Length; i++)
            {
                var obj = new DFBlock.RmbBlockPeopleRecord();
                obj.Position = blockData.BlockPeopleRecords[i].Position;
                obj.XPos = blockData.BlockPeopleRecords[i].XPos;
                obj.YPos = blockData.BlockPeopleRecords[i].YPos;
                obj.ZPos = blockData.BlockPeopleRecords[i].ZPos;
                obj.TextureBitfield = blockData.BlockPeopleRecords[i].TextureBitfield;
                obj.TextureArchive = blockData.BlockPeopleRecords[i].TextureArchive;
                obj.TextureRecord = blockData.BlockPeopleRecords[i].TextureRecord;
                obj.FactionID = blockData.BlockPeopleRecords[i].FactionID;
                obj.Flags = blockData.BlockPeopleRecords[i].Flags;

                cloned.BlockPeopleRecords[i] = obj;
            }

            for (int i = 0; i < blockData.BlockDoorRecords.Length; i++)
            {
                var obj = new DFBlock.RmbBlockDoorRecord();
                obj.Position = blockData.BlockDoorRecords[i].Position;
                obj.XPos = blockData.BlockDoorRecords[i].XPos;
                obj.YPos = blockData.BlockDoorRecords[i].YPos;
                obj.ZPos = blockData.BlockDoorRecords[i].ZPos;
                obj.YRotation = blockData.BlockDoorRecords[i].YRotation;
                obj.OpenRotation = blockData.BlockDoorRecords[i].OpenRotation;
                obj.DoorModelIndex = blockData.BlockDoorRecords[i].DoorModelIndex;
                obj.Unknown = blockData.BlockDoorRecords[i].Unknown;
                obj.NullValue1 = blockData.BlockDoorRecords[i].NullValue1;

                cloned.BlockDoorRecords[i] = obj;
            }

            return cloned;
        }

        public static DFBlock.RmbSubRecord CloneRmbSubRecord(DFBlock.RmbSubRecord record)
        {
            var cloned = new DFBlock.RmbSubRecord();
            cloned.XPos = record.XPos;
            cloned.ZPos = record.ZPos;
            cloned.YRotation = record.YRotation;
            cloned.Exterior = CloneBlockData(record.Exterior);
            cloned.Interior = CloneBlockData(record.Interior);

            return cloned;
        }
    }
#endif
}