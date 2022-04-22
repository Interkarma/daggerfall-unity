using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using Unity.Jobs;

namespace DaggerfallWorkshop.Tests
{
    /// These are not unit test, in a strict sense, but comparizons to better undestand codebase behaviour
    static partial class Comparizons
    {

        /// lets compare integers produced by different rng
        [Test]
        public static void AllAvailableRandomNumberGenerators_NextInteger()
        {
            const uint seed = 1;
            const uint numRepeats = 10;
            Debug.Log($"seed: {seed}");

            Debug.Log("\nSystem.Random:");
            {
                var rnd = new System.Random((int)seed);
                Debug.Log("\t Next():");
                Debug.Log($"\t {_BuildString(() => $"\t {rnd.Next()}", numRepeats)}");
            }

            Debug.Log("\nUnityEngine.Random:");
            {
                var UnityEngine_Random_state = UnityEngine.Random.state;

                UnityEngine.Random.InitState((int)seed);
                Debug.Log("\t Range(int.MinValue,int.MaxValue):");
                Debug.Log($"\t {_BuildString(() => $"\t {UnityEngine.Random.Range(int.MinValue, int.MaxValue)}", numRepeats)}");

                UnityEngine.Random.InitState((int)seed);
                Debug.Log("\t Range(0,int.MaxValue):");
                Debug.Log($"\t {_BuildString(() => $"\t {UnityEngine.Random.Range(0, int.MaxValue)}", numRepeats)}");

                UnityEngine.Random.state = UnityEngine_Random_state;
            }

            Debug.Log("\nUnity.Mathematics.Random:");
            {
                var rnd = new Unity.Mathematics.Random(seed);
                Debug.Log("\t NextInt():");
                Debug.Log($"\t {_BuildString(() => $"\t {rnd.NextInt()}", numRepeats)}");

                rnd = new Unity.Mathematics.Random(seed);
                Debug.Log("\t NextInt(int.MinValue,int.MaxValue):");
                Debug.Log($"\t {_BuildString(() => $"\t {rnd.NextInt(int.MinValue, int.MaxValue)}", numRepeats)}");

                rnd = new Unity.Mathematics.Random(seed);
                Debug.Log("\t NextInt(0,int.MaxValue):");
                Debug.Log($"\t {_BuildString(() => $"\t {rnd.NextInt(0, int.MaxValue)}", numRepeats)}");
            }
        }

        // let's see how they compare, performance-wise
        [Test]
        public static void AllAvailableRandomNumberGenerators_Performance()
        {
            const uint seed = 1;
            const uint numRepeats = (uint)1e6;
            int innerloopBatchCount = Mathf.Max((int)(numRepeats / (SystemInfo.processorCount * 100)),1024);
            Debug.Log($"seed: {seed}");
            Debug.Log($"numRepeats: {numRepeats}");

            Debug.Log("\nSystem.Random:");
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                {
                    var rnd = new System.Random((int)seed);
                    for (uint i = 0; i < numRepeats; i++)
                        rnd.Next();
                }
                stopwatch.Stop();
                Debug.Log($"\t Next():\n\t\t{stopwatch.ElapsedMilliseconds} [ms]\t{stopwatch.ElapsedTicks} [ticks]");
            }

            Debug.Log("\nUnityEngine.Random:");
            var UnityEngine_Random_state = UnityEngine.Random.state;
            {
                UnityEngine.Random.InitState((int)seed);
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                {
                    var rnd = new System.Random((int)seed);
                    for (uint i = 0; i < numRepeats; i++)
                        UnityEngine.Random.Range(int.MinValue, int.MaxValue);
                }
                stopwatch.Stop();
                Debug.Log($"\t Range(int.MinValue, int.MaxValue):\n\t\t{stopwatch.ElapsedMilliseconds} [ms]\t{stopwatch.ElapsedTicks} [ticks]");
            }
            {
                UnityEngine.Random.InitState((int)seed);
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                {
                    var rnd = new System.Random((int)seed);
                    for (uint i = 0; i < numRepeats; i++)
                        UnityEngine.Random.Range(0, int.MaxValue);
                }
                stopwatch.Stop();
                Debug.Log($"\t Range(0, int.MaxValue):\n\t\t{stopwatch.ElapsedMilliseconds} [ms]\t{stopwatch.ElapsedTicks} [ticks]");
            }
            UnityEngine.Random.state = UnityEngine_Random_state;

            Debug.Log("\nUnity.Mathematics.Random:");
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                {
                    var rnd = new Unity.Mathematics.Random(seed);
                    for (uint i = 0; i < numRepeats; i++)
                        rnd.NextInt();
                }
                stopwatch.Stop();
                Debug.Log($"\t NextInt():\n\t\t{stopwatch.ElapsedMilliseconds} [ms]\t{stopwatch.ElapsedTicks} [ticks]");
            }
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                {
                    var rnd = new Unity.Mathematics.Random(seed);
                    for (uint i = 0; i < numRepeats; i++)
                        rnd.NextInt(int.MinValue, int.MaxValue);
                }
                stopwatch.Stop();
                Debug.Log($"\t NextInt(int.MinValue, int.MaxValue):\n\t\t{stopwatch.ElapsedMilliseconds} [ms]\t{stopwatch.ElapsedTicks} [ticks]");
            }
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                {
                    var rnd = new Unity.Mathematics.Random(seed);
                    for (uint i = 0; i < numRepeats; i++)
                        rnd.NextInt(0, int.MaxValue);
                }
                stopwatch.Stop();
                Debug.Log($"\t NextInt(0, int.MaxValue):\n\t\t{stopwatch.ElapsedMilliseconds} [ms]\t{stopwatch.ElapsedTicks} [ticks]");
            }
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                {
                    var job = new _Random_IJob_Burst
                    {
                        Rnd = new Unity.Mathematics.Random(seed),
                        NumRepeats = numRepeats,
                        Min = int.MinValue,
                        Max = int.MaxValue,
                    };
                    job.Schedule().Complete();
                }
                stopwatch.Stop();
                Debug.Log($"\t IJob+Burst NextInt(int.MinValue, int.MaxValue):\n\t\t{stopwatch.ElapsedMilliseconds} [ms]\t{stopwatch.ElapsedTicks} [ticks]");
            }
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                {
                    var job = new _Random_IJob_Burst
                    {
                        Rnd = new Unity.Mathematics.Random(seed),
                        NumRepeats = numRepeats,
                        Min = 0,
                        Max = int.MaxValue,
                    };
                    job.Schedule().Complete();
                }
                stopwatch.Stop();
                Debug.Log($"\t IJob+Burst NextInt(0, int.MaxValue):\n\t\t{stopwatch.ElapsedMilliseconds} [ms]\t{stopwatch.ElapsedTicks} [ticks]");
            }
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                {
                    var job = new _Random_IJobParallelFor_Burst
                    {
                        Rnd = new Unity.Mathematics.Random(seed),
                        Min = int.MinValue,
                        Max = int.MaxValue,
                    };
                    job.Schedule((int)numRepeats, innerloopBatchCount).Complete();
                }
                stopwatch.Stop();
                Debug.Log($"\t IJobParallelFor+Burst NextInt(int.MinValue, int.MaxValue):\n\t\t{stopwatch.ElapsedMilliseconds} [ms]\t{stopwatch.ElapsedTicks} [ticks]");
            }
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                {
                    var job = new _Random_IJobParallelFor_Burst
                    {
                        Rnd = new Unity.Mathematics.Random(seed),
                        Min = 0,
                        Max = int.MaxValue,
                    };
                    job.Schedule((int)numRepeats, innerloopBatchCount).Complete();
                }
                stopwatch.Stop();
                Debug.Log($"\t IJobParallelFor+Burst NextInt(0, int.MaxValue):\n\t\t{stopwatch.ElapsedMilliseconds} [ms]\t{stopwatch.ElapsedTicks} [ticks]");
            }

            Debug.Log("\nnote: Running this test repeatedly makes cpu better at executing it (helps cpu predict code execution pathways). Running it 3>= times should stabilize the results around such optimistic case.");
        }

        static string _BuildString(System.Func<string> func, uint numRepeats)
        {
            var sb = new System.Text.StringBuilder();
            for (uint i = 0; i < numRepeats; i++) sb.Append(func());
            return sb.ToString();
        }

        [Unity.Burst.BurstCompile(CompileSynchronously = true)]
        struct _Random_IJob_Burst : IJob
        {
            public Unity.Mathematics.Random Rnd;
            public uint NumRepeats;
            public int Min, Max;
            void IJob.Execute()
            {
                for (int i = 0; i < NumRepeats; i++)
                    Rnd.NextInt(Min, Max);
            }
        }

        [Unity.Burst.BurstCompile(CompileSynchronously = true)]
        struct _Random_IJobParallelFor_Burst : IJobParallelFor
        {
            public Unity.Mathematics.Random Rnd;
            public int Min, Max;
            void IJobParallelFor.Execute(int index)
            {
                uint localSeed = (uint)Unity.Mathematics.math.max((Rnd.state + index + 1) % uint.MaxValue, 1);
                var localRnd = new Unity.Mathematics.Random(localSeed);
                localRnd.NextInt(Min, Max);
            }
        }

    }
}
