// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Uncanny_Valley
// Contributors:    TheLacus 
// 
// Notes:
//

using System.Collections.Generic;
using UnityEngine;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Handles import and injection of custom meshes and materials with the purpose of providing modding support.
    /// This is done in the shape of a model->model replacement as wall as a 2d billboard->model replacement.
    /// </summary>
    public static class MeshReplacement
    {
        #region Public Methods

        /// <summary>
        /// Import a GameObject from mods.
        /// </summary>
        /// <param name="matrix">Matrix with position and rotation of GameObject.</param>
        /// <returns>Returns the imported model or null.</returns>
        public static GameObject ImportCustomGameobject (uint modelID, Transform parent, Matrix4x4 matrix)
        {
            // Check user settings
            if (!DaggerfallUnity.Settings.MeshAndTextureReplacement)
                return null;

            // Get names
            string modelName = modelID.ToString();;
            string climateModelName = ClimateSeasonName(modelName);

            // Get GameObject
            GameObject prefab;
            if (!LoadGameObjectFromMods("Models", climateModelName, modelName, out prefab))
                return null;

            // Instantiate GameObject
            GameObject go = GameObject.Instantiate(prefab, parent);
            go.name = string.Format("DaggerfallMesh[Replacement][ID ={0}]", modelName);
            go.transform.position = matrix.GetColumn(3);
            go.transform.rotation = GameObjectHelper.QuaternionFromMatrix(matrix);

            // Finalise gameobject
            FinaliseMaterials(go);
            return go;
        }

        /// <summary>
        /// Import a GameObject from mods instead of billboard.
        /// </summary>
        /// <param name="inDungeon">Fix position for dungeon models.</param>
        /// <returns>Returns the imported model or null.</returns>
        public static GameObject ImportCustomFlatGameobject (int archive, int record, Vector3 position, Transform parent, bool inDungeon = false)
        {
            // Check user settings
            if (!DaggerfallUnity.Settings.MeshAndTextureReplacement)
                return null;

            // Get GameObject
            GameObject prefab;
            string name = archive.ToString("D3") + "_" + record.ToString();
            if (!LoadGameObjectFromMods("Flats", name, out prefab))
                return null;

            // Instantiate GameObject
            GameObject go = GameObject.Instantiate(prefab, parent);
            go.name = string.Format("DaggerfallBillboard [Replacement] [TEXTURE.{0:000}, Index={1}]", archive, record);

            // Assign position
            if (inDungeon)
            {
                // Fix origin position for dungeon flats
                int height = ImageReader.GetImageData("TEXTURE." + archive.ToString("D3"), record, createTexture: false).height;
                position.y -= height / 2 * MeshReader.GlobalScale;
            }
            go.transform.localPosition = position;

            // Assign a random rotation
            if (go.GetComponent<FaceWall>() == null)
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
        /// Import a gameobject from mods to use with the terrain system.
        /// </summary>
        /// <returns>True if gameobject is found and imported.</returns>
        public static bool ImportNatureGameObject(int archive, int record, Terrain terrain, int x, int y)
        {
            const int tilemapDim = MapsFile.WorldMapTileDim - 1;

            // Check user settings
            if (!DaggerfallUnity.Settings.MeshAndTextureReplacement)
                return false;

            // Get prototype
            GameObject prefab;
            string name = archive.ToString("D3") + "_" + record.ToString();
            if (!LoadGameObjectFromMods("Flats", name, out prefab))
                return false;

            // Get instance properties
            TerrainData terrainData = terrain.terrainData;
            Vector3 position = new Vector3(x / (float)tilemapDim, 0.0f, y / (float)tilemapDim);
            float rotation = Random.Range(0f, 360f);
            int index = GetTreePrototypeIndex(terrainData, prefab);

            // Add tree instance
            TreeInstance treeInstance = new TreeInstance()
            {
                heightScale = 1,
                widthScale = 1,
                color = Color.grey,
                lightmapColor = Color.grey,
                position = position,
                rotation = rotation,
                prototypeIndex = index
            };
            terrain.AddTreeInstance(treeInstance);

            return true;
        }

        /// <summary>
        /// Remove all tree instances from terrain.
        /// </summary>
        public static void ClearNatureGameObjects(Terrain terrain)
        {
            if (!DaggerfallUnity.Settings.MeshAndTextureReplacement)
                return;

            TerrainData terrainData = terrain.terrainData;
            if (terrainData.treeInstanceCount > 0)
            {
                terrainData.treeInstances = new TreeInstance[0];
                terrainData.treePrototypes = new TreePrototype[0];
                terrainData.RefreshPrototypes();
                terrain.Flush();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get model name for current climate and season.
        /// </summary>
        private static string ClimateSeasonName(string modelName)
        {
            if (!GameManager.HasInstance)
                return modelName;

            // Get climate
            ClimateBases climateBase = ClimateSwaps.FromAPIClimateBase(GameManager.Instance.PlayerGPS.ClimateSettings.ClimateType);
            string name = modelName + "_" + climateBase.ToString();

            // Get season
            if (climateBase != ClimateBases.Desert)
            {
                if (DaggerfallUnity.Instance.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter)
                    name += "Winter";
                else
                    name += "Summer";
            } 
                
            return name;
        }

        /// <summary>
        /// Search and import a GameObject from mods.
        /// In editor and dev builds is also imported from resources for easy testing.
        /// </summary>
        private static bool LoadGameObjectFromMods(string folder, string name, out GameObject go)
        {
            return LoadGameObjectFromMods(folder, name, null, out go);
        }

        /// <summary>
        /// Search and import a GameObject from mods.
        /// In editor and dev builds is also imported from resources for easy testing.
        /// </summary>
        private static bool LoadGameObjectFromMods(string folder, string name, string fallbackName, out GameObject go)
        {

#if UNITY_EDITOR || DEVELOPMENT_BUILD

            // Import Gameobject from Resources
            string path = folder + "/" + name + "/";
            if (Resources.Load(path + name))
            {
                go = Resources.Load(path + name) as GameObject;
                return true;
            }
            else if (fallbackName != null && Resources.Load(path + fallbackName))
            {
                go = Resources.Load(path + fallbackName) as GameObject;
                return true;
            }

#endif
#if !UNITY_EDITOR

            // Get model from mods using load order
            Mod[] mods = ModManager.Instance.GetAllMods(true);
            for (int i = mods.Length; i-- > 0;)
            {
                AssetBundle bundle = mods[i].AssetBundle;
                if (bundle)
                {
                    if (bundle.Contains(name))
                    {
                        go = mods[i].GetAsset<GameObject>(name);
                        return true;
                    }
                    else if (fallbackName != null && bundle.Contains(fallbackName))
                    {
                        go = mods[i].GetAsset<GameObject>(fallbackName);
                        return true;
                    }
                }
            }

#endif

            go = null;
            return false;
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
        /// Assign texture filtermode as user settings and check integrity of materials.
        /// Import textures from disk if available. This is for consistency when custom models
        /// use vanilla textures (or improved versions of them) and the user has installed a texture pack.
        /// </summary>
        /// <param name="object3D">Custom prefab</param>
        static private void FinaliseMaterials(GameObject object3D)
        {
            // Get MaterialReader
            MaterialReader materialReader = DaggerfallUnity.Instance.MaterialReader;

            // Check all MeshRenderers
            MeshRenderer[] meshRenderers = object3D.GetComponentsInChildren<MeshRenderer>();
            foreach (var meshRenderer in meshRenderers)
            {
                // Check all materials
                Material[] materials = meshRenderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (!materials[i])
                    {
                        if (object3D == meshRenderer.gameObject)
                            Debug.LogErrorFormat("{0} is missing material {1}.", object3D.name, i.ToString());
                        else
                            Debug.LogErrorFormat("{0} (child {1}) is missing material {2}.", object3D.name, meshRenderer.name, i.ToString());

                        continue;
                    }

                    if (materials[i].mainTexture)
                    {
                        // Get name of texture
                        string textureName = materials[i].mainTexture.name;

                        // Textures inside StreamingAssets/Textures have the priority 
                        // over the material included inside the assetbundle.
                        if (TextureReplacement.CustomTextureExist(textureName))
                        {
                            // Check that is a daggerfall texture
                            // These textures should follow the nomenclature (archive_record-frame.png) while unique
                            // textures should have unique names (MyModelPack_WoodChair2.png) to avoid involuntary replacements.
                            int archive, record;
                            if (TextureReplacement.IsDaggerfallTexture(textureName, out archive, out record))
                            {
                                // Use material from Daggerfall Unity cache 
                                CachedMaterial cachedMaterialOut;
                                if (materialReader.GetCachedMaterial(archive, record, 0, out cachedMaterialOut))
                                {
                                    materials[i] = cachedMaterialOut.material;
                                    continue;
                                }
                            }
                        }

                        // Assign filtermode
                        FilterMode filterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode;
                        materials[i].mainTexture.filterMode = filterMode;

                        if (materials[i].HasProperty("_BumpMap") && materials[i].GetTexture("_BumpMap"))
                            materials[i].GetTexture("_BumpMap").filterMode = filterMode;

                        if (materials[i].HasProperty("_EmissionMap") && materials[i].GetTexture("_EmissionMap"))
                            materials[i].GetTexture("_EmissionMap").filterMode = filterMode;

                        if (materials[i].HasProperty("_MetallicGlossMap") && materials[i].GetTexture("_MetallicGlossMap"))
                            materials[i].GetTexture("_MetallicGlossMap").filterMode = filterMode;
                    }
                }

                // Confirm finalised materials
                meshRenderer.sharedMaterials = materials;
            }
        }

        #endregion
    }
}