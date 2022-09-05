// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    Andrzej Łukasik (andrew.r.lukasik)
// 
// Notes:
//

using System;
using Unity.Collections;
using Unity.Jobs;

namespace DaggerfallWorkshop
{
    // Static helper class for IJobParallelFor jobs convering index to/from rows & columns.
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

    // Static helper class for jobs providing basic thread safe access to system random.
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

public static class JobUtility
{
    /// <summary>
    /// Schedules a job that calls UnsafeUtility.ReleaseGCObject(gcHandle).
    /// </summary>
    public static JobHandle ReleaseGCObject(ulong gcHandle, JobHandle dependency = default)
        => new ReleaseGCObjectJob(gcHandle).Schedule(dependency);

    /// <summary>
    /// This equation is yet to be (im)proved experimentally, the current version is merely an initial guesswork
    /// </summary>
    public static int OptimalLoopBatchCount(int length)
        => Unity.Mathematics.math.max(length / (UnityEngine.SystemInfo.processorCount * 3), 1);
}
