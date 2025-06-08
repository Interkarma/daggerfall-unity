using System;
using System.Collections.Generic;
using DaggerfallConnect;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class PositionUpdater
    {
        private readonly Random random = new Random();

        public DFBlock UpdateDuplicatedPositions(DFBlock loadedBlock)
        {
            var existingPositions = new HashSet<long>();

            // RMB Flats
            for (var i = 0; i < loadedBlock.RmbBlock.MiscFlatObjectRecords.Length; i++)
            {
                var record = loadedBlock.RmbBlock.MiscFlatObjectRecords[i];
                if (existingPositions.Contains(record.Position))
                {
                    record.Position = GenerateUniquePosition(existingPositions);
                }
                existingPositions.Add(record.Position);
                loadedBlock.RmbBlock.MiscFlatObjectRecords[i] = record;
            }

            // Buildings
            for (var buildingIndex = 0; buildingIndex < loadedBlock.RmbBlock.SubRecords.Length; buildingIndex++)
            {
                var building = loadedBlock.RmbBlock.SubRecords[buildingIndex];
                // Exterior Flats
                for (var extFlatIndex = 0;
                     extFlatIndex < building.Exterior.BlockFlatObjectRecords.Length;
                     extFlatIndex++)
                {
                    var extFlat = building.Exterior.BlockFlatObjectRecords[extFlatIndex];
                    if (existingPositions.Contains(extFlat.Position))
                    {
                        extFlat.Position = GenerateUniquePosition(existingPositions);
                    }
                    existingPositions.Add(extFlat.Position);
                    building.Exterior.BlockFlatObjectRecords[extFlatIndex] = extFlat;
                }

                // Exterior People
                for (var extPeopleIndex = 0;
                     extPeopleIndex < building.Exterior.BlockPeopleRecords.Length;
                     extPeopleIndex++)
                {
                    var extPerson = building.Exterior.BlockPeopleRecords[extPeopleIndex];
                    if (existingPositions.Contains(extPerson.Position))
                    {
                        extPerson.Position = GenerateUniquePosition(existingPositions);
                    }
                    existingPositions.Add(extPerson.Position);
                    building.Exterior.BlockPeopleRecords[extPeopleIndex] = extPerson;
                }

                // Exterior Doors
                for (var extDoorsIndex = 0;
                     extDoorsIndex < building.Exterior.BlockDoorRecords.Length;
                     extDoorsIndex++)
                {
                    var extDoor = building.Exterior.BlockDoorRecords[extDoorsIndex];
                    if (existingPositions.Contains(extDoor.Position))
                    {
                        extDoor.Position = GenerateUniquePosition(existingPositions);
                    }
                    existingPositions.Add(extDoor.Position);
                    building.Exterior.BlockDoorRecords[extDoorsIndex] = extDoor;
                }

                // Internal Flats
                for (var intFlatIndex = 0;
                     intFlatIndex < building.Interior.BlockFlatObjectRecords.Length;
                     intFlatIndex++)
                {
                    var intFlat = building.Interior.BlockFlatObjectRecords[intFlatIndex];
                    if (existingPositions.Contains(intFlat.Position))
                    {
                        intFlat.Position = GenerateUniquePosition(existingPositions);
                    }
                    existingPositions.Add(intFlat.Position);
                    building.Interior.BlockFlatObjectRecords[intFlatIndex] = intFlat;
                }

                // Interior People
                for (var intPeopleIndex = 0;
                     intPeopleIndex < building.Interior.BlockPeopleRecords.Length;
                     intPeopleIndex++)
                {
                    var intPerson = building.Interior.BlockPeopleRecords[intPeopleIndex];
                    if (existingPositions.Contains(intPerson.Position))
                    {
                        intPerson.Position = GenerateUniquePosition(existingPositions);
                    }
                    existingPositions.Add(intPerson.Position);
                    building.Interior.BlockPeopleRecords[intPeopleIndex] = intPerson;
                }

                // Interior Doors
                for (var intDoorsIndex = 0;
                     intDoorsIndex < building.Interior.BlockDoorRecords.Length;
                     intDoorsIndex++)
                {
                    var intDoor = building.Interior.BlockDoorRecords[intDoorsIndex];
                    if (existingPositions.Contains(intDoor.Position))
                    {
                        intDoor.Position = GenerateUniquePosition(existingPositions);
                    }
                    existingPositions.Add(intDoor.Position);
                    building.Interior.BlockDoorRecords[intDoorsIndex] = intDoor;
                }

                loadedBlock.RmbBlock.SubRecords[buildingIndex] = building;
            }

            return loadedBlock;
        }

        private int GenerateUniquePosition(HashSet<long> existingIds)
        {
            int newId;
            do
            {
                newId = random.Next();
            } while (existingIds.Contains(newId));

            return newId;
        }
    }
}