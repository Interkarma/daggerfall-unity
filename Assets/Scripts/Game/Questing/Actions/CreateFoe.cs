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

            // Check for a spawn event
            ulong gameSeconds = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToSeconds();
            if (gameSeconds > lastSpawnTime + spawnInterval)
            {
                // Spawn the foe resource
                SpawnFoe();

                // Update last spawn time and timer
                lastSpawnTime = gameSeconds;
            }
        }

        void SpawnFoe()
        {
            // Roll for spawn chance
            float chance = spawnChance / 100f;
            if (UnityEngine.Random.Range(0f, 1f) > chance)
                return;

            // Get the Foe resource
            Foe foe = ParentQuest.GetFoe(foeSymbol);
            if (foe == null)
                throw new Exception(string.Format("create foe could not find Foe with symbol name {0}", Symbol.Name));

            // Get foe objects
            GameObject[] gameObjects = foe.CreateFoeGameObjects(Vector3.zero);
            if (gameObjects == null || gameObjects.Length != foe.SpawnCount)
                throw new Exception(string.Format("create foe attempted to spawn {0}x{1} and failed.", foe.SpawnCount, Symbol.Name));

            // Place in world near player depending on local area
            PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;
            if (playerEnterExit.IsPlayerInsideBuilding)
            {
                PlaceFoeBuildingInterior(gameObjects, playerEnterExit.Interior);
            }

            // Increment counter
            spawnCounter++;
        }

        // Place foe somewhere near player when inside a building
        // Building interiors have spawn nodes for this placement
        void PlaceFoeBuildingInterior(GameObject[] gameObjects, DaggerfallInterior interiorParent)
        {
            foreach(GameObject go in gameObjects)
            {
                Vector3 spawnPosition;
                bool spawnPositionFound = interiorParent.GetRandomSpawnPoint(out spawnPosition);
                AlignFoe(go, interiorParent.transform, spawnPosition, !spawnPositionFound);
            }
        }

        // Place foe somewhere near player when outside a location navgrid is available
        // Navgrid placement helps foe avoid getting tangled in geometry like buildings
        void PlaceFoeExteriorLocation(GameObject[] gameObjects)
        {
            Debug.LogFormat("Attempt was made to place {0} foes to Exterior Location, but this has not been implemented yet.", gameObjects.Length);
        }

        // Place foe somewhere near player when inside a dungeon
        // Dungeons interiors are complex 3D environments with no navgrid/navmesh or (known) spawn nodes
        // This solution will use the nearest "random flat" editor marker for now
        void PlaceFoeDungeonInterior(GameObject[] gameObjects)
        {
            Debug.LogFormat("Attempt was made to place {0} foes to Dungeon Interior, but this has not been implemented yet.", gameObjects.Length);
        }

        // Place foe somewhere near player when outside and no navgrid available
        // Wilderness environments are currently open so can be placed on ground anywhere within range
        void PlaceFoeWilderness(GameObject[] gameObjects)
        {
            Debug.LogFormat("Attempt was made to place {0} foes to Wilderness, but this has not been implemented yet.", gameObjects.Length);
        }

        // Align foe in world after spawn with handling for fallback placement
        // Will set foe GameObject active when complete
        void AlignFoe(GameObject go, Transform parent, Vector3 localPosition, bool fallbackPlacement = false)
        {
            go.transform.parent = parent;

            if (!fallbackPlacement)
            {
                go.transform.localPosition = localPosition;
            }
            else
            {
                Debug.Log("Fallback placing behind player");
                PlayerMotor playerMotor = GameManager.Instance.PlayerMotor;
                go.transform.position = playerMotor.transform.position + -playerMotor.transform.forward;
            }

            DaggerfallMobileUnit mobileUnit = go.GetComponentInChildren<DaggerfallMobileUnit>();
            if (mobileUnit)
            {
                // Align ground creatures on surface, raise flying creatures slightly into air
                if (mobileUnit.Summary.Enemy.Behaviour != MobileBehaviour.Flying)
                    GameObjectHelper.AlignControllerToGround(go.GetComponent<CharacterController>());
                else
                    go.transform.localPosition += Vector3.up * 1.5f;
            }
            else
            {
                // Just align to ground
                GameObjectHelper.AlignControllerToGround(go.GetComponent<CharacterController>());
            }

            go.SetActive(true);
        }
    }
}