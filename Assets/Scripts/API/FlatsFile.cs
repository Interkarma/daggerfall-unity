// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Michael Rauter (Nystul)
// Contributors:    
// 
// Notes:
//

#region Using Statements
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect.Utility;
#endregion

namespace DaggerfallConnect.Arena2
{
    /// <summary>
    /// Connects to FLATS.CFG and reads flats data.
    /// </summary>
    public class FlatsFile
    {
        #region Fields

        readonly FileProxy flatsFile = new FileProxy();

        readonly Dictionary<int, FlatData> flatsDict = new Dictionary<int, FlatData>();

        #endregion

        #region Properties

        public Dictionary<int, FlatData> FlatsDict
        {
            get { return flatsDict; }
        }

        #endregion

        #region structs

        public struct FlatData
        {
            public int archive;
            public int record;
            public string caption;
            public string gender; // values are 1 for male, 2 for female. If preceded by a "?", in classic the flat would be censored in ChildGard mode.
            public int unknown2;
            public int unknown3;
            public int faceIndex; // index of face in TFAC00I0.RCI
        }

        #endregion

        #region Static Properties

        /// <summary>
        /// Gets default FLATS.CFG filename.
        /// </summary>
        static public string Filename
        {
            get { return "FLATS.CFG"; }
        }

        #endregion

        #region Constructors

        public FlatsFile()
        {
        }

        public FlatsFile(string filePath, FileUsage usage, bool readOnly)
        {
            Load(filePath, usage, readOnly);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load from FLATS.CFG file.
        /// </summary>
        /// <param name="filePath">Absolute path to FLATS.CFG file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public void Load(string filePath, FileUsage usage, bool readOnly)
        {
            // Validate filename
            if (!filePath.EndsWith(Filename, StringComparison.InvariantCultureIgnoreCase))
                return;

            // Load file
            if (!flatsFile.Load(filePath, usage, readOnly))
                return;

            // Parse faction file
            byte[] buffer = flatsFile.Buffer;
            string txt = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            ParseFlats(txt);
        }

        /// <summary>
        /// Gets flat data from flats ID.
        /// </summary>
        /// <param name="flatID">Flat ID.</param>
        /// <param name="flatDataOut">Receives flats data.</param>
        /// <returns>True if successful.</returns>
        public bool GetFlatData(int flatID, out FlatData flatDataOut)
        {
            flatDataOut = new FlatData();
            if (flatsDict.ContainsKey(flatID))
            {
                flatDataOut = flatsDict[flatID];
                return true;
            }

            return false;
        }

        /// <summary>
        /// Turns a flat int back into archive/record format
        /// </summary>
        /// <param name="flat"></param>
        /// <returns></returns>
        public FlatData GetFlatData(int flat)
        {
            FlatData flatData = flatsDict[flat];
            return flatData;
        }

        /// <summary>
        /// Gets flat ID from billboards archive and record index.
        /// </summary>
        /// <param name="archiveIndex">texture archive index of billboard.</param>
        /// <param name="recordIndex">texture record index inside texture archive of billboard.</param>
        /// <returns>Flats ID if found, otherwise -1.</returns>
        public static int GetFlatID(int archiveIndex, int recordIndex)
        {
            int flatID = archiveIndex << 7;
            flatID += recordIndex;
            return flatID;
        }

        public static void ReverseFlatID(int flatID, out int archiveIndex, out int recordIndex)
        {
            archiveIndex = flatID >> 7;
            recordIndex = flatID & 0x7f;
        }


        #endregion

        #region Private Methods

        void ParseFlats(string txt)
        {
            // Clear existing dictionary
            flatsDict.Clear();

            // First pass reads each flat text block in order
            List<string[]> flatBlocks = new List<string[]>();
            using (StringReader reader = new StringReader(txt))
            {
                List<string> currentblock = new List<string>();
                while (true)
                {
                    // Handle end of file
                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        // Store final block
                        if (currentblock.Count > 0)
                            flatBlocks.Add(currentblock.ToArray());
                        break;
                    }

                    // empty lines is end of current block
                    if (string.IsNullOrEmpty(line))
                    {
                        if (currentblock.Count > 0)
                            flatBlocks.Add(currentblock.ToArray());
                        currentblock.Clear();
                        continue;
                    }

                    // trim and add line to current flat block
                    line = line.Trim();
                    currentblock.Add(line);
                }
            }

            // Second pass parses the text block into FlatsData
            for (int i = 0; i < flatBlocks.Count; i++)
            {
                // Start a new flat
                FlatData flat = new FlatData();
                string[] block = flatBlocks[i];

                // Parse flat block
                ParseFlatData(ref block, ref flat);

                int flatID = GetFlatID(flat.archive, flat.record);

                // Store flat just read
                if (!flatsDict.ContainsKey(flatID))
                {
                    flatsDict.Add(flatID, flat);
                }
            }
        }

        void ParseFlatData(ref string[] block, ref FlatData flat)
        {
            string[] parts;
            string line = block[0];

            // Split string into tag and value using ':' character
            parts = line.Split(' ');
            if (parts.Length != 2)
            {
                throw new Exception(string.Format("Invalid flat format for data {0} on flat with archive index {1} and record index {2}", line, flat.archive, flat.record));
            }

            // Get flat texture archive and record index
            flat.archive = ParseInt(parts[0]);
            flat.record = ParseInt(parts[1]);

            // get flat caption
            line = block[1];
            flat.caption = line;

            // get gender
            line = block[2];
            flat.gender = line;

            // get currently unknown values
            line = block[3];
            flat.unknown2 = ParseInt(line);
            line = block[4];
            flat.unknown3 = ParseInt(line);

            // get face index
            line = block[5];
            flat.faceIndex = ParseInt(line);
        }

        int ParseInt(string value)
        {
            return int.Parse(value);
        }

        #endregion
    }
}
