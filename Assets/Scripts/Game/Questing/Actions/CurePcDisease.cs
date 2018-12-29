// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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
    /// Cure specific disease on player through quest system.
    /// </summary>
    public class CurePcDisease : ActionTemplate
    {
        Diseases diseaseType = Diseases.None;
        bool isCureVampirism = false;
        bool isCureLycanthropy = false;

        public override string Pattern
        {
            get { return @"cure vampirism|cure lycanthropy|cure (?<aDisease>[a-zA-Z0-9_.']+)"; }
        }

        public CurePcDisease(Quest parentQuest)
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
            CurePcDisease action = new CurePcDisease(parentQuest);

            // Handle special or standard disease type
            Diseases type = Diseases.None;
            if (string.Compare(match.Value, "cure vampirism", true) == 0)
            {
                action.isCureVampirism = true;
            }
            else if (string.Compare(match.Value, "cure lycanthropy", true) == 0)
            {
                action.isCureLycanthropy = true;
            }
            else
            {
                // Get disease type
                string name = match.Groups["aDisease"].Value;
                Table diseasesTable = QuestMachine.Instance.DiseasesTable;
                if (diseasesTable.HasValue(name))
                    type = (Diseases)Parser.ParseInt(diseasesTable.GetValue("id", name));
                else
                    throw new Exception(string.Format("cure <aDisease> could not find disease matching '{0}'. See 'Quests-Diseases' table for valid disease names.", name));
            }
            action.diseaseType = type;

            return action;
        }

        public override void Update(Task caller)
        {
            // Cure disease on player
            if (diseaseType != Diseases.None)
            {
                GameManager.Instance.PlayerEffectManager.CureDisease(diseaseType);
            }
            else if (isCureVampirism)
            {
                GameManager.Instance.PlayerEffectManager.EndVampirism();
            }
            else if (isCureLycanthropy)
            {
                //GameManager.Instance.PlayerEffectManager.EndLycanthropy();
            }

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Diseases diseaseType;
            public bool isCureVampirism;
            public bool isCureLycanthropy;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.diseaseType = diseaseType;
            data.isCureVampirism = isCureVampirism;
            data.isCureLycanthropy = isCureLycanthropy;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            diseaseType = data.diseaseType;
            isCureVampirism = data.isCureVampirism;
            isCureLycanthropy = data.isCureLycanthropy;
        }

        #endregion
    }
}