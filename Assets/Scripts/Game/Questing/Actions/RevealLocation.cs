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
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Reveals location on travelmap
    /// </summary>
    public class RevealLocation : ActionTemplate
    {
        Symbol placeSymbol;
        bool readMap;

        public override string Pattern
        {
            get { return @"reveal (?<aPlace>\w+) (?<readMap>readmap)|reveal (?<aPlace>\w+)"; }
        }

        public RevealLocation(Quest parentQuest)
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
            RevealLocation action = new RevealLocation(parentQuest);

            action.placeSymbol = new Symbol(match.Groups["aPlace"].Value);
            action.readMap = !string.IsNullOrEmpty(match.Groups["readMap"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            // Get place resource
            Place place = ParentQuest.GetPlace(placeSymbol);
            if (place == null)
                return;

            // Discover location
            GameManager.Instance.PlayerGPS.DiscoverLocation(place.SiteDetails.regionName, place.SiteDetails.locationName);

            if (readMap)
                GameManager.Instance.PlayerEntity.Notebook.AddNote(
                    TextManager.Instance.GetLocalizedText("readMap").Replace("%map", place.SiteDetails.locationName));

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol placeSymbol;
            public bool readMap;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.placeSymbol = placeSymbol;
            data.readMap = readMap;
            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            placeSymbol = data.placeSymbol;
            readMap = data.readMap;
        }

        #endregion
    }
}