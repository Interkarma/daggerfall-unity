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
    /// Condition which checks if player character at a specific place.
    /// </summary>
    public class PcAt : ActionTemplate
    {
        public override string Pattern
        {
            // Notes:
            // Docs use form "pc at aPlace do aTask"
            // But observed quests actually seem to use "pc at aPlace set aTask"
            // Probably a change between writing of docs and Template v1.11.
            // Docs also missing ""pc at aPlace set aTask saying nnnn"
            get { return @"pc at (?<aPlace>\w+) set (?<aTask>[a-zA-Z0-9_.]+)|pc at (?<aPlace>\w+) set (?<aTask>[a-zA-Z0-9_.]+) saying (?<id>\d+)"; }
        }

        public PcAt(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction Create(string source, Quest parentQuest)
        {
            return null;
        }

        public override bool CheckCondition(Task caller)
        {
            return false;
        }

        public override void Update(Task caller)
        {
        }
    }
}