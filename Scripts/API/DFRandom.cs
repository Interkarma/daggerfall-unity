// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Daggerfall appears to use a straight clib rand() for pseudorandom numbers.
    /// Reimplementing here so certain pseudorandom processes can match Daggerfall
    /// closely as possible. This also ensures a consistent number progression
    /// in Unity across all platforms.
    /// </summary>
    public static class DFRandom
    {
        static ulong next = 1;

        public static int rand()
        {
            next = next * 1103515245 + 12345;
            return ((int)((next >> 16) & 0x7FFF));
        }

        public static int random_range(int min, int max)
        {
            return rand() % (max - min + 1) + min;
        }

        public static void srand(int seed)
        {
            next = (uint)seed;
        }

        public static void srand(uint seed)
        {
            next = seed;
        }
    }
}