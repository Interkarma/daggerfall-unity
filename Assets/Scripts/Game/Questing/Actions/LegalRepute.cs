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

using System.Text.RegularExpressions;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Legal repute modifies player legal reputation in current region.
    /// </summary>
    public class LegalRepute : ActionTemplate
    {
        int amount = 0;

        public override string Pattern
        {
            get { return @"legal repute (?<amount>[+-]?\d+)"; }
        }

        public LegalRepute(Quest parentQuest)
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
            LegalRepute action = new LegalRepute(parentQuest);
            action.amount = Parser.ParseInt(match.Groups["amount"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            // Perform action changes
            int region = GameManager.Instance.PlayerGPS.CurrentRegionIndex;
            GameManager.Instance.PlayerEntity.RegionData[region].LegalRep += (short)amount;
            GameManager.Instance.PlayerEntity.ClampLegalReputations();

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public int amount;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.amount = amount;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            amount = data.amount;
        }

        #endregion
    }
}