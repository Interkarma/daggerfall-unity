// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
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
 * - StreamingWorld
 */

using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Handles import and injection of custom meshes and materials with the purpose of providing modding support.
    /// This is done in the shape of a model->model replacement as wall as a 2d billboard->model replacement.
    /// </summary>
    static public class MeshReplacement
    {
        #region Public Methods

        /// <summary>
        /// Import the custom GameObject if available.
        /// </summary>
        /// <remarks>
        /// A GameObject which corresponds to <paramref name="modelID"/>
        /// is searched inside mods and eventually imported and returned. 
        /// On Development builds is also imported from Resources for easier tests.
        /// </remarks>
        /// <param name="matrix">Matrix with position and rotation of GameObject.</param>
        /// <returns>Returns the imported model or null.</returns>
        static public GameObject ImportCustomGameobject (uint modelID, Transform parent, Matrix4x4 matrix)
        {
            // Check user settings
            if (!DaggerfallUnity.Settings.MeshAndTextureReplacement)
                return null;

            // Get name
            string modelName = modelID.ToString();;
            string climateModelName = ClimateSeasonName(modelName);

#if DEVELOPMENT_BUILD

            // Import Gameobject from Resources
            string path = "Models/" + modelName + "/";
            if (Resources.Load(path + climateModelName) != null)
            {
                // Import model fo specific climate/season
                GameObject go = GameObject.Instantiate(Resources.Load(path + climateModelName) as GameObject);
                InstantiateCustomModel(go, parent, matrix, climateModelName);
                return go;
            }
            else if (Resources.Load(path + modelName) != null)
            {
                // Import generic model
                GameObject go = GameObject.Instantiate(Resources.Load(path + modelName) as GameObject);
                InstantiateCustomModel(go, parent, matrix, modelName);
                return go;
            }

#endif

            // Get model from mods using load order
            Mod[] mods = ModManager.Instance.GetAllMods(true);
            for (int i = mods.Length; i-- > 0;)
            {
                AssetBundle bundle = mods[i].AssetBundle;
                if (bundle)
                {
                    // Get Name
                    string name;
                    if (bundle.Contains(climateModelName))
                        name = climateModelName;
                    else if (bundle.Contains(modelName))
                        name = modelName;
                    else
                        continue;

                    // Import GameObject
                    GameObject go = mods[i].GetAsset<GameObject>(name, true);
                    if (go)
                    {
                        InstantiateCustomModel(go, parent, matrix, name);
                        return go;
                    }

                    Debug.LogErrorFormat("Failed to import {0} from {1} as GameObject.", climateModelName, mods[i].Title);
                }
            }

            return null;
        }

        /// <summary>
        /// Import the custom GameObject for billboard if available.
        /// </summary>
        /// <remarks>
        /// A GameObject which corresponds to <paramref name="archive"/> and <paramref name="record"/>
        /// is searched inside mods and eventually imported and returned. 
        /// On Development builds is also imported from Resources for easier tests.
        /// </remarks>
        /// <param name="inDungeon">Fix position for dungeon models.</param>
        /// <returns>Returns the imported model or null.</returns>
        static public GameObject ImportCustomFlatGameobject (int archive, int record, Vector3 position, Transform parent, bool inDungeon = false)
        {
            // Check user settings
            if (!DaggerfallUnity.Settings.MeshAndTextureReplacement)
                return null;

            // Get name
            string modelName = archive.ToString("D3") + "_" + record.ToString();

#if DEVELOPMENT_BUILD

            // Import Gameobject from Resources
            string path = "Flats/" + modelName + "/";
            if (Resources.Load(path + modelName) != null)
            {
                GameObject go = GameObject.Instantiate(Resources.Load(path + modelName) as GameObject);
                InstantiateCustomFlat(go, ref position, parent, archive, record, inDungeon);
                return go;
            }

#endif

            // Get model from mods using load order
            Mod[] mods = ModManager.Instance.GetAllMods(true);
            for (int i = mods.Length; i-- > 0;)
            {
                AssetBundle bundle = mods[i].AssetBundle;
                if (bundle && bundle.Contains(modelName))
                {
                    GameObject go = mods[i].GetAsset<GameObject>(modelName, true);
                    if (go != null)
                    {
                        InstantiateCustomFlat(go, ref position, parent, archive, record, inDungeon);
                        return go;
                    }

                    Debug.LogError("Failed to import " + modelName + " from " + mods[i].Title + " as GameObject.");
                }
            }

            return null;
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
        /// Assign parent, position, rotation and texture filtermode.
        /// </summary>
        static private void InstantiateCustomModel(GameObject go, Transform parent, Matrix4x4 matrix, string modelName)
        {
            // Assign transform properties
            go.transform.parent = parent;
            go.transform.position = matrix.GetColumn(3);
            go.transform.rotation = GameObjectHelper.QuaternionFromMatrix(matrix);

            // Assign name
            go.name = string.Format("DaggerfallMesh[Replacement][ID ={0}]", modelName);

            // Finalise gameobject
            FinaliseMaterials(go);
        }

        /// <summary>
        /// Assign parent, position, rotation and texture filtermode.
        /// </summary>
        static private void InstantiateCustomFlat(GameObject go, ref Vector3 position, Transform parent, int archive, int record, bool inDungeon)
        {
            // Fix origin position for dungeon flats
            if (inDungeon)
            {
                // Get height
                int height = ImageReader.GetImageData("TEXTURE." + archive.ToString("D3"), record, createTexture: false).height;

                // Correct transform
                position.y -= height / 2 * MeshReader.GlobalScale;
            }

            // Assign transform properties
            go.transform.parent = parent;
            go.transform.localPosition = position;

            // Assign a random rotation so that flats in group won't look all aligned.
            // We use a seed becuse we want the models to have the same
            // rotation every time the same location is loaded.
            if (go.GetComponent<FaceWall>() == null)
            {
                UnityEngine.Random.InitState((int)position.x);
                go.transform.Rotate(0, UnityEngine.Random.Range(0f, 360f), 0);
            }

            // Assign name
            go.name = string.Format("DaggerfallBillboard [Replacement] [TEXTURE.{0:000}, Index={1}]", archive, record);

            // Finalise gameobject material
            FinaliseMaterials(go);

            // Add NPC trigger collider
            if (RDBLayout.IsNPCFlat(archive))
            {
                Collider col = go.AddComponent<BoxCollider>();
                col.isTrigger = true;
            }
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
                Material[] materials = meshRenderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i].mainTexture != null)
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
                        materials[i].mainTexture.filterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode;
                        if (materials[i].GetTexture("_BumpMap") != null)
                            materials[i].GetTexture("_BumpMap").filterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode;
                        if (materials[i].GetTexture("_EmissionMap") != null)
                            materials[i].GetTexture("_EmissionMap").filterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode;
                        if (materials[i].GetTexture("_MetallicGlossMap") != null)
                            materials[i].GetTexture("_MetallicGlossMap").filterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode;
                    }
                    else
                        Debug.LogError(string.Format("{0} is missing a material or a texture.", object3D.name));
                }

                // Confirm finalised materials
                meshRenderer.materials = materials;
            }
        }

        #endregion

        #region Legacy Methods

        // This was used to import mesh and materials separately from Resources.
        // It might be useful again in the future.
        //
        //static public Mesh LoadReplacementModel(uint modelID, ref CachedMaterial[] cachedMaterialsOut)
        //{
        //    // Import mesh
        //    GameObject object3D = Resources.Load("Models/" + modelID.ToString() + "/" + modelID.ToString() + "_mesh") as GameObject;
        //
        //    // Import materials
        //    cachedMaterialsOut = new CachedMaterial[object3D.GetComponent<MeshRenderer>().sharedMaterials.Length];
        //
        //    string materialPath;
        //
        //    // If it's not winter or the model doesn't have a winter version, it loads default materials
        //    if ((DaggerfallUnity.Instance.WorldTime.Now.SeasonValue != DaggerfallDateTime.Seasons.Winter) || (Resources.Load("Models/" + modelID.ToString() + "/material_w_0") == null))
        //        materialPath = "/material_";
        //    // If it's winter and the model has a winter version, it loads winter materials
        //   else
        //        materialPath = "/material_w_";
        //
        //    for (int i = 0; i < cachedMaterialsOut.Length; i++)
        //    {
        //        if (Resources.Load("Models/" + modelID.ToString() + materialPath + i) != null)
        //        {
        //            cachedMaterialsOut[i].material = Resources.Load("Models/" + modelID.ToString() + materialPath + i) as Material;
        //            if (cachedMaterialsOut[i].material.mainTexture != null)
        //                cachedMaterialsOut[i].material.mainTexture.filterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode; //assign texture filtermode as user settings
        //            else
        //                Debug.LogError("Custom model " + modelID + " is missing a texture");
        //        }
        //        else
        //            Debug.LogError("Custom model " + modelID + " is missing a material");
        //    }
        //
        //    return object3D.GetComponent<MeshFilter>().sharedMesh;
        //}
        //

        #endregion
    }
}