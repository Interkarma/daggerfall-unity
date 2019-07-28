// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
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
using DaggerfallWorkshop.Game.Utility.ModSupport;
using FullSerializer;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    [Serializable]
    internal struct BookMappingEntry
    {
        /// <summary>
        /// The file name without extension; for example `example-book` for `StreamingAssets\Books\example-book.TXT`.
        /// </summary>
        [SerializeField]
        internal string Name;

        /// <summary>
        /// A readable book title.
        /// </summary>
        [SerializeField]
        internal string Title;
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

        internal static readonly Dictionary<int, string> FileNames = new Dictionary<int, string>();

        /// <summary>
        /// Path to custom books on disk.
        /// </summary>
        public static string BooksPath
        {
            get { return booksPath; }
        }

        #endregion

        #region Internal Methods

        internal static void AssertCustomBooksImportEnabled()
        {
            if (!DaggerfallUnity.Settings.CustomBooksImport)
                throw new InvalidOperationException("Custom books import is disabled.");
        }

        /// <summary>
        /// Read maps data for additional custom books from modding locations.
        /// </summary>
        /// <param name="bookIDNameMapping">Map that associates id to title.</param>
        /// <remarks>
        /// Each entry is associated to a numeric id; 1-111 and 10000 are reserved by classic books.
        /// Titles are added to given bookID dictionary with classic ids.
        /// File names (without TXT extension) are added to another dictionary for retrieval of book resource.
        /// </remarks>
        internal static void FindAdditionalBooks(Dictionary<int, string> bookIDNameMapping)
        {
            AssertCustomBooksImportEnabled();

            // Book IDs are stored inside Mapping folder.
            // They are associated to game installation (with mods) and not specific saves. 
            var ids = new Dictionary<string, int>();
            string idsPath = Path.Combine(mappingPath, "IDs.json");
            if (File.Exists(idsPath))
                ModManager._serializer.TryDeserialize(fsJsonParser.Parse(File.ReadAllText(idsPath)), ref ids);
            int idsCount = ids.Count;

            int currentId = 111;
            foreach (string mapContent in GetBooksMaps())
            {
                var map = new List<BookMappingEntry>();
                fsResult fsResult = ModManager._serializer.TryDeserialize(fsJsonParser.Parse(mapContent), ref map);
                if (fsResult.HasWarnings)
                    Debug.LogWarning(fsResult.FormattedMessages);

                if (fsResult.Succeeded)
                {
                    foreach (var book in map)
                    {
                        string name = book.Name + ".TXT";

                        // Assign book id
                        int id;
                        if (!ids.TryGetValue(name, out id))
                            ids.Add(name, id = ++currentId != 10000 ? currentId : ++currentId);

                        // Store results
                        bookIDNameMapping.Add(id, book.Title);
                        FileNames.Add(id, name);
                    }
                }
            }

            // Sync IDs with persistent json file
            if (ids.Count > idsCount)
            {
                fsData fsData;
                ModManager._serializer.TrySerialize(ids, out fsData).AssertSuccessWithoutWarnings();
                Directory.CreateDirectory(mappingPath);
                File.WriteAllText(idsPath, fsJsonPrinter.PrettyJson(fsData));
            }

            if (FileNames.Count > 0)
                Debug.LogWarningFormat("Imported {0} custom books. Addition of custom books is EXPERIMENTAL and may introduce bugs! " +
                    "Breaking changes to this feature can also be expected until is considered stable.", FileNames.Count);
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
                    book.OpenBook(File.ReadAllBytes(path), name);
                    return true;
                }

                // Seek from mods
                TextAsset textAsset;
                if (ModManager.Instance != null && ModManager.Instance.TryGetAsset(name, false, out textAsset))
                {
                    book.OpenBook(textAsset.bytes, name);
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

            var modsMaps = ModManager.Instance.GetAllModsWithContributes(x => x.BooksMapping != null).SelectMany(mod =>
                mod.ModInfo.Contributes.BooksMapping.Select(x => mod.GetAsset<TextAsset>(x).ToString()));

            return looseFilesMaps != null ? looseFilesMaps.Concat(modsMaps) : modsMaps;
        }

        #endregion
    }
}
