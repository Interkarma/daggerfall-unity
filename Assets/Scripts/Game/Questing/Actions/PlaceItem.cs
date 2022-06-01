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

using System.Text.RegularExpressions;
using System;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// Moves item into world at a reserved site.
    /// </summary>
    public class PlaceItem : ActionTemplate
    {
        Symbol itemSymbol;
        Symbol placeSymbol;
        int marker = -1;
        MarkerPreference markerPreference = MarkerPreference.Default;

        public override string Pattern
        {
            get { return @"place item (?<anItem>[a-zA-Z0-9_.-]+) at (?<aPlace>[a-zA-Z0-9_.-]+) marker (?<marker>\d+)|" +
                         @"place item (?<anItem>[a-zA-Z0-9_.-]+) at (?<aPlace>[a-zA-Z0-9_.-]+) questmarker (?<questmarker>\d+)|" +
                         @"place item (?<anItem>[a-zA-Z0-9_.-]+) at (?<aPlace>[a-zA-Z0-9_.-]+) (?<anymarker>anymarker)|" +
                         @"place item (?<anItem>[a-zA-Z0-9_.-]+) at (?<aPlace>[a-zA-Z0-9_.-]+)"; }
        }

        public PlaceItem(Quest parentQuest)
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
            PlaceItem action = new PlaceItem(parentQuest);
            action.itemSymbol = new Symbol(match.Groups["anItem"].Value);
            action.placeSymbol = new Symbol(match.Groups["aPlace"].Value);

            // Set custom marker
            Group markerGroup = match.Groups["marker"];
            if (markerGroup.Success)
                action.marker = Parser.ParseInt(markerGroup.Value);

            // Set custom quest marker
            Group markerPreferenceGroup = match.Groups["questmarker"];
            if (markerPreferenceGroup.Success)
            {
                action.marker = Parser.ParseInt(markerPreferenceGroup.Value);
                action.markerPreference = MarkerPreference.UseQuestMarker;
            }

            // Set any marker
            Group anyMarkerGroup = match.Groups["anymarker"];
            if (anyMarkerGroup.Success)
                action.markerPreference = MarkerPreference.AnyMarker;

            return action;
        }

        public override void Update(Task caller)
        {
            base.Update(caller);

            // Create SiteLink if not already present
            if (!QuestMachine.HasSiteLink(ParentQuest, placeSymbol))
                QuestMachine.CreateSiteLink(ParentQuest, placeSymbol);

            // Attempt to get Item resource
            Item item = ParentQuest.GetItem(itemSymbol);
            if (item == null)
            {
                SetComplete();
                throw new Exception(string.Format("Could not find Item resource symbol {0}", itemSymbol));
            }

            // Attempt to get Place resource
            Place place = ParentQuest.GetPlace(placeSymbol);
            if (place == null)
            {
                SetComplete();
                throw new Exception(string.Format("Could not find Place resource symbol {0}", placeSymbol));
            }

            // Assign Item to Place
            place.AssignQuestResource(item.Symbol, marker, markerPreference);

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol itemSymbol;
            public Symbol placeSymbol;
            public int marker;
            public MarkerPreference markerPreference;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.itemSymbol = itemSymbol;
            data.placeSymbol = placeSymbol;
            data.marker = marker;
            data.markerPreference = markerPreference;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            itemSymbol = data.itemSymbol;
            placeSymbol = data.placeSymbol;
            marker = data.marker;
            markerPreference = data.markerPreference;
        }

        #endregion
    }
}