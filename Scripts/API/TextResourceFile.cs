// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using System;
using System.IO;
using System.Collections.Generic;
using DaggerfallConnect.Utility;

namespace DaggerfallConnect.Arena2
{
    public class TextResourceFile
    {
        string[] supportedFileNames = { "TEXT.RSC" };
        FileProxy fileProxy;
        bool isLoaded = false;
        TextRecordDatabaseHeader header;
        Dictionary<int, int> recordIdToIndexDict = new Dictionary<int, int>();

        #region Properties

        /// <summary>
        /// True if a text resource file is loaded.
        /// </summary>
        public bool IsLoaded
        {
            get { return isLoaded; }
        }

        /// <summary>
        /// Gets number of text record elements in currently open file.
        /// </summary>
        public int RecordCount
        {
            get { return (int)((header.TextRecordHeaderLength / 6) - 1); }
        }

        #endregion

        #region Structures

        private struct TextRecordDatabaseHeader
        {
            public UInt16 TextRecordHeaderLength;
            public TextRecordHeaderElement[] TextRecordHeaders;
        }

        private struct TextRecordHeaderElement
        {
            public UInt16 TextRecordId;
            public UInt32 Offset;
        }

        #endregion

        #region Formatting
        #endregion

        #region Constructors

        public TextResourceFile()
        {
        }

        public TextResourceFile(string arena2Path, string fileName, FileUsage usage = FileUsage.UseMemory, bool readOnly = true)
        {
            Load(arena2Path, fileName, usage, readOnly);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load a text resource file.
        /// </summary>
        public void Load(string arena2Path, string fileName, FileUsage usage = FileUsage.UseMemory, bool readOnly = true)
        {
            // Check text resource file is supported
            if (!IsFileNameSupported(fileName))
            {
                throw new Exception(string.Format("File '{0}' is not a supported text file.", fileName));
            }

            // Setup new file
            header = new TextRecordDatabaseHeader();
            recordIdToIndexDict.Clear();
            isLoaded = false;

            // Load file
            fileProxy = new FileProxy(Path.Combine(arena2Path, fileName), usage, readOnly);

            // Read file
            BinaryReader reader = fileProxy.GetReader();
            ReadHeader(reader);
            ReadTextRecordHeaders(reader);

            // Raise loaded flag
            isLoaded = true;
        }

        /// <summary>
        /// Gets raw text record bytes by index with all special bytes intact, such as terminating 0xfe.
        /// </summary>
        public byte[] GetRawTextRecordByIndex(int index)
        {
            if (!isLoaded)
                return null;

            BinaryReader reader = fileProxy.GetReader((int)header.TextRecordHeaders[index].Offset);
            return reader.ReadBytes(GetRecordLength(reader));
        }

        /// <summary>
        /// Gets raw text record bytes by id with all special bytes intact, such as terminating 0xfe.
        /// </summary>
        public byte[] GetRawTextRecordById(int id)
        {
            if (!isLoaded || !recordIdToIndexDict.ContainsKey(id))
                return null;

            int index = recordIdToIndexDict[id];
            return GetRawTextRecordByIndex(index);
        }

        #endregion

        #region Private Methods

        void ReadHeader(BinaryReader reader)
        {
            reader.BaseStream.Position = 0;
            header.TextRecordHeaderLength = reader.ReadUInt16();
        }

        void ReadTextRecordHeaders(BinaryReader reader)
        {
            int count = RecordCount;
            header.TextRecordHeaders = new TextRecordHeaderElement[count];
            for (int i = 0; i < count; i++)
            {
                int key = header.TextRecordHeaders[i].TextRecordId = reader.ReadUInt16();
                header.TextRecordHeaders[i].Offset = reader.ReadUInt32();
                recordIdToIndexDict.Add(key, i);
            }
        }

        bool IsFileNameSupported(string fileName)
        {
            for (int i = 0; i < supportedFileNames.Length; i++)
            {
                if (string.Compare(fileName, supportedFileNames[i], true) == 0)
                    return true;
            }

            return false;
        }

        int GetRecordLength(BinaryReader reader)
        {
            long startPosition = reader.BaseStream.Position;

            while (reader.ReadByte() != 0xfe) { }
            int recordLength = (int)(reader.BaseStream.Position - startPosition);

            reader.BaseStream.Position = startPosition;

            return recordLength;
        }

        #endregion
    }
}