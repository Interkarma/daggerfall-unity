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

using System;
using System.Text;
using System.IO;

namespace DaggerfallConnect.Save
{
    /// <summary>
    /// Header of SAVETREE.DAT file.
    /// </summary>
    public class SaveTreeHeader
    {
        // Consts
        public const int HeaderLength = 19;
        public const int VersionNumber = 0x126;

        // Public fields
        public long StreamPosition;                             // Starting position in file stream
        public byte[] RawData;                                  // Raw byte data read from stream
        public int Version;                                     // Version - must be 0x126
        public HeaderCharacterPositionRecord CharacterPosition; // Character position
        public UInt16 MapID;                                    // ID of current map. 0xFFFF if not in a location.
        public Byte Environment;                                // Player environment. 1 = outside, 2 = building, 3 = dungeon

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

            // Read Version - must be 0x126
            Version = reader.ReadInt32();
            if (Version != VersionNumber)
                throw new Exception("SaveTree file has an invalid version number, must be 0x126.");

            // Read character position. A character position record later in the file (record type 0x04) is used for positioning the player, not this.
            // Currently not clear what classic uses this one for.
            CharacterPosition = new HeaderCharacterPositionRecord();
            CharacterPosition.Position = SaveTree.ReadPosition(reader);

            // Read MapID
            MapID = reader.ReadUInt16();

            // Read Environment
            Environment = reader.ReadByte();

            reader.Close();
        }

        void WriteRawData()
        {
            MemoryStream stream = new MemoryStream(RawData, true);
            BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8);

            // Write Version
            writer.Write(Version);

            // Write CharacterPosition.Position
            SaveTree.WritePosition(writer, CharacterPosition.Position);

            // Write MapID
            writer.Write(MapID);

            // Write Environment
            writer.Write(Environment);

            writer.Close();
        }

        #endregion
    }
}
