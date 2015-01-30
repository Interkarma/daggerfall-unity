// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

#region Using Statements
using System;
using System.Text;
#endregion

namespace DaggerfallConnect
{
    /// <summary>
    /// Stores raw bitmap data. The binary format of the image data will depend on the method which returned the DFBitmap object.
    /// </summary>
    public struct DFBitmap
    {
        #region Structure Variables

        /// <summary>Format of the image.</summary>
        public Formats Format;

        /// <summary>Width of the image in pixels.</summary>
        public int Width;

        /// <summary>Height of the image in pixels.</summary>
        public int Height;

        /// <summary>Stride (bytes per horizontal row) of the image.</summary>
        public int Stride;

        /// <summary>Image byte array in specified format.</summary>
        public byte[] Data;

        #endregion

        #region Child Structures

        /// <summary>
        /// Bitmap formats enumeration.
        /// </summary>
        public enum Formats
        {
            /// <summary>Indexed image. 1 byte per pixel. Each byte is an index into the palette colour data.</summary>
            Indexed,
            /// <summary>Colour channels in order alpha, red, green, blue. 4 bytes per pixel.</summary>
            ARGB,
            /// <summary>Colour channels in order red, green, blue, alpha. 4 bytes per pixel.</summary>
            RGBA,
            /// <summary>Colour channels in order alpha, blue, green, red. 4 bytes per pixel.</summary>
            ABGR,
            /// <summary>Colour channels in order blue, green, red, alpha. 4 bytes per pixel.</summary>
            BGRA,

        }

        #endregion
    }
}
