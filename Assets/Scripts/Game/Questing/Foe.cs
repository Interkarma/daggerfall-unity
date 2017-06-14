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
using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// A quest Foe defines an enemy for player to fight as part of a quest.
    /// </summary>
    public class Foe : QuestResource
    {
        // Testing shows classic can spawn at least 50 of the same enemy at a time, and probably many more
        const int maxSpawnCount = 50;

        int spawnCount;
        MobileTypes foeType;

        public MobileTypes FoeType
        {
            get { return foeType; }
        }

        public int SpawnCount
        {
            get { return spawnCount; }
        }

        public Foe(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public Foe(Quest parentQuest, string line)
            : base(parentQuest)
        {
            SetResource(line);
        }

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
            }
        }

        /// <summary>
        /// Creates enemy GameObjects based on spawn count specified in Foe resource (minimum of 1).
        /// Only use this when live enemy is to be first added to scene. Do not use when linking to site or deserializing.
        /// GameObjects created will be disabled, at origin, parentless, and have a new UID for LoadID.
        /// Caller must otherwise complete GameObject setup to suit their needs.
        /// </summary>
        /// <param name="reaction">Foe is hostile by default but can optionally set to passive.</param>
        /// <returns>GameObject[] array of 1-N foes. Array can be null or empty if create fails.</returns>
        public GameObject[] CreateFoeGameObjects(MobileReactions reaction = MobileReactions.Hostile)
        {
            List<GameObject> gameObjects = new List<GameObject>();

            // Seed random
            UnityEngine.Random.InitState(Time.frameCount);

            // Generate GameObjects
            for (int i = 0; i < spawnCount; i++)
            {
                // Generate enemy
                string name = string.Format("DaggerfallEnemy [{0}]", foeType.ToString());
                GameObject go = GameObjectHelper.InstantiatePrefab(DaggerfallUnity.Instance.Option_EnemyPrefab.gameObject, name, null, Vector3.zero);
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
                }

                // Assign load id
                DaggerfallEnemy enemy = go.GetComponent<DaggerfallEnemy>();
                if (enemy)
                {
                    enemy.LoadID = DaggerfallUnity.NextUID;
                }

                // Add to list
                gameObjects.Add(go);
            }

            return gameObjects.ToArray();
        }
    }
}