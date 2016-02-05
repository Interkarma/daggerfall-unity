// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

#region Using Statements
using System;
using System.Text;
#endregion

namespace DaggerfallConnect
{
    /// <summary>
    /// Stores raw bitmap data in indexed or colour byte formats.
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

        #endregion

        #region Static Public Methods

        /// <summary>
        /// Creates a clone of a DFBitmap.
        /// </summary>
        /// <param name="bitmap">DFBitmap source.</param>
        /// <returns>Cloned DFBitmap.</returns>
        static public DFBitmap CloneDFBitmap(DFBitmap bitmap)
        {
            // Create destination bitmap to receive normal image
            DFBitmap newBitmap = new DFBitmap();
            newBitmap.Width = bitmap.Width;
            newBitmap.Height = bitmap.Height;
            newBitmap.Data = new byte[bitmap.Data.Length];
            bitmap.Data.CopyTo(newBitmap.Data, 0);

            return newBitmap;
        }

        #endregion
    }
}
