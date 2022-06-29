// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using System.IO;
using System.Collections.Generic;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using UnityEngine;

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
        string saveName = string.Empty;

        bool isPathOpen = false;
        bool isReadOnly = true;
        readonly Dictionary<int, string> saveGameDict = new Dictionary<int, string>();
        SaveTree saveTree;
        SaveVars saveVars;
        BsaFile mapSave;
        BioFile bioFile;
        RumorFile rumorFile;
        SaveImage saveImage;

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
        /// Gets the bio file from the current save's directory.
        /// </summary>
        public BioFile BioFile
        {
            get { return bioFile; }
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
                return false;

            if (!saveGameDict.ContainsKey(save))
                return false;

            return true;
        }

        /// <summary>
        /// Opens just SaveImage and SaveName for display.
        /// Before reading other data, must call OpenSave() or TryOpenSave().
        /// </summary>
        /// <param name="save">Save index.</param>
        /// <returns>True if successful.</returns>
        public bool LazyOpenSave(int save)
        {
            if (!HasSave(save))
                return false;

            if (!LoadSaveImage(save))
                throw new Exception("Could not lazy open SavImage for index " + save);

            if (!LoadSaveName(save))
                throw new Exception("Could not lazy open SaveName for index " + save);

            return true;
        }

        /// <summary>
        /// Opens the save game index specified.
        /// </summary>
        /// <param name="save">Save index</param>
        /// <param name="loadingInGame">True if the save game is being loaded for regular play, false if loading for Save Explorer.</param>
        /// <returns>True if successful.</returns>
        public bool OpenSave(int save, bool loadingInGame = true)
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

            mapSave = new BsaFile();
            if (!mapSave.Load(Path.Combine(saveGameDict[save], "MAPSAVE.SAV"), FileUsage.UseMemory, true))
                throw new Exception("Could not open MapSave for index " + save);

            if (loadingInGame) // Only check MAPSAVE if loading in-game, not if viewing in Save Explorer. There is a noticeable delay for
                               // Save Explorer as the classic saves are loaded, and a null exception if the Save Explorer is opened
                               // without the game running in the editor, due to PlayerGPS.dfUnity not being instantiated.
                               // Save Explorer currently has no use for MAPSAVE data. This code should be revisited (speed up MAPSAVE processing,
                               // fix null exception, remove this bool check) if MAPSAVE-related functionality is added to Save Explorer.
            {
                PlayerGPS gps = GameManager.Instance.PlayerGPS;
                gps.ClearDiscoveryData();
                for (int regionIndex = 0; regionIndex < 62; regionIndex++)
                {
                    // Generate name from region index
                    string name = string.Format("MAPSAVE.{0:000}", regionIndex);

                    // Get record index
                    int index = mapSave.GetRecordIndex(name);
                    if (index == -1)
                        return false;

                    // Read MAPSAVE data
                    byte[] data = mapSave.GetRecordBytes(index);

                    // Parse MAPSAVE data for discovered locations
                    DFRegion regionData = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(regionIndex);
                    int locationCount = Math.Min(data.Length, (int)regionData.LocationCount);
                    for (int i = 0; i < locationCount; i++)
                    {
                        // If a location is marked as discovered in classic but not DF Unity, discover it for DF Unity
                        if ((data[i] & 0x40) != 0 && !regionData.MapTable[i].Discovered)
                        {
                            DFLocation location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(regionIndex, i);
                            gps.DiscoverLocation(regionData.Name, location.Name);
                        }
                    }
                }
            }

            rumorFile = new RumorFile();
            if (!rumorFile.Load(Path.Combine(saveGameDict[save], "RUMOR.DAT"), FileUsage.UseMemory, true))
                UnityEngine.Debug.Log("Could not open RUMOR.DAT for index " + save);

            // Only import classic rumours when loading in game, not when using Save Explorer
            if (loadingInGame)
            {
                for (int i = 0; i < rumorFile.rumors.Count; i++)
                    GameManager.Instance.TalkManager.ImportClassicRumor(rumorFile.rumors[i]);
            }

            bioFile = new BioFile();
            if (!bioFile.Load(Path.Combine(saveGameDict[save], "BIO.DAT")))
                UnityEngine.Debug.Log("Could not open BIO.DAT for index " + save);

            return true;
        }

        /// <summary>
        /// Opens the save game index specified. Will not throw any exceptions on failure.
        /// </summary>
        /// <param name="save">Save index</param>
        /// <returns>True if successful.</returns>
        public bool TryOpenSave(int save)
        {
            try
            {
                return OpenSave(save);
            }
            catch  (Exception e)
            {
                Debug.LogError($"An Exception occurred while attempting to load Classic save #{save.ToString()}:\n{e}");
                return false;
            }
        }

        #endregion

        #region Private Methods

        /*string GetSaveIndexName(int save)
        {
            return string.Format("SAVE{0}", save);
        }*/

        string GetArena2Path()
        {
            string path1 = Path.Combine(savesPath, "arena2");
            string path2 = Path.Combine(savesPath, "ARENA2");
            if (Directory.Exists(path1))
                return path1;
            else if (Directory.Exists(path2))
                return path2;
            else
                throw new Exception("SaveGame could not locate subordinate 'arena2' or 'ARENA2' path inside " + savesPath);
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
                    !File.Exists(Path.Combine(saves[i], SaveImage.Filename)))
                //!File.Exists(Path.Combine(saves[i], SaveVars.Filename)))      // TODO: Restore this once savevars supported
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
