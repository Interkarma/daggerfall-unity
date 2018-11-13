// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:
// 
// Notes:
//

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Utility.ModSupport
{
    /// <summary>
    /// Defines a <see cref="Component"/> provided by a mod assembly which can be automatically serialized and deserialized.
    /// </summary>
    /// <remarks>
    /// Monobehaviours defined in a mod assembly are not known to Unity when a prefab is loaded and deserialized from an assetbundle.
    /// If a prefab is marked with this attribute, the bundled clone is stripped of any instance of the Monobehaviour and its data
    /// is stored in a json file bundled with the mod. When the GameObject is loaded at runtime, the json file is used to
    /// automatically add back the components (provided by the mod assembly) and deserialize them.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class ImportedComponentAttribute : Attribute
    {
        #region Types

        private struct ImportedComponentsData
        {
            public List<Component> Components;
            public Dictionary<string, ImportedComponentsData> Children;
        }

        /// <summary>
        /// A converter to store references to assets from the mod assetbundle.
        /// </summary>
        private class AssetConverter : fsConverter
        {
            public override bool CanProcess(Type type)
            {
                return typeof(UnityEngine.Object).IsAssignableFrom(type) && !typeof(Component).IsAssignableFrom(type);
            }

            public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
            {
                var asset = instance as UnityEngine.Object;
                serialized = asset ? new fsData(asset.name) : fsData.Null;
                return fsResult.Success;
            }

            public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
            {
                fsResult fsResult = fsResult.Success;
                if (data.IsNull) return fsResult;
                if ((fsResult += CheckType(data, fsDataType.String)).Failed) return fsResult;
                return (fsResult += RetrieveAssetReference(data.AsString, ref instance));
            }

            public override object CreateInstance(fsData data, Type storageType)
            {
                return null;
            }

            public override bool RequestCycleSupport(Type storageType)
            {
                return false;
            }

            private fsResult RetrieveAssetReference(string name, ref object instance)
            {
                var asset = Serializer.Context.Get<Mod>().GetAsset<UnityEngine.Object>(name);
                if (!asset)
                    return fsResult.Fail(string.Format("Failed to restore reference to {0}; ensure that the the asset is bundled with the mod.", name));

                instance = asset;
                return fsResult.Success;
            }
        }

        #endregion

        #region Fields

        readonly static fsSerializer fsSerializer = new fsSerializer();
        readonly static Dictionary<string, Type> types = new Dictionary<string, Type>();

        #endregion

        #region Constructors

        static ImportedComponentAttribute()
        {
            fsSerializer.AddConverter(new AssetConverter());
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Makes the filename for the imported components data.
        /// </summary>
        public static string MakeFileName(string prefabName)
        {
            return string.Format("{0}.imported.json", prefabName);
        }

#if UNITY_EDITOR

        /// <summary>
        /// Saves data for the imported components, if found.
        /// </summary>
        /// <param name="gameObject">A gameobject instance.</param>
        /// <param name="directory">Where the json file will be created.</param>
        /// <returns>Full path of json file or null.</returns>
        public static string Save(GameObject gameObject, string directory)
        {
            // Get all imported components
            var importedComponentsData = new ImportedComponentsData();
            if (!SaveImportedComponents(gameObject, ref importedComponentsData))
                return null;

            // Serialize imported components data
            fsData fsData;
            fsSerializer.TrySerialize(importedComponentsData, out fsData);
            string path = Path.Combine(directory, MakeFileName(gameObject.name));
            File.WriteAllText(path, fsJsonPrinter.PrettyJson(fsData));
            return path;
        }

#endif

        /// <summary>
        /// Restore imported components on this gameobject and its children.
        /// </summary>
        /// <param name="mod">The mod that contains components type and serialized data.</param>
        /// <param name="gameObject">A gameobject instance.</param>
        public static void Restore(Mod mod, GameObject gameObject)
        {
            fsSerializer.Context.Set(mod);
            RestoreImportedComponents(mod, gameObject, fsJsonParser.Parse(LoadSerializedFile(mod, gameObject.name)));
        }

        #endregion

        #region Private Methods

#if UNITY_EDITOR

        /// <summary>
        /// Seeks all imported components on a gameobject and its children.
        /// Imported components are REMOVED from the prefab.
        /// </summary>
        /// <param name="gameObject">A gameobject instance.</param>
        /// <param name="importedComponentsData">Instance to fill with components data.</param>
        private static bool SaveImportedComponents(GameObject gameObject, ref ImportedComponentsData importedComponentsData)
        {
            bool saveNeeded = false;

            // Get and destroy only imported components
            importedComponentsData.Components = new List<Component>();
            foreach (var component in gameObject.GetComponents(typeof(Component)))
            {
                if (GetCustomAttribute(component.GetType(), typeof(ImportedComponentAttribute)) != null)
                {
                    importedComponentsData.Components.Add(component);
                    UnityEngine.Object.DestroyImmediate(component, true);
                    if (!saveNeeded)
                        saveNeeded = true;
                }
            }

            // Iterate children
            if (gameObject.transform.childCount > 0)
            {
                importedComponentsData.Children = new Dictionary<string, ImportedComponentsData>();
                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    Transform transform = gameObject.transform.GetChild(i);
                    var childImportedComponentsData = new ImportedComponentsData();
                    if (SaveImportedComponents(transform.gameObject, ref childImportedComponentsData) && !saveNeeded)
                        saveNeeded = true;
                    importedComponentsData.Children.Add(transform.gameObject.name, childImportedComponentsData);
                }
            }

            return saveNeeded;
        }

#endif

        /// <summary>
        /// Restores and deserializes imported components on a gameobject and its children.
        /// </summary>
        /// <param name="gameObject">A gameobject instance.</param>
        /// <param name="fsData">Serialized ImportedComponentsData for the gameobject.</param>
        private static void RestoreImportedComponents(Mod mod, GameObject gameObject, fsData fsData)
        {
            Dictionary<string, fsData> dict = fsData.AsDictionary;
            foreach (fsData componentData in dict["Components"].AsList)
            {
                // Add component and deserialize
                Type type = FindType(mod, componentData.AsDictionary["$type"].AsString);
                if (type != null)
                {
                    object instance = gameObject.AddComponent(type);
                    fsResult fsResult = fsSerializer.TryDeserialize(componentData, type, ref instance);
                    if (fsResult.HasWarnings)
                        Debug.LogWarning(fsResult.FormattedMessages);
                }
            }

            // Restore children
            fsData children = dict["Children"];
            if (children != null && !children.IsNull)
            {
                foreach (KeyValuePair<string, fsData> childData in children.AsDictionary)
                {
                    Transform child = gameObject.transform.Find(childData.Key);
                    if (child != null)
                        RestoreImportedComponents(mod, child.gameObject, childData.Value);
                }
            }
        }

        private static Type FindType(Mod mod, string typeName)
        {
            if (!string.IsNullOrEmpty(typeName))
            {
                Type type;
                if (types.TryGetValue(typeName, out type))
                    return type;

                type = mod.GetCompiledType(typeName);
                if (type != null)
                {
                    types.Add(typeName, type);
                    return type;
                }
            }

            Debug.LogErrorFormat("Failed to find type {0}.", typeName);
            return null;
        }

        private static string LoadSerializedFile(Mod mod, string name)
        {
            var textAsset = mod.GetAsset<TextAsset>(MakeFileName(name));
            if (textAsset != null)
                return textAsset.text;

            throw new Exception("Serialized data not found!");
        }

        #endregion
    }
}
