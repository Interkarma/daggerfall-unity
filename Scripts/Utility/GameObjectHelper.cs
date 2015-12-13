// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
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
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;

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

        /// <summary>
        /// Adds a single DaggerfallMesh game object to scene.
        /// </summary>
        /// <param name="modelID">ModelID of mesh to add.</param>
        /// <param name="parent">Optional parent of this object.</param>
        /// <param name="makeStatic">Flag to set object static flag.</param>
        /// <param name="useExistingObject">Add mesh to existing object rather than create new.</param>
        /// <param name="ignoreCollider">Force disable collider.</param>
        /// <returns>GameObject.</returns>
        public static GameObject CreateDaggerfallMeshGameObject(
            uint modelID,
            Transform parent,
            bool makeStatic = false,
            GameObject useExistingObject = null,
            bool ignoreCollider = false)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;

            // Create gameobject
            string name = string.Format("DaggerfallMesh [ID={0}]", modelID);
            GameObject go = (useExistingObject != null) ? useExistingObject : new GameObject();
            if (parent != null)
                go.transform.parent = parent;
            go.name = name;

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
            }

            // Assign static
            if (makeStatic)
                go.isStatic = true;

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
            dfMesh.name = string.Format("DaggerfallMesh [ID={0}]", newModelID);
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
                go.isStatic = true;

            return go;
        }

        public static GameObject CreateDaggerfallBillboardGameObject(int archive, int record, Transform parent, bool dungeon = false)
        {
            GameObject go = new GameObject(string.Format("DaggerfallBillboard [TEXTURE.{0:000}, Index={1}]", archive, record));
            if (parent) go.transform.parent = parent;

            DaggerfallBillboard dfBillboard = go.AddComponent<DaggerfallBillboard>();
            dfBillboard.SetMaterial(archive, record, 0, dungeon);

            return go;
        }

        public static void AlignBillboardToGround(GameObject go, Vector2 size, float distance = 2f)
        {
            // Cast ray down to find ground below
            RaycastHit hit;
            Ray ray = new Ray(go.transform.position, Vector3.down);
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

        #region RMB & RDB Block Helpers

        /// <summary>
        /// Layout a complete RMB block game object.
        /// Can be used standalone or as part of a city build.
        /// </summary>
        public static GameObject CreateRMBBlockGameObject(
            string blockName,
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
            DFBlock blockData;
            GameObject go = RMBLayout.CreateBaseGameObject(blockName, out blockData, cloneFrom);

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
            RMBLayout.AddMiscBlockFlats(ref blockData, flatsNode.transform, animalsBillboardBatch, miscBillboardAtlas, miscBillboardBatch);

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
        public static GameObject CreateRDBBlockGameObject(
            string blockName,
            int[] textureTable = null,
            bool allowExitDoors = true,
            DFRegion.DungeonTypes dungeonType = DFRegion.DungeonTypes.HumanStronghold,
            float monsterPower = 0.5f,
            int monsterVariance = 4,
            int seed = 0,
            DaggerfallRDBBlock cloneFrom = null)
        {
            // Get DaggerfallUnity
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return null;

            Dictionary<int, RDBLayout.ActionLink> actionLinkDict = new Dictionary<int, RDBLayout.ActionLink>();

            // Create base object
            DFBlock blockData;
            GameObject go = RDBLayout.CreateBaseGameObject(blockName, actionLinkDict, out blockData, textureTable, allowExitDoors, cloneFrom);
            // Add action doors
            RDBLayout.AddActionDoors(go, actionLinkDict, ref blockData, textureTable);

            // Add lights
            RDBLayout.AddLights(go, ref blockData);

            // Add flats
            DFBlock.RdbObject[] editorObjects;
            GameObject[] startMarkers;
            GameObject[] enterMarkers;
            RDBLayout.AddFlats(go, actionLinkDict, ref blockData, out editorObjects, out startMarkers, out enterMarkers);

            // Set start and enter markers
            DaggerfallRDBBlock dfBlock = go.GetComponent<DaggerfallRDBBlock>();
            if (dfBlock != null)
                dfBlock.SetMarkers(startMarkers, enterMarkers);

            // Add enemies
            RDBLayout.AddFixedEnemies(go, editorObjects, ref blockData);
            RDBLayout.AddRandomEnemies(go, editorObjects, dungeonType, monsterPower, ref blockData, monsterVariance, seed);

            // Link action nodes
            RDBLayout.LinkActionNodes(actionLinkDict);
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

        public static GameObject CreateDaggerfallDungeonGameObject(string multiName, Transform parent)
        {
            // Get dungeon
            DFLocation location;
            if (!FindMultiNameLocation(multiName, out location))
                return null;

            return CreateDaggerfallDungeonGameObject(location, parent);
        }

        public static GameObject CreateDaggerfallDungeonGameObject(DFLocation location, Transform parent)
        {
            if (!location.HasDungeon)
            {
                string multiName = string.Format("{0}/{1}", location.RegionName, location.Name);
                DaggerfallUnity.LogMessage(string.Format("Location '{0}' does not contain a dungeon map", multiName), true);
                return null;
            }

            GameObject go = new GameObject(string.Format("DaggerfallDungeon [Region={0}, Name={1}]", location.RegionName, location.Name));
            if (parent) go.transform.parent = parent;
            DaggerfallDungeon c = go.AddComponent<DaggerfallDungeon>();
            c.SetDungeon(location);

            return go;
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

                // Get door size
                const float thickness = 0.05f;
                Vector3 size = new Vector3(v2.x - v0.x, v2.y - v0.y, v2.z - v0.z) + door.Normal * thickness;

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