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

using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// Tipton calls this "create npc" but its true function seems to be to reserve a quest site
    /// before placing a resource inside. "create npc" is usually followed by "place npc"
    /// but can also be followed by "place item" or "create foe" for example. So not
    /// just intended just for NPCs. This action likely initiates some book-keeping in Daggerfall's
    /// quest system. Emulating behaviour as best understood for now.
    /// 
    /// Notes:
    ///  * This action does not seem related to "create npc anNPC", which is still not researched.
    /// </summary>
    public class ReserveSite : ActionTemplate
    {
        Symbol placeSymbol;

        public override string Pattern
        {
            // Supports both forms "create npc at _symbol_" and "reserve site _symbol_"
            get { return @"create npc at (?<aPlace>\w+)|reserve site (?<aPlace>\w+)"; }
        }

        public ReserveSite(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction Create(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new reserve site action
            ReserveSite reserveSite = new ReserveSite(parentQuest);
            reserveSite.placeSymbol = new Symbol(match.Groups["aPlace"].Value);

            // Attempt to get site being reserved by quest
            Place place = parentQuest.GetPlace(reserveSite.placeSymbol);
            if (place == null)
                throw new Exception(string.Format("Attempted to reserve invalid Place symbol {0}", reserveSite.placeSymbol.Name));

            // Reserve site in quest machine
            QuestMachine.Instance.ReserveSite(parentQuest, place.SiteDetails);

            Debug.LogFormat("Reserved site {0} at {1} in {2}", place.SiteDetails.buildingName, place.SiteDetails.locationName, place.SiteDetails.regionName);

            return reserveSite;
        }
    }
}