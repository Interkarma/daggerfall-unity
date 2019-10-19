// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Uncanny_Valley
// Contributors:    TheLacus 
// 
// Notes:
//

/*
 * TODO:
 * - Support for first person weapons
 * - Support for town NPCs and enemies
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Handles import and injection of GamObjects with the purpose of providing modding support.
    /// Daggerfall meshes and billboards can be replaced with a prefab imported from mods.
    /// </summary>
    public static class MeshReplacement
    {
        #region Fields

        static Func<float> getTreeScaleCallback = () => Random.Range(0.6f, 1.4f);
        static Func<Color32> getTreeColorCallback = () => Color.Lerp(Color.white, Color.grey, Random.value);
        static Action<Terrain> setTreesSettingsCallback = SetTreesSettings;

        static HashSet<Vector2Int> triedBillboards = new HashSet<Vector2Int>();
        static HashSet<uint> triedModels = new HashSet<uint>();

        #endregion

        #region Properties

        public static Func<float> GetTreeScaleCallback
        {
            set { SetCallback(ref getTreeScaleCallback, value); }
        }

        public static Func<Color32> GetTreeColorCallback
        {
            set { SetCallback(ref getTreeColorCallback, value); }
        }

        public static Action<Terrain> SetTreesSettingsCallback
        {
            set { SetCallback(ref setTreesSettingsCallback, value); }
        }

        #endregion

        #region Public Methods

        public static void RetryAssetImports()
        {
            triedBillboards.Clear();
            triedModels.Clear();
        }

        /// <summary>
        /// Seek and import a GameObject from mods to replace a Daggerfall mesh.
        /// </summary>
        /// <param name="modelID">Daggerfall model ID.</param>
        /// <param name="parent">Parent to assign to GameObject.</param>
        /// <param name="matrix">Matrix with position and rotation of GameObject.</param>
        /// <returns>Returns the imported model or null if not found.</returns>
        public static GameObject ImportCustomGameobject (uint modelID, Transform parent, Matrix4x4 matrix)
        {
            GameObject go;
            if (!TryImportGameObject(modelID, true, out go))
                return null;

            go.name = GameObjectHelper.GetGoModelName(modelID) + " [Replacement]";
            go.transform.parent = parent;
            go.transform.position = matrix.GetColumn(3);
            go.transform.rotation = GameObjectHelper.QuaternionFromMatrix(matrix);

            // Finalise gameobject
            FinaliseMaterials(go);
            return go;
        }

        /// <summary>
        /// Seek and import a GameObject from mods to replace a Daggerfall billboard.
        /// </summary>
        /// <param name="archive">Texture archive for original billboard.</param>
        /// <param name="record">Texture record for original billboard.</param>
        /// <param name="position">Position to assign to GameObject.</param>
        /// <param name="parent">Parent to assign to GameObject.</param>
        /// <param name="inDungeon">Fix position for dungeon models.</param>
        /// <returns>Returns the imported model or null if not found.</returns>
        public static GameObject ImportCustomFlatGameobject (int archive, int record, Vector3 position, Transform parent, bool inDungeon = false)
        {
            GameObject go;
            if (!TryImportGameObject(archive, record, true, out go))
                return null;

            go.name = string.Format("DaggerfallBillboard [Replacement] [TEXTURE.{0:000}, Index={1}]", archive, record);
            go.transform.parent = parent;

            // Assign position
            AlignToBase(go.transform, position, archive, record, inDungeon);

            // Assign a random rotation
            var iObjectPositioner = go.GetComponent<IObjectPositioner>();
            if (iObjectPositioner == null || iObjectPositioner.AllowFlatRotation)
            {
                Random.InitState((int)position.x);
                go.transform.Rotate(0, Random.Range(0f, 360f), 0);
            }

            // Add NPC trigger collider
            if (RDBLayout.IsNPCFlat(archive))
            {
                Collider col = go.AddComponent<BoxCollider>();
                col.isTrigger = true;
            }

            // Finalise gameobject materials
            FinaliseMaterials(go);
            return go;
        }

        /// <summary>
        /// Ensures that the requested imported model is assigned to the given transform and is positioned correctly.
        /// If archive and record mismatch, the requested prefab is imported while currently loaded gameobject is destroyed.
        /// This has a similar purpose to <see cref="DaggerfallBillboard.SetMaterial()"/>.
        /// </summary>
        /// <param name="archive">Texture archive for original billboard.</param>
        /// <param name="record">Texture record for original billboard.</param>
        /// <param name="parent">Parent to assign to GameObject.</param>
        /// <param name="position">Position to assign to GameObject.</param>
        /// <param name="inDungeon">Fix position for dungeon models.</param>
        /// <returns>Returns the imported model or null if not found.</returns>
        public static GameObject SwapCustomFlatGameobject(int archive, int record, Transform parent, Vector3 position, bool inDungeon = false)
        {
            GameObject go = null;

            string name = string.Format("DaggerfallBillboard [Replacement] [TEXTURE.{0:000}, Index={1}]", archive, record);
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform transform = parent.GetChild(i);
                if (transform.name == name)
                    AlignToBase((go = transform.gameObject).transform, position, archive, record, inDungeon);
                else if (transform.name.StartsWith("DaggerfallBillboard [Replacement]"))
                    GameObject.Destroy(transform.gameObject);
            }

            return go ?? ImportCustomFlatGameobject(archive, record, position, parent, inDungeon);
        }

        /// <summary>
        /// Import a gameobject from mods to use with the terrain system.
        /// </summary>
        /// <param name="archive">Texture archive for original billboard.</param>
        /// <param name="record">Texture record for original billboard.</param>
        /// <param name="terrain">Unity terrain.</param>
        /// <param name="x">X coordinate on terrain.</param>
        /// <param name="y">Y coordinate on terrain.</param>
        /// <returns>True if gameobject is found and imported.</returns>
        public static bool ImportNatureGameObject(int archive, int record, Terrain terrain, int x, int y)
        {
            const int tilemapDim = MapsFile.WorldMapTileDim - 1;

            GameObject prefab;
            if (!TryImportGameObject(archive, record, false, out prefab))
                return false;

            // Store state of random sequence
            Random.State prevState = Random.state;

            // Get instance properties
            Vector3 position = new Vector3(x / (float)tilemapDim, 0.0f, y / (float)tilemapDim);
            float scale = getTreeScaleCallback();
            Color32 color = getTreeColorCallback();
            float rotation = Random.Range(0f, 360f);
            int index = GetTreePrototypeIndex(terrain.terrainData, prefab);

            // Add tree instance
            TreeInstance treeInstance = new TreeInstance()
            {
                heightScale = scale,
                widthScale = scale,
                color = color,
                lightmapColor = Color.white,
                position = position,
                rotation = rotation,
                prototypeIndex = index
            };
            terrain.AddTreeInstance(treeInstance);

            Random.state = prevState;   // Restore random state
            return true;
        }

        /// <summary>
        /// Remove all tree instances from terrain.
        /// </summary>
        public static void ClearNatureGameObjects(Terrain terrain)
        {
            if (!DaggerfallUnity.Settings.AssetInjection)
                return;

            setTreesSettingsCallback(terrain);

            TerrainData terrainData = terrain.terrainData;
            if (terrainData.treeInstanceCount > 0)
            {
                terrainData.treeInstances = new TreeInstance[0];
                terrainData.treePrototypes = new TreePrototype[0];
                terrainData.RefreshPrototypes();
            }

            terrain.Flush();
        }

        #endregion

        #region Private Methods

        ///<summary>
        /// Seek and import a gameobject from mods from Daggerfall mesh ID.
        ///</summary>
        private static bool TryImportGameObject(uint modelID, bool clone, out GameObject go)
        {
            if (DaggerfallUnity.Settings.AssetInjection)
            {
                if (ModManager.Instance != null)
                {
                    if (!triedModels.Contains(modelID))
                    {
                        if (ModManager.Instance.TryGetAsset(GetName(modelID), clone, out go))
                            return true;
                        else
                            triedModels.Add(modelID);
                    }
                }
            }

            go = null;
            return false;
        }

        ///<summary>
        /// Seek and import a gameobject from mods from Daggerfall billboard texture index.
        ///</summary>
        private static bool TryImportGameObject(int archive, int record, bool clone, out GameObject go)
        {
            if (DaggerfallUnity.Settings.AssetInjection)
            {
                if (ModManager.Instance != null)
                {
                    Vector2Int billboardIdx = new Vector2Int(archive, record);
                    if (!triedBillboards.Contains(billboardIdx))
                    {
                        if (ModManager.Instance.TryGetAsset(GetName(archive, record), clone, out go))
                            return true;
                        else
                            triedBillboards.Add(billboardIdx);
                    }
                }
            }

            go = null;
            return false;
        }

        ///<summary>
        /// Get all accepted names for a model ordered by priority.
        ///</summary>
        private static string[] GetName(uint modelID)
        {
            if (!GameManager.HasInstance)
                return new string[] {modelID.ToString()};

            ClimateBases climateBase = ClimateSwaps.FromAPIClimateBase(GameManager.Instance.PlayerGPS.ClimateSettings.ClimateType);
            string climateName = climateBase == ClimateBases.Desert ?
                string.Format("{0}_{1}", modelID, climateBase):
                string.Format("{0}_{1}{2}", modelID, climateBase, DaggerfallUnity.Instance.WorldTime.Now.SeasonValue);

            return new string[]
            {
                climateName,
                modelID.ToString()
            };
        }

        ///<summary>
        /// Get name for a flat.
        ///</summary>
        private static string GetName(int archive, int record)
        {
            return string.Format("{0:000}_{1}", archive, record);
        }

        /// <summary>
        /// Sets position for a gameobject that replaces a billboard, with the assumption that the origin is at the base of the model.
        /// </summary>
        private static void AlignToBase(Transform transform, Vector3 position, int archive, int record, bool inDungeon)
        {
            // Fix origin position for dungeon flats
            if (inDungeon)
            {
                int height = ImageReader.GetImageData(TextureFile.IndexToFileName(archive), record, createTexture: false).height;
                position.y -= height / 2 * MeshReader.GlobalScale;
            }

            // Assign position
            transform.localPosition = position;
        }

        /// <summary>
        /// Get index of tree prototype on terrain. If new, it will be added to the collection.
        /// </summary>
        private static int GetTreePrototypeIndex(TerrainData terrainData, GameObject prefab)
        {
            // Search existing prototype
            TreePrototype[] treePrototypes = terrainData.treePrototypes;
            for (int i = 0; i < treePrototypes.Length; i++)
            {
                if (treePrototypes[i].prefab == prefab)
                    return i;
            }

            // Add new prototype
            var updatedTreePrototypes = new List<TreePrototype>(treePrototypes);
            var treePrototype = new TreePrototype()
            {
                prefab = prefab,
                bendFactor = 1
            };
            updatedTreePrototypes.Add(treePrototype);
            terrainData.treePrototypes = updatedTreePrototypes.ToArray();
            terrainData.RefreshPrototypes();
            return updatedTreePrototypes.Count - 1;
        }

        /// <summary>
        /// Assign texture filtermode and override materials if found in loose files. 
        /// </summary>
        /// <remarks>
        /// Filtermode is assigned to all supported texture properties. When a material follows
        /// Daggerfall nomenclature a corrispective is seeked from loose files for consistency.
        /// </remarks>
        private static void FinaliseMaterials(GameObject go)
        {
            // Get MaterialReader
            MaterialReader materialReader = DaggerfallUnity.Instance.MaterialReader;

            // Check object and all children
            foreach (var meshRenderer in go.GetComponentsInChildren<MeshRenderer>())
            {
                // Check all materials
                Material[] materials = meshRenderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i])
                    {
                        // Override Daggerfall material with textures from loose files
                        int archive, record;
                        if (TextureReplacement.IsDaggerfallTexture(materials[i].name, out archive, out record)
                            && TextureReplacement.TextureExistsAmongLooseFiles(archive, record, 0))
                        {
                            CachedMaterial cachedMaterialOut;
                            if (materialReader.GetCachedMaterial(archive, record, 0, out cachedMaterialOut))
                            {
                                materials[i] = cachedMaterialOut.material;
                                continue;
                            }
                        }

                        // Assign filtermode to textures
                        TextureReplacement.AssignFiltermode(materials[i]);
                    }
                    else
                    {
                        if (go == meshRenderer.gameObject)
                            Debug.LogWarningFormat("{0} is missing material {1}.", go.name, i.ToString());
                        else
                            Debug.LogWarningFormat("{0} (child {1}) is missing material {2}.", go.name, meshRenderer.name, i.ToString());
                    }
                }

                // Confirm finalised materials
                meshRenderer.sharedMaterials = materials;
            }
        }

        /// <summary>
        /// Set common settings for all trees.
        /// </summary>
        private static void SetTreesSettings(Terrain terrain)
        {
            terrain.treeDistance = 5000;
            terrain.treeBillboardDistance = 100;
            terrain.treeMaximumFullLODCount = 50;
            terrain.treeCrossFadeLength = 20;
        }

        private static void SetCallback<T>(ref T callback, T value)
        {
            if (value == null)
                throw new ArgumentNullException("callback", "This callback must not be set to null.");
            
            callback = value;
        }

        #endregion
    }
}