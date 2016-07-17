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

using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect.Utility;

namespace DaggerfallConnect.Save
{
    /// <summary>
    /// Header of SAVETREE.DAT file.
    /// This header includes the CharacterPosition record data.
    /// </summary>
    public class SaveTreeHeader
    {
        // Consts
        public const int HeaderLength = 18;
        public const int VersionNumber = 0x26;

        // Public fields
        public long StreamPosition;                             // Starting position in file stream
        public byte[] RawData;                                  // Raw byte data read from stream
        public Byte Version;                                    // Version - must be 0x26
        public CharacterPositionRecord CharacterPosition;       // Character position
        public UInt16 Unknown;                                  // Unknown

        /// <summary>
        /// Reads header from file.
        /// </summary>
        /// <param name="reader">BinaryReader into file.</param>
        public void Read(BinaryReader reader)
        {
            StreamPosition = reader.BaseStream.Position;
            RawData = reader.ReadBytes(HeaderLength);
            ReadRawData();
        }

        /// <summary>
        /// Writes header to file.
        /// </summary>
        /// <param name="writer">BinaryWriter into file.</param>
        public void Save(BinaryWriter writer)
        {
            WriteRawData();
            writer.BaseStream.Position = StreamPosition;
            writer.Write(RawData);
        }

        #region Private Methods

        void ReadRawData()
        {
            MemoryStream stream = new MemoryStream(RawData);
            BinaryReader reader = new BinaryReader(stream, Encoding.UTF8);

            // Read Version - must be 0x26
            Version = reader.ReadByte();
            if (Version != VersionNumber)
                throw new Exception("SaveTree file has an invalid version number, must be 0x26.");

            // Read CharacterPosition.RecordType - must be 0x01
            CharacterPosition = new CharacterPositionRecord();
            CharacterPosition.RecordType = reader.ReadByte();
            if (CharacterPosition.RecordType != (int)RecordTypes.CharacterPosition)
                throw new Exception("Expected CharacterPosition in SaveTreeHeader has an invalid record type, must be 0x01.");

            // Read CharacterPosition.Unknown
            CharacterPosition.Unknown = reader.ReadUInt16();

            // Read CharacterPosition.Position
            CharacterPosition.Position = SaveTree.ReadPosition(reader);

            // Read Unknown
            Unknown = reader.ReadUInt16();

            reader.Close();
        }

        void WriteRawData()
        {
            MemoryStream stream = new MemoryStream(RawData, true);
            BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8);

            // Write Version
            writer.Write(Version);

            // Write CharacterPosition.RecordType
            writer.Write(CharacterPosition.RecordType);

            // Write CharacterPosition.Unknown
            writer.Write(CharacterPosition.Unknown);

            // Write CharacterPosition.Position
            SaveTree.WritePosition(writer, CharacterPosition.Position);

            // Write Unknown
            writer.Write(Unknown);

            writer.Close();
        }

        #endregion
    }
}