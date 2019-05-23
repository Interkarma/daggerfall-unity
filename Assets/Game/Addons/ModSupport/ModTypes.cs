// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
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

    /// <summary>
    /// Specify a non-generic, public, static, class method that only takes an <see cref="InitParams"/>
    /// struct for a parameter, to be called automatically by Mod Manager during mod setup.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public class Invoke : Attribute
    {
        public readonly int priority;
        public readonly StateManager.StateTypes startState;

        /// <summary>
        /// Request the mod manager to invoke this method at the specified state.
        /// </summary>
        /// <param name="startState">At which state the ModManager will invoke this method; typically this the Start or the Game state.</param>
        /// <param name="priority">Defines a per-mod order if there are multiple invoked methods for the same state.</param>
        public Invoke(StateManager.StateTypes startState = StateManager.StateTypes.Start, int priority = 99)
        {
            this.priority = priority;
            this.startState = startState;
        }
    }

    /// <summary>
    /// Takes part of load/save logic.
    /// </summary>
    public interface IHasModSaveData
    {
        /// <summary>
        /// The type of a custom class that holds save data and optionally use <see cref="FullSerializer.fsObjectAttribute"/> for versioning.
        /// </summary>
        Type SaveDataType { get; }

        /// <summary>
        /// Makes a new instance of <see cref="SaveDataType"/> with default values.
        /// </summary>
        object NewSaveData();

        /// <summary>
        /// Makes a new instance of <see cref="SaveDataType"/> for the current state or null if there is nothing to save.
        /// </summary>
        object GetSaveData();

        /// <summary>
        /// Restores retrieved data when a save is loaded.
        /// </summary>
        /// <param name="saveData">An instance of <see cref="SaveDataType"/>.</param>
        void RestoreSaveData(object saveData);
    }

    //used by mod builder window
    public enum ModCompressionOptions
    {
        LZ4=0,
        LZMA=1,
        Uncompressed=2,
    }

    public delegate void DFModMessageReceiver(string message, object data, DFModMessageCallback callBack);
    public delegate void DFModMessageCallback(string message, object data);
}
