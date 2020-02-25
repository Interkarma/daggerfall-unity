// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
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
using DaggerfallWorkshop.Utility;
using FullSerializer;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Spawn a Foe resource into the world.
    /// </summary>
    public class CreateFoe : ActionTemplate
    {
        const string optionsMatchStr = @"msg (?<msgId>\d+)";

        Symbol foeSymbol;
        uint spawnInterval;
        int spawnMaxTimes = -1;
        int spawnChance;
        int msgMessageID = -1;

        ulong lastSpawnTime = 0;
        int spawnCounter = 0;

        bool spawnInProgress = false;
        GameObject[] pendingFoeGameObjects;
        int pendingFoesSpawned;
        bool isSendAction = false;

        public override string Pattern
        {
            get
            {
                return @"create foe (?<symbol>[a-zA-Z0-9_.-]+) every (?<minutes>\d+) minutes (?<infinite>indefinitely) with (?<percent>\d+)% success|" +
                       @"create foe (?<symbol>[a-zA-Z0-9_.-]+) every (?<minutes>\d+) minutes (?<count>\d+) times with (?<percent>\d+)% success|" +
                       @"(?<send>send) (?<symbol>[a-zA-Z0-9_.-]+) every (?<minutes>\d+) minutes (?<count>\d+) times with (?<percent>\d+)% success|" +
                       @"(?<send>send) (?<symbol>[a-zA-Z0-9_.-]+) every (?<minutes>\d+) minutes with (?<percent>\d+)% success";
            }
        }

        public CreateFoe(Quest parentQuest)
            : base(parentQuest)
        {
            PlayerEnterExit.OnTransitionDungeonExterior += PlayerEnterExit_OnTransitionExterior;
            PlayerEnterExit.OnTransitionExterior += PlayerEnterExit_OnTransitionExterior;
            StreamingWorld.OnInitWorld += StreamingWorld_OnInitWorld;
        }

        public override void InitialiseOnSet()
        {
            lastSpawnTime = 0;
            spawnCounter = 0;
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
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

            // Handle "send" variant
            if (!string.IsNullOrEmpty(match.Groups["send"].Value))
            {
                action.isSendAction = true;

                // "send" without "count" implies infinite
                if (action.spawnMaxTimes == 0)
                    action.spawnMaxTimes = -1;
            }

            // Split options from declaration
            string optionsSource = source.Substring(match.Length);
            MatchCollection options = Regex.Matches(optionsSource, optionsMatchStr);
            foreach (Match option in options)
            {
                // Message ID
                Group msgIDGroup = option.Groups["msgId"];
                if (msgIDGroup.Success)
                    action.msgMessageID = Parser.ParseInt(msgIDGroup.Value);
            }

            return action;
        }

        public override void Update(Task caller)
        {
            ulong gameSeconds = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToSeconds();

            // Init spawn timer on first update
            if (lastSpawnTime == 0)
                lastSpawnTime = gameSeconds - (uint)UnityEngine.Random.Range(0, spawnInterval);

            // Do nothing if max foes already spawned
            // This can be cleared on next set/rearm
            if (spawnCounter >= spawnMaxTimes && spawnMaxTimes != -1)
                return;

            // Clear pending foes if all have been spawned
            if (spawnInProgress && pendingFoesSpawned >= pendingFoeGameObjects.Length)
            {
                spawnInProgress = false;
                spawnCounter++;
                return;
            }

            // Check for a new spawn event - only one spawn event can be running at a time
            if (gameSeconds >= lastSpawnTime + spawnInterval && !spawnInProgress)
            {
                // Update last spawn time
                lastSpawnTime = gameSeconds;

                // Roll for spawn chance
                if (Dice100.FailedRoll(spawnChance))
                    return;

                // Get the Foe resource
                Foe foe = ParentQuest.GetFoe(foeSymbol);
                if (foe == null)
                {
                    SetComplete();
                    throw new Exception(string.Format("create foe could not find Foe with symbol name {0}", Symbol.Name));
                }

                // Do not spawn if foe is hidden
                if (foe.IsHidden)
                    return;

                // Start deploying GameObjects
                CreatePendingFoeSpawn(foe);
            }

            // Try to deploy a pending spawns
            if (spawnInProgress)
            {
                TryPlacement();
                GameManager.Instance.RaiseOnEncounterEvent();
            }
        }

        #region Private Methods

        void CreatePendingFoeSpawn(Foe foe)
        {
            // Get foe GameObjects
            pendingFoeGameObjects = GameObjectHelper.CreateFoeGameObjects(Vector3.zero, foe.FoeType, foe.SpawnCount, MobileReactions.Hostile, foe);
            if (pendingFoeGameObjects == null || pendingFoeGameObjects.Length != foe.SpawnCount)
            {
                SetComplete();
                throw new Exception(string.Format("create foe attempted to create {0}x{1} GameObjects and failed.", foe.SpawnCount, Symbol.Name));
            }

            // Initiate deployment process
            // Usually the foe will spawn immediately but can take longer depending on available placement space
            // This process ensures these foes have all been deployed before starting next cycle
            spawnInProgress = true;
            pendingFoesSpawned = 0;
        }

        void TryPlacement()
        {
            PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;

            // The "send" variant is only used when player within a town/exterior location
            // The placement will remain pending until player matches conditions
            if (isSendAction)
            {
                if (!GameManager.Instance.PlayerGPS.IsPlayerInLocationRect)
                    return;
            }

            // Place in world near player depending on local area
            if (playerEnterExit.IsPlayerInsideBuilding)
            {
                PlaceFoeBuildingInterior(pendingFoeGameObjects, playerEnterExit.Interior);
            }
            else if (playerEnterExit.IsPlayerInsideDungeon)
            {
                PlaceFoeDungeonInterior(pendingFoeGameObjects, playerEnterExit.Dungeon);
            }
            else if (!playerEnterExit.IsPlayerInside && GameManager.Instance.PlayerGPS.IsPlayerInLocationRect)
            {
                PlaceFoeExteriorLocation(pendingFoeGameObjects, GameManager.Instance.StreamingWorld.CurrentPlayerLocationObject);
            }
            else
            {
                PlaceFoeWilderness(pendingFoeGameObjects);
            }
        }

        #endregion

        #region Placement Methods

        // Place foe somewhere near player when inside a building
        // Building interiors have spawn nodes for this placement so we can roll out foes all at once
        void PlaceFoeBuildingInterior(GameObject[] gameObjects, DaggerfallInterior interiorParent)
        {
            // Must have a DaggerfallLocation parent
            if (interiorParent == null)
            {
                SetComplete();
                throw new Exception("PlaceFoeFreely() must have a DaggerfallLocation parent object.");
            }

            // Always place foes around player rather than use spawn points
            // Spawn points work well for "interior hunt" quests but less so for "directly attack the player"
            // Feel just placing freely will yield best results overall
            PlaceFoeFreely(gameObjects, interiorParent.transform);
            return;
        }

        // Place foe somewhere near player when inside a dungeon
        // Dungeons interiors are complex 3D environments with no navgrid/navmesh or known spawn nodes
        void PlaceFoeDungeonInterior(GameObject[] gameObjects, DaggerfallDungeon dungeonParent)
        {
            PlaceFoeFreely(gameObjects, dungeonParent.transform);
        }

        // Place foe somewhere near player when outside a location navgrid is available
        // Navgrid placement helps foe avoid getting tangled in geometry like buildings
        void PlaceFoeExteriorLocation(GameObject[] gameObjects, DaggerfallLocation locationParent)
        {
            PlaceFoeFreely(gameObjects, locationParent.transform);
        }

        // Place foe somewhere near player when outside and no navgrid available
        // Wilderness environments are currently open so can be placed on ground anywhere within range
        void PlaceFoeWilderness(GameObject[] gameObjects)
        {
            // TODO this false will need to be true when start caching enemies
            GameManager.Instance.StreamingWorld.TrackLooseObject(gameObjects[pendingFoesSpawned], false, -1, -1, true);
            PlaceFoeFreely(gameObjects, null, 8f, 25f);
        }

        // Uses raycasts to find next spawn position just outside of player's field of view
        void PlaceFoeFreely(GameObject[] gameObjects, Transform parent, float minDistance = 5f, float maxDistance = 20f)
        {
            const float overlapSphereRadius = 0.65f;
            const float separationDistance = 1.25f;
            const float maxFloorDistance = 4f;

            // Must have received a valid array
            if (gameObjects == null || gameObjects.Length == 0)
                return;

            // Set parent - otherwise caller must set a parent
            if (parent)
                gameObjects[pendingFoesSpawned].transform.parent = parent;

            // Select a left or right direction outside of camera FOV
            Quaternion rotation;
            float directionAngle = GameManager.Instance.MainCamera.fieldOfView;
            directionAngle += UnityEngine.Random.Range(0f, 4f);
            if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                rotation = Quaternion.Euler(0, -directionAngle, 0);
            else
                rotation = Quaternion.Euler(0, directionAngle, 0);

            // Get direction vector and create a new ray
            Vector3 angle = (rotation * Vector3.forward).normalized;
            Vector3 spawnDirection = GameManager.Instance.PlayerObject.transform.TransformDirection(angle).normalized;
            Ray ray = new Ray(GameManager.Instance.PlayerObject.transform.position, spawnDirection);

            // Check for a hit
            Vector3 currentPoint;
            RaycastHit initialHit;
            if (Physics.Raycast(ray, out initialHit, maxDistance))
            {
                float cos_normal = Vector3.Dot(- spawnDirection, initialHit.normal.normalized);
                if (cos_normal < 1e-6)
                    return;
                float separationForward = separationDistance / cos_normal;

                // Must be greater than minDistance
                float distanceSlack = initialHit.distance - separationForward - minDistance;
                if (distanceSlack < 0f)
                    return;

                // Separate out from hit point
                float extraDistance = UnityEngine.Random.Range(0f, Mathf.Min(2f, distanceSlack));
                currentPoint = initialHit.point - spawnDirection * (separationForward + extraDistance);
            }
            else
            {
                // Player might be in an open area (e.g. outdoors) pick a random point along spawn direction
                currentPoint = GameManager.Instance.PlayerObject.transform.position + spawnDirection * UnityEngine.Random.Range(minDistance, maxDistance);
            }

            // Must be able to find a surface below
            RaycastHit floorHit;
            ray = new Ray(currentPoint, Vector3.down);
            if (!Physics.Raycast(ray, out floorHit, maxFloorDistance))
                return;

            // Ensure this is open space
            Vector3 testPoint = floorHit.point + Vector3.up * separationDistance;
            Collider[] colliders = Physics.OverlapSphere(testPoint, overlapSphereRadius);
            if (colliders.Length > 0)
                return;

            // This looks like a good spawn position
            pendingFoeGameObjects[pendingFoesSpawned].transform.position = testPoint;
            FinalizeFoe(pendingFoeGameObjects[pendingFoesSpawned]);
            gameObjects[pendingFoesSpawned].transform.LookAt(GameManager.Instance.PlayerObject.transform.position);

            // Send msg message on first spawn only
            if (msgMessageID != -1)
            {
                ParentQuest.ShowMessagePopup(msgMessageID);
                msgMessageID = -1;
            }

            // Increment count
            pendingFoesSpawned++;
        }

        // Fine tunes foe position slightly based on mobility and enables GameObject
        void FinalizeFoe(GameObject go)
        {
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

        #endregion

        #region Event Handlers

        private void PlayerEnterExit_OnTransitionExterior(PlayerEnterExit.TransitionEventArgs args)
        {
            // Any foes pending placement to dungeon or building interior are now invalid
            pendingFoeGameObjects = null;
            spawnInProgress = false;
        }

        private void StreamingWorld_OnInitWorld()
        {
            // Any foes pending placement to loose objects container are now invalid
            pendingFoeGameObjects = null;
            spawnInProgress = false;
        }

        #endregion

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol foeSymbol;
            public ulong lastSpawnTime;
            public uint spawnInterval;
            public int spawnMaxTimes;
            public int spawnChance;
            public int spawnCounter;
            public bool isSendAction;
            public int msgMessageID;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.foeSymbol = foeSymbol;
            data.lastSpawnTime = lastSpawnTime;
            data.spawnInterval = spawnInterval;
            data.spawnMaxTimes = spawnMaxTimes;
            data.spawnChance = spawnChance;
            data.spawnCounter = spawnCounter;
            data.isSendAction = isSendAction;
            data.msgMessageID = msgMessageID;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            foeSymbol = data.foeSymbol;
            lastSpawnTime = data.lastSpawnTime;
            spawnInterval = data.spawnInterval;
            spawnMaxTimes = data.spawnMaxTimes;
            spawnChance = data.spawnChance;
            spawnCounter = data.spawnCounter;
            isSendAction = data.isSendAction;
            msgMessageID = data.msgMessageID;

            // Set timer to current game time if not loaded from save
            if (lastSpawnTime == 0)
                lastSpawnTime = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToSeconds();
        }

        #endregion
    }
}