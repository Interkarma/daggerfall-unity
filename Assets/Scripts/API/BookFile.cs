// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors: InconsolableCellist
//
// Notes:
//

using System;
using System.IO;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using UnityEngine;

namespace DaggerfallConnect.Arena2
{
    /// <summary>
    /// Reads a Daggerfall book file.
    /// </summary>
    public class BookFile
    {
        #region Fields

        const string books = "books";
        const string naughty = "naughty";

        readonly FileProxy bookFile = new FileProxy();
        BookHeader header = new BookHeader();

        internal string lastSuccessfulBookHeaderRead = "";

        #endregion

        #region Properties

        public string Title { get { return header.Title; } }
        public string Author { get { return header.Author; } }
        public bool IsNaughty { get { return header.IsNaughty; } }
        public int Price { get { return (int)header.Price; } }
        public int PageCount { get { return header.PageCount; } }

        #endregion

        #region Structs & Enums

        /// <summary>
        /// Defines a book file header.
        /// </summary>
        public struct BookHeader
        {
            public string Title;                    // Title of book
            public string Author;                   // Internal name of book author
            public bool IsNaughty;                  // True if book has adult content
            public Byte[] NullValues;               // Null-value bytes
            public UInt32 Price;                    // Influences book price
            public UInt16 Unknown1;
            public UInt16 Unknown2;
            public UInt16 Unknown3;
            public UInt16 PageCount;                // Number of pages
            public UInt32[] PageOffsets;            // Array of page offsets, PageCount elements long
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts the internal Daggerfall "message" field to a filename that can be found in ARENA2/BOOKS
        /// </summary>
        /// <param name="message">The int32 message field that encodes the book's ID</param>
        /// <returns>A string that represents a file that should exist in the ARENA2/BOOKS directory (but this isn't verified on the filesystem)</returns>
        public static string messageToBookFilename(int message)
        {
            int decodedValue = message & 0xFF;
            return "BOK" + decodedValue.ToString("D5") + ".TXT";
        }

        /// <summary>
        /// Opens the specified book file.
        /// </summary>
        /// <param name="arena2">Arena2 path.</param>
        /// <param name="name">Name of book to open.</param>
        public bool OpenBook(string arena2, string name, FileUsage fileUsage = FileUsage.UseMemory, bool readOnly = true)
        {
            // Validate filename
            if (!name.StartsWith("BOK", StringComparison.InvariantCultureIgnoreCase) ||
                !name.EndsWith(".TXT", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            // Load book file
            string booksPath = Path.Combine(arena2, books);
            if (!bookFile.Load(Path.Combine(booksPath, name), fileUsage, readOnly))
                return false;

            // Read book
            try { ReadHeader(); }
            catch (EndOfStreamException e)
            {
                Debug.LogErrorFormat($"EndOfStreamException encountered for book {name}.\n" +
                    $"The last piece of data that was successfully read was {lastSuccessfulBookHeaderRead}.\n" +
                    $"{e.ToString()}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Open a book file from binary data.
        /// </summary>
        /// <param name="data">Byte array to parse as a book file.</param>
        /// <param name="name">Name to describe book.</param>
        public bool OpenBook(byte[] data, string name)
        {
            bookFile.Load(data, name);
            try { ReadHeader(name.StartsWith("BOK", StringComparison.OrdinalIgnoreCase)); }
            catch (EndOfStreamException e)
            {
                Debug.LogErrorFormat($"EndOfStreamException encountered for modded book {name}.\n" +
                    $"The last piece of data that was successfully read was {lastSuccessfulBookHeaderRead}.\n" +
                    $"{e.ToString()}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Reads the TextResource tokens for this page record.
        /// </summary>
        /// <param name="page">Page index.</param>
        /// <returns>TextResource token array.</returns>
        public TextFile.Token[] GetPageTokens(int page)
        {
            if (page < 0 || page >= PageCount)
                throw new IndexOutOfRangeException("BookFile: Invalid page index.");

            byte[] buffer = bookFile.GetBytes();

            return TextFile.ReadTokens(ref buffer, (int)header.PageOffsets[page], TextFile.Formatting.EndOfPage);
        }

        #endregion

        #region Private Methods

        bool ReadHeader(bool randomPrice = true)
        {
            BinaryReader reader = bookFile.GetReader();
            lastSuccessfulBookHeaderRead = "not even the Title";

            header = new BookHeader();
            header.Title = FileProxy.ReadCStringSkip(reader, 0, 64);
            lastSuccessfulBookHeaderRead = "the Book's Title";
            header.Author = FileProxy.ReadCStringSkip(reader, 0, 64);
            lastSuccessfulBookHeaderRead = "the Book's Author";
            header.IsNaughty = (FileProxy.ReadCStringSkip(reader, 0, 8) == naughty);
            lastSuccessfulBookHeaderRead = "the Book's Naughty flag";
            header.NullValues = reader.ReadBytes(88);
            lastSuccessfulBookHeaderRead = "the Book's Null Values";
            header.Price = reader.ReadUInt32();
            lastSuccessfulBookHeaderRead = "the Book's Price";
            header.Unknown1 = reader.ReadUInt16();
            lastSuccessfulBookHeaderRead = "the Book's Unknown1";
            header.Unknown2 = reader.ReadUInt16();
            lastSuccessfulBookHeaderRead = "the Book's Unknown2";
            header.Unknown3 = reader.ReadUInt16();
            lastSuccessfulBookHeaderRead = "the Book's Unknown3";
            header.PageCount = reader.ReadUInt16();
            lastSuccessfulBookHeaderRead = "the Book's Page Count";
            header.PageOffsets = new UInt32[header.PageCount];
            for (int i = 0; i < header.PageCount; i++)
            {
                header.PageOffsets[i] = reader.ReadUInt32();
                lastSuccessfulBookHeaderRead = "the Book's Page Offsets: Loop " + i + " of " + header.PageCount;
            }

            if (randomPrice)
            {
                // Overwrite price field using random seeded with first 4 bytes.
                // (See https://forums.dfworkshop.net/viewtopic.php?f=23&t=1576)
                reader.BaseStream.Position = 0;
                DFRandom.Seed = reader.ReadUInt32(); // first 4 bytes of book file as a uint
                header.Price = (uint)DFRandom.random_range_inclusive(300, 800);
            }
            lastSuccessfulBookHeaderRead = "";
            return true;
        }

        #endregion
    }
}
