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

#region Using Statements
using UnityEngine;
using Unity.Profiling;
#endregion

namespace DaggerfallConnect
{
    /// <summary>
    /// Stores raw bitmap data in indexed format.
    /// </summary>
    public class DFBitmap
    {
        #region Fields

        /// <summary>Width of the image in pixels.</summary>
        public int Width;

        /// <summary>Height of the image in pixels.</summary>
        public int Height;

        /// <summary>Image byte array in specified format.</summary>
        public byte[] Data;

        /// <summary>Daggerfall palette.</summary>
        public DFPalette Palette = new DFPalette();

        #endregion

        #region Profiler Markers

        static readonly ProfilerMarker
            ___GetColor32 = new ProfilerMarker($"{nameof(DFBitmap)}.{nameof(GetColor32)}");
        
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// Creates an empty DFBitmap with null data.
        /// </summary>
        public DFBitmap()
        {
        }

        /// <summary>
        /// Constructor.
        /// Creates a new DFBitmap with sized and zeroed data array.
        /// </summary>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        public DFBitmap(int width, int height)
        {
            Initialise(width, height);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialise DFBitmap data to specified size.
        /// Any existing data will be discarded.
        /// </summary>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        public void Initialise(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.Data = new byte[this.Width * this.Height];
        }

        /// <summary>
        /// Gets a Color32 array for engine.
        /// </summary>
        /// <param name="alphaIndex">Index to receive transparent alpha.</param>
        /// <returns>Color32 array.</returns>
        public Color32[] GetColor32(int alphaIndex = -1)
        {
            return GetColor32(alphaIndex, -1, Color.clear);
        }

        /// <summary>
        /// Gets a Color32 array for engine.
        /// </summary>
        /// <param name="alphaIndex">Index to receive transparent alpha.</param>
        /// <param name="maskIndex">Index to receive mask colour.</param>
        /// <param name="maskColor">New mask color.</param>
        /// <param name="excludeNonMasked">Exclude non-masked areas of image.</param>
        /// <returns>Color32 array.</returns>
        public Color32[] GetColor32(int alphaIndex, int maskIndex, Color maskColor, bool excludeNonMasked = false)
        {
            ___GetColor32.Begin();

            Color32[] colors = new Color32[Width * Height];
            byte[] paletteBuffer = Palette.PaletteBuffer;
            int paletteHeaderLength = Palette.HeaderLength;

            for (int y = 0; y < Height; y++)
            {
                // Get row position
                int srcRow = y * Width;
                int dstRow = (Height - 1 - y) * Width;

                // Write data for this row
                for (int x = 0; x < Width; x++)
                {
                    int index = Data[srcRow + x];
                    if (index == maskIndex)
                    {
                        colors[dstRow + x] = maskColor;
                    }
                    else if (!excludeNonMasked)
                    {
                        int offset = paletteHeaderLength + index * 3;
                        Color32 c = new Color32(
                            paletteBuffer[offset],
                            paletteBuffer[offset + 1],
                            paletteBuffer[offset + 2],
                            255
                        );
                        if(alphaIndex == index) c.a = 0;
                        colors[dstRow + x] = c;
                    }
                    else
                    {
                        colors[dstRow + x] = Color.clear;
                    }
                }
            }

            ___GetColor32.End();
            return colors;
        }

        #endregion

        #region Static Public Methods

        /// <summary>
        /// Creates a clone of a DFBitmap.
        /// </summary>
        /// <param name="bitmap">DFBitmap source.</param>
        /// <param name="cloneData">True to clone data.</param>
        /// <returns>Cloned DFBitmap.</returns>
        static public DFBitmap CloneDFBitmap(DFBitmap bitmap, bool cloneData = true)
        {
            // Handle null bitmap or data input
            if (bitmap == null || bitmap.Data == null)
                return new DFBitmap();

            // Create destination bitmap to receive normal image
            DFBitmap newBitmap = new DFBitmap();
            newBitmap.Width = bitmap.Width;
            newBitmap.Height = bitmap.Height;
            newBitmap.Data = new byte[bitmap.Data.Length];
            newBitmap.Palette = bitmap.Palette;

            // Clone data
            if (cloneData)
                bitmap.Data.CopyTo(newBitmap.Data, 0);

            return newBitmap;
        }

        #endregion
    }
}
