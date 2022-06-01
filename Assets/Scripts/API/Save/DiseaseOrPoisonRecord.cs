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
    /// Poison or disease record.
    /// SaveTreeRecordTypes = 0x0B
    /// </summary>
    public class DiseaseOrPoisonRecord : SaveTreeBaseRecord
    {
        #region Fields

        DiseaseOrPoisonRecordData parsedData;

        #endregion

        #region Properties

        public DiseaseOrPoisonRecordData ParsedData
        {
            get { return parsedData; }
            set { parsedData = value; }
        }

        #endregion

        #region Structures and Enumerations

        /// <summary>
        /// Stores native data exactly as read from save file.
        /// </summary>
        public struct DiseaseOrPoisonRecordData
        {
            public Byte ID;                         // Type of disease/poison
            public UInt16 damagesSTR;               // If 1 disease damages this stat, if 0 it doesn't
            public UInt16 damagesINT;               // Same as above
            public UInt16 damagesWIL;               // Same as above
            public UInt16 damagesAGI;               // Same as above
            public UInt16 damagesEND;               // Same as above
            public UInt16 damagesPER;               // Same as above
            public UInt16 damagesSPD;               // Same as above
            public UInt16 damagesLUC;               // Same as above
            public UInt16 damagesHEA;               // Same as above
            public UInt16 damagesFAT;               // Same as above
            public UInt16 damagesSPL;               // Same as above
            public UInt16 minDamage;                // Minimum damage done to each stat on a symptom application
            public UInt16 maxDamage;                // Maximum damage done to each stat on a symptom application
            public UInt16 daysOfSymptomsLeft;       // 0xFF = never-ending, 0xFE = symptoms over, Other values = number of symptom applications (days) left
            public UInt16 incubationOver;           // 0 = hasn't applied symptoms yet, 1 = has begun applying symptoms
            public UInt16 totaldamageSTR;           // Total damage this disease has done to this stat
            public UInt16 totaldamageINT;           // Same as above
            public UInt16 totaldamageWIL;           // Same as above
            public UInt16 totaldamageAGI;           // Same as above
            public UInt16 totaldamageEND;           // Same as above
            public UInt16 totaldamagePER;           // Same as above
            public UInt16 totaldamageSPD;           // Same as above
            public UInt16 totaldamageLUC;           // Same as above
        }

        #endregion

        #region Constructors

        public DiseaseOrPoisonRecord()
        {
        }

        public DiseaseOrPoisonRecord(BinaryReader reader, int length)
            : base(reader, length)
        {
            ReadNativeDiseaseOrPoisonData();
        }

        #endregion

        #region Public Methods

        public void CopyTo(DiseaseOrPoisonRecord other)
        {
            // Copy base record data
            base.CopyTo(other);

            // Copy parsed data
            other.parsedData = this.parsedData;
        }

        #endregion

        #region Private Methods

        void ReadNativeDiseaseOrPoisonData()
        {
            // Must be a disease/poison type
            if (recordType != RecordTypes.DiseaseOrPoison)
                return;

            // Prepare stream
            MemoryStream stream = new MemoryStream(RecordData);
            BinaryReader reader = new BinaryReader(stream);

            // Read native data
            parsedData = new DiseaseOrPoisonRecordData();
            parsedData.ID = reader.ReadByte();
            parsedData.damagesSTR = reader.ReadUInt16();
            parsedData.damagesINT = reader.ReadUInt16();
            parsedData.damagesWIL = reader.ReadUInt16();
            parsedData.damagesAGI = reader.ReadUInt16();
            parsedData.damagesEND = reader.ReadUInt16();
            parsedData.damagesPER = reader.ReadUInt16();
            parsedData.damagesSPD = reader.ReadUInt16();
            parsedData.damagesLUC = reader.ReadUInt16();
            parsedData.damagesHEA = reader.ReadUInt16();
            parsedData.damagesFAT = reader.ReadUInt16();
            parsedData.damagesSPL = reader.ReadUInt16();
            parsedData.minDamage = reader.ReadUInt16();
            parsedData.maxDamage = reader.ReadUInt16();
            parsedData.daysOfSymptomsLeft = reader.ReadUInt16();
            parsedData.incubationOver = reader.ReadUInt16();
            parsedData.totaldamageSTR = reader.ReadUInt16();
            parsedData.totaldamageINT = reader.ReadUInt16();
            parsedData.totaldamageWIL = reader.ReadUInt16();
            parsedData.totaldamageAGI = reader.ReadUInt16();
            parsedData.totaldamageEND = reader.ReadUInt16();
            parsedData.totaldamagePER = reader.ReadUInt16();
            parsedData.totaldamageSPD = reader.ReadUInt16();
            parsedData.totaldamageLUC = reader.ReadUInt16();

            // Close stream
            reader.Close();
        }

        #endregion
    }
}
