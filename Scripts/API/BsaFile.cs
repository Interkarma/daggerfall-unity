// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

#region Using Statements
using System;
using System.Text;
using System.IO;
using DaggerfallConnect.Utility;
#endregion

namespace DaggerfallConnect.Arena2
{
    /// <summary>
    /// Connects to a Daggerfall BSA file and extracts records as binary data.
    /// </summary>
    public class BsaFile
    {
        #region Class Variables

        /// <summary>
        /// Abstracts BSA file to a managed disk or memory stream.
        /// </summary>
        private FileProxy managedFile = new FileProxy();

        /// <summary>
        /// Contains the BSA file header data.
        /// </summary>
        private FileHeader header;

        /// <summary>
        /// Array for directories where each item has a string for a name.
        /// </summary>
        private NameRecordDescriptor[] nameRecordDirectory;

        /// <summary>
        /// Array for directories where each item has a number for a name.
        /// </summary>
        private NumberRecordDescriptor[] numberRecordDirectory;

        #endregion

        #region Class Structures

        /// <summary>
        /// Possible directory types enumeration.
        /// </summary>
        public enum DirectoryTypes
        {
            /// <summary>Each directory entry is a string.</summary>
            NameRecord = 0x0100,

            /// <summary>Each directory entry is an unsigned integer.</summary>
            NumberRecord = 0x0200,
        }

        /// <summary>
        /// Represents a BSA file header.
        /// </summary>
        private struct FileHeader
        {
            public long Position;
            public Int16 DirectoryCount;
            public DirectoryTypes DirectoryType;
            public long FirstRecordPosition;
        }

        /// <summary>
        /// A name record directory descriptor.
        /// </summary>
        private struct NameRecordDescriptor
        {
            public long Position;
            public String RecordName;
            public Int32 RecordSize;
            public long RecordPosition;
        }

        /// <summary>
        /// A number record directory descriptor.
        /// </summary>
        private struct NumberRecordDescriptor
        {
            public long Position;
            public UInt32 RecordId;
            public String RecordName;
            public Int32 RecordSize;
            public long RecordPosition;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BsaFile()
        {
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="filePath">Absolute path to BSA file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public BsaFile(string filePath, FileUsage usage, bool readOnly)
        {
            Load(filePath, usage, readOnly);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Number records in the loaded BSA file.
        /// </summary>
        public int Count
        {
            get { return header.DirectoryCount; }
        }

        /// <summary>
        /// Type of directory used for this BSA file.
        /// </summary>
        public DirectoryTypes DirectoryType
        {
            get { return header.DirectoryType; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load BSA file.
        /// </summary>
        /// <param name="filePath">Absolute path to BSA file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Load(string filePath, FileUsage usage, bool readOnly)
        {
            // Ensure filename ends with .BSA
            filePath = filePath.ToUpper();
            if (!filePath.EndsWith(".BSA") &&
                !filePath.EndsWith(".SND"))
                return false;

            // Load file into memory
            if (!managedFile.Load(filePath, usage, readOnly))
                return false;

            // Read file
            if (!Read())
                return false;

            return true;
        }

        /// <summary>
        /// Finds index of a named record.
        /// </summary>
        /// <param name="name">Name to search for.</param>
        /// <returns>Index of name, or -1 if not found.</returns>
        public int GetRecordIndex(string name)
        {
            // Validate
            if (header.DirectoryType != DirectoryTypes.NameRecord)
                return -1;

            // Search for name
            for (int i = 0; i < header.DirectoryCount; i++)
            {
                if (nameRecordDirectory[i].RecordName == name)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Gets length of a record in bytes.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <returns>Length of records in bytes.</returns>
        public int GetRecordLength(int record)
        {
            // Validate
            if (record >= header.DirectoryCount)
                return 0;

            // Return length of this record
            switch (header.DirectoryType)
            {
                case DirectoryTypes.NameRecord:
                    return nameRecordDirectory[record].RecordSize;
                case DirectoryTypes.NumberRecord:
                    return numberRecordDirectory[record].RecordSize;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Gets name of a record as a string. This method is valid for BSA files with either a number or name-based directory.
        /// </summary>
        /// <param name="record">Name of record.</param>
        /// <returns>Name of record as string.</returns>
        public string GetRecordName(int record)
        {
            // Validate
            if (record >= header.DirectoryCount)
                return string.Empty;

            // Return name of this record
            switch (header.DirectoryType)
            {
                case DirectoryTypes.NameRecord:
                    return nameRecordDirectory[record].RecordName;
                case DirectoryTypes.NumberRecord:
                    return numberRecordDirectory[record].RecordName;
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets ID of a number record. This method is valid only for BSA files with a number-based directory.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <returns>ID of record.</returns>
        public uint GetRecordId(int record)
        {
            // Validate
            if (record >= header.DirectoryCount || header.DirectoryType != DirectoryTypes.NumberRecord)
                return 0;

            return numberRecordDirectory[record].RecordId;
        }

        /// <summary>
        /// Retrieves a record as a byte array.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <returns>Byte array containing record data.</returns>
        public byte[] GetRecordBytes(int record)
        {
            // Validate
            if (record >= header.DirectoryCount)
                return null;

            // Read record data into buffer
            BinaryReader reader = managedFile.GetReader(GetRecordPosition(record));
            byte[] buffer = reader.ReadBytes(GetRecordLength(record));

            return buffer;
        }

        /// <summary>
        /// Save new record data back to BSA file. WARNING: This will modify the BSA file. Ensure you have backups.
        ///  BSA file must have been opened with ReadOnly flag disabled.
        /// </summary>
        /// <param name="record">The record to save back.</param>
        /// <param name="buffer">The data to save back. This must be the same length as record data.</param>
        public void RewriteRecord(int record, byte[] buffer)
        {
            // Check data lengths
            if (buffer.Length != GetRecordLength(record))
                throw new Exception("Input array length and BSA record length do not match.");

            // Ensure file is writable
            if (managedFile.ReadOnly)
                throw new Exception(string.Format("BSA file '{0}' is read only.", managedFile.FilePath));

            // Ensure file usage is disk
            if (managedFile.Usage != FileUsage.UseDisk)
                throw new Exception("BSA file usage is not set to FileUsage.UseDisk. Can only save back to a disk file.");

            // Write data back into file
            BinaryWriter writer = managedFile.GetWriter(GetRecordPosition(record));
            writer.Write(buffer);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Retrieves a record as FileProxy object with usage of FileUsage.useMemory.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <returns>FileProxy object.</returns>
        internal FileProxy GetRecordProxy(int record)
        {
            // Validate
            if (record >= header.DirectoryCount)
                return null;

            // Read record data into buffer
            BinaryReader reader = managedFile.GetReader(GetRecordPosition(record));
            byte[] buffer = reader.ReadBytes(GetRecordLength(record));

            return new FileProxy(buffer, GetRecordName(record));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get position (offset) of record in BSA file.
        /// </summary>
        /// <param name="record">Index of record</param>
        private long GetRecordPosition(int record)
        {
            switch (header.DirectoryType)
            {
                case DirectoryTypes.NameRecord:
                    return nameRecordDirectory[record].RecordPosition;
                case DirectoryTypes.NumberRecord:
                    return numberRecordDirectory[record].RecordPosition;
                default:
                    return -1;
            }
        }

        #endregion

        #region Readers

        /// <summary>
        /// Read file.
        /// </summary>
        private bool Read()
        {
            try
            {
                // Step through file
                BinaryReader reader = managedFile.GetReader();
                ReadHeader(reader);
                ReadDirectory(reader);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Read header data.
        /// </summary>
        /// <param name="reader">Reader to stream.</param>
        private void ReadHeader(BinaryReader reader)
        {
            reader.BaseStream.Position = 0;
            header.Position = 0;
            header.DirectoryCount = reader.ReadInt16();
            header.DirectoryType = (DirectoryTypes)reader.ReadUInt16();
            header.FirstRecordPosition = reader.BaseStream.Position;
        }

        /// <summary>
        /// Read directory items.
        /// </summary>
        /// <param name="reader">Reader to stream.</param>
        private void ReadDirectory(BinaryReader reader)
        {
            if (header.DirectoryType == DirectoryTypes.NameRecord)
            {
                // Create name record directory
                nameRecordDirectory = new NameRecordDescriptor[header.DirectoryCount];

                // Read directory
                long recordPosition = header.FirstRecordPosition;
                reader.BaseStream.Position = managedFile.Length - 18 * header.DirectoryCount;
                for (int i = 0; i < header.DirectoryCount; i++)
                {
                    nameRecordDirectory[i].Position = reader.BaseStream.Position;
                    nameRecordDirectory[i].RecordName = managedFile.ReadCString(reader, 0);
                    reader.BaseStream.Position = nameRecordDirectory[i].Position + 14;
                    nameRecordDirectory[i].RecordSize = reader.ReadInt32();
                    nameRecordDirectory[i].RecordPosition = recordPosition;
                    recordPosition += nameRecordDirectory[i].RecordSize;
                }
            }
            else if (header.DirectoryType == DirectoryTypes.NumberRecord)
            {
                // Create number record directory
                numberRecordDirectory = new NumberRecordDescriptor[header.DirectoryCount];

                // Read directory
                long recordPosition = header.FirstRecordPosition;
                reader.BaseStream.Position = managedFile.Length - 8 * header.DirectoryCount;
                for (int i = 0; i < header.DirectoryCount; i++)
                {
                    numberRecordDirectory[i].Position = reader.BaseStream.Position;
                    numberRecordDirectory[i].RecordId = reader.ReadUInt32();
                    numberRecordDirectory[i].RecordName = numberRecordDirectory[i].RecordId.ToString();
                    numberRecordDirectory[i].RecordSize = reader.ReadInt32();
                    numberRecordDirectory[i].RecordPosition = recordPosition;
                    recordPosition += numberRecordDirectory[i].RecordSize;
                }
            }
            else
            {
                throw new Exception("BSA file has an invalid DirectoryType.");
            }
        }

        #endregion
    }
}
