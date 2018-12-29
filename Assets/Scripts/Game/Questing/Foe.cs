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

using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect.Arena2;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// A quest Foe defines an enemy for player to fight as part of a quest.
    /// Foes are a 1-to-many resource (1 Foe resource to many entities).
    /// For example, a quest might send waves of giants at the player.
    /// </summary>
    public class Foe : QuestResource
    {
        #region Fields

        // Highest spawn value across all canonical quests is 6, setting to 8 as maximum in Daggerfall Unity
        const int maxSpawnCount = 8;

        int spawnCount;                     // How many foes to spawn
        MobileTypes foeType;                // MobileType to spawn
        Genders humanoidGender;             // Foe gender for humanoid foes
        bool injuredTrigger;                // True once enemy injured, rearmed each wave
        bool restrained;                    // True if enemy restrained by quest
        int killCount;                      // How many of this enemy spawn player has killed, does not rearm
        string displayName;                 // Foe display name for quest system macros
        string typeName;                    // Foe type name for quest system macros

        #endregion

        #region Properties

        public MobileTypes FoeType
        {
            get { return foeType; }
        }

        public override Genders Gender
        {
            get { return ((int)foeType < 128) ? Genders.Male : humanoidGender; }
        }

        public int SpawnCount
        {
            get { return spawnCount; }
        }

        public bool InjuredTrigger
        {
            get { return injuredTrigger; }
        }

        public bool IsRestrained
        {
            get { return restrained; }
        }

        public int KillCount
        {
            get { return killCount; }
        }

        #endregion

        #region Constructors

        public Foe(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public Foe(Quest parentQuest, string line)
            : base(parentQuest)
        {
            SetResource(line);
        }

        #endregion

        #region Overrides

        public override void SetResource(string line)
        {
            base.SetResource(line);

            string matchStr = @"(Foe|foe) (?<symbol>[a-zA-Z0-9_.-]+) is (?<count>\d+) (?<aFoe>\w+)|(Foe|foe) (?<symbol>[a-zA-Z0-9_.-]+) is (?<aFoe>\w+)";

            Match match = Regex.Match(line, matchStr);
            if (match.Success)
            {
                // Store symbol for quest system
                Symbol = new Symbol(match.Groups["symbol"].Value);

                // Get name of foe and count
                string name = match.Groups["aFoe"].Value;
                int count = Parser.ParseInt(match.Groups["count"].Value);

                // Read foe from table data
                Table foesTable = QuestMachine.Instance.FoesTable;
                if (foesTable.HasValue(name))
                {
                    foeType = (MobileTypes)Parser.ParseInt(foesTable.GetValue("id", name));
                }
                else
                {
                    throw new Exception(string.Format("Foes data table does not contain an entry for {0}", name));
                }

                // Setup spawn count
                spawnCount = Mathf.Clamp(count, 1, maxSpawnCount);

                // Assign name
                SetFoeName();
            }
        }

        public override bool ExpandMacro(MacroTypes macro, out string textOut)
        {
            // Store this foe in quest as last Foe encountered
            // This will be used for subsequent pronoun macros, etc.
            ParentQuest.LastResourceReferenced = this;

            textOut = string.Empty;
            bool result = true;
            switch (macro)
            {
                case MacroTypes.NameMacro1:             // _symbol_ name
                    textOut = typeName;
                    break;

                case MacroTypes.DetailsMacro:           // =symbol_ name
                    textOut = displayName;
                    break;

                default:                                // Macro not supported
                    result = false;
                    break;
            }

            return result;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Triggers injured pickup.
        /// </summary>
        public void SetInjured()
        {
            injuredTrigger = true;
        }

        /// <summary>
        /// Rearms injured pickup.
        /// </summary>
        public void RearmInjured()
        {
            injuredTrigger = false;
        }

        /// <summary>
        /// Sets restrained flag.
        /// </summary>
        public void SetRestrained()
        {
            restrained = true;
        }

        /// <summary>
        /// Clears restrained flag.
        /// </summary>
        public void ClearRestrained()
        {
            restrained = false;
        }

        /// <summary>
        /// Increments count of this Foe killed by player.
        /// </summary>
        /// <param name="amount">Amount to raise by, usually 1.</param>
        public void IncrementKills(int amount = 1)
        {
            killCount += amount;
        }

        #endregion

        #region Private Methods

        void SetFoeName()
        {
            // Set type name with fallback
            MobileEnemy enemy;
            if (EnemyBasics.GetEnemy(foeType, out enemy))
                typeName = enemy.Name;
            else
                typeName = foeType.ToString();

            // Monster types get a random monster name
            // Always treating monsters as male for now as they don't have any gender in game files
            if ((int)foeType < 128)
            {
                DFRandom.srand(DateTime.Now.Millisecond + DFRandom.random_range(1, 1000000));
                displayName = DaggerfallUnity.Instance.NameHelper.MonsterName();
                return;
            }

            // Randomly assign a gender for humanoid foes
            humanoidGender = (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5f) ? Genders.Male : Genders.Female;

            // Create a random display name for humanoid foes
            DFRandom.srand(DateTime.Now.Millisecond);
            NameHelper.BankTypes nameBank = GameManager.Instance.PlayerGPS.GetNameBankOfCurrentRegion();
            displayName = DaggerfallUnity.Instance.NameHelper.FullName(nameBank, humanoidGender);
        }

        #endregion

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public int spawnCount;
            public MobileTypes foeType;
            public Genders humanoidGender;
            public bool injuredTrigger;
            public bool restrained;
            public int killCount;
            public string displayName;
            public string typeName;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.spawnCount = spawnCount;
            data.foeType = foeType;
            data.humanoidGender = humanoidGender;
            data.injuredTrigger = injuredTrigger;
            data.restrained = restrained;
            data.killCount = killCount;
            data.displayName = displayName;
            data.typeName = typeName;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            spawnCount = data.spawnCount;
            foeType = data.foeType;
            humanoidGender = data.humanoidGender;
            injuredTrigger = data.injuredTrigger;
            restrained = data.restrained;
            killCount = data.killCount;
            displayName = data.displayName;
            typeName = data.typeName;
        }

        #endregion
    }
}