// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

#region Using Statements
using System;
#endregion

namespace DaggerfallConnect.Utility
{

    /// <summary>
    /// Provides static power of two calculations.
    /// </summary>
    public class PowerOfTwo
    {

        /// <summary>
        /// Check if value is a power of 2.
        /// </summary>
        /// <param name="x">Value to check.</param>
        /// <returns>True if power of 2.</returns>
        public static bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }

        /// <summary>
        /// Finds next power of 2 size for value.
        /// </summary>
        /// <param name="x">Value.</param>
        /// <returns>Next power of 2.</returns>
        public static int NextPowerOfTwo(int x)
        {
            int i = 1;
            while (i < x) { i <<= 1; }
            return i;
        }

        /// <summary>
        /// Gets count of mipmap levels for value.
        /// </summary>
        /// <param name="x">Value.</param>
        /// <returns>Number of mipmap levels.</returns>
        public static int MipMapCount(int x)
        {
            int count = 0;
            while (x >= 1) { x >>= 1; count++; }
            return count;
        }

        /// <summary>
        /// Gets size of mipmap from value x at level.
        ///  Value x must already be a power of 2.
        ///  Never returns lower than 1.
        /// </summary>
        /// <param name="x">Value.</param>
        /// <param name="level">MipMap level.</param>
        /// <returns>Size of mipmap at level.</returns>
        public static int MipMapSize(int x, int level)
        {
            x >>= level;
            if (x <= 0) x = 1;
            return x;
        }

    }

}
