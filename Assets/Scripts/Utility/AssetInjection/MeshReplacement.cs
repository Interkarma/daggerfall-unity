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
 * - StreamingWorld
 */

using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Handles import and injection of custom meshes and materials
    /// with the purpose of providing modding support.
    /// </summary>
    static public class MeshReplacement
    {
        // Winter tag
        const string winterTag = "_winter";

        #region Public Methods

        /// <summary>
        /// Import the custom GameObject if available
        /// </summary>
        /// <returns>Returns the imported model or null.</returns>
        static public GameObject ImportCustomGameobject (uint modelID, Vector3 position, Transform parent, Quaternion rotation)
        {
            // Check user settings
            if (!DaggerfallUnity.Settings.MeshAndTextureReplacement)
                return null;

            // Get name
            string modelName = modelID.ToString();

            // Import Gameobject from Resources
            // This is useful to test models
            string path = "Models/" + modelName + "/";
            if (Resources.Load(path + modelName) != null)
            {
                // Assign the name according to the current season
                if (IsWinter())
                {
                    if (Resources.Load(path + modelName + winterTag) != null)
                        modelName += winterTag;
                }

                // Import GameObject
                GameObject go = GameObject.Instantiate(Resources.Load(path + modelName) as GameObject);
                InstantiateCustomModel(go, ref position, parent, ref rotation, modelName);
                return go;
            }

            // Get model from mods using load order
            Mod[] mods = ModManager.Instance.GetAllMods(true);
            for (int i = mods.Length; i-- > 0;)
            {
                if (mods[i].AssetBundle.Contains(modelName))
                {
                    // Assign the name according to the current season
                    if (IsWinter())
                    {
                        if (mods[i].AssetBundle.Contains(modelName + winterTag))
                            modelName += winterTag;
                    }

                    GameObject go = mods[i].GetAsset<GameObject>(modelName, true);
                    if (go != null)
                    {
                        InstantiateCustomModel(go, ref position, parent, ref rotation, modelName);
                        return go;
                    }

                    Debug.LogError("Failed to import " + modelName + " from " + mods[i].Title + " as GameObject.");
                }
            }

            return null;
        }

        /// <summary>
        /// Import the custom GameObject for billboard if available
        /// </summary>
        /// <param name="inDungeon">Fix position for dungeon models.</param>
        /// <returns>Returns the imported model or null.</returns>
        static public GameObject ImportCustomFlatGameobject (int archive, int record, Vector3 position, Transform parent, bool inDungeon = false)
        {
            // Check user settings
            if (!DaggerfallUnity.Settings.MeshAndTextureReplacement)
                return null;

            // Get name
            string modelName = archive.ToString("D3") + "_" + record.ToString();

            // Import Gameobject from Resources
            // This is useful to test models
            string path = "Flats/" + modelName + "/";
            if (Resources.Load(path + modelName) != null)
            {
                GameObject go = GameObject.Instantiate(Resources.Load(path + modelName) as GameObject);
                InstantiateCustomFlat(go, ref position, parent, archive, record, inDungeon);
                return go;
            }

            // Get model from mods using load order
            Mod[] mods = ModManager.Instance.GetAllMods(true);
            for (int i = mods.Length; i-- > 0;)
            {
                if (mods[i].AssetBundle.Contains(modelName))
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
        
        /// <summary>
        /// Checks if dungeon flat should have a torch sound.
        /// </summary>
        static public bool HasTorchSound (int archive, int record)
        {  
            if (archive == TextureReader.LightsTextureArchive)
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
                        return true;
                }
            }

            return false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Check if is winter and we are in a location which supports winter models.
        /// </summary>
        static private bool IsWinter()
        {
            return ((GameManager.Instance.PlayerGPS.ClimateSettings.ClimateType != DaggerfallConnect.DFLocation.ClimateBaseType.Desert) 
                && (DaggerfallUnity.Instance.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter));
        }

        /// <summary>
        /// Assign parent, position, rotation and texture filtermode.
        /// </summary>
        static private void InstantiateCustomModel(GameObject go, ref Vector3 position, Transform parent, ref Quaternion rotation, string modelName)
        {
            // Update Position
            go.transform.parent = parent;
            go.transform.position = position;
            go.transform.rotation = rotation;

            // Finalise gameobject
            FinaliseMaterials(go, modelName);
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

            // Update Position
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

            // Finalise gameobject material
            FinaliseMaterials(go, archive.ToString("D3") + "_" + record.ToString());
        }

        /// <summary>
        /// Assign texture filtermode as user settings and check integrity of materials.
        /// Import textures from disk if available. This is for consistency when custom models
        /// use vanilla textures (or improved versions of them) and the user has installed a texture pack.
        /// </summary>
        /// <param name="object3D">Custom prefab</param>
        /// <param name="ModelName">ID of model or sprite to be replaced. Used for debugging</param>
        static private void FinaliseMaterials(GameObject object3D, string ModelName)
        {
            // Get MaterialReader
            MaterialReader materialReader = DaggerfallUnity.Instance.MaterialReader;

            // Get materials
            MeshRenderer meshRenderer = object3D.GetComponent<MeshRenderer>();
            Material[] materials = meshRenderer.materials;

            // Check all materials
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
                    Debug.LogError("Custom model " + ModelName + " is missing a material or a texture");
            }

            // Confirm finalised materials
            meshRenderer.materials = materials;
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