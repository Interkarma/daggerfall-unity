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
        public string ModName;      //Must be lowercase, must match filename
        public string ModTitle;     //displayed in game
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
        }
    }



    public enum SetupType
    {
        None,                       //do not instantiate - default
        Non_MonoBehaviour,          //not a monobehaviour, don't need to create / find / load a scene object
        Component_ModManager,       //add as comp. of mod manager, default for mod controllers
        Component_Player,           //add as comp. of player object
        Component_Camera,           //add as a comp. of camera object
        Component_DaggerfallUnity,  //add as a comp. of daggerfallunity object
        Component_GameManager,      //add as a comp of gameManager object
        Component_ByName,           //ads as a comp. of an object found by name, using objName as target (must be spelled exactly & in scene!)
        Component_Prefab,           //ads as a comp. to a clone from prefab retrieved from the mod's asset bundle 
        Prefab_Load,                //load a prefab from mod asset bundle, can be used by non-monobehaviours as well
        NewObject,                  //create a new game object for monoBehaviours, can be used by non-monobehaviours as well.
    }

    public enum SetupState
    {
        None,
        MenuState,                  //will be created when the main menu loads, or any time after that
        GameState,                  //will be created when the player enters the game state, or any time after that
    }

    public struct SetupOptions
    {
        public bool isMonoBehvaiour;
        public bool hasDefaultConstructor;
        public SetupType setupType;
        public SetupState setupState;
        public string objName;
        public string modName;
        public GameObject targetObj;
        public Type T;
    }

    public struct Source
    {
        public TextAsset sourceTxt;
        public bool isPreCompiled;
    }

    //used to pass basic instructions to runtime compiler
    public class SetupInstructionsAttribute : System.Attribute
    {
        public SetupType setupType = SetupType.None;
        public SetupState setupTime = SetupState.None;
        public string objName;                                  //name of new objects to create in scene, or prefab to load from mod
        public string modName;                                  //name of mod to locate prefab - only needed if loading assets from mod via SetupInstructionsAttritubute

        public SetupInstructionsAttribute()
        {
        }

        public SetupInstructionsAttribute(SetupType setupType = SetupType.None, SetupState setupTime = SetupState.MenuState, string objName = "", string modName = "")
        {
            this.setupType = setupType;
            this.setupTime = setupTime;
            this.objName = objName;
            this.modName = modName;
        }
    }


}
