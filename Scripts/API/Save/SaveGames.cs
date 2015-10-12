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
        public const string SaveNameTxt = "SAVENAME.TXT";

        string savesPath = string.Empty;
        bool isPathOpen = false;
        bool isReadOnly = true;

        Dictionary<int, string> saveGameDict = new Dictionary<int, string>();
        SaveTree saveTree;
        SaveVars saveVars;
        SaveImage saveImage;
        string saveName = string.Empty;

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
        /// Gets SaveTree of currently open save.
        /// </summary>
        public SaveTree SaveTree
        {
            get { return saveTree; }
        }

        /// <summary>
        /// Gets SaveVars of currently open save.
        /// </summary>
        public SaveVars SaveVars
        {
            get { return saveVars; }
        }

        /// <summary>
        /// Gets SaveImage of currently open save.
        /// </summary>
        public SaveImage SaveImage
        {
            get { return saveImage; }
        }

        /// <summary>
        /// Gets name of currently open save.
        /// </summary>
        public string SaveName
        {
            get { return saveName; }
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
        /// Determines if the specified save index exists.
        /// </summary>
        /// <param name="save">Save index.</param>
        /// <returns>True if save index present.</returns>
        public bool HasSave(int save)
        {
            if (!isPathOpen)
                throw new Exception("Save games folder is not open.");

            if (!saveGameDict.ContainsKey(save))
                return false;

            return true;
        }

        /// <summary>
        /// Opens the save game index specified.
        /// </summary>
        /// <param name="save">Save index</param>
        /// <returns>True if successful.</returns>
        public bool OpenSave(int save)
        {
            if (!HasSave(save))
                return false;

            if (!LoadSaveImage(save))
                throw new Exception("Could not open SaveImage for index " + save);

            if (!LoadSaveName(save))
                throw new Exception("Could not open SaveName for index " + save);

            saveTree = new SaveTree();
            if (!saveTree.Open(Path.Combine(saveGameDict[save], SaveTree.Filename)))
                throw new Exception("Could not open SaveTree for index " + save);

            saveVars = new SaveVars();
            if (!saveVars.Open(Path.Combine(saveGameDict[save], SaveVars.Filename)))
                throw new Exception("Could not open SaveVars for index " + save);

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
            saveGameDict.Clear();
            for (int i = 0; i < saves.Length; i++)
            {
                if (!File.Exists(Path.Combine(saves[i], SaveTree.Filename)) ||
                    !File.Exists(Path.Combine(saves[i], SaveImage.Filename)) ||
                    !File.Exists(Path.Combine(saves[i], SaveVars.Filename)))
                {
                    continue;
                }

                saveGameDict.Add(i, saves[i]);
            }

            return saveGameDict.Count;
        }

        bool LoadSaveImage(int save)
        {
            if (!saveGameDict.ContainsKey(save))
                return false;

            saveImage = new SaveImage();
            if (!saveImage.Load(Path.Combine(saveGameDict[save], SaveImage.Filename), FileUsage.UseMemory, true))
                return false;
            if (!saveImage.LoadPalette(Path.Combine(GetArena2Path(), saveImage.PaletteName)))
                return false;

            return true;
        }

        bool LoadSaveName(int save)
        {
            if (!saveGameDict.ContainsKey(save))
                return false;

            FileProxy file = new FileProxy(Path.Combine(saveGameDict[save], SaveNameTxt), FileUsage.UseMemory, true);
            saveName = file.ReadCString(0, 0);
            file.Close();

            return true;
        }

        #endregion
    }
}