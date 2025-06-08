// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

#region Using Statements
using UnityEngine;
using System;
using System.Text;
using System.IO;
#endregion

namespace DaggerfallConnect
{
    /// <summary>
    /// File usage enumeration.
    /// </summary>
    public enum FileUsage
    {
        /// <summary>Usage is not defined and will default to UseDisk if not specifed.</summary>
        Undefined,

        /// <summary>File is loaded and stored in a read-write memory buffer.</summary>
        UseMemory,

        /// <summary>File is opened as read-only from disk.</summary>
        UseDisk,
    }
}

namespace DaggerfallConnect.Utility
{
    /// <summary>
    /// This class abstracts a disk file or memory buffer to an object that can be emitted and read using binary streams.
    /// </summary>
    public class FileProxy
    {
        #region Class Variables

        /// <summary>
        /// Determines if file is read from disk (read-only file stream) or memory buffer (read-write memory stream).
        /// </summary>
        private FileUsage fileUsage;

        /// <summary>
        /// Has file been opened as read only or read-write.
        /// </summary>
        private bool isReadOnly;

        /// <summary>
        /// Stream to file when using FileUsage.UseDisk.
        /// </summary>
        private FileStream fileStream;

        /// <summary>
        /// Byte array when using FileUsage.UseMemory.
        /// </summary>
        private byte[] fileBuffer;

        /// <summary>
        /// Full path of managed file regardless of usage.
        /// </summary>
        private string managedFilePath = string.Empty;

        /// <summary>
        /// Last exception thrown.
        /// </summary>
        private Exception myLastException;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FileProxy()
        {
            fileUsage = FileUsage.Undefined;
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="filePath">Absolute path to file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public FileProxy(string filePath, FileUsage usage, bool readOnly)
        {
            Load(filePath, usage, readOnly);
        }

        /// <summary>
        /// Assign byte array constructor.
        /// </summary>
        /// <param name="data">Byte array to assign (usage will be set to FileUsage.useMemory).</param>
        /// <param name="name">Name, filename, or path  to describe memory buffer.</param>
        public FileProxy(byte[] data, string name)
        {
            Load(data, name);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Length of managed file in bytes.
        /// </summary>
        public int Length
        {
            get
            {
                switch (fileUsage)
                {
                    case FileUsage.UseDisk:
                        if (fileStream == null) return 0; else return (int)fileStream.Length;
                    case FileUsage.UseMemory:
                        if (fileBuffer == null) return 0; else return fileBuffer.Length;
                    default:
                        return 0;
                }
            }
        }

        /// <summary>
        /// Get full path and filename of managed file. Derived from filename for disk files, or specified at construction for managed files.
        /// </summary>
        public string FilePath
        {
            get { return managedFilePath; }
        }

        /// <summary>
        /// Get filename of managed file without path.
        /// </summary>
        public string FileName
        {
            get { return Path.GetFileName(managedFilePath); }
        }

        /// <summary>
        /// Get directory path of managed file without filename.
        /// </summary>
        public string Directory
        {
            get { return Path.GetDirectoryName(managedFilePath); }
        }

        /// <summary>
        /// Get the file usage in effect for this managed file.
        /// </summary>
        public FileUsage Usage
        {
            get { return fileUsage; }
        }

        /// <summary>
        /// Access allowed to file.
        /// </summary>
        public bool ReadOnly
        {
            get { return isReadOnly; }
        }

        /// <summary>
        /// Gets last exception thrown.
        /// </summary>
        public Exception LastException
        {
            get { return myLastException; }
        }

        /// <summary>
        /// Gets byte array when using FileUsage.UseMemory.
        /// </summary>
        public byte[] Buffer
        {
            get { return fileBuffer; }
        }

        /// <summary>
        /// Gets file stream when using FileUsage.UseDisk
        /// </summary>
        public FileStream FileStream
        {
            get { return fileStream; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load a file.
        /// </summary>
        /// <param name="filePath">Absolute path to file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Load(string filePath, FileUsage usage = FileUsage.UseMemory, bool readOnly = true)
        {
            // Determine file access settings
            FileAccess fileAccess;
            FileShare fileShare;
            if (readOnly)
            {
                fileAccess = FileAccess.Read;
                fileShare = FileShare.ReadWrite;
                isReadOnly = true;
            }
            else
            {
                fileAccess = FileAccess.ReadWrite;
                fileShare = FileShare.Read;
                isReadOnly = false;
            }

            // Load based on usage
            switch (usage)
            {
                case FileUsage.UseMemory:
                    return LoadMemory(filePath, fileAccess, fileShare);
                case FileUsage.UseDisk:
                default:
                    return LoadDisk(filePath, fileAccess, fileShare);
            }
        }

        /// <summary>
        /// Load a binary array.
        /// </summary>
        /// <param name="data">Byte array to assign (usage will be set to FileUsage.useMemory).</param>
        /// <param name="name">Name, filename, or path  to describe memory buffer.</param>
        public void Load(byte[] data, string name)
        {
            fileBuffer = data;
            managedFilePath = name;
            fileUsage = FileUsage.UseMemory;
        }

        /// <summary>
        /// Close open file and free memory used for buffer.
        /// </summary>
        public void Close()
        {
            // Exit if no file being managed
            if (String.IsNullOrEmpty(managedFilePath))
                return;

            // Close based on type
            if (fileUsage == FileUsage.UseMemory)
                fileBuffer = null;
            else
                fileStream.Close();

            // Clear filename
            managedFilePath = String.Empty;
        }

        /// <summary>
        /// Gets a binary reader to managed file.
        /// </summary>
        /// <returns>BinaryReader to managed file with UTF8 encoding.</returns>
        public BinaryReader GetReader()
        {
            switch (fileUsage)
            {
                case FileUsage.UseMemory:
                    return new BinaryReader(GetMemoryStream(), Encoding.UTF8);
                case FileUsage.UseDisk:
                    return new BinaryReader(GetFileStream(), Encoding.UTF8);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets a byte array from file.
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            if (Usage == FileUsage.UseMemory)
                return fileBuffer;
            else
                return GetBytes(0, Length);
        }

        /// <summary>
        /// Gets a byte array from file.
        /// </summary>
        /// <param name="position">Start position.</param>
        /// <param name="length">Read length.</param>
        /// <returns></returns>
        public byte[] GetBytes(long position, int length)
        {
            BinaryReader reader = GetReader(position);
            if (reader == null)
                return null;

            return reader.ReadBytes(length);
        }

        /// <summary>
        /// Get a binary reader to managed file starting at the specified position.
        /// </summary>
        /// <param name="position">Position to start in stream (number of bytes from start of file).</param>
        /// <returns>BinaryReader to managed file with UTF8 encoding and set to specified position.</returns>
        public BinaryReader GetReader(long position)
        {
            BinaryReader reader = GetReader();
            if (position < reader.BaseStream.Length)
                reader.BaseStream.Position = position;

            return reader;
        }

        /// <summary>
        /// Gets a binary writer to managed file.
        /// </summary>
        /// <returns>BinaryReader to managed file with UTF8 encoding.</returns>
        public BinaryWriter GetWriter()
        {
            switch (fileUsage)
            {
                case FileUsage.UseMemory:
                    return new BinaryWriter(GetMemoryStream(), Encoding.UTF8);
                case FileUsage.UseDisk:
                    return new BinaryWriter(GetFileStream(), Encoding.UTF8);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Get a binary writer to managed file starting at the specified position.
        /// </summary>
        /// <param name="position">Position to start in stream (number of bytes from start of file).</param>
        /// <returns>BinaryReader to managed file with UTF8 encoding and set to specified position.</returns>
        public BinaryWriter GetWriter(long position)
        {
            BinaryWriter writer = GetWriter();
            if (position < writer.BaseStream.Length)
                writer.BaseStream.Position = position;

            return writer;
        }

        /// <summary>
        /// Reads a UTF8 string of bytes from the managed file.
        /// </summary>
        /// <param name="position">Position to start reading in file (number of bytes from start of file).</param>
        /// <param name="readLength">Number of bytes to read (length=0 for null-terminated.)</param>
        /// <returns>String composed from bytes read (all NULLs are discarded).</returns>
        public string ReadCString(int position, int readLength)
        {
            // End position must be less than length of stream
            if (position + readLength > Length) return string.Empty;

            // Read from new stream
            BinaryReader reader = GetReader();
            reader.BaseStream.Position = position;
            return ReadCString(reader, readLength);
        }

        /// <summary>
        /// Reads a UTF8 string of length bytes from the binary reader.
        /// String may or may not be null terminated.
        /// </summary>
        /// <param name="reader">Source reader.</param>
        /// <param name="readLength">Number of bytes to read (0 for null-terminated).</param>
        /// <returns>String composed from bytes read.</returns>
        public static string ReadCString(BinaryReader reader, int readLength = 0)
        {
            // Find null terminator as Encoding.UTF8.GetString(bytes[]) does not null terminate
            if (readLength == 0)
            {
                long pos = reader.BaseStream.Position;
                while (reader.ReadByte() != 0) readLength++;
                reader.BaseStream.Position = pos;
            }

            return Encoding.UTF8.GetString(reader.ReadBytes(readLength)).TrimEnd('\0');
        }

        /// <summary>
        /// Reads a UTF8 string from binary reader then sets reader position to start + skipLength.
        /// Handles a special case where string may be null terminated but still require fixed byte stride.
        /// </summary>
        /// <param name="reader">Source reader.</param>
        /// <param name="readLength">Number of bytes to read (0 for null-terminated).</param>
        /// <param name="skipLength">Number of bytes to skip from start position after read.</param>
        /// <returns>String composed from bytes read (all NULLs are discarded).</returns>
        public static string ReadCStringSkip(BinaryReader reader, int readLength, int skipLength)
        {
            long pos = reader.BaseStream.Position;
            string str = ReadCString(reader, readLength);
            reader.BaseStream.Position = pos + skipLength;

            return str;
        }

        /// <summary>
        /// Find a string pattern inside file.
        /// </summary>
        /// <param name="pattern">String pattern to search for. Converted to UTF8 and is case sensitive.</param>
        /// <param name="position">Position to begin search.</param>
        /// <returns>Index of pattern found or -1 if not found.</returns>
        public int FindString(string pattern, int position = 0)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(pattern);

            for (int i = position; i < Buffer.Length; i++)
            {
                if (Buffer[i] == bytes[0] && ReadCString(i, pattern.Length) == pattern)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Reads next 2 bytes as a big-endian Int16.
        /// </summary>
        /// <param name="reader">Source reader.</param>
        /// <returns>Big-endian Int16</returns>
        public Int16 beReadInt16(BinaryReader reader)
        {
            return (Int16)endianSwapUInt16(reader.ReadUInt16());
        }

        /// <summary>
        /// Reads next 2 bytes as a big-endian UInt16.
        /// </summary>
        /// <param name="reader">Source reader.</param>
        /// <returns>Big-endian UInt16.</returns>
        public UInt16 beReadUInt16(BinaryReader reader)
        {
            return endianSwapUInt16(reader.ReadUInt16());
        }

        /// <summary>
        /// Reads next 4 bytes as a big-endian Int32.
        /// </summary>
        /// <param name="reader">Source reader.</param>
        /// <returns>Big-endian Int32.</returns>
        public Int32 beReadInt32(BinaryReader reader)
        {
            return (Int32)endianSwapUInt32(reader.ReadUInt32());
        }

        /// <summary>
        /// Reads next 4 bytes as a big-endian UInt32.
        /// </summary>
        /// <param name="reader">Source reader.</param>
        /// <returns>Big-endian Int32.</returns>
        public UInt32 beReadUInt32(BinaryReader reader)
        {
            return endianSwapUInt32(reader.ReadUInt32());
        }

        /// <summary>
        /// Swaps an unsigned 16-bit big-endian value to little-endian.
        /// </summary>
        /// <param name="value">Source reader.</param>
        /// <returns>Little-endian UInt16.</returns>
        public UInt16 endianSwapUInt16(UInt16 value)
        {
            return (UInt16)((value >> 8) | (value << 8));
        }

        /// <summary>
        /// Swaps an unsigned 32-bit big-endian value to little-endian.
        /// </summary>
        /// <param name="value">Source reader.</param>
        /// <returns>Little-endian UInt32.</returns>
        public UInt32 endianSwapUInt32(UInt32 value)
        {
            return (UInt32)((value >> 24) | ((value << 8) & 0x00FF0000) | ((value >> 8) & 0x0000FF00) | (value << 24));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets stream to disk file.
        /// </summary>
        /// <returns>FileStream object.</returns>
        private FileStream GetFileStream()
        {
            return fileStream;
        }

        /// <summary>
        /// Gets stream to memory file.
        /// </summary>
        /// <returns>FileStream object</returns>
        private MemoryStream GetMemoryStream()
        {
            return new MemoryStream(fileBuffer);
        }

        /// <summary>
        /// Loads a file into memory.
        /// Will first check Unity Resources folder for specified filename with a .bytes extensions
        /// and load that instead if available.
        /// </summary>
        /// <param name="filePath">Absolute path of file to load.</param>
        /// <param name="fileAccess">Defines access to file.</param>
        /// <param name="fileShare">Defines shared access to file.</param>
        /// <returns>True if successful, otherwise false.</returns>
        private bool LoadMemory(string filePath, FileAccess fileAccess, FileShare fileShare)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            // Unity cannot use Resources.Load in WebGL
            // TODO: Implement WWW resource loading
            return false;
#else

            // Attempt to locate in Unity Resources folder first
            string fileName = Path.GetFileName(filePath);
            TextAsset asset = Resources.Load<TextAsset>(fileName);
            if (asset != null)
            {
                fileBuffer = asset.bytes;
            }
            else
            {
                // File must exist
                if (!File.Exists(filePath))
                    return false;

                // Load file into memory buffer
                try
                {
                    FileStream file = File.Open(filePath, FileMode.Open, fileAccess, fileShare);
                    fileBuffer = new byte[file.Length];
                    if (file.Length != file.Read(fileBuffer, 0, (int)file.Length))
                        return false;

                    // Close file
                    file.Close();
                }
                catch (Exception e)
                {
                    myLastException = e;
                    Console.WriteLine(e.Message);
                    return false;
                }
            }

            // Store filename
            managedFilePath = filePath;

            // Set usage
            fileUsage = FileUsage.UseMemory;

            return true;
#endif
        }

        /// <summary>
        /// Opens a file from disk.
        /// </summary>
        /// <param name="filePath">Absolute path of file to load.</param>
        /// <param name="fileAccess">Defines access to file.</param>
        /// <param name="fileShare">Defines shared access to file.</param>
        /// <returns>True if successful, otherwise false.</returns>
        private bool LoadDisk(string filePath, FileAccess fileAccess, FileShare fileShare)
        {
            // File must exist
            if (!File.Exists(filePath))
                return false;

            // Open file
            try
            {
                fileStream = File.Open(filePath, FileMode.Open, fileAccess, fileShare);
                if (fileStream == null)
                    return false;
            }
            catch (Exception e)
            {
                myLastException = e;
                Console.WriteLine(e.Message);
                return false;
            }

            // Store filename
            managedFilePath = filePath;

            // Set usage
            fileUsage = FileUsage.UseDisk;

            return true;
        }

        #endregion
    }
}
