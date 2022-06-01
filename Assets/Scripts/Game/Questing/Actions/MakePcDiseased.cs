// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using UnityEngine;
using System.Text.RegularExpressions;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.MagicAndEffects;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// Inflicts a disease on player through quest system.
    /// </summary>
    public class MakePcDiseased : ActionTemplate
    {
        Diseases diseaseType = Diseases.None;

        public override string Pattern
        {
            get { return @"make pc ill with (?<aDisease>[a-zA-Z0-9_.']+)"; }
        }

        public MakePcDiseased(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Get disease type
            Diseases type = Diseases.None;
            string name = match.Groups["aDisease"].Value;
            Table diseasesTable = QuestMachine.Instance.DiseasesTable;
            if (diseasesTable.HasValue(name))
                type = (Diseases)Parser.ParseInt(diseasesTable.GetValue("id", name));
            else
                throw new Exception(string.Format("make pc ill with <aDisease> could not find disease matching '{0}'. See 'Quests-Diseases' table for valid disease names.", name));

            // Factory new action
            MakePcDiseased action = new MakePcDiseased(parentQuest);
            action.diseaseType = type;

            return action;
        }

        public override void Update(Task caller)
        {
            // Inflict disease on player
            if (diseaseType != Diseases.None)
            {
                EntityEffectBundle bundle = GameManager.Instance.PlayerEffectManager.CreateDisease(diseaseType);
                GameManager.Instance.PlayerEffectManager.AssignBundle(bundle, AssignBundleFlags.BypassSavingThrows);
            }

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Diseases diseaseType;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.diseaseType = diseaseType;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            diseaseType = data.diseaseType;
        }

        #endregion
    }
}