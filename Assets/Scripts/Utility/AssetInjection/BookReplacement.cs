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

using System.IO;
using UnityEngine;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Utility.ModSupport;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Handles import and injection of custom books with the purpose of providing modding support.
    /// Book files are imported from mod bundles with load order or loaded directly from disk.
    /// </summary>
    public static class BookReplacement
    {
        #region Fields & Properties

        static readonly string booksPath = Path.Combine(Application.streamingAssetsPath, "Books");

        /// <summary>
        /// Path to custom books on disk.
        /// </summary>
        public static string BooksPath
        {
            get { return booksPath; }
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
    }
}
