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
using DaggerfallConnect;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    public class GivePc : ActionTemplate
    {
        Symbol itemSymbol;
        int textId;
        bool isNothing;

        public override string Pattern
        {
            get { return @"give pc (?<anItem>[a-zA-Z0-9_.]+)||give pc (?<anItem>[a-zA-Z0-9_.]+) notify (?<id>\d+)||give pc (?<nothing>nothing)"; }
        }

        public GivePc(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction Create(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            GivePc action = new GivePc(parentQuest);
            action.itemSymbol = new Symbol(match.Groups["anItem"].Value);
            action.textId = Parser.ParseInt(match.Groups["id"].Value);
            if (!string.IsNullOrEmpty(match.Groups["persist"].Value))
                action.isNothing = true;
            else
                action.isNothing = false;

            return action;
        }

        public override void Update(Task caller)
        {
            base.Update(caller);
        }
    }
}