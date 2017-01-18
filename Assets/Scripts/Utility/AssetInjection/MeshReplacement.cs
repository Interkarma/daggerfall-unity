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

using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Handles import and injection of custom meshes and materials
    /// with the purpose of providing modding support.
    /// </summary>
    static public class DFMeshReplacement
    {
        #region asset-injection

        /// <summary>
        /// Check existence of model in Resources
        /// </summary>
        /// <returns>Bool</returns>

        // Models (mesh + materials)
        static public bool ReplacmentModelExist(uint modelID)
        {
            if (DaggerfallUnity.Settings.MeshAndTextureReplacement //check .ini setting
                 && Resources.Load("Models/" + modelID.ToString() + "/" + modelID.ToString()) != null)
                return true;

            return false;
        }

        // Models (prefabs)
        static public bool ReplacementPrefabExist(uint modelID)
        {
            if (DaggerfallUnity.Settings.MeshAndTextureReplacement //check .ini setting
                 && Resources.Load("Models/" + modelID.ToString() + "/" + modelID.ToString() + "_default") != null)
                return true;

            return false;
        }

        // Billboards
        static public bool ReplacementFlatExist(int archive, int record)
        {
            if (DaggerfallUnity.Settings.MeshAndTextureReplacement //check .ini setting
                 && Resources.Load("Flats/" + archive.ToString() + "_" + record.ToString() + "/" + archive.ToString() + "_" + record.ToString()) != null)
                return true;
      
            return false;
        }

        /// <summary>
        /// Import the model and the material(s) from Resources
        /// </summary>

        // Models (mesh + materials)
        static public Mesh LoadReplacementModel(uint modelID, ref CachedMaterial[] cachedMaterialsOut)
        {
            // Import mesh
            GameObject object3D = Resources.Load("Models/" + modelID.ToString() + "/" + modelID.ToString()) as GameObject;

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

        // Models (prefabs)
        static public void LoadReplacementPrefab(uint modelID, Vector3 position, Transform parent, Quaternion rotation)
        {
            // Import GameObject
            GameObject object3D = null;

            // If it's not winter or the model doesn't have a winter version, it loads default prefab
            if ((DaggerfallUnity.Instance.WorldTime.Now.SeasonValue != DaggerfallDateTime.Seasons.Winter) || (Resources.Load("Models/" + modelID.ToString() + "/" + modelID.ToString() + "_winter") == null))
                object3D = GameObject.Instantiate(Resources.Load("Models/" + modelID.ToString() + "/" + modelID.ToString() + "_default") as GameObject);
            // If it's winter and the model has a winter version, it loads winter prefab
            else
                object3D = GameObject.Instantiate(Resources.Load("Models/" + modelID.ToString() + "/" + modelID.ToString() + "_winter") as GameObject);

            // Position
            object3D.transform.parent = parent;
            object3D.transform.position = position;
            object3D.transform.rotation = rotation;

            FinaliseCustomGameObject(ref object3D, modelID.ToString());
        }

        // Billboards
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
        /// Assign texture filtermode as user settings and check integrity of materials
        /// </summary>
        static public void FinaliseCustomGameObject(ref GameObject object3D, string ModelName)
        {
            for (int i = 0; i < object3D.GetComponent<Renderer>().materials.Length; i++)
            {
                if (object3D.GetComponent<Renderer>().materials[i].mainTexture != null)
                    object3D.GetComponent<Renderer>().materials[i].mainTexture.filterMode = (FilterMode)DaggerfallUnity.Settings.MainFilterMode;
                else
                    Debug.LogError("Custom model " + ModelName + " is missing a material or a texture");
            }
        }

        #endregion
    }
}
