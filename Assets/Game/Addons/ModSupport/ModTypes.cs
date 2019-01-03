// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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
        public string ModTitle;         //displayed in game
        public string ModVersion;
        public string ModAuthor;
        public string ContactInfo;
        public string DFUnity_Version;
        public string ModDescription;
        public string GUID = "invalid";
        public List<string> Files;      //list of assets to add to mod (only used during creation)

        public ModInfo()
        {
            Files = new List<string>();
        }
    }

    public struct SetupOptions : IComparable<SetupOptions>
    {
        public readonly int priority;
        public readonly Mod mod;
        public readonly System.Reflection.MethodInfo mi;

        public SetupOptions(int priority, Mod mod, System.Reflection.MethodInfo mi)
        {
            this.priority = priority;
            this.mod = mod;
            this.mi = mi;
        }


        public int CompareTo(SetupOptions other)
        {
            if (other.priority == priority)
                return 0;
            else if (this.priority < other.priority)
                return -1;
            return 1;
        }
    }

    //passed to mod's Init methods called from ModManager
    public struct InitParams
    {
        public readonly string ModTitle;
        public readonly int ModIndex;
        public readonly int LoadPriority;
        public readonly int LoadedModsCount;
        public readonly Mod Mod;

        public InitParams(Mod Mod, int ModIndex, int LoadedModsCount)
        {
            this.Mod = Mod;
            this.ModTitle = Mod.Title;
            this.LoadPriority = Mod.LoadPriority;
            this.ModIndex = ModIndex;
            this.LoadedModsCount = LoadedModsCount;
        }

    }

    public struct Source
    {
        public TextAsset sourceTxt;
        public bool isPreCompiled;
    }

    //Used to specify functions to be called automaticlly by modmanager during mod setup.
    //To work, must be on a non-generic, public, static, class method that only takes 
    //an InitParams struct for a parameter
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public class Invoke : System.Attribute
    {
        public readonly int priority;
        public readonly StateManager.StateTypes startState;
        public Invoke(StateManager.StateTypes startState = StateManager.StateTypes.Start, int priority = 99)
        {
            this.priority = priority;
            this.startState = startState;
        }
    }

    [Serializable]
    public struct SerializedRecord
    {
        public readonly string gameObjectName;
        public readonly Type componentType;
        public readonly object[] serializedObjects;

        public SerializedRecord(string goName, Component compType, params object[] toSerialize)
        {
            this.gameObjectName = goName;
            this.componentType = compType.GetType();
            this.serializedObjects = toSerialize;
        }
    }

    [Obsolete("Obsolete. Please use ImportedComponent attribute instead.")]
    public interface Idfmod_Serializable
    {
        System.Object[] ToSerialize();
        void Deseralized(params System.Object[] obj);
        bool Ignore { get; set; }
    }

    /// <summary>
    /// Takes part of load/save logic.
    /// </summary>
    public interface IHasModSaveData
    {
        Type SaveDataType { get; }                      // Type of save data object.
        object NewSaveData();                           // Make empty/default save data.
        object GetSaveData();                           // Gets data to be serialized or null.
        void RestoreSaveData(object saveData);          // Apply deserialized data.
    }

    public delegate void DFModMessageReceiver(string message, object data, DFModMessageCallback callBack);
    public delegate void DFModMessageCallback(string message, object data);
}
