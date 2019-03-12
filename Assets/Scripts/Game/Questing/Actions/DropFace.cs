// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
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
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    public class DropFace : ActionTemplate
    {
        Symbol personSymbol;

        public override string Pattern
        {
            get { return @"drop (?<anNPC>[a-zA-Z0-9_.-]+) face"; }
        }

        public DropFace(Quest parentQuest)
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
            DropFace action = new DropFace(parentQuest);
            action.personSymbol = new Symbol(match.Groups["anNPC"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            base.Update(caller);

            // Get related Person resource
            Person person = ParentQuest.GetPerson(personSymbol);
            if (person == null)
                return;

            // Drop face from HUD
            DaggerfallUI.Instance.DaggerfallHUD.EscortingFaces.DropFace(person);

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol personSymbol;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.personSymbol = personSymbol;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            personSymbol = data.personSymbol;
        }

        #endregion
    }
}