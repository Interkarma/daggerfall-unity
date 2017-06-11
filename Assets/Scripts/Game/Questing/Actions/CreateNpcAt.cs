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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// Tipton calls this "create npc" but its true function seems to reserve a quest site
    /// before linking resources. "create npc" is usually followed by "place npc"
    /// but can also be followed by "place item" or "create foe" for example.
    /// This action likely initiates some book-keeping in Daggerfall's quest system.
    /// In Daggerfall Unity this creates a SiteLink in QuestMachine.
    /// </summary>
    public class CreateNpcAt : ActionTemplate
    {
        Symbol placeSymbol;

        public override string Pattern
        {
            get { return @"create npc at (?<aPlace>\w+)"; }
        }

        public CreateNpcAt(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            base.CreateNew(source, parentQuest);

            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            CreateNpcAt action = new CreateNpcAt(parentQuest);
            action.placeSymbol = new Symbol(match.Groups["aPlace"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            base.Update(caller);

            // Attempt to get Place resource
            Place place = ParentQuest.GetPlace(placeSymbol);
            if (place == null)
                throw new Exception(string.Format("Attempted to add SiteLink for invalid Place symbol {0}", placeSymbol.Name));

            // Create SiteLink in QuestMachine
            SiteLink siteLink = new SiteLink();
            siteLink.questUID = ParentQuest.UID;
            siteLink.placeSymbol = placeSymbol;
            siteLink.siteType = place.SiteDetails.siteType;
            siteLink.mapId = place.SiteDetails.mapId;
            siteLink.buildingKey = place.SiteDetails.buildingKey;
            QuestMachine.Instance.AddSiteLink(siteLink);

            // Output debug information
            switch (siteLink.siteType)
            {
                case SiteTypes.Building:
                    Debug.LogFormat("Created Building SiteLink to {0} in {1}/{2}", place.SiteDetails.buildingName, place.SiteDetails.regionName, place.SiteDetails.locationName);
                    break;
                case SiteTypes.Dungeon:
                    Debug.LogFormat("Created Dungeon SiteLink to {0}/{1}", place.SiteDetails.regionName, place.SiteDetails.locationName);
                    break;
            }

            SetComplete();
        }
    }
}