// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using System.IO;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Utility.AssetInjection;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor.BuildingPresets
{
    public class BuildingPreset
    {
        private readonly DFBlock house1;
        private readonly DFBlock house2;
        private readonly DFBlock house3;
        private readonly DFBlock house4;
        private readonly DFBlock house5;
        private readonly DFBlock house6;
        private readonly DFBlock houseForSale;
        private readonly DFBlock tavern;
        private readonly DFBlock guildHall;
        private readonly DFBlock temple;
        private readonly DFBlock furnitureStore;
        private readonly DFBlock bank;
        private readonly DFBlock generalStore;
        private readonly DFBlock pawnShop;
        private readonly DFBlock armorer;
        private readonly DFBlock weaponSmith;
        private readonly DFBlock clothingStore;
        private readonly DFBlock alchemist;
        private readonly DFBlock gemStore;
        private readonly DFBlock bookseller;
        private readonly DFBlock library;
        private readonly DFBlock palace;
        private readonly DFBlock town23;
        private readonly DFBlock ship;

        private ClimateBases climate;
        private ClimateSeason season;
        private WindowStyle windowStyle;

        public BuildingPreset()
        {
            var house1File = GetFile("House1.json");
            var house2File = GetFile("House2.json");
            var house3File = GetFile("House3.json");
            var house4File = GetFile("House4.json");
            var house5File = GetFile("House5.json");
            var house6File = GetFile("House6.json");
            var houseForSaleFile = GetFile("HouseForSale.json");
            var tavernFile = GetFile("Tavern.json");
            var guildHallFile = GetFile("GuildHall.json");
            var templeFile = GetFile("Temple.json");
            var furnitureStoreFile = GetFile("FurnitureStore.json");
            var bankFile = GetFile("Bank.json");
            var generalStoreFile = GetFile("GeneralStore.json");
            var pawnShopFile = GetFile("PawnShop.json");
            var armorerFile = GetFile("Armorer.json");
            var weaponSmithFile = GetFile("WeaponSmith.json");
            var clothingStoreFile = GetFile("ClothingStore.json");
            var alchemistFile = GetFile("Alchemist.json");
            var gemStoreFile = GetFile("GemStore.json");
            var booksellerFile = GetFile("Bookseller.json");
            var libraryFile = GetFile("Library.json");
            var palaceFile = GetFile("Palace.json");
            var town23File = GetFile("Town23.json");
            var shipFile = GetFile("Ship.json");

            house1 = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), house1File);
            house2 = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), house2File);
            house3 = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), house3File);
            house4 = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), house4File);
            house5 = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), house5File);
            house6 = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), house6File);
            houseForSale = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), houseForSaleFile);
            tavern = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), tavernFile);
            guildHall = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), guildHallFile);
            temple = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), templeFile);
            furnitureStore = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), furnitureStoreFile);
            bank = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), bankFile);
            generalStore = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), generalStoreFile);
            pawnShop = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), pawnShopFile);
            armorer = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), armorerFile);
            weaponSmith = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), weaponSmithFile);
            clothingStore = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), clothingStoreFile);
            alchemist = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), alchemistFile);
            gemStore = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), gemStoreFile);
            bookseller = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), booksellerFile);
            library = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), libraryFile);
            palace = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), palaceFile);
            town23 = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), town23File);
            ship = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), shipFile);
        }

        public void SetClimate(ClimateBases climate, ClimateSeason season, WindowStyle windowStyle)
        {
            this.climate = climate;
            this.season = season;
            this.windowStyle = windowStyle;
        }

        public GameObject AddBuildingObject(BuildingReplacementData building, Vector3 position, Vector3 rotation)
        {
            DFLocation.BuildingData buildingData = new DFLocation.BuildingData()
            {
                NameSeed = building.NameSeed, Quality = building.Quality,
                BuildingType = (DFLocation.BuildingTypes)building.BuildingType,
                FactionId = building.FactionId
            };
            DFBlock.RmbSubRecord subRecord = building.RmbSubRecord;

            subRecord.XPos = (int)position.x;
            subRecord.ZPos = (int)position.z;
            subRecord.YRotation = (short)rotation.y;
            for (var i = 0; i < subRecord.Exterior.Block3dObjectRecords.Length; i++)
            {
                var model = subRecord.Exterior.Block3dObjectRecords[i];
                subRecord.Exterior.Block3dObjectRecords[i].YPos = model.YPos + (int)position.y;
            }

            var go = new GameObject("Building From File");
            var buildingComponent = go.AddComponent<Building>();
            buildingComponent.CreateObject(buildingData, subRecord, climate, season, windowStyle);

            return go;
        }

        public GameObject AddBuildingObject(string buildingId, Vector3 position, Vector3 rotation)
        {
            var buildingGroupId = int.Parse(buildingId.Substring(0, 2));
            var buildingIndex = int.Parse(buildingId.Substring(2));
            DFBlock buildingGroup = GetBuildingGroup(buildingGroupId);

            DFLocation.BuildingData buildingData = buildingGroup.RmbBlock.FldHeader.BuildingDataList[buildingIndex - 1];
            DFBlock.RmbSubRecord subRecord =
                RmbBlockHelper.CloneRmbSubRecord(buildingGroup.RmbBlock.SubRecords[buildingIndex - 1]);

            subRecord.XPos = (int)position.x;
            subRecord.ZPos = (int)position.z;
            subRecord.YRotation = (short)rotation.y;
            for (var i = 0; i < subRecord.Exterior.Block3dObjectRecords.Length; i++)
            {
                var model = subRecord.Exterior.Block3dObjectRecords[i];
                subRecord.Exterior.Block3dObjectRecords[i].YPos = model.YPos + (int)position.y;
            }

            var go = new GameObject("Building " + buildingId);
            var buildingComponent = go.AddComponent<Building>();
            buildingComponent.CreateObject(buildingData, subRecord, climate, season, windowStyle);

            return go;
        }

        public GameObject ReplaceBuildingObject(BuildingReplacementData building, Building oldBuilding,
            Boolean useNewInterior, Boolean useNewExterior)
        {
            var oldSubRecord = oldBuilding.GetSubRecord();
            DFLocation.BuildingData buildingData = new DFLocation.BuildingData()
            {
                NameSeed = building.NameSeed,
                Quality = building.Quality,
                BuildingType = (DFLocation.BuildingTypes)building.BuildingType,
                FactionId = building.FactionId
            };

            var exterior = oldSubRecord.Exterior;

            if (useNewExterior)
            {
                exterior = building.RmbSubRecord.Exterior;
                for (var i = 0; i < exterior.Block3dObjectRecords.Length; i++)
                {
                    var model = exterior.Block3dObjectRecords[i];
                    exterior.Block3dObjectRecords[i].YPos = model.YPos + oldBuilding.ModelsYPos;
                }
            }

            DFBlock.RmbSubRecord subRecord = new DFBlock.RmbSubRecord()
            {
                XPos = oldBuilding.XPos,
                ZPos = oldBuilding.ZPos,
                YRotation = oldBuilding.YRotation,
                Interior = useNewInterior ? building.RmbSubRecord.Interior : oldSubRecord.Interior,
                Exterior = exterior
            };

            var go = new GameObject("Building From File");
            var buildingComponent = go.AddComponent<Building>();
            buildingComponent.CreateObject(buildingData, subRecord, climate, season, windowStyle);

            return go;
        }

        public GameObject ReplaceBuildingObject(string buildingId, Building oldBuilding, Boolean useNewInterior, Boolean useNewExterior)
        {
            var buildingGroupId = int.Parse(buildingId.Substring(0, 2));
            var buildingIndex = int.Parse(buildingId.Substring(2));
            DFBlock buildingGroup = GetBuildingGroup(buildingGroupId);

            DFLocation.BuildingData newBuilding = buildingGroup.RmbBlock.FldHeader.BuildingDataList[buildingIndex - 1];
            DFBlock.RmbSubRecord newSubRecord =
                RmbBlockHelper.CloneRmbSubRecord(buildingGroup.RmbBlock.SubRecords[buildingIndex - 1]);

            var oldSubRecord = oldBuilding.GetSubRecord();
            DFLocation.BuildingData buildingData = new DFLocation.BuildingData()
            {
                NameSeed = newBuilding.NameSeed,
                Quality = newBuilding.Quality,
                BuildingType = newBuilding.BuildingType,
                FactionId = newBuilding.FactionId
            };

            var exterior = oldSubRecord.Exterior;
            if (useNewExterior)
            {
                exterior = newSubRecord.Exterior;
                for (var i = 0; i < exterior.Block3dObjectRecords.Length; i++)
                {
                    var model = exterior.Block3dObjectRecords[i];
                    exterior.Block3dObjectRecords[i].YPos = model.YPos + oldBuilding.ModelsYPos;
                }
            }

            DFBlock.RmbSubRecord subRecord = new DFBlock.RmbSubRecord()
            {
                XPos = oldBuilding.XPos,
                ZPos = oldBuilding.ZPos,
                YRotation = oldBuilding.YRotation,
                Interior = useNewInterior ? newSubRecord.Interior : oldSubRecord.Interior,
                Exterior = exterior
            };

            var go = new GameObject("Building " + buildingId);
            var buildingComponent = go.AddComponent<Building>();
            buildingComponent.CreateObject(buildingData, subRecord, climate, season, windowStyle);

            return go;
        }

        // Unlike in AddBuildingObject, we do not want to add the Building Component to this GameObject
        public GameObject AddBuildingPlaceholder(string buildingId)
        {
            var buildingGroupId = int.Parse(buildingId.Substring(0, 2));
            var buildingIndex = int.Parse(buildingId.Substring(2));
            DFBlock buildingGroup = GetBuildingGroup(buildingGroupId);

            DFBlock.RmbSubRecord subRecord = buildingGroup.RmbBlock.SubRecords[buildingIndex - 1];

            return AddBuildingPlaceholder(subRecord);
        }

        public GameObject AddBuildingPlaceholder(DFBlock.RmbSubRecord subRecord)
        {
            var placeholder = new GameObject();
            foreach (var blockRecord in subRecord.Exterior.Block3dObjectRecords)
            {
                var go = RmbBlockHelper.Add3dObject(blockRecord, climate, season, windowStyle);
                go.transform.parent = placeholder.transform;
            }

            return placeholder;
        }

        private DFBlock GetBuildingGroup(int buildingGroupId)
        {
            DFBlock buildingGroup = new DFBlock();
            switch (buildingGroupId)
            {
                case 1:
                    buildingGroup = house1;
                    break;
                case 2:
                    buildingGroup = house2;
                    break;
                case 3:
                    buildingGroup = house3;
                    break;
                case 4:
                    buildingGroup = house4;
                    break;
                case 5:
                    buildingGroup = house5;
                    break;
                case 6:
                    buildingGroup = house6;
                    break;
                case 7:
                    buildingGroup = houseForSale;
                    break;
                case 8:
                    buildingGroup = tavern;
                    break;
                case 9:
                    buildingGroup = guildHall;
                    break;
                case 10:
                    buildingGroup = temple;
                    break;
                case 11:
                    buildingGroup = furnitureStore;
                    break;
                case 12:
                    buildingGroup = bank;
                    break;
                case 13:
                    buildingGroup = generalStore;
                    break;
                case 14:
                    buildingGroup = pawnShop;
                    break;
                case 15:
                    buildingGroup = armorer;
                    break;
                case 16:
                    buildingGroup = weaponSmith;
                    break;
                case 17:
                    buildingGroup = clothingStore;
                    break;
                case 18:
                    buildingGroup = alchemist;
                    break;
                case 19:
                    buildingGroup = gemStore;
                    break;
                case 20:
                    buildingGroup = bookseller;
                    break;
                case 21:
                    buildingGroup = library;
                    break;
                case 22:
                    buildingGroup = palace;
                    break;
                case 23:
                    buildingGroup = town23;
                    break;
                case 24:
                    buildingGroup = ship;
                    break;
            }

            return buildingGroup;
        }

        private static string GetFile(string fileName)
        {
            var path = Environment.CurrentDirectory + "/Assets/Game/Addons/RmbBlockEditor/Editor/BuildingPresets/" +
                       fileName;
            return File.ReadAllText(path);
        }
    }
}