// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using DaggerfallConnect.Utility;

namespace DaggerfallConnect.Arena2
{
    /// <summary>
    /// Connects to and reads text from the TEXT.RSC file.
    /// Also provides helpers for other classes using the text resource format.
    /// </summary>
    public class TextFile
    {
        public const string Filename = "TEXT.RSC";

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

        /// <summary>
        /// Gets a NewLineToken.
        /// </summary>
        public static Token NewLineToken
        {
            get
            {
                Token newLineToken = new Token();
                newLineToken.formatting = Formatting.NewLine;
                return newLineToken;
            }
        }

        public static Token TabToken
        {
            get
            {
                Token tabToken = new Token();
                tabToken.formatting = Formatting.PositionPrefix;
                return tabToken;
            }
        }

        #endregion

        #region Enums

        /// <summary>
        /// Special Text Resource formatting bytes.
        /// http://www.uesp.net/wiki/Daggerfall:Text_Record_Format
        /// This is a work in progress.
        /// </summary>
        public enum Formatting
        {
            Text = -1,

            NewLineOffset = 0x00,
            SameLineOffset = 0x01,
            PullPreceeding = 0x02,

            FirstCharacter = 0x20,
            LastCharacter = 0x7f,

            PositionPrefix = 0xfb,
            FontPrefix = 0xf9,

            JustifyLeft = 0xfc,
            JustifyCenter = 0xfd,

            NewLine = 0x00,
            EndOfPage = 0xf6,
            SubrecordSeparator = 0xff,
            EndOfRecord = 0xfe,

            Nothing = 0xffff,
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

        /// <summary>
        /// Stores a single text or formatting token.
        /// This makes it possible remix localized strings with original formatting.
        /// </summary>
        public struct Token
        {
            public Formatting formatting;
            public string text;
            public int x;
            public int y;
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

        public TextFile()
        {
        }

        public TextFile(string arena2Path, string fileName, FileUsage usage = FileUsage.UseMemory, bool readOnly = true)
        {
            Load(arena2Path, fileName, usage, readOnly);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load a text resource file.
        /// </summary>
        public void Load(string arena2Path, string filename, FileUsage usage = FileUsage.UseMemory, bool readOnly = true)
        {
            // Check text resource file is supported
            if (filename != Filename)
            {
                throw new Exception(string.Format("TextFile: File '{0}' is not a supported text file.", filename));
            }

            // Setup new file
            header = new TextRecordDatabaseHeader();
            recordIdToIndexDict.Clear();
            isLoaded = false;

            // Load file
            fileProxy = new FileProxy(Path.Combine(arena2Path, filename), usage, readOnly);

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
        /// Special formatting characters are parsed into [0x00] format.
        /// </summary>
        public TextRecord GetTextRecordByIndex(int index)
        {
            TextRecord textRecord = new TextRecord();

            if (isLoaded)
            {
                textRecord.id = header.TextRecordHeaders[index].TextRecordId;
                textRecord.text = ParseBytesToString(GetBytesByIndex(index));
            }

            return textRecord;
        }

        /// <summary>
        /// Dumps entire text database to CSV.
        /// </summary>
        /// <param name="path">Full path to output file.</param>
        public void DumpToCSV(string path)
        {
            if (!isLoaded)
                return;

            StreamWriter writer = File.CreateText(path);

            writer.WriteLine("sep=\t");
            writer.WriteLine("\"ID\"\t\"DATA\"");

            for (int i = 0; i < RecordCount; i++)
            {
                TextRecord record = GetTextRecordByIndex(i);
                writer.WriteLine("\"{0}\"\t\"{1}\"", record.id, record.text);
            }

            writer.Close();
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

        string ParseBytesToString(byte[] buffer)
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

        #region Static Helpers

        /// <summary>
        /// Tokenizes Daggerfall text resource data.
        /// </summary>
        /// <param name="buffer">Source buffer containing raw text resource data (e.g. from TEXT.RSC or a book file).</param>
        /// <param name="position">Position in buffer to start tokenizing (e.g. start of record or start of page).</param>
        /// <param name="endToken">Formatting byte that terminates token stream (e.g. Formatting.EndOfRecord or Formatting.EndOfPage).</param>
        /// <returns>Array of text and formatting tokens.</returns>
        public static Token[] ReadTokens(ref byte[] buffer, int position, Formatting endToken)
        {
            List<Token> tokens = new List<Token>();

            while (position < buffer.Length)
            {
                byte nextByte = buffer[position];
                if (nextByte == (byte)endToken)
                    break;

                if (IsFormattingToken(nextByte))
                    tokens.Add(ReadFormattingToken(ref buffer, position, out position));
                else
                    tokens.Add(ReadTextToken(ref buffer, position, out position));     
            }

            return tokens.ToArray();
        }

        private static Token ReadFormattingToken(ref byte[] buffer, int position, out int endPosition)
        {
            Formatting formatting = (Formatting)buffer[position++];

            byte? peek;
            int x = 0, y = 0;
            Token token = new Token();
            token.formatting = formatting;
            switch (token.formatting)
            {
                case Formatting.NewLineOffset:
                    break;
                case Formatting.FontPrefix:
                    x = buffer[position++];
                    break;
                case Formatting.PositionPrefix:
                    peek = PeekByte(ref buffer, position);
                    if (peek != null)
                    {
                        x = peek.Value;
                        position++;
                    }
                    break;
            }
            token.x = x;
            token.y = y;
            endPosition = position;

            return token;
        }

        private static Token ReadTextToken(ref byte[] buffer, int position, out int endPosition)
        {
            // Find length of text data
            int start = position;
            int count = 0;
            while (position < buffer.Length)
            {
                byte nextByte = buffer[position++];
                if (nextByte >= (byte)Formatting.FirstCharacter && nextByte <= (byte)Formatting.LastCharacter)
                    count++;
                else
                    break;
            }

            // Create token
            Token token = new Token();
            token.formatting = Formatting.Text;
            token.text = Encoding.UTF8.GetString(buffer, start, count);
            endPosition = start + count;

            return token;
        }

        private static bool IsFormattingToken(byte value)
        {
            if (value >= (byte)Formatting.FirstCharacter && value <= (byte)Formatting.LastCharacter)
                return false;
            else
                return true;
        }

        private static byte? PeekByte(ref byte[] buffer, int currentIndex, int offsetIndex = 0)
        {
            if (currentIndex + offsetIndex >= buffer.Length)
                return null;
            else
                return buffer[currentIndex + offsetIndex];
        }

        #endregion
    }
}