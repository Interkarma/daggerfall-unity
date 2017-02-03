// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
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
 * 1. Action models in RDB
 * 2. Optimize and improve AssetBundle import
 */

using System.IO;
using UnityEngine;
using System.Collections;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Handles import and injection of custom meshes and materials
    /// with the purpose of providing modding support.
    /// </summary>
    static public class MeshReplacement
    {
        #region asset-injection

        /// <summary>
        /// Check existence of model in Resources
        /// Legacy: this can eventually be removed
        /// </summary>
        /// <returns>Bool</returns>
        static public bool ReplacmentModelExist(uint modelID)
        {
            if (DaggerfallUnity.Settings.MeshAndTextureReplacement //check .ini setting
                 && Resources.Load("Models/" + modelID.ToString() + "/" + modelID.ToString() + "_mesh") != null)
                return true;
      
            return false;
        }

        /// <summary>
        /// Import the model and the material(s) from Resources
        /// </summary>
        static public Mesh LoadReplacementModel(uint modelID, ref CachedMaterial[] cachedMaterialsOut)
        {
            // Import mesh
            GameObject object3D = Resources.Load("Models/" + modelID.ToString() + "/" + modelID.ToString() + "_mesh") as GameObject;
 
            // Import materials
            cachedMaterialsOut = new CachedMaterial[object3D.GetComponent<MeshRenderer>().sharedMaterials.Length];

            string materialPath;

            // If it's not winter or the model doesn't have a winter version, it loads default materials
            if ((DaggerfallUnity.Instance.WorldTime.Now.SeasonValue != DaggerfallDateTime.Seasons.Winter) || (Resources.Load("Models/" + modelID.ToString() + "/material_w_0") == null))
                materialPath = "/material_";
            // If it's winter and the model has a winter version, it loads winter materials
            else
                materialPath = "/material_w_";

            for (int i = 0; i < cachedMaterialsOut.Length; i++)
            {
                if (Resources.Load("Models/" + modelID.ToString() + materialPath + i) != null)
                {
                    cachedMaterialsOut[i].material = Resources.Load("Models/" + modelID.ToString() + materialPath + i) as Material;
                    if (cachedMaterialsOut[i].material.mainTexture != null)
                        cachedMaterialsOut[i].material.mainTexture.filterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode; //assign texture filtermode as user settings
                    else
                        Debug.LogError("Custom model " + modelID + " is missing a texture");
                }
                else
                    Debug.LogError("Custom model " + modelID + " is missing a material");
            }

            return object3D.GetComponent<MeshFilter>().sharedMesh;
        }
  
        /// <summary>
        /// Import the custom GameObject if available
        /// Assetbundles should be created using the Mod Builder inside the Daggerfall Tools
        /// TODO: Integrate with the mod system to import models from mods using load order
        /// </summary>
        static public void ImportCustomGameobject (uint modelID, Vector3 position, Transform parent, Quaternion rotation, out bool modelExist)
        {
            // Check user settings
            if (!DaggerfallUnity.Settings.MeshAndTextureReplacement)
            {
                modelExist = false;
                return;
            }
 
            // Import Gameobject from Resources
            // This is useful to test models
            if (ReplacementPrefabExist(modelID))
            {
                LoadReplacementPrefab(modelID, position, parent, rotation);
                modelExist = true;
                return;
            }
 
            // Load AssetBundle
            string modelsPath = Path.Combine(Application.streamingAssetsPath, "Models");
            string modelName = modelID.ToString();
            modelsPath = Path.Combine(modelsPath, modelName + ".model");
            if (!File.Exists(modelsPath))
            {
                modelExist = false;
                return;
            }
            var LoadedAssetBundle = AssetBundle.LoadFromFile(modelsPath);
            if (LoadedAssetBundle == null)
            {
                Debug.LogError("Failed to load AssetBundle " + modelName + ".model");
                modelExist = false;
                return;
            }
 
            // Check if AssetBundle contain the model we are looking for and assign the name according to the current season
            if (!LoadedAssetBundle.Contains(modelName))
            {
                Debug.LogError("AssetBundle " + modelName + ".model is invalid");
                modelExist = false;
                LoadedAssetBundle.Unload(false);
                return;
            }
            else if ((DaggerfallUnity.Instance.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter) && (LoadedAssetBundle.Contains(modelName + "_winter")))
                modelName += "_winter";
 
            // Instantiate GameObject
            modelExist = true;
            GameObject object3D = GameObject.Instantiate(LoadedAssetBundle.LoadAsset<GameObject>(modelName));
            LoadedAssetBundle.Unload(false);
 
            // Update Position
            object3D.transform.parent = parent;
            object3D.transform.position = position;
            object3D.transform.rotation = rotation;
 
            // Finalise gameobject
            FinaliseCustomGameObject(ref object3D, modelName);
        }
        
        /// <summary>
        /// Check existence of gameobject in Resources
        /// </summary>
        /// <returns>Bool</returns>
        static public bool ReplacementPrefabExist(uint modelID)
        {
            if (DaggerfallUnity.Settings.MeshAndTextureReplacement //check .ini setting
                 && Resources.Load("Models/" + modelID.ToString() + "/" + modelID.ToString()) != null)
                return true;
 
            return false;
        }
        
        /// <summary>
        /// Import gameobject from Resources
        /// </summary>
        static public void LoadReplacementPrefab(uint modelID, Vector3 position, Transform parent, Quaternion rotation)
        {
            // Import GameObject
            GameObject object3D = null;

            // If it's not winter or the model doesn't have a winter version, it loads default prefab
            if ((DaggerfallUnity.Instance.WorldTime.Now.SeasonValue != DaggerfallDateTime.Seasons.Winter) || (Resources.Load("Models/" + modelID.ToString() + "/" + modelID.ToString() + "_winter") == null))
                object3D = GameObject.Instantiate(Resources.Load("Models/" + modelID.ToString() + "/" + modelID.ToString()) as GameObject);
            // If it's winter and the model has a winter version, it loads winter prefab
            else
                object3D = GameObject.Instantiate(Resources.Load("Models/" + modelID.ToString() + "/" + modelID.ToString() + "_winter") as GameObject);

            // Position
            object3D.transform.parent = parent;
            object3D.transform.position = position;
            object3D.transform.rotation = rotation;

            FinaliseCustomGameObject(ref object3D, modelID.ToString());
        }

        /// <summary>
        /// Import the custom GameObject for billboard if available
        /// Assetbundles should be created using the Mod Builder inside the Daggerfall Tools
        /// TODO: Integrate with the mod system to import models from mods using load order
        /// </summary>
        static public void ImportCustomFlatGameobject (int archive, int record, Vector3 position, Transform parent, out bool modelExist)
        {
            // Check user settings
            if (!DaggerfallUnity.Settings.MeshAndTextureReplacement)
            {
                modelExist = false;
                return;
            }

            // Import Gameobject from Resources
            // This is useful to test models
            if (ReplacementFlatExist(archive, record)) 
            {
                LoadReplacementFlat(archive, record, position, parent);
                modelExist = true;
                return;
            }

            // Load AssetBundle
            string modelsPath = Path.Combine(Application.streamingAssetsPath, "Flats");
            string modelName = archive.ToString() + "_" + record.ToString();
            modelsPath = Path.Combine(modelsPath, modelName + ".flat");
            if (!File.Exists(modelsPath))
            {
                modelExist = false;
                return;
            }
            var LoadedAssetBundle = AssetBundle.LoadFromFile(modelsPath);
            if (LoadedAssetBundle == null)
            {
                Debug.LogError("Failed to load AssetBundle " + modelName + ".flat");
                modelExist = false;
                return;
            }
 
            // Check if AssetBundle contain the model we are looking for
            if (!LoadedAssetBundle.Contains(modelName))
            {
                Debug.LogError("AssetBundle " + modelName + ".flat is invalid");
                modelExist = false;
                LoadedAssetBundle.Unload(false);
                return;
            }

            // Instantiate GameObject
            modelExist = true;
            GameObject object3D = GameObject.Instantiate(LoadedAssetBundle.LoadAsset<GameObject>(modelName));
            LoadedAssetBundle.Unload(false);

            // Update Position
            object3D.transform.parent = parent;
            object3D.transform.localPosition = position;
            
            // Finalise gameobject
            FinaliseCustomGameObject(ref object3D, modelName);
        }
        
        /// <summary>
        /// Check existence of gameobject for billboard in Resources
        /// </summary>
        /// <returns>Bool</returns>
        static public bool ReplacementFlatExist(int archive, int record)
        {
            if (DaggerfallUnity.Settings.MeshAndTextureReplacement //check .ini setting
                 && Resources.Load("Flats/" + archive.ToString() + "_" + record.ToString() + "/" + archive.ToString() + "_" + record.ToString()) != null)
                return true;
      
            return false;
        }
        
        /// <summary>
        /// Import gameobject from Resources
        /// </summary>
        static public void LoadReplacementFlat(int archive, int record, Vector3 position, Transform parent)
        {
            // Import GameObject
            GameObject object3D = GameObject.Instantiate(Resources.Load("Flats/" + archive.ToString() + "_" + record.ToString() + "/" + archive.ToString() + "_" + record.ToString()) as GameObject);
 
            // Position
            object3D.transform.parent = parent;
            object3D.transform.localPosition = position;
 
            FinaliseCustomGameObject(ref object3D, archive.ToString() + "_" + record.ToString());
        }
 
        #endregion

        #region Utilities

        /// <summary>
        /// Assign texture filtermode as user settings and check integrity of materials.
        /// Import textures from disk if available. This is for consistency when custom models
        /// use one or more vanilla textures and the user has installed a texture pack.
        /// Vanilla textures should follow the nomenclature (archive_record-frame.png) while unique
        /// textures should have unique names (MyModelPack_WoodChair2.png) to avoid involuntary replacements.
        /// This can also be used to import optional normal maps.
        /// </summary>
        /// <param name="object3D">Custom prefab</param>
        /// <param name="ModelName">ID of model or sprite to be replaced. Used for debugging</param>
        static public void FinaliseCustomGameObject(ref GameObject object3D, string ModelName)
        {
            // Get MeshRenderer
            MeshRenderer meshRenderer = object3D.GetComponent<MeshRenderer>();

            // Check all materials
            for (int i = 0; i < meshRenderer.materials.Length; i++)
            {
                if (meshRenderer.materials[i].mainTexture != null)
                {
                    // Get name of texture
                    string textureName = meshRenderer.materials[i].mainTexture.name;

                    // Use texture(s) from disk if available
                    // Albedo
                    if (TextureReplacement.CustomTextureExist(textureName))
                        meshRenderer.materials[i].mainTexture = TextureReplacement.LoadCustomTexture(textureName);
                    // Normal map
                    if (TextureReplacement.CustomNormalExist(textureName))
                    {
                        meshRenderer.materials[i].EnableKeyword("_NORMALMAP"); // Enable normal map in the shader if the original material doesn't have one
                        meshRenderer.materials[i].SetTexture("_BumpMap", TextureReplacement.LoadCustomNormal(textureName));
                    }

                    // Assign filtermode
                    meshRenderer.materials[i].mainTexture.filterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode;
                    if (meshRenderer.materials[i].GetTexture("_BumpMap") != null)
                        meshRenderer.materials[i].GetTexture("_BumpMap").filterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode;
                }
                else
                    Debug.LogError("Custom model " + ModelName + " is missing a material or a texture");
            }
        }

        #endregion
    }
}