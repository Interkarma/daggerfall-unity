// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyl@dfworkshop.net)
// Contributors:    Gavin Clayton (interkarma@dfworkshop.net)
// 
// Notes:
//

using UnityEngine;
using System.Collections.Generic;
using DaggerfallConnect.Utility;

namespace DaggerfallWorkshop.Game.Utility
{
    /// <summary>
    /// Helper to calculate overland travel time for travel map and Clock resource.
    /// Travel time needs to be coordinated between these systems for quests to provide a
    /// realistic amount of time for player to complete quest.
    /// 
    /// Notes:
    ///  * Created by Lypyl. Moved to own class by Interkarma so logic can be shared across systems.
    /// </summary>
    public class TravelTimeCalculator
    {
        #region Fields

        public const float BaseTemperateTravelTime = 60.5f;         // Represents time to travel 1 pixel on foot recklessly, camping out, for different terrains
        public const float BaseDesert224_225TravelTime = 63.5f;     // Should result in travel times fairly close to classic Daggerfall
        public const float BaseDesert229TravelTime = 65.5f;
        public const float BaseMountain226TravelTime = 67.5f;
        public const float BaseSwamp227_228TravelTime = 72.5f;
        public const float BaseMountain230TravelTime = 60.5f;
        public const float BaseOceanTravelTime = 153.65f;
        public const float InnModifier = .86f;
        public const float HorseMod = .5f;
        public const float CartMod = .75f;
        public const float ShipMod = .3125f;
        public const int CautiousMod = 2;

        List<TerrainTypes> terrains = new List<TerrainTypes>();

        float travelTimeTotalLand = 0;
        float travelTimeTotalWater = 0;

        #endregion

        #region Properties

        /// <summary>
        /// Gets total travel time over land.
        /// </summary>
        public float TravelTimeTotalLand
        {
            get { return travelTimeTotalLand; }
        }

        /// <summary>
        /// Gets total travel time over water.
        /// </summary>
        public float TravelTimeTotalWater
        {
            get { return travelTimeTotalWater; }
        }

        #endregion

        #region Enums

        public enum TerrainTypes
        {
            None = 0,
            ocean = 223,
            Desert = 224,
            Desert2 = 225,
            Mountain = 226,
            Swamp = 227,
            Swamp2 = 228,
            Desert3 = 229,
            Mountain2 = 230,
            Temperate = 231,
            Temperate2 = 232
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates an overland path from player's current location to destination.
        /// This must be called before calculating distance.
        /// </summary>
        public void GeneratePath(DFPosition endPos)
        {
            Vector2[] directions = new Vector2[] { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0), new Vector2(1, 1), new Vector2(-1, 1), new Vector2(1, -1), new Vector2(-1, -1) };
            //Vector2[] directions = new Vector2[] { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0)}; //4 direction movement
            Vector2 current = new Vector2(GameManager.Instance.PlayerGPS.CurrentMapPixel.X, GameManager.Instance.PlayerGPS.CurrentMapPixel.Y);
            Vector2 end = new Vector2(endPos.X, endPos.Y);
            terrains.Clear();
            while (current != end)
            {
                float distance = Vector2.Distance(current, end);
                int selection = 0;

                for (int i = 0; i < directions.Length; i++)
                {
                    Vector2 next = current + directions[i];
                    if (current.x < 0 || current.y < 0 || current.x >= DaggerfallConnect.Arena2.MapsFile.MaxMapPixelX || current.y >= DaggerfallConnect.Arena2.MapsFile.MaxMapPixelY)
                        continue;

                    float check = Vector2.Distance(next, end);
                    if (check < distance)
                    {
                        distance = check;
                        selection = i;
                    }

                }

                current += directions[selection];
                terrains.Add((TerrainTypes)DaggerfallUnity.Instance.ContentReader.MapFileReader.GetClimateIndex((int)current.x, (int)current.y));
            }
        }

        /// <summary>
        /// Clears generated path to tidy up.
        /// </summary>
        public void ClearPath()
        {
            terrains.Clear();
        }

        /// <summary>
        /// Calculates total travel time based on current overland path.
        /// Interkarma Notes:
        ///  * Original used TravelFoot as both local Property and passed as parameter via horse.
        ///  * Kept original behaviour so code executes the same, but should probably be cleaned up.
        /// </summary>
        public void CalculateTravelTimeTotal(
            bool travelFoot,
            bool playerHasCart,
            bool playerHasHorse,
            bool cautiousSpeed = false,
            bool inn = false,
            bool horse = false,
            bool cart = false,
            bool ship = false)
        {
            travelTimeTotalLand = 0;
            travelTimeTotalWater = 0;
            foreach (TerrainTypes terrain in terrains)
            {
                CalculateTravelTime(terrain, travelFoot, playerHasCart, playerHasHorse, cautiousSpeed, inn, horse, cart, ship);
            }

            //Debug.Log(string.Format("Total Time Cost: {0}  Inn: {1} PlayerHasShip {2} PlayerHasCart: {3} PlayerHasHorse: {4}", time, inn, PlayerHasShip, PlayerHasCart, PlayerHasHorse));
        }

        #endregion

        #region Private Methods

        void CalculateTravelTime(TerrainTypes terrainType,
            bool travelFoot,
            bool playerHasCart,
            bool playerHasHorse,
            bool cautiousSpeed = false,
            bool inn = false,
            bool horse = false,
            bool cart = false,
            bool ship = false)
        {
            float travelTimeLand = 0;
            float travelTimeWater = 0;

            switch (terrainType)
            {
                case TerrainTypes.None:
                    travelTimeLand += BaseTemperateTravelTime;
                    break;
                case TerrainTypes.ocean:
                    travelTimeWater += BaseOceanTravelTime;
                    break;
                case TerrainTypes.Desert:
                    travelTimeLand += BaseDesert224_225TravelTime;
                    break;
                case TerrainTypes.Desert2:
                    travelTimeLand += BaseDesert224_225TravelTime;
                    break;
                case TerrainTypes.Mountain:
                    travelTimeLand += BaseMountain226TravelTime;
                    break;
                case TerrainTypes.Swamp:
                    travelTimeLand += BaseSwamp227_228TravelTime;
                    break;
                case TerrainTypes.Swamp2:
                    travelTimeLand += BaseSwamp227_228TravelTime;
                    break;
                case TerrainTypes.Desert3:
                    travelTimeLand += BaseDesert229TravelTime;
                    break;
                case TerrainTypes.Mountain2:
                    travelTimeLand += BaseMountain230TravelTime;
                    break;
                case TerrainTypes.Temperate:
                    travelTimeLand += BaseTemperateTravelTime;
                    break;
                case TerrainTypes.Temperate2:
                    travelTimeLand += BaseTemperateTravelTime;
                    break;
                default:
                    travelTimeLand += BaseTemperateTravelTime;
                    break;
            }

            if (terrainType == TerrainTypes.ocean && !travelFoot)
                travelTimeWater *= ShipMod;
            else if (terrainType != TerrainTypes.ocean)
            {
                if (inn)
                    travelTimeLand *= InnModifier;
                if (playerHasCart)
                    travelTimeLand *= CartMod;
                else if (playerHasHorse)
                    travelTimeLand *= HorseMod;
            }

            if (cautiousSpeed)
            {
                travelTimeLand *= CautiousMod;
                travelTimeWater *= CautiousMod;
            }

            //Debug.Log(string.Format("Time Cost: {0} Terrain Type: {1} Inn: {2} PlayerHasShip {3} PlayerHasCart: {4} PlayerHasHorse: {5}", time, terrainType.ToString(), inn, PlayerHasShip, PlayerHasCart, PlayerHasHorse));
            travelTimeTotalLand += travelTimeLand;
            travelTimeTotalWater += travelTimeWater;
        }

        #endregion
    }
}