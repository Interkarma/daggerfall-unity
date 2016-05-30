// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyl@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections.Generic;

namespace DaggerfallWorkshop.Game.Utility.ModSupport
{
    //TODO - implement asynch asset loading

    [Serializable]
    public class Mod
    {
        public bool Enabled = true;
        public ModInfo ModInfo;                                 //modname.dfmod.json file loaded from inside the bundle
        public AssetBundle AssetBundle;                         //.dfmod file
        public string DirPath;                                  //directory the mod file is in
        public Source[] Sources;                                //any source code found in asset bundle
        public List<System.Reflection.Assembly> Assemblies;     //compiled source code for this mod
        public Dictionary<string, LoadedAsset> LoadedAssests;
        public IModController ModController;                   //registered mod controller for this mod

        public Mod()
        {
            LoadedAssests = new Dictionary<string, LoadedAsset>();
            Assemblies = new List<System.Reflection.Assembly>(1);
        }


        /// <summary>
        /// Load Asset from bundle, creates LoadedAsset struct and adds to list of loaded assests
        /// </summary>
        /// <typeparam name="T">Type of Asset</typeparam>
        /// <param name="assetName">Name of Assest to load</param>
        /// <param name="la">LoadedAssest struct</param>
        /// <returns>bool</returns>
        private bool LoadAssetFromBundle<T>(string assetName, out LoadedAsset la)
        {
            la = new LoadedAsset();

            if (AssetBundle == null || string.IsNullOrEmpty(assetName))
            {
                return false;
            }

            try
            {
                if (LoadedAssests.ContainsKey(assetName))
                {
                    la = LoadedAssests[assetName];
                    return true;
                }
                else if (AssetBundle.Contains(assetName))
                {
                    la.Obj = AssetBundle.LoadAsset(assetName);
                    la.T = typeof(T);
                }
                else
                    return false;

                if (la.Obj != null && la.T != null)
                {
                    LoadedAssests.Add(assetName, la);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return false;
            }

        }


        /// <summary>
        /// Load asset from bundle and return
        /// </summary>
        /// <typeparam name="T">Type of Asset</typeparam>
        /// <param name="assetName">name of asset to load</param>
        /// <param name="makeCopy">create copy of asset if true</param>
        /// <returns>UnityEngine.Object</returns>
        public T GetAssetFromLoadedBundle<T>(string assetName, bool makeCopy = true) where T : UnityEngine.Object
        {
            //T asset = default(T);
            LoadedAsset la;

            if (!LoadAssetFromBundle<T>(assetName, out la))
            {
                Debug.LogWarning(string.Format("Failed to load asset: {0} from mod: {1}", assetName, ModInfo.ModName));
                return null;
            }
            else if (makeCopy)
            {
                return UnityEngine.Object.Instantiate(la.Obj) as T;
            }
            else
                return la.Obj as T;
        }

        /// <summary>
        /// Find Type from Assemblies
        /// </summary>
        /// <param name="type">name of Type</param>
        /// <returns>System.Type</returns>
        public Type GetCompiledType(string type)
        {
            Type t = null;

            if (string.IsNullOrEmpty(type))
                return null;

            for (int i = 0; i < Assemblies.Count; i++)
            {
                if (Assemblies[i] == null)
                    continue;

                t = Assemblies[i].GetType(type);
                if (t != null)
                    return t;
            }

            return t;
        }
    }

}
