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
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Spawn a Foe resource into the world.
    /// </summary>
    public class CreateFoe : ActionTemplate
    {
        Symbol foeSymbol;
        uint spawnInterval;
        int spawnMaxTimes;
        int spawnChance;

        ulong lastSpawnTime = 0;
        int spawnCounter = 0;

        public override string Pattern
        {
            get
            {
                return @"create foe (?<symbol>[a-zA-Z0-9_.-]+) every (?<minutes>\d+) minutes (?<infinite>indefinitely) with (?<percent>\d+)% success|" +
                       @"create foe (?<symbol>[a-zA-Z0-9_.-]+) every (?<minutes>\d+) minutes (?<count>\d+) times with (?<percent>\d+)% success";
            }
        }

        public CreateFoe(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override void InitialiseOnSet()
        {
            lastSpawnTime = 0;
            spawnCounter = 0;
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            base.CreateNew(source, parentQuest);

            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            CreateFoe action = new CreateFoe(parentQuest);
            action.foeSymbol = new Symbol(match.Groups["symbol"].Value);
            action.spawnInterval = (uint)Parser.ParseInt(match.Groups["minutes"].Value) * 60;
            action.spawnMaxTimes = Parser.ParseInt(match.Groups["count"].Value);
            action.spawnChance = Parser.ParseInt(match.Groups["percent"].Value);

            // Handle infinite
            if (!string.IsNullOrEmpty(match.Groups["infinite"].Value))
                action.spawnMaxTimes = -1;

            return action;
        }

        public override void Update(Task caller)
        {
            // Do nothing if max foes already spawned
            // This can be cleared on next set/rearm
            if (spawnCounter >= spawnMaxTimes && spawnMaxTimes != -1)
                return;

            // Check for a spawn
            ulong gameSeconds = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToSeconds();
            if (gameSeconds > lastSpawnTime + spawnInterval)
            {
                // Spawn the foe resource
                SpawnFoe();

                // Update last spawn time and timer
                lastSpawnTime = gameSeconds;

                // Increment counter
                spawnCounter++;
            }
        }

        void SpawnFoe()
        {
            // Roll for spawn chance
            float chance = spawnChance / 100f;
            if (UnityEngine.Random.Range(0f, 1f) < chance)
                return;

            // Get the Foe resource
            Foe foe = ParentQuest.GetFoe(foeSymbol);
            if (foe == null)
                throw new Exception(string.Format("create foe could not find Foe with symbol name {0}", Symbol.Name));

            // Get game objects
            GameObject[] gameObjects = foe.CreateFoeGameObjects();
            if (gameObjects == null || gameObjects.Length != foe.SpawnCount)
                throw new Exception(string.Format("create foe attempted to spawn {0}x{1} and failed.", foe.SpawnCount, Symbol.Name));

            // For simple initial testing just place enemies 2m behind player
            // Will eventually need to spawn enemy within a certain radius of player
            // and ensure not dropped into the void for dungeons and building interiors
            GameObject player = GameManager.Instance.PlayerObject;
            Vector3 position = player.transform.position + (-player.transform.forward * 2);
            for (int i = 0; i < gameObjects.Length; i++)
            {
                gameObjects[i].transform.position = position;
                gameObjects[i].SetActive(true);
            }
        }
    }
}