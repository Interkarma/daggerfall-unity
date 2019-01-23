// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;

namespace DaggerfallWorkshop
{
    public static class JobA
    {
        public static int Idx(int r, int c, int dim)
        {
            return r + (c * dim);
        }

        public static int Row(int index, int dim)
        {
            return index % dim;
        }

        public static int Col(int index, int dim)
        {
            return index / dim;
        }
    }

    public class JobRand
    {
        private static readonly Random Global = new Random();
        [ThreadStatic]
        private static Random threadRandom;

        public static int Next()
        {
            Random localRand = GetLocalRandom();
            return localRand.Next();
        }

        public static int Next(int max)
        {
            Random localRand = GetLocalRandom();
            return localRand.Next(max);
        }

        public static int Next(int min, int max)
        {
            Random localRand = GetLocalRandom();
            return localRand.Next(min, max);
        }

        private static Random GetLocalRandom()
        {
            var localRand = threadRandom;
            if (localRand == null)
            {
                int seed;
                lock (Global) seed = Global.Next();
                localRand = new Random(seed);
                threadRandom = localRand;
            }
            return localRand;
        }
    }

}