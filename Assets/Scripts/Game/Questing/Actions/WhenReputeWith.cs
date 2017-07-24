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
    /// Starts task when reputation with a faction is equal to or greater than a value.
    /// NOTE: This requires more research to build. Just stubbing out now so this action will compile.
    /// </summary>
    public class WhenReputeWith : ActionTemplate
    {
        string factionName;
        int value;

        public override string Pattern
        {
            get { return @"when repute with (?<aFaction>[a-zA-Z0-9_.-]+) is at least (?<value>\d+)"; }
        }

        public WhenReputeWith(Quest parentQuest)
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
            WhenReputeWith action = new WhenReputeWith(parentQuest);
            action.factionName = match.Groups["aFaction"].Value;
            action.value = Parser.ParseInt(match.Groups["value"].Value);

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