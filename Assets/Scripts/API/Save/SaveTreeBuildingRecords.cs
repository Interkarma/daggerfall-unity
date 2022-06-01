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

using System.IO;

namespace DaggerfallConnect.Save
{
    /// <summary>
    /// Building data following header.
    /// </summary>
    public class SaveTreeBuildingRecords
    {
        long streamPosition;
        int recordLength;
        int numberOfBuildings;
        DFLocation.BuildingData[] recordData;

        public long StreamPosition
        {
            get { return streamPosition; }
        }

        public int RecordLength
        {
            get { return recordLength; }
        }

        public int NumberOfBuildings
        {
            get { return numberOfBuildings; }
        }

        public DFLocation.BuildingData[] RecordData
        {
            get { return recordData; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="reader">Reader positioned at start of binary data.</param>
        public SaveTreeBuildingRecords(BinaryReader reader)
        {
            Open(reader);
        }

        void Open(BinaryReader reader)
        {
            streamPosition = reader.BaseStream.Position;
            recordLength = reader.ReadInt32();

            if (recordLength > 0)
            {
                numberOfBuildings = (recordLength / 26);
            }
            else
            {
                numberOfBuildings = 0;
            }

            recordData = new DFLocation.BuildingData[numberOfBuildings];
            for (int i = 0; i < numberOfBuildings; i++)
            {
                recordData[i].NameSeed = reader.ReadUInt16();
                recordData[i].ServiceTimeLimit = reader.ReadUInt32();
                recordData[i].Unknown = reader.ReadUInt16();
                recordData[i].Unknown2 = reader.ReadUInt16();
                recordData[i].Unknown3 = reader.ReadUInt32();
                recordData[i].Unknown4 = reader.ReadUInt32();
                recordData[i].FactionId = reader.ReadUInt16();
                recordData[i].Sector = reader.ReadInt16();
                recordData[i].LocationId = reader.ReadUInt16();
                recordData[i].BuildingType = (DFLocation.BuildingTypes)reader.ReadByte();
                recordData[i].Quality = reader.ReadByte();
            }
        }
    }
}
