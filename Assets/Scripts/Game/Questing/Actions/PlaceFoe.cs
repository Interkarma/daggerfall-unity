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
    public class PlaceFoe : ActionTemplate
    {
        Symbol foeSymbol;
        Symbol placeSymbol;

        public override string Pattern
        {
            get { return @"place foe (?<aFoe>[a-zA-Z0-9_.-]+) at (?<aPlace>\w+)"; }
        }

        public PlaceFoe(Quest parentQuest)
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
            PlaceFoe action = new PlaceFoe(parentQuest);
            action.foeSymbol = new Symbol(match.Groups["aFoe"].Value);
            action.placeSymbol = new Symbol(match.Groups["aPlace"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            base.Update(caller);

            // Create SiteLink if not already present
            if (!QuestMachine.HasSiteLink(ParentQuest, placeSymbol))
                QuestMachine.CreateSiteLink(ParentQuest, placeSymbol);

            // Attempt to get Foe resource
            Foe foe = ParentQuest.GetFoe(foeSymbol);
            if (foe == null)
                throw new Exception(string.Format("Could not find Foe resource symbol {0}", foeSymbol));

            // Attempt to get Place resource
            Place place = ParentQuest.GetPlace(placeSymbol);
            if (place == null)
                throw new Exception(string.Format("Could not find Place resource symbol {0}", placeSymbol));

            // Assign Foe to Place
            place.AssignQuestResource(foeSymbol);

            SetComplete();
        }
    }
}