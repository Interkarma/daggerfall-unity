// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    
// 

using System.Text.RegularExpressions;
using FullSerializer;
using DaggerfallWorkshop.Utility.AssetInjection;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Updates the world data system to use a specific variant for a given
    /// place (location, block, building) either for all instances or for a
    /// particular location.
    /// Specify a single dash for variant to remove any existing variant.
    /// </summary>
    public class WorldUpdate : ActionTemplate
    {
        string type;
        int regionIndex;
        int locationIndex;
        string locationName;
        string blockName;
        int recordIndex;
        string variant;

        public override string Pattern
        {
            get {
                return
                    @"worldupdate (?<type>location) at (?<locationIndex>\d+) in region (?<regionIndex>\d+) variant (?<variant>[a-zA-Z0-9_.-]+)|" +
                    @"worldupdate (?<type>locationnew) named (?<locationName>.+) in region (?<regionIndex>\d+) variant (?<variant>[a-zA-Z0-9_.-]+)|" +
                    @"worldupdate (?<type>block) (?<blockName>[a-zA-Z0-9_.-]+) at (?<locationIndex>\d+) in region (?<regionIndex>\d+) variant (?<variant>[a-zA-Z0-9_.-]+)|" +
                    @"worldupdate (?<type>blockAll) (?<blockName>[a-zA-Z0-9_.-]+) variant (?<variant>[a-zA-Z0-9_.-]+)|" +
                    @"worldupdate (?<type>building) (?<blockName>[a-zA-Z0-9_.-]+) (?<recordIndex>\d+) at (?<locationIndex>\d+) in region (?<regionIndex>\d+) variant (?<variant>[a-zA-Z0-9_.-]+)|" +
                    @"worldupdate (?<type>buildingAll) (?<blockName>[a-zA-Z0-9_.-]+) (?<recordIndex>\d+) variant (?<variant>[a-zA-Z0-9_.-]+)";
            }
        }

        public WorldUpdate(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            WorldUpdate action = new WorldUpdate(parentQuest);
            action.type = match.Groups["type"].Value;
            action.variant = match.Groups["variant"].Value;

            Group regionIndexGroup = match.Groups["regionIndex"];
            if (regionIndexGroup.Success)
                action.regionIndex = Parser.ParseInt(regionIndexGroup.Value);

            Group locationIndexGroup = match.Groups["locationIndex"];
            if (locationIndexGroup.Success)
                action.locationIndex = Parser.ParseInt(locationIndexGroup.Value);

            Group locationNameGroup = match.Groups["locationName"];
            if (locationIndexGroup.Success)
                action.locationName = locationNameGroup.Value;

            Group blockNameGroup = match.Groups["blockName"];
            if (blockNameGroup.Success)
                action.blockName = blockNameGroup.Value;

            Group recordIndexGroup = match.Groups["recordIndex"];
            if (recordIndexGroup.Success)
                action.recordIndex = Parser.ParseInt(recordIndexGroup.Value);

            return action;
        }

        public override void Update(Task caller)
        {
            base.Update(caller);

            if (variant == "-")
                variant = WorldDataVariants.NoVariant;
            int locationKey;
            switch (type)
            {
                case "location":
                    WorldDataVariants.SetLocationVariant(regionIndex, locationIndex, variant);
                    break;
                case "locationnew":
                    WorldDataVariants.SetNewLocationVariant(regionIndex, locationName, variant);
                    break;
                case "block":
                    locationKey = WorldDataReplacement.MakeLocationKey(regionIndex, locationIndex);
                    WorldDataVariants.SetBlockVariant(blockName, variant, locationKey);
                    break;
                case "blockAll":
                    WorldDataVariants.SetBlockVariant(blockName, variant);
                    break;
                case "building":
                    locationKey = WorldDataReplacement.MakeLocationKey(regionIndex, locationIndex);
                    WorldDataVariants.SetBuildingVariant(blockName, recordIndex, variant, locationKey);
                    break;
                case "buildingAll":
                    WorldDataVariants.SetBuildingVariant(blockName, recordIndex, variant);
                    break;
            }

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public string type;
            public int regionIndex;
            public int locationIndex;
            public string locationName;
            public string blockName;
            public int recordIndex;
            public string variant;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.type = type;
            data.regionIndex = regionIndex;
            data.locationIndex = locationIndex;
            data.locationName = locationName;
            data.blockName = blockName;
            data.recordIndex = recordIndex;
            data.variant = variant;
            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            type = data.type;
            regionIndex = data.regionIndex;
            locationIndex = data.locationIndex;
            locationName = data.locationName;
            blockName = data.blockName;
            recordIndex = data.recordIndex;
            variant = data.variant;
        }

        #endregion
    }
}