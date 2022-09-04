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
using System.IO;

namespace DaggerfallConnect.Arena2.Tests
{
    static class BaseImageFileTests
    {

        [Test]
        public static void BaseImageFile_GetBrightness_Color32___EQUALS___Color_RGBToHSV_V()
        {
            Color32 color = new Color32(3, 111, 254, 255);
            Color.RGBToHSV(color, out float h, out float s, out float expected);
            float actual = BaseImageFile.GetBrightness(color);
            float actualDelta = expected - actual;
            // Debug.Log($"expected:\t{expected:R}\nactual:\t\t{actual:R}\nactualDelta:\t{actualDelta:R}");
            Assert.AreEqual(expected: expected, actual: actual, delta: 1e-7f);
        }

        [Test]
        public static void BaseImageFile_GetBrightness_Color___EQUALS___Color_RGBToHSV_V()
        {
            Color color = new Color(0.0123f, 0.5678f, 0.9876f, 1);
            Color.RGBToHSV(color, out float h, out float s, out float expected);
            float actual = BaseImageFile.GetBrightness(color);
            float actualDelta = expected - actual;
            // Debug.Log($"expected:\t{expected:R}\nactual:\t\t{actual:R}\nactualDelta:\t{actualDelta:R}");
            Assert.AreEqual(expected: expected, actual: actual, delta: 1e-7f);
        }

        static void _GetTextureFileDataExampleForTesting(out TextureFile textureFile, out DFBitmap srcBitmap, out DaggerfallWorkshop.GetTextureSettings settings)
        {
            // note: I have no idea what image this returns.
            //       It is not an empty one so it's ok, but might not be good enough for all the test cases so this could use improvement if you know how

            int archive = 210;
            int record = 0;
            int frame = 0;
            int alphaIndex = 0;
            settings = new DaggerfallWorkshop.GetTextureSettings
            {
                archive = archive,
                record = record,
                frame = frame,
                alphaIndex = alphaIndex,
            };
            var dfUnity = DaggerfallWorkshop.DaggerfallUnity.Instance;
            var Arena2Path = dfUnity.Arena2Path;
            textureFile = new TextureFile();
            var hasReferenceTexture = textureFile.Load(Path.Combine(Arena2Path, TextureFile.IndexToFileName(settings.archive)), FileUsage.UseMemory, true);
            srcBitmap = hasReferenceTexture ? textureFile.GetDFBitmap(settings.record, settings.frame) : null;
        }

        [Test]
        public static void GetColor32___EQUALS___GetColor32Async()
        {
            _GetTextureFileDataExampleForTesting(out TextureFile textureFile, out DFBitmap srcBitmap, out var settings);
            // srcBitmap.Width /= 2;// just so unit test takes less time
            // srcBitmap.Height /= 2;// just so unit test takes less time

            Color32[] expectedData = textureFile.GetColor32(srcBitmap, settings.alphaIndex, settings.borderSize, out var _);

            textureFile.GetColor32Async(srcBitmap, settings.alphaIndex, settings.borderSize, out var _, out var actualDataNative).Complete();
            Color32[] actualData = actualDataNative.ToArray();
            actualDataNative.Dispose();

            Assert.AreEqual(expected: expectedData.Length, actual: actualData.Length);
            int length = expectedData.Length;
            for (int i = 0; i < length; i++)
            {
                var expected = expectedData[i];
                var actual = actualData[i];

                // if ((expected.r + expected.g + expected.b + expected.a) + (actual.r + actual.g + actual.b + actual.a) != 0)// lets not log boring empty pixels
                //     Debug.Log($"expected: {expected}\tactual: {actual}");

                Assert.AreEqual(expected: expected.r, actual: actual.r, delta: 1e-7f);
                Assert.AreEqual(expected: expected.g, actual: actual.g, delta: 1e-7f);
                Assert.AreEqual(expected: expected.b, actual: actual.b, delta: 1e-7f);
                Assert.AreEqual(expected: expected.a, actual: actual.a, delta: 1e-7f);
            }
        }

        [Test]
        public static void GetWindowColors32___EQUALS___GetWindowColors32Async()
        {
            _GetTextureFileDataExampleForTesting(out TextureFile textureFile, out DFBitmap srcBitmap, out var settings);
            // srcBitmap.Width /= 2;// just so unit test takes less time
            // srcBitmap.Height /= 2;// just so unit test takes less time

            Color32[] expectedData = textureFile.GetWindowColors32(srcBitmap);

            textureFile.GetWindowColors32Async(srcBitmap, out var actualDataNative).Complete();
            Color32[] actualData = actualDataNative.ToArray();
            actualDataNative.Dispose();

            Assert.AreEqual(expected: expectedData.Length, actual: actualData.Length);
            int length = expectedData.Length;
            for (int i = 0; i < length; i++)
            {
                var expected = expectedData[i];
                var actual = actualData[i];

                // if ((expected.r + expected.g + expected.b + expected.a) + (actual.r + actual.g + actual.b + actual.a) != 0)// lets not log empty pixels
                //     Debug.Log($"expected: {expected}\tactual: {actual}");

                Assert.AreEqual(expected: expected.r, actual: actual.r, delta: 1e-7f);
                Assert.AreEqual(expected: expected.g, actual: actual.g, delta: 1e-7f);
                Assert.AreEqual(expected: expected.b, actual: actual.b, delta: 1e-7f);
                Assert.AreEqual(expected: expected.a, actual: actual.a, delta: 1e-7f);
            }
        }

        [Test]
        public static void GetSpectralEmissionColors32___EQUALS___GetSpectralEmissionColors32Async()
        {
            _GetTextureFileDataExampleForTesting(out TextureFile textureFile, out DFBitmap srcBitmap, out var settings);
            // srcBitmap.Width /= 4;// just so unit test takes less time
            // srcBitmap.Height /= 4;// just so unit test takes less time

            Color32[] albedo = textureFile.GetColor32(srcBitmap, settings.frame, settings.borderSize, out var sz);
            Color eyeEmission = Color.magenta;
            Color otherEmission = Color.cyan;
            byte emissionIndex = 13;

            Color32[] expectedData = textureFile.GetSpectralEmissionColors32(srcBitmap, albedo, settings.borderSize, emissionIndex, eyeEmission, otherEmission);

            textureFile.GetSpectralEmissionColors32Async(srcBitmap, albedo, settings.borderSize, emissionIndex, eyeEmission, otherEmission, out var actualDataNative).Complete();
            Color32[] actualData = actualDataNative.ToArray();
            actualDataNative.Dispose();

            Assert.AreEqual(expected: expectedData.Length, actual: actualData.Length);
            int length = expectedData.Length;
            for (int i = 0; i < length; i++)
            {
                Color32 expected = expectedData[i];
                Color32 actual = actualData[i];

                // if ((expected.r + expected.g + expected.b + expected.a) + (actual.r + actual.g + actual.b + actual.a) != 0)// lets not log empty pixels
                //     Debug.Log($"expected: {expected}\tactual: {actual}");

                Assert.AreEqual(expected: expected.r, actual: actual.r);
                Assert.AreEqual(expected: expected.g, actual: actual.g);
                Assert.AreEqual(expected: expected.b, actual: actual.b);
                Assert.AreEqual(expected: expected.a, actual: actual.a);
            }
        }

        [Test]
        public static void GetFireWallColors32___EQUALS___GetFireWallColors32Async()
        {
            _GetTextureFileDataExampleForTesting(out TextureFile textureFile, out DFBitmap srcBitmap, out var settings);
            // srcBitmap.Width /= 4;// just so unit test takes less time
            // srcBitmap.Height /= 4;// just so unit test takes less time
            
            Color32[] albedo = textureFile.GetColor32(srcBitmap, settings.frame, settings.borderSize, out var sz);
            Color neutralColor = Color.black;
            float scale = 0.333f;

            Color32[] expectedData = textureFile.GetFireWallColors32(ref albedo, sz.Width, sz.Height, neutralColor, scale);

            textureFile.GetFireWallColors32Async(albedo, neutralColor, scale, out var actualDataNative).Complete();
            Color32[] actualData = actualDataNative.ToArray();
            actualDataNative.Dispose();

            Assert.AreEqual(expected: expectedData.Length, actual: actualData.Length);
            int length = expectedData.Length;
            for (int i = 0; i < length; i++)
            {
                Color32 expected = expectedData[i];
                Color32 actual = actualData[i];

                // if ((expected.r + expected.g + expected.b + expected.a) + (actual.r + actual.g + actual.b + actual.a) != 0)// lets not log empty pixels
                //     Debug.Log($"expected: {expected}\tactual: {actual}");

                Assert.AreEqual(expected: expected.r, actual: actual.r);
                Assert.AreEqual(expected: expected.g, actual: actual.g);
                Assert.AreEqual(expected: expected.b, actual: actual.b);
                Assert.AreEqual(expected: expected.a, actual: actual.a);
            }
        }

    }
}
