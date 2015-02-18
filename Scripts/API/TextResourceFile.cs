// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using System;
using System.IO;
using System.Text;
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

        #region Enums

        /// <summary>
        /// Special Text Resource formatting bytes.
        /// http://www.uesp.net/wiki/Daggerfall:Text_Record_Format
        /// </summary>
        public enum Formatting
        {
            NewLineOffset = 0x00,
            SameLineOffset = 0x01,
            PullPreceeding = 0x02,

            FirstCharacter = 0x20,
            LastCharacter = 0x7f,

            PositionPrefix = 0xfb,
            FontPrefix = 0xf9,
            JustifyPreceedingLeft = 0xfc,
            CenterPreceeding = 0xfd,
            NewLine = 0x00,
            EndOfPage = 0xf6,
            SubrecordSeparator = 0xff,
            EndOfRecord = 0xfe,
        }

        #endregion

        #region Structures

        /// <summary>
        /// Stores a specially parsed text record with all control bytes in format [0x00-0xFF].
        /// Otherwise, text is returned in original format with all control bytes intact.
        /// </summary>
        public struct TextRecord
        {
            public int id;
            public string text;
        }

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
        /// Converts index to id. Invalid index returns -1.
        /// </summary>
        public int IndexToId(int index)
        {
            if (!isLoaded)
                return -1;

            return header.TextRecordHeaders[index].TextRecordId;
        }

        /// <summary>
        /// Converts id to index. Invalid id returns -1.
        /// </summary>
        public int IdToIndex(int id)
        {
            if (!isLoaded || !recordIdToIndexDict.ContainsKey(id))
                return -1;

            return recordIdToIndexDict[id];
        }

        /// <summary>
        /// Gets raw bytes by index with all special bytes intact, such as terminating 0xfe.
        /// </summary>
        public byte[] GetBytesByIndex(int index)
        {
            if (!isLoaded)
                return null;

            BinaryReader reader = fileProxy.GetReader((int)header.TextRecordHeaders[index].Offset);
            return reader.ReadBytes(GetRecordLength(reader));
        }

        /// <summary>
        /// Gets raw bytes by id with all special bytes intact, such as terminating 0xfe.
        /// </summary>
        public byte[] GetBytesById(int id)
        {
            int index = IdToIndex(id);
            if (index == -1)
                return null;

            return GetBytesByIndex(index);
        }

        /// <summary>
        /// Gets TextRecord data by index.
        /// Special formatting characters are parsed into %x00 format.
        /// </summary>
        public TextRecord GetTextRecordByIndex(int index)
        {
            TextRecord textRecord = new TextRecord();

            if (isLoaded)
            {
                textRecord.id = header.TextRecordHeaders[index].TextRecordId;
                textRecord.text = ParseBytes(GetBytesByIndex(index));
            }

            return textRecord;
        }

        #endregion

        #region Reading Methods

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

        #region Parsing Methods

        string ParseBytes(byte[] buffer)
        {
            string dst = string.Empty;

            for (int i = 0; i < buffer.Length; i++)
            {
                byte b = buffer[i];
                if (b >= (byte)Formatting.FirstCharacter && b <= (byte)Formatting.LastCharacter)
                {
                    dst += Encoding.UTF8.GetString(buffer, i, 1);           // Pass-through character bytes to string
                }
                else
                {
                    dst += string.Format("[0x{0}]", b.ToString("X2"));      // Format control bytes in [0x00] format
                }
            }
            
            return dst;
        }

        #endregion
    }
}