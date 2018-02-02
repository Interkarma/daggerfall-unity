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

        /// <summary>
        /// Creates enemy GameObjects based on spawn count specified in Foe resource (minimum of 1).
        /// Only use this when live enemy is to be first added to scene. Do not use when linking to site or deserializing.
        /// GameObjects created will be disabled, at origin, parentless, and have a new UID for LoadID.
        /// Caller must otherwise complete GameObject setup to suit their needs.
        /// </summary>
        /// <param name="reaction">Foe is hostile by default but can optionally set to passive.</param>
        /// <returns>GameObject[] array of 1-N foes. Array can be null or empty if create fails.</returns>
        public GameObject[] CreateFoeGameObjects(Vector3 position, int spawnOverride = -1, MobileReactions reaction = MobileReactions.Hostile)
        {
            List<GameObject> gameObjects = new List<GameObject>();

            // Get spawn count allowing for caller to override
            int totalSpawns = (spawnOverride >= 1) ? spawnOverride : spawnCount;

            // Generate GameObjects
            for (int i = 0; i < totalSpawns; i++)
            {
                // Generate enemy
                string name = string.Format("DaggerfallEnemy [{0}]", foeType.ToString());
                GameObject go = GameObjectHelper.InstantiatePrefab(DaggerfallUnity.Instance.Option_EnemyPrefab.gameObject, name, null, position);
                SetupDemoEnemy setupEnemy = go.GetComponent<SetupDemoEnemy>();
                if (setupEnemy != null)
                {
                    // Assign gender randomly
                    MobileGender gender;
                    if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
                        gender = MobileGender.Male;
                    else
                        gender = MobileGender.Female;

                    // Configure enemy
                    setupEnemy.ApplyEnemySettings(foeType, reaction, gender);

                    // Align non-flying units with ground
                    DaggerfallMobileUnit mobileUnit = setupEnemy.GetMobileBillboardChild();
                    if (mobileUnit.Summary.Enemy.Behaviour != MobileBehaviour.Flying)
                        GameObjectHelper.AlignControllerToGround(go.GetComponent<CharacterController>());

                    // Add QuestResourceBehaviour to GameObject
                    QuestResourceBehaviour questResourceBehaviour = go.AddComponent<QuestResourceBehaviour>();
                    questResourceBehaviour.AssignResource(this);
                }

                // Assign load id
                DaggerfallEnemy enemy = go.GetComponent<DaggerfallEnemy>();
                if (enemy)
                {
                    enemy.LoadID = DaggerfallUnity.NextUID;
                    enemy.QuestSpawn = true;
                }

                // Disable GameObject, caller must set active when ready
                go.SetActive(false);

                // Add to list
                gameObjects.Add(go);
            }

            return gameObjects.ToArray();
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
                DFRandom.srand(DateTime.Now.Millisecond);
                displayName = DaggerfallUnity.Instance.NameHelper.MonsterName();
                return;
            }

            // TODO: Create a random humanoid foe name
            // Will get to this testing quests that assign player to defeat and return a class-based NPC Foe
            // Have more problems to solve before getting to name
        }

        #endregion

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public int spawnCount;
            public MobileTypes foeType;
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
            data.injuredTrigger = injuredTrigger;
            data.restrained = restrained;
            data.killCount = killCount;
            data.displayName = displayName;
            data.typeName = typeName;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            SaveData_v1 data = (SaveData_v1)dataIn;
            if (dataIn == null)
                return;

            spawnCount = data.spawnCount;
            foeType = data.foeType;
            injuredTrigger = data.injuredTrigger;
            restrained = data.restrained;
            killCount = data.killCount;
            displayName = data.displayName;
            typeName = data.typeName;
        }

        #endregion
    }
}