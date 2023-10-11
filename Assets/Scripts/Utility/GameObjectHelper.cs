// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Lypyl (lypyl@dfworkshop.net)
// 
// Notes:
//

#define KEEP_PREFAB_LINKS

using UnityEngine;
using System;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Static helper methods to instantiate common types of Daggerfall gameobjects.
    /// </summary>
    public static class GameObjectHelper
    {
        static Dictionary<int, MobileEnemy> enemyDict;
        public static Dictionary<int, MobileEnemy> EnemyDict
        {
            get
            {
                if (enemyDict == null)
                    enemyDict = EnemyBasics.BuildEnemyDict();
                return enemyDict;
            }
        }

        // Animal sounds range. Matched to classic.
        const float animalSoundMaxDistance = 768 * MeshReader.GlobalScale;

        public static void AddAnimalAudioSource(GameObject go, int record)
        {
            DaggerfallAudioSource source = go.AddComponent<DaggerfallAudioSource>();
            source.AudioSource.maxDistance = animalSoundMaxDistance;

            SoundClips sound;
            switch (record)
            {
                case 0:
                case 1:
                    sound = SoundClips.AnimalHorse;
                    break;
                case 3:
                case 4:
                    sound = SoundClips.AnimalCow;
                    break;
                case 5:
                case 6:
                    sound = SoundClips.AnimalPig;
                    break;
                case 7:
                case 8:
                    sound = SoundClips.AnimalCat;
                    break;
                case 9:
                case 10:
                    sound = SoundClips.AnimalDog;
                    break;
                default:
                    sound = SoundClips.None;
                    break;
            }

            source.SetSound(sound, AudioPresets.PlayRandomlyIfPlayerNear);
        }

        public static void AssignAnimatedMaterialComponent(CachedMaterial[] cachedMaterials, GameObject go)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;

            // Look for any animated textures in this material set
            for (int i = 0; i < cachedMaterials.Length; i++)
            {
                CachedMaterial cm = cachedMaterials[i];
                int frameCount = cm.singleFrameCount;
                if (frameCount > 1)
                {
                    // Add texture animation component
                    AnimatedMaterial c = go.AddComponent<AnimatedMaterial>();

                    // Store material for each frame
                    CachedMaterial[] materials = new CachedMaterial[frameCount];
                    for (int frame = 0; frame < frameCount; frame++)
                    {
                        int archiveOut, recordOut, frameOut;
                        MaterialReader.ReverseTextureKey(cm.key, out archiveOut, out recordOut, out frameOut, cm.keyGroup);
                        dfUnity.MaterialReader.GetCachedMaterial(archiveOut, recordOut, frame, out materials[frame]);
                    }

                    // Assign animation properties
                    c.TargetMaterial = cm.material;
                    c.AnimationFrames = materials;
                    if (cm.framesPerSecond > 0)
                        c.FramesPerSecond = cm.framesPerSecond;
                }
            }
        }

        public static Material[] GetMaterialArray(CachedMaterial[] cachedMaterials)
        {
            // Extract a material array from cached material array
            Material[] materials = new Material[cachedMaterials.Length];
            for (int i = 0; i < cachedMaterials.Length; i++)
            {
                materials[i] = cachedMaterials[i].material;
            }

            return materials;
        }

        public static string GetGoModelName(uint modelID)
        {
            return string.Format("DaggerfallMesh [ID={0}]", modelID);
        }

        public static string GetGoFlatName(int textureArchive, int textureRecord)
        {
            return string.Format("DaggerfallBillboard [TEXTURE.{0:000}, Index={1}]", textureArchive, textureRecord);
        }

        /// <summary>
        /// Adds a single DaggerfallMesh game object to scene.
        /// </summary>
        /// <param name="modelID">ModelID of mesh to add.</param>
        /// <param name="parent">Optional parent of this object.</param>
        /// <param name="makeStatic">Flag to set object static flag.</param>
        /// <param name="useExistingObject">Add mesh to existing object rather than create new.</param>
        /// <param name="ignoreCollider">Force disable collider.</param>
        /// <param name="convexCollider">Make collider convex.</param>
        /// <returns>GameObject.</returns>
        public static GameObject CreateDaggerfallMeshGameObject(
            uint modelID,
            Transform parent,
            bool makeStatic = false,
            GameObject useExistingObject = null,
            bool ignoreCollider = false,
            bool convexCollider = false)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;

            // Create gameobject
            GameObject go = (useExistingObject != null) ? useExistingObject : new GameObject();
            if (parent != null)
                go.transform.parent = parent;
            go.name = GetGoModelName(modelID);

            // Add DaggerfallMesh component
            DaggerfallMesh dfMesh = go.GetComponent<DaggerfallMesh>();
            if (dfMesh == null)
                dfMesh = go.AddComponent<DaggerfallMesh>();

            // Get mesh filter and renderer components
            MeshFilter meshFilter = go.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();

            // Assign mesh and materials
            CachedMaterial[] cachedMaterials;
            int[] textureKeys;
            bool hasAnimations;
            Mesh mesh = dfUnity.MeshReader.GetMesh(
                dfUnity,
                modelID,
                out cachedMaterials,
                out textureKeys,
                out hasAnimations,
                dfUnity.MeshReader.AddMeshTangents,
                dfUnity.MeshReader.AddMeshLightmapUVs);

            // Assign animated materials component if required
            if (hasAnimations)
                AssignAnimatedMaterialComponent(cachedMaterials, go);

            // Assign mesh and materials
            if (mesh)
            {
                meshFilter.sharedMesh = mesh;
                meshRenderer.sharedMaterials = GetMaterialArray(cachedMaterials);
                dfMesh.SetDefaultTextures(textureKeys);
            }

            // Assign mesh to collider
            if (dfUnity.Option_AddMeshColliders && !ignoreCollider)
            {
                MeshCollider collider = go.GetComponent<MeshCollider>();
                if (collider == null) collider = go.AddComponent<MeshCollider>();
                collider.sharedMesh = mesh;

                // Enable convex collider if specified
                if (convexCollider)
                    collider.convex = true;
            }

            // Assign static
            if (makeStatic)
                TagStaticGeometry(go);

            return go;
        }

        // TEMP: Changes a Daggerfall mesh to another ID
        // This will eventually be integrated with a future self-assembling mesh prefab
        public static void ChangeDaggerfallMeshGameObject(DaggerfallMesh dfMesh, uint newModelID)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;

            // Get new mesh
            CachedMaterial[] cachedMaterials;
            int[] textureKeys;
            bool hasAnimations;
            Mesh mesh = dfUnity.MeshReader.GetMesh(
                dfUnity,
                newModelID,
                out cachedMaterials,
                out textureKeys,
                out hasAnimations,
                dfUnity.MeshReader.AddMeshTangents,
                dfUnity.MeshReader.AddMeshLightmapUVs);

            // Get mesh filter and renderer components
            MeshFilter meshFilter = dfMesh.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = dfMesh.GetComponent<MeshRenderer>();

            // Update mesh
            if (mesh && meshFilter && meshRenderer)
            {
                meshFilter.sharedMesh = mesh;
                meshRenderer.sharedMaterials = GetMaterialArray(cachedMaterials);
            }

            // Update collider
            MeshCollider collider = dfMesh.GetComponent<MeshCollider>();
            {
                collider.sharedMesh = mesh;
            }

            // Update name
            dfMesh.name = GetGoModelName(newModelID);
        }

        public static GameObject CreateCombinedMeshGameObject(
            ModelCombiner combiner,
            string meshName,
            Transform parent,
            bool makeStatic = false)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;

            // Create gameobject
            GameObject go = new GameObject(meshName);
            if (parent)
                go.transform.parent = parent;

            // Assign components
            DaggerfallMesh dfMesh = go.AddComponent<DaggerfallMesh>();
            MeshFilter meshFilter = go.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();

            // Assign mesh and materials
            CachedMaterial[] cachedMaterials;
            int[] textureKeys;
            bool hasAnimations;
            Mesh mesh = dfUnity.MeshReader.GetCombinedMesh(
                dfUnity,
                combiner,
                out cachedMaterials,
                out textureKeys,
                out hasAnimations,
                dfUnity.MeshReader.AddMeshTangents,
                dfUnity.MeshReader.AddMeshLightmapUVs);

            // Assign animated materials component if required
            if (hasAnimations)
                AssignAnimatedMaterialComponent(cachedMaterials, go);

            // Assign mesh and materials array
            if (mesh)
            {
                meshFilter.sharedMesh = mesh;
                meshRenderer.sharedMaterials = GetMaterialArray(cachedMaterials);
                dfMesh.SetDefaultTextures(textureKeys);
            }

            // Assign collider
            if (dfUnity.Option_AddMeshColliders)
            {
                MeshCollider collider = go.AddComponent<MeshCollider>();
                collider.sharedMesh = mesh;
            }

            // Assign static
            if (makeStatic)
                TagStaticGeometry(go);

            return go;
        }

        public static GameObject CreateDaggerfallBillboardGameObject(int archive, int record, Transform parent)
        {
            string flatName = GetGoFlatName(archive, record);
            GameObject go = new GameObject(flatName);
            if (parent) go.transform.parent = parent;

            Billboard dfBillboard = go.AddComponent<DaggerfallBillboard>();
            dfBillboard.SetMaterial(archive, record);

            if (PlayerActivate.HasCustomActivation(flatName)) 
            {
                // Add box collider to flats with actions for raycasting - only flats that can be activated directly need this, so this can possibly be restricted in future
                // Skip this for flats that already have a collider assigned from elsewhere (e.g. NPC flats)
                if (!go.GetComponent<Collider>())
                {
                    Collider col = go.AddComponent<BoxCollider>();
                    col.isTrigger = true;
                }
            }

            return go;
        }

        public static void AlignBillboardToGround(GameObject go, Vector2 size, float distance = 2f)
        {
            // Cast ray down to find ground below
            RaycastHit hit;
            Ray ray = new Ray(go.transform.position + new Vector3(0, 0.2f, 0), Vector3.down);
            if (!Physics.Raycast(ray, out hit, distance))
                return;

            // Position bottom just above ground by adjusting parent gameobject
            go.transform.position = new Vector3(hit.point.x, hit.point.y + size.y * 0.52f, hit.point.z);
        }

        public static void AlignControllerToGround(CharacterController controller, float distance = 3f)
        {
            // Exit if no controller specified
            if (controller == null)
                return;

            // Cast ray down from slightly above midpoint to find ground below
            RaycastHit hit;
            Ray ray = new Ray(controller.transform.position + new Vector3(0, 0.2f, 0), Vector3.down);
            if (!Physics.Raycast(ray, out hit, distance))
                return;

            // Position bottom just above ground by adjusting parent gameobject
            controller.transform.position = new Vector3(hit.point.x, hit.point.y + controller.height * 0.52f, hit.point.z);
        }

        /// <summary>
        /// Instantiate a GameObject from prefab.
        /// </summary>
        /// <param name="prefab">The source GameObject prefab to clone.</param>
        /// <param name="name">Optional name to set. Use string.Empty for default.</param>
        /// <param name="parent">Optional parent to set. Use null for default.</param>
        /// <param name="position">Optional position to set. Use Vector3.zero for default.</param>
        /// <returns>GameObject.</returns>
        public static GameObject InstantiatePrefab(GameObject prefab, string name, Transform parent, Vector3 position)
        {
            GameObject go = null;

#if UNITY_EDITOR && KEEP_PREFAB_LINKS
            if (prefab != null)
            {
                //go = GameObject.Instantiate(prefab);
                go = UnityEditor.PrefabUtility.InstantiatePrefab(prefab as GameObject) as GameObject;
                if (!string.IsNullOrEmpty(name)) go.name = name;
                if (parent != null) go.transform.parent = parent;
                go.transform.position = position;
            }
#else
            if (prefab != null)
            {
                go = GameObject.Instantiate(prefab);
                if (!string.IsNullOrEmpty(name)) go.name = name;
                if (parent != null) go.transform.parent = parent;
                go.transform.position = position;
            }
#endif

            return go;
        }

        /// <summary>
        /// Gets best parent for an object at spawn time.
        /// Objects should always be placed to some child object in world rather than directly into root of scene.
        /// </summary>
        /// <returns>Best parent transform, or null as fallback.</returns>
        public static Transform GetBestParent()
        {
            PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;

            // Place in world near player depending on local area
            if (playerEnterExit.IsPlayerInsideBuilding)
            {
                return playerEnterExit.Interior.transform;
            }
            else if (playerEnterExit.IsPlayerInsideDungeon)
            {
                return playerEnterExit.Dungeon.transform;
            }
            else if (!playerEnterExit.IsPlayerInside && GameManager.Instance.PlayerGPS.IsPlayerInLocationRect)
            {
                return GameManager.Instance.StreamingWorld.CurrentPlayerLocationObject.transform;
            }
            else if (!playerEnterExit.IsPlayerInside)
            {
                return GameManager.Instance.StreamingTarget.transform;
            }
            else
            {
                return null;
            }
        }

        public static void TagStaticGeometry(GameObject go)
        {
            if (go)
            {
                go.tag = DaggerfallUnity.staticGeometryTag;
            }
        }

        public static bool IsStaticGeometry(GameObject go)
        {
            if (go)
            {
                return go.CompareTag(DaggerfallUnity.staticGeometryTag);
            }

            return false;
        }

        #region RMB & RDB Block Helpers

        /// <summary>
        /// Layout RMB block gamne object from name only.
        /// This will be missing information like building data and should only be used standalone.
        /// </summary>
        public static GameObject CreateRMBBlockGameObject(
            string blockName,
            int layoutX,
            int layoutY,
            int mapId,
            int locationIndex,
            bool addGroundPlane = true,
            DaggerfallRMBBlock cloneFrom = null,
            DaggerfallBillboardBatch natureBillboardBatch = null,
            DaggerfallBillboardBatch lightsBillboardBatch = null,
            DaggerfallBillboardBatch animalsBillboardBatch = null,
            TextureAtlasBuilder miscBillboardAtlas = null,
            DaggerfallBillboardBatch miscBillboardBatch = null,
            ClimateNatureSets climateNature = ClimateNatureSets.TemperateWoodland,
            ClimateSeason climateSeason = ClimateSeason.Summer)
        {
            // Get block data from name
            DFBlock blockData;
            if (!RMBLayout.GetBlockData(blockName, out blockData))
                return null;

            // Create base object from block data
            GameObject go = CreateRMBBlockGameObject(
                blockData,
                layoutX,
                layoutY,
                mapId,
                locationIndex,
                addGroundPlane,
                cloneFrom,
                natureBillboardBatch,
                lightsBillboardBatch,
                animalsBillboardBatch,
                miscBillboardAtlas,
                miscBillboardBatch,
                climateNature,
                climateSeason);

            return go;
        }

        /// <summary>
        /// Layout RMB block game object from DFBlock data.
        /// </summary>
        public static GameObject CreateRMBBlockGameObject(
            DFBlock blockData,
            int layoutX,
            int layoutY,
            int mapId,
            int locationIndex,
            bool addGroundPlane = true,
            DaggerfallRMBBlock cloneFrom = null,
            DaggerfallBillboardBatch natureBillboardBatch = null,
            DaggerfallBillboardBatch lightsBillboardBatch = null,
            DaggerfallBillboardBatch animalsBillboardBatch = null,
            TextureAtlasBuilder miscBillboardAtlas = null,
            DaggerfallBillboardBatch miscBillboardBatch = null,
            ClimateNatureSets climateNature = ClimateNatureSets.TemperateWoodland,
            ClimateSeason climateSeason = ClimateSeason.Summer)
        {
            // Get DaggerfallUnity
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return null;

            // Create base object
            GameObject go = RMBLayout.CreateBaseGameObject(ref blockData, layoutX, layoutY, cloneFrom);

            // Create flats node
            GameObject flatsNode = new GameObject("Flats");
            flatsNode.transform.parent = go.transform;

            // Create lights node
            GameObject lightsNode = new GameObject("Lights");
            lightsNode.transform.parent = go.transform;

            // If billboard batching is enabled but user has not specified
            // a batch, then make our own auto batch for this block
            bool autoLightsBatch = false;
            bool autoNatureBatch = false;
            bool autoAnimalsBatch = false;
            if (dfUnity.Option_BatchBillboards)
            {
                if (natureBillboardBatch == null)
                {
                    autoNatureBatch = true;
                    int natureArchive = ClimateSwaps.GetNatureArchive(climateNature, climateSeason);
                    natureBillboardBatch = GameObjectHelper.CreateBillboardBatchGameObject(natureArchive, flatsNode.transform);
                }
                if (lightsBillboardBatch == null)
                {
                    autoLightsBatch = true;
                    lightsBillboardBatch = GameObjectHelper.CreateBillboardBatchGameObject(TextureReader.LightsTextureArchive, flatsNode.transform);
                }
                if (animalsBillboardBatch == null)
                {
                    autoAnimalsBatch = true;
                    animalsBillboardBatch = GameObjectHelper.CreateBillboardBatchGameObject(TextureReader.AnimalsTextureArchive, flatsNode.transform);
                }
            }

            // Layout light billboards and gameobjects
            RMBLayout.AddLights(ref blockData, flatsNode.transform, lightsNode.transform, lightsBillboardBatch);

            // Layout nature billboards
            RMBLayout.AddNatureFlats(ref blockData, flatsNode.transform, natureBillboardBatch, climateNature, climateSeason);

            // Layout all other flats
            RMBLayout.AddMiscBlockFlats(ref blockData, flatsNode.transform, mapId, locationIndex, animalsBillboardBatch, miscBillboardAtlas, miscBillboardBatch);

            // Layout any subrecord exterior flats
            RMBLayout.AddExteriorBlockFlats(ref blockData, flatsNode.transform, lightsNode.transform, mapId, locationIndex, climateNature, climateSeason);

            // Add ground plane
            if (addGroundPlane)
                RMBLayout.AddGroundPlane(ref blockData, go.transform);

            // Apply auto batches
            if (autoNatureBatch) natureBillboardBatch.Apply();
            if (autoLightsBatch) lightsBillboardBatch.Apply();
            if (autoAnimalsBatch) animalsBillboardBatch.Apply();

            return go;
        }

        /// <summary>
        /// Layout a complete RDB block game object.
        /// </summary>
        /// <param name="blockName">Name of block to create.</param>
        /// <param name="textureTable">Optional texture table for dungeon.</param>
        /// <param name="allowExitDoors">Add exit doors to block (for start blocks).</param>
        /// <param name="dungeonType">Dungeon type for random encounters.</param>
        /// <param name="seed">Seed for random encounters.</param>
        /// <param name="cloneFrom">Clone and build on a prefab object template.</param>
        /// <param name="importEnemies">Import enemies from game data.</param>
        public static GameObject CreateRDBBlockGameObject(
            string blockName,
            int[] textureTable = null,
            bool allowExitDoors = true,
            DFRegion.DungeonTypes dungeonType = DFRegion.DungeonTypes.HumanStronghold,
            float monsterPower = 0.5f,
            int monsterVariance = 4,
            int seed = 0,
            DaggerfallRDBBlock cloneFrom = null,
            bool importEnemies = true)
        {
            // Get DaggerfallUnity
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return null;

            Dictionary<int, RDBLayout.ActionLink> actionLinkDict = new Dictionary<int, RDBLayout.ActionLink>();

            // Create base object
            DFBlock blockData;
            GameManager.Instance.PlayerEnterExit.IsCreatingDungeonObjects = true;
            GameObject go = RDBLayout.CreateBaseGameObject(blockName, actionLinkDict, out blockData, textureTable, allowExitDoors, cloneFrom);

            // Add action doors
            RDBLayout.AddActionDoors(go, actionLinkDict, ref blockData, textureTable);

            // Add lights
            RDBLayout.AddLights(go, ref blockData);

            // Add flats
            DFBlock.RdbObject[] editorObjects;
            GameObject[] startMarkers;
            GameObject[] enterMarkers;
            RDBLayout.AddFlats(go, actionLinkDict, ref blockData, out editorObjects, out startMarkers, out enterMarkers, dungeonType);

            // Set start and enter markers
            DaggerfallRDBBlock dfBlock = go.GetComponent<DaggerfallRDBBlock>();
            if (dfBlock != null)
                dfBlock.SetMarkers(startMarkers, enterMarkers);

            // Add enemies
            if (importEnemies)
            {
                RDBLayout.AddFixedEnemies(go, editorObjects, ref blockData, startMarkers);
                RDBLayout.AddRandomEnemies(go, editorObjects, dungeonType, monsterPower, ref blockData, startMarkers, monsterVariance, seed);
            }

            // Link action nodes
            RDBLayout.LinkActionNodes(actionLinkDict);
            GameManager.Instance.PlayerEnterExit.IsCreatingDungeonObjects = false;

            return go;
        }

        #endregion

        #region Treasure Helpers

        /// <summary>
        /// Creates a generic loot container.
        /// </summary>
        /// <param name="containerType">Type of container.</param>
        /// <param name="containerImage">Icon to display in loot UI.</param>
        /// <param name="position">Position to spawn container.</param>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="textureArchive">Texture archive for billboard containers.</param>
        /// <param name="textureRecord">Texture record for billboard containers.</param>
        /// <param name="loadID">Unique LoadID for save system.</param>
        /// <returns>DaggerfallLoot.</returns>
        public static DaggerfallLoot CreateLootContainer(
            LootContainerTypes containerType,
            InventoryContainerImages containerImage,
            Vector3 position,
            Transform parent,
            int textureArchive,
            int textureRecord,
            ulong loadID = 0,
            EnemyEntity enemyEntity = null,
            bool adjustPosition = true)
        {
            // Setup initial loot container prefab
            GameObject go = InstantiatePrefab(DaggerfallUnity.Instance.Option_LootContainerPrefab.gameObject, containerType.ToString(), parent, position);

            // Setup appearance
            if (MeshReplacement.ImportCustomFlatGameobject(textureArchive, textureRecord, Vector3.zero, go.transform))
            {
                // Use imported model instead of billboard
                GameObject.Destroy(go.GetComponent<Billboard>());
                GameObject.Destroy(go.GetComponent<MeshRenderer>());
            }
            else
            {
                // Setup billboard component
                Billboard dfBillboard = go.GetComponent<Billboard>();
                dfBillboard.SetMaterial(textureArchive, textureRecord);

                // Now move up loot icon by half own size so bottom is aligned with position
                if (adjustPosition)
                    position.y += (dfBillboard.Summary.Size.y / 2f);
            }

            // Setup DaggerfallLoot component to make lootable
            DaggerfallLoot loot = go.GetComponent<DaggerfallLoot>();
            if (loot)
            {
                loot.LoadID = loadID;
                loot.ContainerType = containerType;
                loot.ContainerImage = containerImage;
                loot.TextureArchive = textureArchive;
                loot.TextureRecord = textureRecord;
                if (enemyEntity != null)
                {
                    loot.entityName = TextManager.Instance.GetLocalizedEnemyName(enemyEntity.MobileEnemy.ID);
                    loot.isEnemyClass = (enemyEntity.EntityType == EntityTypes.EnemyClass);
                }
            }

            loot.transform.position = position;

            return loot;
        }

        /// <summary>
        /// Creates a loot container for items dropped by the player.
        /// </summary>
        /// <param name="player">Player object, must have PlayerEnterExit and PlayerMotor attached.</param>
        /// <param name="loadID">Unique LoadID for save system.</param>
        /// <returns>DaggerfallLoot.</returns>
        public static DaggerfallLoot CreateDroppedLootContainer(GameObject player, ulong loadID, int iconArchive = DaggerfallLootDataTables.randomTreasureArchive, int iconRecord = -1)
        {
            // Player must have a PlayerEnterExit component
            PlayerEnterExit playerEnterExit = player.GetComponent<PlayerEnterExit>();
            if (!playerEnterExit)
                throw new Exception("CreateDroppedLootContainer() player game object must have PlayerEnterExit component.");

            // Player must have a PlayerMotor component
            PlayerMotor playerMotor = player.GetComponent<PlayerMotor>();
            if (!playerMotor)
                throw new Exception("CreateDroppedLootContainer() player game object must have PlayerMotor component.");

            // Get parent by context
            Transform parent = null;
            if (GameManager.Instance.IsPlayerInside)
            {
                if (GameManager.Instance.IsPlayerInsideDungeon)
                    parent = playerEnterExit.Dungeon.transform;
                else
                    parent = playerEnterExit.Interior.transform;
            }
            else
            {
                parent = GameManager.Instance.StreamingTarget.transform;
            }

            // Randomise container texture, if not manually set
            if (iconRecord == -1)
            {
                int iconIndex = UnityEngine.Random.Range(0, DaggerfallLootDataTables.randomTreasureIconIndices.Length);
                iconRecord = DaggerfallLootDataTables.randomTreasureIconIndices[iconIndex];
            }

            // Find ground position below player
            Vector3 position = playerMotor.FindGroundPosition();

            // Create loot container
            DaggerfallLoot loot = CreateLootContainer(
                LootContainerTypes.DroppedLoot,
                InventoryContainerImages.Chest,
                position,
                parent,
                iconArchive,
                iconRecord,
                loadID);

            // Set properties
            loot.LoadID = loadID;
            loot.customDrop = true;
            loot.playerOwned = true;
            loot.WorldContext = playerEnterExit.WorldContext;

            // If dropped outside ask StreamingWorld to track loose object
            if (!GameManager.Instance.IsPlayerInside)
            {
                GameManager.Instance.StreamingWorld.TrackLooseObject(loot.gameObject, true);
            }

            return loot;
        }

        /// <summary>
        /// Creates a loot container for enemies slain by the player.
        /// </summary>
        /// <param name="player">Player object, must have PlayerEnterExit attached.</param>
        /// <param name="enemy">Enemy object, must have EnemyMotor attached.</param>
        /// <param name="corpseTexture">Packed corpse texture index from entity summary.</param>
        /// <param name="loadID">Unique LoadID for save system.</param>
        /// <returns>DaggerfallLoot.</returns>
        public static DaggerfallLoot CreateLootableCorpseMarker(GameObject player, GameObject enemy, EnemyEntity enemyEntity, int corpseTexture, ulong loadID)
        {
            // Player must have a PlayerEnterExit component
            PlayerEnterExit playerEnterExit = player.GetComponent<PlayerEnterExit>();
            if (!playerEnterExit)
                throw new Exception("CreateLootableCorpseMarker() player game object must have PlayerEnterExit component.");

            // Enemy must have an EnemyMotor component
            EnemyMotor enemyMotor = enemy.GetComponent<EnemyMotor>();
            if (!enemyMotor)
                throw new Exception("CreateLootableCorpseMarker() enemy game object must have EnemyMotor component.");

            // Get parent by context
            Transform parent = null;
            if (GameManager.Instance.IsPlayerInside)
            {
                if (GameManager.Instance.IsPlayerInsideDungeon)
                    parent = playerEnterExit.Dungeon.transform;
                else
                    parent = playerEnterExit.Interior.transform;
            }
            else
            {
                parent = GameManager.Instance.StreamingTarget.transform;
            }

            // Get corpse marker texture indices
            int archive, record;
            EnemyBasics.ReverseCorpseTexture(corpseTexture, out archive, out record);

            // Find ground position below enemy
            Vector3 position = enemyMotor.FindGroundPosition();

            // Create loot container
            DaggerfallLoot loot = CreateLootContainer(
                LootContainerTypes.CorpseMarker,
                InventoryContainerImages.Corpse2,
                position,
                parent,
                archive,
                record,
                loadID,
                enemyEntity);

            // Set properties
            loot.LoadID = loadID;
            loot.customDrop = true;
            loot.playerOwned = false;
            loot.WorldContext = playerEnterExit.WorldContext;

            // If dropped outside ask StreamingWorld to track loose object
            if (!GameManager.Instance.IsPlayerInside)
            {
                GameManager.Instance.StreamingWorld.TrackLooseObject(loot.gameObject, true);
            }

            return loot;
        }

        /// <summary>
        /// Destroys/Disables a loot container.
        /// Ignores unsupported or persistent container types.
        /// Custom drop containers will be destroyed from world.
        /// Fixed containers will be disabled so their empty state continues to be serialized.
        /// </summary>
        /// <param name="loot">DaggerfallLoot.</param>
        public static void RemoveLootContainer(DaggerfallLoot loot)
        {
            // Only certain container types can be removed from world
            // Other container types (e.g. corpse markers and geometry-based containers) will persist
            if (loot.ContainerType == LootContainerTypes.RandomTreasure ||
                loot.ContainerType == LootContainerTypes.DroppedLoot)
            {
                // Destroy or disable based on custom flag
                if (loot.customDrop)
                    GameObject.Destroy(loot.gameObject);
                else
                    loot.gameObject.SetActive(false);
            }
        }

        #endregion

        #region Quest Resource Helpers

        /// <summary>
        /// Gets the most appropriate parent transform based on player context for a freely spawned object.
        /// Buildings, exteriors, and dungeons all have different parents.
        /// </summary>
        /// <returns>Parent transform.</returns>
        public static Transform GetSpawnParentTransform()
        {
            PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;
            if (playerEnterExit.IsPlayerInsideBuilding)
            {
                return playerEnterExit.Interior.transform;
            }
            else if (playerEnterExit.IsPlayerInsideDungeon)
            {
                return playerEnterExit.Dungeon.transform;
            }
            else if (!playerEnterExit.IsPlayerInside && GameManager.Instance.PlayerGPS.IsPlayerInLocationRect)
            {
                return GameManager.Instance.StreamingWorld.CurrentPlayerLocationObject.transform;
            }
            else
            {
                return GameManager.Instance.StreamingWorld.StreamingTarget;
            }
        }

        /// <summary>
        /// Finds SiteLinks matching this interior and walks Place markers to inject quest resources.
        /// Some of this handling will be split and relocated for other builders.
        /// Just working through the steps in buildings interiors for now.
        /// This will be moved to a different setup class later.
        /// </summary>
        public static void AddQuestResourceObjects(SiteTypes siteType, Transform parent, int buildingKey = 0, bool enableNPCs = true, bool enableFoes = true, bool enableItems = true)
        {
            // Collect any SiteLinks associdated with this site
            SiteLink[] siteLinks = QuestMachine.Instance.GetSiteLinks(siteType, GameManager.Instance.PlayerGPS.CurrentMapID, buildingKey);
            if (siteLinks == null || siteLinks.Length == 0)
                return;

            // Walk through all found SiteLinks
            foreach (SiteLink link in siteLinks)
            {
                // Get the Quest object referenced by this link
                Quest quest = QuestMachine.Instance.GetQuest(link.questUID);
                if (quest == null)
                    throw new Exception(string.Format("Could not find active quest for UID {0}", link.questUID));

                // Get the Place resource referenced by this link
                Place place = quest.GetPlace(link.placeSymbol);
                if (place == null)
                    throw new Exception(string.Format("Could not find Place symbol {0} in quest UID {1}", link.placeSymbol, link.questUID));

                // Get all quest resource behaviours already in scene
                // Slightly expensive but only runs once at layout time or when "place thing" is called
                // Helps ensure a resource is not injected twice
                QuestResourceBehaviour[] resourceBehaviours = Resources.FindObjectsOfTypeAll<QuestResourceBehaviour>();

                // Add any resources for the selected marker for this place
                AddMarkerResourceObjects(siteType, parent, enableNPCs, enableFoes, quest, resourceBehaviours, place.SiteDetails.selectedMarker);

                // Add any resources from other non-selected markers
                if (place.SiteDetails.questSpawnMarkers != null)
                    foreach (QuestMarker marker in place.SiteDetails.questSpawnMarkers)
                        AddMarkerResourceObjects(siteType, parent, enableNPCs, enableFoes, quest, resourceBehaviours, marker);
            }
        }

        private static void AddMarkerResourceObjects(SiteTypes siteType, Transform parent, bool enableNPCs, bool enableFoes, Quest quest, QuestResourceBehaviour[] resourceBehaviours, QuestMarker marker)
        {
            if (marker.targetResources != null)
            {
                foreach (Symbol target in marker.targetResources)
                {
                    // Get target resource
                    QuestResource resource = quest.GetResource(target);
                    if (resource == null)
                        continue;

                    // Skip resources already injected into scene
                    if (IsAlreadyInjected(resourceBehaviours, resource))
                        continue;

                    // Inject to scene based on resource type
                    if (resource is Person && enableNPCs)
                    {
                        AddQuestNPC(siteType, quest, marker, (Person)resource, parent);
                    }
                    else if (resource is Foe && enableFoes)
                    {
                        Foe foe = (Foe)resource;
                        if (foe.KillCount < foe.SpawnCount)
                            AddQuestFoe(siteType, quest, marker, foe, parent);
                    }
                    else if (resource is Item)
                    {
                        AddQuestItem(siteType, quest, marker, (Item)resource, parent);
                    }
                }
            }
        }

        /// <summary>
        /// Tests if a resource is assigned inside a QuestResourceBehaviour array.
        /// </summary>
        /// <param name="resourceBehaviours">Array of quest resource behaviours in scene.</param>
        /// <param name="resource">QuestResource to check if already in scene.</param>
        /// <returns>True if QuestResource already assigned to a QuestResourceBehaviour.</returns>
        static bool IsAlreadyInjected(QuestResourceBehaviour[] resourceBehaviours, QuestResource resource)
        {
            if (resourceBehaviours == null || resourceBehaviours.Length == 0)
                return false;

            foreach (QuestResourceBehaviour resourceBehaviour in resourceBehaviours)
            {
                if (resourceBehaviour.TargetSymbol == resource.Symbol)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Add a quest NPC to marker position.
        /// </summary>
        static void AddQuestNPC(SiteTypes siteType, Quest quest, QuestMarker marker, Person person, Transform parent)
        {
            // Get billboard texture data
            FactionFile.FlatData flatData;
            if (person.IsIndividualNPC)
            {
                // Individuals are always flat1 no matter gender
                flatData = FactionFile.GetFlatData(person.FactionData.flat1);
            }
            else if (person.IsQuestor)
            {
                // When person a questor use saved flat indices from questor data
                flatData.archive = person.QuestorData.billboardArchiveIndex;
                flatData.record = person.QuestorData.billboardRecordIndex;
            }
            else if (person.Gender == Genders.Male)
            {
                // Male has flat1
                flatData = FactionFile.GetFlatData(person.FactionData.flat1);
            }
            else
            {
                // Female has flat2
                flatData = FactionFile.GetFlatData(person.FactionData.flat2);
            }
                        
            Vector3 dungeonBlockPosition = new Vector3(marker.dungeonX * RDBLayout.RDBSide, 0, marker.dungeonZ * RDBLayout.RDBSide);
            Vector3 targetPosition = dungeonBlockPosition + marker.flatPosition;
            Billboard dfBillboard;
            bool inDungeon = siteType == SiteTypes.Dungeon;

            // Import or create target GameObject
            GameObject go = MeshReplacement.ImportCustomFlatGameobject(flatData.archive, flatData.record, targetPosition, parent, inDungeon);
            if (go == null)
            {
                go = CreateDaggerfallBillboardGameObject(flatData.archive, flatData.record, parent);
                go.name = string.Format("Quest NPC [{0}]", person.DisplayName);

                // Set position and adjust up by half height if not inside a dungeon
                go.transform.localPosition = targetPosition;
                dfBillboard = go.GetComponent<Billboard>();
                if (!inDungeon)
                    go.transform.localPosition += new Vector3(0, dfBillboard.Summary.Size.y / 2, 0);

                // Align injected NPC with ground
                AlignBillboardToGround(go, dfBillboard.Summary.Size, 4);
            }
            else
            {
                dfBillboard = go.GetComponent<Billboard>();
            }            
            
            if (dfBillboard != null)
            {
                // Add people data to billboard
                dfBillboard.SetRMBPeopleData(person.FactionIndex, person.FactionData.flags);
            }

            // Add QuestResourceBehaviour to GameObject
            QuestResourceBehaviour questResourceBehaviour = go.AddComponent<QuestResourceBehaviour>();
            questResourceBehaviour.AssignResource(person);

            // Set QuestResourceBehaviour in Person object
            person.QuestResourceBehaviour = questResourceBehaviour;

            // Add StaticNPC behaviour
            StaticNPC npc = go.AddComponent<StaticNPC>();
            npc.SetLayoutData((int)marker.flatPosition.x, (int)marker.flatPosition.y, (int)marker.flatPosition.z, person);

            // Set tag
            go.tag = QuestMachine.questPersonTag;
        }

        /// <summary>
        /// Adds a single quest foe to marker position.
        /// </summary>
        static void AddQuestFoe(SiteTypes siteType, Quest quest, QuestMarker marker, Foe foe, Transform parent)
        {
            // Do not add foe during load process as enemy object may no longer be in starting state
            // Allow the load process to restore enemy state to whatever it was at time of save
            if (SaveLoadManager.Instance.LoadInProgress)
                return;

            // Get foe gender
            MobileGender mobileGender = MobileGender.Unspecified;
            if (foe.Gender == Genders.Male)
                mobileGender = MobileGender.Male;
            else if (foe.Gender == Genders.Female)
                mobileGender = MobileGender.Female;

            // Create enemy GameObject
            Vector3 dungeonBlockPosition = new Vector3(marker.dungeonX * RDBLayout.RDBSide, 0, marker.dungeonZ * RDBLayout.RDBSide);
            GameObject go = CreateEnemy("Quest Foe", foe.FoeType, dungeonBlockPosition + marker.flatPosition, mobileGender, parent);

            // Assign loadID and custom spawn
            DaggerfallEnemy enemy = go.GetComponent<DaggerfallEnemy>();
            if (enemy)
            {
                enemy.LoadID = DaggerfallUnity.NextUID;
                enemy.QuestSpawn = true;
            }

            // Add QuestResourceBehaviour to GameObject
            QuestResourceBehaviour questResourceBehaviour = go.AddComponent<QuestResourceBehaviour>();
            questResourceBehaviour.AssignResource(foe);

            // Set QuestResourceBehaviour in this particular instantiated Foe object
            // Each GameObject placed in world for this Foe will reference same Foe quest resource
            // Keep this one-to-many relationship in mind for Foe handling
            foe.QuestResourceBehaviour = questResourceBehaviour;

            // Rearm injured trigger at time of placement
            // Notes for later:
            //  * This should be rearmed at the beginning of each wave
            //  * Only first wounding of a wave will trigger "injured aFoe" until rearmed on next wave
            foe.RearmInjured();
        }

        /// <summary>
        /// Adds a quest item to marker position.
        /// </summary>
        static void AddQuestItem(SiteTypes siteType, Quest quest, QuestMarker marker, Item item, Transform parent = null)
        {
            // Texture indices for quest items are from world texture record
            int textureArchive = item.DaggerfallUnityItem.WorldTextureArchive;
            int textureRecord = item.DaggerfallUnityItem.WorldTextureRecord;

            // Create billboard
            GameObject go = CreateDaggerfallBillboardGameObject(textureArchive, textureRecord, parent);
            Billboard dfBillboard = go.GetComponent<Billboard>();

            // Set name
            go.name = string.Format("Quest Item [{0} | {1}]", item.Symbol.Original, item.DaggerfallUnityItem.LongName);

            // Marker position
            Vector3 dungeonBlockPosition = new Vector3(marker.dungeonX * RDBLayout.RDBSide, 0, marker.dungeonZ * RDBLayout.RDBSide);
            Vector3 position = dungeonBlockPosition + marker.flatPosition;

            // Dungeon flats have a different origin (centre point) than elsewhere (base point)
            // Find bottom of marker in world space as it should be aligned to placement surface (e.g. ground, table, shelf, etc.)
            if (siteType == SiteTypes.Dungeon)
                position.y += (-DaggerfallLoot.randomTreasureMarkerDim / 2 * MeshReader.GlobalScale);

            // Move up item icon by half own size
            position.y += (dfBillboard.Summary.Size.y / 2f);

            // Assign final position
            go.transform.localPosition = position;

            // Parent to scene marker (if any)
            // This ensures mobile quest objects parented to action marker translates correctly
            DaggerfallMarker sceneMarker = GetDaggerfallMarker(marker.markerID);
            if (sceneMarker)
                go.transform.parent = sceneMarker.transform;

            // Add QuestResourceBehaviour to GameObject
            QuestResourceBehaviour questResourceBehaviour = go.AddComponent<QuestResourceBehaviour>();
            questResourceBehaviour.AssignResource(item);

            // Set QuestResourceBehaviour in Item object
            item.QuestResourceBehaviour = questResourceBehaviour;

            // Assign a trigger collider for clicks
            SphereCollider collider = go.AddComponent<SphereCollider>();
            collider.isTrigger = true;
        }

        /// <summary>
        /// Get special marker in scene matching markerID.
        /// </summary>
        static DaggerfallMarker GetDaggerfallMarker(ulong markerID)
        {
            DaggerfallMarker result = null;
            DaggerfallMarker[] markers = GameObject.FindObjectsOfType<DaggerfallMarker>();
            foreach(DaggerfallMarker marker in markers)
            {
                if (marker.MarkerID == markerID)
                {
                    // Workaround for edge case of duplicate markerIDs in existing saves
                    // When same block used more than once in dungeon it becomes possible to have duplicate marker IDs for quest placement
                    // The below ensures marker is always unique or null to prevent bad parenting behaviour
                    // Only real impact of this change is that quest items will not translate with parent marker object if action record present on marker
                    // This is a very rare situation and mainly used when raising treasure room cage for totem in Daggerfall castle (a unique block)
                    // In vast majority of cases parenting is not even required, so minimal harm just filtering duplicates here
                    // The way marker IDs are generated should still be improved in future
                    if (result == null)
                        result = marker;
                    else
                        return null;
                }
            }

            return result;
        }

        #endregion

        #region Enemy Helpers

        /// <summary>
        /// Create an enemy in the world and perform common setup tasks.
        /// </summary>
        public static GameObject CreateEnemy(string name, MobileTypes mobileType, Vector3 localPosition, MobileGender mobileGender = MobileGender.Unspecified, Transform parent = null, MobileReactions mobileReaction = MobileReactions.Hostile)
        {
            // Create target GameObject
            string displayName = string.Format("{0} [{1}]", name, mobileType.ToString());
            GameObject go = InstantiatePrefab(DaggerfallUnity.Instance.Option_EnemyPrefab.gameObject, displayName, parent, Vector3.zero);
            SetupDemoEnemy setupEnemy = go.GetComponent<SetupDemoEnemy>();

            // Set position
            go.transform.localPosition = localPosition;

            // Assign humanoid gender randomly if unspecfied
            // This does not affect monsters like rats, bats, etc
            MobileGender gender;
            if (mobileGender == MobileGender.Unspecified)
            {
                if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
                    gender = MobileGender.Male;
                else
                    gender = MobileGender.Female;
            }
            else
            {
                gender = mobileGender;
            }

            // Configure enemy
            setupEnemy.ApplyEnemySettings(mobileType, mobileReaction, gender);

            // Align non-flying units with ground
            MobileUnit mobileUnit = setupEnemy.GetMobileBillboardChild();
            if (mobileUnit.Enemy.Behaviour != MobileBehaviour.Flying)
                AlignControllerToGround(go.GetComponent<CharacterController>());

            GameManager.Instance?.RaiseOnEnemySpawnEvent(go);

            return go;
        }

        /// <summary>
        /// Creates enemy GameObjects based on spawn count (minimum of 1, maximum of 8).
        /// Only use this when live enemy is to be first added to scene. Do not use when linking to site or deserializing.
        /// GameObjects created will be disabled, at position specified, parentless, and have a new UID for LoadID.
        /// Caller must otherwise complete GameObject setup to suit their needs before enabling.
        /// </summary>
        /// <param name="reaction">Foe is hostile by default but can optionally set to passive.</param>
        /// <returns>GameObject[] array of 1-N foes. Array can be null or empty if create fails.</returns>
        public static GameObject[] CreateFoeGameObjects(Vector3 position, MobileTypes foeType, int spawnCount = 1, MobileReactions reaction = MobileReactions.Hostile, Foe foeResource = null, bool alliedToPlayer = false)
        {
            List<GameObject> gameObjects = new List<GameObject>();

            // Clamp total spawn count
            int totalSpawns = Mathf.Clamp(spawnCount, 1, 8);

            // Generate GameObjects
            for (int i = 0; i < totalSpawns; i++)
            {
                // Generate enemy
                string name = string.Format("DaggerfallEnemy [{0}]", foeType.ToString());
                GameObject go = GameObjectHelper.InstantiatePrefab(DaggerfallUnity.Instance.Option_EnemyPrefab.gameObject, name, GetBestParent(), position);
                SetupDemoEnemy setupEnemy = go.GetComponent<SetupDemoEnemy>();
                if (setupEnemy != null)
                {
                    // Assign gender randomly
                    MobileGender gender;
                    if (UnityEngine.Random.Range(0f, 1f) < 0.55f)
                        gender = MobileGender.Male;
                    else
                        gender = MobileGender.Female;

                    // Configure enemy
                    setupEnemy.ApplyEnemySettings(foeType, reaction, gender, alliedToPlayer: alliedToPlayer);

                    // Align non-flying units with ground
                    MobileUnit mobileUnit = setupEnemy.GetMobileBillboardChild();
                    if (mobileUnit.Enemy.Behaviour != MobileBehaviour.Flying)
                        GameObjectHelper.AlignControllerToGround(go.GetComponent<CharacterController>());

                    // Add QuestResourceBehaviour to GameObject
                    if (foeResource != null)
                    {
                        QuestResourceBehaviour questResourceBehaviour = go.AddComponent<QuestResourceBehaviour>();
                        questResourceBehaviour.AssignResource(foeResource);
                    }
                }

                // Assign load id
                DaggerfallEnemy enemy = go.GetComponent<DaggerfallEnemy>();
                if (enemy)
                {
                    enemy.LoadID = DaggerfallUnity.NextUID;
                    if (foeResource != null)
                        enemy.QuestSpawn = true;
                }

                // Disable GameObject, caller must set active when ready
                go.SetActive(false);

                GameManager.Instance?.RaiseOnEnemySpawnEvent(go);

                // Add to list
                gameObjects.Add(go);
            }

            return gameObjects.ToArray();
        }

        /// <summary>
        /// Create a new foe spawner.
        /// The spawner will self-destroy once it has emitted foes into world around player.
        /// </summary>
        /// <param name="lineOfSightCheck">Should spawner try to place outside of player's field of view.</param>
        /// <param name="foeType">Type of foe to spawn.</param>
        /// <param name="spawnCount">Number of duplicate foes to spawn.</param>
        /// <param name="minDistance">Minimum distance from player.</param>
        /// <param name="maxDistance">Maximum distance from player.</param>
        /// <param name="parent">Parent GameObject. If none specified the most suitable parent will be selected automatically.</param>
        /// <returns>FoeSpawner GameObject.</returns>
        public static GameObject CreateFoeSpawner(bool lineOfSightCheck = true, MobileTypes foeType = MobileTypes.None, int spawnCount = 0, float minDistance = 4, float maxDistance = 20, Transform parent = null, bool alliedToPlayer = false)
        {
            // Create new foe spawner
            GameObject go = new GameObject();
            FoeSpawner spawner = go.AddComponent<FoeSpawner>();
            spawner.LineOfSightCheck = lineOfSightCheck;
            spawner.FoeType = foeType;
            spawner.SpawnCount = spawnCount;
            spawner.MinDistance = minDistance;
            spawner.MaxDistance = maxDistance;
            spawner.Parent = parent;
            spawner.AlliedToPlayer = alliedToPlayer;

            // Assign position on top of player
            // Spawner can be placed anywhere to work, but rest system considers a spawner to be an enemy "in potentia" for purposes of breaking rest and travel
            // Placing spawner on player at moment of creation will trigger the nearby enemy check even while spawn is pending
            spawner.transform.position = GameManager.Instance.PlayerObject.transform.position;

            return go;
        }

        #endregion

        /// <summary>
        /// Create a billboard batch.
        /// </summary>
        /// <param name="archive">Archive this batch is to use.</param>
        /// <param name="parent">Parent transform.</param>
        /// <returns>Billboard batch GameObject.</returns>
        public static DaggerfallBillboardBatch CreateBillboardBatchGameObject(int archive, Transform parent = null)
        {
            // Create new billboard batch object parented to terrain
            GameObject billboardBatchObject = new GameObject();
            billboardBatchObject.transform.parent = parent;
            billboardBatchObject.transform.localPosition = Vector3.zero;
            DaggerfallBillboardBatch c = billboardBatchObject.AddComponent<DaggerfallBillboardBatch>();

            // Setup batch
            c.SetMaterial(archive);

            return c;
        }

        /// <summary>
        /// Create a billboard batch with custom material/
        /// </summary>
        /// <param name="material">Custom atlas material.</param>
        /// <param name="parent">Parent transform.</param>
        /// <returns>Billboard batch GameObject.</returns>
        public static DaggerfallBillboardBatch CreateBillboardBatchGameObject(Material material, Transform parent = null)
        {
            // Create new billboard batch object parented to terrain
            GameObject billboardBatchObject = new GameObject();
            billboardBatchObject.transform.parent = parent;
            billboardBatchObject.transform.localPosition = Vector3.zero;
            DaggerfallBillboardBatch c = billboardBatchObject.AddComponent<DaggerfallBillboardBatch>();

            // Setup batch
            c.SetMaterial(material);

            return c;
        }

        public static bool FindMultiNameLocation(string multiName, out DFLocation locationOut)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;

            locationOut = new DFLocation();

            if (string.IsNullOrEmpty(multiName))
                return false;

            // Split combined name
            string[] parts = multiName.Split('/');
            if (parts.Length != 2)
            {
                DaggerfallUnity.LogMessage(string.Format("Multi name '{0}' does not follow the structure RegionName/LocationName.", multiName), true);
                return false;
            }

            // Get location
            if (!dfUnity.ContentReader.GetLocation(parts[0], parts[1], out locationOut))
                return false;

            return true;
        }

        public static GameObject CreateDaggerfallLocationGameObject(string multiName, Transform parent)
        {
            // Get city
            DFLocation location;
            if (!FindMultiNameLocation(multiName, out location))
                return null;

            GameObject go = new GameObject(string.Format("DaggerfallLocation [Region={0}, Name={1}]", location.RegionName, location.Name));
            if (parent) go.transform.parent = parent;
            DaggerfallLocation c = go.AddComponent<DaggerfallLocation>() as DaggerfallLocation;
            c.SetLocation(location);

            return go;
        }

        public static GameObject CreateDaggerfallDungeonGameObject(string multiName, Transform parent, bool importEnemies = true)
        {
            // Get dungeon
            DaggerfallDungeon daggerfallDungeon = null;
            DFLocation location;
            if (!FindMultiNameLocation(multiName, out location))
                return null;
            
            GameObject daggerfallDungeonObject;
            daggerfallDungeon = CreateDaggerfallDungeonGameObject(location, parent, out daggerfallDungeonObject);
            daggerfallDungeon.SetDungeon(location, importEnemies);

            return daggerfallDungeonObject;
        }

        public static DaggerfallDungeon CreateDaggerfallDungeonGameObject(DFLocation location, Transform parent, out GameObject go)
        {
            go = null;
            if (!location.HasDungeon)
            {
                string multiName = string.Format("{0}/{1}", location.RegionName, location.Name);
                DaggerfallUnity.LogMessage(string.Format("Location '{0}' does not contain a dungeon map", multiName), true);
                return null;
            }

            go = new GameObject(DaggerfallDungeon.GetSceneName(location));
            if (parent)
                go.transform.parent = parent;
            DaggerfallDungeon daggerfallDungeon = go.AddComponent<DaggerfallDungeon>();

            return daggerfallDungeon;
        }

        public static GameObject CreateDaggerfallTerrainGameObject(Transform parent)
        {
            // Create Unity Terrain game object
            GameObject go = Terrain.CreateTerrainGameObject(null);
            go.gameObject.transform.parent = parent;
            go.gameObject.transform.localPosition = Vector3.zero;

            // Add DaggerfallTerrain component
            go.AddComponent<DaggerfallTerrain>();

            return go;
        }

        /// <summary>
        /// Gets static door array from door information stored in model data.
        /// </summary>
        /// <param name="modelData">Model data for doors.</param>
        /// <param name="blockIndex">Block index for RMB doors.</param>
        /// <param name="recordIndex">Record index of interior.</param>
        /// <param name="buildingMatrix">Individual building matrix.</param>
        /// <returns>Array of doors in this model data.</returns>
        public static StaticDoor[] GetStaticDoors(ref ModelData modelData, int blockIndex, int recordIndex, Matrix4x4 buildingMatrix)
        {
            // Exit if no doors
            if (modelData.Doors == null)
                return null;

            // Add door triggers
            StaticDoor[] staticDoors = new StaticDoor[modelData.Doors.Length];
            for (int i = 0; i < modelData.Doors.Length; i++)
            {
                // Get door and diagonal verts
                ModelDoor door = modelData.Doors[i];
                Vector3 v0 = door.Vert0;
                Vector3 v2 = door.Vert2;

                // Get absolute door size and make thickness uniform from largest width or depth
                float width = Mathf.Abs(v2.x - v0.x);
                float height = Mathf.Abs(v2.y - v0.y);
                float depth = Mathf.Abs(v2.z - v0.z);
                float thickness = Mathf.Max(width, depth);
                Vector3 size = new Vector3(thickness, Mathf.Max(height, thickness), Mathf.Min(height, thickness));

                // Add door to array
                StaticDoor newDoor = new StaticDoor()
                {
                    buildingMatrix = buildingMatrix,
                    doorType = door.Type,
                    blockIndex = blockIndex,
                    recordIndex = recordIndex,
                    doorIndex = door.Index,
                    centre = (v0 + v2) / 2f,
                    normal = door.Normal,
                    size = size,
                };
                staticDoors[i] = newDoor;
            }

            return staticDoors;
        }

        // Helper to extract quaternion from matrix
        public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
        {
            // Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
            Quaternion q = new Quaternion();
            q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
            q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
            q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
            q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;
            q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
            q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
            q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));
            return q;
        }
    }
}