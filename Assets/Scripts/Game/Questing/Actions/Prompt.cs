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
    /// Prompt which displays a yes/no dialog that executes a different task based on user input.
    /// </summary>
    public class Prompt : ActionTemplate
    {
        public override string Pattern
        {
            get { return @"prompt (?<id>\d+) yes (?<yesTaskName>[a-zA-Z0-9_.]+) no (?<noTaskName>[a-zA-Z0-9_.]+)"; }
        }

        public override IQuestAction Create(string source)
        {
            // Source must match pattern
            Match match = Test(source);
            if (match.Success)
                return null;

            // Factory new action
            Prompt action = new Prompt();

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