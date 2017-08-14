// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
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

        public override string Pattern
        {
            get { return @"reveal (?<aPlace>\w+)"; }
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

            return action;
        }

        public override void Update(Task caller)
        {
            // TODO: Perform action changes

            // Get place resource
            Place place = ParentQuest.GetPlace(placeSymbol);
            if (place == null)
                return;

            if (place.SiteDetails.siteType == SiteTypes.Dungeon)
            {
                GameManager.Instance.PlayerGPS.DiscoverLocation(place.SiteDetails.regionName, place.SiteDetails.locationName);            
            }

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            SaveData_v1 data = (SaveData_v1)dataIn;
            if (dataIn == null)
                return;
        }

        #endregion
    }
}