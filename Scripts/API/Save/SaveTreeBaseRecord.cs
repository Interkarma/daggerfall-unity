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
    /// Base record of SAVETREE.DAT.
    /// </summary>
    public class SaveTreeBaseRecord
    {
        #region Fields

        // Constants
        public const int RecordRootLength = 71;
        public const int DungeonDataLengthMultiplier = 39;

        // Source data from file stream
        protected long streamPosition;
        protected int streamLength;
        protected byte[] streamData;

        // RecordBase data
        protected RecordTypes recordType;
        protected RecordRoot recordRoot = new RecordRoot();

        // Tree management
        protected SaveTreeBaseRecord parent;
        protected List<SaveTreeBaseRecord> children = new List<SaveTreeBaseRecord>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets position of data in source file stream.
        /// </summary>
        public long StreamPosition
        {
            get { return streamPosition; }
        }

        /// <summary>
        /// Gets length of raw data in source file stream.
        /// </summary>
        public int StreamLength
        {
            get { return streamLength; }
        }

        /// <summary>
        /// Gets raw data as read from file stream.
        /// </summary>
        public byte[] StreamData
        {
            get { return streamData; }
        }

        /// <summary>
        /// Gets type of this record.
        /// </summary>
        public RecordTypes RecordType
        {
            get { return recordType; }
        }

        /// <summary>
        /// Gets or sets RecordRoot data of this record.
        /// </summary>
        public RecordRoot RecordRoot
        {
            get { return recordRoot; }
            set { recordRoot = value; }
        }

        /// <summary>
        /// Gets length of actual record data, excluding RecordRoot header.
        /// </summary>
        public int RecordLength
        {
            get { return (streamLength > 0) ? streamLength - RecordRootLength : 0; }
        }

        /// <summary>
        /// Gets or sets actual record data, excluding RecordRoot header.
        /// </summary>
        public byte[] RecordData
        {
            get { return GetRecordData(); }
            set { SetRecordData(value); }
        }

        /// <summary>
        /// Gets or sets parent of this record.
        /// </summary>
        public SaveTreeBaseRecord Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        /// <summary>
        /// Gets children of this record.
        /// </summary>
        public List<SaveTreeBaseRecord> Children
        {
            get { return children; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public SaveTreeBaseRecord()
        {
        }

        /// <summary>
        /// Reader constructor.
        /// </summary>
        /// <param name="reader">Reader positioned at start of record data.</param>
        /// <param name="length">Length of data record to read.</param>
        public SaveTreeBaseRecord(BinaryReader reader, int length)
        {
            Open(reader, length);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reads record data from stream.
        /// </summary>
        /// <param name="reader">Reader positioned at start of record data.</param>
        /// <param name="length">Length of record data to read.</param>
        public virtual void Open(BinaryReader reader, int length)
        {
            streamPosition = reader.BaseStream.Position;
            streamLength = length;

            // Cannot read zero-length records
            if (length <= 0)
                return;

            // Peek record type and adjust for dungeon size
            recordType = SaveTree.PeekRecordType(reader);
            if (recordType == RecordTypes.DungeonInformation)
                streamLength *= DungeonDataLengthMultiplier;

            // Read raw record data
            streamData = reader.ReadBytes(streamLength);

            // Read RecordRoot data from start of memory buffer
            ReadRecordRoot();
        }

        #endregion

        #region Private Methods

        byte[] GetRecordData()
        {
            byte[] data = new byte[RecordLength];
            Array.Copy(streamData, RecordRootLength, data, 0, RecordLength);

            return data;
        }

        void SetRecordData(byte[] data)
        {
            if (data.Length != RecordLength)
                throw new Exception("New record data length != original record data length.");

            Array.Copy(data, 0, streamData, RecordRootLength, RecordLength);
        }

        void ReadRecordRoot()
        {
            MemoryStream stream = new MemoryStream(streamData);
            BinaryReader reader = new BinaryReader(stream);

            // Must have RecordRootLength of bytes to read or something has gone wrong
            if (stream.Length < RecordRootLength)
            {
                stream.Close();
                return;
            }

            // Position
            reader.BaseStream.Position = 6;
            recordRoot.Position = SaveTree.ReadPosition(reader);

            // RecordID
            reader.BaseStream.Position = 30;
            recordRoot.RecordID = reader.ReadUInt32();

            // QuestID
            reader.BaseStream.Position = 37;
            recordRoot.QuestID = reader.ReadByte();

            // ParentRecordID
            reader.BaseStream.Position = 38;
            recordRoot.ParentRecordID = reader.ReadUInt32();

            // ParentRecordType
            reader.BaseStream.Position = 66;
            recordRoot.ParentRecordType = (RecordTypes)reader.ReadInt32();

            reader.Close();
        }

        #endregion
    }
}