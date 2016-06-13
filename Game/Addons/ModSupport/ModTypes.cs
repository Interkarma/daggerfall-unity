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
using System.Collections;
using System.Collections.Generic;


namespace DaggerfallWorkshop.Game.Utility.ModSupport
{
    public interface IModController
    {
        string ModName { get;}
        bool IsDisableable { get;}
        void ShowControllerUIWindow();
    }

    //loaded asset - used for lookups w/ mods
    public struct LoadedAsset
    {
        public Type T;
        public UnityEngine.Object Obj;

        public LoadedAsset(Type T, UnityEngine.Object Obj)
        {
            this.T = T;
            this.Obj = Obj;
        }
    }


    //created with mod builder window, seralized to json, bundled into mod
    [System.Serializable]
    public class ModInfo
    {
        public string ModFileName;      //Must be lowercase
        public string ModTitle;         //displayed in game
        public string ModVersion;
        public string ModAuthor;
        public string ContactInfo;
        public string DFUnity_Verion;
        public string ModDescription;
        public List<string> Files;      //list of assets to add to mod (only used during creation)

        public ModInfo()
        {
            if(Application.isEditor)
                Files = new List<string>();

            ModFileName         = "";
        }
    }

    public struct SetupOptions
    {
        public string targetName;
        public Mod mod;
        public Type T;
    }

    //passed to mod's Init methods called from ModManager
    public struct InitParams
    {
        public string ModTitle;
        public int ModIndex;
        public int LoadPriority;
        public int LoadedModsCount;

        public InitParams(string ModTitle, int ModIndex, int LoadPriority, int LoadedModsCount)
        {
            this.ModTitle = ModTitle;
            this.ModIndex = ModIndex;
            this.LoadPriority = LoadPriority;
            this.LoadedModsCount = LoadedModsCount;
        }

    }

    public struct Source
    {
        public TextAsset sourceTxt;
        public bool isPreCompiled;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class Load : System.Attribute
    {
        public string targetName;            //name of static method
        public Load(string targetName = "Init")
        {
            this.targetName = targetName;
        }
    }


}
