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

using UnityEngine;
using System;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;

namespace DaggerfallWorkshop.Game.Utility
{
    /// <summary>
    /// Utility class to help with mobile spawning and navigation in town environments.
    /// The navigation component is intended to be used by wandering NPCs.
    /// Mobile enemies will use normal steering behaviour to follow player.
    /// Combines inverse of automap to carve out navgrid then sets weighting by tile type.
    /// </summary>
    [RequireComponent(typeof(DaggerfallLocation))]
    public class CityNavigation : MonoBehaviour
    {
        #region Fields

        public const int DaggerfallUnitsPerTile = 64;
        public const float HalfTile = (DaggerfallUnitsPerTile * 0.5f) * MeshReader.GlobalScale;

        const int blockDimension = 64;
        const int blockSize = blockDimension * blockDimension;
        const string noLocationError = "CityNavigation is not peered to a DaggerfallLocation GameObject or no location is set.";

        string regionName;
        string locationName;
        int cityWidth = 0;
        int cityHeight = 0;
        byte[,] navGrid = null;

        DaggerfallLocation dfLocation;

        #endregion

        #region Properties

        /// <summary>
        /// Gets region name of this navgrid.
        /// </summary>
        public string RegionName
        {
            get { return regionName; }
        }

        /// <summary>
        /// Gets location name of this navgrid.
        /// </summary>
        public string LocationName
        {
            get { return locationName; }
        }

        /// <summary>
        /// Gets RMB width of city from format time.
        /// </summary>
        public int CityWidth
        {
            get { return cityWidth; }
        }

        /// <summary>
        /// Gets RMB height of city from format time.
        /// </summary>
        public int CityHeight
        {
            get { return cityHeight; }
        }

        /// <summary>
        /// Gets navgrid width in tiles.
        /// </summary>
        public int NavGridWidth
        {
            get { return cityWidth * blockDimension; }
        }

        /// <summary>
        /// Gets navgrid height in tiles.
        /// </summary>
        public int NavGridHeight
        {
            get { return cityHeight * blockDimension; }
        }

        #endregion

        #region Structs & Enums

        /// <summary>
        /// Tile types examined for weightings.
        /// </summary>
        public enum TileTypes
        {
            Water = 0,
            Dirt = 1,
            Grass = 2,
            Stone = 3,
            WaterDirtEdge1 = 5,
            WaterDirtEdge2 = 6,
            WaterGrassEdge1 = 20,
            WaterGrassEdge2 = 21,
            WaterStoneEdge1 = 30,
            WaterStoneEdge2 = 31,
            Road = 46,
            RoadCornerDirt = 47,
            RoadCornerGrass = 55,
        }

        /// <summary>
        /// Tile flags.
        /// </summary>
        [Flags]
        public enum TileFlags
        {
            None = 0,
            Occupied = 1,
        }

        #endregion

        #region Unity

        private void Start()
        {
            dfLocation = GetComponent<DaggerfallLocation>();
        }

        //bool playerWasMoving = false;
        //bool updatePlayerPos = false;

        private void Update()
        {
            //// TEST: Using player as a testing vehicle for navgrid - this will be removed
            //PlayerMotor playerMotor = GameManager.Instance.PlayerMotor;
            //PlayerGPS playerGPS = GameManager.Instance.PlayerGPS;
            //if (playerGPS.HasCurrentLocation && playerGPS.CurrentLocation.Name == locationName)
            //{
            //    if (updatePlayerPos)
            //    {
            //        DFPosition worldPosition = SceneToWorldPosition(playerMotor.transform.position);
            //        Vector3 scenePosition = WorldToScenePosition(worldPosition);
            //        Vector2 uvPosition = GetNavGridUV(worldPosition);
            //        DFPosition navPosition = WorldToNavGridPosition(worldPosition);
            //        int navWeight = GetNavGridWeightWorld(worldPosition);
            //        int navRaw = GetRawNavGridValueLocal(navPosition);
            //        Debug.LogFormat("Player world position  : X={0}, Z={1}", worldPosition.X, worldPosition.Y);
            //        Debug.LogFormat("Player scene position  : X={0}, Y={1}, Z={2}", scenePosition.x, scenePosition.y, scenePosition.z);
            //        Debug.LogFormat("Player navgrid UV      : U={0}, V={1}", uvPosition.x, uvPosition.y);
            //        Debug.LogFormat("Player navgrid position: X={0}, Y={1}", navPosition.X, navPosition.Y);
            //        Debug.LogFormat("Player navgrid weight  : W={0}", navWeight);
            //        Debug.LogFormat("Player navgrid value   : W={0}", navRaw);
            //        updatePlayerPos = false;
            //    }
            //    else
            //    {
            //        if (playerMotor.IsStandingStill && playerWasMoving)
            //            updatePlayerPos = true;
            //        else
            //            updatePlayerPos = false;
            //    }
            //    playerWasMoving = !playerMotor.IsStandingStill;
            //}
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Format the navigation grid based on town width*height in RMB blocks.
        /// A max-size city like Daggerfall is 8x8 RMB blocks.
        /// </summary>
        /// <param name="cityWidth">City RMB blocks wide. Range 1-8.</param>
        /// <param name="cityHeight">City RMB blocks high. Range 1-8.</param>
        public void FormatNavigation(string regionName, string locationName, int cityWidth, int cityHeight)
        {
            // Create grid array
            int width = Mathf.Clamp(cityWidth, 1, 8);
            int height = Mathf.Clamp(cityHeight, 1, 8);
            navGrid = new byte[width * blockDimension, height * blockDimension];
            Array.Clear(navGrid, 0, navGrid.Length);

            // Store city data
            this.regionName = regionName;
            this.locationName = locationName;
            this.cityWidth = width;
            this.cityHeight = height;
        }

        /// <summary>
        /// Set block data in navgrid.
        /// This is done during StreamingWorld location layout when this data is available.
        /// </summary>
        /// <param name="blockData">RMB block data.</param>
        /// <param name="xBlock">X block to set.</param>
        /// <param name="yBlock">Y block to set.</param>
        public void SetRMBData(in DFBlock blockData, int xBlock, int yBlock)
        {
            // Validate
            if (xBlock < 0 || xBlock >= cityWidth ||
                yBlock < 0 || yBlock >= cityHeight)
            {
                throw new Exception("CityNavigation.SetRMBData() coordinates out of range.");
            }

            // Assign data to navgrid
            for (int y = 0; y < blockDimension; y++)
            {
                for (int x = 0; x < blockDimension; x++)
                {
                    // Get source data - tilemap is 16x16 need to divide by 4
                    byte autoMapData = blockData.RmbBlock.FldHeader.AutoMapData[y * blockDimension + x];
                    byte tileRecord = (byte)blockData.RmbBlock.FldHeader.GroundData.GroundTiles[x / 4, y / 4].TextureRecord;

                    // Using inverse of automap - ignore grid position covered by anything (e.g. building, model, flat)
                    if (autoMapData != 0)
                        continue;

                    // Get target position - need to invert Y as blocks laid out from bottom-up
                    int xpos = xBlock * blockDimension + x;
                    int ypos = (cityHeight * blockDimension - blockDimension) - yBlock * blockDimension + y;

                    // Get weight value from tile
                    byte weight = GetTileWeight(tileRecord);

                    // Store final value
                    navGrid[xpos, ypos] = (byte)(weight << 4);
                }
            }
        }

        /// <summary>
        /// Convert a scene position back into virtual world space.
        /// This is specific to the peered location due to floating origin.
        /// </summary>
        /// <param name="scenePosition">Scene position to convert to nearest point in world space.</param>
        /// <returns>World DFPosition.</returns>
        public DFPosition SceneToWorldPosition(Vector3 scenePosition)
        {
            // Validate DaggerfallLocation
            if (!dfLocation || dfLocation.Summary.MapID == 0)
                throw new Exception(noLocationError);

            // Get location origin in both scene and world
            // The SW origin of location in scene spaces aligns with SW terrain tile origin in world space
            Vector3 locationOrigin = dfLocation.transform.position;
            DFPosition worldOrigin = MapsFile.MapPixelToWorldCoord(dfLocation.Summary.MapPixelX, dfLocation.Summary.MapPixelY);

            // Get difference between origin and target position in scene space
            Vector3 difference = scenePosition - locationOrigin;

            // Convert difference into Daggerfall units and apply to origin in world space
            DFPosition result = new DFPosition(
                (int)(difference.x * StreamingWorld.SceneMapRatio) + worldOrigin.X,
                (int)(difference.z * StreamingWorld.SceneMapRatio) + worldOrigin.Y);

            return result;
        }

        /// <summary>
        /// Convert a virtual world position back into scene space.
        /// This is specific to the peered location due to floating origin.
        /// Some precision loss is expected converting back to scene space.
        /// </summary>
        /// <param name="worldPosition">World location to convert to nearest point in scene space.</param>
        /// <param name="refineY">Attempt to refine Y position to actual terrain data.</param>
        /// <returns>Scene Vector3 position.</returns>
        public Vector3 WorldToScenePosition(DFPosition worldPosition, bool refineY = true)
        {
            // Validate DaggerfallLocation
            if (!dfLocation || dfLocation.Summary.MapID == 0)
                throw new Exception(noLocationError);

            // Get location origin in both scene and world
            // The SW origin of location in scene spaces aligns with SW terrain tile origin in world space
            Vector3 locationOrigin = dfLocation.transform.position;
            DFPosition worldOrigin = MapsFile.MapPixelToWorldCoord(dfLocation.Summary.MapPixelX, dfLocation.Summary.MapPixelY);

            // Get difference between origin and target position in scene space
            Vector3 offset = new Vector3(
                (worldPosition.X - worldOrigin.X) / StreamingWorld.SceneMapRatio,
                0,
                (worldPosition.Y - worldOrigin.Y) / StreamingWorld.SceneMapRatio);

            // Calculate X-Z position and use Y from location origin
            // World > scene conversion results in mobile being aligned exactly on edge of tile in scene
            // Move the mobile transform a half-tile into centre so it appears to be properly aligned
            Vector3 result = locationOrigin + offset;
            result.x += HalfTile;
            result.z -= HalfTile;

            // Attempt to refine Y by sampling terrain at this map pixel position
            if (refineY)
            {
                GameObject terrainObject = GameManager.Instance.StreamingWorld.GetTerrainFromPixel(dfLocation.Summary.MapPixelX, dfLocation.Summary.MapPixelY);
                if (terrainObject)
                {
                    // Sample actual terrain height at this scene position for Y and adjust for world compensation
                    Terrain terrain = terrainObject.GetComponent<Terrain>();
                    float height = terrain.SampleHeight(result);
                    result.y = height + GameManager.Instance.StreamingWorld.WorldCompensation.y;
                }
            }

            return result;
        }

        /// <summary>
        /// Convert world position to local navgrid position.
        /// Precision loss expected converting between world and navgrid.
        /// </summary>
        /// <param name="worldPosition">World position - will be clamped inside location area.</param>
        /// <returns>Local navgrid DFPosition.</returns>
        public DFPosition WorldToNavGridPosition(DFPosition worldPosition)
        {
            // Get navgrid coordinates inside valid navgrid range
            Vector2 uv = GetNavGridUV(worldPosition);
            int x = (int)((NavGridWidth - 1) * uv.x);
            int y = (int)((NavGridHeight - 1) * uv.y);

            return new DFPosition(x, y);
        }

        /// <summary>
        /// Convert local navgrid position into a world position.
        /// Precision loss expected converting between world and navgrid.
        /// </summary>
        /// <param name="localPosition">Local position inside navgrid.</param>
        /// <returns>World DFPosition.</returns>
        public DFPosition NavGridToWorldPosition(DFPosition localPosition)
        {
            // Validate DaggerfallLocation
            if (!dfLocation || dfLocation.Summary.MapID == 0)
                throw new Exception(noLocationError);

            // Get location rect in world space
            RectOffset rect = dfLocation.LocationRect;

            // Navgrid fits 1:1 inside location rect but navgrid origin is top-left and world origin is bottom-left
            DFPosition worldPosition = new DFPosition(
                rect.left + localPosition.X * DaggerfallUnitsPerTile,
                rect.bottom - localPosition.Y * DaggerfallUnitsPerTile);

            return worldPosition;
        }

        /// <summary>
        /// Gets UV coordinates of world position inside navgrid.
        /// </summary>
        /// <param name="worldPosition">World coordinates to test.</param>
        /// <returns>UV coordinates inside navgrid - will be clamped to 0-1 range.</returns>
        public Vector2 GetNavGridUV(DFPosition worldPosition)
        {
            RectOffset locationRect = dfLocation.LocationRect;

            int width = locationRect.right - locationRect.left;
            int height = locationRect.top - locationRect.bottom;

            float u = Mathf.Clamp01((float)(worldPosition.X - locationRect.left) / width);
            float v = Mathf.Clamp01((float)(worldPosition.Y - locationRect.bottom) / height);

            return new Vector2(u, v);
        }

        /// <summary>
        /// Gets weight of tile at world position inside navgrid.
        /// </summary>
        /// <param name="worldPosition">World position to test.</param>
        /// <returns>Weight of tile inside navgrid - will be clamped to valid range.</returns>
        public int GetNavGridWeightWorld(DFPosition worldPosition)
        {
            DFPosition pos = WorldToNavGridPosition(worldPosition);

            return navGrid[pos.X, pos.Y] >> 4;
        }

        /// <summary>
        /// Gets weight of tile at local DFPosition inside navgrid array.
        /// Safe to query outside navgrid area, will just return 0.
        /// </summary>
        /// <param name="localPosition">Local navgrid position.</param>
        /// <returns>Weight of position or 0.</returns>
        public int GetNavGridWeightLocal(DFPosition localPosition)
        {
            return GetNavGridWeightLocal(localPosition.X, localPosition.Y);
        }

        /// <summary>
        /// Gets weight of tile at local X, Y position inside navgrid array.
        /// Safe to query outside navgrid area, will just return 0.
        /// </summary>
        /// <param name="x">X position inside navgrid.</param>
        /// <param name="y">Y position inside navgrid.</param>
        /// <returns>Weight of position or 0.</returns>
        public int GetNavGridWeightLocal(int x, int y)
        {
            // Return 0 if accessing outside of array
            if (x < 0 || x >= NavGridWidth || y < 0 || y >= NavGridHeight)
                return 0;

            return navGrid[x, y] >> 4;
        }

        /// <summary>
        /// Sets navgrid tile flags.
        /// Does nothing if outside navgrid area.
        /// </summary>
        /// <param name="localPosition">Local navgrid position.</param>
        /// <param name="flags">Flags to set.</param>
        public void SetFlags(DFPosition localPosition, TileFlags flags)
        {
            SetFlags(localPosition.X, localPosition.Y, flags);
        }

        /// <summary>
        /// Sets navgrid tile flags.
        /// Does nothing if outside navgrid area.
        /// </summary>
        /// <param name="x">X position inside navgrid.</param>
        /// <param name="y">Y position inside navgrid.</param>
        /// <param name="flags">Flags to set.</param>
        public void SetFlags(int x, int y, TileFlags flags)
        {
            // Exit if accessing outside of array
            if (x < 0 || x >= NavGridWidth || y < 0 || y >= NavGridHeight)
                return;

            byte bits = (byte)flags;
            navGrid[x, y] |= bits;
        }

        /// <summary>
        /// Clear navgrid tile flags.
        /// Does nothing if outside navgrid area.
        /// </summary>
        /// <param name="localPosition">Local navgrid position.</param>
        /// <param name="flags">Flags to unset.</param>
        public void ClearFlags(DFPosition localPosition, TileFlags flags)
        {
            ClearFlags(localPosition.X, localPosition.Y, flags);
        }

        /// <summary>
        /// Clears navgrid tile flags.
        /// Does nothing if outside navgrid area.
        /// </summary>
        /// <param name="x">X position inside navgrid.</param>
        /// <param name="y">Y position inside navgrid.</param>
        /// <param name="flags">Flags to unset.</param>
        public void ClearFlags(int x, int y, TileFlags flags)
        {
            // Exit if accessing outside of array
            if (x < 0 || x >= NavGridWidth || y < 0 || y >= NavGridHeight)
                return;

            byte bits = (byte)flags;
            navGrid[x, y] &= (byte)~bits;
        }

        /// <summary>
        /// Checks navgrid tile flags.
        /// Returns false is outside navgrid area.
        /// </summary>
        /// <param name="localPosition">Local navgrid position.</param>
        /// <param name="flags">Flags to check.</param>
        /// <returns>True if flags set, otherwise false.</returns>
        public bool HasFlags(DFPosition localPosition, TileFlags flags)
        {
            return HasFlags(localPosition.X, localPosition.Y, flags);
        }

        /// <summary>
        /// Checks navgrid tile flags.
        /// Returns false is outside navgrid area.
        /// </summary>
        /// <param name="x">X position inside navgrid.</param>
        /// <param name="y">Y position inside navgrid.</param>
        /// <param name="flag">Flags to check.</param>
        /// <returns>True if flags set, otherwise false.</returns>
        public bool HasFlags(int x, int y, TileFlags flags)
        {
            // False if accessing outside of array
            if (x < 0 || x >= NavGridWidth || y < 0 || y >= NavGridHeight)
                return false;

            byte bits = (byte)flags;
            return ((navGrid[x, y] & bits) == bits);
        }

        /// <summary>
        /// Gets a random navgrid spawn point up to radius tiles around origin.
        /// </summary>
        /// <param name="origin">Origin point for spawn. Will be clamped inside navgrid area.</param>
        /// <param name="radius">Radius of spawn range around origin.</param>
        /// <param name="maxAttempts">Maximum attempts to find a valid random spawn point.</param>
        /// <param name="positionOut">Found position. Will be -1,-1 if no position found.</param>
        /// <returns>True if position found.</returns>
        public bool GetRandomSpawnPosition(DFPosition origin, out DFPosition positionOut, int radius=64, int maxAttempts = 10)
        {
            positionOut = new DFPosition(-1, -1);

            bool found = false;
            int totalAttempts = 0;
            DFPosition testPosition = new DFPosition();
            while (!found)
            {
                testPosition.X = origin.X + UnityEngine.Random.Range(-radius, radius + 1);
                testPosition.Y = origin.Y + UnityEngine.Random.Range(-radius, radius + 1);

                if (GetNavGridWeightLocal(testPosition) > 0)
                {
                    positionOut = testPosition;
                    return true;
                }

                if (++totalAttempts > maxAttempts)
                    break;
            }

            return false;
        }

        /// <summary>
        /// Gets raw unclamped tile value at navgrid position.
        /// Position must be inside valid range or will throw exception.
        /// </summary>
        /// <param name="localPosition">Local navgrid position.</param>
        /// <returns>Raw value from navgrid array.</returns>
        public int GetRawNavGridValueLocal(DFPosition localPosition)
        {
            return navGrid[localPosition.X, localPosition.Y];
        }

        /// <summary>
        /// Save navgrid as a raw image.
        /// </summary>
        public void SaveTestRawImage(string path)
        {
            int totalWidth = cityWidth * blockDimension;
            int totalHeight = cityHeight * blockDimension;
            byte[] buffer = new byte[totalWidth * totalHeight];

            int position = 0;
            for (int y = 0; y < totalHeight; y++)
            {
                for (int x = 0; x < totalWidth; x++)
                {
                    buffer[position++] = navGrid[x, y];
                }
            }

            File.WriteAllBytes(path, buffer);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets weight preference of tile.
        /// This influences how likely an wandering NPC will navigate onto this tile.
        /// </summary>
        byte GetTileWeight(byte tile)
        {
            switch((TileTypes)tile)
            {
                case TileTypes.Water:           // Never try to walk on full or edge water tiles
                case TileTypes.WaterDirtEdge1:
                case TileTypes.WaterDirtEdge2:
                case TileTypes.WaterGrassEdge1:
                case TileTypes.WaterGrassEdge2:
                case TileTypes.WaterStoneEdge1:
                case TileTypes.WaterStoneEdge2:
                    return 0;
                case TileTypes.Stone:           // Stone hurts our feet, but walkable
                    return 4;
                case TileTypes.Dirt:            // Dirt is OK, could be dirty or muddy though
                    return 6;
                case TileTypes.Grass:           // Grass is nice!
                    return 12;
                case TileTypes.Road:            // Roads are great!
                case TileTypes.RoadCornerDirt:
                case TileTypes.RoadCornerGrass:
                    return 15;
                default:
                    return 7;                   // Everything else is average
            }
        }

        #endregion
    }
}
