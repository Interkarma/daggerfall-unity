// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Addons.RmbBlockEditor.Elements;
using DaggerfallWorkshop.Utility.AssetInjection;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public static class BuildingHelper
    {
        public static GameObject GetBuildingGameObject(BuildingReplacementData building, Vector3 position, Vector3 rotation)
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
            buildingComponent.CreateObject(buildingData, subRecord);

            return go;
        }

        public static GameObject GetBuildingGameObject(string buildingId, Vector3 position, Vector3 rotation)
        {
            var templates = PersistedBuildingsCatalog.Templates();
            var replacementData = templates[buildingId];

            var buildingData = new DFLocation.BuildingData
            {
                FactionId = replacementData.FactionId,
                Quality = replacementData.Quality,
                BuildingType = (DFLocation.BuildingTypes)replacementData.BuildingType,
                NameSeed = replacementData.NameSeed,
            };
            var subRecord = replacementData.RmbSubRecord;

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
            buildingComponent.CreateObject(buildingData, subRecord);

            return go;
        }


        public static GameObject GetDummyExterior(string buildingId)
        {
            var templates = PersistedBuildingsCatalog.Templates();
            var buildingReplacementData = templates[buildingId];
            return GetDummyExterior(buildingReplacementData);
        }

        public static GameObject GetDummyExterior(BuildingReplacementData buildingReplacementData)
        {
            var placeholder = new GameObject();
            if (buildingReplacementData.RmbSubRecord.Exterior.Block3dObjectRecords == null)
            {
                return placeholder;
            }
            foreach (var blockRecord in buildingReplacementData.RmbSubRecord.Exterior.Block3dObjectRecords)
            {
                var go = RmbBlockHelper.Add3dObject(blockRecord);
                go.transform.parent = placeholder.transform;
            }

            return placeholder;
        }

        public static GameObject GetDummyInterior(BuildingReplacementData buildingReplacementData)
        {
            var placeholder = new GameObject();
            if (buildingReplacementData.RmbSubRecord.Interior.Block3dObjectRecords == null)
            {
                return placeholder;
            }
            foreach (var blockRecord in buildingReplacementData.RmbSubRecord.Interior.Block3dObjectRecords)
            {
                var go = RmbBlockHelper.Add3dObject(blockRecord);
                go.transform.parent = placeholder.transform;
            }

            return placeholder;
        }

        public static VisualElement GetPreview(BuildingReplacementData buildingData)
        {
            var tabs = new Tabs();
            var tab1 = new Tab { value = 0, label = "Exterior" };
            var tab2 = new Tab { value = 1, label = "Interior" };

            var exteriorPreview = GetExteriorPreview(buildingData);
            var interiorPreview = GetInteriorPreview(buildingData);

            tab1.Add(exteriorPreview);
            tab2.Add(interiorPreview);
            tabs.Add(tab1);
            tabs.Add(tab2);

            return tabs;
        }

        public static VisualElement GetInteriorPreview(BuildingReplacementData buildingData)
        {
            try
            {
                var interior = GetDummyInterior(buildingData);
                return new GoPreview(interior);
            }
            catch (Exception e)
            {
                // The building might not have an Interior
                return new VisualElement();
            }
        }

        public static VisualElement GetExteriorPreview(BuildingReplacementData buildingData)
        {
            try
            {
                var exterior = GetDummyExterior(buildingData);
                return new GoPreview(exterior);
            }
            catch (Exception e)
            {
                // The building might not have an Exterior
                return new VisualElement();
            }
        }
    }
}