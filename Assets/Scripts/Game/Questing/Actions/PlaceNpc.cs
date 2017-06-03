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
    /// Moves NPC to a reserved site.
    /// An NPC always starts in their home location but quests can move them around as needed.
    /// Site must be reserved before moving NPC to that location.
    /// 
    /// Notes:
    ///  * TODO: Layout methods need to exclude NPCs from their home location and only inject where they belong for quest.
    ///  * TODO: If no quest is operating on an NPC, or atHome specified, they should just be injected to usual spot.
    /// </summary>
    public class PlaceNpc : ActionTemplate
    {
        Symbol npcSymbol;
        Symbol placeSymbol;

        public override string Pattern
        {
            get { return @"place npc (?<anNPC>\w+) at (?<aPlace>\w+)"; }
        }

        public PlaceNpc(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction Create(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factor new place npc at action
            PlaceNpc placeNpc = new PlaceNpc(parentQuest);
            placeNpc.npcSymbol = new Symbol(match.Groups["anNPC"].Value);
            placeNpc.placeSymbol = new Symbol(match.Groups["aPlace"].Value);

            // TODO: House-keeping to place NPC at site

            Debug.LogFormat("Placed NPC {0} at {1}", placeNpc.npcSymbol.Name, placeNpc.placeSymbol.Name);

            return placeNpc;
        }
    }
}