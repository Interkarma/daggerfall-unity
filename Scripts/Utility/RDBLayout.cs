// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Helper for laying out RDB (dungeon block) data in scene.
    /// </summary>
    public class RDBLayout
    {
        public static float RDBSide = 2048f * MeshReader.GlobalScale;

        int[] textureTable = null;
        DFRegion.DungeonTypes dungeonType = DFRegion.DungeonTypes.HumanStronghold;
        ModelCombiner combiner = new ModelCombiner();
        DaggerfallUnity dfUnity;
        DFBlock blockData;
        List<GameObject> startMarkers = new List<GameObject>();
        bool isStartingBlock;

        int groupIndex = 0;                     // Keeps count of RDB group index during build to reference action records

        GameObject staticModelsNode;
        GameObject actionModelsNode;
        GameObject doorsNode;
        GameObject flatsNode;
        GameObject lightsNode;
        GameObject enemiesNode;

        private Dictionary<int, ActionLink> actionLinkDict = new Dictionary<int, ActionLink>();
        private struct ActionLink
        {
            public GameObject gameObject;
            public int nextKey;
            public int prevKey;
        }

        public RDBLayout(string blockName)
        {
            this.dfUnity = DaggerfallUnity.Instance;
            blockData = dfUnity.ContentReader.BlockFileReader.GetBlock(blockName);
            if (blockData.Type != DFBlock.BlockTypes.Rdb)
                throw new Exception(string.Format("Could not load RDB block {0}", blockName), null);
        }

        /// <summary>
        /// Creates a new RDB GameObject and performs block layout.
        /// Can pass information about dungeon for texture swaps and random enemies.
        /// </summary>
        /// <param name="dfUnity">DaggerfallUnity singleton. Required for content readers and settings.</param>
        /// <param name="blockName">Name of RDB block to build.</param>
        /// <param name="isStartingBlock">True if this is the starting block. Controls exit doors.</param>
        /// <param name="textureTable">Dungeon texture table.</param>
        /// <param name="dungeonType">Type of dungeon for random encounter tables.</param>
        /// <param name="seed">Seed for random encounters.</param>
        /// <returns>GameObject.</returns>
        public static GameObject CreateGameObject(
            string blockName,
            bool isStartingBlock,
            int[] textureTable = null,
            DFRegion.DungeonTypes dungeonType = DFRegion.DungeonTypes.HumanStronghold,
            int seed = 0)
        {
            // Validate
            if (string.IsNullOrEmpty(blockName))
                return null;
            if (!blockName.ToUpper().EndsWith(".RDB"))
                return null;
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return null;

            // Use default texture table if one not specified
            if (textureTable == null)
                textureTable = StaticTextureTables.DefaultTextureTable;

            // Create gameobject
            GameObject go = new GameObject(string.Format("DaggerfallBlock [Name={0}]", blockName));
            DaggerfallRDBBlock dfBlock = go.AddComponent<DaggerfallRDBBlock>();

            // Start new layout
            RDBLayout layout = new RDBLayout(blockName);
            layout.isStartingBlock = isStartingBlock;
            layout.textureTable = textureTable;
            layout.dungeonType = dungeonType;
            layout.staticModelsNode = new GameObject("Static Models");
            layout.actionModelsNode = new GameObject("Action Models");
            layout.doorsNode = new GameObject("Doors");
            layout.flatsNode = new GameObject("Flats");
            layout.lightsNode = new GameObject("Lights");
            layout.enemiesNode = new GameObject("Enemies");

            // Parent child game objects
            layout.staticModelsNode.transform.parent = go.transform;
            layout.actionModelsNode.transform.parent = go.transform;
            layout.doorsNode.transform.parent = go.transform;
            layout.flatsNode.transform.parent = go.transform;
            layout.lightsNode.transform.parent = go.transform;
            layout.enemiesNode.transform.parent = go.transform;

            // List to receive any exit doors found
            List<StaticDoor> allDoors = new List<StaticDoor>();

            // Seed random generator
            UnityEngine.Random.seed = seed;
            
            // Iterate object groups
            layout.groupIndex = 0;
            DFBlock blockData = dfUnity.ContentReader.BlockFileReader.GetBlock(blockName);
            foreach (DFBlock.RdbObjectRoot group in blockData.RdbBlock.ObjectRootList)
            {
                // Skip empty object groups
                if (null == group.RdbObjects)
                {
                    layout.groupIndex++;
                    continue;
                }

                // Iterate objects in this group
                List<StaticDoor> modelDoors;
                foreach (DFBlock.RdbObject obj in group.RdbObjects)
                {
                    // Handle by object type
                    switch (obj.Type)
                    {
                        case DFBlock.RdbResourceTypes.Model:
                            layout.AddRDBModel(obj, out modelDoors, layout.staticModelsNode.transform);
                            if (modelDoors.Count > 0) allDoors.AddRange(modelDoors);
                            break;
                        case DFBlock.RdbResourceTypes.Flat:
                            layout.AddRDBFlat(obj, layout.flatsNode.transform);
                            break;
                        case DFBlock.RdbResourceTypes.Light:
                            layout.AddRDBLight(obj, layout.lightsNode.transform);
                            break;
                        default:
                            break;
                    }
                }

                // Increment group index
                layout.groupIndex++;
            }

            // Link action nodes
            layout.LinkActionNodes();

            // Combine meshes
            if (dfUnity.Option_CombineRDB)
            {
                layout.combiner.Apply();
                GameObject cgo = GameObjectHelper.CreateCombinedMeshGameObject(layout.combiner, "CombinedMeshes", layout.staticModelsNode.transform, dfUnity.Option_SetStaticFlags);
                cgo.GetComponent<DaggerfallMesh>().SetDungeonTextures(textureTable);
            }

            // Fix enemy standing positions for this block
            // Some enemies are floating in air or sunk into ground
            // Can only adjust this after geometry instantiated
            layout.FixEnemyStanding(go);

            // Store start markers in block
            dfBlock.SetStartMarkers(layout.startMarkers.ToArray());

            // Add doors
            if (allDoors.Count > 0)
                layout.AddDoors(allDoors.ToArray(), go);

            return go;
        }

        #region Static Methods

        private static bool IsActionDoor(DFBlock blockData, DFBlock.RdbObject obj, int modelReference)
        {
            const uint redBrickDoor = 72100;

            // Always reject red brick doors, they are not action doors despite having "DOR" attached
            if (blockData.RdbBlock.ModelReferenceList[modelReference].ModelIdNum == redBrickDoor)
                return false;

            // Otherwise Check if this is a door (DOR) or double-door (DDR)
            string description = blockData.RdbBlock.ModelReferenceList[modelReference].Description;
            if (description == "DOR" || description == "DDR")
                return true;

            return false;
        }

        private static bool HasAction(DFBlock blockData, DFBlock.RdbObject obj, int modelReference)
        {
            // Allow for known action types
            DFBlock.RdbActionResource action = obj.Resources.ModelResource.ActionResource;
            if (action.Flags != 0)
                return true;

            return false;
        }

        /// <summary>
        /// Creates action key unique within group.
        /// </summary>
        /// <param name="groupIndex">RDB group index.</param>
        /// <param name="objIndex">RDB object index.</param>
        /// <returns></returns>
        private static int GetActionKey(int groupIndex, int objIndex)
        {
            // Create action key for this object
            return groupIndex * 1000 + objIndex;
        }

        /// <summary>
        /// Constructs a Vector3 from magnitude and direction in RDB action resource.
        /// </summary>
        /// <param name="resource">DFBlock.RdbActionResource</param>
        /// <returns>Vector3.</returns>
        private static Vector3 GetActionVector(ref DFBlock.RdbActionResource resource)
        {
            Vector3 vector = Vector3.zero;
            float magnitude = resource.Magnitude;
            switch (resource.Axis)
            {
                case DFBlock.RdbActionAxes.NegativeX:
                    vector.x = -magnitude;
                    break;
                case DFBlock.RdbActionAxes.NegativeY:
                    vector.y = -magnitude;
                    break;
                case DFBlock.RdbActionAxes.NegativeZ:
                    vector.z = -magnitude;
                    break;

                case DFBlock.RdbActionAxes.PositiveX:
                    vector.x = magnitude;
                    break;
                case DFBlock.RdbActionAxes.PositiveY:
                    vector.y = magnitude;
                    break;
                case DFBlock.RdbActionAxes.PositiveZ:
                    vector.z = magnitude;
                    break;

                default:
                    magnitude = 0f;
                    break;
            }

            return vector;
        }

        #endregion

        #region Private Methods

        private void AddRDBModel(DFBlock.RdbObject obj, out List<StaticDoor> doorsOut, Transform parent)
        {
            bool overrideCombine = false;
            doorsOut = new List<StaticDoor>();

            // Get model reference index and id
            int modelReference = obj.Resources.ModelResource.ModelIndex;
            uint modelId = blockData.RdbBlock.ModelReferenceList[modelReference].ModelIdNum;

            // Get rotation angle for each axis
            float degreesX = -obj.Resources.ModelResource.XRotation / BlocksFile.RotationDivisor;
            float degreesY = -obj.Resources.ModelResource.YRotation / BlocksFile.RotationDivisor;
            float degreesZ = -obj.Resources.ModelResource.ZRotation / BlocksFile.RotationDivisor;

            // Calcuate transform
            Vector3 position = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;

            // Calculate matrix
            Vector3 rx = new Vector3(degreesX, 0, 0);
            Vector3 ry = new Vector3(0, degreesY, 0);
            Vector3 rz = new Vector3(0, 0, degreesZ);
            Matrix4x4 modelMatrix = Matrix4x4.identity;
            modelMatrix *= Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
            modelMatrix *= Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(rz), Vector3.one);
            modelMatrix *= Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(rx), Vector3.one);
            modelMatrix *= Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(ry), Vector3.one);

            // Get model data
            ModelData modelData;
            dfUnity.MeshReader.GetModelData(modelId, out modelData);

            // Discard static exit doors not in starting block
            // Exit doors are just a model slapped over wall or doorway
            // This allows Daggerfall to toggle exits on and off
            if (modelData.DFMesh.ObjectId == 70300)
            {
                if (!isStartingBlock)
                    return;
            }

            // Doors - there are no working static building doors inside dungeons, just the exits and action doors
            bool isActionDoor = IsActionDoor(blockData, obj, modelReference);
            if (isActionDoor)
                parent = doorsNode.transform;
            else if (modelData.Doors != null)
                doorsOut.AddRange(GameObjectHelper.GetStaticDoors(ref modelData, blockData.Index, 0, modelMatrix));

            // Action records
            bool hasAction = HasAction(blockData, obj, modelReference);
            if (hasAction)
                parent = actionModelsNode.transform;

            // Flags
            bool isStatic = dfUnity.Option_SetStaticFlags;
            if (isActionDoor || hasAction)
            {
                // Moving objects are never static or combined
                isStatic = false;
                overrideCombine = true;
            }

            // Add to scene
            if (dfUnity.Option_CombineRDB && !overrideCombine)
            {
                combiner.Add(ref modelData, modelMatrix);
            }
            else
            {
                // Spawn mesh gameobject
                GameObject go = GameObjectHelper.CreateDaggerfallMeshGameObject(modelId, parent, isStatic);
                go.GetComponent<DaggerfallMesh>().SetDungeonTextures(textureTable);

                // Apply transforms
                go.transform.Rotate(0, degreesY, 0, Space.World);
                go.transform.Rotate(degreesX, 0, 0, Space.World);
                go.transform.Rotate(0, 0, degreesZ, Space.World);
                go.transform.localPosition = position;

                // Add action door
                if (isActionDoor)
                { 
                    DaggerfallActionDoor dfActionDoor = go.AddComponent<DaggerfallActionDoor>();

                    // Add action door audio
                    if (dfUnity.Option_DefaultSounds)
                    {
                        AddActionDoorAudioSource(go);
                        dfActionDoor.SetDungeonDoorSounds();
                    }
                }

                // Add action component
                if (hasAction && !isActionDoor)
                    AddAction(go, blockData, obj, modelReference);
            }
        }

        private void AddRDBFlat(DFBlock.RdbObject obj, Transform parent)
        {
            int archive = obj.Resources.FlatResource.TextureArchive;
            int record = obj.Resources.FlatResource.TextureRecord;

            // Spawn billboard gameobject
            GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(archive, record, parent, true);
            Vector3 billboardPosition = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;

            // Add RDB data to billboard
            DaggerfallBillboard dfBillboard = go.GetComponent<DaggerfallBillboard>();
            dfBillboard.SetResourceData(obj.Resources.FlatResource);

            // Set transform
            go.transform.position = billboardPosition;

            // Handle supported editor flats
            if (archive == 199)
            {
                switch (record)
                {
                    case 10:                        // Start marker
                        startMarkers.Add(go);
                        break;
                    case 15:                        // Random enemy
                        if (dfUnity.Option_ImportEnemies)
                        {
                            AddRandomRDBEnemy(obj);
                            go.SetActive(false);
                        }
                        break;
                    case 16:                        // Fixed enemy
                        if (dfUnity.Option_ImportEnemies)
                        {
                            AddFixedRDBEnemy(obj);
                            go.SetActive(false);
                        }
                        break;
                }
            }

            // Add torch burning sound
            if (dfUnity.Option_DefaultSounds && archive == 210)
            {
                switch (record)
                {
                    case 0:
                    case 1:
                    case 6:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                        AddTorchAudioSource(go);
                        break;
                }
            }
        }

        private void AddRDBLight(DFBlock.RdbObject obj, Transform parent)
        {
            // Do nothing if import option not enabled
            if (!dfUnity.Option_ImportPointLights)
                return;

            // Spawn light gameobject
            float radius = obj.Resources.LightResource.Radius * MeshReader.GlobalScale;
            GameObject go = GameObjectHelper.CreateDaggerfallRDBPointLight(radius, parent);
            Vector3 lightPosition = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;

            // Add component
            DaggerfallLight c = go.AddComponent<DaggerfallLight>();
            if (dfUnity.Option_AnimatedPointLights)
                c.Animate = true;

            // Set transform
            go.transform.position = lightPosition;
        }

        private void AddFixedRDBEnemy(DFBlock.RdbObject obj)
        {
            // Get type value and ignore known invalid types
            int typeValue = (int)(obj.Resources.FlatResource.FactionMobileId & 0xff);
            if (typeValue == 99)
                return;

            // Cast to enum
            MobileTypes type = (MobileTypes)(obj.Resources.FlatResource.FactionMobileId & 0xff);

            AddEnemy(obj, type);
        }

        private void AddRandomRDBEnemy(DFBlock.RdbObject obj)
        {
            // Get dungeon type index
            int index = (int)dungeonType >> 8;
            if (index < RandomEncounters.EncounterTables.Length)
            {
                // Get encounter table
                RandomEncounterTable table = RandomEncounters.EncounterTables[index];

                // Get random monster from table
                // Normally this would be weighted by player level
                MobileTypes type = table.Enemies[UnityEngine.Random.Range(0, table.Enemies.Length)];

                // Add enemy
                AddEnemy(obj, type);
            }
            else
            {
                DaggerfallUnity.LogMessage(string.Format("RDBLayout: Dungeon type {0} is out of range or unknown.", dungeonType), true);
            }
        }

        private void AddEnemy(DFBlock.RdbObject obj, MobileTypes type)
        {
            // Get default reaction
            MobileReactions reaction = MobileReactions.Hostile;
            if (obj.Resources.FlatResource.FlatData.Reaction == (int)DFBlock.EnemyReactionTypes.Passive)
                reaction = MobileReactions.Passive;

            // Spawn enemy gameobject
            GameObject go = GameObjectHelper.CreateDaggerfallEnemyGameObject(type, enemiesNode.transform, reaction);
            if (go == null)
                return;

            // Set transform
            Vector3 enemyPosition = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;
            go.transform.position = enemyPosition;
        }

        private void AddAction(GameObject go, DFBlock blockData, DFBlock.RdbObject obj, int modelReference)
        {
            // Get model action record and description
            DFBlock.RdbActionResource action = obj.Resources.ModelResource.ActionResource;
            string description = blockData.RdbBlock.ModelReferenceList[modelReference].Description;

            // Check for known action types
            Vector3 actionRotation = Vector3.zero;
            Vector3 actionTranslation = Vector3.zero;
            if ((action.Flags & (int)DFBlock.RdbActionFlags.Rotation) == (int)DFBlock.RdbActionFlags.Rotation)
                actionRotation = (GetActionVector(ref action) / BlocksFile.RotationDivisor);
            if ((action.Flags & (int)DFBlock.RdbActionFlags.Translation) == (int)DFBlock.RdbActionFlags.Translation)
                actionTranslation = GetActionVector(ref action) * MeshReader.GlobalScale;

            // A quick hack to fix special-case rotation issues.
            // Currently unknown if there is data indicating different rotation behaviour or if something else is happening.
            switch (description)
            {
                case "LID":
                    actionRotation = new Vector3(0, 0, -90f);       // Coffin lids in Scourg barrow
                    break;
                case "WHE":
                    actionRotation = new Vector3(0, -360f, 0);      // Wheels in Direnni Tower
                    break;
            }

            // Create action component
            DaggerfallAction c = go.AddComponent<DaggerfallAction>();
            c.ActionEnabled = true;
            c.ModelDescription = description;
            c.ActionRotation = actionRotation;
            c.ActionTranslation = actionTranslation;
            c.ActionSoundID = obj.Resources.ModelResource.SoundId;

            // Set duration in seconds
            // Not sure what timescale native value represents
            // Using 1/20 of native value in seconds
            c.ActionDuration = (float)action.Duration / 20f;
            c.ActionFlags = action.Flags;

            // Create action links
            ActionLink link;
            link.gameObject = go;
            link.nextKey = GetActionKey(groupIndex, action.NextObjectIndex);
            link.prevKey = GetActionKey(groupIndex, action.PreviousObjectIndex);
            actionLinkDict.Add(GetActionKey(groupIndex, obj.Index), link);

            // Add sound
            AddActionAudioSource(go, (uint)c.ActionSoundID);

            return;
        }

        /// <summary>
        /// Links action chains together.
        /// </summary>
        private void LinkActionNodes()
        {
            // Exit if no actions
            if (actionLinkDict.Count == 0)
                return;

            // Iterate through actions
            foreach (var item in actionLinkDict)
            {
                ActionLink link = item.Value;

                // Link to next node
                if (actionLinkDict.ContainsKey(link.nextKey))
                    link.gameObject.GetComponent<DaggerfallAction>().NextObject = actionLinkDict[link.nextKey].gameObject;

                // Link to previous node
                if (actionLinkDict.ContainsKey(link.prevKey))
                    link.gameObject.GetComponent<DaggerfallAction>().PreviousObject = actionLinkDict[link.prevKey].gameObject;
            }
        }

        private void FixEnemyStanding(GameObject go)
        {
            Component[] mobiles = go.GetComponentsInChildren(typeof(DaggerfallMobileUnit));
            if (mobiles == null)
                return;

            foreach (DaggerfallMobileUnit enemy in mobiles)
            {
                // Don't change for flying enemies
                if (enemy.Summary.Enemy.Behaviour == MobileBehaviour.Flying)
                    continue;

                // Align to ground
                Vector2 size = enemy.Summary.RecordSizes[0];
                GameObjectHelper.AlignBillboardToGround(enemy.transform.parent.gameObject, size);
            }
        }

        private void AddTorchAudioSource(GameObject go)
        {
            // Apply looping burning sound to flaming torches and fires
            // Set to linear rolloff or the burning sound is audible almost everywhere
            DaggerfallAudioSource c = go.AddComponent<DaggerfallAudioSource>();
            c.AudioSource.dopplerLevel = 0;
            c.AudioSource.rolloffMode = AudioRolloffMode.Linear;
            c.AudioSource.maxDistance = 4f;
            c.AudioSource.volume = 0.6f;
            c.SetSound(SoundClips.Burning, AudioPresets.LoopIfPlayerNear);
        }

        private void AddActionAudioSource(GameObject go, uint id)
        {
            if (id > 0)
            {
                DaggerfallAudioSource c = go.AddComponent<DaggerfallAudioSource>();
                c.SetSound(id);
            }
        }

        private void AddActionDoorAudioSource(GameObject go)
        {
            DaggerfallAudioSource c = go.AddComponent<DaggerfallAudioSource>();
            c.Preset = AudioPresets.OnDemand;
        }

        private void AddDoors(StaticDoor[] doors, GameObject target)
        {
            if (doors != null && target != null)
            {
                DaggerfallStaticDoors c = target.AddComponent<DaggerfallStaticDoors>();
                c.Doors = doors;
            }
        }

        #endregion
    }
}