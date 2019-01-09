// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using System.Text.RegularExpressions;
using FullSerializer;
using DaggerfallConnect;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    public class WhenPcEntersExits : ActionTemplate
    {
        bool onEnter = true;
        string sourceExteriorType = string.Empty;
        int indexExteriorType = -1;
        DFRegion.LocationTypes currentLocationType = DFRegion.LocationTypes.None;
        DFRegion.LocationTypes previousLocationType = DFRegion.LocationTypes.None;

        public override string Pattern
        {
            get { return @"when pc (?<enters>enters) (?<exteriorType>\w+)|when pc (?<exits>exits) (?<exteriorType>\w+)"; }
        }

        public WhenPcEntersExits(Quest parentQuest)
            : base(parentQuest)
        {
            IsTriggerCondition = true;
            PlayerGPS.OnEnterLocationRect += PlayerGPS_OnEnterLocationRect;
            PlayerGPS.OnExitLocationRect += PlayerGPS_OnExitLocationRect;
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            WhenPcEntersExits action = new WhenPcEntersExits(parentQuest);
            action.sourceExteriorType = match.Groups["exteriorType"].Value;
            if (!string.IsNullOrEmpty(match.Groups["enters"].Value))
                action.onEnter = true;
            else if (!string.IsNullOrEmpty(match.Groups["exits"].Value))
                action.onEnter = false;
            else
                throw new Exception("WhenPcEntersExits: Syntax is 'when pc enters exteriorType | when pc exits exteriorType");

            // Get location type index
            Table placesTable = QuestMachine.Instance.PlacesTable;
            int p1 = placesTable.GetInt("p1", action.sourceExteriorType);
            if (p1 != 2)
            {
                throw new Exception("WhenPcEntersExits: This trigger condition can only be used with exterior types of p1=2 in Quests-Places table.");
            }
            action.indexExteriorType = placesTable.GetInt("p2", action.sourceExteriorType);

            // Set location type (if any) when quest starts
            PlayerGPS playerGPS = GameManager.Instance.PlayerGPS;
            if (playerGPS.IsPlayerInLocationRect)
            {
                if (playerGPS.CurrentLocation.Loaded)
                    action.currentLocationType = playerGPS.CurrentLocation.MapTableData.LocationType;
            }

            return action;
        }

        public override bool CheckTrigger(Task caller)
        {
            // Has player entered or exited a target location
            if (HasEnteredTarget() || HasExitedTarget())
                return true;

            return false;
        }

        #region Private Methods

        bool HasEnteredTarget()
        {
            // Must be tracking enter and have a valid current location type
            if (!onEnter || currentLocationType == DFRegion.LocationTypes.None)
                return false;

            // Handle entering "anywhere"
            if (indexExteriorType == -1)
                return true;

            // Handle entering specific location types
            if (indexExteriorType == (int)currentLocationType)
                return true;

            return false;
        }

        bool HasExitedTarget()
        {
            // Must be tracking exit and have a valid previous location type
            if (onEnter || previousLocationType == DFRegion.LocationTypes.None)
                return false;

            // Handle exiting "anywhere"
            if (indexExteriorType == -1)
                return true;

            // Handle exiting specific location types
            if (indexExteriorType == (int)previousLocationType)
                return true;

            return false;
        }

        #endregion

        #region Events Handlers

        private void PlayerGPS_OnEnterLocationRect(DFLocation location)
        {
            previousLocationType = currentLocationType;
            if (location.Loaded)
                currentLocationType = location.MapTableData.LocationType;
            else
                currentLocationType = DFRegion.LocationTypes.None;
        }

        private void PlayerGPS_OnExitLocationRect()
        {
            previousLocationType = currentLocationType;
            currentLocationType = DFRegion.LocationTypes.None;
        }

        #endregion

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public bool onEnter;
            public string sourceExteriorType;
            public int indexExteriorType;
            public DFRegion.LocationTypes currentLocationType;
            public DFRegion.LocationTypes previousLocationType;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.onEnter = onEnter;
            data.sourceExteriorType = sourceExteriorType;
            data.indexExteriorType = indexExteriorType;
            data.currentLocationType = currentLocationType;
            data.previousLocationType = previousLocationType;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            onEnter = data.onEnter;
            sourceExteriorType = data.sourceExteriorType;
            indexExteriorType = data.indexExteriorType;
            currentLocationType = data.currentLocationType;
            previousLocationType = data.previousLocationType;
        }

        #endregion
    }
}