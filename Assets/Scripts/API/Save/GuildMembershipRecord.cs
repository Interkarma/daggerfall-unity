// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Allofich
// Contributors:
//
// Notes:
//

using System;
using System.IO;

namespace DaggerfallConnect.Save
{
    /// <summary>
    /// Guild membership record.
    /// SaveTreeRecordTypes = 0x0A
    /// </summary>
    public class GuildMembershipRecord : SaveTreeBaseRecord
    {
        #region Fields

        GuildMembershipRecordData parsedData;

        #endregion

        #region Properties

        public GuildMembershipRecordData ParsedData
        {
            get { return parsedData; }
            set { parsedData = value; }
        }

        #endregion

        #region Structures and Enumerations

        /// <summary>
        /// Stores native data exactly as read from save file.
        /// </summary>
        public struct GuildMembershipRecordData
        {
            public Byte rank;                       // Rank in guild, starting at 0
            public Byte notedByGuild;               // Under research. Increased by doing quests? Related to rewards, promotions.
            public Byte guildType;                  // Guild type. Used in classic for %gdd, %lev and %pct macros, choosing guild quest filename prefix, and blessings (not used in classic)
            public UInt16 factionID;
            public UInt32 timeOfLastRankChange;     // Time of joining guild or last promotion or demotion
            public Byte unused;                     // Seems to be unused
            public Byte blessingMagnitude;          // Amount of blessing (not used in classic)
            public UInt16 unused2;                  // Seems to be unused
        }

        #endregion

        #region Constructors

        public GuildMembershipRecord()
        {
        }

        public GuildMembershipRecord(BinaryReader reader, int length)
            : base(reader, length)
        {
            ReadNativeGuildMembershipData();
        }

        #endregion

        #region Public Methods

        public void CopyTo(GuildMembershipRecord other)
        {
            // Copy base record data
            base.CopyTo(other);

            // Copy parsed data
            other.parsedData = this.parsedData;
        }

        #endregion

        #region Private Methods

        void ReadNativeGuildMembershipData()
        {
            // Must be a guild membership type
            if (recordType != RecordTypes.GuildMembership &&
                recordType != RecordTypes.OldGuild)
                return;

            // Prepare stream
            MemoryStream stream = new MemoryStream(RecordData);
            BinaryReader reader = new BinaryReader(stream);

            // Read native data
            parsedData = new GuildMembershipRecordData();
            parsedData.rank = reader.ReadByte();
            parsedData.notedByGuild = reader.ReadByte();
            parsedData.guildType = reader.ReadByte();
            parsedData.factionID = reader.ReadUInt16();
            parsedData.timeOfLastRankChange = reader.ReadUInt32();
            parsedData.unused = reader.ReadByte();
            parsedData.blessingMagnitude = reader.ReadByte();
            parsedData.unused2 = reader.ReadUInt16();

            // Close stream
            reader.Close();
        }

        #endregion
    }
}
