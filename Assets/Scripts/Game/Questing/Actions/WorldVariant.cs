// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    
// 

using System.Text.RegularExpressions;
using DaggerfallWorkshop.Utility;
using FullSerializer;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallConnect;

namespace DaggerfallWorkshop.Game.Questing
{
    public class WorldVariant : ActionTemplate
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
                    @"world variant location<type> at (?<locationIndex>\d+) in region (?<regionIndex>\d+) to (?<variant>[a-zA-Z0-9_.-]+)|" +
                    @"world variant locationnew<type> named (?<locationName>.+) in region (?<regionIndex>\d+) to (?<variant>[a-zA-Z0-9_.-]+)|" +
                    @"world variant block<type> (?<blockName>[a-zA-Z0-9_.-]+) at (?<locationIndex>\d+) in region (?<regionIndex>\d+) to (?<variant>[a-zA-Z0-9_.-]+)|" +
                    @"world variant blockAll<type> (?<blockName>[a-zA-Z0-9_.-]+) to (?<variant>[a-zA-Z0-9_.-]+)|" +
                    @"world variant building<type> (?<blockName>[a-zA-Z0-9_.-]+) (?<recordIndex>\d+) at (?<locationIndex>\d+) in region (?<regionIndex>\d+) to (?<variant>[a-zA-Z0-9_.-]+)|" +
                    @"world variant buildingAll<type> (?<blockName>[a-zA-Z0-9_.-]+) (?<recordIndex>\d+) to (?<variant>[a-zA-Z0-9_.-]+)|";
            }
        }

        public WorldVariant(Quest parentQuest)
            : base(parentQuest)
        {
            IsTriggerCondition = true;
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            WorldVariant action = new WorldVariant(parentQuest);
            action.type = match.Groups["type"].Value;
            action.regionIndex = Parser.ParseInt(match.Groups["regionIndex"].Value);
            action.locationIndex = Parser.ParseInt(match.Groups["locationIndex"].Value);
            action.locationName = match.Groups["locationName"].Value;
            action.blockName = match.Groups["blockName"].Value;
            action.recordIndex = Parser.ParseInt(match.Groups["recordIndex"].Value);

            /* Resolve new location name to location index
            if (action.regionIndex >= 0 && !string.IsNullOrEmpty(action.locationName))
            {
                DFRegion dfRegion = new DFRegion();
                if (WorldDataReplacement.GetDFRegionAdditionalLocationData(regionIndex, ref dfRegion))
                {
                    dfRegion.MapNameLookup.TryGetValue(action.locationName, out action.locationIndex);
                }
            }*/

            return action;
        }

        public override void Update(Task caller)
        {
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