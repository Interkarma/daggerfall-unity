// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

#region Using Statements
using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect.Utility;
#endregion

namespace DaggerfallConnect.Save
{
    /// <summary>
    /// Enumerates and extracts data from Daggerfall save games.
    /// </summary>
    public class SaveGames
    {
        string savesPath = string.Empty;
        bool isPathOpen = false;
        bool isReadOnly = true;
        Exception lastException;
        List<string> saveGameList = new List<string>();

        SaveTree saveTree = new SaveTree();
        SaveImage saveImage = new SaveImage();

        #region Properties

        /// <summary>
        /// Gets currently open parent path containing ARENA2 and SAVE0-SAVE5.
        /// </summary>
        public string SavesPath
        {
            get { return savesPath; }
        }

        /// <summary>
        /// Returns true if current path is open.
        /// </summary>
        public bool IsPathOpen
        {
            get { return isPathOpen; }
        }

        /// <summary>
        /// Returns true if read only mode enabled.
        /// </summary>
        public bool IsReadOnly
        {
            get { return isReadOnly; }
        }

        /// <summary>
        /// Gets last exception.
        /// </summary>
        public Exception LastException
        {
            get { return lastException; }
        }

        /// <summary>
        /// Gets SaveTree of currently open save.
        /// </summary>
        public SaveTree SaveTree
        {
            get { return saveTree; }
        }

        /// <summary>
        /// Gets SaveImage of currently open save.
        /// </summary>
        public SaveImage SaveImage
        {
            get { return saveImage; }
        }

        #endregion

        #region Constructors

        public SaveGames()
        {
        }

        public SaveGames(string path, bool readOnly = true)
            : base()
        {
            OpenSavesPath(path, readOnly);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Enumerates all save games in parent path provided.
        /// </summary>
        /// <param name="path">Path to parent Daggerfall folder containing ARENA2 and SAVE0-SAVE5 folders.</param>
        /// <param name="readOnly">Open save as read only.</param>
        /// <returns>True if save opened successfully.</returns>
        public bool OpenSavesPath(string path, bool readOnly = true)
        {
            isPathOpen = false;
            int savesFound = EnumerateSaves(path);
            if (savesFound == 0)
                return false;

            isPathOpen = true;
            isReadOnly = readOnly;
            savesPath = path;

            return true;
        }

        /// <summary>
        /// Opens the save game index specified.
        /// </summary>
        /// <param name="save">Save index</param>
        /// <returns>True if successful.</returns>
        public bool OpenSave(int save)
        {
            try
            {
                if (!isPathOpen)
                    throw new Exception("Save games folder is not open.");

                if (save < 0 || save >= saveGameList.Count)
                    throw new IndexOutOfRangeException("Save game index out of range.");

                if (!saveTree.Open(Path.Combine(saveGameList[save], SaveTree.Filename)))
                    throw new Exception("Could not open SaveTree for index " + save);

                if (!LoadSaveImage(save))
                    throw new Exception("Could not open SaveImage for index" + save);
            }
            catch (Exception ex)
            {
                lastException = ex;
                return false;
            }

            return true;
        }

        #endregion

        #region Private Methods

        string GetSaveIndexName(int save)
        {
            return string.Format("SAVE{0}", save);
        }

        string GetArena2Path()
        {
            return Path.Combine(savesPath, "arena2");
        }

        int EnumerateSaves(string path)
        {
            // Get save directories
            string[] saves = Directory.GetDirectories(path, "SAVE?", SearchOption.TopDirectoryOnly);
            if (saves == null || saves.Length == 0)
                return 0;

            // Test each directory
            saveGameList.Clear();
            for (int i = 0; i < saves.Length; i++)
            {
                if (!File.Exists(Path.Combine(saves[i], SaveTree.Filename)) ||
                    !File.Exists(Path.Combine(saves[i], SaveImage.Filename)))
                {
                    continue;
                }

                saveGameList.Add(saves[i]);
            }

            return saveGameList.Count;
        }

        bool LoadSaveImage(int save)
        {
            saveImage = new SaveImage();
            if (!saveImage.Load(Path.Combine(saveGameList[save], SaveImage.Filename), FileUsage.UseMemory, true))
                return false;
            if (!saveImage.LoadPalette(Path.Combine(GetArena2Path(), saveImage.PaletteName)))
                return false;

            return true;
        }

        #endregion
    }
}