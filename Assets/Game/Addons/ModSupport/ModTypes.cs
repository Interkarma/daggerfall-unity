// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyl@dfworkshop.net)
// Contributors:    TheLacus
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Utility.ModSupport
{
    /// <summary>
    /// An asset loaded from a mod.
    /// </summary>
    public struct LoadedAsset
    {
        public Type T;
        public UnityEngine.Object Obj;
        public float TimeStamp;

        public LoadedAsset(Type T, UnityEngine.Object Obj)
        {
            this.T = T;
            this.Obj = Obj;
            this.TimeStamp = 0;
        }
    }

    /// <summary>
    /// The content of a json mod manifest file, created from the Mod Builder and bundled with the mod itself.
    /// </summary>
    [Serializable]
    [fsObject(Processor = typeof(IgnoreNullProcessor))]
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

#pragma warning disable 649
        /// <summary>
        /// Automatic asset injections defined by manifest .json file.
        /// These values are not available for edits from mods at runtime.
        /// </summary>
        public ModContributes Contributes { get; internal set; }
#pragma warning restore 649

        /// <summary>
        /// A list of mods that this mod depends on or is otherwise compatible with only if certain conditions are met.
        /// </summary>
        [SerializeField]
        public ModDependency[] Dependencies;

        public ModInfo()
        {
            Files = new List<string>();
        }
    }

    /// <summary>
    /// Contributes provided by a mod to be injected in game.
    /// </summary>
    /// <remarks>
    /// The purpose of this section of the manifest file is to signal the presence of additional
    /// assets for the core game, which are imported automatically without the need of one-time
    /// scripts for every mod, or even scripting knowledge on the modder side.
    /// This class can be expanded over time as necessary but breaking changes should be avoided.
    /// <remarks/>
    [Serializable]
    public sealed class ModContributes
    {
        /// <summary>
        /// Look-up maps that announce additional books to be imported.
        /// </summary>
        public string[] BooksMapping { get; internal set; }

        /// <summary>
        /// Names of additional quest lists to be automatically imported
        /// </summary>
        public string[] QuestLists { get; internal set; }

        /// <summary>
        /// Names of spell icon packs; each name corresponds to a <see cref="Texture2D"/>
        /// asset and a <see cref="TextAsset"/> with `.json` extension.
        /// </summary>
        public string[] SpellIcons { get; internal set; }
    }

    /// <summary>
    /// A set of rules that defines the limits within which a mod is required or otherwise compatible with another one.
    /// </summary>
    /// <remarks>
    /// These are all the possible combinations:
    /// - Dependency: must be available, have higher priority and follow specified criteria.
    /// - Optional dependency: if is available it must have higher priority and follow specified criteria.
    /// - Peer dependency: must be available and follow specified criteria but higher priority is not required.
    /// - Optional peer dependency: if is available it must follow specified criteria but higher priority is not required.
    /// </remarks>
    [Serializable]
    public struct ModDependency
    {
        /// <summary>
        /// Name of target mod.
        /// </summary>
        [SerializeField]
        public string Name;

        /// <summary>
        /// If true, target mod doesn't need to be available, but must validate these criteria if it is.
        /// </summary>
        [SerializeField]
        public bool IsOptional;

        /// <summary>
        /// If true, target mod can be positioned anywhere in the load order, otherwise must be positioned above.
        /// </summary>
        [SerializeField]
        public bool IsPeer;

        /// <summary>
        /// If not null this string is the minimum accepted version with format X.Y.Z.
        /// Pre-release identifiers following an hyphen are ignored in target version so they must be omitted here.
        /// For example "1.0.0" is equal to "1.0.0-rc.1".
        /// </summary>
        [SerializeField]
        public string Version;
    }

    /// <summary>
    /// Options for a mod setup intro point, meaning a method with the <see cref="Invoke"/> attribute.
    /// </summary>
    public struct SetupOptions : IComparable<SetupOptions>
    {
        /// <summary>
        /// The priority within invokable methods for the same mod.
        /// </summary>
        public readonly int priority;

        /// <summary>
        /// The mod that found target method inside its assemblies.
        /// </summary>
        public readonly Mod mod;

        /// <summary>
        /// The method to be invoked.
        /// </summary>
        public readonly System.Reflection.MethodInfo mi;

        public SetupOptions(int priority, Mod mod, System.Reflection.MethodInfo mi)
        {
            this.priority = priority;
            this.mod = mod;
            this.mi = mi;
        }

        /// <summary>
        /// Compares methods for their priority.
        /// </summary>
        public int CompareTo(SetupOptions other)
        {
            if (other.priority == priority)
                return 0;
            else if (this.priority < other.priority)
                return -1;
            return 1;
        }
    }

    /// <summary>
    /// Data passed to methods with the <see cref="Invoke"/> attribute when they are invoked by the Mod Manager.
    /// It contains informations required to initialize custom scripts provided by a mod,
    /// including the <see cref="Mod"/> instance associated to the class that receives this data.
    /// </summary>
    public struct InitParams
    {
        /// <summary>
        /// The title of the mod.
        /// </summary>
        public readonly string ModTitle;

        /// <summary>
        /// The position of the mod inside the mods collection.
        /// </summary>
        public readonly int ModIndex;
        
        /// <summary>
        /// The position of the mod in the load order.
        /// </summary>
        public readonly int LoadPriority;

        /// <summary>
        /// The total number of mods loaded by Mod Manager.
        /// </summary>
        public readonly int LoadedModsCount;

        /// <summary>
        /// The associated Mod instance that gives access, among the others, to bundled assets.
        /// </summary>
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
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class Invoke : Attribute
    {
        public readonly int Priority;
        public readonly StateManager.StateTypes StartState;

        /// <summary>
        /// Request the mod manager to invoke this method at the specified state.
        /// </summary>
        /// <param name="startState">At which state the ModManager will invoke this method; typically this the Start or the Game state.</param>
        /// <param name="priority">Defines a per-mod order if there are multiple invoked methods for the same state.</param>
        public Invoke(StateManager.StateTypes startState = StateManager.StateTypes.Start, int priority = 99)
        {
            this.Priority = priority;
            this.StartState = startState;
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
        LZ4 = 0,
        LZMA = 1,
        Uncompressed = 2,
    }

    public delegate void DFModMessageReceiver(string message, object data, DFModMessageCallback callBack);
    public delegate void DFModMessageCallback(string message, object data);

    /// <summary>
    /// One of the physics layer that was reserved for a mod.
    /// </summary>
    public sealed class ModReservedLayer : IDisposable
    {
        static readonly Stack<int> availableLayers = new Stack<int>(new[] { 28, 29, 30, 31 });

        readonly int layer;
        bool disposed = false;

        /// <summary>
        /// The numeric index of the layer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Layer has already been released.</exception>
        public int Layer
        {
            get
            {
                AssertIsNotReleased();
                return layer;
            }
        }

        private ModReservedLayer()
        {
            layer = availableLayers.Pop();
        }

        /// <summary>
        /// Gets the name of the layer.
        /// </summary>
        /// <returns>The name associated to this layer index.</returns>
        public sealed override string ToString()
        {
            return LayerMask.LayerToName(layer);
        }

        /// <summary>
        /// Releases this layer so it can be made available to other mods.
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                availableLayers.Push(layer);
                disposed = true;
            }
        }

        private void AssertIsNotReleased()
        {
            if (disposed)
                throw new ObjectDisposedException(this.ToString(), "Layer has already been released.");
        }

        /// <summary>
        /// Requests a reserved layer. There is a limited number of layers so it
        /// should be released with <see cref="Dispose()"/> if not used anymore.
        /// </summary>
        /// <returns>A reserved layer or null if none is available.</returns>
        public static ModReservedLayer Request()
        {
            if (availableLayers.Count > 0)
                return new ModReservedLayer();

            return null;
        }
    }

    /// <summary>
    /// Do not serialize members whose values are null.
    /// </summary>
    public class IgnoreNullProcessor : fsObjectProcessor
    {
        public override void OnAfterSerialize(Type storageType, object instance, ref fsData data)
        {
            if (!data.IsDictionary)
                throw new NotSupportedException("fsData is not a dictionary.");

            RemoveNullMembers(data);
        }

        private void RemoveNullMembers(fsData data)
        {
            if (data.IsDictionary)
            {
                var dict = data.AsDictionary;
                foreach (var item in dict.ToArray())
                {
                    if (item.Value.IsNull)
                        dict.Remove(item.Key);
                    else if (item.Value.IsDictionary || item.Value.IsList)
                        RemoveNullMembers(item.Value);
                }
            }
            else if (data.IsList)
            {
                var list = data.AsList;
                if (list.Count > 0 && (list[0].IsDictionary || list[0].IsList))
                {
                    foreach (var item in list)
                        RemoveNullMembers(item);
                }
            }
        }
    }
}
