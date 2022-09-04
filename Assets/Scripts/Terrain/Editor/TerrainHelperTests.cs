// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Andrzej Łukasik (andrew.r.lukasik)
// Contributors:    
// 
// Notes:
//

using NUnit.Framework;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;

namespace DaggerfallWorkshop.Tests
{
    static class TerrainHelperTests
    {

        [Test]
        public static void new_CubicInterpolator_produces_identical_results_as_the_old_one()
        {
            float
                v0 = UnityEngine.Random.Range(0f, 1f),
                v1 = UnityEngine.Random.Range(0f, 1f),
                v2 = UnityEngine.Random.Range(0f, 1f),
                v3 = UnityEngine.Random.Range(0f, 1f),
                fracy = UnityEngine.Random.Range(0f, 1f),
                expected = TerrainHelper.CubicInterpolator_old(v0, v1, v2, v3, fracy),
                actual = TerrainHelper.CubicInterpolator(v0, v1, v2, v3, fracy),
                actualDelta = expected - actual;
            // Debug.Log($"expected:\t{expected:R}\nactual:\t\t{actual:R}\nactualDelta:\t{actualDelta:R}");
            Assert.AreEqual(expected: expected, actual: actual, delta: 1e-7f);
        }

        [Test]
        public static void new_BilinearInterpolator_produces_identical_results_as_the_old_one()
        {
            float
                valx0y0 = UnityEngine.Random.Range(0f, 1f),
                valx0y1 = UnityEngine.Random.Range(0f, 1f),
                valx1y0 = UnityEngine.Random.Range(0f, 1f),
                valx1y1 = UnityEngine.Random.Range(0f, 1f),
                u = UnityEngine.Random.Range(0f, 1f),
                v = UnityEngine.Random.Range(0f, 1f),
                expected = TerrainHelper.BilinearInterpolator_old(valx0y0, valx0y1, valx1y0, valx1y1, u, v),
                actual = TerrainHelper.BilinearInterpolator(valx0y0, valx0y1, valx1y0, valx1y1, u, v),
                actualDelta = expected - actual;
            // Debug.Log($"expected:\t{expected:R}\nactual:\t\t{actual:R}\nactualDelta:\t{actualDelta:R}");
            Assert.AreEqual(expected: expected, actual: actual, delta: 1e-7f);
        }

        [Test]
        public static void new_GetGradient_produces_identical_results_as_the_old_one()
        {
            float
                x0y0 = UnityEngine.Random.Range(0f, 1f),
                x1y0 = UnityEngine.Random.Range(0f, 1f),
                x0y1 = UnityEngine.Random.Range(0f, 1f),
                expected = TerrainHelper.GetGradient_old(x0y0, x1y0, x0y1),
                actual = TerrainHelper.GetGradient(x0y0, x1y0, x0y1),
                actualDelta = expected - actual;
            // Debug.Log($"expected:\t{expected:R}\nactual:\t\t{actual:R}\nactualDelta:\t{actualDelta:R}");
            Assert.AreEqual(expected: expected, actual: actual, delta: 1e-7f);
        }

        [Test]
        public static void new_AverageHeights_produces_identical_results_as_the_old_one()
        {
            int
                mapWidth = 13,
                mapHeight = mapWidth,
                length = mapWidth * mapWidth;

            // create input data
            var inputData = new byte[length];
            for (int i = 0; i < length; i++)
                inputData[i] = (byte)UnityEngine.Random.Range(0, 255);

            var heightArray = (byte[])inputData.Clone();
            var heightArrayNative = new NativeArray<byte>(inputData, Allocator.Temp);

            // Search for locations
            for (int y = 1; y < mapHeight - 1; y++)
            for (int x = 1; x < mapWidth - 1; x++)
            {
                // Use Sobel filter for gradient
                float x0y0 = heightArray[y * mapWidth + x];
                float x1y0 = heightArray[y * mapWidth + (x + 1)];
                float x0y1 = heightArray[(y + 1) * mapWidth + x];
                TerrainHelper.AverageHeights(heightArray, x, y, mapWidth);
                TerrainHelper.AverageHeights(heightArrayNative, x, y, mapWidth);
            }

            // test
            for (int i = 0; i < length; i++)
            {
                byte
                    original = inputData[i],
                    expected = heightArray[i],
                    actual = heightArrayNative[i];
                int
                    actualDelta = actual - expected;

                // Debug.Log($"original:{original}\t expected:{expected:000}\t actual:{actual:000}\t delta:{actualDelta}");
                Assert.AreEqual(expected: expected, actual: actual, delta: 0);
            }
        }

    }
}
