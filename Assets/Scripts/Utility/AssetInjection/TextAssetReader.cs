// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using FullSerializer;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// An helper class that creates a virtual layer for reading text from mods and loose files.
    /// </summary>
    public static class TextAssetReader
    {
        /// <summary>
        /// Gets content of a text file from loose files or a text asset from mods.
        /// A folder named 'Assets' (i.e. Assets/Game/Mods/ExampleMod/Assets) is the equivalent of the 'StreamingAssets' folder on disk.
        /// </summary>
        /// <param name="relativePath">Relative path to text asset with forward slashes (i.e. 'Data/foo.json' for 'Assets/Data/foo.json' and 'StreamingAssets/Data/foo.json').</param>
        /// <param name="content">Read content of text asset or null.</param>
        /// <returns>True if file found.</returns>
        public static bool TryRead(string relativePath, out string content)
        {
            if (relativePath == null)
                throw new ArgumentNullException("relativePath");

            string path = Path.Combine(Application.streamingAssetsPath, relativePath);
            if (File.Exists(path))
            {
                content = File.ReadAllText(path);
                return true;
            }

            TextAsset textAsset;
            if (ModManager.Instance && ModManager.Instance.TryGetAsset(Path.GetFileName(relativePath), false, out textAsset))
            {
                content = textAsset.text;
                return true;
            }

            content = null;
            return false;
        }

        /// <summary>
        /// Gets deserialized content of a text file from loose files or a text asset from mods.
        /// A folder named 'Assets' (i.e. Assets/Game/Mods/ExampleMod/Assets) is the equivalent of the 'StreamingAssets' folder on disk.
        /// </summary>
        /// <param name="relativePath">Relative path to text asset with forward slashes (i.e. 'Data/foo.json' for 'Assets/Data/foo.json' and 'StreamingAssets/Data/foo.json').</param>
        /// <param name="data">Deserialized content or type default.</param>
        /// <returns>True if asset found and parsed.</returns>
        public static bool TryRead<T>(string relativePath, out T data)
        {
            string content;
            if (TryRead(relativePath, out content))
            {
                data = (T)SaveLoadManager.Deserialize(typeof(T), content);
                return true;
            }

            data = default(T);
            return false;
        }

        /// <summary>
        /// Gets content of all text files from loose files and text assets from mods inside the given relative directory.
        /// A folder named 'Assets' (i.e. Assets/Game/Mods/ExampleMod/Assets) is the equivalent of the 'StreamingAssets' folder on disk.
        /// </summary>
        /// <param name="relativeDirectory">Relative path to a directory with forward slashes (i.e. 'Data/Foo' for 'Assets/Data/Foo' and 'StreamingAssets/Data/Foo').</param>
        /// <param name="extension">The extension without leading dot (i.e. 'txt' for 'foo.txt') or null to match all extensions.</param>
        /// <returns>A list of text contents.</returns>
        public static List<string> ReadAll(string relativeDirectory, string extension = null)
        {
            if (relativeDirectory == null)
                throw new ArgumentNullException("relativeDirectory");

            var content = new List<string>();

            string dirPath = Path.Combine(Application.streamingAssetsPath, relativeDirectory);
            if (Directory.Exists(dirPath))
            {
                foreach (string path in extension != null ? Directory.GetFiles(dirPath, string.Format("*.{0}", extension)) : Directory.GetFiles(dirPath))
                    content.Add(File.ReadAllText(path));
            }

            if (ModManager.Instance)
            {
                var assets = ModManager.Instance.FindAssets<TextAsset>(string.Format("Assets/{0}", relativeDirectory), extension != null ? string.Format(".{0}", extension) : null);
                if (assets != null)
                {
                    foreach (TextAsset asset in assets)
                        content.Add(asset.text);
                }
            }

            return content;
        }

        /// <summary>
        /// Gets deserialized content of all text files from loose files and text assets from mods inside the given relative directory.
        /// A folder named 'Assets' (i.e. Assets/Game/Mods/ExampleMod/Assets) is the equivalent of the 'StreamingAssets' folder on disk.
        /// </summary>
        /// <param name="relativeDirectory">Relative path to a directory with forward slashes (i.e. 'Data/Foo' for 'Assets/Data/Foo' and 'StreamingAssets/Data/Foo').</param>
        /// <param name="extension">The extension without leading dot (i.e. 'json' for 'foo.json') or null to match all extensions.</param>
        /// <returns>A list of parsed contents.</returns>
        public static IEnumerable<T> ReadAll<T>(string relativeDirectory, string extension = null)
        {
            return ReadAll(relativeDirectory, extension).Select(x => (T)SaveLoadManager.Deserialize(typeof(T), x));
        }

        /// <summary>
        /// Reads all text assets with the given name, which are expected to contain a list or array of items, and merges all contributes in a single collection.
        /// If an item, identified by <paramref name="isItemData"/>, is found more than one time, all of its data is merged in a single instance following mods load order.
        /// </summary>
        /// <param name="items">A collection of items, which may be non-empty.</param>
        /// <param name="name">Name of serialized text assets.</param>
        /// <param name="isItemData">A delegate that checks if serialized data of an item, provided as a dictionary, can be deserialized on top of the given item.</param>
        /// <typeparam name="T">Type of each item, which can be a struct or a class.</typeparam>
        public static void Merge<T>(List<T> items, string name, Func<T, Dictionary<string, fsData>, bool> isItemData = null) where T: new()
        {
            if (items == null)
                throw new ArgumentNullException("items");
            
            if (name == null)
                throw new ArgumentNullException("name");

            if (ModManager.Instance)
            {
                foreach (Mod mod in ModManager.Instance.Mods.Where(x => x.Enabled && x.HasAsset(name)))
                {
                    var textAsset = mod.GetAsset<TextAsset>(name);
                    if (textAsset)
                    {
                        foreach (fsData itemData in fsJsonParser.Parse(textAsset.text).AsList)
                        {
                            T item;
                            int index;

                            if (isItemData != null)
                            {
                                Dictionary<string, fsData> data = itemData.AsDictionary;
                                if ((index = items.FindIndex(x => isItemData(x, data))) != -1)
                                    item = items[index];
                                else
                                    item = new T();
                            }
                            else
                            {
                                index = -1;
                                item = new T();
                            }

                            fsResult fsResult = ModManager._serializer.TryDeserialize(itemData, ref item);
                            if (fsResult.HasWarnings)
                                Debug.LogWarningFormat("Deserialization of {0} from {1} produced the following messages: {2}", name, mod.Title, fsResult.FormattedMessages);

                            if (fsResult.Succeeded)
                            {
                                if (index != -1)
                                    items[index] = item;
                                else
                                    items.Add(item);
                            }
                        }
                    }
                }
            }
        }
    }
}
