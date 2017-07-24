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
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Starts task when faction is available to quest system.
    /// NOTE: This requires more research to build. Just stubbing out now so this action will compile.
    /// </summary>
    public class WhenFactionIsAvailable : ActionTemplate
    {
        string factionName;

        public override string Pattern
        {
            get { return @"when (?<aFaction>[a-zA-Z0-9_.-]+) is available"; }
        }

        public WhenFactionIsAvailable(Quest parentQuest)
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
            WhenFactionIsAvailable action = new WhenFactionIsAvailable(parentQuest);
            action.factionName = match.Groups["aFaction"].Value;

            return action;
        }

        public override object GetSaveData()
        {
            return string.Empty;
        }

        public override void RestoreSaveData(object dataIn)
        {
        }
    }
}