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
using System.IO;
using DaggerfallConnect;

namespace DaggerfallWorkshop.Game.Utility
{
    /// <summary>
    /// Utility class to help with mobile spawning and navigation in town environments.
    /// The navigation component is intended to be used by wandering NPCs.
    /// Mobile enemies will use normal steering behaviour to follow player.
    /// 
    /// Combines inverse of automap to carve out navgrid then sets weighting by tile type.
    /// For a max-size city of 8x8 blocks, result is a 512x512 tile grid of bytes (64x64 per RMB block).
    /// Each byte has the following flags in bits 1-4:
    ///  * 1 = Can move UP to tile y-1
    ///  * 2 = Can move DOWN to tile y+1
    ///  * 4 = Can move LEFT to tile x-1
    ///  * 8 = Can move RIGHT to tile x+1
    ///  * If all bits are clear then tile will always have a weighting of 0.
    /// Bits 5-8 are a weighting value from 0-15. The higher the value, the higher the weighting.
    ///  * 0 = Never use this tile
    ///  * ...
    ///  * 15 = Prefer this tile more than lower value tiles.
    /// With the above information, it is possible to quickly select a spawn location
    /// and for entities to navigate around towns using a variety of steering behaviours.
    /// Will keep simple for now for implementation speed and inrementally improve in future.
    /// </summary>
    public class CityNavigation : MonoBehaviour
    {
        #region Fields

        const int blockDimension = 64;
        const int blockSize = blockDimension * blockDimension;

        string regionName;
        string locationName;
        int cityWidth = 0;
        int cityHeight = 0;
        byte[,] navGrid = null;

        #endregion

        #region Properties

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

        #endregion

        #region Structs & Enums

        /// <summary>
        /// Navigation flags describing paths out of this tile.
        /// </summary>
        [Flags]
        public enum NavFlags
        {
            None = 0,
            Up = 1,
            Down = 2,
            Left = 4,
            Right = 8,
        }

        /// <summary>
        /// Tile types examined for weightings.
        /// </summary>
        public enum TileTypes
        {
            Water = 0,
            Dirt = 1,
            Grass = 2,
            Stone = 3,
            Road = 46,
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
            this.cityWidth = cityWidth;
            this.cityHeight = cityHeight;
        }

        /// <summary>
        /// Set block data in navgrid.
        /// This is done during StreamingWorld location layout when this data is available.
        /// </summary>
        /// <param name="blockData">RMB block data.</param>
        /// <param name="xBlock">X block to set.</param>
        /// <param name="yBlock">Y block to set.</param>
        public void SetRMBData(ref DFBlock blockData, int xBlock, int yBlock)
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

            // TODO: Generate up/down/left/right navflags from each grid position
            // This is not necessary for spawning mobiles outdoors but will be helpful for wandering NPCs
            // Right now I just need valid position for city mobile spawns
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
                case TileTypes.Water:       // Never try to swim
                    return 0;
                case TileTypes.Stone:       // Stone hurts out feet, but walkable
                    return 4;
                case TileTypes.Dirt:        // Dirt is OK, could be dirty or muddy though
                    return 6;
                case TileTypes.Grass:       // Grass is nice!
                    return 12;
                case TileTypes.Road:        // Roads are great!
                    return 15;
                default:
                    return 7;               // Everything else is average
            }
        }

        #endregion
    }
}
