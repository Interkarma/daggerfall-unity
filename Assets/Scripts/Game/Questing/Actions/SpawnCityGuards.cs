// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    DFIronman, Hazelnut
// 
// Notes:
//

using System.Text.RegularExpressions;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Spawns city guards using PlayerEntity.SpawnCityGuards().
    /// </summary>
    public class SpawnCityGuards : ActionTemplate
    {
        bool immediateSpawn;

        public override string Pattern
        {
            get { return @"spawncityguards (immediate)?"; }
        }

        public SpawnCityGuards(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Trim source end or trailing white space will be split to an empty symbol at end of array
            source = source.TrimEnd();

            SpawnCityGuards action = new SpawnCityGuards(parentQuest);
            action.immediateSpawn = match.Groups.Count > 1;

            return action;
        }

        public override void Update(Task caller)
        {
            base.Update(caller);

            GameManager.Instance.PlayerEntity.SpawnCityGuards(immediateSpawn);

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public bool immediateSpawn;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.immediateSpawn = immediateSpawn;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            immediateSpawn = data.immediateSpawn;
        }

        #endregion
    }
}