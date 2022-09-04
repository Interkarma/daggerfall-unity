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
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using DaggerfallWorkshop.Utility;

static class ImageProcessingTests
{

    [Test]
    public static void SharpenAsync___EQUALS___Sharpen()
    {
        const int
            width = 64,
            height = 64,
            length = width * height;
        Color32[] input = new Color32[length];
        for (int i = 0; i < length; i++)
            input[i] = UnityEngine.Random.ColorHSV();

        var ___sw = System.Diagnostics.Stopwatch.StartNew();
        Color32[] expectedData = ImageProcessing.Sharpen(input, width, height);
        var expectedTime = ___sw.Elapsed;
        Color32[] actualData = (Color32[])input.Clone();
        {
            var buff = actualData.AsNativeArray(out ulong gcHandle);
            ___sw.Restart();
            ImageProcessing.SharpenAsync(buff, width, height).Complete();
            var actualTime = ___sw.Elapsed;
            actualData = buff.ToArray();
            JobUtility.ReleaseGCObject(gcHandle);

            Debug.Log($"Sharpen took {expectedTime.Milliseconds}[ms] ({expectedTime.Ticks}[ticks]) \t SharpenAsync took {actualTime.Milliseconds}[ms] ({actualTime.Ticks}[ticks])");
        }
        
        for (int i = 0; i < length; i++)
        {
            var expected = expectedData[i];
            var actual = actualData[i];
            // Debug.Log($"input:{input[i]}\texpected:{expected}\tactual:{actual}");
            Assert.AreEqual(expected: expected, actual: actual);
        }
    }

    [Test]
    public static void WrapBorderAsync___EQUALS___WrapBorder()
    {
        const int
            width = 64,
            height = 64,
            length = width * height,
            border = 3;
        Color32[] input = new Color32[length];
        for (int i = 0; i < length; i++)
            input[i] = UnityEngine.Random.ColorHSV();

        var ___sw = System.Diagnostics.Stopwatch.StartNew();
        Color32[] expectedData = (Color32[])input.Clone();
        ImageProcessing.WrapBorder(expectedData, new DaggerfallConnect.Utility.DFSize(width, height), border, true, true);
        var expectedTime = ___sw.Elapsed;
        Color32[] actualData = (Color32[])input.Clone();
        {
            var buff = actualData.AsNativeArray(out ulong gcHandle);
            ___sw.Restart();
            ImageProcessing.WrapBorderAsync(buff, width, height, border, true, true).Complete();
            var actualTime = ___sw.Elapsed;
            actualData = buff.ToArray();
            JobUtility.ReleaseGCObject(gcHandle);

            Debug.Log($"WrapBorder took {expectedTime.Milliseconds}[ms] ({expectedTime.Ticks}[ticks]) \t WrapBorderAsync took {actualTime.Milliseconds}[ms] ({actualTime.Ticks}[ticks])");
        }
        
        for (int i = 0; i < length; i++)
        {
            var expected = expectedData[i];
            var actual = actualData[i];
            // Debug.Log($"input:{input[i]}\texpected:{expected}\tactual:{actual}");
            Assert.AreEqual(expected: expected, actual: actual);
        }
    }

}
