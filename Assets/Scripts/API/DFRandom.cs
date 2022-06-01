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

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Reimplementing key parts of Daggerfall's random library.
    /// This ensures critical random number sequences (e.g. building names)
    /// will match Daggerfall's output across all platforms.
    /// </summary>
    public static class DFRandom
    {
        static ulong next = 1;
        static ulong savedNext;

        public static uint Seed
        {
            get { return (uint)next; }
            set { next = value; }
        }

        /// <summary>
        /// Seed random generator.
        /// </summary>
        /// <param name="seed">Seed int.</param>
        public static void srand(int seed)
        {
            next = (uint)seed;
        }

        /// <summary>
        /// Seed random generator.
        /// </summary>
        /// <param name="seed">Seed uint.</param>
        public static void srand(uint seed)
        {
            next = seed;
        }

        /// <summary>
        /// Generate a random number.
        /// </summary>
        /// <returns></returns>
        public static uint rand()
        {
            next = next * 1103515245 + 12345;
            return ((uint)((next >> 16) & 0x7FFF));
        }

        /// <summary>
        /// Generates a random number between min and max (exclusive).
        /// </summary>
        /// <param name="min">Minimum number.</param>
        /// <param name="max">Maximum number (exclusive).</param>
        /// <returns>Random number between min and max - 1.</returns>
        public static int random_range(int min, int max)
        {
            return (int)rand() % (max - min) + min;
        }

        /// <summary>
        /// Generates a random number between min and max (inclusive).
        /// </summary>
        /// <param name="min">Minimum number.</param>
        /// <param name="max">Maximum number (inclusive).</param>
        /// <returns>Random number between min and max - 1.</returns>
        public static int random_range_inclusive(int min, int max)
        {
            return (int)rand() % (max - min + 1) + min;
        }

        /// <summary>
        /// Generates a random number between 0 and max (exclusive).
        /// </summary>
        /// <param name="max">Maximum number (exclusive).</param>
        /// <returns>Random number between 0 and max - 1.</returns>
        public static int random_range(int max)
        {
            return (int)rand() % max;
        }

        public static void SaveSeed()
        {
            savedNext = next;
        }

        public static void RestoreSeed()
        {
            next = savedNext;
        }
    }
}
