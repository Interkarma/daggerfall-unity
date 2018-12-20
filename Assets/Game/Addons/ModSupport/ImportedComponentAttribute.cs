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
using System.Linq;
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
        #region Converters

        /// <summary>
        /// A converter to store imported components on a gameobject.
        /// This is only used on the actual prefab, not field references.
        /// </summary>
        private class ImportedComponentsConverter : fsDirectConverter
        {
            public override Type ModelType
            {
                get { return typeof(GameObject); }
            }

            public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
            {
                var gameObject = instance as GameObject;
                if (gameObject)
                    return SerializeImportedComponents(gameObject, out serialized);

                serialized = fsData.Null;
                return fsResult.Fail("GameObject is not instantiated.");
            }

            public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
            {
                fsResult fsResult = fsResult.Success;
                var gameObject = instance as GameObject;
                if (!gameObject) return fsResult += fsResult.Fail("GameObject is not instantiated.");
                if ((fsResult += CheckType(data, fsDataType.Object)).Failed) return fsResult;
                return fsResult += DeserializeImportedComponents(data, gameObject, Serializer.Context.Get<Mod>());
            }

            public override object CreateInstance(fsData data, Type storageType)
            {
                return null;
            }

            public override bool RequestCycleSupport(Type storageType)
            {
                return false;
            }

            private fsResult SerializeImportedComponents(GameObject gameObject, out fsData fsData)
            {
                fsData = fsData.Null;
                fsResult fsResult = fsResult.Success;

                // Get and destroy only imported components
                List<fsData> components = null;
                foreach (var component in gameObject.GetComponents<Component>().Where(x => x))
                {
                    if (GetCustomAttribute(component.GetType(), typeof(ImportedComponentAttribute)) != null)
                    {
                        fsData componentData;
                        if ((fsResult += Serializer.TrySerialize(component, out componentData)).Failed) return fsResult;
                        if ((fsResult += ValidateComponentData(componentData)).Failed) return fsResult;
                        (components ?? (components = new List<fsData>())).Add(componentData);
                        UnityEngine.Object.DestroyImmediate(component, true);
                    }
                }

                // Iterate children
                Dictionary<string, fsData> children = null;
                if (gameObject.transform.childCount > 0)
                {
                    children = new Dictionary<string, fsData>();
                    for (int i = 0; i < gameObject.transform.childCount; i++)
                    {
                        Transform transform = gameObject.transform.GetChild(i);
                        fsData childData;
                        if ((fsResult += SerializeImportedComponents(transform.gameObject, out childData)).Failed) return fsResult;
                        if (!childData.IsNull)
                            children.Add(transform.gameObject.name, childData);
                    }
                }

                // Make fsData only if there are imported components
                if (components != null || children != null)
                {
                    fsData = new fsData(new Dictionary<string, fsData>
                    {
                        { "Components", new fsData(components) },
                        { "Children", new fsData(children) }
                    });
                }

                return fsResult;
            }

            private fsResult DeserializeImportedComponents(fsData fsData, GameObject gameObject, Mod mod)
            {
                fsResult fsResult = fsResult.Success;

                if ((fsResult += CheckType(fsData, fsDataType.Object)).Failed) return fsResult;
                Dictionary<string, fsData> dict = fsData.AsDictionary;

                // Restore components on this gameobject
                fsData components;
                if ((fsResult += CheckKey(dict, "Components", out components)).Failed) return fsResult;
                if ((fsResult += CheckType(components, fsDataType.Array)).Failed) return fsResult;
                foreach (fsData componentData in components.AsList)
                {
                    // Get type name
                    string typeName;
                    if ((fsResult += DeserializeMember(componentData.AsDictionary, null, "$type", out typeName)).Failed) return fsResult;

                    // Add component and deserialize
                    Type type = FindType(mod, typeName);
                    if (type == null) return fsResult += fsResult.Fail(string.Format("Failed to find type {0}.", typeName));
                    object instance = gameObject.AddComponent(type);
                    if ((fsResult += fsSerializer.TryDeserialize(componentData, type, ref instance)).Failed) return fsResult;
                }

                // Restore components on children
                fsData children;
                if ((fsResult += CheckKey(dict, "Children", out children)).Failed) return fsResult;
                if (!children.IsNull)
                {
                    if ((fsResult += CheckType(children, fsDataType.Object)).Failed) return fsResult;
                    foreach (KeyValuePair<string, fsData> childData in children.AsDictionary)
                    {
                        Transform child = gameObject.transform.Find(childData.Key);
                        if (child == null) return fsResult += fsResult.Fail(string.Format("{0} not found on {1}", childData.Key, gameObject.name));
                        if ((fsResult += DeserializeImportedComponents(childData.Value, child.gameObject, mod)).Failed) return fsResult;
                    }
                }

                return fsResult;
            }

            private static fsResult ValidateComponentData(fsData fsData)
            {
                // FullSerializer handles cyclic references but wants to create the instance on the first reference met.
                // Objects derived from Component must use AddComponent() so this is only possible if the first reference is this one.
                // For example a component can't reference another component on a child gameobject, while the opposite should be fine.
                // In practice, if we have a $ref it means the serialized data has already been stored somewhere else...
                return !fsData.AsDictionary.ContainsKey("$ref") ? fsResult.Success :
                    fsResult.Fail("Unable to preserve field reference to component met before the component itself" +
                        " because a component instance can't exist without a gameobject; use GetComponent() in Start() instead.");
            }
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
        /// Imported components are REMOVED from the prefab.
        /// </summary>
        /// <param name="gameObject">A gameobject instance.</param>
        /// <param name="directory">Where the json file will be created.</param>
        /// <returns>Full path of json file or null.</returns>
        public static string Save(GameObject gameObject, string directory)
        {
            fsData fsData;
            fsResult fsResult = fsSerializer.TrySerialize(typeof(GameObject), typeof(ImportedComponentsConverter), gameObject, out fsData);
            if (fsResult.Failed || fsResult.HasWarnings)
                throw new Exception(string.Format("Serialization of {0} {1} with messages:\n{2}",
                    gameObject.name, fsResult.Succeeded ? "succeeded" : "failed", fsResult.FormattedMessages));

            if (fsData.IsNull)
                return null;

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
            object instance = gameObject;
            fsData fsData = fsJsonParser.Parse(LoadSerializedFile(mod, gameObject.name));
            fsResult fsResult = fsSerializer.TryDeserialize(fsData, typeof(GameObject), typeof(ImportedComponentsConverter), ref instance);
            if (fsResult.Failed || fsResult.HasWarnings)
                Debug.LogErrorFormat("Deserialization of {0} from {1} {2} with messages:\n{3}",
                    gameObject.name, mod.Title, fsResult.Succeeded ? "succeeded" : "failed", fsResult.FormattedMessages);
        }

        #endregion

        #region Private Methods

        private static Type FindType(Mod mod, string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                return null;

            Type type;
            if (types.TryGetValue(typeName, out type))
                return type;

            return types[typeName] = mod.GetCompiledType(typeName);
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
