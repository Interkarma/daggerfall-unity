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
using System;
using System.Text.RegularExpressions;
using System.Collections;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// A quest Foe defines an enemy for player to fight as part of a quest.
    /// </summary>
    public class Foe : QuestResource
    {
        public Foe(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public Foe(Quest parentQuest, string line)
            : base(parentQuest)
        {
            SetResource(line);
        }

        public override void SetResource(string line)
        {
            base.SetResource(line);

            string matchStr = @"(Foe|foe) is (?<symbol>[a-zA-Z0-9_.-]+)||(Foe|foe) is (?<count>\d+) (?<symbol>[a-zA-Z0-9_.-]+)";

            Match match = Regex.Match(line, matchStr);
            if (match.Success)
            {
            }
        }
    }
}