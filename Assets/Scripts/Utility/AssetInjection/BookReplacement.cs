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
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using FullSerializer;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    [Serializable]
    internal struct BookMappingEntry
    {
#pragma warning disable 649
        /// <summary>
        /// The file name with the extension; for example `example-book.TXT` for `StreamingAssets\Books\example-book.TXT`.
        /// </summary>
        [SerializeField]
        internal string Name;

        /// <summary>
        /// A readable book title.
        /// </summary>
        [SerializeField]
        internal string Title;

        /// <summary>
        /// An unique ID used for serialization; must be between 256 and <see cref="int.MaxValue"/> excluding 10000.
        /// </summary>
        [SerializeField]
        internal int ID;

        /// <summary>
        /// If true this book is not found inside random loots or bookshelves and must be made available directly by mods.
        /// </summary>
        [SerializeField]
        internal bool IsUnique;

        /// <summary>
        /// Book is available only when this global variable is set.
        /// </summary>
        [SerializeField]
        internal int? WhenVarSet;
#pragma warning restore 649
    }

    /// <summary>
    /// Handles import and injection of custom books with the purpose of providing modding support.
    /// Book files are imported from mod bundles with load order or loaded directly from disk.
    /// </summary>
    public static class BookReplacement
    {
        #region Fields & Properties

        static readonly string booksPath = Path.Combine(Application.streamingAssetsPath, "Books");
        static readonly string mappingPath = Path.Combine(booksPath, "Mapping");

        internal static readonly Dictionary<int, BookMappingEntry> BookMappingEntries = new Dictionary<int, BookMappingEntry>();
        static bool customBooksSeeked;

        /// <summary>
        /// Path to custom books on disk.
        /// </summary>
        public static string BooksPath
        {
            get { return booksPath; }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Read maps data for additional custom books from modding locations.
        /// </summary>
        /// <param name="bookIDNameMapping">Map that associates id to title.</param>
        /// <remarks>
        /// Each entry is associated to a numeric id; 0-255 and 10000 are reserved by classic books.
        /// Titles are added to given bookID dictionary with classic ids.
        /// File names (without TXT extension) are added to another dictionary for retrieval of book resource.
        /// </remarks>
        internal static void FindAdditionalBooks(Dictionary<int, string> bookIDNameMapping)
        {
            if (!customBooksSeeked)
            {
                foreach (string mapContent in GetBooksMaps())
                {
                    var map = new List<BookMappingEntry>();
                    fsResult fsResult = ModManager._serializer.TryDeserialize(fsJsonParser.Parse(mapContent), ref map);
                    if (fsResult.HasWarnings)
                        Debug.LogWarning(fsResult.FormattedMessages);

                    if (fsResult.Succeeded)
                    {
                        foreach (BookMappingEntry entry in map)
                        {
                            if (string.IsNullOrEmpty(entry.Name) || string.IsNullOrEmpty(entry.Title) || entry.ID == 0)
                            {
                                Debug.LogError("Failed to register book because required informations are missing.");
                                continue;
                            }

                            if (bookIDNameMapping.ContainsKey(entry.ID))
                            {
                                Debug.LogErrorFormat("Failed to register book {0} because id {1} is already in use by {2}.", entry.Title, entry.ID, bookIDNameMapping[entry.ID]);
                                continue;
                            }
                            else if (entry.ID == 10000)
                            {
                                Debug.LogErrorFormat("Failed to register book {0} because id 10000 is reserved.", entry.Title);
                                continue;
                            }

                            BookMappingEntries.Add(entry.ID, entry);
                        }
                    }
                }

                customBooksSeeked = true;
            }

            foreach (var entry in BookMappingEntries)
                bookIDNameMapping.Add(entry.Key, entry.Value.Title); 
        }

        /// <summary>
        /// Checks if all conditions for book availability are met.
        /// Unknown books are considered as an absence of conditions and true is returned.
        /// </summary>
        /// <param name="id">Book id.</param>
        /// <returns>True if all conditions are met.</returns>
        internal static bool BookMeetsConditions(int id)
        {
            BookMappingEntry entry;
            if (!BookMappingEntries.TryGetValue(id, out entry))
                return true;

            return !entry.IsUnique
                && (!entry.WhenVarSet.HasValue || GameManager.Instance.PlayerEntity.GlobalVars.GetGlobalVar(entry.WhenVarSet.Value));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Seeks book data from mods.
        /// If a book file with the given name is found, data is loaded on the given book instance.
        /// If no data is found, the instance is left unaltered.
        /// </summary>
        /// <param name="name">Name of book including .TXT extension</param>
        /// <param name="book">Book instance on which imported data is loaded.</param>
        /// <returns>True if book is found.</returns>
        public static bool TryImportBook(string name, BookFile book)
        {
            if (DaggerfallUnity.Settings.AssetInjection)
            {
                // Seek from loose files
                string path = Path.Combine(booksPath, name);
                if (File.Exists(path))
                {
                    if (book.OpenBook(File.ReadAllBytes(path), name))
                        return true;
                }

                // Seek from mods
                TextAsset textAsset;
                if (ModManager.Instance != null && ModManager.Instance.TryGetAsset(name, false, out textAsset))
                {
                    if (book.OpenBook(textAsset.bytes, name))
                        return true;
                }
            }

            return false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets content of book mapping json files from loose files and mods.
        /// </summary>
        private static IEnumerable<string> GetBooksMaps()
        {
            var looseFilesMaps = Directory.Exists(mappingPath) ?
                Directory.GetFiles(mappingPath, "*.json").Where(x => !x.EndsWith("IDs.json")).Select(x => File.ReadAllText(x)) :
                null;

            var modsMaps = ModManager.Instance ? ModManager.Instance.GetAllModsWithContributes(x => x.BooksMapping != null).SelectMany(mod =>
                mod.ModInfo.Contributes.BooksMapping.Select(x => mod.GetAsset<TextAsset>(x).ToString())) :
                null;

            return looseFilesMaps != null && modsMaps != null ?
                looseFilesMaps.Concat(modsMaps) :
                looseFilesMaps ?? modsMaps ?? Enumerable.Empty<string>();
        }

        #endregion
    }
}
